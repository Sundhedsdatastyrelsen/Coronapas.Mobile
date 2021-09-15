using SSICPAS.Services.Status;
using System;

namespace SSICPAS.Services.Interfaces
{
    public interface IImagerReceiver
    {
        event EventHandler<StatusEventArgs> OnBarcodeScanned;
    }
}