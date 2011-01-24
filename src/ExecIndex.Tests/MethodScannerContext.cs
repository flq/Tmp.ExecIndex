using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using ExecIndex.Tests.Support;
using NUnit.Framework;
using System.Linq;

namespace ExecIndex.Tests
{
    [TestFixture,Ignore]
    public class MethodScannerContext
    {
        private readonly TestsCompiler _testsCompiler;
        private IMethodScanner _methodScanner;

        public MethodScannerContext()
        {
            _testsCompiler = new TestsCompiler();
        }

        protected IList<MethodInfo> MethodInfos { get; private set; }

        protected void AssemblyFrom(params string[] codeFiles)
        {
            _testsCompiler.With(codeFiles);
        }

        [TestFixtureTearDown]
        public void End()
        {
            _testsCompiler.Dispose();
        }

        protected void ScanDelegateBased<T>(BindingFlags bindingFlags)
        {
            _methodScanner = new DelegateBasedMethodScanner<T>(bindingFlags);
            MethodInfos = _methodScanner.Scan(_testsCompiler.Assembly.AsEnumerable()).ToList();
        }

        protected void ScanMethodInfoBased<T>(Expression<Action<T>> methodSelector)
        {
            _methodScanner = new MethodInfoBasedScanner<T>(methodSelector);
            MethodInfos = _methodScanner.Scan(_testsCompiler.Assembly.AsEnumerable()).ToList();
        }
    }
}
