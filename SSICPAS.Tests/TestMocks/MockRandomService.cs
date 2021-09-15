using SSICPAS.Core.Interfaces;

namespace SSICPAS.Tests.TestMocks
{
    public class MockRandomService: IRandomService
    {
        public MockRandomService()
        {
        }

        double IRandomService.GenerateRandomDouble()
        {
            return 0.5;
        }

        public string GenerateRandomString(int length)
        {
            return "a string";
        }
    }
}
