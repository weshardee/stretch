using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (MeshFilter))]
[RequireComponent (typeof (MeshRenderer))]
[RequireComponent (typeof (Material))]
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
        public float value;
        public Vector2 position;
        public Vector2 xEdgePosition;
        public Vector2 yEdgePosition;
        public Material mat;
    }

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
    [Range(0, 1)]public float threshold = 0.5f;

    private static Vector3 GizmoSize = Vector3.one / 10;

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
        if (true) {
            Refresh();
        }
    }

    void OnDrawGizmos()
    {
        float cellSize = 1 / resolution;
        float cellsX = width * resolution;
        float cellsY = height * resolution;

        for (int y = 0; y < cellsY; y ++) {
            for (int x = 0; x < cellsX; x++) {
                DrawVoxelDebug(x, y);
            }
        }
    }

    void DrawVoxelDebug(int x, int y) {
        Vector2 offset = transform.position;

        // draw center point
        float size = 1 / resolution;
        Vector3 centerPoint = new Vector3(x * size, y * size, 0) + transform.position;
        Gizmos.DrawCube(centerPoint, GizmoSize);

        // draw edge positions if game is running
        if (voxels == null) {
            return;
        }
        Voxel v = voxels[x, y];
        Vector3 globalPosition = (offset + v.position);
        Vector3 globalXEdge = (offset + v.xEdgePosition);
        Vector3 globalYEdge = (offset + v.yEdgePosition);

        if (v.xEdgePosition != Vector2.zero) Debug.DrawLine(globalPosition, globalXEdge, Color.red);
        if (v.yEdgePosition != Vector2.zero) Debug.DrawLine(globalPosition, globalYEdge, Color.red);
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
            print(x);
            for (int y = 0; y < cellsX; y++) {
                Voxel sw = voxels[x + 0, y + 0]; // (0, 0)
                Voxel se = voxels[x + 1, y + 0]; // (1, 0)
                Voxel ne = voxels[x + 1, y + 1]; // (1, 1)
                Voxel nw = voxels[x + 0, y + 1]; // (0, 1)

                // Interpolate edges
                InterpolateEdges(x, y);
                TriangulateCell(sw, nw, ne, se);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
    }

    void InterpolateEdges(int x, int y) {
        Voxel sw = voxels[x + 0, y + 0]; // (0, 0)
        Voxel se = voxels[x + 1, y + 0]; // (1, 0)
        Voxel ne = voxels[x + 1, y + 1]; // (1, 1)
        Voxel nw = voxels[x + 0, y + 1]; // (0, 1)

        if (y == 0) sw.xEdgePosition = LerpEdge(sw, se);
        if (x == 0) sw.yEdgePosition = LerpEdge(sw, nw);
        nw.xEdgePosition = LerpEdge(nw, ne);
        se.yEdgePosition = LerpEdge(se, ne);

        voxels[x + 0, y + 0] = sw; // (0, 0)
        voxels[x + 1, y + 0] = se; // (1, 0)
        voxels[x + 1, y + 1] = ne; // (1, 1)
        voxels[x + 0, y + 1] = nw; // (0, 1)
    }

    Vector2 LerpEdge(Voxel a, Voxel b) {
        // only lerp if there's an edge
        if (a.value < threshold == b.value < threshold)
        {
            return a.position;
        }

        float lerp = (threshold - a.value) / (b.value - a.value);

        return Vector2.Lerp(a.position, b.position, lerp);
    }

    void TriangulateCell(Voxel a, Voxel b, Voxel c, Voxel d) {
        // expected shape:
        // [b, c]
        // [a, d]

        int maskA = a.value > threshold ? 1 << 3 : 0; // (0, 0)
        int maskB = b.value > threshold ? 1 << 2 : 0; // (1, 0)
        int maskC = c.value > threshold ? 1 << 1 : 0; // (1, 1)
        int maskD = d.value > threshold ? 1 << 0 : 0; // (0, 1)
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
                AddQuad(cornerA, cornerB, cornerC, cornerD);
                break;
            case MarchStates.A:
                AddTriangle(cornerA, edgeAB, edgeDA);
                break;
            case MarchStates.B:
                AddTriangle(cornerB, edgeBC, edgeAB);
                break;
            case MarchStates.D:
                AddTriangle(cornerD, edgeDA, edgeCD);
                break;
            case MarchStates.C:
                AddTriangle(cornerC, edgeCD, edgeBC);
                break;
            case MarchStates.AB:
                AddQuad(cornerA, cornerB, edgeBC, edgeDA);
                break;
            case MarchStates.DA:
                AddQuad(cornerD, cornerA, edgeAB, edgeCD);
                break;
            case MarchStates.CD:
                AddQuad(cornerC, cornerD, edgeDA, edgeBC);
                break;
            case MarchStates.BC:
                AddQuad(cornerB, cornerC, edgeCD, edgeAB);
                break;
            case MarchStates.AC:
                AddPentagon(cornerA, edgeAB, edgeBC, cornerC, edgeCD);
                AddTriangle(cornerA, edgeCD, edgeDA);
                break;
            case MarchStates.BD:
                AddPentagon(cornerB, edgeBC, edgeCD, cornerD, edgeDA);
                AddTriangle(cornerB, edgeDA, edgeAB);
                break;
            case MarchStates.BCD:
                AddPentagon(cornerC, cornerD, edgeDA, edgeAB, cornerB);
                break;
            case MarchStates.ACD:
                AddPentagon(cornerD, cornerA, edgeAB, edgeBC, cornerC);
                break;
            case MarchStates.ABC:
                AddPentagon(cornerB, cornerC, edgeCD, edgeDA, cornerA);
                break;
            case MarchStates.ABD:
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

    void CreateVoxel(int x, int y)
    {
        Voxel v = voxels[x, y];

        // calc position
        float localX = x * voxelSize;
        float localY = y * voxelSize;
        v.position = new Vector2(localX, localY);

        // set initial edge positions
        float halfVoxelSize = voxelSize / 2f;
        v.xEdgePosition = v.position;
        v.yEdgePosition = v.position;

        voxels[x, y] = v;
        SetVoxel(x, y, 0);
    }

    public void SetVoxel(int x, int y, float value)
    {
        // verify this is in bounds
        if (x >= voxels.GetLength(0) || x < 0) return;
        if (y >= voxels.GetLength(1) || y < 0) return;

        // clamp value
        value = value > 1 ? 1 : value;
        value = value < 0 ? 0 : value;

        // store that value
        Voxel v = voxels[x, y];
        v.value = value;
        // v.mat.color = Color.Lerp(ColorOn, ColorOff, value);

        // set back to the grid
        voxels[x, y] = v;

        // queue refresh
        needsUpdate = true;
    }

    public void SetVoxelAt(Vector2 point, float value) {
        int x, y;
        PointToGridCoord(point, out x, out y);
        SetVoxel(x, y, value);
    }

    public float GetValueAt(Vector2 point) {
        int x, y;
        PointToGridCoord(point, out x, out y);
        return voxels[x, y].value;
    }

    private void PointToGridCoord(Vector2 point, out int x, out int y)
    {
        point = point + Vector2.one * voxelSize / 2;
        x = (int)(point.x * resolution);
        y = (int)(point.y * resolution);
    }

    public Vector2 ToGlobalPosition(float x, float y) {
        x = x / resolution;
        y = y / resolution;

        Vector2 position = new Vector2(x, y);
        return position + (Vector2)transform.position;
    }
}
