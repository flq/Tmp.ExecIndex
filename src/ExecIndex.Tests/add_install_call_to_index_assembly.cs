using System;
using System.Linq.Expressions;
using System.Reflection;
using ExecIndex.Tests.Support;
using NUnit.Framework;
using System.Collections.Generic;

namespace ExecIndex.Tests
{
    [TestFixture]
    public class add_install_call_to_index_assembly : IndexChangeContext
    {
        private CallIn _entryPoint;

        [TestFixtureSetUp]
        public void Given()
        {
            Construct_fresh_index();
            var asmbly = With_a_monitored_assembly_stored_under("friend.dll");
            Add_calls(c => c.Install(null, null), asmbly);
            _entryPoint = Get_entry_point_from_index();
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