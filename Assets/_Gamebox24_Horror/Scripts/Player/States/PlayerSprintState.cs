using UnityEngine;

public class PlayerSprintState : PlayerBaseState
{
    private readonly int _moveSpeedHash = Animator.StringToHash("MoveSpeed");
    private readonly int _sprintBlendTreeHash = Animator.StringToHash("SprintBlendTree");
    private readonly AudioClip _footstepAudioClip = AudioLibrary.Instance.GetAudioClip("footstepRun");
    
    private const float CrossFadeDuration = 0.1f;
    private const float AnimationDampTime = 0.1f;
    
    public PlayerSprintState(PlayerStateMachine stateMachine) : base(stateMachine)
    {}

    public override void Enter()
    {
        stateMachine.AudioSource.clip = _footstepAudioClip;
        stateMachine.AudioSource.loop = false;
        
        stateMachine.Animator.CrossFadeInFixedTime(_sprintBlendTreeHash, CrossFadeDuration);
        
        stateMachine.PlayerAnimateEvents.OnFootstep += OnFootstep;
        stateMachine.InputReader.OnJumpPerformed += SwitchToJumpState;
        stateMachine.InputReader.OnSprintEnded += SwitchToMoveState;
    }

    public override void Tick()
    {
        if (!stateMachine.Controller.isGrounded)
        {
            stateMachine.SwitchState(new PlayerFallState(stateMachine));
        }

        CalculateMoveDirection();
        Rotate();
        Move();
        
        stateMachine.Animator.SetFloat(_moveSpeedHash, 
            stateMachine.MoveDirection.sqrMagnitude > 0f ? 1f : 0f, 
            AnimationDampTime, 
            Time.deltaTime
        );
    }

    protected override void CalculateMoveDirection()
    {
        base.CalculateMoveDirection();

        stateMachine.Velocity.x *= stateMachine.SprintModifier;
        stateMachine.Velocity.z *= stateMachine.SprintModifier;
    }

    public override void Exit()
    {
        stateMachine.PlayerAnimateEvents.OnFootstep -= OnFootstep;
        stateMachine.InputReader.OnJumpPerformed -= SwitchToJumpState;
        stateMachine.InputReader.OnSprintEnded -= SwitchToMoveState;
    }
    
    private void SwitchToJumpState()
    {
        stateMachine.SwitchState(new PlayerJumpState(stateMachine));
    }
    
    private void SwitchToMoveState()
    {
        stateMachine.SwitchState(new PlayerMoveState(stateMachine));
    }

    private void OnFootstep()
    {
        stateMachine.AudioSource.volume = Random.Range(0.1f, 0.4f);
        stateMachine.AudioSource.pitch = Random.Range(0.8f, 1.2f);
        stateMachine.AudioSource.Play();
    }
}