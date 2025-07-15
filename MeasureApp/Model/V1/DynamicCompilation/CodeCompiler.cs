using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Windows;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.IO;
using System.Diagnostics;
using Microsoft.CodeAnalysis.Emit;

namespace MeasureApp.Model.DynamicCompilation
{
    // Implement Reference: https://bit.ly/3HSS9gP
    public static class CodeCompiler
    {
        // 测试函数
        public static string SayHello()
        {
            return "Hello";
        }

        public static SyntaxTree Parse(string code)
        {
            return CSharpSyntaxTree.ParseText(code);
        }

        public static CSharpCompilation Compile(SyntaxTree syntaxTree, string assemblyName = null)
        {
            assemblyName = assemblyName ?? Path.GetRandomFileName();

            // 元数据引用
            // var Assemblies = AppDomain.CurrentDomain.GetAssemblies().Select(x => MetadataReference.CreateFromFile(x.Location));
            var Assemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies().Select(Assembly.Load)
            .Select(x => MetadataReference.CreateFromFile(x.Location)).ToList();
            Assemblies.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            Assemblies.Add(MetadataReference.CreateFromFile(typeof(ViewModel.MainWindowDataContext).Assembly.Location));

            // 编译对象
            CSharpCompilation compilation = CSharpCompilation.Create(assemblyName,
                syntaxTrees: new[] { syntaxTree },
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                references: Assemblies);

            return compilation;
        }

        public static Assembly Emit2Mem(CSharpCompilation compilation)
        {
            // IL代码存入内存
            var ms = new MemoryStream();
            var result = compilation.Emit(ms);

            if (result.Success)
            {
                // 加载程序集
                ms.Seek(0, SeekOrigin.Begin);
                return Assembly.Load(ms.ToArray());
            }
            else
            {
                foreach (var d in result.Diagnostics)
                {
                    Debug.WriteLine("{0}: {1}", d.Id, d.GetMessage());
                }
                throw new Exception(string.Join("\n", result.Diagnostics));
            }
        }

        public static Assembly Emit2Dll(CSharpCompilation compilation, string dllPath, string pdbPath = null)
        {
            // Emit
            var result = compilation.Emit(dllPath, pdbPath);

            if (result.Success)
            {
                // 加载程序集
                return Load(dllPath, pdbPath);
            }
            else
            {
                foreach (var d in result.Diagnostics)
                {
                    Debug.WriteLine("{0}: {1}", d.Id, d.GetMessage());
                }
                throw new Exception(string.Join("\n", result.Diagnostics));
            }
        }

        // CreateInstance/InvokeMember
        public static Assembly Run(string code, string assemblyName = null, string dllPath = null, string pdbPath = null)
        {
            var syntaxTree = Parse(code);
            var compilation = Compile(syntaxTree, assemblyName);
            Assembly assembly;
            if (dllPath == null)
                assembly = Emit2Mem(compilation);
            else
                assembly = Emit2Dll(compilation, dllPath, pdbPath);
            return assembly;
        }

        public static Assembly Load(string dllPath, string pdbPath = null)
        {
            //return Assembly.LoadFrom(dllPath);
            if (pdbPath == null)
            {
                byte[] dllBytes = File.ReadAllBytes(dllPath);
                return Assembly.Load(dllBytes);
            }
            else
            {
                byte[] dllBytes = File.ReadAllBytes(dllPath);
                byte[] pdbBytes = File.ReadAllBytes(pdbPath);
                return Assembly.Load(dllBytes, pdbBytes);
            }
        }
    }
}
