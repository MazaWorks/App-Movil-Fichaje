using Plugin.LocalNotifications;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FichajeQindel
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OptionPage : ContentPage
    {
        public OptionPage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, true);

            if (Application.Current.Properties.ContainsKey("UserName")) username.Text = Application.Current.Properties["UserName"].ToString();
            if (Application.Current.Properties.ContainsKey("Api_token")) api_token.Text = Application.Current.Properties["Api_token"].ToString();
            if (Application.Current.Properties.ContainsKey("NumHoras")) horas.Time = (TimeSpan)Application.Current.Properties["NumHoras"];
            if (Application.Current.Properties.ContainsKey("NotificationsEnabled")) notifications.IsToggled = (bool)Application.Current.Properties["NotificationsEnabled"];
        }
        private void OnSave(object sender, EventArgs e)
        {
            Application.Current.Properties["UserName"] = username.Text;
            Application.Current.Properties["Api_token"] = api_token.Text;
            Application.Current.Properties["NumHoras"] = horas.Time;
            Application.Current.Properties["NotificationsEnabled"] = notifications.IsToggled;
            if ((bool)Application.Current.Properties["NotificationsEnabled"])
            {
                CrossLocalNotifications.Current.Cancel(0);
            }
            OnGoBack(null, null);
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
        async void OnGoBack(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}