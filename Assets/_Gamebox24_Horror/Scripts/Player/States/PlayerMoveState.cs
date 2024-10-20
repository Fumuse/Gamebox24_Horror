using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    private readonly int _moveSpeedHash = Animator.StringToHash("MoveSpeed");
    private readonly int _moveBlendTreeHash = Animator.StringToHash("MoveBlendTree");
    private readonly AudioClip _footstepAudioClip = AudioLibrary.Instance.GetAudioClip("footstepWalk");
    
    private const float AnimationDampTime = 0.1f;
    private const float CrossFadeDuration = 0.1f;
    
    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    
    public override void Enter()
    {
        FixStartPosition();
        
        stateMachine.AudioSource.clip = _footstepAudioClip;
        stateMachine.AudioSource.loop = false;
        
        stateMachine.Animator.CrossFadeInFixedTime(_moveBlendTreeHash, CrossFadeDuration);
        
        stateMachine.PlayerAnimateEvents.OnFootstep += OnFootstep;
        stateMachine.InputReader.OnJumpPerformed += SwitchToJumpState;
        stateMachine.InputReader.OnSprintStarted += SwitchToSprintState;
    }

    private void FixStartPosition()
    {
        stateMachine.Velocity.y = Physics.gravity.y;
        
        CalculateMoveDirection();
        Rotate();
        Move();
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

    public override void Exit()
    {
        stateMachine.PlayerAnimateEvents.OnFootstep -= OnFootstep;
        stateMachine.InputReader.OnJumpPerformed -= SwitchToJumpState;
        stateMachine.InputReader.OnSprintStarted -= SwitchToSprintState;
    }
    
    private void SwitchToJumpState()
    {
        stateMachine.SwitchState(new PlayerJumpState(stateMachine));
    }
    
    private void SwitchToSprintState()
    {
        stateMachine.SwitchState(new PlayerSprintState(stateMachine));
    }

    private void OnFootstep()
    {
        if (stateMachine.MoveDirection.sqrMagnitude <= 0f) return;
        
        stateMachine.AudioSource.volume = Random.Range(0.2f, 0.5f);
        stateMachine.AudioSource.pitch = Random.Range(0.8f, 1.2f);
        stateMachine.AudioSource.Play();
    }
}