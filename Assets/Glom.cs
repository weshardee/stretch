using UnityEngine;
using System.Collections;

public class Glom : MonoBehaviour {
	// editor components
	public LayerMask layerMask;
	public Glom otherGlom;

	// local components
	private SpringJoint2D _GlomJoint;
	
	// state flags
	private bool _CanGlom = true;
	public bool CanGlom { 
		get {
			return _CanGlom; 
		} 
		set {
			_CanGlom = value;
			if (!value) {
				UnGlom();
			}
		}
	}
	private bool _IsPulsing = false;
	
	public bool IsGlommed { 
		get {
			return _GlomJoint.enabled;
		} 
		private set {
			_GlomJoint.enabled = value;
			StopPulse();
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
		
		if (_IsPulsing && Time.time > _PulseEnd) {
			StopPulse();
		}
	}
	
	void OnCollisionEnter2D(Collision2D coll){
		if (!IsGlommed && CanGlom) {
			GlomTo(coll);
		}
	}

	void GlomTo(Collision2D coll) {
		ContactPoint2D contactPoint = coll.contacts[0];
		Debug.Log(name + ": collision with " + coll.transform.name);
		
		// set the point of contact as the connected anchor point of the _GlomJoint
		Vector2 point = (Vector2)coll.contacts[0].point;
		_GlomJoint.connectedAnchor = contactPoint.point;

		// set joint status
		IsGlommed = true;	
		if (otherGlom != null) {
			otherGlom.UnGlom();
		}
	}
	
	public void UnGlom() {
		Debug.Log(name + ": release");
		IsGlommed = false;
	}
	
	public void Pulse() {
		// pump up the volume
		_IsPulsing = true;
		_PulseEnd = Time.time + _PulseDuration;
		circleCollider.radius = _PulseRadius;
		Debug.Log(name + ": pulse until " + _PulseEnd);
	}
	
	public void StopPulse() {
		if (_IsPulsing) {
			Debug.Log(name + ": stop pulse");
			_IsPulsing = false;
			circleCollider.radius = _RegularRadius;
		}
	}
}
