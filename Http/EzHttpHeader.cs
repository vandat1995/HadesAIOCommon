﻿using System.Collections.Generic;

namespace HadesAIOCommon.Http
{
    public class EzHttpHeader
    {
        private readonly Dictionary<string, string> headers = new();

        public EzHttpHeader()
        {
            //Add("Upgrade-Insecure-Requests", "1");
        }

        public EzHttpHeader(Dictionary<string, string> others): this()
        {
            foreach (var item in others)
            {
                headers[item.Key] = item.Value;
            }
        }

        public EzHttpHeader UserAgent(string userAgent)
        {
            headers["user-agent"] = userAgent;
            return this;
        }
        public EzHttpHeader Cookie(string cookie)
        {
            headers["cookie"] = cookie;
            return this;
        }
        public string FetchDest
        {
            set => headers["sec-fetch-dest"] = value;
        }
        public string FetchMode
        {
            set => headers["sec-fetch-mode"] = value;
        }
        public string FetchSite
        {
            set => headers["sec-fetch-site"] = value;
        }
        public string FetchUser
        {
            set => headers["sec-fetch-user"] = value;
        }
        public EzHttpHeader Add(string key, string value)
        {
            headers[key] = value;
            return this;
        }
        public EzHttpHeader AjaxHeader()
        {
            FetchDest = "empty";
            FetchMode = "cors";
            FetchSite = "same-origin";
            Add("accept", "*/*");
            return this;
        }
        public EzHttpHeader NormalHeader()
        {
            FetchUser = "?1";
            FetchDest = "document";
            FetchMode = "navigate";
            FetchSite = "none";
            return this;
        }
        public Dictionary<string, string> Build => headers;
    }
}
