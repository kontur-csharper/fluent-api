using System;
using System.Collections;
using System.Collections.Generic;

namespace PrintingConfigTests.TestingModels
{
    public class TestingCollection<T> : ICollection<T>, ICollection
    {
        private readonly List<T> list = new List<T>();
        public IEnumerator<T> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item) => list.Add(item);
        public void Clear() => list.Clear();
        public bool Contains(T item) => list.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);
        public bool Remove(T item) => list.Remove(item);

        public void CopyTo(Array array, int index) => ((ICollection) list).CopyTo(array, index);
        public int Count => list.Count;
        public bool IsSynchronized => ((ICollection) list).IsSynchronized;
        public object SyncRoot => ((ICollection) list).SyncRoot;
        public bool IsReadOnly => ((ICollection<T>) list).IsReadOnly;
        public T this[int index] => list[index];
    }
}