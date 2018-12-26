using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

[SelectionBase]
public class BluePlayerController : Player {
    public override Transform Target { get { return PlayerManager.instance.redPlayer ? PlayerManager.instance.redPlayer.transform : null; } }

    public override XboxController WhichXboxController { get { return XboxController.First; } }

    public override void AttachCamera() {
        GameObject blueCameraObject = Instantiate(this.cameraPrefab, Vector3.zero, Quaternion.identity, this.transform);
        Camera blueCamera = blueCameraObject.GetComponent<Camera>();
        if (PlayerManager.instance.twoPlayerMode) {
            blueCamera.rect = new Rect(0f, 0f, 0.5f, 1f);
        } else {
            blueCamera.rect = new Rect(0f, 0f, 1f, 1f);
        }
        CameraController blueCameraController = blueCameraObject.GetComponent<CameraController>();
        blueCameraController.target = this;
        blueCameraController.offset = new Vector3(0, -6.75f, -0.5f);
        blueCameraController.pitch = 5.1f;
        blueCameraObject.name = "Blue Player Camera";
        blueCameraObject.GetComponent<AudioListener>().enabled = true;
    }
}
