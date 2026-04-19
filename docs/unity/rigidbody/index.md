---
layout: page
title: AddComponent と物理演算
permalink: /unity/rigidbody/
---

# AddComponent と物理演算

Unity にはオブジェクトを物理法則に従って動かす**物理エンジン**が組み込まれています。このページでは、スクリプトからコンポーネントを追加する方法と、物理演算を担う **Rigidbody** コンポーネントを学びます。

## 学習目標

- `AddComponent<T>()` を使ってコンポーネントをスクリプトから追加できる
- Rigidbody コンポーネントの役割を説明できる
- 物理エンジンがオブジェクトの動作を自動的に管理することを理解できる

## 前提知識

- [Transform でオブジェクトを操作する](/unity-csharp-learning/unity/transform/) を読んでいること

---

## 1. AddComponent<T>() でコンポーネントを追加する

Unity Inspector ビューから手動でコンポーネントを追加できますが、スクリプトからも追加できます。

**`GameObject.AddComponent<T>`** — ゲームオブジェクトにコンポーネントを追加します。<!-- [公式ドキュメント]() -->

**書式：GameObject.AddComponent メソッド**
```csharp
public T AddComponent<T>() where T : Component;
```

| 型パラメータ | 説明 |
|---|---|
| `T` | 追加するコンポーネントの型 |

使い方はこれまでのメソッドと少し異なり、丸括弧 `()` の前に **山括弧 `<>`** を付けて追加したいコンポーネントの型を指定します。

> 💡 **ポイント**: 通常のパラメータは `()` に入れて「値」を渡しますが、型パラメータは `<>` に入れて「型」を渡します。`AddComponent<Rigidbody>()` の `Rigidbody` は値ではなく、「どの型のコンポーネントを追加するか」を指定しています。

```csharp
gameObject.AddComponent<Rigidbody>();
```

---

## 2. Rigidbody で物理演算を有効にする

**Rigidbody** コンポーネントを追加すると、そのオブジェクトは Unity の物理エンジンによって管理されます。重力・衝突・摩擦といった物理的な挙動が自動的に計算されます。

こうした物理法則をゼロからプログラムするのは非常に複雑ですが、Rigidbody を追加するだけで Unity が肩代わりしてくれます。

```csharp
using UnityEngine;

public class Sample : MonoBehaviour
{
    private void Start()
    {
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.AddComponent<Rigidbody>();
    }
}
```

<video controls src="./video.mp4"></video>

Rigidbody を追加した球体は、ゲーム開始と同時に重力で落ちていきます。

> 💡 **ポイント**: ステージ（地面）には Rigidbody を追加しません。Rigidbody がついたオブジェクトは重力の影響を受けて落下してしまうためです。

---

## ワンポイントアドバイス

Rigidbody を追加した後でも、Inspector ビューで **Is Kinematic** をオンにすると物理エンジンの制御を無効化し、スクリプトから直接位置・回転を制御できます。物理演算と直接制御を使い分けたい場面で役立ちます。

---

## まとめ

- `AddComponent<Rigidbody>()` で物理演算コンポーネントをスクリプトから追加できる
- `<T>` の `T` に追加したいコンポーネントの型を指定する
- Rigidbody を追加すると Unity の物理エンジンが重力・衝突を自動的に処理する
- 地面など動いてほしくないオブジェクトには Rigidbody を追加しない

---

## 理解度チェック

1. `AddComponent<Rigidbody>()` を呼ぶと何が起きますか？
2. `<Rigidbody>` の山括弧 `<>` は何の役割がありますか？
3. ステージ（地面）に Rigidbody を追加すると何が起こりますか？

<details markdown="1">
<summary>解答を見る</summary>

1. ゲームオブジェクトに Rigidbody コンポーネントが追加され、重力・衝突などの物理演算が有効になる。
2. 追加するコンポーネントの**型**を指定するための型パラメータ。`<Rigidbody>` であれば Rigidbody コンポーネントを追加することを表す。
3. Rigidbody がついたオブジェクトは重力の影響を受けるため、ステージが落下してしまう。

</details>

---

## 次のステップ

[チュートリアル: ドミノ倒し](/unity-csharp-learning/unity/domino/) では、これまで学んだすべてを組み合わせてドミノ倒しのシミュレーションを完成させます。
