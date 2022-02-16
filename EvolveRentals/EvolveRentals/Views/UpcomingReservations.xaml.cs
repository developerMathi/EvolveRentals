﻿using EvolveRentals.Popups;
using EvolveRentalsController;
using EvolveRentalsModel;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EvolveRentals.Views
{
    public partial class UpcomingReservations : ContentPage
    {
        List<CustomerReservationModel> customerReservationModels;
        int customerId;
        string token;
        List<string> filters;
        public UpcomingReservations()
        {
            InitializeComponent();
            customerId = (int)App.Current.Properties["CustomerId"];
            token = App.Current.Properties["currentToken"].ToString();
            customerReservationModels = null;
            filters = new List<string>();
            filters.Add("Reservation number");
            filters.Add("Check-Out location");
            filters.Add("Check-In location");
            filters.Add("Status");
            filterPicker.ItemsSource = filters;


            var filter_tab = new TapGestureRecognizer();
            filter_tab.Tapped += (s, e) =>
            {
                filterPicker.Focus();
            };
            filterFrame.GestureRecognizers.Add(filter_tab);

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


            bool busy = false;
            if (!busy)
            {
                try
                {
                    busy = true;
                    await PopupNavigation.Instance.PushAsync(new LoadingPopup("Loading upcomig reservations.."));

                    await Task.Run(async () =>
                    {
                        try
                        {
                            customerReservationModels = getReservations(customerId, token);
                        }
                        catch (Exception ex)
                        {
                            await PopupNavigation.Instance.PushAsync(new ErrorWithClosePagePopup(ex.Message));
                        }


                    });
                }
                finally
                {
                    busy = false;
                    if (PopupNavigation.Instance.PopupStack.Count == 1)
                    {
                        await PopupNavigation.Instance.PopAllAsync();
                    }
                    else if (PopupNavigation.Instance.PopupStack.Count > 1)
                    {
                        if (PopupNavigation.Instance.PopupStack[PopupNavigation.Instance.PopupStack.Count - 1].GetType() != typeof(ErrorWithClosePagePopup))
                        {
                            await PopupNavigation.Instance.PopAllAsync();
                        }
                    }

                }

                if(customerReservationModels!= null)
                {
                    if (customerReservationModels.Count > 0)
                    {
                        List<CustomerReservationModel> upreserItemSource = new List<CustomerReservationModel>();
                        for (int i = customerReservationModels.Count - 1; i >= 0; i--)
                        {
                            if (customerReservationModels[i].VehilceId == 0)
                            {
                                customerReservationModels[i].VehicleModel = customerReservationModels[i].Sample + " (Sample)";
                            }
                            upreserItemSource.Add(customerReservationModels[i]);
                        }
                        upcomingReservationList.ItemsSource = upreserItemSource;
                        upcomingReservationList.HeightRequest = upcomingReservationList.RowHeight * customerReservationModels.Count;
                    }
                    else
                    {
                        upcomingReservationList.IsVisible = false;
                        emptyReservation.IsVisible = true;
                    }
                }
                
                else
                {
                    upcomingReservationList.IsVisible = false;
                    emptyReservation.IsVisible = true;
                }
                
            }
        }

        private List<CustomerReservationModel> getReservations(int customerId, string token)
        {
            RegisterController registerController = new RegisterController();
            List<CustomerReservationModel> reservationModels = null;
            try
            {
                reservationModels = registerController.getReservations(customerId, token);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return reservationModels;

        }

        private void HomeBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private void UpcomingReservationList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //CustomerReservationModel reservationModel = upcomingReservationList.SelectedItem as CustomerReservationModel;

            //    Navigation.PushModalAsync(new ViewReservation(reservationModel.ReservationId));

        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            upcomingReservationList.BeginRefresh();

            if (string.IsNullOrWhiteSpace(e.NewTextValue))
            {
                if (customerReservationModels != null)
                {
                    if (customerReservationModels.Count > 0)
                    {
                        List<CustomerReservationModel> reservationItemSource = new List<CustomerReservationModel>();
                        for (int i = customerReservationModels.Count - 1; i >= 0; i--)
                        {
                            reservationItemSource.Add(customerReservationModels[i]);
                        }


                        upcomingReservationList.ItemsSource = reservationItemSource;
                        upcomingReservationList.HeightRequest = upcomingReservationList.RowHeight * customerReservationModels.Count;
                    }
                    else
                    {
                        upcomingReservationList.IsVisible = false;
                        emptyReservation.IsVisible = true;
                    }
                }
            }
            else
            {
                if (customerReservationModels != null)
                {
                    if (customerReservationModels.Count > 0)
                    {
                        List<CustomerReservationModel> reservationItemSource = new List<CustomerReservationModel>();
                        for (int i = customerReservationModels.Count - 1; i >= 0; i--)
                        {
                            reservationItemSource.Add(customerReservationModels[i]);
                        }

                        upcomingReservationList.ItemsSource = reservationItemSource.Where(i => i.ReservationNumber.Contains(e.NewTextValue));
                    }
                    else
                    {
                        upcomingReservationList.IsVisible = false;
                        emptyReservation.IsVisible = true;
                    }
                }
            }


            upcomingReservationList.EndRefresh();
            int rowcounter = 0;
            foreach (var item in upcomingReservationList.ItemsSource)
            {
                rowcounter++;
            }
            upcomingReservationList.HeightRequest = upcomingReservationList.RowHeight * rowcounter;
            //List<CustomerAgreementModel> models = new List<CustomerAgreementModel>();
            //models = myRentalsList.ItemsSource as List<CustomerAgreementModel>;
            //if(models != null)
            //{
            //    if (models.Count > 0)
            //    {
            //        myRentalsList.HeightRequest = 220 * models.Count;
            //    }
            //}
        }

        private void SearchBar_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            upcomingReservationList.BeginRefresh();

            if (string.IsNullOrWhiteSpace(e.NewTextValue))
            {
                if (customerReservationModels != null)
                {
                    if (customerReservationModels.Count > 0)
                    {
                        List<CustomerReservationModel> reservationItemSource = new List<CustomerReservationModel>();
                        for (int i = customerReservationModels.Count - 1; i >= 0; i--)
                        {
                            reservationItemSource.Add(customerReservationModels[i]);
                        }


                        upcomingReservationList.ItemsSource = reservationItemSource;
                        upcomingReservationList.HeightRequest = upcomingReservationList.RowHeight * customerReservationModels.Count;
                    }
                    else
                    {
                        upcomingReservationList.IsVisible = false;
                        emptyReservation.IsVisible = true;
                    }
                }
            }
            else
            {
                if (customerReservationModels != null)
                {
                    if (customerReservationModels.Count > 0)
                    {
                        List<CustomerReservationModel> reservationItemSource = new List<CustomerReservationModel>();
                        for (int i = customerReservationModels.Count - 1; i >= 0; i--)
                        {
                            reservationItemSource.Add(customerReservationModels[i]);
                        }

                        upcomingReservationList.ItemsSource = reservationItemSource.Where(i => i.CheckoutLocation.Contains(e.NewTextValue));
                    }
                    else
                    {
                        upcomingReservationList.IsVisible = false;
                        emptyReservation.IsVisible = true;
                    }
                }
            }


            upcomingReservationList.EndRefresh();
            int rowcounter = 0;
            foreach (var item in upcomingReservationList.ItemsSource)
            {
                rowcounter++;
            }
            upcomingReservationList.HeightRequest = upcomingReservationList.RowHeight * rowcounter;
        }

        private void SearchBar_TextChanged_2(object sender, TextChangedEventArgs e)
        {
            upcomingReservationList.BeginRefresh();

            if (string.IsNullOrWhiteSpace(e.NewTextValue))
            {
                if (customerReservationModels != null)
                {
                    if (customerReservationModels.Count > 0)
                    {
                        List<CustomerReservationModel> reservationItemSource = new List<CustomerReservationModel>();
                        for (int i = customerReservationModels.Count - 1; i >= 0; i--)
                        {
                            reservationItemSource.Add(customerReservationModels[i]);
                        }


                        upcomingReservationList.ItemsSource = reservationItemSource;
                        upcomingReservationList.HeightRequest = upcomingReservationList.RowHeight * customerReservationModels.Count;
                    }
                    else
                    {
                        upcomingReservationList.IsVisible = false;
                        emptyReservation.IsVisible = true;
                    }
                }
            }
            else
            {
                if (customerReservationModels != null)
                {
                    if (customerReservationModels.Count > 0)
                    {
                        List<CustomerReservationModel> reservationItemSource = new List<CustomerReservationModel>();
                        for (int i = customerReservationModels.Count - 1; i >= 0; i--)
                        {
                            reservationItemSource.Add(customerReservationModels[i]);
                        }

                        upcomingReservationList.ItemsSource = reservationItemSource.Where(i => i.CheckinLocation.Contains(e.NewTextValue));
                    }
                    else
                    {
                        upcomingReservationList.IsVisible = false;
                        emptyReservation.IsVisible = true;
                    }
                }
            }


            upcomingReservationList.EndRefresh();
            int rowcounter = 0;
            foreach (var item in upcomingReservationList.ItemsSource)
            {
                rowcounter++;
            }
            upcomingReservationList.HeightRequest = upcomingReservationList.RowHeight * rowcounter;
        }

        private void filterPicker_Unfocused(object sender, FocusEventArgs e)
        {
            if (filterPicker.SelectedItem != null)
            {
                if (filterPicker.SelectedItem.ToString() == "Reservation number")
                {
                    checkoutLocationSearchBar.IsVisible = false;
                    checkinLocationSearchBar.IsVisible = false;
                    statusSearchBar.IsVisible = false;
                    numberSearchBar.IsVisible = true;
                }
                if (filterPicker.SelectedItem.ToString() == "Check-Out location")
                {
                    checkinLocationSearchBar.IsVisible = false;
                    statusSearchBar.IsVisible = false;
                    numberSearchBar.IsVisible = false;
                    checkoutLocationSearchBar.IsVisible = true;

                }
                if (filterPicker.SelectedItem.ToString() == "Check-In location")
                {
                    statusSearchBar.IsVisible = false;
                    numberSearchBar.IsVisible = false;
                    checkoutLocationSearchBar.IsVisible = false;
                    checkinLocationSearchBar.IsVisible = true;
                }
                if (filterPicker.SelectedItem.ToString() == "Status")
                {
                    numberSearchBar.IsVisible = false;
                    checkoutLocationSearchBar.IsVisible = false;
                    checkinLocationSearchBar.IsVisible = false;
                    statusSearchBar.IsVisible = true;
                }
            }
        }

        private void SearchBar_TextChanged_3(object sender, TextChangedEventArgs e)
        {
            upcomingReservationList.BeginRefresh();

            if (string.IsNullOrWhiteSpace(e.NewTextValue))
            {
                if (customerReservationModels != null)
                {
                    if (customerReservationModels.Count > 0)
                    {
                        List<CustomerReservationModel> reservationItemSource = new List<CustomerReservationModel>();
                        for (int i = customerReservationModels.Count - 1; i >= 0; i--)
                        {
                            reservationItemSource.Add(customerReservationModels[i]);
                        }


                        upcomingReservationList.ItemsSource = reservationItemSource;
                        upcomingReservationList.HeightRequest = upcomingReservationList.RowHeight * customerReservationModels.Count;
                    }
                    else
                    {
                        upcomingReservationList.IsVisible = false;
                        emptyReservation.IsVisible = true;
                    }
                }
            }
            else
            {
                if (customerReservationModels != null)
                {
                    if (customerReservationModels.Count > 0)
                    {
                        List<CustomerReservationModel> reservationItemSource = new List<CustomerReservationModel>();
                        for (int i = customerReservationModels.Count - 1; i >= 0; i--)
                        {
                            reservationItemSource.Add(customerReservationModels[i]);
                        }

                        upcomingReservationList.ItemsSource = reservationItemSource.Where(i => i.Status.ToLower().Contains(e.NewTextValue));
                    }
                    else
                    {
                        upcomingReservationList.IsVisible = false;
                        emptyReservation.IsVisible = true;
                    }
                }
            }


            upcomingReservationList.EndRefresh();
            int rowcounter = 0;
            foreach (var item in upcomingReservationList.ItemsSource)
            {
                rowcounter++;
            }
            upcomingReservationList.HeightRequest = upcomingReservationList.RowHeight * rowcounter;
        }

        private void btnBack_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private void filterPicker_Focused(object sender, FocusEventArgs e)
        {
            filterPicker.Focus();
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {

        }

        private void filterButton_Clicked(object sender, EventArgs e)
        {
            filterPicker.Focus();
        }

        private void upcomingReservationList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            upcomingReservationList.SelectedItem = null;
            var reservationModel = e.Item as CustomerReservationModel;
            Navigation.PushModalAsync(new ViewReservation(reservationModel.ReservationId));

        }
    }
}