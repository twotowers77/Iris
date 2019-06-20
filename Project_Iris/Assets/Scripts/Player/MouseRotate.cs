using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MouseRotate {

    public float XSensitivity = 2f;     //X 敏感度
    public float YSensitivity = 2f;     //Y 敏感度
    public bool clampVR = true;         //Vertical Rotation Limited
    public float MinimumX = -90F;       //X-Axis Minimum Angle Degree
    public float MaximumX = 90F;        //Y-Axis Max Angle Degree
    public bool smooth;          //smooth setting 
    public float smoothTime = 5f;       //부드러움 (*Time.deltaTime)
    public bool lockCursor = true;      //Mouse Visible (可視性の有無)

    private Quaternion CharacterTargetRotate;
    private Quaternion CameraTargetRotate;
    private bool m_cursorIsLocked = true;

    public void Init(Transform character, Transform camera)
    {
        CharacterTargetRotate = character.localRotation;
        CameraTargetRotate = camera.localRotation;
    }
    public void LookRotation(Transform character, Transform camera)
    {
        float yRotate = Input.GetAxis("Mouse X") * XSensitivity;
        float xRotate = Input.GetAxis("Mouse Y") * YSensitivity;

        CharacterTargetRotate *= Quaternion.Euler(0f, yRotate, 0f);
        CameraTargetRotate *= Quaternion.Euler(-xRotate, 0f, 0f);

        if (clampVR)
            CameraTargetRotate = ClampRotationX(CameraTargetRotate);
        if (smooth)
        {
            character.localRotation = Quaternion.Slerp(character.localRotation, CharacterTargetRotate, smoothTime * Time.deltaTime);
            camera.localRotation = Quaternion.Slerp(camera.localRotation, CameraTargetRotate, smoothTime * Time.deltaTime);
        }
        else
        {
            character.localRotation = CharacterTargetRotate;
            camera.localRotation = CameraTargetRotate;
        }
		UpdateCursorLock();
	}

    public void SetCursorLock(bool value)
    {
        lockCursor = value;
        if (!lockCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void UpdateCursorLock()
    {
        if (lockCursor) InternalLockUpdate();
    }

    private void InternalLockUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            m_cursorIsLocked = false;
        }
        else if (Input.GetMouseButtonUp(0)) m_cursorIsLocked = true;

        if (m_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!m_cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    Quaternion ClampRotationX(Quaternion quat)
    {
        quat.x /= quat.w;
        quat.y /= quat.w;
        quat.z /= quat.w;
        quat.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(quat.x);
        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);
        quat.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return quat;
    }
}
