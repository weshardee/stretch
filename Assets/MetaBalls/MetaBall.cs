using UnityEngine;

public class MetaBall : MonoBehaviour {
    public float power = 1;

	void OnDrawGizmos() {
		Gizmos.DrawWireSphere(transform.position, power);
	}

    public float CalcInfluence(Vector2 globalPoint) {
        float distance = (globalPoint - (Vector2)transform.position).sqrMagnitude;
        float influence = 1 - distance * power;
        if (influence > 1) influence = 1;
        if (influence < 0) influence = 0;
        return influence;
    }
}
