using UnityEngine;
using System.Collections;
using SteveSharp;

public class Rolly : MonoBehaviour {

    public int dr;
    public int dc;
    public float secsPerMove = 0.5f;

    public float alertSecsPerMove = 0.1f;
    public bool hasVision = false;
    public Material alertedVisionMaterial;
    public Material normalVisionMaterial;

    Int2 delta { get { return new Int2(dr, dc); } }
    float moveTimer = 0f;
    int flip = 1;
    bool seesPlayer = false;

    GridEntity ent;

    LineRenderer line;

	// Use this for initialization
	void Awake() {
        ent = GetComponent<GridEntity>();
        line = GetComponent<LineRenderer>();
	}

    void UpdateVision()
    {
        if( !hasVision )
            return;

        // scan the grid and see if we can see the player
        bool seesPlayerNow = false;
        int steps = 1;
        while(true)
        {
            Int2 peek = steps * flip * delta;

            if( !ent.CheckBounds( ent.pos+peek ) )
                break;

            var other = ent.Peek(peek);
            if( other != null )
            {
                if( other.GetComponent<Seg>() )
                    seesPlayerNow = true;
                else
                    // blocked by a wall or something
                    seesPlayerNow = false;
                break;
            }

            steps++;
        }

        if( seesPlayerNow && !seesPlayer )
        {
            // immediately move! and faster!
            moveTimer = -1;
        }
        seesPlayer = seesPlayerNow;

        line.SetVertexCount(2);
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position
                + (Vector3.right * (steps*flip*delta).col)
                + (Vector3.up * -1*(steps*flip*delta).row) );
        line.SetWidth(0, 1);
        line.material = seesPlayer ? alertedVisionMaterial : normalVisionMaterial;
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateVision();

        moveTimer -= Time.deltaTime;

        if( moveTimer < 0f )
        {
            if( !ent.TryMove( flip*delta ) )
            {
                var other = ent.Peek( flip*delta );
                // we hit a player?
                if( other.GetComponent<Seg>() != null )
                {
                    MainController.main.OnWormSegHit( other.GetComponent<Seg>(), gameObject );
                }
                else
                {
                    // a wall. turn around
                    flip *= -1;
                    ent.TryMove( flip*delta );
                }
            }

            if( seesPlayer )
                moveTimer = alertSecsPerMove;
            else
                moveTimer = secsPerMove;
        }

        if( seesPlayer )
        {
            ent.smoothTime = alertSecsPerMove*0.5f;
            transform.localScale = new Vector3(1.2f, 1.2f, 1f);
        }
        else
        {
            ent.smoothTime = secsPerMove*0.5f;
            transform.localScale = new Vector3(1f, 1f, 1f);
        }

	}
}
