using UnityEngine;

public class Food : MonoBehaviour
{
    private BoidManager manager;

    private void Start()
    {
        manager = FindObjectOfType<BoidManager>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        manager.food.Remove(transform);
        Destroy(gameObject);
    }
}
