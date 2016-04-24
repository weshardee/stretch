﻿using UnityEngine;

public class VoxelMap : MonoBehaviour {
    [SerializeField]
    private VoxelGrid voxelGridPrefab;

    [SerializeField]
    private int size;
    [SerializeField]
    private int chunkResolution;
    [SerializeField]
    private int voxelResolution;

    VoxelGrid[] chunks;

    private float halfSize;
    private float voxelSize;
    private float chunkSize;

    private BoxCollider2D box;

    void Awake () {
        halfSize = size / 2;
        chunkSize = size / chunkResolution;
        voxelSize = chunkSize / voxelResolution;

        chunks = new VoxelGrid[size * size];

        // initialize voxels
        for (int i = 0, y = 0; y < size; y++) {
            for (int x = 0; x < size; x++, i++) {
                CreateChunk(i, x, y);
            }
        }

        // initialize clickability
        box = gameObject.AddComponent<BoxCollider2D>();
        box.size = Vector2.one * size;
    }

    void Start() {

    }

    void Update () {
        bool isLeftMouse = Input.GetMouseButton(0);
        bool isRightMouse = Input.GetMouseButton(1);
        if (isLeftMouse || isRightMouse) {
            Vector3 mouseClickPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D clickedCollider = Physics2D.OverlapPoint(mouseClickPoint);
            if (clickedCollider = box) {
                SetVoxel(mouseClickPoint - transform.position + Vector3.one * halfSize, isLeftMouse);
            }
        }
    }

    void CreateChunk(int i, int x, int y) {
        VoxelGrid chunk = Instantiate(voxelGridPrefab) as VoxelGrid;
        chunk.transform.parent = transform;
        chunk.transform.localPosition = new Vector2(x * chunkSize - halfSize, y * chunkSize - halfSize);

        chunk.Initialize(voxelResolution, chunkSize);
    }

    void SetVoxel(Vector2 point, bool state) {
        int chunkX = (int)(point.x / size * chunkResolution);
        int chunkY = (int)(point.y / size * chunkResolution);
        Debug.Log(new Vector2(chunkX, chunkY));

        VoxelGrid chunk = chunks[chunkY * size + chunkX];
        Vector2 pointInChunk = point - (Vector2)chunk.gameObject.transform.localPosition;
    }
}
