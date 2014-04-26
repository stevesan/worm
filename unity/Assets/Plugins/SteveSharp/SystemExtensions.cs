
using UnityEngine;

namespace SteveSharp
{
    public static class SystemExtensions
    {
        public static void IdentityLocals( this Transform t )
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = new Vector3(1,1,1);
        }

        public static bool CheckBounds<T>( this T[,] grid, int r, int c )
        {
            return r >= 0 && c >= 0
                && r < grid.GetLength(0)
                && c < grid.GetLength(1);
        }
    }
}
