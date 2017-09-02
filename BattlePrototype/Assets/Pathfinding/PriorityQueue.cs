using System.Collections;
using System.Collections.Generic;
using System;

public class PriorityQueue<T>
{
    private IComparer<T> _comparer;
    private T[] _data;
    private int _count = 0;

    public PriorityQueue()
      : this(11)
    {

    }

    public PriorityQueue(int initialCapacity, IComparer<T> comparer)
    {
        if (initialCapacity < 0)
        {
            throw new ArgumentOutOfRangeException("initialCapacity");
        }

        if (comparer == null)
        {
            comparer = Comparer<T>.Default;
        }

        _data = new T[initialCapacity];

        _comparer = comparer;
    }

    public PriorityQueue(int initialCapacity)
      : this(initialCapacity, null)
    {

    }

    private void IncreaseCapacity()
    {
        int size = (_count + 1) << 1;

        T[] data = new T[size];

        Array.Copy(_data, data, _data.Length);

        _data = data;
    }

    public void Enqueue(T item)
    {
        if (_count == _data.Length)
        {
            IncreaseCapacity();
        }

        int index = _count;

        _data[_count] = item;

        _count += 1;

        while (index > 0)
        {
            int parent = (index - 1) / 2;

            if (_comparer.Compare(_data[index], _data[parent]) >= 0)
            {
                break;
            }

            T element = _data[index];

            _data[index] = _data[parent];
            _data[parent] = element;

            index = parent;
        }
    }

    public void Clear()
    {
        if (_count > 0)
        {
            Array.Clear(this._data, 0, _count);

            _count = 0;
        }
    }

    public int Count
    {
        get
        {
            return _count;
        }
    }

    public T Dequeue()
    {
        if (_count <= 0)
        {
            throw new InvalidOperationException("Queue empty.");
        }

        _count -= 1;

        T first = _data[0];

        _data[0] = _data[_count];

        _data[_count] = default(T);

        int index = 0;

        while (true)
        {
            int left = (index << 1) + 1;

            if (left >= _count)
            {
                return first;
            }

            int right = left + 1;

            if (right < _count)
            {
                if (_comparer.Compare(_data[left], _data[right]) > 0)
                {
                    left = right;
                }
            }

            if (_comparer.Compare(_data[index], _data[left]) <= 0)
            {
                return first;
            }

            T element = _data[index];

            _data[index] = _data[left];
            _data[left] = element;

            index = left;
        }
    }
}