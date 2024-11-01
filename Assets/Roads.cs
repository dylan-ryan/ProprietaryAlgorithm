using UnityEngine;
using System.Collections.Generic;

public class RoadPlacer : MonoBehaviour
{
    public GameObject roadCubePrefab;
    public GameObject buildingCubePrefab;
    public int roadLength;

    private Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
    private HashSet<Vector2Int> placedRoads = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> placedBuildings = new HashSet<Vector2Int>();

    private void Start()
    {
        GenerateRoads();
        PlaceBuildings();
    }

    public void GenerateRoads()
    {
        Vector2Int currentPos = Vector2Int.zero;
        Vector2Int direction = Vector2Int.up;
        for (int i = 0; i < roadLength; i++)
        {
            if (CountNearbyRoads(currentPos) <= 1)
            {
                PlaceRoadSegment(currentPos);
            }

            if (Random.value > 0.7f)
            {
                direction = directions[Random.Range(0, directions.Length)];
            }
            currentPos += direction;
        }
    }

    void PlaceRoadSegment(Vector2Int position)
    {
        if (!placedRoads.Contains(position))
        {
            placedRoads.Add(position);
            Instantiate(roadCubePrefab, new Vector3(position.x, 0, position.y), Quaternion.identity);
        }
    }

    int CountNearbyRoads(Vector2Int position)
    {
        int count = 0;
        foreach (Vector2Int dir in directions)
        {
            if (placedRoads.Contains(position + dir))
            {
                count++;
            }
        }
        return count;
    }

    void PlaceBuildings()
    {
        foreach (Vector2Int roadPosition in placedRoads)
        {
            PlaceBuildingsAround(roadPosition);
        }
    }

    void PlaceBuildingsAround(Vector2Int roadPosition)
    {
        foreach (Vector2Int dir in directions)
        {
            Vector2Int buildingPosition = roadPosition + dir;
            if (!placedRoads.Contains(buildingPosition) && !placedBuildings.Contains(buildingPosition))
            {
                float buildingHeight = Random.Range(1f, 3f);
                GameObject building = Instantiate(buildingCubePrefab, new Vector3(buildingPosition.x, buildingHeight / 2, buildingPosition.y), Quaternion.identity);
                building.transform.localScale = new Vector3(1f, buildingHeight, 1f);
                placedBuildings.Add(buildingPosition);
            }
        }
    }
}
