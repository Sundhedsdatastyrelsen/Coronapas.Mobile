using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Runtime;
using SSICPAS.Services.Interfaces;
using SSICPAS.Services.Scanner;
using SSICPAS.Services.Status;

namespace SSICPAS.Droid.Services.ImagerService
{
    [BroadcastReceiver]
    public class DataWedgeReceiver : BroadcastReceiver, IImagerReceiver
    {
        // This intent string contains the source of the data as a string
        private static string SOURCE_TAG = "com.motorolasolutions.emdk.datawedge.source";
        // This intent string contains the barcode symbology as a string
        private static string LABEL_TYPE_TAG = "com.motorolasolutions.emdk.datawedge.label_type";
        // This intent string contains the captured data as a string
        // (in the case of MSR this data string contains a concatenation of the track data)
        private static string DATA_STRING_TAG = "com.motorolasolutions.emdk.datawedge.data_string";

        public static string INTENT_BARCODE_ACTION = "SSICPAS_BARCODE.RECVR";
        public static string INTENT_RESULT_ACTION = "com.symbol.datawedge.api.RESULT_ACTION";
        public static string INTENT_SOFT_SCAN_ACTION = "com.symbol.datawedge.api.ACTION_SOFTSCANTRIGGER";
        public static string INTENT_CATEGORY = "android.intent.category.DEFAULT";

        public static string INTENT_RESULT_ENUMERATE_SCANNER = "com.symbol.datawedge.api.RESULT_ENUMERATE_SCANNERS";
        public event EventHandler<StatusEventArgs> OnBarcodeScanned;
        public event EventHandler<List<ScannerModel>> ScannersInfoReceived;

        public override void OnReceive(Context context, Intent intent)
        {
            string action = intent.Action;

            if (action.Equals(INTENT_BARCODE_ACTION))
            {
                OnReceiveBarcodeIntentResult(intent);
            }

            if (intent.HasExtra(INTENT_RESULT_ENUMERATE_SCANNER))
            {
                OnReceiveEnumerateScannerInfoIntentResult(intent);

            }
        }

        private void OnReceiveBarcodeIntentResult(Intent intent)
        {
            string sLabelType;
            
            string source = intent.GetStringExtra(SOURCE_TAG);
            if (source == null)
            {
                source = "scanner";
            }
            
            string data = intent.GetStringExtra(DATA_STRING_TAG);
            if (source != "scanner")
            {
                return;
            }
            
            if (data == null || data.Length <= 0)
            {
                return;
            }
            
            sLabelType = intent.GetStringExtra(LABEL_TYPE_TAG);

            if (!string.IsNullOrEmpty(sLabelType))
            {
                // Format of the label type string is LABEL-TYPE-SYMBOLOGY.
                // Skip the LABEL-TYPE- portion to get just the symbology
                sLabelType = sLabelType.Substring(11);
            }
            else
            {
                sLabelType = "Unknown";
            }

            if (OnBarcodeScanned != null && sLabelType == "QRCODE")
            {
                OnBarcodeScanned(this, new StatusEventArgs(data.ToString()));
            }
        }

        private void OnReceiveEnumerateScannerInfoIntentResult(Intent intent)
        {
            List<ScannerModel> scannerModelList = new List<ScannerModel>();
            IJavaObject scannerList = intent.GetSerializableExtra(INTENT_RESULT_ENUMERATE_SCANNER);

            var bundleList = scannerList.JavaCast<JavaList<Bundle>>();
            if ((bundleList != null) && bundleList.Size() > 0)
            {
                foreach (Bundle bundle in bundleList)
                {
                    string name = bundle.GetString("SCANNER_NAME");
                    bool connectionState = bundle.GetBoolean("SCANNER_CONNECTION_STATE");
                    string identifier = bundle.GetString("SCANNER_IDENTIFIER");

                    ScannerModel model = new ScannerModel(
                        identifier,
                        name,
                        connectionState);

                    if (model.Id == "INTERNAL_CAMERA")
                    {
                        model.IsMainCamera = true;
                    }
                    scannerModelList.Add(model);
                }
            }
            scannerModelList = scannerModelList.Where(x => x.ConnectionState).ToList();

            if (ScannersInfoReceived != null)
            {
                ScannersInfoReceived(this, scannerModelList);
            }
        }
    }
}