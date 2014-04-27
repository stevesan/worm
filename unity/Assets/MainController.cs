using UnityEngine;
using System.Collections.Generic;
using SteveSharp;

public class MainController : MonoBehaviour {

    public static MainController main = null;

    public MapSpawner map;
    public TextAsset[] levelMapSrcs;
    public GameObject startScreen;
    public GameObject debriefScreen;
    public GameObject deathScreen;

    public GameObject wormSegPrefab;
    public AudioClip bump;
    public AudioClip move;
    public AudioClip grow;
    public AudioClip reverse;
    public AudioClip die;
    public AudioClip error;
    public AudioClip split;
    public AudioClip merge;
    public AudioClip beatlevel;
    public float repeatPeriod = 0.1f;

    float repeatTimer = 0f;

    //----------------------------------------
    //  Game stuff
    //----------------------------------------
    HashSet< List<Worm> > detachedTails = new HashSet< List<Worm> >();
    List<Worm> tail = new List<Worm>();
    Worm head;

    void Awake()
    {
        main = this;
    }

    public void OnWormHit( Worm victim, GameObject hitter )
    {
        if( state == "level" )
        {
            state = "dead";
            deathScreen.SetActive(true);
            AudioSource.PlayClipAtPoint( die, transform.position );
        }
    }

    void Start()
    {
        InputStack.Push(this);

        debriefScreen.SetActive(false);
        deathScreen.SetActive(false);
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

        tail.Clear();
        detachedTails.Clear();
        
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
            InLevelUpdate();

            //----------------------------------------
            //  Cheats
            //----------------------------------------
            if( Input.GetKeyDown(KeyCode.Equals) )
                SwitchLevel(currLevel+1);
            if( Input.GetKeyDown(KeyCode.Minus) )
                SwitchLevel(currLevel-1);
            if( Input.GetKeyDown(KeyCode.Alpha0) )
                SwitchLevel(currLevel);
        }
        else if( state == "dead" )
        {
            if( InputStack.IsActive(this) && Input.GetKeyDown(KeyCode.Space) )
            {
                deathScreen.SetActive(false);
                SwitchLevel(currLevel);
            }
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
                    // beat game!
                    map.Clear();
                    startScreen.SetActive(true);
                    state = "start";
                }
            }
        }
        else if( state == "ending" )
        {
        }
    }

    void InLevelUpdate()
    {
        if( !InputStack.IsActive(this) )
            return;

        //----------------------------------------
        //  Reverse key
        //----------------------------------------
        if( Input.GetKeyDown(KeyCode.R) )
        {
            if( tail.Count == 0 )
                AudioSource.PlayClipAtPoint( error, transform.position );
            else
            {
                // reverse head/tail
                var oldHead = head;
                head.isHead = false;
                head = tail[tail.Count-1];
                head.isHead = true;
                tail.RemoveAt(tail.Count-1);
                tail.Reverse();
                tail.Add(oldHead);
                AudioSource.PlayClipAtPoint( reverse, transform.position );
            }
        }

        if( Input.GetKeyDown(KeyCode.Space) )
        {
            if( tail.Count == 0 )
                AudioSource.PlayClipAtPoint( error, transform.position );
            else
            {
                // split in two!
                int totalSegs = tail.Count + 1;
                int numInNew = totalSegs / 2;
                int numInOld = totalSegs - numInNew - 1;
                List<Worm> newTail = new List<Worm>();
                for( int i = 0; i < numInNew; i++ )
                {
                    newTail.Add( tail[numInOld + i] );
                    newTail.GetLast().isDetached = true;
                }
                detachedTails.Add(newTail);
                tail.RemoveRange(numInOld, numInNew);
                AudioSource.PlayClipAtPoint( split, transform.position );
            }
        }

        repeatTimer -= Time.deltaTime;

        int dr = 0;
        int dc = 0;

        /*
        if( Input.GetKeyDown(KeyCode.W)
                || Input.GetKeyDown(KeyCode.A)
                || Input.GetKeyDown(KeyCode.S)
                || Input.GetKeyDown(KeyCode.D) )
            repeatTimer = -1;
            */

        if( (repeatTimer < 0 && Input.GetKey(KeyCode.W) ) )
            dr -= 1;
        else if( (repeatTimer < 0 && Input.GetKey(KeyCode.S) ) )
            dr += 1;
        else if( (repeatTimer < 0 && Input.GetKey(KeyCode.A) ) )
            dc -= 1;
        else if( (repeatTimer < 0 && Input.GetKey(KeyCode.D) ) )
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
                if( detachedTails.Count > 0 )
                {
                    AudioSource.PlayClipAtPoint( error, transform.position );
                    Debug.Log("detached tails in level!");
                }
                // check for remaining fruits..
                else if( map.entsRoot.GetComponentsInChildren<Fruit>().Length > 0 )
                {
                    AudioSource.PlayClipAtPoint( error, transform.position );
                    Debug.Log("fruits remain!");
                }
                else
                {
                    AudioSource.PlayClipAtPoint( beatlevel, transform.position );

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
            else if( other != null && other.GetComponent<Worm>() != null )
            {
                var hitBit = other.GetComponent<Worm>();
                if( RemergeTail(hitBit) )
                {
                    AudioSource.PlayClipAtPoint( merge, transform.position );
                }
                else
                    AudioSource.PlayClipAtPoint( error, transform.position );
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
                    //AudioSource.PlayClipAtPoint( move, transform.position );

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

            //----------------------------------------
            //  reset timer
            //  Do NOT do this if we did not move..
            //----------------------------------------
            if( repeatTimer < 0 )
                repeatTimer = repeatPeriod;
        }

    }

    bool RemergeTail( Worm hit )
    {
        // find the inactive tail that this bit is a part of
        List<Worm> hitTail = null;
        foreach( var tail in detachedTails )
        {
            // is the hit piece the tail or head of this tail?
            if( hit == tail.GetFirst() || hit == tail.GetLast() )
            {
                hitTail = tail;
                break;
            }
        }

        if( hitTail == null )
            return false;
        else
        {
            detachedTails.Remove(hitTail);

            // reconnect so the new head is the OTHER end of the hit tail
            if( hit == hitTail.GetFirst() )
                hitTail.Reverse();

            head.isHead = false;
            hitTail.Add(head);
            hitTail.AddRange(tail);
            head = hitTail.GetFirst();
            head.isHead = true;
            head.isDetached = false;
            hitTail.RemoveAt(0);

            // discard old tail and replace with new
            tail.Clear();
            tail = hitTail;
            foreach( var bit in tail )
                bit.isDetached = false;
            return true;
        }
    }
}
