using System.Linq;
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

		private InputActionsMaskedStack.InputActionConflictsReport m_LastInputConflictsReport = new();

		public void Initialize(SamplePlayerControls controls)
		{
			m_PlayerControls = controls;

			m_PlayerControls.Sample_Game.SetCallbacks(this);				// Subscribes this ISample_GameActions interface
			m_PlayerControls.Enable(this, m_PlayerControls.Sample_Game);    // Enables actions so they actually fire the events.

			// ===================================================
			// HOW THIS WORKS:
			//
			// The input context enables the requested actions if no input mask is used.
			// Here this is done by the SamplePlayerControls, which implements IInputContext interface to simplify things. Check the SamplePlayerControlsExtension.cs file.
			// Pass "this" to the "source" parameter, so you can track easily who is enabling the actions.
			//
			// The context remembers what actions were enabled by the user. When an UIScope or other pushes an input mask to the stack, all action states are reavluated
			// and only the actions that are allowed by the current mask remain enabled. The rest are disabled. Check InputActionsMaskedStack.PushOrSetActionsMask() method for more details.
			//
			// Example:
			// When system pop-up opens up it's UIScope should have Focus Layer set with the option to isolate input.
			// This will push input mask with the UIScope managed actions to the stack. The input context will then disable all actions that are not allowed by the mask.
			// When the pop-up is closed, the mask is removed and initial (your) input actions are enabled again.
			//
			// IMPORTANT: for all this to work, input actions MUST ALWAYS be enabled through the input context, never directly through InputAction.Enable() method.
			//			  Check the LateUpdate() method below for an example on how to watch out for out of context enabled actions and report them.
			//
			// IMPORTANT 2: UI input actions like clicks, navigation, scrolls etc. are special and should be used ONLY by the InputSystemUIInputModule. Check the SampleSceneController for more info.
			// ===================================================
		}

		public void Uninitialize()
		{
			m_PlayerControls.Sample_Game.SetCallbacks(null);
			m_PlayerControls.DisableAll(this); // Because enabling source was "this", we can easily disable all actions that were enabled by us. Yay!

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

		private void LateUpdate()
		{
			// Check for InputActions conflicts at the end of every frame and report.
			if (m_PlayerControls != null) {
				var conflictsReport = m_PlayerControls.InputActionsMaskedStack.GetConflictingActionRequests(m_PlayerControls.GetUIActions());

				if (!m_LastInputConflictsReport.Equals(conflictsReport) && conflictsReport.HasIssuesFound) {
					var conflictStrings = conflictsReport.Conflicts.Select(pair => $"- {pair.Key.name} [{string.Join(", ", pair.Value)}]");
					var illegalStrings = conflictsReport.IllegalActions.Select(action => $"- {action.name} [ILLEGAL]");

					Debug.LogError($"[Input] Input actions in conflict found:\n{string.Join('\n', conflictStrings.Concat(illegalStrings))}", this);
				}

				m_LastInputConflictsReport = conflictsReport;
			}
		}
	}
}