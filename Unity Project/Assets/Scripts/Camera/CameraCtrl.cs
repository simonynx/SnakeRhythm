using UnityEngine;
using System.Collections;

public class CameraCtrl : MonoBehaviour {
    public bool debug = false;
    public Camera cam;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
	}

    public virtual void Yaw ( float _delta ) {
    }

    public virtual void Roll ( float _delta ) {
    }

    public virtual void Zoom ( float _delta ) {
    }
}
