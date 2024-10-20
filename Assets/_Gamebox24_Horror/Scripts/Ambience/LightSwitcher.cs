using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LightSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject[] lights;
    [SerializeField] private float rayDistance = 10f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Vector3 raycastDirection;
    [SerializeField] private Vector3 raycastPositionModifier;

    private BoxCollider _collider;
    private CancellationTokenSource _cancellationSource;

    private Vector3 _raycastPosition;

    private void Start()
    {
        _collider = GetComponent<BoxCollider>();
        GetRaycastPointPosition();
    }

    private void OnEnable()
    {
        _cancellationSource = new();
        WaitTrigger();
    }

    private void OnDisable()
    {
        if (_cancellationSource != null)
            _cancellationSource.Cancel();
    }
    
    /// <summary>
    /// Формируем точку для луча-триггера
    /// </summary>
    private void GetRaycastPointPosition()
    {
        Bounds bounds = _collider.bounds;
        _raycastPosition = new Vector3(
            bounds.min.x + raycastPositionModifier.x, 
            bounds.min.y + raycastPositionModifier.y, 
            bounds.max.z + raycastPositionModifier.z
        );
    } 

    private async void WaitTrigger()
    {
        while(true)
        {
            CheckTrigger();
            
            bool isCanceled = await UniTask.WaitForEndOfFrame(this, _cancellationSource.Token)
                .SuppressCancellationThrow();
            if (isCanceled) return;
        }
    }

    /// <summary>
    /// Проверяем, пересёк ли игрок триггер для активации затухания света
    /// </summary>
    private void CheckTrigger()
    {
        Debug.DrawRay(_raycastPosition, raycastDirection * rayDistance, Color.red);
        
        if (Physics.Raycast(_raycastPosition, raycastDirection, rayDistance, targetLayer))
        {
            TurnOffLights();
        }
    }

    /// <summary>
    /// Тушим свет
    /// </summary>
    private void TurnOffLights()
    {
        if (_cancellationSource != null) 
            _cancellationSource.Cancel();
        
        foreach (GameObject light in lights)
        {
            light.SetActive(false);
        }
    }
}
