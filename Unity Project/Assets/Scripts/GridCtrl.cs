using UnityEngine;
using System.Collections;

public enum GridType
{
	Trap,
	Normal,
	Spring,
}

public class GridCtrl : MonoBehaviour
{
	public float width = 2.5f;
	public float height = 1.5f;

	GridType type_;
	public GridType type { 
		get{ return type_; } 
		set { 
			type_ = value;
			CommitData ();
		}
	}

	public float bounceHeight = 2f;

    public bool isCanBounce;

	public bool isBounce { get; set;}
	Vector3 originPos;
	Vector3 bouncePeakPos;
	float beginBounceTime;
	float bounceUpTime = 0.25f;
	float bounceDownTime = 0.25f;

	void Start ()
	{
		CommitData ();
	}

	void CommitData(){
		switch (type) {
		case GridType.Normal:
			GetComponent<MeshRenderer> ().material.color = Color.white;
			break;
		case GridType.Trap:
			GetComponent<MeshRenderer> ().material.color = Color.blue;
			break;
		case GridType.Spring:
			GetComponent<MeshRenderer> ().material.color = Color.green;
			break;
		default:
			break;
		}
		transform.localScale = new Vector3(width, 4f, height);
	}
	
	void Update ()
	{
		if (isBounce) {
			if (Time.time - beginBounceTime < bounceUpTime) {
				transform.position = Vector3.Lerp (transform.position, bouncePeakPos, Time.deltaTime * 3f);
			} else if (Time.time - beginBounceTime < bounceUpTime + bounceDownTime) {
				transform.position = Vector3.Lerp (transform.position, originPos, Time.deltaTime * 3f);
			} else {
				transform.position = originPos;
				isBounce = false;
				Debug.Log ("Bounce Complete! pos:"+originPos+",Time:" + Time.time);
			}
		}
	}

	public void Bounce(){
		if (type != GridType.Spring)
			return;

		if (isBounce || !isCanBounce)
			return;
		originPos = transform.position;
		bouncePeakPos = originPos + Vector3.up * bounceHeight;
		beginBounceTime = Time.time;
		isBounce = true;
	}

    public void Reset() {
        isBounce = false;
        isCanBounce = false;
        transform.position = Vector3.zero;
    }
}

