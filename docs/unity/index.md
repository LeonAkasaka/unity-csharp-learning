---
layout: page
title: Unity 基礎
permalink: /unity/
---

# Unity 基礎

Unity エディターの使い方と、C# スクリプトを使ったゲームオブジェクトの操作を学びます。

## このセクションの内容

| # | トピック | 概要 |
|---|---|---|
| 1 | Unity エディター入門（準備中） | 画面構成・基本操作 |
| 2 | [Start メソッドとスクリプト](./start-method/) | MonoBehaviour と Start メソッド |
| 2.1 | [Debug.Log でスクリプトの実行を確認する](./debug-log/) | Debug.Log・Console ビュー |
| 3 | [GameObject の生成と操作](./gameobject-basics/) | CreatePrimitive・変数・name プロパティ |
| 4 | [Transform でオブジェクトを操作する](./transform/) | localScale・position・rotation |
| 5 | [AddComponent と物理演算](./rigidbody/) | Rigidbody と物理エンジン |
| 6 | [チュートリアル: ドミノ倒し](./domino/) | 総合演習・for 文・課題 |
| 7 | [Update メソッドと連続実行](./update-basics/) | Update・Translate・Time.deltaTime |
| 8 | [Input System で入力操作](./input-system/) | Keyboard.current・isPressed |
| 8.1 | [補足: 旧来の Input クラスと InputManager](./legacy-input/) | Input クラス・InputManager |
| 9 | [フィールドでデータを維持する](./fields-basics/) | フィールド・SerializeField |
| 10 | [Time クラスと時間制御](./time-basics/) | Time.time・Time.timeScale・duration |
| 10.1 | [補足: 現実時間の取得（DateTime と DateTimeOffset）](./time-datetime/) | DateTime・DateTimeOffset |
| 11 | [チュートリアル: 歩行者信号機](./traffic-light/) | ステートマシン・GetComponent・material.color |
| 12 | [Rigidbody で力を加える](./rigidbody-force/) | GetComponent キャッシュ・AddForce |
| 13 | [Collider とトリガー判定](./collider-trigger/) | Is Trigger・OnTriggerEnter・Destroy |
| 14 | [プレハブ（Prefab）](./prefab-basics/) | Prefab 作成・インスタンス・変更の伝播 |
| 15 | [チュートリアル: アイテム収集](./item-collection/) | AddForce・OnTriggerEnter・Prefab・CompareTag |

## 前提知識

- C# の基本的な構文はこのセクションで都度解説しますが、[C# 基礎](/unity-csharp-learning/csharp/) のセクションと並行して学習することを推奨します

## 学習目標

- Unity エディターを使いこなせるようになる
- C# スクリプトで GameObject を操作できるようになる
- 簡単なゲームの仕組みを実装できるようになる
