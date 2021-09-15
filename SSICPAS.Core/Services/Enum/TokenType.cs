using System.Collections.Generic;

namespace SSICPAS.Core.Services.Enum
{
    public enum TokenType
    {
        DK1,
        DK2,
        DK3,
        HC1,
        Unknown
    }

    public static class TokenTypeExtension
    {
        private static Dictionary<string, TokenType> tokenTypeDictionary = new Dictionary<string, TokenType>()
        {
            {"DK1", TokenType.DK1},
            {"DK2", TokenType.DK2},
            {"DK3", TokenType.DK3},
            {"HC1", TokenType.HC1},
        };

        public static TokenType GetTokenType(string prefix)
        {
            return tokenTypeDictionary.TryGetValue(prefix, out var result) ? result : TokenType.Unknown;
        }
    }
}