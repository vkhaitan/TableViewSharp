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

using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Com.Evrencoskun.Tableview;
using Com.Evrencoskun.Tableview.Adapter.Recyclerview;


namespace Com.Evrencoskun.Tableview.Listener.Scroll
{
    /// <summary>Created by evrencoskun on 19/06/2017.</summary>
    public class HorizontalRecyclerViewListener : RecyclerView.OnScrollListener, RecyclerView.IOnItemTouchListener
    {
        private static readonly string LogTag =
            typeof(Com.Evrencoskun.Tableview.Listener.Scroll.HorizontalRecyclerViewListener).Name;

        private CellRecyclerView mColumnHeaderRecyclerView;

        private RecyclerView.LayoutManager mCellLayoutManager;

        private RecyclerView mLastTouchedRecyclerView;

        private int mXPosition;

        private bool mIsMoved;

        private int mScrollPosition;

        private int mScrollPositionOffset = 0;

        private RecyclerView mCurrentRVTouched = null;

        private VerticalRecyclerViewListener mVerticalRecyclerViewListener;

        public HorizontalRecyclerViewListener(ITableView tableView)
        {
            // X position means column position
            this.mColumnHeaderRecyclerView = tableView.GetColumnHeaderRecyclerView();
            this.mCellLayoutManager = tableView.GetCellRecyclerView().GetLayoutManager();
            this.mVerticalRecyclerViewListener = tableView.GetVerticalRecyclerViewListener();
        }

        public virtual bool OnInterceptTouchEvent(RecyclerView rv, MotionEvent e)
        {
            // Prevent multitouch, once we start to listen with a RV,
            // we ignore any other RV until the touch is released (UP)
            if (mCurrentRVTouched != null && rv != mCurrentRVTouched)
            {
                return true;
            }

            if (e.Action == MotionEventActions.Down)
            {
                mCurrentRVTouched = rv;
                if (rv.ScrollState == RecyclerView.ScrollStateIdle)
                {
                    if (mLastTouchedRecyclerView != null && rv != mLastTouchedRecyclerView)
                    {
                        if (mLastTouchedRecyclerView == mColumnHeaderRecyclerView)
                        {
                            mColumnHeaderRecyclerView.RemoveOnScrollListener(this);
                            mColumnHeaderRecyclerView.StopScroll();
                            Log.Debug(LogTag,
                                "Scroll listener  has been removed to " +
                                "mColumnHeaderRecyclerView at last touch control");
                        }
                        else
                        {
                            int lastTouchedIndex = GetIndex(mLastTouchedRecyclerView);
                            // Control whether the last touched recyclerView is still attached or not
                            if (lastTouchedIndex >= 0 && lastTouchedIndex < mCellLayoutManager.ChildCount)
                            {
                                // Control the scroll listener is already removed. For instance; if user
                                // scroll the parent recyclerView vertically, at that time,
                                // ACTION_CANCEL
                                // will be triggered that removes the scroll listener of the last
                                // touched
                                // recyclerView.
                                if (!((CellRecyclerView) mLastTouchedRecyclerView).IsHorizontalScrollListenerRemoved())
                                {
                                    // Remove scroll listener of the last touched recyclerView
                                    // Because user touched another recyclerView before the last one get
                                    // SCROLL_STATE_IDLE state that removes the scroll listener
                                    ((RecyclerView) mCellLayoutManager.GetChildAt(lastTouchedIndex))
                                        .RemoveOnScrollListener(this);
                                    Log.Debug(LogTag,
                                        "Scroll listener  has been removed to " + mLastTouchedRecyclerView.Id +
                                        " CellRecyclerView " + "at last touch control");
                                    // the last one scroll must be stopped to be sync with others
                                    ((RecyclerView) mCellLayoutManager.GetChildAt(lastTouchedIndex)).StopScroll();
                                }
                            }
                        }
                    }

                    mXPosition = ((CellRecyclerView) rv).GetScrolledX();
                    rv.AddOnScrollListener(this);
                    Log.Debug(LogTag, "Scroll listener  has been added to " + rv.Id + " at action " + "down");
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
                        int nScrollX = ((CellRecyclerView) rv).GetScrolledX();
                        // Is it just touched without scrolling then remove the listener
                        if (mXPosition == nScrollX && !mIsMoved)
                        {
                            rv.RemoveOnScrollListener(this);
                            Log.Debug(LogTag,
                                "Scroll listener  has been removed to " + rv.Id + " at " + "action" + " up");
                        }

                        mLastTouchedRecyclerView = rv;
                    }
                    else
                    {
                        if (e.Action == MotionEventActions.Cancel)
                        {
                            // ACTION_CANCEL action will be triggered if users try to scroll vertically
                            // For this situation, it doesn't matter whether the x position is changed or not
                            // Beside this, even if moved action will be triggered, scroll listener won't
                            // triggered on cancel action. So, we need to change state of the mIsMoved value as
                            // well.
                            // Renew the scroll position and its offset
                            RenewScrollPosition(rv);
                            rv.RemoveOnScrollListener(this);
                            Log.Debug(LogTag,
                                "Scroll listener  has been removed to " + rv.Id + " at action " + "cancel");
                            mIsMoved = false;
                            mLastTouchedRecyclerView = rv;
                            mCurrentRVTouched = null;
                        }
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
            // Column Header should be scrolled firstly. Because it is the compared recyclerView to
            // make column width fit.
            if (recyclerView == mColumnHeaderRecyclerView)
            {
                base.OnScrolled(recyclerView, dx, dy);
                // Scroll each cell recyclerViews
                for (int i = 0; i < mCellLayoutManager.ChildCount; i++)
                {
                    CellRecyclerView child = (CellRecyclerView) mCellLayoutManager.GetChildAt(i);
                    // Scroll horizontally
                    child.ScrollBy(dx, 0);
                }
            }
            else
            {
                // Scroll column header recycler view as well
                //mColumnHeaderRecyclerView.scrollBy(dx, 0);
                base.OnScrolled(recyclerView, dx, dy);
                // Scroll each cell recyclerViews except the current touched one
                for (int i = 0; i < mCellLayoutManager.ChildCount; i++)
                {
                    CellRecyclerView child = (CellRecyclerView) mCellLayoutManager.GetChildAt(i);
                    if (child != recyclerView)
                    {
                        // Scroll horizontally
                        child.ScrollBy(dx, 0);
                    }
                }
            }
        }

        public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
        {
            base.OnScrollStateChanged(recyclerView, newState);
            if (newState == RecyclerView.ScrollStateIdle)
            {
                // Renew the scroll position and its offset
                RenewScrollPosition(recyclerView);
                recyclerView.RemoveOnScrollListener(this);
                Log.Debug(LogTag,
                    "Scroll listener has been removed to " + recyclerView.Id + " at " + "onScrollStateChanged");
                mIsMoved = false;
                // When a user scrolls horizontally, VerticalRecyclerView add vertical scroll
                // listener because of touching process.However, mVerticalRecyclerViewListener
                // doesn't know anything about it. So, it is necessary to remove the last touched
                // recyclerView which uses the mVerticalRecyclerViewListener.
                bool isNeeded = mLastTouchedRecyclerView != mColumnHeaderRecyclerView;
                mVerticalRecyclerViewListener.RemoveLastTouchedRecyclerViewScrollListener(isNeeded);
            }
        }

        private int GetIndex(RecyclerView rv)
        {
            for (int i = 0; i < mCellLayoutManager.ChildCount; i++)
            {
                if (mCellLayoutManager.GetChildAt(i) == rv)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// This method calculates the current scroll position and its offset to help new attached
        /// recyclerView on window at that position and offset
        /// </summary>
        /// <seealso cref="GetScrollPosition()"/>
        /// <seealso cref="GetScrollPositionOffset()"/>
        private void RenewScrollPosition(RecyclerView recyclerView)
        {
            LinearLayoutManager layoutManager = (LinearLayoutManager) recyclerView.GetLayoutManager();
            mScrollPosition = layoutManager.FindFirstCompletelyVisibleItemPosition();
            // That means there is no completely visible Position.
            if (mScrollPosition == -1)
            {
                mScrollPosition = layoutManager.FindFirstVisibleItemPosition();
                // That means there is just a visible item on the screen
                if (mScrollPosition == layoutManager.FindLastVisibleItemPosition())
                {
                }
                else
                {
                    // in this case we use the position which is the last & first visible item.
                    // That means there are 2 visible item on the screen. However, second one is not
                    // completely visible.
                    mScrollPosition = mScrollPosition + 1;
                }
            }

            mScrollPositionOffset = layoutManager.FindViewByPosition(mScrollPosition).Left;
        }

        /// <summary>
        /// When parent RecyclerView scrolls vertically, the child horizontal recycler views should be
        /// displayed on right scroll position.
        /// </summary>
        /// <remarks>
        /// When parent RecyclerView scrolls vertically, the child horizontal recycler views should be
        /// displayed on right scroll position. So the first complete visible position of the
        /// recyclerView is stored as a member to use it for a new attached recyclerview whose
        /// orientation is horizontal as well.
        /// </remarks>
        /// <seealso cref="GetScrollPositionOffset()"/>
        public virtual int GetScrollPosition()
        {
            return mScrollPosition;
        }

        /// <summary>Users can scroll the recyclerViews to the any x position which may not the exact position.
        /// 	</summary>
        /// <remarks>
        /// Users can scroll the recyclerViews to the any x position which may not the exact position. So
        /// we need to know store the offset value to locate a specific location for a new attached
        /// recyclerView
        /// </remarks>
        /// <seealso cref="GetScrollPosition()"/>
        public virtual int GetScrollPositionOffset()
        {
            return mScrollPositionOffset;
        }

        public virtual void SetScrollPositionOffset(int offset)
        {
            mScrollPositionOffset = offset;
        }

        /// <summary>To change default scroll position that is before TableView is not populated.
        /// 	</summary>
        public virtual void SetScrollPosition(int position)
        {
            this.mScrollPosition = position;
        }
    }
}
