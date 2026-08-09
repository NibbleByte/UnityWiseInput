using DevLocker.WiseInput.Sample;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DevLocker.WiseInput.Sample
{
	/// <summary>
	/// Sample player controller responsible for moving the "player".
	/// </summary>
	public class SampleGamePlayerController : MonoBehaviour, SamplePlayerControls.ISample_GameActions
	{
		public float MoveSpeed = 2f;
		public float JumpForce = 4f;

		[SerializeField]
		private Rigidbody m_Rigidbody;
		private Vector3 m_Velocity;

		private SamplePlayerControls m_PlayerControls;

		public void Initialize(SamplePlayerControls controls)
		{
			m_PlayerControls = controls;

			m_PlayerControls.Sample_Game.SetCallbacks(this);
			m_PlayerControls.Enable(this, m_PlayerControls.Sample_Game);
		}

		public void Uninitialize()
		{
			m_PlayerControls.Sample_Game.SetCallbacks(null);
			m_PlayerControls.DisableAll(this);

			m_PlayerControls = null;
		}

		public void OnMovement(InputAction.CallbackContext context)
		{
			float movement = context.ReadValue<float>();
			m_Velocity = new Vector3(movement, 0f, 0f);
		}

		public void OnJump(InputAction.CallbackContext context)
		{
			if (context.performed) {

				float velocityY = m_Rigidbody.linearVelocity.y;
				if (Mathf.Abs(velocityY - 0f) < 0.01f) {
					m_Rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
				}

			}
		}

		void FixedUpdate()
		{
			m_Rigidbody.MovePosition(m_Rigidbody.position + m_Velocity * Time.fixedDeltaTime * MoveSpeed);
		}
	}
}