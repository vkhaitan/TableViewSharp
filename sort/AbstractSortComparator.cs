using System;


namespace Com.Evrencoskun.Tableview.Sort
{
    /// <summary>Created by cedricferry on 6/2/18.</summary>
    public abstract class AbstractSortComparator
    {
        protected internal SortState mSortState;

        protected internal virtual int CompareContent(object o1, object o2)
        {
            if (o1 == null && o2 == null)
            {
                return 0;
            }
            else if (o1 == null)
            {
                return -1;
            }
            else if (o2 == null)
            {
                return 1;
            }
            else
            {
                Type type = o1.GetType();
                if (type == typeof(string))
                {
                    return string.CompareOrdinal(((string) o1), (string) o2);
                }
                else if (typeof(IComparable<bool>).IsAssignableFrom(type))
                {
                    return ((IComparable<bool>) o1).CompareTo((bool) o2);
                }
                else if (typeof(IComparable<int>).IsAssignableFrom(type))
                {
                    return ((IComparable<int>) o1).CompareTo((int) o2);
                }
                else if (typeof(IComparable<float>).IsAssignableFrom(type))
                {
                    return ((IComparable<float>) o1).CompareTo((float) o2);
                }
                else if (typeof(IComparable<double>).IsAssignableFrom(type))
                {
                    return ((IComparable<double>) o1).CompareTo((double) o2);
                }
                else if (typeof(IComparable<DateTime>).IsAssignableFrom(type))
                {
                    return ((IComparable<DateTime>) o1).CompareTo((DateTime) o2);
                }
                else if (typeof(IComparable).IsAssignableFrom(type))
                {
                    return ((IComparable) o1).CompareTo(o2);
                }
                else
                    return -1;
            }
        }
    }
}
