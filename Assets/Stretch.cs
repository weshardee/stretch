using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Stretch : MonoBehaviour {
	public SpringJoint2D collapsingSpring;
	public SpringJoint2D reachingSpring;

	public const float MaxStretch = 1.5f;

	private bool _WasStretched;
	public bool IsStretching { get; private set; }
	public float HowStretched { get; private set; }
	
	private Renderer indicatorRenderer;
	
	void Start () {
		reachingSpring.enabled = false;
		indicatorRenderer = GetComponentInChildren<Renderer>();
		indicatorRenderer.enabled = false;
	}
	
	void Update () {
		Vector2 input = GetInput();
		float howStretched = input.sqrMagnitude;
		
		_WasStretched = IsStretching;
		IsStretching = (howStretched > 0) && (howStretched >= HowStretched); // check if stretch is decreasing
		HowStretched = howStretched;
		
		bool stretchJustStarted = !_WasStretched && IsStretching;
		bool stretchJustEnded = _WasStretched && !IsStretching;
		
		if (stretchJustStarted) {
			indicatorRenderer.enabled = true;
			collapsingSpring.enabled = false;
			reachingSpring.enabled = true;
		} else if (stretchJustEnded) {
			indicatorRenderer.enabled = false;
			collapsingSpring.enabled = true;
			reachingSpring.enabled = false;
		}
		
		if (IsStretching) {
			Vector2 goalPosition = input * MaxStretch; 
			transform.localPosition = goalPosition;
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
