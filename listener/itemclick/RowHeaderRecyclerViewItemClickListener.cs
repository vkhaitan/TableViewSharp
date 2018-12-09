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
using Com.Evrencoskun.Tableview;
using Com.Evrencoskun.Tableview.Adapter.Recyclerview;
using Com.Evrencoskun.Tableview.Adapter.Recyclerview.Holder;


namespace Com.Evrencoskun.Tableview.Listener.Itemclick
{
    /// <summary>Created by evrencoskun on 26/09/2017.</summary>
    public class RowHeaderRecyclerViewItemClickListener : AbstractItemClickListener
    {
        public RowHeaderRecyclerViewItemClickListener(CellRecyclerView recyclerView, ITableView tableView) : base(
            recyclerView, tableView)
        {
        }

        protected internal override bool ClickAction(RecyclerView view, MotionEvent e)
        {
            // Get interacted view from x,y coordinate.
            Android.Views.View childView = view.FindChildViewUnder(e.GetX(), e.GetY());
            if (childView != null )
            {
                // Find the view holder
                AbstractViewHolder holder = (AbstractViewHolder) mRecyclerView.GetChildViewHolder(childView);
                int row = holder.AdapterPosition;
                // Control to ignore selection color
                if (!mTableView.IsIgnoreSelectionColors())
                {
                    mSelectionHandler.SetSelectedRowPosition(holder, row);
                }

                if (GetTableViewListener() != null)
                {
                    // Call ITableView listener for item click
                    GetTableViewListener().OnRowHeaderClicked(holder, row);
                }

                return true;
            }

            return false;
        }

        protected internal override void LongPressAction(MotionEvent e)
        {
            // Consume the action for the time when the recyclerView is scrolling.
            if (mRecyclerView.ScrollState != RecyclerView.ScrollStateIdle)
            {
                return;
            }

            // Get interacted view from x,y coordinate.
            Android.Views.View child = mRecyclerView.FindChildViewUnder(e.GetX(), e.GetY());
            if (child != null && GetTableViewListener() != null)
            {
                // Find the view holder
                RecyclerView.ViewHolder holder = mRecyclerView.GetChildViewHolder(child);
                // Call ITableView listener for long click
                GetTableViewListener().OnRowHeaderLongPressed(holder, holder.AdapterPosition);
            }
        }
    }
}
