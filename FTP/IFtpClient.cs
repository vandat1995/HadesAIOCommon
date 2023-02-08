using System;
using System.Collections.Generic;
using System.Text;

namespace HadesAIOCommon.FTP
{
    public interface IFtpClient
    {
        void Download(string remoteFile, string localFile);
        void Upload(string remoteFile, string localFile);
        void Delete(string remoteFile);
        IList<string> ListFiles(string directory);
    }
}
