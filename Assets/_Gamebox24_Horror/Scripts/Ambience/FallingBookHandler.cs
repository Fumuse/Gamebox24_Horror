using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FallingBookHandler : MonoBehaviour
{
    [SerializeField] private float rayDistance = 10f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Vector3 raycastDirection;
    [SerializeField] private Vector3 raycastPositionModifier;
    
    private readonly int _bookTriggerHash = Animator.StringToHash("BookFall");
    
    private CancellationTokenSource _cancellationSource;
    private Animator _animator;
    private AudioSource _audioSource;
    private Vector3 _raycastPosition;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
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
        _raycastPosition = new Vector3(
            transform.position.x + raycastPositionModifier.x, 
            transform.position.y + raycastPositionModifier.y, 
            transform.position.z + raycastPositionModifier.z
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
    /// Проверяем, пересёк ли игрок триггер для активации анимации падения книги
    /// </summary>
    private void CheckTrigger()
    {
        Debug.DrawRay(_raycastPosition, raycastDirection * rayDistance, Color.red);

        if (Physics.Raycast(_raycastPosition, raycastDirection, rayDistance, playerLayer))
        {
            if (_cancellationSource != null) 
                _cancellationSource.Cancel();
            
            _animator.SetTrigger(_bookTriggerHash);
        }
    }

    private void SoundEvent()
    {
        _audioSource.Play();
    }
}
