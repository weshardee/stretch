using UnityEngine;

public class Stretch : MonoBehaviour
{
    // editor references
    public GameObject head;
    public GameObject core;
    private SpringJoint2D spring;

    // constants
    public const float SpreadDistance = 1.5f;
    public const float DeadZone = 0.2f;
    public const float MaxStretch = 15f;

    // local references
    private Rigidbody2D headBody;
    private Rigidbody2D coreBody;
    private Transform headTransform;
    private Transform coreTransform;
    private Glom coreGlom;
    private SliderJoint2D headSlider;
    private SliderJoint2D slider;

    // stretching state
    public float stretchDistance { get; private set; }
    public float stretchPercent { get; private set; }
    public float angle { get; private set; }

    // states
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
            spring.distance = value ? 0 : SpreadDistance;
        }
    }

    void Awake()
    {
        // cache references
        headTransform = head.transform;
        coreTransform = core.transform;
        coreGlom = core.GetComponent<Glom>();
        headBody = head.GetComponent<Rigidbody2D>();
        coreBody = core.GetComponent<Rigidbody2D>();

        // set up slider
        slider = core.AddComponent<SliderJoint2D>();
        slider.connectedBody = headBody;
        slider.enabled = true;
        slider.autoConfigureAngle = false;

        // set up spring
        spring = core.GetComponent<SpringJoint2D>();
    }

    void FixedUpdate()
    {
        UpdateStretchDetails();
        slider.angle = angle;
    }

    private void UpdateStretchDetails()
    {
        stretchDistance = (headTransform.position - coreTransform.position).sqrMagnitude;
        stretchPercent = stretchDistance / MaxStretch;
    }

    public void Expand(Vector2 direction)
    {
        direction.Normalize();

        // toggle direction based on which side is glued
        Rigidbody2D endBody;
        Rigidbody2D rootBody;

        if (coreGlom.isOn)
        {
            endBody = headBody;
            rootBody = coreBody;
        }
        else
        {
            endBody = coreBody;
            rootBody = headBody;
        }

        // set slider angle
        angle = Vector2.Angle(Vector2.right, direction);
        if (direction.y < 0)
        {
            angle = angle * -1;
        }
        if (coreGlom.isOn)
        {
            angle -= 180;
        }
        slider.angle = angle;

        // update collapsing state and spring distance
        isCollapsing = false;

        // make sure the end starts off in the right direction
        // to prevent spring weirdness
        endBody.position = rootBody.position + direction * 0.01f;
        endBody.AddForce(direction, ForceMode2D.Force);
    }
}
