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
using Android.Content;
using Android.Support.V7.Widget;
using Com.Evrencoskun.Tableview.Adapter.Recyclerview.Holder;


namespace Com.Evrencoskun.Tableview.Adapter.Recyclerview
{
    /// <summary>Created by evrencoskun on 10/06/2017.</summary>
    public abstract class AbstractRecyclerViewAdapter<T> : RecyclerView.Adapter where T : class
    {
        protected internal IList<T> mItemList;

        protected internal Context mContext;

        public AbstractRecyclerViewAdapter(Context context) : this(context, null)
        {
        }

        public AbstractRecyclerViewAdapter(Context context, IList<T> itemList)
        {
            mContext = context;
            if (itemList == null)
            {
                mItemList = new AList<T>();
            }
            else
            {
                SetItems(itemList);
            }
        }
        

        public override int ItemCount => mItemList.Count;

        public IList<T> Items
        {
            get => mItemList;
            set => SetItems(value);
        } 


        public virtual IList<T> GetItems()
        {
            return mItemList;
        }

        public virtual void SetItems(IList<T> itemList)
        {
            mItemList = new AList<T>(itemList);
            this.NotifyDataSetChanged();
        }

        public virtual void SetItems(IList<T> itemList, bool notifyDataSet)
        {
            mItemList = new AList<T>(itemList);
            if (notifyDataSet)
            {
                this.NotifyDataSetChanged();
            }
        }
        

        public virtual T GetItem(int position)
        {
            if (mItemList == null || mItemList.Count == 0 || position < 0 || position >= mItemList.Count)
            {
                return null;
            }

            return mItemList[position];
        }

        public virtual void DeleteItem(int position)
        {
            if (position != RecyclerView.NoPosition)
            {
                mItemList.RemoveAt(position);
                NotifyItemRemoved(position);
            }
        }

        public virtual void DeleteItemRange(int positionStart, int itemCount)
        {
            for (int i = positionStart + itemCount - 1; i >= positionStart; i--)
            {
                if (i != RecyclerView.NoPosition)
                {
                    mItemList.RemoveAt(i);
                }
            }

            NotifyItemRangeRemoved(positionStart, itemCount);
        }

        public virtual void AddItem(int position, T item)
        {
            if (position != RecyclerView.NoPosition && item != null)
            {
                mItemList.Insert(position, item);
                NotifyItemInserted(position);
            }
        }

        public virtual void AddItemRange(int positionStart, IList<T> items)
        {
            if (items != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (i != RecyclerView.NoPosition)
                    {
                        mItemList.Insert((i + positionStart), items[i]);
                    }
                }

                NotifyItemRangeInserted(positionStart, items.Count);
            }
        }

        public virtual void ChangeItem(int position, T item)
        {
            if (position != RecyclerView.NoPosition && item != null)
            {
                mItemList[position] = item;
                NotifyItemChanged(position);
            }
        }

        public virtual void ChangeItemRange(int positionStart, IList<T> items)
        {
            if (mItemList.Count > positionStart + items.Count && items != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (i != RecyclerView.NoPosition)
                    {
                        mItemList[i + positionStart] = items[i];
                    }
                }

                NotifyItemRangeChanged(positionStart, items.Count);
            }
        }

        public override int GetItemViewType(int position)
        {
            return 1;
        }
    }
}
