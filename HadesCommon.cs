using HadesAIOCommon.Http;
using Leaf.xNet;
using Newtonsoft.Json.Linq;
using OtpNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace HadesAIOCommon
{
    public class HadesCommon
    {
        private const string CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const string ALPHABET_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string NUMBERS = "0123456789";

        private static readonly object MUTEX = new();
        private static readonly Random rand = new();

        public static void Delay(int seconds)
        {
            Thread.Sleep(1000 * seconds);
        }
        public static void Delay(int min, int max)
        {
            Delay(rand.Next(min, max));
        }
        public static int Random(int min, int max)
        {
            return rand.Next(min, max);
        }
        public static bool SetThreadPool(int n)
        {
            bool s1 = ThreadPool.SetMaxThreads(1600, 3200);
            int minT = n + Environment.ProcessorCount;
            bool s2 = ThreadPool.SetMinThreads(minT, 3200);
            return s1 && s2;
        }


        public static Bitmap Base64ToBitmap(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            Bitmap bitmap;
            using (MemoryStream ms = new(bytes))
            {
                bitmap = new Bitmap(ms);
            }
            return bitmap;
        }
        public static void DisposeBitmap(Bitmap bitmap)
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    bitmap.Dispose();
                }
            }
            catch
            {
            }
        }
        public static string Get2FACode(string salt)
        {
            salt = salt.Replace(" ", "");
            var totp = new Totp(Base32Encoding.ToBytes(salt));
            if (totp.RemainingSeconds() < 5)
            {
                Thread.Sleep(5000);
                return Get2FACode(salt);
            }
            return totp.ComputeTotp();
        }

        public static string GetRandomFullName()
        {
            try
            {
                string body = "type=fullname&number=1&X-Requested-With=XMLHttpRequest";
                var headers = new EzHttpHeader().AjaxHeader().Add("x-requested-with", "XMLHttpRequest").Build;
                string json = EzHttpRequest.Instance.Post("https://randommer.io/Name", body, EzHttpRequest.ContentType.FORM_URLENCODED_UTF8, headers);
                List<string>? names = JSON.Parse<List<string>>(json);
                return names?[0] ?? "John Cena " + Random(1, 100);
            }
            catch
            {
            }
            return "John Cena" + Random(1, 100);
        }
        public static string RandomString(int size, bool lowerCase = false)
        {
            string str = new(Enumerable.Repeat(CHARS, size)
              .Select(s => s[rand.Next(s.Length)]).ToArray());
            return lowerCase ? str.ToLower() : str;
        }
        public static string RandomAlphabetString(int size, bool lowerCase = false)
        {
            string str = new(Enumerable.Repeat(ALPHABET_CHARS, size)
              .Select(s => s[rand.Next(s.Length)]).ToArray());
            return lowerCase ? str.ToLower() : str;
        }
        public static T? RandomInList<T>(IList<T> list)
        {
            if (list == null || list.Count == 0)
            {
                return default;
            }
            return list[rand.Next(list.Count)];
        }
        public static T? GetRandomInListThenRemove<T>(List<T> list)
        {
            lock (MUTEX)
            {
                if (list == null || list.Count == 0)
                {
                    return default;
                }
                var i = rand.Next(list.Count);
                T item = list[i];
                list.RemoveAt(i);
                return item;
            }
        }

        public static string HtmlDecode(string html)
        {
            return string.IsNullOrWhiteSpace(html)
                ? string.Empty
                : System.Web.HttpUtility.HtmlDecode(html);
        }
        public static bool IsNumeric(string input)
        {
            return Regex.IsMatch(input, "^\\d+$");
        }
        public static string EscapeString(string input)
        {
            return Regex.Replace(StripUnicode(input), "[^a-zA-Z0-9 ]", "");
        }
        public static string StripUnicode(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }
            input = Regex.Replace(input, "(á|à|ả|ã|ạ|ă|ắ|ằ|ẳ|ẵ|ặ|â|ấ|ầ|ẩ|ẫ|ậ|Á|À|Ả|Ã|Ạ|Ă|Ắ|Ằ|Ẳ|Ẵ|Ặ|Â|Ấ|Ầ|Ẩ|Ẫ|Ậ)", "a");
            input = Regex.Replace(input, "(đ|Đ)", "d");
            input = Regex.Replace(input, "(é|è|ẻ|ẽ|ẹ|ê|ế|ề|ể|ễ|ệ|É|È|Ẻ|Ẽ|Ẹ|Ê|Ế|Ề|Ể|Ễ|Ệ)", "e");
            input = Regex.Replace(input, "(í|ì|ỉ|ĩ|ị|Í|Ì|Ỉ|Ĩ|Ị)", "i");
            input = Regex.Replace(input, "(ó|ò|ỏ|õ|ọ|ô|ố|ồ|ổ|ỗ|ộ|ơ|ớ|ờ|ở|ỡ|ợ|Ó|Ò|Ỏ|Õ|Ọ|Ô|Ố|Ồ|Ổ|Ỗ|Ộ|Ơ|Ớ|Ờ|Ở|Ỡ|Ợ|ö)", "o");
            input = Regex.Replace(input, "(ú|ù|ủ|ũ|ụ|ư|ứ|ừ|ử|ữ|ự|Ú|Ù|Ủ|Ũ|Ụ|Ư|Ứ|Ừ|Ử|Ữ|Ự|ü)", "u");
            input = Regex.Replace(input, "(ý|ỳ|ỷ|ỹ|ỵ|Ý|Ỳ|Ỷ|Ỹ|Ỵ)", "y");
            input = Regex.Replace(input, "''ss|'s|'ss|''s|'|''|'x|'xx|''x|''xx", "");
            input = Regex.Replace(input, "\\u0300|\\u0301|\\u0303|\\u0309|\\u0323", "");
            input = Regex.Replace(input, "\\u02C6|\\u0306|\\u031B", "");
            input = Regex.Replace(input, "ç", "c");
            return input;
        }
        public static string GetValueByRegex(string text, string pattern, int groupIndex = 1)
        {
            Match match = Regex.Match(text, pattern);
            return match.Success ? match.Groups[groupIndex].Value : string.Empty;
        }
        public static IEnumerable<string> GetValuesByRegex(string text, string pattern, int groupIndex = 1)
        {
            MatchCollection matches = Regex.Matches(text, pattern);
            if (matches == null || matches.Count == 0)
            {
                return Enumerable.Empty<string>();
            }

            return matches.Cast<Match>().Select(m => m.Groups[groupIndex].Value);
        }

        public static bool IsLiveProxy(string proxy)
        {
            try
            {
                string[] temp = proxy.Split(':', '|').Select(x => x.Trim()).ToArray();
                ProxyClient proxyClient = HttpProxyClient.Parse($"{temp[0]}:{temp[1]}");
                if (temp.Length > 3)
                {
                    proxyClient.Username = temp[2];
                    proxyClient.Password = temp[3];
                }
                var headers = new EzHttpHeader().NormalHeader().Build;
                string json = EzHttpRequest.Instance.Get("https://www.facebook.com/", proxyClient, false, 10000, headers);
                return true;
            }
            catch (HttpException)
            {
            }
            return false;
        }
        public static void DeleteLocalFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }
            try
            {
                File.Delete(path);
            }
            catch
            {
            }
        }
        public static IEnumerable<string> GetFilesInDirectory(string dir)
        {
            return Directory.GetFiles(dir)
                .Where(x => !Path.GetFileName(x).StartsWith("."));
        }

    }
}
