using UnityEngine;
using System.Collections;

public class MainController : MonoBehaviour {

    public MapSpawner mapSpawner;
    public TextAsset[] levelMapSrcs;
    public GameObject startScreen;

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
                mapSpawner.Spawn(levelMapSrcs[0].text);
            }
        }
	
	}
}
