using System.Collections.Generic;
using Com.Evrencoskun.Tableview.Adapter;

namespace Com.Evrencoskun.Tableview.Sort
{
    /// <summary>Created by cedricferry on 6/2/18.</summary>
    public class RowHeaderSortComparator : AbstractSortComparator, IComparer<IRowHeader>
    {
        private static readonly string LogTag = typeof(Com.Evrencoskun.Tableview.Sort.RowHeaderSortComparator).Name;

        public RowHeaderSortComparator(SortState sortState)
        {
            this.mSortState = sortState;
        }

        public virtual int Compare(IRowHeader o1, IRowHeader o2)
        {
            if (mSortState == SortState.Descending)
            {
                return CompareContent(((ISortableModel) o2).GetContent(), ((ISortableModel) o1).GetContent());
            }
            else
            {
                return CompareContent(((ISortableModel) o1).GetContent(), ((ISortableModel) o2).GetContent());
            }
        }
    }
}
