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
		Debug.Log(_FrontTarget);
        _CoreTarget = Core.AddComponent<TargetJoint2D>();
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
		_FrontTarget.target = (Vector2)CoreTransform.position + force;
        _CoreTarget.target = (Vector2)FrontTransform.position - force;

        // draw debug lines
        Debug.DrawLine(Core.transform.position, _FrontTarget.target, Color.green);
		//Debug.DrawLine(Core.transform.position, CoreTarget.transform.position, Color.green);
	}
}
