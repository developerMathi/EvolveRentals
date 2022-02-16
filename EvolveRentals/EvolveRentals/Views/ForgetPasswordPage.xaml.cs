﻿using EvolveRentals.Popups;
using EvolveRentalsController;
using EvolveRentalsModel.AccessModels;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EvolveRentals.Views
{
    public partial class ForgetPasswordPage : ContentPage
    {
        GetForgetPasswordMobileRequest forgetPasswordMobileRequest;
        private string token;
        private string email;
        GetForgetPasswordMobileResponse forgetPasswordMobileResponse;
        public ForgetPasswordPage()
        {
            InitializeComponent();
            var assembly = typeof(LoginPage);
            forgetPasswordMobileResponse = null;
            forgetPasswordMobileRequest = new GetForgetPasswordMobileRequest();
            token = Application.Current.Properties["currentToken"].ToString();
            //sendEmailBtn.ImageSource = ImageSource.FromResource("MaxVonGrafKftMobile.Assets.sendEmail.png", assembly);
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
                else
                {
                    await PopupNavigation.Instance.PopAsync();

                }
            }

        }

        protected override bool OnBackButtonPressed()
        {
            if (PopupNavigation.Instance.PopupStack.Count > 0) { return true; }

            // Always return true because this method is not asynchronous.
            // We must handle the action ourselves: see above.
            return false;
        }

        private async void sendEmailBtn_Clicked(object sender, EventArgs e)
        {
            if (!new EmailAddressAttribute().IsValid(emailEntry.Text) || string.IsNullOrEmpty(emailEntry.Text))
            {
                await PopupNavigation.Instance.PushAsync(new Error_popup("Please enter a valid email address"));
            }
            else
            {
                forgetPasswordMobileRequest.email = emailEntry.Text;
                forgetPasswordMobileRequest.clienTime = DateTime.Now;
                forgetPasswordMobileRequest.callBackUrl = "";
                bool busy = false;
                if (!busy)
                {
                    try
                    {
                        busy = true;
                        await PopupNavigation.Instance.PushAsync(new LoadingPopup("Loading.."));
                        await Task.Run(() =>
                        {
                            forgetPasswordMobileResponse = getForgetPasswordMobileRequest(forgetPasswordMobileRequest, token);
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
                    if (forgetPasswordMobileResponse != null)
                    {
                        if (forgetPasswordMobileResponse.message.ErrorCode == "200")
                        {
                            if (forgetPasswordMobileResponse.retval != 0)
                            {
                                email = emailEntry.Text;
                                await PopupNavigation.Instance.PushAsync(new SuccessPopUp("An email has been sent with instructions on how to reset your password", 5, email));
                            }
                        }
                        else
                        {
                            await PopupNavigation.Instance.PushAsync(new ErrorWithClosePagePopup("No profile found for this email ID"));
                        }
                    }
                }
            }
        }

        private GetForgetPasswordMobileResponse getForgetPasswordMobileRequest(GetForgetPasswordMobileRequest forgetPasswordMobileRequest, string token)
        {
            GetForgetPasswordMobileResponse response;
            CustomerController controller = new CustomerController();
            try
            {
                response = controller.getForgetPasswordMobileRequest(forgetPasswordMobileRequest, token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        private void btnBack_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}