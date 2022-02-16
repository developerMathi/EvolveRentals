using EvolveRentals.Popups;
using EvolveRentals.Views;
using EvolveRentalsController;
using EvolveRentalsModel;
using EvolveRentalsModel.AccessModels;
using Rg.Plugins.Popup.Services;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace EvolveRentals
{
    public partial class App : Xamarin.Forms.Application
    {
        private ApiToken apiToken;
        private GetClientDetailsForMobileResponse getClientDetailsForMobile;

        public App()
        {
            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
            Xamarin.Forms.Device.SetFlags(new string[] { "MediaElement_Experimental", "Brush_Experimental" });
            InitializeComponent();

            var currentVersion = VersionTracking.CurrentVersion;

            if (!App.Current.Properties.ContainsKey("IsOnborded"))
            {
                App.Current.Properties.Add("IsOnborded", false);
            }


            MainPage = new NavigationPage(new Productor());


        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
