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
			if (_isSticky == value) {
				return;
			}
			_isSticky = value;
			if (value) {
				On();
			} else {
				IsOn = false;
			}
		}
	}
	
	public bool IsOn { 
		get {
			return _GlomJoint.enabled;
		} 
		private set {
			_GlomJoint.enabled = value;
		} 
	}
	
	private float _lastCollisionExpiration = 0;
	private Collision2D _lastCollision;
	private Collision2D _LastCollision {
		get {
			if (_lastCollisionExpiration < Time.time) {
				return null;
			}
			return _lastCollision;
		}
		set {
			_lastCollision = value;
			_lastCollisionExpiration = Time.time + _CollisionExitLag;
		}
	}

	// active glom info
	private Vector2 _GlomPoint;
	
	private const float _Radius = 0.5f;
	private CircleCollider2D circleCollider;
	
	// other
	private const float _PulseRadius = 0.6f;
	private const float _RegularRadius = 0.5f;
	private float _PulseEnd = 0;
	private const float _PulseDuration = 0.1f; // in seconds 
	private const float _CollisionExitLag = 0f; // in seconds
	
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
		if (IsOn) {
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
		if (IsSticky) {
			On();
		}
	}

	public bool On() {
		if (IsOn) {
			return true;
		}
		
		// Debug.Log(name + ": try to glom");
		Collision2D coll = _LastCollision;
		if (coll == null) {
			return false;
		}
		
		ContactPoint2D contactPoint = coll.contacts[0];
		// Debug.Log(name + ": glom to " + coll.transform.name);
		
		// set the point of contact as the connected anchor point of the _GlomJoint
		Vector2 point = (Vector2)coll.contacts[0].point;
		_GlomJoint.connectedAnchor = contactPoint.point;

		// set joint status
		IsOn = true;	
		if (otherGlom != null) {
			otherGlom.IsSticky = false;
		}
		
		return IsOn;
	}
	
	public void Swap(Glom glom) {
		if (glom.IsOn) {
			_LastCollision = glom._lastCollision;
			On();
		}
	}
}
