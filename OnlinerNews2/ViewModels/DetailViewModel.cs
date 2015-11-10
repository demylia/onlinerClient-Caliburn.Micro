using Caliburn.Micro;
using ModelPortable;
using OnlinerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace OnlinerServices.ViewModels
{
   public class DetailViewModel: Screen
    {
        private readonly INavigationService navigationService;
        private IDataManager datamanager;
        private string content;
		
        public NewsItem Parameter { get; set; }
        public NewsItem NewsItem { get; set; }

        public DetailViewModel(INavigationService navigationService, IDataManager datamanager)
        {
            this.datamanager = datamanager;
            this.navigationService = navigationService;
        }

		#region Observed Properties
		public string Content
		{
			get { return content; }
			set
			{
				content = value; 
				NotifyOfPropertyChange(() => Content);
			}
		}
		#endregion

		#region Lifecycle
		protected override void OnActivate()
        {
			Content = Parameter.Description;
		}
		#endregion

		#region Watching of an article

		public async void OpenInBrowser()
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(Parameter.Link, UriKind.Absolute));
        }

		public void LoadFullText()
		{
			GetContentOfNewsAsync();
		}

		 private async void GetContentOfNewsAsync()
        {
			Content = await datamanager.GetContentByLinkAsync(Parameter.Link);
			
		}
		#endregion

	}
}
