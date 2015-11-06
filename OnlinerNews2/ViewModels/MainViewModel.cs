using Caliburn.Micro;
using ModelPortable;
using OnlinerServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace OnlinerServices.ViewModels
{
    public class MainViewModel : Screen
    {
        private readonly INavigationService navigationService;
        private IDataManager dataManager;
		private ObservableCollection<NewsItem> news;
		private string textSearch;

        public MainViewModel(INavigationService navigationService, IDataManager dataManager)
        {
            this.dataManager = dataManager;
            this.navigationService = navigationService;
         }

		#region Observed Properties
		public ObservableCollection<NewsItem> News
        {
            get { return news; }
            set
            {
                news = value;
                NotifyOfPropertyChange(() => News);
            }
        }

        public string TextSearch
        {
            get { return textSearch; }
            set
            {
                textSearch = value;
                NotifyOfPropertyChange(() => TextSearch);
            }
        }
		#endregion

		#region Navigation
		public void GoToDetail(NewsItem item)
		{
			navigationService.NavigateToViewModel<DetailViewModel>(item);
		}
		#endregion

		#region Displaying and searching of  the news
		protected override async void OnActivate()
		{
			News = await ReadDataAsync();
			if (News == null)
			{
				GetNews();
				await WriteDataAsync();
			}
		}

        public async void Search()
        {
            if (string.IsNullOrEmpty(TextSearch))
            {
              News = await  ReadDataAsync();
            }
            else
            {
                var res = await ReadDataAsync();
                News = new ObservableCollection<NewsItem>(res.Where(s => s.Title.IndexOf(TextSearch,StringComparison.OrdinalIgnoreCase) >= 0));
            }
        }

        public void RefreshNews()
        {
            GetNews();
        }

		private async void GetNews()
		{
			News = new ObservableCollection<NewsItem>(await dataManager.GetNewsAsync());
		}
		#endregion

		#region Write/Read a data on the disk
		private async Task WriteDataAsync()
        {
            var dcs = new DataContractSerializer(typeof(List<NewsItem>));

            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync("news.dat", CreationCollisionOption.ReplaceExisting))
            {
                dcs.WriteObject(stream, News);
            }
        }

        private async Task<ObservableCollection<NewsItem>> ReadDataAsync()
        {
            
            var myStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("news.dat");
            DataContractSerializer dcs = new DataContractSerializer(typeof(List<NewsItem>));

           return new ObservableCollection<NewsItem>((IEnumerable<NewsItem>)dcs.ReadObject(myStream));
        }
		#endregion
	}
}