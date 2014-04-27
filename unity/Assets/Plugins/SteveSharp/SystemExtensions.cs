
using System.Collections.Generic;
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

        public static T GetFirst<T>( this List<T> list )
        {
            return list[0];
        }

        public static T GetLast<T>( this List<T> list )
        {
            return list[ list.Count-1 ];
        }
    }
}
