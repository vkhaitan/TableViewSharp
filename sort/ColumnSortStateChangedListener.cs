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


namespace Com.Evrencoskun.Tableview.Sort
{
    public abstract class ColumnSortStateChangedListener
    {
        /// <summary>Dispatches sorting changes on a column to listeners.</summary>
        /// <param name="column">Column to be sorted.</param>
        /// <param name="sortState">SortState of the column to be sorted.</param>
        public virtual void OnColumnSortStatusChanged(int column, SortState sortState)
        {
        }

        /// <summary>Dispatches sorting changes to the row header column to listeners.</summary>
        /// <param name="sortState">SortState of the row header column.</param>
        public virtual void OnRowHeaderSortStatusChanged(SortState sortState)
        {
        }
    }
}
