using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Osakana4242 {
	public class PlayerController : MonoBehaviour {

		public float maxSpeed = 7;
		public float jumpTakeOffSpeed = 7;

		public Vector3 velocity;
		public Rect movableRect = new Rect(-3f, 0f, 6f, 10f);
		public BallController ball;
		public Vector2 kickPower = new Vector2(1f, 3f);
		public Animator animator;
		public int hitCount;

		void Awake() {
			gameObject.OnCollisionEnter2DAsObservable().
				TakeUntilDestroy(gameObject).
				Subscribe(_collision => {
					// rb
					//  +- collider1
					// collider1 が衝突した場合
					// _collision.gameObject が指すのは collider1 の gameObject ではなく rb の gameObject

					var go = _collision.gameObject;
					if (go == null) return;
					var ball = go.GetComponent<BallController>();
					if (ball == null) return;
					this.ball = ball;
				});

			gameObject.OnCollisionExit2DAsObservable().
				TakeUntilDestroy(gameObject).
				Subscribe(_collision => {
					var go = _collision.gameObject;
					if (go == null) return;
					var ball = go.GetComponent<BallController>();
					if (ball == null) return;
					this.ball = null;
				});
		}

		public void ManualFixedUpdate() {
			var pos = transform.position;

			if (pos.x < movableRect.xMin) {
				pos.x = movableRect.xMin;
			}
			if (movableRect.xMax <= pos.x) {
				pos.x = movableRect.xMax - float.Epsilon;
			}

			pos += velocity * Time.deltaTime;
			transform.position = pos;

			if (hasKick) {
				hasKick = false;
				if (ball != null) {
					var posA = transform.position;
					var posB = ball.transform.position;
					var d = posB - posA;
					var x = Mathf.Clamp(d.x * 2f, -1f, 1f);
					ball.velocity.x = x * kickPower.x;
					ball.velocity.y = kickPower.y;
					hitCount += 1;
					kickPower.x += 0.25f; // power up
				}
			}
		}

		public bool hasKick;
		Vector3 lastInput;

		public void ManualUpdate() {
			Vector3 move = Vector2.zero;
			move.x = Input.GetAxis("Horizontal");
			if (move.x != 0f) {
				lastInput = move.normalized;
			}
			if (Input.GetKeyDown(KeyCode.Z)) {
				this.hasKick = true;
				move = lastInput;
				animator.Play("kick", 0, 0f);
			}
			velocity = move * maxSpeed;

		}
	}
}