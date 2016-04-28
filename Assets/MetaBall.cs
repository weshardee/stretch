using UnityEngine;
using System.Collections;

public class MetaBall : MonoBehaviour {
    public float power = 1;

	void OnDrawGizmos() {
		Gizmos.DrawSphere(transform.position, power);
	}

    public float CalcInfluence(Vector2 globalPoint) {
        float distance = (globalPoint - (Vector2)transform.position).sqrMagnitude;
        float influence = 1 - distance * power;
        return influence;
    }
}
