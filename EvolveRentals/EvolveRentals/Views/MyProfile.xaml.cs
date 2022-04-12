﻿using EvolveRentals.Popups;
using EvolveRentalsController;
using EvolveRentalsModel;
using EvolveRentalsModel.AccessModels;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EvolveRentals.Views
{
    public partial class MyProfile : ContentPage
    {
        CustomerReview customerReview;
        GetMobileCustomerByIDResponse getMobileCustomerResponse;
        GetMobileCustomerByIDRequest getMobileCustomerByIDRequest;
        GetCustomerPortalDetailsMobileRequest portalDetailsMobileRequest;
        GetCustomerPortalDetailsMobileResponse PortalDetailsMobileResponse;
        GetCustomerAgreementReservationCountRequest GetCustomerAgreementReservationCountRequest;
        GetCustomerAgreementReservationCountResponse reservationCountResponse;

        string token;
        int customerId;
        CustomerController customoerController;

        public MyProfile()
        {
            InitializeComponent();
            customoerController = new CustomerController();
            token = Application.Current.Properties["currentToken"].ToString();
            customerId = (int)Application.Current.Properties["CustomerId"];
            getMobileCustomerByIDRequest = new GetMobileCustomerByIDRequest();
            getMobileCustomerByIDRequest.CustomerId = customerId;
            getMobileCustomerResponse = null;
            portalDetailsMobileRequest = new GetCustomerPortalDetailsMobileRequest();
            portalDetailsMobileRequest.customerId = customerId;
            PortalDetailsMobileResponse = null;
            GetCustomerAgreementReservationCountRequest = new GetCustomerAgreementReservationCountRequest();
            GetCustomerAgreementReservationCountRequest.customerID = customerId;
            GetCustomerAgreementReservationCountRequest.clientId = Constants.ClientId;

        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();
            bool canRun = true;


            if (PopupNavigation.Instance.PopupStack.Count > 0)
            {
                if ((PopupNavigation.Instance.PopupStack[PopupNavigation.Instance.PopupStack.Count - 1] is editPrrofilePhotoPage || PopupNavigation.Instance.PopupStack[PopupNavigation.Instance.PopupStack.Count - 1] is AddCustomerPhotoPopup || PopupNavigation.Instance.PopupStack[PopupNavigation.Instance.PopupStack.Count - 1] is VehicleImagePopup) && Navigation.ModalStack[Navigation.ModalStack.Count-1] is UpdateProfile)
                {
                    canRun = false;
                }
                else
                {
                    await PopupNavigation.Instance.PopAllAsync();
                }
               

            }

            if (canRun)
            {
                bool busy = false;
                if (!busy)
                {
                    try
                    {
                        busy = true;
                        await PopupNavigation.Instance.PushAsync(new LoadingPopup("Loading profile..."));

                        await Task.Run(async () =>
                        {
                            try
                            {
                                //customerReview = getCustomerById(customerId, token);
                                getMobileCustomerResponse = getMobileCustomerById(getMobileCustomerByIDRequest, token);
                                PortalDetailsMobileResponse = getCustomerDetailsWithProfilePic(portalDetailsMobileRequest, token);
                                reservationCountResponse = GetCustomerAgreementReservationCount(GetCustomerAgreementReservationCountRequest, token);
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
                        customerReview = getMobileCustomerResponse.customerDetails;
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
                    if (getMobileCustomerResponse.message.ErrorCode == "200")
                    {
                        if (customerReview != null)
                        {
                            firstNameLabel.Text = customerReview.FirstName;
                            LastNameLabel.Text = customerReview.LastName;
                            dobLabel.Text = string.Format("{0:MMMM d, yyyy}", customerReview.DateOfbirth);
                            addressLabel.Text = customerReview.Address1 + customerReview.Address2 + ", " + customerReview.City + ", " + customerReview.ZipCode + ", " + customerReview.CountryName;
                            MobileNoLabel.Text = !string.IsNullOrEmpty(customerReview.hPhone) ? String.Format("{0:(###) ###-####}", long.Parse(customerReview.hPhone)) : "";
                            EmailLabel.Text = customerReview.Email;

                            if (PortalDetailsMobileResponse != null)
                            {
                                if (PortalDetailsMobileResponse.customerReview != null)
                                {
                                    if (PortalDetailsMobileResponse.customerReview.CustomerImages.Count > 0)
                                    {
                                        if (PortalDetailsMobileResponse.customerReview.CustomerImages[PortalDetailsMobileResponse.customerReview.CustomerImages.Count - 1].Base64 != null)
                                        {
                                            byte[] Base64Stream = Convert.FromBase64String(PortalDetailsMobileResponse.customerReview.CustomerImages[PortalDetailsMobileResponse.customerReview.CustomerImages.Count - 1].Base64);
                                            profileImage.Source = ImageSource.FromStream(() => new MemoryStream(Base64Stream));
                                        }
                                    }

                                }
                            }
                        }
                        if (reservationCountResponse != null)
                        {
                            //totalReservationNoLabel.Text = reservationCountResponse.custTotcount.totalReservationCount.ToString();
                            totalagreementNoLabel.Text = reservationCountResponse.custTotcount.totalAgreementCount.ToString();
                        }
                        else
                        {
                            await PopupNavigation.Instance.PushAsync(new ErrorWithClosePagePopup(reservationCountResponse.message.ErrorMessage + " Error in getting Toatal reservation and agreement count"));
                        }
                    }

                    else
                    {
                        await PopupNavigation.Instance.PushAsync(new ErrorWithClosePagePopup(getMobileCustomerResponse.message.ErrorMessage));
                    }

                }
            }
                
        }


        private GetCustomerAgreementReservationCountResponse GetCustomerAgreementReservationCount(GetCustomerAgreementReservationCountRequest getCustomerAgreementReservationCountRequest, string token)
        {
            GetCustomerAgreementReservationCountResponse response = new GetCustomerAgreementReservationCountResponse();
            try
            {
                response = customoerController.GetCustomerAgreementReservationCount(getCustomerAgreementReservationCountRequest, token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        private GetCustomerPortalDetailsMobileResponse getCustomerDetailsWithProfilePic(GetCustomerPortalDetailsMobileRequest portalDetailsMobileRequest, string token)
        {
            GetCustomerPortalDetailsMobileResponse response = new GetCustomerPortalDetailsMobileResponse();
            try
            {
                response = customoerController.getCustomerDetailsWithProfilePic(portalDetailsMobileRequest, token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        private GetMobileCustomerByIDResponse getMobileCustomerById(GetMobileCustomerByIDRequest getMobileCustomerByIDRequest, string token)
        {
            GetMobileCustomerByIDResponse response = new GetMobileCustomerByIDResponse();
            try
            {
                response = customoerController.getMobileCustomerById(getMobileCustomerByIDRequest, token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        //private CustomerReview getCustomerById(int customerId, string token)
        //{
        //    CustomerReview customerReview = new CustomerReview();
        //    try
        //    {
        //        customerReview = customoerController.getCustomerById(customerId, token);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return customerReview;
        //}

        private void UpdateBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new UpdateProfile(customerReview));
        }

        private void ContactUsBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new ContactUs());
        }

        private void LogoutBtn_Clicked(object sender, EventArgs e)
        {
            App.Current.Properties["CustomerId"] = 0;
            Navigation.PushModalAsync(new WelcomPage());
        }

        void changePwdBtn_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushModalAsync(new changePasswordPage());
        }

        void btnBack_Clicked(System.Object sender, System.EventArgs e)
        {
            if (Navigation.ModalStack.Count > 0)
            {
                Navigation.PopModalAsync();
            }
            else
            {
                var pageOne = new HomePage();
                NavigationPage.SetHasNavigationBar(pageOne, false);
                NavigationPage mypage = new NavigationPage(pageOne);
                Application.Current.MainPage = mypage;
            }
        }
    }
}