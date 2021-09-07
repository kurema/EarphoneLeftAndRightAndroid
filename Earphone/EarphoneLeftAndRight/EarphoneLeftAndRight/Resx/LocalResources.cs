using System;
using System.Collections.Generic;
using System.Text;

namespace EarphoneLeftAndRight.Resx
{
    internal static class LocalResources
    {
        private static Dependency.IPlatformLocalization _localization;
        private static Dependency.IPlatformLocalization localization => _localization = _localization ?? Xamarin.Forms.DependencyService.Get<Dependency.IPlatformLocalization>();

        internal static string Left => localization.WordLeft;
        internal static string Right => localization.WordRight;
        internal static string LeftRight => $"{localization.WordLeft} / {localization.WordRight}";
    }
}
