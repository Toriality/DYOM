using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;
    public CinemachineFreeLook cineThirdPersonCamera;
    public CinemachineFreeLook cineCombatCamera;

    public float rotationSpeed;

    public CameraStyle currentStyle;
    public BasicZooms currentZoom = BasicZooms.normal;

    public Transform combatLookAt;

    public GameObject thirdPersonCam;
    public GameObject combatCam;

    public enum CameraStyle
    {
        Basic,
        Combat,
        Topdown
    }

    public enum BasicZooms
    {
        normal = 0,
        near = 1,
        far = 2,
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Switch camera styles
        if (Input.GetKey(KeyCode.Mouse1)) SwitchCameraStyle(CameraStyle.Combat);
        if (Input.GetKeyUp(KeyCode.Mouse1)) SwitchCameraStyle(CameraStyle.Basic);
        if (Input.GetKeyDown(KeyCode.V)) SwitchCameraZoom();

        else if (currentStyle == CameraStyle.Combat)
        {
            Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
            orientation.forward = dirToCombatLookAt.normalized;

            playerObj.forward = dirToCombatLookAt.normalized;

        }
    }

    private void SwitchCameraZoom()
    {
        if ((int)currentZoom >= 2) currentZoom = 0;
        else currentZoom++;

        if (currentZoom == BasicZooms.near) {
            cineThirdPersonCamera.m_Orbits[0].m_Radius = 1;
            cineThirdPersonCamera.m_Orbits[1].m_Radius = 2;
            cineThirdPersonCamera.m_Orbits[2].m_Radius = 3;
        }
        if (currentZoom == BasicZooms.normal)
        {
            cineThirdPersonCamera.m_Orbits[0].m_Radius = 2;
            cineThirdPersonCamera.m_Orbits[1].m_Radius = 4;
            cineThirdPersonCamera.m_Orbits[2].m_Radius = 5;
        }
        if (currentZoom == BasicZooms.far)
        {
            cineThirdPersonCamera.m_Orbits[0].m_Radius = 4;
            cineThirdPersonCamera.m_Orbits[1].m_Radius = 8;
            cineThirdPersonCamera.m_Orbits[2].m_Radius = 10;
        }


    }
        
    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        if (newStyle == CameraStyle.Basic)
        {
            cineCombatCamera.Priority = 0;
            cineThirdPersonCamera.Priority = 1;
        }
        else if (newStyle == CameraStyle.Combat)
        {
            cineCombatCamera.Priority = 1;
            cineThirdPersonCamera.Priority = 0;
        }
        currentStyle = newStyle;
    }
}
