using EvolveRentalsModel;
using EvolveRentalsModel.AccessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EvolveRentals.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IntialLoading : ContentPage
    {
        private ApiToken apiToken;
        private GetClientDetailsForMobileResponse getClientDetailsForMobile;
        string _token;
        bool IsUpdatesAvailable;
        string currentVersion;

        public IntialLoading()
        {
            InitializeComponent();

            if (!App.Current.Properties.ContainsKey("CustomerId"))
            {
                App.Current.Properties.Add("CustomerId", 0);
            }

            //if (!App.Current.Properties.ContainsKey("InquiryID"))
            //{
            //    App.Current.Properties.Add("InquiryID", 0);
            //}
            currentVersion = VersionTracking.CurrentVersion;
           
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var list = new List<SlidePageItems>
            {
               new SlidePageItems(){ id=0,label= "Rent. Drive. Zero Footprint. The first and only 100% carbon neutral car rental company! Rent now in Las Vegas!",imageMobile="ms-appx:///miroshnichenko.mp4",imageTap="tesla1.png",Skiplabel="Skip"},
                 new SlidePageItems(){ id=1,label="EVolve your experience. Focus on the journey and let the electric car get you there.",imageMobile="ms-appx:///luxury.mp4",imageTap="tesla2.png", Skiplabel="Skip"},
                   new SlidePageItems(){ id=2,label= "Your Tesla car experience is ready to drive.",imageMobile="ms-appx:///Yacht.mp4",imageTap="tesla4.png", Skiplabel="Next"}
            };
            TheCarousel.ItemsSource = list;

        }

        private void skipLabelTap_Tapped(object sender, EventArgs e)
        { 
            var obj = (Button)sender;
            SlidePageItems tappedPage = obj.BindingContext as SlidePageItems;
            if (tappedPage.id == 2)
            {
                Navigation.PushAsync(new LoginPage());
            }
            else
            {
                //TheCarousel.ScrollTo(2);
                Navigation.PushAsync(new LoginPage());
            }
        }

        private void TheCarousel_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            //int w = TheCarousel.Position;
            //DisplayAlert("sdad", w.ToString(), "wdwd");
        }
    }

    public class SlidePageItems
    {
        public int id { get; set; }
        public string label { get; set; }
        public string Skiplabel { get; set; }
        public string imageMobile { get; set; }
        public string imageTap { get; set; }
    }
}