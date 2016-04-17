using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Stretch : MonoBehaviour {
	public Transform core;
	public SpringJoint2D collapsingSpring;
	public SpringJoint2D reachingSpring;

	public const float MaxStretch = 2f;
	public const float DeadZone = 0.2f;
	public const float CollapseThreshold = 0.2f;

	private bool _CanStretch;
	
	private bool _IsStretching = false;
	public bool IsStretching { 
		get {
			return _IsStretching;
		} 
		private set {
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
		bool isCollapsing = howStretched < HowStretched - CollapseThreshold; // check if stretch is decreasing
		bool isNeutral = howStretched < DeadZone;
		HowStretched = howStretched;
		
		IsStretching = !isCollapsing && !isNeutral && _CanStretch;
		if (IsStretching) {
			transform.position = (Vector2)core.position + input * MaxStretch;
		} else if (isCollapsing) {
			_CanStretch = false;
		} else if (isNeutral) {
			_CanStretch = true;
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
