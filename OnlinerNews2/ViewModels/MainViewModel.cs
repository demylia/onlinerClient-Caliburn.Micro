using Caliburn.Micro;
using ModelPortable;
using OnlinerNews2.Infrastructure;
using OnlinerServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using System.Diagnostics;

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
       
        private string rssAdress = "http://section.onliner.by/feed";
        private Tuple<ObservableCollection<NewsItem>, string, FileNames>[] sectionsOfNews; 

        public MainViewModel(INavigationService navigationService, IDataManager dataManager, IWriteReadData localManager)
        {
            this.dataManager = dataManager;
            this.navigationService = navigationService;
			this.locDataManager = localManager;
            News = new ObservableCollection<NewsItem>();
            PeopleNews = new ObservableCollection<NewsItem>();
            
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
                NotifyOfPropertyChange(() => CanSearch);
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
            await LoadPage();
        }

        public bool CanSearch
        {
            get { return refresh; }
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
            foreach (var section in sectionsOfNews)
            {
                var adress = rssAdress.Replace("section", section.Item2);
                await GetNews(section.Item1, adress);
                await WriteDataAsync(section.Item1, section.Item3);
            }
            //await GetNews(News,"http://tech.onliner.by/feed");
            //await GetNews(PeopleNews, "http://people.onliner.by/feed");
            //await WriteDataAsync(News, FileNames.TechNews);
            //await WriteDataAsync(PeopleNews, FileNames.PeopleNews);
            RefreshPage = !RefreshPage;
            ProgressBar = !ProgressBar;
        }
        private async Task LoadPage()
        {
            
            PeopleNews = await ReadDataAsync(FileNames.PeopleNews2);
            News = await ReadDataAsync(FileNames.TechNews4);
            sectionsOfNews = new Tuple<ObservableCollection<NewsItem>, string, FileNames>[]
                                {
                                    new Tuple<ObservableCollection<NewsItem>, string, FileNames>(News, "tech",FileNames.TechNews4 ) ,
                                    new Tuple<ObservableCollection<NewsItem>, string, FileNames>(PeopleNews, "people",FileNames.PeopleNews2)
                                };

            if (News.Count == 0)
                RefreshNews();
            ProgressBar = !ProgressBar;
           

        }

		private  async Task GetNews( ObservableCollection<NewsItem> collection,string adress)
		{
            await Task.Run( async() => cashe = await dataManager.GetNewsDeserializeAsync(adress));
			//if (collection == null)
   //             collection = new ObservableCollection<NewsItem>(cashe);
			//else
                AddingDataFromCasheToCollection(cashe,collection);
		}

        private void AddingDataFromCasheToCollection(IEnumerable<NewsItem> cashe, ObservableCollection<NewsItem> collection)
        {
            Stopwatch st = new Stopwatch();
            st.Start();
            collection.Clear();
            foreach (var item in cashe)
                collection.Add(item);
            st.Stop();
            //var time = st.ElapsedTicks;
            //st.Start();
            //collection.Clear();
            
            //Parallel.ForEach(cashe, (item) => collection.Add(item));// exception- marcshall to UI  
           
           
            //st.Stop();
            //var time2 = st.ElapsedTicks;
            
        }
		#endregion

		#region Write/Read a data on the disk
		private async Task WriteDataAsync(ObservableCollection<NewsItem> collection, FileNames fileName)
        {
    		await locDataManager.WriteDataAsync(collection, fileName);
        }
	
        private async Task<ObservableCollection<NewsItem>> ReadDataAsync(FileNames fileName)
        {
           
			var result = await locDataManager.ReadDataAsync(fileName);
			cashe = (IEnumerable<NewsItem>)result.ToArray().Clone();
			return result;
		}
		#endregion

        
	}
}