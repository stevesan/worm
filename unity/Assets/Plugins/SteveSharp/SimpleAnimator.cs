
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace SteveSharp
{
    public class SimpleAnimator : MonoBehaviour
    {
        [System.Serializable]
        public class Key
        {
            public bool isRelative = false;
            public Vector3 position;
            public float motionTime;
            public float afterTime;
        }

        public List<Key> keys;
        public bool playOnStart;
        public float timeScale = 1f;

        bool stopRequested = false;
        bool isPlaying = false;

        bool ShouldStop()
        {
            return stopRequested || !gameObject.activeInHierarchy;
        }

        IEnumerator PlaybackCoroutine()
        {
            isPlaying = true;

            for( int ik = 0; ik < keys.Count; ik++ )
            {
                if(ShouldStop())
                {
                    isPlaying = false;
                    yield break;
                }

                var key = keys[ik];
                float keyStartTime = Time.time;
                Vector3 vel = Vector3.zero;
                Vector3 goal = key.isRelative ? transform.position+key.position : key.position;
                while( Time.time - keyStartTime < key.motionTime*timeScale )
                {
                    if(ShouldStop())
                    {
                        isPlaying = false;
                        yield break;
                    }
                    transform.position = Vector3.SmoothDamp(
                            transform.position, 
                            goal, ref vel, key.motionTime*timeScale*0.1f );
                    yield return null;
                }
                transform.position = goal;

                yield return new WaitForSeconds(key.afterTime*timeScale);
            }

            isPlaying = false;
        }

        public void Play()
        {
            if( !isPlaying )
                StartCoroutine(PlaybackCoroutine());
        }

        void Start()
        {
            if( playOnStart )
                Play();
        }
    }
}
