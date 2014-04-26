using UnityEngine;
using System.Collections;

public class MainController : MonoBehaviour {

    public MapSpawner[] levels;
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
            }
        }
	
	}
}
