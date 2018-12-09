using System;


namespace Com.Evrencoskun.Tableview.Sort
{
    /// <summary>Created by cedricferry on 6/2/18.</summary>
    public class RowHeaderSortHelper
    {
        private static readonly string LogTag = typeof(Com.Evrencoskun.Tableview.Sort.RowHeaderSortHelper).Name;

        private SortState mSortState;

        public RowHeaderSortHelper()
        {
        }

        private void SortingStatusChanged(SortState sortState)
        {
            mSortState = sortState;
        }

        // TODO: Should we add an interface and listner and call listner when it is sorted?
        public virtual void SetSortingStatus(SortState status)
        {
            mSortState = status;
            SortingStatusChanged(status);
        }

        public virtual void ClearSortingStatus()
        {
            mSortState = SortState.Unsorted;
        }

        public bool Sorting => IsSorting();

        public virtual bool IsSorting()
        {
            return mSortState != SortState.Unsorted;
        }

        public SortState SortingStatus
        {
            get => GetSortingStatus();
            set => SetSortingStatus(value);
        }

        public virtual SortState GetSortingStatus()
        {
            return mSortState;
        }

        [System.Serializable]
        public class TableViewSorterException : Exception
        {
            public TableViewSorterException(RowHeaderSortHelper _enclosing) : base(
                "For sorting process, column header view holders must be " + "extended from " +
                "AbstractSorterViewHolder class")
            {
                this._enclosing = _enclosing;
            }

            private readonly RowHeaderSortHelper _enclosing;
        }
    }
}
