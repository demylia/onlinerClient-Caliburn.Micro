using ModelPortable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace OnlinerServices
{
    public interface IDataManager
    {
        
        Task<IEnumerable<NewsItem>> GetNewsAsync(string adress);
        Task<string> GetContentByLinkAsync(string link);
    }
}
