using System;
using System.Collections.Generic;
using UnityEngine;

public class VoxelGrid : MonoBehaviour {
    enum MarchStates {
        Empty, // 0000
        D, // 0001
        C, // 0010
        CD, // 0011
        B, // 0100
        BD, // 0101
        BC, // 0110
        BCD, // 0111
        A, // 1000
        DA, // 1001
        AC, // 1010
        ACD, // 1011
        AB, // 1100
        ABD, // 1101
        ABC, // 1110
        Filled // 1111
    }

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
                Voxel sw = voxels[x + 0, y + 0]; // (0, 0)
                Voxel se = voxels[x + 1, y + 0]; // (1, 0)
                Voxel ne = voxels[x + 1, y + 1]; // (1, 1)
                Voxel nw = voxels[x + 0, y + 1]; // (0, 1)
                TriangulateCell(sw, nw, ne, se);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
    }

    void TriangulateCell(Voxel a, Voxel b, Voxel c, Voxel d) {
        // expected shape:
        // [d, c]
        // [a, b]

        int maskA = a.state ? 1 << 3 : 0; // (0, 0)
        int maskB = b.state ? 1 << 2 : 0; // (1, 0)
        int maskC = c.state ? 1 << 1 : 0; // (1, 1)
        int maskD = d.state ? 1 << 0 : 0; // (0, 1)
        int finalMask = maskA | maskB | maskC | maskD;

        if (finalMask == 0) {
            return;
        }

        // TODO maybe only make the ones I need?
        Vector2 cornerA = a.position;
        Vector2 cornerB = b.position;
        Vector2 cornerC = c.position;
        Vector2 cornerD = d.position;
        Vector2 edgeAB = a.yEdgePosition;
        Vector2 edgeBC = b.xEdgePosition;
        Vector2 edgeCD = d.yEdgePosition;
        Vector2 edgeDA = a.xEdgePosition;

        MarchStates state = (MarchStates)finalMask;
        switch (state)
        {
            case MarchStates.Filled:
                print(MarchStates.Filled);
                AddQuad(cornerA, cornerB, cornerC, cornerD);
                break;
            case MarchStates.A:
                print(MarchStates.A);
                AddTriangle(cornerA, edgeAB, edgeDA);
                break;
            case MarchStates.B:
                print(MarchStates.B);
                AddTriangle(cornerB, edgeBC, edgeAB);
                break;
            case MarchStates.D:
                print(MarchStates.D);
                AddTriangle(cornerD, edgeDA, edgeCD);
                break;
            case MarchStates.C:
                print(MarchStates.C);
                AddTriangle(cornerC, edgeCD, edgeBC);
                break;
            case MarchStates.AB:
                print(MarchStates.AB);
                AddQuad(cornerA, cornerB, edgeBC, edgeDA);
                break;
            case MarchStates.DA:
                print(MarchStates.DA);
                AddQuad(cornerD, cornerA, edgeAB, edgeCD);
                break;
            case MarchStates.CD:
                print(MarchStates.CD);
                AddQuad(cornerC, cornerD, edgeDA, edgeBC);
                break;
            case MarchStates.BC:
                print(MarchStates.BC);
                AddQuad(cornerB, cornerC, edgeCD, edgeAB);
                break;
            case MarchStates.AC:
                print(MarchStates.AC);
                AddTriangle(cornerA, edgeAB, edgeDA);
                AddTriangle(cornerC, edgeCD, edgeBC);
                break;
            case MarchStates.BD:
                print(MarchStates.BD);
                AddTriangle(cornerB, edgeBC, edgeAB);
                AddTriangle(cornerD, edgeDA, edgeCD);
                break;
            case MarchStates.BCD:
                print(MarchStates.BCD);
                AddPentagon(cornerC, cornerD, edgeDA, edgeAB, cornerB);
                break;
            case MarchStates.ACD:
                print(MarchStates.ACD);
                AddPentagon(cornerD, cornerA, edgeAB, edgeBC, cornerC);
                break;
            case MarchStates.ABC:
                print(MarchStates.ABC);
                AddPentagon(cornerB, cornerC, edgeCD, edgeDA, cornerA);
                break;
            case MarchStates.ABD:
                print(MarchStates.ABD);
                AddPentagon(cornerA, cornerB, edgeBC, edgeCD, cornerD);
                break;
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

    void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d) {
        AddTriangle(a, b, c);
        AddTriangle(a, c, d);
    }

    void AddPentagon(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 e) {
        AddTriangle(a, b, c);
        AddTriangle(a, c, d);
        AddTriangle(a, d, e);
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
