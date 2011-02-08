using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using ExecIndex.Tests.Support;
using NUnit.Framework;

namespace ExecIndex.Tests
{
    [TestFixture,Ignore("context class")]
    public class IndexChangeContext
    {
        public void Construct_fresh_index()
        {
            ensure_this_assembly_is_missing_locally("TheIndex.dll");
            var c = new TestsCompiler(@"..\..\..\TheIndex");
            c
                .ReferenceThisAssembly("ExecIndex.dll")
                .StoreAssemblyAs("TheIndex.dll")
                .With("EntryPoint.cs", "Resolver.cs");
        }

        public Assembly With_a_monitored_assembly_stored_under(string assemblyName)
        {
            var testsCompiler = new TestsCompiler();
            testsCompiler
                .StoreAssemblyAs(assemblyName)
                .With("ClassWithPublicAndPrivate.cs");
            return testsCompiler.Assembly;
        }

        public CallIn Get_entry_point_from_index()
        {
            byte[] bytes = null;
            using ( var fs = File.Open("TheIndex.dll",FileMode.Open))
            {
                bytes = new byte[fs.Length];
                fs.Read(bytes, 0, (int)fs.Length); //Will never be that large here
            }

            var a = Assembly.Load(bytes);
            return (CallIn)Activator.CreateInstance(a.GetType("TheIndex.EntryPoint"));
        }

        protected void ensure_this_assembly_is_missing_locally(string assemblyFileName)
        {
            if (File.Exists(assemblyFileName))
                File.Delete(assemblyFileName);
        }

        protected void Add_calls(Expression<Action<CallIn>> methodSelector, params Assembly[] asmblies)
        {
            using (var updater = new AssemblyUpdater("TheIndex.dll"))
            {
                
                updater.For(methodSelector)
                    .AddCallsWithTheseAssemblies(asmblies);
            }
        }

        protected void Remove_calls(Expression<Action<CallIn>> methodSelector, params Assembly[] asmblies)
        {
            using (var updater = new AssemblyUpdater("TheIndex.dll"))
            {

                updater.For(methodSelector)
                    .RemoveCallsToTheseAssemblies(asmblies);
            }
        }
    }
}