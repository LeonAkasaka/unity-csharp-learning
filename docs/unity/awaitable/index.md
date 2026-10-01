---
layout: page
title: Awaitable と async / await
permalink: /unity/awaitable/
---

# Awaitable と async / await

[コルーチンでは書きにくいこと](/unity-csharp-learning/unity/coroutine-limits/) では、コルーチンでは結果を返せないこと、失敗を `try` / `catch` で受け取れないこと、止めたときに中で後片付けできないことを確かめました。Unity には、C# の `async` / `await` でフレームや秒数を待つための **Awaitable** 型が用意されています。このページでは、押しボタン式の信号機を `Awaitable` で書き直し、これらがどう解決するのかを確かめます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `async Awaitable` のメソッドを書き、`Awaitable.NextFrameAsync` と `Awaitable.WaitForSecondsAsync` でフレームや秒数を待てる
- `Awaitable<TResult>` を返すメソッドで、結果を `return` で返せる
- `await` した処理の例外を、`try` / `catch` で受け取れる
- `CancellationToken` で処理を止め、止められた処理の中の `finally` で後片付けできる
- async メソッドがゲームオブジェクトの破棄で止まらないことを説明し、`destroyCancellationToken` で止められる
- Unity から呼ばれるメソッドを `async void` にする理由を説明できる

## 前提知識

- [コルーチンでは書きにくいこと](/unity-csharp-learning/unity/coroutine-limits/) を読んでいること
- [async と await](/unity-csharp-learning/csharp/async-await/) を読んでいること
- [キャンセル](/unity-csharp-learning/csharp/task-cancellation/) を読んでいること

---

## 1. シーンを準備する

[コルーチンでは書きにくいこと](/unity-csharp-learning/unity/coroutine-limits/) で使ったシーンを、そのまま使います。

- 球体の `Signal` に、`PushButtonSignal` スクリプトがアタッチされている
- `PushButton`（**PUSH**）の **On Click ()** に **PushButtonSignal → OnPushButton ()** が設定されている
- `PowerButton`（**POWER**）の **On Click ()** に **PushButtonSignal → OnPowerButton ()** が設定されている

`Signal` の `PushButtonSignal` で `Blink Count` を変更していた場合は、`2` に戻しておきます。このページでも、`PushButtonSignal` スクリプトを書き換えながら進めます。

---

## 2. コルーチンを async メソッドに書き換える

[コルーチンでは書きにくいこと](/unity-csharp-learning/unity/coroutine-limits/) の 3 節のスクリプトを、`Awaitable` を使って書き直します。電源のボタンは 4 節で作り直すので、ここでは `OnPowerButton` の中身を空にしておきます。`PushButtonSignal` スクリプトを次のように書き換えます。

```csharp
using System;
using UnityEngine;

public class PushButtonSignal : MonoBehaviour
{
    [SerializeField] private float _guideInterval = 5f;
    [SerializeField] private int _blinkCount = 2;

    private Renderer _renderer;
    private bool _isRequested;

    private async void Start()
    {
        _renderer = GetComponent<Renderer>();
        await RunSignal();
    }

    public void OnPushButton()
    {
        Debug.Log($"{Time.time:F2} 秒: 押しボタン");
        _isRequested = true;
    }

    public void OnPowerButton()
    {
        // 4 節で書く
    }

    private async Awaitable RunSignal()
    {
        while (true)
        {
            SetColor(Color.red, "赤");

            while (!await WaitForPush(_guideInterval))
            {
                Debug.Log($"{Time.time:F2} 秒: 案内「ボタンを押してください」");
            }
            _isRequested = false;

            await Awaitable.WaitForSecondsAsync(1f);

            SetColor(Color.blue, "青");
            await Awaitable.WaitForSecondsAsync(2f);

            await Blink(_blinkCount);
        }
    }

    private async Awaitable<bool> WaitForPush(float timeout)
    {
        float deadline = Time.time + timeout;
        while (!_isRequested && Time.time < deadline)
        {
            await Awaitable.NextFrameAsync();
        }
        return _isRequested;
    }

    private async Awaitable Blink(int count)
    {
        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "点滅の回数は 1 以上にしてください");
        }

        for (int i = 0; i < count; i++)
        {
            SetColor(Color.gray, "灰");
            await Awaitable.WaitForSecondsAsync(0.25f);

            SetColor(Color.blue, "青");
            await Awaitable.WaitForSecondsAsync(0.25f);
        }
    }

    private void SetColor(Color color, string label)
    {
        _renderer.material.color = color;
        Debug.Log($"{Time.time:F2} 秒: {label}");
    }
}
```

Play ボタンを押して **PUSH** ボタンをクリックすると、コルーチンの版と同じように動きます。Console には次のように表示されます。これは実行結果の例です。秒数は、ボタンをクリックした時刻とフレームレートによって変わります。

```
0.00 秒: 赤
1.00 秒: 押しボタン
2.04 秒: 青
4.06 秒: 灰
4.32 秒: 青
4.58 秒: 灰
4.84 秒: 青
5.10 秒: 赤
10.12 秒: 案内「ボタンを押してください」
```

### async Awaitable のメソッド

`RunSignal` と `Blink` は、戻り値の型を `IEnumerator` から [Awaitable](https://docs.unity3d.com/ScriptReference/Awaitable.html) に変え、`async` 修飾子を付けました。`Awaitable` は `UnityEngine` 名前空間の型で、C# の `Task` と同じように、async メソッドの戻り値にでき、`await` で完了を待てます。`async` / `await` の使い方は、[async と await](/unity-csharp-learning/csharp/async-await/) で学んだものと同じです。

コルーチンの `yield return` は、`Awaitable` のメソッドを `await` する形に置き換わります。

**`Awaitable.WaitForSecondsAsync()`** — 指定した秒数（ゲーム時間）がたつと完了する `Awaitable` を返します。

**書式：[Awaitable.WaitForSecondsAsync メソッド](https://docs.unity3d.com/ScriptReference/Awaitable.WaitForSecondsAsync.html)**
```csharp
public static Awaitable WaitForSecondsAsync(float seconds, CancellationToken cancellationToken = default);
```

| パラメータ | 説明 |
|---|---|
| `seconds` | 待つ秒数 |
| `cancellationToken` | 待つのをやめさせるためのキャンセルトークン（4 節で使う）。省略できる |

**`Awaitable.NextFrameAsync()`** — 次のフレームで完了する `Awaitable` を返します。

**書式：[Awaitable.NextFrameAsync メソッド](https://docs.unity3d.com/ScriptReference/Awaitable.NextFrameAsync.html)**
```csharp
public static Awaitable NextFrameAsync(CancellationToken cancellationToken = default);
```

| パラメータ | 説明 |
|---|---|
| `cancellationToken` | 待つのをやめさせるためのキャンセルトークン（4 節で使う）。省略できる |

コルーチンの書き方と、`Awaitable` の書き方は、次のように対応します。

| コルーチン | `Awaitable` |
|---|---|
| `IEnumerator` を返すメソッド | `async Awaitable` のメソッド |
| `yield return null;` | `await Awaitable.NextFrameAsync();` |
| `yield return new WaitForSeconds(秒数);` | `await Awaitable.WaitForSecondsAsync(秒数);` |
| `yield return new WaitUntil(() => 条件);` | `while (!条件) { await Awaitable.NextFrameAsync(); }` |
| `yield return Blink(2);` | `await Blink(2);` |
| `StartCoroutine(RunSignal());` | `await RunSignal();`（呼び出し元も async メソッドにする） |

`Awaitable` には `WaitUntil` にあたるメソッドがないので、`WaitForPush` では、条件を満たすまで `NextFrameAsync` で 1 フレームずつ待つループを書いています。

### 結果を return で返す

`WaitForPush` の戻り値の型は `Awaitable<bool>` です。[Awaitable\<T\>](https://docs.unity3d.com/ScriptReference/Awaitable_1.html) は、完了したときに `T` 型の結果を持つ `Awaitable` で、`Task<TResult>` と同じように、async メソッドの中で `return` した値が結果になります。

```csharp
return _isRequested;
```

呼び出し元では、`await WaitForPush(_guideInterval)` の値として結果を受け取れます。

```csharp
while (!await WaitForPush(_guideInterval))
{
    Debug.Log($"{Time.time:F2} 秒: 案内「ボタンを押してください」");
}
```

`await WaitForPush(...)` は、ボタンが押されるか時間切れになるまで待ち、押されたかどうかの `bool` になります。コルーチンの版のように、コールバックとローカル変数を用意する必要はありません。

### Start を async void にする

コルーチンは `StartCoroutine` で開始しましたが、async メソッドは呼び出せば実行が始まります。`Start` を `async void` にして、`await RunSignal();` で呼び出しています。

[async と await](/unity-csharp-learning/csharp/async-await/) では、`async void` はイベントハンドラーのように戻り値が `void` と決められているメソッドだけに使う、と学びました。`Start` は Unity から呼ばれるメソッドで、戻り値を受け取って `await` する呼び出し元がありません。そのため、`async void` にします。`async void` にする理由は、この後の 3 節と「よくあるミス」でも確かめます。

---

## 3. 例外を try / catch で受け取る

`Blink` は、点滅の回数が 1 未満のときに例外を投げます。`Signal` の `PushButtonSignal` で `Blink Count` を `0` にして、Play ボタンを押し、**PUSH** ボタンをクリックすると、青になって 2 秒後に例外が表示されます。

```
0.00 秒: 赤
1.00 秒: 押しボタン
2.04 秒: 青
ArgumentOutOfRangeException: 点滅の回数は 1 以上にしてください
Parameter name: count
```

Console で例外の行を選択すると、スタックトレースは次のようになっています（一部を省略しています）。これは表示の例です。行番号は、スクリプトの書き方によって変わります。

```
PushButtonSignal.Blink (System.Int32 count) (at Assets/PushButtonSignal.cs:64)
UnityEngine.Awaitable.PropagateExceptionAndRelease () (at <...>:0)
PushButtonSignal.RunSignal () (at Assets/PushButtonSignal.cs:46)
UnityEngine.Awaitable.PropagateExceptionAndRelease () (at <...>:0)
PushButtonSignal.Start () (at Assets/PushButtonSignal.cs:15)
```

`Blink` で起きた例外が、`await Blink(...)` していた `RunSignal` へ、さらに `await RunSignal()` していた `Start` へと伝わっていることがわかります。コルーチンでは `Blink` しか表示されませんでしたが、async メソッドでは、普通のメソッドの呼び出しと同じように、呼び出し元をたどれます。

例外が呼び出し元に伝わるので、`await` を `try` / `catch` で囲めば、例外を受け取れます。`PushButtonSignal` スクリプトを次のように書き換えます。変わったのは、`RunSignal` の最後の `await Blink(_blinkCount);` を `try` / `catch` で囲んだところだけです。

```csharp
using System;
using UnityEngine;

public class PushButtonSignal : MonoBehaviour
{
    [SerializeField] private float _guideInterval = 5f;
    [SerializeField] private int _blinkCount = 2;

    private Renderer _renderer;
    private bool _isRequested;

    private async void Start()
    {
        _renderer = GetComponent<Renderer>();
        await RunSignal();
    }

    public void OnPushButton()
    {
        Debug.Log($"{Time.time:F2} 秒: 押しボタン");
        _isRequested = true;
    }

    public void OnPowerButton()
    {
        // 4 節で書く
    }

    private async Awaitable RunSignal()
    {
        while (true)
        {
            SetColor(Color.red, "赤");

            while (!await WaitForPush(_guideInterval))
            {
                Debug.Log($"{Time.time:F2} 秒: 案内「ボタンを押してください」");
            }
            _isRequested = false;

            await Awaitable.WaitForSecondsAsync(1f);

            SetColor(Color.blue, "青");
            await Awaitable.WaitForSecondsAsync(2f);

            try
            {
                await Blink(_blinkCount);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.LogWarning($"{Time.time:F2} 秒: 点滅できなかったので、1 秒待つ（{e.GetType().Name}）");
                await Awaitable.WaitForSecondsAsync(1f);
            }
        }
    }

    private async Awaitable<bool> WaitForPush(float timeout)
    {
        float deadline = Time.time + timeout;
        while (!_isRequested && Time.time < deadline)
        {
            await Awaitable.NextFrameAsync();
        }
        return _isRequested;
    }

    private async Awaitable Blink(int count)
    {
        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "点滅の回数は 1 以上にしてください");
        }

        for (int i = 0; i < count; i++)
        {
            SetColor(Color.gray, "灰");
            await Awaitable.WaitForSecondsAsync(0.25f);

            SetColor(Color.blue, "青");
            await Awaitable.WaitForSecondsAsync(0.25f);
        }
    }

    private void SetColor(Color color, string label)
    {
        _renderer.material.color = color;
        Debug.Log($"{Time.time:F2} 秒: {label}");
    }
}
```

`Blink Count` が `0` のまま Play ボタンを押して、**PUSH** ボタンをクリックすると、警告を表示して 1 秒待ち、赤に戻ります。Console には次のように表示されます。これは実行結果の例です。

```
0.00 秒: 赤
1.00 秒: 押しボタン
2.04 秒: 青
4.06 秒: 点滅できなかったので、1 秒待つ（ArgumentOutOfRangeException）
5.08 秒: 赤
```

コルーチンでは、`catch` 句のある `try` ブロックの中に `yield return` を書けませんでした（CS1626）。`await` には、この制約はありません。確認が終わったら、`Blink Count` を `2` に戻します。

---

## 4. CancellationToken で止める

電源のボタンを作り直します。[キャンセル](/unity-csharp-learning/csharp/task-cancellation/) で学んだ `CancellationTokenSource` と `CancellationToken` を使います。`PushButtonSignal` スクリプトを次のように書き換えます。

```csharp
using System;
using System.Threading;
using UnityEngine;

public class PushButtonSignal : MonoBehaviour
{
    [SerializeField] private float _guideInterval = 5f;
    [SerializeField] private int _blinkCount = 2;

    private Renderer _renderer;
    private bool _isRequested;
    private CancellationTokenSource _powerSource;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        PowerOn();
    }

    public void OnPushButton()
    {
        Debug.Log($"{Time.time:F2} 秒: 押しボタン");
        _isRequested = true;
    }

    public void OnPowerButton()
    {
        if (_powerSource != null)
        {
            _powerSource.Cancel();
        }
        else
        {
            PowerOn();
        }
    }

    private async void PowerOn()
    {
        _isRequested = false;
        _powerSource = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
        try
        {
            await RunSignal(_powerSource.Token);
        }
        catch (OperationCanceledException)
        {
            Debug.Log($"{Time.time:F2} 秒: 停止");
        }
        finally
        {
            _powerSource.Dispose();
            _powerSource = null;
        }
    }

    private async Awaitable RunSignal(CancellationToken token)
    {
        try
        {
            while (true)
            {
                SetColor(Color.red, "赤");

                while (!await WaitForPush(_guideInterval, token))
                {
                    Debug.Log($"{Time.time:F2} 秒: 案内「ボタンを押してください」");
                }
                _isRequested = false;

                await Awaitable.WaitForSecondsAsync(1f, token);

                SetColor(Color.blue, "青");
                await Awaitable.WaitForSecondsAsync(2f, token);

                try
                {
                    await Blink(_blinkCount, token);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Debug.LogWarning($"{Time.time:F2} 秒: 点滅できなかったので、1 秒待つ（{e.GetType().Name}）");
                    await Awaitable.WaitForSecondsAsync(1f, token);
                }
            }
        }
        finally
        {
            SetColor(Color.black, "消灯");
        }
    }

    private async Awaitable<bool> WaitForPush(float timeout, CancellationToken token)
    {
        float deadline = Time.time + timeout;
        while (!_isRequested && Time.time < deadline)
        {
            await Awaitable.NextFrameAsync(token);
        }
        return _isRequested;
    }

    private async Awaitable Blink(int count, CancellationToken token)
    {
        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "点滅の回数は 1 以上にしてください");
        }

        for (int i = 0; i < count; i++)
        {
            SetColor(Color.gray, "灰");
            await Awaitable.WaitForSecondsAsync(0.25f, token);

            SetColor(Color.blue, "青");
            await Awaitable.WaitForSecondsAsync(0.25f, token);
        }
    }

    private void SetColor(Color color, string label)
    {
        _renderer.material.color = color;
        Debug.Log($"{Time.time:F2} 秒: {label}");
    }
}
```

Play ボタンを押して、次の操作をします。

1. **PUSH** ボタンをクリックする
2. 点滅が始まったら、**POWER** ボタンをクリックする
3. 黒の間に **PUSH** ボタンをクリックしてから、**POWER** ボタンをクリックする
4. **PUSH** ボタンをクリックする

Console には次のように表示されます。これは実行結果の例です。

```
0.00 秒: 赤
1.00 秒: 押しボタン
2.04 秒: 青
4.06 秒: 灰
4.32 秒: 青
4.40 秒: 消灯
4.40 秒: 停止
4.90 秒: 押しボタン
5.40 秒: 赤
6.40 秒: 押しボタン
7.44 秒: 青
```

[コルーチンの制御](/unity-csharp-learning/unity/coroutine-control/) の電源のボタンと同じように動きます。ただし、止め方と後片付けのしかたが違います。

### トークンを渡して、止めるよう頼む

信号を動かす処理は、`PowerOn` メソッドにまとめました。`PowerOn` は、`CancellationTokenSource` を作り、そのトークンを `RunSignal` に渡します。`RunSignal` は、受け取ったトークンを、`WaitForPush`、`Blink`、`Awaitable.WaitForSecondsAsync`、`Awaitable.NextFrameAsync` に渡していきます。

**POWER** ボタンで `OnPowerButton` が呼ばれると、`_powerSource.Cancel()` でキャンセルを要求します。このとき待っていた `Awaitable.WaitForSecondsAsync` や `Awaitable.NextFrameAsync` は、`OperationCanceledException` を投げて待つのをやめます。この例外は、`Blink`、`RunSignal` と呼び出し元へ伝わり、`PowerOn` の `catch` で受け取られて「停止」と表示されます。

`PowerOn` は、`Start` と `OnPowerButton` から呼ばれ、戻り値を `await` する呼び出し元がないので、`async void` にしています。`finally` では、使い終わった `CancellationTokenSource` を `Dispose` し、`_powerSource` を `null` に戻します。`_powerSource` が `null` かどうかで、信号が動いているかを判断できます。

### 止められた処理の中で後片付けする

`RunSignal` の本体は、`try` / `finally` で囲んであります。キャンセルで `OperationCanceledException` が投げられると、`RunSignal` の `finally` が実行され、球体を黒にして「消灯」と表示します。Console で「消灯」が「停止」より先に表示されているのは、このためです。

[コルーチンの制御](/unity-csharp-learning/unity/coroutine-control/) では、`StopCoroutine` で止めたコルーチンの中の `finally` は実行されないので、止めた側の `OnPowerButton` で色を決め直しました。async メソッドでは、止められる側が自分で後片付けを書けます。止める側は「止めてほしい」と頼むだけで、何が途中だったのかを知る必要はありません。

> 💡 **ポイント**: `Awaitable.WaitForSecondsAsync` などにトークンを渡さなかった場合、`Cancel` を呼んでも、待っている処理は止まりません。止めたい処理には、すべての `await` にトークンを渡していく必要があります。

---

## 5. ゲームオブジェクトとの関係

`PowerOn` で `CancellationTokenSource` を作るときに使った `CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken)` は、ゲームオブジェクトが破棄されたときにも処理を止めるためのものです。

### async メソッドは、ゲームオブジェクトに結び付いていない

コルーチンは、`StartCoroutine` を呼んだゲームオブジェクトに結び付いていて、ゲームオブジェクトを非アクティブにしたり破棄したりすると止まりました。async メソッドは、ゲームオブジェクトとは関係なく動きます。

3 節のスクリプト（キャンセルのない版）で確かめると、次のようになります。

- Play 中に `Signal` を非アクティブにしても、`RunSignal` は動き続け、Console には点滅や赤の表示が続く
- Play 中に `Signal` を破棄（`Destroy`）しても、`RunSignal` は動き続ける。次に `SetColor` が呼ばれたとき、破棄された `Renderer` を使おうとして、次の例外が表示される

```
MissingReferenceException: The object of type 'UnityEngine.MeshRenderer' has been destroyed but you are still trying to access it.
```

### destroyCancellationToken

**`MonoBehaviour.destroyCancellationToken`** — その `MonoBehaviour` が破棄されたときにキャンセルされるトークンを取得します。

**書式：[MonoBehaviour.destroyCancellationToken プロパティ](https://docs.unity3d.com/ScriptReference/MonoBehaviour-destroyCancellationToken.html)**
```csharp
public CancellationToken destroyCancellationToken { get; }
```

`destroyCancellationToken` を async メソッドに渡しておくと、ゲームオブジェクトやスクリプトが破棄されたときに、待っている処理が `OperationCanceledException` で止まります。Play を止めたときも、シーンのゲームオブジェクトが破棄されるので、同じように止まります。

このスクリプトでは、**POWER** ボタンと破棄の、どちらでも止めたいので、2 つを 1 つのトークンにまとめています。

**`CancellationTokenSource.CreateLinkedTokenSource()`** — 指定したトークンのどれかがキャンセルされると、自分もキャンセルされる `CancellationTokenSource` を作ります。

**書式：[CancellationTokenSource.CreateLinkedTokenSource メソッド](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtokensource.createlinkedtokensource)**
```csharp
public static CancellationTokenSource CreateLinkedTokenSource(CancellationToken token);
```

| パラメータ | 説明 |
|---|---|
| `token` | つなげるトークン。このトークンがキャンセルされると、作られた `CancellationTokenSource` もキャンセルされる |

`_powerSource` は、`_powerSource.Cancel()` を呼んだときにも、`destroyCancellationToken` がキャンセルされたときにもキャンセルされます。4 節のスクリプトで `Signal` を Play 中に破棄すると、例外は表示されず、`RunSignal` の `finally` と `PowerOn` の `catch` が実行されて、Console に「消灯」と「停止」が表示されます。

---

## 動作確認

1. 1 節のシーンを開き、`PushButtonSignal` スクリプトを 4 節のコードに書き換える
2. `Signal` の `PushButtonSignal` の `Blink Count` が `2` になっていることを確かめる
3. Play ボタンを押し、4 節の 1〜4 の操作をする
4. Play を止める

3 では、球体の色と Console の表示が、4 節の実行結果の例と同じ形になることを確認してください。

4 で Play を止めると、`destroyCancellationToken` がキャンセルされるので、Console に次の 2 行が表示されます。Play を止めた後の `Time.time` は `0` になります。

```
0.00 秒: 消灯
0.00 秒: 停止
```

---

## よくあるミス

### Unity から呼ばれるメソッドを async Awaitable にする

`Start` を `async void` ではなく `async Awaitable` にしても、コンパイルエラーにはならず、Unity から呼ばれて動きます。

```csharp
// ❌ NG: Start の中で例外が起きても、Console に何も表示されない
private async Awaitable Start()
{
    _renderer = GetComponent<Renderer>();
    await RunSignal();
}
```

しかし、2 節のスクリプトの `Start` をこのように書き換え、`Blink Count` を `0` にして試すと、青になった後に信号が止まるのに、Console には例外が表示されません。`Start` が返した `Awaitable` を受け取って `await` する呼び出し元がないので、`Awaitable` に記録された例外を誰も取り出さないためです。`async Awaitable` のメソッドを呼び出して、戻り値を `await` せずに捨てたときも、同じように例外が表示されません。

`Start` やボタンのハンドラーのように、Unity から呼ばれて結果を誰も待たないメソッドは、`async void` にします。`async void` のメソッドで受け取られなかった例外は、Console に表示されます。そこから呼ぶメソッドは `async Awaitable` にして、必ず `await` します。

---

## ワンポイントアドバイス

### Awaitable と Task の違い

Unity の async メソッドでは、`Task` も使えます。しかし、Unity のドキュメントでは、Unity のコードでは基本的に `Awaitable` を使うことが勧められています。主な違いは次のとおりです。

- `Awaitable.NextFrameAsync` や `Awaitable.WaitForSecondsAsync` のように、フレームやゲーム時間に合わせて待つメソッドがある
- `Awaitable` のオブジェクトは、Unity が使い回しています（プール）。そのため、1 つの `Awaitable` を 2 回以上 `await` してはいけません。[ValueTask](/unity-csharp-learning/csharp/value-task/) と同じ制約です
- `await` の後の続きを実行するタイミングが違う
  - `Task` の `await` は、続きを同期コンテキストに渡します（[await の前後で実行されるスレッド](/unity-csharp-learning/csharp/await-threads/) を参照）。Unity はメインスレッドに同期コンテキストを設定しているので、メインスレッドで `await` した `Task` の続きも、メインスレッドで実行されます。ただし、続きが実行されるのは、次のフレームの `Update` のときまで遅れます。また、`ConfigureAwait(false)` を付けると、続きはスレッドプールで実行され、その中ではほとんどの Unity の API を使えなくなります
  - `Awaitable` の `await` は、待っている処理が完了したその場で、同じフレームのうちに続きを実行します。どのスレッドで続きを実行するかは、`Awaitable.BackgroundThreadAsync` と `Awaitable.MainThreadAsync` を `await` して、明示的に切り替えられます

詳しくは、Unity のマニュアルの [Asynchronous programming with the Awaitable class](https://docs.unity3d.com/Manual/async-awaitable-introduction.html) を参照してください。

---

## まとめ

- `async Awaitable` のメソッドでは、`await Awaitable.NextFrameAsync()` で次のフレームまで、`await Awaitable.WaitForSecondsAsync(秒数)` で指定した秒数だけ待てる
- `Awaitable<TResult>` を返すメソッドでは、結果を `return` で返し、呼び出し元は `await` の値として受け取れる
- `await` した処理の例外は呼び出し元に伝わり、`try` / `catch` で受け取れる
- `CancellationToken` を渡しておくと、`Cancel` で待っている処理が `OperationCanceledException` で止まり、止められた処理の中の `finally` も実行される
- async メソッドはゲームオブジェクトの非アクティブ化や破棄では止まらない。`destroyCancellationToken` を渡して、破棄されたときに止める
- Unity から呼ばれるメソッドは `async void` にし、そこから呼ぶメソッドは `async Awaitable` にして `await` する

---

## 理解度チェック

1. コルーチンの版の `WaitForPush` と、`Awaitable` の版の `WaitForPush` では、押されたかどうかの受け取り方がどう違いますか？
2. 次のスクリプトをゲームオブジェクトにアタッチして Play ボタンを押すと、Console にどの順で表示されますか？

   ```csharp
   using System;
   using UnityEngine;

   public class AwaitSample : MonoBehaviour
   {
       private async void Start()
       {
           try
           {
               Debug.Log("A");
               await Fail();
               Debug.Log("C");
           }
           catch (InvalidOperationException e)
           {
               Debug.Log($"D: {e.Message}");
           }
           finally
           {
               Debug.Log("E");
           }
       }

       private async Awaitable Fail()
       {
           Debug.Log("B");
           await Awaitable.NextFrameAsync();
           throw new InvalidOperationException("失敗");
       }
   }
   ```

3. async メソッドで `destroyCancellationToken` を使わないと、ゲームオブジェクトを破棄したときに何が起きますか？
4. （応用）4 節のスクリプトの `WaitForPush` を、押されたときは待ち始めてから押されるまでの秒数を、時間切れのときは `-1` を返す `Awaitable<float>` のメソッドに変更してください。`RunSignal` では、押されたときに「○○ 秒で押された」と Console に表示します。

<details markdown="1">
<summary>解答を見る</summary>

1. コルーチンの版では、`Action<bool>` のコールバックを引数で渡し、ラムダ式の中でローカル変数に代入して受け取りました。`Awaitable` の版では、`WaitForPush` が `return` で返した値を、`await WaitForPush(...)` の値として受け取れます。
2. `A`、`B`、`D: 失敗`、`E` の順に表示されます。`Fail` で起きた例外が `await Fail()` に伝わるので、`C` は表示されず、`catch` の `D: 失敗` が表示されます。最後に `finally` の `E` が表示されます。
3. async メソッドは、ゲームオブジェクトが破棄されても動き続けます。破棄された後で、破棄されたコンポーネント（この例では `Renderer`）を使おうとすると、`MissingReferenceException` が投げられます。
4. `WaitForPush` で待ち始めた時刻を覚えておき、押されたら経過時間を返します。`RunSignal` では、結果が `0` 未満の間、案内を表示して待ち直します。

   ```csharp
   using System;
   using System.Threading;
   using UnityEngine;

   public class PushButtonSignal : MonoBehaviour
   {
       [SerializeField] private float _guideInterval = 5f;
       [SerializeField] private int _blinkCount = 2;

       private Renderer _renderer;
       private bool _isRequested;
       private CancellationTokenSource _powerSource;

       private void Start()
       {
           _renderer = GetComponent<Renderer>();
           PowerOn();
       }

       public void OnPushButton()
       {
           Debug.Log($"{Time.time:F2} 秒: 押しボタン");
           _isRequested = true;
       }

       public void OnPowerButton()
       {
           if (_powerSource != null)
           {
               _powerSource.Cancel();
           }
           else
           {
               PowerOn();
           }
       }

       private async void PowerOn()
       {
           _isRequested = false;
           _powerSource = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
           try
           {
               await RunSignal(_powerSource.Token);
           }
           catch (OperationCanceledException)
           {
               Debug.Log($"{Time.time:F2} 秒: 停止");
           }
           finally
           {
               _powerSource.Dispose();
               _powerSource = null;
           }
       }

       private async Awaitable RunSignal(CancellationToken token)
       {
           try
           {
               while (true)
               {
                   SetColor(Color.red, "赤");

                   float waited = await WaitForPush(_guideInterval, token);
                   while (waited < 0f)
                   {
                       Debug.Log($"{Time.time:F2} 秒: 案内「ボタンを押してください」");
                       waited = await WaitForPush(_guideInterval, token);
                   }
                   Debug.Log($"{Time.time:F2} 秒: {waited:F2} 秒で押された");
                   _isRequested = false;

                   await Awaitable.WaitForSecondsAsync(1f, token);

                   SetColor(Color.blue, "青");
                   await Awaitable.WaitForSecondsAsync(2f, token);

                   try
                   {
                       await Blink(_blinkCount, token);
                   }
                   catch (ArgumentOutOfRangeException e)
                   {
                       Debug.LogWarning($"{Time.time:F2} 秒: 点滅できなかったので、1 秒待つ（{e.GetType().Name}）");
                       await Awaitable.WaitForSecondsAsync(1f, token);
                   }
               }
           }
           finally
           {
               SetColor(Color.black, "消灯");
           }
       }

       private async Awaitable<float> WaitForPush(float timeout, CancellationToken token)
       {
           float start = Time.time;
           float deadline = start + timeout;
           while (!_isRequested && Time.time < deadline)
           {
               await Awaitable.NextFrameAsync(token);
           }
           return _isRequested ? Time.time - start : -1f;
       }

       private async Awaitable Blink(int count, CancellationToken token)
       {
           if (count < 1)
           {
               throw new ArgumentOutOfRangeException(nameof(count), "点滅の回数は 1 以上にしてください");
           }

           for (int i = 0; i < count; i++)
           {
               SetColor(Color.gray, "灰");
               await Awaitable.WaitForSecondsAsync(0.25f, token);

               SetColor(Color.blue, "青");
               await Awaitable.WaitForSecondsAsync(0.25f, token);
           }
       }

       private void SetColor(Color color, string label)
       {
           _renderer.material.color = color;
           Debug.Log($"{Time.time:F2} 秒: {label}");
       }
   }
   ```

   Play ボタンを押して、案内が表示されてから約 1 秒後に **PUSH** ボタンをクリックすると、Console には次のように表示されます（実行結果の例）。

   ```
   0.00 秒: 赤
   5.02 秒: 案内「ボタンを押してください」
   6.00 秒: 押しボタン
   6.02 秒: 1.00 秒で押された
   7.04 秒: 青
   ```

</details>

---

## 次のステップ

[Unity から通信する](/unity-csharp-learning/networking/unity-webrequest/) では、`UnityWebRequest` でサーバーと通信し、応答をコルーチンと `await` の両方の書き方で待ちます。
