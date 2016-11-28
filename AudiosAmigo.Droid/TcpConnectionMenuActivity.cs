using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Views;
using AudiosAmigo.Droid;

namespace AudiosAmigo.Droid2
{
    [Activity(Label = "Choose A Connection")]
    public class TcpConnectionMenuActivity : AppCompatActivity
    {
        private ISharedPreferences _preferences;

        private List<TcpConnectionInfo> _connections;

        private RecyclerView _recyclerView;

        private TcpConnectionAdapter _adapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.TcpConnectionMenu);
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            _preferences = GetSharedPreferences("TcpConnectionMenuActivity", FileCreationMode.Private);

            _connections = new List<TcpConnectionInfo>();
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.connection_recycler_view);
            _recyclerView.SetItemAnimator(new DefaultItemAnimator());
            
            var layoutManager = new LinearLayoutManager(this);
            _recyclerView.SetLayoutManager(layoutManager);
            
            _adapter = new TcpConnectionAdapter(_connections);
            _recyclerView.SetAdapter(_adapter);

            var fab = FindViewById<FloatingActionButton>(Resource.Id.connection_fab);
            int i = 1;
            fab.Click += (s, e) =>
            {
                _connections.Insert(0, new TcpConnectionInfo(i++.ToString(), "d", 5678, "test"));
                _adapter.NotifyItemInserted(0);
                _recyclerView.ScrollToPosition(0);
                CommitSharedPrefs();
            };

            var helperCallback = new ItemTouchHelperCallback(0,  ItemTouchHelper.Left | ItemTouchHelper.Right);
            helperCallback.SwipeCalled += (s, e) =>
            {
                _connections.RemoveAt(e.AdapterPosition);
                _adapter.NotifyItemRemoved(e.AdapterPosition);
                _recyclerView.ScrollToPosition(e.AdapterPosition);
                CommitSharedPrefs();
            };
            var itemTouchHelper = new ItemTouchHelper(helperCallback);
            itemTouchHelper.AttachToRecyclerView(_recyclerView);

            LoadSharedPrefs();
        }

        private void CommitSharedPrefs()
        {
            _preferences.Edit().PutString("savedConnections", Translate.ObjectToString(_connections)).Commit();
        }

        private void LoadSharedPrefs()
        {
            var savedConnectionsString = _preferences.GetString("savedConnections", 
                Translate.ObjectToString(new List<TcpConnectionInfo>()));
            _connections.Clear();
            foreach (var connection in Translate.StringToObject<List<TcpConnectionInfo>>(savedConnectionsString))
            {
                _connections.Add(connection);
            }
            _adapter.NotifyDataSetChanged();
            _recyclerView.ScrollToPosition(0);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}