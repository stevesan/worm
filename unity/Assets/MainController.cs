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

    MultiKeyManager moveKeys = new MultiKeyManager();

    //----------------------------------------
    //  Game stuff
    //----------------------------------------
    HashSet< List<Seg> > worms = new HashSet< List<Seg> >();
    List<Seg> activeWorm = new List<Seg>();
    Dictionary<int, List<Seg> > key2worm = new Dictionary<int, List<Seg>>();
    Dictionary<List<Seg>, int > worm2key = new Dictionary<List<Seg>, int>();

    void Awake()
    {
        main = this;

        moveKeys.AddKey( KeyCode.W );
        moveKeys.AddKey( KeyCode.A );
        moveKeys.AddKey( KeyCode.S );
        moveKeys.AddKey( KeyCode.D );
    }

    int GetFreeWormKey()
    {
        for( int key = 1; key < 9; key++ )
        {
            if( !key2worm.ContainsKey(key) )
                return key;
        }
        return -1;
    }

    public void OnWormSegHit( Seg victim, GameObject hitter )
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

        activeWorm.Clear();
        worms.Clear();
        key2worm.Clear();
        worm2key.Clear();
        moveKeys.Reset();

        // find the player and make it active
        var head = map.entsRoot.GetComponentInChildren<Seg>();
        Debug.Log("found player ent = "+head.gameObject.name);
        head.isHead = true;
        head.isActive = true;
        activeWorm = new List<Seg>();
        activeWorm.Add(head);
        worms.Add(activeWorm);
        key2worm[1] = activeWorm;
        worm2key[activeWorm] = 1;
        
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
            if( activeWorm.Count == 0 )
                AudioSource.PlayClipAtPoint( error, transform.position );
            else
            {
                // reverse head/tail
                activeWorm.GetFirst().isHead = false;
                activeWorm.Reverse();
                activeWorm.GetFirst().isHead = true;
                AudioSource.PlayClipAtPoint( reverse, transform.position );
            }
        }

        if( Input.GetKeyDown(KeyCode.Space) )
        {
            if( activeWorm.Count == 0 )
                AudioSource.PlayClipAtPoint( error, transform.position );
            else
            {
                // split in two!
                int totalSegs = activeWorm.Count;
                int numInNew = totalSegs / 2;
                int numInOld = totalSegs - numInNew;
                List<Seg> newWorm = new List<Seg>();
                for( int i = 0; i < numInNew; i++ )
                {
                    newWorm.Add( activeWorm[numInOld + i] );
                    newWorm.GetLast().isHead = false;
                    newWorm.GetLast().isActive = false;
                }
                worms.Add(newWorm);
                activeWorm.RemoveRange(numInOld, numInNew);
                AudioSource.PlayClipAtPoint( split, transform.position );

                // assign a key
                int key = GetFreeWormKey();

                if( key != -1 )
                {
                    key2worm[ key ] = newWorm;
                    worm2key[newWorm] = key;
                }
            }
        }

        for( int key = 1; key < 9; key++ )
        {
            if( Input.GetKeyDown(""+key) && key2worm.ContainsKey(key) )
            {
                // switch to this worm
                foreach( var seg in activeWorm )
                {
                    seg.isActive = false;
                    seg.isHead = false;
                }

                activeWorm = key2worm[key];
                foreach( var seg in activeWorm )
                {
                    seg.isActive = true;
                    seg.isHead = false;
                }
                activeWorm.GetFirst().isHead = true;
            }
        }

        repeatTimer -= Time.deltaTime;

        int dr = 0;
        int dc = 0;

        KeyCode moveKey = moveKeys.Update();

        if( repeatTimer < 0 )
        {
            if( moveKey == KeyCode.W )
                dr -= 1;
            else if( moveKey == KeyCode.S )
                dr += 1;
            else if( moveKey == KeyCode.A )
                dc -= 1;
            else if( moveKey == KeyCode.D )
                dc += 1;
        }

        if( dr != 0 || dc != 0 )
        {
            var head = activeWorm.GetFirst();
            var other = head.ent.Peek(dr, dc);

            if( other != null && other.GetComponent<LevelExit>() != null )
            {
                if( worms.Count > 1 )
                {
                    AudioSource.PlayClipAtPoint( error, transform.position );
                    Debug.Log("detached worms in level!");
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
            else if( other != null && other.GetComponent<Seg>() != null
                  && !other.GetComponent<Seg>().isActive )
            {
                var hitBit = other.GetComponent<Seg>();
                if( RemergeTail(hitBit) )
                {
                    AudioSource.PlayClipAtPoint( merge, transform.position );
                }
                else
                    AudioSource.PlayClipAtPoint( error, transform.position );
            }
            else
            {
                // do move

                bool grew = false;
                // but maybe eating a fruit?
                if( other != null && other.GetComponent<Fruit>() != null )
                {
                    AudioSource.PlayClipAtPoint( grow, transform.position );

                    map.RemoveEntity(other);
                    Destroy(other.gameObject);

                    // create new worm segment at end later
                    grew = true;
                }

                Int2 endPos = activeWorm.GetLast().ent.pos;

                if( !TryMoveWorm(activeWorm, new Int2(dr,dc) ) )
                    AudioSource.PlayClipAtPoint( bump, transform.position );
                else
                {
                    if( grew )
                    {
                        var newSeg = map.SpawnPrefab( wormSegPrefab, endPos.row, endPos.col );
                        activeWorm.Add(newSeg.GetComponent<Seg>());
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

    bool TryMoveWorm( List<Seg> worm, Int2 delta )
    {
        Seg head = worm.GetFirst();
        Int2 nextDest = head.ent.pos;

        if( !head.ent.TryMove(delta) )
            return false;
        else
        {
            // move all worm segments
            for( int i = 1; i < worm.Count; i++ )
            {
                var seg = worm[i];
                Int2 temp = seg.ent.pos;
                seg.ent.TryMove(nextDest - seg.ent.pos);
                nextDest = temp;
            }

            return true;
        }
    }

    bool RemergeTail( Seg hit )
    {
        // find the inactive tail that this bit is a part of
        List<Seg> hitWorm = null;
        foreach( var worm in worms )
        {
            // is the hit piece the tail or head of this tail?
            if( hit == worm.GetFirst() || hit == worm.GetLast() )
            {
                hitWorm = worm;
                break;
            }
        }

        if( hitWorm == null )
            return false;
        else
        {
            // assimilate!
            worms.Remove(hitWorm);
            if( worm2key.ContainsKey(hitWorm) )
            {
                int key = worm2key[hitWorm];
                worm2key.Remove(hitWorm);
                key2worm.Remove(key);
            }

            // reconnect so the new head is the OTHER end of the hit tail
            if( hit == hitWorm.GetFirst() )
                hitWorm.Reverse();
            activeWorm.GetFirst().isHead = false;
            activeWorm.InsertRange(0, hitWorm);
            foreach( var seg in activeWorm )
            {
                seg.isActive = true;
                seg.isHead = false;
            }
            activeWorm.GetFirst().isHead = true;
            return true;
        }
    }
}
