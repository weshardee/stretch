using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Stretch : MonoBehaviour {
	// editor references
	public GameObject Front;
	public GameObject Core;
	public SpringJoint2D CollapseSpring;
	
	// constants
	public const float SpreadForce = 3f;
	public const float DeadZone = 0.2f;
	public const float RelaxThreshold = 0.1f;
	public const float MaxStretch = 15f;

    // local references
	private Transform FrontTransform;
	private Transform CoreTransform;
    private TargetJoint2D _FrontTarget;
    private TargetJoint2D _CoreTarget;
	private Glom _FrontGlom;
	private Glom _CoreGlom;
	
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
            _FrontTarget.enabled = value;
            _CoreTarget.enabled = value;
        }
    }
		
	void Awake () {
		FrontTransform = Front.transform;
		CoreTransform = Core.transform;
		
		_FrontTarget = Front.AddComponent<TargetJoint2D>();
        _CoreTarget = Core.AddComponent<TargetJoint2D>();
		
		_FrontGlom = Front.GetComponent<Glom>();
        _CoreGlom = Core.GetComponent<Glom>();
    }

    void Update () {
		UpdateStretchDetails();
		
		if (isExpanding) {
			Expand();
		}
	}
	
	private void UpdateStretchDetails() {
		stretchDistance = (FrontTransform.position - CoreTransform.position).sqrMagnitude;
		stretchPercent = stretchDistance / MaxStretch;
	}
			
	private void Expand() {
		isExpanding = true;
		Vector2 force = spread * SpreadForce;
		
		TargetJoint2D rootTarget;
		TargetJoint2D endTarget;
		Transform rootTransform;
		Transform endTransform;
		
		// toggle direction based on which side is glued
		if (_CoreGlom.IsOn) {
			rootTarget = _CoreTarget;
			rootTransform = CoreTransform;
			endTarget = _FrontTarget;
			endTransform = FrontTransform;
		} else {
			rootTarget = _FrontTarget;
			rootTransform = FrontTransform;
			endTarget = _CoreTarget;
			endTransform = CoreTransform;
		}
		
		endTarget.target = (Vector2)rootTransform.position + force;
        rootTarget.target = (Vector2)endTransform.position - force;

        // draw debug lines
        Debug.DrawLine(rootTarget.target, rootTransform.position, Color.green);
        Debug.DrawLine(endTarget.target, endTransform.position, Color.green);
	}
}
