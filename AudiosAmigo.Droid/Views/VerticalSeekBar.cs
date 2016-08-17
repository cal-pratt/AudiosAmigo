using System;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace AudiosAmigo.Droid.Views
{
    public class VerticalSeekBar : SeekBar
    {
        private int _min;

        public int Min
        {
            get { return _min; }
            set
            {
                _min = value;
                if (_min > Progress)
                {
                    Progress = _min;
                }
                OnSizeChanged(Width, Height, 0, 0);
            }
        }

        public override int Progress
        {
            get
            {
                return base.Progress.Clamp(Min, Max);
            }
            set
            {
                base.Progress = value.Clamp(Min, Max);
                OnSizeChanged(Width, Height, 0, 0);
            }
        }

        public Vibrator Vibrator { get; set; }

        protected VerticalSeekBar(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer){ }

        public VerticalSeekBar(Context context) : base(context){ }

        public VerticalSeekBar(Context context, IAttributeSet attrs) : base(context, attrs){ }

        public VerticalSeekBar(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle){ }
        
        
        public override void Draw(Android.Graphics.Canvas canvas)
        {
            canvas.Rotate(-90);
            canvas.Translate(-Height, 0);
            OnDraw(canvas);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(heightMeasureSpec, widthMeasureSpec);
            SetMeasuredDimension(MeasuredHeight, MeasuredWidth);
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(h, w, oldh, oldw);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (!Enabled)
            {
                return false;
            }

            if (e.Action == MotionEventActions.Down)
            {
                if (!(Height - e.GetY() >= Thumb.Bounds.Left && Height - e.GetY() <= Thumb.Bounds.Right
                    && e.GetX() <= Thumb.Bounds.Bottom && e.GetX() >= Thumb.Bounds.Top))
                {
                    return false;
                }
                Selected = Pressed = true;
                Vibrator?.Vibrate(10);
            }
            else if (e.Action == MotionEventActions.Up || e.Action == MotionEventActions.Cancel)
            {
                Selected = Pressed = false;
            }

            if (e.Action == MotionEventActions.Move ||
                e.Action == MotionEventActions.Down ||
                e.Action == MotionEventActions.Up)
            {
                Progress = Max - (int)(Max * (e.GetY() - Thumb.Bounds.Height()) / (Height - Thumb.Bounds.Height()));
            }

            return true;
        }
    }

    internal static class Extension
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            return val.CompareTo(min) < 0 ? min : val.CompareTo(max) > 0 ? max : val;
        }
    }
}
