---
layout: page
title: UnityEngine.Random で乱数を生成する
permalink: /unity/random-basics/
---

# UnityEngine.Random で乱数を生成する

ゲームにおけるランダム性は、アイテムのドロップ率・敵の出現位置・AI の行動選択など、多くの場面で活用されます。Unity では **`UnityEngine.Random`** クラスを使って手軽に乱数を生成できます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `Random.Range()` で範囲内のランダムな値を生成できる
- float 版と int 版の挙動の違いを説明できる
- `Random.value`・`Random.insideUnitSphere` などの便利プロパティを使える
- シード値と擬似乱数の関係を説明できる
- `UnityEngine.Random` と `System.Random` を混同せずに使い分けられる

## 前提知識

- [チュートリアル: アイテム収集](/unity-csharp-learning/unity/item-collection/) を読んでいること

---

## 1. Random.Range() — 範囲内のランダムな値

**`Random.Range()`** — 指定した範囲内のランダムな値を返します。<!-- [公式ドキュメント]() -->

float 版と int 版で最大値の扱いが異なるため注意が必要です。

### float 版（最大値を含む）

**書式：Random.Range メソッド（float）**
```csharp
public static float Range(float minInclusive, float maxInclusive);
```

| パラメータ | 説明 |
|---|---|
| `minInclusive` | 最小値（この値を含む） |
| `maxInclusive` | 最大値（**この値を含む**） |

```csharp
var x = Random.Range(-5f, 5f);  // -5.0 〜 5.0 の float
```

### int 版（最大値を含まない）

**書式：Random.Range メソッド（int）**
```csharp
public static int Range(int minInclusive, int maxExclusive);
```

| パラメータ | 説明 |
|---|---|
| `minInclusive` | 最小値（この値を含む） |
| `maxExclusive` | 最大値（**この値を含まない**） |

```csharp
var n = Random.Range(0, 6);  // 0 〜 5 の int（6 は出ない）
```

> ⚠️ **int 版は最大値を含みません。** `Random.Range(0, 6)` が返す値は `0`・`1`・`2`・`3`・`4`・`5` のいずれかです。`6` は返りません。配列のインデックス選択に使うと自然に境界内に収まります。

---

## 2. ランダムな座標にオブジェクトを出現させる

`Random.Range()` と `transform.position` を組み合わせると、フィールド内のランダムな位置にオブジェクトを配置できます。

```csharp
// RandomSpawner.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class RandomSpawner : MonoBehaviour
{
    [SerializeField]
    private float _range = 5f;

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            var x = Random.Range(-_range, _range);
            var z = Random.Range(-_range, _range);

            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.position = new Vector3(x, 0.5f, z);
        }
    }
}
```

クリックするたびに X・Z をランダムに決めた座標に立方体が生成されます。`_range` を Inspector から変更することで出現範囲を調整できます。

---

## 3. その他のランダムプロパティ

### Random.value

**`Random.value`** — 0.0 以上 1.0 以下のランダムな float を返します。確率の判定によく使います。<!-- [公式ドキュメント]() -->

**書式：value プロパティ**
```csharp
public static float value { get; }
```

```csharp
// 30% の確率で true
if (Random.value < 0.3f)
{
    Debug.Log("レアアイテムが出現！");
}
```

### Random.insideUnitSphere

**`Random.insideUnitSphere`** — 半径 1 の球の内部にあるランダムな点（`Vector3`）を返します。3D 空間へのばらつきある配置に便利です。<!-- [公式ドキュメント]() -->

**書式：insideUnitSphere プロパティ**
```csharp
public static Vector3 insideUnitSphere { get; }
```

```csharp
// 半径 5 の球内ランダム座標に生成
var pos = Random.insideUnitSphere * 5f;
Instantiate(_original, pos, Quaternion.identity);
```

### Random.insideUnitCircle

**`Random.insideUnitCircle`** — 半径 1 の円の内部にあるランダムな点（`Vector2`）を返します。2D 平面への散布に使えます。<!-- [公式ドキュメント]() -->

**書式：insideUnitCircle プロパティ**
```csharp
public static Vector2 insideUnitCircle { get; }
```

```csharp
// XZ 平面上の半径 5 の円内ランダム座標
var circle = Random.insideUnitCircle * 5f;
var pos = new Vector3(circle.x, 0f, circle.y);
```

---

## 4. シード値と擬似乱数

コンピューターの乱数は**擬似乱数**（Pseudo-random）です。数学的な計算式によって生成されるため、厳密には「本当のランダム」ではありません。**シード値**（種）と呼ばれる初期値から計算を始め、毎回決まった順序で値を生成します。

シード値が同じなら、何度実行しても**まったく同じ順序**の乱数列が得られます。

**`Random.InitState()`** — 乱数のシード値を設定します。<!-- [公式ドキュメント]() -->

**書式：InitState メソッド**
```csharp
public static void InitState(int seed);
```

| パラメータ | 説明 |
|---|---|
| `seed` | 乱数列の初期値。同じ seed なら同じ乱数列が得られる |

```csharp
Random.InitState(42);
Debug.Log(Random.Range(0, 100));  // 実行するたびに同じ値
Debug.Log(Random.Range(0, 100));  // 2回目も毎回同じ値
```

通常のゲームプレイでは Unity が起動時に自動でシードを設定するため、意識する必要はありません。シードを固定する主な用途は以下のとおりです。

| 用途 | 説明 |
|---|---|
| プロシージャル生成 | ダンジョンや地形を同じシードで再現する |
| デバッグ | 再現性のある乱数でバグの再現をしやすくする |
| マルチプレイヤー同期 | 全クライアントで同じ乱数列を使う |

---

## 5. 補足: System.Random との混同に注意

C# には `UnityEngine.Random` とは別に、**`System.Random`** クラスが標準ライブラリとして存在します。

```csharp
// using System; があると Random が System.Random を指してしまう
using System;

var r = new Random();       // System.Random のインスタンス
r.Next(0, 10);              // System.Random のメソッド
```

`System.Random` はインスタンス化が必要で、スレッドセーフではなく、`Vector3` や `insideUnitSphere` のような Unity 向けプロパティもありません。Unity のゲームロジックでは原則 **`UnityEngine.Random`** を使います。

両方を使う必要がある場合は、完全修飾名で区別します。

```csharp
using UnityEngine;

// 明示的に名前空間を指定して使い分ける
var unityRandom = UnityEngine.Random.Range(0, 10);
var sysRandom   = new System.Random();
```

> 💡 **ポイント**: `using System;` を書いている場合、`Random` と書くだけでは `System.Random` と解釈されコンパイルエラーになります。`UnityEngine.Random.Range(...)` と完全修飾名で書くか、`using System;` を削除しましょう。

---

## まとめ

- `Random.Range(float, float)` は最大値を含む。`Random.Range(int, int)` は最大値を含まない
- `Random.value` は 0〜1 の float。確率判定（`< 0.3f` で 30%）に使える
- `Random.insideUnitSphere` / `insideUnitCircle` で球・円内のランダム座標を取得できる
- 乱数は擬似乱数。`Random.InitState(seed)` でシードを固定すると同じ乱数列を再現できる
- Unity では `UnityEngine.Random` を使う。`System.Random` と混同しないよう注意

---

## 理解度チェック

1. `Random.Range(0, 10)` と `Random.Range(0f, 10f)` の返す値の範囲はそれぞれどう違いますか？
2. アイテムが 20% の確率でドロップする判定を `Random.value` で書いてください。
3. シード値を固定すると乱数列はどうなりますか？

<details markdown="1">
<summary>解答を見る</summary>

1. `int` 版は 0〜9（10 を含まない）。`float` 版は 0.0〜10.0（10.0 を含む）。
2. `if (Random.value < 0.2f) { /* ドロップ処理 */ }`
3. 同じシード値なら何度実行しても必ず同じ順序の乱数列が得られる。

</details>

---

## 次のステップ

[Instantiate() でオブジェクトを生成する](/unity-csharp-learning/unity/instantiate/) では、プレハブをスクリプトから複製してシーンに動的に生成する方法を学びます。
