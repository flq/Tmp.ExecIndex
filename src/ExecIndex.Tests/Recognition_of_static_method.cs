using System;
using System.Reflection;
using ExecIndex.Tests.Support;
using NUnit.Framework;

namespace ExecIndex.Tests
{

    public class scanning_w_methinfo_scanner : MethodScannerContext
    {
        public scanning_w_methinfo_scanner()
        {
            AssemblyFrom("ClassWithPublicAndPrivate.cs");
            ScanMethodInfoBased<CallIn>(c=>c.Install(null,null));
        }

        [Test]
        public void a_single_method_was_found()
        {
            MethodInfos.HasCount(1);
        }

        [Test]
        public void method_name_matches()
        {
            MethodInfos[0].Name.IsEqualTo("StringAndList");
        }
    }

    public class scanning_w_delegate_scanner : MethodScannerContext
    {
        public scanning_w_delegate_scanner()
        {
            AssemblyFrom("ClassWithPublicAndPrivate.cs");
            ScanDelegateBased<Action<string>>(BindingFlags.Public | BindingFlags.Static);
        }

        [Test]
        public void a_single_method_was_found()
        {
            MethodInfos.HasCount(1);
        }

        [Test]
        public void method_name_matches()
        {
            MethodInfos[0].Name.IsEqualTo("VoidMethodOneParam");
        }
    }
}