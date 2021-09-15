using System.Threading.Tasks;
using SSICPAS.Core.Services.Model.CoseModel;

namespace SSICPAS.Core.Services.Interface
{
    public interface ICertificationService
    {
        public Task VerifyCoseSign1Object(CoseSign1Object coseSign1Object);
    }
}