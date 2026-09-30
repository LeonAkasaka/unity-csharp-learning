---
layout: page
title: HTTP のメソッドとステータスコード
permalink: /networking/http-methods/
---

# HTTP のメソッドとステータスコード

[ASP.NET Core でサーバーを作る](/unity-csharp-learning/networking/aspnetcore-server/) では、`GET` のリクエストに決まった文字列を返すサーバーを作りました。このページでは、サーバーがメッセージの一覧を持ち、クライアントからの指示でメッセージを追加、取得、書き換え、削除できるようにします。そのために、`GET` 以外の HTTP の**メソッド**と、パスやクエリ文字列から値を受け取る方法、結果に応じた**ステータスコード**の返し方を学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `GET`、`POST`、`PUT`、`DELETE` のメソッドの意味と使い分けを説明できる
- パスの一部（ルートパラメーター）やクエリ文字列から値を受け取れる
- リクエストの本文を読める
- 処理の結果に応じて、`200`、`201`、`204`、`400`、`404` などのステータスコードを返せる
- 複数のリクエストから同じデータを使うときに、`lock` で守る必要がある理由を説明できる

## 前提知識

- [ASP.NET Core でサーバーを作る](/unity-csharp-learning/networking/aspnetcore-server/) を読んでいること。このページでは、そのページで作った `SampleHttpServer` を書き換えます
- [Dictionary\<TKey, TValue\>](/unity-csharp-learning/csharp/dictionary/) を読んでいること
- [共有データと lock](/unity-csharp-learning/csharp/thread-safety/) を読んでいること

---

## 1. リソースとメソッド

HTTP では、URL のパスで「何に対して」を、メソッドで「何をするか」を表します。パスが指す対象を**リソース**（resource）と呼びます。

このページでは、メッセージの一覧をリソースとして扱い、次のように決めます。

| メソッド | パス | すること | 成功したときのステータスコード |
|---|---|---|---|
| `GET` | `/messages` | メッセージの一覧を取得する | `200 OK` |
| `GET` | `/messages/{id}` | 番号が `id` のメッセージを取得する | `200 OK` |
| `POST` | `/messages` | メッセージを追加する | `201 Created` |
| `PUT` | `/messages/{id}` | 番号が `id` のメッセージを書き換える | `204 No Content` |
| `DELETE` | `/messages/{id}` | 番号が `id` のメッセージを削除する | `204 No Content` |

同じ `/messages` でも、`GET` なら取得、`POST` なら追加になります。パスには動詞（`/addMessage` など）を入れず、操作の種類はメソッドで区別するのが一般的な設計です。

各メソッドの意味は、次のとおりです。

| メソッド | 意味 | サーバーのデータを変えるか | 同じリクエストを何度送っても結果が同じか |
|---|---|---|---|
| `GET` | 取得する | 変えない | 同じ |
| `POST` | 新しく作る | 変える | 違う（送るたびに増える） |
| `PUT` | 指定したもので置き換える | 変える | 同じ |
| `DELETE` | 削除する | 変える | 同じ（2 回目は、もうないだけ） |

最後の列の性質は、通信に失敗してリクエストを送り直すかどうかを判断するときに重要になります。`PUT` や `DELETE` は送り直しても結果が変わりませんが、`POST` を送り直すと、同じメッセージが 2 つ追加されるかもしれません。エラー処理の回で、もう一度扱います。

---

## 2. メッセージを保存するクラス

まず、メッセージを保存しておくクラスを作ります。メッセージには、追加した順に 1 から番号を付け、番号とメッセージの組を `Dictionary<int, string>` に保存します。

[ASP.NET Core でサーバーを作る](/unity-csharp-learning/networking/aspnetcore-server/) の「7. 同時に複数の接続を処理する」で見たように、Kestrel は、複数のリクエストのハンドラーを別々のスレッドで同時に呼び出すことがあります。`Dictionary<TKey, TValue>` は、複数のスレッドから同時に書き換えると、中身が壊れることがあります。そこで、[共有データと lock](/unity-csharp-learning/csharp/thread-safety/) で学んだ `lock` 文を使い、一度に 1 つのスレッドだけがメッセージを読み書きするようにします。

`SampleHttpServer` の `Program.cs` の最後に、次のクラスを追加します。トップレベルのステートメントと同じファイルに型を宣言するときは、型の宣言をステートメントより後ろに書きます。

```csharp
class MessageStore
{
    private readonly Dictionary<int, string> _messages = new Dictionary<int, string>();
    private int _nextId = 1;

    public List<KeyValuePair<int, string>> GetAll()
    {
        lock (_messages)
        {
            return new List<KeyValuePair<int, string>>(_messages);
        }
    }

    public bool TryGet(int id, out string? text)
    {
        lock (_messages)
        {
            return _messages.TryGetValue(id, out text);
        }
    }

    public int Add(string text)
    {
        lock (_messages)
        {
            int id = _nextId;
            _nextId++;
            _messages.Add(id, text);
            return id;
        }
    }

    public bool TryUpdate(int id, string text)
    {
        lock (_messages)
        {
            if (!_messages.ContainsKey(id))
            {
                return false;
            }
            _messages[id] = text;
            return true;
        }
    }

    public bool TryRemove(int id)
    {
        lock (_messages)
        {
            return _messages.Remove(id);
        }
    }
}
```

`GetAll` は、`Dictionary` そのものではなく、中身をコピーした `List` を返します。`Dictionary` そのものを返すと、呼び出し側が `lock` の外で読んでいる最中に、別のスレッドが書き換えるおそれがあるからです。

---

## 3. 一覧を返す

`Program.cs` の上の部分を、次のように書き換えます。ログを出すミドルウェアはそのまま残し、前の回の `/` と `/hello` のハンドラーは削除します。

```csharp
using System.Text;

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

app.MapGet("/messages", () =>
{
    StringBuilder sb = new StringBuilder();
    foreach (KeyValuePair<int, string> pair in store.GetAll())
    {
        sb.Append($"{pair.Key}: {pair.Value}\n");
    }
    return sb.ToString();
});

app.Run("http://localhost:8080");
```

`store` は、`app.Run` でサーバーが動いている間、ずっと同じオブジェクトが使われます。すべてのリクエストのハンドラーが、この 1 つの `store` を共有します。

一覧は、1 行に 1 つずつ `番号: メッセージ` の形で並べた文字列にして返します。ハンドラーが文字列を返すと、`200 OK` の応答になります。サーバーを起動して、curl でアクセスします。

```powershell
curl.exe -i http://localhost:8080/messages
```

まだメッセージがないので、本文は空です。実行結果の例です。`Date` の日時は実行するたびに変わります。

```
HTTP/1.1 200 OK
Content-Type: text/plain; charset=utf-8
Date: Sun, 27 Sep 2026 17:30:59 GMT
Server: Kestrel
Transfer-Encoding: chunked

```

---

## 4. メッセージを追加する（POST）

メッセージを追加するハンドラーを、`app.Run` の前に追加します。

```csharp
app.MapPost("/messages", async (HttpRequest request) =>
{
    using StreamReader reader = new StreamReader(request.Body);
    string text = await reader.ReadToEndAsync();
    if (text == "")
    {
        return Results.BadRequest();
    }
    int id = store.Add(text);
    return Results.Created($"/messages/{id}", null);
});
```

**書式：[EndpointRouteBuilderExtensions.MapPost メソッド](https://learn.microsoft.com/dotnet/api/microsoft.aspnetcore.builder.endpointroutebuilderextensions.mappost)**
```csharp
public static RouteHandlerBuilder MapPost(this IEndpointRouteBuilder endpoints, string pattern, Delegate handler);
```

`MapPost` は、`POST` メソッドのリクエストに対するハンドラーを登録します。使い方は `MapGet` と同じです。

### 本文を読む

`POST` のリクエストでは、追加するメッセージを本文に入れて送ってもらいます。ハンドラーの引数に `HttpRequest` 型の引数を書くと、ASP.NET Core がリクエストの情報を渡してくれます。

[HttpRequest.Body プロパティ](https://learn.microsoft.com/dotnet/api/microsoft.aspnetcore.http.httprequest.body) は、本文を読むための `Stream` です。[TCP で送る](/unity-csharp-learning/networking/tcp-send/) で `NetworkStream` を読んだときと同じように、`StreamReader` の `ReadToEndAsync` で、本文をすべて文字列として読みます。本文の終わりは、ASP.NET Core が `Content-Length` や `Transfer-Encoding: chunked` を見て判断してくれるので、接続が閉じられるのを待つ必要はありません。

### 結果を返す

ハンドラーの結果を文字列以外の形で返すときは、[Results クラス](https://learn.microsoft.com/dotnet/api/microsoft.aspnetcore.http.results) のメソッドを使います。

**書式：[Results.Created メソッド](https://learn.microsoft.com/dotnet/api/microsoft.aspnetcore.http.results.created)**
```csharp
public static IResult Created(string? uri, object? value);
```

| パラメータ | 説明 |
|---|---|
| `uri` | 作ったリソースのパス。応答の `Location` ヘッダーになる |
| `value` | 本文にするデータ。`null` なら本文は空になる |

`Results.Created` は、`201 Created` の応答を返します。`201` は「リクエストによって新しいリソースを作った」ことを表します。`Location` ヘッダーで、作ったメッセージのパスをクライアントに知らせます。

**書式：[Results.BadRequest メソッド](https://learn.microsoft.com/dotnet/api/microsoft.aspnetcore.http.results.badrequest)**
```csharp
public static IResult BadRequest(object? error = null);
```

`Results.BadRequest` は、`400 Bad Request` の応答を返します。`400` は「リクエストの内容に問題がある」ことを表します。ここでは、本文が空のときに返しています。

### curl で追加する

サーバーを起動し直して、curl でメッセージを 2 つ追加します。`-X` でメソッドを、`-d` で本文を指定します。

```powershell
curl.exe -i -X POST -d "Hello" http://localhost:8080/messages
```

実行結果の例です。`Date` の日時は実行するたびに変わります。

```
HTTP/1.1 201 Created
Content-Length: 0
Date: Sun, 27 Sep 2026 17:31:35 GMT
Server: Kestrel
Location: /messages/1

```

```powershell
curl.exe -i -X POST -d "Good morning" http://localhost:8080/messages
```

2 つ目の応答では、`Location` が `/messages/2` になります。一覧を取得すると、2 つのメッセージが並んでいます。

```powershell
curl.exe http://localhost:8080/messages
```

```
1: Hello
2: Good morning
```

本文を付けずに `POST` を送ると、`400` が返ります。

```powershell
curl.exe -i -X POST http://localhost:8080/messages
```

```
HTTP/1.1 400 Bad Request
Content-Length: 0
Date: Sun, 27 Sep 2026 17:30:59 GMT
Server: Kestrel

```

> 💡 **ポイント**: `-d` を付けると、curl は `Content-Type: application/x-www-form-urlencoded`（Web ページのフォームの形式）というヘッダーを付けて送ります。このサーバーは `Content-Type` を見ずに、本文をそのまま 1 つのメッセージとして扱っています。本文の形式を決めてやり取りする方法は、次の回で扱います。

---

## 5. 1 つのメッセージを取得する（ルートパラメーター）

番号を指定して、1 つのメッセージを取得するハンドラーを追加します。

```csharp
app.MapGet("/messages/{id:int}", (int id) =>
{
    if (store.TryGet(id, out string? text))
    {
        return Results.Text(text);
    }
    return Results.NotFound();
});
```

パスのパターンに `{id:int}` と書くと、その部分に来た値を、同じ名前の引数 `id` で受け取れます。パスの一部を値として受け取るこの仕組みを、**ルートパラメーター**（route parameter）と呼びます。`/messages/2` なら、`id` に `2` が入ります。

`:int` は**ルート制約**（[route constraint](https://learn.microsoft.com/aspnet/core/fundamentals/routing#route-constraints)）で、「この部分は整数に限る」という条件です。`/messages/abc` のように整数でない値が来ると、このパターンには一致しないので、ほかのハンドラーにも一致しなければ `404` になります。

`Results.Text` は文字列を本文にした `200 OK` の応答を、[Results.NotFound](https://learn.microsoft.com/dotnet/api/microsoft.aspnetcore.http.results.notfound) は `404 Not Found` の応答を返します。

サーバーを起動し直すと、メッセージは消えています。メッセージはサーバーのメモリにしか保存していないので、サーバーを止めると失われるからです。前の節の 2 つの `POST` を送り直してから、試します。

```powershell
curl.exe http://localhost:8080/messages/2
```

```
Good morning
```

存在しない番号や、整数でない値を指定すると、`404` が返ります。サーバーのログで確かめます。

```powershell
curl.exe http://localhost:8080/messages/9
curl.exe http://localhost:8080/messages/abc
```

```
--> GET /messages/9
<-- 404
--> GET /messages/abc
<-- 404
```

同じ `404` でも、`/messages/9` はハンドラーが `Results.NotFound` を返したもの、`/messages/abc` はどのハンドラーにも一致しなかったものです。

---

## 6. 一覧を絞り込む（クエリ文字列）

一覧のハンドラーを、指定した文字列を含むメッセージだけを返せるように書き換えます。

```csharp
app.MapGet("/messages", (string? contains) =>
{
    StringBuilder sb = new StringBuilder();
    foreach (KeyValuePair<int, string> pair in store.GetAll())
    {
        if (contains == null || pair.Value.Contains(contains))
        {
            sb.Append($"{pair.Key}: {pair.Value}\n");
        }
    }
    return sb.ToString();
});
```

ハンドラーの引数に、パスのパターンに含まれない名前（ここでは `contains`）の単純な型の引数を書くと、ASP.NET Core は、同じ名前の**クエリ文字列**から値を取り出して渡します。クエリ文字列は、URL の `?` の後ろに `名前=値` の形で書きます。クエリ文字列がないときは、`null` が渡されます。

```powershell
curl.exe "http://localhost:8080/messages?contains=Good"
```

```
2: Good morning
```

ルートパラメーターとクエリ文字列は、どちらも URL で値を渡す方法です。一般に、どのリソースかを指すもの（メッセージの番号）はパスに、一覧の絞り込みや並べ方などの条件はクエリ文字列に入れます。

引数がどこから値を受け取るのかについて、詳しくは [Minimal API のパラメーター バインド](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis/parameter-binding) を参照してください。

> 💡 **ポイント**: クエリ文字列を含む URL は、`"` で囲んで渡します。クエリ文字列を `&` でつないで複数の値を渡すとき、`&` は PowerShell では特別な意味を持つ記号なので、囲まないと正しく渡せません。

---

## 7. 書き換える（PUT）と削除する（DELETE）

最後に、書き換えと削除のハンドラーを追加します。

```csharp
app.MapPut("/messages/{id:int}", async (int id, HttpRequest request) =>
{
    using StreamReader reader = new StreamReader(request.Body);
    string text = await reader.ReadToEndAsync();
    if (text == "")
    {
        return Results.BadRequest();
    }
    if (store.TryUpdate(id, text))
    {
        return Results.NoContent();
    }
    return Results.NotFound();
});

app.MapDelete("/messages/{id:int}", (int id) =>
{
    if (store.TryRemove(id))
    {
        return Results.NoContent();
    }
    return Results.NotFound();
});
```

[MapPut](https://learn.microsoft.com/dotnet/api/microsoft.aspnetcore.builder.endpointroutebuilderextensions.mapput) と [MapDelete](https://learn.microsoft.com/dotnet/api/microsoft.aspnetcore.builder.endpointroutebuilderextensions.mapdelete) は、それぞれ `PUT` と `DELETE` のハンドラーを登録します。ルートパラメーターと `HttpRequest` のように、ハンドラーには複数の引数を書けます。

**書式：[Results.NoContent メソッド](https://learn.microsoft.com/dotnet/api/microsoft.aspnetcore.http.results.nocontent)**
```csharp
public static IResult NoContent();
```

`Results.NoContent` は、`204 No Content` の応答を返します。`204` は「成功したが、返す本文はない」ことを表します。書き換えや削除は、成功したことさえ伝われば十分なので、`204` を返しています。

サーバーを起動し直し、2 つの `POST` を送ってから試します。

```powershell
curl.exe -i -X PUT -d "Good night" http://localhost:8080/messages/2
```

```
HTTP/1.1 204 No Content
Date: Sun, 27 Sep 2026 17:30:59 GMT
Server: Kestrel

```

```powershell
curl.exe -X DELETE http://localhost:8080/messages/1
curl.exe -X DELETE http://localhost:8080/messages/1
curl.exe http://localhost:8080/messages
```

```
2: Good night
```

サーバーのログを見ると、1 回目の `DELETE` は `204`、2 回目の `DELETE` は、もう番号 1 のメッセージがないので `404` になっています。

```
--> PUT /messages/2
<-- 204
--> DELETE /messages/1
<-- 204
--> DELETE /messages/1
<-- 404
--> GET /messages
<-- 200
```

### 登録していないメソッド

`/messages/2` に、ハンドラーを登録していないメソッド（ここでは `PATCH`）を送ってみます。

```powershell
curl.exe -i -X PATCH http://localhost:8080/messages/2
```

```
HTTP/1.1 405 Method Not Allowed
Content-Length: 0
Date: Sun, 27 Sep 2026 17:30:59 GMT
Server: Kestrel
Allow: DELETE, GET, PUT

```

パスに一致するハンドラーはあるが、メソッドが一致しないときは、ASP.NET Core が自動で `405 Method Not Allowed` を返します。`Allow` ヘッダーには、このパスで使えるメソッドが並んでいます。

---

## 8. ステータスコードのまとめ

ステータスコードは、1 桁目で大まかな意味がわかるように決められています。

| 範囲 | 意味 |
|---|---|
| `2xx` | 成功した |
| `3xx` | 別の場所を見てほしい（リダイレクト） |
| `4xx` | クライアントのリクエストに問題がある |
| `5xx` | サーバーの側で問題が起きた |

このページで使ったステータスコードは次のとおりです。

| ステータスコード | 意味 | このページで返した場面 |
|---|---|---|
| `200 OK` | 成功した | 一覧や 1 つのメッセージを返した |
| `201 Created` | 新しいリソースを作った | `POST` でメッセージを追加した |
| `204 No Content` | 成功したが、返す本文はない | `PUT` で書き換えた、`DELETE` で削除した |
| `400 Bad Request` | リクエストの内容に問題がある | 本文が空だった |
| `404 Not Found` | 対象が見つからない | 番号のメッセージがない、パスに一致するハンドラーがない |
| `405 Method Not Allowed` | そのメソッドは使えない | ハンドラーを登録していないメソッドが来た |

ハンドラーの中で例外がスローされ、キャッチされなかったときは、`500 Internal Server Error` が返ります。`500` は、サーバーのプログラムに不具合があることを表します。

クライアントは、本文を読む前にステータスコードを見れば、リクエストがどうなったのかを判断できます。`4xx` ならリクエストを直す必要があり、`5xx` ならサーバーの問題なので、しばらく待って送り直すと成功するかもしれません。Unity からサーバーと通信するときも、まずステータスコードで結果を判断します。

---

## 完成したコード

このページで作った `Program.cs` の全体です。

```csharp
using System.Text;

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
    StringBuilder sb = new StringBuilder();
    foreach (KeyValuePair<int, string> pair in store.GetAll())
    {
        if (contains == null || pair.Value.Contains(contains))
        {
            sb.Append($"{pair.Key}: {pair.Value}\n");
        }
    }
    return sb.ToString();
});

app.MapGet("/messages/{id:int}", (int id) =>
{
    if (store.TryGet(id, out string? text))
    {
        return Results.Text(text);
    }
    return Results.NotFound();
});

app.MapPost("/messages", async (HttpRequest request) =>
{
    using StreamReader reader = new StreamReader(request.Body);
    string text = await reader.ReadToEndAsync();
    if (text == "")
    {
        return Results.BadRequest();
    }
    int id = store.Add(text);
    return Results.Created($"/messages/{id}", null);
});

app.MapPut("/messages/{id:int}", async (int id, HttpRequest request) =>
{
    using StreamReader reader = new StreamReader(request.Body);
    string text = await reader.ReadToEndAsync();
    if (text == "")
    {
        return Results.BadRequest();
    }
    if (store.TryUpdate(id, text))
    {
        return Results.NoContent();
    }
    return Results.NotFound();
});

app.MapDelete("/messages/{id:int}", (int id) =>
{
    if (store.TryRemove(id))
    {
        return Results.NoContent();
    }
    return Results.NotFound();
});

app.Run("http://localhost:8080");

class MessageStore
{
    private readonly Dictionary<int, string> _messages = new Dictionary<int, string>();
    private int _nextId = 1;

    public List<KeyValuePair<int, string>> GetAll()
    {
        lock (_messages)
        {
            return new List<KeyValuePair<int, string>>(_messages);
        }
    }

    public bool TryGet(int id, out string? text)
    {
        lock (_messages)
        {
            return _messages.TryGetValue(id, out text);
        }
    }

    public int Add(string text)
    {
        lock (_messages)
        {
            int id = _nextId;
            _nextId++;
            _messages.Add(id, text);
            return id;
        }
    }

    public bool TryUpdate(int id, string text)
    {
        lock (_messages)
        {
            if (!_messages.ContainsKey(id))
            {
                return false;
            }
            _messages[id] = text;
            return true;
        }
    }

    public bool TryRemove(int id)
    {
        lock (_messages)
        {
            return _messages.Remove(id);
        }
    }
}
```

---

## よくあるミス

### ルートパラメーターに制約を付け忘れる

`{id:int}` の `:int` を付けずに `{id}` と書いても、`/messages/2` は同じように動きます。しかし、`/messages/abc` にアクセスすると、結果が変わります。

```csharp
// ❌ NG: 制約がないので、整数でない値もこのパターンに一致する
app.MapGet("/messages/{id}", (int id) =>
{
    // （中身は同じ）
});
```

`/messages/abc` がこのパターンに一致し、ASP.NET Core は `abc` を `int` の引数 `id` に変換しようとして失敗します。その結果、`400 Bad Request` の応答と一緒に、開発者向けのエラーの詳細が本文として返されます。また、変換の失敗は例外としてミドルウェアを通り抜けるので、ログの `<--` の行が出力されません。

```
--> GET /messages/abc
fail: Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware[1]
```

ルートパラメーターの型が決まっているときは、制約を付けて、一致しない値をパターンの段階で除外します。

---

## まとめ

- HTTP では、パスでリソースを、メソッドで操作を表す。`GET` は取得、`POST` は追加、`PUT` は置き換え、`DELETE` は削除
- `GET`、`PUT`、`DELETE` は、同じリクエストを何度送っても結果が変わらない。`POST` は送るたびに結果が変わる
- `MapPost`、`MapPut`、`MapDelete` で、メソッドごとにハンドラーを登録する
- パスの `{id:int}` はルートパラメーターで、同じ名前の引数で受け取る。`:int` はルート制約
- パスのパターンにない名前の引数は、クエリ文字列から値を受け取る
- 本文は `HttpRequest.Body` の `Stream` から読む
- `Results` クラスのメソッドで、`201`、`204`、`400`、`404` などのステータスコードを返す。ステータスコードの 1 桁目は、成功（2）、クライアントの問題（4）、サーバーの問題（5）などを表す
- ハンドラーは同時に呼び出されることがあるので、共有するデータは `lock` で守る

---

## 理解度チェック

1. メッセージの番号 3 を削除するリクエストの、メソッドとパスを答えてください。
2. `POST /messages` を 2 回送るのと、`PUT /messages/1` を 2 回送るのとでは、サーバーのデータにどのような違いが出ますか？
3. このページのサーバーに `curl.exe -X DELETE http://localhost:8080/messages` を送ると、ステータスコードはいくつになりますか？
4. `MessageStore` のメソッドで `lock` を使わないと、どのような問題が起きる可能性がありますか？

<details markdown="1">
<summary>解答を見る</summary>

1. メソッドは `DELETE`、パスは `/messages/3` です。
2. `POST` を 2 回送ると、同じ内容のメッセージが 2 つ追加されます。`PUT` を 2 回送っても、番号 1 のメッセージが同じ内容で 2 回置き換えられるだけなので、1 回送ったときと結果は変わりません。
3. `405` です。`/messages` には `GET` と `POST` のハンドラーがありますが、`DELETE` のハンドラーはないので、ASP.NET Core が `405 Method Not Allowed` を返します。
4. Kestrel は複数のリクエストのハンドラーを別々のスレッドで同時に呼び出すことがあるので、`Dictionary` が同時に書き換えられて中身が壊れたり、2 つのメッセージに同じ番号が付いたりする可能性があります。

</details>

---

## 次のステップ

[Unity から通信する](/unity-csharp-learning/networking/unity-webrequest/) では、Unity から `UnityWebRequest` を使って、このページで作ったサーバーにリクエストを送ります。

ブラウザで開いて Web ページとして表示される HTML を返す方法と、そのときに気をつけるインジェクションの危険は、[ブラウザに HTML を返す（補足）](/unity-csharp-learning/networking/html-response/) で扱います。
