using System;
using System.Collections;
using System.Collections.Generic;

namespace Com.Evrencoskun.Tableview
{
    public class AList<T> : List<T>
    {
        public AList()
        {
        }

        public AList(int size) : base(size)
        {
        }

        public AList(IEnumerable<T> t)
        {
            AddRange(t);
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;
            IList list = obj as IList;
            if (list == null)
                return false;
            if (list.Count != Count)
                return false;
            for (int n = 0; n < list.Count; n++)
            {
                if (!object.Equals(this[n], list[n]))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int n = 0;
            foreach (object o in this)
                if (o != null)
                    n += o.GetHashCode();
            return n;
        }

        public void RemoveElement(T elem)
        {
            Remove(elem);
        }

        public void TrimToSize()
        {
            Capacity = Count;
        }

        public void EnsureCapacity(int c)
        {
            if (c > Capacity && c > Count)
                Capacity = c;
        }
    }
}
