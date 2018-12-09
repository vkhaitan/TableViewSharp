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
using Android.Support.V4.Util;
using Android.Support.V7.Util;
using Com.Evrencoskun.Tableview.Adapter;

namespace Com.Evrencoskun.Tableview.Sort
{
    /// <summary>Created by evrencoskun on 23.11.2017.</summary>
    public class ColumnSortCallback : DiffUtil.Callback
    {
        private static readonly string LogTag = typeof(Com.Evrencoskun.Tableview.Sort.ColumnSortCallback).Name;

        private IList<IList<ICell>> mOldCellItems;

        private IList<IList<ICell>> mNewCellItems;

        private int mColumnPosition;

        public override int NewListSize
        {
            get { return mNewCellItems.Count; }
        }

        public override int OldListSize
        {
            get { return mOldCellItems.Count; }
        }

        public ColumnSortCallback(IList<IList<ICell>> oldCellItems, IList<IList<ICell>> newCellItems, int column)
        {
            this.mOldCellItems = oldCellItems;
            this.mNewCellItems = newCellItems;
            this.mColumnPosition = column;
        }


        public override bool AreItemsTheSame(int oldItemPosition, int newItemPosition)
        {
            // Control for precaution from IndexOutOfBoundsException
            if (mOldCellItems.Count > oldItemPosition && mNewCellItems.Count > newItemPosition)
            {
                if (mOldCellItems[oldItemPosition].Count > mColumnPosition &&
                    mNewCellItems[newItemPosition].Count > mColumnPosition)
                {
                    // Compare ids
                    string oldId = ((ISortableModel) mOldCellItems[oldItemPosition][mColumnPosition]).GetId();
                    string newId = ((ISortableModel) mNewCellItems[newItemPosition][mColumnPosition]).GetId();
                    return oldId.Equals(newId);
                }
            }

            return false;
        }

        public override bool AreContentsTheSame(int oldItemPosition, int newItemPosition)
        {
            // Control for precaution from IndexOutOfBoundsException
            if (mOldCellItems.Count > oldItemPosition && mNewCellItems.Count > newItemPosition)
            {
                if (mOldCellItems[oldItemPosition].Count > mColumnPosition &&
                    mNewCellItems[newItemPosition].Count > mColumnPosition)
                {
                    // Compare contents
                    object oldContent = ((ISortableModel) mOldCellItems[oldItemPosition][mColumnPosition]).GetContent();
                    object newContent = ((ISortableModel) mNewCellItems[newItemPosition][mColumnPosition]).GetContent();
                    return ObjectsCompat.Equals(oldContent, newContent);
                }
            }

            return false;
        }
    }
}
