using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.CSharp;
using System.Linq;

namespace ExecIndex.Tests.Support
{
    internal class TestsCompiler : IDisposable
    {
        

        //Assuming it runs in bin/debug
        private const string BaseDirectoryCompileFiles = @"..\..\CompileArtefacts";
        private readonly CSharpCodeProvider provider;
        private readonly CompilerParameters parms;
        private CompilerResults result;
        private readonly string baseDir;

        private readonly string assemblyName = Path.GetRandomFileName();

        public TestsCompiler(string baseDir = BaseDirectoryCompileFiles)
        {
            this.baseDir = baseDir;
            provider = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v3.5" } });
            parms = new CompilerParameters
                      {
                          GenerateExecutable = false,
                          OutputAssembly = assemblyName,
                          GenerateInMemory = true
                      };
        }

        
        public TestsCompiler With(params string[] files)
        {
            result = provider.CompileAssemblyFromFile(parms,
              files.Select(f => Path.Combine(baseDir, f)).ToArray());
            outputErrorsIfAny();
            return this;
        }

        
        public TestsCompiler ReferenceThisAssembly(string assembly)
        {
            parms.ReferencedAssemblies.Add(assembly);
            return this;
        }

        public Assembly Assembly
        {
            get
            {
                outputErrorsIfAny();
                return result.CompiledAssembly;
            }
        }

        private void outputErrorsIfAny()
        {
            if (result.Errors.Count > 0)
            {
                foreach (CompilerError e in result.Errors)
                    Console.WriteLine("Error in line {0} in file {1}: {2}", e.Line, e.FileName, e.ErrorText);
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