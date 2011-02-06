using System.Collections.Generic;
using ExecIndex.Tests.Support;
using NUnit.Framework;

namespace ExecIndex.Tests
{
    [TestFixture]
    public class remove_one_call_in_index_assembly : IndexChangeContext
    {
        private CallIn _entryPoint;

        [TestFixtureSetUp]
        public void Given()
        {
            Construct_fresh_index();
            var asmbly1 = With_a_monitored_assembly_stored_under("friend1.dll");
            var asmbly2 = With_a_monitored_assembly_stored_under("friend2.dll");
            Add_calls(c => c.Install(null, null), asmbly1, asmbly2);
            Remove_calls(c => c.Install(null, null), asmbly1);
            _entryPoint = Get_entry_point_from_index();
        }

        [Test]
        public void the_index_will_call_into_the_assembly_left()
        {
            var l = new List<string>();
            _entryPoint.Install("A", l);
            l.HasCount(1);
            l[0].Contains("friend2").IsTrue();
        }
    }
}