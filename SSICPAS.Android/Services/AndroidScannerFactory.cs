#nullable enable
using System.Collections.Generic;
using Android.Content.PM;
using SSICPAS.Droid.Services.ImagerService;
using SSICPAS.Models;
using SSICPAS.Services.Interfaces;
using Xamarin.Essentials;

namespace SSICPAS.Droid.Services
{
    public class AndroidScannerFactory : IScannerFactory
    {
        private Dictionary<SupportedScanner, string> _supportedScannerPackages = new Dictionary<SupportedScanner, string>()
        {
            { SupportedScanner.DataWedge, "com.symbol.datawedge"}
        };

        private Dictionary<SupportedScanner, string> _supportedManufacturers = new Dictionary<SupportedScanner, string>()
        {
            { SupportedScanner.Newland, "Newland"},
            { SupportedScanner.CHD, "CHD" },
            { SupportedScanner.CipherLab, "CipherLab" },
            { SupportedScanner.Honeywell, "Honeywell" }
        };

        private IImagerScanner _currentScanner { get; set; }
        public IImagerScanner? GetAvailableScanner()
        {
            PackageManager? pm = Android.App.Application.Context.PackageManager;
            foreach (var scanner in _supportedScannerPackages)
            {
                try
                {
                    pm?.GetPackageInfo(scanner.Value, PackageInfoFlags.Activities);
                    _currentScanner = new ZebraTechnologyScanner();
                    break;
                }
                catch (PackageManager.NameNotFoundException)
                {
                }
            }

            foreach (var manufacturer in _supportedManufacturers)
            {
                if (DeviceInfo.Manufacturer == manufacturer.Value)
                {
                    if (manufacturer.Key == SupportedScanner.Newland)
                    {
                        _currentScanner = new NewlandScanner();
                        break;
                    }
                    else if (manufacturer.Key == SupportedScanner.CHD)
                    {
                        _currentScanner = new CHDScanner();
                        break;
                    }
                    else if (manufacturer.Key == SupportedScanner.CipherLab)
                    {
                        _currentScanner = new CipherLabScanner();
                        break;
                    }
                    else if (manufacturer.Key == SupportedScanner.Honeywell)
                    {
                        _currentScanner = new HoneywellScanner();
                        break;
                    }
                }
            }

            return _currentScanner;
        }

        public IScannerConfig? GetScannerConfig()
        {
            switch (_currentScanner)
            {
                case ZebraTechnologyScanner _:
                    var config = new ZebraScannerConfig()
                    {
                        IsUPCE0 = false,
                        IsUPCE1 = false
                    };
                    return config;
                default:
                    return null;
            }
        }
    }

    public enum SupportedScanner
    {
        DataWedge,
        Newland,
        CipherLab,
        Honeywell,
        CHD,
    }
}