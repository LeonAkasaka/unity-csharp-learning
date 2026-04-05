---
layout: page
title: Rigidbody で力を加える
permalink: /unity/rigidbody-force/
---

# Rigidbody で力を加える

Rigidbody コンポーネントへの参照をスクリプトから取得し、**力（Force）を加えてオブジェクトを動かす**方法を学びます。キーボード入力と組み合わせることで、プレイヤーキャラクターの移動制御を実装できます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `GetComponent<T>()` でコンポーネントの参照を取得できる
- `Start` でコンポーネントをキャッシュして `Update` で使うパターンを使える
- `Rigidbody.AddForce()` でオブジェクトに力を加えられる
- キーボード入力に応じてオブジェクトを動かせる

## 前提知識

- [チュートリアル: 歩行者信号機](/unity-csharp-learning/unity/traffic-light/) を読んでいること

---

## 1. GetComponent<T>() でコンポーネントを取得する

Inspector ビューから追加したコンポーネントの参照を、スクリプトから取得するには **`GetComponent<T>()`** を使います。

**`Component.GetComponent<T>()`** — このオブジェクトに追加されているコンポーネントを取得します。見つからない場合は `null` を返します。<!-- [公式ドキュメント]() -->

**書式：GetComponent メソッド**
```csharp
public T GetComponent<T>();
```

| 型パラメータ | 説明 |
|---|---|
| `T` | 取得したいコンポーネントの型（例: `Rigidbody`） |

`GetComponent<Rigidbody>()` と書くと、その GameObject に追加されている Rigidbody コンポーネントを取得できます。

対象のコンポーネントが**見つからない場合は `null` が返ります**。`null` のまま使おうとすると実行時エラーになるため、確認が必要な場面では **`TryGetComponent<T>()`** を使うと安全です。

**`Component.TryGetComponent<T>()`** — コンポーネントを取得し、見つかった場合は `true`、見つからない場合は `false` を返します。<!-- [公式ドキュメント]() -->

**書式：TryGetComponent メソッド**
```csharp
public bool TryGetComponent<T>(out T component);
```

| パラメータ | 説明 |
|---|---|
| `T` | 取得したいコンポーネントの型 |
| `component` | 取得できた場合にコンポーネントが入る `out` 変数 |

```csharp
if (TryGetComponent<Rigidbody>(out var rb))
{
    // rb を安全に使える
}
```

> 💡 **使い分け**: コンポーネントが必ず存在することが確かなら `GetComponent`、存在するかどうか不確かなら `TryGetComponent` を使います。

---

## 2. コンポーネントはフィールドにキャッシュする

`GetComponent<T>()` は `Update` の中で呼ばず、`Start` で一度だけ呼んでフィールドに保存（キャッシュ）するのが基本です。

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Rigidbody _rigidbody;  // ← フィールドに保存

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();  // ← Start で一度だけ取得
    }

    private void Update()
    {
        // _rigidbody を使って毎フレーム処理
    }
}
```

> 💡 **ポイント**: `GetComponent` は毎フレーム呼ぶとパフォーマンスへの影響が積み重なります。`Start` で取得してフィールドに持っておきましょう。

---

## 3. AddForce() で力を加える

Rigidbody コンポーネントの **`AddForce()`** メソッドで、オブジェクトに力を加えられます。加えた力は物理エンジンによって自動的に速度・移動に変換されます。

**`Rigidbody.AddForce()`** — Rigidbody に力を加えます。<!-- [公式ドキュメント]() -->

**書式：Rigidbody.AddForce メソッド**
```csharp
public void AddForce(Vector3 force);
```

| パラメータ | 説明 |
|---|---|
| `force` | 加える力の方向と大きさ（`Vector3`） |

`Vector3` は X・Y・Z の3方向の値を持つ構造体です（[Transform ページ](/unity-csharp-learning/unity/transform/)で解説済み）。たとえば `new Vector3(1, 0, 0)` は X 軸方向に 1 の力を意味します。

---

## 4. キーボード入力で移動する

Input System のキー入力と `AddForce` を組み合わせると、キーボードでオブジェクトを動かせます。

```csharp
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
}
```

`move` は `Vector3.zero`（全方向ゼロ）から始まり、押されているキーに応じて X または Z 方向の値を足しています。最終的に `AddForce` に渡すと、その方向に力が加わります。

---

### 動作確認の手順

1. 新しいシーンを作成する
2. **GameObject → 3D Object → Sphere** で Sphere を追加し、名前を `Player` にする
3. Inspector ビューで **Add Component → Rigidbody** を追加する
4. 空の GameObject に `Player` スクリプトを作成してアタッチする（またはそのまま Sphere にアタッチ）
5. Play ボタンを押して方向キーで球体を動かす

---

## まとめ

- `GetComponent<T>()` で Inspector から追加されたコンポーネントの参照を取得できる
- `GetComponent` は `Start` で一度だけ呼び、フィールドに保存しておく
- `AddForce(Vector3)` でオブジェクトに力を加えて物理的に移動させられる
- キー入力 → `Vector3` の組み立て → `AddForce` の流れで自然な移動操作を実現できる

---

## 理解度チェック

1. `GetComponent<T>()` を `Update` ではなく `Start` で呼ぶ理由は何ですか？
2. `AddForce(new Vector3(0, 0, 1))` はどの方向に力を加えますか？
3. キーを離したとき、Rigidbody は即座に停止しますか？それとも少しずつ遅くなりますか？

<details markdown="1">
<summary>解答を見る</summary>

1. `GetComponent` はコストのかかる処理で、毎フレーム呼ぶとパフォーマンスへの影響が積み重なるため。`Start` で一度だけ取得してフィールドに保持する。
2. Z 軸方向（画面奥方向）に力が加わる。
3. 少しずつ遅くなる。物理エンジンの摩擦・抵抗が働いて減速する（Rigidbody の Drag 設定に依存）。

</details>

---

## 次のステップ

[Collider とトリガー判定](/unity-csharp-learning/unity/collider-trigger/) では、オブジェクト同士が重なったことをスクリプトで検知する方法を学びます。
