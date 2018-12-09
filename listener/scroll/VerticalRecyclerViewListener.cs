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

using System;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Com.Evrencoskun.Tableview;
using Com.Evrencoskun.Tableview.Adapter.Recyclerview;


namespace Com.Evrencoskun.Tableview.Listener.Scroll
{
    /// <summary>Created by evrencoskun on 30/06/2017.</summary>
    public class VerticalRecyclerViewListener : RecyclerView.OnScrollListener, RecyclerView.IOnItemTouchListener
    {
        private static readonly string LogTag =
            typeof(Com.Evrencoskun.Tableview.Listener.Scroll.VerticalRecyclerViewListener).Name;

        private CellRecyclerView mRowHeaderRecyclerView;

        private CellRecyclerView mCellRecyclerView;

        private RecyclerView mLastTouchedRecyclerView;

        private int mYPosition;

        private bool mIsMoved;

        private RecyclerView mCurrentRVTouched = null;

        public VerticalRecyclerViewListener(ITableView tableView)
        {
            // Y Position means row position
            this.mRowHeaderRecyclerView = tableView.GetRowHeaderRecyclerView();
            this.mCellRecyclerView = tableView.GetCellRecyclerView();
        }

        private float dx = 0;

        private float dy = 0;

        /// <summary>check which direction the user is scrolling</summary>
        /// <param name="ev"/>
        /// <returns/>
        private bool VerticalDirection(MotionEvent ev)
        {
            if (ev.Action == MotionEventActions.Move)
            {
                if (dx == 0)
                {
                    dx = ev.GetX();
                }

                if (dy == 0)
                {
                    dy = ev.GetY();
                }

                float xdiff = Math.Abs(dx - ev.GetX());
                float ydiff = Math.Abs(dy - ev.GetY());
                dx = ev.GetX();
                dy = ev.GetY();
                // if user scrolled more horizontally than vertically
                if (xdiff > ydiff)
                {
                    return false;
                }
            }

            return true;
        }

        public virtual bool OnInterceptTouchEvent(RecyclerView rv, MotionEvent e)
        {
            // Prevent multitouch, once we start to listen with a RV,
            // we ignore any other RV until the touch is released (UP)
            if ((mCurrentRVTouched != null && rv != mCurrentRVTouched))
            {
                return true;
            }

            // If scroll direction is not Vertical, then ignore and reset last RV touched
            if (!VerticalDirection(e))
            {
                mCurrentRVTouched = null;
                return false;
            }

            if (e.Action == MotionEventActions.Down)
            {
                mCurrentRVTouched = rv;
                if (rv.ScrollState == RecyclerView.ScrollStateIdle)
                {
                    if (mLastTouchedRecyclerView != null && rv != mLastTouchedRecyclerView)
                    {
                        RemoveLastTouchedRecyclerViewScrollListener(false);
                    }

                    mYPosition = ((CellRecyclerView) rv).GetScrolledY();
                    rv.AddOnScrollListener(this);
                    if (rv == mCellRecyclerView)
                    {
                        Log.Debug(LogTag, "mCellRecyclerView scroll listener added");
                    }
                    else
                    {
                        if (rv == mRowHeaderRecyclerView)
                        {
                            Log.Debug(LogTag, "mRowHeaderRecyclerView scroll listener added");
                        }
                    }

                    // Refresh the value;
                    mIsMoved = false;
                }
            }
            else
            {
                if (e.Action == MotionEventActions.Move)
                {
                    mCurrentRVTouched = rv;
                    // Why does it matter ?
                    // user scroll any recyclerView like brushing, at that time, ACTION_UP will be
                    // triggered
                    // before scrolling. So, we need to store whether it moved or not.
                    mIsMoved = true;
                }
                else
                {
                    if (e.Action == MotionEventActions.Up)
                    {
                        mCurrentRVTouched = null;
                        int nScrollY = ((CellRecyclerView) rv).GetScrolledY();
                        // TODO: Even if moved value is true and it may not scroll. This should be fixed.
                        // TODO: The scenario is scroll lightly center RecyclerView vertically.
                        // TODO: Below if condition may be changed later.
                        // Is it just touched without scrolling then remove the listener
                        if (mYPosition == nScrollY && !mIsMoved && rv.ScrollState == RecyclerView.ScrollStateIdle)
                        {
                            rv.RemoveOnScrollListener(this);
                            if (rv == mCellRecyclerView)
                            {
                                Log.Debug(LogTag, "mCellRecyclerView scroll listener removed from up ");
                            }
                            else
                            {
                                if (rv == mRowHeaderRecyclerView)
                                {
                                    Log.Debug(LogTag, "mRowHeaderRecyclerView scroll listener removed from up");
                                }
                            }
                        }

                        mLastTouchedRecyclerView = rv;
                    }
                }
            }

            return false;
        }

        public virtual void OnTouchEvent(RecyclerView rv, MotionEvent e)
        {
        }

        public virtual void OnRequestDisallowInterceptTouchEvent(bool disallowIntercept)
        {
        }

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            // CellRecyclerViews should be scrolled after the RowHeaderRecyclerView.
            // Because it is one of the main compared criterion to make each columns fit.
            if (recyclerView == mCellRecyclerView)
            {
                base.OnScrolled(recyclerView, dx, dy);
            }
            else
            {
                // The below code has been moved in CellLayoutManager
                //mRowHeaderRecyclerView.scrollBy(0, dy);
                if (recyclerView == mRowHeaderRecyclerView)
                {
                    base.OnScrolled(recyclerView, dx, dy);
                    mCellRecyclerView.ScrollBy(0, dy);
                }
            }
        }

        public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
        {
            base.OnScrollStateChanged(recyclerView, newState);
            if (newState == RecyclerView.ScrollStateIdle)
            {
                recyclerView.RemoveOnScrollListener(this);
                mIsMoved = false;
                mCurrentRVTouched = null;
                if (recyclerView == mCellRecyclerView)
                {
                    Log.Debug(LogTag, "mCellRecyclerView scroll listener removed from " + "onScrollStateChanged");
                }
                else
                {
                    if (recyclerView == mRowHeaderRecyclerView)
                    {
                        Log.Debug(LogTag,
                            "mRowHeaderRecyclerView scroll listener removed from " + "onScrollStateChanged");
                    }
                }
            }
        }

        /// <summary>
        /// If current recyclerView that is touched to scroll is not same as the last one, this method
        /// helps to remove the scroll listener of the last touched recyclerView.
        /// </summary>
        /// <remarks>
        /// If current recyclerView that is touched to scroll is not same as the last one, this method
        /// helps to remove the scroll listener of the last touched recyclerView.
        /// This method is a little bit different from HorizontalRecyclerViewListener.
        /// </remarks>
        /// <param name="isNeeded">
        /// Is mCellRecyclerView scroll listener should be removed ? The scenario is a
        /// user scrolls vertically using RowHeaderRecyclerView. After that, the user
        /// scrolls horizontally using ColumnHeaderRecyclerView.
        /// </param>
        public virtual void RemoveLastTouchedRecyclerViewScrollListener(bool isNeeded)
        {
            if (mLastTouchedRecyclerView == mCellRecyclerView)
            {
                mCellRecyclerView.RemoveOnScrollListener(this);
                mCellRecyclerView.StopScroll();
                Log.Debug(LogTag, "mCellRecyclerView scroll listener removed from last touched");
            }
            else
            {
                mRowHeaderRecyclerView.RemoveOnScrollListener(this);
                mRowHeaderRecyclerView.StopScroll();
                Log.Debug(LogTag, "mRowHeaderRecyclerView scroll listener removed from last touched");
                if (isNeeded)
                {
                    mCellRecyclerView.RemoveOnScrollListener(this);
                    mCellRecyclerView.StopScroll();
                    Log.Debug(LogTag, "mCellRecyclerView scroll listener removed from last touched");
                }
            }
        }
    }
}
