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
using Com.Evrencoskun.Tableview.Adapter;

namespace Com.Evrencoskun.Tableview.Sort
{
    /// <summary>Created by evrencoskun on 25.11.2017.</summary>
    public class ColumnSortComparator : AbstractSortComparator, IComparer<(IRowHeader row, IList<ICell> cells)>
    {
        private int mXPosition;

        public ColumnSortComparator(int xPosition, SortState sortState)
        {
            this.mXPosition = xPosition;
            this.mSortState = sortState;
        }

        public virtual int Compare(IList<ICell> t1, IList<ICell> t2)
        {
            object o1 = ((ISortableModel) t1[mXPosition]).GetContent();
            object o2 = ((ISortableModel) t2[mXPosition]).GetContent();
            if (mSortState == SortState.Descending)
            {
                return CompareContent(o2, o1);
            }
            else
            {
                // Default sorting process is ASCENDING
                return CompareContent(o1, o2);
            }
        }

        public int Compare((IRowHeader row, IList<ICell> cells) t1, (IRowHeader row, IList<ICell> cells) t2)
        {
            object o1 = ((ISortableModel)t1.cells[mXPosition]).GetContent();
            object o2 = ((ISortableModel)t2.cells[mXPosition]).GetContent();
            if (mSortState == SortState.Descending)
            {
                return CompareContent(o2, o1);
            }
            else
            {
                // Default sorting process is ASCENDING
                return CompareContent(o1, o2);
            }
        }
    }
}
