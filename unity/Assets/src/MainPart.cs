using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Osakana4242 {
	public sealed class MainPart : MonoBehaviour {
		public Data data;

		public TMPro.TextMeshProUGUI progressTextUI;
		public TMPro.TextMeshProUGUI centerTextUI;

		StateMachine<MainPart> sm_;
		public ResourceBank resource;
		public GameObject cameraGo;
		public PlayerController player;
		public BallController ball;

		void Awake() {
			sm_ = new StateMachine<MainPart>(stateInit_g_);
			Application.logMessageReceived += OnLog;
		}

		public void OnLog(string condition, string stackTrace, LogType type) {
			switch (type) {
				case LogType.Exception:
				Debug.Break();
				GameObject.Destroy(gameObject);
				Application.Quit();
				break;
			}
		}

		void OnDestroy() {
			Application.logMessageReceived -= OnLog;
			sm_ = null;
		}

		void FixedUpdate() {
			if (data.isPlaying) {
			}
			player.ManualFixedUpdate();
			ball.ManualFixedUpdate();
		}

		void Update() {
			sm_.Update(this);
		}

		static StateMachine<MainPart>.StateFunc stateExit_g_ = (_evt) => {
			var self = _evt.owner;
			switch (_evt.type) {
				case StateMachineEventType.Enter: {
						self.data.isPlaying = false;
						UnityEngine.SceneManagement.SceneManager.LoadScene("main");
						return null;
					}
				default:
				return null;
			}
		};

		static StateMachine<MainPart>.StateFunc stateInit_g_ = (_evt) => {
			switch (_evt.type) {
				case StateMachineEventType.Enter: {
						var self = _evt.owner;
						self.progressTextUI.text = "";
						self.centerTextUI.text = "READY";

						{
							Random.InitState(1);
						}
						return null;
					}
				case StateMachineEventType.Update: {
						if (1f <= _evt.sm.time) {
							return stateMain_g_;
						}
						return null;
					}

				default:
				return null;
			}
		};

		bool hasJump_;

		static StateMachine<MainPart>.StateFunc stateMain_g_ = (_evt) => {
			var self = _evt.owner;
			// self.StepWave();
			self.data.isPlaying = true;

			self.player.ManualUpdate();

			{
				var sb = new System.Text.StringBuilder();
				sb.AppendFormat("{0:F0}\n", self.player.hitCount);
				self.progressTextUI.text = sb.ToString();
			}
			{
				self.centerTextUI.text = "";
			}

			var hasGameOver = 1 <= self.player.hitCount && self.ball.hasGround;
			if (hasGameOver) {
				return stateGameOver_g_;
			}

			if (Input.GetKeyDown(KeyCode.Z)) {
				self.hasJump_ = true;
			}

			if (Input.GetKeyDown(KeyCode.R)) {
				return stateExit_g_;
			}

			return null;
		};

		static StateMachine<MainPart>.StateFunc stateGameOver_g_ = (_evt) => {
			var self = _evt.owner;
			switch (_evt.type) {
				case StateMachineEventType.Enter:
				self.centerTextUI.text = "NO MIYABI...";
				self.data.isPlaying = false;
				break;
			}

			if (3f <= _evt.sm.time) {
				return stateResult_g_;
			}

			return null;
		};

		static StateMachine<MainPart>.StateFunc stateResult_g_ = (_evt) => {
			var self = _evt.owner;
			switch (_evt.type) {
				case StateMachineEventType.Enter:
				self.centerTextUI.text = "PRESS Z KEY";
				self.data.isPlaying = false;
				self.cameraGo.GetComponent<CameraController>().target = null;
				break;
			}

			if (Input.GetKeyDown(KeyCode.Z)) {
				return stateExit_g_;
			}

			return null;
		};

		[System.Serializable]
		public class Data {
			public bool isPlaying;
		}

	}
}
