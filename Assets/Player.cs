using UnityEngine;
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
	public SpringJoint2D CollapseSpring;
	
	// local references
	private Glom _FrontGlom;
	private Glom _CoreGlom;
	private Stretch _Stretch;
	private Rigidbody2D _CoreBody;
	private Rigidbody2D _FrontBody;

	// constants
	public const float StretchForce = 13f;
	public const float DeadZone = 0.2f;
	public const float InputReleaseThreshold = 0.1f;
	public const float MaxStretch = 15f;
	public const float GrabDuration = 0.1f;
	public const float PullReleaseDistanceThreshold = 0.05f;
	public const float PullReleaseVelocityThreshold = 0.05f;
	
	// state
	private PlayerState _State = PlayerState.Loose;
	private float _LastInputMagnitude = 0;
	private float _GrabTimeout = 0;
	private bool _UseGravity {
		set {
			float gravityScale = value ? 1 : 0;
			_FrontBody.gravityScale = gravityScale;
			_CoreBody.gravityScale = gravityScale;
		}
	}

	void Awake () {
		_Stretch = GetComponent<Stretch>();
				
		_FrontGlom = Front.GetComponent<Glom>();
		_FrontBody = Front.GetComponent<Rigidbody2D>();

		_CoreGlom = Core.GetComponent<Glom>();
		_CoreBody = Core.GetComponent<Rigidbody2D>();
		
		// disable front glom at start
		_FrontGlom.IsSticky = false;
	}
	
	void Update () {
		// Debug.Log(_State);
		switch (_State) {
			case PlayerState.Loose: {
				_UseGravity = true;
				_Stretch.isCollapsing = true;
				_CoreGlom.IsSticky = true;
				_FrontGlom.IsSticky = false;

				// change state if core gets glommed
				if (_CoreGlom.IsOn) {
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
				
				// switch state on input release
				if (input == Vector2.zero) {
					_LastInputMagnitude = 0;
					_GrabTimeout = Time.time + GrabDuration;
					_State = PlayerState.Grab;
				} else {
					_Stretch.spread = input;
				}
				break;
			}
			case PlayerState.Grab: {
				_UseGravity = false;
				_Stretch.isExpanding = true;
				_CoreGlom.IsSticky = true;
				_FrontGlom.IsSticky = false;
				
				if (_FrontGlom.IsOn) {
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
				
				bool isFinishedPulling = 
					_Stretch.stretchDistance < PullReleaseDistanceThreshold
					&& _CoreBody.velocity.sqrMagnitude < PullReleaseVelocityThreshold;				
				
				if (isFinishedPulling) {
					_CoreGlom.Swap(_FrontGlom);
					_State = PlayerState.Loose;
				}
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
		
		// store relative input state
		float inputMagnitude = input.sqrMagnitude;
		if (inputMagnitude < _LastInputMagnitude - InputReleaseThreshold) {
			inputMagnitude = 0;
			input = Vector2.zero;
		}	
		_LastInputMagnitude = inputMagnitude;		
				
		return input;
	}
}
