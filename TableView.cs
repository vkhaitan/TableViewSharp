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

using System;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Support.Annotation;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Evrencoskun.Tableview.Adapter;
using Com.Evrencoskun.Tableview.Adapter.Recyclerview;
using Com.Evrencoskun.Tableview.Adapter.Recyclerview.Holder;
using Com.Evrencoskun.Tableview.Handler;
using Com.Evrencoskun.Tableview.Layoutmanager;
using Com.Evrencoskun.Tableview.Listener;
using Com.Evrencoskun.Tableview.Listener.Itemclick;
using Com.Evrencoskun.Tableview.Listener.Scroll;
using Com.Evrencoskun.Tableview.Preference;
using Com.Evrencoskun.Tableview.Sort;
using Com.Evrencoskun.Tableview.Util;
using TableViewSharp;

namespace Com.Evrencoskun.Tableview
{
    /// <summary>Created by evrencoskun on 11/06/2017.</summary>
    public class TableView : FrameLayout, ITableView, ITableViewListener
    {
        private static readonly string LogTag = typeof(Com.Evrencoskun.Tableview.TableView).Name;

        protected internal CellRecyclerView mCellRecyclerView;

        protected internal CellRecyclerView mColumnHeaderRecyclerView;

        protected internal CellRecyclerView mRowHeaderRecyclerView;

        protected internal AbstractTableAdapter mTableAdapter;

        private ITableViewListener mTableViewListener;

        private VerticalRecyclerViewListener mVerticalRecyclerListener;

        private HorizontalRecyclerViewListener mHorizontalRecyclerViewListener;

        private ColumnHeaderRecyclerViewItemClickListener mColumnHeaderRecyclerViewItemClickListener;

        private RowHeaderRecyclerViewItemClickListener mRowHeaderRecyclerViewItemClickListener;

        private ColumnHeaderLayoutManager mColumnHeaderLayoutManager;

        private LinearLayoutManager mRowHeaderLayoutManager;

        private CellLayoutManager mCellLayoutManager;

        private DividerItemDecoration mVerticalItemDecoration;

        private DividerItemDecoration mHorizontalItemDecoration;

        private SelectionHandler mSelectionHandler;

        private ColumnSortHandler mColumnSortHandler;

        private VisibilityHandler mVisibilityHandler;

        private ScrollHandler mScrollHandler;

        private FilterHandler mFilterHandler;

        private PreferencesHandler mPreferencesHandler;

        private ColumnWidthHandler mColumnWidthHandler;

        private int mRowHeaderWidth;

        private int mColumnHeaderHeight;

        private int mSelectedColor;

        private int mUnSelectedColor;

        private int mShadowColor;

        private int mSeparatorColor = -1;

        private bool mHasFixedWidth;

        private bool mIgnoreSelectionColors;

        private bool mShowHorizontalSeparators = true;

        private bool mShowVerticalSeparators = true;

        private bool mIsSortable;

        public TableView(Context context) : base(context)
        {
            InitialDefaultValues(null);
            Initialize();
        }

        public TableView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            InitialDefaultValues(attrs);
            Initialize();
        }

        public TableView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            InitialDefaultValues(null);
            Initialize();
        }

        private void InitialDefaultValues(IAttributeSet attrs)
        {
            // Dimensions
            mRowHeaderWidth = (int) Resources.GetDimension(Resource.Dimension.default_row_header_width);
            mColumnHeaderHeight = (int) Resources.GetDimension(Resource.Dimension.default_column_header_height);
            // Colors
            mSelectedColor =
                ContextCompat.GetColor(Context, Resource.Color.table_view_default_selected_background_color);
            mUnSelectedColor =
                ContextCompat.GetColor(Context, Resource.Color.table_view_default_unselected_background_color);
            mShadowColor = ContextCompat.GetColor(Context, Resource.Color.table_view_default_shadow_background_color);
            if (attrs == null)
            {
                // That means TableView is created programmatically.
                return;
            }

            // Get values from xml attributes
            TypedArray a = Context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.TableView, 0, 0);
            try
            {
                // Dimensions
                mRowHeaderWidth = (int) a.GetDimension(Resource.Styleable.TableView_row_header_width, mRowHeaderWidth);
                mColumnHeaderHeight = (int) a.GetDimension(Resource.Styleable.TableView_column_header_height,
                    mColumnHeaderHeight);
                // Colors
                mSelectedColor = a.GetColor(Resource.Styleable.TableView_selected_color, mSelectedColor);
                mUnSelectedColor = a.GetColor(Resource.Styleable.TableView_unselected_color, mUnSelectedColor);
                mShadowColor = a.GetColor(Resource.Styleable.TableView_shadow_color, mShadowColor);
                mSeparatorColor = a.GetColor(Resource.Styleable.TableView_separator_color,
                    ContextCompat.GetColor(Context, Resource.Color.table_view_default_separator_color));
                // Booleans
                mShowVerticalSeparators = a.GetBoolean(Resource.Styleable.TableView_show_vertical_separator,
                    mShowVerticalSeparators);
                mShowHorizontalSeparators = a.GetBoolean(Resource.Styleable.TableView_show_horizontal_separator,
                    mShowHorizontalSeparators);
            }
            finally
            {
                a.Recycle();
            }
        }

        private void Initialize()
        {
            // Create Views
            mColumnHeaderRecyclerView = CreateColumnHeaderRecyclerView();
            mRowHeaderRecyclerView = CreateRowHeaderRecyclerView();
            mCellRecyclerView = CreateCellRecyclerView();
            // Add Views
            AddView(mColumnHeaderRecyclerView);
            AddView(mRowHeaderRecyclerView);
            AddView(mCellRecyclerView);
            // Create Handlers
            mSelectionHandler = new SelectionHandler(this);
            mVisibilityHandler = new VisibilityHandler(this);
            mScrollHandler = new ScrollHandler(this);
            mPreferencesHandler = new PreferencesHandler(this);
            mColumnWidthHandler = new ColumnWidthHandler(this);
            InitializeListeners();
        }

        protected internal virtual void InitializeListeners()
        {
            // --- Listeners to help Scroll synchronously ---
            // It handles Vertical scroll listener
            mVerticalRecyclerListener = new VerticalRecyclerViewListener(this);
            // Set this listener both of Cell RecyclerView and RowHeader RecyclerView
            mRowHeaderRecyclerView.AddOnItemTouchListener(mVerticalRecyclerListener);
            mCellRecyclerView.AddOnItemTouchListener(mVerticalRecyclerListener);
            // It handles Horizontal scroll listener
            mHorizontalRecyclerViewListener = new HorizontalRecyclerViewListener(this);
            // Set scroll listener to be able to scroll all rows synchrony.
            mColumnHeaderRecyclerView.AddOnItemTouchListener(mHorizontalRecyclerViewListener);
            // --- Listeners to help item clicks ---
            // Create item click listeners
            mColumnHeaderRecyclerViewItemClickListener =
                new ColumnHeaderRecyclerViewItemClickListener(mColumnHeaderRecyclerView, this);
            mRowHeaderRecyclerViewItemClickListener =
                new RowHeaderRecyclerViewItemClickListener(mRowHeaderRecyclerView, this);
            // Add item click listeners for both column header & row header recyclerView
            mColumnHeaderRecyclerView.AddOnItemTouchListener(mColumnHeaderRecyclerViewItemClickListener);
            mRowHeaderRecyclerView.AddOnItemTouchListener(mRowHeaderRecyclerViewItemClickListener);
            // Add Layout change listener both of Column Header  & Cell recyclerView to detect
            // changing size
            // For some case, it is pretty necessary.
            TableViewLayoutChangeListener layoutChangeListener = new TableViewLayoutChangeListener(this);
            mColumnHeaderRecyclerView.AddOnLayoutChangeListener(layoutChangeListener);
            mCellRecyclerView.AddOnLayoutChangeListener(layoutChangeListener);

            SetTableViewListener(this);
        }

        protected internal virtual CellRecyclerView CreateColumnHeaderRecyclerView()
        {
            CellRecyclerView recyclerView = new CellRecyclerView(Context);
            // Set layout manager
            recyclerView.SetLayoutManager(GetColumnHeaderLayoutManager());
            // Set layout params
            FrameLayout.LayoutParams layoutParams =
                new FrameLayout.LayoutParams(FrameLayout.LayoutParams.WrapContent, mColumnHeaderHeight);
            layoutParams.LeftMargin = mRowHeaderWidth;
            recyclerView.LayoutParameters = layoutParams;
            if (IsShowHorizontalSeparators())
            {
                // Add vertical item decoration to display column line
                recyclerView.AddItemDecoration(GetHorizontalItemDecoration());
            }

            return recyclerView;
        }

        protected internal virtual CellRecyclerView CreateRowHeaderRecyclerView()
        {
            CellRecyclerView recyclerView = new CellRecyclerView(Context);
            // Set layout manager
            recyclerView.SetLayoutManager(GetRowHeaderLayoutManager());
            // Set layout params
            FrameLayout.LayoutParams layoutParams =
                new FrameLayout.LayoutParams(mRowHeaderWidth, FrameLayout.LayoutParams.WrapContent);
            layoutParams.TopMargin = mColumnHeaderHeight;
            recyclerView.LayoutParameters = layoutParams;
            if (IsShowVerticalSeparators())
            {
                // Add vertical item decoration to display row line
                recyclerView.AddItemDecoration(GetVerticalItemDecoration());
            }

            return recyclerView;
        }

        protected internal virtual CellRecyclerView CreateCellRecyclerView()
        {
            CellRecyclerView recyclerView = new CellRecyclerView(Context);
            // Disable multitouch
            recyclerView.MotionEventSplittingEnabled = false;
            // Set layout manager
            recyclerView.SetLayoutManager(GetCellLayoutManager());
            // Set layout params
            FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(FrameLayout.LayoutParams.WrapContent,
                FrameLayout.LayoutParams.WrapContent);
            layoutParams.LeftMargin = mRowHeaderWidth;
            layoutParams.TopMargin = mColumnHeaderHeight;
            recyclerView.LayoutParameters = layoutParams;
            if (IsShowVerticalSeparators())
            {
                // Add vertical item decoration to display row line on center recycler view
                recyclerView.AddItemDecoration(GetVerticalItemDecoration());
            }

            return recyclerView;
        }

        public virtual void SetAdapter(AbstractTableAdapter tableAdapter)
        {
            if (tableAdapter != null)
            {
                this.mTableAdapter = tableAdapter;
                this.mTableAdapter.SetRowHeaderWidth(mRowHeaderWidth);
                this.mTableAdapter.SetColumnHeaderHeight(mColumnHeaderHeight);
                this.mTableAdapter.SetTableView(this);
                // set adapters
                if (mColumnHeaderRecyclerView != null)
                {
                    mColumnHeaderRecyclerView.SetAdapter(mTableAdapter.ColumnHeaderRecyclerViewAdapter);
                }

                if (mRowHeaderRecyclerView != null)
                {
                    mRowHeaderRecyclerView.SetAdapter(mTableAdapter.RowHeaderRecyclerViewAdapter);
                }

                if (mCellRecyclerView != null)
                {
                    mCellRecyclerView.SetAdapter(mTableAdapter.GetCellRecyclerViewAdapter());
                    // Create Sort Handler
                    mColumnSortHandler = new ColumnSortHandler(this);
                    // Create Filter Handler
                    mFilterHandler = new FilterHandler(this);
                }
            }
        }

        public bool FixedWidth
        {
            get => HasFixedWidth();
            set => SetHasFixedWidth(value);
        }

        public virtual bool HasFixedWidth()
        {
            return mHasFixedWidth;
        }

        public virtual void SetHasFixedWidth(bool hasFixedWidth)
        {
            this.mHasFixedWidth = hasFixedWidth;
            // RecyclerView has also the same control to provide better performance.
            mColumnHeaderRecyclerView.HasFixedSize = hasFixedWidth;
        }

        public bool IgnoreSelectionColors
        {
            get => IsIgnoreSelectionColors();
            set => SetIgnoreSelectionColors(value);
        }

        public virtual bool IsIgnoreSelectionColors()
        {
            return mIgnoreSelectionColors;
        }

        public virtual void SetIgnoreSelectionColors(bool ignoreSelectionColor)
        {
            this.mIgnoreSelectionColors = ignoreSelectionColor;
        }

        public bool ShowHorizontalSeparators
        {
            get => IsShowHorizontalSeparators();
            set => SetShowHorizontalSeparators( value);
        }


        public virtual bool IsShowHorizontalSeparators()
        {
            return mShowHorizontalSeparators;
        }

        public bool Sortable
        {
            get => IsSortable();
        }

        public virtual bool IsSortable()
        {
            return mIsSortable;
        }

        public virtual void SetShowHorizontalSeparators(bool showSeparators)
        {
            this.mShowHorizontalSeparators = showSeparators;
        }

        public virtual bool ShowVerticalSeparators
        {
            get => IsShowVerticalSeparators();
            set => SetShowVerticalSeparators(value);
        }


        public virtual bool IsShowVerticalSeparators()
        {
            return mShowVerticalSeparators;
        }

        public virtual void SetShowVerticalSeparators(bool showSeparators)
        {
            this.mShowVerticalSeparators = showSeparators;
        }

        public CellRecyclerView CellRecyclerView
        {
            get => GetCellRecyclerView();
        }

        public virtual CellRecyclerView GetCellRecyclerView()
        {
            return mCellRecyclerView;
        }

        public CellRecyclerView ColumnHeaderRecyclerView => GetColumnHeaderRecyclerView();

        public virtual CellRecyclerView GetColumnHeaderRecyclerView()
        {
            return mColumnHeaderRecyclerView;
        }

        public CellRecyclerView RowHeaderRecyclerView => GetRowHeaderRecyclerView();
        

        public virtual CellRecyclerView GetRowHeaderRecyclerView()
        {
            return mRowHeaderRecyclerView;
        }

        public ColumnHeaderLayoutManager ColumnHeaderLayoutManager => GetColumnHeaderLayoutManager();

        public virtual ColumnHeaderLayoutManager GetColumnHeaderLayoutManager()
        {
            if (mColumnHeaderLayoutManager == null)
            {
                mColumnHeaderLayoutManager = new ColumnHeaderLayoutManager(Context, this);
            }

            return mColumnHeaderLayoutManager;
        }

        public CellLayoutManager CellLayoutManager => GetCellLayoutManager();

        public virtual CellLayoutManager GetCellLayoutManager()
        {
            if (mCellLayoutManager == null)
            {
                mCellLayoutManager = new CellLayoutManager(Context, this);
            }

            return mCellLayoutManager;
        }

        public LinearLayoutManager RowHeaderLayoutManager => GetRowHeaderLayoutManager();

        public virtual LinearLayoutManager GetRowHeaderLayoutManager()
        {
            if (mRowHeaderLayoutManager == null)
            {
                mRowHeaderLayoutManager = new LinearLayoutManager(Context, LinearLayoutManager.Vertical, false);
            }

            return mRowHeaderLayoutManager;
        }

        public HorizontalRecyclerViewListener HorizontalRecyclerViewListener => GetHorizontalRecyclerViewListener();

        public virtual HorizontalRecyclerViewListener GetHorizontalRecyclerViewListener()
        {
            return mHorizontalRecyclerViewListener;
        }

        public virtual VerticalRecyclerViewListener VerticalRecyclerViewListener =>  GetVerticalRecyclerViewListener();

        public virtual VerticalRecyclerViewListener GetVerticalRecyclerViewListener()
        {
            return mVerticalRecyclerListener;
        }

        public virtual ITableViewListener TableViewListener
        {
            get => GetTableViewListener();
            set => SetTableViewListener(value);
        }

        public virtual ITableViewListener GetTableViewListener()
        {
            return mTableViewListener;
        }

        public virtual void SetTableViewListener(ITableViewListener tableViewListener)
        {
            this.mTableViewListener = tableViewListener;
        }

        public virtual void SortColumn(int columnPosition, SortState sortState)
        {
            mIsSortable = true;
            mColumnSortHandler.Sort(columnPosition, sortState);
        }

        public virtual void SortRowHeader(SortState sortState)
        {
            mIsSortable = true;
            mColumnSortHandler.SortByRowHeader(sortState);
        }

        public virtual void RemeasureColumnWidth(int column)
        {
            // Remove calculated width value to be ready for recalculation.
            GetColumnHeaderLayoutManager().RemoveCachedWidth(column);
            // Recalculate of the width values of the columns
            GetCellLayoutManager().FitWidthSize(column, false);
        }

        
        public AbstractTableAdapter Adapter
        {
            get => GetAdapter();
            set => SetAdapter(value);
        }

        public virtual AbstractTableAdapter GetAdapter()
        {
            return mTableAdapter;
        }

        public virtual void Filter(Com.Evrencoskun.Tableview.Filter.Filter filter)
        {
            mFilterHandler.Filter(filter);
        }

        public FilterHandler FilterHandler => GetFilterHandler();

        public virtual FilterHandler GetFilterHandler()
        {
            return mFilterHandler;
        }

        public virtual SortState GetSortingStatus(int column)
        {
            return mColumnSortHandler.GetSortingStatus(column);
        }

        public virtual SortState RowHeaderSortingStatus => GetRowHeaderSortingStatus();


        public virtual SortState GetRowHeaderSortingStatus()
        {
            return mColumnSortHandler.GetRowHeaderSortingStatus();
        }

        public virtual void ScrollToColumnPosition(int column)
        {
            mScrollHandler.ScrollToColumnPosition(column);
        }

        public virtual void ScrollToColumnPosition(int column, int offset)
        {
            mScrollHandler.ScrollToColumnPosition(column, offset);
        }

        public virtual void ScrollToRowPosition(int row)
        {
            mScrollHandler.ScrollToRowPosition(row);
        }

        public virtual void ScrollToRowPosition(int row, int offset)
        {
            mScrollHandler.ScrollToRowPosition(row, offset);
        }

        public virtual ScrollHandler ScrollHandler => GetScrollHandler();
        
        public virtual ScrollHandler GetScrollHandler()
        {
            return mScrollHandler;
        }

        public virtual void ShowRow(int row)
        {
            mVisibilityHandler.ShowRow(row);
        }

        public virtual void HideRow(int row)
        {
            mVisibilityHandler.HideRow(row);
        }

        public virtual void ShowAllHiddenRows()
        {
            mVisibilityHandler.ShowAllHiddenRows();
        }

        public virtual void ClearHiddenRowList()
        {
            mVisibilityHandler.ClearHideRowList();
        }

        public virtual void ShowColumn(int column)
        {
            mVisibilityHandler.ShowColumn(column);
        }

        public virtual void HideColumn(int column)
        {
            mVisibilityHandler.HideColumn(column);
        }

        public virtual bool IsColumnVisible(int column)
        {
            return mVisibilityHandler.IsColumnVisible(column);
        }

        public virtual void ShowAllHiddenColumns()
        {
            mVisibilityHandler.ShowAllHiddenColumns();
        }

        public virtual void ClearHiddenColumnList()
        {
            mVisibilityHandler.ClearHideColumnList();
        }

        public virtual bool IsRowVisible(int row)
        {
            return mVisibilityHandler.IsRowVisible(row);
        }

        public int SelectedRow
        {
            get => GetSelectedRow();
            set => SetSelectedRow(value);
        }

        /// <summary>Returns the index of the selected row, -1 if no row is selected.</summary>
        public virtual int GetSelectedRow()
        {
            return mSelectionHandler.GetSelectedRowPosition();
        }

        public virtual void SetSelectedRow(int row)
        {
            // Find the row header view holder which is located on row position.
            AbstractViewHolder rowViewHolder =
                (AbstractViewHolder) GetRowHeaderRecyclerView().FindViewHolderForAdapterPosition(row);
            mSelectionHandler.SetSelectedRowPosition(rowViewHolder, row);
        }

        /// <summary>Returns the index of the selected column, -1 if no column is selected.</summary>
        public int SelectedColumn
        {
            get => GetSelectedColumn();
            set => SetSelectedColumn(value);
        }
        public virtual int GetSelectedColumn()
        {
            return mSelectionHandler.GetSelectedColumnPosition();
        }

        public virtual void SetSelectedColumn(int column)
        {
            // Find the column view holder which is located on column position .
            AbstractViewHolder columnViewHolder =
                (AbstractViewHolder) GetColumnHeaderRecyclerView().FindViewHolderForAdapterPosition(column);
            mSelectionHandler.SetSelectedColumnPosition(columnViewHolder, column);
        }

        public virtual void SetSelectedCell(int column, int row)
        {
            // Find the cell view holder which is located on x,y (column,row) position.
            AbstractViewHolder cellViewHolder = GetCellLayoutManager().GetCellViewHolder(column, row);
            mSelectionHandler.SetSelectedCellPositions(cellViewHolder, column, row);
        }

        public SelectionHandler SelectionHandler
        {
            get => GetSelectionHandler();
        }
        public virtual SelectionHandler GetSelectionHandler()
        {
            return mSelectionHandler;
        }

        public ColumnSortHandler ColumnSortHandler => GetColumnSortHandler();
        public ColumnSortHandler GetColumnSortHandler()
        {
            return mColumnSortHandler;
        }

        public virtual DividerItemDecoration HorizontalItemDecoration => GetHorizontalItemDecoration();

        public virtual DividerItemDecoration GetHorizontalItemDecoration()
        {
            if (mHorizontalItemDecoration == null)
            {
                mHorizontalItemDecoration = CreateItemDecoration(DividerItemDecoration.Horizontal);
            }

            return mHorizontalItemDecoration;
        }

        public virtual DividerItemDecoration VerticalItemDecoration => GetVerticalItemDecoration();

        public virtual DividerItemDecoration GetVerticalItemDecoration()
        {
            if (mVerticalItemDecoration == null)
            {
                mVerticalItemDecoration = CreateItemDecoration(DividerItemDecoration.Vertical);
            }

            return mVerticalItemDecoration;
        }

        protected internal virtual DividerItemDecoration CreateItemDecoration(int orientation)
        {
            Android.Graphics.Drawables.Drawable divider =
                ContextCompat.GetDrawable(Context, Resource.Drawable.cell_line_divider);
            // That means; There is a custom separator color from user.
            if (mSeparatorColor != -1)
            {
                // Change its color
                divider.SetColorFilter(new Color(mSeparatorColor), PorterDuff.Mode.SrcAtop);
            }

            DividerItemDecoration itemDecoration = new DividerItemDecoration(Context, orientation);
            itemDecoration.SetDrawable(divider);
            return itemDecoration;
        }

        /// <summary>This method helps to change default selected color programmatically.</summary>
        /// <param name="selectedColor">It must be Color int.</param>
        public virtual void SetSelectedColor(int selectedColor)
        {
            this.mSelectedColor = selectedColor;
        }

        public int SelectedColor => GetSelectedColor(); 

        [ColorInt]
        public virtual int GetSelectedColor()
        {
            return mSelectedColor;
        }

        public int SeparatorColor
        {
            get => GetSeparatorColor();
            set => SetSeparatorColor(value);
        }

        public virtual void SetSeparatorColor(int mSeparatorColor)
        {
            this.mSeparatorColor = mSeparatorColor;
        }

        [ColorInt]
        public virtual int GetSeparatorColor()
        {
            return mSeparatorColor;
        }

        /// <summary>This method helps to change default unselected color programmatically.</summary>
        /// <param name="unSelectedColor">It must be Color int.</param>
        public int UnSelectedColor
        {
            get => GetUnSelectedColor();
            set => SetUnSelectedColor(value);
        }
        public virtual void SetUnSelectedColor(int unSelectedColor)
        {
            this.mUnSelectedColor = unSelectedColor;
        }

        [ColorInt]
        public virtual int GetUnSelectedColor()
        {
            return mUnSelectedColor;
        }

        public int ShadowColor
        {
            get => GetShadowColor();
            set => SetShadowColor(value);
        }

        public virtual void SetShadowColor(int shadowColor)
        {
            this.mShadowColor = shadowColor;
        }

        [ColorInt]
        public virtual int GetShadowColor()
        {
            return mShadowColor;
        }

        /// <summary>get row header width</summary>
        /// <returns>size in pixel</returns>
        public int RowHeaderWidth
        {
            get => GetRowHeaderWidth();
            set => SetRowHeaderWidth(value);
        }
        
        public virtual int GetRowHeaderWidth()
        {
            return mRowHeaderWidth;
        }

        /// <summary>set RowHeaderWidth</summary>
        /// <param name="rowHeaderWidth">in pixel</param>
        public virtual void SetRowHeaderWidth(int rowHeaderWidth)
        {
            this.mRowHeaderWidth = rowHeaderWidth;
            if (mRowHeaderRecyclerView != null)
            {
                // Update RowHeader layout width
                ViewGroup.LayoutParams layoutParams = mRowHeaderRecyclerView.LayoutParameters;
                layoutParams.Width = rowHeaderWidth;
                mRowHeaderRecyclerView.LayoutParameters = layoutParams;
                mRowHeaderRecyclerView.RequestLayout();
            }

            if (mColumnHeaderRecyclerView != null)
            {
                // Update ColumnHeader left margin
                FrameLayout.LayoutParams layoutParams =
                    (FrameLayout.LayoutParams) mColumnHeaderRecyclerView.LayoutParameters;
                layoutParams.LeftMargin = rowHeaderWidth;
                mColumnHeaderRecyclerView.LayoutParameters = layoutParams;
                mColumnHeaderRecyclerView.RequestLayout();
            }

            if (mCellRecyclerView != null)
            {
                // Update Cells left margin
                FrameLayout.LayoutParams layoutParams = (FrameLayout.LayoutParams) mCellRecyclerView.LayoutParameters;
                layoutParams.LeftMargin = rowHeaderWidth;
                mCellRecyclerView.LayoutParameters = layoutParams;
                mCellRecyclerView.RequestLayout();
            }

            if (GetAdapter() != null)
            {
                // update CornerView size
                GetAdapter().SetRowHeaderWidth(rowHeaderWidth);
            }
        }

        public virtual void SetColumnWidth(int columnPosition, int width)
        {
            mColumnWidthHandler.SetColumnWidth(columnPosition, width);
        }


        protected override IParcelable OnSaveInstanceState()
        {
            SavedState state = new SavedState(base.OnSaveInstanceState());
            // Save all preferences of The TableView before the configuration changed.
            state.preferences = mPreferencesHandler.SavePreferences();
            return state;
        }

        protected override void OnRestoreInstanceState(IParcelable state)
        {
            if (!(state is SavedState))
            {
                base.OnRestoreInstanceState(state);
                return;
            }

            SavedState savedState = (SavedState) state;
            base.OnRestoreInstanceState(savedState.SuperState);
            // Reload the preferences
            mPreferencesHandler.LoadPreferences(savedState.preferences);
        }

        /// <summary>
        /// ITableViewListener Implementation
        /// </summary>
        public event EventHandler<CellEventArgs> CellClicked;

        public event EventHandler<CellEventArgs> CellLongPressed;
        public event EventHandler<ColumnEventArgs> ColumnHeaderClicked;
        public event EventHandler<ColumnEventArgs> ColumnHeaderLongPressed;
        public event EventHandler<RowEventArgs> RowHeaderClicked;
        public event EventHandler<RowEventArgs> RowHeaderLongPressed;

        public class ColumnEventArgs : EventArgs
        {
            public RecyclerView.ViewHolder ColumnHeaderView { get; set; }
            public int Column { get; set; }
        }

        public class RowEventArgs : EventArgs
        {
            public RecyclerView.ViewHolder RowHeaderView { get; set; }
            public int Row { get; set; }
        }

        public class CellEventArgs : EventArgs
        {
            public RecyclerView.ViewHolder CellView { get; set; }
            public int Column { get; set; }
            public int Row { get; set; }
        }

        public void OnCellClicked(RecyclerView.ViewHolder cellView, int column, int row)
        {
            CellClicked.SafeFire(this, new CellEventArgs {CellView = cellView, Column = column, Row = row});
        }

        public void OnCellLongPressed(RecyclerView.ViewHolder cellView, int column, int row)
        {
            CellLongPressed.SafeFire(this, new CellEventArgs {CellView = cellView, Column = column, Row = row});
        }

        public void OnColumnHeaderClicked(RecyclerView.ViewHolder columnHeaderView, int column)
        {
            ColumnHeaderClicked.SafeFire(this,
                new ColumnEventArgs {ColumnHeaderView = columnHeaderView, Column = column});
        }

        public void OnColumnHeaderLongPressed(RecyclerView.ViewHolder columnHeaderView, int column)
        {
            ColumnHeaderLongPressed.SafeFire(this,
                new ColumnEventArgs {ColumnHeaderView = columnHeaderView, Column = column});
        }

        public void OnRowHeaderClicked(RecyclerView.ViewHolder rowHeaderView, int row)
        {
            RowHeaderClicked.SafeFire(this, new RowEventArgs {RowHeaderView = rowHeaderView, Row = row});
        }

        public void OnRowHeaderLongPressed(RecyclerView.ViewHolder rowHeaderView, int row)
        {
            RowHeaderLongPressed.SafeFire(this, new RowEventArgs {RowHeaderView = rowHeaderView, Row = row});
        }
    }
}
