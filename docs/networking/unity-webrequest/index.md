---
layout: page
title: Unity から通信する
permalink: /networking/unity-webrequest/
---

# Unity から通信する

[HTTP のメソッドとステータスコード](/unity-csharp-learning/networking/http-methods/) では、メッセージの一覧を持つサーバーを作り、curl から操作しました。このページでは、curl の代わりに Unity からサーバーにリクエストを送ります。Unity 標準の **UnityWebRequest** を使ってメッセージの一覧を取得し、メッセージを追加して、結果を Console ビューで確かめます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `UnityWebRequest` で `GET` と `POST` のリクエストを送れる
- 応答を、コルーチンと `await` の両方の書き方で待てる
- `result`、`responseCode`、`error` を使って、通信が成功したかどうかを判断できる
- `[ContextMenu]` 属性を使って、Play モード中に Inspector ビューからメソッドを呼び出せる

## 前提知識

- [HTTP のメソッドとステータスコード](/unity-csharp-learning/networking/http-methods/) を読んでいること。このページでは、そのページで完成した `SampleHttpServer` を使います
- [Debug.Log でスクリプトの実行を確認する](/unity-csharp-learning/unity/debug-log/) を読んでいること
- [コルーチンの基本](/unity-csharp-learning/unity/coroutines/) を読んでいること
- [async と await](/unity-csharp-learning/csharp/async-await/) を読んでいること
- [Awaitable と async / await](/unity-csharp-learning/unity/awaitable/) を読んでいること

---

## 1. サーバーを準備する

[TCP で送る](/unity-csharp-learning/networking/tcp-send/) で学んだように、通信する 2 つのプログラムは、片方ずつ確かめます。サーバーはすでに curl で確かめ済みなので、ここでは Unity の側だけを作ります。

`SampleHttpServer` のフォルダーで、サーバーを起動します。

```powershell
dotnet run
```

別のターミナルから、curl でメッセージを 2 つ追加し、一覧を確かめておきます。

```powershell
curl.exe -X POST -d "Hello" http://localhost:8080/messages
curl.exe -X POST -d "Good morning" http://localhost:8080/messages
curl.exe http://localhost:8080/messages
```

```
1: Hello
2: Good morning
```

サーバーは、Unity から試している間、起動したままにしておきます。

---

## 2. UnityWebRequest

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

## 3. 一覧を取得する（コルーチン）

メニューバーの **GameObject → Create Empty** を選択し、空のゲームオブジェクトを作成します。名前を `MessageClient` に変更してください。

`MessageClient` を選択し、Inspector ビューの **Add Component → New script** から `MessageClient` という名前のスクリプトを作成してアタッチします。スクリプトを次のように書き換えます。

```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class MessageClient : MonoBehaviour
{
    [SerializeField] private string _baseUrl = "http://localhost:8080";

    [ContextMenu("一覧を取得する")]
    private void GetMessages()
    {
        StartCoroutine(GetMessagesCoroutine());
    }

    private IEnumerator GetMessagesCoroutine()
    {
        using UnityWebRequest request = UnityWebRequest.Get($"{_baseUrl}/messages");
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

`_baseUrl` は、サーバーの URL です。`[SerializeField]` を付けているので、Inspector ビューから変更できます。

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

Play モードに入り、Hierarchy ビューで `MessageClient` を選択します。Inspector ビューで `Message Client` コンポーネントの見出しを右クリックする（または見出しの右端の **⋮** を押す）と、メニューに **一覧を取得する** が表示されます。選択すると、Console ビューに次のように表示されます。

```
200
1: Hello
2: Good morning
```

サーバーのログには、Unity から届いたリクエストが表示されます。

```
--> GET /messages
<-- 200
```

curl から送ったときと同じように、Unity からのリクエストもサーバーのログで確かめられます。

---

## 4. await で書く

Unity 6 では、`SendWebRequest` の戻り値を、コルーチンの `yield return` の代わりに `await` で待つこともできます。スクリプトを次のように書き換えます。1 つのメッセージを取得するメソッドと、結果を出力する処理をまとめた `LogResult` メソッドも追加しています。

```csharp
using UnityEngine;
using UnityEngine.Networking;

public class MessageClient : MonoBehaviour
{
    [SerializeField] private string _baseUrl = "http://localhost:8080";
    [SerializeField] private int _messageId = 1;

    [ContextMenu("一覧を取得する")]
    private async void GetMessages()
    {
        using UnityWebRequest request = UnityWebRequest.Get($"{_baseUrl}/messages");
        await request.SendWebRequest();
        LogResult(request);
    }

    [ContextMenu("1 件を取得する")]
    private async void GetMessage()
    {
        using UnityWebRequest request = UnityWebRequest.Get($"{_baseUrl}/messages/{_messageId}");
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

コルーチンの版では、`GetMessages` から `StartCoroutine` で別のメソッドを動かしていました。`await` を使うと、リクエストを作る、送って待つ、結果を見る、という流れを 1 つのメソッドの中に上から順に書けます。`[ContextMenu]` から呼び出すメソッドは戻り値を受け取る相手がいないので、`async void` にしています。

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

Play モードに入り、コンポーネントのメニューから **一覧を取得する** を選ぶと、Console ビューに次のように表示されます。

```
GET http://localhost:8080/messages → 200
1: Hello
2: Good morning
```

Inspector ビューで `Message Id` を `2` にしてから **1 件を取得する** を選ぶと、次のように表示されます。

```
GET http://localhost:8080/messages/2 → 200
Good morning
```

コルーチンと `await` のどちらを使っても、通信の結果は同じです。このシリーズでは、この後、`await` の書き方を使います。

---

## 5. メッセージを追加する（POST）

メッセージを追加するメソッドを追加します。スクリプトを次のように書き換えます。

```csharp
using UnityEngine;
using UnityEngine.Networking;

public class MessageClient : MonoBehaviour
{
    [SerializeField] private string _baseUrl = "http://localhost:8080";
    [SerializeField] private int _messageId = 1;
    [SerializeField] private string _messageText = "Hello from Unity";

    [ContextMenu("一覧を取得する")]
    private async void GetMessages()
    {
        using UnityWebRequest request = UnityWebRequest.Get($"{_baseUrl}/messages");
        await request.SendWebRequest();
        LogResult(request);
    }

    [ContextMenu("1 件を取得する")]
    private async void GetMessage()
    {
        using UnityWebRequest request = UnityWebRequest.Get($"{_baseUrl}/messages/{_messageId}");
        await request.SendWebRequest();
        LogResult(request);
    }

    // 追加
    [ContextMenu("追加する")]
    private async void PostMessage()
    {
        using UnityWebRequest request = UnityWebRequest.Post($"{_baseUrl}/messages", _messageText, "text/plain");
        await request.SendWebRequest();
        LogResult(request);
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Location: {request.GetResponseHeader("Location")}");
        }
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

**書式：[UnityWebRequest.GetResponseHeader メソッド](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.GetResponseHeader.html)**
```csharp
public string GetResponseHeader(string name);
```

`GetResponseHeader` は、応答のヘッダーの値を返します。サーバーは、`201 Created` の応答の `Location` ヘッダーで、追加したメッセージのパスを知らせてくれるので、それを出力しています。

Play モードに入り、**追加する** を選ぶと、Console ビューに次のように表示されます。`201` の応答は本文が空なので、1 行目の後には何も表示されません。

```
POST http://localhost:8080/messages → 201

```

```
Location: /messages/3
```

続けて **一覧を取得する** を選ぶと、Unity から追加したメッセージが一覧に加わっています。

```
GET http://localhost:8080/messages → 200
1: Hello
2: Good morning
3: Hello from Unity
```

サーバーのログにも、`POST` が届いて `201` を返したことが表示されます。

```
--> POST /messages
<-- 201
--> GET /messages
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

Inspector ビューで `Message Id` を `9`（存在しない番号）にして **1 件を取得する** を選ぶと、Console ビューにエラーとして次のように表示されます。

```
GET http://localhost:8080/messages/9 は失敗しました: ProtocolError / 404 / HTTP/1.1 404 Not Found
```

サーバーからは `404 Not Found` の応答が届いています。通信そのものはできているので、`ConnectionError` ではなく `ProtocolError` になります。

### サーバーに接続できないとき

サーバーのターミナルで Ctrl+C を押してサーバーを止め、**一覧を取得する** を選ぶと、次のように表示されます。

```
GET http://localhost:8080/messages は失敗しました: ConnectionError / 0 / Cannot connect to destination host
```

今度は応答が届いていないので、`result` は `ConnectionError`、`responseCode` は `0` です。

どちらの場合も、`SendWebRequest` を待つところで例外はスローされません。通信が成功したかどうかは、必ず `result` で確かめます。

---

## 7. HTTP で接続する設定

Unity の Player 設定には、暗号化されていない `http://` の通信を許可するかどうかを決める **Allow downloads over HTTP** という項目があります（**Edit → Project Settings → Player → Other Settings**）。スクリプトからは [PlayerSettings.insecureHttpOption](https://docs.unity3d.com/ScriptReference/PlayerSettings-insecureHttpOption.html) にあたります。

| 選択肢 | 意味 |
|---|---|
| **Not Allowed** | `http://` の通信を許可しない（既定値） |
| **Allowed in Development Builds** | 開発用のビルド（Development Build）でだけ許可する |
| **Always Allowed** | 常に許可する |

詳しくは [Player 設定のマニュアル](https://docs.unity3d.com/Manual/playersettings-windows.html) を参照してください。

このページの手順は、この設定が既定値の **Not Allowed** のまま、Unity Editor の Play モードで `http://localhost:8080` に接続できることを確かめています。一方、ビルドしたアプリから `http://` のサーバーに接続するには、この設定を変える必要があります。インターネット上のサーバーと通信するときに `https://` を使う理由とあわせて、後の回で扱います。

---

## よくあるミス

### result を確かめずに本文を使う

`result` を確かめずに `downloadHandler.text` を使うと、失敗したことに気付けません。

```csharp
// ❌ NG: 失敗しても、そのまま本文を使ってしまう
await request.SendWebRequest();
Debug.Log(request.downloadHandler.text);
```

たとえば、存在しない番号を指定したときの `404` の応答は、本文が空です。このコードでは空の行が出力されるだけで、エラーは表示されません。「メッセージが空だった」のか「メッセージがなかった」のかを区別できないので、必ず先に `result` を確かめます。

### サーバーを起動し忘れる

サーバーを起動せずに Play モードで試すと、「6. 失敗を判定する」と同じ `ConnectionError` になります。Unity の側を疑う前に、curl でサーバーに接続できるかを確かめます。

---

## まとめ

- `UnityWebRequest` は、Unity 標準の HTTP 通信のクラス。`Get` や `Post` でリクエストを作り、`SendWebRequest` で送る
- 応答は、コルーチンの `yield return` か、`await` で待つ。どちらも、待っている間ゲームは止まらない
- 使い終わった `UnityWebRequest` は `Dispose` する。`using` で宣言すると自動で `Dispose` される
- 結果は `result` で確かめる。接続できないときは `ConnectionError`、`4xx` や `5xx` の応答は `ProtocolError` になり、どちらも例外はスローされない
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

[JSON でやり取りする](/unity-csharp-learning/networking/json/) では、メッセージを文字列ではなく、名前や時刻などを含むデータとしてやり取りするために、JSON を使います。
