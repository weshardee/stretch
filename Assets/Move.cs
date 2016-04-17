using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Move : MonoBehaviour {

	private float _Speed = 1f;
	private float _MaxStretch = 1.5f;
	private float _MaxShrink = 0.5f;
	private float _StretchForce = 0.0001f;
	
	private Vector2 _BasePosition;
	private Vector2 _GoalPosition;
	private bool _WasStretched;
	private bool _IsStretching;
	private bool _IsCollapsing;
	private float _HowStretched;
	public SpringJoint2D collapsingSpring;
	public SpringJoint2D reachingSpring;
	private Rigidbody2D body;
	
	void Start () {
		body = GetComponent<Rigidbody2D>();
		reachingSpring.enabled = false;
	}
	
	void Update () {
		Vector2 input = GetInput();
		float howStretched = input.sqrMagnitude;
		
		_WasStretched = _IsStretching;
		_IsStretching = (howStretched > 0) && (howStretched >= _HowStretched); // check if stretch is decreasing
		_IsCollapsing = (howStretched > 0) && (howStretched < _HowStretched);
		_HowStretched = howStretched;
		
		bool stretchJustStarted = !_WasStretched && _IsStretching;
		bool stretchJustEnded = _WasStretched && !_IsStretching;
		
		if (stretchJustStarted) {
			collapsingSpring.enabled = false;
			reachingSpring.enabled = true;
		} else if (stretchJustEnded) {
			collapsingSpring.enabled = true;
			reachingSpring.enabled = false;
		}
		
		if (_IsStretching) {
			_GoalPosition = input * _MaxStretch; 
			transform.localPosition = _GoalPosition;
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

}
