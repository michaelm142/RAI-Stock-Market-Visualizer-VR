using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRelitiveToCamera : MonoBehaviour
{
    public Vector3 offset;

    public float Tolerance = 0.1f;

    private bool moving;

    private new Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 point = camera.transform.localToWorldMatrix.MultiplyPoint(offset);
        if (!moving && Vector3.Distance(transform.position, point) > Tolerance)
            moving = true;
        if (Vector3.Distance(transform.position, point) < Tolerance)
            moving = false;

        if (moving)
        {
            transform.position = Vector3.Lerp(transform.position, point, 0.01f);
            transform.rotation = Quaternion.Lerp(transform.rotation, camera.transform.rotation, 0.01f);
        }
    }
}
