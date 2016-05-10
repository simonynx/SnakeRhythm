using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Road : MonoBehaviour
{
	public Transform Player;
	public float speed = 1f;
	public int stringIndex = 0;
	public float gridHeight = 2;
	public float roadViewRectHeight = 40;

	List<GridCtrl> roadGrids;
	exGameObjectPool gridPool;
	Dictionary<float, GridCtrl> grids;

	protected SongPlayer songPlayer;

	void Awake ()
	{
		gridPool = new exGameObjectPool ();
		gridPool.prefab = Resources.Load ("Grid") as GameObject;
		gridPool.size = 40;
		gridPool.Init ();
		songPlayer = SongPlayer.instance;
	}

	void initGrids(){
		roadGrids = new List<GridCtrl> ();
		grids = new Dictionary<float, GridCtrl> ();

		float viewBeats = 5;
		float numNotesOfOneBeat = 4;

		float beatOffSet = songPlayer.GetCurrentBeat ();
	
		var index = 0;
		var i = beatOffSet;
		while (i < songPlayer.GetCurrentBeat () + viewBeats + 0.5f) {
			var grid = gridPool.Request<GridCtrl> ();
			grid.type = GridType.Normal;
			grid.height = gridHeight;
			grid.BeatTime = i;
			var y = index * gridHeight;
			grid.transform.parent = transform;
			grid.transform.localPosition = new Vector3 (0f, 0f, y);
			roadGrids.Add (grid);

			index++;
			i += 1 / numNotesOfOneBeat;
		}

		for (int j = 0; j < songPlayer.Song.Notes.Count; j++) {
			var note = songPlayer.Song.Notes [j];
			if (note.StringIndex != stringIndex) {
				continue;
			}
			
			if (note.Time - songPlayer.GetCurrentBeat () > viewBeats) {
				continue;
			}
			
			if (note.Time - songPlayer.GetCurrentBeat () <  0.5) {
				continue;
			}
			
			var process = (note.Time - songPlayer.GetCurrentBeat ()) / viewBeats;
			var grid = gridPool.Request<GridCtrl> ();
			grid.type = GridType.Spring;
			grid.height = gridHeight;
			var y = process * roadViewRectHeight;
			grid.transform.parent = transform;
			grid.transform.localPosition = new Vector3 (0f, 0f, y);
			grids.Add (note.Time, grid);
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
			if (note.StringIndex != stringIndex) {
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
				grid.transform.parent = transform;
				grid.transform.localPosition = new Vector3 (0f, 1f, y);
				grids.Add (note.Time, grid);
			}
		}

		var removeList = new List<GridCtrl> ();
		foreach (var grid in grids.Values) {
			if (grid == null)
				continue;
			var process = (grid.BeatTime - songPlayer.GetCurrentBeat ()) / 5;
			var y = process * roadViewRectHeight;
			grid.transform.localPosition = new Vector3 (0f, 1f, y);
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
			var prePos = roadGrid.transform.localPosition;
			prePos.z = process * roadViewRectHeight;
			roadGrid.transform.localPosition = prePos;

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
			var y = last.transform.localPosition.z + gridHeight;
			grid.transform.parent = transform;
			grid.transform.localPosition = new Vector3 (0f, 0f, y);
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
		initGrids ();

		Player.gameObject.SetActive (true);
	}

	void OnDestroy(){
		for (int i = 0; i < gridPool.data.Length; i++) {
			Destroy (gridPool.data [i]);
		}
	}


}
