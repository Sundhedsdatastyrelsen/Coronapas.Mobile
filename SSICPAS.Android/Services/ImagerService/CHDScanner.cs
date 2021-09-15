using System.Collections.Generic;
using Android.App;
using Android.Content;
using SSICPAS.Services.Interfaces;
using SSICPAS.Services.Scanner;
using Device.Sdk;
using Device.Common;

namespace SSICPAS.Droid.Services.ImagerService
{
    public class CHDScanner : IImagerScanner
    {
        private Context _context = null;
        private bool _bRegistered = false;
        public IImagerReceiver Receiver { get; private set; }
        public List<ScannerModel> AvailableScanners { get; set; }
        public bool IsEnabled { get; private set; } = false;
        private static ScanManager mScanner;

        public CHDScanner()
        {
            _context = Application.Context;
            mScanner = new ScanManager();
            mScanner.ADecodeAPIInit();
            mScanner.ADecodeSetResultType(ScanConst.ResultType.DcdResultUsermsg);
            Receiver = new CHDReceiver(mScanner);
        }

        public void Disable()
        {
            if (Receiver != null && null != _context && _bRegistered)
            {
                _context.UnregisterReceiver(Receiver as BroadcastReceiver);
                _bRegistered = false;
            }

            IsEnabled = false;
        }

        public void SetSelectedScanner(ScannerModel scannerModel)
        {

        }
     
        public void Enable()
        {
            if (IsEnabled)
            {
                return;
            }

            if (null != Receiver && null != _context)
            {
                IntentFilter filter = new IntentFilter();
                filter.AddAction(Device.Common.ScanConst.IntentUsermsg);                
                _context.RegisterReceiver(Receiver as BroadcastReceiver, filter);
                _bRegistered = true;
            }

            IsEnabled = true;
        }

        public void SetConfig(IScannerConfig config)
        {
            
        }
    }
}