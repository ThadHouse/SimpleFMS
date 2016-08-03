using System;
using System.Collections.Generic;

namespace SimpleFMS.Base.Tuples
{
    public struct ValueTuple<T1, T2> : IEquatable<ValueTuple<T1, T2>>
    {
        public bool Equals(ValueTuple<T1, T2> other)
        {
            return EqualityComparer<T1>.Default.Equals(First, other.First) && EqualityComparer<T2>.Default.Equals(Second, other.Second);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ValueTuple<T1, T2> && Equals((ValueTuple<T1, T2>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T1>.Default.GetHashCode(First) * 397) ^ EqualityComparer<T2>.Default.GetHashCode(Second);
            }
        }

        public static bool operator ==(ValueTuple<T1, T2> left, ValueTuple<T1, T2> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ValueTuple<T1, T2> left, ValueTuple<T1, T2> right)
        {
            return !left.Equals(right);
        }

        public bool IsDefault()
        {
            return Equals(new ValueTuple<T1, T2>());
        }

        public T1 First { get; }
        public T2 Second { get; }

        public ValueTuple(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }
    }
}
