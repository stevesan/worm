
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
    }
}
