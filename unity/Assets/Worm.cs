using UnityEngine;
using System.Collections.Generic;
using SteveSharp;

public class Worm : MonoBehaviour
{
    public GridEntity ent;
    public GameObject segmentPrefab;

    public bool isHead = false;

    void Awake()
    {
        ent = GetComponent<GridEntity>();
    }

    void Update()
    {
        if( isHead )
            transform.localScale = new Vector3(1,1,1);
        else
            transform.localScale = new Vector3(0.5f,0.5f,0.5f);
    }
}
