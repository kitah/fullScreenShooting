using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 宝石制御コンポーネント
public class Gem : MonoBehaviour {
	public int m_exp; // 取得できる経験値
	public float m_breke = 0.9f; // 散らばる時の減速量、数値が小さいほどすぐ減速
	public float m_followAccel = 0.01f; // プレイヤーを追尾する時の加速度、数値が大きいほどすぐ加速
	public AudioClip m_goldClip; // 宝石を取得した時に再生するSE
	
	private Vector3 m_direction; // 散らばる時の進行方向
	private float m_speed; // 散らばる時の速さ
	private bool m_isFollow; // プレイヤーを追尾するモードに入った場合 true
	private float m_followSpeed; // プレイヤーを追尾する速さ
	
	// Update is called once per frame
	void Update () {
		// プレイヤーの現在位置を取得
		var playerPos = Player.m_instance.transform.localPosition;

		// プレイヤーと宝石の距離を計算
		var distance = Vector3.Distance(playerPos, transform.localPosition);

		// プレイヤーと宝石の距離が近づいた場合
		if(distance < Player.m_instance.m_magnetDistance) {
			// プレイヤーを追尾するモード
			m_isFollow = true;
		}

		// プレイヤーを追尾するモードかつプレイヤーがまだ死亡していない場合
		if(m_isFollow && Player.m_instance.gameObject.activeSelf) {
			// プレイヤーの現在位置へ向かうベクトルを作成
			var direction = playerPos - transform.localPosition;
			direction.Normalize();

			// 宝石をプレイヤーが存在する方向に移動
			transform.localPosition += direction * m_followSpeed;

			// 加速しながら近づく
			m_followSpeed += m_followAccel;
			return;
		}

		// 散らばる速度を計算
		var velocity = m_direction * m_speed;

		// 散らばる
		transform.localPosition += velocity;

		// だんだん減速
		m_speed *= m_breke;

		// 宝石が画面外に出ないように制限
		transform.localPosition = Utils.ClampPosition(transform.localPosition);
	}

	// 宝石が出現する時に初期化する関数
	public void Init(int score, float speedMin, float speedMax) {
		// 宝石がどの方向に散らばるかランダム
		var angle = Random.Range(0, 360);

		// 進行方向をラジアン値に
		var f = angle * Mathf.Deg2Rad;

		// 進行方向のベクトル作成
		m_direction = new Vector3(Mathf.Cos(f), Mathf.Sin(f), 0);

		// 宝石の散らばる速さをランダムに決定
		m_speed = Mathf.Lerp(speedMin, speedMax, Random.value);

		// 8秒後に宝石を削除
		Destroy(gameObject, 8);
	}

	// 他のオブジェクトと衝突した時に呼び出される関数
	private void OnTriggerEnter2D(Collider2D collision) {
		// 衝突したオブジェクトがプレイヤーではない場合は無視
		if(!collision.name.Contains("Player")) return;

		// 宝石を削除
		Destroy(gameObject);

		// プレイヤーの経験値を増やす
		var player = collision.GetComponent<Player>();
		player.AddExp(m_exp);

		// 宝石を取得した時のSEを再生
		var audioSource = FindObjectOfType<AudioSource>();
		audioSource.PlayOneShot(m_goldClip);
	}
}
