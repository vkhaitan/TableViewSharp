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
using Com.Evrencoskun.Tableview.Adapter.Recyclerview.Holder;
using Com.Evrencoskun.Tableview.Handler;
using Com.Evrencoskun.Tableview.Layoutmanager;
using Com.Evrencoskun.Tableview.Listener.Itemclick;


namespace Com.Evrencoskun.Tableview.Adapter.Recyclerview
{
    /// <summary>Created by evrencoskun on 10/06/2017.</summary>
    public class CellRecyclerViewAdapter : AbstractRecyclerViewAdapter<IList<ICell>>
    {
        private static readonly string LogTag =
            typeof(Com.Evrencoskun.Tableview.Adapter.Recyclerview.CellRecyclerViewAdapter).Name;

        private ITableView mTableView;

        private readonly RecyclerView.RecycledViewPool mRecycledViewPool;

        private int mRecyclerViewId = 0;

        public CellRecyclerViewAdapter(Context context, IList<IList<ICell>> itemList, ITableView tableView) : base(
            context, itemList)
        {
            // This is for testing purpose
            this.mTableView = tableView;
            // Create view pool to share Views between multiple RecyclerViews.
            mRecycledViewPool = new RecyclerView.RecycledViewPool();
        }

        //TODO set the right value.
        //mRecycledViewPool.setMaxRecycledViews(0, 110);
        public sealed override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            return OnCreateAbstractViewHolder(parent, viewType);
        }

        public virtual AbstractViewHolder OnCreateAbstractViewHolder(ViewGroup parent, int viewType)
        {
            // Create a RecyclerView as a Row of the CellRecyclerView
            CellRecyclerView recyclerView = new CellRecyclerView(mContext);
            // Use the same view pool
            recyclerView.SetRecycledViewPool(mRecycledViewPool);
            if (mTableView.IsShowHorizontalSeparators())
            {
                // Add divider
                recyclerView.AddItemDecoration(mTableView.GetHorizontalItemDecoration());
            }

            // To get better performance for fixed size TableView
            recyclerView.HasFixedSize = mTableView.HasFixedWidth();
            // set touch mHorizontalListener to scroll synchronously
            recyclerView.AddOnItemTouchListener(mTableView.GetHorizontalRecyclerViewListener());
            // Add Item click listener for cell views
            recyclerView.AddOnItemTouchListener(new CellRecyclerViewItemClickListener(recyclerView, mTableView));
            // Set the Column layout manager that helps the fit width of the cell and column header
            // and it also helps to locate the scroll position of the horizontal recyclerView
            // which is row recyclerView
            recyclerView.SetLayoutManager(new ColumnLayoutManager(mContext, mTableView));
            // Create CellRow adapter
            recyclerView.SetAdapter(new CellRowRecyclerViewAdapter(mContext, mTableView));
            // This is for testing purpose to find out which recyclerView is displayed.
            recyclerView.Id = mRecyclerViewId;
            mRecyclerViewId++;
            return new CellRecyclerViewAdapter.CellRowViewHolder(recyclerView);
        }

        public sealed override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            OnBindViewHolder((AbstractViewHolder) holder, position);
        }

        public virtual void OnBindViewHolder(AbstractViewHolder holder, int yPosition)
        {
            CellRecyclerViewAdapter.CellRowViewHolder viewHolder = (CellRecyclerViewAdapter.CellRowViewHolder) holder;
            CellRowRecyclerViewAdapter viewAdapter = (CellRowRecyclerViewAdapter) viewHolder.recyclerView.GetAdapter();
            // Get the list
            IList<ICell> rowList = mItemList[yPosition];
            // Set Row position
            viewAdapter.SetYPosition(yPosition);
            // Set the list to the adapter
            viewAdapter.SetItems(rowList);
        }

        public sealed override void OnViewAttachedToWindow(Java.Lang.Object holder)
        {
            OnViewAttachedToWindow((AbstractViewHolder) holder);
        }


        public virtual void OnViewAttachedToWindow(AbstractViewHolder holder)
        {
            base.OnViewAttachedToWindow(holder);
            CellRecyclerViewAdapter.CellRowViewHolder viewHolder = (CellRecyclerViewAdapter.CellRowViewHolder) holder;
            ScrollHandler scrollHandler = mTableView.GetScrollHandler();
            // The below code helps to display a new attached recyclerView on exact scrolled position.
            ((ColumnLayoutManager) viewHolder.recyclerView.GetLayoutManager()).ScrollToPositionWithOffset(
                scrollHandler.GetColumnPosition(), scrollHandler.GetColumnPositionOffset());
            SelectionHandler selectionHandler = mTableView.GetSelectionHandler();
            if (selectionHandler.IsAnyColumnSelected())
            {
                AbstractViewHolder cellViewHolder =
                    (AbstractViewHolder) viewHolder.recyclerView.FindViewHolderForAdapterPosition(
                        selectionHandler.GetSelectedColumnPosition());
                if (cellViewHolder != null)
                {
                    // Control to ignore selection color
                    if (!mTableView.IsIgnoreSelectionColors())
                    {
                        cellViewHolder.SetBackgroundColor(mTableView.GetSelectedColor());
                    }

                    cellViewHolder.SetSelected(AbstractViewHolder.SelectionState.Selected);
                }
            }
            else
            {
                if (selectionHandler.IsRowSelected(holder.AdapterPosition))
                {
                    selectionHandler.ChangeSelectionOfRecyclerView(viewHolder.recyclerView,
                        AbstractViewHolder.SelectionState.Selected, mTableView.GetSelectedColor());
                }
            }
        }

        public sealed override void OnViewDetachedFromWindow(Java.Lang.Object holder)
        {
            OnViewDetachedFromWindow((AbstractViewHolder) holder);
        }

        public virtual void OnViewDetachedFromWindow(AbstractViewHolder holder)
        {
            base.OnViewDetachedFromWindow(holder);
            // Clear selection status of the view holder
            mTableView.GetSelectionHandler().ChangeSelectionOfRecyclerView(
                ((CellRecyclerViewAdapter.CellRowViewHolder) holder).recyclerView,
                AbstractViewHolder.SelectionState.Unselected, mTableView.GetUnSelectedColor());
        }

        public sealed override void OnViewRecycled(Java.Lang.Object holder)
        {
            OnViewRecycled((AbstractViewHolder) holder);
        }

        public virtual void OnViewRecycled(AbstractViewHolder holder)
        {
            base.OnViewRecycled(holder);
            CellRecyclerViewAdapter.CellRowViewHolder viewHolder = (CellRecyclerViewAdapter.CellRowViewHolder) holder;
            // ScrolledX should be cleared at that time. Because we need to prepare each
            // recyclerView
            // at onViewAttachedToWindow process.
            viewHolder.recyclerView.ClearScrolledX();
        }

        internal class CellRowViewHolder : AbstractViewHolder
        {
            internal readonly CellRecyclerView recyclerView;

            internal CellRowViewHolder(Android.Views.View itemView) : base(itemView)
            {
                recyclerView = (CellRecyclerView) itemView;
            }
        }

        public virtual void NotifyCellDataSetChanged()
        {
            CellRecyclerView[] visibleRecyclerViews =
                mTableView.GetCellLayoutManager().GetVisibleCellRowRecyclerViews();
            if (visibleRecyclerViews.Length > 0)
            {
                foreach (CellRecyclerView cellRowRecyclerView in visibleRecyclerViews)
                {
                    cellRowRecyclerView.GetAdapter().NotifyDataSetChanged();
                }
            }
            else
            {
                NotifyDataSetChanged();
            }
        }

        /// <summary>This method helps to get cell item model that is located on given column position.
        /// 	</summary>
        /// <param name="columnPosition"/>
        public virtual IList<ICell> GetColumnItems(int columnPosition)
        {
            IList<ICell> cellItems = new AList<ICell>();
            for (int i = 0; i < mItemList.Count; i++)
            {
                IList<ICell> rowList = (IList<ICell>) mItemList[i];
                if (rowList.Count > columnPosition)
                {
                    cellItems.Add(rowList[columnPosition]);
                }
            }

            return cellItems;
        }

        public virtual void RemoveColumnItems(int column)
        {
            // Firstly, remove columns from visible recyclerViews.
            // To be able provide removing animation, we need to notify just for given column position.
            CellRecyclerView[] visibleRecyclerViews =
                mTableView.GetCellLayoutManager().GetVisibleCellRowRecyclerViews();
            foreach (CellRecyclerView cellRowRecyclerView in visibleRecyclerViews)
            {
                ((AbstractRecyclerViewAdapter<ICell>) cellRowRecyclerView.GetAdapter()).DeleteItem(column);
            }

            // Lets change the model list silently
            // Create a new list which the column is already removed.
            IList<IList<ICell>> cellItems = new AList<IList<ICell>>();
            for (int i = 0; i < mItemList.Count; i++)
            {
                IList<ICell> rowList = new AList<ICell>((IList<ICell>) mItemList[i]);
                if (rowList.Count > column)
                {
                    rowList.RemoveAt(column);
                }

                cellItems.Add(rowList);
            }

            // Change data without notifying. Because we already did for visible recyclerViews.
            SetItems(cellItems, false);
        }

        public virtual void AddColumnItems(int column, IList<ICell> cellColumnItems)
        {
            // It should be same size with exist model list.
            if (cellColumnItems.Count != mItemList.Count || cellColumnItems.Contains(null))
            {
                return;
            }

            // Firstly, add columns from visible recyclerViews.
            // To be able provide removing animation, we need to notify just for given column position.
            CellLayoutManager layoutManager = mTableView.GetCellLayoutManager();
            for (int i = layoutManager.FindFirstVisibleItemPosition();
                i < layoutManager.FindLastVisibleItemPosition() + 1;
                i++)
            {
                // Get the cell row recyclerView that is located on i position
                RecyclerView cellRowRecyclerView = (RecyclerView) layoutManager.FindViewByPosition(i);
                // Add the item using its adapter.
                ((AbstractRecyclerViewAdapter<ICell>) cellRowRecyclerView.GetAdapter()).AddItem(column,
                    cellColumnItems[i]);
            }

            // Lets change the model list silently
            IList<IList<ICell>> cellItems = new AList<IList<ICell>>();
            for (int i_1 = 0; i_1 < mItemList.Count; i_1++)
            {
                IList<ICell> rowList = new AList<ICell>((IList<ICell>) mItemList[i_1]);
                if (rowList.Count > column)
                {
                    rowList.Insert(column, cellColumnItems[i_1]);
                }

                cellItems.Add(rowList);
            }

            // Change data without notifying. Because we already did for visible recyclerViews.
            SetItems(cellItems, false);
        }
    }
}
