using System.Collections;
using System.Collections.Generic;
using Com.Evrencoskun.Tableview.Adapter;

namespace Com.Evrencoskun.Tableview.Sort
{
    /// <summary>Created by cedricferry on 14/2/18.</summary>
    public class RowHeaderForCellSortComparator : IComparer, IComparer<IList<ICell>>
    {
        private IList<IRowHeader> mReferenceList;

        private IList<IList<ICell>> mColumnList;

        private SortState mRortState;

        private RowHeaderSortComparator mRowHeaderSortComparator;

        public RowHeaderForCellSortComparator(IList<IRowHeader> referenceList, IList<IList<ICell>> columnList,
            SortState sortState)
        {
            this.mReferenceList = referenceList;
            this.mColumnList = columnList;
            this.mRortState = sortState;
            this.mRowHeaderSortComparator = new RowHeaderSortComparator(sortState);
        }

        public virtual int Compare(object o, object t1)
        {
            object o1 = ((ISortableModel) mReferenceList[this.mColumnList.IndexOf((IList<ICell>) o)]).GetContent();
            object o2 = ((ISortableModel) mReferenceList[this.mColumnList.IndexOf((IList<ICell>) t1)]).GetContent();
            if (mRortState == SortState.Descending)
            {
                return mRowHeaderSortComparator.CompareContent(o2, o1);
            }
            else
            {
                return mRowHeaderSortComparator.CompareContent(o1, o2);
            }
        }

        public int Compare(IList<ICell> o, IList<ICell> t1)
        {
            object o1 = ((ISortableModel) mReferenceList[this.mColumnList.IndexOf(o)]).GetContent();
            object o2 = ((ISortableModel) mReferenceList[this.mColumnList.IndexOf(t1)]).GetContent();
            if (mRortState == SortState.Descending)
            {
                return mRowHeaderSortComparator.CompareContent(o2, o1);
            }
            else
            {
                return mRowHeaderSortComparator.CompareContent(o1, o2);
            }
        }
    }
}
