using UnityEngine;

public class ScaleWithStretch : MonoBehaviour
{
    private Stretch _stretch;

    // Use this for initialization
    void Start()
    {
        _stretch = GetComponentInParent<Stretch>();
    }

    // Update is called once per frame
    void Update()
    {
        float howStretched = _stretch.stretchPercent;
        Vector2 scale = Vector2.one * Mathf.Lerp(1, 0, howStretched);
        transform.localScale = scale;
    }
}
