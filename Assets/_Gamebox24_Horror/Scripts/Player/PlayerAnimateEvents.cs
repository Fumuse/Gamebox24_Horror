using System;
using UnityEngine;

public class PlayerAnimateEvents : MonoBehaviour
{
    public Action OnFootstep;

    public void FootstepEvent()
    {
        OnFootstep?.Invoke();
    }
}