using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SliderJoint2D))]
public class ChangeMotorSpeed : MonoBehaviour
{
    [SerializeField] private float speedPlatform = 1.75f;
    [SerializeField] private float platformWaitingTime = 1.5f;

    private SliderJoint2D sliderJoint;
    private JointMotor2D motorJoint;
    private bool isMoving = true;

    private void Awake()
    {
        sliderJoint = GetComponent<SliderJoint2D>();

        motorJoint = sliderJoint.motor;
        motorJoint.motorSpeed = speedPlatform;
        sliderJoint.motor = motorJoint;
    }

    private void Update()
    {
        if (Mathf.Abs(sliderJoint.jointSpeed) < 0.01f && isMoving)
        {
            isMoving = false;
            StartCoroutine(ChangeMotorSpeedCoroutine());
        }
    }

    private IEnumerator ChangeMotorSpeedCoroutine()
    {
        yield return new WaitForSeconds(platformWaitingTime);
        motorJoint.motorSpeed *= -1;
        sliderJoint.motor = motorJoint;

        yield return new WaitForSeconds(0.2f);
        isMoving = true;
    }
}