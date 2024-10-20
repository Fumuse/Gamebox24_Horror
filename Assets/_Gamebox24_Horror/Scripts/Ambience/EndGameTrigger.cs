using System;
using UnityEngine;

public class EndGameTrigger : MonoBehaviour
{
    public static Action OnEndGameTriggerEnter;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerStateMachine player))
        {
            OnEndGameTriggerEnter?.Invoke();
        }
    }
}
