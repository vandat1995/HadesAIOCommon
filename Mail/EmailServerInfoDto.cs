using System;
using System.Collections.Generic;
using System.Text;

namespace HadesAIOCommon.Mail
{
    public class EmailServerInfoDto
    {
        public EmailServerInfoDto(string host, int port, EmailServerType type)
        {
            Host = host;
            Port = port;
            ServerType = type;
        }

        public string Host { get; set; }
        public int Port { get; set; }
        public EmailServerType ServerType { get; set; }
    }

    public enum EmailServerType
    {
        Imap,
        Pop3
    }
}
