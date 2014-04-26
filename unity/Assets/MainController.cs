using UnityEngine;
using System.Collections.Generic;

public class MainController : MonoBehaviour {

    public MapSpawner map;
    public TextAsset[] levelMapSrcs;
    public GameObject startScreen;

    public GameObject wormSegPrefab;
    public AudioClip bump;
    public AudioClip move;
    public AudioClip grow;


    //----------------------------------------
    //  Game stuff
    //----------------------------------------
    List<Worm> tail = new List<Worm>();
    Worm head;

	void Start()
    {
        InputStack.Push(this);
	}

    string state = "start";
	
	// Update is called once per frame
	void Update() {

        if( state == "start" )
        {
            if( InputStack.IsActive(this) && Input.GetKeyDown(KeyCode.Space) )
            {
                state = "play";
                startScreen.SetActive(false);
                map.Spawn(levelMapSrcs[0].text);

                // find the player and make it active
                head = map.entsRoot.GetComponentInChildren<Worm>();
                head.isHead = true;
                Debug.Log("found player ent = "+head.gameObject.name);
            }
        }
        else if( state == "play" )
        {
            //----------------------------------------
            //  Handle player input
            //----------------------------------------
            if( InputStack.IsActive(this) )
            {
                int dr = 0;
                int dc = 0;

                if( Input.GetKeyDown(KeyCode.W) )
                    dr -= 1;
                if( Input.GetKeyDown(KeyCode.S) )
                    dr += 1;
                if( Input.GetKeyDown(KeyCode.A) )
                    dc -= 1;
                if( Input.GetKeyDown(KeyCode.D) )
                    dc += 1;

                if( dr != 0 || dc != 0 )
                {
                    var other = head.ent.Peek(dr, dc);

                    if( other != null
                            && other.gameObject.GetComponent<Fruit>() != null )
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
                    else
                    {
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
                else if( Input.GetKeyDown(KeyCode.R) )
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
            }
        }
	
	}
}
