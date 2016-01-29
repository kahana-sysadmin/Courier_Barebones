using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	Experiment exp { get { return Experiment.Instance; } }

	public PlayerControls controls;
	public GameObject visuals;

	ObjectLogTrack objLogTrack;
	

	// Use this for initialization
	void Start () {
		objLogTrack = GetComponent<ObjectLogTrack> ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void TurnOnVisuals(bool isVisible){
		visuals.SetActive (isVisible);
	}

	GameObject waitForCollisionObject;
	bool isLookingForObject = false;
	public IEnumerator WaitForObjectCollision(string objectName){
		isLookingForObject = true;
		Debug.Log("WAITING FOR COLLISION WITH: " + objectName);
		
		string lastCollisionName = "";
		while (lastCollisionName != objectName) {
			if(waitForCollisionObject != null){
				lastCollisionName = waitForCollisionObject.name;
			}
			yield return 0;
		}

		Debug.Log ("FOUND BUILDING");
		
		isLookingForObject = false;

	}

	public GameObject GetCollisionObject(){
		return waitForCollisionObject;
	}

	void OnCollisionEnter(Collision collision){
		waitForCollisionObject = collision.gameObject;

		//log building collision
		if (collision.gameObject.tag == "Building"){
			objLogTrack.LogCollision (collision.gameObject.name);
		}
		
	}


}
