using UnityEngine;
using System.Collections;

public class Glom : MonoBehaviour {
	// editor components
	public LayerMask layerMask;
	public Glom otherGlom;

	// local components
	private SpringJoint2D _GlomJoint;
	
	// state flags
	private bool _isSticky;
	public bool IsSticky {
		get {
			return _isSticky;
		}
		set {
			_isSticky = value;
			if (value) {
				Try();
			}
		}
	}
	
	public bool IsGlommed { 
		get {
			return _GlomJoint.enabled;
		} 
		private set {
			_GlomJoint.enabled = value;
		} 
	}
	
	private Collision2D _LastCollision;
	private float _LastCollisionExpiration;

	// active glom info
	private Vector2 _GlomPoint;
	
	private const float _Radius = 0.5f;
	private CircleCollider2D circleCollider;
	
	// other
	private const float _PulseRadius = 0.6f;
	private const float _RegularRadius = 0.5f;
	private float _PulseEnd = 0;
	private const float _PulseDuration = 0.1f; // in seconds 
	private const float _CollisionExitLag = 1.5f; // in seconds
	
	void Awake() {
		circleCollider = GetComponent<CircleCollider2D>();
		
		// create and configure joint
		_GlomJoint = gameObject.AddComponent<SpringJoint2D>();
		_GlomJoint.autoConfigureConnectedAnchor = false;
		_GlomJoint.autoConfigureDistance = false;
		_GlomJoint.distance = 0;
		_GlomJoint.enableCollision = true;
		_GlomJoint.enabled = false;
		_GlomJoint.frequency = 4;
		_GlomJoint.dampingRatio = 1;
	}
	
	void Update () {
		if (IsGlommed) {
			Vector2 anchorInWorldSpace = _GlomJoint.anchor + (Vector2)transform.position;
			Debug.DrawLine(anchorInWorldSpace, _GlomJoint.connectedAnchor, Color.green);
		}
	}
	
	void OnCollisionStay2D(Collision2D coll) {
		TrackCollision(coll);
	}
	
	void OnCollisionExit2D(Collision2D coll) {
		TrackCollision(coll);
	}
	
	void TrackCollision(Collision2D coll) {
		_LastCollision = coll;
		_LastCollisionExpiration = Time.time + _CollisionExitLag;
		if (IsSticky) {
			Try();
		}
	}

	public bool Try() {
		if (IsGlommed) {
			return true;
		}

		if (_LastCollisionExpiration < Time.time) {
			return false;
		}
		
		if (_LastCollision == null) {
			return false;
		}
		
		Collision2D coll = _LastCollision;
		ContactPoint2D contactPoint = coll.contacts[0];
		Debug.Log(name + ": glom to " + coll.transform.name);
		
		// set the point of contact as the connected anchor point of the _GlomJoint
		Vector2 point = (Vector2)coll.contacts[0].point;
		_GlomJoint.connectedAnchor = contactPoint.point;

		// set joint status
		IsGlommed = true;	
		if (otherGlom != null) {
			otherGlom.UnGlom();
		}
		
		return IsGlommed;
	}
	
	public void UnGlom() {
		Debug.Log(name + ": glom release");
		IsSticky = false;
		IsGlommed = false;
	}
}
