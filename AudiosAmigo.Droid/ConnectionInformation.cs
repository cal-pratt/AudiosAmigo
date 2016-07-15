using System;
using System.Collections.Generic;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using Android.App;
using Android.Views;
using Android.Widget;

namespace AudiosAmigo.Droid
{
    public class ConnectionInformation
    {
        public static IObservable<Tuple<string, int>> FromDiaglog(Activity activity)
        {
            var dialog = new Dialog(activity, Android.Resource.Style.ThemeHolo);
            dialog.Window.SetSoftInputMode(SoftInput.AdjustPan);
            dialog.SetContentView(Resource.Layout.connect_menu);
            dialog.SetCancelable(false);
            dialog.SetTitle("    Enter Desktop Information");

            var ip = dialog.FindViewById<EditText>(Resource.Id.ip);
            var port = dialog.FindViewById<EditText>(Resource.Id.port);
            var password = dialog.FindViewById<EditText>(Resource.Id.password);
            var connect = dialog.FindViewById<Button>(Resource.Id.connect);

            var adapter = new ArrayAdapter(
                activity, 
                Android.Resource.Layout.SimpleListItem1,
                new List<Java.Lang.String>());

            var ipListView = dialog.FindViewById<ListView>(Resource.Id.list);
            ipListView.Adapter = adapter;

            UdpUtil.ReceivePortInfo(Constants.ClientBroadcastListenerPort)
                .ObserveOn(NewThreadScheduler.Default)
                .SubscribeOn(NewThreadScheduler.Default)
                .Subscribe(endpoint => activity.RunOnUiThread(() =>
                {
                    adapter.Add(new Java.Lang.String($"{endpoint.Address}:{endpoint.Port}"));
                    adapter.NotifyDataSetChanged();
                }));

            NewThreadScheduler.Default.Schedule(() =>
            {
                UdpUtil.IntWriter(Constants.ClientBroadcastListenerPort).Invoke(
                    new IPEndPoint(IPAddress.Broadcast, Constants.ServerBroadcastListenerPort));
            });

            new ObservableOnItemClickListener<Java.Lang.String>(ipListView).Subscribe(item =>
            {
                var split = item.Split(":");
                ip.Text = split[0];
                port.Text = split[1];
            });

            var observableButton = new ObservableClickListener(connect)
                .Where(pressed => ip.Text != "" && Regex.IsMatch(port.Text, @"^\d+$"));

            dialog.Show();

            observableButton.Subscribe(pressed =>
            {
                dialog.Hide();
            });

            return observableButton.Select(pressed => Tuple.Create(ip.Text, int.Parse(port.Text)));
        }
    }
}
