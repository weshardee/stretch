using UnityEngine;

public class PlayerMesh : MonoBehaviour
{
    struct GridPoint
    {
        public bool value;
        public Vector2 position;
    }

    private Stretch stretch;
    private const float EndRadius = 0.5f;
    private Transform end1;
    private Transform end2;
    private Color InsideColor = Color.green;
    private Color OutsideColor = Color.red;
    private float resolution = 10f;
    private const float InfluenceThreshold = 0.99f;
    private const float InfluenceZeroRadius = 1f;
    private VoxelGrid grid;
    private float[,] weights;
    private Vector2[,] points;
    private float threshold;

    void Start()
    {
        // cache components
        grid = GetComponentInChildren<VoxelGrid>();
        stretch = GetComponent<Stretch>();
        end1 = stretch.head.transform;
        end2 = stretch.core.transform;

        // cache properties
        resolution = grid.resolution;
        threshold = grid.threshold;
    }

    // Update is called once per frame
    void Update()
    {
        float currentDistance = stretch.stretchDistance;

        Vector2 pos1 = end1.position;
        Vector2 pos2 = end2.position;
        Vector2 buffer = Vector2.one * EndRadius;

        // calculate bounds; use node radius as a buffer
        // TODO use scale to tighten the buffer
        Vector2 corner1 = Vector2.Max(pos1 + buffer, pos2 + buffer); // upper right
        Vector2 corner2 = Vector2.Min(pos1 - buffer, pos2 - buffer); // bottom left

        float width = corner1.x - corner2.x;
        float height = corner1.y - corner2.y;
        int gridWidth = (int)(width * resolution) + 1;
        int gridHeight = (int)(height * resolution) + 1;

        // calculate a grid of in/out positions
        weights = new float[gridWidth, gridHeight];
        points = new Vector2[gridWidth, gridHeight];

        for (int x = 0; x < weights.GetLength(0); x++)
        {
            for (int y = 0; y < weights.GetLength(1); y++)
            {
                // instantiate the current grid point
                Vector2 point = WorldPositionFromGridCoordsVector(x, y, corner2);
                points[x, y] = point;

                // TODO some math to figure the state of the gridpoint
                // calculate the weight from each end
                float pow1 = CalcInfluence(end1, point);
                float pow2 = CalcInfluence(end2, point);

                // save it to the grid
                weights[x, y] = pow1 + pow2;
            }
        }

        // grid.Use(weights);
        grid.transform.position = corner2;
    }

    void OnDrawGizmos()
    {
        if (weights == null)
        {
            return;
        }
        for (int x = 0; x < weights.GetLength(0); x++)
        {
            for (int y = 0; y < weights.GetLength(1); y++)
            {
                DrawDebugPoint(x, y);
            }
        }
    }

    void DrawDebugPoint(int x, int y)
    {
        Vector2 point = points[x, y];
        float weight = weights[x, y];

        Color color = weight > threshold ? InsideColor : OutsideColor;
        Debug.DrawRay(point, Vector3.one / resolution / 2, color);
    }

    Vector2 WorldPositionFromGridCoordsVector(int x, int y, Vector2 offset)
    {
        return new Vector2(x, y) / resolution + offset;
    }

    float CalcInfluence(Transform end, Vector2 point)
    {
        float r = (point - (Vector2)end.position).sqrMagnitude;
        if (r > InfluenceZeroRadius)
        {
            return 0;
        }

        // adjust radius by a factor
        r /= InfluenceZeroRadius;

        float power = r * r * r * (r * (r * 6 - 15) + 10); // http://www.geisswerks.com/ryan/BLOBS/blobs.html
        return 1 - power;
    }
}
