using System.Collections;

namespace Glyphy.Misc
{
    public class LazySortedList<T> : IList<T>
    {
        public bool IsSorted => _isSorted;
        public IReadOnlyList<T> Raw => _innerList;

        int ICollection<T>.Count => _innerList.Count;

        bool ICollection<T>.IsReadOnly => false;

        private readonly IComparer<T> _comparer;
        private readonly List<T> _innerList = new();
        private bool _isSorted = true;

        public LazySortedList() : this(Comparer<T>.Default) { }

        public LazySortedList(IComparer<T> comparer)
        {
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        int IList<T>.IndexOf(T item)
        {
            EnsureSorted();
            return _innerList.IndexOf(item);
        }

        void IList<T>.Insert(int index, T item)
        {
            //If the list is already sorted check if the new item satisfies the sort operation for the position it is being inserted at.

            bool isStillSorted = IsItemSortedAtIndex(index, item);

            _innerList.Insert(index, item);

            _isSorted = isStillSorted;
        }

        void IList<T>.RemoveAt(int index) => _innerList.RemoveAt(index);

        void ICollection<T>.Add(T item)
        {
            bool isStillSorted = IsItemSortedAtIndex(_innerList.Count, item);

            _innerList.Add(item);

            _isSorted = isStillSorted;
        }

        void ICollection<T>.Clear()
        {
            _innerList.Clear();
            _isSorted = true;
        }

        bool ICollection<T>.Contains(T item) => _innerList.Contains(item);

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            EnsureSorted();
            _innerList.CopyTo(array, arrayIndex);
        }

        bool ICollection<T>.Remove(T item) => _innerList.Remove(item);

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            EnsureSorted();
            return _innerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();

        T IList<T>.this[int index]
        {
            get
            {
                EnsureSorted();
                return _innerList[index];
            }
            set
            {
                _innerList[index] = value;
                _isSorted = false;
            }
        }

        private void EnsureSorted()
        {
            if (!_isSorted)
            {
                _innerList.Sort(_comparer);
                _isSorted = true;
            }
        }

        private bool IsItemSortedAtIndex(int index, T item)
        {
            return _isSorted
                && (index == 0 || _comparer.Compare(_innerList[index - 1], item) <= 0)
                && (index == _innerList.Count || _comparer.Compare(item, _innerList[index]) <= 0);
        }

        public void AddRange(IEnumerable<T> collection)
        {
            _innerList.AddRange(collection);
            _isSorted = false;
        }

        public void Sort() => EnsureSorted();
    }
}
