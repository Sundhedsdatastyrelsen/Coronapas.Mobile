using System;
using SkiaSharp;
using SSICPAS.Configuration;
using SSICPAS.Core.Interfaces;

namespace SSICPAS.Utils
{
    public static class FontUtils
    {
        public static string MonoSpaceResourseFilePath = "SSICPAS.Resources.Fonts.IBMPlexMono-Regular.ttf";

        public static SKTypeface GetMonospaceSKTypeface()
        {
            return SKTypeface.FromStream(
                IoCContainer.Resolve<IAssemblyService>()
                    .GetSharedFormsAssembly()
                    .GetManifestResourceStream(MonoSpaceResourseFilePath));
        }
    }
}
