---
layout: page
title: コルーチンの待機方法
permalink: /unity/coroutine-waits/
---

# コルーチンの待機方法

前のページでは、時間を数えるループを `WaitSeconds` にまとめました。秒数待ちはさまざまな処理で必要になるため、Unity にも待機用の型が用意されています。自作した待機処理と比べながら、時間や条件を指定して待つ書き方を学びます。

## 学習目標

- 自作の秒数待ちを `WaitForSeconds` に置き換えられる
- ゲーム時間の待機と現実時間の待機を使い分けられる
- 条件待ちのループを理解してから、`WaitUntil` に置き換えられる
- フレーム待ちと、物理更新後・描画後の待機を区別できる

## 前提知識

- [コルーチンを組み合わせる](/unity-csharp-learning/unity/coroutine-composition/) を読んでいること
- [Input System で入力操作](/unity-csharp-learning/unity/input-system/) を読んでいること（パッケージは導入・設定済み）

---

## 1. WaitForSeconds で秒数を待つ

自作の `WaitSeconds` は、毎フレーム `Time.deltaTime` を足し、指定した秒数になったかを調べていました。「指定時間が過ぎるまで待つ」というよく使う処理を、その都度実装せずに書けるのが `WaitForSeconds` です。待つ秒数を Unity に渡せば、時間を数えるループを自分で書く必要がなくなります。

空のゲームオブジェクトに、動作確認用の `SampleCoroutine` スクリプトを作成してアタッチします。

**書式：[WaitForSeconds コンストラクター](https://docs.unity3d.com/ScriptReference/WaitForSeconds-ctor.html)**

```csharp
public WaitForSeconds(float seconds);
```

| パラメータ | 説明 |
|---|---|
| `seconds` | 待つゲーム時間の秒数 |

```csharp
using System.Collections;
using UnityEngine;

public class SampleCoroutine : MonoBehaviour
{
    private IEnumerator Start()
    {
        Debug.Log("開始");
        yield return new WaitForSeconds(1f);
        Debug.Log("約 1 秒後");
    }
}
```

「開始」と表示し、約 1 秒後に次の行へ進みます。

```
開始
約 1 秒後
```

`WaitForSeconds` は子の `IEnumerator` ではなく、Unity に秒数待ちを指示するオブジェクトです。`yield return` は渡したものによって待ち方が変わります。`null` なら次のフレーム、子の `IEnumerator` ならその終了、`WaitForSeconds` なら指定時間を待ちます。

`WaitForSeconds` は現在のフレームの終わりから指定時間を待ち、指定時間が過ぎた最初のフレームで再開します。自作の待機ループと、細かな計測開始・再開のタイミングまで同じではありません。

---

## 2. ゲーム時間と現実時間を使い分ける

一時停止中には、キャラクターの動作やゲーム内の時間待ちも止まってほしい場合があります。一方、一時停止画面の案内などは、ゲームが止まっていても一定時間後に表示したいことがあります。どちらも「1 秒待つ」処理ですが、何の時間を基準にするかが違います。

`Time.deltaTime` で数える時間と `WaitForSeconds` は、`Time.timeScale` の影響を受けます。`timeScale` が `0` の間も待ち時間を進めたい場合は、`WaitForSecondsRealtime` を使います。次の例では、ゲーム時間を止めた状態で現実時間の 1 秒を待ち、その後ゲーム時間を再開します。

**書式：[WaitForSecondsRealtime コンストラクター](https://docs.unity3d.com/ScriptReference/WaitForSecondsRealtime-ctor.html)**

```csharp
public WaitForSecondsRealtime(float time);
```

| パラメータ | 説明 |
|---|---|
| `time` | 待つ現実時間の秒数 |

```csharp
using System.Collections;
using UnityEngine;

public class SampleCoroutine : MonoBehaviour
{
    private IEnumerator Start()
    {
        Time.timeScale = 0f;
        Debug.Log("一時停止");
        yield return new WaitForSecondsRealtime(1f);
        Debug.Log("現実時間で約 1 秒経過");
        Time.timeScale = 1f;
        yield return new WaitForSeconds(1f);
        Debug.Log("ゲーム時間で約 1 秒経過");
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
```

表示は次の順です。最初の待機はゲームが一時停止していても進み、約 1 秒後にゲーム時間を動かし直してから、次の 1 秒を待ちます。

```
一時停止
現実時間で約 1 秒経過
ゲーム時間で約 1 秒経過
```

`OnDisable` は、途中で Play を止めた場合にも `timeScale` を戻すための処理です。

---

## 3. 条件を満たすまでループで待つ

入力を受けてから次の処理へ進みたい場合、何秒待てばよいかは決められません。必要なのは時間の経過ではなく、「要求を受け取った」という条件が成り立つまで待つことです。

時間待ちのループが経過秒数を調べていたように、条件待ちのループでは `bool` のフィールドを調べます。条件がまだ成り立たない間は `yield return null` で中断し、次のフレームにもう一度調べます。

```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SampleCoroutine : MonoBehaviour
{
    private bool _isRequested;

    private IEnumerator Start()
    {
        Debug.Log("入力待ち");
        while (!_isRequested)
        {
            yield return null;
        }
        Debug.Log("条件成立");
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
        {
            _isRequested = true;
        }
    }
}
```

最初は「入力待ち」だけが表示されます。Game ビューをクリックしてから Space キーを押すと、「条件成立」が表示されます。

`Update` は Input System のキー入力を調べ、押されたフレームに `_isRequested` を `true` にします。待機ループはフレームごとに条件を調べ、`true` になったらループを抜けて次の行へ進みます。

---

## 4. 条件を返す処理を渡す

Unity には、条件を満たすまで待つ `WaitUntil` も用意されています。使う前に、「条件の値」と「条件を調べる処理」を区別しましょう。

`_isRequested` は、その時点の `bool` 値です。一方、`WaitUntil` には、後から何度も呼んで条件を調べられる処理を渡します。そこで、引数を取らずに `bool` を返す**ラムダ式**を使います。

**書式：[ラムダ式](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/lambda-expressions)**

```csharp
() => 条件式
```

| 部分 | 意味 |
|---|---|
| `()` | 引数を受け取らない |
| `=>` | 左側の引数を受け取り、右側の処理を行う |
| `_isRequested` | 呼ばれた時点のフィールドの値を返す |

`() => _isRequested` は「呼ばれたときに `_isRequested` を調べて返す処理」です。最初の `false` を保存して返し続けるわけではありません。詳しくは [ラムダ式](/unity-csharp-learning/csharp/lambda/) を参照してください。次の節の全体コードで、この式を使います。

---

## 5. 条件待ちを WaitUntil に置き換える

条件を満たすまで待つたびに `while` と `yield return null` を書く代わりに、`WaitUntil` へ「何を満たせば次へ進めるか」を渡せます。条件の確認と待機の繰り返しを Unity に任せることで、呼び出す側には待ちたい条件が直接表れます。

**書式：[WaitUntil コンストラクター](https://docs.unity3d.com/ScriptReference/WaitUntil-ctor.html)**

```csharp
public WaitUntil(Func<bool> predicate);
```

| パラメータ | 説明 |
|---|---|
| `predicate` | 引数を取らず、待機を終えるときに `true` を返す処理。`Func<bool>` はその処理を受け取る型 |

3 節の待機ループを `WaitUntil` に置き換えた全体コードです。

```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SampleCoroutine : MonoBehaviour
{
    private bool _isRequested;

    private IEnumerator Start()
    {
        Debug.Log("入力待ち");
        yield return new WaitUntil(() => _isRequested);
        Debug.Log("条件成立");
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
        {
            _isRequested = true;
        }
    }
}
```

Space キーを押すと、ループ版と同じ順に表示されます。

```
入力待ち
条件成立
```

Unity は `Update` の後、`LateUpdate` の前に渡された処理で条件を調べ、`true` なら再開します。ループ版と目的は同じですが、条件を調べるタイミングまで完全に同じとは限りません。

条件が `true` の間だけ待ちたい場合は [WaitWhile](https://docs.unity3d.com/ScriptReference/WaitWhile.html) も使えます。

### フレーム内の再開タイミングを選ぶ

`yield return null` は次のフレームを待ちますが、物理更新の結果を使いたい場合や、描画が終わってから画面を取得したい場合には、どの段階まで待つかも重要です。

| 待機 | 再開する段階 | 用途の例 |
|---|---|---|
| `yield return null` | 次のフレームのコルーチンの再開段階 | 毎フレーム少しずつ処理を進める |
| `WaitForFixedUpdate` | 次の物理更新が終わった後 | 物理更新後の状態を使う |
| `WaitForEndOfFrame` | カメラや GUI の描画が終わった後 | 描画結果の取得 |

**書式：[WaitForFixedUpdate クラス](https://docs.unity3d.com/ScriptReference/WaitForFixedUpdate.html)**

```csharp
yield return new WaitForFixedUpdate();
```

**書式：[WaitForEndOfFrame クラス](https://docs.unity3d.com/ScriptReference/WaitForEndOfFrame.html)**

```csharp
yield return new WaitForEndOfFrame();
```

同じ `SampleCoroutine` で、物理更新後、描画後の順にログを出す最小例です。秒数を待つ指定ではなく、Unity の処理段階を待つ指定になっています。

```csharp
using System.Collections;
using UnityEngine;

public class SampleCoroutine : MonoBehaviour
{
    private IEnumerator Start()
    {
        Debug.Log("開始");
        yield return new WaitForFixedUpdate();
        Debug.Log("物理更新後");
        yield return new WaitForEndOfFrame();
        Debug.Log("描画後");
    }
}
```

```
開始
物理更新後
描画後
```

物理更新は、描画の各フレームに必ず 1 回ずつ起きるわけではありません。また、`Time.timeScale` が `0` の間は物理更新も止まるため、`WaitForFixedUpdate` で待っている処理も進みません。`WaitForEndOfFrame` を Editor で確認するときは Game ビューを表示します。Scene ビューへ切り替えると、その待機が進まなくなることがあります。[待機命令の一覧](https://docs.unity3d.com/Manual/coroutines-yield-instructions.html)も参照してください。

---

## 6. 実例：信号機を待機 API で書き換える

前のページの信号機を、ここまでに学んだ待機方法で書き換えます。

- 自作の `WaitSeconds` を `new WaitForSeconds(...)` に置き換える
- 赤にしたら、Space キーによる要求を `WaitUntil` で待つ
- 次の赤でも入力を待てるよう、要求を受け取った後に `_isRequested` を `false` に戻す

`Signal` の `CoroutineSignal` を次の全体コードに書き換えます。同じ球体を使い、キー入力は Input System の `Update` で直接調べます。

### 完成したコード

```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoroutineSignal : MonoBehaviour
{
    private Renderer _renderer;
    private bool _isRequested;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        StartCoroutine(RunSignal());
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            RequestGreen();
        }
    }

    public void RequestGreen()
    {
        Debug.Log($"{Time.time:F2} 秒: 切り替え要求");
        _isRequested = true;
    }

    private IEnumerator RunSignal()
    {
        while (true)
        {
            SetColor(Color.red, "赤");
            yield return new WaitUntil(() => _isRequested);
            _isRequested = false;

            yield return new WaitForSeconds(1f);

            SetColor(Color.blue, "青");
            yield return new WaitForSeconds(2f);

            yield return Blink(2); // 追加：点滅が終わるまで待つ
        }
    }

    // 追加：灰色と青を 0.25 秒ずつ、count 回繰り返す
    private IEnumerator Blink(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SetColor(Color.gray, "灰");
            yield return new WaitForSeconds(0.25f);

            SetColor(Color.blue, "青");
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void SetColor(Color color, string label)
    {
        _renderer.material.color = color;
        Debug.Log($"{Time.time:F2} 秒: {label}");
    }
}
```

---

## 動作確認

1. `Signal` の `CoroutineSignal` を「完成したコード」に書き換える
2. Play ボタンを押し、Game ビューをクリックする
3. 何もしない間は赤のままであることを確認する
4. Space キーを押して、約 1 秒後に青、約 2 秒後から点滅、点滅を 2 回繰り返した後に赤へ戻ることを確認する
5. 赤に戻ってから、もう一度押すまで待機することを確認する

Console の実行結果の例です。秒数はキーを押した時刻とフレームレートによって変わります。

```
0.00 秒: 赤
1.00 秒: 切り替え要求
2.04 秒: 青
4.06 秒: 灰
4.32 秒: 青
4.58 秒: 灰
4.84 秒: 青
5.10 秒: 赤
```

---

## まとめ

- `WaitForSeconds` はゲーム時間、`WaitForSecondsRealtime` は現実時間で待つ
- `while` と `yield return null` を組み合わせると、条件を満たすまで待てる
- ラムダ式 `() => _isRequested` は、呼ばれた時点の値を返す処理を表す
- `WaitUntil` は渡された処理で条件を調べ、`true` になると再開する

---

## 理解度チェック

1. `timeScale` が `0` の間も進めたい待機には、どちらの秒数待ちを使いますか？
2. `WaitUntil` に、`_isRequested` そのものではなく `() => _isRequested` を渡すのはなぜですか？
3. `WaitUntil` の後の `_isRequested = false;` を消すとどうなりますか？

<details markdown="1">
<summary>解答を見る</summary>

1. `WaitForSecondsRealtime` を使います。
2. 一度だけ取得した値ではなく、後から何度も条件を調べる処理を渡すためです。
3. 一度押すと `true` のまま残ります。次に赤へ戻っても条件を満たしているので、もう一度押さずに先へ進みます。

</details>

---

## 次のステップ

[コルーチンの開始と停止](/unity-csharp-learning/unity/coroutine-control/) では、同じシーンの信号機を P キーで開始・停止できるようにし、開始したコルーチンを保存して止める方法を学びます。
