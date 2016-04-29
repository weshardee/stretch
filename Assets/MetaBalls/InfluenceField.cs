using UnityEngine;

[RequireComponent (typeof (VoxelGrid))]
public class InfluenceField : MonoBehaviour {
    private MetaBall[] balls;
	private VoxelGrid grid;

    // Use this for initialization
    void Start () {
        grid = GetComponent<VoxelGrid>();
        balls = GetComponentsInChildren<MetaBall>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 gridPosition = grid.transform.position;
		int gridWidth = (int)(grid.width * grid.resolution);
        int gridHeight = (int)(grid.height * grid.resolution);

        for (int gridX = 0; gridX < grid.voxelsX; gridX++)
        {
            for (int gridY = 0; gridY < grid.voxelsY; gridY++)
            {
                Vector2 point = grid.ToPoint(gridX, gridY);
                float sum = 0;
                foreach (MetaBall ball in balls)
                {
                    sum += ball.CalcInfluence(point);
                }
                grid.SetVoxelAt(point, sum);
            }
        }
    }
}
