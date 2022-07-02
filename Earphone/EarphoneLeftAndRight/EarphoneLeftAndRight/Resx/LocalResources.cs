using System;
using System.Collections.Generic;
using System.Text;

namespace EarphoneLeftAndRight.Resx
{
    internal static class LocalResources
    {
        private static Dependency.IPlatformLocalization? _localization;
        private static Dependency.IPlatformLocalization Localization => _localization ??= Xamarin.Forms.DependencyService.Get<Dependency.IPlatformLocalization>();

        internal static string Left => Localization.WordLeft;
        internal static string Right => Localization.WordRight;
        internal static string LeftRight => $"{Localization.WordLeft} / {Localization.WordRight}";
    }
}
