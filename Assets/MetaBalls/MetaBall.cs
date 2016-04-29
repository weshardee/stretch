using UnityEngine;

public class MetaBall : MonoBehaviour {
    public float power = 1;

	void OnDrawGizmos() {
		Gizmos.DrawWireSphere(transform.position, power);
	}

    public float CalcInfluence(Vector2 point)
    {
        // TODO performance bottleneck? Maybe cache position on update?
        Vector2 deltaVector = point - (Vector2)transform.localPosition;
        float r = deltaVector.sqrMagnitude;

        //if (r > power)
        //{
        //    return 0;
        //}

        // adjust radius by a factor
        r /= power;

        float influence = r * r * r * (r * (r * 6 - 15) + 10); // http://www.geisswerks.com/ryan/BLOBS/blobs.html

        if (influence > 1) influence = 1;
        if (influence < 0) influence = 0;

        return 1 - influence;
    }
}
