using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Stretch : MonoBehaviour {
	// editor references
	public Transform Front;
	public Transform Core;
	public GameObject FrontTarget;
	public GameObject CoreTarget;
	public SpringJoint2D CollapseSpring;
	
	// local references
	private Renderer _FrontTargetRenderer;
	private Renderer _CoreTargetRenderer;
	private SpringJoint2D _FrontTargetSpring;
	private SpringJoint2D _CoreTargetSpring;

	// constants
	public const float SpreadForce = 3f;
	public const float DeadZone = 0.2f;
	public const float RelaxThreshold = 0.1f;
	public const float MaxStretch = 15f;
	
	// stretching state
	public Vector2 spread;
	public float stretchDistance { get; private set; }
	public float stretchPercent { get; private set; }
	
	// states
	private bool _isHolding = false;
	public bool isHolding {
		get {
			return _isHolding;
		}
		set {
			_isHolding = value;
			if (value) {
				isExpanding = false;
				isCollapsing = false;
			}
		}
	}
	
	private bool _isCollapsing = true;
	public bool isCollapsing {
		get {
			return _isCollapsing;
		}
		set {
			_isCollapsing = value;
			if (value) {
				isExpanding = false;
				isHolding = false;
			}
			
			// set collapse spring state
			// CollapseSpring.enabled = value;
		}
	}
	
	private bool _isExpanding = false;
	public bool isExpanding { 
		get {
			return _isExpanding;
		} 
		set {
			_isExpanding = value;
			if (value) {
				isCollapsing = false;
				isHolding = false;
			}
			
			// enable expand targets
			CoreTarget.SetActive(value);
			FrontTarget.SetActive(value);
		} 
	}
		
	void Awake () {
		_FrontTargetRenderer = FrontTarget.GetComponentInChildren<Renderer>();
		_FrontTargetSpring = FrontTarget.GetComponent<SpringJoint2D>();

		_CoreTargetRenderer = CoreTarget.GetComponentInChildren<Renderer>();
		_CoreTargetSpring = CoreTarget.GetComponent<SpringJoint2D>();
	}
	
	void Update () {
		UpdateStretchDetails();
		
		if (isExpanding) {
			Expand();
		}
	}
	
	private void UpdateStretchDetails() {
		stretchDistance = (Front.position - Core.position).sqrMagnitude;
		stretchPercent = stretchDistance / MaxStretch;
	}
			
	private void Expand() {
		isExpanding = true;
		float spreadMagnitude = spread.sqrMagnitude;
		Vector2 force = spread * SpreadForce;
		FrontTarget.transform.position = (Vector2)Core.position + force;
		CoreTarget.transform.position = (Vector2)Front.position - force;
		
		// draw debug lines
		Debug.DrawLine(Front.transform.position, FrontTarget.transform.position, Color.green);
		Debug.DrawLine(Core.transform.position, CoreTarget.transform.position, Color.green);
	}
}
