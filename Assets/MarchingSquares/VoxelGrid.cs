using UnityEngine;

public class VoxelGrid : MonoBehaviour {
    [SerializeField]
    private Transform voxelPrefab;
    public int resolution;

    bool[] voxels;

    void Awake () {
        voxels = new bool[resolution * resolution];
    }

    void Update () {

    }
}
