using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Views;
using Android.Widget;

namespace AudiosAmigo.Droid
{
    public class ItemTouchHelperCallback : ItemTouchHelper.SimpleCallback
    {
        public class EventArgs : System.EventArgs
        {
            public int AdapterPosition { get; }

            public EventArgs(int adapterPosition)
            {
                AdapterPosition = adapterPosition;
            }
        }

        public event EventHandler<EventArgs> SwipeCalled;
        
        public event EventHandler<EventArgs> MoveCalled;

        public ItemTouchHelperCallback(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {

        }

        public ItemTouchHelperCallback(int dragDirs, int swipeDirs) : base(dragDirs, swipeDirs)
        {
        }

        public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target)
        {
            OnMoveCalled(new EventArgs(viewHolder.AdapterPosition));
            return false;
        }

        public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
        {
            OnSwipeCalled(new EventArgs(viewHolder.AdapterPosition));
        }

        protected virtual void OnSwipeCalled(EventArgs e)
        {
            SwipeCalled?.Invoke(this, e);
        }

        protected virtual void OnMoveCalled(EventArgs e)
        {
            MoveCalled?.Invoke(this, e);
        }
    }
}