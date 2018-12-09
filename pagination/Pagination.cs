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
using Com.Evrencoskun.Tableview;
using Com.Evrencoskun.Tableview.Adapter;
using Com.Evrencoskun.Tableview.Adapter.Recyclerview;
using Com.Evrencoskun.Tableview.Filter;
using Com.Evrencoskun.Tableview.Sort;
using System.Linq;
using Com.Evrencoskun.Tableview.Util;

namespace Com.Evrencoskun.Tableview.Pagination
{
    public class Pagination : IPagination , Pagination.OnTableViewPageTurnedListener
    {
        private const int DefaultItemsPerPage = 10;

        private int itemsPerPage;

        private int currentPage;

        private int pageCount;

        private IList<IList<ICell>> originalCellData;

        private IList<IList<ICell>> currentPageCellData;

        private IList<IRowHeader> originalRowData;

        private IList<IRowHeader> currentPageRowData;

        private RowHeaderRecyclerViewAdapter mRowHeaderRecyclerViewAdapter;

        private CellRecyclerViewAdapter mCellRecyclerViewAdapter;

        private ITableView tableView;

        private Pagination.OnTableViewPageTurnedListener onTableViewPageTurnedListener;

        /// <summary>Basic constructor, TableView instance is required.</summary>
        /// <param name="tableView">The TableView to be paginated.</param>
        public Pagination(ITableView tableView) : this(tableView, DefaultItemsPerPage, null)
        {
            adapterDataSetChangedListener = new _AdapterDataSetChangedListener_203(this);
            filterChangedListener = new _FilterChangedListener_223(this);
            columnSortStateChangedListener = new _ColumnSortStateChangedListener_240(this);
        }

        /// <summary>Applies pagination to the supplied TableView with number of items per page.
        /// 	</summary>
        /// <param name="tableView">The TableView to be paginated.</param>
        /// <param name="itemsPerPage">The number of items per page.</param>
        public Pagination(ITableView tableView, int itemsPerPage) : this(tableView, itemsPerPage, null)
        {
            adapterDataSetChangedListener = new _AdapterDataSetChangedListener_203(this);
            filterChangedListener = new _FilterChangedListener_223(this);
            columnSortStateChangedListener = new _ColumnSortStateChangedListener_240(this);
        }

        /// <summary>
        /// Applies pagination to the supplied TableView with number of items per page and an
        /// OnTableViewPageTurnedListener for handling changes in the TableView pagination.
        /// </summary>
        /// <param name="tableView">The TableView to be paginated.</param>
        /// <param name="itemsPerPage">The number of items per page.</param>
        /// <param name="listener">The OnTableViewPageTurnedListener for the TableView.</param>
        public Pagination(ITableView tableView, int itemsPerPage, Pagination.OnTableViewPageTurnedListener listener)
        {
            adapterDataSetChangedListener = new _AdapterDataSetChangedListener_203(this);
            filterChangedListener = new _FilterChangedListener_223(this);
            columnSortStateChangedListener = new _ColumnSortStateChangedListener_240(this);
            Initialize(tableView, itemsPerPage, listener);
        }

        private void Initialize(ITableView _tableView, int _itemsPerPage,
            Pagination.OnTableViewPageTurnedListener listener)
        {
            this.onTableViewPageTurnedListener = listener;
            this.itemsPerPage = _itemsPerPage;
            this.tableView = _tableView;
            this.mRowHeaderRecyclerViewAdapter =
                (RowHeaderRecyclerViewAdapter) _tableView.GetRowHeaderRecyclerView().GetAdapter();
            this.mCellRecyclerViewAdapter = (CellRecyclerViewAdapter) _tableView.GetCellRecyclerView().GetAdapter();
            this.tableView.GetColumnSortHandler().AddColumnSortStateChangedListener(columnSortStateChangedListener);
            this.tableView.GetAdapter().AddAdapterDataSetChangedListener(adapterDataSetChangedListener);
            this.tableView.GetFilterHandler().AddFilterChangedListener(filterChangedListener);
            this.originalCellData = _tableView.GetAdapter().GetCellRecyclerViewAdapter().GetItems();
            this.originalRowData = _tableView.GetAdapter().GetRowHeaderRecyclerViewAdapter().GetItems();
            this.currentPage = 1;
            ReloadPages();
        }

        private void ReloadPages()
        {
            if (originalCellData != null && originalRowData != null)
            {
                PaginateData();
                GoToPage(currentPage);
            }
        }

        private void PaginateData()
        {
            int start;
            int end;
            currentPageCellData = new AList<IList<ICell>>();
            currentPageRowData = new AList<IRowHeader>();
            // No pagination if itemsPerPage is 0, all data will be loaded into the TableView.
            if (itemsPerPage == 0)
            {
                foreach (var i in originalCellData)
                    currentPageCellData.Add(i);
                foreach (var i in originalRowData)
                    currentPageRowData.Add(i);
                pageCount = 1;
                start = 0;
                end = currentPageCellData.Count;
            }
            else
            {
                start = (currentPage * itemsPerPage) - itemsPerPage;
                end = (currentPage * itemsPerPage) > originalCellData.Count
                    ? originalCellData.Count
                    : (currentPage * itemsPerPage);
                for (int x = start; x < end; x++)
                {
                    currentPageCellData.Add(originalCellData[x]);
                    currentPageRowData.Add(originalRowData[x]);
                }

                // Using ceiling to calculate number of pages, e.g. 103 items of 10 items per page
                // will result to 11 pages.
                pageCount = (int) Math.Ceiling((double) originalCellData.Count / itemsPerPage);
            }

            // Sets the paginated data to the TableView.
            mRowHeaderRecyclerViewAdapter.SetItems(currentPageRowData, true);
            mCellRecyclerViewAdapter.SetItems(currentPageCellData, true);
            // Dispatches TableView changes to Listener interface
            if (onTableViewPageTurnedListener != null)
            {
                onTableViewPageTurnedListener.OnPageTurned(currentPageCellData.Count, start, end - 1);
            }
        }

        public virtual void NextPage()
        {
            currentPage = currentPage + 1 > pageCount ? currentPage : ++currentPage;
            PaginateData();
        }

        public virtual void PreviousPage()
        {
            currentPage = currentPage - 1 == 0 ? currentPage : --currentPage;
            PaginateData();
        }

        public virtual void GoToPage(int page)
        {
            currentPage = (page > pageCount || page < 1)
                ? (page > pageCount && pageCount > 0 ? pageCount : currentPage)
                : page;
            PaginateData();
        }

        public virtual void SetItemsPerPage(int numItems)
        {
            itemsPerPage = numItems;
            currentPage = 1;
            PaginateData();
        }

        public virtual void SetOnTableViewPageTurnedListener(
            Pagination.OnTableViewPageTurnedListener onTableViewPageTurnedListener)
        {
            this.onTableViewPageTurnedListener = onTableViewPageTurnedListener;
        }

        public virtual void RemoveOnTableViewPageTurnedListener()
        {
            this.onTableViewPageTurnedListener = null;
        }

        public int CurrentPage
        {
            get => GetCurrentPage();
        }

        public virtual int GetCurrentPage()
        {
            return currentPage;
        }

        public int ItemsPerPage
        {
            get => GetItemsPerPage();
            set => SetItemsPerPage(value);
        } 
        
        
        
        public virtual int GetItemsPerPage()
        {
            return itemsPerPage;
        }

        public int PageCount
        {
            get => GetPageCount();
           
        }
        public virtual int GetPageCount()
        {
            return pageCount;
        }

        public bool Paginated
        {
            get => IsPaginated();
        }


        public virtual bool IsPaginated()
        {
            return itemsPerPage > 0;
        }

        private sealed class _AdapterDataSetChangedListener_203 : AdapterDataSetChangedListener
        {
            public _AdapterDataSetChangedListener_203(Pagination _enclosing)
            {
                this._enclosing = _enclosing;
            }


            public override void OnRowHeaderItemsChanged(IList<IRowHeader> rowHeaderItems)
            {
                if (rowHeaderItems != null)
                {
                    this._enclosing.originalRowData = new AList<IRowHeader>(rowHeaderItems);
                    this._enclosing.ReloadPages();
                }
            }

            public override void OnCellItemsChanged(IList<IList<ICell>> cellItems)
            {
                if (cellItems != null)
                {
                    this._enclosing.originalCellData = new AList<IList<ICell>>(cellItems);
                    this._enclosing.ReloadPages();
                }
            }

            private readonly Pagination _enclosing;
        }

        private AdapterDataSetChangedListener adapterDataSetChangedListener;

        private sealed class _FilterChangedListener_223 : FilterChangedListener
        {
            public _FilterChangedListener_223(Pagination _enclosing)
            {
                this._enclosing = _enclosing;
            }


            public override void OnFilterChanged(IList<IList<ICell>> filteredCellItems,
                IList<IRowHeader> filteredRowHeaderItems)
            {
                this._enclosing.originalCellData = new AList<IList<ICell>>(filteredCellItems);
                this._enclosing.originalRowData = new AList<IRowHeader>(filteredRowHeaderItems);
                this._enclosing.ReloadPages();
            }

            public override void OnFilterCleared(IList<IList<ICell>> originalCellItems,
                IList<IRowHeader> originalRowHeaderItems)
            {
                this._enclosing.originalCellData = new AList<IList<ICell>>(originalCellItems);
                this._enclosing.originalRowData = new AList<IRowHeader>(originalRowHeaderItems);
                this._enclosing.ReloadPages();
            }

            private readonly Pagination _enclosing;
        }

        private FilterChangedListener filterChangedListener;

        private sealed class _ColumnSortStateChangedListener_240 : ColumnSortStateChangedListener
        {
            public _ColumnSortStateChangedListener_240(Pagination _enclosing)
            {
                this._enclosing = _enclosing;
            }

            public override void OnColumnSortStatusChanged(int column, SortState sortState)
            {
                this._enclosing.PaginateOnColumnSort(column, sortState);
            }

            public override void OnRowHeaderSortStatusChanged(SortState sortState)
            {
                this._enclosing.PaginateOnColumnSort(-1, sortState);
            }

            private readonly Pagination _enclosing;
        }

        private ColumnSortStateChangedListener columnSortStateChangedListener;

        private void PaginateOnColumnSort(int column, SortState sortState)
        {
            AList<IRowHeader> sortedRowHeaderList = new AList<IRowHeader>(originalRowData);
            AList<IList<ICell>> sortedList = new AList<IList<ICell>>(originalCellData);
            if (sortState != SortState.Unsorted)
            {
                if (column == -1)
                {
                    sortedRowHeaderList.Sort(new RowHeaderSortComparator(sortState));
                    RowHeaderForCellSortComparator rowHeaderForCellSortComparator =
                        new RowHeaderForCellSortComparator(originalRowData, originalCellData, sortState);
                    sortedList.Sort(rowHeaderForCellSortComparator);
                }
                else
                {
                    sortedList.Sort(new ColumnSortComparator(column, sortState));
                    ColumnForRowHeaderSortComparator columnForRowHeaderSortComparator =
                        new ColumnForRowHeaderSortComparator(originalRowData, originalCellData, column, sortState);
                    sortedRowHeaderList.Sort(columnForRowHeaderSortComparator);
                }
            }

            originalRowData = new AList<IRowHeader>(sortedRowHeaderList);
            originalCellData = new AList<IList<ICell>>(sortedList);
            ReloadPages();
        }

        public class PageTurnedEventArgs : EventArgs
        {
            public  int NumItems { get; set;  }
            public int ItemsStart { get; set;  }
            public int ItemsEnd { get; set; }
        }
        public event EventHandler<PageTurnedEventArgs> PageTurned;
        public void OnPageTurned(int numItems, int itemsStart, int itemsEnd)
        {
            PageTurned.SafeFire(this, new PageTurnedEventArgs() { NumItems = numItems, ItemsEnd = itemsEnd, ItemsStart = itemsStart });
        }

        /// <summary>Listener interface for changing of TableView page.</summary>
        public interface OnTableViewPageTurnedListener
        {
            /// <summary>Called when the page is changed in the TableView.</summary>
            /// <param name="numItems">The number of items currently being displayed in the TableView.
            /// 	</param>
            /// <param name="itemsStart">The starting item currently being displayed in the TableView.
            /// 	</param>
            /// <param name="itemsEnd">The ending item currently being displayed in the TableView.
            /// 	</param>
            void OnPageTurned(int numItems, int itemsStart, int itemsEnd);
        }
    }
}
