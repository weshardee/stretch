using UnityEngine;

public class VoxelGrid : MonoBehaviour {
    [SerializeField]
    private GameObject voxelPrefab;
    private int resolution;
    private float size;

    bool[,] voxels;
    Material[,] mats;
    private float voxelSize;
    private const float VoxelGutter = 0.02f;

    private static Color ColorOn = Color.black;
    private static Color ColorOff = Color.white;


    public void Initialize (int resolution, float size) {
        this.resolution = resolution;
        this.size = size;
        voxels = new bool[resolution, resolution];
        mats = new Material[resolution, resolution];
        voxelSize = size / resolution;

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
        mats[x, y] = v.GetComponent<Renderer>().material;
        SetVoxel(x, y, false);
    }

    public void SetVoxel(int x, int y, bool state) {
        voxels[x, y] = state;
        mats[x, y].color = state ? ColorOn : ColorOff;
    }
}
