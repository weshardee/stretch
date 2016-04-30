using UnityEngine;

public class MetaBall : MonoBehaviour {
    public float power = 1;

	void OnDrawGizmos() {
		Gizmos.DrawWireSphere(transform.position, power);
	}

    public float CalcInfluence(Vector2 point)
    {
        // TODO performance bottleneck? Maybe cache position on update?
        var deltaVector = point - (Vector2)transform.localPosition;
        var r = deltaVector.sqrMagnitude;

        var influence = (power*power)/(r*r);

        if (influence > 1) influence = 1;
        if (influence < 0) influence = 0;

        return influence;
    }
}
