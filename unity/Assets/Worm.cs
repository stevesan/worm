using UnityEngine;
using System.Collections.Generic;
using SteveSharp;

public class Worm : MonoBehaviour
{
    public GridEntity ent;
    public GameObject segmentPrefab;

    public bool isHead = false;
    public bool isDetached = false;

    void Awake()
    {
        ent = GetComponent<GridEntity>();
    }

    void Update()
    {
        if( isHead )
            transform.localScale = new Vector3(1,1,1);
        else if( isDetached )
            transform.localScale = new Vector3(0.5f,0.5f,0.5f);
        else
            transform.localScale = new Vector3(0.7f,0.7f,0.7f);
    }
}
