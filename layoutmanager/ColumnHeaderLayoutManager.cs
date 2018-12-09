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
using Com.Evrencoskun.Tableview;
using Com.Evrencoskun.Tableview.Adapter.Recyclerview.Holder;
using Com.Evrencoskun.Tableview.Util;


namespace Com.Evrencoskun.Tableview.Layoutmanager
{
    /// <summary>Created by evrencoskun on 30/07/2017.</summary>
    public class ColumnHeaderLayoutManager : LinearLayoutManager
    {
        private IDictionary<int, int> mCachedWidthList = new Dictionary<int, int>();

        private ITableView mTableView;

        public ColumnHeaderLayoutManager(Context context, ITableView tableView) : base(context)
        {
            //private SparseArray<Integer> mCachedWidthList;
            mTableView = tableView;
            this.Orientation = LinearLayoutManager.Horizontal;
        }

        public override void MeasureChildWithMargins(Android.Views.View child, int widthUsed, int heightUsed)
        {
            base.MeasureChildWithMargins(child, widthUsed, heightUsed);
            // If has fixed width is true, than calculation of the column width is not necessary.
            if (mTableView.HasFixedWidth())
            {
                return;
            }

            MeasureChild(child, widthUsed, heightUsed);
        }

        public override void MeasureChild(Android.Views.View child, int widthUsed, int heightUsed)
        {
            // If has fixed width is true, than calculation of the column width is not necessary.
            if (mTableView.HasFixedWidth())
            {
                base.MeasureChild(child, widthUsed, heightUsed);
                return;
            }

            int position = GetPosition(child);
            int cacheWidth = GetCacheWidth(position);
            // If the width value of the cell has already calculated, then set the value
            if (cacheWidth != -1)
            {
                TableViewUtils.SetWidth(child, cacheWidth);
            }
            else
            {
                base.MeasureChild(child, widthUsed, heightUsed);
            }
        }

        public virtual void SetCacheWidth(int position, int width)
        {
            mCachedWidthList[position] = width;
        }

        public virtual int GetCacheWidth(int position)
        {
            int cachedWidth = 0;
            if (mCachedWidthList.TryGetValue(position, out cachedWidth))
            {
                return cachedWidth;
            }
            else
            {
                return -1;
            }
        }

        public int FirstItemLeft => GetFirstItemLeft();

        public virtual int GetFirstItemLeft()
        {
            Android.Views.View firstColumnHeader = FindViewByPosition(FindFirstVisibleItemPosition());
            return firstColumnHeader.Left;
        }

        /// <summary>Helps to recalculate the width value of the cell that is located in given position.
        /// 	</summary>
        public virtual void RemoveCachedWidth(int position)
        {
            mCachedWidthList.Remove(position);
        }

        /// <summary>Clears the widths which have been calculated and reused.</summary>
        public virtual void ClearCachedWidths()
        {
            mCachedWidthList.Clear();
        }

        public virtual void CustomRequestLayout()
        {
            int left = GetFirstItemLeft();
            int right;
            for (int i = FindFirstVisibleItemPosition(); i < FindLastVisibleItemPosition() + 1; i++)
            {
                // Column headers should have been already calculated.
                right = left + GetCacheWidth(i);
                Android.Views.View columnHeader = FindViewByPosition(i);
                columnHeader.Left = left;
                columnHeader.Right = right;
                LayoutDecoratedWithMargins(columnHeader, columnHeader.Left, columnHeader.Top, columnHeader.Right,
                    columnHeader.Bottom);
                // + 1 is for decoration item.
                left = right + 1;
            }
        }

        public AbstractViewHolder[] VisibleViewHolders => GetVisibleViewHolders();

        public virtual AbstractViewHolder[] GetVisibleViewHolders()
        {
            int visibleChildCount = FindLastVisibleItemPosition() - FindFirstVisibleItemPosition() + 1;
            int index = 0;
            AbstractViewHolder[] views = new AbstractViewHolder[visibleChildCount];
            for (int i = FindFirstVisibleItemPosition(); i < FindLastVisibleItemPosition() + 1; i++)
            {
                views[index] =
                    (AbstractViewHolder) mTableView.GetColumnHeaderRecyclerView().FindViewHolderForAdapterPosition(i);
                index++;
            }

            return views;
        }

        public virtual AbstractViewHolder GetViewHolder(int xPosition)
        {
            return (AbstractViewHolder) mTableView.GetColumnHeaderRecyclerView()
                .FindViewHolderForAdapterPosition(xPosition);
        }
    }
}
