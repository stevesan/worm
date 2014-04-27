using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {

    public Vector3 vel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        transform.position += Time.deltaTime * vel;
	
	}
}
