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

using Com.Evrencoskun.Tableview;
using Com.Evrencoskun.Tableview.Preference;


namespace Com.Evrencoskun.Tableview.Handler
{
    /// <summary>Created by evrencoskun on 3.03.2018.</summary>
    public class PreferencesHandler
    {
        private TableView tableView;

        private ScrollHandler scrollHandler;

        private SelectionHandler selectionHandler;

        public PreferencesHandler(TableView tableView)
        {
            this.tableView = tableView;
            this.scrollHandler = tableView.GetScrollHandler();
            this.selectionHandler = tableView.GetSelectionHandler();
        }

        public virtual Preferences SavePreferences()
        {
            Preferences preferences = new Preferences();
            preferences.columnPosition = scrollHandler.GetColumnPosition();
            preferences.columnPositionOffset = scrollHandler.GetColumnPositionOffset();
            preferences.rowPosition = scrollHandler.GetRowPosition();
            preferences.rowPositionOffset = scrollHandler.GetRowPositionOffset();
            preferences.selectedColumnPosition = selectionHandler.GetSelectedColumnPosition();
            preferences.selectedRowPosition = selectionHandler.GetSelectedRowPosition();
            return preferences;
        }

        public virtual void LoadPreferences(Preferences preferences)
        {
            scrollHandler.ScrollToColumnPosition(preferences.columnPosition, preferences.columnPositionOffset);
            scrollHandler.ScrollToRowPosition(preferences.rowPosition, preferences.rowPositionOffset);
            selectionHandler.SetSelectedColumnPosition(preferences.selectedColumnPosition);
            selectionHandler.SetSelectedRowPosition(preferences.selectedRowPosition);
        }
    }
}
