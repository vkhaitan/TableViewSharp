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
using System.Diagnostics;
using Android.Support.V7.Util;
using Android.Util;
using Com.Evrencoskun.Tableview;
using Com.Evrencoskun.Tableview.Adapter;
using Com.Evrencoskun.Tableview.Adapter.Recyclerview;
using Com.Evrencoskun.Tableview.Sort;
using Com.Evrencoskun.Tableview.Util;

namespace Com.Evrencoskun.Tableview.Handler
{
    /// <summary>Created by evrencoskun on 24.11.2017.</summary>
    public class ColumnSortHandler : ColumnSortStateChangedListener
    {
        private CellRecyclerViewAdapter mCellRecyclerViewAdapter;

        private RowHeaderRecyclerViewAdapter mRowHeaderRecyclerViewAdapter;

        private ColumnHeaderRecyclerViewAdapter mColumnHeaderRecyclerViewAdapter;

        private IList<ColumnSortStateChangedListener> columnSortStateChangedListeners =
            new List<ColumnSortStateChangedListener>();

        private bool mEnableAnimation = true;

        public virtual bool IsEnableAnimation()
        {
            return mEnableAnimation;
        }

        public virtual void SetEnableAnimation(bool mEnableAnimation)
        {
            this.mEnableAnimation = mEnableAnimation;
        }

        public ColumnSortHandler(ITableView tableView)
        {
            this.mCellRecyclerViewAdapter = (CellRecyclerViewAdapter)tableView.GetCellRecyclerView().GetAdapter();
            this.mRowHeaderRecyclerViewAdapter =
                (RowHeaderRecyclerViewAdapter)tableView.GetRowHeaderRecyclerView().GetAdapter();
            this.mColumnHeaderRecyclerViewAdapter =
                (ColumnHeaderRecyclerViewAdapter)tableView.GetColumnHeaderRecyclerView().GetAdapter();

            AddColumnSortStateChangedListener(this);
        }

        public virtual void SortByRowHeader(SortState sortState)
        {
            IList<IRowHeader> originalRowHeaderList = mRowHeaderRecyclerViewAdapter.GetItems();
            IList<IList<ICell>> originalList = mCellRecyclerViewAdapter.GetItems();

            List<IRowHeader> sortedRowHeaderList; 
            List<IList<ICell>> sortedList;

            AList<(IRowHeader row, IList<ICell> cells)> sortedCombinedList = new AList<(IRowHeader, IList<ICell>)>();
            for(int i=0; i < originalRowHeaderList.Count; ++i)
            {
                sortedCombinedList.Add((originalRowHeaderList[i], originalList[i]));
            }

            if (sortState != SortState.Unsorted)
            {
                // Do descending / ascending sort
                sortedCombinedList.Sort(new RowHeaderSortComparator(sortState));
                sortedRowHeaderList = sortedCombinedList.ConvertAll(((IRowHeader row, IList<ICell> cells) input) => input.row);
                sortedList = sortedCombinedList.ConvertAll(((IRowHeader row, IList<ICell> cells) input) => input.cells);
                mRowHeaderRecyclerViewAdapter.GetRowHeaderSortHelper().SetSortingStatus(sortState);
                // Set sorted data list
                SwapItems(originalRowHeaderList, sortedRowHeaderList, sortedList, sortState);
            }


        }

        public virtual void Sort(int column, SortState sortState)
        {
            IList<IList<ICell>> originalList = mCellRecyclerViewAdapter.GetItems();
            IList<IRowHeader> originalRowHeaderList = mRowHeaderRecyclerViewAdapter.GetItems();

            AList<(IRowHeader row, IList<ICell> cells)> sortedCombinedList = new AList<(IRowHeader row, IList<ICell> cells)>();
            for (int i= 0; i < originalList.Count; ++i)
            {
                sortedCombinedList.Add((originalRowHeaderList[i], originalList[i]));
            }

            List<IList<ICell>> sortedList; 
            List<IRowHeader> sortedRowHeaderList;
            if (sortState != SortState.Unsorted)
            {
                // Do descending / ascending sort
                sortedCombinedList.Sort(new ColumnSortComparator(column, sortState));
                sortedList = sortedCombinedList.ConvertAll(((IRowHeader row, IList<ICell> cells) input) => input.cells);
                sortedRowHeaderList = sortedCombinedList.ConvertAll(((IRowHeader row, IList<ICell> cells) input) => input.row);

                mColumnHeaderRecyclerViewAdapter.GetColumnSortHelper().SetSortingStatus(column, sortState);
                SwapItems(originalList, sortedList, column, sortedRowHeaderList, sortState);
            }
        }

        private void SwapItems(IList<IRowHeader> oldRowHeader, IList<IRowHeader> newRowHeader,
            IList<IList<ICell>> newColumnItems, SortState sortState)
        {
            // Set new items without calling notifyCellDataSetChanged method of CellRecyclerViewAdapter
            mRowHeaderRecyclerViewAdapter.SetItems(newRowHeader, !mEnableAnimation);
            mCellRecyclerViewAdapter.SetItems(newColumnItems, !mEnableAnimation);
            if (mEnableAnimation)
            {
                // Find the differences between old cell items and new items.
                RowHeaderSortCallback diffCallback = new RowHeaderSortCallback(oldRowHeader, newRowHeader);
                DiffUtil.DiffResult diffResult = DiffUtil.CalculateDiff(diffCallback);
                diffResult.DispatchUpdatesTo(mRowHeaderRecyclerViewAdapter);
                diffResult.DispatchUpdatesTo(mCellRecyclerViewAdapter);
            }

            foreach (ColumnSortStateChangedListener listener in columnSortStateChangedListeners)
            {
                listener.OnRowHeaderSortStatusChanged(sortState);
            }
        }

        private void SwapItems(IList<IList<ICell>> oldItems, IList<IList<ICell>> newItems, int column,
            IList<IRowHeader> newRowHeader, SortState sortState)
        {
            // Set new items without calling notifyCellDataSetChanged method of CellRecyclerViewAdapter
            mCellRecyclerViewAdapter.SetItems(newItems, !mEnableAnimation);
            mRowHeaderRecyclerViewAdapter.SetItems(newRowHeader, !mEnableAnimation);
            if (mEnableAnimation)
            {
                // Find the differences between old cell items and new items.
                ColumnSortCallback diffCallback = new ColumnSortCallback(oldItems, newItems, column);
                DiffUtil.DiffResult diffResult = DiffUtil.CalculateDiff(diffCallback);
                diffResult.DispatchUpdatesTo(mCellRecyclerViewAdapter);
                diffResult.DispatchUpdatesTo(mRowHeaderRecyclerViewAdapter);
            }

            foreach (ColumnSortStateChangedListener listener in columnSortStateChangedListeners)
            {
                listener.OnColumnSortStatusChanged(column, sortState);
            }
        }

        public virtual void SwapItems(IList<IList<ICell>> newItems, int column)
        {
            IList<IList<ICell>> oldItems = mCellRecyclerViewAdapter.GetItems();
            // Set new items without calling notifyCellDataSetChanged method of CellRecyclerViewAdapter
            mCellRecyclerViewAdapter.SetItems((IList<IList<Adapter.ICell>>)newItems, !mEnableAnimation);
            if (mEnableAnimation)
            {
                // Find the differences between old cell items and new items.
                ColumnSortCallback diffCallback = new ColumnSortCallback(oldItems, newItems, column);
                DiffUtil.DiffResult diffResult = DiffUtil.CalculateDiff(diffCallback);
                diffResult.DispatchUpdatesTo(mCellRecyclerViewAdapter);
                diffResult.DispatchUpdatesTo(mRowHeaderRecyclerViewAdapter);
            }
        }

        public virtual SortState GetSortingStatus(int column)
        {
            return mColumnHeaderRecyclerViewAdapter.GetColumnSortHelper().GetSortingStatus(column);
        }

        public SortState RowHeaderSortingStatus => GetRowHeaderSortingStatus();

        public virtual SortState GetRowHeaderSortingStatus()
        {
            return mRowHeaderRecyclerViewAdapter.GetRowHeaderSortHelper().GetSortingStatus();
        }

        /// <summary>Sets the listener for the changes in column sorting.</summary>
        /// <param name="listener">ColumnSortStateChangedListener listener.</param>
        public virtual void AddColumnSortStateChangedListener(ColumnSortStateChangedListener listener)
        {
            if (columnSortStateChangedListeners == null)
            {
                columnSortStateChangedListeners = new AList<ColumnSortStateChangedListener>();
            }

            columnSortStateChangedListeners.Add(listener);
        }

        public class ColumnSortStateChangedEventArgs : EventArgs
        {
            public int Column { get; set; }
            public SortState SortState { get; set;  }
        }

        public event EventHandler<ColumnSortStateChangedEventArgs> ColumnSortStatusChanged;
        public event EventHandler<SortState> RowHeaderSortStatusChanged;

        public override void OnColumnSortStatusChanged(int column, SortState sortState)
        {
            ColumnSortStatusChanged.SafeFire(this, new ColumnSortStateChangedEventArgs { Column = column, SortState = sortState });
        }

        public override void OnRowHeaderSortStatusChanged(SortState sortState)
        {
            RowHeaderSortStatusChanged.SafeFire(this, sortState);
        }

    }
}
