using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using YVR.Interaction.Runtime;

public class InputManager : MonoBehaviour
{
    private static InputManager m_Instance;
    public static InputManager instance => m_Instance;
    public YVRInputActions inputActions;
    private void Awake()
    {
        if(m_Instance == null)
            m_Instance = this;
    }
    
    private void Start()
    {
        inputActions = new YVRInputActions();
        inputActions.Enable();
        inputActions.YVRRight.PrimaryButton.performed += AddQuadPoint;
        inputActions.YVRRight.SecondaryButton.performed += ClearTemporaryPoints;
    }

    private void ClearTemporaryPoints(InputAction.CallbackContext context)
    { 
        QuadManager.instance.ClearTemporaryPoints();
    }

    private void AddQuadPoint(InputAction.CallbackContext context)
    {
        QuadManager.instance.AddQuadPoint();
    }

    private void OnDisable()
    {
        inputActions.Disable();
        inputActions.YVRRight.PrimaryButton.performed -= AddQuadPoint;
    }
}
