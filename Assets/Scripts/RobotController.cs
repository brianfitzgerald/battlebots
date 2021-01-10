using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public List<WheelCollider> wheelColliders;
    public List<PartController> hammers;
    public float maxMotorTorque;
    public float maxSteeringAngle;

    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation * Quaternion.Euler(0, 0, -90);
    }

    public void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        foreach (WheelCollider collider in wheelColliders)
        {
            collider.steerAngle = steering;
            collider.motorTorque = motor;
            ApplyLocalPositionToVisuals(collider);
        }
    }

    // Where the CPU lives.
    public List<HingeJoint> childHinges = new List<HingeJoint>();

    // During actual building, generate a hierarchy of hinge joints based on what a piece is connected to. Once a piece is damaged enough, destroy the hinge joint and mark as non gameplay.
    // For now, we just add a hinge to every piece.
    public GameObject body;

    public Vector2 springSpringDamper;
    void Start()
    {
        foreach (var child in transform.GetComponentsInChildren<Rigidbody>())
        {
            // if (child.gameObject.GetComponent<WheelCollider>())
            // {
            //     continue;
            // }
            // ignore cosmetic parts
            var partController = child.GetComponent<PartController>();
            if (partController != null)
            {
                if (partController.partType == PartType.COSMETIC)
                {
                    continue;
                }
                if (partController.partType == PartType.HAMMER)
                {
                    hammers.Add(partController);
                }
            }
            if (child.name == "Body")
            {
                Debug.Log("Found Body");
                continue;
            }
            var hinge = body.AddComponent<HingeJoint>();
            hinge.useSpring = true;
            var spring = new JointSpring();
            spring.spring = springSpringDamper.x;
            spring.damper = springSpringDamper.y;
            hinge.spring = spring;
            hinge.connectedBody = child;
        }
    }

    void Update()
    {

    }
}
