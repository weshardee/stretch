using UnityEngine;

public class Glom : MonoBehaviour {
    // editor components
    [SerializeField] private LayerMask layerMask;

    // local components
    private DistanceJoint2D _GlomJoint;

    // state flags
    private bool _isSticky;
    public bool IsSticky {
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
                IsOn = false;
            }
        }
    }

    public bool IsOn {
        get {
            return _GlomJoint.enabled;
        }
        private set {
            _GlomJoint.enabled = value;
        }
    }

    private float _lastCollisionExpiration = 0;
    private Collision2D _lastCollision;
    private Collision2D _LastCollision {
        get {
            if (_lastCollisionExpiration < Time.time) {
                return null;
            }
            return _lastCollision;
        }
        set {
            _lastCollision = value;
            _lastCollisionExpiration = Time.time + _CollisionExitLag;
        }
    }

    // active glom info
    private const float _Radius = 0.5f;

    // other
    private const float _CollisionExitLag = 0f; // in seconds

    void Awake() {
        // create and configure joint
        _GlomJoint = gameObject.AddComponent<DistanceJoint2D>();
        _GlomJoint.enableCollision = true;
        _GlomJoint.anchor = new Vector2(0, 0.5f);
        _GlomJoint.autoConfigureConnectedAnchor = false;
        _GlomJoint.autoConfigureDistance = false;
        _GlomJoint.distance = 0;
        _GlomJoint.enableCollision = true;
        _GlomJoint.enabled = false;
    }

    void Update () {
        if (IsOn) {
            Vector2 anchor = transform.TransformVector(_GlomJoint.anchor) + transform.position;
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
        _LastCollision = coll;
        if (IsSticky) {
            On();
        }
    }

    public bool On() {
        if (IsOn) {
            return true;
        }

        // Debug.Log(name + ": try to glom");
        Collision2D coll = _LastCollision;
        if (coll == null) {
            return false;
        }

        ContactPoint2D contactPoint = coll.contacts[0];

        // set the point of contact as the connected anchor point of the _GlomJoint
        Vector2 point = contactPoint.point;
        _GlomJoint.connectedAnchor = point;

        // set the anchor on the node to the direction of the contact point
        Vector2 anchorDirection = point - (Vector2)transform.position;
        _GlomJoint.anchor = anchorDirection.normalized * 0.5f;

        // TODO push glommed node to surface

        // set joint status
        IsOn = true;
        return IsOn;
    }

    public void Swap(Glom glom) {
        if (glom.IsOn) {
            _LastCollision = glom._lastCollision;
            On();
        }
    }
}
