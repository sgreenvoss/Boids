using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    [SerializeField] public int boidCount;
    [SerializeField] public GameObject boidPrefab;
    public GameObject worldBounds;
    static public List<Transform> boidPositions = new();
    private List<Boid> boids = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Bounds cubeBounds = worldBounds.GetComponent<BoxCollider>().bounds;
        Vector3 max = cubeBounds.max;
        Vector3 min = cubeBounds.min;

        for (int i = 0; i < boidCount; i++)
        {
            float randomX = Random.Range(min.x + 1, max.x - 1);
            float randomY = Random.Range(min.y + 1, max.y - 1);
            float randomZ = Random.Range(min.z + 1, max.z - 1);

            Vector3 randomPos = new Vector3(randomX, randomY, randomZ);
            GameObject boid = Instantiate(boidPrefab, randomPos, Random.rotation);
            Boid b = boid.GetComponent<Boid>();
            b.manager = this;
            boids.Add(b);
            boidPositions.Add(boid.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
       // foreach
    }
}
