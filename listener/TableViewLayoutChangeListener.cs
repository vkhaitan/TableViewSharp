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

using Android.Views;
using Com.Evrencoskun.Tableview;
using Com.Evrencoskun.Tableview.Adapter.Recyclerview;
using Com.Evrencoskun.Tableview.Layoutmanager;


namespace Com.Evrencoskun.Tableview.Listener
{
    /// <summary>Created by evrencoskun on 21.01.2018.</summary>
    public class TableViewLayoutChangeListener : Java.Lang.Object, View.IOnLayoutChangeListener
    {
        private CellRecyclerView mCellRecyclerView;

        private CellRecyclerView mColumnHeaderRecyclerView;

        private CellLayoutManager mCellLayoutManager;

        public TableViewLayoutChangeListener(ITableView tableView)
        {
            this.mCellRecyclerView = tableView.GetCellRecyclerView();
            this.mColumnHeaderRecyclerView = tableView.GetColumnHeaderRecyclerView();
            this.mCellLayoutManager = tableView.GetCellLayoutManager();
        }

        public virtual void OnLayoutChange(Android.Views.View v, int left, int top, int right, int bottom, int oldLeft,
            int oldTop, int oldRight, int oldBottom)
        {
            if (v.IsShown && (right - left) != (oldRight - oldLeft))
            {
                // Control who need the remeasure
                if (mColumnHeaderRecyclerView.Width > mCellRecyclerView.Width)
                {
                    // Remeasure all nested CellRow recyclerViews
                    mCellLayoutManager.RemeasureAllChild();
                }
                else
                {
                    if (mCellRecyclerView.Width > mColumnHeaderRecyclerView.Width)
                    {
                        // It seems Column Header is needed.
                        mColumnHeaderRecyclerView.LayoutParameters.Width = ViewGroup.LayoutParams.WrapContent;
                        mColumnHeaderRecyclerView.RequestLayout();
                    }
                }
            }
        }
    }
}
