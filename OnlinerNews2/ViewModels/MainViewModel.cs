using Caliburn.Micro;
using ModelPortable;
using OnlinerNews2.Infrastructure;
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
		private IWriteReadData locDataManager;
		private ObservableCollection<NewsItem> news;
        private ObservableCollection<NewsItem> peopleNews;
        private IEnumerable<NewsItem> cashe;
		private string textSearch;
		private bool refresh = true;
        private bool progress = true;

        public MainViewModel(INavigationService navigationService, IDataManager dataManager, IWriteReadData localManager)
        {
            this.dataManager = dataManager;
            this.navigationService = navigationService;
			this.locDataManager = localManager;
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

        public ObservableCollection<NewsItem> PeopleNews
        {
            get { return peopleNews; }
            set
            {
                peopleNews = value;
                NotifyOfPropertyChange(() => PeopleNews);
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

		public bool ProgressBar
		{
			get { return progress; }
			set
			{
				progress = value;
				NotifyOfPropertyChange(() => ProgressBar);
			}
		}

		private bool RefreshPage
		{
            get { return refresh; }
			set
			{
				refresh = value;
				NotifyOfPropertyChange(() => CanRefreshNews);
			}
		}
		#endregion

		#region Navigation
		public void GoToDetail(NewsItem item)
		{
			navigationService.NavigateToViewModel<DetailViewModel>(item);
		}
		#endregion

		#region Displaying and searching of the news
		protected override async void OnActivate()
		{
            PeopleNews = await ReadDataAsync();
            News = await ReadDataAsync();
			if (News.Count == 0)
				RefreshNews();
            ProgressBar = !ProgressBar;
		}

        public  void Search()
        {
			if (string.IsNullOrEmpty(TextSearch))
                AddingDataFromCasheToCollection(cashe,News);
			else
			{
				var res = cashe.Where(s => s.Title.IndexOf(TextSearch, StringComparison.OrdinalIgnoreCase) >= 0);
                AddingDataFromCasheToCollection(res,News);
			}
        }
		public bool CanRefreshNews
		{
			get { return refresh; }
		}
        public async void RefreshNews()
        {
            ProgressBar = !ProgressBar;
            RefreshPage = !RefreshPage;
            await GetNews(News,"http://tech.onliner.by/feed");
            await GetNews(PeopleNews, "http://people.onliner.by/feed");
            await WriteDataAsync();
            RefreshPage = !RefreshPage;
            ProgressBar = !ProgressBar;
        }

		private  async Task GetNews(ObservableCollection<NewsItem> collection,string adress)
		{
           // string adress = "http://tech.onliner.by/feed";
            cashe = await dataManager.GetNewsDeserializeAsync(adress);
			if (collection == null)
                collection = new ObservableCollection<NewsItem>(cashe);
			else
                AddingDataFromCasheToCollection(cashe,collection);
		}

		private void AddingDataFromCasheToCollection(IEnumerable<NewsItem> cashe, ObservableCollection<NewsItem> collection)
		{
			collection.Clear();
			foreach (var item in cashe)
				collection.Add(item);
		}
		#endregion

		#region Write/Read a data on the disk
		private async Task WriteDataAsync()
        {
			await locDataManager.WriteDataAsync(News.ToList());
        }
	
        private async Task<ObservableCollection<NewsItem>> ReadDataAsync()
        {
			var result = await locDataManager.ReadDataAsync();
			cashe = (IEnumerable<NewsItem>)result.ToArray().Clone();
			return result;
		}
		#endregion
	}
}