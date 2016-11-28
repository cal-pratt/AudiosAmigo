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
    public class TcpConnectionViewHolder : RecyclerView.ViewHolder
    {
        public TextView Name { get; private set; }

        public TextView Host { get; private set; }

        public TextView Port { get; private set; }

        public TcpConnectionViewHolder(View itemView) : base (itemView)
        {
            Name = itemView.FindViewById<TextView>(Resource.Id.name);
            Host = itemView.FindViewById<TextView>(Resource.Id.ip_address);
            Port = itemView.FindViewById<TextView>(Resource.Id.port);
        }
    }
}