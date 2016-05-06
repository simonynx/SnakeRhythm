﻿using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public static Player instance;

	public float speed = 3.0F;
	public float jumpSpeed = 8.0F;
	public float jumpForward = 3f;
	public float gravity = 20.0F;
	private Vector3 moveDirection = Vector3.zero;

//	Rigidbody rigidbody;
//	Vector3 velocity;

	bool isJump = false;
	CharacterController controller;
	
	void Start () {
		instance = this;
//		rigidbody = GetComponent<Rigidbody> ();
		controller = GetComponent<CharacterController>();

		moveDirection = Vector3.forward * speed;

//		InvokeRepeating ("jump", 0f, 1f);
	}

	void Update () {
//		velocity = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical")).normalized * 10;
		moveDirection.y -= gravity * Time.deltaTime;
		controller.Move(moveDirection * Time.deltaTime);
		if (isJump && controller.isGrounded) {
			moveDirection = Vector3.forward * speed;
			isJump = false;
		}
	}

	void FixedUpdate() {
//		rigidbody.MovePosition (rigidbody.position + velocity * Time.fixedDeltaTime);
	}

	public void jump(){
		if (controller.isGrounded) {
			if (isJump)
				return;
			isJump = true;
			moveDirection.z += jumpForward;
			moveDirection.y = jumpSpeed;
		}
	}
}