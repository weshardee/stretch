using UnityEngine;
using System.Collections;

public class Glom : MonoBehaviour {
	// editor components
	public FixedJoint2D glomJoint;
	public LayerMask layerMask;
	
	// state flags
	public bool CanGlom = true;
	public bool IsGlommed { get; private set; }
	
	void Start () {
		glomJoint = gameObject.AddComponent<FixedJoint2D>();
		glomJoint.enabled = false;
	}
	
	void Update () {
		// if (!IsGlommed && CanGlom) {
		// 	// check for collision
		// 	bool shouldGlom = body.IsTouchingLayers(layerMask);
		// 	GlomTo()
		// }
		
		// Debug.Log(IsGlommed);
	}
	
	void OnCollisionEnter2D(Collision2D coll){
		if (!IsGlommed && CanGlom) {
			Debug.Log("tada");
			GlomTo(coll);
		}
	}

	void GlomTo(Collision2D coll) {
		IsGlommed = true;	
		glomJoint.enabled = true;
	}
}
