﻿/*
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

using System;


namespace Com.Evrencoskun.Tableview.Filter
{
    [System.Serializable]
    public class FilterException : Exception
    {
        public FilterException() : base("Error in searching from table.")
        {
        }

        public FilterException(string column, string query) : base(
            "Error searching " + query + " in column " + column + " of table. Column contents " + "are not of class " +
            typeof(string).FullName)
        {
        }
    }
}