using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Move : MonoBehaviour {

	private float _Speed = 1f;
	private float _MaxStretch = 5f;
	private float _MaxShrink = 0.5f;
	
	void Start () {
	
	}
	
	void Update () {
		Vector2 input = GetInput();
		float howStretched = input.sqrMagnitude;
		
		// TODO constrain position by environment
		
		transform.localPosition = input * _MaxStretch;
		transform.localScale = (1 - _MaxShrink * howStretched) * Vector2.one;
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
