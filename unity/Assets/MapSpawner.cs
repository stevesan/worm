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

    public GridEntity[,] grid { get; private set; }

    public GameObject entsRoot = null;

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
        entsRoot = new GameObject("map root");
        entsRoot.transform.parent = transform;
        entsRoot.transform.IdentityLocals();

        int numRows = 0;
        int numCols = 0;
        var spawned = new List<GridEntity>();
        foreach( string line in src.Split(new char[]{'\n'}) )
        {
            int cols = 0;

            foreach( char c in line )
            {
                string cs = ""+c;
                if( entMappingsFast.ContainsKey(cs) )
                {
                    var prefab = entMappingsFast[cs];

                    if( !prefab.GetComponent<GridEntity>() )
                        Debug.LogError("No GridEntity component on entity prefab "+prefab.name);
                    else
                    {
                        var inst = (GameObject)GameObject.Instantiate(prefab);
                        inst.transform.parent = entsRoot.transform;
                        spawned.Add(inst.GetComponent<GridEntity>());
                    }
                }
                else
                    spawned.Add(null);

                cols += 1;
            }

            if( cols > 0 )
            {
                numCols = Mathf.Max( cols, numCols );
                numRows += 1;
            }
        }

        Debug.Log("size = "+numRows+", "+numCols);

        int spawnedId = 0;
        grid = new GridEntity[numRows, numCols];

        for( int r = 0; r < numRows; r++ )
        for( int c = 0; c < numCols; c++ )
        {
            grid[r,c] = spawned[spawnedId++];
            if( grid[r,c] != null )
            {
                grid[r,c].host = this;
                //Debug.Log("setting "+grid[r,c].gameObject.name+" to "+r+","+c);
                grid[r,c].row = r;
                grid[r,c].col = c;
            }
        }
    }
}
