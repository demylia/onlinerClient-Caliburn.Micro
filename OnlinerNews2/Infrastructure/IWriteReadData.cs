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
		Task WriteDataAsync(ObservableCollection<NewsItem> data, FileNames fileName);
		Task<ObservableCollection<NewsItem>> ReadDataAsync(FileNames fileName);
    }
    public enum FileNames
    {
        TechNews4,
        PeopleNews2
    }
}
