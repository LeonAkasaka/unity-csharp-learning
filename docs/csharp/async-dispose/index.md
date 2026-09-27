---
layout: page
title: IAsyncDisposable と await using
permalink: /csharp/async-dispose/
---

# IAsyncDisposable と await using

[IDisposable と using](/unity-csharp-learning/csharp/dispose-using/) で学んだ `Dispose` は、戻り値のないふつうのメソッドです。後片付けに時間がかかる場合、`Dispose` の中で待つと、呼び出したスレッドが止まってしまいます。**IAsyncDisposable** は、後片付けを非同期に行う `DisposeAsync` メソッドを宣言したインターフェイスです。**await using** 文を使うと、`DisposeAsync` を確実に呼び出し、その完了を `await` で待てます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- 後片付けを非同期に行う必要がある理由を説明できる
- `IAsyncDisposable` を実装し、`DisposeAsync` に非同期の後片付けを書ける
- `await using` 文と `await using` 宣言で、`DisposeAsync` を確実に呼び出せる
- `await using` が `try` / `finally` と `await DisposeAsync()` に置き換えられることを説明できる

## 前提知識

- [IAsyncEnumerable と await foreach](/unity-csharp-learning/csharp/async-streams/) を読んでいること
- [IDisposable と using](/unity-csharp-learning/csharp/dispose-using/) を読んでいること

---

## 1. 後片付けに時間がかかるとき

ファイルに書き込むクラスの多くは、書き込むデータをいったんメモリにためておき、ある程度たまってからまとめてファイルに書き出します。1 バイトずつファイルに書き出すより、まとめて書き出すほうが速いからです。そのため、ファイルを閉じるときには、まだメモリに残っているデータをファイルに書き出してから閉じる必要があります。通信の接続を閉じるときも、相手に「接続を終える」ことを伝えて、応答を待つことがあります。

このような後片付けは、ファイルやネットワークの読み書きを待つ処理です。[スレッドを使わずに待つ](/unity-csharp-learning/csharp/async-without-threads/) で学んだように、待つ処理は `await` でスレッドを手放しながら待つのが理想です。しかし、`IDisposable` の `Dispose` は `void` を返すメソッドなので、`Dispose` の中で待つと、その間、呼び出したスレッドが止まります。

そこで、後片付けを非同期に行うための [IAsyncDisposable インターフェイス](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable) が用意されています。

**書式：[IAsyncDisposable.DisposeAsync メソッド](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable.disposeasync)**
```csharp
public interface IAsyncDisposable
{
    ValueTask DisposeAsync();
}
```

`DisposeAsync` は、後片付けの完了を表す `ValueTask` を返します。戻り値が `Task` ではなく `ValueTask` なのは、[ValueTask](/unity-csharp-learning/csharp/value-task/) で学んだ理由によります。後片付けするものが残っていないなど、中断せずに終わることも多いからです。

---

## 2. IAsyncDisposable を実装する

[IDisposable と using](/unity-csharp-learning/csharp/dispose-using/) の `A` クラスを、`IAsyncDisposable` を実装するように書き換えます。`DisposeAsync` は async メソッドにして、時間のかかる後片付けの代わりに `Task.Delay` を `await` します。

```csharp
A a = new A("a");
a.M();
await a.DisposeAsync();
Console.WriteLine("終了");

class A : IAsyncDisposable
{
    private string _name;

    public A(string name)
    {
        _name = name;
    }

    public void M()
    {
        Console.WriteLine($"{_name}.M");
    }

    public async ValueTask DisposeAsync()
    {
        Console.WriteLine($"{_name}.DisposeAsync 開始");
        await Task.Delay(100);
        Console.WriteLine($"{_name}.DisposeAsync 終了");
    }
}
```

```
a.M
a.DisposeAsync 開始
a.DisposeAsync 終了
終了
```

`DisposeAsync` が返した `ValueTask` を `await` しているので、後片付けが終わってから `終了` が表示されます。後片付けを待っている間、スレッドは手放されています。

ただし、`Dispose` を自分で呼ぶときと同じように、`M` で例外が発生すると `DisposeAsync` は呼ばれません。

---

## 3. await using 文

`using` 文が `Dispose` を確実に呼ぶのと同じように、**await using 文** は `DisposeAsync` を確実に呼び、その完了を `await` します。

**書式：[await using 文](https://learn.microsoft.com/dotnet/csharp/language-reference/statements/using)**
```
await using (型 変数名 = 式)
{
    // 変数を使う処理
}
```

| 要素 | 説明 |
|---|---|
| `型` | `IAsyncDisposable` を実装した型 |
| `式` | 後片付けが必要なオブジェクトを作る式 |

ブロックを抜けるときに、`DisposeAsync` が呼ばれ、その完了を `await` します。以降のコード例の `A` クラスは、前の節と同じものを使います。

```csharp
await using (A a = new A("a"))
{
    a.M();
}
Console.WriteLine("終了");

// A クラスは前のコード例と同じ
```

```
a.M
a.DisposeAsync 開始
a.DisposeAsync 終了
終了
```

`await using` は `await` を含むので、`await` と同じく、async メソッドかトップレベルステートメントの中で使います。

### await using 宣言

`using` 宣言と同じように、変数の宣言の前に `await using` を付ける **await using 宣言** も使えます。変数を宣言したスコープの終わりで、`DisposeAsync` が呼ばれます。複数の変数を宣言したときは、宣言と逆の順に後片付けされます。

```csharp
await RunAsync();
Console.WriteLine("RunAsync から戻った");

async Task RunAsync()
{
    await using A a = new A("a");
    await using A b = new A("b");
    a.M();
    b.M();
}

// A クラスは前のコード例と同じ
```

```
a.M
b.M
b.DisposeAsync 開始
b.DisposeAsync 終了
a.DisposeAsync 開始
a.DisposeAsync 終了
RunAsync から戻った
```

`RunAsync` の終わりで、後から宣言した `b` の `DisposeAsync` が先に呼ばれます。`b` の後片付けが終わってから `a` の後片付けが始まり、両方が終わってから `RunAsync` が完了します。

---

## 4. await using の正体

コンパイラーは `await using` 文を、おおよそ次のような `try` / `finally` に置き換えます。[IDisposable と using](/unity-csharp-learning/csharp/dispose-using/) の `using` 文の正体と、`Dispose()` が `await DisposeAsync()` になっている点だけが違います。

```
{
    A a = new A("a");
    try
    {
        // ブロックの中の処理
    }
    finally
    {
        if (a != null)
        {
            await a.DisposeAsync();
        }
    }
}
```

[IAsyncEnumerable と await foreach](/unity-csharp-learning/csharp/async-streams/) で学んだ `await foreach` の正体も、最後に `finally` で `await enumerator.DisposeAsync()` を呼んでいました。`IAsyncEnumerator<T>` は `IAsyncDisposable` を実装しているので、`await foreach` は、取り出し役に対して `await using` と同じ後片付けをしているのです。

`finally` に置き換えられるので、ブロックの中で例外が発生しても、`DisposeAsync` は呼ばれます。次のコードは、前のコード例とは別のプログラムです。

```csharp
try
{
    await using A a = new A("a");
    a.M();
    throw new InvalidOperationException("途中で失敗");
}
catch (InvalidOperationException e)
{
    Console.WriteLine($"catch: {e.Message}");
}

// A クラスは前のコード例と同じ
```

```
a.M
a.DisposeAsync 開始
a.DisposeAsync 終了
catch: 途中で失敗
```

例外が `try` ブロックを抜ける前に、`a` の `DisposeAsync` が呼ばれ、その完了を待ってから `catch` に進んでいます。

---

## 5. IDisposable と IAsyncDisposable の両方を実装するクラス

.NET のファイルやネットワークのクラスの多くは、`IDisposable` と `IAsyncDisposable` の両方を実装しています。呼び出し元は、async メソッドの中なら `await using`、そうでなければ `using` と、場面に合わせて選べます。

たとえば、ファイルに文字列を書き込む [StreamWriter クラス](https://learn.microsoft.com/dotnet/api/system.io.streamwriter) は、両方を実装しています。`StreamWriter` は、書き込んだ文字列をメモリにためておき、閉じるときに残りをファイルに書き出します。`await using` で使うと、この書き出しを待つ間もスレッドを手放せます。[WriteLineAsync メソッド](https://learn.microsoft.com/dotnet/api/system.io.streamwriter.writelineasync) は、1 行の文字列を非同期に書き込みます。

```csharp
string path = Path.GetTempFileName();

await using (StreamWriter writer = new StreamWriter(path))
{
    await writer.WriteLineAsync("1 行目");
    await writer.WriteLineAsync("2 行目");
}

string text = await File.ReadAllTextAsync(path);
Console.Write(text);

File.Delete(path);
```

```
1 行目
2 行目
```

`await using` 文のブロックを抜けるときに `DisposeAsync` が呼ばれ、メモリに残っていた文字列がファイルに書き出されてから、ファイルが閉じられます。そのため、ブロックの後の `File.ReadAllTextAsync` では、書き込んだ 2 行を読み込めます。

---

## よくあるミス

### using と await using を取り違える

`using` 文に書けるのは `IDisposable` を実装した型、`await using` 文に書けるのは `IAsyncDisposable` を実装した型です。取り違えると、コンパイルエラーになります。

```csharp
// ❌ NG: A は IAsyncDisposable だけを実装しているので、using では使えない（CS8418）
// using (A a = new A("a"))
// {
// }

// ❌ NG: B は IDisposable だけを実装しているので、await using では使えない（CS8417）
// await using (B b = new B())
// {
// }
```

どちらのエラーメッセージでも、もう一方の書き方ではないかと指摘されます。両方を実装しているクラスなら、どちらでも使えます。

---

## ワンポイントアドバイス

### DisposeAsync を実装するときの約束

[IDisposable と using](/unity-csharp-learning/csharp/dispose-using/) で学んだ `Dispose` の約束は、`DisposeAsync` にも当てはまります。`DisposeAsync` は 2 回以上呼ばれても問題なく動くようにし、後片付けのあとでメソッドを呼ばれたら `ObjectDisposedException` を投げます。

`IDisposable` と `IAsyncDisposable` の両方を実装するときは、どちらが呼ばれても同じ後片付けになるようにします。呼び出し元はどちらか一方だけを呼ぶので、`DisposeAsync` から `Dispose` を呼ぶ必要はありません。

---

## まとめ

- ファイルへの書き出しや通信の終了など、後片付けには待つ処理が含まれることがある。`Dispose` の中で待つと、呼び出したスレッドが止まる
- `IAsyncDisposable` は、`ValueTask` を返す `DisposeAsync` メソッドを宣言したインターフェイス
- `await using` 文は、ブロックを抜けるときに `DisposeAsync` を呼び、その完了を `await` する
- `await using` 宣言では、スコープの終わりで、宣言と逆の順に `DisposeAsync` が呼ばれる
- `await using` は `try` / `finally` と `await DisposeAsync()` に置き換えられるので、例外が発生しても後片付けされる
- `await foreach` は、取り出し役の `IAsyncEnumerator<T>` に対して、`await using` と同じ後片付けをしている
- `using` には `IDisposable`、`await using` には `IAsyncDisposable` を実装した型を書く

---

## 理解度チェック

1. `IDisposable` の `Dispose` で時間のかかる後片付けをすると、どのような問題がありますか？
2. 次のコードを実行すると何が出力されますか？`A` クラスは、このページの 2 節と同じものとします。

   ```csharp
   await using (A x = new A("x"))
   {
       await using A y = new A("y");
       y.M();
   }
   ```

3. `await using` 文のブロックの中で例外が発生したとき、`DisposeAsync` は呼ばれますか？理由も説明してください。

<details markdown="1">
<summary>解答を見る</summary>

1. `Dispose` は `void` を返すメソッドなので、後片付けを待つ間、呼び出したスレッドが止まってしまいます。`IAsyncDisposable` の `DisposeAsync` なら、`await` でスレッドを手放しながら待てます。
2. 次のように出力されます。`y` はブロックの中で宣言されているので、ブロックの終わりで `x` より先に後片付けされます。`y` の `DisposeAsync` が終わってから、`x` の `DisposeAsync` が始まります。

   ```
   y.M
   y.DisposeAsync 開始
   y.DisposeAsync 終了
   x.DisposeAsync 開始
   x.DisposeAsync 終了
   ```

3. 呼ばれます。`await using` 文はコンパイラーによって `try` / `finally` に置き換えられ、`DisposeAsync` は `finally` の中で `await` されるからです。

</details>

---

## 次のステップ

これで、C# の非同期処理のセクションは終わりです。[C# 言語入門](/unity-csharp-learning/csharp/) の目次に戻って、ほかのトピックも確認してみましょう。
