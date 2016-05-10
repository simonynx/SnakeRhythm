using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public float speed = 3.0F;
    public float jumpSpeed = 8.0F;
    public float jumpForward = 4f;
    public float gravity = 20.0F;

	public float jumpHeight = 2f;

    private Vector3 moveDirection = Vector3.zero;

    //	Rigidbody rigidbody;
    //	Vector3 velocity;

    bool isJump = false;
    CharacterController controller;

	Vector3 originPos;
	Vector3 jumpPeakPos;
	float beginJumpTime;
	float jumpUpTime = 0.15f;
	float jumpDownTime = 0.15f;

    void Start() {
        //		rigidbody = GetComponent<Rigidbody> ();
        controller = GetComponent<CharacterController>();

        moveDirection = Vector3.forward * speed;

        //		InvokeRepeating ("jump", 0f, 1f);
    }

    void Update() {
        //		velocity = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical")).normalized * 10;
//        moveDirection.y -= gravity * Time.deltaTime;
//        controller.Move(moveDirection * Time.deltaTime);
//        if (isJump && controller.isGrounded) {
//            moveDirection = Vector3.forward * speed;
//            isJump = false;
//        }
//
//        var ray = new Ray(transform.position, transform.up * -1);
//        RaycastHit hitInfo;
//        if(Physics.Raycast(ray, out hitInfo, 1f)) {
//            var grid = hitInfo.transform.GetComponent<GridCtrl>();
//            if(grid != null) {
//				Road.instance.ResetCanBounceStatus ();
//                grid.isCanBounce = true;
//            }
//        }

		if (isJump) {
			if (Time.time - beginJumpTime < jumpUpTime) {
				transform.position = Vector3.Lerp (transform.position, jumpPeakPos, Time.deltaTime * 5f);
			} else if (Time.time - beginJumpTime < jumpUpTime + jumpDownTime) {
				transform.position = Vector3.Lerp (transform.position, originPos, Time.deltaTime * 5f);
			} else {
				transform.position = originPos;
				isJump = false;
				Debug.Log ("Bounce Complete! pos:"+originPos+",Time:" + Time.time);
			}
		}
    }

    void FixedUpdate() {
        //		rigidbody.MovePosition (rigidbody.position + velocity * Time.fixedDeltaTime);
    }

    public void jump() {
//        if (controller.isGrounded) {
            if (isJump)
                return;
            isJump = true;
//            moveDirection.z += jumpForward;
//            moveDirection.y = jumpSpeed;

			beginJumpTime = Time.time;
			originPos = transform.position;
			jumpPeakPos = originPos + Vector3.up * jumpHeight;
//        }
    }
}
