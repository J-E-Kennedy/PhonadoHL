using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class ControllerActions : MonoBehaviour
{
    private List<InputDevice> devices;


    private InputDevice leftHandedController;
    private InputDevice rightHandedController;
    private InputFeatureUsage<bool> triggerFeature;
    private InputFeatureUsage<bool> bumperFeature;
    private InputFeatureUsage<object> testFeature;
    private bool leftTriggerPressed;
    private bool leftBumperPressed;
    private bool rightTriggerPressed;
    private bool rightBumperPressed;
   // public object testValue;

    private bool leftTriggerValue;
    private bool leftBumperValue;
    private bool rightTriggerValue;
    private bool rightBumperValue;
    [HideInInspector] public bool LeftIsConnected => leftHandedController.isValid;
    [HideInInspector] public bool RightIsConnected => rightHandedController.isValid;
    [HideInInspector] public UnityEvent RightTriggerFullPress;
    [HideInInspector] public UnityEvent RightTriggerRelease;
    [HideInInspector] public UnityEvent RightBumperPressed;
    [HideInInspector] public UnityEvent RightBumperRelease;
    [HideInInspector] public UnityEvent LeftTriggerFullPress;
    [HideInInspector] public UnityEvent LeftTriggerRelease;
    [HideInInspector] public UnityEvent LeftBumperPressed;
    [HideInInspector] public UnityEvent LeftBumperRelease;

    [HideInInspector]
    public Vector3 LeftControllerPosition
    {
        get
        {
            Vector3 devicePosition = new Vector3();
            if (LeftIsConnected)
            {
                if (leftHandedController.TryGetFeatureValue(CommonUsages.devicePosition, out devicePosition))
                {
                }
            }

            return devicePosition;
        }
    }

    [HideInInspector]
    public Quaternion LeftControllerRotation
    {
        get
        {
            Quaternion deviceRotation = Quaternion.identity;
            if (LeftIsConnected)
            {
                if (leftHandedController.TryGetFeatureValue(CommonUsages.deviceRotation, out deviceRotation))
                {
                }
            }

            return deviceRotation;
        }
    }

    [HideInInspector]
    public Vector3 RightControllerPosition
    {
        get
        {
            Vector3 devicePosition = new Vector3();
            if (RightIsConnected)
            {
                if (rightHandedController.TryGetFeatureValue(CommonUsages.devicePosition, out devicePosition))
                {
                }
            }

            return devicePosition;
        }
    }

    [HideInInspector]
    public Quaternion RightControllerRotation
    {
        get
        {
            Quaternion deviceRotation = Quaternion.identity;
            if (RightIsConnected)
            {
                if (rightHandedController.TryGetFeatureValue(CommonUsages.deviceRotation, out deviceRotation))
                {
                }
            }

            return deviceRotation;
        }
    }

    void Start()
    {
        testFeature = new InputFeatureUsage<object>("TriggerButton");

        triggerFeature = CommonUsages.triggerButton;
        bumperFeature = CommonUsages.secondaryButton;
        List<InputDeviceRole> controllerRoles = new List<InputDeviceRole>()
            {InputDeviceRole.LeftHanded, InputDeviceRole.RightHanded};
        for (int i = 0; i < controllerRoles.Count; i++)
        {
            devices = new List<InputDevice>();
            InputDevices.GetDevicesWithRole(controllerRoles[i], devices);
            if (devices.Count > 0)
            {
                if (i == 0)
                {
                    leftHandedController = devices[0];
                }
                else
                {
                    rightHandedController = devices[0];
                }
            }
        }
    }


    void Update()
    {
        if (LeftIsConnected)
        {
            //checking for trigger first run
            if (leftHandedController.TryGetFeatureValue(triggerFeature, out leftTriggerValue)
                && leftTriggerValue)
            {
                //will invoke event for trigger press/release
                LeftTriggerFullPress.Invoke();
                leftTriggerPressed = true;
            }
            else if (leftTriggerPressed)
            {
                LeftTriggerRelease.Invoke();
                leftTriggerPressed = false;
            }

            if (leftHandedController.TryGetFeatureValue(bumperFeature, out leftBumperValue)
                && leftBumperValue)
            {
                LeftBumperPressed.Invoke();
                leftBumperPressed = true;
            }
            else if (leftBumperPressed)
            {
                LeftBumperRelease.Invoke();
                leftBumperPressed = false;
            }

            //leftHandedController.TryGetFeatureValue(testFeature, out testValue);
        }

        if (RightIsConnected)
        {
            if (rightHandedController.TryGetFeatureValue(triggerFeature, out rightTriggerValue)
                && rightTriggerValue)
            {
                RightTriggerFullPress.Invoke();
                rightTriggerPressed = true;
            }
            else if (rightTriggerPressed)
            {
                RightTriggerRelease.Invoke();
                rightTriggerPressed = false;
            }

            if (rightHandedController.TryGetFeatureValue(bumperFeature, out rightBumperValue)
                && rightBumperValue)
            {
                RightBumperPressed.Invoke();
                rightBumperPressed = true;
            }
            else if (rightBumperPressed)
            {
                RightBumperRelease.Invoke();
                rightBumperPressed = false;
            }
        } 
    }

    public void OnGUI()
    {
        GUI.color = Color.blue;
        GUI.Label(new Rect(0,0, 500, 25), $"Left Trigger: {leftTriggerPressed} - {leftTriggerValue}");
        GUI.Label(new Rect(0,25, 500, 25), $"Left Bumper: {leftBumperPressed} - {leftBumperValue}");
        GUI.Label(new Rect(0,50, 500, 25), $"Right Trigger: {rightTriggerPressed} - {rightTriggerValue}");
        GUI.Label(new Rect(0,75, 500, 25), $"Right Bumper: {rightBumperPressed} - {rightBumperValue}");
    }
}

