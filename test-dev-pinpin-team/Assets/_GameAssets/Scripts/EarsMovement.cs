using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarsMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform character;
    [SerializeField] private Rigidbody body;

    private Transform root;
    private Transform child;
    private Vector3 defaultAngle;
    private Vector3 childAngle;

    [Header("Properties")]
    [SerializeField] private float bending;
    [SerializeField] private float bendingStrenght = -25f;
    [SerializeField] private float bendingDuration = 0.2f;
    [SerializeField] private float maxAngle = 60f;

    [SerializeField] private float childFactor = 2.3f;

    void Awake()
    {
        root = this.transform;
        child = root.GetChild(0);

        defaultAngle = root.localRotation.eulerAngles;
        childAngle = child.localRotation.eulerAngles;
    }

    void LateUpdate()
    {
        //calculate a bending angle to the ears propertionate to the players velocity
        float temp = Vector3.Dot(character.forward, body.velocity.normalized) * body.velocity.magnitude * bendingStrenght;
        temp = Mathf.Sign(temp) * Mathf.Min(Mathf.Abs(temp), maxAngle);
        bending = Mathf.Lerp(bending, temp, (1f / bendingDuration) * Time.deltaTime);

        //apply bending
        root.localRotation = Quaternion.Euler(defaultAngle + Vector3.right * bending);
        child.localRotation = Quaternion.Euler(childAngle + Vector3.right * (bending / childFactor));
    }
}
