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
using Com.Evrencoskun.Tableview;
using Com.Evrencoskun.Tableview.Adapter.Recyclerview;
using Com.Evrencoskun.Tableview.Adapter.Recyclerview.Holder;
using Com.Evrencoskun.Tableview.Layoutmanager;


namespace Com.Evrencoskun.Tableview.Handler
{
    /// <summary>Created by evrencoskun on 24/10/2017.</summary>
    public class SelectionHandler
    {
        public const int UnselectedPosition = -1;

        private int mSelectedRowPosition = UnselectedPosition;

        private int mSelectedColumnPosition = UnselectedPosition;

        private bool shadowEnabled = true;

        private ITableView mTableView;

        private AbstractViewHolder mPreviousSelectedViewHolder;

        private CellRecyclerView mColumnHeaderRecyclerView;

        private CellRecyclerView mRowHeaderRecyclerView;

        private CellLayoutManager mCellLayoutManager;

        public SelectionHandler(ITableView tableView)
        {
            this.mTableView = tableView;
            this.mColumnHeaderRecyclerView = mTableView.GetColumnHeaderRecyclerView();
            this.mRowHeaderRecyclerView = mTableView.GetRowHeaderRecyclerView();
            this.mCellLayoutManager = mTableView.GetCellLayoutManager();
        }

        public bool ShadowEnabled
        {
            get => IsShadowEnabled();
            set => SetShadowEnabled(value);
        }

        public virtual bool IsShadowEnabled()
        {
            return shadowEnabled;
        }

        public virtual void SetShadowEnabled(bool shadowEnabled)
        {
            this.shadowEnabled = shadowEnabled;
        }

        public virtual void SetSelectedCellPositions(AbstractViewHolder selectedViewHolder, int column, int row)
        {
            this.SetPreviousSelectedView(selectedViewHolder);
            this.mSelectedColumnPosition = column;
            this.mSelectedRowPosition = row;
            if (shadowEnabled)
            {
                SelectedCellView();
            }
        }

        public virtual void SetSelectedColumnPosition(AbstractViewHolder selectedViewHolder, int column)
        {
            this.SetPreviousSelectedView(selectedViewHolder);
            this.mSelectedColumnPosition = column;
            SelectedColumnHeader();
            // Set unselected others
            mSelectedRowPosition = UnselectedPosition;
        }

        public int SelectedColumnPosition
        {
            get => GetSelectedColumnPosition();
            set => SetSelectedColumnPosition(value);
        }

        public virtual int GetSelectedColumnPosition()
        {
            return mSelectedColumnPosition;
        }

        public virtual void SetSelectedRowPosition(AbstractViewHolder selectedViewHolder, int row)
        {
            this.SetPreviousSelectedView(selectedViewHolder);
            this.mSelectedRowPosition = row;
            SelectedRowHeader();
            // Set unselected others
            mSelectedColumnPosition = UnselectedPosition;
        }

        public int SelectedRowPosition
        {
            get => GetSelectedRowPosition();
            set => SetSelectedRowPosition(value);
        }

        public virtual int GetSelectedRowPosition()
        {
            return mSelectedRowPosition;
        }

        public virtual void SetPreviousSelectedView(AbstractViewHolder viewHolder)
        {
            RestorePreviousSelectedView();
            if (mPreviousSelectedViewHolder != null)
            {
                // Change color
                mPreviousSelectedViewHolder.SetBackgroundColor(mTableView.GetUnSelectedColor());
                // Change state
                mPreviousSelectedViewHolder.SetSelected(AbstractViewHolder.SelectionState.Unselected);
            }

            AbstractViewHolder oldViewHolder =
                mCellLayoutManager.GetCellViewHolder(GetSelectedColumnPosition(), GetSelectedRowPosition());
            if (oldViewHolder != null)
            {
                // Change color
                oldViewHolder.SetBackgroundColor(mTableView.GetUnSelectedColor());
                // Change state
                oldViewHolder.SetSelected(AbstractViewHolder.SelectionState.Unselected);
            }

            this.mPreviousSelectedViewHolder = viewHolder;
            // Change color
            mPreviousSelectedViewHolder.SetBackgroundColor(mTableView.GetSelectedColor());
            // Change state
            mPreviousSelectedViewHolder.SetSelected(AbstractViewHolder.SelectionState.Selected);
        }

        private void RestorePreviousSelectedView()
        {
            if (mSelectedColumnPosition != UnselectedPosition && mSelectedRowPosition != UnselectedPosition)
            {
                UnselectedCellView();
            }
            else
            {
                if (mSelectedColumnPosition != UnselectedPosition)
                {
                    UnselectedColumnHeader();
                }
                else
                {
                    if (mSelectedRowPosition != UnselectedPosition)
                    {
                        UnselectedRowHeader();
                    }
                }
            }
        }

        private void SelectedRowHeader()
        {
            // Change background color of the selected cell views
            ChangeVisibleCellViewsBackgroundForRow(mSelectedRowPosition, true);
            // Change background color of the column headers to be shown as a shadow.
            if (shadowEnabled)
            {
                ChangeSelectionOfRecyclerView(mColumnHeaderRecyclerView, AbstractViewHolder.SelectionState.Shadowed,
                    mTableView.GetShadowColor());
            }
        }

        private void UnselectedRowHeader()
        {
            ChangeVisibleCellViewsBackgroundForRow(mSelectedRowPosition, false);
            // Change background color of the column headers to be shown as a normal.
            ChangeSelectionOfRecyclerView(mColumnHeaderRecyclerView, AbstractViewHolder.SelectionState.Unselected,
                mTableView.GetUnSelectedColor());
        }

        private void SelectedCellView()
        {
            int shadowColor = mTableView.GetShadowColor();
            // Change background color of the row header which is located on Y Position of the cell
            // view.
            AbstractViewHolder rowHeader =
                (AbstractViewHolder) mRowHeaderRecyclerView.FindViewHolderForAdapterPosition(mSelectedRowPosition);
            // If view is null, that means the row view holder was already recycled.
            if (rowHeader != null)
            {
                // Change color
                rowHeader.SetBackgroundColor(shadowColor);
                // Change state
                rowHeader.SetSelected(AbstractViewHolder.SelectionState.Shadowed);
            }

            // Change background color of the column header which is located on X Position of the cell
            // view.
            AbstractViewHolder columnHeader =
                (AbstractViewHolder) mColumnHeaderRecyclerView
                    .FindViewHolderForAdapterPosition(mSelectedColumnPosition);
            if (columnHeader != null)
            {
                // Change color
                columnHeader.SetBackgroundColor(shadowColor);
                // Change state
                columnHeader.SetSelected(AbstractViewHolder.SelectionState.Shadowed);
            }
        }

        private void UnselectedCellView()
        {
            int unSelectedColor = mTableView.GetUnSelectedColor();
            // Change background color of the row header which is located on Y Position of the cell
            // view.
            AbstractViewHolder rowHeader =
                (AbstractViewHolder) mRowHeaderRecyclerView.FindViewHolderForAdapterPosition(mSelectedRowPosition);
            // If view is null, that means the row view holder was already recycled.
            if (rowHeader != null)
            {
                // Change color
                rowHeader.SetBackgroundColor(unSelectedColor);
                // Change state
                rowHeader.SetSelected(AbstractViewHolder.SelectionState.Unselected);
            }

            // Change background color of the column header which is located on X Position of the cell
            // view.
            AbstractViewHolder columnHeader =
                (AbstractViewHolder) mColumnHeaderRecyclerView
                    .FindViewHolderForAdapterPosition(mSelectedColumnPosition);
            if (columnHeader != null)
            {
                // Change color
                columnHeader.SetBackgroundColor(unSelectedColor);
                // Change state
                columnHeader.SetSelected(AbstractViewHolder.SelectionState.Unselected);
            }
        }

        private void SelectedColumnHeader()
        {
            ChangeVisibleCellViewsBackgroundForColumn(mSelectedColumnPosition, true);
            ChangeSelectionOfRecyclerView(mRowHeaderRecyclerView, AbstractViewHolder.SelectionState.Shadowed,
                mTableView.GetShadowColor());
        }

        private void UnselectedColumnHeader()
        {
            ChangeVisibleCellViewsBackgroundForColumn(mSelectedColumnPosition, false);
            ChangeSelectionOfRecyclerView(mRowHeaderRecyclerView, AbstractViewHolder.SelectionState.Unselected,
                mTableView.GetUnSelectedColor());
        }

        public virtual bool IsCellSelected(int column, int row)
        {
            return (GetSelectedColumnPosition() == column && GetSelectedRowPosition() == row) ||
                   IsColumnSelected(column) || IsRowSelected(row);
        }

        public virtual AbstractViewHolder.SelectionState GetCellSelectionState(int column, int row)
        {
            if (IsCellSelected(column, row))
            {
                return AbstractViewHolder.SelectionState.Selected;
            }

            return AbstractViewHolder.SelectionState.Unselected;
        }

        public virtual bool IsColumnSelected(int column)
        {
            return (GetSelectedColumnPosition() == column && GetSelectedRowPosition() == UnselectedPosition);
        }

        public virtual bool IsColumnShadowed(int column)
        {
            return (GetSelectedColumnPosition() == column && GetSelectedRowPosition() != UnselectedPosition) ||
                   (GetSelectedColumnPosition() == UnselectedPosition &&
                    GetSelectedRowPosition() != UnselectedPosition);
        }

        public bool AnyColumnSelected => IsAnyColumnSelected();

        public virtual bool IsAnyColumnSelected()
        {
            return (GetSelectedColumnPosition() !=
                    Com.Evrencoskun.Tableview.Handler.SelectionHandler.UnselectedPosition && GetSelectedRowPosition() ==
                    Com.Evrencoskun.Tableview.Handler.SelectionHandler.UnselectedPosition);
        }

        public virtual AbstractViewHolder.SelectionState GetColumnSelectionState(int column)
        {
            if (IsColumnShadowed(column))
            {
                return AbstractViewHolder.SelectionState.Shadowed;
            }
            else
            {
                if (IsColumnSelected(column))
                {
                    return AbstractViewHolder.SelectionState.Selected;
                }
                else
                {
                    return AbstractViewHolder.SelectionState.Unselected;
                }
            }
        }

        public virtual bool IsRowSelected(int row)
        {
            return (GetSelectedRowPosition() == row && GetSelectedColumnPosition() == UnselectedPosition);
        }

        public virtual bool IsRowShadowed(int row)
        {
            return (GetSelectedRowPosition() == row && GetSelectedColumnPosition() != UnselectedPosition) ||
                   (GetSelectedRowPosition() == UnselectedPosition &&
                    GetSelectedColumnPosition() != UnselectedPosition);
        }

        public virtual AbstractViewHolder.SelectionState GetRowSelectionState(int row)
        {
            if (IsRowShadowed(row))
            {
                return AbstractViewHolder.SelectionState.Shadowed;
            }
            else
            {
                if (IsRowSelected(row))
                {
                    return AbstractViewHolder.SelectionState.Selected;
                }
                else
                {
                    return AbstractViewHolder.SelectionState.Unselected;
                }
            }
        }

        private void ChangeVisibleCellViewsBackgroundForRow(int row, bool isSelected)
        {
            int backgroundColor = mTableView.GetUnSelectedColor();
            AbstractViewHolder.SelectionState selectionState = AbstractViewHolder.SelectionState.Unselected;
            if (isSelected)
            {
                backgroundColor = mTableView.GetSelectedColor();
                selectionState = AbstractViewHolder.SelectionState.Selected;
            }

            CellRecyclerView recyclerView = (CellRecyclerView) mCellLayoutManager.FindViewByPosition(row);
            if (recyclerView == null)
            {
                return;
            }

            ChangeSelectionOfRecyclerView(recyclerView, selectionState, backgroundColor);
        }

        private void ChangeVisibleCellViewsBackgroundForColumn(int column, bool isSelected)
        {
            int backgroundColor = mTableView.GetUnSelectedColor();
            AbstractViewHolder.SelectionState selectionState = AbstractViewHolder.SelectionState.Unselected;
            if (isSelected)
            {
                backgroundColor = mTableView.GetSelectedColor();
                selectionState = AbstractViewHolder.SelectionState.Selected;
            }

            // Get visible Cell ViewHolders by Column Position
            for (int i = mCellLayoutManager.FindFirstVisibleItemPosition();
                i < mCellLayoutManager.FindLastVisibleItemPosition() + 1;
                i++)
            {
                CellRecyclerView cellRowRecyclerView = (CellRecyclerView) mCellLayoutManager.FindViewByPosition(i);
                AbstractViewHolder holder =
                    (AbstractViewHolder) cellRowRecyclerView?.FindViewHolderForAdapterPosition(column);
                if (holder != null)
                {
                    // Get each view container of the cell view and set unselected color.
                    holder.SetBackgroundColor(backgroundColor);
                    // Change selection status of the view holder
                    holder.SetSelected(selectionState);
                }
            }
        }

        public virtual void ChangeRowBackgroundColorBySelectionStatus(AbstractViewHolder viewHolder,
            AbstractViewHolder.SelectionState selectionState)
        {
            if (shadowEnabled && selectionState == AbstractViewHolder.SelectionState.Shadowed)
            {
                viewHolder.SetBackgroundColor(mTableView.GetShadowColor());
            }
            else
            {
                if (selectionState == AbstractViewHolder.SelectionState.Selected)
                {
                    viewHolder.SetBackgroundColor(mTableView.GetSelectedColor());
                }
                else
                {
                    viewHolder.SetBackgroundColor(mTableView.GetUnSelectedColor());
                }
            }
        }

        public virtual void ChangeColumnBackgroundColorBySelectionStatus(AbstractViewHolder viewHolder,
            AbstractViewHolder.SelectionState selectionState)
        {
            if (shadowEnabled && selectionState == AbstractViewHolder.SelectionState.Shadowed)
            {
                viewHolder.SetBackgroundColor(mTableView.GetShadowColor());
            }
            else
            {
                if (selectionState == AbstractViewHolder.SelectionState.Selected)
                {
                    viewHolder.SetBackgroundColor(mTableView.GetSelectedColor());
                }
                else
                {
                    viewHolder.SetBackgroundColor(mTableView.GetUnSelectedColor());
                }
            }
        }

        public virtual void ChangeSelectionOfRecyclerView(CellRecyclerView recyclerView,
            AbstractViewHolder.SelectionState selectionState, int backgroundColor)
        {
            LinearLayoutManager linearLayoutManager = (LinearLayoutManager) recyclerView.GetLayoutManager();
            for (int i = linearLayoutManager.FindFirstVisibleItemPosition();
                i < linearLayoutManager.FindLastVisibleItemPosition() + 1;
                i++)
            {
                AbstractViewHolder viewHolder = (AbstractViewHolder) recyclerView.FindViewHolderForAdapterPosition(i);
                if (viewHolder != null)
                {
                    if (!mTableView.IsIgnoreSelectionColors())
                    {
                        // Change background color
                        viewHolder.SetBackgroundColor(backgroundColor);
                    }

                    // Change selection status of the view holder
                    viewHolder.SetSelected(selectionState);
                }
            }
        }

        public virtual void ClearSelection()
        {
            UnselectedRowHeader();
            UnselectedCellView();
            UnselectedColumnHeader();
        }

        public virtual void SetSelectedRowPosition(int row)
        {
            this.mSelectedRowPosition = row;
        }

        public virtual void SetSelectedColumnPosition(int column)
        {
            this.mSelectedColumnPosition = column;
        }
    }
}
