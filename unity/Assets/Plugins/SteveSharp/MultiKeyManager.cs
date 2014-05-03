
using UnityEngine;
using System.Collections.Generic;

namespace SteveSharp
{
    public class MultiKeyManager : MonoBehaviour
    {
        public float downSustainTime;

        HashSet<KeyCode> keys = new HashSet<KeyCode>();
        Dictionary<KeyCode, int> downFrames = new Dictionary<KeyCode, int>();
        Dictionary<KeyCode, float> downSustainTimers = new Dictionary<KeyCode, float>();

        KeyCode activeKey = KeyCode.None;

        public void AddKey( KeyCode code )
        {
            keys.Add(code);
        }

        public void Reset()
        {
            downFrames.Clear();
            downSustainTimers.Clear();
        }

        int lastUpdatedFrame = -1;

        void Update()
        {
            InternalUpdate();
        }

        public KeyCode GetActiveKey()
        {
            InternalUpdate();
            return activeKey;
        }

        bool IsKeyHeldOrSustained(KeyCode code)
        {
            return Input.GetKey(code) || downSustainTimers.GetSafe(code, -1f) > 0f;
        }

        private void InternalUpdate()
        {
            if( Time.frameCount <= lastUpdatedFrame )
                return;

            lastUpdatedFrame = Time.frameCount;

            // update states
            foreach( var code in keys )
            {
                if( Input.GetKeyDown(code) )
                {
                    downFrames[code] = Time.frameCount;
                    downSustainTimers[code] = downSustainTime+Time.deltaTime;
                }

                // tick down down-sustain timers
                if( downSustainTimers.ContainsKey(code) )
                    downSustainTimers[code] = downSustainTimers[code] - Time.deltaTime;
            }

            // return the key that is still down with the most recent down time
            int latestFrame = -1;
            KeyCode bestCode = KeyCode.None;
            foreach( var code in keys )
            {
                int frame = downFrames.ContainsKey(code) ?
                    downFrames[code] :
                    -1;

                if( IsKeyHeldOrSustained(code) && frame > latestFrame )
                {
                    latestFrame = frame;
                    bestCode = code;
                }
            }

            activeKey = bestCode;
        }
    }
}
