﻿using EvolveRentals.Popups;
using EvolveRentals.Utilties;
using EvolveRentals.ViewModels;
using EvolveRentalsController;
using EvolveRentalsModel;
using EvolveRentalsModel.AccessModels;
using EvolveRentalsModel.Constants;
using EvolveRentalsServices.ApiService;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EvolveRentals.Views
{

    public partial class HomePageDetail : ContentPage
    {
        RegistrationDBModel registrationDBModel;
        GetReservationAgreementMobileRequest registrationDBModelRequest;
        GetReservationAgreementMobileResponse registrationDBModelResponse;
        GetReservationByIDMobileRequest reservationByIDMobileRequest;
        GetReservationByIDMobileResponse reservationByIDMobileResponse;
        GetReservationByIDMobileResponse reservationByIDMobileResponseForExtend;
        GetAgreementByAgreementIdMobileResponse agreementIdMobileResponse;
        GetAgreementByAgreementIdMobileRequest agreementIdMobileRequest;
        getAgreementByCustomerIdMobileRequest getAgreementByCustomerIdMobileRequest;
        List<CustomerAgreementModel> customerAgreementModels;
        List<CustomerAgreementModel> customerAgreementModelsForList;
        ExtendAgreementRequest request;
        ExtendAgreementResponse response;


        bool isreservation;
        bool isAgreement;
        bool isbookingBtnVisible = false;

        int agreementId;
        int vehicleId;
        new ObservableCollection<Event> agreementTimerList;

        int customerId;

        string _token;
        bool isAgreeRefreshed;
        int lastAgreementId;
        string lastAgreementStatus;
        DateTime estTime;
        TimeSpan dateDiff;
        private OverDueBalanceViewModel overDueBalanceViewModel;


        public HomePageDetail()
        {
            InitializeComponent();
            customerId = (int)Application.Current.Properties["CustomerId"];
            _token = Application.Current.Properties["currentToken"].ToString();
            registrationDBModelRequest = new GetReservationAgreementMobileRequest();
            registrationDBModelRequest.customerId = customerId;
            registrationDBModelResponse = null;
            registrationDBModel = null;
            agreementIdMobileResponse = null;
            agreementIdMobileRequest = new GetAgreementByAgreementIdMobileRequest();
            getAgreementByCustomerIdMobileRequest = new getAgreementByCustomerIdMobileRequest();
            getAgreementByCustomerIdMobileRequest.customerId = customerId;
            customerAgreementModels = null;
            lastAgreementId = 0;
            lastAgreementStatus = null;

            reservationByIDMobileRequest = new GetReservationByIDMobileRequest();
            isreservation = false;
            isAgreement = false;
            agreementId = 0;
            vehicleId = 0;
            isAgreeRefreshed = false;
            estTime = DateTime.Now;
            request = new ExtendAgreementRequest();
            response = null;
            overDueBalanceViewModel = new OverDueBalanceViewModel();

            ICommand refreshCommand = new Command(() =>
            {
                refreshView.IsRefreshing = true;
                this.OnAppearing();
                refreshView.IsRefreshing = false;
            });
            refreshView.Command = refreshCommand;

            // BooknowBtn.BackgroundColor = (Color)App.Current.Properties["PrimaryColor"];
        }

        public void unSelectedTab()
        {
            btnMyRentals.BackgroundColor = Color.FromHex("#EAEAEA");
            btnPastRental.BackgroundColor = Color.FromHex("#EAEAEA");

            btnMyRentals.TextColor = Color.Black;
            btnPastRental.TextColor = Color.Black;

            grdPastRentals.IsVisible = false;
            grdRentals.IsVisible = false;
            //lastAgreementStack.IsVisible = false;
            //emptyReservation.IsVisible = false;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            Constants.IsHomeDetail = true;
            DateTime timeUtc = DateTime.UtcNow;
            TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
            estTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, estZone);
            dateDiff = DateTime.Now - estTime;

            unSelectedTab();
            btnMyRentals.BackgroundColor = Color.FromHex("#74fa8e");
            btnMyRentals.TextColor = Color.White;
            grdRentals.IsVisible = true;
            //lastAgreementStack.IsVisible = false;
            Constants.IsHome = true;
            bool canRun = true;

            if (PopupNavigation.Instance.PopupStack.Count > 0)
            {
                if (PopupNavigation.Instance.PopupStack[PopupNavigation.Instance.PopupStack.Count - 1] is editPrrofilePhotoPage || PopupNavigation.Instance.PopupStack[PopupNavigation.Instance.PopupStack.Count - 1] is AddCustomerPhotoPopup || PopupNavigation.Instance.PopupStack[PopupNavigation.Instance.PopupStack.Count - 1] is VehicleImagePopup )
                {
                    canRun = false;
                }
                if (PopupNavigation.Instance.PopupStack[PopupNavigation.Instance.PopupStack.Count - 1].GetType() == typeof(ErrorWithClosePagePopup))
                {
                    await PopupNavigation.Instance.PopAllAsync();
                }
            }

            if (canRun)
            {

                Common.mMasterPage.Master = new HomePageMaster();
                Common.mMasterPage.IsPresented = false;

                bool busy = false;
                if (!busy)
                {
                    try
                    {
                        busy = true;
                        await PopupNavigation.Instance.PushAsync(new LoadingPopup("Loading home.."));

                        await Task.Run(async () =>
                        {
                            try
                            {
                                //registrationDBModel = getRegistrationDBModel(customerId, _token);
                                registrationDBModelResponse = getMobileRegistrationDBModel(registrationDBModelRequest, _token);


                                //if (!isAgreeRefreshed)
                                //{
                                //    customerAgreementModels = getReservations(customerId, _token);
                                //}
                                //isAgreeRefreshed = true;
                            }

                            //registrationDBModel.Reservations[0].ReservationId
                            catch (Exception ex)
                            {
                                App.Current.Properties["CustomerId"] = 0;
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

                    if (registrationDBModelResponse != null)
                    {
                        if (registrationDBModelResponse.message.ErrorCode == "200")
                        {
                            registrationDBModel = setUpForHomeView(registrationDBModelResponse.regDB);

                            if (registrationDBModel.Reservations.Count > 0)
                            {
                                reservation_carousel.ItemsSource = registrationDBModel.Reservations;
                                reservation_carousel.IsVisible = true;
                                nobookinStack.IsVisible = false;
                            }
                            else
                            {
                                reservation_carousel.IsVisible = false;
                                nobookinStack.IsVisible = true;
                            }

                            if (registrationDBModel.Agreements.Count > 0)
                            {
                                agree_carousel.ItemsSource = registrationDBModel.Agreements;
                                agree_carousel.IsVisible = true;
                                noAgreeStack.IsVisible = false;
                            }
                            else
                            {
                                agree_carousel.IsVisible = false;
                                noAgreeStack.IsVisible = true;
                            }

                        }
                        else
                        {
                            await PopupNavigation.Instance.PushAsync(new ErrorWithClosePagePopup(registrationDBModelResponse.message.ErrorMessage));
                        }
                    }
                }
                //if (registrationDBModel != null)
                //{
                //    if (registrationDBModel.Reservations.Count > 0)
                //    {
                //        if (registrationDBModel.Reservations[0].Status == "Open" || registrationDBModel.Reservations[0].Status == "New" || registrationDBModel.Reservations[0].Status == "Quote" || registrationDBModel.Reservations[0].Status == "Canceled")
                //        {
                //            isreservation = true;
                //            isAgreement = false;
                //            reservationByIDMobileRequest.ReservationID = registrationDBModel.Reservations[0].ReservationId;

                //            busy = false;
                //            if (!busy)
                //            {
                //                try
                //                {
                //                    busy = true;
                //                    grdRentals.IsVisible = false;
                //                    lastAgreementStack.IsVisible = false;
                //                    LoadingStack.IsVisible = true;
                //                    await Task.Run(() =>
                //                    {
                //                        try
                //                        {
                //                            reservationByIDMobileResponse = getReservationByID(reservationByIDMobileRequest, _token);
                //                        }
                //                        catch (Exception ex)
                //                        {
                //                            PopupNavigation.Instance.PushAsync(new ErrorWithClosePagePopup(ex.Message));
                //                        }
                //                    });
                //                }
                //                finally
                //                {
                //                    busy = false;
                //                    grdRentals.IsVisible = true;
                //                    LoadingStack.IsVisible = false;
                //                    lastAgreementStack.IsVisible = false;
                //                    //reservationByIDMobileResponse.reservationData.Reservationview.StartDateStrForView = ((DateTime)reservationByIDMobileResponse.reservationData.Reservationview.StartDate).ToString("dddd, dd MMMM yyyy hh:mm tt");
                //                    //reservationByIDMobileResponse.reservationData.Reservationview.EndDateStrForView = ((DateTime)reservationByIDMobileResponse.reservationData.Reservationview.EndDate).ToString("dddd, dd MMMM yyyy hh:mm tt");
                //                    reservationByIDMobileResponse.reservationData.Reservationview.PageTitle = Enum.GetName(typeof(ReservationStatuses), reservationByIDMobileResponse.reservationData.Reservationview.Status);
                //                    if (reservationByIDMobileResponse.reservationData.Reservationview.Status == (short)ReservationStatuses.Quote)
                //                    {
                //                        reservationByIDMobileResponse.reservationData.Reservationview.PageTitle = "Pending";
                //                        reservationByIDMobileResponse.reservationData.Reservationview.ExtendVisible = true;
                //                    }
                //                    if (reservationByIDMobileResponse.reservationData.Reservationview.Status == (short)ReservationStatuses.Open)
                //                    {
                //                        reservationByIDMobileResponse.reservationData.Reservationview.PageTitle = "Pending Pickup";
                //                        reservationByIDMobileResponse.reservationData.Reservationview.ExtendVisible = true;
                //                    }
                //                    if (reservationByIDMobileResponse.reservationData.Reservationview.Status == (short)ReservationStatuses.Canceled)
                //                    {
                //                        reservationByIDMobileResponse.reservationData.Reservationview.ExtendVisible = false;
                //                    }

                //                    if (reservationByIDMobileResponse.vehicleTypeModel == null)
                //                    {
                //                        reservationByIDMobileResponse = FixAsResponsibleToReservationByVehicle(reservationByIDMobileResponse);
                //                    }



                //                    reservationByIDMobileResponse.isTimerVisible = false;
                //                    List<GetReservationByIDMobileResponse> upreserItemSource = new List<GetReservationByIDMobileResponse>();
                //                    upreserItemSource.Add(reservationByIDMobileResponse);
                //                    upcomingReservation.ItemsSource = upreserItemSource;
                //                    upcomingReservation.HeightRequest = upcomingReservation.RowHeight;
                //                    if (reservationByIDMobileResponse.reservationData.Reservationview.Status == (short)ReservationStatuses.Canceled)
                //                    {
                //                        BooknowBtn.IsVisible = true;
                //                        isbookingBtnVisible = true;
                //                    }
                //                    else
                //                    {
                //                        BooknowBtn.IsVisible = false;
                //                        isbookingBtnVisible = false;
                //                    }
                //                }

                //            }
                //            //if(registrationDBModel.Reservations[0].Status == "Canceled")
                //            //{
                //            //    BooknowBtn.IsVisible = true;
                //            //}
                //        }
                //        else if (registrationDBModel.Reservations[0].Status == "CheckOut")
                //        {
                //            isreservation = false;
                //            isAgreement = true;
                //            if (registrationDBModel.Agreements.Count > 0)
                //            {
                //                if (registrationDBModel.Agreements[0].Status == "Open")
                //                {
                //                    isAgreement = true;
                //                    agreementTimerList = new ObservableCollection<Event>() { new Event { Date = registrationDBModel.Agreements[0].CheckinDate } };


                //                    Setup();
                //                    agreementIdMobileRequest.agreementId = registrationDBModel.Agreements[0].AgreementId;
                //                    agreementId = registrationDBModel.Agreements[0].AgreementId;
                //                    int vehicleID = registrationDBModel.Agreements[0].VehicleId;
                //                    vehicleId = vehicleID;
                //                    request.agreementId = agreementId;

                //                    busy = false;
                //                    if (!busy)
                //                    {
                //                        try
                //                        {
                //                            busy = true;
                //                            grdRentals.IsVisible = false;
                //                            lastAgreementStack.IsVisible = false;
                //                            LoadingStack.IsVisible = true;
                //                            await Task.Run(() =>
                //                            {
                //                                try
                //                                {
                //                                    agreementIdMobileResponse = getAgreement(agreementIdMobileRequest, _token, vehicleID);
                //                                }
                //                                catch (Exception ex)
                //                                {
                //                                    PopupNavigation.Instance.PushAsync(new ErrorWithClosePagePopup(ex.Message));
                //                                }
                //                            });
                //                        }
                //                        finally
                //                        {
                //                            busy = false;
                //                            grdRentals.IsVisible = false;
                //                            LoadingStack.IsVisible = false;
                //                            lastAgreementStack.IsVisible = true;
                //                            AgreementNumberLabel.Text = agreementIdMobileResponse.custAgreement.AgreementDetail.AgreementNumber;
                //                            AgreementReviewDetailSet agreement = agreementIdMobileResponse.custAgreement;
                //                            string agrStatus = Enum.GetName(typeof(AgreementStatusConst), agreementIdMobileResponse.custAgreement.AgreementDetail.Status);
                //                            statusLabel.Text = Enum.GetName(typeof(AgreementStatusConst), agreementIdMobileResponse.custAgreement.AgreementDetail.Status);
                //                            if (agrStatus == "Open")
                //                            {
                //                                statusLabel.Text = "Active";
                //                            }
                //                            vehicleNameLabel.Text = agreement.AgreementDetail.Year + " " + agreement.AgreementDetail.VehicleMakeName + " " + agreement.AgreementDetail.ModelName;
                //                            VehicleTypeLabel.Text = agreement.AgreementDetail.VehicleType;
                //                            seatsCount.Text = agreementIdMobileResponse.agreementVehicle.Seats;
                //                            bagsCount.Text = agreementIdMobileResponse.agreementVehicle.Baggages.ToString();
                //                            TransType.Text = agreementIdMobileResponse.agreementVehicle.Transmission;
                //                            totalAmountLabel.Text = "Days: " + agreement.AgreementDetail.TotalDays.ToString();
                //                            pickUpLocationLabel.Text = "Pivet Atlanta 2244 Metropolitan Pkwy SW, Atlanta, GA 30315";
                //                            pickUpDateLabel.Text = agreement.AgreementDetail.CheckoutDate.ToString("dddd, dd MMMM yyyy hh:mm tt");
                //                            dropOffLocationLabel.Text = "Pivet Atlanta 2244 Metropolitan Pkwy SW, Atlanta, GA 30315";
                //                            dropOffDateLabel.Text = agreement.AgreementDetail.CheckinDate.ToString("dddd, dd MMMM yyyy hh:mm tt");
                //                            VehicleImage.Source = ImageSource.FromUri(new Uri(agreementIdMobileResponse.agreementVehicle.ImageUrl));




                //                            if (estTime > agreement.AgreementDetail.CheckinDate)
                //                            {
                //                                setUpOverDueBalance();
                //                            }

                //                        }

                //                    }
                //                }
                //                else
                //                {
                //                    isAgreement = false;
                //                    isreservation = false;
                //                    upcomingReservation.IsVisible = false;
                //                    emptyReservation.IsVisible = true;
                //                    BooknowBtn.IsVisible = true;
                //                }
                //            }
                //        }

                //    }
                //    else
                //    {
                //        upcomingReservation.IsVisible = false;
                //        emptyReservation.IsVisible = true;
                //        BooknowBtn.IsVisible = true;
                //        // upReserveFrame.HeightRequest = 290;
                //    }


                //    if (customerAgreementModels != null)
                //    {
                //        if (customerAgreementModels.Count > 0)
                //        {
                //            lastAgreementId = registrationDBModel.Agreements[0].AgreementId;
                //            lastAgreementStatus = registrationDBModel.Agreements[0].Status;
                //            if (customerAgreementModels[customerAgreementModels.Count - 1].Status == "Open")
                //            {
                //                customerAgreementModels.RemoveAt(customerAgreementModels.Count - 1);
                //            }

                //            List<CustomerAgreementModel> agreementItemSource = new List<CustomerAgreementModel>();

                //            foreach (CustomerAgreementModel camfl in customerAgreementModels)
                //            {
                //                if (camfl.Status != null)
                //                {
                //                    if (camfl.Status == "Close" || camfl.Status == "Pending_Payment")
                //                    {
                //                        camfl.custAgreement.AgreementTotal.totalAmountStr = ((decimal)camfl.custAgreement.AgreementTotal.TotalAmount).ToString("0.00");
                //                        if (camfl.Status == "Pending_Payment")
                //                        {
                //                            camfl.Status = "Pending Payment";
                //                        }

                //                        agreementItemSource.Add(camfl);
                //                    }
                //                }
                //            }
                //            agreementItemSource.Reverse();

                //            myRentals.ItemsSource = agreementItemSource;
                //            myRentals.HeightRequest = agreementItemSource.Count * 400;
                //            emptyMyrentals.IsVisible = false;
                //            myRentals.IsVisible = true;
                //        }
                //        else
                //        {
                //            emptyMyrentals.IsVisible = true;
                //            myRentals.IsVisible = false;
                //        }
                //    }

                //    else
                //    {
                //        myRentals.IsVisible = false;
                //        // emptyMyrentals.IsVisible = true;
                //        // myRentFrame.HeightRequest = 290;
                //    }


                //    var AllReservationTap = new TapGestureRecognizer();
                //    AllReservationTap.Tapped += async (s, e) =>
                //    {
                //        Constants.IsHome = false;
                //        if (Navigation.NavigationStack[Navigation.NavigationStack.Count - 1].GetType() != typeof(UpcomingReservations))
                //        {
                //            await Navigation.PushAsync(new UpcomingReservations());
                //        }
                //    };
                //    //allReservationLabel.GestureRecognizers.Add(AllReservationTap);

                //    var AllmyrentalsTap = new TapGestureRecognizer();
                //    AllmyrentalsTap.Tapped += async (s, e) =>
                //    {
                //        Constants.IsHome = false;
                //        if (Navigation.NavigationStack[Navigation.NavigationStack.Count - 1].GetType() != typeof(MyRentals))
                //        {
                //            await Navigation.PushAsync(new MyRentals());
                //        }
                //    };
                //    //allAgreementLabel.GestureRecognizers.Add(AllmyrentalsTap);
                //}
            }


        }

        private RegistrationDBModel setUpForHomeView(RegistrationDBModel regDB)
        {
            RegistrationDBModel newregDB = regDB;

            if(newregDB.Reservations != null)
            {
                if (newregDB.Reservations.Count > 0)
                {
                    foreach(CustomerReservationModel crm in newregDB.Reservations)
                    {
                        crm.isReservationVisible = true;
                        crm.isExtraVisible = false;
                        if (crm.VehilceId == 0)
                        {
                            crm.VehicleModel = crm.Sample + " (Sample)";
                            crm.VehicleImageUrl = crm.VehicleTypeImageUrl;
                        }
                    }
                    if(newregDB.Reservations.Count > 2)
                    {
                        newregDB.Reservations.Add(new CustomerReservationModel() { isExtraVisible = true, isReservationVisible = false });
                    }
                }

                if (newregDB.Agreements.Count > 0)
                {
                    foreach (CustomerAgreementModel cam in newregDB.Agreements)
                    {
                        cam.isReservationVisible = true;
                        cam.isExtraVisible = false;
                    }
                    if (newregDB.Agreements.Count > 2)
                    {
                        newregDB.Agreements.Add(new CustomerAgreementModel() { isExtraVisible = true, isReservationVisible = false });
                    }
                }
            }

            return newregDB;
        }

        private void setUpOverDueBalance()
        {
            refreshBalance();


            // this.BindingContext = overDueBalanceViewModel;

            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                Device.BeginInvokeOnMainThread(() => refreshBalance());
                return true;
            });

        }

        private void refreshBalance()
        {
            DateTime timeUtc = DateTime.UtcNow;
            TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
            estTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, estZone);
            request.extendDate = estTime.ToString("MM/dd/yyyy hh:mm tt").Replace("PM", "pm").Replace("AM", "am");
            AgreementController controller = new AgreementController();
            response = controller.extendAgreement(request, _token);

            if (response != null)
            {
                if (response.agreementReview != null)
                {
                    if (response.agreementReview.BalanceDue != null)
                    {
                        //averDueStack.IsVisible = true;
                        //overdueBalanceAmount.Text = ((decimal)response.agreementReview.BalanceDue).ToString("0.00");
                    }
                }
            }
        }

        private GetReservationByIDMobileResponse FixAsResponsibleToReservationByVehicle(GetReservationByIDMobileResponse reservationByIDMobileResponse)
        {
            if (reservationByIDMobileResponse.vehicleModel != null)
            {
                reservationByIDMobileResponse.vehicleTypeModel = new VehicleTypeWithRatesViewModel();
                reservationByIDMobileResponse.vehicleTypeModel.ImageUrl = reservationByIDMobileResponse.vehicleModel.ImageUrl;
                reservationByIDMobileResponse.vehicleTypeModel.Seats = reservationByIDMobileResponse.vehicleModel.Seats;
                reservationByIDMobileResponse.vehicleTypeModel.Baggages = reservationByIDMobileResponse.vehicleModel.Baggages;
                reservationByIDMobileResponse.vehicleTypeModel.Transmission = reservationByIDMobileResponse.vehicleModel.Transmission;
                reservationByIDMobileResponse.vehicleTypeModel.VehicleTypeName = reservationByIDMobileResponse.vehicleModel.VehicleType;
                reservationByIDMobileResponse.vehicleTypeModel.Sample = reservationByIDMobileResponse.vehicleModel.Year.ToString() + " " + reservationByIDMobileResponse.vehicleModel.Make + " " + reservationByIDMobileResponse.vehicleModel.Model;
            }
            return reservationByIDMobileResponse;
        }

        private List<CustomerAgreementModel> getReservations(int customerId, string token)
        {
            RegisterController registerController = new RegisterController();
            List<CustomerAgreementModel> agreementModels = null;
            try
            {
                agreementModels = registerController.getAgreements(customerId, token);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return agreementModels;
        }

        private GetReservationAgreementMobileResponse getMobileRegistrationDBModel(GetReservationAgreementMobileRequest registrationDBModelRequest, string token)
        {
            GetReservationAgreementMobileResponse response = null;
            try
            {
                RegisterController registerController = new RegisterController();
                response = registerController.getMobileRegistrationDBModel(registrationDBModelRequest, token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        //private RegistrationDBModel getRegistrationDBModel(int customerId, string _token)
        //{
        //    RegisterController register = new RegisterController();
        //    return register.getRegistrationDBModel(customerId, _token);
        //}

        private void BooknowBtn_Clicked(object sender, EventArgs e)
        {
            Constants.IsHome = false;
            Navigation.PushModalAsync(new BookNow());
        }

        //private void UpcomingReservation_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        //{
        //    GetReservationByIDMobileResponse reservationModel = upcomingReservation.SelectedItem as GetReservationByIDMobileResponse;
        //    ((ListView)sender).SelectedItem = null;
        //    if (Navigation.NavigationStack[Navigation.NavigationStack.Count - 1].GetType() != typeof(ViewReservation))
        //    {
        //        Constants.IsHome = false;
        //        Navigation.PushModalAsync(new ViewReservation(reservationModel.reservationData.Reservationview.ReserveId));
        //    }
        //}



        private void btnMyRentals_Clicked(object sender, EventArgs e)
        {
            unSelectedTab();
            btnMyRentals.BackgroundColor = Color.FromHex("#74fa8e");
            btnMyRentals.TextColor = Color.White;
            //if (isreservation)
            //{
                grdRentals.IsVisible = true;
                //BooknowBtn.IsVisible = isbookingBtnVisible;
            //}
            //else if (isAgreement)
            //{
            //    //lastAgreementStack.IsVisible = true;
            //}
            //else
            //{
            //    grdRentals.IsVisible = true;
            //    emptyReservation.IsVisible = true;
            //    //BooknowBtn.IsVisible = true;
            //}
        }

        private void btnPastRental_Clicked(object sender, EventArgs e)
        {
            unSelectedTab();
            //BooknowBtn.IsVisible = false;
            btnPastRental.BackgroundColor = Color.FromHex("#74fa8e");
            btnPastRental.TextColor = Color.White;
            grdPastRentals.IsVisible = true;
        }

        private void btnMenu_Clicked(object sender, EventArgs e)
        {
            Common.mMasterPage.Master = new HomePageMaster();
            Common.mMasterPage.IsPresented = true;

        }

        private GetReservationByIDMobileResponse getReservationByID(GetReservationByIDMobileRequest reservationByIDMobileRequest, string token)
        {

            GetReservationByIDMobileResponse getReservationByID = null;
            RegisterController register = new RegisterController();
            try
            {
                getReservationByID = register.getReservationByID(reservationByIDMobileRequest, token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return getReservationByID;
        }

        //private void myRentals_ItemTapped(object sender, ItemTappedEventArgs e)
        //{
        //    CustomerAgreementModel agreementModel = myRentals.SelectedItem as CustomerAgreementModel;
        //    Navigation.PushModalAsync(new AgreementScreen(agreementModel.AgreementId, agreementModel.VehicleId));
        //}
        private GetAgreementByAgreementIdMobileResponse getAgreement(GetAgreementByAgreementIdMobileRequest agreementByAgreementIdMobileRequest, string token, int vehicleId)
        {
            AgreementController agreementController = new AgreementController();
            GetAgreementByAgreementIdMobileResponse response = null;
            try
            {
                response = agreementController.getAgreement(agreementByAgreementIdMobileRequest, token, vehicleId);
                //getVehicleDetailsMobile = vehicleController.getVehicleTypesMobile(token);
                //foreach(VehicleTypeMobileResult vtmr in getVehicleDetailsMobile.listVehicle)
                //{
                //    if(vtmr.ve)
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return response;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (agreementId > 0 && vehicleId > 0)
            {
                Navigation.PushModalAsync(new AgreementScreen(agreementId, vehicleId));
            }
        }

        private ObservableCollection<Event> AllEvents { get; set; }



        public class Event
        {
            public DateTime Date { get; set; }
            public string EventTitle { get; set; }
            public TimeSpan Timespan { get; set; }
            public string Days => Timespan.Days.ToString("00");
            public string Hours => Timespan.Hours.ToString("00");
            public string Minutes => Timespan.Minutes.ToString("00");
            public string Seconds => Timespan.Seconds.ToString("00");
            public string BgColor { get; set; }
        }

        private void Setup()
        {
            AllEvents = agreementTimerList;
            bool isv = false;

            //Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            //{
            //    eventList.ItemsSource = null;
            //    foreach (var evt in AllEvents)
            //    {
            //        if (evt.Date >= estTime)
            //        {
            //            var timespan = evt.Date - DateTime.Now + dateDiff;
            //            evt.Timespan = timespan;
            //            evt.BgColor = "#42C16F";
            //            timerLabel.Text = "Time Remaining ";
            //        }
            //        else
            //        {
            //            var timespan = DateTime.Now - dateDiff - evt.Date;
            //            evt.Timespan = timespan;
            //            evt.BgColor = "#74fa8e";
            //            timerLabel.Text = "Due time : ";
            //            timerLabel.TextColor = Color.FromHex("#74fa8e");
            //        }


            //    }


            //    //Thread.Sleep(1000);
            //    eventList.ItemsSource = AllEvents;

            //    return true;
            //});


            //Device.StartTimer(new TimeSpan(0, 0, 1), () =>
            //{
            //    foreach (var evt in AllEvents)
            //    {
            //        if (evt.Date >= estTime)
            //        {
            //            var timespan = evt.Date - DateTime.Now + dateDiff;
            //            evt.Timespan = timespan;
            //            evt.BgColor = "#42C16F";
            //            timerLabel.Text = "Time Remaining ";
            //        }
            //        else
            //        {
            //            var timespan = DateTime.Now - dateDiff - evt.Date;
            //            evt.Timespan = timespan;
            //            evt.BgColor = "#74fa8e";
            //            timerLabel.Text = "Overdue : ";
            //            timerLabel.TextColor = Color.FromHex("#74fa8e");
            //        }


            //    }
            //    if (isv)
            //    {
            //        eventList.ItemsSource = AllEvents;
            //        eventList.IsVisible = true;

            //        eventListnew.ItemsSource = null;
            //        eventListnew.IsVisible = false;
            //        isv = false;
            //    }
            //    else
            //    {
            //        eventListnew.ItemsSource = AllEvents;
            //        eventListnew.IsVisible = true;

            //        eventList.ItemsSource = null;
            //        eventList.IsVisible = false;
            //        isv = true;
            //    }

            //    //Thread.Sleep(1000);


            //    return true;
            //});
        }

        private void statusBtn_Clicked(object sender, EventArgs e)
        {
            if (reservationByIDMobileResponse.reservationData.Reservationview.Status == (short)ReservationStatuses.Quote)
            {
                PopupNavigation.Instance.PushAsync(new Error_popup("Waiting for background check results or waiting for insurance documents to be generated"));
            }
        }

        private void btnChat_Tapped(object sender, EventArgs e)
        {

        }

        void extendBtn_Clicked(System.Object sender, System.EventArgs e)
        {
            if (isreservation)
            {
                PopupNavigation.Instance.PushAsync(new Popups.ExtendPopup(reservationByIDMobileResponse.reservationData));
            }
            else if (isAgreement)
            {

                agreementIdMobileResponse.custAgreement.AgreementDetail.RateDetailsList = agreementIdMobileResponse.custAgreement.RateDetailsList;
                agreementIdMobileResponse.custAgreement.AgreementDetail.AgreementInsurance = agreementIdMobileResponse.custAgreement.AgreementInsuranceReview;
                agreementIdMobileResponse.custAgreement.AgreementDetail.vehicleResponse = new GetVehicleIdByCodeResponse();
                agreementIdMobileResponse.custAgreement.AgreementDetail.vehicleResponse.VehicleID = agreementIdMobileResponse.agreementVehicle.VehicleId.ToString();
                int locationIdForPayment = Convert.ToInt32(agreementIdMobileResponse.custAgreement.AgreementDetail.RateLocation);

                //PopupNavigation.Instance.PushAsync(new Popups.ExtendPopup(agreementIdMobileResponse.custAgreement.AgreementDetail, locationIdForPayment));
            }

        }

        void upcomingReservation_Refreshing(System.Object sender, System.EventArgs e)
        {
            //upcomingReservation.IsRefreshing = true;
            this.OnAppearing();
            //upcomingReservation.IsRefreshing = false;
        }

        //private void upcomingReservation_ItemTapped(object sender, ItemTappedEventArgs e)
        //{
        //    GetReservationByIDMobileResponse reservationModel = upcomingReservation.SelectedItem as GetReservationByIDMobileResponse;
        //    ((ListView)sender).SelectedItem = null;
        //    if (Navigation.NavigationStack[Navigation.NavigationStack.Count - 1].GetType() != typeof(ViewReservation))
        //    {
        //        Constants.IsHome = false;
        //        Navigation.PushModalAsync(new ViewReservation(reservationModel.reservationData.Reservationview.ReserveId));
        //    }
        //}
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Constants.IsHomeDetail = false;
        }

        private void reservationMoreBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new UpcomingReservations());
        }

        private void agreeMoreBtn_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new MyRentals());
        }

        private void viewReservationDetail_Clicked(object sender, EventArgs e)
        {
            var obj = (Label)sender;
            var reservationModel = obj.BindingContext as CustomerReservationModel;
            Navigation.PushModalAsync(new ViewReservation(reservationModel.ReservationId));
        }

        private void viewAgreeDetail_Clicked(object sender, EventArgs e)
        {
            var obj = (Label)sender;
            var agreemodel = obj.BindingContext as CustomerAgreementModel;
            Navigation.PushModalAsync(new AgreementScreen(agreemodel.AgreementId, agreemodel.VehicleId));
        }

        private void viewReservationDetailArrow_Clicked(object sender, EventArgs e)
        {
            var obj = (ImageButton)sender;
            var reservationModel = obj.BindingContext as CustomerReservationModel;
            Navigation.PushModalAsync(new ViewReservation(reservationModel.ReservationId));
        }

        private void viewAgreeDetailBtn_Clicked(object sender, EventArgs e)
        {
            var obj = (ImageButton)sender;
            var agreemodel = obj.BindingContext as CustomerAgreementModel;
            Navigation.PushModalAsync(new AgreementScreen(agreemodel.AgreementId, agreemodel.VehicleId));
        }
    }
}