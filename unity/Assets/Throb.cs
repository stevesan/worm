using UnityEngine;
using System.Collections;

public class Throb : MonoBehaviour {

    public float freq;
    public float amp;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        transform.localScale = new Vector3(
                1 + amp*Mathf.Sin( 2*Mathf.PI*freq * Time.time ),
                1 + amp*Mathf.Sin( 2*Mathf.PI*freq * Time.time ),
                1f );
	}
}
