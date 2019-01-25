using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

	public Enemy[] m_enemyPrefabs; // 敵のプレハブを管理する配列
	public float m_intervalFrom; // 出現間隔(ゲームの経過時間が0sのとき)
	public float m_intervalTo; // 出現間隔(ゲームの経過時間がm_elapsedTimeMaxの時)
	public float m_elapsedTimeMax; // 経過時間の最大値
	public float m_elapsedTime; // 経過時間

	
	private float m_timer; // 出現タイミングを管理するタイマー
	
	// Update is called once per frame
	private void Update () {
		// 経過時間を更新する
		m_elapsedTime += Time.deltaTime;

		// 出現タイミングを管理するタイマーを更新する
		m_timer += Time.deltaTime;

		// ゲームの経過時間から出現間隔を算出 ゲームの経過時間が長くなるほど、敵の出現間隔が短くなる
		var t = m_elapsedTime / m_elapsedTimeMax;
		var interval = Mathf.Lerp(m_intervalFrom, m_intervalTo, t);

		// まだ敵が出現するタイミングではない場合、このフレームの処理はここで終える
		if(m_timer < interval) return;

		// 出現タイミングを管理するタイマーをリセット
		m_timer = 0;

		// 出現する敵をランダムに決定
		var enemyIndex = Random.Range(0, m_enemyPrefabs.Length);

		// 出現する敵のプレハブを配列から取得
		var enemyPrefab = m_enemyPrefabs[enemyIndex];

		// 敵のゲームオブジェクト生成
		var enemy = Instantiate(enemyPrefab);

		// 敵を画面外のどの位置に出現させるかランダムに
		var respawnType = (RESPAWN_TYPE)Random.Range(0, (int)RESPAWN_TYPE.SIZEOF);

		// 敵を初期化
		enemy.Init(respawnType);
	}
}
