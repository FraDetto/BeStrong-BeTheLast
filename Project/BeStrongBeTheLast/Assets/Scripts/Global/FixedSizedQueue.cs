/*
MIT License
Copyright (c) 2019: Francesco Dettori, Jacopo Frasson, Riccardo Lombardi, Michele Maione
Author: Michele Maione
Contributors:
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Collections.Concurrent;

class FixedSizedQueue<T>
{

    private ConcurrentQueue<T> Q = new ConcurrentQueue<T>();

    private readonly object LockObject = new object();
    private readonly int Limit;


    public int Count =>
        Q.Count;


    public FixedSizedQueue(int limit) =>
        Limit = limit;

    public void Enqueue(T obj)
    {
        if (!Contains(obj))
        {
            Q.Enqueue(obj);

            lock (LockObject)
                while (Q.Count > Limit && Q.TryDequeue(out T overflow)) ;
        }
    }

    public T Peek()
    {
        if (Q.Count == 0)
            return default;

        Q.TryPeek(out T t);

        return t;
    }

    private bool Contains(T obj)
    {
        foreach (var x in Q)
            if (x.Equals(obj))
                return true;

        return false;
    }

}