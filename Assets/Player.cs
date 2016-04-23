using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public enum PlayerState {
    Loose,
    Grounded,
    Reach,
    Grab,
    Pull,
    CollapsingToCore,
}

public class Player : MonoBehaviour {

    // editor references
    public Transform head;
    public Transform core;
    public SpringJoint2D collapseSpring;

    // local references
    private Glom headGlom;
    private Glom coreGlom;
    private Stretch stretch;
    private Rigidbody2D coreBody;
    private Rigidbody2D headBody;

    // constants
    public const float StretchForce = 13f;
    public const float DeadZone = 0.2f;
    public const float InputReleaseThreshold = 0.1f;
    public const float MaxStretch = 15f;
    public const float GrabDuration = 0.1f;
    public const float ReachDuration = 40f;
    public const float PullReleaseDistanceThreshold = 0.05f;

    // state
    private Vector2 reachDirection;
    private PlayerState state = PlayerState.Loose;
    private float grabTimeout = 0;
    private float reachTimeout = 0;
    private bool useGravity {
        set {
            float gravityScale = value ? 1 : 0;
            headBody.gravityScale = gravityScale;
            coreBody.gravityScale = gravityScale;
        }
    }

    void Awake () {
        stretch = GetComponent<Stretch>();

        headGlom = head.GetComponent<Glom>();
        headBody = head.GetComponent<Rigidbody2D>();

        coreGlom = core.GetComponent<Glom>();
        coreBody = core.GetComponent<Rigidbody2D>();

        // disable front glom at start
        headGlom.isSticky = false;
    }

    void Update () {
        // Debug.Log(_State);
        switch (state) {
            case PlayerState.Loose: {
                useGravity = true;
                stretch.isCollapsing = true;
                coreGlom.isSticky = true;
                headGlom.isSticky = false;

                // change state if core gets glommed
                if (coreGlom.isOn) {
                    state = PlayerState.Grounded;
                }
                break;
            }
            case PlayerState.Grounded: {
                useGravity = false;
                stretch.isCollapsing = true;
                coreGlom.isSticky = true;
                headGlom.isSticky = false;

                // change state if input starts
                reachDirection = GetInput();
                if (reachDirection != Vector2.zero) {
                    Reach();
                }
                break;
            }
            case PlayerState.Reach: {
                useGravity = false;
                coreGlom.isSticky = true;
                headGlom.isSticky = false;

                stretch.Expand(reachDirection);

                if (reachTimeout < Time.time || headBody.IsSleeping()) {
                    Grab();
                }
                break;
            }
            case PlayerState.Grab: {
                useGravity = false;
                coreGlom.isSticky = true;
                headGlom.isSticky = true;

                if (headGlom.isOn) {
                    state = PlayerState.Pull;
                } else if (grabTimeout < Time.time) {
                    state = PlayerState.Loose;
                }
                break;
            }
            case PlayerState.Pull: {
                useGravity = false;
                stretch.isCollapsing = true;
                coreGlom.isSticky = false;
                headGlom.isSticky = true;

                bool isFinishedPulling = stretch.stretchDistance < PullReleaseDistanceThreshold;

                if (isFinishedPulling) {
                    SwapEnds();
                    state = PlayerState.Grounded;
                }
                break;
            }
        }
    }

    private void Reach() {
        reachTimeout = Time.time + ReachDuration;
        state = PlayerState.Grab;
    }

    private void Grab() {
        grabTimeout = Time.time + GrabDuration;
        state = PlayerState.Grab;
    }

    private Vector2 GetInput() {
        // Read input
        float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        float vertical = CrossPlatformInputManager.GetAxis("Vertical");

        // combine axes
        Vector2 input = new Vector2(horizontal, vertical);

        // normalize input if it exceeds 1 in combined length:
        if (input.sqrMagnitude > 1) {
            input.Normalize();
        }

        return input;
    }

    private void SwapEnds() {
        Glom tempGlom = headGlom;
        Rigidbody2D tempBody = headBody;

        headBody = coreBody;
        headGlom = coreGlom;

        coreBody = tempBody;
        coreGlom = tempGlom;
    }
}
