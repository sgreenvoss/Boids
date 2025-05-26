using UnityEditor;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    [SerializeField] public int boidCount;
    [SerializeField] public GameObject boidPrefab;
    public GameObject worldBounds;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Bounds cubeBounds = worldBounds.GetComponent<BoxCollider>().bounds;
        Vector3 max = cubeBounds.max;
        Vector3 min = cubeBounds.min;

        for (int i = 0; i < boidCount; i++)
        {
            float randomX = Random.Range(min.x, max.x);
            float randomY = Random.Range(min.y, max.y);
            float randomZ = Random.Range(min.z, max.z);

            Vector3 randomPos = new Vector3(randomX, randomY, randomZ);
            Instantiate(boidPrefab, randomPos, Random.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
