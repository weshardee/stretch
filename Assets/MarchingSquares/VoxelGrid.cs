using System.Collections.Generic;
using UnityEngine;

public class VoxelGrid : MonoBehaviour {
    [SerializeField]
    private GameObject voxelPrefab;

    bool[,] voxels;
    Material[,] mats;
    private float voxelSize;

    private static Color ColorOn = Color.black;
    private static Color ColorOff = Color.white;
    private const float Z = -1f;

    private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> triangles;

    public void Initialize (int resolution, float size) {
        voxels = new bool[resolution, resolution];
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
        Refresh();
    }

    void Start() {

    }

    void Update () {

    }

    void Refresh() {
        mesh.SetVertices(vertices);
        Triangulate();
        RefreshVoxelColors();
    }

    void Triangulate() {
        // mesh.SetTriangles(triangles);
    }

    void RefreshVoxelColors() {
        for (int x = 0; x < mats.GetLength(0); x++) {
            for (int y = 0; y < mats.GetLength(1); y++) {
                bool state = voxels[x, y];
                mats[x, y].color = state ? ColorOn : ColorOff;
            }
        }
    }

    void CreateVoxel(int i, int x, int y) {
        GameObject v = Instantiate(voxelPrefab) as GameObject;
        v.transform.parent = transform;

        float localX = (x + 0.5f) * voxelSize;
        float localY = (y + 0.5f) * voxelSize;
        v.transform.localPosition = new Vector3(localX, localY, Z);
        v.transform.localScale = Vector2.one * voxelSize * 0.1f;
        mats[x, y] = v.GetComponent<Renderer>().material;
        SetVoxel(x, y, false);
    }

    public void SetVoxel(int x, int y, bool state) {
        voxels[x, y] = state;
        mats[x, y].color = state ? ColorOn : ColorOff;
        Refresh();
    }
}
