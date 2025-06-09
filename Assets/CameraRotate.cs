using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public bool rotate;
    Vector3 point;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        point = FindObjectOfType<BoidManager>().transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (rotate)
            transform.RotateAround(point, Vector3.up, Time.deltaTime);
    }
}
