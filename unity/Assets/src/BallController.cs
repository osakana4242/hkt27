using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class BallController : MonoBehaviour {
	public Vector3 velocity;
	public float gravity = 2f;
	public Rect movableRect = new Rect(-3f, 0f, 6f, 10f);
	public bool hasGround;

	public void ManualFixedUpdate() {
		var pos = transform.position;
		if (!hasGround) {
			velocity.y -= gravity * Time.deltaTime;
		}
		pos += velocity * Time.deltaTime;

		if (pos.x < movableRect.xMin ) {
			pos.x = movableRect.xMin;
			velocity.x = Mathf.Abs(velocity.x);
		}
		if (movableRect.xMax <= pos.x) {
			pos.x = movableRect.xMax - float.Epsilon;
			velocity.x = -Mathf.Abs(velocity.x);
		}

		if (pos.y < movableRect.yMin) {
			pos.y = movableRect.yMin;
            velocity.y = Mathf.Abs(velocity.y * 0.1f);
            velocity.x = velocity.x * 0.1f;
			hasGround = true;
		} else {
			hasGround = false;
		}

		transform.position = pos;
	}

	// Update is called once per frame
	public void ManualUpdate() {

	}
}
