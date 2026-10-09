---
layout: page
title: 失敗に備える
permalink: /networking/failure-handling/
---

# 失敗に備える

[Unity から通信する](/unity-csharp-learning/networking/unity-webrequest/) では、`result` で通信の成否を確かめました。しかし、実際のゲームでは、失敗したことがわかるだけでは足りません。応答がいつまでも返ってこない、同じリクエストを重ねて送ってしまう、応答を待っている間に GameObject が破棄される、一時的な失敗で処理があきらめてしまう、といった問題に備える必要があります。このページでは、サーバーにわざと遅い応答や失敗を返すパスを用意し、Unity の側でそれぞれに備える方法を学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `UnityWebRequest.timeout` で、応答を待つ時間に制限を付けられる
- 前のリクエストが終わるまで、次のリクエストを送らないようにできる
- `destroyCancellationToken` を使って、GameObject が破棄されたときにリクエストを中止できる
- 送り直してよい失敗と、送り直してはいけない失敗を区別して、リクエストを送り直せる

## 前提知識

- [JSON でやり取りする](/unity-csharp-learning/networking/json/) を読んでいること。このページでは、そのページの `SampleHttpServer` にパスを追加します
- [キャンセル](/unity-csharp-learning/csharp/task-cancellation/) を読んでいること
- [例外](/unity-csharp-learning/csharp/exceptions/) を読んでいること

---

## 1. 通信の失敗の種類

通信が失敗したとき、`UnityWebRequest` では次のように表れます。

| 失敗 | `result` | `responseCode` | `error` の例 |
|---|---|---|---|
| サーバーに接続できない | `ConnectionError` | `0` | `Cannot connect to destination host` |
| 応答が時間内に返ってこない | `ConnectionError` | `0` | `Request timeout` |
| リクエストに問題がある（`4xx`） | `ProtocolError` | `400`、`404` など | `HTTP/1.1 404 Not Found` |
| サーバーの側で問題が起きた（`5xx`） | `ProtocolError` | `500`、`503` など | `HTTP/1.1 503 Service Unavailable` |

このうち、応答が時間内に返ってこない場合は、そのままでは起きません。`UnityWebRequest` は、標準では時間の制限なく応答を待ち続けるからです。まず、これを確かめるためのパスをサーバーに用意します。

---

## 2. 確かめるためのパスをサーバーに追加する

`SampleHttpServer` の `Program.cs` の `app.Run` の前に、次のコードを追加します。どちらも、失敗を確かめるためだけのパスです。

```csharp
// 動作確認用：指定した秒数だけ待ってから応答する
app.MapGet("/slow", async (int seconds) =>
{
    await Task.Delay(TimeSpan.FromSeconds(seconds));
    return "遅い応答です";
});

// 動作確認用：3 回に 2 回は 503 を返す
int unstableCount = 0;
app.MapGet("/unstable", () =>
{
    int count = Interlocked.Increment(ref unstableCount);
    if (count % 3 != 0)
    {
        return Results.StatusCode(503);
    }
    return Results.Text("やっと成功しました");
});
```

| パス | 動作 |
|---|---|
| `/slow?seconds=5` | 指定した秒数だけ待ってから、`200 OK` を返す |
| `/unstable` | 呼ばれた回数を数え、3 回に 2 回は `503 Service Unavailable` を返す |

`503 Service Unavailable` は、「サーバーが一時的にリクエストを処理できない」ことを表すステータスコードです。サーバーが混み合っているときや、準備中のときなどに返されます。

`/unstable` の回数は、複数のリクエストから同時に数えられることがあるので、[共有データと lock](/unity-csharp-learning/csharp/thread-safety/) で学んだ `Interlocked.Increment` で数えています。

`Task.Delay` で待っている間も、Kestrel はほかのリクエストを処理できます。遅いリクエストが 1 つあっても、サーバー全体が止まることはありません。

---

## 3. FailureTester を作る

Unity で、失敗を確かめるためのコンポーネントを作ります。メニューバーの **GameObject → Create Empty** で空のゲームオブジェクトを作り、名前を `FailureTester` にします。Inspector ビューの **Add Component → New script** から `FailureTester` という名前のスクリプトを作ってアタッチし、次のように書きます。

```csharp
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class FailureTester : MonoBehaviour
{
    [SerializeField] private string _baseUrl = "http://localhost:8080";
    [SerializeField] private int _timeoutSeconds = 3;
    [SerializeField] private int _slowSeconds = 5;
    [SerializeField] private int _maxAttempts = 3;
    [SerializeField] private string _retryPath = "/unstable";

    private bool _isBusy;

    [ContextMenu("遅いパスを取得する")]
    private async void GetSlow()
    {
        if (_isBusy)
        {
            Debug.LogWarning("前のリクエストが終わっていないので、送りません");
            return;
        }
        _isBusy = true;
        try
        {
            CancellationToken token = destroyCancellationToken;
            using UnityWebRequest request = UnityWebRequest.Get($"{_baseUrl}/slow?seconds={_slowSeconds}");
            request.timeout = _timeoutSeconds;
            using CancellationTokenRegistration registration = token.Register(request.Abort);

            try
            {
                await request.SendWebRequest();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("GameObject が破棄されたので、リクエストを中止しました");
                return;
            }
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"失敗しました: {request.result} / {request.responseCode} / {request.error}");
                return;
            }
            Debug.Log($"{name}: {request.downloadHandler.text}");
        }
        finally
        {
            _isBusy = false;
        }
    }

    [ContextMenu("送り直しながら取得する")]
    private async void GetWithRetry()
    {
        string text = await GetWithRetryAsync($"{_baseUrl}{_retryPath}");
        if (text != null)
        {
            Debug.Log($"成功しました: {text}");
        }
    }

    private async Awaitable<string> GetWithRetryAsync(string url)
    {
        for (int attempt = 1; attempt <= _maxAttempts; attempt++)
        {
            using UnityWebRequest request = UnityWebRequest.Get(url);
            request.timeout = _timeoutSeconds;
            await request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                return request.downloadHandler.text;
            }

            Debug.LogWarning($"{attempt} 回目は失敗しました: {request.result} / {request.responseCode} / {request.error}");
            bool canRetry = request.result == UnityWebRequest.Result.ConnectionError || request.responseCode >= 500;
            if (!canRetry)
            {
                Debug.LogError("送り直しても結果が変わらない失敗なので、あきらめます");
                return null;
            }
            if (attempt < _maxAttempts)
            {
                await Awaitable.WaitForSecondsAsync(attempt);
            }
        }
        Debug.LogError($"{_maxAttempts} 回送っても成功しなかったので、あきらめます");
        return null;
    }
}
```

このスクリプトには、2 つのメニューがあります。

| メニュー | すること |
|---|---|
| **遅いパスを取得する** | `/slow?seconds={Slow Seconds}` を、時間の制限を付けて取得する。重ねて送らないことと、破棄されたときの中止も扱う |
| **送り直しながら取得する** | `{Retry Path}` のパスを、失敗したら送り直しながら取得する |

以下の節で、このスクリプトの各部分を順に見ていきます。

---

## 4. 時間の制限を付ける

**書式：[UnityWebRequest.timeout プロパティ](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-timeout.html)**
```csharp
public int timeout { get; set; }
```

`timeout` は、応答を待つ時間の上限（秒）です。既定値は `0` で、`0` のときは時間の制限がなく、応答が届くまで待ち続けます。

サーバーが止まっているわけではないのに、何かの理由で応答が返ってこないことがあります。時間の制限がないと、ゲームはいつまでも応答を待ち続け、プレイヤーには何が起きているのかわかりません。通信するときは、必ず `timeout` を設定します。

`GetSlow` では、`request.timeout = _timeoutSeconds;` で、制限を 3 秒にしています。一方、`Slow Seconds` の既定値は 5 秒なので、サーバーは 5 秒待ってから応答します。Play モードで **遅いパスを取得する** を選ぶと、3 秒後に Console ビューに次のエラーが表示されます。

```
失敗しました: ConnectionError / 0 / Request timeout
```

時間切れは、`result` が `ConnectionError`、`error` が `Request timeout` になります。応答は届いていないので、`responseCode` は `0` です。

Inspector ビューで `Slow Seconds` を `1` にして、もう一度選ぶと、制限の時間内に応答が届くので、次のように表示されます。

```
FailureTester: 遅い応答です
```

---

## 5. 重ねて送らない

ボタンを押して送信するゲームで、プレイヤーがボタンを続けて何度も押すと、同じリクエストが何度も送られます。[本文でデータを送る](/unity-csharp-learning/networking/request-body/) で学んだように、`POST` を重ねて送ると、同じメッセージが何件も追加されてしまいます。

`GetSlow` では、`_isBusy` フィールドで、リクエストを送っている最中かどうかを覚えています。

- 送り始める前に `_isBusy` を確かめ、`true` なら警告を出して、何も送らずに戻る
- 送り始めるときに `_isBusy` を `true` にする
- 終わったら、`finally` で `_isBusy` を `false` に戻す

`finally` に書いているのは、途中で `return` したときや、例外がスローされたときにも、必ず `false` に戻すためです。戻し忘れると、それ以降、二度とリクエストを送れなくなります。

`Slow Seconds` を `2` にして、**遅いパスを取得する** を続けて 2 回選ぶと、2 回目は送られずに、次のように表示されます。

```
前のリクエストが終わっていないので、送りません
FailureTester: 遅い応答です
```

1 行目が 2 回目の呼び出しの警告、2 行目が 1 回目のリクエストの結果です。サーバーのログを見ると、`/slow` へのリクエストは 1 回しか届いていません。

---

## 6. 破棄されたら中止する

応答を待っている間に、シーンが切り替わったり、GameObject が削除されたりすることがあります。このとき、何も備えていないと、応答が届いた後の処理で問題が起きます。

破棄に備えないとどうなるかを確かめるために、次のスクリプト `FailureTesterNg` を作り、別の空のゲームオブジェクト `FailureTesterNg` にアタッチします。

```csharp
using UnityEngine;
using UnityEngine.Networking;

public class FailureTesterNg : MonoBehaviour
{
    [SerializeField] private string _baseUrl = "http://localhost:8080";
    [SerializeField] private int _slowSeconds = 5;

    // ❌ NG: 破棄に備えていない
    [ContextMenu("破棄に備えずに取得する")]
    private async void GetSlowNoCancel()
    {
        using UnityWebRequest request = UnityWebRequest.Get($"{_baseUrl}/slow?seconds={_slowSeconds}");
        await request.SendWebRequest();
        Debug.Log($"{name}: {request.downloadHandler.text}");
    }
}
```

Play モードで **破棄に備えずに取得する** を選び、応答が届く前に Hierarchy ビューで `FailureTesterNg` を削除すると、応答が届いたときに次の例外がスローされます。

```
MissingReferenceException: The object of type 'FailureTesterNg' has been destroyed but you are still trying to access it.
```

`await` の後の処理は、GameObject が破棄された後でも実行されます。そこで、破棄されたコンポーネントの `name` にアクセスしたので、例外がスローされました。Play モードを終了したときも、シーンの GameObject はすべて破棄されます。

### destroyCancellationToken

**書式：[MonoBehaviour.destroyCancellationToken プロパティ](https://docs.unity3d.com/ScriptReference/MonoBehaviour-destroyCancellationToken.html)**
```csharp
public CancellationToken destroyCancellationToken { get; }
```

`destroyCancellationToken` は、コンポーネントが破棄されるとキャンセルされる `CancellationToken` です。[キャンセル](/unity-csharp-learning/csharp/task-cancellation/) で学んだ `CancellationToken` を、Unity がコンポーネントごとに用意してくれています。

`GetSlow` では、この `CancellationToken` に、キャンセルされたときに呼び出す処理を登録しています。

```csharp
CancellationToken token = destroyCancellationToken;
// （中略）
using CancellationTokenRegistration registration = token.Register(request.Abort);
```

**書式：[CancellationToken.Register メソッド](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken.register)**
```csharp
public CancellationTokenRegistration Register(Action callback);
```

**書式：[UnityWebRequest.Abort メソッド](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.Abort.html)**
```csharp
public void Abort();
```

`Register` は、キャンセルされたときに `callback` を呼び出すように登録します。ここでは `request.Abort` を登録しているので、コンポーネントが破棄されると、リクエストが中止されます。`Register` の戻り値の `CancellationTokenRegistration` は、`Dispose` すると登録を取り消します。`using` で宣言して、メソッドを抜けるときに登録を取り消しています。

中止されたリクエストを `await` していると、`OperationCanceledException` がスローされます。`GetSlow` では、これをキャッチして、結果を使わずに戻っています。

```csharp
try
{
    await request.SendWebRequest();
}
catch (OperationCanceledException)
{
    Debug.Log("GameObject が破棄されたので、リクエストを中止しました");
    return;
}
```

`destroyCancellationToken` をメソッドの最初でローカル変数に入れているのは、コンポーネントが破棄された後に、そのプロパティを読まずに済むようにするためです。

`Slow Seconds` を `5`、`Timeout Seconds` を `10` にして **遅いパスを取得する** を選び、応答が届く前に Hierarchy ビューで `FailureTester` を削除します（または Play モードを終了します）。Console ビューには、例外ではなく、次のように表示されます。

```
GameObject が破棄されたので、リクエストを中止しました
```

なお、中止したのは Unity の側だけです。サーバーのログを見ると、サーバーは 5 秒待った後、いつもどおり `200` の応答を返しています。

---

## 7. 送り直す

`503 Service Unavailable` のように一時的な失敗であれば、少し待ってから送り直すと成功することがあります。しかし、どんな失敗でも送り直してよいわけではありません。

| 失敗 | 送り直すか | 理由 |
|---|---|---|
| 接続できない、時間切れ（`ConnectionError`） | 送り直す | サーバーの再起動中や、通信が一時的に不安定なだけかもしれない |
| `5xx` | 送り直す | サーバーの一時的な問題かもしれない |
| `4xx` | 送り直さない | リクエストの内容に問題があるので、同じリクエストを何度送っても結果は変わらない |

さらに、[本文でデータを送る](/unity-csharp-learning/networking/request-body/) で学んだように、`POST` を送り直すと、同じデータが 2 回追加されるおそれがあります。たとえば、サーバーはメッセージを追加したのに、その応答が時間切れで届かなかった場合です。このページでは、送り直すのは、何度送っても結果が変わらない `GET` だけにしています。

### GetWithRetryAsync

`GetWithRetryAsync` は、URL を受け取り、成功したら本文を、あきらめたら `null` を返すメソッドです。戻り値の型は `Awaitable<string>` にしています。[Awaitable](https://docs.unity3d.com/ScriptReference/Awaitable.html) は、Unity の非同期処理のための型で、`Task<string>` と同じように `await` できます。

- ループの中で、毎回新しい `UnityWebRequest` を作る。一度送った `UnityWebRequest` は、もう一度送れない（`SendWebRequest` を 2 回呼ぶと、`InvalidOperationException` がスローされる）
- 成功したら、本文を返して終わる
- 失敗したら、送り直してよい失敗かを判断する。`4xx` なら、すぐにあきらめる
- 送り直すときは、少し待ってから送る。待つ時間は、1 回目の後は 1 秒、2 回目の後は 2 秒のように、だんだん長くする

**書式：[Awaitable.WaitForSecondsAsync メソッド](https://docs.unity3d.com/ScriptReference/Awaitable.WaitForSecondsAsync.html)**
```csharp
public static Awaitable WaitForSecondsAsync(float seconds, CancellationToken cancellationToken = default);
```

`WaitForSecondsAsync` は、指定した秒数（ゲームの時間）だけ待つ `Awaitable` を返します。コルーチンの `yield return new WaitForSeconds(...)` にあたります。

送り直すまでの時間をだんだん長くするのは、サーバーが混み合っているときに、多くのクライアントが一斉に送り直して、さらに混み合うのを避けるためです。

### 確かめる

サーバーを起動し直して `/unstable` の回数を 0 に戻してから、Play モードで **送り直しながら取得する** を選ぶと、次のように表示されます。

```
1 回目は失敗しました: ProtocolError / 503 / HTTP/1.1 503 Service Unavailable
2 回目は失敗しました: ProtocolError / 503 / HTTP/1.1 503 Service Unavailable
成功しました: やっと成功しました
```

2 回失敗しても、送り直して 3 回目で成功しました。サーバーのログにも、`/unstable` へのリクエストが 3 回届いています。

```
--> GET /unstable
<-- 503
--> GET /unstable
<-- 503
--> GET /unstable
<-- 200
```

Inspector ビューで `Retry Path` を `/messages/999`（存在しないメッセージ）にして選ぶと、`404` は送り直さずに、すぐにあきらめます。

```
1 回目は失敗しました: ProtocolError / 404 / HTTP/1.1 404 Not Found
送り直しても結果が変わらない失敗なので、あきらめます
```

サーバーを止めてから選ぶと、接続できない失敗なので、3 回送り直した後にあきらめます。

```
1 回目は失敗しました: ConnectionError / 0 / Cannot connect to destination host
2 回目は失敗しました: ConnectionError / 0 / Cannot connect to destination host
3 回目は失敗しました: ConnectionError / 0 / Cannot connect to destination host
3 回送っても成功しなかったので、あきらめます
```

---

## よくあるミス

### _isBusy を finally の外で戻す

```csharp
// ❌ NG: 失敗して途中で return すると、_isBusy が true のまま残る
_isBusy = true;
await request.SendWebRequest();
if (request.result != UnityWebRequest.Result.Success)
{
    Debug.LogError("失敗しました");
    return;
}
_isBusy = false;
```

一度でも失敗すると、`_isBusy` が `true` のまま残り、それ以降はずっと「前のリクエストが終わっていない」と判断されて、何も送れなくなります。状態を元に戻す処理は、`finally` に書きます。

### 送り直すときに同じ UnityWebRequest を使う

失敗した `UnityWebRequest` の `SendWebRequest` をもう一度呼ぶと、`InvalidOperationException`（`UnityWebRequest has already been sent; cannot begin sending the request again`）がスローされます。送り直すときは、`UnityWebRequest` を作り直します。

---

## まとめ

- `UnityWebRequest.timeout` の既定値は `0` で、時間の制限がない。通信するときは必ず設定する。時間切れは `ConnectionError`、`Request timeout` になる
- 送信中かどうかをフィールドで覚え、終わるまで次のリクエストを送らない。フィールドを戻す処理は `finally` に書く
- `await` の後の処理は、GameObject が破棄された後でも実行される。`destroyCancellationToken` に `Abort` を登録して、破棄されたときにリクエストを中止する。中止されると、`await` で `OperationCanceledException` がスローされる
- 接続できない、時間切れ、`5xx` は、待ってから送り直すと成功することがある。`4xx` は送り直しても結果が変わらない。`POST` の送り直しは、データが重複するおそれがある
- 送り直すときは、`UnityWebRequest` を作り直し、待つ時間をだんだん長くする

---

## 理解度チェック

1. `UnityWebRequest.timeout` を設定しないと、サーバーが応答を返さないときにどうなりますか？
2. `_isBusy` を `false` に戻す処理を `finally` に書くのはなぜですか？
3. 応答を待っている間に GameObject が破棄されたとき、`destroyCancellationToken` を使わないと、どのような問題が起きることがありますか？
4. 次の失敗のうち、送り直してよいものはどれですか？ `ConnectionError`、`400 Bad Request`、`503 Service Unavailable`、`404 Not Found`

<details markdown="1">
<summary>解答を見る</summary>

1. 時間の制限がないので、応答が届くまでいつまでも待ち続けます。
2. 途中で `return` したときや、例外がスローされたときにも、必ず `false` に戻すためです。戻し忘れると、それ以降リクエストを送れなくなります。
3. `await` の後の処理が、破棄された後でも実行されます。その処理で破棄されたコンポーネントや GameObject にアクセスすると、`MissingReferenceException` がスローされます。
4. `ConnectionError` と `503 Service Unavailable` です。`400` と `404` はリクエストの内容の問題なので、同じリクエストを送り直しても結果は変わりません。

</details>

---

## 次のステップ

[複数のクライアントをつなぐ](/unity-csharp-learning/networking/multiple-clients/) では、複数のクライアントを同じサーバーにつなぎ、全員で 1 つのデータを使います。
