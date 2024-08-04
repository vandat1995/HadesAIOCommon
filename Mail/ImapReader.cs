using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using MailKit;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using MailKit.Net.Proxy;

namespace HadesAIOCommon.Mail
{
    public class ImapReader : IEmailReader
    {
        //private const string AuthFailedMsg = "authentication failed, login failed";
        private const int ImapSslPort = 993;
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);
        public string? Proxy { get; set; }
        public int Maxfetch { get; set; } = 10;

        private readonly int _port;
        private readonly string _host;
        private readonly string _email;
        private readonly string _password;

        private readonly ImapClient _imapClient = new();

        public ImapReader(string host, int port, string email, string password)
        {
            _host = host;
            _port = port;
            _email = email;
            _password = password;
        }

        public Dictionary<UniqueId, MimeMessage> GetInboxEmails(int limit = 50)
        {
            Maxfetch = limit;
            return GetEmails(SearchQuery.All);
        }

        public Dictionary<UniqueId, MimeMessage> GetInboxEmailsFrom(string fromAddress, int limit = 50)
        {
            Maxfetch = limit;
            return GetEmails(SearchQuery.FromContains(fromAddress));
        }

        public Dictionary<UniqueId, MimeMessage> GetUnSeenEmailsFrom(string fromAddress, int limit = 10)
        {
            Maxfetch = limit;
            return GetEmails(SearchQuery.NotSeen.And(SearchQuery.FromContains(fromAddress)));
        }

        public void MarkSeen(UniqueId uniqueId)
        {
            try
            {
                OpenInbox();
                _imapClient.Inbox.SetFlags(uniqueId, MessageFlags.Seen, false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private Dictionary<UniqueId, MimeMessage> GetEmails(SearchQuery query)
        {
            OpenInbox();
            var list = new Dictionary<UniqueId, MimeMessage>();
            var uniqueIds = _imapClient.Inbox.Search(query);
            var max = uniqueIds.Count >= Maxfetch ? Maxfetch : uniqueIds.Count;
            for (var i = uniqueIds.Count - 1; i >= uniqueIds.Count - max; i--)
            {
                var uid = uniqueIds[i];
                var msg = _imapClient.Inbox.GetMessage(uid);
                list[uid] = msg;
            }

            return list;
        }

        private void OpenInbox()
        {
            if (!_imapClient.IsConnected)
            {
                InitConnection();
            }

            if (!_imapClient.Inbox.IsOpen)
            {
                _imapClient.Inbox.Open(FolderAccess.ReadWrite);
            }
        }

        private void InitConnection()
        {
            if (_imapClient.IsConnected)
            {
                return;
            }

            var secure = _port == ImapSslPort ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.None;
            if (!string.IsNullOrWhiteSpace(Proxy))
            {
                var temp = Proxy.Split(':').Select(x => x.Trim()).ToArray();
                _imapClient.ProxyClient = new HttpProxyClient(temp[0], Convert.ToInt32(temp[1]));
            }

            _imapClient.Connect(_host, _port, secure);
            _imapClient.Timeout = (int)Timeout.TotalMilliseconds;
            _imapClient.Authenticate(_email, _password);
        }

        public void Dispose()
        {
            try
            {
                _imapClient.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static string GetSenderEmailAddress(InternetAddressList vars)
        {
            foreach (var var in vars.Mailboxes)
            {
                return var.Address;
            }

            return string.Empty;
        }

        public static string GetSenderName(InternetAddressList vars)
        {
            foreach (var var in vars.Mailboxes)
            {
                return var.Name;
            }

            return string.Empty;
        }
    }
}
