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

namespace MeasureApp.Model
{
    public static class CodeCompiler
    {
        // 删除Location为空的Assembly?测试程序多次运行
        private static readonly IEnumerable<PortableExecutableReference> Assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Select<Assembly, PortableExecutableReference>(
            (x) =>
        {
            return MetadataReference.CreateFromFile(x.Location);
        }
        );
        // 测试函数
        public static string SayHello()
        {
            return "Hello";
        }

        public static Type Run(string code, string className)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var type = CompileType(className, syntaxTree);
            return type;
        }

        private static Type CompileType(string originalClassName, SyntaxTree syntaxTree)
        {
            // 指定编译选项。
            var assemblyName = $"{originalClassName}.g";
            // BUG:多次执行Assemblies数量不同
            //var Assemblies = AppDomain.CurrentDomain.GetAssemblies().Select(x => MetadataReference.CreateFromFile(x.Location));
            Debug.WriteLine($"Assemblies Count={Assemblies.Count()}");
            CSharpCompilation compilation = CSharpCompilation.Create(assemblyName, new[] { syntaxTree },
                    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(
                    //加入到引用
                    Assemblies
                    );


            // 编译到内存流中。
            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (result.Success)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    var assembly = Assembly.Load(ms.ToArray());
                    return assembly.GetTypes().First(x => x.Name == originalClassName);
                }
                throw new Exception(string.Join("\n", result.Diagnostics));
            }
        }
    }
}
