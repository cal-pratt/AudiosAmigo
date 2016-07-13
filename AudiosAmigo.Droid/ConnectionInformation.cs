using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.Widget;

namespace AudiosAmigo.Droid
{
    public class ConnectionInformation
    {
        public static IObservable<Tuple<string, int>> FromDiaglog(Context context)
        {
            var dialog = new Dialog(context);
            dialog.SetContentView(Resource.Layout.connect_menu);
            dialog.SetCancelable(false);
            dialog.SetTitle("Enter Desktop Information");

            var ip = dialog.FindViewById<EditText>(Resource.Id.ip);
            var port = dialog.FindViewById<EditText>(Resource.Id.port);
            var password = dialog.FindViewById<EditText>(Resource.Id.password);
            var button = dialog.FindViewById<Button>(Resource.Id.connect);
            var ipListView = dialog.FindViewById<ListView>(Resource.Id.list);
            var ips = new[]
            {
                "192.168.2.19:12345",
                "111.222.333.444:55555",
                "333.222.333.222:55555",
                "111.222.333.444:55556",
                "333.222.222.444:53335",
                "333.222.333.333:55555",
                "222.222.333.444:55556",
                "333.222.333.444:55555",
                "333.222.444.222:44488",
                "111.222.333.444:55556"
            };
            var ipList = new List<string>(ips);
            var adapter = new ArrayAdapter(context, Android.Resource.Layout.SimpleListItem1, ipList);
            ipListView.Adapter = adapter;

            new ObservableOnItemClickListener<Java.Lang.String>(ipListView).Subscribe(item =>
            {
                var split = item.Split(":");
                ip.Text = split[0];
                port.Text = split[1];
            });

            var observableButton = new ObservableClickListener(button)
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
