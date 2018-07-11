using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;

namespace ChorusLib
{
    class WebRequestDownloader : IFileDownloader
    {
        public string DownloadFile(string address, string downloadLocation)
        {
            return DownloadFile(address, downloadLocation, new CookieContainer());
        }

        private string DownloadFile(string address, string downloadLocation, CookieContainer cookieBox)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
            request.AllowAutoRedirect = false;
            request.CookieContainer = cookieBox;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (ExecuteRedirect(downloadLocation, cookieBox, response, out string filePath)) return filePath;

                filePath = GetLocalFilePath(response, downloadLocation);
                using (Stream dataStream = response.GetResponseStream())
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    dataStream.CopyTo(fileStream);
                }

                return filePath;
            }
        }

        private bool ExecuteRedirect(string downloadLocation, CookieContainer cookieBox, HttpWebResponse response, out string filePath)
        {
            filePath = null;
            var locationHeader = response.GetResponseHeader("Location");
            if (locationHeader != string.Empty)
            {
                filePath = DownloadFile(locationHeader, downloadLocation, cookieBox);
                return true;
            }
            return false;
        }

        private string GetLocalFilePath(HttpWebResponse response, string downloadLocation)
        {
            string fileName;
            var contentDispositionString = response.GetResponseHeader("Content-Disposition");
            if (!string.IsNullOrEmpty(contentDispositionString))
            {
                var contentDisposition = new ContentDisposition(contentDispositionString);
                fileName = contentDisposition.FileName;
            }
            else
            {
                fileName = Path.GetFileName(response.ResponseUri.LocalPath);
            }
            return Path.Combine(downloadLocation, fileName);
        }
    }
}
