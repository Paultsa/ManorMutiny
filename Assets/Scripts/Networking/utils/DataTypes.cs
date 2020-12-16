using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTypeUtils
{
    // Since we dont have completely modifyable pairs in c# (this is like c++ std::pair)
    public class Pair<A, B>
    {

        public A first;
        public B second;

        public Pair(A a, B b)
        {
            first = a;
            second = b;
        }

        /*public static bool operator ==(Pair<A, B> left, Pair<A, B> right)
        {
            dynamic a1 = left.first;
            dynamic b1 = left.second;
            dynamic a2 = right.first;
            dynamic b2 = right.second;

            return a1 == a2 && b1 == b2;
        }

        public static bool operator !=(Pair<A, B> left, Pair<A, B> right)
        {
            return !(left == right);
        }*/
    }
}
