using HadesAIOCommon.Http;
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
            var topt = new Totp(Base32Encoding.ToBytes(salt));
            if (topt.RemainingSeconds() < 2)
            {
                Thread.Sleep(2000);
                return Get2FACode(salt);
            }
            return topt.ComputeTotp();
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
    }
}
