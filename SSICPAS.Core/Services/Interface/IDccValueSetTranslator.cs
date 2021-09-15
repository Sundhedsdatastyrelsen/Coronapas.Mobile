using System.Threading.Tasks;
using SSICPAS.Core.Services.Model.EuDCCModel.ValueSet;

namespace SSICPAS.Core.Services.Interface
{
    public interface IDCCValueSetTranslator
    {
        public Task InitValueSetAsync();
        public object Translate(DCCValueSetEnum key, string code);
        public string GetDCCCode(DCCValueSetEnum key, object value);
    }
}