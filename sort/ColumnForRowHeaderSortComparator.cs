using System.Collections;
using System.Collections.Generic;
using Com.Evrencoskun.Tableview.Adapter;

namespace Com.Evrencoskun.Tableview.Sort
{
    /// <summary>
    /// In order to keep RowHeader DataSet and Main DataSet aligned
    /// it is required to sort RowHeader the same.
    /// </summary>
    /// <remarks>
    /// In order to keep RowHeader DataSet and Main DataSet aligned
    /// it is required to sort RowHeader the same.
    /// So if MainDataSet row 1 moved to position 10, RowHeader 1 move to position 10 too.
    /// To accomplish that we need to set a comparator that use MainDataSet
    /// in order to sort RowHeader.
    /// </remarks>
    public class ColumnForRowHeaderSortComparator : IComparer<IRowHeader>, IComparer
    {
        private IList<IRowHeader> mRowHeaderList;

        private IList<IList<ICell>> mReferenceList;

        private int column;

        private SortState mRortState;

        private ColumnSortComparator mColumnSortComparator;

        public ColumnForRowHeaderSortComparator(IList<IRowHeader> rowHeader, IList<IList<ICell>> referenceList,
            int column, SortState sortState)
        {
            this.mRowHeaderList = rowHeader;
            this.mReferenceList = referenceList;
            this.column = column;
            this.mRortState = sortState;
            this.mColumnSortComparator = new ColumnSortComparator(column, sortState);
        }

        public virtual int Compare(object o, object t1)
        {
            return Compare((ISortableModel) o, (ISortableModel) t1);
        }

        public int Compare(IRowHeader o, IRowHeader t1)
        {
            object o1 = ((ISortableModel) mReferenceList[this.mRowHeaderList.IndexOf(o)][column]).GetContent();
            object o2 = ((ISortableModel) mReferenceList[this.mRowHeaderList.IndexOf(t1)][column]).GetContent();
            if (mRortState == SortState.Descending)
            {
                return mColumnSortComparator.CompareContent(o2, o1);
            }
            else
            {
                return mColumnSortComparator.CompareContent(o1, o2);
            }
        }
    }
}
