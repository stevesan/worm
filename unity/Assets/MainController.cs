using UnityEngine;
using System.Collections.Generic;

public class MainController : MonoBehaviour {

    public MapSpawner map;
    public TextAsset[] levelMapSrcs;
    public GameObject startScreen;
    public GameObject debriefScreen;

    public GameObject wormSegPrefab;
    public AudioClip bump;
    public AudioClip move;
    public AudioClip grow;
    public float repeatPeriod = 0.1f;

    float repeatTimer = 0f;

    //----------------------------------------
    //  Game stuff
    //----------------------------------------
    List<Worm> tail = new List<Worm>();
    Worm head;

    void Start()
    {
        InputStack.Push(this);

        debriefScreen.SetActive(false);
        startScreen.SetActive(true);
    }

    string state = "start";
    int currLevel = 0;

    void SwitchLevel( int level )
    {
        if( level >= levelMapSrcs.Length )
        {
            Debug.LogError("no map src for level "+level);
            return;
        }

        currLevel = level;

        map.Spawn(levelMapSrcs[level].text);

        // find the player and make it active
        head = map.entsRoot.GetComponentInChildren<Worm>();
        head.isHead = true;
        Debug.Log("found player ent = "+head.gameObject.name);
        
        state = "level";
    }

    // Update is called once per frame
    void Update() {

        if( state == "start" )
        {
            if( InputStack.IsActive(this) && Input.GetKeyDown(KeyCode.Space) )
            {
                state = "level";
                startScreen.SetActive(false);
                SwitchLevel(0);
            }
        }
        else if( state == "level" )
        {
            //----------------------------------------
            //  Handle player input
            //----------------------------------------
            UpdatePlayerMove();
        }
        else if( state == "debrief" )
        {
            if( InputStack.IsActive(this) && Input.GetKeyDown(KeyCode.Space) )
            {
                debriefScreen.SetActive(false);
                if( currLevel+1 < levelMapSrcs.Length )
                    SwitchLevel(currLevel+1);
                else
                {
                    // beat!
                    startScreen.SetActive(true);
                    state = "start";
                }
            }
        }
        else if( state == "ending" )
        {
        }
    }

    void UpdatePlayerMove()
    {
        if( !InputStack.IsActive(this) )
            return;

        //----------------------------------------
        //  Reverse key
        //----------------------------------------
        if( Input.GetKeyDown(KeyCode.R) )
        {
            // reverse head/tail
            var oldHead = head;
            head.isHead = false;
            head = tail[tail.Count-1];
            head.isHead = true;
            tail.RemoveAt(tail.Count-1);
            tail.Reverse();
            tail.Add(oldHead);
        }

        repeatTimer -= Time.deltaTime;

        if( Input.GetKeyDown(KeyCode.LeftShift) )
            repeatTimer = -1;
        bool run = Input.GetKey(KeyCode.LeftShift);

        int dr = 0;
        int dc = 0;

        if( Input.GetKeyDown(KeyCode.W) || (repeatTimer < 0 && Input.GetKey(KeyCode.W) && run) )
            dr -= 1;
        if( Input.GetKeyDown(KeyCode.S) || (repeatTimer < 0 && Input.GetKey(KeyCode.S) && run) )
            dr += 1;
        if( Input.GetKeyDown(KeyCode.A) || (repeatTimer < 0 && Input.GetKey(KeyCode.A) && run) )
            dc -= 1;
        if( Input.GetKeyDown(KeyCode.D) || (repeatTimer < 0 && Input.GetKey(KeyCode.D) && run) )
            dc += 1;

        if( dr != 0 || dc != 0 )
        {
            var other = head.ent.Peek(dr, dc);

            if( other != null
                    && other.GetComponent<Fruit>() != null )
            {
                AudioSource.PlayClipAtPoint( grow, transform.position );

                map.RemoveEntity(other);
                Destroy(other.gameObject);

                // create new worm segment
                head.isHead = false;
                tail.Insert(0,head);
                int newRow = head.ent.row + dr;
                int newCol = head.ent.col + dc;
                var newObj = map.SpawnPrefab( wormSegPrefab, newRow, newCol );
                head = newObj.GetComponent<Worm>();
                head.isHead = true;
            }
            else if( other != null && other.GetComponent<LevelExit>() != null )
            {
                state = "debrief";
                debriefScreen.SetActive(true);
                map.Clear();
                head = null;
                Debug.Log("beat level "+currLevel);
            }
            else
            {
                // normal move
                int oldRow = head.ent.row;
                int oldCol = head.ent.col;

                if( !head.ent.TryMove(dr,dc) )
                    AudioSource.PlayClipAtPoint( bump, transform.position );
                else
                {
                    AudioSource.PlayClipAtPoint( move, transform.position );

                    // move all tail segments
                    foreach( var seg in tail )
                    {
                        int r = seg.ent.row;
                        int c = seg.ent.col;
                        seg.ent.TryMove(oldRow-seg.ent.row, oldCol-seg.ent.col);
                        oldRow = r;
                        oldCol = c;
                    }
                }
            }
        }

        //----------------------------------------
        //  reset timer
        //----------------------------------------
        if( repeatTimer < 0 )
            repeatTimer = repeatPeriod;
    }
}
