using UnityEngine;
using System.Collections;

public class SkyController : MonoBehaviour {
    
        private Camera mainCamera;

	// Use this for initialization
	void Start () {
        mainCamera = GameObject.FindObjectOfType<Camera>().GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        

        this.transform.position = Vector3.zero - mainCamera.transform.position * .3f;
	}
}
