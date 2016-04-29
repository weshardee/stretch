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

        for (int gridX = 0; gridX < gridWidth; gridX++)
        {
            for (int gridY = 0; gridY < gridHeight; gridY++)
            {
                Vector2 globalPosition = grid.ToGlobalPosition(gridX, gridY);
                float sum = 0;
                foreach (MetaBall ball in balls)
                {
                    sum += ball.CalcInfluence(globalPosition);
                }
                grid.SetVoxelAt(globalPosition, sum);
            }
        }
    }
}
