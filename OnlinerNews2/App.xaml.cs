using Caliburn.Micro;
using OnlinerServices.ViewModels;
using OnlinerServices.Views;
using OnlinerServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using OnlinerNews2.Infrastructure;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace OnlinerServices
{
    public sealed partial class App : CaliburnApplication
    {
        private WinRTContainer container;
        

        public App()
        {
            InitializeComponent();
            Initialize();
        }

        protected override void Configure()
        {
            container = new WinRTContainer();
            container.RegisterWinRTServices();
            

            container.PerRequest<MainViewModel>();
            container.PerRequest<DetailViewModel>();

            container.Singleton<IDataManager, DataManagerOnlinerTech>();
			container.Singleton<IWriteReadData, LocalDataManager>();

            MessageBinder.SpecialValues.Add("$clickeditem", c => ((ItemClickEventArgs)c.EventArgs).ClickedItem);

        }
        protected override object GetInstance(Type service, string key)
        {
            var instance = container.GetInstance(service, key);
            if (instance != null)
                return instance;
            throw new Exception("Could not locate any instances.");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }


        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }
        protected override void PrepareViewFirst(Frame rootFrame)
        {
            container.RegisterNavigationService(rootFrame);
        }
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            DisplayRootView<MainView>();
        }
    }
}