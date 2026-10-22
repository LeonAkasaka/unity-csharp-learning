---
layout: page
title: コルーチンでは書きにくいこと
permalink: /unity/coroutine-limits/
---

# コルーチンでは書きにくいこと

処理をいくつかのコルーチンに分けると、「終わったか」だけでなく「どんな結果になったか」も呼び出し元で判断したくなります。また、子が失敗したときには、呼び出し元で案内を出したり、次の処理を変えたりしたくなります。コルーチンの終了待ちで受け取れるものを確かめ、結果や失敗を扱う方法を考えます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- コルーチンから呼び出し元へ値を返せない理由を説明し、コールバックで結果を受け取れる
- コルーチンの中で例外が起きたとき、そのコルーチンと呼び出し元がどうなるかを説明できる
- `yield return` を `try` / `catch` で囲めないことを説明できる
- コルーチンを `MonoBehaviour` を継承していないクラスでは開始できないことを説明できる
- コルーチンで書きにくいことを、C# の `Task` と `async` / `await` の機能に対応付けられる

## 前提知識

- [コルーチンの開始と停止](/unity-csharp-learning/unity/coroutine-control/) を読んでいること
- [ラムダ式](/unity-csharp-learning/csharp/lambda/) と [デリゲートの変数渡しとコールバック](/unity-csharp-learning/csharp/delegate-callback/) を読んでいること
- [Task と Task\<T\>](/unity-csharp-learning/csharp/tasks/) と [async と await](/unity-csharp-learning/csharp/async-await/) を読んでいると、5 節の対応がわかりやすくなります

---

## 1. 結果は return で返せない

入力待ちを一定時間で終えると、「要求を受け取った」のか「時間切れになった」のかによって、その後の処理を変えたくなります。子の終了を待つだけでは、この違いはわかりません。待機の終了とは別に、判定結果を呼び出し元へ渡す必要があります。

空のゲームオブジェクトに、動作確認用の `SampleCoroutine` スクリプトを作成してアタッチします。まず、判定結果を `bool` で返そうとした場合を考えます。

通常のメソッドなら `return true;` と書けますが、イテレーターではコンパイルエラーになります。次は意図的な NG 例です。

```csharp
using System.Collections;
using UnityEngine;

public class SampleCoroutine : MonoBehaviour
{
    private IEnumerator Check()
    {
        yield return null;
        return true; // NG: CS1622
    }
}
```

```
error CS1622: Cannot return a value from an iterator. Use the yield return statement to return a value, or yield break to end the iteration.
```

`yield return` で返すものは、Unity が待ち方を決めるために受け取ります。親が子の処理結果を受け取るための戻り値にはなりません。

### コールバックで結果を受け取る

戻り値を結果に使えないため、結果が決まったときに呼び出してもらう処理を、引数で渡す方法があります。これがコールバックです。子の `Check` は、結果を渡すための処理 `onCompleted` を受け取り、結果が決まった時点で呼び出します。

```csharp
using System;
using System.Collections;
using UnityEngine;

public class SampleCoroutine : MonoBehaviour
{
    private IEnumerator Start()
    {
        bool result = false;
        yield return Check(value => result = value);
        Debug.Log($"結果: {result}");
    }

    private IEnumerator Check(Action<bool> onCompleted)
    {
        yield return null;
        onCompleted(true);
    }
}
```

```
結果: True
```

子は 1 フレーム待ってから `onCompleted(true)` を呼びます。親はラムダ式の中でローカル変数 `result` に代入し、子の終了後にその値を読みます。`yield return Check(...)` 自体が `bool` を返すわけではありません。

---

## 2. 子の失敗は親の catch に届かない

結果を受け取れれば、成功した場合の処理を分けられます。しかし、子が途中で例外を投げた場合にも、親が失敗を受け取って対応できるとは限りません。次の例では、子が 1 フレーム待ってから失敗するとき、親の続きと `Update` がどうなるかをログで追います。

```csharp
using System;
using System.Collections;
using UnityEngine;

public class SampleCoroutine : MonoBehaviour
{
    private IEnumerator Start()
    {
        Debug.Log("A");
        yield return Fail();
        Debug.Log("C");
    }

    private IEnumerator Fail()
    {
        Debug.Log("B");
        yield return null;
        throw new InvalidOperationException("失敗");
    }

    private void Update()
    {
        if (Time.frameCount <= 3)
        {
            Debug.Log($"Update: {Time.frameCount}");
        }
    }
}
```

Play ボタンを押すと、次の順に表示されます。

```
A
B
Update: 1
Update: 2
InvalidOperationException: 失敗
Update: 3
```

子の `Fail` で例外が起きると、親の `Start` も続きを実行されずに終わり、`C` は表示されません。`Update` はその後も呼ばれます。子を進めているのは Unity であり、通常のメソッド呼び出しのように、例外が親へ投げ直される仕組みではありません。

---

## 3. yield return を try / catch で囲めない

さらに、親の待機を `try` / `catch` で囲む書き方自体も、コンパイルエラーになります。次は意図的な NG 例です。

```csharp
using System;
using System.Collections;
using UnityEngine;

public class SampleCoroutine : MonoBehaviour
{
    private IEnumerator Start()
    {
        try
        {
            yield return Fail(); // NG: CS1626
        }
        catch (InvalidOperationException e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    private IEnumerator Fail()
    {
        yield return null;
        throw new InvalidOperationException("失敗");
    }
}
```

```
error CS1626: Cannot yield a value in the body of a try block with a catch clause
```

C# のイテレーターでは、`catch` 句のある `try` の中に `yield return` を書けません。結果と同じように、失敗をコールバックで渡す方法は使えます（理解度チェックの 4 で試します）。

---

## 4. MonoBehaviour の外では開始できない

待機する処理を、`MonoBehaviour` を継承しない普通のクラスに分けたいとします。

```csharp
using System.Collections;
using UnityEngine;

public class PushWaiter
{
    public void Begin()
    {
        StartCoroutine(Wait()); // ❌ NG: CS0103 のコンパイルエラーになる
    }

    private IEnumerator Wait()
    {
        yield return null;
    }
}
```

このように書くと、次のコンパイルエラーになります。

```
error CS0103: The name 'StartCoroutine' does not exist in the current context
```

`StartCoroutine` は `MonoBehaviour` のメソッドなので、`MonoBehaviour` を継承していないクラスでは呼べません。[コルーチンの開始と停止](/unity-csharp-learning/unity/coroutine-control/) で学んだように、コルーチンは `StartCoroutine` を呼んだスクリプトのゲームオブジェクトに結び付いて動きます。普通のクラスでコルーチンを使うには、どれかの `MonoBehaviour` を受け取って、その `StartCoroutine` を呼んでもらう必要があります。

---

## 5. Task と async / await との対応

このページと [コルーチンの開始と停止](/unity-csharp-learning/unity/coroutine-control/) で見てきた、コルーチンで書きにくいことを整理します。C# の `Task` と `async` / `await` では、それぞれ次のように書けます。

| やりたいこと | コルーチン | `Task` と `async` / `await` | C# のページ |
|---|---|---|---|
| 時間がたつまで待つ | `yield return new WaitForSeconds(秒数)` | `await Task.Delay(ミリ秒)` | [スレッドを使わずに待つ](/unity-csharp-learning/csharp/async-without-threads/) |
| 結果を返す | `return` で返せない。コールバックで渡す | `Task<TResult>` を返す async メソッドで `return` する | [Task と Task\<T\>](/unity-csharp-learning/csharp/tasks/)、[async と await](/unity-csharp-learning/csharp/async-await/) |
| 失敗を受け取る | 呼び出し元に伝わらない。`yield return` を `try` / `catch` で囲めない | 例外が `await` した側に伝わる。`await` を `try` / `catch` で囲める | [async と await](/unity-csharp-learning/csharp/async-await/) |
| 途中で止める | 外から `StopCoroutine` で打ち切る。中では気付けず、`finally` も実行されない | `CancellationToken` で止めるよう頼み、中で気付いて後片付けできる | [キャンセル](/unity-csharp-learning/csharp/task-cancellation/) |
| 複数の処理をすべて待つ | 先にすべて `StartCoroutine` で開始し、`yield return` で 1 つずつ待つ | `Task.WhenAll` | [複数の Task を待つ](/unity-csharp-learning/csharp/task-whenall/) |
| 動かす場所 | `MonoBehaviour` の `StartCoroutine` が必要 | どのクラスのメソッドでも書ける | — |

コルーチンも `async` / `await` も、「待っている間に処理を止めず、続きを後から実行する」という点は同じです。どちらも、待っている間にスレッドを 1 つ占有することはありません。違うのは、`async` / `await` では、結果の受け渡しと例外を、普通のメソッドと同じように `return` と `try` / `catch` で書けることです。

Unity には、`async` / `await` で、フレームや秒数を待つための [Awaitable](https://docs.unity3d.com/ScriptReference/Awaitable.html) という型が用意されています。[Awaitable と async / await](/unity-csharp-learning/unity/awaitable/) で、Space キーで青への切り替えを要求する信号機を `Awaitable` で書き直します。

---

## 6. 実例：信号機で結果と失敗を扱う

前のページの `Signal` と `CoroutineSignal` に適用します。Space キーで要求し、P キーで電源を切り替える入力処理はそのまま使います。

### 時間切れか、要求されたかを受け取る

5 秒たっても要求されなければ案内を表示し、もう一度待つようにします。`WaitForRequest` は「要求か時間切れまで待つ処理」で、結果の `bool` をコールバックで渡します。1 節の `Check` と同じ受け渡しです。

<details markdown="1">
<summary>案内を加えた信号機の全体コード</summary>

```csharp
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoroutineSignal : MonoBehaviour
{
    [SerializeField] private float _guideInterval = 5f; // 追加：案内するまでの秒数

    private Renderer _renderer;
    private bool _isRequested;
    private Coroutine _signalRoutine;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _signalRoutine = StartCoroutine(RunSignal());
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            RequestGreen();
        }

        if (keyboard.pKey.wasPressedThisFrame)
        {
            TogglePower();
        }
    }

    public void RequestGreen()
    {
        Debug.Log($"{Time.time:F2} 秒: 切り替え要求");
        _isRequested = true;
    }

    public void TogglePower()
    {
        if (_signalRoutine != null)
        {
            StopCoroutine(_signalRoutine);
            _signalRoutine = null;
            SetColor(Color.black, "消灯");
        }
        else
        {
            _isRequested = false;
            _signalRoutine = StartCoroutine(RunSignal());
        }
    }

    private IEnumerator RunSignal()
    {
        while (true)
        {
            SetColor(Color.red, "赤");

            // 変更：要求されるまで、_guideInterval 秒ごとに案内する
            bool wasRequested = false;
            while (!wasRequested)
            {
                yield return WaitForRequest(_guideInterval, result => wasRequested = result);
                if (!wasRequested)
                {
                    Debug.Log($"{Time.time:F2} 秒: 案内「Space キーを押してください」");
                }
            }
            _isRequested = false;

            yield return new WaitForSeconds(1f);

            SetColor(Color.blue, "青");
            yield return new WaitForSeconds(2f);

            yield return Blink(2);
        }
    }

    // 追加：切り替えが要求されるか、timeout 秒たつまで待ち、要求されたかどうかを onCompleted に渡す
    private IEnumerator WaitForRequest(float timeout, Action<bool> onCompleted)
    {
        float deadline = Time.time + timeout;
        yield return new WaitUntil(() => _isRequested || Time.time >= deadline);
        onCompleted(_isRequested);
    }

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

</details>

Play 中に Game ビューをクリックし、約 10 秒待ってから Space キーを押します。5 秒ごとに案内が出て、要求すると青・点滅・赤へ進みます。`RunSignal` のローカル変数 `wasRequested` が、コールバックの受け取り先です。

### 点滅の失敗を確かめる

さらに `Blink Count` を Inspector から変更できるようにします。1 未満なら `Blink` が例外を投げます。次が、この段階での全体コードです。

### 完成したコード

```csharp
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoroutineSignal : MonoBehaviour
{
    [SerializeField] private float _guideInterval = 5f;
    [SerializeField] private int _blinkCount = 2; // 追加：点滅の回数

    private Renderer _renderer;
    private bool _isRequested;
    private Coroutine _signalRoutine;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _signalRoutine = StartCoroutine(RunSignal());
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            RequestGreen();
        }

        if (keyboard.pKey.wasPressedThisFrame)
        {
            TogglePower();
        }
    }

    public void RequestGreen()
    {
        Debug.Log($"{Time.time:F2} 秒: 切り替え要求");
        _isRequested = true;
    }

    public void TogglePower()
    {
        if (_signalRoutine != null)
        {
            StopCoroutine(_signalRoutine);
            _signalRoutine = null;
            SetColor(Color.black, "消灯");
        }
        else
        {
            _isRequested = false;
            _signalRoutine = StartCoroutine(RunSignal());
        }
    }

    private IEnumerator RunSignal()
    {
        while (true)
        {
            SetColor(Color.red, "赤");

            bool wasRequested = false;
            while (!wasRequested)
            {
                yield return WaitForRequest(_guideInterval, result => wasRequested = result);
                if (!wasRequested)
                {
                    Debug.Log($"{Time.time:F2} 秒: 案内「Space キーを押してください」");
                }
            }
            _isRequested = false;

            yield return new WaitForSeconds(1f);

            SetColor(Color.blue, "青");
            yield return new WaitForSeconds(2f);

            yield return Blink(_blinkCount); // 変更：回数をフィールドから渡す
        }
    }

    private IEnumerator WaitForRequest(float timeout, Action<bool> onCompleted)
    {
        float deadline = Time.time + timeout;
        yield return new WaitUntil(() => _isRequested || Time.time >= deadline);
        onCompleted(_isRequested);
    }

    private IEnumerator Blink(int count)
    {
        // 追加：回数が 1 未満なら例外を投げる
        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "点滅の回数は 1 以上にしてください");
        }

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

`Blink Count` を `0` にして Play ボタンを押し、Game ビューで Space キーを押すと、青になって約 2 秒後に `ArgumentOutOfRangeException` が出ます。親の `RunSignal` も止まるため、青のまま変わりません。これは 2 節と同じ制約です。

入力処理は続いています。P キーを押して消灯し、もう一度押すと赤から開始し直します。スタックトレースには子の `Blink` が現れますが、通常のメソッド呼び出しのような `RunSignal` の呼び出し元まではたどれません。

---

## 動作確認

1. これまでのシーンを開き、`CoroutineSignal` スクリプトを 6 節の「完成したコード」に書き換える
2. Play ボタンを押し、10 秒ほど待ってから Space キーを押す
3. Play を止め、`Signal` の `CoroutineSignal` の `Blink Count` を `0` にする
4. Play ボタンを押し、Space キーを押す
5. P キーを 2 回押す

2 では、5 秒ごとに案内が表示され、Space キーを押すと青になり、点滅してから赤に戻ることを確認してください。Console の表示は、5 秒間隔の案内と、要求後の色の切り替えを示す形になります。

4 と 5 では、Console に次のように表示されます。これは実行結果の例です。秒数は、キーを押した時刻とフレームレートによって変わります。

```
0.00 秒: 赤
1.45 秒: 切り替え要求
2.49 秒: 青
ArgumentOutOfRangeException: 点滅の回数は 1 以上にしてください
Parameter name: count
5.45 秒: 消灯
5.55 秒: 赤
```

例外が表示された後、P キーを 2 回押すまで、球体は青のまま変わらないことを確認してください。確認が終わったら、`Blink Count` を `2` に戻します。

---

## まとめ

- `yield return` を含むメソッドは `return` で値を返せない（CS1622）。結果は、引数で渡したコールバックで受け取る
- コルーチンの中で例外が起きると、そのコルーチンも、`yield return` でその終わりを待っていた呼び出し元も終わる。例外は呼び出し元に伝わらない
- `catch` 句のある `try` ブロックの中に `yield return` を書けない（CS1626）
- `StartCoroutine` は `MonoBehaviour` のメソッドなので、`MonoBehaviour` を継承していないクラスではコルーチンを開始できない
- `Task` と `async` / `await` では、結果を `return` で返し、例外を `try` / `catch` で受け取り、`CancellationToken` で止めるよう頼める

---

## 理解度チェック

1. コルーチンにするメソッドで `return true;` と書けないのはなぜですか？
2. 次のスクリプトをゲームオブジェクトにアタッチして Play ボタンを押すと、Console にどのように表示されますか？`Update` の行も含めて答えてください。

   ```csharp
   using System;
   using System.Collections;
   using UnityEngine;

   public class FailSample : MonoBehaviour
   {
       private IEnumerator Start()
       {
           Debug.Log("A");
           yield return Fail();
           Debug.Log("C");
       }

       private IEnumerator Fail()
       {
           Debug.Log("B");
           yield return null;
           throw new InvalidOperationException("失敗");
       }

       private void Update()
       {
           if (Time.frameCount <= 3)
           {
               Debug.Log($"Update: {Time.frameCount}");
           }
       }
   }
   ```

3. 6 節の `Blink` の例外を `RunSignal` の側で受け取れない理由を、2・3 節の説明から 2 つ答えてください。
4. （応用）6 節の「完成したコード」の `Blink` を、回数が 1 未満のときは例外を投げずに、`Action<Exception>` 型のコールバックで失敗を知らせて終わるように変更してください。`RunSignal` では、失敗を受け取ったら警告を表示し、1 秒待ってから赤に戻るようにします。

<details markdown="1">
<summary>解答を見る</summary>

1. `yield return` を含むメソッドはイテレーターで、イテレーターでは `return` で値を返せないためです（CS1622）。`yield return` で返した値は、呼び出し元ではなく Unity が受け取り、待ち方の合図として使います。
2. 次のように表示されます。`Fail` の例外で `Fail` も `Start` も終わるので、`C` は表示されません。`Update` は例外の後も呼ばれ続けます。

   ```
   A
   B
   Update: 1
   Update: 2
   InvalidOperationException: 失敗
   Update: 3
   ```

3. 1 つは、`Blink` の例外が `RunSignal` に伝わらず、`RunSignal` も一緒に終わってしまうためです。もう 1 つは、`catch` 句のある `try` ブロックの中に `yield return` を書けない（CS1626）ので、`yield return Blink(...)` を `try` / `catch` で囲めないためです。
4. `Blink` に `Action<Exception>` 型の引数を加え、回数が 1 未満のときは `onError` を呼んでから `yield break` で終わります。`RunSignal` では、6 節の `wasRequested` と同じように、ラムダ式で失敗をローカル変数に受け取ります。

   ```csharp
   using System;
   using System.Collections;
   using UnityEngine;
   using UnityEngine.InputSystem;

   public class CoroutineSignal : MonoBehaviour
   {
       [SerializeField] private float _guideInterval = 5f;
       [SerializeField] private int _blinkCount = 2;

       private Renderer _renderer;
       private bool _isRequested;
       private Coroutine _signalRoutine;

       private void Start()
       {
           _renderer = GetComponent<Renderer>();
           _signalRoutine = StartCoroutine(RunSignal());
       }

       private void Update()
       {
           var keyboard = Keyboard.current;
           if (keyboard == null) return;

           if (keyboard.spaceKey.wasPressedThisFrame)
           {
               RequestGreen();
           }

           if (keyboard.pKey.wasPressedThisFrame)
           {
               TogglePower();
           }
       }

       public void RequestGreen()
       {
           Debug.Log($"{Time.time:F2} 秒: 切り替え要求");
           _isRequested = true;
       }

       public void TogglePower()
       {
           if (_signalRoutine != null)
           {
               StopCoroutine(_signalRoutine);
               _signalRoutine = null;
               SetColor(Color.black, "消灯");
           }
           else
           {
               _isRequested = false;
               _signalRoutine = StartCoroutine(RunSignal());
           }
       }

       private IEnumerator RunSignal()
       {
           while (true)
           {
               SetColor(Color.red, "赤");

               bool wasRequested = false;
               while (!wasRequested)
               {
                   yield return WaitForRequest(_guideInterval, result => wasRequested = result);
                   if (!wasRequested)
                   {
                       Debug.Log($"{Time.time:F2} 秒: 案内「Space キーを押してください」");
                   }
               }
               _isRequested = false;

               yield return new WaitForSeconds(1f);

               SetColor(Color.blue, "青");
               yield return new WaitForSeconds(2f);

               // 変更：失敗をコールバックで受け取る
               Exception blinkError = null;
               yield return Blink(_blinkCount, e => blinkError = e);
               if (blinkError != null)
               {
                   Debug.LogWarning($"{Time.time:F2} 秒: 点滅できなかったので、1 秒待つ（{blinkError.GetType().Name}）");
                   yield return new WaitForSeconds(1f);
               }
           }
       }

       private IEnumerator WaitForRequest(float timeout, Action<bool> onCompleted)
       {
           float deadline = Time.time + timeout;
           yield return new WaitUntil(() => _isRequested || Time.time >= deadline);
           onCompleted(_isRequested);
       }

       // 変更：失敗を onError で知らせる
       private IEnumerator Blink(int count, Action<Exception> onError)
       {
           if (count < 1)
           {
               onError(new ArgumentOutOfRangeException(nameof(count), "点滅の回数は 1 以上にしてください"));
               yield break;
           }

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

   `Blink Count` を `0` にして Space キーを押すと、Console には次のように表示されます（実行結果の例）。例外で止まらずに、赤に戻ります。

   ```
   0.00 秒: 赤
   1.02 秒: 切り替え要求
   2.06 秒: 青
   4.10 秒: 点滅できなかったので、1 秒待つ（ArgumentOutOfRangeException）
   5.12 秒: 赤
   ```

   失敗を受け取れるようにはなりましたが、結果のコールバックと失敗のコールバックで、ローカル変数とラムダ式が 1 組ずつ増えました。`async` / `await` なら、`try` / `catch` で囲むだけで済みます。

</details>

---

## 次のステップ

[Awaitable と async / await](/unity-csharp-learning/unity/awaitable/) では、Space キーで青への切り替えを要求する信号機を Unity の `Awaitable` で書き直し、結果を `return` で返し、例外を `try` / `catch` で受け取り、`CancellationToken` で止める方法を学びます。
