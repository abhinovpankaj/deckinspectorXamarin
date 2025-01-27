﻿using Mobile.Code.ViewModels;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile.Code.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VisualBuildingLocationForm : ContentPage
    {
        private ISpeechToText _speechRecongnitionInstance;
        //ProjectAddEditViewModel vm;
        public VisualBuildingLocationForm()
        {
            InitializeComponent();
            //if (Device.RuntimePlatform == Device.iOS)
            //{
            //    recordDes.IsVisible = true;
            //    recordDes.IsEnabled = false;
            //}
            _speechRecongnitionInstance = DependencyService.Get<ISpeechToText>();

            MessagingCenter.Subscribe<ISpeechToText, string>(this, "STT", (sender, args) =>
            {

                SpeechToTextFinalResultRecieved(args);
            });

            MessagingCenter.Subscribe<ISpeechToText>(this, "Final", (sender) =>
            {
                ImageButton btn = sender as ImageButton;
                if (btn.ClassId == "recordDes")
                {
                    recordDes.IsEnabled = true;
                }

            });

            MessagingCenter.Subscribe<IMessageSender, string>(this, "STT", (sender, args) =>
            {
                SpeechToTextFinalResultRecieved(args);
            });

            //this.BindingContext =vm= new ProjectAddEditViewModel();
        }
        protected async override void OnAppearing()
        {

            //   VisualBuildingLocationFormViewModel vm = ((VisualBuildingLocationFormViewModel)this.BindingContext);
            //  MessagingCenter.Unsubscribe<Camera2Forms.CameraPage, ObservableCollection<MultiImage>>(this, "ImageList");

            base.OnAppearing();
            await ((VisualBuildingLocationFormViewModel)this.BindingContext).Load();

            //MessagingCenter.Subscribe<Camera2Forms.CameraPage, ObservableCollection<MultiImage>>(this, "ImageList", async (Camera2Forms.CameraPage obj, ObservableCollection<MultiImage> item) =>
            //// MessagingCenter.Subscribe<Camera2Forms.CameraPage, ObservableCollection<MultiImage>>(this, "ImageList", async (obj, item) =>
            //{
            //    var items = item as ObservableCollection<MultiImage>;
            //    foreach (var photo in items)
            //    {
            //        VisualBuildingLocationPhoto newObj = new VisualBuildingLocationPhoto() { ImageUrl = photo.Image, Id = Guid.NewGuid().ToString(), VisualBuildingId = vm.VisualForm.Id, DateCreated = DateTime.Now };
            //        if (App.IsInvasive == true)
            //        {

            //            _ = vm.AddNewPhoto(newObj);
            //        }
            //        else
            //        {

            //            _ = vm.AddNewPhoto(newObj);
            //            //  await VisualProjectLocationPhotoDataStore.AddItemAsync(newObj);
            //        }
            //        // await VisualBuildingLocationPhotoDataStore.AddItemAsync(newObj);

            //    }
            //  //  items.Clear();


            //});
            //vm.Load();
        }
        private void SpeechToTextFinalResultRecieved(string args)
        {
            if (txtName.IsFocused)
            {
                txtName.Text += args;
            }

            if (txtDes.IsFocused)
            {
                txtDes.Text += args;
            }
        }

        private void recordClick(object sender, EventArgs e)
        {
            ImageButton btn = sender as ImageButton;
            try
            {
                if (btn.ClassId == "recordName")
                {
                    txtName.Focus();

                }

                else if (btn.ClassId == "recordDes")
                {
                    txtDes.Focus();

                }
                _speechRecongnitionInstance.StartSpeechToText();
            }
            catch (Exception ex)
            {

                txtName.Text = ex.Message;
            }

            if (Device.RuntimePlatform == Device.iOS)
            {
                recordName.IsEnabled = false;

                recordDes.IsEnabled = false;
            }

        }
        protected override bool OnBackButtonPressed() => false;

        private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            VisualBuildingLocationFormViewModel vm = (VisualBuildingLocationFormViewModel)this.BindingContext;
            vm.CheckAnyRadioButtonChecked = true;
        }
        //protected  override bool OnBackButtonPressed()
        //{
        //    //var result = await this.DisplayAlert("Alert!", "Do you really want to exit?", "Yes", "No");
        //    //Device.BeginInvokeOnMainThread(async () => {

        //    //    if (result) {

        //    //        await Shell.Current.Navigation.PopAsync();

        //    //    } // or anything else
        //    //});

        //    return true;

        //    //  return true; // prevent Xamarin.Forms from processing back button
        //    // var result = await Shell.Current.DisplayAlert(
        //    //          "Alert",
        //    //          "Are you sure you want to go back?",
        //    //          "Yes", "No");
        //    // if (result)
        //    // {
        //    //      Shell.Current.Navigation.PopAsync();
        //    //     return await Task.FromResult<bool>(true);
        //    // }
        //    // // By returning TRUE and not calling base we cancel the hardware back button :)
        //    // //base.OnBackButtonPressed();
        //    // // Task.FromResult(true);
        //    //return await Task.FromResult<bool>(true);
        //}
        //protected override bool OnBackButtonPressed()
        //{

        //    Device.BeginInvokeOnMainThread(async () =>
        //    {
        //        if (await DisplayAlert("Exit?", "Are you sure you want to exit from this page?", "Yes", "No"))
        //        {
        //            base.OnBackButtonPressed();
        //            await App.Current.MainPage.Navigation.PopAsync();
        //        }
        //    });

        //    return true;
        //}
        //protected  override void OnNavigating(object sender, ShellNavigatingEventArgs e)
        //{
        //    // Cancel back navigation if data is unsaved
        //    if (e.Source == ShellNavigationSource.Pop)
        //    {
        //        if (await DisplayAlert("Exit?", "Are you sure you want to exit from this page?", "Yes", "No"))
        //        {
        //            base.OnBackButtonPressed();
        //            await App.Current.MainPage.Navigation.PopAsync();
        //        }
        //        e.Cancel();
        //    }
        //}


    }
}