using System;
using System.Collections;
using System.Collections.Generic;

namespace Thrift.Collections
{
    public class THashSet<T> : ICollection<T>, IEnumerable<T>, IEnumerable
    {
        private HashSet<T> @set;

        public int Count
        {
            get
            {
                return this.@set.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public THashSet()
        {
        }

        public void Add(T item)
        {
            this.@set.Add(item);
        }

        public void Clear()
        {
            this.@set.Clear();
        }

        public bool Contains(T item)
        {
            return this.@set.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.@set.CopyTo(array, arrayIndex);
        }

        public IEnumerator GetEnumerator()
        {
            return this.@set.GetEnumerator();
        }

        public bool Remove(T item)
        {
            return this.@set.Remove(item);
        }

        IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>)this.@set).GetEnumerator();
        }
    }
}