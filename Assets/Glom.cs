using UnityEngine;
using System.Collections;

public class Glom : MonoBehaviour {
	// editor components
	public SpringJoint2D glomJoint;
	public LayerMask layerMask;
	
	// state flags
	private bool _CanGlom;
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
		glomJoint.frequency = 4;
		glomJoint.dampingRatio = 1;
	}
	
	void Update () {
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
	
	void UnGlom() {
		IsGlommed = false;
		glomJoint.enabled = false;
	}
}
