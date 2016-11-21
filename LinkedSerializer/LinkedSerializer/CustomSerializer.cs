using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace LinkedSerializer
{
    [DebuggerDisplay("{Data}")] //Debug purposes
    internal class ListNode

    {
        public ListNode Prev;

        public ListNode Next;

        public ListNode Rand; // произвольный элемент внутри списка

        public string Data;

    }

    //In case of search in dictionary causes O(N) 
    //Serialization and deserialization causes O(N)
    //Encoding.Default could be potentially risky, but it depends on env
    //Added lock for consistent with multithreading calls
    internal class ListRand
    {
        public ListNode Head;

        public ListNode Tail;

        public int Count;

        private readonly object _lock = new object();

        //4 bytes
        private const int IntSize = 4;
        
        //offset,next,prev,rand
        private const int NumberOfIntFields = 4;

        //Format in file will be (int,[int,int,int,int,string]*n) - (count,(offset,next,prev,rand)*n)
        public void Serialize(FileStream s)
        {
            //WriteCount
            s.WriteInt(Count);
            var items = new Dictionary<ListNode, int>();
            
            var indexer = 0;

            //fill indexes
            //iterator
            var item = Head;
            while (item != null)
            {
                items.Add(item, indexer++);
                item = item.Next;
            }

            //fill references
            item = Head;
            var startIndex = IntSize;
            
            while (item != null)
            {
                var offset = startIndex + IntSize * NumberOfIntFields + Encoding.Default.GetByteCount(item.Data);
                s.WriteInt(offset);
                s.WriteInt(item.Next != null ? items[item.Next] : -1);
                s.WriteInt(item.Prev != null ? items[item.Prev] : -1);
                s.WriteInt(item.Rand != null ? items[item.Rand] : -1);
                s.WriteStr(item.Data);

                item = item.Next;
                startIndex = offset;
            }
        }

        public void Deserialize(FileStream s)
        {
            lock (_lock)
            {
                //count of nodes
                var count = s.ReadInt();
                if (count < 1)
                    return;

                var startIndex = IntSize;
                var items = new Dictionary<int, ListNode>();
                for (int i = 0; i < count; i++)
                    items.Add(i, new ListNode());
                //reading by node
                for (int i = 0; i < count; i++)
                {
                    var node = items[i];
                    var offset = s.ReadInt();
                    var next = s.ReadInt();
                    var prev = s.ReadInt();
                    var rand = s.ReadInt();
                    var str = s.ReadStr(offset - startIndex - IntSize * NumberOfIntFields);

                    node.Next = items.TryGetValue(next);
                    node.Prev = items.TryGetValue(prev);
                    node.Rand = items.TryGetValue(rand);
                    node.Data = str;

                    startIndex = offset;
                }
                Head = items.First().Value;
                Tail = items.Last().Value;
                Count = count;
            }
        }

        public void Add(ListNode node)
        {
            lock (_lock)
            {
                //Empty
                if (Count == 0)
                {
                    Head = node;
                    Tail = node;
                    Count++;
                    return;
                }
                //Add to tail
                node.Prev = Tail;
                Tail.Next = node;
                Tail = node;
                Count++;
            }
        }

    }

    public static class DictHelper
    {
        /// <summary>
        /// Getting value by key or returning default V
        /// </summary>
        /// <typeparam name="K">Key</typeparam>
        /// <typeparam name="V">Value</typeparam>
        /// <param name="dict">Source dict</param>
        /// <param name="key">Key object</param>
        /// <returns>Value object</returns>
        public static V TryGetValue<K, V>(this Dictionary<K, V> dict, K key)
        {
            if (dict.ContainsKey(key))
                return dict[key];
            return default(V);
        }
    }
    public static class FsHelper
    {
        /// <summary>
        /// Writes int to filestream
        /// </summary>
        /// <param name="fs">FileStream</param>
        /// <param name="value">int object</param>
        public static void WriteInt(this FileStream fs, int value)
        {
            fs.Write(BitConverter.GetBytes(value), 0, 4);
        }

        /// <summary>
        /// Writes string to filestream
        /// </summary>
        /// <param name="fs">FileStream</param>
        /// <param name="value">string object</param>
        public static void WriteStr(this FileStream fs, string value)
        {
            fs.Write(Encoding.Default.GetBytes(value), 0, Encoding.Default.GetByteCount(value));
        }

        /// <summary>
        /// Reads int from filestream
        /// </summary>
        /// <param name="fs">FileStream</param>
        /// <returns>int object</returns>
        public static int ReadInt(this FileStream fs)
        {
            var buff = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                buff[i] = (byte)fs.ReadByte();
            }
            return BitConverter.ToInt32(buff, 0);
        }

        /// <summary>
        /// Reads string from filestream
        /// </summary>
        /// <param name="fs">FileStream</param>
        /// <param name="length">lentgth of the string</param>
        /// <returns>string object</returns>
        public static string ReadStr(this FileStream fs, int length)
        {
            var buff = new byte[length];
            for (int i = 0; i < length; i++)
            {
                buff[i] = (byte)fs.ReadByte();
            }
            return Encoding.Default.GetString(buff);
        }
    }
}
