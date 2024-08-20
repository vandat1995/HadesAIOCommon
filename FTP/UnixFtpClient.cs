using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace HadesAIOCommon.FTP
{
    public class UnixFtpClient : IFtpClient
    {
        private readonly string address;
        private readonly NetworkCredential credential;
        private readonly int bufferSize = 2048;
        public UnixFtpClient(string host, int port, string user, string password)
        {
            address = $"ftp://{host}";
            if (port > 0 && port != 21)
            {
                address += $":{port}";
            }
            credential = new NetworkCredential(user, password);
        }
        public IList<string> ListFiles(string directory)
        {
            /* Create an FTP Request */
            FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(address + "/" + directory);
            ftpRequest.Credentials = credential;
            ftpRequest.UseBinary = true;
            ftpRequest.UsePassive = true;
            ftpRequest.KeepAlive = false;
            /* Specify the Type of FTP Request */
            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
            Stream ftpStream = ftpResponse.GetResponseStream();
            StreamReader ftpReader = new StreamReader(ftpStream, Encoding.UTF8);
            List<string> lines = new List<string>();
            string l;
            while ((l = ftpReader.ReadLine()) != null)
            {
                lines.Add(l);
            }
            /* Resource Cleanup */
            ftpReader.Close();
            ftpStream.Close();
            ftpResponse.Close();

            Regex directoryListingRegex = new(
                @"^([d-])((?:[rwxt-]{3}){3})\s+\d{1,}\s+.*?(\d{1,})\s+(\w+)\s+(\d{1,2})\s+(\d{4})?(\d{1,2}:\d{2})?\s+(.+?)\s?$",
                RegexOptions.Compiled | RegexOptions.Multiline |
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            List<string> directoryList = lines
                .Where(line => line[0] != 'd')
                .Select(line =>
                {
                    Match match = directoryListingRegex.Match(line);
                    if (match.Success)
                    {
                        return match.Groups[8].Value;
                    }
                    return string.Empty;
                })
                .Where(line => line != string.Empty)
                .ToList();

            return directoryList;
        }

        public void Download(string remoteFile, string localFile, bool shouldDeleteWhenDownloaded = false)
        {
            try
            {
                string remotePath = address + "/" + UrlEncode(remoteFile);
                using var webClient = new WebClient();
                webClient.Credentials = credential;
                webClient.DownloadFile(remotePath, localFile);
                if (shouldDeleteWhenDownloaded)
                {
                    Delete(remoteFile);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"FileName: [{remoteFile}]. {e.Message}");
            }
        }

        public void Upload(string remoteFile, string localFile)
        {
            string remotePath = address + "/" + UrlEncode(remoteFile);
            FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(remotePath);
            ftpRequest.Credentials = credential;
            ftpRequest.UseBinary = true;
            ftpRequest.UsePassive = true;
            ftpRequest.KeepAlive = false;
            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
            Stream ftpStream = ftpRequest.GetRequestStream();
            FileStream localFileStream = new FileStream(localFile, FileMode.Create);
            byte[] byteBuffer = new byte[bufferSize];
            int bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
            /* Upload the File by Sending the Buffered Data Until the Transfer is Complete */
            while (bytesSent != 0)
            {
                ftpStream.Write(byteBuffer, 0, bytesSent);
                bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
            }
            /* Resource Cleanup */
            localFileStream.Close();
            ftpStream.Close();
        }

        public void Delete(string remoteFile)
        {
            try
            {
                string remotePath = address + "/" + UrlEncode(remoteFile);
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(remotePath);
                ftpRequest.Credentials = credential;
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = false;
                ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                ftpResponse.Close();
            }
            catch
            {
            }
        }

        private static string UrlEncode(string str)
        {
            return Uri.EscapeDataString(str);
        }
    }
}
