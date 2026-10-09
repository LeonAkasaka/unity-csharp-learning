---
layout: page
title: Unity から通信する
permalink: /networking/unity-webrequest/
---

# Unity から通信する

ここまでは、curl を使ってサーバーにリクエストを送ってきました。このページでは、Unity からリクエストを送ります。Unity 標準の **UnityWebRequest** を使って、自分のサーバーからデータを取得したり、サーバーにデータを送ったりして、結果を Console ビューとサーバーのログで確かめます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `UnityWebRequest` で `GET` と `POST` のリクエストを送れる
- 応答を、コルーチンと `await` の両方の書き方で待てる
- `result`、`responseCode`、`error` を使って、通信が成功したかどうかを判断できる
- `[ContextMenu]` 属性を使って、Play モード中に Inspector ビューからメソッドを呼び出せる

## 前提知識

- [本文でデータを送る](/unity-csharp-learning/networking/request-body/) を読んでいること。このページでは、そのページまでに作った `SampleHttpServer` のプロジェクトを書き換えます
- [Debug.Log でスクリプトの実行を確認する](/unity-csharp-learning/unity/debug-log/) を読んでいること
- [コルーチンの基本](/unity-csharp-learning/unity/coroutines/) を読んでいること
- [async と await](/unity-csharp-learning/csharp/async-await/) を読んでいること
- [Awaitable と async / await](/unity-csharp-learning/unity/awaitable/) を読んでいること

---

## 1. UnityWebRequest

**UnityWebRequest** は、Unity に標準で含まれている、HTTP のリクエストを送るためのクラスです。.NET にも `HttpClient` などの通信用のクラスがありますが、`UnityWebRequest` は、Unity が対応するさまざまなプラットフォーム（Web ブラウザー上で動く WebGL など）で同じように使えるように作られています。Unity で HTTP の通信をするときは、まず `UnityWebRequest` を使います。

**書式：[UnityWebRequest クラス](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.html)**
```csharp
public class UnityWebRequest : IDisposable
```

`UnityWebRequest` は `UnityEngine.Networking` 名前空間にあるので、スクリプトの先頭に `using UnityEngine.Networking;` を書きます。`IDisposable` を実装していて、使い終わったら `Dispose` する必要があります。

`UnityWebRequest` を使うときは、次の 3 つの段階を踏みます。

| 段階 | すること |
|---|---|
| 1. 作る | `UnityWebRequest.Get` などで、送るリクエストを表すオブジェクトを作る |
| 2. 送る | `SendWebRequest` でリクエストを送り、応答が届くまで待つ |
| 3. 結果を見る | `result` で成功したかを確かめ、応答の本文やステータスコードを読む |

応答が届くまでには時間がかかります。その間ゲームが止まらないように、`SendWebRequest` はすぐに戻り、通信はバックグラウンドで進みます。応答を待つには、コルーチンか `await` を使います。

---

## 2. サーバーを準備する

[TCP で送る](/unity-csharp-learning/networking/tcp-send/) で学んだように、通信する 2 つのプログラムは、片方ずつ確かめます。先にサーバーを作って curl で確かめておけば、Unity から試してうまくいかないときに、Unity の側だけを調べれば済みます。

`SampleHttpServer` の `Program.cs` を、次のように書き換えます。ログを出すミドルウェアのほかには、決まった文字列を返すハンドラーを 1 つだけ登録します。

```csharp
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();

app.Use(async (context, next) =>
{
    HttpRequest request = context.Request;
    Console.WriteLine($"--> {request.Method} {request.Path}{request.QueryString}");
    await next(context);
    Console.WriteLine($"<-- {context.Response.StatusCode}");
});

app.MapGet("/hello", () => "こんにちは、サーバーです");

app.Run("http://localhost:8080");
```

サーバーを起動します。

```powershell
dotnet run
```

別のターミナルから、curl で確かめます。

```powershell
curl.exe http://localhost:8080/hello
```

```
こんにちは、サーバーです
```

サーバーは、Unity から試している間、起動したままにしておきます。

### HTTP で接続する設定

このサーバーの URL は、`https://` ではなく `http://` で始まります。`http://` の通信は暗号化されないので、Unity には、これを許可するかどうかを決める設定があります。

Unity の Player 設定の **Allow downloads over HTTP** という項目です（**Edit → Project Settings → Player → Other Settings**）。スクリプトからは [PlayerSettings.insecureHttpOption](https://docs.unity3d.com/ScriptReference/PlayerSettings-insecureHttpOption.html) にあたります。

| 選択肢 | 意味 |
|---|---|
| **Not Allowed** | `http://` の通信を許可しない（既定値） |
| **Allowed in Development Builds** | 開発用のビルド（Development Build）でだけ許可する |
| **Always Allowed** | 常に許可する |

詳しくは [Player 設定のマニュアル](https://docs.unity3d.com/Manual/playersettings-windows.html) を参照してください。

このページの手順は、この設定が既定値の **Not Allowed** のまま、Unity Editor の Play モードで `http://localhost:8080` に接続できることを確かめています。一方、ビルドしたアプリから `http://` のサーバーに接続するには、この設定を変える必要があります。インターネット上のサーバーと通信するときに `https://` を使う理由とあわせて、後の回で扱います。

---

## 3. GET を送る（コルーチン）

Unity のメニューバーの **GameObject → Create Empty** を選択し、空のゲームオブジェクトを作成します。名前を `RequestTester` に変更してください。

`RequestTester` を選択し、Inspector ビューの **Add Component → New script** から `RequestTester` という名前のスクリプトを作成してアタッチします。スクリプトを次のように書き換えます。

```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class RequestTester : MonoBehaviour
{
    [SerializeField] private string _getUrl = "http://localhost:8080/hello";

    [ContextMenu("GET を送る")]
    private void SendGet()
    {
        StartCoroutine(SendGetCoroutine());
    }

    private IEnumerator SendGetCoroutine()
    {
        using UnityWebRequest request = UnityWebRequest.Get(_getUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"失敗しました: {request.error}");
            yield break;
        }
        Debug.Log($"{request.responseCode}\n{request.downloadHandler.text}");
    }
}
```

`_getUrl` は、`GET` のリクエストを送る URL です。`[SerializeField]` を付けているので、Inspector ビューから変更できます。

### リクエストを作る

**書式：[UnityWebRequest.Get メソッド](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.Get.html)**
```csharp
public static UnityWebRequest Get(string uri);
```

`UnityWebRequest.Get` は、指定した URL に `GET` のリクエストを送るための `UnityWebRequest` を作ります。この時点では、まだリクエストは送られていません。

`UnityWebRequest` は、使い終わったら `Dispose` して、通信に使ったメモリなどを解放する必要があります。`using` で宣言すると、メソッドを抜けるときに自動で `Dispose` されます。

### リクエストを送って待つ

**書式：[UnityWebRequest.SendWebRequest メソッド](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.SendWebRequest.html)**
```csharp
public UnityWebRequestAsyncOperation SendWebRequest();
```

`SendWebRequest` は、リクエストを送り始めて、すぐに戻ります。戻り値の `UnityWebRequestAsyncOperation` を `yield return` すると、コルーチンは応答が届くまで待ち、届いたら次の行から再開します。待っている間も、ゲームのほかの処理は止まりません。

### 結果を見る

応答が届いたら、次のプロパティで結果を確かめます。

**書式：[UnityWebRequest.result プロパティ](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-result.html)**
```csharp
public UnityWebRequest.Result result { get; }
```

`result` は、通信の結果です。成功したときは `UnityWebRequest.Result.Success` になります。ほかの値は「6. 失敗を判定する」で扱います。

**書式：[UnityWebRequest.responseCode プロパティ](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-responseCode.html)**
```csharp
public long responseCode { get; }
```

`responseCode` は、応答のステータスコードです。

**書式：[UnityWebRequest.error プロパティ](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-error.html)**
```csharp
public string error { get; }
```

`error` は、失敗したときの、エラーの説明です。

**書式：[UnityWebRequest.downloadHandler プロパティ](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-downloadHandler.html)**
```csharp
public DownloadHandler downloadHandler { get; set; }
```

`downloadHandler` は、受け取った応答の本文を扱う [DownloadHandler](https://docs.unity3d.com/ScriptReference/Networking.DownloadHandler.html) です。`UnityWebRequest.Get` で作ったリクエストには、本文を受け取るための `DownloadHandler` があらかじめ設定されています。

**書式：[DownloadHandler.text プロパティ](https://docs.unity3d.com/ScriptReference/Networking.DownloadHandler-text.html)**
```csharp
public string text { get; }
```

`text` は、受け取った本文を文字列にしたものです。

`result` が `Success` でなければ、エラーを `Debug.LogError` で出力して、`yield break` でコルーチンを終わらせます。成功したときは、ステータスコードと本文を出力します。

### ContextMenu からメソッドを呼び出す

**書式：[ContextMenu 属性](https://docs.unity3d.com/ScriptReference/ContextMenu.html)**
```csharp
[ContextMenu("メニューに表示する名前")]
```

`[ContextMenu]` 属性を付けたメソッドは、Inspector ビューのコンポーネントのメニューから呼び出せるようになります。ボタンなどの UI を作らなくても、好きなときにリクエストを送って試せます。

### 確かめる

Play モードに入り、Hierarchy ビューで `RequestTester` を選択します。Inspector ビューで `Request Tester` コンポーネントの見出しを右クリックする（または見出しの右端の **⋮** を押す）と、メニューに **GET を送る** が表示されます。選択すると、Console ビューに次のように表示されます。

```
200
こんにちは、サーバーです
```

サーバーのログには、Unity から届いたリクエストが表示されます。

```
--> GET /hello
<-- 200
```

curl から送ったときと同じように、Unity からのリクエストもサーバーのログで確かめられます。

---

## 4. await で書く

Unity 6 では、`SendWebRequest` の戻り値を、コルーチンの `yield return` の代わりに `await` で待つこともできます。スクリプトを次のように書き換えます。結果を出力する処理は、この後のリクエストでも使うので、`LogResult` メソッドにまとめています。

```csharp
using UnityEngine;
using UnityEngine.Networking;

public class RequestTester : MonoBehaviour
{
    [SerializeField] private string _getUrl = "http://localhost:8080/hello";

    [ContextMenu("GET を送る")]
    private async void SendGet()
    {
        using UnityWebRequest request = UnityWebRequest.Get(_getUrl);
        await request.SendWebRequest();
        LogResult(request);
    }

    private void LogResult(UnityWebRequest request)
    {
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{request.method} {request.url} は失敗しました: {request.result} / {request.responseCode} / {request.error}");
            return;
        }
        Debug.Log($"{request.method} {request.url} → {request.responseCode}\n{request.downloadHandler.text}");
    }
}
```

コルーチンの版では、`SendGet` から `StartCoroutine` で別のメソッドを動かしていました。`await` を使うと、リクエストを作る、送って待つ、結果を見る、という流れを 1 つのメソッドの中に上から順に書けます。`[ContextMenu]` から呼び出すメソッドは戻り値を受け取る相手がいないので、`async void` にしています。

`LogResult` では、どのリクエストの結果なのかがわかるように、`request.method`（メソッド）と `request.url`（URL）も出力しています。

**書式：[UnityWebRequest.method プロパティ](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-method.html)**
```csharp
public string method { get; set; }
```

**書式：[UnityWebRequest.url プロパティ](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-url.html)**
```csharp
public string url { get; set; }
```

`method` はリクエストのメソッド（`GET` など）、`url` はリクエストを送る URL です。`UnityWebRequest.Get` などで作ったときに設定されます。

Play モードに入り、コンポーネントのメニューから **GET を送る** を選ぶと、Console ビューに次のように表示されます。

```
GET http://localhost:8080/hello → 200
こんにちは、サーバーです
```

コルーチンと `await` のどちらを使っても、通信の結果は同じです。このシリーズでは、この後、`await` の書き方を使います。

### インターネット上の文書を取得する

`_getUrl` を変えれば、同じスクリプトで、インターネット上で公開されている文書も取得できます。

Play モードを終了してから、Inspector ビューで `Get Url` を次の URL に書き換えます。インターネットの技術の標準を定めた文書である **RFC** のうち、`example.com` のように説明や例のために使うドメイン名を決めた RFC 2606 の URL です。

```
https://www.rfc-editor.org/rfc/rfc2606.txt
```

Play モードに入り、**GET を送る** を選ぶと、Console ビューの一覧に次のように表示されます。

```
GET https://www.rfc-editor.org/rfc/rfc2606.txt → 200
```

Console ビューの一覧には、ログの先頭の 2 行だけが表示されます。RFC 2606 の本文は空の行から始まるので、一覧では 1 行目だけが見えます。ログを選択すると、Console ビューの下の欄に、`Network Working Group` で始まる本文の全体が表示されます。ブラウザで同じ URL を開くと、同じ文書が表示されます。

このリクエストは `SampleHttpServer` には送られていないので、サーバーのログには何も表示されません。接続先は、URL だけで決まります。

確かめたら、`Get Url` を `http://localhost:8080/hello` に戻しておきます。

---

## 5. POST を送る

次は、サーバーに `POST` でデータを送ります。

### サーバーにハンドラーを追加する

`SampleHttpServer` の `Program.cs` で、`/hello` のハンドラーの後ろに、次のハンドラーを追加します。届いた本文をサーバーのログに出力し、`受け取りました: ` を付けて送り返します。

```csharp
app.MapPost("/echo", async (Stream body) =>
{
    using StreamReader reader = new StreamReader(body);
    string text = await reader.ReadToEndAsync();
    Console.WriteLine($"本文: {text}");
    return $"受け取りました: {text}";
});
```

本文は、[本文でデータを送る](/unity-csharp-learning/networking/request-body/) と同じように、`Stream` 型の引数で受け取り、`StreamReader` で読んでいます。

`Program.cs` の全体は、次のようになります。

```csharp
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();

app.Use(async (context, next) =>
{
    HttpRequest request = context.Request;
    Console.WriteLine($"--> {request.Method} {request.Path}{request.QueryString}");
    await next(context);
    Console.WriteLine($"<-- {context.Response.StatusCode}");
});

app.MapGet("/hello", () => "こんにちは、サーバーです");

app.MapPost("/echo", async (Stream body) =>
{
    using StreamReader reader = new StreamReader(body);
    string text = await reader.ReadToEndAsync();
    Console.WriteLine($"本文: {text}");
    return $"受け取りました: {text}";
});

app.Run("http://localhost:8080");
```

サーバーを Ctrl+C で止めて起動し直し、curl で確かめます。

```powershell
curl.exe -X POST -d "Hello" http://localhost:8080/echo
```

```
受け取りました: Hello
```

### Unity から送る

`RequestTester` のスクリプトを、次のように書き換えます。`POST` を送るメソッドと、その送り先と本文を決めるフィールドを追加しています。

```csharp
using UnityEngine;
using UnityEngine.Networking;

public class RequestTester : MonoBehaviour
{
    [SerializeField] private string _getUrl = "http://localhost:8080/hello";
    [SerializeField] private string _postUrl = "http://localhost:8080/echo";
    [SerializeField] private string _postText = "Hello from Unity";

    [ContextMenu("GET を送る")]
    private async void SendGet()
    {
        using UnityWebRequest request = UnityWebRequest.Get(_getUrl);
        await request.SendWebRequest();
        LogResult(request);
    }

    [ContextMenu("POST を送る")]
    private async void SendPost()
    {
        using UnityWebRequest request = UnityWebRequest.Post(_postUrl, _postText, "text/plain");
        await request.SendWebRequest();
        LogResult(request);
    }

    private void LogResult(UnityWebRequest request)
    {
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{request.method} {request.url} は失敗しました: {request.result} / {request.responseCode} / {request.error}");
            return;
        }
        Debug.Log($"{request.method} {request.url} → {request.responseCode}\n{request.downloadHandler.text}");
    }
}
```

**書式：[UnityWebRequest.Post メソッド](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.Post.html)**（文字列の本文）
```csharp
public static UnityWebRequest Post(string uri, string postData, string contentType);
```

| パラメータ | 説明 |
|---|---|
| `uri` | リクエストを送る URL |
| `postData` | 本文にする文字列 |
| `contentType` | 本文の種類。`Content-Type` ヘッダーになる。ここでは、ただのテキストを表す `text/plain` |

Play モードに入り、**POST を送る** を選ぶと、Console ビューに次のように表示されます。

```
POST http://localhost:8080/echo → 200
受け取りました: Hello from Unity
```

サーバーのログには、Unity から届いたリクエストと本文が表示されます。

```
--> POST /echo
本文: Hello from Unity
<-- 200
```

---

## 6. 失敗を判定する

通信は、いつも成功するとは限りません。`result` の型は、`UnityWebRequest` の中で宣言された列挙型です。

**書式：[UnityWebRequest.Result 列挙型](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.Result.html)**
```csharp
public enum Result
{
    InProgress,
    Success,
    ConnectionError,
    ProtocolError,
    DataProcessingError
}
```

それぞれの値の意味は次のとおりです。

| 値 | 意味 |
|---|---|
| `Success` | 成功した |
| `ConnectionError` | サーバーに接続できなかった |
| `ProtocolError` | 応答は届いたが、ステータスコードが失敗を表していた（`4xx` や `5xx`） |
| `DataProcessingError` | 受け取ったデータの処理に失敗した |
| `InProgress` | まだ通信中 |

### ステータスコードが失敗を表しているとき

Play モードを終了してから、Inspector ビューで `Get Url` を、登録していないパスの `http://localhost:8080/nothing` に書き換えます。Play モードに入って **GET を送る** を選ぶと、Console ビューにエラーとして次のように表示されます。

```
GET http://localhost:8080/nothing は失敗しました: ProtocolError / 404 / HTTP/1.1 404 Not Found
```

サーバーからは `404 Not Found` の応答が届いています。通信そのものはできているので、`ConnectionError` ではなく `ProtocolError` になります。

### サーバーに接続できないとき

`Get Url` を `http://localhost:8080/hello` に戻します。サーバーのターミナルで Ctrl+C を押してサーバーを止め、**GET を送る** を選ぶと、次のように表示されます。

```
GET http://localhost:8080/hello は失敗しました: ConnectionError / 0 / Cannot connect to destination host
```

今度は応答が届いていないので、`result` は `ConnectionError`、`responseCode` は `0` です。

どちらの場合も、`SendWebRequest` を待つところで例外はスローされません。通信が成功したかどうかは、必ず `result` で確かめます。

---

## 完成したコード

このページで作った `RequestTester` のスクリプトの全体です。

```csharp
using UnityEngine;
using UnityEngine.Networking;

public class RequestTester : MonoBehaviour
{
    [SerializeField] private string _getUrl = "http://localhost:8080/hello";
    [SerializeField] private string _postUrl = "http://localhost:8080/echo";
    [SerializeField] private string _postText = "Hello from Unity";

    [ContextMenu("GET を送る")]
    private async void SendGet()
    {
        using UnityWebRequest request = UnityWebRequest.Get(_getUrl);
        await request.SendWebRequest();
        LogResult(request);
    }

    [ContextMenu("POST を送る")]
    private async void SendPost()
    {
        using UnityWebRequest request = UnityWebRequest.Post(_postUrl, _postText, "text/plain");
        await request.SendWebRequest();
        LogResult(request);
    }

    private void LogResult(UnityWebRequest request)
    {
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{request.method} {request.url} は失敗しました: {request.result} / {request.responseCode} / {request.error}");
            return;
        }
        Debug.Log($"{request.method} {request.url} → {request.responseCode}\n{request.downloadHandler.text}");
    }
}
```

`SampleHttpServer` の `Program.cs` は、「5. POST を送る」で示した全体のとおりです。

---

## よくあるミス

### result を確かめずに本文を使う

`result` を確かめずに `downloadHandler.text` を使うと、失敗したことに気付けません。

```csharp
// ❌ NG: 失敗しても、そのまま本文を使ってしまう
await request.SendWebRequest();
Debug.Log(request.downloadHandler.text);
```

たとえば、`SampleHttpServer` が返す `404` の応答は、本文が空です。このコードでは空の行が出力されるだけで、エラーは表示されません。「本文が空だった」のか「失敗した」のかを区別できないので、必ず先に `result` を確かめます。

### サーバーを起動し忘れる

サーバーを起動せずに Play モードで試すと、「6. 失敗を判定する」と同じ `ConnectionError` になります。Unity の側を疑う前に、curl でサーバーに接続できるかを確かめます。

---

## まとめ

- `UnityWebRequest` は、Unity 標準の HTTP 通信のクラス。`Get` や `Post` でリクエストを作り、`SendWebRequest` で送る
- 応答は、コルーチンの `yield return` か、`await` で待つ。どちらも、待っている間ゲームは止まらない
- 使い終わった `UnityWebRequest` は `Dispose` する。`using` で宣言すると自動で `Dispose` される
- 結果は `result` で確かめる。接続できないときは `ConnectionError`、`4xx` や `5xx` の応答は `ProtocolError` になり、どちらも例外はスローされない
- 通信する 2 つのプログラムは、片方ずつ確かめる。サーバーを curl で確かめてから、Unity の側を確かめる
- `[ContextMenu]` 属性を付けたメソッドは、Inspector ビューのコンポーネントのメニューから呼び出せる

---

## 理解度チェック

1. `UnityWebRequest` を `using` で宣言するのはなぜですか？
2. サーバーが `404 Not Found` を返したとき、`result` と `responseCode` はそれぞれ何になりますか？
3. サーバーが起動していないとき、`result` と `responseCode` はそれぞれ何になりますか？
4. `UnityWebRequest.Post` の 3 つ目の引数に `"text/plain"` を渡すと、リクエストのどこに反映されますか？

<details markdown="1">
<summary>解答を見る</summary>

1. `UnityWebRequest` は、使い終わったら `Dispose` して、通信に使ったメモリなどを解放する必要があるからです。`using` で宣言すると、メソッドを抜けるときに自動で `Dispose` されます。
2. `result` は `ProtocolError`、`responseCode` は `404` です。応答は届いているので、接続のエラーにはなりません。
3. `result` は `ConnectionError`、`responseCode` は `0` です。応答が届いていないので、ステータスコードはありません。
4. リクエストの `Content-Type` ヘッダーになります。

</details>

---

## 次のステップ

[JSON でやり取りする](/unity-csharp-learning/networking/json/) では、文字列だけでなく、名前や時刻などを含むデータをやり取りするために、JSON を使います。
