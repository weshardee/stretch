using UnityEngine;

public class Stretch : MonoBehaviour {
    // editor references
    public GameObject head;
    public GameObject core;
    private SpringJoint2D spring;

    // constants
    public const float SpreadDistance = 2f;
    public const float DeadZone = 0.2f;
    public const float MaxStretch = 15f;

    // local references
    private Rigidbody2D headBody;
    private Rigidbody2D coreBody;
    private Transform headTransform;
    private Transform coreTransform;
    private Glom coreGlom;
    private SliderJoint2D headSlider;
    private SliderJoint2D coreSlider;

    // stretching state
    public Vector2 spread;
    public float stretchDistance { get; private set; }
    public float stretchPercent { get; private set; }

    // states
    private bool _isCollapsing = true;
    public bool isCollapsing {
        get {
            return _isCollapsing;
        }
        set {
            _isCollapsing = value;
            spring.distance = value ? 0 : SpreadDistance;
        }
    }

    void Awake () {
        headTransform = head.transform;
        coreTransform = core.transform;
        coreGlom = core.GetComponent<Glom>();

        headBody = head.GetComponent<Rigidbody2D>();
        coreBody = core.GetComponent<Rigidbody2D>();

        // set up sliders
        headSlider = head.AddComponent<SliderJoint2D>();
        coreSlider = core.AddComponent<SliderJoint2D>();

        headSlider.connectedBody = coreBody;
        coreSlider.connectedBody = headBody;

        headSlider.enabled = false;
        coreSlider.enabled = false;

        headSlider.autoConfigureAngle = false;
        coreSlider.autoConfigureAngle = false;

        // set up spring
        spring = core.GetComponent<SpringJoint2D>();
    }

    void Update () {
        UpdateStretchDetails();
    }

    private void UpdateStretchDetails() {
        stretchDistance = (headTransform.position - coreTransform.position).sqrMagnitude;
        stretchPercent = stretchDistance / MaxStretch;
    }

    public void Expand(Vector2 direction) {
        direction.Normalize();

        // toggle direction based on which side is glued
        Transform rootTransform;
        SliderJoint2D rootSlider;
        SliderJoint2D endSlider;
        Rigidbody2D endBody;

        if (coreGlom.isOn) {
            rootTransform = coreTransform;
            rootSlider = coreSlider;
            endSlider = headSlider;
            endBody = headBody;
        } else {
            rootTransform = headTransform;
            rootSlider = headSlider;
            endSlider = coreSlider;
            endBody = coreBody;
        }

        // set slider angle
        // TODO this could probably be managed with a single slider
        endSlider.enabled = false;
        rootSlider.enabled = true;
        rootSlider.angle = Vector2.Angle(Vector2.right, direction);
        if (direction.y < 0) {
            rootSlider.angle = rootSlider.angle * -1;
        }

        // update collapsing state and spring distance
        isCollapsing = false;

        // give a little nudge to get the spring going
        endBody.AddForce(direction, ForceMode2D.Impulse);
    }
}
