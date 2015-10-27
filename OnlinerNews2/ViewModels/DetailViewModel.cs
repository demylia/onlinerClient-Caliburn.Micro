using Caliburn.Micro;
using ModelPortable;
using OnlinerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnlinerNews2.ViewModels
{
   public class DetailViewModel: Screen
    {
        private readonly INavigationService navigationService;
        private IDataManager datamanager;
        
        string title;
        string link;
        string imagePath;
        string content;

        public NewsItem Parameter { get; set; }
        public NewsItem NewsItem { get; set; }

        public DetailViewModel(INavigationService navigationService, IDataManager datamanager)
        {
            this.datamanager = datamanager;
            this.navigationService = navigationService;
        }

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }

        public string Link
        {
            get { return link; }
            set
            {
                link = value;
                NotifyOfPropertyChange(() => Link);
            }
        }

        public string ImagePath
        {
            get { return imagePath; }
            set
            {
                imagePath = value;
                NotifyOfPropertyChange(() => ImagePath);
            }
        }

        public string Content
        {
            get { return content; }
            set
            {
                content = value;
                NotifyOfPropertyChange(() => Content);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected override void OnActivate()
        {
            Title = Parameter.Title;
            ImagePath = Parameter.ImagePath;
            Link = Parameter.Link;
            GetContentOfNewsAsync();
        }

        private async void GetContentOfNewsAsync()
        {
            Content = await datamanager.GetContentByLinkAsync(Link);
        }
        public async void OpenInBrowser()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(Link, UriKind.Absolute));
        }
        
    }
}
