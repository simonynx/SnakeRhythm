using UnityEngine;
using System.Collections;

public class GameCameraCtrl : CameraCtrl {

    public static GameCameraCtrl instance = null;

    public Transform traceTarget;
    public Vector3 offset = Vector3.zero;
    
    public float minDistance = 2.0f;
    public float maxDistance = 10.0f;
    
    public float moveDampingDuration = 0.1f;
    public float rotDampingDuration = 0.1f;
    public float zoomDampingDuration = 0.3f;
    
    public bool useNightVision = false;
    
    float destDistance;

    void Awake() {
        instance = this;
    }
    
    void Start () {
        if (!traceTarget) enabled = false;
    }

    void OnEnabled()
    {
        if (!traceTarget) enabled = false;
        cam.transform.localRotation = Quaternion.identity;

        destDistance = (traceTarget.transform.position - cam.transform.position).magnitude;
        destDistance = Mathf.Clamp(destDistance, minDistance, maxDistance);
    }
    
    void Update () {
        if ( debug ) {
            DebugInput ();
        }
    }
    
    void LateUpdate () {
        UpdateTransform ();
    }
    
    void DebugInput () {
        // float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        // if ( Mathf.Abs(zoomDelta) >= 0.01f ) {
        //     destDistance *= (1.0f - zoomDelta);
        // }
        
        float zoomDelta = -1.0f * Input.GetAxis("Mouse ScrollWheel");
        if ( Mathf.Abs(zoomDelta) >= 0.01f ) {
            destDistance = Mathf.Pow( 2, zoomDelta * 0.2f) * destDistance;
        }
    }
    
    public override void Zoom ( float _delta ) {
        destDistance = Mathf.Pow( 2, _delta * 0.2f) * destDistance;
    }
    
    //
    Vector3 curVel = Vector3.zero;
    float curZoomVel = 0.0f;
    
    void UpdateTransform () {
        // limit current distance
        destDistance = Mathf.Clamp(destDistance, minDistance, maxDistance);
        
        Vector3 lookAtPoint = GetLookAtPoint();
        transform.position = Vector3.SmoothDamp( transform.position, lookAtPoint, ref curVel, moveDampingDuration );
        
        float dist = Mathf.SmoothDamp( -cam.transform.localPosition.z, destDistance, ref curZoomVel, zoomDampingDuration );
        cam.transform.localPosition = -Vector3.forward * dist;
    }
    
    public Vector3 GetLookAtPoint () {
        if ( traceTarget ) {
            return traceTarget.position + offset;
        }
        return transform.position;
    }
    
    public void MoveTo ( Vector3 _pos ) {
        Vector3 lookAtPoint = GetLookAtPoint();
        Vector3 delta = lookAtPoint - _pos;
        
        Set ( delta.magnitude );
    }
    
    public void Set ( float _distance ) {
        destDistance = _distance;
        destDistance = Mathf.Clamp(destDistance, minDistance, maxDistance);
    }
    
    public void OnDrawGizmos () {
        if ( cam == null ) {
            return;
        }
        
        Vector3 lookAtPoint = GetLookAtPoint();

        // current camera pos
        Gizmos.color = new Color( 1.0f, 1.0f, 0.0f, 0.8f );
        Gizmos.DrawSphere ( cam.transform.position, 0.2f );
        
        // current camera lookat line
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine ( cam.transform.position, lookAtPoint );
        
        // lookat point
        Gizmos.color = new Color( 1.0f, 0.0f, 0.0f, 0.8f );
        Gizmos.DrawSphere ( lookAtPoint, 0.2f );
    }
}
