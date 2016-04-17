using UnityEngine;
using System.Collections;

public class ScaleWithStretch : MonoBehaviour {

	public Transform end1;
	public Transform end2;
	
	private const float _ScaleAtMaxStretch = 0.7f;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float distance = (end1.position - end2.position).sqrMagnitude;	
		float howStretched = distance / Stretch.MaxStretch;
		Vector2 scale = Vector2.one * Mathf.Lerp(1, _ScaleAtMaxStretch, howStretched);		
		end1.localScale = scale;
		end2.localScale = scale;
	}
}
