using UnityEngine;
using System.Collections.Generic;

namespace SteveSharp
{
    public class Utility
    {
        public static bool Assert( bool cond, string msg = "see callstack in log" )
        {
            if( !cond )
                Debug.LogError("Failed Assert: "+msg);
            return cond;
        }

        public static C MyInstantiate<C>( C prefab, Vector3 pos, Transform parent = null ) where C : MonoBehaviour
        {
            GameObject inst = (GameObject)GameObject.Instantiate(prefab.gameObject, pos, Quaternion.identity);
            inst.transform.parent = parent;
            inst.SetActive(true);
            return inst.GetComponent<C>();
        }

        public static GameObject Instantiate( GameObject prefab, Vector3 pos, Transform parent = null ) 
        {
            GameObject inst = (GameObject)GameObject.Instantiate(prefab.gameObject, pos, Quaternion.identity);
            inst.transform.parent = parent;
            inst.SetActive(true);
            return inst;
        }

        public static C FindAncestor<C>( GameObject obj ) where C:MonoBehaviour
        {
            while( true )
            {
                if( obj == null )
                    return null;

                C comp = obj.GetComponent<C>();
                if( comp != null )
                    return comp;

                if( obj.transform.parent == null )
                    return null;

                obj = obj.transform.parent.gameObject;
            }
        }

        public static bool CanSee<C>( C target, Vector3 startPos ) where C : MonoBehaviour
        {
            Vector3 toTarget = target.transform.position - startPos;
            RaycastHit hit = new RaycastHit();
            if( Physics.Raycast( startPos, toTarget.normalized, out hit ) )
            {
                C hitComp = FindAncestor<C>(hit.collider.gameObject);
                if( hitComp == target )
                    return true;
            }
            return false;
        }

        public static float Unlerp( float from, float to, float x )
        {
            return (x - from) / (to - from);
        }

        public static int SignOrZero( float val, float eps = 1e-4f )
        {
            if( Mathf.Abs(val) < eps )
                return 0;
            else
                return (int)Mathf.Sign(val);
        }

        public static float LinearMap(
                float x0, float x1,
                float y0, float y1,
                float x )
        {
            float t = Unlerp( x0, x1, x );
            return Mathf.Lerp( y0, y1, t );
        }

        public static void GrowBounds( ref Bounds b, Bounds other )
        {
            b.min = Vector3.Min( b.min, other.min );
            b.max = Vector3.Max( b.max, other.max );
        }

        public static double GetSystemTime()
        {
            System.DateTime origin = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            System.TimeSpan diff = System.DateTime.Now.ToUniversalTime() - origin;
            return System.Math.Floor(diff.TotalMilliseconds);
        }

        public static Vector3 SampleCircleXY( Vector3 c, float radius )
        {
            Vector3 x = new Vector3( c.x + radius, c.y + radius, c.z + radius );

            while( Vector3.Distance( c, x ) > radius )
            {
                x = new Vector3(
                        c.x + Mathf.Lerp(-radius, radius, Random.value),
                        c.y + Mathf.Lerp(-radius, radius, Random.value),
                        c.z + Mathf.Lerp(-radius, radius, Random.value) );
            }

            return x;
        }

        public static Vector3 UpdateLerp(
                Vector3 curr,
                Vector3 goal,
                ref Vector3 lastStart,  // used to store the last start position
                float time )
        {
            if( Vector3.Distance(curr, goal) < 1e-4 )
            {
                curr = goal;
                lastStart = goal;
            }
            else
            {
                Vector3 dir = (goal-curr).normalized;
                float speed = Vector3.Distance(goal,lastStart) / time;
                curr += Vector3.ClampMagnitude(
                        dir*speed*Time.deltaTime,
                        Vector3.Distance(goal, curr) );
            }
            return curr;
        }
    }

}
