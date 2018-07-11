using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChorusLib
{
    public interface IChorusApi
    {
        Task<List<Song>> SearchAsync(SongProps filter);
        Task<List<Song>> SearchAsync(string filter);
    }
}