using UnityEngine;

public class PlayerMesh : MonoBehaviour {
    struct GridPoint {
        public bool value;
        public Vector2 position;
    }

    private Stretch stretch;
    private const float EndRadius = 0.5f;
    private Transform end1;
    private Transform end2;
    private Color InsideColor = Color.green;
    private Color OutsideColor = Color.red;
    private const int Resolution = 10;
    private const float InfluenceThreshold = 0.99f;
    private const float InfluenceZeroRadius = 1f;

    void Start () {
        stretch = GetComponent<Stretch>();
        end1 = stretch.head.transform;
        end2 = stretch.core.transform;
    }

	// Update is called once per frame
	void Update () {
        // TODO draw a debug cube around the area to iterate over
        float currentDistance = stretch.stretchDistance;

        Vector2 pos1 = end1.position;
        Vector2 pos2 = end2.position;
        Vector2 buffer = Vector2.one * EndRadius;

        // calculate bounds
        // TODO use scale to tighten the buffer
        Vector2 corner1 = Vector2.Max(pos1 + buffer, pos2 + buffer);
        Vector2 corner2 = Vector2.Min(pos1 - buffer, pos2 - buffer);
        // Vector2 corner3 = new Vector2(corner1.x, corner2.y);
        // Vector2 corner4 = new Vector2(corner2.x, corner1.y);

        float width = corner1.x - corner2.x;
        float height = corner1.y - corner2.y;
        int gridWidth = (int)(width * Resolution) + 1;
        int gridHeight = (int)(height * Resolution) + 1;

        // draw debug bounds
        // Debug.DrawLine(corner1, corner3, BoundsColor);
        // Debug.DrawLine(corner1, corner4, BoundsColor);
        // Debug.DrawLine(corner2, corner3, BoundsColor);
        // Debug.DrawLine(corner2, corner4, BoundsColor);

        // calculate a grid of in/out positions
        GridPoint[,] grid = new GridPoint[gridWidth,gridHeight];
        for (int x = 0; x < grid.GetLength(0); x++) {
            for (int y = 0; y < grid.GetLength(1); y++) {
                // instantiate the current grid point
                GridPoint gridPoint = new GridPoint();
                gridPoint.position = WorldPositionFromGridCoordsVector(x, y, corner2);

                // save it to the grid
                grid[x, y] = gridPoint;


                // TODO some math to figure the state of the gridpoint
                // calculate the weight from each end
                float pow1 = CalcInfluence(end1, gridPoint.position);
                float pow2 = CalcInfluence(end2, gridPoint.position);
                gridPoint.value = pow1 + pow2 >= InfluenceThreshold;
                if (x == 0 && y == 0) Debug.Log(pow1 + pow2);

                // draw debug grid
                if (y > 0) DrawDebugGridLine(gridPoint, grid[x, y - 1]);
                if (x > 0) DrawDebugGridLine(gridPoint, grid[x - 1, y]);
            }
        }
    }

    void DrawDebugGridLine(GridPoint a, GridPoint b) {
        Color color = a.value || b.value ? InsideColor : OutsideColor;
        Debug.DrawLine(a.position, b.position, color);
    }

    Vector2 WorldPositionFromGridCoordsVector(int x, int y, Vector2 offset) {
        return new Vector2(x, y) / Resolution + offset;
    }

    float CalcInfluence(Transform end, Vector2 point) {
        float r = (point - (Vector2)end.position).sqrMagnitude;
        if (r > InfluenceZeroRadius) {
            return 0;
        }

        // adjust radius by a factor
        r /= InfluenceZeroRadius;

        float power = r * r * r * (r * (r * 6 - 15) + 10); // http://www.geisswerks.com/ryan/BLOBS/blobs.html
        return 1 - power;
    }
}
