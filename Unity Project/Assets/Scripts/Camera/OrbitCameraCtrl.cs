using UnityEngine;
using System.Collections;

public class OrbitCameraCtrl : CameraCtrl {

    public Transform traceTarget;
    public Vector3 offset = Vector3.zero;

    public float minDistance = 2.0f;
    public float maxDistance = 10.0f;

    public float minCameraRotUp = -50.0f;
    public float maxCameraRotUp = 80.0f;

    public float moveDampingDuration = 0.1f;
    public float rotDampingDuration = 0.1f;
    public float zoomDampingDuration = 0.3f;

	[Header("与人物相距距离")]
    public float destDistance = 10f;
	[Header("上下偏移角度")]
	public float destCameraRotUp = 40f;
	[Header("左右偏移角度")]
	public float destCameraRotSide = 0f;

	void Start () {
        Vector3 lookAtPoint = GetLookAtPoint();

        transform.rotation = Quaternion.LookRotation(lookAtPoint - cam.transform.position);
        cam.transform.localRotation = Quaternion.identity;

//        destDistance = (traceTarget.transform.position - cam.transform.position).magnitude;
//        destDistance = Mathf.Clamp(destDistance, minDistance, maxDistance);

//        destCameraRotUp = transform.eulerAngles.x;
//        destCameraRotUp = Mathf.Clamp(destCameraRotUp, minCameraRotUp, maxCameraRotUp);

//        destCameraRotSide = transform.eulerAngles.y;
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
        if (Input.GetMouseButton(1)) {
            destCameraRotSide += Input.GetAxis("Mouse X")*5;
            destCameraRotUp -= Input.GetAxis("Mouse Y")*5;
        }

        // float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        // if ( Mathf.Abs(zoomDelta) >= 0.01f ) {
        //     destDistance *= (1.0f - zoomDelta);
        // }

        float zoomDelta = -1.0f * Input.GetAxis("Mouse ScrollWheel");
        if ( Mathf.Abs(zoomDelta) >= 0.01f ) {
            destDistance = Mathf.Pow( 2, zoomDelta * 0.2f) * destDistance;
        }
    }

    public override void Yaw ( float _delta ) {
        destCameraRotSide += _delta;
    }

    public override void Roll ( float _delta ) {
        destCameraRotUp += _delta;
    }

    public override void Zoom ( float _delta ) {
        destDistance = Mathf.Pow( 2, _delta * 0.2f) * destDistance;
    }

    //
    float curCameraRotUpVel = 0.0f;
    float curCameraRotSideVel = 0.0f;
    Vector3 curVel = Vector3.zero;
    float curZoomVel = 0.0f;

    void UpdateTransform () {
        // limit current distance
        destDistance = Mathf.Clamp(destDistance, minDistance, maxDistance);

        // limit current rotation
        destCameraRotUp = Mathf.Clamp(destCameraRotUp, minCameraRotUp, maxCameraRotUp);

        Vector3 eulerAngles = transform.eulerAngles;
        if ( float.IsNaN(curCameraRotUpVel) ) curCameraRotUpVel = 0.0f;
        if ( float.IsNaN(curCameraRotSideVel) ) curCameraRotSideVel = 0.0f;

        float newCameraRotUp = Mathf.SmoothDampAngle(eulerAngles.x, destCameraRotUp, ref curCameraRotUpVel, rotDampingDuration);
        float newCameraRotSide = Mathf.SmoothDampAngle(eulerAngles.y, destCameraRotSide, ref curCameraRotSideVel, rotDampingDuration);
        transform.rotation = Quaternion.Euler(newCameraRotUp, newCameraRotSide, 0);

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

        //
        Vector3 dir = delta;
        dir.Normalize();

        Quaternion quat = Quaternion.LookRotation(dir);
        Vector3 eulerAngles = quat.eulerAngles;

        Set ( eulerAngles.x, eulerAngles.y, delta.magnitude );
    }

    public void Set ( float _cameraRotUp, float _cameraRotSide, float _distance ) {
        destDistance = _distance;
        destDistance = Mathf.Clamp(destDistance, minDistance, maxDistance);

        destCameraRotUp = _cameraRotUp;
        destCameraRotUp = Mathf.Clamp(destCameraRotUp, minCameraRotUp, maxCameraRotUp);

        destCameraRotSide = _cameraRotSide;
    }

    public void OnDrawGizmos () {
        if ( cam == null ) {
            return;
        }

        Vector3 lookAtPoint = GetLookAtPoint();
        Quaternion destOriginRotation = Quaternion.Euler(destCameraRotUp, destCameraRotSide, 0);
        Vector3 destPos = lookAtPoint - destOriginRotation * Vector3.forward * destDistance;

        // current camera pos
        Gizmos.color = new Color( 1.0f, 1.0f, 0.0f, 0.8f );
        Gizmos.DrawSphere ( cam.transform.position, 0.2f );

        // current camera lookat line
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine ( cam.transform.position, lookAtPoint );

        // lookat point
        Gizmos.color = new Color( 1.0f, 0.0f, 0.0f, 0.8f );
        Gizmos.DrawSphere ( lookAtPoint, 0.2f );

        // dest camera pos
        Gizmos.color = new Color( 1.0f, 1.0f, 1.0f, 0.5f );
        Gizmos.DrawSphere ( destPos, 0.2f );

        // dest camera lookat line
        Gizmos.color = Color.gray;
        Gizmos.DrawLine ( destPos, lookAtPoint );
    }
}
