---
layout: page
title: Instantiate() でオブジェクトを生成する
permalink: /unity/instantiate/
---

# Instantiate() でオブジェクトを生成する

プレハブをスクリプトで複製してシーンに生成するには **`Instantiate()`** を使います。ゲーム中に敵・アイテム・エフェクトを動的に出現させる際の基本メソッドです。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `Instantiate()` でプレハブからオブジェクトを複製できる
- 生成位置・向きを指定して任意の場所にスポーンできる
- 戻り値を受け取り、生成直後のオブジェクトを操作できる
- マウスクリックをトリガーにオブジェクトを生成できる

## 前提知識

- [プレハブ（Prefab）](/unity-csharp-learning/unity/prefab-basics/) を読んでいること

---

## 1. Instantiate() とは

**`Object.Instantiate()`** — オブジェクト（プレハブを含む）を複製してシーンに追加します。<!-- [公式ドキュメント]() -->

**書式：Instantiate メソッド（基本）**
```csharp
public static Object Instantiate(Object original);
```

| パラメータ | 説明 |
|---|---|
| `original` | 複製元のオブジェクト（プレハブまたはシーン上のオブジェクト） |

`MonoBehaviour` を継承したクラスの中ではクラス名を付けずに `Instantiate(...)` と呼び出せます。`SerializeField` で複製元のプレハブをフィールドに持ち、`Instantiate()` に渡すのが基本パターンです。

```csharp
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _original;

    private void Update()
    {
        // 何らかのタイミングで複製する
        Instantiate(_original);
    }
}
```

この基本形では、複製されたオブジェクトはプレハブと同じ位置・向きに生成されます（プレハブの Transform が原点設定なら原点に現れます）。

> ⚠️ `_original` が `null` のままだと実行時エラーになります。Inspector ビューから必ずプレハブを設定してください。

---

## 2. 生成位置と向きを指定する

**書式：Instantiate メソッド（位置・向き指定）**
```csharp
public static Object Instantiate(Object original, Vector3 position, Quaternion rotation);
```

| パラメータ | 説明 |
|---|---|
| `original` | 複製元のオブジェクト |
| `position` | 生成先のワールド座標 |
| `rotation` | 生成時の向き（`Quaternion`） |

**`Quaternion.identity`** は「回転なし（標準の向き）」を意味します。向きを変えずに位置だけ指定したい場合に使います。

```csharp
// このスクリプトを持つオブジェクトの位置に生成する
Instantiate(_original, transform.position, Quaternion.identity);
```

スポナー（出現地点）を複数配置した場合、それぞれが持つ `transform.position` を渡せば、各スポナーの位置にオブジェクトを生成できます。

---

## 3. 戻り値で生成直後のオブジェクトを操作する

`Instantiate()` は複製したオブジェクトを戻り値で返します。受け取ることで、生成直後にコンポーネントを取得したりプロパティを変更できます。

```csharp
var item = Instantiate(_original, transform.position, Quaternion.identity);
var rb = item.GetComponent<Rigidbody>();
rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
```

`_original` が `GameObject` 型のフィールドであれば、戻り値も自動的に `GameObject` として扱えます。

---

## 4. 実践例: クリックでオブジェクトを生成する

ここまでの内容をまとめた動作確認用のサンプルです。マウスの左クリックを検知して、スクリプトを持つオブジェクトの位置にプレハブを複製し、生成直後に上方向へ力を加えます。

```csharp
// ClickSpawner.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _original;

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            var obj = Instantiate(_original, transform.position, Quaternion.identity);
            var rb = obj.GetComponent<Rigidbody>();
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }
    }
}
```

`Mouse.current.leftButton.wasPressedThisFrame` は、そのフレームで左クリックが押されたときだけ `true` になります（[Input System](/unity-csharp-learning/unity/input-system/) で解説済み）。クリックするたびにオブジェクトが1個生成され、ポンと上に飛び上がります。

### 動作確認の手順

1. 新しいシーンに **GameObject → 3D Object → Sphere** を追加し、名前を `Item` にする
2. Inspector で **Add Component → Physics → Rigidbody** を追加する
3. Hierarchy の `Item` を Project ビューへドラッグ & ドロップしてプレハブ化し、シーン上の `Item` は削除する
4. **GameObject → Create Empty** を追加し、`ClickSpawner` スクリプトを作成してアタッチする
5. Inspector の **Original** 欄に `Item` プレハブを設定する
6. Play ボタンを押してクリックするたびに Sphere が生成されることを確認する

---

## まとめ

- `Instantiate(original)` でプレハブからオブジェクトを複製できる
- `Instantiate(original, position, rotation)` で生成位置と向きを指定できる
- `Quaternion.identity` は「回転なし」を表す定数
- 戻り値を受け取ることで、生成直後のオブジェクトを操作できる
- `Mouse.current.leftButton.wasPressedThisFrame` でクリックをトリガーにして生成できる

---

## 理解度チェック

1. `Instantiate(_original)` の基本形でオブジェクトを生成したとき、どこに生成されますか？
2. スポナーオブジェクトの位置にアイテムを生成する1行を書いてください。
3. `Quaternion.identity` は何を意味しますか？

<details markdown="1">
<summary>解答を見る</summary>

1. プレハブ（`_original`）の Transform 設定と同じ位置・向きに生成される。プレハブが原点設定なら原点付近に生成される。
2. `Instantiate(_original, transform.position, Quaternion.identity);`
3. 回転なし（標準の向き）を意味する `Quaternion` の定数。

</details>

---

## 次のステップ

[GameObject の親子関係](/unity-csharp-learning/unity/gameobject-hierarchy/) では、Hierarchy の木構造・ローカル座標系・スクリプトでの親子操作を学び、`Instantiate()` と組み合わせてオブジェクトの生成・管理を整理する方法を学びます。
