using System;
using System.Collections.Generic;
using System.Text;

using EarphoneLeftAndRight.Models;
using System.Collections.ObjectModel;

using System.Threading.Tasks;

namespace EarphoneLeftAndRight.Storages
{
    public class LicenseStorage
    {
        public static ObservableCollection<License.ILicenseEntry> NugetDatas { get => nugetDatas = nugetDatas ?? GetNugetDatasTotal(); private set => nugetDatas = value; }
        public static bool IsLicenseTextLoaded = false;
        private static ObservableCollection<License.ILicenseEntry> nugetDatas;

        public static ObservableCollection<License.ILicenseEntry> GetNugetDatasTotal()
        {
            var result = new ObservableCollection<License.ILicenseEntry>(GetNugetDatasCsv());
            foreach (var item in AdditonalLicense)
            {
                result.Add(item);
            }
            return result;
        }


        public static License.NormalLicense[] AdditonalLicense => new License.NormalLicense[]
        {
            new License.NormalLicense(){Name="Google Noto Fonts.Noto Sans CJK JP",LicenseUrl="http://scripts.sil.org/cms/scripts/page.php?site_id=nrsi&id=OFL",ProjectName="WordbookImpressApp",Version="v2017-06-01-serif-cjk-1-1"}
        };

        public static ObservableCollection<License.NugetData> GetNugetDatasCsv()
        {
            ObservableCollection<License.NugetData> nugets;
            try
            {
                using (var sr = new System.IO.StreamReader(typeof(LicenseStorage).Assembly.GetManifestResourceStream(nameof(EarphoneLeftAndRight) + ".Licenses.nuget.csv")))
                {
                    nugets = License.GetNugetData(sr);
                }
            }
            catch
            {
                return new ObservableCollection<License.NugetData>();
            }
            return nugets;
        }

        public async static Task LoadNugetDatasLicenseText()
        {
            var nugets = NugetDatas;
            foreach (var item in nugets)
            {
                try
                {
                    item.LicenseText = await LoadLicenseText(item.Name);
                }
                catch
                {
                    item.LicenseText = item.LicenseUrl;
                }
            }
            IsLicenseTextLoaded = true;
            NugetDatas = nugets;
        }

        public async static Task<string> LoadLicenseText(string name)
        {
            try
            {
                using (var sr = new System.IO.StreamReader(typeof(LicenseStorage).Assembly.GetManifestResourceStream(nameof(EarphoneLeftAndRight) + ".Licenses." + name + ".txt")))
                {
                    return await sr.ReadToEndAsync();
                }
            }
            catch { }
            return "";
        }
    }
}
