using System;
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
        public static author? AuthorInformation { get; private set; }
        public static string AuthorInformationPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AuthorInformation.xml");
        public static string AuthorInformationUrl => "https://kurema.github.io/api/impress/author.xml";

        public static async Task<T?> LoadLocalData<T>(string path)
        {
            if (!File.Exists(path))
            {
                return default;
            }
            try
            {
                return await DeserializeAsync<T>(path);
            }
            catch
            {
                return default;
            }
        }

        public static async Task LoadRemoteDatas()
        {
            AuthorInformation = await LoadRemoteData<author>(AuthorInformationUrl, AuthorInformationPath) ?? await LoadLocalData<author>(AuthorInformationPath);
            OnUpdated();
        }

        public static async Task<T?> LoadRemoteData<T>(string url, string localPath)
        {
            using System.Net.WebClient wc = new();
            wc.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            var client = new System.Net.Http.HttpClient();
            try
            {
                var str = await client.GetStringAsync(url);
                var result = await DeserializeAsync<T>(new StringReader(str));
                if (File.Exists(localPath)) File.Delete(localPath);
                using (var sw = new StreamWriter(localPath))
                {
                    await sw.WriteAsync(str);
                }
                return result;
            }
            catch
            {
                return default;
            }
        }

        public static event EventHandler? Updated;
        public static void OnUpdated()
        {
            Updated?.Invoke(null, new EventArgs());
        }

        public static string? GetStringByMultilingal(IEnumerable<multilingalEntry>? entry, System.Globalization.CultureInfo? culture = null)
        {
            if (entry == null || entry.Count() == 0) return null;
            culture ??= System.Globalization.CultureInfo.CurrentCulture;
            var w = entry.Where((e) => e.language == culture.Name || e.language == culture.Parent.Name);
            if (w.Count() > 0) { return w.First().Value; } else { return null; }
        }


        public static async Task<T?> DeserializeAsync<T>(string path)
        {
            try
            {
                using var sr = new StreamReader(path);
                return await DeserializeAsync<T?>(sr);
            }
            finally
            {
            }
        }

        static readonly System.Threading.SemaphoreSlim semaphore = new(1, 1);

        public static async Task<T?> DeserializeAsync<T>(TextReader sr)
        {
            await semaphore.WaitAsync();
            try
            {
                var xs = new XmlSerializer(typeof(T));
                using var xr = XmlReader.Create(sr, new XmlReaderSettings() { CheckCharacters = false });
                return await Task.Run<T>(() => { try { return (T)xs.Deserialize(xr); } catch { return default!; } });
            }
            finally
            {
                semaphore.Release();
            }
        }

    }
}
