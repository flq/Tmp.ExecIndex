using ExecIndex.Tests.Support;
using NUnit.Framework;

namespace ExecIndex.Tests
{
    [TestFixture]
    public class reindexing_the_entry_assembly
    {
        private IModifyAssembly _updater;

        [SetUp]
        public void Given()
        {
            var updater = new AssemblyUpdater("TheIndex.dll");
            _updater = updater.For(c => c.Install(null, null));
        }

        [Test]
        public void the_setup_finds_the_concerned_method()
        {
            _updater.HasAccess.IsTrue();
        }
    }
}