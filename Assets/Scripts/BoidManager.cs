using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    [SerializeField] public int boidCount;
    [SerializeField] public GameObject boidPrefab;
    Vector3 worldBounds = new Vector3(23f, 14f, 19f);

    private List<Boid> boids = new();


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
    }

    public List<Boid> GetNeighbors(Boid boid, float radius)
    {
        List<Boid> neighbors = new List<Boid>();
        foreach (var other in boids)
        {
            if (other != boid)
            {
                float dist = Vector3.Distance(boid.transform.position, other.transform.position);
                if (dist < radius)
                    neighbors.Add(other);
            }
        }
        return neighbors;
    }

    public Bounds GetWorldBounds()
    {
        return new Bounds(transform.position, worldBounds * 2f);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, worldBounds * 2f);
    }
}
