using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ChorusLib
{
    class WebRequestDownloader : IFileDownloader
    {
        public Task<string> DownloadFileAsync(string address, string downloadLocation)
        {
            return DownloadFileAsync(address, downloadLocation, new CookieContainer());
        }

        private async Task<string> DownloadFileAsync(string address, string downloadLocation, CookieContainer cookieBox)
        {
            HttpWebRequest request = WebRequest.CreateHttp(address);
            
            request.AllowAutoRedirect = false;
            request.CookieContainer = cookieBox;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            {
                string filePath = await ExecuteRedirectAsync(downloadLocation, cookieBox, response);
                if (filePath != null) return filePath;

                filePath = GetLocalFilePath(response, downloadLocation);
                using (Stream dataStream = response.GetResponseStream())
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await dataStream.CopyToAsync(fileStream);
                }

                return filePath;
            }
        }

        private async Task<string> ExecuteRedirectAsync(string downloadLocation,
            CookieContainer cookieBox, HttpWebResponse response)
        {
            var redirectLocation = GetRedirectLocationFromHeader(response);

            if (redirectLocation != null)
            {
                return await DownloadFileAsync(redirectLocation, downloadLocation, cookieBox);
            }
            return null;
        }

        
        private static string GetRedirectLocationFromHeader(HttpWebResponse response)
        {
            var header = response.GetResponseHeader("Location");
            return header == string.Empty ? null : header;
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
