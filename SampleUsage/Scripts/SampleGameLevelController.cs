using UnityEngine;

namespace DevLocker.WiseInput.Sample
{
	/// <summary>
	/// Sample level controler that manages the sample game.
	/// </summary>
	public class SampleGameLevelController : MonoBehaviour
	{
		public GameObject LevelRoot;
		public SampleGamePlayerController Player;

		private void OnEnable()
		{
			LevelRoot.SetActive(true);
			Player.Initialize(SampleSceneController.Instance.PlayerControls);
		}

		private void OnDisable()
		{
			// Stopping play mode may already have destroyed these objects.
			if (Player == null || LevelRoot == null)
				return;

			Player.Uninitialize();
			LevelRoot.SetActive(false);
		}
	}
}