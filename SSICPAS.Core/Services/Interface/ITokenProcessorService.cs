using System.Threading.Tasks;
using SSICPAS.Core.Services.Model;

namespace SSICPAS.Core.Services.Interface
{
    public interface ITokenProcessorService
    {
        Task<TokenValidateResultModel> DecodePassportTokenToModel(string base45String);
        Task SetDCCValueSetTranslator(IDCCValueSetTranslator translator, IDCCValueSetTranslator ratListTranslator);
        ITokenPayload MapToModelFromJson(string json, string tokenPrefix);
    }
}