using System.Collections.Generic;

namespace HadesAIOCommon.FTP
{
    public interface IFtpClient
    {
        void Download(string remoteFile, string localFile, bool shouldDeleteWhenDownloaded = false);
        void Upload(string remoteFile, string localFile);
        void Delete(string remoteFile);
        IList<string> ListFiles(string directory);
    }
}
