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
    public class RowHeaderRecyclerViewAdapter : AbstractRecyclerViewAdapter<IRowHeader>
    {
        private ITableAdapter mTableAdapter;

        private ITableView mTableView;

        private RowHeaderSortHelper mRowHeaderSortHelper;

        public RowHeaderRecyclerViewAdapter(Context context, IList<IRowHeader> itemList, ITableAdapter tableAdapter) :
            base(context, itemList)
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
            return mTableAdapter.OnCreateRowHeaderViewHolder(parent, viewType);
        }

        public sealed override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            OnBindViewHolder((AbstractViewHolder) holder, position);
        }

        public virtual void OnBindViewHolder(AbstractViewHolder holder, int position)
        {
            mTableAdapter.OnBindRowHeaderViewHolder(holder, GetItem(position), position);
        }

        public override int GetItemViewType(int position)
        {
            return mTableAdapter.GetRowHeaderItemViewType(position);
        }

        public sealed override void OnViewAttachedToWindow(Java.Lang.Object holder)
        {
            OnViewAttachedToWindow((AbstractViewHolder) holder);
        }


        public virtual void OnViewAttachedToWindow(AbstractViewHolder viewHolder)
        {
            base.OnViewAttachedToWindow(viewHolder);
            AbstractViewHolder.SelectionState selectionState =
                mTableView.GetSelectionHandler().GetRowSelectionState(viewHolder.AdapterPosition);
            // Control to ignore selection color
            if (!mTableView.IsIgnoreSelectionColors())
            {
                // Change background color of the view considering it's selected state
                mTableView.GetSelectionHandler().ChangeRowBackgroundColorBySelectionStatus(viewHolder, selectionState);
            }

            // Change selection status
            viewHolder.SetSelected(selectionState);
        }

        public RowHeaderSortHelper RowHeaderSortHelper => GetRowHeaderSortHelper();

        public virtual RowHeaderSortHelper GetRowHeaderSortHelper()
        {
            if (mRowHeaderSortHelper == null)
            {
                // It helps to store sorting state of row headers
                this.mRowHeaderSortHelper = new RowHeaderSortHelper();
            }

            return mRowHeaderSortHelper;
        }
    }
}
