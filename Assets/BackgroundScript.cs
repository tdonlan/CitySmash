using UnityEngine;
using System.Collections;

public class BackgroundScript : MonoBehaviour {
    private Camera mainCamera;
    private Vector3 startPos;

	// Use this for initialization
	void Start () {
        mainCamera = GameObject.FindObjectOfType<Camera>().GetComponent<Camera>();
        startPos = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {

        var offset = Vector3.zero - mainCamera.transform.position;
        offset *= .1f;
        this.transform.position = startPos + new Vector3(offset.x,offset.y*.5f,0);

      
	}
}
