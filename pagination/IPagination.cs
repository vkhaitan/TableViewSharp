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


using Java.Security;

namespace Com.Evrencoskun.Tableview.Pagination
{
    public interface IPagination
    {
        /// <summary>Loads the next page of the data set to the table view.</summary>
        void NextPage();

        /// <summary>Loads the previous page of the data set to the table view.</summary>
        void PreviousPage();

        /// <summary>Loads the data set of the specified page to the table view.</summary>
        /// <param name="page">The page to be loaded.</param>
        void GoToPage(int page);

        /// <summary>Sets the number of items (rows) per page to be displayed in the table view.
        /// 	</summary>
        /// <param name="numItems">The number of items per page.</param>
        void SetItemsPerPage(int numItems);

        /// <summary>Sets the OnTableViewPageTurnedListener for this Pagination.</summary>
        /// <param name="onTableViewPageTurnedListener">The OnTableViewPageTurnedListener.</param>
        void SetOnTableViewPageTurnedListener(Pagination.OnTableViewPageTurnedListener onTableViewPageTurnedListener);

        /// <summary>Removes the OnTableViewPageTurnedListener for this Pagination.</summary>
        void RemoveOnTableViewPageTurnedListener();

        /// <returns>The current page loaded to the table view.</returns>
        ///
        int CurrentPage { get; }
        int GetCurrentPage();

        /// <returns>The number of items per page loaded to the table view.</returns>
        int ItemsPerPage { get; }
        int GetItemsPerPage();

        /// <returns>The number of pages in the pagination.</returns>
        int PageCount { get;  }
        int GetPageCount();

        /// <returns>Current pagination state of the table view.</returns>
        bool Paginated { get;  }
        bool IsPaginated();
    }
}
