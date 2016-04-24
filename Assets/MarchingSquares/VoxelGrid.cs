using UnityEngine;

public class VoxelGrid : MonoBehaviour {
    public int resolution;

    bool[] voxels;

    void Awake () {
        voxels = new bool[resolution * resolution];
    }

    void Update () {

    }
}
