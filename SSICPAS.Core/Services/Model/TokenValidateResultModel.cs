using SSICPAS.Core.Services.Enum;
using SSICPAS.Core.Services.Interface;

namespace SSICPAS.Core.Services.Model
{
    public class TokenValidateResultModel
    {
        public TokenValidateResult ValidationResult { get; set; } = TokenValidateResult.Invalid;
        public ITokenPayload DecodedModel { get; set; }
        public string DecodedJson { get; set; }
    }
}