using UnityEngine;
using System.Collections.Generic;
using SteveSharp;

public class MapSpawner : MonoBehaviour
{
    public TextAsset mapsrc;
    public bool autoSpawn = false;

    [System.Serializable]
    public class EntityMapping
    {
        public string character;
        public GameObject prefab;
    }
    public List<EntityMapping> entMappings;


    Dictionary<string, GameObject> entMappingsFast;

    void Awake()
    {
        entMappingsFast = new Dictionary<string, GameObject>();
        foreach( var mapping in entMappings )
        {
            entMappingsFast[mapping.character] = mapping.prefab;
        }
    }

    void Start()
    {
        if( autoSpawn )
            Spawn();
    }

    public void Spawn()
    {
        var root = new GameObject("map root");
        root.transform.parent = transform;
        root.transform.IdentityLocals();

        float x = 0;
        float y = 0;
        foreach( string line in mapsrc.text.Split(new char[]{'\n'}) )
        {
            x = 0;

            foreach( char c in line )
            {
                string cs = ""+c;
                if( entMappingsFast.ContainsKey(cs) )
                {
                    var prefab = entMappingsFast[cs];
                    var inst = (GameObject)GameObject.Instantiate(prefab);
                    inst.transform.parent = root.transform;
                    inst.transform.localPosition = new Vector3(x, y, 0);
                }

                x += 1;
            }

            y -= 1;
        }
    }
}
