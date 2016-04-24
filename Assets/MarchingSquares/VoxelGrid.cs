using UnityEngine;

public class VoxelGrid : MonoBehaviour {
    [SerializeField]
    private GameObject voxelPrefab;
    private int resolution;
    private float size;

    bool[] voxels;
    private float voxelSize;
    private const float VoxelGutter = 0.02f;

    public void Initialize (int resolution, float size) {
        Debug.Log(resolution);
        this.resolution = resolution;
        this.size = size;
        voxels = new bool[resolution * resolution];
        voxelSize = size / resolution;

        Debug.Log(voxels.Length);
        // initialize voxels
        for (int i = 0, y = 0; y < resolution; y++) {
            for (int x = 0; x < resolution; x++, i++) {
                CreateVoxel(i, x, y);
            }
        }
    }

    void Start() {

    }

    void Update () {

    }

    void CreateVoxel(int i, int x, int y) {
        GameObject v = Instantiate(voxelPrefab) as GameObject;
        v.transform.parent = transform;
        v.transform.localPosition = new Vector2((x + 0.5f) * voxelSize, (y + 0.5f) * voxelSize);
        v.transform.localScale = Vector2.one * (voxelSize - VoxelGutter);
    }
}
