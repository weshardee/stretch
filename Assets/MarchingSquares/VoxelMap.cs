using UnityEngine;

public class VoxelMap : MonoBehaviour {
    [SerializeField]
    private VoxelGrid voxelGridPrefab;

    [SerializeField]
    private int size;
    [SerializeField]
    private int chunkResolution;
    [SerializeField]
    private int voxelResolution;

    bool[] chunks;

    private float halfSize;
    private float voxelSize;
    private float chunkSize;

    void Awake () {
        halfSize = size / 2;
        chunkSize = size / chunkResolution;
        voxelSize = chunkSize / voxelResolution;

        chunks = new bool[size * size];

        // initialize voxels
        for (int i = 0, y = 0; y < size; y++) {
            for (int x = 0; x < size; x++, i++) {
                CreateChunk(i, x, y);
            }
        }
    }

    void Start() {

    }

    void Update () {

    }

    void CreateChunk(int i, int x, int y) {
        VoxelGrid chunk = Instantiate(voxelGridPrefab) as VoxelGrid;
        chunk.transform.parent = transform;
        chunk.transform.localPosition = new Vector2(x * chunkSize - halfSize, y * chunkSize - halfSize);

        chunk.Initialize(chunkResolution, chunkSize);
    }
}
