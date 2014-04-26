using UnityEngine;
using System.Collections.Generic;
using SteveSharp;

public class Worm : MonoBehaviour
{
    public GridEntity ent;

    public AudioClip bump;
    public AudioClip move;

    void Start()
    {
        ent = GetComponent<GridEntity>();
    }

    void Update()
    {
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
            if( !ent.TryMove(dr,dc) )
                AudioSource.PlayClipAtPoint( bump, transform.position );
            else
                AudioSource.PlayClipAtPoint( move, transform.position );
            }
        }
    }
}
