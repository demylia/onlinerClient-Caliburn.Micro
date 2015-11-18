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
		private IEnumerable<NewsItem> cashe;
		private string textSearch;
		private bool refresh = true;
		private int progress = 1;

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

		public string TextSearch
        {
            get { return textSearch; }
            set
            {
                textSearch = value;
                NotifyOfPropertyChange(() => TextSearch);
            }
        }

		public int ProgressBar
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
			
			News = await ReadDataAsync();
			if (News.Count == 0)
				RefreshNews();
		}

        public  void Search()
        {
			if (string.IsNullOrEmpty(TextSearch))
				AddingData(cashe);
			else
			{
				var res = cashe.Where(s => s.Title.IndexOf(TextSearch, StringComparison.OrdinalIgnoreCase) >= 0);
				AddingData(res);
			}
        }
		public bool CanRefreshNews
		{
			get { return refresh; }
		}
        public async void RefreshNews()
        {
			Refresh = false;
            await GetNews();
			await WriteDataAsync();
			Refresh = true;
		}

		private async Task GetNews()
		{

			cashe = await dataManager.GetNewsDeserializeAsync();
			if (News == null)
				News = new ObservableCollection<NewsItem>(cashe);
			else
				AddingData(cashe);
		}

		private void AddingData(IEnumerable<NewsItem> collection)
		{
			News.Clear();
			foreach (var item in collection)
				News.Add(item);
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