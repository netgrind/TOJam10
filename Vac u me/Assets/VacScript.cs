﻿using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class VacScript : MonoBehaviour {

	public string suckInput;
	public string blowInput;
	public string horiAxis;
	public string vertAxis;

	public float suctionDistance = 1;
	public float suctionPower = 1;

	public float collectDistance = .5f;
	public float moveDistancePerSecond = 10;
	public float positionLerp = .1f;
	public GameObject suctionObject;
	public float width = 10;
	public float depth = 5;
	public ReceiptText recipt;

	public CollectedItemManager collectedItemManager;

	Vector3 targetPosition;
	float dir;

	float money = 0;
	float weight = 0;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		MoveVac ();
		Suck ();
	}

	void MoveVac(){
		targetPosition.x += Input.GetAxis (horiAxis) * moveDistancePerSecond * Time.deltaTime;
		targetPosition.z += Input.GetAxis (vertAxis) * moveDistancePerSecond * Time.deltaTime;
		targetPosition.y = transform.position.y;
		targetPosition.x = Mathf.Max (-width, Mathf.Min (width, targetPosition.x));
		targetPosition.z = Mathf.Max (-depth, Mathf.Min (depth, targetPosition.z));
		transform.position = Vector3.Lerp (transform.position, targetPosition, positionLerp);
	}

	void Suck(){
		float ndir = 0;
		if (Input.GetButton (suckInput))
			ndir += 1;
		if (Input.GetButton (blowInput))
			ndir -= 1;
		dir = Mathf.Lerp (dir,ndir,.1f);
		suctionObject.transform.localScale = Vector3.one * suctionDistance * dir;
		for(int i = Game.all.Count-1;i>=0;i--) {
			ObjectScript o = Game.all[i];
			if (!o.collected ) {
				if(dir > .7f){
					float d = Vector3.Distance (transform.position, o.transform.position);
					if(o.GetType() == typeof(StickyObject)&&((StickyObject) o).stickiness>0){
						((StickyObject) o).stickiness-= d;
					}else{
						o.transform.localScale = Vector3.one * Mathf.Min (1, Mathf.Max (0, d - collectDistance) * 2);
					}
					if (d < collectDistance) {
						Collect (o);
					}
					d = Mathf.Lerp (1, 0, Mathf.Min (d / suctionDistance, 1)) * ndir * suctionPower;

					o.body.AddForce ((transform.position - o.transform.position) * d, ForceMode.Force);
				} else {
					o.transform.localScale = Vector3.one;
				}
			}
		}
		Shader.SetGlobalVector("_vac",new Vector4(transform.position.x,transform.position.y,transform.position.z,Mathf.Max(0,suctionDistance*dir)));
	}

	void Collect(ObjectScript o){
		o.Collect ();
		if(o.collected){
			GameObject g = o.gameObject;
			recipt.addItemToText(o);
			Destroy(o);
			collectedItemManager.addItem(g);
			Debug.Log(g.GetComponent<Collider>().bounds);
		}
	
	}
}
