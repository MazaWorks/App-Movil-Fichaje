using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using System.Net;
using RestSharp;
using Dtos;
using Plugin.LocalNotifications;

namespace FichajeQindel
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private static int hour;
        private static int minute;
        private static bool timerOn = false;
        private static string valor;
        private static bool fiche = false;
        public MainPage()
        {
            InitializeComponent();
            ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => true;

            NavigationPage.SetHasBackButton(this, false);
            ApiCall(true);
        }
        async void OnOptionPage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new OptionPage());
        }
        void OnDoThings(object sender, EventArgs e)
        {
            ApiCall(false);
        }
        void Refresh(object sender, EventArgs e)
        {
            ApiCall(true);
        }
        bool OnSynch()
        {
            if (fiche) {
                ++minute;
                if (minute == 60)
                {
                    minute = 0;
                    ++hour;
                }
                string hours = hour.CompareTo(10) != -1 ? hour.ToString() : "0" + hour.ToString();
                string minutes = minute.CompareTo(10) != -1 ? minute.ToString() : "0" + minute.ToString();
                counter.Text = hours + ":" + minutes;
                Device.StartTimer(TimeSpan.FromMinutes(1), OnTimerTick);
            }
            return false;
        }
        bool OnTimerTick()
        {
            if (fiche) {
                ++minute;
                if (minute == 60)
                {
                    minute = 0;
                    ++hour;
                }
                string hours = hour.CompareTo(10) != -1 ? hour.ToString() : "0" + hour.ToString();
                string minutes = minute.CompareTo(10) != -1 ? minute.ToString() : "0" + minute.ToString();
                counter.Text = hours + ":" + minutes;
                return true;
            } else {
                return false;
            }
        }
        void ApiCall(bool refresh)
        {
            actIndicator.IsVisible = true;
            counter.IsVisible = false;
            var client = new RestClient("https://fichaje.qindel.com");

            var request = new RestRequest("api/timesheets/active", Method.GET);
            request.AddHeader("X-AUTH-USER", Application.Current.Properties.ContainsKey("UserName") ? Application.Current.Properties["UserName"].ToString() : "null");
            request.AddHeader("X-AUTH-TOKEN", Application.Current.Properties.ContainsKey("Api_token") ? Application.Current.Properties["Api_token"].ToString() : "null");

            client.ExecuteAsync<List<Timesheet>>(request, response => {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (response.Data.Count != 0)
                    {
                        fiche = true;
                        DateTime now = DateTime.Now;
                        DateTime begin = response.Data[0].begin.ToLocalTime();
                        hour = (now - begin).Hours;
                        minute = (now - begin).Minutes;
                        if (!timerOn)
                        {
                            string hours = hour.CompareTo(10) != -1 ? hour.ToString() : "0" + hour.ToString();
                            string minutes = minute.CompareTo(10) != -1 ? minute.ToString() : "0" + minute.ToString();
                            valor = hours + ":" + minutes;
                            timerOn = true;
                            if ((bool)Application.Current.Properties["NotificationsEnabled"])
                            {
                                TimeSpan time = Application.Current.Properties.ContainsKey("NumHoras") ? (TimeSpan)Application.Current.Properties["NumHoras"] : new TimeSpan(8, 0, 0);
                                CrossLocalNotifications.Current.Cancel(0);
                                CrossLocalNotifications.Current.Show("Fichaje Qindel", "Amigo, lleva trabajando mas de " + time.Hours + ":" + (time.Minutes.CompareTo(10) != -1 ? time.Minutes.ToString() : "0" + time.Minutes.ToString()) + " horas", 0, begin.AddHours(time.Hours).AddMinutes(time.Minutes));
                            }
                            Device.StartTimer(TimeSpan.FromSeconds(60 - ((now - begin).Seconds) % 60), OnSynch);
                        }
                        else
                        {
                            if ((bool)Application.Current.Properties["NotificationsEnabled"])
                            {
                                TimeSpan time = Application.Current.Properties.ContainsKey("NumHoras") ? (TimeSpan)Application.Current.Properties["NumHoras"] : new TimeSpan(8, 0, 0);
                                CrossLocalNotifications.Current.Cancel(0);
                                CrossLocalNotifications.Current.Show("Fichaje Qindel", "Amigo, lleva trabajando mas de " + time.Hours + ":" + (time.Minutes.CompareTo(10) != -1 ? time.Minutes.ToString() : "0" + time.Minutes.ToString()) + " horas", 0, begin.AddHours(time.Hours).AddMinutes(time.Minutes));
                            }
                            string hours = hour.CompareTo(10) != -1 ? hour.ToString() : "0" + hour.ToString();
                            string minutes = minute.CompareTo(10) != -1 ? minute.ToString() : "0" + minute.ToString();
                            valor = hours + ":" + minutes;
                        }
                    }
                    else
                    {
                        fiche = false;
                        valor = "No se ha fichado";
                    }
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (fiche)
                        {
                            llegue.IsEnabled = false;
                            mevoy.IsEnabled = true;
                        }
                        else
                        {
                            mevoy.IsEnabled = false;
                            llegue.IsEnabled = true;
                        }
                        if (!refresh)
                        {
                            if (fiche)
                            {
                                client = new RestClient("https://fichaje.qindel.com");

                                request = new RestRequest("api/timesheets/" + response.Data[0].id + "/stop", Method.PATCH);
                                request.AddHeader("X-AUTH-USER", Application.Current.Properties.ContainsKey("UserName") ? Application.Current.Properties["UserName"].ToString() : "null");
                                request.AddHeader("X-AUTH-TOKEN", Application.Current.Properties.ContainsKey("Api_token") ? Application.Current.Properties["Api_token"].ToString() : "null");
                                client.ExecuteAsync<List<Timesheet>>(request, response2 =>
                                {
                                    if (response2.StatusCode == HttpStatusCode.OK)
                                    {
                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            fiche = false;
                                            timerOn = false;
                                            llegue.IsEnabled = true;
                                            mevoy.IsEnabled = false;
                                            actIndicator.IsVisible = false;
                                            counter.IsVisible = true;
                                            counter.Text = "No se ha fichado";
                                            CrossLocalNotifications.Current.Cancel(0);
                                        });
                                    }
                                    else if (response.StatusCode == HttpStatusCode.Forbidden)
                                    {
                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            CrossLocalNotifications.Current.Cancel(0);
                                            llegue.IsEnabled = false;
                                            mevoy.IsEnabled = false;
                                            fiche = false;
                                            actIndicator.IsVisible = false;
                                            counter.IsVisible = true;
                                            counter.Text = "Error con las credenciales";
                                        });
                                    }
                                    else
                                    {
                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            CrossLocalNotifications.Current.Cancel(0);
                                            llegue.IsEnabled = false;
                                            mevoy.IsEnabled = false;
                                            fiche = false;
                                            actIndicator.IsVisible = false;
                                            counter.IsVisible = true;
                                            counter.Text = "Problema con la peticion";
                                        });
                                    }
                                });
                            }
                            else
                            {
                                client = new RestClient("https://fichaje.qindel.com");

                                request = new RestRequest("api/timesheets/recent", Method.GET);
                                request.AddHeader("X-AUTH-USER", Application.Current.Properties.ContainsKey("UserName") ? Application.Current.Properties["UserName"].ToString() : "null");
                                request.AddHeader("X-AUTH-TOKEN", Application.Current.Properties.ContainsKey("Api_token") ? Application.Current.Properties["Api_token"].ToString() : "null");
                                client.ExecuteAsync<List<Timesheet>>(request, response2 =>
                                {
                                    if (response2.StatusCode == HttpStatusCode.OK)
                                    {
                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            client = new RestClient("https://fichaje.qindel.com");

                                            request = new RestRequest("api/timesheets/" + response2.Data[0].id + "/restart", Method.PATCH);
                                            request.AddHeader("X-AUTH-USER", Application.Current.Properties.ContainsKey("UserName") ? Application.Current.Properties["UserName"].ToString() : "null");
                                            request.AddHeader("X-AUTH-TOKEN", Application.Current.Properties.ContainsKey("Api_token") ? Application.Current.Properties["Api_token"].ToString() : "null");
                                            client.ExecuteAsync<List<Timesheet>>(request, response3 =>
                                            {
                                                if (response3.StatusCode == HttpStatusCode.OK)
                                                {
                                                    Device.BeginInvokeOnMainThread(() =>
                                                    {
                                                        fiche = true;
                                                        llegue.IsEnabled = false;
                                                        mevoy.IsEnabled = true;
                                                        actIndicator.IsVisible = false;
                                                        counter.IsVisible = true;
                                                        hour = 0;
                                                        minute = 0;
                                                        counter.Text = "00:00";
                                                        timerOn = true;
                                                        if ((bool)Application.Current.Properties["NotificationsEnabled"])
                                                        {
                                                            TimeSpan time = Application.Current.Properties.ContainsKey("NumHoras") ? (TimeSpan)Application.Current.Properties["NumHoras"] : new TimeSpan(8, 0, 0);
                                                            Console.WriteLine(time);
                                                            CrossLocalNotifications.Current.Cancel(0);
                                                            CrossLocalNotifications.Current.Show("Fichaje Qindel", "Amigo, lleva trabajando mas de " + time.Hours + ":" + (time.Minutes.CompareTo(10) != -1 ? time.Minutes.ToString() : "0" + time.Minutes.ToString()) + " horas", 0, DateTime.Now.AddHours(time.Hours).AddMinutes(time.Minutes));
                                                        }
                                                        Device.StartTimer(TimeSpan.FromMinutes(1), OnTimerTick);
                                                    });
                                                }
                                                else if (response.StatusCode == HttpStatusCode.Forbidden)
                                                {
                                                    Device.BeginInvokeOnMainThread(() =>
                                                    {
                                                        CrossLocalNotifications.Current.Cancel(0);
                                                        llegue.IsEnabled = false;
                                                        mevoy.IsEnabled = false;
                                                        fiche = false;
                                                        actIndicator.IsVisible = false;
                                                        counter.IsVisible = true;
                                                        counter.Text = "Error con las credenciales";
                                                    });
                                                }
                                                else
                                                {
                                                    Device.BeginInvokeOnMainThread(() =>
                                                    {
                                                        CrossLocalNotifications.Current.Cancel(0);
                                                        llegue.IsEnabled = false;
                                                        mevoy.IsEnabled = false;
                                                        fiche = false;
                                                        actIndicator.IsVisible = false;
                                                        counter.IsVisible = true;
                                                        counter.Text = "Problema con la peticion";
                                                    });
                                                }
                                            });
                                        });
                                    }
                                    else if (response.StatusCode == HttpStatusCode.Forbidden)
                                    {
                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            CrossLocalNotifications.Current.Cancel(0);
                                            llegue.IsEnabled = false;
                                            mevoy.IsEnabled = false;
                                            fiche = false;
                                            actIndicator.IsVisible = false;
                                            counter.IsVisible = true;
                                            counter.Text = "Error con las credenciales";
                                        });
                                    }
                                    else
                                    {
                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            CrossLocalNotifications.Current.Cancel(0);
                                            llegue.IsEnabled = false;
                                            mevoy.IsEnabled = false;
                                            fiche = false;
                                            actIndicator.IsVisible = false;
                                            counter.IsVisible = true;
                                            counter.Text = "Problema con la peticion";
                                        });
                                    }
                                });
                            }
                        }
                        else
                        {
                            actIndicator.IsVisible = false;
                            counter.IsVisible = true;
                            counter.Text = valor;
                        }
                    });
                }
                else if(response.StatusCode == HttpStatusCode.Forbidden)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        CrossLocalNotifications.Current.Cancel(0);
                        llegue.IsEnabled = false;
                        mevoy.IsEnabled = false;
                        fiche = false;
                        actIndicator.IsVisible = false;
                        counter.IsVisible = true;
                        counter.Text = "Error con las credenciales";
                    });
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        CrossLocalNotifications.Current.Cancel(0);
                        llegue.IsEnabled = false;
                        mevoy.IsEnabled = false;
                        fiche = false;
                        actIndicator.IsVisible = false;
                        counter.IsVisible = true;
                        counter.Text = "Problema con la peticion";
                    });
                }
            });
        }
    }
}