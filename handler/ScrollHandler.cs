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
using Com.Evrencoskun.Tableview;
using Com.Evrencoskun.Tableview.Layoutmanager;


namespace Com.Evrencoskun.Tableview.Handler
{
    /// <summary>Created by evrencoskun on 13.01.2018.</summary>
    public class ScrollHandler
    {
        private ITableView mTableView;

        private CellLayoutManager mCellLayoutManager;

        private LinearLayoutManager mRowHeaderLayoutManager;

        private ColumnHeaderLayoutManager mColumnHeaderLayoutManager;

        public ScrollHandler(ITableView tableView)
        {
            this.mTableView = tableView;
            this.mCellLayoutManager = tableView.GetCellLayoutManager();
            this.mRowHeaderLayoutManager = tableView.GetRowHeaderLayoutManager();
            this.mColumnHeaderLayoutManager = tableView.GetColumnHeaderLayoutManager();
        }

        public virtual void ScrollToColumnPosition(int columnPosition)
        {
            // TableView is not on screen yet.
            if (!((Android.Views.View) mTableView).IsShown)
            {
                // Change default value of the listener
                mTableView.GetHorizontalRecyclerViewListener().SetScrollPosition(columnPosition);
            }

            // Column Header should be scrolled firstly because of fitting column width process.
            ScrollColumnHeader(columnPosition, 0);
            ScrollCellHorizontally(columnPosition, 0);
        }

        public virtual void ScrollToColumnPosition(int columnPosition, int offset)
        {
            // TableView is not on screen yet.
            if (!((Android.Views.View) mTableView).IsShown)
            {
                // Change default value of the listener
                mTableView.GetHorizontalRecyclerViewListener().SetScrollPosition(columnPosition);
                mTableView.GetHorizontalRecyclerViewListener().SetScrollPositionOffset(offset);
            }

            // Column Header should be scrolled firstly because of fitting column width process.
            ScrollColumnHeader(columnPosition, offset);
            ScrollCellHorizontally(columnPosition, offset);
        }

        public virtual void ScrollToRowPosition(int rowPosition)
        {
            mRowHeaderLayoutManager.ScrollToPosition(rowPosition);
            mCellLayoutManager.ScrollToPosition(rowPosition);
        }

        public virtual void ScrollToRowPosition(int rowPosition, int offset)
        {
            mRowHeaderLayoutManager.ScrollToPositionWithOffset(rowPosition, offset);
            mCellLayoutManager.ScrollToPositionWithOffset(rowPosition, offset);
        }

        private void ScrollCellHorizontally(int columnPosition, int offset)
        {
            CellLayoutManager cellLayoutManager = mTableView.GetCellLayoutManager();
            for (int i = cellLayoutManager.FindFirstVisibleItemPosition();
                i < cellLayoutManager.FindLastVisibleItemPosition() + 1;
                i++)
            {
                RecyclerView cellRowRecyclerView = (RecyclerView) cellLayoutManager.FindViewByPosition(i);
                if (cellRowRecyclerView != null)
                {
                    ColumnLayoutManager columnLayoutManager =
                        (ColumnLayoutManager) cellRowRecyclerView.GetLayoutManager();
                    columnLayoutManager.ScrollToPositionWithOffset(columnPosition, offset);
                }
            }
        }

        private void ScrollColumnHeader(int columnPosition, int offset)
        {
            mTableView.GetColumnHeaderLayoutManager().ScrollToPositionWithOffset(columnPosition, offset);
        }

        public int ColumnPosition => GetColumnPosition();

        public virtual int GetColumnPosition()
        {
            return mColumnHeaderLayoutManager.FindFirstVisibleItemPosition();
        }
        
        public int ColumnPositionOffset => GetColumnPositionOffset();

        public virtual int GetColumnPositionOffset()
        {
            Android.Views.View child =
                mColumnHeaderLayoutManager.FindViewByPosition(mColumnHeaderLayoutManager
                    .FindFirstVisibleItemPosition());
            if (child != null)
            {
                return child.Left;
            }

            return 0;
        }
        
        public int RowPosition => GetRowPosition();

        public virtual int GetRowPosition()
        {
            return mRowHeaderLayoutManager.FindFirstVisibleItemPosition();
        }

        public int RowPositionOffset => GetRowPositionOffset();
        public virtual int GetRowPositionOffset()
        {
            Android.Views.View child =
                mRowHeaderLayoutManager.FindViewByPosition(mRowHeaderLayoutManager.FindFirstVisibleItemPosition());
            if (child != null)
            {
                return child.Left;
            }

            return 0;
        }
    }
}
