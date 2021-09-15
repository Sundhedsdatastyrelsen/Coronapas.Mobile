using Android.Content;
using Com.Cipherlab.Barcode;
using SSICPAS.Services.Interfaces;
using SSICPAS.Services.Status;
using System;

namespace SSICPAS.Droid.Services.ImagerService
{
    class CipherLabReceiver : BroadcastReceiver, IImagerReceiver
    {
        public event EventHandler<StatusEventArgs> OnBarcodeScanned;

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == GeneralString.IntentPASSTOAPP)
            {
                string barcode = intent.GetStringExtra(GeneralString.BcReaderData);
                string barcodeType = intent.GetStringExtra(GeneralString.BcReaderCodeTypeStr);

                if (OnBarcodeScanned != null && barcodeType == "QR Code")
                {
                    OnBarcodeScanned(this, new StatusEventArgs(barcode));
                }
            }
        }
    }
}