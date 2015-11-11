using ModelPortable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace OnlinerNews2.Infrastructure
{
	public class LocalDataManager: IWriteReadData
	{
		private const string file = "news.dat";

		public async Task WriteDataAsync(List<NewsItem> data)
		{
			var dcs = new DataContractSerializer(typeof(List<NewsItem>));

			using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(file, CreationCollisionOption.ReplaceExisting))
			{
				dcs.WriteObject(stream, data);
			}
		}

		public async Task<ObservableCollection<NewsItem>> ReadDataAsync()
		{
			try
			{
				var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(file);
				DataContractSerializer dcs = new DataContractSerializer(typeof(List<NewsItem>));

				return new ObservableCollection<NewsItem>((IEnumerable<NewsItem>)dcs.ReadObject(stream));
			}
			catch (FileNotFoundException)
			{
				return new ObservableCollection<NewsItem>();
			}

		}
	}
}
