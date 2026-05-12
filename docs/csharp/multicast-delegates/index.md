---
layout: page
title: マルチキャストデリゲート
permalink: /csharp/multicast-delegates/
---

# マルチキャストデリゲート

デリゲートには複数のメソッドを登録でき、1 回の呼び出しで全員に通知できます。このしくみを**マルチキャストデリゲート**と呼びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `+=` でデリゲートにメソッドを追加できる
- `-=` で登録したメソッドを解除できる
- 戻り値がある場合に最後のメソッドの値だけが返ることを説明できる
- `GetInvocationList()` で登録済みメソッドを個別に呼び出せる

## 前提知識

- [デリゲートの変数渡しとコールバック](/unity-csharp-learning/csharp/delegate-callback/) を読んでいること

---

## 1. `+=` でメソッドを追加する

**書式：デリゲートへのメソッド追加**
```
デリゲート変数 += メソッド名;
```

| 要素 | 説明 |
|---|---|
| `デリゲート変数` | 複数のメソッドを管理するデリゲート変数 |
| `+=` | デリゲートに新しいメソッドを追加する演算子 |
| `メソッド名` | 追加するメソッド（シグネチャが一致すること） |

```csharp
using System;

public delegate void Notify();

public class Program
{
    public static void Main()
    {
        Notify? notify = null;
        notify += SayHello;
        notify += SayGoodbye;

        notify?.Invoke();
    }

    private static void SayHello()
    {
        Console.WriteLine("こんにちは！");
    }

    private static void SayGoodbye()
    {
        Console.WriteLine("さようなら！");
    }
}
```

```
こんにちは！
さようなら！
```

`notify?.Invoke()` を 1 回呼び出すだけで、登録した順に `SayHello` → `SayGoodbye` が実行されます。

---

## 2. `-=` でメソッドを解除する

**書式：デリゲートからのメソッド解除**
```
デリゲート変数 -= メソッド名;
```

| 要素 | 説明 |
|---|---|
| `-=` | 登録されているメソッドをデリゲートから取り除く演算子 |

```csharp
using System;

public delegate void Notify();

public class Program
{
    public static void Main()
    {
        Notify? notify = null;
        notify += SayHello;
        notify += SayGoodbye;

        notify -= SayHello;   // SayHello だけ解除

        notify?.Invoke();
    }

    private static void SayHello()
    {
        Console.WriteLine("こんにちは！");
    }

    private static void SayGoodbye()
    {
        Console.WriteLine("さようなら！");
    }
}
```

```
さようなら！
```

`SayHello` が解除されたため、`SayGoodbye` だけが呼び出されます。登録されていないメソッドを `-=` で解除しようとしても、エラーにはなりません。

---

## 3. 戻り値がある場合

複数のメソッドが登録されたデリゲートに戻り値がある場合、**最後に登録したメソッドの戻り値だけ**が返ります。

```csharp
using System;

public delegate int Calculate(int x);

public class Program
{
    public static void Main()
    {
        Calculate calc = Double;
        calc += Triple;

        int result = calc(5);
        Console.WriteLine(result);  // Triple(5) = 15 だけが返る
    }

    private static int Double(int x) => x * 2;   // 10 が返るが捨てられる
    private static int Triple(int x) => x * 3;   // 15 が返る
}
```

```
15
```

`Double` の戻り値 `10` は無視されます。複数の戻り値を個別に使いたい場合は、次のセクションで説明する `GetInvocationList()` を使います。

---

## 4. `GetInvocationList()` で個別に呼び出す

`GetInvocationList()` は、デリゲートに登録されているメソッド一覧を配列で返します。これを使うと、各メソッドの戻り値を個別に受け取れます。

**`Delegate.GetInvocationList`** — デリゲートに登録されている全メソッドを `Delegate[]` として返します。<!-- [公式ドキュメント]() -->

**書式：Delegate.GetInvocationList メソッド**
```csharp
Delegate[] GetInvocationList();
```

| パラメータ | 型 | 説明 |
|---|---|---|
| （なし） | — | パラメータはありません |
| **戻り値** | `Delegate[]` | 登録されているメソッドを順番に並べた配列 |

```csharp
using System;

public delegate int Calculate(int x);

public class Program
{
    public static void Main()
    {
        Calculate calc = Double;
        calc += Triple;

        foreach (Calculate c in calc.GetInvocationList())
        {
            int result = c(5);
            Console.WriteLine(result);
        }
    }

    private static int Double(int x) => x * 2;
    private static int Triple(int x) => x * 3;
}
```

```
10
15
```

各メソッドを個別に呼び出しているため、`Double` の結果 `10` と `Triple` の結果 `15` をそれぞれ取得できます。

---

## よくあるミス

```csharp
// ❌ NG: null のデリゲートに += で追加後、?.Invoke() を使わずに呼び出す
Notify? notify = null;
notify += SayHello;
notify();           // コンパイルエラー（nullable なので直接呼び出し不可）

// ✅ OK: ?.Invoke() を使う
notify?.Invoke();
```

> 💡 **ポイント**: `null` だったデリゲートに `+=` でメソッドを追加すると、内部的に新しいデリゲートインスタンスが生成されます。変数は `null` ではなくなりますが、`?.Invoke()` を使う習慣をつけると安全です。

---

## まとめ

- `+=` でデリゲートに複数のメソッドを追加でき、呼び出しは登録順に実行される
- `-=` で登録済みのメソッドを解除できる
- 戻り値があるマルチキャストデリゲートは、最後のメソッドの値だけが返る
- `GetInvocationList()` を使うと各メソッドの戻り値を個別に受け取れる

---

## 理解度チェック

以下の問いに答えられるか確認しましょう。

1. デリゲートに `+=` で 3 つのメソッドを追加して呼び出すと、どのような順で実行されますか？
2. 次のコードの出力結果は何になりますか？

   ```csharp
   using System;

   public delegate void Log(string msg);

   public class Program
   {
       public static void Main()
       {
           Log? log = PrintA;
           log += PrintB;
           log -= PrintA;
           log?.Invoke("test");
       }

       private static void PrintA(string msg) => Console.WriteLine($"A:{msg}");
       private static void PrintB(string msg) => Console.WriteLine($"B:{msg}");
   }
   ```

3. （応用）戻り値 `int` を持つデリゲートに複数のメソッドを登録し、全メソッドの戻り値の合計を求めるにはどう書きますか？

<details markdown="1">
<summary>解答を見る</summary>

1. 登録した順（`+=` で追加した順）に実行されます。
2. `PrintA` が `-=` で解除されているため、出力は次のとおりです。

   ```
   B:test
   ```

3. `GetInvocationList()` で個別に呼び出して合計します。

   ```csharp
   int total = 0;
   foreach (Calculate c in calc.GetInvocationList())
   {
       total += c(5);
   }
   Console.WriteLine(total);
   ```

</details>

---

## 次のステップ

[イベント](/unity-csharp-learning/csharp/events/) では、`event` キーワードによってデリゲートをより安全に扱う方法と、発行者/購読者パターンを学びます。
