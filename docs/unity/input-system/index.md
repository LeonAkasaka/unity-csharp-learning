---
layout: page
title: Input System で入力操作
permalink: /unity/input-system/
---

# Input System で入力操作

プレイヤーのキーボード入力を受け取るには、Unity の **Input System** を使います。このページでは、キー入力の判定方法と、`Update` メソッド内で入力を処理する理由を学びます。

> **前提**: このページでは Input System パッケージがプロジェクトに導入・設定済みであることを前提とします。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `Keyboard.current` でキーボードの状態を取得できる
- `isPressed` と `wasPressedThisFrame` の使い分けができる
- Update 内で入力を処理する理由を説明できる
- 入力に応じてオブジェクトを動かせる

## 前提知識

- [Update メソッドと連続実行](/unity-csharp-learning/unity/update-basics/) を読んでいること

---

## 1. Input System とは

Unity には入力を扱うパッケージとして **Input System** があります。キーボード・マウス・ゲームパッドなど、さまざまなデバイスの入力を統一した方法で扱えます。

Input System の機能を使うには、スクリプトの先頭に `using` ディレクティブを追加します。

```csharp
using UnityEngine.InputSystem;
```

---

## 2. Keyboard.current でキー入力を取得する

**`Keyboard.current`** — 現在アクティブなキーボードデバイスを取得します。<!-- [公式ドキュメント]() -->

**書式：Keyboard.current プロパティ**
```csharp
public static Keyboard current { get; }
```

`Keyboard.current` からは、キーごとのプロパティを通じて各キーの状態にアクセスします。たとえば →キー に対応する `rightArrowKey` は次のように定義されています。

**`Keyboard.rightArrowKey`** — →キーの入力状態を表す `ButtonControl` を返します。<!-- [公式ドキュメント]() -->

**書式：Keyboard.rightArrowKey プロパティ**
```csharp
public ButtonControl rightArrowKey { get; }
```

戻り値の型が **`ButtonControl`** であることに注目してください。`Keyboard` が持つキープロパティはすべて `ButtonControl` 型です。代表的なものを以下に示します。

| プロパティ | 対応するキー |
|---|---|
| `rightArrowKey` | → |
| `leftArrowKey` | ← |
| `upArrowKey` | ↑ |
| `downArrowKey` | ↓ |
| `spaceKey` | Space |
| `enterKey` | Enter |
| `escapeKey` | Escape |

---

`ButtonControl` 型は、キーの押下状態を調べるための以下のプロパティを持っています。

**`ButtonControl.isPressed`** — そのキーが**現在押されている**かどうかを返します。<!-- [公式ドキュメント]() -->

**`ButtonControl.wasPressedThisFrame`** — そのキーが**このフレームで初めて押された**かどうかを返します。<!-- [公式ドキュメント]() -->

**書式：ButtonControl のプロパティ**
```csharp
public bool isPressed { get; }
public bool wasPressedThisFrame { get; }
```

**使用例（→キーの場合）**
```
Keyboard.current.rightArrowKey.isPressed           // 押している間ずっと true
Keyboard.current.rightArrowKey.wasPressedThisFrame  // 押した瞬間の1フレームだけ true
```

| プロパティ | `true` になるタイミング | 用途の例 |
|---|---|---|
| `isPressed` | キーを押している間ずっと | 移動・継続的な操作 |
| `wasPressedThisFrame` | キーを押した瞬間の1フレームのみ | ジャンプ・攻撃など単発の操作 |

---

## 3. Update 内で入力を処理する理由

入力の判定は `Update` 内に書きます。`Start` に書くとゲーム開始時の1フレームしか判定されないため、プレイヤーのリアルタイムな操作に応答できません。

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSample : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            Debug.Log("→ キーが押されています");
        }
    }
}
```

---

## 4. 入力に応じてオブジェクトを動かす

前のページで学んだ `Translate` と `Time.deltaTime` と組み合わせて、キー入力でオブジェクトを移動させます。

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSample : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            transform.Translate(Vector3.right * 5.0f * Time.deltaTime);
        }

        if (Keyboard.current.leftArrowKey.isPressed)
        {
            transform.Translate(Vector3.left * 5.0f * Time.deltaTime);
        }
    }
}
```

→ キーを押している間だけ右へ、← キーを押している間だけ左へ移動します。

> 💡 **ポイント**: コード内の `5.0f` は1秒あたりの移動速度です。値を変えるだけで速度を調整できますが、同じ数値が複数箇所に散らばると管理しにくくなります。次のページでこの問題を解決します。

---

## ワンポイントアドバイス

Input System が登場する以前、Unity では **`Input` クラス** と **InputManager** が入力の主役でした。`Input.GetKey(KeyCode.RightArrow)` のような書き方で、追加パッケージなしにキー入力を扱うことができ、多くのプロジェクトを長年にわたって支えてきた実績ある仕組みです。現在も動作しますが、新規プロジェクトでは Input System が推奨されています。

詳しくは **[補足: 旧来の Input クラスと InputManager](/unity-csharp-learning/unity/legacy-input/)** を参照してください。

---

## よくあるミス

```csharp
// ❌ NG: using ディレクティブがないとコンパイルエラーになる
if (Keyboard.current.spaceKey.isPressed) { }

// ✅ OK: ファイルの先頭に using を追加する
using UnityEngine.InputSystem;
```

---

## まとめ

- Input System を使うには `using UnityEngine.InputSystem;` が必要
- `Keyboard.current.キー名.isPressed` で押している間を判定できる
- `wasPressedThisFrame` は押した瞬間だけ `true` になる
- 入力処理は `Update` 内に書く

---

## 理解度チェック

以下の問いに答えられるか確認しましょう。

1. `isPressed` と `wasPressedThisFrame` はどう使い分けますか？
2. 入力判定を `Start` に書くとどうなりますか？
3. スペースキーが押された瞬間だけ処理を実行するにはどう書きますか？

<details markdown="1">
<summary>解答を見る</summary>

1. 押している間ずっと処理したい場合は `isPressed`、押した瞬間だけ処理したい場合は `wasPressedThisFrame` を使う。
2. ゲーム開始時の1フレームしか判定されず、プレイヤーの入力に応答できない。
3. ```csharp
   if (Keyboard.current.spaceKey.wasPressedThisFrame)
   {
       // 処理
   }
   ```

</details>

---

## 次のステップ

[フィールドでデータを維持する](/unity-csharp-learning/unity/fields-basics/) では、速度などのパラメータをフィールドで管理し、Inspector から調整できるようにする方法を学びます。
