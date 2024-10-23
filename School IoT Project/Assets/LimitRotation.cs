using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitRotation : MonoBehaviour
{
    public Vector2 limit = new Vector2(20f, -20f);

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3(Mathf.Clamp(transform.eulerAngles.x, limit.y, limit.x), transform.eulerAngles.y, Mathf.Clamp(transform.eulerAngles.z, limit.y, limit.x));
    }
}
