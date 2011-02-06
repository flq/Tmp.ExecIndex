using System;
using System.IO;
using ExecIndex.Tests.Support;
using NUnit.Framework;
using System.Collections.Generic;

namespace ExecIndex.Tests
{
    public class calling_into_a_remote_assembly : IndexChangeContext
    {
        private CallIn _entryPoint;

        [TestFixtureSetUp]
        public void Given()
        {
            Construct_fresh_index();
            var remoteLocation = Path.Combine(Path.GetTempPath(), "friend.dll");
            ensure_this_assembly_is_missing_locally("friend.dll");
            var a = With_a_monitored_assembly_stored_under(remoteLocation);

            using (var updater = new AssemblyUpdater("TheIndex.dll"))
            {
                updater.For(c => c.Install(null, null));
                updater.ReindexWithTheseAssemblies(a.AsEnumerable());
            }

            _entryPoint = Get_entry_point_from_index();
        }

        [Test]
        public void the_index_will_call_into_the_remote_assembly()
        {
            var l = new List<string>();
            _entryPoint.Install("A", l);
            l.HasCount(1);
        }

    }
}