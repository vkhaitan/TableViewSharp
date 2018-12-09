/*
* Copyright (c) 2018. Evren Coşkun
*
*  Licensed under the Apache License, Version 2.0 (the "License");
*  you may not use this file except in compliance with the License.
*  You may obtain a copy of the License at
*
*        http://www.apache.org/licenses/LICENSE-2.0
*
*  Unless required by applicable law or agreed to in writing, software
*  distributed under the License is distributed on an "AS IS" BASIS,
*  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*  See the License for the specific language governing permissions and
*  limitations under the License.
*
*/

using Android.Content;
using Android.Support.V7.Widget;
using Android.Util;
using Com.Evrencoskun.Tableview;
using Com.Evrencoskun.Tableview.Listener.Scroll;
using TableViewSharp;

namespace Com.Evrencoskun.Tableview.Adapter.Recyclerview
{
    /// <summary>Created by evrencoskun on 19/06/2017.</summary>
    public class CellRecyclerView : RecyclerView
    {
        private static readonly string LogTag = typeof(Com.Evrencoskun.Tableview.Adapter.Recyclerview.CellRecyclerView)
            .Name;

        private int mScrolledX = 0;

        private int mScrolledY = 0;

        private bool mIsHorizontalScrollListenerRemoved = true;

        private bool mIsVerticalScrollListenerRemoved = true;

        public CellRecyclerView(Context context) : base(context)
        {
            // These are necessary.
            this.HasFixedSize = false;
            this.NestedScrollingEnabled = false;
            // These are for better scrolling process.
            this.SetItemViewCacheSize(context.Resources.GetInteger(Resource.Integer.default_item_cache_size));
            DrawingCacheEnabled = true;
            DrawingCacheQuality = Android.Views.DrawingCacheQuality.High;
        }

        public override void OnScrolled(int dx, int dy)
        {
            mScrolledX += dx;
            mScrolledY += dy;
            base.OnScrolled(dx, dy);
        }

        public int ScrolledX => GetScrolledX();

        public virtual int GetScrolledX()
        {
            return mScrolledX;
        }

        public int ScrolledY => GetScrolledY();

        public virtual void ClearScrolledX()
        {
            mScrolledX = 0;
        }

        public virtual int GetScrolledY()
        {
            return mScrolledY;
        }

        public override void AddOnScrollListener(RecyclerView.OnScrollListener listener)
        {
            if (listener is HorizontalRecyclerViewListener)
            {
                if (mIsHorizontalScrollListenerRemoved)
                {
                    mIsHorizontalScrollListenerRemoved = false;
                    base.AddOnScrollListener(listener);
                }
                else
                {
                    // Do not let add the listener
                    Log.Warn(LogTag,
                        "mIsHorizontalScrollListenerRemoved has been tried to add itself " +
                        "before remove the old one");
                }
            }
            else
            {
                if (listener is VerticalRecyclerViewListener)
                {
                    if (mIsVerticalScrollListenerRemoved)
                    {
                        mIsVerticalScrollListenerRemoved = false;
                        base.AddOnScrollListener(listener);
                    }
                    else
                    {
                        // Do not let add the listener
                        Log.Warn(LogTag,
                            "mIsVerticalScrollListenerRemoved has been tried to add itself " +
                            "before remove the old one");
                    }
                }
                else
                {
                    base.AddOnScrollListener(listener);
                }
            }
        }

        public override void RemoveOnScrollListener(RecyclerView.OnScrollListener listener)
        {
            if (listener is HorizontalRecyclerViewListener)
            {
                if (mIsHorizontalScrollListenerRemoved)
                {
                    // Do not let remove the listener
                    Log.Error(LogTag,
                        "HorizontalRecyclerViewListener has been tried to remove " + "itself before add new one");
                }
                else
                {
                    mIsHorizontalScrollListenerRemoved = true;
                    base.RemoveOnScrollListener(listener);
                }
            }
            else
            {
                if (listener is VerticalRecyclerViewListener)
                {
                    if (mIsVerticalScrollListenerRemoved)
                    {
                        // Do not let remove the listener
                        Log.Error(LogTag,
                            "mIsVerticalScrollListenerRemoved has been tried to remove " + "itself before add new one");
                    }
                    else
                    {
                        mIsVerticalScrollListenerRemoved = true;
                        base.RemoveOnScrollListener(listener);
                    }
                }
                else
                {
                    base.RemoveOnScrollListener(listener);
                }
            }
        }

        public bool HorizontalScrollListenerRemoved => mIsHorizontalScrollListenerRemoved;

        public virtual bool IsHorizontalScrollListenerRemoved()
        {
            return mIsHorizontalScrollListenerRemoved;
        }

        public bool ScrollOthers => IsScrollOthers();

        public virtual bool IsScrollOthers()
        {
            return !mIsHorizontalScrollListenerRemoved;
        }

        /// <summary>Begin a standard fling with an initial velocity along each axis in pixels per second.
        /// 	</summary>
        /// <remarks>
        /// Begin a standard fling with an initial velocity along each axis in pixels per second.
        /// If the velocity given is below the system-defined minimum this method will return false
        /// and no fling will occur.
        /// </remarks>
        /// <param name="velocityX">Initial horizontal velocity in pixels per second</param>
        /// <param name="velocityY">Initial vertical velocity in pixels per second</param>
        /// <returns>
        /// true if the fling was started, false if the velocity was too low to fling or
        /// LayoutManager does not support scrolling in the axis fling is issued.
        /// </returns>
        /// <seealso cref="Android.Support.V7.Widget.RecyclerView.LayoutManager.CanScrollVertically()
        /// 	"/>
        /// <seealso cref="Android.Support.V7.Widget.RecyclerView.LayoutManager.CanScrollHorizontally()
        /// 	"/>
        public override bool Fling(int velocityX, int velocityY)
        {
            // Adjust speeds to be able to provide smoother scroll.
            //velocityX *= 0.6;
            //velocityY *= 0.6;
            return base.Fling(velocityX, velocityY);
        }
    }
}
