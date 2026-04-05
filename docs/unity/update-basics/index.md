---
layout: page
title: Update メソッドと連続実行
permalink: /unity/update-basics/
---

# Update メソッドと連続実行

`Start` メソッドがゲーム開始時に1回だけ実行されるのに対し、`Update` メソッドは**毎フレーム繰り返し呼び出され続けます**。このページでは Update の仕組みを体験し、オブジェクトをスクリプトで動かす基本を学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- Update メソッドが毎フレーム実行されることを確認できる
- `transform.Translate` でオブジェクトを継続的に移動できる
- フレームレートに依存しない移動を `Time.deltaTime` で実現できる

## 前提知識

- [チュートリアル: ドミノ倒し](/unity-csharp-learning/unity/domino/) を読んでいること

---

## 1. Update メソッドとは

`Update` は `Start` と並ぶ MonoBehaviour の基本メソッドです。

| メソッド | 実行タイミング | 主な用途 |
|---|---|---|
| `Start` | ゲーム開始時に **1回** | 初期配置・初期化 |
| `Update` | **毎フレーム** 繰り返し | 入力処理・継続的な移動 |

1秒間に画面が更新される回数を**フレームレート**と呼びます。60fps（フレーム毎秒）の環境では、`Update` は1秒間に60回呼び出されます。

---

## 2. Debug.Log で連続実行を体験する

まず `Debug.Log` を使って、`Update` が繰り返し呼ばれていることを確認します。

```csharp
using UnityEngine;

public class UpdateSample : MonoBehaviour
{
    private void Update()
    {
        Debug.Log("Update が呼ばれました");
    }
}
```

このスクリプトをゲームオブジェクトにアタッチして実行すると、Console ビューにメッセージが連続して出力され続けます。`Start` に同じコードを書いた場合は1回だけ出力されることと比べてみましょう。

---

## 3. transform.Translate でオブジェクトを動かす

`Update` 内で座標を変化させると、オブジェクトが継続的に動き続けます。

**`Transform.Translate`** — Transform を指定した方向・距離だけ移動します。<!-- [公式ドキュメント]() -->

**書式：Transform.Translate メソッド**
```csharp
public void Translate(Vector3 translation);
```

| パラメータ | 型 | 説明 |
|---|---|---|
| `translation` | `Vector3` | 移動する方向と距離 |

---

`Vector3` にはよく使う方向があらかじめ定数として用意されています。たとえば `Vector3.right` は `new Vector3(1, 0, 0)`（X 方向に 1）と同じ値です。

**書式：Vector3 方向定数**
```
Vector3.right    // new Vector3( 1,  0,  0) と同じ（右）
Vector3.left     // new Vector3(-1,  0,  0) と同じ（左）
Vector3.up       // new Vector3( 0,  1,  0) と同じ（上）
Vector3.down     // new Vector3( 0, -1,  0) と同じ（下）
Vector3.forward  // new Vector3( 0,  0,  1) と同じ（奥）
Vector3.back     // new Vector3( 0,  0, -1) と同じ（手前）
```

---

```csharp
using UnityEngine;

public class UpdateSample : MonoBehaviour
{
    private void Update()
    {
        transform.Translate(Vector3.right * 0.1f);
    }
}
```

これを実行すると、オブジェクトが右方向に動き続けます。

---

## 4. フレームレート依存の問題

上のコードには問題があります。`0.1f` は**1フレームあたりの移動量**です。フレームレートが異なると、同じコードでも速度が変わってしまいます。

| フレームレート | 1秒あたりの Update 呼び出し回数 | 1秒あたりの移動量 |
|---|---|---|
| 30 fps | 30回 | 3.0 |
| 60 fps | 60回 | 6.0 |
| 120 fps | 120回 | 12.0 |

端末の性能や負荷によってフレームレートは変動するため、**環境によって速度が変わる**という不安定な挙動になります。

---

## 5. Time.deltaTime でフレームレートに依存しない移動

この問題を解決するのが `Time.deltaTime` です。

**`Time.deltaTime`** — 前のフレームからの経過時間（秒）を返します。<!-- [公式ドキュメント]() -->

**書式：Time.deltaTime プロパティ**
```csharp
public static float deltaTime { get; }
```

60fps では約 `0.0167`、30fps では約 `0.0333` の値になります。つまりフレームレートが高いほど小さい値です。移動量に `Time.deltaTime` を掛けることで、**フレームレートが変わっても1秒あたりの移動量が一定**になります。

```csharp
using UnityEngine;

public class UpdateSample : MonoBehaviour
{
    private void Update()
    {
        // 1秒あたり 3.0 進む（フレームレートによらず一定）
        transform.Translate(Vector3.right * 3.0f * Time.deltaTime);
    }
}
```

> 💡 **ポイント**: `Time.deltaTime` を掛けた場合のもう一方の数値（ここでは `3.0f`）は「**1秒あたりの移動量（速度）**」を意味します。`Time` クラスには他にも時間管理に役立つプロパティがあります。詳細は Time クラスのページ（準備中）で扱います。

---

## まとめ

- `Update` はゲームが実行されている間、**毎フレーム繰り返し呼ばれる**
- `transform.Translate` でオブジェクトを継続的に移動できる
- `Vector3.right` などの定数で方向を簡潔に表せる
- **`Time.deltaTime`** を掛けることでフレームレートに依存しない移動になる

---

## 理解度チェック

以下の問いに答えられるか確認しましょう。

1. `Start` と `Update` の実行タイミングの違いを説明してください。
2. `transform.Translate(Vector3.up * 2.0f * Time.deltaTime)` を実行すると、オブジェクトはどのように動きますか？
3. `Time.deltaTime` を掛けないとどのような問題が起きますか？

<details markdown="1">
<summary>解答を見る</summary>

1. `Start` はゲーム開始時に **1回だけ**、`Update` は **毎フレーム繰り返し** 実行される。
2. 上方向（Y+）に **1秒あたり 2.0** の速度で動き続ける。
3. フレームレートが高い端末ほど速く動き、低い端末ほど遅く動く。環境によって速度が変わる不安定な動作になる。

</details>

---

## 次のステップ

[Input System で入力操作](/unity-csharp-learning/unity/input-system/) では、キーボード入力を受け取ってオブジェクトを操作する方法を学びます。
