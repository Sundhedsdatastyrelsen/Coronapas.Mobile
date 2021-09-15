using SSICPAS.Services.Interfaces;
using SSICPAS.Services.Scanner;
using System;
using Honeywell.AIDC.CrossPlatform;
using Android.Content;
using Android.App;
using System.Collections.Generic;
using SSICPAS.Services.Status;
using System.Threading.Tasks;

namespace SSICPAS.Droid.Services.ImagerService
{    
    public class HoneywellScanner : IImagerScanner, IImagerReceiver
    {
        private BarcodeReader _barcodeReader = null;
        private Context _context = null;
        private ScannerModel _selectedScanner = null;

        public bool IsEnabled { get; private set; } = false;
        public event EventHandler<StatusEventArgs> OnBarcodeScanned;
        public IImagerReceiver Receiver { get; private set; } = null;

        public HoneywellScanner()
        {
            _context = Application.Context;
            _barcodeReader = new BarcodeReader(_context);
            Receiver = this;
        }

        public async void Disable()
        {
            await _barcodeReader.EnableAsync(false);
            await _barcodeReader.CloseAsync();

            IsEnabled = false;
        }

        public async void Enable()
        {
            if (IsEnabled)
            {
                return;
            }

            await _barcodeReader.OpenAsync();
            await SetScannerAndSymbologySettings();
            await _barcodeReader.EnableAsync(true);

            IsEnabled = true;
        }

        private async Task SetScannerAndSymbologySettings()
        {
            Dictionary<string, object> settings = new Dictionary<string, object>()
            {
                // Symbology settings
                { _barcodeReader.SettingKeys.AztecEnabled, false },
                { _barcodeReader.SettingKeys.CenterDecodeEnabled, false },
                { _barcodeReader.SettingKeys.ChinaPostEnabled, false },
                { _barcodeReader.SettingKeys.CodabarEnabled, false },
                { _barcodeReader.SettingKeys.CodablockAEnabled, false },
                { _barcodeReader.SettingKeys.CodablockFEnabled, false },
                { _barcodeReader.SettingKeys.Code11Enabled, false },
                { _barcodeReader.SettingKeys.Code128Enabled, false },
                { _barcodeReader.SettingKeys.Code39Base32Enabled, false },
                { _barcodeReader.SettingKeys.Code39Enabled, false },
                { _barcodeReader.SettingKeys.Code39FullAsciiEnabled, false },
                { _barcodeReader.SettingKeys.Code93Enabled, false },
                { _barcodeReader.SettingKeys.CompositeEnabled, false },
                { _barcodeReader.SettingKeys.CompositeWithUpcEnabled, false },
                { _barcodeReader.SettingKeys.DatamatrixEnabled, false },
                { _barcodeReader.SettingKeys.DotCodeEnabled, false },
                { _barcodeReader.SettingKeys.Ean13Enabled, false },
                { _barcodeReader.SettingKeys.Ean8Enabled, false },
                { _barcodeReader.SettingKeys.GridMatrixEnabled, false },
                { _barcodeReader.SettingKeys.Gs1128Enabled, false },
                { _barcodeReader.SettingKeys.HanXinEnabled, false },
                { _barcodeReader.SettingKeys.Iata25Enabled, false },
                { _barcodeReader.SettingKeys.Interleaved25Enabled, false },
                { _barcodeReader.SettingKeys.Isbt128Enabled, false },
                { _barcodeReader.SettingKeys.KoreanPostEnabled, false },
                { _barcodeReader.SettingKeys.Matrix25Enabled, false },
                { _barcodeReader.SettingKeys.MaxicodeEnabled, false },
                { _barcodeReader.SettingKeys.MicroPdf417Enabled, false },
                { _barcodeReader.SettingKeys.MsiEnabled, false },
                { _barcodeReader.SettingKeys.Pdf417Enabled, false },
                { _barcodeReader.SettingKeys.RssEnabled, false },
                { _barcodeReader.SettingKeys.RssExpandedEnabled, false },
                { _barcodeReader.SettingKeys.RssLimitedEnabled, false },
                { _barcodeReader.SettingKeys.Standard25Enabled, false },
                { _barcodeReader.SettingKeys.TelepenEnabled, false },
                { _barcodeReader.SettingKeys.TelepenOldStyleEnabled, false },
                { _barcodeReader.SettingKeys.Tlc39Enabled, false },
                { _barcodeReader.SettingKeys.UpcAEnable, false },
                { _barcodeReader.SettingKeys.UpcE1Enabled, false },
                { _barcodeReader.SettingKeys.UpcEEnabled, false },
                { _barcodeReader.SettingKeys.QrCodeEnabled, true },

                // Scanner settings
                { _barcodeReader.SettingKeys.DataProcessorLaunchBrowser, false },
                { _barcodeReader.SettingKeys.DataProcessorLaunchEZConfig, false },
            };

            await _barcodeReader.SetAsync(settings);

            _barcodeReader.BarcodeDataReady += OnBarcodeDataReady;
        }

        public void SetConfig(IScannerConfig config)
        {

        }

        public void SetSelectedScanner(ScannerModel scannerModel)
        {
            _selectedScanner = scannerModel;
        }

        private void OnBarcodeDataReady(object sender, BarcodeDataArgs e)
        {
            if (OnBarcodeScanned != null &&  e.SymbologyType == BarcodeSymbologies.Qr)
            {
                OnBarcodeScanned(this, new StatusEventArgs(e.Data));
            }
        }
    }
}