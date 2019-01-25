using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// プレイヤーを制御するコンポーネント
public class Player : MonoBehaviour {

	public float m_speed; // 移動の速さ
	public Shot m_shotPrefab; // 弾のプレハブ
	public float m_shotSpeed; //弾の移動の速さ
	public float m_shotAngleRange; // 複数の弾を発射する時の角度
	public float m_shotTimer; // 弾の発射タイミングを管理するタイマー
	public int m_shotCount; // 弾の発射数
	public float m_shotInterval; // 弾の発射レート
	public int m_hpMax; // HP最大値
	public int m_hp; // HP
	public float m_magnetDistance; // 宝石を引きつける距離

	// プレイヤーのインスタンスを管理するstatic変数
	public static Player m_instance;
	public int m_nextExpBase; // 次のレベルまでに必要な経験値の基本値
	public int m_nextExpInterval; // 次のレベルまでに必要な経験値の増加値
	public int m_level; // レベル
	public int m_exp; // 経験値
	public int m_prevNeedExp; // 前のレベルに必要だった経験値
	public int m_needExp; // 次のレベルに必要な経験値
	public AudioClip m_levelUpClip; // レベルアップ時に再生するSE
	public AudioClip m_damageClip; // ダメージを受けた時に再生するSE
	public int m_levelMax; // レベルの最大値
	public int m_shotCountFrom; // 弾の発射数(レベル最小時)
	public int m_shotCountTo; // 弾の発射数(レベル最大時)
	public float m_shotIntervalFrom; // 弾の発射レート(最小)
	public float m_shotIntervalTo; // 弾の発射レート最大
	public float m_magnetDistanceFrom; // 宝石を引きつける距離(最小)
	public float m_magnetDistanceTo; // 宝石を引きつける距離(最大)

	// 毎フレーム呼び出し
	void Update () {
		var h = Input.GetAxis("Horizontal");
		var v = Input.GetAxis("Vertical");

		// 矢印キーが押されている方向にプレイヤーを移動
		var velocity = new Vector3(h, v) * m_speed;
		transform.localPosition += velocity;
		transform.localPosition = Utils.ClampPosition(transform.localPosition);

		// プレイヤーのスクリーン上の座標を計算
		var screenPos = Camera.main.WorldToScreenPoint(transform.position);

		// プレイヤーから見たマウスカーソルの方向を計算
		var direction = Input.mousePosition - screenPos;

		// マウスカーソルが存在する方向の角度を取得
		var angle = Utils.GetAngle(Vector3.zero, direction);

		// プレイヤーがマウスカーソルの方向を見るように
		var angles = transform.localEulerAngles;
		angles.z = angle - 90;
		transform.localEulerAngles = angles;

		// 弾の発射タイミングを管理するタイマーを更新
		m_shotTimer += Time.deltaTime;

		// まだ弾の発射タイミングではない場合は、ここで処理を終える
		if(m_shotTimer < m_shotInterval) return;

		// 弾の発射タイミングを管理するタイマーをリセット
		m_shotTimer = 0;

		// 弾を発射する
		ShootNWay(angle, m_shotAngleRange, m_shotSpeed, m_shotCount);
	}

	// ゲーム開始時に呼び出し
	private void Awake() {
		// 他のクラスからプレイヤーを参照できるように、static変数にインスタンス情報を格納
		m_instance = this;

		m_hp = m_hpMax;

		m_level = 1;
		m_needExp = GetNeedExp(1);

		m_shotCount = m_shotCountFrom; // 弾の発射数
		m_shotInterval = m_shotIntervalFrom; // 弾の発射レート
		m_magnetDistance = m_magnetDistanceFrom; // 宝石を引きつける距離
	}

	private void ShootNWay(float angleBase, float angleRange, float speed, int count) {
		var pos = transform.localPosition; // プレイヤーの位置
		var rot = transform.localRotation; // プレイヤーの向き

		// 弾を複数発射する場合
		if(1 < count) {
			// 発射回数分ループ処理　
			for(int i = 0 ; i < count ; ++i) {
				// 弾の発射角度を計算
				var angle = angleBase + angleRange * ((float)i / (count - 1) - 0.5f);

				// 発射する弾を生成
				var shot = Instantiate(m_shotPrefab, pos, rot);

				// 弾を発射する方向と速さを設定
				shot.Init(angle, speed);
			}
		// 弾をひとつだけ発射
		}else if(count == 1){
			// 発射する弾を生成
			var shot = Instantiate(m_shotPrefab, pos, rot);

			// 弾を発射する方向と速さ
			shot.Init(angleBase, speed);
		}
	}

	// ダメージを受ける関数 敵とぶつかった時に呼び出し
	public void Damage(int damage) {
		
		// ダメージを受けた時のSEを再生する
		var audioSource = FindObjectOfType<AudioSource>();
		audioSource.PlayOneShot(m_damageClip);

		// HPをへらす
		m_hp -= damage;

		// HPがまだある場合ここで処理終え
		if(0 < m_hp) return;

		// プレイヤーが死亡したので非表示に ゲームオーバーの演出とか
		gameObject.SetActive(false);
	}

	// 経験値を増やす関数 宝石を取得した時に呼び出し
	public void AddExp(int exp) {
		// 経験値を増やす
		m_exp +=  exp;

		// まだレベルアップに必要な経験値に足りていないばあいここでおわり
		if(m_exp < m_needExp) return;

		// レベルアップする
		m_level++;

		// 今回のレベルアップに必要だった経験値を記憶 ゲージの表示に使用するため
		m_needExp = GetNeedExp(m_level);

		// レベルアップ字にボム発動 
		var angleBase = 0;
		var angleRange = 360;
		var count = 28;
		ShootNWay(angleBase, angleRange, 0.15f, count);
		ShootNWay(angleBase, angleRange, 0.2f, count);		
		ShootNWay(angleBase, angleRange, 0.25f, count);

		// レベルアップした時のSEを再生する
		var audioSource = FindObjectOfType<AudioSource>();
		audioSource.PlayOneShot(m_levelUpClip);

		// レベルアップしたので、各種パラメータを更新する
		var t = (float)(m_level - 1) / (m_levelMax - 1);
		m_shotCount = Mathf.RoundToInt(Mathf.Lerp(m_shotCountFrom, m_shotCountTo, t)); // 弾の発射数
		m_shotInterval = Mathf.Lerp(m_shotIntervalFrom, m_shotIntervalTo, t); // 弾の発射レート
		m_magnetDistance = Mathf.Lerp(m_magnetDistanceFrom, m_magnetDistanceTo, t); // 宝石を引きつける距離
	}

	// 指定されたレベルに必要な経験値を計算する関数
	private int GetNeedExp(int level) {
		return m_nextExpBase + m_nextExpInterval * ((level - 1) * (level - 1));
	}
}
