using UnityEngine;

public class VoxelClickInput : MonoBehaviour {
    VoxelGrid grid;
    const float ClickForce = 0.05f;

    private BoxCollider2D box;

    void Awake () {
        grid = GetComponent<VoxelGrid>();

        // initialize clickability
        box = gameObject.AddComponent<BoxCollider2D>();
        box.size = new Vector2(grid.width, grid.height);
        box.offset = new Vector2(grid.width / 2, grid.height / 2);
    }

    void Start() {

    }

    void FixedUpdate () {
        bool isLeftMouse = Input.GetMouseButton(0);
        bool isRightMouse = Input.GetMouseButton(1);

        if (isLeftMouse || isRightMouse) {
            Vector3 mouseClickPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D clickedCollider = Physics2D.OverlapPoint(mouseClickPoint);
            if (clickedCollider == box) {
                Vector2 localPoint = mouseClickPoint - transform.position;
                float value = grid.GetValueAt(localPoint);
                if (isLeftMouse) {
                    value += ClickForce;
                } else {
                    value -= ClickForce;
                }
                grid.SetVoxelAt(localPoint, value);
            }
        }
    }
}
