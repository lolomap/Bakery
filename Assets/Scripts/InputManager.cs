using System;
using UnityEngine;
using UnityEngine.Serialization;

public class InputManager : MonoBehaviour
{ 
    public static InputManager Instance;
    
    public Vector2 CurrentPosition { get; private set; } = Vector2.zero;
    public Vector2 DeltaPosition { get; private set; } = Vector2.zero;
    
    private Vector2 _lastPosition = Vector2.zero;

    private void Awake()
    {
        Instance ??= this;
    }
    
    public void Update()
    {
        CurrentPosition = Input.mousePosition;
        DeltaPosition = CurrentPosition - _lastPosition;
        _lastPosition = CurrentPosition;
		
        //float scroll = Input.mouseScrollDelta.y;
    }
}