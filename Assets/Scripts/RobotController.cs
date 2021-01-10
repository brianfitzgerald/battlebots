using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public List<Rigidbody> wheels;
    public List<HingeJoint> hammerHinges;
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

        // front left, front right, back left, back right
        for (var i = 0; i < wheels.Count; i++)
        {
            var wheel = wheels[i];
            var force = i % 2 == 0 ? motor - steering : motor + steering;
            // Debug.Log($"{force} {steering} {motor}");
            wheels[i].AddForce(wheel.transform.forward * force, ForceMode.Acceleration);
            Debug.DrawRay(wheel.transform.position, wheel.transform.forward * force, Color.green, Time.fixedDeltaTime * 2);
            // ApplyLocalPositionToVisuals(collider);
        }
    }

    // Where the CPU lives.
    public List<HingeJoint> childHinges = new List<HingeJoint>();

    // During actual building, generate a hierarchy of hinge joints based on what a piece is connected to. Once a piece is damaged enough, destroy the hinge joint and mark as non gameplay.
    // For now, we just add a hinge to every piece.
    public GameObject body;

    public Vector2 partSpringDamper;
    public Vector2 wheelSpringDamper;
    void Start()
    {
        foreach (Transform child in transform)
        {
            // if (child.gameObject.GetComponent<WheelCollider>())
            // {
            //     continue;
            // }
            // ignore cosmetic parts
            var partController = child.gameObject.GetComponent<PartController>();
            if (partController != null)
            {
                Debug.Log(partController.gameObject.name);
                if (partController.partType == PartType.COSMETIC)
                {
                    continue;
                }
            }
            if (child.name == "Body")
            {
                // Debug.Log("Found Body");
                continue;
            }
            if (child.GetComponent<Rigidbody>() == null)
            {
                return;
            }
            var hinge = body.AddComponent<HingeJoint>();
            hinge.useSpring = true;
            var spring = new JointSpring();
            if (partController != null && partController.partType == PartType.WHEEL)
            {
                spring.spring = wheelSpringDamper.x;
                spring.damper = wheelSpringDamper.y;
            }
            else
            {
                spring.spring = partSpringDamper.x;
                spring.damper = partSpringDamper.y;
            }
            hinge.spring = spring;
            if (partController != null && partController.partType == PartType.HAMMER)
            {
                hinge.anchor = partController.anchor.transform.localPosition;
                hinge.axis = new Vector3(0, 0, 1);
                hammerHinges.Add(hinge);
            }
            hinge.connectedBody = child.GetComponent<Rigidbody>();
        }
    }


    public bool bonking;
    public float bonkTime;

    public void Bonk()
    {
        bonking = !bonking;
        var spring = new JointSpring();
        spring.targetPosition = bonking ? 30 : 0;
        spring.spring = partSpringDamper.x;
        spring.damper = partSpringDamper.y;
        foreach (var hammer in hammerHinges)
        {
            hammer.spring = spring;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Bonk();
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            Bonk();
        }
    }
}
