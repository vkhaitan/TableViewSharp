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

using Com.Evrencoskun.Tableview.Sort;


namespace Com.Evrencoskun.Tableview.Adapter.Recyclerview.Holder
{
    /// <summary>Created by evrencoskun on 16.12.2017.</summary>
    public class AbstractSorterViewHolder : AbstractViewHolder
    {
        private SortState mSortState = SortState.Unsorted;

        public AbstractSorterViewHolder(Android.Views.View itemView) : base(itemView)
        {
        }

        public virtual void OnSortingStatusChanged(SortState pSortState)
        {
            this.mSortState = pSortState;
        }

        public SortState SortState => mSortState;

        public virtual SortState GetSortState()
        {
            return mSortState;
        }
    }
}
