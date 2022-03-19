using System;

namespace Pathfinding_Visulazier.Basic
{
    public class Vector2D<T> : IComparable where T : IComparable
    {
        public T X { get; }
        public T Y { get; }
        public Vector2D(T x, T y)
        {
            X = x;
            Y = y;
        }

        public int CompareTo(object obj)
        {
            if (obj is Vector2D<T> vector)
            {
                if (X.CompareTo(vector.X) > 0 && Y.CompareTo(vector.Y) > 0)
                    return 1;
                else if (X.CompareTo(vector.X) < 0 && Y.CompareTo(vector.Y) < 0)
                    return -1;
                else
                    return 0;
            }
            else
                return 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector2D<T> vector)
                return vector.X.Equals(X) && vector.Y.Equals(Y);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return Convert.ToInt32(X) * 5 + Convert.ToInt32(Y) * 17 - 31;
        }

        public override string ToString()
        {
            return X.ToString() + " " + Y.ToString();
        }
    }
}
