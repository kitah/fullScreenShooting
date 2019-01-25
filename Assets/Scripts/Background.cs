using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 背景を制御するコンポーネント
public class Background : MonoBehaviour {

	public Transform m_player; // プレイヤー
	public Vector2 m_limit;

	
	// Update is called once per frame
	void Update () {
		// プレイヤーの現在位置を取得
		var pos = m_player.localPosition;

		// 画面端の位置を取得
		var limit = Utils.m_moveLimit;

		// プレイヤーが画面のどの位置に存在するのかを0から1の値に置き換え
		var tx = 1 - Mathf.InverseLerp(-limit.x, limit.x, pos.x);
		var ty = 1 - Mathf.InverseLerp(-limit.y, limit.y, pos.y);

		// プレイヤーの現在位置から背景の表示位置を算出
		var x = Mathf.Lerp(-m_limit.x, m_limit.x, tx);
		var y = Mathf.Lerp(-m_limit.y, m_limit.y, ty);

		// 背景の表示位置を更新
		transform.localPosition = new Vector3(x, y, 0);
	}
}
