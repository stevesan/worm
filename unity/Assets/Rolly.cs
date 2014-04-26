using UnityEngine;
using System.Collections;
using SteveSharp;

public class Rolly : MonoBehaviour {

    public int dr;
    public int dc;
    public float secsPerMove = 0.5f;

    Int2 delta { get { return new Int2(dr, dc); } }
    float moveTimer = 0f;
    int flip = 1;

    GridEntity ent;

	// Use this for initialization
	void Awake() {
        ent = GetComponent<GridEntity>();
	
	}
	
	// Update is called once per frame
	void Update () {

        moveTimer -= Time.deltaTime;

        if( moveTimer < 0f )
        {
            Int2 want = ent.pos + flip*delta;

            if( ent.CanMove(flip*delta) )
                ent.TryMove( flip*delta );
            else
            {
                flip *= -1;
                ent.TryMove( flip*delta );
            }
            moveTimer = secsPerMove;
        }
	}
}
