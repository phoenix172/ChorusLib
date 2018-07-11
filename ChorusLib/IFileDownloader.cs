using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChorusLib
{
    interface IFileDownloader
    {
        Task<string> DownloadFileAsync(string address, string downloadLocation);
    }
}
