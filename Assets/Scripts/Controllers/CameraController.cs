using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class CameraController : MonoBehaviour {
    public Vector3 offset;
    public float pitch = 2f;
    public float zoomSpeed = 0.01f;
    public float minZoom = 5f;
    public float maxZoom = 15f;
    public float yawSpeed = 100f;
    public float minYaw = -10f;
    public float maxYaw = 10f;

    public Player target;

    private float currentYaw = 0f;
    private float currentZoom = 10;
    
    void LateUpdate() {
        Transform targetTransform = this.target.gameObject.transform;

        float moveCamVert = XCI.GetAxis(this.target.MoveCameraVerticalAxis, this.target.WhichXboxController);
        float moveCamHor = XCI.GetAxis(this.target.MoveCameraHorizontalAxis, this.target.WhichXboxController);
        if (XCI.GetNumPluggedCtrlrs() == 0) {
            moveCamVert = Input.GetAxis("Mouse X");
            moveCamHor = Input.GetAxis("Mouse Y");
        }
        
        this.currentZoom -=  moveCamVert * zoomSpeed;
        this.currentZoom = Mathf.Clamp(this.currentZoom, this.minZoom, this.maxZoom);
        
        float yaw = moveCamHor;
        this.currentYaw += yaw * this.yawSpeed * Time.deltaTime;
        this.currentYaw = Mathf.Clamp(this.currentYaw, this.minYaw, this.maxYaw);
        this.transform.position = targetTransform.position - targetTransform.forward * this.currentZoom - this.offset;
        this.transform.LookAt(targetTransform.position + Vector3.up * this.pitch);
        this.transform.RotateAround(targetTransform.position, Vector3.up, this.currentYaw);
    }
}