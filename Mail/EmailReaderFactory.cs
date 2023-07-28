using System;
using System.Collections.Generic;
using System.Text;

namespace HadesAIOCommon.Mail
{
    public static class EmailReaderFactory
    {
        private const int ImapSslPort = 993;
        private const string HotmailImapHost = "outlook.office365.com";
        private const string HotmailImapHost2 = "imap-mail.outlook.com";
        private const string GmxDotComImapHost = "imap.gmx.com";
        private const string MailDotComImapHost = "imap.mail.com";

        public static IEmailReader GetReader(string email, string password)
        {
            var serverInfo = GetServerInfo(email);
            return new ImapReader(serverInfo.Host, serverInfo.Port, email, password);
        }

        private static EmailServerInfoDto GetServerInfo(string email)
        {
            var serverInfo = new EmailServerInfoDto(GetImapHost(email), ImapSslPort, EmailServerType.Imap);
            return serverInfo;
        }

        private static string GetImapHost(string email)
        {
            var temp = email.Split('@');
            if (temp.Length < 2)
            {
                throw new ArgumentException($"Invalid email format: {email}");
            }

            var domain = temp[1];
            if (domain.Contains("gmx."))
            {
                return GmxDotComImapHost;
            }
            return domain == "mail.com" ? MailDotComImapHost : HotmailImapHost2;
        }
    }
}
