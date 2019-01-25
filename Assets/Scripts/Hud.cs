using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 情報表示用のUI制御コンポーネント
public class Hud : MonoBehaviour {

	public Image m_hpGauge; // HPゲージ
	public Image m_expGauge; // 経験値ゲージ
	public Text m_levelText; // レベルのテキスト
	public GameObject m_gameOverText; // ゲームオーバーのテキスト
	
	// Update is called once per frame
	void Update () {
		// プレイヤー取得
		var player = Player.m_instance;

		// HPのゲージの表示を更新
		var hp = player.m_hp;
		var hpMax = player.m_hpMax;
		m_hpGauge.fillAmount = (float)hp/hpMax;

		// 経験値のゲージの表示を更新
		var exp = player.m_exp;
		var prevNeedExp = player.m_prevNeedExp;
		var needExp = player.m_needExp;
		m_expGauge.fillAmount = (float)(exp - prevNeedExp) / (needExp - prevNeedExp);

		// レベルのテキストの表示を更新
		m_levelText.text = player.m_level.ToString();

		// プレイヤーが非表示ならゲームオーバーと表示
		m_gameOverText.SetActive(!player.gameObject.activeSelf);
	}
}
