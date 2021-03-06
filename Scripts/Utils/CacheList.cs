﻿using System;
using System.Collections;

namespace M8 {
    /// <summary>
    /// Simple caching to minimize allocation, when order of items does not matter.
    /// </summary>
    public class CacheList<T> : IEnumerable {
        private T[] mItems;
        private int mCount;

        public T this[int ind] {
            get {
                if(ind >= mCount)
                    throw new IndexOutOfRangeException();

                return mItems[ind];
            }
        }

        public int Count { get { return mCount; } }

        public int Capacity { get { return mItems.Length; } }

        public bool IsFull { get { return Count - Capacity >= 0; } }

        public CacheList(int capacity) {
            mCount = 0;
            mItems = new T[capacity];
        }

        public IEnumerator GetEnumerator() {
            for(int i = 0; i < mCount; i++)
                yield return mItems[i];
        }

        /// <summary>
        /// Double the amount of capacity
        /// </summary>
        public void Expand() {
            System.Array.Resize(ref mItems, mItems.Length*2);
        }

        /// <summary>
        /// Increase capacity by given amount.
        /// </summary>
        public void Expand(int amount) {
            if(amount <= 0)
                throw new ArgumentException(string.Format("Invalid amount ({0})"+amount));

            System.Array.Resize(ref mItems, mItems.Length + amount);
        }

        public void Add(T item) {
            if(IsFull)
                throw new InvalidOperationException(string.Format("Maximum capacity of {0} reached.", mItems.Length));

            mItems[mCount] = item;
            mCount++;
        }

        /// <summary>
        /// Remove and return the first available item.
        /// </summary>
        public T Remove() {
            if(mCount > 0) {
                T ret = mItems[0];

                mItems[0] = mItems[mCount - 1];
                mCount--;
                mItems[mCount] = default(T);

                return ret;
            }

            return default(T);
        }

        public bool Remove(T item) {
            for(int i = 0; i < mCount; i++) {
                if(mItems[i].Equals(item)) {
                    //move last item to this index, decrement count, then 'nullify'
                    mItems[i] = mItems[mCount - 1];
                    mCount--;
                    mItems[mCount] = default(T);
                    return true;
                }
            }

            return false;
        }

        public int RemoveAll(Predicate<T> match) {
            int c = 0;
            for(int i = 0; i < mCount; i++) {
                if(match(mItems[i])) {
                    mItems[i] = mItems[mCount - 1];
                    mCount--;
                    mItems[mCount] = default(T);

                    i--;
                    c++;
                }
            }
            return c;
        }

        public void Clear() {
            for(int i = 0; i < mCount; i++)
                mItems[i] = default(T);

            mCount = 0;
        }

        public bool Exists(T item) {
            for(int i = 0; i < mCount; i++) {
                if(mItems[i].Equals(item))
                    return true;
            }

            return false;
        }
    }
}