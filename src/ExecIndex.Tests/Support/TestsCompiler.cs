using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.CSharp;
using System.Linq;

namespace ExecIndex.Tests.Support
{
    public class TestsCompiler : IDisposable
    {
        //Assuming it runs in bin/debug
        private const string BaseDirectoryCompileFiles = @"..\..\CompileArtefacts";
        private readonly CSharpCodeProvider provider;
        private readonly CompilerParameters parms;
        private CompilerResults result;

        private readonly string assemblyName = Path.GetRandomFileName();

        public TestsCompiler()
        {
            provider = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v3.5" } });
            parms = new CompilerParameters
                      {
                          GenerateExecutable = false,
                          OutputAssembly = assemblyName,
                          GenerateInMemory = true
                      };
            //parms.ReferencedAssemblies.Add("Bin4Net.Publish.dll");
            //parms.ReferencedAssemblies.Add("System.Core");
        }

        public TestsCompiler With(params string[] files)
        {
            result = provider.CompileAssemblyFromFile(parms,
              files.Select(f => Path.Combine(BaseDirectoryCompileFiles, f)).ToArray());
            return this;
        }

        
        public Assembly Assembly
        {
            get
            {
                if (result.Errors.Count > 0)
                {
                    foreach (CompilerError e in result.Errors)
                        Console.WriteLine("Error in line {0} in file {1}: {2}", e.Line, e.FileName, e.ErrorText);
                    throw new InvalidOperationException("The compiler encountered errors: Please review the console output");
                }
                return result.CompiledAssembly;
            }
        }

        public TestsCompiler StoreAssemblyAs(string fullPath)
        {
            parms.GenerateInMemory = false;
            parms.OutputAssembly = fullPath;
            return this;
        }

        public void Dispose()
        {
            if (File.Exists(assemblyName))
                File.Delete(assemblyName);
        }
    }
}