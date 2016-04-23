using UnityEngine;

public class Glom : MonoBehaviour {
    // editor components
    [SerializeField] private LayerMask layerMask;

    // local components
    private DistanceJoint2D glomJoint;

    // state flags
    private bool _isSticky;
    public bool isSticky {
        get {
            return _isSticky;
        }
        set {
            if (_isSticky == value) {
                return;
            }
            _isSticky = value;
            if (value) {
                On();
            } else {
                isOn = false;
            }
        }
    }

    public bool isOn {
        get {
            return glomJoint.enabled;
        }
        private set {
            glomJoint.enabled = value;
        }
    }

    private float _lastCollisionExpiration = 0;
    private Collision2D _lastCollision;
    private Collision2D lastCollision {
        get {
            if (_lastCollisionExpiration < Time.time) {
                return null;
            }
            return _lastCollision;
        }
        set {
            _lastCollision = value;
            _lastCollisionExpiration = Time.time + CollisionExitLag;
        }
    }

    // active glom info
    private const float Radius = 0.5f;

    // other
    private const float CollisionExitLag = 0f; // in seconds

    void Awake() {
        // create and configure joint
        glomJoint = gameObject.AddComponent<DistanceJoint2D>();
        glomJoint.enableCollision = true;
        glomJoint.anchor = new Vector2(0, 0.5f);
        glomJoint.autoConfigureConnectedAnchor = false;
        glomJoint.autoConfigureDistance = false;
        glomJoint.distance = 0;
        glomJoint.enableCollision = true;
        glomJoint.enabled = false;
    }

    void Update () {
        if (isOn) {
            Vector2 anchor = transform.TransformVector(glomJoint.anchor) + transform.position;
            Debug.DrawLine(anchor, transform.position, Color.blue);
            // Debug.DrawLine(anchorInWorldSpace, _GlomJoint.connectedAnchor, Color.blue);
        }
    }

    void OnCollisionStay2D(Collision2D coll) {
        TrackCollision(coll);
    }

    void OnCollisionExit2D(Collision2D coll) {
        TrackCollision(coll);
    }

    void TrackCollision(Collision2D coll) {
        lastCollision = coll;
        if (isSticky) {
            On();
        }
    }

    public bool On() {
        if (isOn) {
            return true;
        }

        // Debug.Log(name + ": try to glom");
        Collision2D coll = lastCollision;
        if (coll == null) {
            return false;
        }

        ContactPoint2D contactPoint = coll.contacts[0];

        // set the point of contact as the connected anchor point of the _GlomJoint
        Vector2 point = contactPoint.point;
        glomJoint.connectedAnchor = point;

        // set the anchor on the node to the direction of the contact point
        Vector2 anchorDirection = point - (Vector2)transform.position;
        glomJoint.anchor = anchorDirection.normalized * 0.5f;

        // TODO push glommed node to surface

        // set joint status
        isOn = true;
        return isOn;
    }
}
