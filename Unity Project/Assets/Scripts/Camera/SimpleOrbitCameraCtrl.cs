using UnityEngine;
using System.Collections;

public class SimpleOrbitCameraCtrl : CameraCtrl {

    public Transform traceTarget;
    public Vector3 lookAtOffset = Vector3.zero;

    public float minDistance = 2.0f;
    public float maxDistance = 10.0f;

    public float minCameraRotUp = -50.0f;
    public float maxCameraRotUp = 80.0f;

    public float moveDampingDuration = 0.1f;
    public float rotDampingDuration = 0.1f;
    public float zoomDampingDuration = 0.3f;

    float destDistance;
    float destCameraRotUp;
    float destCameraRotSide;

    float curDistance;
    Vector3 curLookAtPoint;

	void Start () {
        curLookAtPoint = GetLookAtPoint();
        transform.LookAt(curLookAtPoint);

        Quaternion destOriginRotation = transform.rotation;
        Vector3 angles = destOriginRotation.eulerAngles;
        destCameraRotSide = angles.y;
        destCameraRotUp = angles.x;

        destDistance = (traceTarget.position - transform.position).magnitude;
        destDistance = Mathf.Clamp( destDistance, minDistance, maxDistance );
        curDistance = destDistance;
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

        float zoomDelta = -1.0f * Input.GetAxis("Mouse ScrollWheel");
        if ( Mathf.Abs(zoomDelta) >= 0.01f ) {
            destDistance = Mathf.Pow( 2, zoomDelta * 0.2f) * destDistance;
        }
    }

    float curCameraRotUpVel = 0.0f;
    float curCameraRotSideVel = 0.0f;
    Vector3 curVel = Vector3.zero;
    float curZoomVel = 0.0f;

    void UpdateTransform () {
        // limit current distance
        destDistance = Mathf.Clamp( destDistance, minDistance, maxDistance );

        // limit current rotation
        destCameraRotUp = Mathf.Clamp( destCameraRotUp, minCameraRotUp, maxCameraRotUp );

        // damping camera
        Vector3 eulerAngles = transform.eulerAngles;
        if ( float.IsNaN(curCameraRotUpVel) ) curCameraRotUpVel = 0.0f;
        if ( float.IsNaN(curCameraRotSideVel) ) curCameraRotSideVel = 0.0f;

        float newCameraRotUp = Mathf.SmoothDampAngle(eulerAngles.x, destCameraRotUp, ref curCameraRotUpVel, rotDampingDuration);
        float newCameraRotSide = Mathf.SmoothDampAngle(eulerAngles.y, destCameraRotSide, ref curCameraRotSideVel, rotDampingDuration);
        transform.rotation = Quaternion.Euler(newCameraRotUp, newCameraRotSide, 0);

        curDistance = Mathf.SmoothDamp( curDistance, destDistance, ref curZoomVel, zoomDampingDuration );

        Vector3 lookAtPoint = GetLookAtPoint();
        curLookAtPoint = Vector3.SmoothDamp( curLookAtPoint, lookAtPoint, ref curVel, moveDampingDuration );
        transform.position = curLookAtPoint - transform.forward * curDistance;
    }

    public Vector3 GetLookAtPoint () {
        if ( traceTarget ) {
            return traceTarget.position + lookAtOffset;
        }
        return Vector3.zero;
    }

    public void MoveTo ( Vector3 _pos ) {
        // Vector3 lookAtPoint = GetLookAtPoint();
        // Vector3 delta = lookAtPoint - _pos;

        // //
        // Vector3 dir = delta;
        // dir.Normalize();

        // Quaternion quat = Quaternion.LookRotation(dir);
        // Vector3 eulerAngles = quat.eulerAngles;

        // Set ( eulerAngles.x, eulerAngles.y, delta.magnitude );
    }

    public void Set ( float _cameraRotUp, float _cameraRotSide, float _distance ) {
        // destDistance = _distance;
        // destDistance = Mathf.Clamp(destDistance, minDistance, maxDistance);

        // destCameraRotUp = _cameraRotUp;
        // destCameraRotUp = Mathf.Clamp(destCameraRotUp, minCameraRotUp, maxCameraRotUp);

        // destCameraRotSide = _cameraRotSide;
    }

    public void OnDrawGizmos () {
        if ( cam == null ) {
            return;
        }

        Quaternion destOriginRotation = Quaternion.Euler(destCameraRotUp, destCameraRotSide, 0);
        Vector3 lookAtPoint = GetLookAtPoint();
        Vector3 destPos = lookAtPoint - destOriginRotation * Vector3.forward * destDistance;

        // current camera pos
        Gizmos.color = new Color( 1.0f, 1.0f, 0.0f, 0.8f );
        Gizmos.DrawSphere ( transform.position, 0.2f );

        // current camera lookat line
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine ( transform.position, lookAtPoint );

        // dest camera pos
        Gizmos.color = new Color( 1.0f, 1.0f, 1.0f, 0.5f );
        Gizmos.DrawSphere ( destPos, 0.2f );

        // dest camera lookat line
        Gizmos.color = Color.gray;
        Gizmos.DrawLine ( destPos, lookAtPoint );

        // orgin coord
        Gizmos.color = Color.green;
        Gizmos.DrawLine ( lookAtPoint, lookAtPoint + destOriginRotation * Vector3.up * 0.5f );
        Gizmos.color = Color.red;
        Gizmos.DrawLine ( lookAtPoint, lookAtPoint + destOriginRotation * Vector3.right * 0.5f );
        Gizmos.color = Color.blue;
        Gizmos.DrawLine ( lookAtPoint, lookAtPoint + destOriginRotation * Vector3.forward * 0.5f );
    }
}
