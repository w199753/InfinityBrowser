﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using Infinity.Collections.LowLevel;

namespace Infinity.Collections
{
    [Serializable]
    public class TArray<T>
    {
        public int length;
        public ref T this[int index]
        {
            get
            {
                return ref m_Array[index];
            }
        }

        private T[] m_Array;

        public TArray()
        {
            length = 0;
            m_Array = new T[64];
        }

        public TArray(in int capacity = 64)
        {
            length = 0;
            m_Array = new T[capacity];
        }

        public void Clear()
        {
            length = 0;
        }

        public int Add(in T value)
        {
            if (length >= m_Array.Length)
            {
                var newArray = new T[m_Array.Length * 2];
                Array.Copy(m_Array, newArray, m_Array.Length);
                m_Array = newArray;
            }

            m_Array[length] = value;
            int cacheIndex = length;
            ++length;

            return cacheIndex;
        }

        public int AddUnique(in T value)
        {
            for(int i = 0; i < length; ++i)
            {
                if (value.Equals(m_Array[i]))
                {
                    return -1;
                }
            }
            Add(value);

            return 0;
        }

        public void Remove(in T value)
        {
            for (int i = 0; i < length; ++i)
            {
                if (value.Equals(m_Array[i]))
                {
                    RemoveAtIndex(i);
                    break;
                }
            }
        }
        
        public void RemoveAtIndex(in int index)
        {
            int lastIndex = length - 1;
            Array.Copy(m_Array, index + 1, m_Array, index, lastIndex - index);

            m_Array[lastIndex] = default(T);
            length--;
        }

        public void RemoveSwap(in T value)
        {
            for (int i = 0; i < length; ++i)
            {
                if (value.Equals(m_Array[i]))
                {
                    RemoveSwapAtIndex(i);
                    break;
                }
            }
        }
        
        public void RemoveSwapAtIndex(in int index)
        {
            if(index != length - 1)
            {
                m_Array[index] = m_Array[length - 1];
            }
            
            m_Array[length - 1] = default(T);
            length--;
        }

        public void Resize(in int newLength, in bool keepData = true)
        {
            if (keepData)
            {
                var newArray = new T[newLength];
                Array.Copy(m_Array, newArray, m_Array.Length);
                m_Array = newArray;
            } else {
                m_Array = new T[newLength];
            }

            length = newLength;
        }
    }

    public unsafe struct TPtrArray<T> : IDisposable where T : unmanaged
    {
        public int length;
        public ref T* this[int index]
        {
            get
            {
                return ref m_Array[index];
            }
        }

        private T** m_Array;
        private int m_Capacity;

        public TPtrArray(in int capacity = 64)
        {
            length = 0;
            m_Capacity = capacity;
            m_Array = (T**)MemoryUtility.Malloc(sizeof(T), capacity);
        }

        public void Clear()
        {
            length = 0;
        }

        public int Add(in T* ptr)
        {
            if (length >= m_Capacity) {
                m_Capacity *= 2;
                T** newArray = (T**)MemoryUtility.Malloc(sizeof(T*), m_Capacity);
                //ReadOnlySpan<T> span = new ReadOnlySpan<T>(m_Array, length);
                //span.CopyTo(new Span<T>(newArray, length));
                MemoryUtility.Free(m_Array);
                m_Array = newArray;
            }

            m_Array[length] = ptr;
            int cacheIndex = length;
            ++length;

            return cacheIndex;
        }

        public int AddUnique(in T* ptr)
        {
            for (int i = 0; i < length; ++i)
            {
                if (ptr == m_Array[i]) {
                    return -1;
                }
            }
            Add(ptr);

            return 0;
        }

        public void Remove(in T* ptr)
        {
            for (int i = 0; i < length; ++i)
            {
                if (ptr == m_Array[i]) {
                    RemoveAtIndex(i);
                    break;
                }
            }
        }

        public void RemoveAtIndex(in int index)
        {
            /*Span<T> copySpan = new Span<T>(m_Array, length);
            Span<T> dscSpan = copySpan.Slice(index, length - index);
            Span<T> srcSpan = copySpan.Slice(index + 1, length - (index + 1));
            srcSpan.CopyTo(dscSpan);*/

            m_Array[length - 1] = null;
            length--;
        }

        public void RemoveSwap(in T* ptr)
        {
            for (int i = 0; i < length; ++i)
            {
                if (ptr == m_Array[i]) {
                    RemoveSwapAtIndex(i);
                    break;
                }
            }
        }

        public void RemoveSwapAtIndex(in int index)
        {
            if (index != length - 1) {
                m_Array[index] = m_Array[length - 1];
            }

            m_Array[length - 1] = null;
            length--;
        }

        public void Resize(in int newLength, in bool keepData = true)
        {
            if (keepData)
            {
                T** newArray = (T**)MemoryUtility.Malloc(sizeof(T*), newLength);
                //ReadOnlySpan<T> span = new ReadOnlySpan<T>(m_Array, newLength);
                //span.CopyTo(new Span<T>(newArray, newLength));
                MemoryUtility.Free(m_Array);
                m_Array = newArray;
            } else {
                MemoryUtility.Free(m_Array);
                m_Array = (T**)MemoryUtility.Malloc(sizeof(T*), newLength);
            }

            length = newLength;
        }

        public void Dispose()
        {
            MemoryUtility.Free(m_Array);
            m_Array = null;
            m_Capacity = 0;
        }
    }

    internal unsafe class TValueArrayDebugger<T> where T : unmanaged
    {
        TValueArray<T> m_Target;

        public TValueArrayDebugger(TValueArray<T> target)
        {
            m_Target = target;
        }

        public int Length
        {
            get
            {
                return m_Target.length;
            }
        }

        public List<T> Array
        {
            get
            {
                var result = new List<T>();
                for (int i = 0; i < m_Target.length; ++i)
                {
                    result.Add(m_Target[i]);
                }
                return result;
            }
        }
    }

    [DebuggerTypeProxy(typeof(TValueArrayDebugger<>))]
    public unsafe struct TValueArray<T> : IDisposable where T : unmanaged
    {
        public int length;
        public T* NativePtr
        {
            get
            {
                return m_Array;
            }
        }
        public ref T this[int index]
        {
            get
            {
                return ref m_Array[index];
            }
        }

        private T* m_Array;
        private int m_Capacity;

        public TValueArray(in int capacity = 64)
        {
            length = 0;
            m_Capacity = capacity;
            m_Array = (T*)MemoryUtility.Malloc(sizeof(T), capacity);
        }

        internal void Init(in int capacity = 64)
        {
            length = 0;
            m_Capacity = capacity;
            m_Array = (T*)MemoryUtility.Malloc(sizeof(T), capacity);
        }

        public void Clear()
        {
            length = 0;
        }

        public int Add(in T value)
        {
            if (length >= m_Capacity)
            {
                m_Capacity *= 2;
                T* newArray = (T*)MemoryUtility.Malloc(sizeof(T), m_Capacity);
                ReadOnlySpan<T> span = new ReadOnlySpan<T>(m_Array, length);
                span.CopyTo(new Span<T>(newArray, length));
                MemoryUtility.Free(m_Array);
                m_Array = newArray;
            }

            m_Array[length] = value;
            int cacheIndex = length;
            ++length;

            return cacheIndex;
        }

        public int AddUnique(in T value)
        {
            for (int i = 0; i < length; ++i)
            {
                if (value.Equals(m_Array[i]))
                {
                    return -1;
                }
            }
            Add(value);

            return 0;
        }

        public void Remove(in T value)
        {
            for (int i = 0; i < length; ++i)
            {
                if (value.Equals(m_Array[i]))
                {
                    RemoveAtIndex(i);
                    break;
                }
            }
        }

        public void RemoveAtIndex(in int index)
        {
            Span<T> copySpan = new Span<T>(m_Array, length);
            Span<T> dscSpan = copySpan.Slice(index, length - index);
            Span<T> srcSpan = copySpan.Slice(index + 1, length - (index + 1));
            srcSpan.CopyTo(dscSpan);

            m_Array[length - 1] = default(T);
            length--;
        }

        public void RemoveSwap(in T value)
        {
            for (int i = 0; i < length; ++i)
            {
                if (value.Equals(m_Array[i]))
                {
                    RemoveSwapAtIndex(i);
                    break;
                }
            }
        }

        public void RemoveSwapAtIndex(in int index)
        {
            if (index != length - 1)
            {
                m_Array[index] = m_Array[length - 1];
            }

            m_Array[length - 1] = default(T);
            length--;
        }

        public void Resize(in int newLength, in bool keepData = true)
        {
            if (keepData)
            {
                T* newArray = (T*)MemoryUtility.Malloc(sizeof(T), newLength);
                ReadOnlySpan<T> span = new ReadOnlySpan<T>(m_Array, newLength);
                span.CopyTo(new Span<T>(newArray, newLength));
                MemoryUtility.Free(m_Array);
                m_Array = newArray;
            } else {
                MemoryUtility.Free(m_Array);
                m_Array = (T*)MemoryUtility.Malloc(sizeof(T), newLength);
            }

            length = newLength;
        }

        public void Dispose()
        {
            MemoryUtility.Free(m_Array);
            m_Array = null;
            m_Capacity = 0;
        }
    }
}
