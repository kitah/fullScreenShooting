using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// プレイヤーが発射する弾を制御するコンポーネント
public class Shot : MonoBehaviour {

	// 速度
	private Vector3 m_velocity;

	// Update is called once per frame
	void Update () {
		transform.localPosition += m_velocity;
	}

	public void Init(float angle, float speed) {
		// 弾の発射角度をベクトルに変換
		var direction = Utils.GetDirection(angle);

		// 発射角度と速さから速度を求める
		m_velocity = direction * speed;

		// 弾が進行方向を向くように
		var angles = transform.localEulerAngles;
		angles.z = angle - 90;
		transform.localEulerAngles = angles;

		// 2秒後に削除
		Destroy(gameObject, 2);
	}
}
