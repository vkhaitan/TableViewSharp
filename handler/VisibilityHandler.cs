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
using Android.Util;
using Com.Evrencoskun.Tableview;


namespace Com.Evrencoskun.Tableview.Handler
{
    /// <summary>Created by evrencoskun on 24.12.2017.</summary>
    public class VisibilityHandler
    {
        private static readonly string LogTag = typeof(Com.Evrencoskun.Tableview.Handler.VisibilityHandler).Name;

        private ITableView mTableView;

        private SparseArray<VisibilityHandler.Row> mHideRowList = new SparseArray<VisibilityHandler.Row>();

        private SparseArray<VisibilityHandler.Column> mHideColumnList = new SparseArray<VisibilityHandler.Column>();

        public VisibilityHandler(ITableView tableView)
        {
            this.mTableView = tableView;
        }

        public virtual void HideRow(int row)
        {
            // add row the list
            mHideRowList.Put(row, GetRowValueFromPosition(row));
            // remove row model from adapter
            mTableView.GetAdapter().RemoveRow(row);
        }

        public virtual void ShowRow(int row)
        {
            ShowRow(row, true);
        }

        private void ShowRow(int row, bool removeFromList)
        {
            VisibilityHandler.Row hiddenRow = mHideRowList.Get(row);
            if (hiddenRow != null)
            {
                // add row model to the adapter
                mTableView.GetAdapter().AddRow(row, (Adapter.IRowHeader) hiddenRow.GetRowHeaderModel(),
                    (IList<Adapter.ICell>) hiddenRow.GetCellModelList());
            }
            else
            {
                Log.Error(LogTag, "This row is already visible.");
            }

            if (removeFromList)
            {
                mHideRowList.Remove(row);
            }
        }

        public virtual void ClearHideRowList()
        {
            mHideRowList.Clear();
        }

        public virtual void ShowAllHiddenRows()
        {
            for (int i = 0; i < mHideRowList.Size(); i++)
            {
                int row = mHideRowList.KeyAt(i);
                ShowRow(row, false);
            }

            ClearHideRowList();
        }

        public virtual bool IsRowVisible(int row)
        {
            return mHideRowList.Get(row) == null;
        }

        public virtual void HideColumn(int column)
        {
            // add column the list
            mHideColumnList.Put(column, GetColumnValueFromPosition(column));
            // remove row model from adapter
            mTableView.GetAdapter().RemoveColumn(column);
        }

        public virtual void ShowColumn(int column)
        {
            ShowColumn(column, true);
        }

        private void ShowColumn(int column, bool removeFromList)
        {
            VisibilityHandler.Column hiddenColumn = mHideColumnList.Get(column);
            if (hiddenColumn != null)
            {
                // add column model to the adapter
                mTableView.GetAdapter().AddColumn(column, (Adapter.IColumnHeader) hiddenColumn.GetColumnHeaderModel(),
                    (IList<Adapter.ICell>) hiddenColumn.GetCellModelList());
            }
            else
            {
                Log.Error(LogTag, "This column is already visible.");
            }

            if (removeFromList)
            {
                mHideColumnList.Remove(column);
            }
        }

        public virtual void ClearHideColumnList()
        {
            mHideColumnList.Clear();
        }

        public virtual void ShowAllHiddenColumns()
        {
            for (int i = 0; i < mHideColumnList.Size(); i++)
            {
                int column = mHideColumnList.KeyAt(i);
                ShowColumn(column, false);
            }

            ClearHideColumnList();
        }

        public virtual bool IsColumnVisible(int column)
        {
            return mHideColumnList.Get(column) == null;
        }

        public class Row
        {
            private int mYPosition;

            private object mRowHeaderModel;

            private IList<object> mCellModelList;

            public Row(VisibilityHandler _enclosing, int row, object rowHeaderModel, IList<object> cellModelList)
            {
                this._enclosing = _enclosing;
                this.mYPosition = row;
                this.mRowHeaderModel = rowHeaderModel;
                this.mCellModelList = cellModelList;
            }
            
            public int YPosition => GetYPosition();
            public object RowHeaderModel => GetRowHeaderModel();
            public IList<object> CellModelList => GetCellModelList();


            public virtual int GetYPosition()
            {
                return this.mYPosition;
            }

            public virtual object GetRowHeaderModel()
            {
                return this.mRowHeaderModel;
            }

            public virtual IList<object> GetCellModelList()
            {
                return this.mCellModelList;
            }

            private readonly VisibilityHandler _enclosing;
        }

        public class Column
        {
            private int mYPosition;

            private object mColumnHeaderModel;

            private IList<object> mCellModelList;

            public Column(VisibilityHandler _enclosing, int yPosition, object columnHeaderModel,
                IList<object> cellModelList)
            {
                this._enclosing = _enclosing;
                this.mYPosition = yPosition;
                this.mColumnHeaderModel = columnHeaderModel;
                this.mCellModelList = cellModelList;
            }

            public int YPosition => GetYPosition();
            public object ColumnHeaderModel => GetColumnHeaderModel();
            public IList<object> CellModelList => GetCellModelList();

            public virtual int GetYPosition()
            {
                return this.mYPosition;
            }

            public virtual object GetColumnHeaderModel()
            {
                return this.mColumnHeaderModel;
            }

            public virtual IList<object> GetCellModelList()
            {
                return this.mCellModelList;
            }

            private readonly VisibilityHandler _enclosing;
        }

        private VisibilityHandler.Row GetRowValueFromPosition(int row)
        {
            object rowHeaderModel = mTableView.GetAdapter().GetRowHeaderItem(row);
            IList<object> cellModelList = (IList<object>) mTableView.GetAdapter().GetCellRowItems(row);
            return new VisibilityHandler.Row(this, row, rowHeaderModel, cellModelList);
        }

        private VisibilityHandler.Column GetColumnValueFromPosition(int column)
        {
            object columnHeaderModel = mTableView.GetAdapter().GetColumnHeaderItem(column);
            IList<object> cellModelList = (IList<object>) mTableView.GetAdapter().GetCellColumnItems(column);
            return new VisibilityHandler.Column(this, column, columnHeaderModel, cellModelList);
        }

        public SparseArray<VisibilityHandler.Row> HideRowList => GetHideRowList();


        public virtual SparseArray<VisibilityHandler.Row> GetHideRowList()
        {
            return mHideRowList;
        }
        
        public SparseArray<VisibilityHandler.Column> HideColumnList => GetHideColumnList();

        public virtual SparseArray<VisibilityHandler.Column> GetHideColumnList()
        {
            return mHideColumnList;
        }

        public virtual void SetHideRowList(SparseArray<VisibilityHandler.Row> rowList)
        {
            this.mHideRowList = rowList;
        }

        public virtual void SetHideColumnList(SparseArray<VisibilityHandler.Column> columnList)
        {
            this.mHideColumnList = columnList;
        }
    }
}
