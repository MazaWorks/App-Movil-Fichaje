using Dtos;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using Xamarin.Forms;

namespace FichajeQindel
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());

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