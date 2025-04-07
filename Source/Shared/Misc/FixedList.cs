using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Misc 
{
    public class FixedList<T> : IEnumerable<T>
    {
        private readonly int maximumSize;
        private readonly List<T> content;

        public FixedList(int maximumSize) 
        {
            this.maximumSize = maximumSize;
            content = new List<T>();
        }

        public void Add(T item)
        {
            content.Add(item);
            CheckCount();
        }

        public bool Remove(T item)
        {
            return content.Remove(item);
        }

        private void CheckCount() 
        {
            if (content.Count > maximumSize)
            {
                content.Remove(content[0]);
            }
        }

        IEnumerator<T> GetEnumerator() => content.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => content.GetEnumerator();
    }
}