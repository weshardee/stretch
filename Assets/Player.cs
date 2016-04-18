using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public enum PlayerState {
	Loose,
	Grounded,
	Reaching,
	Grabbing,
	CollapsingToFront,
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
	
	// state
	private PlayerState _State = PlayerState.Loose;
	private float _LastInputMagnitude = 0;
	private bool _WasStretching = false;
	private bool _IsStretching = false;
	public float stretchDistance { get; private set; }
	public float stretchPercent { get; private set; }
			
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
		// Debug.Log(_State);
		switch (_State) {
			case PlayerState.Loose: {
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
				_Stretch.isCollapsing = true;
				_CoreGlom.IsSticky = true;
				_FrontGlom.IsSticky = false;

				// change state if input starts
				if (GetInput() != Vector2.zero) {
					_State = PlayerState.Reaching;
				}
				break;
			}
			case PlayerState.Reaching: {
				_Stretch.isExpanding = true;
				_CoreGlom.IsSticky = true;
				_FrontGlom.IsSticky = false;
				
				// update stretch direction
				Vector2 input = GetInput();
				_Stretch.spread = input;
				
				// switch state on input release
				if (HasInputStopped(input)) {
					_LastInputMagnitude = 0;
					_State = PlayerState.Grabbing;
				}
				break;
			}
			case PlayerState.Grabbing: {
				_Stretch.isHolding = true;
				_CoreGlom.IsSticky = true;
				_FrontGlom.IsSticky = false;
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
		
		_LastInputMagnitude = input.sqrMagnitude;
		
		return input;
	}
	
	private bool HasInputStopped(Vector2 input) {
		float inputMagnitude = input.sqrMagnitude;
		bool hasStopped = inputMagnitude <= _LastInputMagnitude - InputReleaseThreshold;
		_LastInputMagnitude = inputMagnitude;
		
		Debug.Log(_LastInputMagnitude + " " + inputMagnitude);
		
		return hasStopped;
	}
}
