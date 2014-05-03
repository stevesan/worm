
using UnityEngine;
using System.Collections.Generic;

namespace SteveSharp
{
    public class MultiKeyManager : MonoBehaviour
    {
        public float maxCommandBufferedTime;

        HashSet<KeyCode> keys = new HashSet<KeyCode>();
        Dictionary<KeyCode, int> downFrames = new Dictionary<KeyCode, int>();

        public class Command
        {
            public KeyCode key;
            public float time;
            public Command( KeyCode key, float time )
            {
                this.key = key;
                this.time = time;
            }
        }
        Queue<Command> commandQueue = new Queue<Command>();

        KeyCode latestHeldKey = KeyCode.None;

        public void AddKey( KeyCode code )
        {
            keys.Add(code);
        }

        public void Reset()
        {
            downFrames.Clear();
            commandQueue.Clear();
        }

        int lastUpdatedFrame = -1;

        void Update()
        {
            InternalUpdate();
        }

        // Call this to query the "active key" when the game is ready to respond to something
        public KeyCode GetActiveKey()
        {
            InternalUpdate();

            // any valid commands which should 
            while( commandQueue.Count > 0 )
            {
                var com = commandQueue.Dequeue();
                if( com.time < Time.time-maxCommandBufferedTime )
                {
                    // command is dead..ignore
                }
                else
                {
                    // command is live - return it
                    return com.key;
                }
            }

            return latestHeldKey;
        }

        private void InternalUpdate()
        {
            if( Time.frameCount <= lastUpdatedFrame )
                return;

            lastUpdatedFrame = Time.frameCount;

            // update states
            foreach( var key in keys )
            {
                if( Input.GetKeyDown(key) )
                {
                    downFrames[key] = Time.frameCount;
                    commandQueue.Enqueue( new Command(key, Time.time) );
                }
            }

            // update the hold key
            int latestFrame = -1;
            latestHeldKey = KeyCode.None;
            foreach( var key in keys )
            {
                int frame = downFrames.ContainsKey(key) ?
                    downFrames[key] :
                    -1;

                if( Input.GetKey(key) && frame > latestFrame )
                {
                    latestFrame = frame;
                    latestHeldKey = key;
                }
            }
        }
    }
}
