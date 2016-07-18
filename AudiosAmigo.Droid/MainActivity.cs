using System;
using System.Net.Sockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Android.App;
using Android.Content.PM;
using Android.Widget;
using Android.OS;

namespace AudiosAmigo.Droid
{
    [Activity(Label = "AudiosAmigo.Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            RequestedOrientation = ScreenOrientation.SensorLandscape;
            SetContentView(Resource.Layout.Main);

            var ipAddress = FindViewById<TextView>(Resource.Id.ip_address);
            var connectInfo = ConnectionInformation.FromDiaglog(this);
            connectInfo.Subscribe(
                info => ipAddress.Text = $"{info.Item1}:{info.Item2}");
            connectInfo.ObserveOn(NewThreadScheduler.Default).Subscribe(
                info => Connect(info.Item1, info.Item2, info.Item3));

            var disconnect = FindViewById<Button>(Resource.Id.disconnect);
            new ObservableClickListener(disconnect).Subscribe(pressed => Recreate());
        }

        public void Connect(string ip, int port, string password)
        {
            var client = new TcpClient(ip, port);
            var communication = new SecureTcpClientCommunication(
                new TcpClientCommunication(client),
                new Encrpytion(password, Constants.EncrpytionInitVector));
            var communicator = new Communicator<Command>(
                communication, NewThreadScheduler.Default);
            var handler = new AudioClient(this);
            communicator.SubscribeOn(NewThreadScheduler.Default).Subscribe(handler);
            handler.SubscribeOn(NewThreadScheduler.Default).Subscribe(communicator);
        }
    }
}
