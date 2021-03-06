﻿using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour {

	public List<ObjectScript> objects;
	public int count = 20;
	public float delayPerObject = .1f;
	public float spawnVelocity = 1;
	List<GameObject> spawned;
	int haveSpawned = 0;
	[HideInInspector]public bool allSpawned = false;

	// Use this for initialization
	void OnEnable () {
		spawned = new List<GameObject> ();
		for (int i = 0; i<count; i++) {
			Invoke("Spawn",i* delayPerObject);
		}
		allSpawned = false;
	}
	
	void Spawn(){
		int i = Mathf.FloorToInt (Random.value * objects.Count);
		if (objects [i] == null)
			objects [i] = Game.GetObjectOfType (Game.Type.General);
		GameObject g = Instantiate<ObjectScript>(objects [i]).gameObject;
		spawned.Add (g);
		Rigidbody r = g.GetComponent<Rigidbody> ();
		if (r == null)
			r = g.GetComponentInChildren<Rigidbody> ();
		if(r!=null)
			r.velocity = Random.onUnitSphere * spawnVelocity;
		g.transform.parent = this.transform;
		g.transform.localPosition = Vector3.zero;
		Utils.ZeroChildPosition (g.transform);
		haveSpawned++;
		if (haveSpawned == count)
			allSpawned = true;
    }

	// Update is called once per frame
	void Update () {
		if (transform.childCount==0&&allSpawned) {
			Destroy(gameObject);
		}
		if (spawned.Count != 0&&spawned [0] == null) {
			spawned.RemoveAt(0);
		}
	}
}
