using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;

public class BoidManager : MonoBehaviour
{
    [SerializeField] public int boidCount;
    [SerializeField] public GameObject boidPrefab;
    [SerializeField] public GameObject foodPrefab;
    Vector3 worldBounds = new Vector3(23f, 14f, 19f);
    bool foodEnabled = false;
    int foodCount = 40;

    private List<Boid> boids = new();
    public List<Transform> food = new();
    static public Grid world;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < boidCount; i++)
        {
            Vector3 position = transform.position + new Vector3(
                Random.Range(-worldBounds.x, worldBounds.x),
                Random.Range(-worldBounds.y, worldBounds.y),
                Random.Range(-worldBounds.z, worldBounds.z)
            );
            GameObject boidGO = Instantiate(boidPrefab, position, Quaternion.identity);
            Boid boid = boidGO.GetComponent<Boid>();
            boids.Add(boid);
        }

        world = new Grid(boids, 4f);

        if (!foodEnabled) return;
        for (int i = 0; i < foodCount; i++)
        {
            Vector3 position = transform.position + new Vector3(
                Random.Range(-worldBounds.x, worldBounds.x),
                Random.Range(-worldBounds.y, worldBounds.y),
                Random.Range(-worldBounds.z, worldBounds.z)
            );
            GameObject foodGO = Instantiate(foodPrefab, position, Quaternion.identity);
            food.Add(foodGO.transform);
        }


    }

    private void Update()
    {
        world.UpdateGrid(boids);
    }

    public List<Boid> GetNeighbors(Boid boid)
    {
        return world.GetNeighborsHash(boid);
    }

    public Vector3 GetFood(Transform boid, float radius)
    {
        Vector3 minDist = worldBounds; // max possible dist(?)
        float maxD = 25f; 
        bool changed = false;
        foreach (var f in food)
        {
            // TODO: remove distance function

            float dist = Vector3.Distance(boid.position, f.transform.position);
            if (dist < radius && dist < maxD) {
                minDist = boid.position - f.transform.position;
                changed = true;
            }
        }
        if (changed) return minDist;
        else return Vector3.zero;
    }

    public Bounds GetWorldBounds()
    {
        return new Bounds(transform.position, worldBounds * 2f);
    }

    //private void OnDrawGizmos()
    //{
    //    if (world ==null || world.spatialHash == null) return;
    //    Gizmos.color = new Color(.2f, .8f, 1f, .25f);
    //    foreach (var kvp in world.spatialHash)
    //    {
    //        Vector3Int cell = kvp.Key;
    //        if (kvp.Value == null || kvp.Value.Count == 0) continue;

    //        Vector3 worldPos = world.CellToWorld(cell);
    //        Gizmos.DrawCube(worldPos, Vector3.one * world.cellSize);
    //    }
    //}

}

public class Grid
{
    public Dictionary<Vector3Int, List<Boid>> spatialHash = new();
    public float cellSize;
    public Grid(List<Boid> boids, float _cellSize)
    {
        cellSize = _cellSize;
        spatialHash.Clear();
        foreach (Boid boid in boids)
        {
            Vector3Int cell = WorldToCell(boid.transform.position);
            if (!spatialHash.ContainsKey(cell))
            {
                spatialHash[cell] = new List<Boid>();
            }
            spatialHash[cell].Add(boid);
        }
    }

    public void UpdateGrid(List<Boid> boids)
    {
        spatialHash.Clear();
        foreach (Boid boid in boids)
        {
            Vector3Int cell = WorldToCell(boid.transform.position);
            if (!spatialHash.ContainsKey(cell))
            {
                spatialHash[cell] = new List<Boid>();
            }
            spatialHash[cell].Add(boid);
        }
    }

    public List<Boid> GetNeighborsHash(Boid boid)
    {
        List<Boid> neighbors = new();
        Vector3Int centerCell = WorldToCell(boid.transform.position);
        for (int x = -1; x <= 1; x++)
        for (int y = -1; y <= 1; y++)
        for (int z = -1; z <= 1; z++)
        {
                Vector3Int offset = new Vector3Int(x, y, z);
                Vector3Int neighborCell = centerCell + offset;
                if (spatialHash.TryGetValue(neighborCell, out var boidList))
                {
                    foreach (var other in boidList)
                    {
                        if (other != boid &&
                            (other.transform.position - boid.transform.position).sqrMagnitude <
                            boid.neighborRadius * boid.neighborRadius)
                        {
                            neighbors.Add(other);
                        }
                    }
                }
        }
        return neighbors;
    }

    private Vector3Int WorldToCell(Vector3 position)
    {
        return new Vector3Int(
            Mathf.FloorToInt(position.x / cellSize),
            Mathf.FloorToInt(position.y / cellSize),
            Mathf.FloorToInt(position.z / cellSize)
        );
    }
    public Vector3 CellToWorld(Vector3Int cell)
    {
        return (Vector3)cell * cellSize + Vector3.one * (cellSize / 2f);
    }

   
  
}
