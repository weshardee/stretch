using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Stretch : MonoBehaviour {
	// editor references
	public GameObject Front;
	public GameObject Core;
	public SpringJoint2D CollapseSpring;
	
	// constants
	public const float SpreadDistance = 2f;
	public const float DeadZone = 0.2f;
	public const float MaxStretch = 15f;

    // local references
	private Transform FrontTransform;
	private Transform CoreTransform;
    private SpringJoint2D _FrontTarget;
    private SpringJoint2D _CoreTarget;
	private Glom _FrontGlom;
	private Glom _CoreGlom;
	private SliderJoint2D FrontSlider;
	private SliderJoint2D CoreSlider;
	
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
			CollapseSpring.enabled = value;
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
		
		_FrontTarget = Front.AddComponent<SpringJoint2D>();
        _CoreTarget = Core.AddComponent<SpringJoint2D>();
		
		_FrontGlom = Front.GetComponent<Glom>();
        _CoreGlom = Core.GetComponent<Glom>();
		
		// set up sliders
		FrontSlider = Front.AddComponent<SliderJoint2D>();
		CoreSlider = Core.AddComponent<SliderJoint2D>();
		
		FrontSlider.connectedBody = Core.GetComponent<Rigidbody2D>();
		CoreSlider.connectedBody = Front.GetComponent<Rigidbody2D>();
		
		FrontSlider.enabled = false;
		CoreSlider.enabled = false;
		
		FrontSlider.autoConfigureAngle = false;
		CoreSlider.autoConfigureAngle = false;
		
		// configure targets
		ConfigureTarget(_FrontTarget);
		ConfigureTarget(_CoreTarget);
    }
	
	void ConfigureTarget(SpringJoint2D target) {
		target.autoConfigureDistance = false;
		target.distance = 0;
		target.dampingRatio = 1;
		target.enableCollision = true;
		target.enabled = false;
		target.frequency = 2.5f;
	}

    void Update () {
		UpdateStretchDetails();
	}
	
	private void UpdateStretchDetails() {
		stretchDistance = (FrontTransform.position - CoreTransform.position).sqrMagnitude;
		stretchPercent = stretchDistance / MaxStretch;
	}
			
	public void Expand(Vector2 direction) {
		isExpanding = true;
		Vector2 distance = direction.normalized * SpreadDistance;
		
		SpringJoint2D rootTarget;
		SpringJoint2D endTarget;
		Transform rootTransform;
		Transform endTransform;
		SliderJoint2D rootSlider;
		SliderJoint2D endSlider;

		// toggle direction based on which side is glued
		if (_CoreGlom.IsOn) {
			rootTarget = _CoreTarget;
			rootTransform = CoreTransform;
			rootSlider = CoreSlider;
			endTarget = _FrontTarget;
			endTransform = FrontTransform;
			endSlider = FrontSlider;
		} else {
			rootTarget = _FrontTarget;
			rootTransform = FrontTransform;
			rootSlider = FrontSlider;
			endTarget = _CoreTarget;
			endTransform = CoreTransform;
			endSlider = CoreSlider;
		}
		
		// set slider angle
		endSlider.enabled = false;
		rootSlider.enabled = true;
		rootSlider.angle = Vector2.Angle(Vector2.right, direction);
		if (direction.y < 0) {
			rootSlider.angle = rootSlider.angle * -1;
		}
		
		// TODO this could probably be managed with a single slider
		
		// set stretch targets
		endTarget.connectedAnchor = (Vector2)rootTransform.position + distance;
        rootTarget.connectedAnchor = (Vector2)rootTransform.position;

        // draw debug lines
        Debug.DrawLine(rootTarget.connectedAnchor, rootTransform.position, Color.green);
        Debug.DrawLine(endTarget.connectedAnchor, rootTransform.position, Color.green);
	}
}
