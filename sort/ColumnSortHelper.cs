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

using System;
using System.Collections.Generic;
using Com.Evrencoskun.Tableview.Adapter.Recyclerview.Holder;
using Com.Evrencoskun.Tableview.Layoutmanager;


namespace Com.Evrencoskun.Tableview.Sort
{
    /// <summary>Created by evrencoskun on 15.12.2017.</summary>
    public class ColumnSortHelper
    {
        private IList<ColumnSortHelper.Directive> mSortingColumns = new AList<ColumnSortHelper.Directive>();

        private ColumnHeaderLayoutManager mColumnHeaderLayoutManager;

        public ColumnSortHelper(ColumnHeaderLayoutManager columnHeaderLayoutManager)
        {
            this.mColumnHeaderLayoutManager = columnHeaderLayoutManager;
        }

        private void SortingStatusChanged(int column, SortState sortState)
        {
            AbstractViewHolder holder = mColumnHeaderLayoutManager.GetViewHolder(column);
            if (holder != null)
            {
                if (holder is AbstractSorterViewHolder)
                {
                    ((AbstractSorterViewHolder) holder).OnSortingStatusChanged(sortState);
                }
                else
                {
                    throw new ArgumentException("Column Header ViewHolder must extend " + "AbstractSorterViewHolder");
                }
            }
        }

        public virtual void SetSortingStatus(int column, SortState status)
        {
            ColumnSortHelper.Directive directive = GetDirective(column);
            if (directive != EmptyDirective)
            {
                mSortingColumns.Remove(directive);
            }

            if (status != SortState.Unsorted)
            {
                mSortingColumns.Add(new ColumnSortHelper.Directive(column, status));
            }

            SortingStatusChanged(column, status);
        }

        public virtual void ClearSortingStatus()
        {
            mSortingColumns.Clear();
        }

        public bool Sorting => IsSorting();

        public virtual bool IsSorting()
        {
            return mSortingColumns.Count != 0;
        }

        public virtual SortState GetSortingStatus(int column)
        {
            return GetDirective(column).direction;
        }

        private ColumnSortHelper.Directive GetDirective(int column)
        {
            for (int i = 0; i < mSortingColumns.Count; i++)
            {
                ColumnSortHelper.Directive directive = mSortingColumns[i];
                if (directive.column == column)
                {
                    return directive;
                }
            }

            return EmptyDirective;
        }

        public class Directive
        {
            public int column;

            public SortState direction;

            public Directive(int column, SortState direction)
            {
                this.column = column;
                this.direction = direction;
            }
        }

        private static ColumnSortHelper.Directive EmptyDirective =
            new ColumnSortHelper.Directive(-1, SortState.Unsorted);
    }
}
