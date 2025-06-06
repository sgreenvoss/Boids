using UnityEngine;
using System.Collections.Generic;

public class Boid : MonoBehaviour
{
    public Vector3 velocity;
    public float maxSpeed = 5f;
    public float maxForce = 0.5f;
    // [SerializeField] int perceptualRadius;
    //Rigidbody rb;

    // Boid behavior weights
    public float separationWeight = 0.5f;
    public float alignmentWeight = 1.2f;
    public float cohesionWeight = 1.5f;

    // Detection radius
    public float neighborRadius = 5f;
    public float separationRadius = 0.5f;

    private BoidManager manager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = FindObjectOfType<BoidManager>();
        velocity = Random.insideUnitSphere * maxSpeed;
    }

    private void FixedUpdate()
    {
        List<Boid> neighbors = manager.GetNeighbors(this, neighborRadius);

        Vector3 separation = AvoidCollision(neighbors) * separationWeight;
        Vector3 alignment = MatchVel(neighbors) * alignmentWeight;
        Vector3 cohesion = CenterFlock(neighbors) * cohesionWeight;
        Vector3 boundsSteer = ComputeBoundsSteer();

        Vector3 acceleration = separation + alignment + cohesion + boundsSteer;

        // Apply acceleration
        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        transform.position += velocity * Time.deltaTime;

        if (velocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(velocity);
    }
    Vector3 AvoidCollision(List<Boid> neighbors)
    {
        Vector3 steer = Vector3.zero;
        int count = 0;
        foreach (var boid in neighbors)
        {
            // possibly slow (involving sqrts) - investigate
            float d = Vector3.Distance(transform.position, boid.transform.position);
            if (d > 0 && d < separationRadius)
            {
                Vector3 diff = (transform.position - boid.transform.position).normalized;
                steer += diff / d;
                count++;
            }
        }
        if (count > 0) steer /= count;
        if (steer == Vector3.zero) return Vector3.zero;

        steer = steer.normalized * maxSpeed - velocity;
        return Vector3.ClampMagnitude(steer, maxForce);
    }
    Vector3 MatchVel(List<Boid> neighbors)
    {
        Vector3 avgVelocity = Vector3.zero;
        int count = 0;
        foreach(var boid in neighbors)
        {
            avgVelocity += boid.velocity;
            count++;
        }
        if (count == 0) return Vector3.zero;

        avgVelocity /= count;
        Vector3 steer = avgVelocity.normalized * maxSpeed - velocity;
        return Vector3.ClampMagnitude(steer, maxForce);
    }
    Vector3 CenterFlock(List<Boid> neighbors)
    {
        Vector3 center = Vector3.zero;
        int count = 0;
        foreach (var boid in neighbors)
        {
            center += boid.transform.position;
            count++;
        }
        if (count == 0) return Vector3.zero;

        center /= count;
        Vector3 desired = (center - transform.position).normalized * maxSpeed;
        Vector3 steer = desired - velocity;
        return Vector3.ClampMagnitude(steer, maxForce);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, neighborRadius);
    }

    Vector3 ComputeBoundsSteer()
    {
        Bounds bounds = manager.GetWorldBounds();
        Vector3 steer = Vector3.zero;
        Vector3 pos = transform.position;

        float threshold = 2f; // Start steering when close to edge
        float turnForce = maxForce * 2f;   // Strength of the boundary correction

        if (pos.x < bounds.min.x + threshold)
            steer.x = turnForce;
        else if (pos.x > bounds.max.x - threshold)
            steer.x = -turnForce;

        if (pos.y < bounds.min.y + threshold)
            steer.y = turnForce;
        else if (pos.y > bounds.max.y - threshold)
            steer.y = -turnForce;

        if (pos.z < bounds.min.z + threshold)
            steer.z = turnForce;
        else if (pos.z > bounds.max.z - threshold)
            steer.z = -turnForce;

        return Vector3.ClampMagnitude(steer, maxForce);
    }

}
