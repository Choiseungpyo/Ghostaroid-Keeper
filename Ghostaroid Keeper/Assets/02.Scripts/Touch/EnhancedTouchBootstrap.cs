using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class EnhancedTouchBootstrap : Singleton<EnhancedTouchBootstrap>
{
    protected override void Awake()
    {
        base.Awake();
        EnhancedTouchSupport.Enable();
    }
}