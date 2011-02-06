using System.Collections.Generic;
using ExecIndex.Tests.Support;
using NUnit.Framework;

namespace ExecIndex.Tests
{
    [TestFixture]
    public class add_two_install_calls_to_index_assembly : IndexChangeContext
    {
        private CallIn _entryPoint;

        [TestFixtureSetUp]
        public void Given()
        {
            Construct_fresh_index();
            var asmbly1 = With_a_monitored_assembly_stored_under("friend1.dll");
            var asmbly2 = With_a_monitored_assembly_stored_under("friend2.dll");
            Add_calls(c => c.Install(null, null), asmbly1, asmbly2);
            _entryPoint = Get_entry_point_from_index();
        }

        [Test]
        public void the_index_will_call_into_the_registered_assemblies()
        {
            var l = new List<string>();
            _entryPoint.Install("A", l);
            l.HasCount(2);
            l[0].Contains("friend1").IsTrue();
            l[1].Contains("friend2").IsTrue();
        }
    }
}