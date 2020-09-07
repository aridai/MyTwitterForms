using System;
using Prism.Navigation;

namespace MyTwitterForms.Tests
{
    internal class StubNavigationResult : INavigationResult
    {
        public bool Success => true;

        public Exception Exception => new Exception();
    }
}
