using UnityEngine;
using System.Collections;

public class Fruit : MonoBehaviour {

    public Vector3 scaleVel = Vector3.zero;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        transform.localScale = Vector3.SmoothDamp( transform.localScale,
                new Vector3(0.5f, 0.5f, 0.5f), ref scaleVel, 0.2f );
	
	}
}
