using System;
using System.Reflection;
using ExecIndex.Tests.Support;
using NUnit.Framework;
using System.Collections.Generic;

namespace ExecIndex.Tests
{
    [TestFixture]
    public class integration_test_ofreindexing_the_entry_assembly
    {
        private IModifyAssembly _updater;
        private CallIn _entryPoint;

        [TestFixtureSetUp]
        public void Given()
        {
            var testsCompiler = new TestsCompiler();
            testsCompiler
                .StoreAssemblyAs("friend.dll")
                .With("ClassWithPublicAndPrivate.cs");

            using (var updater = new AssemblyUpdater("TheIndex.dll"))
            {
                _updater = updater.For(c => c.Install(null, null));
                _updater.ReindexWithTheseAssemblies(testsCompiler.Assembly.AsEnumerable());
            }

            var a = Assembly.LoadFrom("TheIndex.dll");
            _entryPoint = (CallIn)Activator.CreateInstance(a.GetType("TheIndex.EntryPoint"));
        }

        [Test]
        public void the_setup_finds_the_concerned_method()
        {
            _updater.HasAccess.IsTrue();
        }

        [Test]
        public void the_index_will_call_into_the_registered_assembly()
        {
            var l = new List<string>();
            _entryPoint.Install("A", l);
            l.HasCount(1);
        }
    }
}