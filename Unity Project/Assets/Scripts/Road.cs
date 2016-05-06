using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Road : MonoBehaviour
{
	public float speed = 1f;

	List<GridCtrl> grids;

	public exGameObjectPool gridPool;

	float currentTime;

	// Use this for initialization
	void Start ()
	{
		gridPool.Init ();
		initGrids ();
		currentTime = Time.time + 3;
	}

	void initGrids(){
		grids = new List<GridCtrl> ();
		for (int i = 0; i < 20; i++) {
			var type = (GridType)Random.Range (0, 3);
			var grid = gridPool.Request<GridCtrl> ();
			grid.type = type;
			grid.transform.position = new Vector3 (0f, 0f, grid.height * i);
			grids.Add (grid);
		}

		resetGrids ();
	}

	void resetGrids(){
		for (int i = 1; i < grids.Count; i++) {
			var pre = grids [i - 1];
			if (pre.type == GridType.Spring) {
				grids [i].type = GridType.Trap;
			}
			if (pre.type == GridType.Trap || pre.type == GridType.Normal) {
				grids [i].type = Random.Range (0, 50) > 25 ? GridType.Normal : GridType.Spring;
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButton (0)) {
			BounceSpring ();
			Player.instance.jump ();
		}
	}

	void FixedUpdate(){
		if (Time.time - currentTime > speed) {
			currentTime = Time.time;

			var first = grids [0];
			if (first.isBounce) {
				Debug.Log ("Bounce Grid pos:" + first.transform.position);
			}
			first.isBounce = false;
			first.transform.position = Vector3.zero;
			gridPool.Return<GridCtrl> (first);
			grids.RemoveAt (0);

			var last = grids [grids.Count - 1];
			var pushNew = gridPool.Request<GridCtrl> ();
			if (last.type == GridType.Spring) {
				pushNew.type = GridType.Trap;
			}
			if (last.type == GridType.Trap || last.type == GridType.Normal) {
				pushNew.type = Random.Range (0, 50) > 25 ? GridType.Normal : GridType.Spring;
			}

			var lastPos = last.transform.position;
			lastPos.x = 0;
			lastPos.y = 0;
			pushNew.transform.position = lastPos + new Vector3(0f, 0f,pushNew.height);
			grids.Add (pushNew);
		}
	}

	void BounceSpring(){
		for (int i = 0; i < grids.Count; i++) {
			if (grids [i].type == GridType.Spring) {
				grids [i].Bounce ();
				return;
			}
		}
	}
}
