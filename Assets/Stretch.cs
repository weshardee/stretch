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
	private Glom _FrontGlom;
	private Glom _CoreGlom;

	// constants
	public const float StretchForce = 3f;
	public const float DeadZone = 0.2f;
	public const float RelaxThreshold = 0.3f;
	public const float MaxStretch = 15f;

	
	// stretching state
	private float _LastInputMagnitude = 0;
	private bool _WasStretching = false;
	private bool _IsStretching = false;
	public float stretchDistance { get; private set; }
	public float stretchPercent { get; private set; }
	
	public bool IsStretching { 
		get {
			return _IsStretching;
		} 
		private set {
			_IsStretching = value;
			
			// show targets for debug
			CoreTarget.SetActive(value);
			FrontTarget.SetActive(value);
			
			// enable springs to stretch
			_FrontTargetSpring.enabled = true;
			_CoreTargetSpring.enabled = true;

			// disable collapse spring
			CollapseSpring.enabled = !value;
			
			// trigger start/stop behavior
			if (_WasStretching && !_IsStretching) {
				OnStretchRelease();
			} else if (!_WasStretching && IsStretching) {
				OnStretchStart();
			}
			_WasStretching = _IsStretching;
		} 
	}
		
	void Start () {
		_FrontGlom = Front.GetComponent<Glom>();
		_FrontTargetRenderer = FrontTarget.GetComponentInChildren<Renderer>();
		_FrontTargetSpring = FrontTarget.GetComponent<SpringJoint2D>();

		_CoreGlom = Core.GetComponent<Glom>();
		_CoreTargetRenderer = CoreTarget.GetComponentInChildren<Renderer>();
		_CoreTargetSpring = CoreTarget.GetComponent<SpringJoint2D>();
		
		// disable front glom at start
		_FrontGlom.CanGlom = false;
	}
	
	void Update () {
		// get input
		Vector2 input = GetInput();
		float inputMagnitude = input.sqrMagnitude;
		
		// setup current state based on input
		bool isCollapsing = inputMagnitude < _LastInputMagnitude - RelaxThreshold; // check if stretch is decreasing
		bool isNeutral = inputMagnitude < DeadZone;
		bool canStretch = _CoreGlom.IsGlommed  && !isCollapsing && !isNeutral;

		// store magnitude for next check
		_LastInputMagnitude = inputMagnitude;
		
		// store current stretch state
		IsStretching = canStretch;
		if (IsStretching) {
			Vector2 force = input * StretchForce;
			FrontTarget.transform.position = (Vector2)Core.position + force;
			CoreTarget.transform.position = (Vector2)Front.position - force;
			
			// draw debug lines
			Debug.DrawLine(Front.transform.position, FrontTarget.transform.position, Color.green);
			Debug.DrawLine(Core.transform.position, CoreTarget.transform.position, Color.green);
		}

		stretchDistance = (Front.position - Core.position).sqrMagnitude;
		stretchPercent = stretchDistance / MaxStretch;
		
		// reset glom
		if (_FrontGlom.IsGlommed && !_CoreGlom.IsGlommed && stretchDistance < DeadZone) {
			_FrontGlom.CanGlom = false;
			_CoreGlom.CanGlom = true;
			_CoreGlom.Pulse();
			Debug.Log(name + ": is relaxed");
		}
	}
	
	private Vector2 GetInput()
	{		
		// Read input
		float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
		float vertical = CrossPlatformInputManager.GetAxis("Vertical");

		// combine axes
		Vector2 input = new Vector2(horizontal, vertical);

		// normalize input if it exceeds 1 in combined length:
		if (input.sqrMagnitude > 1)
		{
			input.Normalize();
		}
		
		return input;
	}
	
	void OnStretchStart() {
		Debug.Log(name + " stretch started");
	}
	
	void OnStretchRelease() {
		Debug.Log(name + " stretch released");
		_FrontGlom.CanGlom = true;
		_FrontGlom.Pulse(); // try a little extra hard to see if we hit something
		
		// stop everything for testing
		// Front.GetComponent<Rigidbody2D>().isKinematic = true;
		// Core.GetComponent<Rigidbody2D>().isKinematic = true;
	}
}
