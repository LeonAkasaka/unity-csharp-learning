---
layout: page
title: ラムダ式
permalink: /csharp/lambda/
---

# ラムダ式

**ラムダ式**を使うと、メソッドをその場で短く書いてデリゲートに渡せます。`Action` / `Func` という組み込みのデリゲート型と組み合わせると、毎回 `delegate` 型を宣言しなくて済みます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `=>` を使ったラムダ式の構文を書ける
- 式ラムダと文ラムダの違いを説明できる
- `Action` / `Func` 組み込みデリゲート型を使える

## 前提知識

- [イベント](/unity-csharp-learning/csharp/events/) を読んでいること

---

## 1. ラムダ式とは

これまでデリゲートに渡すメソッドは、名前付きのメソッドとして別途定義する必要がありました。ラムダ式を使うと、**メソッドをその場でインラインに書いて**デリゲート変数に代入できます。

```csharp
using System;

public delegate void Greet(string name);

public class Program
{
    public static void Main()
    {
        // 名前付きメソッドを使う従来の書き方
        Greet greetOld = SayHello;

        // ラムダ式を使う書き方
        Greet greetNew = (name) => Console.WriteLine($"こんにちは、{name}！");

        greetOld("Alice");
        greetNew("Bob");
    }

    private static void SayHello(string name)
    {
        Console.WriteLine($"こんにちは、{name}！");
    }
}
```

```
こんにちは、Alice！
こんにちは、Bob！
```

---

## 2. ラムダ式の構文

**書式：式ラムダ（結果を返す 1 行の場合）**
```
(パラメータリスト) => 式
```

**書式：文ラムダ（複数行の処理が必要な場合）**
```
(パラメータリスト) =>
{
    文;
    ...
}
```

| 要素 | 説明 |
|---|---|
| `パラメータリスト` | メソッドの引数。1 つのときは `()` を省略できる |
| `=>` | 「ラムダ演算子」。「〜のとき、〜を行う」と読める |
| `式` | 式ラムダでは戻り値になる式を 1 つだけ書く |
| `{ 文; }` | 文ラムダでは通常のメソッド本体と同じように書く |

```csharp
using System;

public delegate int Transform(int x);

public class Program
{
    public static void Main()
    {
        // 式ラムダ：1 行で結果を返す
        Transform doubleIt = x => x * 2;

        // 文ラムダ：複数行の処理
        Transform tripleWithLog = (x) =>
        {
            int result = x * 3;
            Console.WriteLine($"{x} を 3 倍 → {result}");
            return result;
        };

        Console.WriteLine(doubleIt(5));
        tripleWithLog(4);
    }
}
```

```
10
4 を 3 倍 → 12
```

---

## 3. `Action` と `Func`

毎回 `delegate` 型を宣言しなくても、.NET には汎用のデリゲート型が用意されています。

**`Action<T>`** — 戻り値なし（`void`）のデリゲートです。<!-- [公式ドキュメント]() -->

**書式：Action\<T\> 型**
```csharp
Action<T1, T2, ...>
```

| パラメータ | 型 | 説明 |
|---|---|---|
| `T1`, `T2`, ... | 任意の型 | メソッドの引数の型（最大 16 個まで指定可能） |

引数がゼロのときは `Action`、引数が 1 つなら `Action<T>` を使います。

**`Func<T, TResult>`** — 戻り値ありのデリゲートです。<!-- [公式ドキュメント]() -->

**書式：Func\<T, TResult\> 型**
```csharp
Func<T1, T2, ..., TResult>
```

| パラメータ | 型 | 説明 |
|---|---|---|
| `T1`, `T2`, ... | 任意の型 | 引数の型（省略可能） |
| `TResult` | 任意の型 | **最後の型パラメータ**が戻り値の型 |

```csharp
using System;

public class Program
{
    public static void Main()
    {
        // Action<string>：string を受け取り void を返す
        Action<string> print = message => Console.WriteLine(message);

        // Func<int, int>：int を受け取り int を返す
        Func<int, int> square = x => x * x;

        // Func<int, int, int>：int を 2 つ受け取り int を返す
        Func<int, int, int> add = (a, b) => a + b;

        print("Hello from Action!");
        Console.WriteLine(square(6));
        Console.WriteLine(add(3, 4));
    }
}
```

```
Hello from Action!
36
7
```

---

## 4. イベントとラムダ式

ラムダ式はイベントの購読にも使えます。名前付きメソッドを用意する必要がなくなるため、短い処理であれば読みやすくなります。

```csharp
using System;

public class Button
{
    public event Action? Clicked;
    public void Click() => Clicked?.Invoke();
}

public class Program
{
    public static void Main()
    {
        var btn = new Button();

        // ラムダ式でその場に購読処理を書く
        btn.Clicked += () => Console.WriteLine("ボタンがクリックされました");

        btn.Click();
    }
}
```

```
ボタンがクリックされました
```

> 💡 **ポイント**: ラムダ式でイベントを購読した場合、同じラムダ式を `-=` で解除することはできません（別のインスタンスとして扱われるため）。解除が必要な場合は名前付きメソッドを使いましょう。

---

## よくあるミス

```csharp
// ❌ NG: Func の最後の型パラメータが戻り値であることを忘れて引数と混同する
Func<int, int> wrong = (a, b) => a + b;   // 引数が 2 つなのに型パラメータが 2 つしかない

// ✅ OK: 引数 2 つ、戻り値 1 つで合計 3 つの型パラメータが必要
Func<int, int, int> correct = (a, b) => a + b;
```

---

## まとめ

- ラムダ式 `(パラメータ) => 式` でメソッドをインラインに書いてデリゲートに渡せる
- 式ラムダ（1 行）と文ラムダ（ブロック）の 2 種類がある
- `Action<T>` は戻り値なし、`Func<T, TResult>` は戻り値ありの組み込みデリゲート型
- ラムダ式でイベントを購読できるが、解除が必要な場合は名前付きメソッドを使う

---

## 理解度チェック

以下の問いに答えられるか確認しましょう。

1. `Action<int, string>` 型のデリゲートが受け取れるメソッドのシグネチャを答えてください。
2. 次のコードの出力結果は何になりますか？

   ```csharp
   using System;

   public class Program
   {
       public static void Main()
       {
           Func<int, int, string> format = (a, b) => $"{a} + {b} = {a + b}";
           Console.WriteLine(format(3, 5));
       }
   }
   ```

3. （応用）`List<int>` を受け取り、各要素を 2 乗した合計を返す `Func` デリゲートをラムダ式で書いてください（LINQ は使わず `foreach` で実装してください）。

<details markdown="1">
<summary>解答を見る</summary>

1. `int` と `string` を引数にとり、戻り値が `void` のメソッドです。例: `void M(int n, string s)`

2. ```
   3 + 5 = 8
   ```

3. ```csharp
   Func<List<int>, int> sumOfSquares = list =>
   {
       int total = 0;
       foreach (int n in list)
       {
           total += n * n;
       }
       return total;
   };
   ```

</details>

---

## 次のステップ

[変数キャプチャ](/unity-csharp-learning/csharp/variable-capture/) では、ラムダ式が外側スコープの変数を取り込む「クロージャ」のしくみを学びます。
