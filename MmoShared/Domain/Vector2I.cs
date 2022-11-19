using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using ProtoBuf;

namespace MmoServer.Core
{
    // Representation of 2D vectors and points.
    [ProtoContract]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2I : IEquatable<Vector2I>, IFormattable
    {
        [ProtoMember(1)]
        public int x
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return m_X; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { m_X = value; }
        }

        [ProtoMember(2)]
        public int y
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return m_Y; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { m_Y = value; } 
        }

        private int m_X;
        private int m_Y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2I(int x, int y)
        {
            m_X = x;
            m_Y = y;
        }

        // Set x and y components of an existing Vector.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(int x, int y)
        {
            m_X = x;
            m_Y = y;
        }

        // Access the /x/ or /y/ component using [0] or [1] respectively.
        public int this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    default:
                        throw new IndexOutOfRangeException(String.Format("Invalid Vector2Int index addressed: {0}!", index));
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    default:
                        throw new IndexOutOfRangeException(String.Format("Invalid Vector2Int index addressed: {0}!", index));
                }
            }
        }

        // Returns the length of this vector (RO).
        [JsonIgnore]
        public float magnitude { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return (float)Math.Sqrt(x * x + y * y); } }

        // Returns the squared length of this vector (RO).
        [JsonIgnore]
        public int sqrMagnitude { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return x * x + y * y; } }

        // Returns the distance between /a/ and /b/.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(Vector2I a, Vector2I b)
        {
            float diff_x = a.x - b.x;
            float diff_y = a.y - b.y;

            return (float)Math.Sqrt(diff_x * diff_x + diff_y * diff_y);
        }

        // Returns a vector that is made from the smallest components of two vectors.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2I Min(Vector2I lhs, Vector2I rhs) { return new Vector2I(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y)); }

        // Returns a vector that is made from the largest components of two vectors.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2I Max(Vector2I lhs, Vector2I rhs) { return new Vector2I(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y)); }

        // Multiplies two vectors component-wise.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2I Scale(Vector2I a, Vector2I b) { return new Vector2I(a.x * b.x, a.y * b.y); }

        // Multiplies every component of this vector by the same component of /scale/.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Scale(Vector2I scale) { x *= scale.x; y *= scale.y; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clamp(Vector2I min, Vector2I max)
        {
            x = Math.Max(min.x, x);
            x = Math.Min(max.x, x);
            y = Math.Max(min.y, y);
            y = Math.Min(max.y, y);
        }

        // Converts a Vector2Int to a [[Vector2]].
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector2(Vector2I v)
        {
            return new Vector2(v.x, v.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2I FloorToInt(Vector2 v)
        {
            return new Vector2I(
                (int)Math.Floor(v.X),
                (int)Math.Floor(v.Y)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2I CeilToInt(Vector2 v)
        {
            return new Vector2I(
                (int)Math.Ceiling(v.X),
                (int)Math.Ceiling(v.Y)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2I RoundToInt(Vector2 v)
        {
            return new Vector2I(
                (int)Math.Round(v.X),
                (int)Math.Round(v.Y)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2I operator-(Vector2I v)
        {
            return new Vector2I(-v.x, -v.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2I operator+(Vector2I a, Vector2I b)
        {
            return new Vector2I(a.x + b.x, a.y + b.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2I operator-(Vector2I a, Vector2I b)
        {
            return new Vector2I(a.x - b.x, a.y - b.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2I operator*(Vector2I a, Vector2I b)
        {
            return new Vector2I(a.x * b.x, a.y * b.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2I operator*(int a, Vector2I b)
        {
            return new Vector2I(a * b.x, a * b.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2I operator*(Vector2I a, int b)
        {
            return new Vector2I(a.x * b, a.y * b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2I operator/(Vector2I a, int b)
        {
            return new Vector2I(a.x / b, a.y / b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator==(Vector2I lhs, Vector2I rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator!=(Vector2I lhs, Vector2I rhs)
        {
            return !(lhs == rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object other)
        {
            if (!(other is Vector2I)) return false;

            return Equals((Vector2I)other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector2I other)
        {
            return x == other.x && y == other.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2);
        }

        /// *listonly*
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return ToString(null, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format)
        {
            return ToString(format, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (formatProvider == null)
                formatProvider = CultureInfo.InvariantCulture.NumberFormat;
            return $"({x.ToString(format, formatProvider)}, {y.ToString(format, formatProvider)})";
        }

        public static Vector2I zero { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => s_Zero; }
        public static Vector2I one { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => s_One; }
        public static Vector2I up { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => s_Up; }
        public static Vector2I down { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => s_Down; }
        public static Vector2I left { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => s_Left; }
        public static Vector2I right { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => s_Right; }

        private static readonly Vector2I s_Zero = new Vector2I(0, 0);
        private static readonly Vector2I s_One = new Vector2I(1, 1);
        private static readonly Vector2I s_Up = new Vector2I(0, 1);
        private static readonly Vector2I s_Down = new Vector2I(0, -1);
        private static readonly Vector2I s_Left = new Vector2I(-1, 0);
        private static readonly Vector2I s_Right = new Vector2I(1, 0);
    }
}