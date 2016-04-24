using UnityEngine;

public class VoxelGrid : MonoBehaviour {
    [SerializeField]
    private GameObject voxelPrefab;
    [SerializeField]
    private int resolution;

    bool[] voxels;
    private float voxelSize;
    private const float VoxelGutter = 0.02f;

    void Awake () {
        voxels = new bool[resolution * resolution];
        voxelSize = 1f / resolution;

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
        v.transform.localPosition = new Vector2(x * voxelSize, y * voxelSize);
        v.transform.localScale = Vector2.one * (voxelSize - VoxelGutter);
    }
}
