---
layout: page
title: Collider とトリガー判定
permalink: /unity/collider-trigger/
---

# Collider とトリガー判定

Unity の **Collider（コライダー）** コンポーネントはオブジェクトの衝突範囲を定義します。このページでは、衝突ではなく「交差（すり抜けながら触れる）」を検知する**トリガー**の使い方と、交差を検知したときにオブジェクトを削除する `Destroy` を学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- Collider と Is Trigger の役割を説明できる
- `OnTriggerEnter` でオブジェクト同士の交差を検知できる
- `other.gameObject` で相手のオブジェクトを取得できる
- `Destroy()` でシーンからオブジェクトを削除できる

## 前提知識

- [Rigidbody で力を加える](/unity-csharp-learning/unity/rigidbody-force/) を読んでいること

---

## 1. Collider とは

**Collider** コンポーネントはゲームオブジェクトの**当たり判定の形状**を定義します。`GameObject.CreatePrimitive()` や Unity エディターで 3D オブジェクトを作成すると、形状に合った Collider（Sphere Collider、Box Collider など）が自動的に追加されます。

Rigidbody を持つオブジェクトが Collider を持つ別のオブジェクトに触れると、物理エンジンによって**衝突（押し合い・反発）**が発生します。

---

## 2. Is Trigger — 衝突せずに交差を検知する

アイテム回収のように「触れたことを検知したいが、ぶつかって弾き飛ばされたくない」場合は、Collider の **Is Trigger** をオンにします。

![Inspector で Box Collider の Is Trigger にチェックを入れる](./image.png)

| Is Trigger | 動作 |
|---|---|
| オフ（既定） | Rigidbody と物理的に衝突する（反発・押し合い） |
| オン | 交差しても弾かれない。交差をスクリプトで検知できる |

> 💡 **補足**: Is Trigger をオンにしても Collider は消えません。「見えない交差検知ゾーン」として機能します。

---

## 3. OnTriggerEnter — 交差を検知するイベントメソッド

Rigidbody を持つオブジェクトが、Is Trigger がオンの Collider に侵入したとき、**`OnTriggerEnter()`** が呼ばれます。

**`MonoBehaviour.OnTriggerEnter()`** — Is Trigger の Collider に別のオブジェクトが侵入したとき呼ばれます。<!-- [公式ドキュメント]() -->

**書式：OnTriggerEnter メソッド**
```csharp
private void OnTriggerEnter(Collider other);
```

| パラメータ | 説明 |
|---|---|
| `other` | 交差した相手の `Collider` コンポーネント |

`other` から相手の `Collider` にアクセスできます。さらに `other.gameObject` で相手の **GameObject** 自体を取得できます。

```csharp
private void OnTriggerEnter(Collider other)
{
    Debug.Log($"触れた: {other.gameObject.name}");
}
```

---

## 4. Destroy() — オブジェクトをシーンから削除する

**`Object.Destroy()`** — ゲームオブジェクトやコンポーネントをシーンから削除します。<!-- [公式ドキュメント]() -->

**書式：Object.Destroy メソッド**
```csharp
public static void Destroy(Object obj);
```

| パラメータ | 説明 |
|---|---|
| `obj` | 削除する Object（GameObject やコンポーネント） |

`OnTriggerEnter` の中で `Destroy(other.gameObject)` を呼ぶと、触れた相手を削除できます。

```csharp
private void OnTriggerEnter(Collider other)
{
    Destroy(other.gameObject);  // 交差した相手を削除
}
```

---

## 5. まとめサンプル

以下は「Player に触れると消えるアイテム」の最小実装です。

**シーンの準備**
1. Sphere を追加し、名前を `Player`、Rigidbody を追加する
2. Cube を追加し、名前を `Item`、Inspector で Box Collider の **Is Trigger** をオンにする
3. Player に `Player.cs` スクリプトをアタッチする

```csharp
// Player.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        var move = Vector3.zero;
        if (Keyboard.current.rightArrowKey.isPressed) move.x += 1f;
        if (Keyboard.current.leftArrowKey.isPressed)  move.x -= 1f;
        if (Keyboard.current.upArrowKey.isPressed)    move.z += 1f;
        if (Keyboard.current.downArrowKey.isPressed)  move.z -= 1f;
        _rigidbody.AddForce(move);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
    }
}
```

Player を Item に重ねると、Item が消えます。

> ⚠️ **注意**: この実装では、Is Trigger がオンの**すべてのオブジェクト**に触れると削除してしまいます。実際のゲームでは、相手が本当にアイテムかどうかを確認（識別）する処理が必要です。これについては次のチュートリアルで扱います。

---

## まとめ

- Collider はオブジェクトの当たり判定形状。3D Object を作成すると自動付与される
- **Is Trigger** をオンにすると物理衝突しなくなり、交差をスクリプトで検知できる
- `OnTriggerEnter(Collider other)` は Rigidbody が Is Trigger の Collider に入ったとき呼ばれる
- `other.gameObject` で交差した相手の GameObject を取得できる
- `Destroy(gameObject)` でシーンからオブジェクトを削除できる

---

## 理解度チェック

1. Collider の Is Trigger をオンにすると、衝突の動作はどう変わりますか？
2. `OnTriggerEnter` が呼ばれるためには、どちらのオブジェクトが Rigidbody を持っている必要がありますか？
3. `Destroy(gameObject)` と `Destroy(other.gameObject)` の違いは何ですか？

<details markdown="1">
<summary>解答を見る</summary>

1. 物理的な衝突（反発）が起きなくなり、オブジェクト同士がすり抜ける。その代わり、交差を `OnTriggerEnter` で検知できるようになる。
2. どちらか一方（または両方）が Rigidbody を持っていれば `OnTriggerEnter` が呼ばれる。
3. `Destroy(gameObject)` は**自分自身**を削除し、`Destroy(other.gameObject)` は**触れた相手**を削除する。

</details>

---

## 次のステップ

[プレハブ（Prefab）](/unity-csharp-learning/unity/prefab-basics/) では、同じ設定のゲームオブジェクトを大量に配置するための「ひな形」の仕組みを学びます。
