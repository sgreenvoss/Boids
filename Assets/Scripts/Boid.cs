using UnityEngine;
using System.Collections.Generic;


public class Boid : MonoBehaviour
{
    public Vector3 velocity;
    public float minSpeed = 2f;
    public float maxSpeed = 8f;
    public float maxForce = 0.5f;
    public float flockRandomStrength = 1f;
    // [SerializeField] int perceptualRadius;
    //Rigidbody rb;

    // Boid behavior weights
    public float separationWeight = 0.5f;
    public float alignmentWeight = 1.2f;
    public float cohesionWeight = 1.5f;
    public float randomWeight = 1f;
    public float feedWeight = 1.5f;

    // Detection radius
    public float neighborRadius = 5f;
    public float separationRadius = 0.5f;

    private BoidManager manager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /*separationWeight += Random.Range(-1, 1);
        alignmentWeight += Random.Range(-1, 1);
        cohesionWeight += Random.Range(-1, 1);
        randomWeight += Random.Range(-1, 1); */

        manager = FindObjectOfType<BoidManager>();

        velocity = Random.insideUnitSphere * maxSpeed;
    }

    private void Update()
    {
        List<Boid> neighbors = manager.GetNeighbors(this);

        Vector3 acceleration = AllMethods(neighbors);
        acceleration += RNG() * randomWeight;
        acceleration += Feed() * feedWeight;

        // Apply acceleration
        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        velocity = velocity.normalized * Mathf.Clamp(speed, minSpeed, maxSpeed);
        transform.position += velocity * Time.deltaTime;

        if (velocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(velocity);

        Bounds bounds = manager.GetWorldBounds();
        Vector3 pos = transform.position;
        Vector3 min = bounds.min + Vector3.one * 0.1f;
        Vector3 max = bounds.max - Vector3.one * 0.1f;
        pos.x = Mathf.Clamp(pos.x, min.x, max.x);
        pos.y = Mathf.Clamp(pos.y, min.y, max.y);
        pos.z = Mathf.Clamp(pos.z, min.z, max.z);
        transform.position = pos;
    }

    Vector3 AllMethods(List<Boid> neighbors)
    {
        Vector3 separation = Vector3.zero;
        int s_count = 0;
        Vector3 avgVelocity = Vector3.zero;
        Vector3 center = Vector3.zero;
        int count = 0;

        foreach (var boid in neighbors)
        {
            float d = Vector3.Distance(transform.position, boid.transform.position);
            if (d > 0 && d < separationRadius)
            {
                Vector3 diff = (transform.position - boid.transform.position).normalized;
                separation += diff / d;
                s_count++;
            }
            avgVelocity += boid.velocity;
            center += boid.transform.position;

            count++;
        }
        if (s_count > 0)
        {
            separation /= s_count;
            separation = separation.normalized * maxSpeed - velocity;
            separation = Vector3.ClampMagnitude(separation, maxForce);
        }

        if (count > 0) {
            avgVelocity /= count;
            avgVelocity = avgVelocity.normalized * maxSpeed - velocity;
            avgVelocity = Vector3.ClampMagnitude(avgVelocity, maxForce);

            center /= count;
            Vector3 desired = (center - transform.position).normalized * maxSpeed;
            center = desired - velocity;
            center = Vector3.ClampMagnitude(center, maxForce);
        }
        return separation * separationWeight +
                avgVelocity * alignmentWeight +
                center * cohesionWeight +
                ComputeBoundsSteer();

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

        float threshold = 4f; // Start steering farther out
        float k = maxForce * 4f;

        if (pos.x < bounds.min.x + threshold)
            steer.x = Mathf.Lerp(k, 0, (pos.x - bounds.min.x) / threshold);
        else if (pos.x > bounds.max.x - threshold)
            steer.x = -Mathf.Lerp(k, 0, (bounds.max.x - pos.x) / threshold);

        if (pos.y < bounds.min.y + threshold)
            steer.y = Mathf.Lerp(k, 0, (pos.y - bounds.min.y) / threshold);
        else if (pos.y > bounds.max.y - threshold)
            steer.y = -Mathf.Lerp(k, 0, (bounds.max.y - pos.y) / threshold);

        if (pos.z < bounds.min.z + threshold)
            steer.z = Mathf.Lerp(k, 0, (pos.z - bounds.min.z) / threshold);
        else if (pos.z > bounds.max.z - threshold)
            steer.z = -Mathf.Lerp(k, 0, (bounds.max.z - pos.z) / threshold);

        return Vector3.ClampMagnitude(steer, maxForce);
    }

    Vector3 RNG()
    {
        return Random.insideUnitSphere * flockRandomStrength;
    }
    
    Vector3 Feed()
    {
        return manager.GetFood(transform, neighborRadius);
    }


}
