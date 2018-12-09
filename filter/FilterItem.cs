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


namespace Com.Evrencoskun.Tableview.Filter
{
    public class FilterItem
    {
        private FilterType filterType;

        private string filter;

        private int column;

        public FilterItem(FilterType type, int column, string filter)
        {
            this.filterType = type;
            this.column = column;
            this.filter = filter;
        }

        public FilterType FilterType
        {
            get { return GetFilterType(); }
        }

        public string Filter
        {
            get { return GetFilter(); }
        }

        public int Column
        {
            get { return GetColumn(); }
        }

        public virtual FilterType GetFilterType()
        {
            return filterType;
        }

        public virtual string GetFilter()
        {
            return filter;
        }

        public virtual int GetColumn()
        {
            return column;
        }
    }
}
