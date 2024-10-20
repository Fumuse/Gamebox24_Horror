using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RostrumHandler : MonoBehaviour
{
    [SerializeField] private GameObject rostrumUIPanel;
    [SerializeField] private float closePanelWaitTime = 5f;
    private readonly int _lightActiveHash = Animator.StringToHash("RostrumActive");
    
    private Animator _animator;
    private AudioSource _audioSource;
    private CancellationTokenSource _tokenSource;

    public static Action OnRostrumTriggered;
    public static Action OnRostrumTriggerEnded;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnDisable()
    {
        if (_tokenSource != null)
            _tokenSource.Cancel();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!this.enabled) return;
        
        OnRostrumTriggered?.Invoke();
        
        _audioSource.Play();
        rostrumUIPanel.SetActive(true);
        
        LightOff();
        ClosePanel();
    }

    /// <summary>
    /// Автозакрытие книги
    /// </summary>
    private async void ClosePanel()
    {
        _tokenSource = new();
        bool isCanceled = await UniTask.WaitForSeconds(closePanelWaitTime, cancellationToken: _tokenSource.Token).SuppressCancellationThrow();
        if (isCanceled) return;
        
        rostrumUIPanel.SetActive(false);
        
        OnRostrumTriggerEnded?.Invoke();

        this.enabled = false;
    }

    /// <summary>
    /// Выключаем свечение
    /// </summary>
    private void LightOff()
    {
        _animator.SetBool(_lightActiveHash, true);
    }
}