﻿using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public enum PlayerState {
	Loose,
	Grounded,
	Reach,
	Grab,
	Pull,
	CollapsingToCore,
}

public class Player : MonoBehaviour {
	
	// editor references
	public Transform Front;
	public Transform Core;
	public GameObject FrontTarget;
	public GameObject CoreTarget;
	public SpringJoint2D CollapseSpring;
	
	// local references
	private SpringJoint2D _FrontTargetSpring;
	private SpringJoint2D _CoreTargetSpring;
	private Glom _FrontGlom;
	private Glom _CoreGlom;
	private Stretch _Stretch;

	// constants
	public const float StretchForce = 3f;
	public const float DeadZone = 0.2f;
	public const float InputReleaseThreshold = 0.1f;
	public const float MaxStretch = 15f;
	public const float GrabDuration = 5f;
	
	// state
	private PlayerState _State = PlayerState.Loose;
	private float _LastInputMagnitude = 0;
	private bool _WasStretching = false;
	private bool _IsStretching = false;
	public float stretchDistance { get; private set; }
	public float stretchPercent { get; private set; }
	private float _GrabTimeout = 0;
	private bool _UseGravity {
		set {
			float gravityScale = value ? 1 : 0;
			Front.GetComponent<Rigidbody2D>().gravityScale = gravityScale;
			Core.GetComponent<Rigidbody2D>().gravityScale = gravityScale;
		}
	}
	
			
	void Awake () {
		_Stretch = GetComponent<Stretch>();
				
		_FrontGlom = Front.GetComponent<Glom>();
		_FrontTargetSpring = FrontTarget.GetComponent<SpringJoint2D>();

		_CoreGlom = Core.GetComponent<Glom>();
		_CoreTargetSpring = CoreTarget.GetComponent<SpringJoint2D>();
		
		// disable front glom at start
		_FrontGlom.IsSticky = false;
	}
	
	void Update () {
		Debug.Log(_State);
		switch (_State) {
			case PlayerState.Loose: {
				_UseGravity = true;
				_Stretch.isCollapsing = true;
				_CoreGlom.IsSticky = true;
				_FrontGlom.IsSticky = false;

				// change state if core gets glommed
				if (_CoreGlom.IsGlommed) {
					_State = PlayerState.Grounded;
				}
				break;
			}
			case PlayerState.Grounded: {
				_UseGravity = false;
				_Stretch.isCollapsing = true;
				_CoreGlom.IsSticky = true;
				_FrontGlom.IsSticky = false;

				// change state if input starts
				if (GetInput() != Vector2.zero) {
					_LastInputMagnitude = 0;
					_State = PlayerState.Reach;
				}
				break;
			}
			case PlayerState.Reach: {
				_UseGravity = false;
				_Stretch.isExpanding = true;
				_CoreGlom.IsSticky = true;
				_FrontGlom.IsSticky = false;
				
				// update stretch direction
				Vector2 input = GetInput();
				_Stretch.spread = input;
				
				// switch state on input release
				if (HasInputStopped(input)) {
					_LastInputMagnitude = 0;
					_GrabTimeout = Time.time + GrabDuration;
					_State = PlayerState.Grab;
				}
				break;
			}
			case PlayerState.Grab: {
				_UseGravity = false;
				_Stretch.isExpanding = true;
				_CoreGlom.IsSticky = true;
				_FrontGlom.IsSticky = true;
				
				if (_FrontGlom.IsGlommed) {
					_State = PlayerState.Pull;
				} else if (_GrabTimeout < Time.time) {
					_State = PlayerState.Loose;
				}
				break;
			}
			case PlayerState.Pull: {
				_UseGravity = false;
				_Stretch.isCollapsing = true;
				_CoreGlom.IsSticky = false;
				_FrontGlom.IsSticky = true;
				break;
			}
		}
	}
	
	private Vector2 GetInput() {
		// Read input
		float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
		float vertical = CrossPlatformInputManager.GetAxis("Vertical");

		// combine axes
		Vector2 input = new Vector2(horizontal, vertical);

		// normalize input if it exceeds 1 in combined length:
		if (input.sqrMagnitude > 1) {
			input.Normalize();
		}
				
		return input;
	}
	
	private bool HasInputStopped(Vector2 input) {
		float inputMagnitude = input.sqrMagnitude;
		bool hasStopped = inputMagnitude <= _LastInputMagnitude - InputReleaseThreshold;
		_LastInputMagnitude = inputMagnitude;		
		return hasStopped;
	}
}
