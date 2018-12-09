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
using Android.Text;
using Com.Evrencoskun.Tableview;


namespace Com.Evrencoskun.Tableview.Filter
{
    /// <summary>Class used to store multiple filters for the TableView filtering feature.
    /// 	</summary>
    public class Filter
    {
        /// <summary>List of filter items to be used for filtering.</summary>
        private AList<FilterItem> filterItems;

        /// <summary>The TableView instance used in this scope.</summary>
        private ITableView tableView;

        /// <param name="tableView">The TableView to be filtered.</param>
        public Filter(ITableView tableView)
        {
            this.tableView = tableView;
            this.filterItems = new AList<FilterItem>();
        }

        /// <summary>Adds new filter item to the list.</summary>
        /// <remarks>
        /// Adds new filter item to the list. This should be used strictly once
        /// for filtering the whole table.
        /// </remarks>
        /// <param name="filter">the filter string.</param>
        public virtual void Set(string filter)
        {
            Set(-1, filter);
        }

        /// <summary>Adds new filter item to the list.</summary>
        /// <remarks>
        /// Adds new filter item to the list. The filter will be used on the
        /// specified column.
        /// </remarks>
        /// <param name="column">the target column for filtering.</param>
        /// <param name="filter">the filter string.</param>
        public virtual void Set(int column, string filter)
        {
            FilterItem filterItem = new FilterItem(column == -1 ? FilterType.All : FilterType.Column, column, filter);
            if (IsAlreadyFiltering(column, filterItem))
            {
                if (TextUtils.IsEmpty(filter))
                {
                    Remove(column, filterItem);
                }
                else
                {
                    Update(column, filterItem);
                }
            }
            else
            {
                if (!TextUtils.IsEmpty(filter))
                {
                    Add(filterItem);
                }
            }
        }

        /// <summary>Adds new filter item to the list of this class.</summary>
        /// <param name="filterItem">The filter item to be added to the list.</param>
        private void Add(FilterItem filterItem)
        {
            filterItems.Add(filterItem);
            tableView.Filter(this);
        }

        /// <summary>Removes a filter item from the list of this class.</summary>
        /// <param name="column">The column to be checked for removing the filter item.</param>
        /// <param name="filterItem">The filter item to be removed.</param>
        private void Remove(int column, FilterItem filterItem)
        {
            // This would remove a FilterItem from the Filter list when the filter is cleared.
            // Used Iterator for removing instead of canonical loop to prevent ConcurrentModificationException.
            int index = -1;
            for (int i = 0; i < filterItems.Count; ++i)
            {
                FilterItem item = filterItems[i];
                if (column == -1 && item.GetFilterType().Equals(filterItem.GetFilterType()))
                {
                    index = i;
                    break;
                }
                else if (item.GetColumn() == filterItem.GetColumn())
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
                filterItems.RemoveAt(index);


            tableView.Filter(this);
        }

        /// <summary>Updates a filter item from the list of this class.</summary>
        /// <param name="column">The column in which filter item will be updated.</param>
        /// <param name="filterItem">The updated filter item.</param>
        private void Update(int column, FilterItem filterItem)
        {
            // This would update an existing FilterItem from the Filter list.
            // Used Iterator for updating instead of canonical loop to prevent ConcurrentModificationException.
            int index = -1;
            for (int i = 0; i < filterItems.Count; ++i)
            {
                FilterItem item = filterItems[i];
                if (column == -1 && item.GetFilterType().Equals(filterItem.GetFilterType()))
                {
                    index = i;
                    break;
                }
                else if (item.GetColumn() == filterItem.GetColumn())
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
                filterItems[index] = filterItem;
            tableView.Filter(this);
        }

        /// <summary>Method to check if a filter item is already added based on the column to be filtered.
        /// 	</summary>
        /// <param name="column">The column to be checked if the list is already filtering.</param>
        /// <param name="filterItem">The filter item to be checked.</param>
        /// <returns>True if a filter item for a specific column or for ALL is already in the list.
        /// 	</returns>
        private bool IsAlreadyFiltering(int column, FilterItem filterItem)
        {
            // This would determine if Filter is already filtering ALL or a specified COLUMN.
            foreach (FilterItem item in filterItems)
            {
                if (column == -1 && item.GetFilterType().Equals(filterItem.GetFilterType()))
                {
                    return true;
                }
                else
                {
                    if (item.GetColumn() == filterItem.GetColumn())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>Returns the list of filter items.</summary>
        /// <returns>The list of filter items.</returns>

        public IList<FilterItem> FilterItems => GetFilterItems();
        
        public virtual IList<FilterItem> GetFilterItems()
        {
            return this.filterItems;
        }
    }
}
