using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 爆発エフェクト制御コンポーネント
public class Explosion : MonoBehaviour {

	// 爆発エフェクトが生成された時に呼び出される関数
	void Start () {
		// 演出が完了したら削除
		var particleSystem = GetComponent<ParticleSystem>();
		Destroy(gameObject, particleSystem.main.duration);
	}
}
