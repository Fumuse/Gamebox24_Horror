using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class DarknessHaunting : MonoBehaviour
{
    [SerializeField] private GameObject darkness;
    [SerializeField] private Transform hauntingTarget;
    [SerializeField] private float minDistance = 15f;
    [SerializeField] private float secondForWaitBeforeDarknessSpawn = 5f;
    [SerializeField] private float followSpeed = 3f;
    [SerializeField] private float darknessFadeInDuration = 3f;
    [SerializeField] private float maxDarknessEmission = 5f;
    [SerializeField] private float maxWhisperVolume = .2f;
    [SerializeField] private PostProcessVolume postVolume;
    [SerializeField] private GameObject gameOverTextContainer;

    private CancellationTokenSource _tokenSource;

    public static Action OnDarknessEnter;

    private void OnEnable()
    {
        _tokenSource = new();
        
        RostrumHandler.OnRostrumTriggerEnded += OnRostrumTriggerEnded;
        EndGameTrigger.OnEndGameTriggerEnter += OnEndGameTriggerEnter;
    }

    private void OnDisable()
    {
        if (_tokenSource != null)
            _tokenSource.Cancel();
        
        RostrumHandler.OnRostrumTriggerEnded -= OnRostrumTriggerEnded;
        EndGameTrigger.OnEndGameTriggerEnter -= OnEndGameTriggerEnter;
    }

    private async void Haunting()
    {
        while (true)
        {
            bool isCanceled = await UniTask.WaitForEndOfFrame(this, _tokenSource.Token).SuppressCancellationThrow();
            if (isCanceled) return;

            CheckDistanceToTargetAndMove();
        }
    }

    /// <summary>
    /// Включаем отображение Тьмы
    /// </summary>
    private async void ShowDarkness()
    {
        ParticleSystem fog = darkness.GetComponentInChildren<ParticleSystem>();
        var fogMain = fog.main;
        fogMain.startLifetime = 0;
        
        darkness.SetActive(true);
        fog.Play();
        
        StartAudio();
        
        float elapsedTime = 0f;
        while (elapsedTime < darknessFadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            
            float time = Mathf.Clamp01(elapsedTime / darknessFadeInDuration);
            fogMain.startLifetime = Mathf.Lerp(0, maxDarknessEmission, time);
            
            bool isCanceled = await UniTask.WaitForEndOfFrame(this, _tokenSource.Token).SuppressCancellationThrow();
            if (isCanceled) return;
        }
        
        fogMain.startLifetime = maxDarknessEmission;
    }

    /// <summary>
    /// Включаем звук шёпота
    /// </summary>
    private void StartAudio()
    {
        AudioSource audioSource = darkness.GetComponent<AudioSource>();
        audioSource.volume = 0;
        
        DOTween.To(
            () => audioSource.volume, 
            x => audioSource.volume = x, 
            maxWhisperVolume, 
            darknessFadeInDuration
        );
    }

    /// <summary>
    /// Проверяем расстояние между Тьмой и игроком и перемещаем Тьму следом за игроком
    /// </summary>
    private void CheckDistanceToTargetAndMove()
    {
        float distance = Vector3.Distance(hauntingTarget.transform.position, transform.position);
        if (distance > minDistance)
        {
            Vector3 nextPosition = hauntingTarget.position - (new Vector3(0, 0, 1f) * minDistance);
            transform.position = Vector3.Lerp(transform.position, nextPosition, followSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Активируем Тьму через несколько секунд после завершения чтения свитка на трибуне
    /// </summary>
    private async void OnRostrumTriggerEnded()
    {
        bool isCanceled = await UniTask.WaitForSeconds(
            secondForWaitBeforeDarknessSpawn, cancellationToken: _tokenSource.Token).SuppressCancellationThrow();
        if (isCanceled) return;

        ShowDarkness();
        Haunting();
    }

    /// <summary>
    /// Завершаем игроку, если игрок вошёл внутрь Тьмы
    /// </summary>
    /// <param name="other"></param>
    private async void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerStateMachine player))
        {
            if (_tokenSource != null)
                _tokenSource.Cancel();
            
            OnDarknessEnter?.Invoke();
                
            Tween tween = DOTween.To(
                () => postVolume.weight, 
                x => postVolume.weight = x, 
                1f, 
                2f
            );

            _tokenSource = new();
            bool isCanceled = await tween.AsyncWaitForCompletion().AsUniTask()
                .AttachExternalCancellation(_tokenSource.Token).SuppressCancellationThrow();
            if (isCanceled) return;
            
            gameOverTextContainer.SetActive(true);
        }
    }

    /// <summary>
    /// Резко перемещаем Тьму для завершения игры при пересечении последнего триггера
    /// </summary>
    private void OnEndGameTriggerEnter()
    {
        if (_tokenSource != null)
            _tokenSource.Cancel();

        transform.DOMove(hauntingTarget.transform.position, 1f);
    }
}
