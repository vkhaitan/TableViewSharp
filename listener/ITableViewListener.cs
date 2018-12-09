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


namespace Com.Evrencoskun.Tableview.Listener
{
    /// <summary>Created by evrencoskun on 20/09/2017.</summary>
    public interface ITableViewListener
    {
        void OnCellClicked(RecyclerView.ViewHolder cellView, int column, int row);

        void OnCellLongPressed(RecyclerView.ViewHolder cellView, int column, int row);

        void OnColumnHeaderClicked(RecyclerView.ViewHolder columnHeaderView, int column);

        void OnColumnHeaderLongPressed(RecyclerView.ViewHolder columnHeaderView, int column);

        void OnRowHeaderClicked(RecyclerView.ViewHolder rowHeaderView, int row);

        void OnRowHeaderLongPressed(RecyclerView.ViewHolder rowHeaderView, int row);
    }
}
