using UnityEngine;

[RequireComponent(typeof(InputReader))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PlayerAnimateEvents))]
public class PlayerStateMachine : StateMachine
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private float sprintModifier = 2f;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float jumpForce = 5f;

    public float MovementSpeed => speed;
    public float RotationSpeed => rotationSpeed;
    public float SprintModifier => sprintModifier;
    public float JumpForce => jumpForce;

    public Vector2 MoveDirection
    {
        get
        {
            if (PlayerControlsLocked) return Vector2.zero;
            return InputReader.PlayerMovePosition;
        }
    }

    public bool PlayerControlsLocked { get; private set; } = false;

    public Vector3 Velocity;

    public InputReader InputReader { get; private set; }
    public Animator Animator { get; private set; }
    public CharacterController Controller { get; private set; }
    public AudioSource AudioSource { get; private set; }
    public AudioSource ReserveAudioSource { get; private set; }
    public PlayerAnimateEvents PlayerAnimateEvents { get; private set; }

    private void Start()
    {
        InputReader = GetComponent<InputReader>();
        Animator = GetComponent<Animator>();
        Controller = GetComponent<CharacterController>();

        AudioSource[] audioSources = GetComponents<AudioSource>();
        AudioSource = audioSources[0];
        ReserveAudioSource = audioSources[1];
        
        PlayerAnimateEvents = GetComponent<PlayerAnimateEvents>();

        SwitchState(new PlayerMoveState(this));
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        RostrumHandler.OnRostrumTriggered += OnLockControls;
        RostrumHandler.OnRostrumTriggerEnded += OnUnlockControls;
        DarknessHaunting.OnDarknessEnter += OnLockControls;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        RostrumHandler.OnRostrumTriggered -= OnLockControls;
        RostrumHandler.OnRostrumTriggerEnded -= OnUnlockControls;
        DarknessHaunting.OnDarknessEnter -= OnLockControls;
    }

    protected void OnLockControls()
    {
        PlayerControlsLocked = true;
        InputReader.enabled = false;
    }

    protected void OnUnlockControls()
    {
        PlayerControlsLocked = false;
        InputReader.enabled = true;
    }
}