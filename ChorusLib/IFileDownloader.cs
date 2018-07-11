using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChorusLib
{
    interface IFileDownloader
    {
        string DownloadFile(string address, string downloadLocation);
    }
}
