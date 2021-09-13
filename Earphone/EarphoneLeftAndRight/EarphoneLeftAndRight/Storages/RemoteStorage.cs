﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using EarphoneLeftAndRight.Schemas.AuthorInformation;

using System.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace EarphoneLeftAndRight.Storages
{
    public static class RemoteStorage
    {
        public static author AuthorInformation { get; private set; }
        public static string AuthorInformationPath { get; set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AuthorInformation.xml");
        public static string AuthorInformationUrl => "https://kurema.github.io/api/impress/author.xml";

        public static async Task<T> LoadLocalData<T>(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                return default(T);
            }
            try
            {
                return await DeserializeAsync<T>(path);
            }
            catch
            {
                return default(T);
            }
        }

        public static async Task LoadRemoteDatas()
        {
            AuthorInformation = await LoadRemoteData<author>(AuthorInformationUrl, AuthorInformationPath) ?? await LoadLocalData<author>(AuthorInformationPath);
            OnUpdated();
        }

        public static async Task<T> LoadRemoteData<T>(string url,string localPath)
        {
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                wc.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                var client = new System.Net.Http.HttpClient();
                try
                {
                    var str = await client.GetStringAsync(url);
                    var result = await DeserializeAsync<T>(new System.IO.StringReader(str));
                    if (System.IO.File.Exists(localPath)) System.IO.File.Delete(localPath);
                    using(var sw=new System.IO.StreamWriter(localPath))
                    {
                        await sw.WriteAsync(str);
                    }
                    return result;
                }
                catch
                {
                    return default(T);
                }
            }
        }

        public static event EventHandler Updated;
        public static void OnUpdated()
        {
            Updated?.Invoke(null, new EventArgs());
        }

        public static string GetStringByMultilingal(IEnumerable<multilingalEntry> entry, System.Globalization.CultureInfo culture = null)
        {
            if (entry == null || entry.Count() == 0) return null;
            culture = culture ?? System.Globalization.CultureInfo.CurrentCulture;
            var w = entry.Where((e) => e.language == culture.Name || e.language == culture.Parent.Name);
            if (w.Count() > 0) { return w.First().Value; } else { return null; }
        }


        public static async Task<T> DeserializeAsync<T>(string path)
        {
            try
            {
                using (var sr = new StreamReader(path))
                {
                    return await DeserializeAsync<T>(sr);
                }
            }
            finally
            {
            }
        }

        static System.Threading.SemaphoreSlim semaphore = new System.Threading.SemaphoreSlim(1, 1);

        public static async Task<T> DeserializeAsync<T>(TextReader sr)
        {
            await semaphore.WaitAsync();
            try
            {
                var xs = new XmlSerializer(typeof(T));
                using (var xr = XmlReader.Create(sr, new XmlReaderSettings() { CheckCharacters = false }))
                {
                    return await Task.Run<T>(() => { try { return (T)xs.Deserialize(xr); } catch { return default; } });
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

    }
}
