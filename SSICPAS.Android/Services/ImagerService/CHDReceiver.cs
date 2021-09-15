using System;
using Android.Content;
using SSICPAS.Services.Interfaces;
using SSICPAS.Services.Status;
using Device.Sdk;
using Device.Common;

namespace SSICPAS.Droid.Services.ImagerService
{
    [BroadcastReceiver]
    public class CHDReceiver : BroadcastReceiver, IImagerReceiver
    {
        public event EventHandler<StatusEventArgs> OnBarcodeScanned;
        public ScanManager mScanner { get; set; }
        public DecodeResult mDecodeResult { get; set; }

        public CHDReceiver() { }

        public CHDReceiver(ScanManager scanManager)
        {
            mScanner = scanManager;
            mDecodeResult = new DecodeResult();

        }

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent == null)
            {
                return;
            }

            if (intent.Action == Device.Common.ScanConst.IntentUsermsg)
            {
                OnReceiveBarcodeIntentResult(intent);
            }
        }

        private void OnReceiveBarcodeIntentResult(Intent intent)
        {
            if (intent == null)
            {
                return;
            }
            mScanner.ADecodeGetResult(mDecodeResult.Recycle());

            string barcode = mDecodeResult.ToString();

            if (barcode == null)
            {
                return;
            }

            if (OnBarcodeScanned != null && mDecodeResult.SymType == Device.Common.ScanConst.SymbologyID.DcdSymQr)
            {
                OnBarcodeScanned(this, new StatusEventArgs(barcode));
            }
        }
    }
}