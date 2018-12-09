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

using Android.Views;
using Com.Evrencoskun.Tableview;
using Com.Evrencoskun.Tableview.Adapter.Recyclerview.Holder;


namespace Com.Evrencoskun.Tableview.Adapter
{
    /// <summary>Created by evrencoskun on 10/06/2017.</summary>
    public interface ITableAdapter
    {
        int GetColumnHeaderItemViewType(int position);

        int GetRowHeaderItemViewType(int position);

        int GetCellItemViewType(int position);

        Android.Views.View GetCornerView();

        AbstractViewHolder OnCreateCellViewHolder(ViewGroup parent, int viewType);

        void OnBindCellViewHolder(AbstractViewHolder holder, ICell cellItemModel, int columnPosition, int rowPosition);

        AbstractViewHolder OnCreateColumnHeaderViewHolder(ViewGroup parent, int viewType);

        void OnBindColumnHeaderViewHolder(AbstractViewHolder holder, IColumnHeader columnHeaderItemModel,
            int columnPosition);

        AbstractViewHolder OnCreateRowHeaderViewHolder(ViewGroup parent, int viewType);

        void OnBindRowHeaderViewHolder(AbstractViewHolder holder, IRowHeader rowHeaderItemModel, int rowPosition);

        Android.Views.View OnCreateCornerView();

        ITableView TableView { get; }

        ITableView GetTableView();

        /// <summary>Sets the listener for changes of data set on the TableView.</summary>
        /// <param name="listener">The AdapterDataSetChangedListener listener.</param>
        void AddAdapterDataSetChangedListener(AdapterDataSetChangedListener listener);
    }
}
