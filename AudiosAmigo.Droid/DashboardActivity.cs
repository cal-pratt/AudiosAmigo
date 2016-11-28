using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AudiosAmigo.Droid2;

namespace AudiosAmigo.Droid
{
    [Activity(Label = "Audio's Amigo Dashboard", MainLauncher = true, Icon = "@drawable/audio_icon") ]
    public class DashboardActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Dashboard);
            var setup = FindViewById<Button>(Resource.Id.setup_button);
            setup.Click += (s, e) => StartActivity(typeof(TcpConnectionMenuActivity));
            var connect = FindViewById<Button>(Resource.Id.connect_button);
            connect.Click += (s, e) => StartActivity(typeof(MainActivity));
        }
    }
}