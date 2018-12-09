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

using System.Collections;
using System.Collections.Generic;
using Com.Evrencoskun.Tableview;
using Com.Evrencoskun.Tableview.Adapter;
using Com.Evrencoskun.Tableview.Adapter.Recyclerview;
using Com.Evrencoskun.Tableview.Filter;


namespace Com.Evrencoskun.Tableview.Handler
{
    public class FilterHandler
    {
        private CellRecyclerViewAdapter mCellRecyclerViewAdapter;

        private RowHeaderRecyclerViewAdapter mRowHeaderRecyclerViewAdapter;

        private IList<IList<ICell>> originalCellDataStore;

        private IList<IList<ICell>> originalCellData;

        private IList<IList<ICell>> filteredCellList;

        private IList<IRowHeader> originalRowDataStore;

        private IList<IRowHeader> originalRowData;

        private IList<IRowHeader> filteredRowList;

        private IList<FilterChangedListener> filterChangedListeners;

        public FilterHandler(ITableView tableView)
        {
            adapterDataSetChangedListener = new _AdapterDataSetChangedListener_120(this);
            tableView.GetAdapter().AddAdapterDataSetChangedListener(adapterDataSetChangedListener);
            this.mCellRecyclerViewAdapter = (CellRecyclerViewAdapter) tableView.GetCellRecyclerView().GetAdapter();
            this.mRowHeaderRecyclerViewAdapter =
                (RowHeaderRecyclerViewAdapter) tableView.GetRowHeaderRecyclerView().GetAdapter();
        }

        public virtual void Filter(Com.Evrencoskun.Tableview.Filter.Filter filter)
        {
            if (originalCellDataStore == null || originalRowDataStore == null)
            {
                return;
            }

            originalCellData = new AList<IList<ICell>>(originalCellDataStore);
            originalRowData = new AList<IRowHeader>(originalRowDataStore);
            filteredCellList = new AList<IList<ICell>>();
            filteredRowList = new AList<IRowHeader>();
            if (filter.GetFilterItems().Count == 0)
            {
                filteredCellList = new AList<IList<ICell>>(originalCellDataStore);
                filteredRowList = new AList<IRowHeader>(originalRowDataStore);
                DispatchFilterClearedToListeners(originalCellDataStore, originalRowDataStore);
            }
            else
            {
                for (int x = 0; x < filter.GetFilterItems().Count;)
                {
                    FilterItem filterItem = filter.GetFilterItems()[x];
                    if (filterItem.GetFilterType().Equals(FilterType.All))
                    {
                        foreach (IList<ICell> itemsList in originalCellData)
                        {
                            foreach (ICell item in itemsList)
                            {
                                if (((IFilterableModel) item).GetFilterableKeyword().ToLower()
                                    .Contains(filterItem.GetFilter().ToLower()))
                                {
                                    filteredCellList.Add(itemsList);
                                    filteredRowList.Add(originalRowData[filteredCellList.IndexOf(itemsList)]);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (IList<ICell> itemsList in originalCellData)
                        {
                            if (((IFilterableModel) itemsList[filterItem.GetColumn()]).GetFilterableKeyword().ToLower()
                                .Contains(filterItem.GetFilter().ToLower()))
                            {
                                filteredCellList.Add(itemsList);
                                filteredRowList.Add(originalRowData[filteredCellList.IndexOf(itemsList)]);
                            }
                        }
                    }

                    // If this is the last filter to be processed, the filtered lists will not be cleared.
                    if (++x < filter.GetFilterItems().Count)
                    {
                        originalCellData = new AList<IList<ICell>>(filteredCellList);
                        originalRowData = new AList<IRowHeader>(filteredRowList);
                        filteredCellList.Clear();
                        filteredRowList.Clear();
                    }
                }
            }

            // Sets the filtered data to the TableView.
            mRowHeaderRecyclerViewAdapter.SetItems(filteredRowList, true);
            mCellRecyclerViewAdapter.SetItems((IList<IList<ICell>>) filteredCellList, true);
            // Tells the listeners that the TableView is filtered.
            DispatchFilterChangedToListeners(filteredCellList, filteredRowList);
        }

        private sealed class _AdapterDataSetChangedListener_120 : AdapterDataSetChangedListener
        {
            public _AdapterDataSetChangedListener_120(FilterHandler _enclosing)
            {
                this._enclosing = _enclosing;
            }

            public override void OnRowHeaderItemsChanged(IList<IRowHeader> rowHeaderItems)
            {
                if (rowHeaderItems != null)
                {
                    this._enclosing.originalRowDataStore = new AList<IRowHeader>(rowHeaderItems);
                }
            }

            public override void OnCellItemsChanged(IList<IList<ICell>> cellItems)
            {
                if (cellItems != null)
                {
                    this._enclosing.originalCellDataStore = new AList<IList<ICell>>(cellItems);
                }
            }


            private readonly FilterHandler _enclosing;
        }

        private AdapterDataSetChangedListener adapterDataSetChangedListener;

        private void DispatchFilterChangedToListeners(IList<IList<ICell>> filteredCellItems,
            IList<IRowHeader> filteredRowHeaderItems)
        {
            if (filterChangedListeners != null)
            {
                foreach (FilterChangedListener listener in filterChangedListeners)
                {
                    listener.OnFilterChanged(filteredCellItems, filteredRowHeaderItems);
                }
            }
        }

        private void DispatchFilterClearedToListeners(IList<IList<ICell>> originalCellItems,
            IList<IRowHeader> originalRowHeaderItems)
        {
            if (filterChangedListeners != null)
            {
                foreach (FilterChangedListener listener in filterChangedListeners)
                {
                    listener.OnFilterCleared(originalCellItems, originalRowHeaderItems);
                }
            }
        }

        public virtual void AddFilterChangedListener(FilterChangedListener listener)
        {
            if (filterChangedListeners == null)
            {
                filterChangedListeners = new AList<FilterChangedListener>();
            }

            filterChangedListeners.Add(listener);
        }
    }
}
