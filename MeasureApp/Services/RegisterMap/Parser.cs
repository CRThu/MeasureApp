using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasureApp.Services.RegisterMap
{
    public static class Parser
    {
        /// <summary>
        /// 从指定的Excel文件路径解析寄存器映射。
        /// </summary>
        /// <param name="filePaths">一个或多个Excel文件的路径。</param>
        /// <returns>解析出的RegFile集合。</returns>
        public static List<RegFile> ParseFromExcel(IEnumerable<string> filePaths)
        {
            var regFiles = new List<RegFile>();

            // 确保在读取Excel文件前注册编码提供程序
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            uint regFileIndex = 0;
            foreach (var filePath in filePaths)
            {
                using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                using var reader = ExcelReaderFactory.CreateReader(stream);
                var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = false
                    }
                });

                foreach (DataTable sheet in result.Tables)
                {
                    try
                    {
                        var regFile = new RegFile(regFileIndex++, sheet.TableName);
                        ParseSheet(sheet, regFile);
                        if (regFile.Registers.Any())
                        {
                            regFiles.Add(regFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        // 重新抛出带有文件名的异常，以便顶层捕获
                        throw new Exception($"解析文件 '{Path.GetFileName(filePath)}' 失败: {ex.Message}", ex);
                    }
                }
            }
            return regFiles;
        }

        private static void ParseSheet(DataTable sheet, RegFile regFile)
        {
            Register currentRegister = null;

            for (int i = 0; i < sheet.Rows.Count; i++)
            {
                DataRow row = sheet.Rows[i];
                string firstCol = GetCellString(row, 0);

                if (string.IsNullOrWhiteSpace(firstCol))
                    continue;

                try
                {
                    // 定位行 *register*, <reg>, <regname>, <bitwidth> 
                    // 可选行 *address*, <addr>
                    // 忽略行 bit, name, rw, default, desc
                    if (firstCol.Contains("register", StringComparison.OrdinalIgnoreCase))
                    {
                        currentRegister = ParseRegisterHeader(sheet, ref i, regFile);
                    }
                    // 位段行 [<endbit>:<startbit>], <name>, _, _, <desc>
                    else if (firstCol.StartsWith("["))
                    {
                        if (currentRegister == null)
                        {
                            // 在定义任何寄存器之前就出现了位域定义
                            throw CreateParsingException(sheet.TableName, i + 1, 1, "在 'register name' 出现之前找到了位域定义。");
                        }
                        ParseBitFieldRow(row, currentRegister);
                    }
                }
                catch (InvalidDataException ex)
                {
                    // 捕获并重新抛出，添加文件和Sheet信息
                    throw new InvalidDataException($"错误发生在 Sheet '{sheet.TableName}', 第 {i + 1} 行: {ex.Message}", ex);
                }
            }
        }

        private static Register ParseRegisterHeader(DataTable sheet, ref int rowIndex, RegFile regFile)
        {
            DataRow row = sheet.Rows[rowIndex];

            // reg.name
            string regName = GetCellString(row, 1);
            if (string.IsNullOrWhiteSpace(regName))
                throw CreateParsingException(sheet.TableName, rowIndex + 1, 2, "寄存器名称不能为空。");

            // reg.bitwidth
            string bitWidthStr = GetCellString(row, 3);
            if (string.IsNullOrWhiteSpace(bitWidthStr) || !uint.TryParse(bitWidthStr, out uint bitWidth))
                //throw CreateParsingException(sheet.TableName, rowIndex + 1, 4, $"无效的位宽值: '{bitWidthStr}'。");
                bitWidth = 0;

            rowIndex++;
            if (rowIndex >= sheet.Rows.Count)
                throw CreateParsingException(sheet.TableName, rowIndex + 1, 1, "期望找到寄存器地址行，但已到达文件末尾。");

            // reg.address
            DataRow addrRow = sheet.Rows[rowIndex];
            string addrString = GetCellString(addrRow, 1).Replace("0x", "");
            if (string.IsNullOrWhiteSpace(addrString) || !uint.TryParse(addrString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint address))
                throw CreateParsingException(sheet.TableName, rowIndex + 1, 2, $"无效的寄存器地址: '{GetCellString(addrRow, 1)}'。");

            // 跳过位域标题行 *bit*
            if (rowIndex + 1 < sheet.Rows.Count)
            {
                string firstCol = GetCellString(sheet.Rows[rowIndex + 1], 0);
                if (firstCol.Contains("bit", StringComparison.OrdinalIgnoreCase))
                    rowIndex++;
            }

            return regFile.AddReg(regName, address, bitWidth);
        }

        private static void ParseBitFieldRow(DataRow row, Register currentRegister)
        {
            // [bf.endbit:bf.startbit]
            string rangeString = GetCellString(row, 0);
            if (!ParseBitRange(rangeString, out uint startBit, out uint endBit))
                throw CreateParsingException(currentRegister.Parent.Name, row.Table.Rows.IndexOf(row) + 1, 1, $"无效的位域范围格式: '{rangeString}'。");

            // bf.name
            string bfName = GetCellString(row, 1);
            if (string.IsNullOrWhiteSpace(bfName))
                throw CreateParsingException(currentRegister.Parent.Name, row.Table.Rows.IndexOf(row) + 1, 2, "位域名不能为空。");

            // bf.desc
            string bfDesc = GetCellString(row, 4).Trim('"');

            currentRegister.AddBits(bfName, startBit, endBit, bfDesc);
        }

        private static bool ParseBitRange(string rangeString, out uint startBit, out uint endBit)
        {
            startBit = 0;
            endBit = 0;
            if (string.IsNullOrWhiteSpace(rangeString))
                return false;

            string cleaned = rangeString.Trim().Trim('[', ']');
            var parts = cleaned.Split(':');

            if (parts.Length == 2
                && uint.TryParse(parts[0], out endBit)
                && uint.TryParse(parts[1], out startBit))
                return true;

            if (parts.Length == 1
                && uint.TryParse(parts[0], out uint bit))
            {
                startBit = bit;
                endBit = bit;
                return true;
            }
            return false;
        }

        private static string GetCellString(DataRow row, int columnIndex)
        {
            if (columnIndex >= row.Table.Columns.Count || row.IsNull(columnIndex))
                return string.Empty;
            return row[columnIndex].ToString().Trim();
        }

        // 创建带有详细位置信息的异常
        private static InvalidDataException CreateParsingException(string sheetName, int row, int col, string message)
        {
            return new InvalidDataException($"在 Sheet '{sheetName}' 的单元格 {GetExcelColumnName(col)}{row} 附近发生错误: {message}");
        }

        // 辅助函数，将列索引转换为Excel列名（1->A, 2->B）
        private static string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }
            return columnName;
        }


        public static List<RegFile> ParseFromTxt(IEnumerable<string> filePaths)
        {
            var regFiles = new List<RegFile>();

            foreach (var filePath in filePaths)
            {
                string[][] regInfos = File.ReadAllLines(filePath).Select(l => l.Split(',')).ToArray();

                RegFile regfile = null;
                foreach (var regInfo in regInfos)
                {
                    if (regInfo[0] == "+RF")
                    {
                        regfile = new RegFile((uint)regFiles.Count, regInfo[1]);
                        regFiles.Add(regfile);
                    }
                    else if (regInfo[0] == "+REG")
                    {
                        if (regfile == null)
                            throw new Exception("未初始化REGFILE");
                        uint addr = Convert.ToUInt32(regInfo[1], 16);
                        uint bitWidth = Convert.ToUInt32(regInfo[2], 16);
                        regfile.AddReg(regInfo[3], addr, bitWidth);
                    }
                    else
                    {
                        uint addr = Convert.ToUInt32(regInfo[0], 16);
                        uint endBit = Convert.ToUInt32(regInfo[1], 16);
                        uint startBit = Convert.ToUInt32(regInfo[2], 16);
                        string name = regInfo[3];
                        string desc = regInfo.Length >= 5 ? regInfo[4] : "<DESC>";
                        var reg = regfile?.Registers.Where(r => r.Address == addr).FirstOrDefault();
                        if (reg == null)
                            throw new Exception("未初始化REG");
                        reg.AddBits(name, startBit, endBit, desc);
                    }
                }
            }
            return regFiles;
        }
    }
}
