using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace susy_bakaShared.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class RemoveRigidbodyMotion : MonoBehaviour
    {

        private Rigidbody rigidbodyObj;

        void Awake()
        {
            rigidbodyObj = GetComponent<Rigidbody>();
            rigidbodyObj.velocity = Vector3.zero;
            rigidbodyObj.angularVelocity = Vector3.zero;
            rigidbodyObj.sleepThreshold = 0.0f;
            rigidbodyObj.detectCollisions = true;
            rigidbodyObj.maxAngularVelocity = 0;
            rigidbodyObj.useGravity = false;
        }

        void Update()
        {
            rigidbodyObj.velocity = Vector3.zero;
            rigidbodyObj.angularVelocity = Vector3.zero;
            rigidbodyObj.sleepThreshold = 0.0f;
            rigidbodyObj.detectCollisions = true;
            rigidbodyObj.maxAngularVelocity = 0;
            rigidbodyObj.useGravity = false;
        }
    }
}