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
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using Com.Evrencoskun.Tableview;
using Com.Evrencoskun.Tableview.Adapter.Recyclerview;
using Com.Evrencoskun.Tableview.Adapter.Recyclerview.Holder;
using Com.Evrencoskun.Tableview.Util;

namespace Com.Evrencoskun.Tableview.Adapter
{
    /// <summary>Created by evrencoskun on 10/06/2017.</summary>
    public abstract class AbstractTableAdapter : AdapterDataSetChangedListener, ITableAdapter
    {
        private int mRowHeaderWidth;

        private int mColumnHeaderHeight;

        private Context mContext;

        private ColumnHeaderRecyclerViewAdapter mColumnHeaderRecyclerViewAdapter;

        private RowHeaderRecyclerViewAdapter mRowHeaderRecyclerViewAdapter;

        private CellRecyclerViewAdapter mCellRecyclerViewAdapter;

        private Android.Views.View mCornerView;

        protected internal IList<IColumnHeader> mColumnHeaderItems;

        protected internal IList<IRowHeader> mRowHeaderItems;

        protected internal IList<IList<ICell>> mCellItems;

        private ITableView mTableView;

        private IList<AdapterDataSetChangedListener> dataSetChangedListeners;

        public AbstractTableAdapter(Context context)
        {
            mContext = context;
            AddAdapterDataSetChangedListener(this);
        }

        public ITableView TableView
        {
            get => GetTableView();
            set => SetTableView((Com.Evrencoskun.Tableview.TableView)value);
        }

        public virtual void SetTableView(TableView tableView)
        {
            mTableView = tableView;
            Initialize();
        }

        private void Initialize()
        {
            // Create Column header RecyclerView Adapter
            mColumnHeaderRecyclerViewAdapter = new ColumnHeaderRecyclerViewAdapter(mContext, mColumnHeaderItems, this);
            // Create Row Header RecyclerView Adapter
            mRowHeaderRecyclerViewAdapter = new RowHeaderRecyclerViewAdapter(mContext, mRowHeaderItems, this);
            // Create Cell RecyclerView Adapter
            mCellRecyclerViewAdapter = new CellRecyclerViewAdapter(mContext, mCellItems, mTableView);
        }

        public void SetColumnHeaderItems(IList<IColumnHeader> columnHeaderItems)
        {
            if (columnHeaderItems == null)
            {
                return;
            }

            mColumnHeaderItems = columnHeaderItems;
            // Invalidate the cached widths for letting the view measure the cells width
            // from scratch.
            mTableView.GetColumnHeaderLayoutManager().ClearCachedWidths();
            // Set the items to the adapter
            mColumnHeaderRecyclerViewAdapter.SetItems(mColumnHeaderItems);
            DispatchColumnHeaderDataSetChangesToListeners(columnHeaderItems);
        }

        public void SetRowHeaderItems(IList<IRowHeader> rowHeaderItems)
        {
            if (rowHeaderItems == null)
            {
                return;
            }

            mRowHeaderItems = rowHeaderItems;
            // Set the items to the adapter
            mRowHeaderRecyclerViewAdapter.SetItems(mRowHeaderItems);
            DispatchRowHeaderDataSetChangesToListeners(mRowHeaderItems);
        }

        public void SetCellItems(IList<IList<ICell>> cellItems)
        {
            if (cellItems == null)
            {
                return;
            }

            mCellItems = cellItems;
            // Invalidate the cached widths for letting the view measure the cells width
            // from scratch.
            mTableView.GetCellLayoutManager().ClearCachedWidths();
            // Set the items to the adapter
            mCellRecyclerViewAdapter.SetItems(mCellItems);
            DispatchCellDataSetChangesToListeners(mCellItems);
        }

        public void SetAllItems(IList<IColumnHeader> columnHeaderItems, IList<IRowHeader> rowHeaderItems,
            IList<IList<ICell>> cellItems)
        {
            // Set all items
            SetColumnHeaderItems(columnHeaderItems);
            SetRowHeaderItems(rowHeaderItems);
            SetCellItems(cellItems);
            // Control corner view
            if ((columnHeaderItems != null && columnHeaderItems.Count != 0) &&
                (rowHeaderItems != null && rowHeaderItems.Count != 0) && (cellItems != null && cellItems.Count != 0) &&
                mTableView != null && mCornerView == null)
            {
                // Create corner view
                mCornerView = OnCreateCornerView();
                mTableView.AddView(mCornerView, new FrameLayout.LayoutParams(mRowHeaderWidth, mColumnHeaderHeight));
            }
            else
            {
                if (mCornerView != null)
                {
                    // Change corner view visibility
                    if (rowHeaderItems != null && rowHeaderItems.Count != 0)
                    {
                        mCornerView.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        mCornerView.Visibility = ViewStates.Gone;
                    }
                }
            }
        }

        public Android.Views.View CornerView
        {
            get => GetCornerView();
        }

        public Android.Views.View GetCornerView()
        {
            return mCornerView;
        }

        public ColumnHeaderRecyclerViewAdapter ColumnHeaderRecyclerViewAdapter => GetColumnHeaderRecyclerViewAdapter();

        public virtual ColumnHeaderRecyclerViewAdapter GetColumnHeaderRecyclerViewAdapter()
        {
            return mColumnHeaderRecyclerViewAdapter;
        }
        public RowHeaderRecyclerViewAdapter RowHeaderRecyclerViewAdapter => GetRowHeaderRecyclerViewAdapter();

        public virtual RowHeaderRecyclerViewAdapter GetRowHeaderRecyclerViewAdapter()
        {
            return mRowHeaderRecyclerViewAdapter;
        }

        public CellRecyclerViewAdapter CellRecyclerViewAdapter => GetCellRecyclerViewAdapter();

        public Context Context { get => mContext; }

        public virtual CellRecyclerViewAdapter GetCellRecyclerViewAdapter()
        {
            return mCellRecyclerViewAdapter;
        }

        public virtual void SetRowHeaderWidth(int rowHeaderWidth)
        {
            this.mRowHeaderWidth = rowHeaderWidth;
            if (mCornerView != null)
            {
                ViewGroup.LayoutParams layoutParams = mCornerView.LayoutParameters;
                layoutParams.Width = rowHeaderWidth;
            }
        }

        public virtual void SetColumnHeaderHeight(int columnHeaderHeight)
        {
            this.mColumnHeaderHeight = columnHeaderHeight;
        }

        public virtual IColumnHeader GetColumnHeaderItem(int position)
        {
            if ((mColumnHeaderItems == null || mColumnHeaderItems.Count == 0) || position < 0 ||
                position >= mColumnHeaderItems.Count)
            {
                return null;
            }

            return mColumnHeaderItems[position];
        }

        public virtual IRowHeader GetRowHeaderItem(int position)
        {
            if ((mRowHeaderItems == null || mRowHeaderItems.Count == 0) || position < 0 ||
                position >= mRowHeaderItems.Count)
            {
                return null;
            }

            return mRowHeaderItems[position];
        }

        public virtual ICell GetCellItem(int columnPosition, int rowPosition)
        {
            if ((mCellItems == null || mCellItems.Count == 0) || columnPosition < 0 ||
                rowPosition >= mCellItems.Count || mCellItems[rowPosition] == null || rowPosition < 0 ||
                columnPosition >= mCellItems[rowPosition].Count)
            {
                return null;
            }

            return mCellItems[rowPosition][columnPosition];
        }

        public virtual IList<IRowHeader> GetCellRowItems(int rowPosition)
        {
            return (IList<IRowHeader>) mCellRecyclerViewAdapter.GetItem(rowPosition);
        }

        public virtual void RemoveRow(int rowPosition)
        {
            mCellRecyclerViewAdapter.DeleteItem(rowPosition);
            mRowHeaderRecyclerViewAdapter.DeleteItem(rowPosition);
        }

        public virtual void RemoveRow(int rowPosition, bool updateRowHeader)
        {
            mCellRecyclerViewAdapter.DeleteItem(rowPosition);
            // To be able update the row header data
            if (updateRowHeader)
            {
                rowPosition = mRowHeaderRecyclerViewAdapter.ItemCount - 1;
                // Cell RecyclerView items should be notified.
                // Because, other items stores the old row position.
                mCellRecyclerViewAdapter.NotifyDataSetChanged();
            }

            mRowHeaderRecyclerViewAdapter.DeleteItem(rowPosition);
        }

        public virtual void RemoveRowRange(int rowPositionStart, int itemCount)
        {
            mCellRecyclerViewAdapter.DeleteItemRange(rowPositionStart, itemCount);
            mRowHeaderRecyclerViewAdapter.DeleteItemRange(rowPositionStart, itemCount);
        }

        public virtual void RemoveRowRange(int rowPositionStart, int itemCount, bool updateRowHeader)
        {
            mCellRecyclerViewAdapter.DeleteItemRange(rowPositionStart, itemCount);
            // To be able update the row header data sets
            if (updateRowHeader)
            {
                rowPositionStart = mRowHeaderRecyclerViewAdapter.ItemCount - 1 - itemCount;
                // Cell RecyclerView items should be notified.
                // Because, other items stores the old row position.
                mCellRecyclerViewAdapter.NotifyDataSetChanged();
            }

            mRowHeaderRecyclerViewAdapter.DeleteItemRange(rowPositionStart, itemCount);
        }

        public virtual void AddRow(int rowPosition, IRowHeader rowHeaderItem, IList<ICell> cellItems)
        {
            mCellRecyclerViewAdapter.AddItem(rowPosition, cellItems);
            mRowHeaderRecyclerViewAdapter.AddItem(rowPosition, rowHeaderItem);
        }

        public virtual void AddRowRange(int rowPositionStart, IList<IRowHeader> rowHeaderItem,
            IList<IList<ICell>> cellItems)
        {
            mRowHeaderRecyclerViewAdapter.AddItemRange(rowPositionStart, rowHeaderItem);
            mCellRecyclerViewAdapter.AddItemRange(rowPositionStart, cellItems);
        }

        public virtual void ChangeRowHeaderItem(int rowPosition, IRowHeader rowHeaderModel)
        {
            mRowHeaderRecyclerViewAdapter.ChangeItem(rowPosition, rowHeaderModel);
        }

        public virtual void ChangeRowHeaderItemRange(int rowPositionStart, IList<IRowHeader> rowHeaderModelList)
        {
            mRowHeaderRecyclerViewAdapter.ChangeItemRange(rowPositionStart, rowHeaderModelList);
        }

        public virtual void ChangeCellItem(int columnPosition, int rowPosition, ICell cellModel)
        {
            IList<ICell> cellItems = (IList<ICell>) mCellRecyclerViewAdapter.GetItem(rowPosition);
            if (cellItems != null && cellItems.Count > columnPosition)
            {
                // Update cell row items.
                cellItems[columnPosition] = cellModel;
                mCellRecyclerViewAdapter.ChangeItem(rowPosition, cellItems);
            }
        }

        public virtual void ChangeColumnHeader(int columnPosition, IColumnHeader columnHeaderModel)
        {
            mColumnHeaderRecyclerViewAdapter.ChangeItem(columnPosition, columnHeaderModel);
        }

        public virtual void ChangeColumnHeaderRange(int columnPositionStart, IList<IColumnHeader> columnHeaderModelList)
        {
            mColumnHeaderRecyclerViewAdapter.ChangeItemRange(columnPositionStart, columnHeaderModelList);
        }

        public virtual IList<ICell> GetCellColumnItems(int columnPosition)
        {
            return mCellRecyclerViewAdapter.GetColumnItems(columnPosition);
        }

        public virtual void RemoveColumn(int columnPosition)
        {
            mColumnHeaderRecyclerViewAdapter.DeleteItem(columnPosition);
            mCellRecyclerViewAdapter.RemoveColumnItems(columnPosition);
        }

        public virtual void AddColumn(int columnPosition, IColumnHeader columnHeaderItem, IList<ICell> cellItems)
        {
            mColumnHeaderRecyclerViewAdapter.AddItem(columnPosition, columnHeaderItem);
            mCellRecyclerViewAdapter.AddColumnItems(columnPosition, cellItems);
        }

        public void NotifyDataSetChanged()
        {
            mColumnHeaderRecyclerViewAdapter.NotifyDataSetChanged();
            mRowHeaderRecyclerViewAdapter.NotifyDataSetChanged();
            mCellRecyclerViewAdapter.NotifyCellDataSetChanged();
        }

        public virtual ITableView GetTableView()
        {
            return mTableView;
        }

        private void DispatchColumnHeaderDataSetChangesToListeners(IList<IColumnHeader> newColumnHeaderItems)
        {
            if (dataSetChangedListeners != null)
            {
                foreach (AdapterDataSetChangedListener listener in dataSetChangedListeners)
                {
                    listener.OnColumnHeaderItemsChanged(newColumnHeaderItems);
                }
            }
        }

        private void DispatchRowHeaderDataSetChangesToListeners(IList<IRowHeader> newRowHeaderItems)
        {
            if (dataSetChangedListeners != null)
            {
                foreach (AdapterDataSetChangedListener listener in dataSetChangedListeners)
                {
                    listener.OnRowHeaderItemsChanged(newRowHeaderItems);
                }
            }
        }

        private void DispatchCellDataSetChangesToListeners(IList<IList<ICell>> newCellItems)
        {
            if (dataSetChangedListeners != null)
            {
                foreach (AdapterDataSetChangedListener listener in dataSetChangedListeners)
                {
                    listener.OnCellItemsChanged(newCellItems);
                }
            }
        }

        /// <summary>Sets the listener for changes of data set on the TableView.</summary>
        /// <param name="listener">The AdapterDataSetChangedListener listener.</param>
        public virtual void AddAdapterDataSetChangedListener(AdapterDataSetChangedListener listener)
        {
            if (dataSetChangedListeners == null)
            {
                dataSetChangedListeners = new AList<AdapterDataSetChangedListener>();
            }

            dataSetChangedListeners.Add(listener);
        }

        public abstract int GetColumnHeaderItemViewType(int position);
        public abstract int GetRowHeaderItemViewType(int position);
        public abstract int GetCellItemViewType(int position);
        public abstract AbstractViewHolder OnCreateCellViewHolder(ViewGroup parent, int viewType);

        public abstract void OnBindCellViewHolder(AbstractViewHolder holder, ICell cellItemModel, int columnPosition,
            int rowPosition);

        public abstract AbstractViewHolder OnCreateColumnHeaderViewHolder(ViewGroup parent, int viewType);

        public abstract void OnBindColumnHeaderViewHolder(AbstractViewHolder holder,
            IColumnHeader columnHeaderItemModel, int columnPosition);

        public abstract AbstractViewHolder OnCreateRowHeaderViewHolder(ViewGroup parent, int viewType);

        public abstract void OnBindRowHeaderViewHolder(AbstractViewHolder holder, IRowHeader rowHeaderItemModel,
            int rowPosition);

        public abstract View OnCreateCornerView();

        public class DataSetChangedEventArgs : EventArgs
        {
            public IList<IColumnHeader> ColumnHeaderItems { get; set; } 
            public IList<IRowHeader> RowHeaderItems { get; set; }
            public IList<IList<ICell>> CellItems { get; set; }
        }

        public event EventHandler<IList<IList<ICell>>> CellItemsChanged;
        public event EventHandler<IList<IColumnHeader>> ColumnHeaderItemsChanged;
        public event EventHandler<DataSetChangedEventArgs> DataSetChanged;
        public event EventHandler<IList<IRowHeader>> RowHeaderItemsChanged;

        public override void OnCellItemsChanged(IList<IList<ICell>> cellItems)
        {
            CellItemsChanged.SafeFire(this, cellItems );
        }

        public override void OnColumnHeaderItemsChanged(IList<IColumnHeader> columnHeaderItems)
        {
            ColumnHeaderItemsChanged.SafeFire(this, columnHeaderItems);
        }

        public override void OnDataSetChanged(IList<IColumnHeader> columnHeaderItems, IList<IRowHeader> rowHeaderItems, IList<IList<ICell>> cellItems)
        {
            DataSetChanged.SafeFire(this, new DataSetChangedEventArgs { CellItems = cellItems, ColumnHeaderItems = columnHeaderItems, RowHeaderItems = rowHeaderItems });

        }

        public override void OnRowHeaderItemsChanged(IList<IRowHeader> rowHeaderItems)
        {
            RowHeaderItemsChanged.SafeFire(this, rowHeaderItems);
        }
    }
}
