using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public static GameController instance;

	public int roadCount =2;

	public GameObject startButton;

	public List<Road> roads;

	SongPlayer songPlayer;

	void Start () {
		instance = this;
		songPlayer = GetComponent<SongPlayer> ();
	}
	
	void Update () {
		if (Input.GetKeyDown (KeyCode.F)) {
			//			BounceSpring ();
			roads[0].Player.GetComponent<Player>().jump();
		}

		if (Input.GetKeyDown (KeyCode.J)) {
			//			BounceSpring ();
			roads[1].Player.GetComponent<Player>().jump();
		}
	}

	void initRoad(){

		roads = new List<Road> ();
		for (int i = 0; i < roadCount; i++) {
			var roadGO = new GameObject ("road" + i);
			var road = roadGO.AddComponent<Road> ();
			var playerGO = GameObject.CreatePrimitive (PrimitiveType.Cube);
			playerGO.AddComponent<Player> ();
			playerGO.transform.parent = road.transform;
			playerGO.transform.localPosition = new Vector3 (0, 3f, 0);
			road.Player = playerGO.transform;
			road.stringIndex = i + 1;
			road.StartPlay ();
			road.transform.position = new Vector3 (-3 + i * 6, 0f, 0f);
			roads.Add (road);
		}

	}

	public void StartPlay(){
		startButton.SetActive (false);
		songPlayer.SetSong (songPlayer.Song);

		initRoad ();

		songPlayer.Play ();
	}

	public void StopPlay(){
		foreach (var item in roads) {
			Destroy (item.gameObject);
		}
		startButton.SetActive (true);
	}
}
