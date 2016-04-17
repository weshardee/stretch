using UnityEngine;
using System.Collections;

public class Glom : MonoBehaviour {
	// editor components
	public LayerMask layerMask;
	
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
	private bool _IsTrying = false;
	
	public bool IsGlommed { 
		get {
			return _GlomJoint.enabled;
		} 
		private set {
			_GlomJoint.enabled = value;
		} 
	}
	
	// active glom info
	private Vector2 _GlomPoint;
	
	private const float _Radius = 0.5f;
	private CircleCollider2D circleCollider;
	
	// other
	private const float _PumpedRadius = 0.55f;
	private const float _RegularRadius = 0.5f;
	
	void Start () {
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
	
	void OnCollisionEnter2D(Collision2D coll){
		if (!IsGlommed && CanGlom) {
			GlomTo(coll);
		}
	}

	void GlomTo(Collision2D coll) {
		Debug.Log("collide");
		StopTry();
		_GlomPoint = coll.contacts[0].point;
		
		// set the point of contact as the connected anchor point of the _GlomJoint
		Vector2 point = (Vector2)coll.contacts[0].point;
		_GlomJoint.connectedAnchor = point;

		// set joint status
		IsGlommed = true;	
	}
	
	public void UnGlom() {
		IsGlommed = false;
	}
	
	public void Try() {
		CanGlom = true;
		
		// pump up the volume
		Debug.Log("try pump");
		_IsTrying = true;
		circleCollider.radius = _PumpedRadius;
	}
	
	public void StopTry() {
		if (_IsTrying) {
			Debug.Log("stop try");
			_IsTrying = false;
			circleCollider.radius = _RegularRadius;
		}
	}
}
