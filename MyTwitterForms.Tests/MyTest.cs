using System;
using Xunit;
using Xunit.Abstractions;

namespace MyTwitterForms.Tests
{
    public class MyTest : IDisposable
    {
        private readonly ITestOutputHelper outputHelper;

        public MyTest(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
        }

        [Fact]
        public void Test()
        {
            this.outputHelper.WriteLine("Test");
        }

        void IDisposable.Dispose() { }
    }
}
