using UnityEngine;
using System.Collections.Generic;

public class RoadPlacer : MonoBehaviour
{
    public GameObject roadCubePrefab;
    public GameObject buildingCubePrefab;
    public GameObject windowPrefab;
    public int roadLength;

    private Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
    private HashSet<Vector2Int> placedRoads = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> placedBuildings = new HashSet<Vector2Int>();
    private List<GameObject> spawnedObjects = new List<GameObject>();

    private void Start()
    {
        GenerateCity();
    }

    public void GenerateCity()
    {
        GenerateRoads();
        PlaceBuildings();
    }

    public void RegenerateCity()
    {
        ClearCity();
        GenerateCity();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void ClearCity()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            Destroy(obj);
        }
        spawnedObjects.Clear();

        placedRoads.Clear();
        placedBuildings.Clear();
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
            GameObject road = Instantiate(roadCubePrefab, new Vector3(position.x, 0, position.y), Quaternion.identity);
            spawnedObjects.Add(road);
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

            if (Random.value < 0.5f)
            {
                continue;
            }

            if (!placedRoads.Contains(buildingPosition) && !placedBuildings.Contains(buildingPosition))
            {
                float buildingHeight = Random.Range(1f, 3f);
                GameObject building = Instantiate(buildingCubePrefab, new Vector3(buildingPosition.x, buildingHeight / 2, buildingPosition.y), Quaternion.identity);
                building.transform.localScale = new Vector3(1f, buildingHeight, 1f);
                placedBuildings.Add(buildingPosition);
                spawnedObjects.Add(building);

                Renderer buildingRenderer = building.GetComponent<Renderer>();
                if (buildingRenderer != null)
                {
                    buildingRenderer.material.color = new Color(Random.value, Random.value, Random.value);
                }


                PlaceWindows(building, buildingHeight);
            }
        }
    }

    void PlaceWindows(GameObject building, float buildingHeight)
    {
        float windowSpacing = 1f;
        int windowCount = Mathf.FloorToInt(buildingHeight / windowSpacing);

        for (int i = 1; i < windowCount; i++)
        {
            float yPosition = i * windowSpacing - (buildingHeight / 2);

            GameObject frontWindow = Instantiate(windowPrefab, building.transform.position + new Vector3(0, yPosition, 0.5f), Quaternion.identity, building.transform);
            GameObject backWindow = Instantiate(windowPrefab, building.transform.position + new Vector3(0, yPosition, -0.5f), Quaternion.identity, building.transform);
            GameObject leftWindow = Instantiate(windowPrefab, building.transform.position + new Vector3(-0.5f, yPosition, 0), Quaternion.Euler(0, 90, 0), building.transform);
            GameObject rightWindow = Instantiate(windowPrefab, building.transform.position + new Vector3(0.5f, yPosition, 0), Quaternion.Euler(0, 90, 0), building.transform);

            spawnedObjects.Add(frontWindow);
            spawnedObjects.Add(backWindow);
            spawnedObjects.Add(leftWindow);
            spawnedObjects.Add(rightWindow);
        }
    }
}
