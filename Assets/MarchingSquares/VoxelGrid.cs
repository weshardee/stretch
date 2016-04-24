using System.Collections.Generic;
using UnityEngine;

public class VoxelGrid : MonoBehaviour {
    struct Voxel {
        public bool state;
        public Vector2 position;
        public Vector2 xEdgePosition;
        public Vector2 yEdgePosition;
        public Material mat;
    }

    [SerializeField]
    private GameObject voxelPrefab;
    public float resolution;
    public float width;
    public float height;

    Voxel[,] voxels;
    Material[,] mats;
    private float voxelSize;

    private static Color ColorOn = Color.black;
    private static Color ColorOff = Color.white;
    private const float Z = -1f;

    private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> triangles;
    private bool needsUpdate = false;

    public void Awake () {
        // calc sizes
        int voxelsX = (int)(Mathf.Ceil(width) * resolution);
        int voxelsY = (int)(Mathf.Ceil(height) * resolution);
        voxelSize = 1f / resolution;

        // initialize voxels
        voxels = new Voxel[voxelsX, voxelsY];
        for (int y = 0; y < voxelsY; y++) {
            for (int x = 0; x < voxelsX; x++) {
                CreateVoxel(x, y);
            }
        }

        // make the mesh
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.name = "VoxelGrid Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        // Refresh();
    }

    void Start() {

    }

    void Update () {
        for (int y = 0; y < voxels.GetLength(1); y++) {
            for (int x = 0; x < voxels.GetLength(0); x++) {
                DrawVoxelDebug(x, y);
            }
        }

        if (needsUpdate) {
            Refresh();
        }
    }

    void DrawVoxelDebug(int x, int y) {
        Vector2 offset = transform.position;
        Voxel v = voxels[x, y];
        Vector3 globalPosition = (offset + v.position);

        Debug.DrawLine(globalPosition, Vector2.zero, Color.red, 10f);
    }

    void Refresh() {
        // mesh.SetVertices(vertices);
        // Triangulate();
        needsUpdate = false;
    }

    void Triangulate() {
        // mesh.SetTriangles(triangles);
    }

    void CreateVoxel(int x, int y) {
        GameObject o = Instantiate(voxelPrefab) as GameObject;
        o.transform.parent = transform;

        float localX = (x + 0.5f) * voxelSize;
        float localY = (y + 0.5f) * voxelSize;
        o.transform.localPosition = new Vector3(localX, localY, Z);
        o.transform.localScale = Vector2.one * voxelSize * 0.1f;

        // set voxel material
        Voxel v = voxels[x, y];
        v.mat = o.GetComponent<Renderer>().material;

        // set initial voxel positions
        float halfVoxelSize = voxelSize / 2f;
        v.position = o.transform.localPosition;
        v.xEdgePosition = v.position + Vector2.right * halfVoxelSize;
        v.yEdgePosition = v.position + Vector2.up * halfVoxelSize;

        voxels[x, y] = v;
        SetVoxel(x, y, false);
    }

    public void SetVoxel(int x, int y, bool state) {
        Voxel v = voxels[x, y];
        v.state = state;
        v.mat.color = state ? ColorOn : ColorOff;

        // queue refresh
        needsUpdate = true;
    }

    public void SetVoxelAt(Vector2 point, bool state) {
        int x = (int)(point.x * resolution);
        int y = (int)(point.y * resolution);
        SetVoxel(x, y, state);
    }
}
