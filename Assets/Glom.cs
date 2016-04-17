﻿using UnityEngine;
using System.Collections;

public class Glom : MonoBehaviour {
	// editor components
	public SpringJoint2D glomJoint;
	public LayerMask layerMask;
	
	// state flags
	public bool CanGlom = true;
	public bool IsGlommed { get; private set; }
	
	// active glom info
	private Vector2 _GlomPoint;
	
	private const float _Radius = 0.5f;
	
	void Start () {
		// create and configure joint
		glomJoint = gameObject.AddComponent<SpringJoint2D>();
		glomJoint.autoConfigureConnectedAnchor = false;
		glomJoint.autoConfigureDistance = false;
		glomJoint.distance = 0;
		glomJoint.enableCollision = true;
		glomJoint.enabled = false;
		glomJoint.frequency = 10;
	}
	
	void Update () {
		// if (!IsGlommed && CanGlom) {
		// 	// check for collision
		// 	bool shouldGlom = body.IsTouchingLayers(layerMask);
		// 	GlomTo()
		// }
		
		// Debug.Log(IsGlommed);
		if (IsGlommed) {
			Vector2 anchorInWorldSpace = glomJoint.anchor + (Vector2)transform.position;
			Debug.DrawLine(anchorInWorldSpace, glomJoint.connectedAnchor, Color.green);
		}
	}
	
	void OnCollisionEnter2D(Collision2D coll){
		if (!IsGlommed && CanGlom) {
			GlomTo(coll);
		}
	}

	void GlomTo(Collision2D coll) {
		IsGlommed = true;	
		glomJoint.enabled = true;
		_GlomPoint = coll.contacts[0].point;
		
		// set the point of contact as the connected anchor point of the glomJoint
		Vector2 point = (Vector2)coll.contacts[0].point;
		glomJoint.connectedAnchor = point;
	}
}
