using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Stretch : MonoBehaviour {
	public Transform core;
	public SpringJoint2D collapsingSpring;
	public SpringJoint2D reachingSpring;

	public const float MaxStretch = 1.5f;

	private bool _WasStretched = false;
	private bool _IsStretching = false;
	public bool IsStretching { 
		get {
			return _IsStretching;
		} 
		private set {
			_WasStretched = _IsStretching;
			_IsStretching = value;
			indicatorRenderer.enabled = value;
			collapsingSpring.enabled = !value;
			reachingSpring.enabled = value;
		} 
	}
	public float HowStretched { get; private set; }
	
	private Renderer indicatorRenderer;
	
	void Start () {
		indicatorRenderer = GetComponentInChildren<Renderer>();
	}
	
	void Update () {
		Vector2 input = GetInput();
		float howStretched = input.sqrMagnitude;
		bool isCollapsing = howStretched < HowStretched; // check if stretch is decreasing
		bool isNeutral = howStretched == 0;
		HowStretched = howStretched;
		
		IsStretching = !isCollapsing && !isNeutral;
		if (IsStretching) {
			transform.position = (Vector2)core.position + input * MaxStretch;
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
