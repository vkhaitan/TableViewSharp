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


namespace Com.Evrencoskun.Tableview.Adapter
{
    public abstract class AdapterDataSetChangedListener
    {
        /// <summary>Dispatches changes on column header items to listener.</summary>
        /// <param name="columnHeaderItems">The current column header items.</param>
        public virtual void OnColumnHeaderItemsChanged(IList<IColumnHeader> columnHeaderItems)
        {
        }

        /// <summary>Dispatches changes on row header items to listener.</summary>
        /// <param name="rowHeaderItems">The current row header items.</param>
        public virtual void OnRowHeaderItemsChanged(IList<IRowHeader> rowHeaderItems)
        {
        }

        /// <summary>Dispatches changes on cell items to listener.</summary>
        /// <param name="cellItems">The current cell items.</param>
        public virtual void OnCellItemsChanged(IList<IList<ICell>> cellItems)
        {
        }

        /// <summary>Dispatches the changes on column header, row header and cell items.</summary>
        /// <param name="columnHeaderItems">The current column header items.</param>
        /// <param name="rowHeaderItems">The current row header items.</param>
        /// <param name="cellItems">The current cell items.</param>
        public virtual void OnDataSetChanged(IList<IColumnHeader> columnHeaderItems, IList<IRowHeader> rowHeaderItems,
            IList<IList<ICell>> cellItems)
        {
        }
    }
}
