using UnityEngine;
using System.Collections;

public class MainController : MonoBehaviour {

    public MapSpawner[] levels;
    public GameObject startScreen;

	// Use this for initialization
	void OnEnable() {
        InputStack.Push(this);
	}

    void OnDisable()
    {
        InputStack.Pop(this);
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
