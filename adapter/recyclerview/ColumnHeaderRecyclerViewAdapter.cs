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

using System.Collections.Generic;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Com.Evrencoskun.Tableview;
using Com.Evrencoskun.Tableview.Adapter;
using Com.Evrencoskun.Tableview.Adapter.Recyclerview.Holder;
using Com.Evrencoskun.Tableview.Sort;


namespace Com.Evrencoskun.Tableview.Adapter.Recyclerview
{
    /// <summary>Created by evrencoskun on 10/06/2017.</summary>
    public class ColumnHeaderRecyclerViewAdapter : AbstractRecyclerViewAdapter<IColumnHeader>
    {
        private static readonly string LogTag =
            typeof(Com.Evrencoskun.Tableview.Adapter.Recyclerview.ColumnHeaderRecyclerViewAdapter).Name;

        private ITableAdapter mTableAdapter;

        private ITableView mTableView;

        private ColumnSortHelper mColumnSortHelper;

        public ColumnHeaderRecyclerViewAdapter(Context context, IList<IColumnHeader> itemList,
            ITableAdapter tableAdapter) : base(context, itemList)
        {
            this.mTableAdapter = tableAdapter;
            this.mTableView = tableAdapter.GetTableView();
        }

        public sealed override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            return OnCreateAbstractViewHolder(parent, viewType);
        }

        public virtual AbstractViewHolder OnCreateAbstractViewHolder(ViewGroup parent, int viewType)
        {
            return mTableAdapter.OnCreateColumnHeaderViewHolder(parent, viewType);
        }

        public sealed override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            OnBindViewHolder((AbstractViewHolder) holder, position);
        }

        public virtual void OnBindViewHolder(AbstractViewHolder holder, int position)
        {
            mTableAdapter.OnBindColumnHeaderViewHolder(holder, GetItem(position), position);
        }

        public override int GetItemViewType(int position)
        {
            return mTableAdapter.GetColumnHeaderItemViewType(position);
        }

        public sealed override void OnViewAttachedToWindow(Java.Lang.Object holder)
        {
            base.OnViewAttachedToWindow(holder);
            OnViewAttachedToWindow((AbstractViewHolder) holder);
        }

        public virtual void OnViewAttachedToWindow(AbstractViewHolder viewHolder)
        {
            AbstractViewHolder.SelectionState selectionState =
                mTableView.GetSelectionHandler().GetColumnSelectionState(viewHolder.AdapterPosition);
            // Control to ignore selection color
            if (!mTableView.IsIgnoreSelectionColors())
            {
                // Change background color of the view considering it's selected state
                mTableView.GetSelectionHandler()
                    .ChangeColumnBackgroundColorBySelectionStatus(viewHolder, selectionState);
            }

            // Change selection status
            viewHolder.SetSelected(selectionState);
            // Control whether the TableView is sortable or not.
            if (mTableView.IsSortable())
            {
                if (viewHolder is AbstractSorterViewHolder)
                {
                    // Get its sorting state
                    SortState state = GetColumnSortHelper().GetSortingStatus(viewHolder.AdapterPosition);
                    // Fire onSortingStatusChanged
                    ((AbstractSorterViewHolder) viewHolder).OnSortingStatusChanged(state);
                }
            }
        }

        public ColumnSortHelper ColumnSortHelper => GetColumnSortHelper();

        public virtual ColumnSortHelper GetColumnSortHelper()
        {
            if (mColumnSortHelper == null)
            {
                // It helps to store sorting state of column headers
                this.mColumnSortHelper = new ColumnSortHelper(mTableView.GetColumnHeaderLayoutManager());
            }

            return mColumnSortHelper;
        }
    }
}
