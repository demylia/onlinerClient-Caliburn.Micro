using ModelPortable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlinerNews2.Infrastructure
{
	public interface IWriteReadData
	{
		Task WriteDataAsync(ObservableCollection<NewsItem> data, string fileName);
		Task<ObservableCollection<NewsItem>> ReadDataAsync(string fileName);
    }
}
