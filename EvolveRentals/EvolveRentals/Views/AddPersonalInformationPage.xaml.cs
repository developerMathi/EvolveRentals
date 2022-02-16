﻿using EvolveRentals.Popups;
using EvolveRentalsController;
using EvolveRentalsModel;
using EvolveRentalsModel.AccessModels;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EvolveRentals.Views
{
    public partial class AddPersonalInformationPage : ContentPage
    {
        string _token;
        GetAllCountryForMobileResponse countryResponse = null;
        GetAllStateForMobileResponse stateResponse = null;
        private CustomerReview customer;
        public static CustomerImages images;

        CustomerSerach customerSerach;
        List<CustomerSeachResult> customerSeachResults;
        private int fromVal;

        public AddPersonalInformationPage(CustomerReview customer)
        {

            InitializeComponent();
            //var assembly = typeof(LoginPage);
            //NxtBtn.ImageSource = ImageSource.FromResource("MaxVonGrafKftMobile.Assets.nextIcon.png", assembly);
            //BacKBtn.ImageSource = ImageSource.FromResource("MaxVonGrafKftMobile.Assets.backicon.png", assembly);
            //PhotoBtn.ImageSource = ImageSource.FromResource("MaxVonGrafKftMobile.Assets.cameraIcon.png", assembly);


            this.fromVal = 0;

            this.customer = customer;
            images = new CustomerImages();
            customerSerach = new CustomerSerach();
            customerSeachResults = null;

            _token = App.Current.Properties["currentToken"].ToString();

            countryResponse = getAllCountry(_token);
            List<string> countryList = new List<string>();
            if (countryResponse.countryList.Count > 0) { foreach (Country k in countryResponse.countryList) { countryList.Add(k.CountryName); }; }
            countryPicker.ItemsSource = countryList;
            DateOfBithEntry.MaximumDate = DateTime.Now.AddYears(-21);


            if (Constants.admin != null)
            {
                if (Constants.admin.CountryId > 0)
                {
                    if (Constants.countriesHasState.Contains(Constants.admin.CountryId))
                    {
                        statePicker.Title = "State";
                        PostalCodeEntry.Placeholder = "Zip code";
                    };

                    string cliCountry = returnCountryNameByID(Constants.admin.CountryId);
                    if (cliCountry != null)
                    {
                        countryPicker.SelectedItem = cliCountry;
                    }
                }
            }
            else
            {
                countryPicker.SelectedItem = "USA";
            }







            List<string> stateList = new List<string>();
            int? counid = null;
            foreach (Country c in countryResponse.countryList) { if (c.CountryName == countryPicker.SelectedItem.ToString()) { counid = c.CountryId; } };

            if (counid != null)
            {
                GetAllStateForMobileRequest stateRequest = new GetAllStateForMobileRequest();
                stateRequest.CountryID = counid.Value;
                stateResponse = getStates(stateRequest, _token);
                if (stateResponse.stateList.Count > 0) { foreach (State s in stateResponse.stateList) { stateList.Add(s.StateName); }; }
                statePicker.ItemsSource = stateList;
                if (countryPicker.SelectedItem.ToString() == "USA")
                {
                    statePicker.SelectedItem = "GEORGIA";
                }
            }

        }

        public AddPersonalInformationPage(CustomerReview customer, int fromVal) : this(customer)
        {
            this.fromVal = fromVal;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (PopupNavigation.Instance.PopupStack.Count > 0)
            {
                if (PopupNavigation.Instance.PopupStack[PopupNavigation.Instance.PopupStack.Count - 1].GetType() == typeof(ErrorWithClosePagePopup))
                {
                    await PopupNavigation.Instance.PopAllAsync();
                }
            }


            MessagingCenter.Subscribe<AddCustomerPhotoPopup>(this, "profilePhotoAdded", sender =>
            {
                if (images != null)
                {
                    profileImage.Source = ImageSource.FromFile(images.PhysicalPath);
                }

            });

        }


        private GetAllCountryForMobileResponse getAllCountry(string access_token)
        {
            CommonController commonControllerCountry = new CommonController();
            GetAllCountryForMobileResponse countryResponse;
            countryResponse = commonControllerCountry.GetAllCountry(access_token);
            return countryResponse;
        }

        private void PhotoBtn_Clicked(object sender, EventArgs e)
        {
            if (images == null)
            {
                PopupNavigation.Instance.PushAsync(new AddCustomerPhotoPopup());
            }
            else
            {
                PopupNavigation.Instance.PushAsync(new AddCustomerPhotoPopup(images));
            }


        }

        //protected override bool OnBackButtonPressed()
        //{
        //    if (PopupNavigation.Instance.PopupStack.Count > 0) { return true; }

        //    // Always return true because this method is not asynchronous.
        //    // We must handle the action ourselves: see above.
        //    return false;
        //}

        private void BacKBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private void NxtBtn_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(FirstNameEntry.Text))
            {
                PopupNavigation.Instance.PushAsync(new Error_popup("Please enter a first name."));
            }
            else if (string.IsNullOrEmpty(LastNameEntry.Text))
            {
                PopupNavigation.Instance.PushAsync(new Error_popup("Please enter a last name."));
            }
            else if (string.IsNullOrEmpty(ContactNoEntry.Text))
            {
                PopupNavigation.Instance.PushAsync(new Error_popup("Please enter a valid contact number."));
            }
            else if (ContactNoEntry.Text.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "").Length != 10)
            {
                PopupNavigation.Instance.PushAsync(new Error_popup("Please enter a valid contact no."));
            }

            else if (DateOfBithEntry.Date.AddYears(21).AddDays(1) > DateTime.Now || DateOfBithEntry.Date == null)
            {
                PopupNavigation.Instance.PushAsync(new Error_popup("You must be at least 21 years old to reserve a vehicle."));
            }
            else if (string.IsNullOrEmpty(AddressEntry.Text))
            {
                PopupNavigation.Instance.PushAsync(new Error_popup("Please enter your address."));
            }
            else if (string.IsNullOrEmpty(CityEntry.Text))
            {
                PopupNavigation.Instance.PushAsync(new Error_popup("Please enter your city."));
            }
            else if (countryPicker.SelectedIndex == -1)
            {
                PopupNavigation.Instance.PushAsync(new Error_popup("Please select your country"));
            }
            else if (statePicker.SelectedIndex == -1)
            {
                PopupNavigation.Instance.PushAsync(new Error_popup("Please select your state"));
            }
            else if (string.IsNullOrEmpty(PostalCodeEntry.Text))
            {
                PopupNavigation.Instance.PushAsync(new Error_popup("Please enter your zip code."));
            }


            else
            {
                customerSerach.hPhone = ContactNoEntry.Text;
                customerSerach.Active = true;
                CustomerController customerController = new CustomerController();
                try
                {
                    customerSeachResults = customerController.getCustomerByFilter(customerSerach, _token);
                }
                catch (Exception ex)
                {
                    PopupNavigation.Instance.PushAsync(new Error_popup(ex.Message));
                }

                if (customerSeachResults != null)
                {
                    if (customerSeachResults.Count > 0)
                    {
                        ContactNoEntry.Text = null;
                        PopupNavigation.Instance.PushAsync(new Error_popup("Already a customer exist with this phone number. Please check your contact number"));
                    }
                    else
                    {
                        navigateNextPage();
                    }
                }




            }
        }

        private void navigateNextPage()
        {
            customer.FirstName = FirstNameEntry.Text;
            customer.LastName = LastNameEntry.Text;
            customer.DateOfbirth = DateOfBithEntry.Date;
            //if (!string.IsNullOrEmpty(Address2Entry.Text))
            //{
            //    customer.Address1 = AddressEntry.Text + " " + Address2Entry.Text;
            //}
            //else
            //{
            //    customer.Address1 = AddressEntry.Text;
            //}
            customer.Address1 = AddressEntry.Text;

            customer.City = CityEntry.Text;
            customer.CountryId = returnCountryIdByCountryName(countryPicker.SelectedItem.ToString());
            customer.CountryName = countryPicker.SelectedItem.ToString();
            customer.StateId = returnStateIdByStateName(statePicker.SelectedItem.ToString());
            customer.StateName = statePicker.SelectedItem.ToString();
            customer.ZipCode = PostalCodeEntry.Text;
            customer.hPhone = ContactNoEntry.Text.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
            customer.bPhone = ContactNoEntry.Text.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
            customer.cPhone = ContactNoEntry.Text.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
            if (images != null)
            {
                if (images.Base64 != null)
                {
                    //List<CustomerImages> customerIm = new List<CustomerImages>();
                    //customerIm.Add(images);
                    //customer.CustomerImages = customerIm;
                    if (fromVal == 1)
                    {
                        Navigation.PushModalAsync(new OtherInformationPage(customer, images, fromVal));
                    }
                    else
                    {
                        Navigation.PushModalAsync(new OtherInformationPage(customer, images));
                    }

                }
                else
                {
                    images = null;
                    if (fromVal == 1)
                    {
                        Navigation.PushModalAsync(new OtherInformationPage(customer, images, fromVal));
                    }
                    else
                    {
                        Navigation.PushModalAsync(new OtherInformationPage(customer, images));
                    }
                }
            }

            else
            {
                if (fromVal == 1)
                {
                    Navigation.PushModalAsync(new OtherInformationPage(customer, images, fromVal));
                }
                else
                {
                    Navigation.PushModalAsync(new OtherInformationPage(customer, images));
                }
            }

        }

        private int? returnStateIdByStateName(string v)
        {
            int staID = 0;
            foreach (State p in stateResponse.stateList) { if (p.StateName == v) { staID = p.StateID; } }
            return staID;
        }

        private int? returnCountryIdByCountryName(string v)
        {
            int? cntryId = null;
            foreach (Country o in countryResponse.countryList) { if (o.CountryName == v) { cntryId = o.CountryId; } }
            return cntryId.Value;
        }

        private string returnCountryNameByID(int? id)
        {
            string counName = null;
            foreach (Country o in countryResponse.countryList) { if (o.CountryId == id) { counName = o.CountryName; } }
            return counName;
        }

        private void CountryPicker_Unfocused(object sender, FocusEventArgs e)
        {
            if (countryPicker.SelectedIndex != -1)
            {
                string countryName = countryPicker.SelectedItem.ToString();
                List<string> stateList = new List<string>();
                int? counid = null;
                foreach (Country c in countryResponse.countryList) { if (c.CountryName == countryName) { counid = c.CountryId; } };

                if (counid != null)
                {
                    GetAllStateForMobileRequest stateRequest = new GetAllStateForMobileRequest();
                    stateRequest.CountryID = counid.Value;
                    stateResponse = getStates(stateRequest, _token);
                    if (stateResponse.stateList.Count > 0) { foreach (State s in stateResponse.stateList) { stateList.Add(s.StateName); }; }
                    statePicker.ItemsSource = stateList;
                    if (Constants.countriesHasState.Contains((int)counid))
                    {
                        statePicker.Title = "State";
                        PostalCodeEntry.Placeholder = "Zip code";
                    };

                }

            }

        }

        private GetAllStateForMobileResponse getStates(GetAllStateForMobileRequest stateRequest, string token)
        {
            CommonController stateController = new CommonController();
            GetAllStateForMobileResponse sResponse;
            sResponse = stateController.GetAllStateByCountryID(stateRequest, token);
            return sResponse;
        }

        private void DateOfBithEntry_Focused(object sender, FocusEventArgs e)
        {
            DateOfBithEntry.Placeholder = DateOfBithEntry.MaximumDate.ToString("MM/dd/yyyy");
        }

        private void btnBack_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<AddCustomerPhotoPopup>(this, "profilePhotoAdded");
        }

        private void calenderBtn_Clicked(object sender, EventArgs e)
        {
            DateOfBithEntry.Focus();
        }
    }
}