using System;
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

    private static Color ColorOff = Color.black;
    private static Color ColorOn = Color.white;
    private const float Z = -1f;

    private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> triangles;
    private bool needsUpdate = false;

    public void Awake () {
        // calc sizes
        int voxelsX = (int)(Mathf.Ceil(width) * resolution) + 1;
        int voxelsY = (int)(Mathf.Ceil(height) * resolution) + 1;
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

        // initialize mesh
        Refresh();
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
        Vector3 globalXEdge = (offset + v.xEdgePosition);
        Vector3 globalYEdge = (offset + v.yEdgePosition);

        Debug.DrawLine(globalPosition, globalXEdge, Color.red);
        Debug.DrawLine(globalPosition, globalYEdge, Color.red);
    }

    void Refresh() {
        Triangulate();
        needsUpdate = false;
    }

    void Triangulate() {
        triangles.Clear();
        vertices.Clear();
        mesh.Clear();

        int cellsX = voxels.GetLength(0) - 1;
        int cellsY = voxels.GetLength(1) - 1;

        for (int x = 0; x < cellsX; x++) {
            for (int y = 0; y < cellsX; y++) {
                Voxel a = voxels[x, y]; // (0, 0)
                Voxel b = voxels[x + 1, y]; // (1, 0)
                Voxel c = voxels[x, y + 1]; // (0, 1)
                Voxel d = voxels[x + 1, y + 1]; // (1, 1)
                TriangulateCell(a, b, c, d);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
    }

    void TriangulateCell(Voxel a, Voxel b, Voxel c, Voxel d) {
        int maskA = a.state ? 1 << 0 : 0; // (0, 0)
        int maskB = b.state ? 1 << 1 : 0; // (1, 0)
        int maskC = c.state ? 1 << 2 : 0; // (0, 1)
        int maskD = d.state ? 1 << 3 : 0; // (1, 1)
        int finalMask = maskA | maskB | maskC | maskD;

        string binary = Convert.ToString(finalMask, 2);
        print(binary);

        // TODO eliminate this offset crap
        Vector2 offset = transform.position;
        Vector2 bottomLeft = a.position;
        Vector2 bottomMiddle = a.xEdgePosition;
        Vector2 leftMiddle = a.yEdgePosition;

        if (a.state) {
            AddTriangle(bottomLeft, leftMiddle, bottomMiddle);
        }
    }

    void AddTriangle(Vector3 a, Vector3 b, Vector3 c) {
        // TODO check if vertex exists and use that one if it does
        int i = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        triangles.Add(i);
        triangles.Add(i + 1);
        triangles.Add(i + 2);
    }

    void CreateVoxel(int x, int y) {
        GameObject o = Instantiate(voxelPrefab) as GameObject;
        o.transform.parent = transform;

        float localX = x * voxelSize;
        float localY = y * voxelSize;
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

        // set back to the grid
        voxels[x, y] = v;

        // queue refresh
        needsUpdate = true;
    }

    public void SetVoxelAt(Vector2 point, bool state) {
        point = point + Vector2.one * voxelSize / 2;
        int x = (int)(point.x * resolution);
        int y = (int)(point.y * resolution);
        SetVoxel(x, y, state);
    }
}
