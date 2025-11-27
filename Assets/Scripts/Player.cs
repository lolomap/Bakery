using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, Controls.IPlayerActions
{
	public float Speed;
	public Quaternion MovementRotation = Quaternion.Euler(0f, 45f, 0f);

	private Vector3 _direction;

	private CharacterController _controller;

	private void Awake()
	{
		_controller = GetComponent<CharacterController>();
	}

	private void Update()
	{
		_controller.Move(_direction * (Speed * Time.deltaTime));	
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		Vector2 inputDirection = context.ReadValue<Vector2>();
		
		Matrix4x4 isoMatrix = Matrix4x4.Rotate(MovementRotation);
		_direction = isoMatrix.MultiplyVector(new(inputDirection.x, 0f, inputDirection.y));
	}

	public void OnLook(InputAction.CallbackContext context)
	{
		
	}

	public void OnJump(InputAction.CallbackContext context)
	{
		
	}
}