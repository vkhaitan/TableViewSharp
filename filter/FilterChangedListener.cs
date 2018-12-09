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


namespace Com.Evrencoskun.Tableview.Filter
{
    public abstract class FilterChangedListener
    {
        /// <summary>Called when a filter has been changed.</summary>
        /// <param name="filteredCellItems">The list of filtered cell items.</param>
        /// <param name="filteredRowHeaderItems">The list of filtered row items.</param>
        public virtual void OnFilterChanged(IList<IList<ICell>> filteredCellItems,
            IList<IRowHeader> filteredRowHeaderItems)
        {
        }

        /// <summary>Called when the TableView filters are cleared.</summary>
        /// <param name="originalCellItems">The unfiltered cell item list.</param>
        /// <param name="originalRowHeaderItems">The unfiltered row header list.</param>
        public virtual void OnFilterCleared(IList<IList<ICell>> originalCellItems,
            IList<IRowHeader> originalRowHeaderItems)
        {
        }
    }
}
