using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Move : MonoBehaviour {

	private float _Speed = 1f;
	private float _MaxStretch = 1.5f;
	private float _MaxShrink = 0.5f;
	private float _StretchForce = 10f;
	
	private Vector2 _BasePosition;
	private bool _WasStretched;
	private bool _IsStretched;
	private float _HowStretched;
	private SpringJoint2D spring;
	private Rigidbody2D body;
	
	void Start () {
		body = GetComponent<Rigidbody2D>();
		spring = GetComponent<SpringJoint2D>();
	}
	
	void Update () {
		Vector2 input = GetInput();
		float howStretched = input.sqrMagnitude;
		
		_WasStretched = _IsStretched;
		_IsStretched = (howStretched > 0) && (howStretched >= _HowStretched); // check if stretch is decreasing
		_HowStretched = howStretched;
		
		bool stretchJustStarted = !_WasStretched && _IsStretched;
		bool stretchJustEnded = _WasStretched && !_IsStretched;
		
		if (stretchJustStarted) {
			spring.enabled = false;
		} else if (stretchJustEnded) {
			spring.enabled = true;
		}
		
		if (_IsStretched) {
			body.AddForce(input * _StretchForce, ForceMode2D.Force);		
			// transform.localPosition = _BasePosition + input * _MaxStretch;
			// transform.localScale = (1 - _MaxShrink * howStretched) * Vector2.one;
		} else {
			_BasePosition = transform.position;
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
