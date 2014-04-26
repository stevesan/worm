
namespace SteveSharp
{
    public struct Int2
    {
        public int x;
        public int y;

        public int row { get { return x; } set { x = value; } }
        public int col { get { return y; } set { y = value; } }

        public Int2( int x, int y )
        {
            this.x = x;
            this.y = y;
        }

        public static Int2 operator * ( int s, Int2 u )
        {
            return new Int2(s*u.x, s*u.y);
        }

        public static Int2 operator + ( Int2 u, Int2 v )
        {
            return new Int2(u.x + v.x, u.y + v.y);
        }
    }
}
