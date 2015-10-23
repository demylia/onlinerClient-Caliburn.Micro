using Caliburn.Micro;
using ModelPortable;
using OnlinerServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlinerNews2.ViewModels
{
    public class MainViewModel : Screen
    {
        private readonly INavigationService _navigationService;
        private ObservableCollection<NewsItem> news;
        private IDataManager _dataManager;

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

        //Methods
        public void GoToDetail(NewsItem item)
        {
            _navigationService.NavigateToViewModel<DetailViewModel>(item);
       
        }

        protected override async void OnActivate()
        {
            string adress = "http://tech.onliner.by/feed";
            News = new ObservableCollection<NewsItem>(await _dataManager.GetNewsAsync(adress));
        }
        
    }
}