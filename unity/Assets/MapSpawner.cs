using UnityEngine;
using System.Collections.Generic;
using SteveSharp;

public class MapSpawner : MonoBehaviour
{
    [System.Serializable]
    public class EntityMapping
    {
        public string character;
        public GameObject prefab;
    }
    public List<EntityMapping> entMappings;

    GameObject[,] grid;

    Dictionary<string, GameObject> entMappingsFast;

    void Awake()
    {
        entMappingsFast = new Dictionary<string, GameObject>();
        foreach( var mapping in entMappings )
        {
            entMappingsFast[mapping.character] = mapping.prefab;
        }
    }

    public void Spawn(string src)
    {
        var root = new GameObject("map root");
        root.transform.parent = transform;
        root.transform.IdentityLocals();

        int row = 0;
        int col = 0;
        var spawned = new List<GameObject>();
        foreach( string line in src.Split(new char[]{'\n'}) )
        {
            col = 0;

            foreach( char c in line )
            {
                string cs = ""+c;
                if( entMappingsFast.ContainsKey(cs) )
                {
                    var prefab = entMappingsFast[cs];
                    var inst = (GameObject)GameObject.Instantiate(prefab);
                    inst.transform.parent = root.transform;
                    inst.transform.IdentityLocals();
                    inst.transform.localPosition = new Vector3(col, -row, 0);
                    spawned.Add(inst);
                }
                else
                    spawned.Add(null);

                col += 1;
            }

            row += 1;
        }

        int numRows = row;
        int numCols = col;
        int spawnedId = 0;
        grid = new GameObject[numRows, numCols];

        for( int r = 0; r < numRows; r++ )
        for( int c = 0; c < numCols; c++ )
            grid[r,c] = spawned[spawnedId++];
    }
}
