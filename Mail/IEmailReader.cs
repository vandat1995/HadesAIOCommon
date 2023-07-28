using MailKit;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HadesAIOCommon.Mail
{
    public interface IEmailReader : IDisposable
    {
        Dictionary<UniqueId, MimeMessage> GetInboxEmails(int limit = 50);
        Dictionary<UniqueId, MimeMessage> GetInboxEmailsFrom(string fromAddress, int limit = 50);
        Dictionary<UniqueId, MimeMessage> GetUnSeenEmailsFrom(string fromAddress, int limit = 10);
        void MarkSeen(UniqueId uniqueId);
    }
}
