using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            LinkedList<int> a = new LinkedList<int>();
            a.AddFirst(1);
            var first = a.First;
            for (int i = 2; i < 1000000; i++)
            {
                a.AddAfter(first, i);
                first = first.Next;
            }
            foreach (var item in a)
            {
                Console.WriteLine(item);
            }

        }
    }
}
