using UnityEngine;

public class Boid : MonoBehaviour
{
    public BoidManager manager;
    [SerializeField] int perceptualRadius;
    Rigidbody rb;
    public Vector3 currentVel; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentVel = Vector3.zero;
    }

    Vector3 AvoidCollision()
    {
        return Vector3.zero;
    }
    Vector3 MatchVel()
    {
        return Vector3.zero;
    }
    Vector3 CenterFlock()
    {
        return Vector3.zero;
    }
    Vector3 ArbitrateAccel()
    {
        return Vector3.zero;
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log("trying");
        Collider[] colliders = Physics.OverlapSphere(transform.position, perceptualRadius);
        foreach (var collider in colliders)
        {
            Boid otherBoid = collider.GetComponent<Boid>();
            if (otherBoid != null) Debug.DrawLine(transform.position, collider.transform.position);
        }
    }

}
