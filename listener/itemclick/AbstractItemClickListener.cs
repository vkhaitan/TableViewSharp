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
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Com.Evrencoskun.Tableview;
using Com.Evrencoskun.Tableview.Adapter.Recyclerview;
using Com.Evrencoskun.Tableview.Handler;
using Com.Evrencoskun.Tableview.Listener;


namespace Com.Evrencoskun.Tableview.Listener.Itemclick
{
    /// <summary>Created by evrencoskun on 22.11.2017.</summary>
    public abstract class AbstractItemClickListener : Java.Lang.Object, RecyclerView.IOnItemTouchListener
    {
        private ITableViewListener mListener;

        protected internal GestureDetector mGestureDetector;

        protected internal CellRecyclerView mRecyclerView;

        protected internal SelectionHandler mSelectionHandler;

        protected internal ITableView mTableView;


        public AbstractItemClickListener(CellRecyclerView recyclerView, ITableView tableView)
        {
            this.mRecyclerView = recyclerView;
            this.mTableView = tableView;
            this.mSelectionHandler = tableView.GetSelectionHandler();
            mGestureDetector = new GestureDetector(mRecyclerView.Context, new _SimpleOnGestureListener_46(this));
        }

        private sealed class _SimpleOnGestureListener_46 : GestureDetector.SimpleOnGestureListener
        {
            public _SimpleOnGestureListener_46(AbstractItemClickListener _enclosing)
            {
                this._enclosing = _enclosing;
            }

            internal MotionEvent start;

            public override bool OnSingleTapUp(MotionEvent e)
            {
                return true;
            }

            public override bool OnSingleTapConfirmed(MotionEvent e)
            {
                return this._enclosing.ClickAction(this._enclosing.mRecyclerView, e);
            }

            public override bool OnDown(MotionEvent e)
            {
                this.start = e;
                return false;
            }

            public override void OnLongPress(MotionEvent e)
            {
                // Check distance to prevent scroll to trigger the event
                if (this.start != null && Math.Abs(this.start.RawX - e.RawX) < 20 &&
                    Math.Abs(this.start.RawY - e.RawY) < 20)
                {
                    this._enclosing.LongPressAction(e);
                }
            }

            private readonly AbstractItemClickListener _enclosing;
        }

        public virtual bool OnInterceptTouchEvent(RecyclerView view, MotionEvent e)
        {
            return mGestureDetector.OnTouchEvent(e);
        }

        public virtual void OnTouchEvent(RecyclerView view, MotionEvent motionEvent)
        {
        }

        public virtual void OnRequestDisallowInterceptTouchEvent(bool disallowIntercept)
        {
        }

        protected internal virtual ITableViewListener GetTableViewListener()
        {
            if (mListener == null)
            {
                mListener = mTableView.GetTableViewListener();
            }

            return mListener;
        }

        protected internal abstract bool ClickAction(RecyclerView view, MotionEvent e);

        protected internal abstract void LongPressAction(MotionEvent e);
    }
}
