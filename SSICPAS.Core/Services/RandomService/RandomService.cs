using System;
using System.Linq;
using SSICPAS.Core.Interfaces;

namespace SSICPAS.Core.Services.RandomService
{
    public class RandomService: IRandomService
    {
        private Random _random = new Random();
        
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ~!_+{}|:;'\"<>?!@#$%^&*()";
        
        public double GenerateRandomDouble()
        {
            return _random.NextDouble();
        }
        
        public string GenerateRandomString(int length)
        {
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}
