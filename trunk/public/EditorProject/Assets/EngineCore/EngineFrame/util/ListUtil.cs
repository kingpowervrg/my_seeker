using System;
using System.Collections.Generic;
using System.IO;

namespace GOEngine.Implement
{
    static class ListUtil
    {
        public static void Sort<T>(this List<T> list, Func<T, T, int> compare)
        {
            if (list.Count > 0)
            {
                int pivot = list.Count / 2;
                List<T> left = new List<T>();
                List<T> right = new List<T>();
                T pivotVal = list[pivot];

                for (int i = 0; i < list.Count; i++)
                {
                    if (i == pivot)
                        continue;
                    if (compare(list[i], pivotVal) > 0)
                        right.Add(list[i]);
                    else
                        left.Add(list[i]);
                }
                left.Sort(compare);
                right.Sort(compare);

                for (int i = 0; i < pivot; i++)
                {
                    list[i] = left[i];
                }
                list[pivot] = pivotVal;
                for (int i = 0; i < right.Count; i++)
                {
                    list[pivot + i + 1] = right[i];
                }
            }
        }

        public static void Sort<T>(this LinkedList<T> lnk, Func<T, T, int> compare)
        {
            LinkedListNode<T> cNode;
            LinkedListNode<T> pNode;
            LinkedListNode<T> tNode;
            cNode = lnk.First;
            int result;
            bool IsSwitch;
            while (cNode != lnk.Last)
            {
                tNode = cNode;
                pNode = cNode;
                IsSwitch = false;
                do
                {
                    pNode = pNode.Next;
                    result = compare(tNode.Value, pNode.Value);
                    if (result > 0)
                    {
                        tNode = pNode;
                        IsSwitch = true;
                    }
                } while (pNode != lnk.Last);
                if (IsSwitch)
                {
                    lnk.Remove(tNode);
                    lnk.AddBefore(cNode, tNode);
                }
                cNode = tNode.Next;
            }
        }
    }
}
