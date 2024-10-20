using UnityEngine;

public abstract class PlayerBaseState : State
{
    protected readonly PlayerStateMachine stateMachine;
    
    private Matrix4x4 _matrix;
    
    protected PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        _matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    }

    protected virtual void CalculateMoveDirection()
    {
        Vector3 movement = new Vector3(stateMachine.MoveDirection.x, 0f, stateMachine.MoveDirection.y);
        Vector3 skewedMovement = _matrix.MultiplyPoint3x4(movement);
        
        stateMachine.Velocity.x = skewedMovement.x * stateMachine.MovementSpeed;
        stateMachine.Velocity.z = skewedMovement.z * stateMachine.MovementSpeed;
    }
    
    protected void ApplyGravity()
    {
        if (stateMachine.Velocity.y > Physics.gravity.y)
        {
            stateMachine.Velocity.y += Physics.gravity.y * Time.deltaTime;
        }
    }

    protected void Move()
    {
        stateMachine.Controller.Move(stateMachine.Velocity * Time.deltaTime);
    }

    protected void Rotate()
    {
        if (stateMachine.MoveDirection == Vector2.zero) return;
        
        Vector3 direction = new Vector3(
            stateMachine.MoveDirection.x, 
            0f, 
            stateMachine.MoveDirection.y).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        
        stateMachine.transform.rotation = Quaternion.RotateTowards(
            stateMachine.transform.rotation, 
            targetRotation, 
            stateMachine.RotationSpeed * Time.deltaTime
        );
    }
}