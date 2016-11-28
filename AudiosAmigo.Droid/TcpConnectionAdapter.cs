using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace AudiosAmigo.Droid
{
    public class TcpConnectionAdapter : RecyclerView.Adapter
    {
        private readonly List<TcpConnectionInfo> _connections;

        public TcpConnectionAdapter(List<TcpConnectionInfo> connections)
        {
            _connections = connections;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new TcpConnectionViewHolder(
                LayoutInflater.From(parent.Context).Inflate(
                    Resource.Layout.TcpConnectionCardView, parent, false));
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = holder as TcpConnectionViewHolder;
            vh.Name.Text = _connections[position].Name;
            vh.Host.Text = _connections[position].Host;
            vh.Port.Text = _connections[position].Port.ToString();
        }

        public override int ItemCount => _connections.Count;
    }
}