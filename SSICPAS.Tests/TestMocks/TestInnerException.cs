using System;
namespace SSICPAS.Tests.TestMocks
{
    public class TestInnerException : Exception
    {
        public TestInnerException() : base() { }
        public TestInnerException(string message) : base(message) { }

        public string StackTraceString { get; set; }
        public override string StackTrace => StackTraceString;
    }
}
