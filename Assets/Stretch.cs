using UnityEngine;

public class Stretch : MonoBehaviour
{
    // editor references
    public GameObject head;
    public GameObject core;
    public SpringJoint2D collapseSpring;

    // constants
    public const float SpreadDistance = 2f;
    public const float DeadZone = 0.2f;
    public const float MaxStretch = 15f;

    // local references
    private Transform headTransform;
    private Transform coreTransform;
    private SpringJoint2D headTarget;
    private SpringJoint2D coreTarget;
    private Glom coreGlom;
    private SliderJoint2D headSlider;
    private SliderJoint2D coreSlider;

    // stretching state
    public Vector2 spread;
    public float stretchDistance { get; private set; }
    public float stretchPercent { get; private set; }

    // states
    private bool _isHolding = false;
    public bool isHolding
    {
        get
        {
            return _isHolding;
        }
        set
        {
            _isHolding = value;
            if (value)
            {
                isExpanding = false;
                isCollapsing = false;
            }
        }
    }

    private bool _isCollapsing = true;
    public bool isCollapsing
    {
        get
        {
            return _isCollapsing;
        }
        set
        {
            _isCollapsing = value;
            if (value)
            {
                isExpanding = false;
                isHolding = false;
            }

            // set collapse spring state
            collapseSpring.enabled = value;
        }
    }

    private bool _isExpanding = false;
    public bool isExpanding
    {
        get
        {
            return _isExpanding;
        }
        set
        {
            _isExpanding = value;
            if (value) {
                isColl
           apsing = false;
                isHolding = false;
            }

            // enable expand targets
            headTarget.enabled = value;
            coreTarget.enabled = value;
        }
    }

    void Awake () {
        headTransform = head.transform;
        coreTransform = core.transform;

        headTarget = head.AddComponent<SpringJoint2D>();
        coreTarget = core.AddComponent<SpringJoint2D>();

        coreGlom = core.GetComponent<Glom>();

        // set up sliders
        headSlider = head.AddComponent<SliderJoint2D>();
        coreSlider = core.AddComponent<SliderJoint2D>();

        headSlider.connectedBody = core.GetComponent<Rigidbody2D>();
        coreSlider.connectedBody = head.GetComponent<Rigidbody2D>();

        headSlider.enabled = false;
        coreSlider.enabled = false;

        headSlider.autoConfigureAngle = false;
        coreSlider.autoConfigureAngle = false;

        // configure targets
        ConfigureTarget(headTarget);
        ConfigureTarget(coreTarget);
    }

    void ConfigureTarget(SpringJoint2D target) {
        target.autoConfigureDistance = false;
        target.distance = 0;

        target.dampingRatio = 1;
        target.enableCollision = true;
        target.enabled = false;
        target.frequency = 2.5f;
    }

    void Update () {
        UpdateStretchDetails();
    }()


    private void UpdateStretchDetails() {
        stretchDistance = (headTransform.position - coreTransform.position).sqrMagnitude;
        stretchPercent = stretchDistanc
   e / MaxStretch;
    }

    public void Expand(Vector2 direction) {
        isExpanding = true;
        Vector2 distance = direction.norm
   alized * SpreadDistance;

        SpringJoint2D rootTarget;
        SpringJoint2D endTarget;
        Transform rootTransform;
        SliderJoint2D rootSlider;
        SliderJoint2D endSlider;

        // toggle direction based on which side is glued
        if (coreGlom.isOn) {
            rootTarget = coreTarget;
            rootTransform
       = coreTransform;
            rootSlider = coreSlider;
            endTarget = headTarget;
            endSlider = headSlider;
        } else {
            rootTarget = headTarget;

        else
       otTransform = headTransform;
            rootSlider = headSlider;
            endTarget = coreTarget;
            endSlider = coreSlider;
        }

        // set slider angle
        endSlider.enabled = false;
        rootSlider.enabled = true;
        rootSlider.angle = Vector2.Angle(Vector2.right, direction);
        if (direction.y < 0) {
            rootSlider.angle = rootSlider.angle * -1;
        }


        // TODO this could probably be managed with a single slider

        // set stretch targets
        endTarget.connectedAnchor = (Vector2)rootTransform.position + distance;
        rootTarget.connectedAnchor = (Vector2)rootTransform.position;

        // draw debug lines
        Debug.DrawLine(rootTarget.connectedAnchor, rootTransform.position, Color.green);
        Debug.DrawLine(endTarget.connectedAnchor, rootTransform.position, Color.green);
    }
}
