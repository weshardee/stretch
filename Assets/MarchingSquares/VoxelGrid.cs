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

    Voxel[,] voxels;
    Material[,] mats;
    private float voxelSize;

    private static Color ColorOn = Color.black;
    private static Color ColorOff = Color.white;
    private const float Z = -1f;

    private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> triangles;

    public void Initialize (int resolution, float size) {
        voxels = new Voxel[resolution, resolution];
        mats = new Material[resolution, resolution];
        voxelSize = size / resolution;

        // initialize voxels
        for (int i = 0, y = 0; y < resolution; y++) {
            for (int x = 0; x < resolution; x++, i++) {
                CreateVoxel(i, x, y);
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

    }

    void Refresh() {
        // mesh.SetVertices(vertices);
        // Triangulate();
    }

    void Triangulate() {
        // mesh.SetTriangles(triangles);
    }

    void CreateVoxel(int i, int x, int y) {
        GameObject o = Instantiate(voxelPrefab) as GameObject;
        o.transform.parent = transform;

        float localX = (x + 0.5f) * voxelSize;
        float localY = (y + 0.5f) * voxelSize;
        o.transform.localPosition = new Vector3(localX, localY, Z);
        o.transform.localScale = Vector2.one * voxelSize * 0.1f;

        voxels[x, y].mat = o.GetComponent<Renderer>().material;
        SetVoxel(x, y, false);
    }

    public void SetVoxel(int x, int y, bool state) {
        Voxel v = voxels[x, y];
        v.state = state;
        v.mat.color = state ? ColorOn : ColorOff;
        Refresh();
    }
}
