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
using Android.Views;
using Com.Evrencoskun.Tableview.Adapter;
using Com.Evrencoskun.Tableview.Adapter.Recyclerview;
using Com.Evrencoskun.Tableview.Handler;
using Com.Evrencoskun.Tableview.Layoutmanager;
using Com.Evrencoskun.Tableview.Listener;
using Com.Evrencoskun.Tableview.Listener.Scroll;
using Com.Evrencoskun.Tableview.Sort;


namespace Com.Evrencoskun.Tableview
{
    /// <summary>Created by evrencoskun on 19/06/2017.</summary>
    public interface ITableView
    {
        void AddView(Android.Views.View child, ViewGroup.LayoutParams @params);

        bool HasFixedWidth();

        bool IsIgnoreSelectionColors();

        bool IsShowHorizontalSeparators();

        bool IsShowVerticalSeparators();

        bool IsSortable();

        CellRecyclerView GetCellRecyclerView();

        CellRecyclerView GetColumnHeaderRecyclerView();

        CellRecyclerView GetRowHeaderRecyclerView();

        ColumnHeaderLayoutManager GetColumnHeaderLayoutManager();

        CellLayoutManager GetCellLayoutManager();

        LinearLayoutManager GetRowHeaderLayoutManager();

        HorizontalRecyclerViewListener GetHorizontalRecyclerViewListener();

        VerticalRecyclerViewListener GetVerticalRecyclerViewListener();

        ITableViewListener GetTableViewListener();

        SelectionHandler GetSelectionHandler();


        DividerItemDecoration GetHorizontalItemDecoration();

        DividerItemDecoration GetVerticalItemDecoration();

        SortState GetSortingStatus(int column);

        SortState GetRowHeaderSortingStatus();

        void ScrollToColumnPosition(int column);

        void ScrollToColumnPosition(int column, int offset);

        void ScrollToRowPosition(int row);

        void ScrollToRowPosition(int row, int offset);

        void ShowRow(int row);

        void HideRow(int row);

        bool IsRowVisible(int row);

        void ShowAllHiddenRows();

        void ClearHiddenRowList();

        void ShowColumn(int column);

        void HideColumn(int column);

        bool IsColumnVisible(int column);

        void ShowAllHiddenColumns();

        void ClearHiddenColumnList();

        int GetShadowColor();

        int GetSelectedColor();

        int GetUnSelectedColor();

        int GetSeparatorColor();

        void SortColumn(int columnPosition, SortState sortState);

        void SortRowHeader(SortState sortState);

        void RemeasureColumnWidth(int column);

        int GetRowHeaderWidth();

        void SetRowHeaderWidth(int rowHeaderWidth);


        /// <summary>Filters the whole table using the provided Filter object which supports multiple filters.
        ///     </summary>
        /// <param name="filter">The filter object.</param>
        void Filter(Com.Evrencoskun.Tableview.Filter.Filter filter);

        /// <summary>Retrieves the FilterHandler of the TableView.</summary>
        /// <returns>The FilterHandler of the TableView.</returns>
        /// <summary>Retrieves the ScrollHandler of the TableView.</summary>
        /// <returns>The ScrollHandler of the TableView.</returns>
        ScrollHandler GetScrollHandler();

        AbstractTableAdapter GetAdapter();
        ColumnSortHandler GetColumnSortHandler();
        FilterHandler GetFilterHandler();
    }
}
