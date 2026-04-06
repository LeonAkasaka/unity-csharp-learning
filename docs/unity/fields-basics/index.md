---
layout: page
title: フィールドでデータを維持する
permalink: /unity/fields-basics/
---

# フィールドでデータを維持する

`Update` はフレームごとに実行されますが、メソッド内で宣言した変数はフレームをまたいで保持されません。フレーム間でデータを持ち続けるには**フィールド**を使います。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- ローカル変数とフィールドの違いを説明できる
- フィールドを宣言してフレーム間でデータを維持できる
- `[SerializeField]` を使って Inspector からフィールドの値を編集できる

## 前提知識

- [Input System で入力操作](/unity-csharp-learning/unity/input-system/) を読んでいること

---

## 1. ローカル変数はフレームをまたがない

`Update` 内で変数を宣言すると、その変数は**そのフレームの中だけで有効**で、次のフレームでは消えてしまいます。たとえばカウンターを作ろうとして次のように書いても、意図した動作にはなりません。

```csharp
using UnityEngine;

public class FieldSample : MonoBehaviour
{
    private void Update()
    {
        var count = 0;     // 毎フレーム 0 にリセットされる
        count++;
        Debug.Log(count);  // 常に 1 と出力される
    }
}
```

`count` は毎フレーム `0` で初期化されるため、値が積み上がりません。

---

## 2. フィールドでデータを維持する

クラス内・メソッドの外に変数を宣言すると、インスタンスが生きている間ずっとデータを保持できます。このような変数を**フィールド**と呼びます。

**書式：フィールドの宣言**
```
アクセス修飾子 型 フィールド名 = 初期値;
```

| 要素 | 説明 |
|---|---|
| `アクセス修飾子` | `private`（クラス内のみ）や `public`（外部からも参照可）など |
| `型` | `int`・`float`・`string` など |
| `フィールド名` | 変数の名前 |
| `= 初期値` | 省略可。省略した場合はデフォルト値（数値なら `0`）が入る |

> 💡 **ポイント**: アクセス修飾子や型の詳細は [C# 基礎](/unity-csharp-learning/csharp/) のセクションで改めて学びます。ここでは「クラスの中、メソッドの外に書くとフレームをまたいで保持される」と覚えておきましょう。

フィールドを使ってカウンターを書き直すと、正しく動作します。

```csharp
using UnityEngine;

public class FieldSample : MonoBehaviour
{
    private int _count = 0;  // フィールド: フレームをまたいで保持される

    private void Update()
    {
        _count++;
        Debug.Log(_count);  // 1, 2, 3 … と増えていく
    }
}
```

> 💡 **ポイント**: `private` フィールドには**アンダースコア `_` を先頭に付ける**命名慣習があります（`_count`・`_speed` など）。メソッド内のローカル変数（`var count`）との区別が一目でつき、コードが読みやすくなります。

---

## 3. 速度をフィールドで管理する

前のページで書いた移動コードに登場した `5.0f` はハードコードされた数値です。速度を変えるたびにコード内を検索して書き直す必要があります。

```csharp
// ❌ NG: 速度が複数箇所にハードコードされていて管理しにくい
transform.Translate(Vector3.right * 5.0f * Time.deltaTime);
transform.Translate(Vector3.left * 5.0f * Time.deltaTime);
```

この数値をフィールドに切り出すと、1か所を変えるだけで全体に反映されます。

```csharp
// ✅ OK: 速度をフィールドで管理する
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMover : MonoBehaviour
{
    private float _speed = 5.0f;  // 速度をフィールドで管理

    private void Update()
    {
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            transform.Translate(Vector3.right * _speed * Time.deltaTime);
        }

        if (Keyboard.current.leftArrowKey.isPressed)
        {
            transform.Translate(Vector3.left * _speed * Time.deltaTime);
        }
    }
}
```

---

## 4. [SerializeField]で Inspector から値を編集する

`private` フィールドはクラスの外部からアクセスできませんが、**`[SerializeField]` 属性**を付けると、コードを書き直さずに Unity の Inspector ビューから値を編集できるようになります。

**`[SerializeField]`** — `private` フィールドを Inspector に表示して編集できるようにします。<!-- [公式ドキュメント]() -->

**書式：SerializeField 属性**
```
[SerializeField] アクセス修飾子 型 フィールド名 = 初期値;
```

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMover : MonoBehaviour
{
    [SerializeField] private float _speed = 5.0f;

    private void Update()
    {
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            transform.Translate(Vector3.right * _speed * Time.deltaTime);
        }

        if (Keyboard.current.leftArrowKey.isPressed)
        {
            transform.Translate(Vector3.left * _speed * Time.deltaTime);
        }
    }
}
```

Inspector ビューの Script コンポーネントに **Speed** フィールドが表示され、コードを変更せずに速度を調整できます。Play 中に値を変えてリアルタイムに挙動を確認することもできます。

> 💡 **ポイント**: `public` にしても Inspector に表示されますが、`[SerializeField] private` にすることで「外部からは参照できないが、Inspector からは編集できる」という適切なカプセル化が保たれます。

---

## まとめ

- メソッド内のローカル変数はフレームをまたいで保持されない
- **フィールド**はクラス内・メソッドの外に宣言し、フレーム間でデータを維持できる
- パラメータをフィールドに切り出すと変更が1か所で済む
- **`[SerializeField]`** を付けると `private` を保ちながら Inspector から値を編集できる

---

## 理解度チェック

以下の問いに答えられるか確認しましょう。

1. `Update` 内でカウンターを作りたいのに、値が毎フレーム `1` になってしまいます。原因は何ですか？
2. `[SerializeField]` を付けるメリットを2つ答えてください。
3. 次のコードの `_speed` フィールドを Inspector から編集できるように修正してください。

   ```csharp
   private float _speed = 3.0f;
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. カウンターが `Update` メソッド内（ローカル変数）で宣言されているため、毎フレーム `0` に初期化されている。フィールドとしてクラスレベルで宣言する必要がある。
2. ①コードを変更せずに Inspector から値を調整できる　②`private` を保ちながら外部編集できるためカプセル化が崩れない。
3. ```csharp
   [SerializeField] private float _speed = 3.0f;
   ```

</details>
