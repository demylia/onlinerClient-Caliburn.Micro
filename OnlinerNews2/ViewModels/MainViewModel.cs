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
        private readonly INavigationService _navigationService;
      
        private ObservableCollection<NewsItem> news;
        private IDataManager _dataManager;
        private string textSearch;

        public MainViewModel(INavigationService navigationService, IDataManager dataManager)
        {
            
            this._dataManager = dataManager;
            this._navigationService = navigationService;
            
        }

        //Observed Properties
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

        //Methods
        protected override async void OnActivate()
        {
            News = await ReadDataAsync();
            if (News == null)
            {
                GetNews();
                await WriteDataAsync();
            }
           
           
        }
      
        public void GoToDetail(NewsItem item)
        {
            _navigationService.NavigateToViewModel<DetailViewModel>(item);
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
           // string adress = "http://tech.onliner.by/feed";
          //  News = new ObservableCollection<NewsItem>(await _dataManager.GetNewsAsync(adress));
            News = new ObservableCollection<NewsItem>(await _dataManager.GetNewsDeserializeAsync());

        }
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

       
    }
}