using System;
using System.Net.Sockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Android.App;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.Graphics;
using Android.Widget;
using Android.OS;
using Android.Views;
using AudiosAmigo.Droid.Observables;

namespace AudiosAmigo.Droid
{
    [Activity(Label = "AudiosAmigo.Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private const float SliderWidthRatio = 0.18f;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            RequestedOrientation = ScreenOrientation.SensorLandscape;
            SetContentView(Resource.Layout.Main);

            var adView = FindViewById<AdView>(Resource.Id.adView);
            MobileAds.Initialize(ApplicationContext, adView.AdUnitId);
            adView.LoadAd(new AdRequest.Builder().Build());

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
            var height = FindViewById(Resource.Id.slider_scroll).Height;
            var width = (int) (height*SliderWidthRatio);

            var systemBitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.audiosrv);
            var muteBitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.muteblue);

            var inflater = (LayoutInflater) GetSystemService(LayoutInflaterService);
            var vibrator = (Vibrator) GetSystemService(VibratorService);

            var volumeSliderBuilder = new VolumeSlider.Builder(
                inflater, vibrator, width, height, muteBitmap);

            var deviceSelectorBuilder = new DeviceSelector.Builder(
                inflater, width, width);

            var audioDeviceControllerBuilder = new AudioDeviceController.Builder(
                volumeSliderBuilder,
                FindViewById<ViewGroup>(Resource.Id.normal_sliders),
                FindViewById<ViewGroup>(Resource.Id.master_sliders),
                FindViewById<ViewGroup>(Resource.Id.system_sliders),
                FindViewById<TextView>(Resource.Id.status),
                systemBitmap);
            
            var controller = new AudioController(
                audioDeviceControllerBuilder,
                deviceSelectorBuilder,
                FindViewById<ViewGroup>(Resource.Id.device_container));

            var handler = new AudioClient(controller);

            var client = new TcpClient(ip, port);
            var communication = new SecureTcpClientCommunication(
                new TcpClientCommunication(client),
                new Encrpytion(password, Constants.EncrpytionInitVector));
            var communicator = new Communicator<Command>(
                communication, NewThreadScheduler.Default);

            communicator.SubscribeOn(NewThreadScheduler.Default).Subscribe(handler);
            handler.SubscribeOn(NewThreadScheduler.Default).Subscribe(communicator);
        }
    }
}
