---
layout: page
title: コルーチンでは書きにくいこと
permalink: /unity/coroutine-limits/
---

# コルーチンでは書きにくいこと

コルーチンを使うと、時間をまたぐ処理を上から順に書けます。しかし、普通のメソッドなら当たり前にできる「結果を呼び出し元に返す」「失敗を `try` / `catch` で受け取る」は、コルーチンではできません。このページでは、[コルーチンの制御](/unity-csharp-learning/unity/coroutine-control/) で作った押しボタン式の信号機に機能を加えながら、コルーチンのどこで行き詰まるのかを確かめます。最後に、C# の `Task` と `async` / `await` が、それぞれをどう解決しているのかを整理します。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- コルーチンから呼び出し元へ値を返せない理由を説明し、コールバックで結果を受け取れる
- コルーチンの中で例外が起きたとき、そのコルーチンと呼び出し元がどうなるかを説明できる
- `yield return` を `try` / `catch` で囲めないことを説明できる
- コルーチンを `MonoBehaviour` を継承していないクラスでは開始できないことを説明できる
- コルーチンで書きにくいことを、C# の `Task` と `async` / `await` の機能に対応付けられる

## 前提知識

- [コルーチンの制御](/unity-csharp-learning/unity/coroutine-control/) を読んでいること
- [ラムダ式](/unity-csharp-learning/csharp/lambda/) と [デリゲートの変数渡しとコールバック](/unity-csharp-learning/csharp/delegate-callback/) を読んでいること
- [Task と Task\<T\>](/unity-csharp-learning/csharp/tasks/) と [async と await](/unity-csharp-learning/csharp/async-await/) を読んでいると、5 節の対応がわかりやすくなります

---

## 1. シーンを準備する

[コルーチンの制御](/unity-csharp-learning/unity/coroutine-control/) の「動作確認」で作ったシーンを、そのまま使います。

- 球体の `Signal` に、`PushButtonSignal` スクリプトがアタッチされている
- `PushButton`（**PUSH**）の **On Click ()** に **PushButtonSignal → OnPushButton ()** が設定されている
- `PowerButton`（**POWER**）の **On Click ()** に **PushButtonSignal → OnPowerButton ()** が設定されている

このページでは、`PushButtonSignal` スクリプトを書き換えながら進めます。`OnPushButton` と `OnPowerButton` のメソッドは残すので、ボタンの設定は変更しません。

---

## 2. 結果を返したい

押しボタン式の信号機に、ボタンがなかなか押されないときの案内を加えます。赤になってから 5 秒たっても押されなければ、Console に「ボタンを押してください」と表示し、また 5 秒待ちます。

「ボタンが押されるか、指定した秒数がたつまで待つ」部分は、`WaitForPush` という別のコルーチンに分けることにします。待ち終わったときに、呼び出し元の `RunSignal` は「押されたのか、時間切れなのか」を知る必要があります。普通のメソッドなら、`bool` を返せば済みます。

```csharp
// ❌ NG: CS1622 のコンパイルエラーになる
private IEnumerator WaitForPush(float timeout)
{
    float deadline = Time.time + timeout;
    yield return new WaitUntil(() => _isRequested || Time.time >= deadline);
    return _isRequested;
}
```

このように書くと、次のコンパイルエラーになります。

```
error CS1622: Cannot return a value from an iterator. Use the yield return statement to return a value, or yield break to end the iteration.
```

`yield return` を含むメソッドはイテレーターなので、`return` で値を返せません。`yield return` で返した値は Unity が受け取り、「どう待つか」の合図として使います。呼び出し元の `RunSignal` には届きません。呼び出し元の `yield return WaitForPush(...)` も、`WaitForPush` が終わるまで待つだけで、値を受け取る書き方はありません。

### コールバックで結果を受け取る

コルーチンから結果を受け取るには、結果を渡すためのメソッド（コールバック）を引数で渡します。`PushButtonSignal` スクリプトを次のように書き換えます。

```csharp
using System;
using System.Collections;
using UnityEngine;

public class PushButtonSignal : MonoBehaviour
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

    public void OnPushButton()
    {
        Debug.Log($"{Time.time:F2} 秒: 押しボタン");
        _isRequested = true;
    }

    public void OnPowerButton()
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

            // 変更：押されるまで、_guideInterval 秒ごとに案内する
            bool isPushed = false;
            while (!isPushed)
            {
                yield return WaitForPush(_guideInterval, result => isPushed = result);
                if (!isPushed)
                {
                    Debug.Log($"{Time.time:F2} 秒: 案内「ボタンを押してください」");
                }
            }
            _isRequested = false;

            yield return new WaitForSeconds(1f);

            SetColor(Color.blue, "青");
            yield return new WaitForSeconds(2f);

            yield return Blink(2);
        }
    }

    // 追加：ボタンが押されるか、timeout 秒たつまで待ち、押されたかどうかを onCompleted に渡す
    private IEnumerator WaitForPush(float timeout, Action<bool> onCompleted)
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

`Action<bool>` を使うので、`using System;` を追加しています。

`WaitForPush` は、最後に `onCompleted(_isRequested)` を呼んで、押されたかどうかを渡します。`RunSignal` は、`result => isPushed = result` というラムダ式を渡しています。このラムダ式は、受け取った結果をローカル変数 `isPushed` に代入します。`yield return WaitForPush(...)` は `WaitForPush` が終わるまで待つので、次の行では `isPushed` に結果が入っています。

Play ボタンを押して、10 秒ほど待ってから **PUSH** ボタンをクリックすると、Console には次のように表示されます。これは実行結果の例です。秒数は、ボタンをクリックした時刻とフレームレートによって変わります。

```
0.00 秒: 赤
5.02 秒: 案内「ボタンを押してください」
10.04 秒: 案内「ボタンを押してください」
12.42 秒: 押しボタン
13.46 秒: 青
15.48 秒: 灰
15.74 秒: 青
16.00 秒: 灰
16.26 秒: 青
16.52 秒: 赤
```

コールバックを使えば、結果を受け取れます。ただし、`bool isPushed = WaitForPush(...)` のように戻り値として受け取るのではなく、ラムダ式の中で変数に代入するという回り道をしています。受け取りたい結果が増えるたびに、コールバックの引数やローカル変数も増えていきます。

---

## 3. 失敗を受け取りたい

点滅の回数を Inspector から変更できるようにします。回数が 1 未満のときは点滅できないので、`Blink` は例外を投げることにします。`PushButtonSignal` スクリプトを次のように書き換えます。

```csharp
using System;
using System.Collections;
using UnityEngine;

public class PushButtonSignal : MonoBehaviour
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

    public void OnPushButton()
    {
        Debug.Log($"{Time.time:F2} 秒: 押しボタン");
        _isRequested = true;
    }

    public void OnPowerButton()
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

            bool isPushed = false;
            while (!isPushed)
            {
                yield return WaitForPush(_guideInterval, result => isPushed = result);
                if (!isPushed)
                {
                    Debug.Log($"{Time.time:F2} 秒: 案内「ボタンを押してください」");
                }
            }
            _isRequested = false;

            yield return new WaitForSeconds(1f);

            SetColor(Color.blue, "青");
            yield return new WaitForSeconds(2f);

            yield return Blink(_blinkCount); // 変更：回数をフィールドから渡す
        }
    }

    private IEnumerator WaitForPush(float timeout, Action<bool> onCompleted)
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

`Signal` を選択し、Inspector ビューで `PushButtonSignal` の `Blink Count` を `0` にしてから、Play ボタンを押して **PUSH** ボタンをクリックします。青になって 2 秒後に、Console に例外が表示されます。これは実行結果の例です。

```
0.00 秒: 赤
1.00 秒: 押しボタン
2.04 秒: 青
ArgumentOutOfRangeException: 点滅の回数は 1 以上にしてください
Parameter name: count
```

例外が起きた後、球体は青のまま変わりません。**PUSH** ボタンをクリックしても、Console に「押しボタン」と表示されるだけで、信号は進みません。

### 例外で、呼び出し元のコルーチンも終わる

コルーチンの中で例外が投げられると、Unity はその例外を Console に表示し、そのコルーチンを終わらせます。`yield return Blink(...)` で `Blink` の終わりを待っていた `RunSignal` も、続きを実行されずに終わります。信号が進まなくなったのは、このためです。

一方、ほかのスクリプトやボタンの処理は動き続けます。`_signalRoutine` には終わった `RunSignal` のコルーチンが残っているので、**POWER** ボタンを 1 回クリックすると「消灯」になり、もう 1 回クリックすると最初の赤から動き出します。

Console で例外の行を選択すると、例外が起きた場所（スタックトレース）が表示されます。先頭の行は次のようになっていて、`Blink` の中で起きたことはわかります。これは表示の例です。`d__10` の数字や行番号は、スクリプトの書き方や環境によって変わります。行番号が、例外を投げた行ではなく、メソッドの終わりの `}` の行を指すこともあります。

```
PushButtonSignal+<Blink>d__10.MoveNext () (at Assets/PushButtonSignal.cs:90)
```

しかし、`Blink` を呼び出した `RunSignal` は表示されません。`Blink` を進めているのは Unity なので、普通のメソッドの呼び出しのように「どこから呼ばれたか」をたどれないのです。

### yield return は try / catch で囲めない

点滅に失敗したら、警告を表示して先へ進めたいとします。普通のメソッドなら、呼び出しを `try` / `catch` で囲めば済みます。

```csharp
// ❌ NG: CS1626 のコンパイルエラーになる
try
{
    yield return Blink(_blinkCount);
}
catch (ArgumentOutOfRangeException e)
{
    Debug.LogWarning(e.Message);
}
```

このように書くと、次のコンパイルエラーになります。

```
error CS1626: Cannot yield a value in the body of a try block with a catch clause
```

C# のイテレーターでは、`catch` 句のある `try` ブロックの中に `yield return` を書けません。そもそも、`Blink` の例外は `RunSignal` に伝わらず、`RunSignal` ごと終わってしまうので、`RunSignal` の側では受け取りようがありません。

コルーチンで失敗を呼び出し元に知らせるには、結果と同じように、コールバックで渡すしかありません（理解度チェックの 4 で試します）。

---

## 4. MonoBehaviour の外では開始できない

ボタンを待つ処理を、`MonoBehaviour` を継承しない普通のクラスに分けたいとします。

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

`StartCoroutine` は `MonoBehaviour` のメソッドなので、`MonoBehaviour` を継承していないクラスでは呼べません。[コルーチンの制御](/unity-csharp-learning/unity/coroutine-control/) で学んだように、コルーチンは `StartCoroutine` を呼んだスクリプトのゲームオブジェクトに結び付いて動きます。普通のクラスでコルーチンを使うには、どれかの `MonoBehaviour` を受け取って、その `StartCoroutine` を呼んでもらう必要があります。

---

## 5. Task と async / await との対応

このページと [コルーチンの制御](/unity-csharp-learning/unity/coroutine-control/) で見てきた、コルーチンで書きにくいことを整理します。C# の `Task` と `async` / `await` では、それぞれ次のように書けます。

| やりたいこと | コルーチン | `Task` と `async` / `await` | C# のページ |
|---|---|---|---|
| 時間がたつまで待つ | `yield return new WaitForSeconds(秒数)` | `await Task.Delay(ミリ秒)` | [スレッドを使わずに待つ](/unity-csharp-learning/csharp/async-without-threads/) |
| 結果を返す | `return` で返せない。コールバックで渡す | `Task<TResult>` を返す async メソッドで `return` する | [Task と Task\<T\>](/unity-csharp-learning/csharp/tasks/)、[async と await](/unity-csharp-learning/csharp/async-await/) |
| 失敗を受け取る | 呼び出し元に伝わらない。`yield return` を `try` / `catch` で囲めない | 例外が `await` した側に伝わる。`await` を `try` / `catch` で囲める | [async と await](/unity-csharp-learning/csharp/async-await/) |
| 途中で止める | 外から `StopCoroutine` で打ち切る。中では気付けず、`finally` も実行されない | `CancellationToken` で止めるよう頼み、中で気付いて後片付けできる | [キャンセル](/unity-csharp-learning/csharp/task-cancellation/) |
| 複数の処理をすべて待つ | 先にすべて `StartCoroutine` で開始し、`yield return` で 1 つずつ待つ | `Task.WhenAll` | [複数の Task を待つ](/unity-csharp-learning/csharp/task-whenall/) |
| 動かす場所 | `MonoBehaviour` の `StartCoroutine` が必要 | どのクラスのメソッドでも書ける | — |

コルーチンも `async` / `await` も、「待っている間に処理を止めず、続きを後から実行する」という点は同じです。どちらも、待っている間にスレッドを 1 つ占有することはありません。違うのは、`async` / `await` では、結果の受け渡しと例外を、普通のメソッドと同じように `return` と `try` / `catch` で書けることです。

Unity には、`async` / `await` で、フレームや秒数を待つための [Awaitable](https://docs.unity3d.com/ScriptReference/Awaitable.html) という型が用意されています。[Awaitable と async / await](/unity-csharp-learning/unity/awaitable/) で、押しボタン式の信号機を `Awaitable` で書き直します。

---

## 動作確認

1. 1 節のシーンを開き、`PushButtonSignal` スクリプトを 3 節のコードに書き換える
2. Play ボタンを押し、10 秒ほど待ってから **PUSH** ボタンをクリックする
3. Play を止め、`Signal` の `PushButtonSignal` の `Blink Count` を `0` にする
4. Play ボタンを押し、**PUSH** ボタンをクリックする
5. **POWER** ボタンを 2 回クリックする

2 では、5 秒ごとに案内が表示され、ボタンをクリックすると青になり、点滅してから赤に戻ることを確認してください。Console の表示は、2 節の実行結果の例と同じ形になります。

4 と 5 では、Console に次のように表示されます。これは実行結果の例です。秒数は、ボタンをクリックした時刻とフレームレートによって変わります。

```
0.00 秒: 赤
1.45 秒: 押しボタン
2.49 秒: 青
ArgumentOutOfRangeException: 点滅の回数は 1 以上にしてください
Parameter name: count
5.45 秒: 消灯
5.55 秒: 赤
```

例外が表示された後、**POWER** ボタンを 2 回クリックするまで、球体は青のまま変わらないことを確認してください。確認が終わったら、`Blink Count` を `2` に戻します。

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

3. 3 節で、`Blink` の例外を `RunSignal` の側で受け取れないのはなぜですか？理由を 2 つ答えてください。
4. （応用）3 節のスクリプトの `Blink` を、回数が 1 未満のときは例外を投げずに、`Action<Exception>` 型のコールバックで失敗を知らせて終わるように変更してください。`RunSignal` では、失敗を受け取ったら警告を表示し、1 秒待ってから赤に戻るようにします。

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
4. `Blink` に `Action<Exception>` 型の引数を加え、回数が 1 未満のときは `onError` を呼んでから `yield break` で終わります。`RunSignal` では、2 節の `isPushed` と同じように、ラムダ式で失敗をローカル変数に受け取ります。

   ```csharp
   using System;
   using System.Collections;
   using UnityEngine;

   public class PushButtonSignal : MonoBehaviour
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

       public void OnPushButton()
       {
           Debug.Log($"{Time.time:F2} 秒: 押しボタン");
           _isRequested = true;
       }

       public void OnPowerButton()
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

               bool isPushed = false;
               while (!isPushed)
               {
                   yield return WaitForPush(_guideInterval, result => isPushed = result);
                   if (!isPushed)
                   {
                       Debug.Log($"{Time.time:F2} 秒: 案内「ボタンを押してください」");
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

       private IEnumerator WaitForPush(float timeout, Action<bool> onCompleted)
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

   `Blink Count` を `0` にして **PUSH** ボタンをクリックすると、Console には次のように表示されます（実行結果の例）。例外で止まらずに、赤に戻ります。

   ```
   0.00 秒: 赤
   1.02 秒: 押しボタン
   2.06 秒: 青
   4.10 秒: 点滅できなかったので、1 秒待つ（ArgumentOutOfRangeException）
   5.12 秒: 赤
   ```

   失敗を受け取れるようにはなりましたが、結果のコールバックと失敗のコールバックで、ローカル変数とラムダ式が 1 組ずつ増えました。`async` / `await` なら、`try` / `catch` で囲むだけで済みます。

</details>

---

## 次のステップ

[Awaitable と async / await](/unity-csharp-learning/unity/awaitable/) では、押しボタン式の信号機を Unity の `Awaitable` で書き直し、結果を `return` で返し、例外を `try` / `catch` で受け取り、`CancellationToken` で止める方法を学びます。
