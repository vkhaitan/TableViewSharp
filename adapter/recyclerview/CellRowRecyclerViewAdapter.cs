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
using Android.Views;
using Com.Evrencoskun.Tableview;
using Com.Evrencoskun.Tableview.Adapter;
using Com.Evrencoskun.Tableview.Adapter.Recyclerview.Holder;
using Java.Lang;


namespace Com.Evrencoskun.Tableview.Adapter.Recyclerview
{
    /// <summary>Created by evrencoskun on 10/06/2017.</summary>
    public class CellRowRecyclerViewAdapter : AbstractRecyclerViewAdapter<ICell>
    {
        private static readonly string LogTag =
            typeof(Com.Evrencoskun.Tableview.Adapter.Recyclerview.CellRowRecyclerViewAdapter).Name;

        private int mYPosition;

        private ITableAdapter mTableAdapter;

        private ITableView mTableView;

        public CellRowRecyclerViewAdapter(Context context, ITableView tableView) : base(context, null)
        {
            this.mTableAdapter = tableView.GetAdapter();
            this.mTableView = tableView;
        }

        public sealed override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            return OnCreateAbstractViewHolder(parent, viewType);
        }

        public virtual RecyclerView.ViewHolder OnCreateAbstractViewHolder(ViewGroup parent, int viewType)
        {
            return mTableAdapter.OnCreateCellViewHolder(parent, viewType);
        }

        public sealed override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            OnBindViewHolder((AbstractViewHolder) holder, position);
        }

        public virtual void OnBindViewHolder(AbstractViewHolder holder, int xPosition)
        {
            mTableAdapter.OnBindCellViewHolder(holder, GetItem(xPosition), xPosition, mYPosition);
        }

        public int YPosition
        {
            get => GetYPosition();
            set => SetYPosition(value);
        }

        public virtual int GetYPosition()
        {
            return mYPosition;
        }

        public virtual void SetYPosition(int rowPosition)
        {
            mYPosition = rowPosition;
        }

        public override int GetItemViewType(int position)
        {
            return mTableAdapter.GetCellItemViewType(position);
        }

        public sealed override void OnViewAttachedToWindow(Object holder)
        {
            base.OnViewAttachedToWindow(holder);
            OnViewAttachedToWindow((AbstractViewHolder) holder);
        }

        public virtual void OnViewAttachedToWindow(AbstractViewHolder viewHolder)
        {
            AbstractViewHolder.SelectionState selectionState = mTableView.GetSelectionHandler()
                .GetCellSelectionState(viewHolder.AdapterPosition, mYPosition);
            // Control to ignore selection color
            if (!mTableView.IsIgnoreSelectionColors())
            {
                // Change the background color of the view considering selected row/cell position.
                if (selectionState == AbstractViewHolder.SelectionState.Selected)
                {
                    viewHolder.SetBackgroundColor(mTableView.GetSelectedColor());
                }
                else
                {
                    viewHolder.SetBackgroundColor(mTableView.GetUnSelectedColor());
                }
            }

            // Change selection status
            viewHolder.SetSelected(selectionState);
        }


        public sealed override bool OnFailedToRecycleView(Object holder)
        {
            return OnFailedToRecycleView((AbstractViewHolder) holder);
        }

        public virtual bool OnFailedToRecycleView(AbstractViewHolder holder)
        {
            return holder.OnFailedToRecycleView();
        }

        public sealed override void OnViewRecycled(Object holder)
        {
            base.OnViewRecycled(holder);
            OnViewRecycled((AbstractViewHolder) holder);
        }

        public virtual void OnViewRecycled(AbstractViewHolder holder)
        {
            holder.OnViewRecycled();
        }
    }
}
