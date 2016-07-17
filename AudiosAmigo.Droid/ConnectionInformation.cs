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
            var dialog = new Dialog(activity, Android.Resource.Style.ThemeHoloNoActionBar);
            dialog.Window.SetSoftInputMode(SoftInput.AdjustPan);
            dialog.SetContentView(Resource.Layout.connect_menu);
            dialog.SetCancelable(false);

            var ip = dialog.FindViewById<EditText>(Resource.Id.ip);
            var port = dialog.FindViewById<EditText>(Resource.Id.port);
            var password = dialog.FindViewById<EditText>(Resource.Id.password);
            var connect = dialog.FindViewById<Button>(Resource.Id.connect);
            var search = dialog.FindViewById<Button>(Resource.Id.search);

            var adapter = new ArrayAdapter(
                activity, 
                Android.Resource.Layout.SimpleListItem1,
                new List<Java.Lang.String>());

            var ipListView = dialog.FindViewById<ListView>(Resource.Id.search_list);
            ipListView.Adapter = adapter;

            var ipSet = new HashSet<Java.Lang.String>();
            UdpUtil.ReceivePortInfo(Constants.ClientBroadcastListenerPort)
                .ObserveOn(NewThreadScheduler.Default)
                .SubscribeOn(NewThreadScheduler.Default)
                .Select(endpoint => new Java.Lang.String($"{endpoint.Address}:{endpoint.Port}"))
                .Subscribe(session => activity.RunOnUiThread(() =>
                {
                    if (ipSet.Add(session))
                    {
                        adapter.Add(session);
                        adapter.NotifyDataSetChanged();
                    }
                }));

            var observableSearch = new ObservableClickListener(search);
            observableSearch.Subscribe(pressed => adapter.Clear());
            observableSearch.SubscribeOn(NewThreadScheduler.Default).Subscribe(pressed =>
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

            var observableConnect = new ObservableClickListener(connect)
                .Where(pressed => ip.Text != "" && Regex.IsMatch(port.Text, @"^\d+$"));

            dialog.Show();

            observableConnect.Subscribe(pressed =>
            {
                dialog.Hide();
            });

            return observableConnect.Select(pressed => Tuple.Create(ip.Text, int.Parse(port.Text)));
        }
    }
}
