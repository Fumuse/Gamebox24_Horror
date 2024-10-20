using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    private readonly int _fallHash = Animator.StringToHash("Fall");
    private readonly AudioClip _landingAudioClip = AudioLibrary.Instance.GetAudioClip("groundLanding");
    
    private const float CrossFadeDuration = 0.1f;

    public PlayerFallState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.Velocity.y = 0f;
        
        stateMachine.ReserveAudioSource.clip = _landingAudioClip;
        stateMachine.ReserveAudioSource.loop = false;
        stateMachine.ReserveAudioSource.pitch = Random.Range(0.8f, 1.2f);
        stateMachine.ReserveAudioSource.volume = Random.Range(0.1f, 0.4f);

        stateMachine.Animator.CrossFadeInFixedTime(_fallHash, CrossFadeDuration);
    }

    public override void Tick()
    {
        ApplyGravity();
        Move();

        if (stateMachine.Controller.isGrounded)
        {
            stateMachine.ReserveAudioSource.Play();
            
            if (stateMachine.InputReader.PlayerSprinted)
            {
                stateMachine.SwitchState(new PlayerSprintState(stateMachine));
            }
            else
            {
                stateMachine.SwitchState(new PlayerMoveState(stateMachine));
            }
        }
    }

    public override void Exit() { }
}