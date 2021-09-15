using SSICPAS.Services.Interfaces;
using SSICPAS.Services.Scanner;
using Com.Cipherlab.Barcode;
using Com.Cipherlab.Barcode.Decoderparams;
using Com.Cipherlab.Barcode.Decoder;
using Android.Content;
using Android.App;

namespace SSICPAS.Droid.Services.ImagerService
{
    class CipherLabScanner : IImagerScanner
    {
        private ReaderManager _readerManager = null;
        private Context _context = null;
        private bool _bRegistered = false;
        private IntentFilter _intentFilter = null;

        private ScannerModel _selectedScanner { get; set; }

        public IImagerReceiver Receiver { get; private set; }
        public bool IsEnabled { get; private set; } = false;

        public CipherLabScanner()
        {
            _context = Application.Context.ApplicationContext;
            _readerManager = ReaderManager.InitInstance(_context);
            Receiver = new CipherLabReceiver();
        }

        public void Disable()
        {
            if (Receiver != null && _context != null && _bRegistered)
            {
                _context.UnregisterReceiver(Receiver as BroadcastReceiver);
                _bRegistered = false;
            }

            _readerManager.Release();
            IsEnabled = false;
        }

        public void Enable()
        {
            if (IsEnabled)
            {
                return;
            }

            if (_context != null && Receiver != null)
            {
                _intentFilter = new IntentFilter();
                _intentFilter.AddAction(GeneralString.IntentPASSTOAPP);
                _context.RegisterReceiver(Receiver as BroadcastReceiver, _intentFilter);
                _bRegistered = true;
            }

            IsEnabled = true;
        }

        public void SetConfig(IScannerConfig config)
        {
            Decoders decoders = new Decoders();
            decoders.EnableAustrailianPostal = Enable_State.False;
            decoders.EnableAztec = Enable_State.False;
            decoders.EnableChinese2Of5 = Enable_State.False;
            decoders.EnableCip39 = Enable_State.False;
            decoders.EnableCodabar = Enable_State.False;
            decoders.EnableCode11 = Enable_State.False;
            decoders.EnableCode128 = Enable_State.False;
            decoders.EnableCode39 = Enable_State.False;
            decoders.EnableCode93 = Enable_State.False;
            decoders.EnableCompositeCCAB = Enable_State.False;
            decoders.EnableCompositeCCC = Enable_State.False;
            decoders.EnableCompositeTlc39 = Enable_State.False;
            decoders.EnableDataMatrix = Enable_State.False;
            decoders.EnableDotCode = Enable_State.False;
            decoders.EnableDutchPostal = Enable_State.False;
            decoders.EnableEanJan13 = Enable_State.False;
            decoders.EnableEanJan8 = Enable_State.False;
            decoders.EnableFrenchPharmacode = Enable_State.False;
            decoders.EnableGs1128 = Enable_State.False;
            decoders.EnableGs1DataBar14 = Enable_State.False;
            decoders.EnableGs1DataBarExpanded = Enable_State.False;
            decoders.EnableGs1DataBarLimited = Enable_State.False;
            decoders.EnableGs1DatabarToUpcEan = Enable_State.False;
            decoders.EnableHanxin = Enable_State.False;
            decoders.EnableIndustrial2Of5 = Enable_State.False;
            decoders.EnableInterleaved2Of5 = Enable_State.False;
            decoders.EnableIsbt128 = Enable_State.False;
            decoders.EnableJapanPostal = Enable_State.False;
            decoders.EnableKorean3Of5 = Enable_State.False;
            decoders.EnableLaetus = Enable_State.False;
            decoders.EnableMatrix2Of5 = Enable_State.False;
            decoders.EnableMaxiCode = Enable_State.False;
            decoders.EnableMicroPDF417 = Enable_State.False;
            decoders.EnableMicroQR = Enable_State.False;
            decoders.EnableMRZ = Enable_State.False;
            decoders.EnableMsi = Enable_State.False;
            decoders.EnablePDF417 = Enable_State.False;
            decoders.EnablePlessey = Enable_State.False;
            decoders.EnableTelepen = Enable_State.False;
            decoders.EnableTriopticCode39 = Enable_State.False;
            decoders.EnableUccCoupon = Enable_State.False;
            decoders.EnableUKPostal = Enable_State.False;
            decoders.EnableUpcA = Enable_State.False;
            decoders.EnableUpcE = Enable_State.False;
            decoders.EnableUpcE1 = Enable_State.False;
            decoders.EnableUPUFICSPostal = Enable_State.False;
            decoders.EnableUSPlanet = Enable_State.False;
            decoders.EnableUSPostnet = Enable_State.False;
            decoders.EnableUSPSPostal = Enable_State.False;

            decoders.EnableQRcode = Enable_State.True;
            _readerManager.Set_Decoders_Status(decoders);

            ReaderOutputConfiguration settings = new ReaderOutputConfiguration();
            settings.EnableKeyboardEmulation = KeyboardEmulationType.None;
            settings.AutoEnterWay = OutputEnterWay.Disable;
            settings.AutoEnterChar = OutputEnterChar.None;
            settings.ShowCodeLen = Enable_State.False;
            settings.ShowCodeType = Enable_State.False;
            settings.SzPrefixCode = string.Empty;
            settings.SzSuffixCode = string.Empty;
            settings.UseDelim = (char)0;
            settings.SzCharsetName = "UTF-8";
            settings.ClearPreviousData = Enable_State.True;
            _readerManager.Set_ReaderOutputConfiguration(settings);
        }

        public void SetSelectedScanner(ScannerModel scannerModel)
        {
            _selectedScanner = scannerModel;
        }

    }
}