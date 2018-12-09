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
using System.Collections;
using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Com.Evrencoskun.Tableview;
using Com.Evrencoskun.Tableview.Adapter.Recyclerview;
using Com.Evrencoskun.Tableview.Adapter.Recyclerview.Holder;
using Com.Evrencoskun.Tableview.Listener.Scroll;
using Com.Evrencoskun.Tableview.Util;


namespace Com.Evrencoskun.Tableview.Layoutmanager
{
    /// <summary>Created by evrencoskun on 24/06/2017.</summary>
    public class CellLayoutManager : LinearLayoutManager
    {
        private static readonly string LogTag = typeof(Com.Evrencoskun.Tableview.Layoutmanager.CellLayoutManager).Name;

        private const int IgnoreLeft = -99999;

        private ColumnHeaderLayoutManager mColumnHeaderLayoutManager;

        private LinearLayoutManager mRowHeaderLayoutManager;

        private CellRecyclerView mRowHeaderRecyclerView;

        private CellRecyclerView mCellRecyclerView;

        private HorizontalRecyclerViewListener mHorizontalListener;

        private ITableView mTableView;

        private readonly IDictionary<int, IDictionary<int, int>> mCachedWidthList =
            new Dictionary<int, IDictionary<int, int>>();

        private int mLastDy = 0;

        private bool mNeedSetLeft;

        private bool mNeedFit;

        public CellLayoutManager(Context context, ITableView tableView) : base(context)
        {
            //TODO: Store a single instance for both cell and column cache width values.
            this.mTableView = tableView;
            this.mCellRecyclerView = tableView.GetCellRecyclerView();
            this.mColumnHeaderLayoutManager = tableView.GetColumnHeaderLayoutManager();
            this.mRowHeaderLayoutManager = tableView.GetRowHeaderLayoutManager();
            this.mRowHeaderRecyclerView = tableView.GetRowHeaderRecyclerView();
            Initialize();
        }

        private void Initialize()
        {
            this.Orientation = Vertical;
        }

        // Add new one
        public override void OnAttachedToWindow(RecyclerView view)
        {
            base.OnAttachedToWindow(view);
            // initialize the instances
            if (mCellRecyclerView == null)
            {
                mCellRecyclerView = mTableView.GetCellRecyclerView();
            }

            if (mHorizontalListener == null)
            {
                mHorizontalListener = mTableView.GetHorizontalRecyclerViewListener();
            }
        }

        public override int ScrollVerticallyBy(int dy, RecyclerView.Recycler recycler, RecyclerView.State state)
        {
            if (mRowHeaderRecyclerView.ScrollState == RecyclerView.ScrollStateIdle &&
                !mRowHeaderRecyclerView.IsScrollOthers())
            {
                // CellRecyclerViews should be scrolled after the RowHeaderRecyclerView.
                // Because it is one of the main compared criterion to make each columns fit.
                mRowHeaderRecyclerView.ScrollBy(0, dy);
            }

            int scroll = base.ScrollVerticallyBy(dy, recycler, state);
            // It is important to determine right position to fit all columns which are the same y pos.
            mLastDy = dy;
            return scroll;
        }

        public override void OnScrollStateChanged(int state)
        {
            base.OnScrollStateChanged(state);
            if (state == RecyclerView.ScrollStateIdle)
            {
                // It is important to set it 0 to be able to know which direction is being scrolled
                mLastDy = 0;
            }
        }

        /// <summary>This method helps to fit all columns which are displayed on screen.</summary>
        /// <remarks>
        /// This method helps to fit all columns which are displayed on screen.
        /// Especially it will be called when TableView is scrolled on vertically.
        /// </remarks>
        public virtual void FitWidthSize(bool scrollingUp)
        {
            int left = mColumnHeaderLayoutManager.GetFirstItemLeft();
            for (int i = mColumnHeaderLayoutManager.FindFirstVisibleItemPosition();
                i < mColumnHeaderLayoutManager.FindLastVisibleItemPosition() + 1;
                i++)
            {
                left = FitSize(i, left, scrollingUp);
            }

            mNeedSetLeft = false;
        }

        /// <summary>This method helps to fit a column.</summary>
        /// <remarks>
        /// This method helps to fit a column. it will be called when TableView is scrolled on
        /// horizontally.
        /// </remarks>
        public virtual void FitWidthSize(int position, bool scrollingLeft)
        {
            FitSize(position, IgnoreLeft, false);
            if (mNeedSetLeft & scrollingLeft)
            {
                // Works just like invoke later of swing utils.
                Android.OS.Handler handler = new Android.OS.Handler();
                handler.Post(new _Runnable_144(this));
            }
        }

        private sealed class _Runnable_144 : Java.Lang.Object, Java.Lang.IRunnable
        {
            public _Runnable_144(CellLayoutManager _enclosing)
            {
                this._enclosing = _enclosing;
            }

            public void Run()
            {
                this._enclosing.FitWidthSize2(true);
            }

            private readonly CellLayoutManager _enclosing;
        }

        private int FitSize(int position, int left, bool scrollingUp)
        {
            int cellRight = -1;
            int columnCacheWidth = mColumnHeaderLayoutManager.GetCacheWidth(position);
            Android.Views.View column = mColumnHeaderLayoutManager.FindViewByPosition(position);
            if (column != null)
            {
                // Determine default right
                cellRight = column.Left + columnCacheWidth + 1;
                if (scrollingUp)
                {
                    // Loop reverse order
                    for (int i = FindLastVisibleItemPosition(); i >= FindFirstVisibleItemPosition(); i--)
                    {
                        cellRight = Fit(position, i, left, cellRight, columnCacheWidth);
                    }
                }
                else
                {
                    // Loop for all rows which are visible.
                    for (int j = FindFirstVisibleItemPosition(); j < FindLastVisibleItemPosition() + 1; j++)
                    {
                        cellRight = Fit(position, j, left, cellRight, columnCacheWidth);
                    }
                }
            }
            else
            {
                Log.Error(LogTag, "Warning: column couldn't found for " + position);
            }

            return cellRight;
        }

        private int Fit(int xPosition, int yPosition, int left, int right, int columnCachedWidth)
        {
            CellRecyclerView child = (CellRecyclerView) FindViewByPosition(yPosition);
            if (child != null)
            {
                ColumnLayoutManager childLayoutManager = (ColumnLayoutManager) child.GetLayoutManager();
                int cellCacheWidth = GetCacheWidth(yPosition, xPosition);
                Android.Views.View cell = childLayoutManager.FindViewByPosition(xPosition);
                // Control whether the cell needs to be fitted by column header or not.
                if (cell != null)
                {
                    if (cellCacheWidth != columnCachedWidth || mNeedSetLeft)
                    {
                        // This is just for setting width value
                        if (cellCacheWidth != columnCachedWidth)
                        {
                            cellCacheWidth = columnCachedWidth;
                            TableViewUtils.SetWidth(cell, cellCacheWidth);
                            SetCacheWidth(yPosition, xPosition, cellCacheWidth);
                        }

                        // Even if the cached values are same, the left & right value wouldn't change.
                        // mNeedSetLeft & the below lines for it.
                        if (left != IgnoreLeft && cell.Left != left)
                        {
                            // Calculate scroll distance
                            int scrollX = Math.Max(cell.Left, left) - Math.Min(cell.Left, left);
                            // Update its left
                            cell.Left = left;
                            int offset = mHorizontalListener.GetScrollPositionOffset();
                            // It shouldn't be scroll horizontally and the problem is gotten just for
                            // first visible item.
                            if (offset > 0 && xPosition == childLayoutManager.FindFirstVisibleItemPosition() &&
                                mCellRecyclerView.ScrollState != RecyclerView.ScrollStateIdle)
                            {
                                int scrollPosition = mHorizontalListener.GetScrollPosition();
                                offset = mHorizontalListener.GetScrollPositionOffset() + scrollX;
                                // Update scroll position offset value
                                mHorizontalListener.SetScrollPositionOffset(offset);
                                // Scroll considering to the desired value.
                                childLayoutManager.ScrollToPositionWithOffset(scrollPosition, offset);
                            }
                        }

                        if (cell.Width != cellCacheWidth)
                        {
                            if (left != IgnoreLeft)
                            {
                                // TODO: + 1 is for decoration item. It should be gotten from a
                                // generic method  of layoutManager
                                // Set right
                                right = cell.Left + cellCacheWidth + 1;
                                cell.Right = right;
                                childLayoutManager.LayoutDecoratedWithMargins(cell, cell.Left, cell.Top, cell.Right,
                                    cell.Bottom);
                            }

                            mNeedSetLeft = true;
                        }
                    }
                }
            }

            return right;
        }

        /// <summary>Alternative method of fitWidthSize().</summary>
        /// <remarks>
        /// Alternative method of fitWidthSize().
        /// The main difference is this method works after main thread draw the ui components.
        /// </remarks>
        public virtual void FitWidthSize2(bool scrollingLeft)
        {
            // The below line helps to change left & right value of the each column
            // header views
            // without using requestLayout().
            mColumnHeaderLayoutManager.CustomRequestLayout();
            // Get the right scroll position information from Column header RecyclerView
            int columnHeaderScrollPosition = mTableView.GetColumnHeaderRecyclerView().GetScrolledX();
            int columnHeaderOffset = mColumnHeaderLayoutManager.GetFirstItemLeft();
            int columnHeaderFirstItem = mColumnHeaderLayoutManager.FindFirstVisibleItemPosition();
            // Fit all visible columns widths
            for (int i = mColumnHeaderLayoutManager.FindFirstVisibleItemPosition();
                i < mColumnHeaderLayoutManager.FindLastVisibleItemPosition() + 1;
                i++)
            {
                FitSize2(i, scrollingLeft, columnHeaderScrollPosition, columnHeaderOffset, columnHeaderFirstItem);
            }

            mNeedSetLeft = false;
        }

        /// <summary>Alternative method of fitWidthSize().</summary>
        /// <remarks>
        /// Alternative method of fitWidthSize().
        /// The main difference is this method works after main thread draw the ui components.
        /// </remarks>
        public virtual void FitWidthSize2(int position, bool scrollingLeft)
        {
            // The below line helps to change left & right value of the each column
            // header views
            // without using requestLayout().
            mColumnHeaderLayoutManager.CustomRequestLayout();
            // Get the right scroll position information from Column header RecyclerView
            int columnHeaderScrollPosition = mTableView.GetColumnHeaderRecyclerView().GetScrolledX();
            int columnHeaderOffset = mColumnHeaderLayoutManager.GetFirstItemLeft();
            int columnHeaderFirstItem = mColumnHeaderLayoutManager.FindFirstVisibleItemPosition();
            // Fit all visible columns widths
            FitSize2(position, scrollingLeft, columnHeaderScrollPosition, columnHeaderOffset, columnHeaderFirstItem);
            mNeedSetLeft = false;
        }

        private void FitSize2(int position, bool scrollingLeft, int columnHeaderScrollPosition, int columnHeaderOffset,
            int columnHeaderFirstItem)
        {
            int columnCacheWidth = mColumnHeaderLayoutManager.GetCacheWidth(position);
            Android.Views.View column = mColumnHeaderLayoutManager.FindViewByPosition(position);
            if (column != null)
            {
                // Loop for all rows which are visible.
                for (int j = FindFirstVisibleItemPosition(); j < FindLastVisibleItemPosition() + 1; j++)
                {
                    // Get CellRowRecyclerView
                    CellRecyclerView child = (CellRecyclerView) FindViewByPosition(j);
                    if (child != null)
                    {
                        ColumnLayoutManager childLayoutManager = (ColumnLayoutManager) child.GetLayoutManager();
                        // Checking Scroll position is necessary. Because, even if they have same width
                        // values, their scroll positions can be different.
                        if (!scrollingLeft && columnHeaderScrollPosition != child.GetScrolledX())
                        {
                            // Column Header RecyclerView has the right scroll position. So,
                            // considering it
                            childLayoutManager.ScrollToPositionWithOffset(columnHeaderFirstItem, columnHeaderOffset);
                        }

                        Fit2(position, j, columnCacheWidth, column, childLayoutManager);
                    }
                }
            }
        }

        private void Fit2(int xPosition, int yPosition, int columnCachedWidth, Android.Views.View column,
            ColumnLayoutManager childLayoutManager)
        {
            int cellCacheWidth = GetCacheWidth(yPosition, xPosition);
            Android.Views.View cell = childLayoutManager.FindViewByPosition(xPosition);
            // Control whether the cell needs to be fitted by column header or not.
            if (cell != null)
            {
                if (cellCacheWidth != columnCachedWidth || mNeedSetLeft)
                {
                    // This is just for setting width value
                    if (cellCacheWidth != columnCachedWidth)
                    {
                        cellCacheWidth = columnCachedWidth;
                        TableViewUtils.SetWidth(cell, cellCacheWidth);
                        SetCacheWidth(yPosition, xPosition, cellCacheWidth);
                    }

                    // The left & right values of Column header can be considered. Because this
                    // method will be worked
                    // after drawing process of main thread.
                    if (column.Left != cell.Left || column.Right != cell.Right)
                    {
                        // TODO: + 1 is for decoration item. It should be gotten from a generic
                        // method  of layoutManager
                        // Set right & left values
                        cell.Left = column.Left;
                        cell.Right = column.Right + 1;
                        childLayoutManager.LayoutDecoratedWithMargins(cell, cell.Left, cell.Top, cell.Right,
                            cell.Bottom);
                        mNeedSetLeft = true;
                    }
                }
            }
        }

        public virtual bool ShouldFitColumns(int yPosition)
        {
            // Scrolling horizontally
            if (mCellRecyclerView.ScrollState == RecyclerView.ScrollStateIdle)
            {
                int lastVisiblePosition = FindLastVisibleItemPosition();
                CellRecyclerView lastCellRecyclerView = (CellRecyclerView) FindViewByPosition(lastVisiblePosition);
                if (lastCellRecyclerView != null)
                {
                    if (yPosition == lastVisiblePosition)
                    {
                        return true;
                    }
                    else
                    {
                        if (lastCellRecyclerView.IsScrollOthers() && yPosition == lastVisiblePosition - 1)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public override void MeasureChildWithMargins(Android.Views.View child, int widthUsed, int heightUsed)
        {
            base.MeasureChildWithMargins(child, widthUsed, heightUsed);
            // If has fixed width is true, than calculation of the column width is not necessary.
            if (mTableView.HasFixedWidth())
            {
                return;
            }

            int position = GetPosition(child);
            ColumnLayoutManager childLayoutManager =
                (ColumnLayoutManager) ((CellRecyclerView) child).GetLayoutManager();
            // the below codes should be worked when it is scrolling vertically
            if (mCellRecyclerView.ScrollState != RecyclerView.ScrollStateIdle)
            {
                if (childLayoutManager.IsNeedFit())
                {
                    // Scrolling up
                    if (mLastDy < 0)
                    {
                        Log.Error(LogTag, position + " fitWidthSize all vertically up");
                        FitWidthSize(true);
                    }
                    else
                    {
                        // Scrolling down
                        Log.Error(LogTag, position + " fitWidthSize all vertically down");
                        FitWidthSize(false);
                    }

                    // all columns have been fitted.
                    childLayoutManager.ClearNeedFit();
                }

                // Set the right initialPrefetch size to improve performance
                childLayoutManager.InitialPrefetchItemCount = childLayoutManager.ChildCount;
            }
            else
            {
                // That means,populating for the first time like fetching all data to display.
                // It shouldn't be worked when it is scrolling horizontally ."getLastDx() == 0"
                // control for it.
                if (childLayoutManager.GetLastDx() == 0 &&
                    mCellRecyclerView.ScrollState == RecyclerView.ScrollStateIdle)
                {
                    if (childLayoutManager.IsNeedFit())
                    {
                        mNeedFit = true;
                        // all columns have been fitted.
                        childLayoutManager.ClearNeedFit();
                    }

                    if (mNeedFit)
                    {
                        // for the first time to populate adapter
                        if (mRowHeaderLayoutManager.FindLastVisibleItemPosition() == position)
                        {
                            FitWidthSize2(false);
                            Log.Error(LogTag, position + " fitWidthSize populating data for the first time");
                            mNeedFit = false;
                        }
                    }
                }
            }
        }

        public virtual AbstractViewHolder[] GetVisibleCellViewsByColumnPosition(int xPosition)
        {
            int visibleChildCount = FindLastVisibleItemPosition() - FindFirstVisibleItemPosition() + 1;
            int index = 0;
            AbstractViewHolder[] viewHolders = new AbstractViewHolder[visibleChildCount];
            for (int i = FindFirstVisibleItemPosition(); i < FindLastVisibleItemPosition() + 1; i++)
            {
                CellRecyclerView cellRowRecyclerView = (CellRecyclerView) FindViewByPosition(i);
                AbstractViewHolder holder =
                    (AbstractViewHolder) cellRowRecyclerView.FindViewHolderForAdapterPosition(xPosition);
                viewHolders[index] = holder;
                index++;
            }

            return viewHolders;
        }

        public virtual AbstractViewHolder GetCellViewHolder(int xPosition, int yPosition)
        {
            CellRecyclerView cellRowRecyclerView = (CellRecyclerView) FindViewByPosition(yPosition);
            if (cellRowRecyclerView != null)
            {
                return (AbstractViewHolder) cellRowRecyclerView.FindViewHolderForAdapterPosition(xPosition);
            }

            return null;
        }

        public virtual void RemeasureAllChild()
        {
            // TODO: the below code causes requestLayout() improperly called by com.evrencoskun
            // TODO: .tableview.adapter
            for (int j = 0; j < ChildCount; j++)
            {
                CellRecyclerView recyclerView = (CellRecyclerView) GetChildAt(j);
                recyclerView.LayoutParameters.Width = ViewGroup.LayoutParams.WrapContent;
                recyclerView.RequestLayout();
            }
        }

        /// <summary>Allows to set cache width value for single cell item.</summary>
        public virtual void SetCacheWidth(int row, int column, int width)
        {
            IDictionary<int, int> cellRowCache = null;
            if (!mCachedWidthList.TryGetValue(row, out cellRowCache))
            {
                cellRowCache = new Dictionary<int, int>();
            }

            cellRowCache[column] = width;
            mCachedWidthList[row] = cellRowCache;
        }

        /// <summary>Allows to set cache width value for all cell items that is located on column position.
        /// 	</summary>
        public virtual void SetCacheWidth(int column, int width)
        {
            for (int i = 0; i < mRowHeaderRecyclerView.GetAdapter().ItemCount; i++)
            {
                // set cache width for single cell item.
                SetCacheWidth(i, column, width);
            }
        }

        public virtual int GetCacheWidth(int row, int column)
        {
            IDictionary<int, int> cellRowCaches = null;
            if (mCachedWidthList.TryGetValue(row, out cellRowCaches))
            {
                int cachedWidth = -1;
                if (cellRowCaches.TryGetValue(column, out cachedWidth))
                {
                    return cachedWidth;
                }
            }

            return -1;
        }

        /// <summary>Clears the widths which have been calculated and reused.</summary>
        public virtual void ClearCachedWidths()
        {
            mCachedWidthList.Clear();
        }

        public CellRecyclerView[] VisibleCellRowRecyclerViews => GetVisibleCellRowRecyclerViews();

        public virtual CellRecyclerView[] GetVisibleCellRowRecyclerViews()
        {
            int length = FindLastVisibleItemPosition() - FindFirstVisibleItemPosition() + 1;
            CellRecyclerView[] recyclerViews = new CellRecyclerView[length];
            int index = 0;
            for (int i = FindFirstVisibleItemPosition(); i < FindLastVisibleItemPosition() + 1; i++)
            {
                recyclerViews[index] = (CellRecyclerView) FindViewByPosition(i);
                index++;
            }

            return recyclerViews;
        }
    }
}
