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
using Com.Evrencoskun.Tableview.Adapter.Recyclerview;
using Com.Evrencoskun.Tableview.Adapter.Recyclerview.Holder;
using Com.Evrencoskun.Tableview.Util;


namespace Com.Evrencoskun.Tableview.Layoutmanager
{
    /// <summary>Created by evrencoskun on 10/06/2017.</summary>
    public class ColumnLayoutManager : LinearLayoutManager
    {
        private static readonly string
            LogTag = typeof(Com.Evrencoskun.Tableview.Layoutmanager.ColumnLayoutManager).Name;

        private ITableView mTableView;

        private CellRecyclerView mCellRowRecyclerView;

        private CellRecyclerView mColumnHeaderRecyclerView;

        private ColumnHeaderLayoutManager mColumnHeaderLayoutManager;

        private CellLayoutManager mCellLayoutManager;

        private bool mNeedFitForVerticalScroll;

        private bool mNeedFitForHorizontalScroll;

        private int mLastDx = 0;

        private int mYPosition;

        public ColumnLayoutManager(Context context, ITableView tableView) : base(context)
        {
            this.mTableView = tableView;
            this.mColumnHeaderRecyclerView = mTableView.GetColumnHeaderRecyclerView();
            this.mColumnHeaderLayoutManager = mTableView.GetColumnHeaderLayoutManager();
            this.mCellLayoutManager = mTableView.GetCellLayoutManager();
            // Set default orientation
            this.Orientation = Horizontal;
            //If you are using a RecyclerView.RecycledViewPool, it might be a good idea to set this
            // flag to true so that views will be available to other RecyclerViews immediately.
            this.RecycleChildrenOnDetach = true;
        }

        public override void OnAttachedToWindow(RecyclerView view)
        {
            base.OnAttachedToWindow(view);
            mCellRowRecyclerView = (CellRecyclerView) view;
            mYPosition = GetRowPosition();
        }

        public override void MeasureChildWithMargins(Android.Views.View child, int widthUsed, int heightUsed)
        {
            base.MeasureChildWithMargins(child, widthUsed, heightUsed);
            // If has fixed width is true, than calculation of the column width is not necessary.
            if (mTableView.HasFixedWidth())
            {
                return;
            }

            MeasureChild(child, widthUsed, heightUsed);
        }

        public override void MeasureChild(Android.Views.View child, int widthUsed, int heightUsed)
        {
            int columnPosition = GetPosition(child);
            // Get cached width size of column and cell
            int cacheWidth = mCellLayoutManager.GetCacheWidth(mYPosition, columnPosition);
            int columnCacheWidth = mColumnHeaderLayoutManager.GetCacheWidth(columnPosition);
            // Already each of them is same width size.
            if (cacheWidth != -1 && cacheWidth == columnCacheWidth)
            {
                // Control whether we need to set width or not.
                if (child.MeasuredWidth != cacheWidth)
                {
                    TableViewUtils.SetWidth(child, cacheWidth);
                }
            }
            else
            {
                Android.Views.View columnHeaderChild = mColumnHeaderLayoutManager.FindViewByPosition(columnPosition);
                if (columnHeaderChild == null)
                {
                    return;
                }

                // Need to calculate which one has the broadest width ?
                FitWidthSize(child, mYPosition, columnPosition, cacheWidth, columnCacheWidth, columnHeaderChild);
            }

            // Control all of the rows which has same column position.
            if (ShouldFitColumns(columnPosition, mYPosition))
            {
                if (mLastDx < 0)
                {
                    Log.Warn(LogTag, "x: " + columnPosition + " y: " + mYPosition + " fitWidthSize " + "left side ");
                    mCellLayoutManager.FitWidthSize(columnPosition, true);
                }
                else
                {
                    mCellLayoutManager.FitWidthSize(columnPosition, false);
                    Log.Warn(LogTag, "x: " + columnPosition + " y: " + mYPosition + " fitWidthSize " + "right side");
                }

                mNeedFitForVerticalScroll = false;
            }

            // It need to be cleared to prevent unnecessary calculation.
            mNeedFitForHorizontalScroll = false;
        }

        private void FitWidthSize(Android.Views.View child, int row, int column, int cellWidth, int columnHeaderWidth,
            Android.Views.View columnHeaderChild)
        {
            if (cellWidth == -1)
            {
                // Alternatively, TableViewUtils.getWidth(child);
                cellWidth = child.MeasuredWidth;
            }

            if (columnHeaderWidth == -1)
            {
                // Alternatively, TableViewUtils.getWidth(columnHeaderChild)
                columnHeaderWidth = columnHeaderChild.MeasuredWidth;
            }

            if (cellWidth != 0)
            {
                if (columnHeaderWidth > cellWidth)
                {
                    cellWidth = columnHeaderWidth;
                }
                else
                {
                    if (cellWidth > columnHeaderWidth)
                    {
                        columnHeaderWidth = cellWidth;
                    }
                }

                // Control whether column header needs to be change interns of width
                if (columnHeaderWidth != columnHeaderChild.Width)
                {
                    TableViewUtils.SetWidth(columnHeaderChild, columnHeaderWidth);
                    mNeedFitForVerticalScroll = true;
                    mNeedFitForHorizontalScroll = true;
                }

                // Set the value to cache it for column header.
                mColumnHeaderLayoutManager.SetCacheWidth(column, columnHeaderWidth);
            }

            // Set the width value to cache it for cell .
            TableViewUtils.SetWidth(child, cellWidth);
            mCellLayoutManager.SetCacheWidth(row, column, cellWidth);
        }

        private bool ShouldFitColumns(int xPosition, int yPosition)
        {
            if (mNeedFitForHorizontalScroll)
            {
                if (!mCellRowRecyclerView.IsScrollOthers() && mCellLayoutManager.ShouldFitColumns(yPosition))
                {
                    if (mLastDx > 0)
                    {
                        int last = FindLastVisibleItemPosition();
                        //Log.e(LOG_TAG, "Warning: findFirstVisibleItemPosition is " + last);
                        if (xPosition == last)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (mLastDx < 0)
                        {
                            int first = FindFirstVisibleItemPosition();
                            //Log.e(LOG_TAG, "Warning: findFirstVisibleItemPosition is " + first);
                            if (xPosition == first)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public override int ScrollHorizontallyBy(int dx, RecyclerView.Recycler recycler, RecyclerView.State state)
        {
            if (mColumnHeaderRecyclerView.ScrollState == RecyclerView.ScrollStateIdle &&
                mCellRowRecyclerView.IsScrollOthers())
            {
                // Every CellRowRecyclerViews should be scrolled after the ColumnHeaderRecyclerView.
                // Because it is the main compared one to make each columns fit.
                mColumnHeaderRecyclerView.ScrollBy(dx, 0);
            }

            // It is important to determine the next attached view to fit all columns
            mLastDx = dx;
            // Set the right initialPrefetch size to improve performance
            this.InitialPrefetchItemCount = 2;
            return base.ScrollHorizontallyBy(dx, recycler, state);
        }

        public int RowPosition => GetRowPosition();

        private int GetRowPosition()
        {
            return mCellLayoutManager.GetPosition(mCellRowRecyclerView);
        }

        public int LastDx => GetLastDx();

        public virtual int GetLastDx()
        {
            return mLastDx;
        }

        public bool NeedFit => IsNeedFit();

        public virtual bool IsNeedFit()
        {
            return mNeedFitForVerticalScroll;
        }

        public virtual void ClearNeedFit()
        {
            mNeedFitForVerticalScroll = false;
        }

        public virtual AbstractViewHolder[] GetVisibleViewHolders()
        {
            int visibleChildCount = FindLastVisibleItemPosition() - FindFirstVisibleItemPosition() + 1;
            int index = 0;
            AbstractViewHolder[] views = new AbstractViewHolder[visibleChildCount];
            for (int i = FindFirstVisibleItemPosition(); i < FindLastVisibleItemPosition() + 1; i++)
            {
                views[index] = (AbstractViewHolder) mCellRowRecyclerView.FindViewHolderForAdapterPosition(i);
                index++;
            }

            return views;
        }
    }
}
