using Cinemachine;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float minOrthoSize = 3f;
    [SerializeField] private float maxOrthoSize = 6f;

    private CinemachineVirtualCamera _virtualCamera;
    
    private void Start()
    {
        if (mainCamera.TryGetComponent(out CinemachineBrain brain))
        {
            if (brain.ActiveVirtualCamera.VirtualCameraGameObject.TryGetComponent(
                    out CinemachineVirtualCamera virtualCamera))
            {
                _virtualCamera = virtualCamera;
            }
        }
    }

    private void OnEnable()
    {
        InputReader.OnCameraZoom += OnCameraZoom;
    }

    private void OnDisable()
    {
        InputReader.OnCameraZoom -= OnCameraZoom;
    }

    private void OnCameraZoom(int zoomModifier)
    {
        if (mainCamera.orthographicSize + zoomModifier > maxOrthoSize) return;
        if (mainCamera.orthographicSize + zoomModifier < minOrthoSize) return;
        
        if (_virtualCamera != null)
        {
            _virtualCamera.m_Lens.OrthographicSize += zoomModifier;
        }
        else
        {
            mainCamera.orthographicSize += zoomModifier;
        }
    }
}