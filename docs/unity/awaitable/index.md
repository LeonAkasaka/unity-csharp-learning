---
layout: page
title: Awaitable と async / await
permalink: /unity/awaitable/
---

# Awaitable と async / await

前のページでは、コルーチンから結果を渡すためにコールバックを使い、子の例外を親で受け取れないことも確認しました。待機を含む処理でも、通常のメソッドのように `return` で結果を返し、`try` / `catch` で失敗に対応したいことがあります。Unity の `Awaitable` は、フレームや秒数の待機を C# の `async` / `await` で扱うための型です。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `async Awaitable` のメソッドを書き、`Awaitable.NextFrameAsync` と `Awaitable.WaitForSecondsAsync` でフレームや秒数を待てる
- `Awaitable<TResult>` を返すメソッドで、結果を `return` で返せる
- 結果の再利用と完了の共有を区別し、同じ `Awaitable` を二度待たずに扱える
- `await` した処理の例外を、`try` / `catch` で受け取れる
- `CancellationToken` で処理を止め、止められた処理の中の `finally` で後片付けできる
- async メソッドがゲームオブジェクトの破棄で止まらないことを説明し、`destroyCancellationToken` で止められる
- Unity から呼ばれるメソッドを `async void` にする理由を説明できる

## 前提知識

- [コルーチンでは書きにくいこと](/unity-csharp-learning/unity/coroutine-limits/) を読んでいること
- [async と await](/unity-csharp-learning/csharp/async-await/) を読んでいること
- [キャンセル](/unity-csharp-learning/csharp/task-cancellation/) を読んでいること

---

## 1. await でフレームと秒数を待つ

`async` / `await` で書く場合も、待機中に Unity のフレームを止めず、待機が終わってから続きを実行する必要があります。Unity が用意したフレーム待ちや秒数待ちを `await` することで、この流れを書けます。

`Awaitable.NextFrameAsync` は次のフレームを、`Awaitable.WaitForSecondsAsync` はゲーム時間の秒数を待ちます。コルーチンの `yield return` に対応する待機を、`await` で書きます。

空のゲームオブジェクトに、動作確認用の `SampleAwaitable` スクリプトを作成してアタッチします。

**書式：[Awaitable.NextFrameAsync メソッド](https://docs.unity3d.com/ScriptReference/Awaitable.NextFrameAsync.html)**

```csharp
public static Awaitable NextFrameAsync(CancellationToken cancellationToken = default);
```

**書式：[Awaitable.WaitForSecondsAsync メソッド](https://docs.unity3d.com/ScriptReference/Awaitable.WaitForSecondsAsync.html)**

```csharp
public static Awaitable WaitForSecondsAsync(float seconds, CancellationToken cancellationToken = default);
```

| パラメータ | 説明 |
|---|---|
| `seconds` | 待つゲーム時間の秒数 |
| `cancellationToken` | キャンセルを伝えるトークン。4 節で使う。省略もできる |

```csharp
using UnityEngine;

public class SampleAwaitable : MonoBehaviour
{
    private async void Start()
    {
        Debug.Log("開始");
        await Awaitable.NextFrameAsync();
        Debug.Log("次のフレーム");
        await Awaitable.WaitForSecondsAsync(1f);
        Debug.Log("約 1 秒後");
    }
}
```

```
開始
次のフレーム
約 1 秒後
```

`async` メソッドは呼び出すと実行を始めます。`await` は待機が終わるまでそのメソッドを中断し、終わると次の行から再開します。Unity から呼ばれる `Start` は、戻り値を `await` する相手がいないため `async void` にしています。そこから呼んで待つメソッドには、次の節のように `async Awaitable` を使います。

### 物理更新後・描画後まで待つ

フレーム内の再開段階を選ぶ必要がある場合は、次のメソッドも使えます。[コルーチンの待機方法](/unity-csharp-learning/unity/coroutine-waits/#フレーム内の再開タイミングを選ぶ)で扱った待機に対応します。

**書式：[Awaitable.FixedUpdateAsync メソッド](https://docs.unity3d.com/ScriptReference/Awaitable.FixedUpdateAsync.html)**

```csharp
public static Awaitable FixedUpdateAsync(CancellationToken cancellationToken = default);
```

**書式：[Awaitable.EndOfFrameAsync メソッド](https://docs.unity3d.com/ScriptReference/Awaitable.EndOfFrameAsync.html)**

```csharp
public static Awaitable EndOfFrameAsync(CancellationToken cancellationToken = default);
```

どちらもメインスレッドから呼びます。`FixedUpdateAsync` は次の物理更新まで、`EndOfFrameAsync` はフレーム内の描画を含む処理が終わるまで待ちます。キャンセル用の引数は、先ほどの待機と同様に省略できます。

```csharp
using UnityEngine;

public class SampleAwaitable : MonoBehaviour
{
    private async void Start()
    {
        Debug.Log("開始");
        await Awaitable.FixedUpdateAsync();
        Debug.Log("物理更新後");
        await Awaitable.EndOfFrameAsync();
        Debug.Log("描画後");
    }
}
```

```
開始
物理更新後
描画後
```

このコードは Play 中に Game ビューを表示して確認します。`NextFrameAsync` と取り替えれば同じになるとは限らず、用途に合った再開段階を選ぶ必要があります。

---

## 2. 結果を return で返す

待機後の判定結果で処理を分けたい場合、コルーチンでは結果を受け取る変数とコールバックを用意しました。`Awaitable<bool>` を使うと、「待ってから `bool` の結果を受け取る」ことを、1 つの式で書けます。

```csharp
using UnityEngine;

public class SampleAwaitable : MonoBehaviour
{
    private async void Start()
    {
        bool result = await Check();
        Debug.Log($"結果: {result}");
    }

    private async Awaitable<bool> Check()
    {
        await Awaitable.NextFrameAsync();
        return true;
    }
}
```

```
結果: True
```

子の `Check` は `Awaitable<bool>` を返します。`return true;` で返した処理結果を、親が `await Check()` の値として受け取れます。コルーチンで使ったコールバックと、受け取り用のラムダ式は不要です。

### 戻り値の Awaitable と、受け取った結果を区別する

同じ結果を後でも使いたい場合は、`await` して受け取った値を保存します。**同じ `Awaitable` を保存し、二度 `await` することはできません。** Unity は `Awaitable` を内部のプールに返して再利用するためです。複数の呼び出し元で同じ `Awaitable` を待つ場合も、この制約は変わりません。

一方、メソッドをもう一度呼び、その回の戻り値を一度だけ待つことはできます。次の例では `Check` を別々に 2 回実行し、最初に得た `bool` の結果も再利用します。

```csharp
using UnityEngine;

public class SampleAwaitable : MonoBehaviour
{
    private async void Start()
    {
        bool first = await Check();
        bool second = await Check();
        Debug.Log($"別々の処理: {first}, {second}");
        Debug.Log($"結果を再利用: {first}");
    }

    private async Awaitable<bool> Check()
    {
        Debug.Log("Check 開始");
        await Awaitable.NextFrameAsync();
        return true;
    }
}
```

```
Check 開始
Check 開始
別々の処理: True, True
結果を再利用: True
```

「同じ結果を使う」と「同じ待機用オブジェクトを再び待つ」は違います。内部ではオブジェクトが再利用される場合がありますが、各呼び出しで得た `Awaitable` を待つのは一度ずつです。ループの各回で `Awaitable.NextFrameAsync()` を呼んで待つ書き方も、この使い方です。

### 一度だけ開始した処理の完了を共有したい場合

複数箇所で同じ処理の完了を待ちたい場合は、繰り返し待てる `Task` に結果を受け渡す方法があります。次の `ToTask` は `Awaitable<bool>` を一度だけ待ち、その結果を `Task<bool>` に渡します。これは自分で定義したメソッドであり、Unity の組み込みメソッドではありません。

```csharp
using System.Threading.Tasks;
using UnityEngine;

public class SampleAwaitable : MonoBehaviour
{
    private async void Start()
    {
        Task<bool> shared = ToTask(Check());
        Debug.Log($"共有 1: {await shared}");
        Debug.Log($"共有 2: {await shared}");
    }

    private async Task<bool> ToTask(Awaitable<bool> operation)
    {
        return await operation;
    }

    private async Awaitable<bool> Check()
    {
        Debug.Log("Check 開始");
        await Awaitable.NextFrameAsync();
        return true;
    }
}
```

```
Check 開始
共有 1: True
共有 2: True
```

ここでは処理を開始するのは一度だけです。二度待っているのは `Task<bool>` であり、元の `Awaitable<bool>` ではありません。`Task` を使うと、そのオブジェクトの割り当てが必要になります。常に変換するのではなく、完了の共有が必要な場所で使います。多重 `await` の制約と変換については、[Unity のコード例](https://docs.unity3d.com/Manual/async-awaitable-examples.html)も参照してください。

---

## 3. 例外を try / catch で受け取る

待っていた処理が失敗した場合も、呼び出し元で失敗を知り、必要な対応をまとめて書きたいことがあります。`await` では、子の処理で起きた例外を、待っている側の `try` / `catch` で受け取れます。次の例では、成功時の続き、失敗時の対応、どちらの場合も必要な後片付けを分けています。

```csharp
using System;
using UnityEngine;

public class SampleAwaitable : MonoBehaviour
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

```
A
B
D: 失敗
E
```

子の `Fail` が投げた例外は、親の `await Fail()` に伝わります。`C` は実行されず、`catch` で「D: 失敗」を表示し、最後に `finally` の `E` を表示します。コルーチンでできなかった、待機を `try` / `catch` で囲む書き方も使えます。

---

## 4. キャンセルを伝えて後片付けする

処理を途中で止めるときにも、表示を戻したり、使っていたものを片付けたりする必要があります。外から処理を打ち切るだけでは、処理自身がその後片付けを行えません。キャンセルの要求を待機中の処理へ伝え、終了するときの `finally` で後片付けする形にします。

[キャンセル](/unity-csharp-learning/csharp/task-cancellation/) で学んだトークンを、待機にも渡します。次は 10 秒待つ処理を、P キーで止める例です。

```csharp
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class SampleAwaitable : MonoBehaviour
{
    private CancellationTokenSource _source;

    private async void Start()
    {
        _source = new CancellationTokenSource();
        try
        {
            await Work(_source.Token);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("停止");
        }
        finally
        {
            _source.Dispose();
            _source = null;
        }
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.pKey.wasPressedThisFrame)
        {
            _source?.Cancel();
        }
    }

    private void OnDestroy()
    {
        _source?.Cancel();
    }

    private async Awaitable Work(CancellationToken token)
    {
        try
        {
            Debug.Log("開始");
            await Awaitable.WaitForSecondsAsync(10f, token);
            Debug.Log("完了");
        }
        finally
        {
            Debug.Log("後片付け");
        }
    }
}
```

Play ボタンを押し、Game ビューをクリックしてから 10 秒以内に P キーを押すと、次の順に表示されます。

```
開始
後片付け
停止
```

`Cancel()` は停止の要求です。待機に渡したトークンがキャンセルされると、`OperationCanceledException` が起きます。子の `finally` で後片付けし、親の `catch` で停止を受け取れます。P キーを押さなければ、「開始」→ 約 10 秒後に「完了」→「後片付け」の順です。

親の `finally` はトークンソースを `Dispose` し、参照を `null` に戻します。`OnDestroy` は途中で Play を止めたり、このコンポーネントを破棄したりした場合にもキャンセルするための処理です。

### キャンセル要求と、処理の終了を区別する

`Cancel()` は、トークンを受け取った処理に終了を要求する操作です。あらゆる処理を外から強制的に止める機能ではありません。待機 API がトークンに対応していれば待機を中断でき、自分で書いた長い計算なら、その計算の中でもキャンセルを確認する必要があります。

また、呼び出し元が待つのをやめることと、開始済みの通信や読み込みそのものを中止することも別です。実際に何を中止できるかは、その API の仕様を確認します。キャンセルが別の操作へ自動で伝わるとは限らないため、子へもトークンを渡す、対応する中止 API を使うなど、終了までの経路を決めます。

`Cancel()` から戻った時点で、常にすべての後片付けが終わっているとも限りません。終了を確認する必要がある場合は、開始した処理の完了まで待ちます。トークンソースの `Dispose()` は停止の要求ではなく、使い終わった資源の解放です。キャンセル済みのソースを次の実行に使い回さず、実行ごとに作成して、終了時に破棄します。

---

## 5. ゲームオブジェクトの破棄に備える

async メソッドは、ゲームオブジェクトに自動では結び付いていません。ゲームオブジェクトが非アクティブになっても待機は続き、破棄した後でコンポーネントを使おうとすると `MissingReferenceException` になることがあります。

4 節では `OnDestroy` から自分でキャンセルしました。Unity が持つ `destroyCancellationToken` を使って、破棄の通知と操作による停止をまとめる方法もあります。

### 破棄とキャンセルを結び付ける

**`MonoBehaviour.destroyCancellationToken`** — その `MonoBehaviour` が破棄されたときにキャンセルされるトークンを取得します。

**書式：[MonoBehaviour.destroyCancellationToken プロパティ](https://docs.unity3d.com/ScriptReference/MonoBehaviour-destroyCancellationToken.html)**
```csharp
public CancellationToken destroyCancellationToken { get; }
```

`destroyCancellationToken` を async メソッドに渡しておくと、ゲームオブジェクトやスクリプトが破棄されたときに、待っている処理が `OperationCanceledException` で止まります。Play を止めたときも、シーンのゲームオブジェクトが破棄されるので、同じように止まります。

操作による停止と破棄の両方を伝えるには、2 つを 1 つのトークンにまとめます。

**`CancellationTokenSource.CreateLinkedTokenSource()`** — 指定したトークンのどれかがキャンセルされると、自分もキャンセルされる `CancellationTokenSource` を作ります。

**書式：[CancellationTokenSource.CreateLinkedTokenSource メソッド](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtokensource.createlinkedtokensource)**
```csharp
public static CancellationTokenSource CreateLinkedTokenSource(CancellationToken token);
```

| パラメータ | 説明 |
|---|---|
| `token` | つなげるトークン。このトークンがキャンセルされると、作られた `CancellationTokenSource` もキャンセルされる |

このソースは、自分の `Cancel()` を呼んだときにも、渡した破棄用トークンがキャンセルされたときにもキャンセルされます。6 節の信号機で、この方法を使います。

### どの出来事を、処理の終わりにするか

オブジェクトが存在していることと、その処理がまだ必要であることは同じではありません。たとえば、閉じた画面を非アクティブにして再利用する場合、破棄されるまで待たずに表示中の処理を止めたいことがあります。

| 処理を続けたい期間 | 終了を伝える方法 |
|---|---|
| コンポーネントが破棄されるまで | `destroyCancellationToken` を渡す |
| コンポーネントやゲームオブジェクトが有効な間だけ | `OnEnable` で開始し、`OnDisable` で自分のソースをキャンセルする |
| 入力で開始して、取り消すまで | 操作用のソースを作り、取り消す操作から `Cancel()` する |
| アプリケーションや Play モードが終了するまで | `Application.exitCancellationToken` を渡す。詳しくは[メインスレッドの補足](/unity-csharp-learning/unity/main-thread/#アプリや-play-モードの終了にも備える) |

次は、有効な間だけ 10 秒の処理を行う最小例です。無効化されたらキャンセルし、再び有効になったら新しい処理を始めます。

```csharp
using System;
using System.Threading;
using UnityEngine;

public class SampleAwaitable : MonoBehaviour
{
    private CancellationTokenSource _enabledSource;

    private void OnEnable()
    {
        _enabledSource = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
        RunWhileEnabled(_enabledSource);
    }

    private void OnDisable()
    {
        _enabledSource?.Cancel();
    }

    private async void RunWhileEnabled(CancellationTokenSource source)
    {
        try
        {
            Debug.Log("有効な間の処理: 開始");
            await Awaitable.WaitForSecondsAsync(10f, source.Token);
            Debug.Log("有効な間の処理: 完了");
        }
        catch (OperationCanceledException)
        {
            Debug.Log("有効な間の処理: 停止");
        }
        finally
        {
            if (ReferenceEquals(_enabledSource, source)) _enabledSource = null;
            source.Dispose();
        }
    }
}
```

10 秒以内にコンポーネントのチェックを外し、もう一度チェックを付けてから再び外すと、次の順に表示されます。操作しなければ、開始から約 10 秒後に「完了」が表示されます。

```
有効な間の処理: 開始
有効な間の処理: 停止
有効な間の処理: 開始
有効な間の処理: 停止
```

この例の停止は、無効化が async メソッドを自動で止めた結果ではありません。`OnDisable` が明示的にキャンセルした結果です。ソースを引数で受け取り、その実行自身が最後に破棄します。`ReferenceEquals` は、終了した処理のソースが現在のものと同じときだけ参照を消すための確認です。以前の処理の後片付けで、新しい実行の参照を消さないようにしています。

---

## 6. 実例：前のページの信号機を書き換える

ここまでの原理を、前のページの `Signal` と `CoroutineSignal` に適用します。Space キーで要求し、P キーで電源を切り替える操作はそのままです。

### 待機と結果を await に置き換える

`yield return` を `await` にし、`WaitForRequest` は `Awaitable<bool>` を返すようにします。1・2 節と同じ書き方です。この段階では、P キーの停止処理は空にしています。

<details markdown="1">
<summary>待機と結果を書き換えた全体コード</summary>

```csharp
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoroutineSignal : MonoBehaviour
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
        // 後の「キャンセルと後片付けを加える」で実装する
    }

    private async Awaitable RunSignal()
    {
        while (true)
        {
            SetColor(Color.red, "赤");

            while (!await WaitForRequest(_guideInterval))
            {
                Debug.Log($"{Time.time:F2} 秒: 案内「Space キーを押してください」");
            }
            _isRequested = false;

            await Awaitable.WaitForSecondsAsync(1f);

            SetColor(Color.blue, "青");
            await Awaitable.WaitForSecondsAsync(2f);

            await Blink(_blinkCount);
        }
    }

    private async Awaitable<bool> WaitForRequest(float timeout)
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

</details>

### 点滅の失敗を catch で受け取る

`await Blink(...)` を `try` / `catch` で囲みます。`Blink Count` が `0` なら警告を表示し、1 秒待って赤へ戻ります。3 節と同じ例外の受け取り方です。

<details markdown="1">
<summary>例外を受け取る版の全体コード</summary>

```csharp
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoroutineSignal : MonoBehaviour
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
        // 後の「キャンセルと後片付けを加える」で実装する
    }

    private async Awaitable RunSignal()
    {
        while (true)
        {
            SetColor(Color.red, "赤");

            while (!await WaitForRequest(_guideInterval))
            {
                Debug.Log($"{Time.time:F2} 秒: 案内「Space キーを押してください」");
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

    private async Awaitable<bool> WaitForRequest(float timeout)
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

</details>

### キャンセルと後片付けを加える

停止要求のトークンと `destroyCancellationToken` を結び付け、待機するメソッドに渡します。止められる側の `RunSignal` が `finally` で球体を黒にします。次が全機能を含む全体コードです。

### 完成したコード

```csharp
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoroutineSignal : MonoBehaviour
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

                while (!await WaitForRequest(_guideInterval, token))
                {
                    Debug.Log($"{Time.time:F2} 秒: 案内「Space キーを押してください」");
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

    private async Awaitable<bool> WaitForRequest(float timeout, CancellationToken token)
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

P キーで止めると、`RunSignal` の `finally` で消灯し、`PowerOn` の `catch` で停止を受け取ります。コルーチン版で止める側に書いていた後片付けを、止められる側にまとめられました。

破棄の違いは、上の「例外を受け取る版」と「完成したコード」をそれぞれ使って確かめられます。青になってから `Signal` を非アクティブにすると async の処理は続きます。`Signal` を破棄すると、前者では次に色を変えるときに `MissingReferenceException` が出ますが、後者ではキャンセルされて「消灯」「停止」と表示されます。

---

## 動作確認

1. これまでのシーンを開き、`CoroutineSignal` スクリプトを 6 節の「完成したコード」に書き換える
2. `Signal` の `CoroutineSignal` の `Blink Count` が `2` になっていることを確かめる
3. Play ボタンを押し、次の操作をする：Space で要求 → 点滅中に P で停止 → 停止中に Space → P で再開始 → Space で要求
4. Play を止める

3 では、点滅中に停止すると黒になり、再開始すると赤で入力を待つことを確認してください。Console では「消灯」の後に「停止」と表示されます。

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

しかし、6 節の「待機と結果を書き換えた全体コード」の `Start` をこのように書き換え、`Blink Count` を `0` にして試すと、青になった後に信号が止まるのに、Console には例外が表示されません。`Start` が返した `Awaitable` を受け取って `await` する呼び出し元がないので、`Awaitable` に記録された例外を誰も取り出さないためです。`async Awaitable` のメソッドを呼び出して、戻り値を `await` せずに捨てたときも、同じように例外が表示されません。

`Start` や入力から呼ぶメソッドのように、Unity から呼ばれて結果を誰も待たないメソッドは、`async void` にします。`async void` のメソッドで受け取られなかった例外は、Console に表示されます。そこから呼ぶメソッドは `async Awaitable` にして、必ず `await` します。

---

## ワンポイントアドバイス

### Awaitable と Task の違い

Unity の async メソッドでは、`Task` も使えます。しかし、Unity のドキュメントでは、Unity のコードでは基本的に `Awaitable` を使うことが勧められています。主な違いは次のとおりです。

- `Awaitable.NextFrameAsync` や `Awaitable.WaitForSecondsAsync` のように、フレームやゲーム時間に合わせて待つメソッドがある
- `Awaitable` のオブジェクトは、Unity が使い回しています（プール）。同じものを二度待たず、結果の再利用と完了の共有を分けて考えます。2 節で具体例を示しました

`await` は、常に別スレッドへ処理を移す指定ではありません。待機後にどのスレッドで再開するか、GameObject を操作できるのはどこかは、[補足：Unity のメインスレッドと別スレッド](/unity-csharp-learning/unity/main-thread/)で、スレッド ID を出力しながら確認します。`Task` の同期コンテキストについては、[await の前後で実行されるスレッド](/unity-csharp-learning/csharp/await-threads/)も参照してください。

詳しくは、Unity のマニュアルの [Asynchronous programming with the Awaitable class](https://docs.unity3d.com/Manual/async-awaitable-introduction.html) を参照してください。

---

## まとめ

- `async Awaitable` のメソッドでは、`await Awaitable.NextFrameAsync()` で次のフレームまで、`await Awaitable.WaitForSecondsAsync(秒数)` で指定した秒数だけ待てる
- `Awaitable<TResult>` を返すメソッドでは、結果を `return` で返し、呼び出し元は `await` の値として受け取れる
- 同じ `Awaitable` を二度待たない。結果の値を再利用するか、完了を共有する必要があれば `Task` に受け渡す
- `await` した処理の例外は呼び出し元に伝わり、`try` / `catch` で受け取れる
- `CancellationToken` を渡しておくと、`Cancel` で待っている処理が `OperationCanceledException` で止まり、止められた処理の中の `finally` も実行される
- async メソッドはゲームオブジェクトの非アクティブ化や破棄では止まらない。`destroyCancellationToken` を渡して、破棄されたときに止める
- 無効化・操作による取り消し・アプリの終了など、処理を続けたい期間に合わせてキャンセルを伝える
- Unity から呼ばれるメソッドは `async void` にし、そこから呼ぶメソッドは `async Awaitable` にして `await` する

---

## 理解度チェック

1. コルーチンの版の `WaitForRequest` と、`Awaitable` の版の `WaitForRequest` では、要求されたかどうかの受け取り方がどう違いますか？
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
4. （応用）6 節の「完成したコード」の `WaitForRequest` を、要求されたときは待ち始めてから要求されるまでの秒数を、時間切れのときは `-1` を返す `Awaitable<float>` のメソッドに変更してください。`RunSignal` では、要求されたときに「○○ 秒で押された」と Console に表示します。

<details markdown="1">
<summary>解答を見る</summary>

1. コルーチンの版では、`Action<bool>` のコールバックを引数で渡し、ラムダ式の中でローカル変数に代入して受け取りました。`Awaitable` の版では、`WaitForRequest` が `return` で返した値を、`await WaitForRequest(...)` の値として受け取れます。
2. `A`、`B`、`D: 失敗`、`E` の順に表示されます。`Fail` で起きた例外が `await Fail()` に伝わるので、`C` は表示されず、`catch` の `D: 失敗` が表示されます。最後に `finally` の `E` が表示されます。
3. async メソッドは、ゲームオブジェクトが破棄されても動き続けます。破棄された後で、破棄されたコンポーネント（この例では `Renderer`）を使おうとすると、`MissingReferenceException` が投げられます。
4. `WaitForRequest` で待ち始めた時刻を覚えておき、押されたら経過時間を返します。`RunSignal` では、結果が `0` 未満の間、案内を表示して待ち直します。

   ```csharp
   using System;
   using System.Threading;
   using UnityEngine;
   using UnityEngine.InputSystem;

   public class CoroutineSignal : MonoBehaviour
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

                   float waited = await WaitForRequest(_guideInterval, token);
                   while (waited < 0f)
                   {
                       Debug.Log($"{Time.time:F2} 秒: 案内「Space キーを押してください」");
                       waited = await WaitForRequest(_guideInterval, token);
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

       private async Awaitable<float> WaitForRequest(float timeout, CancellationToken token)
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

   Play ボタンを押して、案内が表示されてから約 1 秒後に Space キーを押すと、Console には次のように表示されます（実行結果の例）。

   ```
   0.00 秒: 赤
   5.02 秒: 案内「Space キーを押してください」
   6.00 秒: 切り替え要求
   6.02 秒: 1.00 秒で押された
   7.04 秒: 青
   ```

</details>

---

## 次のステップ

[補足：Unity のメインスレッドと別スレッド](/unity-csharp-learning/unity/main-thread/)では、待機と別スレッドでの計算を区別し、計算結果を GameObject に安全に反映する手順を学びます。

[Unity から通信する](/unity-csharp-learning/networking/unity-webrequest/) では、`UnityWebRequest` でサーバーと通信し、応答をコルーチンと `await` の両方の書き方で待ちます。
