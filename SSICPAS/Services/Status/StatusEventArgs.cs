using System;

namespace SSICPAS.Services.Status
{
    /// <summary>
    /// Custom event args for use by the scanner
    /// </summary>
    public class StatusEventArgs : EventArgs
    {
        public string Data => barcodeData;

        private string barcodeData;
        
        public StatusEventArgs(string dataIn)
        {
            barcodeData = dataIn;
        }
    }
}
