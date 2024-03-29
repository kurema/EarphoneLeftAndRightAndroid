﻿using System;
using System.Collections.Generic;
using System.Text;

using EarphoneLeftAndRight.Models;
using System.Collections.ObjectModel;

using System.Threading.Tasks;

namespace EarphoneLeftAndRight.Storages
{
    public class LicenseStorage
    {
        public static ObservableCollection<License.ILicenseEntry> NugetDatas { get => nugetDatas ??= GetNugetDatasTotal(); private set => nugetDatas = value; }
        public static bool IsLicenseTextLoaded = false;
        private static ObservableCollection<License.ILicenseEntry>? nugetDatas;

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
            new License.NormalLicense(){Name="Material Icons",LicenseUrl="https://www.apache.org/licenses/LICENSE-2.0.html",ProjectName=nameof(EarphoneLeftAndRight),Version="4.0.0", ProjectUrl="https://material.io/", LicenseText="Apache License, Version 2.0"},
            new License.NormalLicense(){Name="DSEG",LicenseUrl="https://github.com/keshikan/DSEG/blob/master/DSEG-LICENSE.txt",ProjectName=nameof(EarphoneLeftAndRight),Version="0.46",ProjectUrl="https://github.com/keshikan/DSEG",LicenseText="SIL Open Font License 1.1\nCopyright (c) 2020, keshikan (https://www.keshikan.net),\nwith Reserved Font Name \"DSEG\"."}
        };

        public static ObservableCollection<License.NugetData> GetNugetDatasCsv()
        {
            ObservableCollection<License.NugetData> nugets;
            try
            {
                using var sr = new System.IO.StreamReader(typeof(LicenseStorage).Assembly.GetManifestResourceStream(nameof(EarphoneLeftAndRight) + ".Licenses.nuget.csv"));
                nugets = License.GetNugetData(sr);
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
                if (item.Name is null) continue;
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
                using var sr = new System.IO.StreamReader(typeof(LicenseStorage).Assembly.GetManifestResourceStream(nameof(EarphoneLeftAndRight) + ".Licenses." + name + ".txt"));
                return await sr.ReadToEndAsync();
            }
            catch { }
            return "";
        }
    }
}
