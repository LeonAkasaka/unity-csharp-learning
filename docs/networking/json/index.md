---
layout: page
title: JSON でやり取りする
permalink: /networking/json/
---

# JSON でやり取りする

[本文でデータを送る](/unity-csharp-learning/networking/request-body/) では、メッセージをただの文字列として送り、一覧はメッセージを 1 行に 1 つずつ並べたテキストで返していました。このページでは、メッセージに送信者の名前と送った時刻を加え、データの形をはっきり決めてやり取りするために **JSON** を使います。サーバーでは ASP.NET Core の JSON の機能を、Unity では `JsonUtility` を使います。あわせて、URL に値を入れるときに必要な**エンコード**も学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- JSON の書き方と、データのやり取りに JSON を使う理由を説明できる
- ASP.NET Core のハンドラーで、JSON を受け取り、JSON を返せる
- `Content-Type` ヘッダーの役割を説明できる
- Unity の `JsonUtility` で JSON とオブジェクトを変換でき、その制約を説明できる
- URL のクエリ文字列に入れる値を、`Uri.EscapeDataString` でエンコードできる

## 前提知識

- [Unity から通信する](/unity-csharp-learning/networking/unity-webrequest/) を読んでいること。このページでは、`SampleHttpServer` のプロジェクトを書き換え、Unity では新しく `MessageClient` を作ります
- [レコード](/unity-csharp-learning/csharp/records/) を読んでいること
- [補足: 現実時間の取得（DateTime と DateTimeOffset）](/unity-csharp-learning/unity/time-datetime/) を読んでいること

---

## 1. テキストの書式の限界

今のサーバーは、メッセージの一覧を次のようなテキストで返しています。

```
1: Hello
2: Good morning
```

受け取った側は、各行を `: ` で区切って、番号とメッセージに分けることになります。しかし、この書式には次のような問題があります。

- メッセージに改行が含まれていると、1 つのメッセージが 2 行に分かれてしまい、区切りを判断できない
- 送信者の名前や送った時刻を加えるには、新しい区切り方を考えなければならない。名前に `: ` が含まれていたらどうするか、といった取り決めも必要になる
- 区切り方を変えるたびに、サーバーとクライアントの両方で、読み書きするコードを作り直すことになる

データの形を決めて送る方法は、多くのプログラムで共通に使える形式として、すでに決められています。Web で最も広く使われているのが **JSON**（JavaScript Object Notation）です。.NET にも Unity にも、JSON を読み書きする機能が用意されています。

---

## 2. JSON の書き方

JSON では、データを次の組み合わせで書きます。

| 書き方 | 意味 | 例 |
|---|---|---|
| `{ "名前": 値, ... }` | **オブジェクト**。名前と値の組の集まり。C# のクラスのオブジェクトにあたる | `{ "id": 1, "text": "Hello" }` |
| `[ 値, ... ]` | **配列**。値の並び | `[ 1, 2, 3 ]` |
| `"..."` | 文字列 | `"こんにちは"` |
| `123`、`1.5` | 数値 | `1` |
| `true`、`false` | 真偽値 | `true` |
| `null` | 値がないこと | `null` |

名前は必ず `"` で囲みます。値には、オブジェクトや配列を入れることもできます。このページでは、1 つのメッセージを次の JSON で表します。

```json
{
  "id": 1,
  "sender": "Player1",
  "text": "おはよう",
  "createdAt": "2026-09-27T19:00:08.9236173+00:00"
}
```

`createdAt` は、サーバーがメッセージを受け取った時刻です。JSON には日時の型がないので、日時は決まった書式の文字列で書きます。ここでは、`2026-09-27T19:00:08+00:00` のように、日付と時刻の間に `T` を入れ、最後に UTC（協定世界時）からの差を付けた **ISO 8601** という書式を使います。

改行や `: ` がメッセージに含まれていても、文字列の中に書かれている限り、区切りと取り違えることはありません。JSON の文字列の中では、改行は `\n` のように書き表すことが決められているからです。

---

## 3. サーバーで JSON を扱う

`SampleHttpServer` の `Program.cs` を、次のように書き換えます。

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

MessageStore store = new MessageStore();

app.MapGet("/messages", (string? contains) =>
{
    List<Message> messages = store.GetAll();
    if (contains != null)
    {
        messages = messages.FindAll(message => message.Text.Contains(contains));
    }
    return new MessageList(messages);
});

app.MapGet("/messages/{id:int}", (int id) =>
{
    if (store.TryGet(id, out Message? message))
    {
        return Results.Ok(message);
    }
    return Results.NotFound();
});

app.MapPost("/messages", (PostMessageRequest body) =>
{
    if (string.IsNullOrEmpty(body.Sender) || string.IsNullOrEmpty(body.Text))
    {
        return Results.BadRequest();
    }
    Message message = store.Add(body.Sender, body.Text);
    return Results.Created($"/messages/{message.Id}", message);
});

app.Run("http://localhost:8080");

record Message(int Id, string Sender, string Text, DateTimeOffset CreatedAt);
record PostMessageRequest(string Sender, string Text);
record MessageList(List<Message> Messages);

class MessageStore
{
    private readonly List<Message> _messages = new List<Message>();
    private int _nextId = 1;

    public List<Message> GetAll()
    {
        lock (_messages)
        {
            return new List<Message>(_messages);
        }
    }

    public bool TryGet(int id, out Message? message)
    {
        lock (_messages)
        {
            message = _messages.Find(m => m.Id == id);
            return message != null;
        }
    }

    public Message Add(string sender, string text)
    {
        lock (_messages)
        {
            Message message = new Message(_nextId, sender, text, DateTimeOffset.UtcNow);
            _nextId++;
            _messages.Add(message);
            return message;
        }
    }
}
```

`MessageStore` は、メッセージの一覧を保存するクラスです。[本文でデータを送る](/unity-csharp-learning/networking/request-body/) の `messages` と同じように、`List` を `lock` で守りながら読み書きします。読み書きをクラスのメソッドにまとめておくと、ハンドラーごとに `lock` を書く必要がなくなり、書き忘れを防げます。

### データの形をレコードで決める

サーバーがやり取りするデータの形を、3 つの**レコード**で決めています。

| レコード | 使う場面 |
|---|---|
| `Message` | 1 つのメッセージ。番号、送信者、本文、時刻 |
| `PostMessageRequest` | `POST` のリクエストの本文。クライアントが送る送信者と本文 |
| `MessageList` | 一覧の応答。メッセージの配列を `Messages` という名前で包む |

`PostMessageRequest` に番号や時刻がないのは、それらはサーバーが決めるものだからです。クライアントが送ってきた値を使うと、番号が重なったり、でたらめな時刻が記録されたりするおそれがあります。

一覧を、配列のまま返さずに `MessageList` で包んでいる理由は、「5. Unity で JSON を扱う」で説明します。

### オブジェクトを返すと JSON になる

ハンドラーが文字列以外のオブジェクトを返すと、ASP.NET Core は、それを JSON に変換して本文にします。`Results.Ok` や `Results.Created` に渡したオブジェクトも同じです。このとき、`Content-Type` は `application/json; charset=utf-8` になります。

C# のプロパティ名は `Sender` のように大文字で始まりますが、ASP.NET Core は、JSON の名前を `sender` のように小文字で始まる形（**キャメルケース**）に変換します。JSON では、キャメルケースの名前がよく使われるからです。

### JSON の本文を受け取る

`MapPost` のハンドラーの引数を、`PostMessageRequest` 型にしています。ハンドラーの引数に、ルートパラメーターでもクエリ文字列でもない、クラスやレコードの型を書くと、ASP.NET Core はリクエストの本文を JSON として読み、その型のオブジェクトに変換して渡します。JSON の名前の大文字と小文字は区別せずに対応させます。

[本文でデータを送る](/unity-csharp-learning/networking/request-body/) では `Stream` 型の引数で本文を受け取り、`StreamReader` で文字列として読んでいましたが、その処理と JSON からの変換を、ASP.NET Core が引き受けてくれます。

JSON に `sender` や `text` がないと、そのプロパティは `null` になります。そこで、`string.IsNullOrEmpty` で確かめて、足りなければ `400 Bad Request` を返しています。

### curl で確かめる

JSON には `"` がたくさん含まれていて、コマンドに直接書くと、シェルによって引用符の扱いが変わり、正しく送れないことがあります。そこで、送る JSON をファイルに書いておき、curl にはファイルを渡します。

作業用のフォルダーに、次の内容の `message.json` と `message2.json` を作ります。文字コードは UTF-8 で保存します（Windows 11 のメモ帳は、標準で UTF-8 で保存します）。

`message.json`

```json
{"sender":"Player1","text":"おはよう"}
```

`message2.json`

```json
{"sender":"Player1","text":"1+1=2"}
```

サーバーを起動し直して、そのフォルダーから curl で送ります。`--data-binary "@ファイル名"` は、ファイルの中身をそのまま本文にして送るオプションです。`-H` でヘッダーを追加して、本文が JSON であることを `Content-Type` で伝えます。

```powershell
curl.exe -i -H "Content-Type: application/json" --data-binary "@message.json" http://localhost:8080/messages
```

実行結果の例です。`Date` と `createdAt` の日時は実行するたびに変わります。

```
HTTP/1.1 201 Created
Content-Type: application/json; charset=utf-8
Date: Sun, 27 Sep 2026 19:00:08 GMT
Server: Kestrel
Location: /messages/1
Transfer-Encoding: chunked

{"id":1,"sender":"Player1","text":"おはよう","createdAt":"2026-09-27T19:00:08.9236173+00:00"}
```

```powershell
curl.exe -H "Content-Type: application/json" --data-binary "@message2.json" http://localhost:8080/messages
curl.exe http://localhost:8080/messages
```

一覧は、次のような JSON で返ります。実際には 1 行で表示されます。

```json
{"messages":[{"id":1,"sender":"Player1","text":"おはよう","createdAt":"2026-09-27T19:00:08.9236173+00:00"},{"id":2,"sender":"Player1","text":"1+1=2","createdAt":"2026-09-27T19:00:08.9549756+00:00"}]}
```

### Content-Type の役割

`Content-Type` は、本文がどんな形式のデータなのかを相手に伝えるヘッダーです。ASP.NET Core は、`Content-Type` が `application/json` のときだけ、本文を JSON として読みます。同じ JSON のファイルを、`Content-Type: text/plain` を付けて送ると、次の応答が返ります。

```powershell
curl.exe -i -H "Content-Type: text/plain" --data-binary "@message.json" http://localhost:8080/messages
```

```
HTTP/1.1 415 Unsupported Media Type
Content-Length: 0
Date: Sun, 27 Sep 2026 18:40:14 GMT
Server: Kestrel

```

`415 Unsupported Media Type` は、「その形式の本文は受け付けられない」ことを表すステータスコードです。本文の中身が JSON であっても、`Content-Type` で JSON だと伝えなければ、サーバーは JSON として扱いません。

---

## 4. 補足：JsonSerializer で変換を確かめる

ASP.NET Core は、内部で [System.Text.Json](https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializer.serialize) の `JsonSerializer` クラスを使って、オブジェクトと JSON を変換しています。`JsonSerializer` を直接使って、どのような JSON になるかを確かめます。コンソールアプリで、次のコードを実行します。

```csharp
using System.Text.Json;

Message message = new Message(1, "Player1", "こんにちは", new DateTimeOffset(2026, 9, 28, 12, 0, 0, TimeSpan.Zero));
Console.WriteLine(JsonSerializer.Serialize(message));
Console.WriteLine(JsonSerializer.Serialize(message, new JsonSerializerOptions(JsonSerializerDefaults.Web)));

string json = "{\"id\":2,\"sender\":\"Player2\",\"text\":\"Hi\",\"createdAt\":\"2026-09-28T12:00:00+00:00\"}";
Message? m2 = JsonSerializer.Deserialize<Message>(json, new JsonSerializerOptions(JsonSerializerDefaults.Web));
Console.WriteLine(m2);

record Message(int Id, string Sender, string Text, DateTimeOffset CreatedAt);
```

実行結果の例です。3 行目の日時の書式は、パソコンの地域の設定によって変わります。

```
{"Id":1,"Sender":"Player1","Text":"こんにちは","CreatedAt":"2026-09-28T12:00:00+00:00"}
{"id":1,"sender":"Player1","text":"こんにちは","createdAt":"2026-09-28T12:00:00+00:00"}
Message { Id = 2, Sender = Player2, Text = Hi, CreatedAt = 2026/09/28 12:00:00 +00:00 }
```

**書式：[JsonSerializer.Serialize メソッド](https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializer.serialize)**
```csharp
public static string Serialize<TValue>(TValue value, JsonSerializerOptions? options = null);
```

**書式：[JsonSerializer.Deserialize メソッド](https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializer.deserialize)**
```csharp
public static TValue? Deserialize<TValue>(string json, JsonSerializerOptions? options = null);
```

オブジェクトを JSON などの形式に変換することを**シリアライズ**（serialize）、その逆を**デシリアライズ**（deserialize）と呼びます。

結果から、次のことがわかります。

- `JsonSerializer` の標準の設定では、名前は C# のプロパティ名のまま（`Id`）になる。[JsonSerializerDefaults.Web](https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializerdefaults) を指定すると、キャメルケース（`id`）になり、読むときに大文字と小文字を区別しなくなる。ASP.NET Core は、この `Web` の設定を使っている
- `JsonSerializer` の標準の設定では、日本語は `ユ` のような書き方（**エスケープ**）で出力される。`\u` の後の 4 桁は、文字の番号を 16 進数で表したもので、JSON として正しい書き方なので、読む側では元の文字に戻る。なお、「3. サーバーで JSON を扱う」の curl の結果のとおり、ASP.NET Core の応答では、日本語はエスケープされずにそのまま出力される

---

## 5. Unity で JSON を扱う

Unity では、[JsonUtility](https://docs.unity3d.com/ScriptReference/JsonUtility.html) クラスを使って、JSON とオブジェクトを変換します。

**書式：[JsonUtility.FromJson メソッド](https://docs.unity3d.com/ScriptReference/JsonUtility.FromJson.html)**
```csharp
public static T FromJson<T>(string json);
```

**書式：[JsonUtility.ToJson メソッド](https://docs.unity3d.com/ScriptReference/JsonUtility.ToJson.html)**
```csharp
public static string ToJson(object obj);
```

`FromJson` は JSON を `T` 型のオブジェクトに、`ToJson` はオブジェクトを JSON に変換します。

### データのクラスを用意する

`JsonUtility` で変換するクラスは、次の決まりを守って書きます。

- クラスに `[Serializable]` 属性を付ける
- 変換する値は、`public` なフィールドにする（プロパティは変換されない）
- フィールドの名前を、JSON の名前と大文字、小文字まで同じにする

Project ビューで `Assets` フォルダーを右クリックし、**Create → Scripting → Empty C# Script** から `MessageData` という名前のスクリプトを作ります。次のように書きます。

```csharp
using System;

[Serializable]
public class MessageData
{
    public int id;
    public string sender;
    public string text;
    public string createdAt;
}

[Serializable]
public class MessageListData
{
    public MessageData[] messages;
}

[Serializable]
public class PostMessageData
{
    public string sender;
    public string text;
}
```

サーバーのレコードと対応させて、3 つのクラスを作っています。`createdAt` を `string` にしている理由は、次の制約の表で説明します。

### JsonUtility の制約

`JsonUtility` は、Unity の Inspector ビューに値を保存する仕組みと同じ規則で変換します。そのため、.NET の `JsonSerializer` とは違う、次のような制約があります。

| 制約 | 実際の動作 |
|---|---|
| JSON の最上位に配列を置けない | `JsonUtility.FromJson<MessageData[]>("[...]")` は、`ArgumentException`（`Return type must represent an object type. Received an array.`）をスローする |
| `DateTimeOffset` や `DateTime` を扱えない | `DateTimeOffset` のフィールドには値が入らず、既定値のままになる。`ToJson` の出力にも含まれない |
| プロパティは変換されない | `{ get; set; }` のプロパティは、`ToJson` の出力に含まれない |
| 名前の大文字と小文字を区別する | JSON の `"ID"` は、フィールド `id` に入らない |
| JSON にない名前のフィールドは既定値のまま | `text` が JSON になければ、`text` は `null` になる |

サーバーが一覧を `{"messages":[...]}` のようにオブジェクトで包んで返しているのは、1 つ目の制約のためです。時刻は `string` として受け取り、使うときに [DateTimeOffset.Parse](https://learn.microsoft.com/dotnet/api/system.datetimeoffset.parse) で `DateTimeOffset` に変換します。

JSON の `ユ` のようなエスケープは、`JsonUtility` でも元の文字に戻ります。

> 💡 **ポイント**: `JsonUtility` の制約が合わないときは、Unity のパッケージとして提供されている **Newtonsoft Json**（`com.unity.nuget.newtonsoft-json`）を使う方法もあります。このシリーズでは、Unity 標準の `JsonUtility` で足りるように、サーバーの JSON の形を決めています。

---

## 6. MessageClient を作る

Unity のメニューバーの **GameObject → Create Empty** を選択し、空のゲームオブジェクトを作成します。名前を `MessageClient` に変更してください。

`MessageClient` を選択し、Inspector ビューの **Add Component → New script** から `MessageClient` という名前のスクリプトを作成してアタッチします。スクリプトを次のように書き換えます。

```csharp
using System;
using UnityEngine;
using UnityEngine.Networking;

public class MessageClient : MonoBehaviour
{
    [SerializeField] private string _baseUrl = "http://localhost:8080";
    [SerializeField] private string _sender = "Player2";
    [SerializeField] private string _text = "こんにちは";
    [SerializeField] private string _keyword = "1+1";

    [ContextMenu("一覧を取得する")]
    private async void GetMessages()
    {
        using UnityWebRequest request = UnityWebRequest.Get($"{_baseUrl}/messages");
        await request.SendWebRequest();
        if (!IsSuccess(request))
        {
            return;
        }

        MessageListData list = JsonUtility.FromJson<MessageListData>(request.downloadHandler.text);
        LogMessages(list);
    }

    [ContextMenu("検索する")]
    private async void SearchMessages()
    {
        string url = $"{_baseUrl}/messages?contains={Uri.EscapeDataString(_keyword)}";
        using UnityWebRequest request = UnityWebRequest.Get(url);
        await request.SendWebRequest();
        if (!IsSuccess(request))
        {
            return;
        }

        MessageListData list = JsonUtility.FromJson<MessageListData>(request.downloadHandler.text);
        LogMessages(list);
    }

    [ContextMenu("追加する")]
    private async void PostMessage()
    {
        PostMessageData data = new PostMessageData();
        data.sender = _sender;
        data.text = _text;
        string json = JsonUtility.ToJson(data);

        using UnityWebRequest request = UnityWebRequest.Post($"{_baseUrl}/messages", json, "application/json");
        await request.SendWebRequest();
        if (!IsSuccess(request))
        {
            return;
        }

        MessageData message = JsonUtility.FromJson<MessageData>(request.downloadHandler.text);
        Debug.Log($"追加しました: {Format(message)}");
    }

    private static bool IsSuccess(UnityWebRequest request)
    {
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{request.method} {request.url} は失敗しました: {request.result} / {request.responseCode} / {request.error}");
            return false;
        }
        return true;
    }

    private static void LogMessages(MessageListData list)
    {
        string text = $"{list.messages.Length} 件のメッセージ";
        foreach (MessageData message in list.messages)
        {
            text += $"\n{Format(message)}";
        }
        Debug.Log(text);
    }

    private static string Format(MessageData message)
    {
        DateTimeOffset createdAt = DateTimeOffset.Parse(message.createdAt);
        return $"[{message.id}] {message.sender}: {message.text} ({createdAt.ToLocalTime():HH:mm:ss})";
    }
}
```

- **一覧を取得する**：受け取った JSON を `JsonUtility.FromJson<MessageListData>` で変換し、1 行に 1 つずつ出力します。
- **追加する**：`PostMessageData` を `JsonUtility.ToJson` で JSON にし、`Content-Type` を `application/json` にして送ります。応答の `201 Created` の本文には、追加されたメッセージの JSON が入っているので、それを変換して出力します。
- **検索する**：「7. URL に値を入れる」で説明します。

`Format` メソッドは、時刻を `DateTimeOffset.Parse` で変換し、`ToLocalTime` でパソコンの地域の時刻に直して表示します。サーバーは UTC で記録しているので、日本の時刻では 9 時間進んだ時刻になります。

Inspector ビューには、`Sender`、`Text`、`Keyword` が表示されます。既定値のまま、Play モードで **一覧を取得する**、**追加する**、**一覧を取得する** の順に選ぶと、Console ビューに次のように表示されます。実行結果の例です。時刻は実行するたびに変わります。

```
2 件のメッセージ
[1] Player1: おはよう (04:00:48)
[2] Player1: 1+1=2 (04:00:48)
```

```
追加しました: [3] Player2: こんにちは (04:00:54)
```

```
3 件のメッセージ
[1] Player1: おはよう (04:00:48)
[2] Player1: 1+1=2 (04:00:48)
[3] Player2: こんにちは (04:00:54)
```

Unity から送った日本語の本文が、文字化けせずにサーバーに保存されていることがわかります。`UnityWebRequest.Post` は、文字列の本文を UTF-8 で送ります。

---

## 7. URL に値を入れる

**検索する** は、`Keyword` の文字列を含むメッセージを、クエリ文字列の `contains` で検索します。既定値の `1+1` のまま選ぶと、次のように表示されます。

```
1 件のメッセージ
[2] Player1: 1+1=2 (04:00:48)
```

このとき、`Keyword` の値を、そのまま URL に入れてはいけません。URL では、いくつかの記号に特別な意味があるからです。

| 記号 | URL での意味 |
|---|---|
| `?` | パスとクエリ文字列の区切り |
| `&` | クエリ文字列の中の、`名前=値` の組の区切り |
| `=` | 名前と値の区切り |
| `#` | ページの中の位置（フラグメント）の始まり。サーバーには送られない |
| `+` | クエリ文字列では、空白を表す |

たとえば `1+1` をそのまま入れて `?contains=1+1` とすると、サーバーは `+` を空白と解釈し、`1 1` を含むメッセージを探します。その結果、1 件も見つかりません。

```json
{"messages":[]}
```

値に含まれる記号を、特別な意味を持たない書き方に変換することを**パーセントエンコーディング**（percent-encoding）と呼びます。記号を UTF-8 のバイト列にし、各バイトを `%` と 16 進数 2 桁で書き表します。`+` は `%2B` になります。

**書式：[Uri.EscapeDataString メソッド](https://learn.microsoft.com/dotnet/api/system.uri.escapedatastring)**
```csharp
public static string EscapeDataString(string stringToEscape);
```

`Uri.EscapeDataString` は、文字列をパーセントエンコーディングします。`SearchMessages` では、`Keyword` をこのメソッドに通してから URL に入れています。サーバーのログで、送られた URL を比べます。

```
--> GET /messages?contains=1%2B1
<-- 200
--> GET /messages?contains=1+1
<-- 200
```

1 つ目がエンコードした場合、2 つ目がエンコードしなかった場合です。

`こんにちは` のような日本語を、エンコードせずに URL に入れた場合、`UnityWebRequest` は自動で `%E3%81%93%E3%82%93...` のようにエンコードして送ります。しかし、`+` や `&` のような記号は、URL の区切りとして意味があるので、自動ではエンコードされません。クエリ文字列に入れる値は、いつも `Uri.EscapeDataString` を通します。

---

## よくあるミス

### Content-Type を text/plain のままにする

[Unity から通信する](/unity-csharp-learning/networking/unity-webrequest/) の `SendPost` のように、`UnityWebRequest.Post` の 3 つ目の引数を `"text/plain"` のままにすると、本文が JSON でも、サーバーは `415 Unsupported Media Type` を返します。

```csharp
// ❌ NG: Content-Type が text/plain なので、サーバーは JSON として読まない
using UnityWebRequest request = UnityWebRequest.Post($"{_baseUrl}/messages", json, "text/plain");
```

Console ビューには、`ProtocolError / 415 / HTTP/1.1 415 Unsupported Media Type` と表示されます。JSON を送るときは、`"application/json"` を指定します。

### フィールドの名前を JSON と合わせない

`MessageData` のフィールドを `Id` や `Sender` のように大文字で始めると、JSON の `id` や `sender` と名前が一致しないので、値が入りません。エラーにはならず、`0` や `null` のまま処理が進むので、気付きにくいミスです。`JsonUtility` で受け取るクラスのフィールド名は、JSON の名前と大文字、小文字まで同じにします。

---

## まとめ

- JSON は、オブジェクト（`{ }`）、配列（`[ ]`）、文字列、数値、真偽値、`null` でデータを書き表す形式。改行や記号を含むデータも、区切りと取り違えずに送れる
- ASP.NET Core のハンドラーは、オブジェクトを返すと JSON に変換し、クラスやレコードの引数には JSON の本文を変換して渡す。名前はキャメルケースになる
- `Content-Type` は本文の形式を伝えるヘッダー。JSON を送るときは `application/json` にする。合わないと `415` が返る
- Unity の `JsonUtility` は、`[Serializable]` のクラスの `public` なフィールドを変換する。最上位の配列、`DateTimeOffset`、プロパティは扱えず、名前の大文字と小文字を区別する
- 番号や時刻のように、サーバーが決めるべき値はクライアントから受け取らない
- クエリ文字列に入れる値は、`Uri.EscapeDataString` でパーセントエンコーディングする

---

## 理解度チェック

1. サーバーが一覧を `[...]` の配列のまま返さず、`{"messages":[...]}` のように包んで返しているのはなぜですか？
2. `PostMessageRequest` に `Id` や `CreatedAt` を含めていないのはなぜですか？
3. JSON の本文を `Content-Type: text/plain` で送ると、このページのサーバーは何番のステータスコードを返しますか？
4. `Keyword` が `Q&A` のとき、エンコードせずに `?contains=Q&A` とすると、サーバーの `contains` にはどんな値が入りますか？

<details markdown="1">
<summary>解答を見る</summary>

1. Unity の `JsonUtility` は、JSON の最上位に配列を置けないからです。オブジェクトで包めば、配列をフィールド（`messages`）として受け取れます。
2. 番号や時刻は、サーバーが決めるべき値だからです。クライアントが送った値を使うと、番号が重なったり、正しくない時刻が記録されたりするおそれがあります。
3. `415`（Unsupported Media Type）です。ASP.NET Core は、`Content-Type` が `application/json` のときだけ本文を JSON として読みます。
4. `Q` が入ります。`&` はクエリ文字列の中で `名前=値` の組を区切る記号なので、`A` は別の組の名前として扱われます。

</details>

---

## 次のステップ

[失敗に備える](/unity-csharp-learning/networking/failure-handling/) では、時間がかかりすぎるリクエストや、一時的に失敗するリクエストに備える方法を学びます。
