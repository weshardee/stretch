using UnityEngine;

[RequireComponent (typeof (VoxelGrid))]
public class InfluenceField : MonoBehaviour {
    public MetaBall[] balls;
	private VoxelGrid grid;

    // Use this for initialization
    void Start () {
        grid = GetComponent<VoxelGrid>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 gridPosition = grid.transform.position;
		int gridWidth = (int)(grid.width * grid.resolution);
        int gridHeight = (int)(grid.height * grid.resolution);

        for (int gridX = 0; gridX < gridWidth; gridX++)
        {
            float globalX = gridX * grid.resolution + gridPosition.x;
            for (int gridY = 0; gridY < gridHeight; gridY++)
            {
                float globalY = gridY * grid.resolution + gridPosition.y;
                Vector2 globalPosition = new Vector2(globalX, globalY);
                float sum = 0;
                foreach (MetaBall ball in balls)
                {
                    sum += ball.CalcInfluence(globalPosition);
                }
                grid.SetVoxel(gridX, gridY, sum);
            }
        }

        Debug.Log(gridWidth);
    }
}
