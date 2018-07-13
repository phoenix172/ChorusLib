using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChorusLib
{
    public interface IChorusApi
    {
        Task<List<Song>> SearchAsync(SongProps filter, int page = 1);
        Task<List<Song>> SearchAsync(string filter);
    }
}