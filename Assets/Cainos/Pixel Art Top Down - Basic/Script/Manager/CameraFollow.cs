using UnityEngine;
using Cinemachine;

public class CameraFollow : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;

    public void ChangeFollowTarget(Transform newTarget)
    {
        virtualCamera.Follow = newTarget;
        virtualCamera.LookAt = newTarget; // nếu bạn cũng muốn camera nhìn vào target
    }
}
