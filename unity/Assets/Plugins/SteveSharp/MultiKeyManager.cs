
using UnityEngine;
using System.Collections.Generic;

namespace SteveSharp
{
    public class MultiKeyManager
    {
        HashSet<KeyCode> keys = new HashSet<KeyCode>();
        Dictionary<KeyCode, int> downFrames = new Dictionary<KeyCode, int>();

        public void AddKey( KeyCode code )
        {
            keys.Add(code);
        }

        public void Reset()
        {
            downFrames.Clear();
        }

        public KeyCode Update()
        {
            // update down times
            foreach( var code in keys )
            {
                if( Input.GetKeyDown(code) )
                    downFrames[code] = Time.frameCount;
            }

            // return the key that is still down with the most recent down time
            int latestFrame = -1;
            KeyCode bestCode = KeyCode.None;
            foreach( var code in keys )
            {
                int frame = downFrames.ContainsKey(code) ?
                    downFrames[code] :
                    -1;

                if( Input.GetKey(code) && frame > latestFrame )
                {
                    latestFrame = frame;
                    bestCode = code;
                }
            }

            return bestCode;
        }
    }
}
