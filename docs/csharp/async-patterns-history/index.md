---
layout: page
title: 非同期処理のパターンの変遷（補足）
permalink: /csharp/async-patterns-history/
---

# 非同期処理のパターンの変遷（補足）

時間のかかる処理を呼び出し元の流れとは別に進め、完了したら知らせる処理を **非同期処理**（asynchronous processing）といいます。.NET では、非同期処理を行うメソッドの形が、バージョンとともに 3 つのパターンへと移り変わってきました。このページでは、それぞれのパターンの形と、`Task` と `async` / `await` にたどり着くまでの流れを紹介します。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- APM・EAP・TAP の 3 つのパターンで、メソッドの形がどう違うかを説明できる
- 古いコードや資料で `BeginXxx` / `EndXxx` や `XxxCompleted` イベントを見かけたとき、どのパターンかを見分けられる
- TAP が現在の標準になった理由を説明できる

## 前提知識

- [継続と ContinueWith](/unity-csharp-learning/csharp/task-continuation/) を読んでいること
- [イベント](/unity-csharp-learning/csharp/events/) を読んでいること

---

## 1. 3 つのパターン

.NET の非同期処理のパターンには、次の 3 つがあります。

| パターン | 登場したバージョン | メソッドの形 |
|---|---|---|
| APM（Asynchronous Programming Model） | .NET Framework 1.0 | `BeginXxx` と `EndXxx` の組 |
| EAP（Event-based Asynchronous Pattern） | .NET Framework 2.0 | `XxxAsync` メソッドと `XxxCompleted` イベントの組 |
| TAP（Task-based Asynchronous Pattern） | .NET Framework 4.0 | `Task` を返す `XxxAsync` メソッド |

C# 5（.NET Framework 4.5）で `async` と `await` が追加され、TAP のメソッドを上から順に読める形で呼び出せるようになりました。現在、新しく作られる非同期のメソッドはすべて TAP です。APM と EAP は、古いクラスや古いコードに残っているだけです。

---

## 2. APM：BeginXxx と EndXxx

APM では、1 つの処理を、開始する `BeginXxx` メソッドと、結果を受け取る `EndXxx` メソッドの組で表します。たとえば [Stream クラス](https://learn.microsoft.com/dotnet/api/system.io.stream) には、読み込みを行う `Read` メソッドの APM 版として、`BeginRead` と `EndRead` があります。

**書式：[Stream.BeginRead メソッド](https://learn.microsoft.com/dotnet/api/system.io.stream.beginread)**
```csharp
public virtual IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state);
```

**書式：[Stream.EndRead メソッド](https://learn.microsoft.com/dotnet/api/system.io.stream.endread)**
```csharp
public virtual int EndRead(IAsyncResult asyncResult);
```

| 要素 | 説明 |
|---|---|
| `callback` | 処理が完了したときに呼ばれるメソッド。[AsyncCallback](https://learn.microsoft.com/dotnet/api/system.asynccallback) は、`IAsyncResult` を 1 つ受け取るデリゲート型 |
| `state` | コールバックに渡したい任意のオブジェクト。使わないときは `null` |
| [IAsyncResult](https://learn.microsoft.com/dotnet/api/system.iasyncresult) | 開始した処理を表すオブジェクト。`EndRead` に渡して結果を受け取る |

`BeginRead` は、読み込みを開始するとすぐに戻ります。読み込みが完了すると `callback` が呼ばれるので、その中で `EndRead` を呼んで、読み込んだバイト数を受け取ります。読み込みで例外が発生していれば、`EndRead` がその例外を投げます。

次のコードは、[MemoryStream クラス](https://learn.microsoft.com/dotnet/api/system.io.memorystream)（メモリ上のバイト配列を読み書きする `Stream`）から、APM で 5 バイト読み込みます。

```csharp
byte[] data = { 10, 20, 30, 40, 50 };
using MemoryStream stream = new MemoryStream(data);
using CountdownEvent done = new CountdownEvent(1);

byte[] buffer = new byte[data.Length];
stream.BeginRead(buffer, 0, buffer.Length, result =>
{
    int count = stream.EndRead(result);
    Console.WriteLine($"{count} バイト読み込んだ: {string.Join(", ", buffer)}");
    done.Signal();
}, null);

done.Wait();
Console.WriteLine("Main: 終了");
```

```
5 バイト読み込んだ: 10, 20, 30, 40, 50
Main: 終了
```

完了後の処理をコールバックに書く点は、[継続と ContinueWith](/unity-csharp-learning/csharp/task-continuation/) の `ContinueWith` と同じです。ただし、開始した処理を表す `IAsyncResult` からは、`Task` のように続けて継続を登録することも、完了を待つこともできません。このコードでも、完了を待つために [スレッドプール](/unity-csharp-learning/csharp/thread-pool/) で使った `CountdownEvent` を用意しています。

---

## 3. EAP：XxxAsync と XxxCompleted イベント

EAP では、処理を開始する `XxxAsync` メソッドと、完了を知らせる `XxxCompleted` イベントの組で表します。完了後の処理は、[イベント](/unity-csharp-learning/csharp/events/) のイベントハンドラーとして登録します。結果や例外は、イベントの引数から受け取ります。

EAP の例として、[BackgroundWorker クラス](https://learn.microsoft.com/dotnet/api/system.componentmodel.backgroundworker) を使います。`BackgroundWorker` は、[RunWorkerAsync メソッド](https://learn.microsoft.com/dotnet/api/system.componentmodel.backgroundworker.runworkerasync) を呼ぶと、`DoWork` イベントに登録した処理をスレッドプールで実行し、終わると [RunWorkerCompleted イベント](https://learn.microsoft.com/dotnet/api/system.componentmodel.backgroundworker.runworkercompleted) を発生させます。`BackgroundWorker` は `System.ComponentModel` 名前空間にあるので、`using` ディレクティブが必要です。

```csharp
using System.ComponentModel;

using BackgroundWorker worker = new BackgroundWorker();
using CountdownEvent done = new CountdownEvent(1);

worker.DoWork += (sender, e) =>
{
    int sum = 0;
    for (int i = 1; i <= 100; i++)
    {
        sum += i;
    }
    e.Result = sum;
};
worker.RunWorkerCompleted += (sender, e) =>
{
    if (e.Error != null)
    {
        Console.WriteLine($"エラー: {e.Error.Message}");
    }
    else
    {
        Console.WriteLine($"結果 = {e.Result}");
    }
    done.Signal();
};

worker.RunWorkerAsync();
done.Wait();
Console.WriteLine("Main: 終了");
```

```
結果 = 5050
Main: 終了
```

`DoWork` の中で計算した結果を `e.Result` に入れると、`RunWorkerCompleted` のイベントハンドラーで `e.Result` として受け取れます。`DoWork` の中で例外が発生したときは、例外は投げられず、`e.Error` に入ります。

EAP は、イベントという慣れた仕組みで完了を受け取れるので、APM より扱いやすくなりました。特に、画面を持つアプリケーションで、画面を操作するスレッドにイベントを届ける仕組みを備えていたため、よく使われました。しかし、完了後の処理をイベントハンドラーに書く点は変わらず、処理をつなげると、イベントハンドラーの中で次の処理を開始することになります。`e.Result` は `object?` 型なので、結果を受け取るときにキャストも必要です。

---

## 4. TAP：Task を返すメソッド

TAP では、非同期のメソッドは `Task` か `Task<T>` を返し、名前の末尾に `Async` を付けます。`Stream` の `Read` なら、TAP 版は [ReadAsync メソッド](https://learn.microsoft.com/dotnet/api/system.io.stream.readasync) です。

```csharp
byte[] data = { 10, 20, 30, 40, 50 };
using MemoryStream stream = new MemoryStream(data);

byte[] buffer = new byte[data.Length];
Task<int> task = stream.ReadAsync(buffer, 0, buffer.Length);
int count = task.Result;
Console.WriteLine($"{count} バイト読み込んだ: {string.Join(", ", buffer)}");
```

```
5 バイト読み込んだ: 10, 20, 30, 40, 50
```

開始した処理が `Task<int>` というオブジェクトで返ってくるので、[Task と Task\<T\>](/unity-csharp-learning/csharp/tasks/) で学んだ方法で、完了を待つ・結果を受け取る・例外を受け止める・継続を登録する、のどれもできます。完了を待つための `CountdownEvent` も、結果を受け取るための `EndRead` も必要ありません。ここでは結果を `Result` で受け取っていますが、次のページで学ぶ `await` を使えば、スレッドを止めずに受け取れます。

APM や EAP のメソッドしかない古いクラスも、TAP の形に変換して使えます。APM は [TaskFactory.FromAsync メソッド](https://learn.microsoft.com/dotnet/api/system.threading.tasks.taskfactory.fromasync) で、EAP は [TaskCompletionSource\<TResult\> クラス](https://learn.microsoft.com/dotnet/api/system.threading.tasks.taskcompletionsource-1) で `Task` に包めます。`TaskCompletionSource<TResult>` は、[スレッドを使わずに待つ](/unity-csharp-learning/csharp/async-without-threads/) で使います。

---

## 5. 3 つのパターンの比較

| | APM | EAP | TAP |
|---|---|---|---|
| 開始 | `BeginXxx` | `XxxAsync` | `XxxAsync` |
| 完了の知らせ方 | `AsyncCallback` を呼ぶ | `XxxCompleted` イベントを発生させる | 返した `Task` が完了する |
| 結果の受け取り方 | `EndXxx` の戻り値 | イベントの引数（`e.Result` など） | `Result` や `await` |
| 例外の受け取り方 | `EndXxx` が投げる | イベントの引数（`e.Error`） | `Wait` や `Result`、`await` で投げられる |
| 完了を待つ | 自分で用意する | 自分で用意する | `Wait` や `await` |

EAP と TAP はどちらもメソッド名が `XxxAsync` ですが、戻り値が `void` なら EAP、`Task` なら TAP です。

APM と EAP では、処理を開始するメソッドと、完了を受け取る場所（コールバックやイベントハンドラー）が分かれていました。TAP では、開始した処理が `Task` という 1 つのオブジェクトとして手元に返ってきます。オブジェクトなので、変数に入れたり、配列に入れてまとめて待ったり、メソッドの戻り値として返したりできます。この扱いやすさが、`async` と `await` の土台になりました。

---

## ワンポイントアドバイス

### デリゲートの BeginInvoke は使えない

.NET Framework では、どのデリゲートにも `BeginInvoke` / `EndInvoke` メソッドがあり、デリゲートが指すメソッドを APM でスレッドプールに実行させられました。古い資料にはこの使い方がよく出てきますが、現在の .NET では、呼び出すと `PlatformNotSupportedException` が投げられます。代わりに `Task.Run` を使います。

---

## まとめ

- .NET の非同期処理のパターンは、APM（.NET Framework 1.0）→ EAP（.NET Framework 2.0）→ TAP（.NET Framework 4.0）と移り変わった
- APM は `BeginXxx` / `EndXxx` の組で、完了を `AsyncCallback` で知らせる
- EAP は `XxxAsync` メソッドと `XxxCompleted` イベントの組で、結果や例外をイベントの引数で渡す
- TAP は `Task` を返す `XxxAsync` メソッドで、完了・結果・例外を `Task` オブジェクトで扱う
- 新しく書くコードでは TAP を使う。APM や EAP は、`FromAsync` や `TaskCompletionSource<TResult>` で `Task` に変換できる

---

## 理解度チェック

1. `DownloadAsync` という名前のメソッドが、EAP と TAP のどちらのパターンかを見分けるには、どこを見ればよいですか？
2. EAP の `BackgroundWorker` で、`DoWork` の中で発生した例外は、どこで受け取れますか？
3. APM と比べて、TAP のメソッドが扱いやすい理由を 1 つ挙げてください。

<details markdown="1">
<summary>解答を見る</summary>

1. 戻り値の型を見ます。`void` を返し、完了を `DownloadCompleted` のようなイベントで知らせるなら EAP、`Task` や `Task<T>` を返すなら TAP です。
2. `RunWorkerCompleted` イベントのイベントハンドラーで、引数の `e.Error` から受け取れます。例外は投げられません。
3. 開始した処理が `Task` オブジェクトとして返ってくるので、完了を待つ仕組みを自分で用意しなくても `Wait` や `await` で待てることです。ほかに、結果を `EndXxx` を呼ばずに `Result` や `await` で受け取れること、継続を登録してつなげられることも挙げられます。

</details>

---

## 次のステップ

[async と await](/unity-csharp-learning/csharp/async-await/) では、TAP のメソッドを、スレッドを止めずに上から順に読める形で呼び出す方法を学びます。
