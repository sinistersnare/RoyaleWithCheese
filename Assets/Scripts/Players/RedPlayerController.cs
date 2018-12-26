using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

[SelectionBase]
public class RedPlayerController : Player {
    public override Transform Target { get { return PlayerManager.instance.bluePlayer.transform; } }

    public override XboxController WhichXboxController { get { return XboxController.Second; } }

    public override void AttachCamera() {
        GameObject redCameraObject = Instantiate(this.cameraPrefab, Vector3.zero, Quaternion.identity, this.transform);
        Camera redCamera = redCameraObject.GetComponent<Camera>();
        redCamera.rect = new Rect(0.5f, 0f, 0.5f, 1f);
        CameraController redCameraController = redCameraObject.GetComponent<CameraController>();
        redCameraController.target = this;
        redCameraController.offset = new Vector3(0, -6.75f, -0.5f);
        redCameraController.pitch = 5.1f;
        redCameraObject.name = "Red Player Camera";
    }
}
