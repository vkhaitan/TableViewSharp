using System.Collections.Generic;
using Android.Support.V7.Util;
using Com.Evrencoskun.Tableview.Adapter;

namespace Com.Evrencoskun.Tableview.Sort
{
    /// <summary>Created by cedricferry on 6/2/18.</summary>
    public class RowHeaderSortCallback : DiffUtil.Callback
    {
        private static readonly string LogTag = typeof(Com.Evrencoskun.Tableview.Sort.RowHeaderSortCallback).Name;

        private IList<IRowHeader> mOldCellItems;

        private IList<IRowHeader> mNewCellItems;

        public override int NewListSize
        {
            get { return mNewCellItems.Count; }
        }

        public override int OldListSize
        {
            get { return mOldCellItems.Count; }
        }

        public RowHeaderSortCallback(IList<IRowHeader> oldCellItems, IList<IRowHeader> newCellItems)
        {
            this.mOldCellItems = oldCellItems;
            this.mNewCellItems = newCellItems;
        }


        public override bool AreItemsTheSame(int oldItemPosition, int newItemPosition)
        {
            // Control for precaution from IndexOutOfBoundsException
            if (mOldCellItems.Count > oldItemPosition && mNewCellItems.Count > newItemPosition)
            {
                // Compare ids
                string oldId = ((ISortableModel) mOldCellItems[oldItemPosition]).GetId();
                string newId = ((ISortableModel) mNewCellItems[newItemPosition]).GetId();
                return oldId.Equals(newId);
            }

            return false;
        }

        public override bool AreContentsTheSame(int oldItemPosition, int newItemPosition)
        {
            // Control for precaution from IndexOutOfBoundsException
            if (mOldCellItems.Count > oldItemPosition && mNewCellItems.Count > newItemPosition)
            {
                // Compare contents
                object oldContent = ((ISortableModel) mOldCellItems[oldItemPosition]).GetContent();
                object newContent = ((ISortableModel) mNewCellItems[newItemPosition]).GetContent();
                return oldContent.Equals(newContent);
            }

            return false;
        }
    }
}
