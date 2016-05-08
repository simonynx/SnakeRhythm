using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Road : MonoBehaviour
{
	public static Road instance;

	public Transform Player;
	public GameObject startButton;

	public float speed = 1f;

	public float roadViewRectHeight = 40;

	public float gridHeight = 2;

	Dictionary<float, GridCtrl> grids;
	List<GridCtrl> roadGrids;

	public exGameObjectPool gridPool;

	SongPlayer songPlayer;

	// Use this for initialization
	void Start ()
	{
		instance = this;
		gridPool.Init ();
		songPlayer = GetComponent<SongPlayer> ();
	}

	void initGrids(){
		roadGrids = new List<GridCtrl> ();
		grids = new Dictionary<float, GridCtrl> ();

		float heightOfOneBeat = 5;
		float numNotesOfOneBeat = 4;

		float beatOffSet = songPlayer.GetCurrentBeat ();
	
		var index = 0;
		var i = beatOffSet;
		while (i < songPlayer.GetCurrentBeat () + heightOfOneBeat + 0.5f) {
			var grid = gridPool.Request<GridCtrl> ();
			grid.type = GridType.Normal;
			grid.height = gridHeight;
			grid.BeatTime = i;
			var y = index * gridHeight;
			grid.transform.position = new Vector3 (0f, 0f, y);
			roadGrids.Add (grid);

			index++;
			i += 1 / numNotesOfOneBeat;
		}

		for (int j = 0; j < songPlayer.Song.Notes.Count; j++) {
			var note = songPlayer.Song.Notes [j];
			if (note.StringIndex != 1) {
				continue;
			}
			
			if (note.Time - songPlayer.GetCurrentBeat () > heightOfOneBeat) {
				continue;
			}
			
			if (note.Time - songPlayer.GetCurrentBeat () <  0.5) {
				continue;
			}
			
			var process = (note.Time - songPlayer.GetCurrentBeat ()) / heightOfOneBeat;
			var grid = gridPool.Request<GridCtrl> ();
			grid.type = GridType.Spring;
			grid.height = gridHeight;
			var y = process * roadViewRectHeight;
			grid.transform.position = new Vector3 (0f, 0f, y);
			grids.Add (note.Time, grid);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButton (0)) {
//			BounceSpring ();
//			Player.instance.jump ();
		}
	}

	void FixedUpdate(){
		if (!songPlayer.IsPlaying ())
			return;
		updateRoadNotePosition ();
		updateNotePosition ();
	}

	void updateNotePosition(){

		for (int j = 0; j < songPlayer.Song.Notes.Count; j++) {
			var note = songPlayer.Song.Notes [j];
			if (note.StringIndex != 1) {
				continue;
			}

			if (note.Time - songPlayer.GetCurrentBeat () > 5) {
				continue;
			}

			if (note.Time - songPlayer.GetCurrentBeat () <  0.5) {
				continue;
			}
			if (!grids.ContainsKey (note.Time)) {
				var process = (note.Time - songPlayer.GetCurrentBeat ()) / 5;
				var grid = gridPool.Request<GridCtrl> ();
				grid.type = GridType.Spring;
				grid.BeatTime = note.Time;
				grid.height = gridHeight;
				var y = process * roadViewRectHeight;
				grid.transform.position = new Vector3 (0f, 1f, y);
				grids.Add (note.Time, grid);
			}
		}

		var removeList = new List<GridCtrl> ();
		foreach (var grid in grids.Values) {
			if (grid == null)
				continue;
			var process = (grid.BeatTime - songPlayer.GetCurrentBeat ()) / 5;
			var y = process * roadViewRectHeight;
			grid.transform.position = new Vector3 (0f, 1f, y);
			if (grid.BeatTime - songPlayer.GetCurrentBeat () <  -2) {
				removeList.Add (grid);
			}
		}

		foreach (var item in removeList) {
			grids.Remove (item.BeatTime);
			gridPool.Return<GridCtrl> (item);
		}
		removeList.Clear ();
	}

	void updateRoadNotePosition(){
		var needCreateNew = true;
		for (var i = 0; i < roadGrids.Count; i++) {
			var roadGrid = roadGrids [i];
			if (roadGrid.BeatTime - songPlayer.GetCurrentBeat () > 5 + 0.5) {
				needCreateNew = false;
			}

			var process = (roadGrid.BeatTime - songPlayer.GetCurrentBeat ()) / 5;
			var prePos = roadGrid.transform.position;
			prePos.z = process * roadViewRectHeight;
			roadGrid.transform.position = prePos;

			if (roadGrid.BeatTime - songPlayer.GetCurrentBeat () < -1.5f) {
				gridPool.Return<GridCtrl> (roadGrid);
				roadGrids.Remove (roadGrid);
				i--;
			}
		}
		if (needCreateNew) {
			var last = roadGrids [roadGrids.Count - 1];
			var grid = gridPool.Request<GridCtrl> ();
			grid.type = GridType.Normal;
			grid.height = gridHeight;
			grid.BeatTime = last.BeatTime + (float)1/4;
			var y = last.transform.position.z + gridHeight;
			grid.transform.position = new Vector3 (0f, 0f, y);
			roadGrids.Add (grid);
		}
	}

	void BounceSpring(){
		for (int i = 0; i < grids.Count; i++) {
			if (grids [i].type == GridType.Spring) {
				grids [i].Bounce ();
			}
		}
	}

	public void ResetCanBounceStatus(){
		for (int i = 0; i < grids.Count; i++) {
			if (grids [i].type == GridType.Spring) {
				grids [i].isCanBounce = false;
			}
		}
	}

	public void StartPlay(){
		songPlayer.SetSong (songPlayer.Song);
		initGrids ();

		Player.gameObject.SetActive (true);
		startButton.SetActive (false);

		songPlayer.Play ();
	}
}
