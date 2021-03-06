using UnityEngine;
using System.Collections.Generic;
using SteveSharp;

// a segment of a worm
public class Seg : MonoBehaviour
{
    public GridEntity ent;
    public GameObject segmentPrefab;

    public bool isHead = false;
    public bool isActive = true;

    public Vector3 scaleVel = Vector3.zero;

    void Awake()
    {
        ent = GetComponent<GridEntity>();
    }

    void Start()
    {
        // do a lil "grow" from nothing animation
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        Vector3 targetScale = Vector3.zero;
        if( isHead )
            targetScale = new Vector3(1,1,1);
        else if( isActive )
            targetScale = new Vector3(0.7f,0.7f,0.7f);
        else
            targetScale = new Vector3(0.5f,0.5f,0.5f);
        transform.localScale = Vector3.SmoothDamp( transform.localScale,
                targetScale, ref scaleVel, 0.05f );
    }
}
