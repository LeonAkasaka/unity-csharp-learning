---
layout: page
title: IDisposable と using
permalink: /csharp/dispose-using/
---

# IDisposable と using

**IDisposable** は、使い終わったときの後片付けを行う `Dispose` メソッドを宣言したインターフェイスです。`IDisposable` を実装したクラスは、`using` 文を使うと、例外が発生しても `Dispose` を確実に呼び出せます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `IDisposable` インターフェイスを実装したクラスを定義できる
- `using` 文が `try` / `finally` に置き換えられることを説明できる
- `using` 宣言を使い、複数の変数の `Dispose` が宣言と逆の順序で呼ばれることを説明できる

## 前提知識

- [インターフェイス](/unity-csharp-learning/csharp/interfaces/) を読んでいること
- [例外の基本](/unity-csharp-learning/csharp/exceptions/) を読んでいること

---

## 1. IDisposable インターフェイス

[IDisposable インターフェイス](https://learn.microsoft.com/dotnet/api/system.idisposable) は、`Dispose` メソッドを 1 つだけ宣言したインターフェイスです。`Dispose` には、オブジェクトを使い終わったときの後片付けの処理を書きます。何を後片付けするかは、実装するクラスが決めます。

**書式：[IDisposable.Dispose メソッド](https://learn.microsoft.com/dotnet/api/system.idisposable.dispose)**
```csharp
public interface IDisposable
{
    void Dispose();
}
```

次の `A` クラスは、`IDisposable` を実装した最小のクラスです。どのオブジェクトのメソッドが呼ばれたかがわかるように、コンストラクターで受け取った名前を付けて表示します。

```csharp
A a = new A("a");
a.M();
a.Dispose();

class A : IDisposable
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

    public void Dispose()
    {
        Console.WriteLine($"{_name}.Dispose");
    }
}
```

```
a.M
a.Dispose
```

### 例外が発生すると Dispose が呼ばれない

`Dispose` を最後に呼び出すだけでは、途中で例外が発生したときに `Dispose` が呼ばれません。以降のコード例では、上の `A` クラスを使います。例外を発生させるために、[例外の基本](/unity-csharp-learning/csharp/exceptions/) で使った `int.Parse("abc")` を使います。

```csharp
try
{
    A a = new A("a");
    a.M();
    int.Parse("abc");
    a.Dispose();
}
catch (FormatException)
{
    Console.WriteLine("catch");
}
```

```
a.M
catch
```

`int.Parse("abc")` で例外が発生したので、`a.Dispose()` は飛ばされています。

`finally` を使えば、例外が発生しても `Dispose` を呼び出せます。

```csharp
try
{
    A a = new A("a");
    try
    {
        a.M();
        int.Parse("abc");
    }
    finally
    {
        a.Dispose();
    }
}
catch (FormatException)
{
    Console.WriteLine("catch");
}
```

```
a.M
a.Dispose
catch
```

例外が呼び出し元の `catch` に届く前に、`finally` で `Dispose` が呼ばれています。ただ、`IDisposable` を実装したオブジェクトを使うたびにこれを書くのは面倒です。そこで、C# には `using` 文が用意されています。

---

## 2. using 文

`using` 文は、ブロックを抜けるときに、宣言した変数の `Dispose` を自動で呼び出します。

**書式：[using 文](https://learn.microsoft.com/dotnet/csharp/language-reference/statements/using)**
```
using (型 変数名 = 式)
{
    // 変数を使う処理
}
```

| 要素 | 説明 |
|---|---|
| `型 変数名 = 式` | オブジェクトを作り、変数に入れる。型は `IDisposable` を実装している必要がある |
| ブロック | 変数を使う処理を書く。変数はこのブロックの中だけで使える |

```csharp
try
{
    using (A a = new A("a"))
    {
        a.M();
        int.Parse("abc");
        a.M();
    }
}
catch (FormatException)
{
    Console.WriteLine("catch");
}
```

```
a.M
a.Dispose
catch
```

`int.Parse("abc")` で例外が発生しても、ブロックを抜けるときに `Dispose` が呼ばれています。

### using 文の正体は try / finally

コンパイラーは `using` 文を、おおよそ次のような `try` / `finally` に置き換えます。前の節で自分で書いたコードと同じ形です。

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
            a.Dispose();
        }
    }
}
```

このように、書き方を短くするためだけに用意された構文を **シンタックスシュガー**（糖衣構文）といいます。`finally` に置き換えられるので、例外だけでなく `return` でブロックを抜けたときも `Dispose` が呼ばれます。

```csharp
Run();
Console.WriteLine("Run から戻った");

void Run()
{
    using (A a = new A("a"))
    {
        a.M();
        return;
    }
}
```

```
a.M
a.Dispose
Run から戻った
```

`using` 文に書けるのは、`IDisposable` を実装した型だけです。`Dispose` を呼び出せない型を書くと、コンパイルエラーになります。

```csharp
// ❌ NG: int は IDisposable を実装していない
// using (int x = 1)  // CS1674
// {
// }
```

---

## 3. using 宣言

C# 8.0 以降では、ブロックを書かずに、変数の宣言の前に `using` を付けるだけで済む **using 宣言** を使えます。変数を宣言したスコープ（ブロック）の終わりで、`Dispose` が呼ばれます。

**書式：[using 宣言](https://learn.microsoft.com/dotnet/csharp/language-reference/statements/using)**
```
using 型 変数名 = 式;
```

using 宣言を複数並べると、**宣言したのと逆の順序** で `Dispose` が呼ばれます。

```csharp
Run();
Console.WriteLine("Run から戻った");

void Run()
{
    using A a = new A("a");
    using A b = new A("b");
    a.M();
    b.M();
    Console.WriteLine("Run の最後");
}
```

```
a.M
b.M
Run の最後
b.Dispose
a.Dispose
Run から戻った
```

`Run` の終わりで、後に宣言した `b` から先に `Dispose` が呼ばれています。上のコードは、`using` 文を入れ子にした次のコードと同じ意味です。

```
using (A a = new A("a"))
{
    using (A b = new A("b"))
    {
        a.M();
        b.M();
        Console.WriteLine("Run の最後");
    }
}
```

後から作ったオブジェクトは、先に作ったオブジェクトを使っていることがあります。逆の順序で `Dispose` が呼ばれるので、後から作ったオブジェクトの後片付けが終わるまで、先に作ったオブジェクトは使える状態のまま残ります。

---

## 4. Dispose を実装するときの約束

自分で `IDisposable` を実装するときは、次の 2 つを守ります。

- `Dispose` が 2 回以上呼ばれても、エラーにならないようにする
- `Dispose` した後にメソッドが呼ばれたら、[ObjectDisposedException](https://learn.microsoft.com/dotnet/api/system.objectdisposedexception) を投げる

`A` クラスを、この 2 つを守るように書き換えます。`Dispose` したかどうかを `_disposed` フィールドで記録します。

```csharp
A a = new A("a");
a.Dispose();
a.Dispose();

try
{
    a.M();
}
catch (ObjectDisposedException e)
{
    Console.WriteLine($"{e.GetType().Name}: {e.ObjectName}");
}

class A : IDisposable
{
    private string _name;
    private bool _disposed;

    public A(string name)
    {
        _name = name;
    }

    public void M()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(A));
        }
        Console.WriteLine($"{_name}.M");
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        Console.WriteLine($"{_name}.Dispose");
    }
}
```

```
a.Dispose
ObjectDisposedException: A
```

2 回目の `Dispose` は何もせずに戻るので、`a.Dispose` は 1 回だけ表示されています。`Dispose` した後の `M` では、`ObjectDisposedException` が投げられます。`IDisposable` を実装した .NET のクラスの多くも、`Dispose` した後に使うと `ObjectDisposedException` を投げます。

---

## ワンポイントアドバイス

### using ディレクティブとの違い

ファイルの先頭に書く `using System;` は **using ディレクティブ** といい、名前空間を省略して型を書けるようにするものです。このページの `using` 文・`using` 宣言とは、同じキーワードを使っているだけで、まったく別の機能です。

---

## まとめ

- `IDisposable` は、後片付けの処理を書く `Dispose` メソッドを宣言したインターフェイス
- `using` 文は、ブロックを抜けるときに `Dispose` を呼び出す。正体は `try` / `finally` なので、例外や `return` で抜けても `Dispose` が呼ばれる
- using 宣言は、スコープの終わりで `Dispose` を呼び出す。複数あるときは宣言と逆の順序で呼ばれる
- `Dispose` は 2 回呼ばれても安全にし、`Dispose` した後の操作では `ObjectDisposedException` を投げる

---

## 理解度チェック

1. `using` 文のブロックの中で例外が発生したとき、`Dispose` は呼ばれますか？理由も説明してください。
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   using (A a = new A("a"))
   {
       using A b = new A("b");
       a.M();
       b.M();
   }
   Console.WriteLine("終了");

   class A : IDisposable
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

       public void Dispose()
       {
           Console.WriteLine($"{_name}.Dispose");
       }
   }
   ```

3. （応用）次の `using` 文を、`using` を使わずに `try` / `finally` で書き換えてください。

   ```csharp
   using (A a = new A("a"))
   {
       a.M();
   }
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. 呼ばれます。`using` 文はコンパイラーによって `try` / `finally` に置き換えられ、`Dispose` は `finally` の中で呼ばれるからです。
2. 次のように出力されます。`b` は `using` 文のブロックの中で宣言されているので、そのブロックの終わりで、`a` より先に `Dispose` が呼ばれます。

   ```
   a.M
   b.M
   b.Dispose
   a.Dispose
   終了
   ```

3. ```csharp
   A a = new A("a");
   try
   {
       a.M();
   }
   finally
   {
       a.Dispose();
   }
   ```

   `new A("a")` は `null` にならないので、`finally` の `null` チェックは省略しています。

</details>

---

## 次のステップ

[スレッドの基本](/unity-csharp-learning/csharp/threads/) では、複数の処理を同時に進めるためのスレッドを学びます。
