using System;
using System.Collections.Generic;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.Provider;
using Android.Views;
using Android.Widget;

namespace AudiosAmigo.Droid
{
    public class ConnectionInformation
    {
        public static IObservable<Tuple<string, int, string>> FromDiaglog(Activity activity)
        {
            var dialog = new Dialog(activity, Android.Resource.Style.ThemeHoloNoActionBar);
            dialog.Window.SetSoftInputMode(SoftInput.AdjustPan);
            dialog.SetContentView(Resource.Layout.connect_menu);
            dialog.SetCancelable(false);

            #region FindViewById

            var ip = dialog.FindViewById<EditText>(Resource.Id.ip);

            var port = dialog.FindViewById<EditText>(Resource.Id.port);

            var password = dialog.FindViewById<EditText>(Resource.Id.password);

            var connect = dialog.FindViewById<Button>(Resource.Id.connect);

            var search = dialog.FindViewById<Button>(Resource.Id.search);

            var searchListView = dialog.FindViewById<ListView>(Resource.Id.search_list);

            var save = dialog.FindViewById<Button>(Resource.Id.save);

            var saveListView = dialog.FindViewById<ListView>(Resource.Id.save_list);

            #endregion

            #region search

            var searchAdapter = new ArrayAdapter(
                activity,
                Android.Resource.Layout.SimpleListItem1,
                new List<Java.Lang.String>());

            searchListView.Adapter = searchAdapter;

            var searchSet = new HashSet<Java.Lang.String>();
            UdpUtil.ReceivePortInfo(Constants.ClientBroadcastListenerPort)
                .ObserveOn(NewThreadScheduler.Default)
                .SubscribeOn(NewThreadScheduler.Default)
                .Select(endpoint => new Java.Lang.String($"{endpoint.Address}:{endpoint.Port}"))
                .Subscribe(session => activity.RunOnUiThread(() =>
                {
                    if (searchSet.Add(session))
                    {
                        searchAdapter.Add(session);
                        searchAdapter.NotifyDataSetChanged();
                    }
                }));

            var observableSearch = new ObservableClickListener(search);
            observableSearch.Subscribe(pressed => searchAdapter.Clear());
            observableSearch.SubscribeOn(NewThreadScheduler.Default).Subscribe(pressed =>
            {
                UdpUtil.IntWriter(Constants.ClientBroadcastListenerPort).Invoke(
                    new IPEndPoint(IPAddress.Broadcast, Constants.ServerBroadcastListenerPort));
            });

            new ObservableOnItemClickListener<Java.Lang.String>(searchListView)
                .Subscribe(item =>
                {
                    var split = item.Split(":");
                    ip.Text = split[0];
                    port.Text = split[1];
                    password.Text = "";
                });

            #endregion

            #region save

            var id = Settings.Secure.GetString(activity.ContentResolver, Settings.Secure.AndroidId);
            var passEncrpytion = new Encrpytion(id, Constants.EncrpytionInitVector);
            var sharedPref = activity.GetSharedPreferences(
                activity.GetString(Resource.String.preference_save_file_key), FileCreationMode.Private);

            var saveList = new List<Java.Lang.String>();
            foreach (var session in sharedPref.All.Keys)
            {
                saveList.Add(new Java.Lang.String(session));
            }

            var saveAdapter = new ArrayAdapter(
                activity,
                Android.Resource.Layout.SimpleListItem1,
                saveList);

            saveListView.Adapter = saveAdapter;

            new ObservableClickListener(save)
                .Where(pressed => ip.Text != "" && Regex.IsMatch(port.Text, @"^\d+$"))
                .Select(pressed => $"{ip.Text}:{port.Text}")
                .Subscribe(session =>
                {
                    if (!sharedPref.Contains(session))
                    {
                        saveAdapter.Add(new Java.Lang.String(session));
                        saveAdapter.NotifyDataSetChanged();
                    }
                    var pass = Translate.ByteArrayToBase64String(
                        passEncrpytion.Encrypt(
                            Translate.StringToByteArray(
                                password.Text)));
                    sharedPref.Edit().PutString(session, pass).Commit();
                });

            new ObservableOnItemLongClickListener<Java.Lang.String>(saveListView)
                .Subscribe(item => 
                {
                    var deleteBuilder = new AlertDialog.Builder(activity);
                    var title = activity.GetString(Resource.String.connect_menu_confirm_delete_title);
                    var pos = activity.GetString(Resource.String.connect_menu_confirm_delete_pos);
                    var neg = activity.GetString(Resource.String.connect_menu_confirm_delete_neg);
                    var msg = activity.GetString(Resource.String.connect_menu_confirm_delete_msg);
                    deleteBuilder.SetTitle(title);
                    deleteBuilder.SetMessage(msg);
                    deleteBuilder.SetPositiveButton(pos, (senderAlert, args) =>
                    {
                        var session = (string) item;
                        sharedPref.Edit().Remove(session).Commit();
                        saveAdapter.Remove(item);
                        saveAdapter.NotifyDataSetChanged();
                    });
                    deleteBuilder.SetNegativeButton(neg, (senderAlert, args) => { });
                    var deleteDialog = deleteBuilder.Create();
                    deleteDialog.Show();
                });

            new ObservableOnItemClickListener<Java.Lang.String>(saveListView)
                .Subscribe(item =>
                {
                    var pass = Translate.ByteArrayToString(
                        passEncrpytion.Decrypt(
                            Translate.Base64StringToByteArray(
                                sharedPref.GetString((string) item, ""))));
                    var split = item.Split(":");
                    ip.Text = split[0];
                    port.Text = split[1];
                    password.Text = pass;
                });

            #endregion

            var observableConnect = new ObservableClickListener(connect)
                .Where(pressed => ip.Text != "" && Regex.IsMatch(port.Text, @"^\d+$"));

            dialog.Show();

            observableConnect.Subscribe(pressed =>
            {
                dialog.Hide();
            });

            return observableConnect.Select(pressed => Tuple.Create(ip.Text, int.Parse(port.Text), 
                password.Text == "" ? Constants.DefaultSessionPassword : password.Text));
        }
    }
}
