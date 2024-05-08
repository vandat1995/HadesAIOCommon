using HadesAIOCommon.Constants;
using Leaf.xNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace HadesAIOCommon.Http
{
    public class EzHttpRequest
    {
        public class ContentType
        {
            public const string FORM_URLENCODED = "application/x-www-form-urlencoded";
            public const string FORM_URLENCODED_UTF8 = "application/x-www-form-urlencoded; charset=UTF-8";
            public const string CONTENT_TYPE_JSON = "application/json";
            public const string CONTENT_TYPE_JSON_UTF8 = "application/json; charset=UTF-8";
        }

        private const int DEFAULT_TIMEOUT = 60000;
        private EzHttpRequest() { }
        private static readonly EzHttpRequest instance = new();
        public static EzHttpRequest Instance => instance;

        public string Get(string url, Dictionary<string, string>? headers = null, int timeout = 0)
        {
            using var request = new HttpRequest();
            request.IgnoreProtocolErrors = true;
            request.AllowAutoRedirect = true;
            request.ConnectTimeout = DEFAULT_TIMEOUT;
            request.UserAgent = HadesAIOConst.DEFAULT_CHROME_USER_AGENT;
            if (timeout != 0)
            {
                request.ConnectTimeout = timeout;
            }
            if (headers != null)
            {
                foreach (var h in headers)
                {
                    request.AddHeader(h.Key, h.Value);
                }
            }
            return request.Get(url).ToString();
        }
        public string Get(string url, string? proxy, Dictionary<string, string>? headers, string ua = "")
        {
            ProxyClient? proxyClient = null;
            if (!string.IsNullOrWhiteSpace(proxy))
            {
                proxyClient = HttpProxyClient.Parse(proxy);
            }
            return Get(url, proxyClient, false, 0, headers, ua, true);
        }
        public string Get(string url, ProxyClient? proxy, bool ignoreError = false, int timeout = 0,
            Dictionary<string, string>? headers = null, string ua = "", bool redirect = false)
        {
            using var request = new HttpRequest();
            request.UserAgent = !string.IsNullOrWhiteSpace(ua) ? ua : HadesAIOConst.DEFAULT_CHROME_USER_AGENT;
            request.ConnectTimeout = timeout != 0 ? timeout : DEFAULT_TIMEOUT;
            request.AllowAutoRedirect = redirect;
            request.IgnoreProtocolErrors = ignoreError;
            if (headers != null)
            {
                foreach (var h in headers)
                {
                    request.AddHeader(h.Key, h.Value);
                }
            }

            if (proxy != null)
            {
                request.Proxy = proxy;
            }
            var response = request.Get(url);
            return response.ToString();
        }

        public CookieStorage GetCookie(string url, ProxyClient? proxy = null)
        {
            using var request = new HttpRequest();
            request.IgnoreProtocolErrors = true;
            request.AllowAutoRedirect = true;
            request.ConnectTimeout = DEFAULT_TIMEOUT;
            request.UserAgent = HadesAIOConst.DEFAULT_CHROME_USER_AGENT;
            if (proxy != null)
            {
                request.Proxy = proxy;
            }
            HttpResponse response = request.Get(url);
            return response.Cookies;
        }

        public string Post(string URL)
        {
            using var request = new HttpRequest();
            request.AllowAutoRedirect = true;
            request.ConnectTimeout = DEFAULT_TIMEOUT;
            request.UserAgent = HadesAIOConst.DEFAULT_CHROME_USER_AGENT;
            return request.Post(URL).ToString();
        }
        public string Post(string url, string body, string contentType,
            Dictionary<string, string>? headers = null, ProxyClient? proxy = null,
            bool ignoreError = true, string ua = "")
        {
            using var request = new HttpRequest();
            request.IgnoreProtocolErrors = ignoreError;
            request.AllowAutoRedirect = true;
            request.ConnectTimeout = DEFAULT_TIMEOUT;
            request.UserAgent = ua != string.Empty ? ua : HadesAIOConst.DEFAULT_CHROME_USER_AGENT;
            if (proxy != null)
            {
                request.Proxy = proxy;
            }
            if (headers != null)
            {
                foreach (var h in headers)
                {
                    request.AddHeader(h.Key, h.Value);
                }
            }
            return request.Post(url, body, contentType).ToString();
        }
        public string Post(string url, MultipartContent content,
            Dictionary<string, string>? headers = null,
            ProxyClient? proxy = null, string ua = "")
        {
            using var request = new HttpRequest();
            request.IgnoreProtocolErrors = true;
            request.AllowAutoRedirect = true;
            request.UserAgent = ua != string.Empty ? ua : HadesAIOConst.DEFAULT_CHROME_USER_AGENT;
            if (proxy != null)
            {
                request.Proxy = proxy;
            }
            if (headers != null)
            {
                foreach (var h in headers)
                {
                    request.AddHeader(h.Key, h.Value);
                }
            }
            return request.Post(url, content).ToString();
        }

        public byte[] Download(string url)
        {
            using var request = new HttpRequest();
            return request.Get(url).ToBytes();
        }
        public void Download(string url, string localPath, string ua = "")
        {
            using var request = new HttpRequest();
            request.IgnoreProtocolErrors = true;
            request.ConnectTimeout = 1000 * 60 * 5;
            request.UserAgent = ua != string.Empty ? ua : HadesAIOConst.DEFAULT_CHROME_USER_AGENT;
            request.Get(url).ToFile(localPath);
        }

        public static ProxyClient? ParseProxyClient(string? proxy)
        {
            if (string.IsNullOrWhiteSpace(proxy))
            {
                return null;
            }
            ProxyClient? proxyClient = null;
            var temp = proxy.Split('|', ':');
            if (temp.Length >= 2)
            {
                proxyClient = HttpProxyClient.Parse($"{temp[0]}:{temp[1]}");
                if (temp.Length >= 4)
                {
                    proxyClient.Username = temp[2];
                    proxyClient.Password = temp[3];
                }
            }
            return proxyClient;
        }

    }
}
