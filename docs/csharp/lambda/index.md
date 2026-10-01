---
layout: page
title: ラムダ式
permalink: /csharp/lambda/
---

# ラムダ式

**ラムダ式**を使うと、メソッドをその場で短く書いてデリゲートに渡せます。[デリゲートの基本](/unity-csharp-learning/csharp/delegates/) で学んだ `Action` / `Func` と組み合わせると、デリゲート型もメソッドも別に宣言せずに済みます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `=>` を使ったラムダ式の構文を書ける
- 式ラムダと文ラムダの違いを説明できる
- ラムダ式を `Action` / `Func` の変数に代入できる

## 前提知識

- [イベント](/unity-csharp-learning/csharp/events/) を読んでいること

---

## 1. ラムダ式とは

これまでデリゲートに渡すメソッドは、名前付きのメソッドとして別途定義する必要がありました。ラムダ式を使うと、**メソッドをその場でインラインに書いて**デリゲート変数に代入できます。

```csharp
// 名前付きメソッドを使う従来の書き方
Greet greetOld = SayHello;

// ラムダ式を使う書き方
Greet greetNew = (name) => Console.WriteLine($"こんにちは、{name}！");

greetOld("Alice");
greetNew("Bob");

void SayHello(string name)
{
    Console.WriteLine($"こんにちは、{name}！");
}

delegate void Greet(string name);
```

```
こんにちは、Alice！
こんにちは、Bob！
```

---

## 2. ラムダ式の構文

**書式：[式ラムダ](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/lambda-expressions#expression-lambdas)（本体が 1 つの式の場合）**
```
(パラメータリスト) => 式
```

**書式：[文ラムダ](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/lambda-expressions#statement-lambdas)（本体に文を並べる場合）**
```
(パラメータリスト) =>
{
    文;
    ...
}
```

| 要素 | 説明 |
|---|---|
| `パラメータリスト` | メソッドの引数。型を書かずに 1 つだけ受け取るときは `()` を省略できる（`x => x * 2`） |
| `=>` | 「ラムダ演算子」。「〜のとき、〜を行う」と読める |
| `式` | 式ラムダでは式を 1 つだけ書く。戻り値のあるデリゲートでは、式の値が戻り値になる |
| `{ 文; }` | 文ラムダでは通常のメソッド本体と同じように書く |

```csharp
// 式ラムダ：本体は x * 2 という式 1 つ
Transform doubleIt = x => x * 2;

// 文ラムダ：本体はブロック。値は return で返す
Transform tripleWithLog = (x) =>
{
    int result = x * 3;
    Console.WriteLine($"{x} を 3 倍 → {result}");
    return result;
};

Console.WriteLine(doubleIt(5));
tripleWithLog(4);

delegate int Transform(int x);
```

```
10
4 を 3 倍 → 12
```

---

## 3. ラムダ式と `Action` / `Func`

[デリゲートの基本](/unity-csharp-learning/csharp/delegates/) では、`Action` / `Func` の変数に名前付きのメソッドを代入しました。ラムダ式も、同じように代入できます。デリゲート型を宣言する必要も、メソッドに名前を付ける必要もなくなります。

```csharp
// Action<string>：string を受け取り void を返す
Action<string> print = message => Console.WriteLine(message);

// Func<int, int>：int を受け取り int を返す
Func<int, int> square = x => x * x;

// Func<int, int, int>：int を 2 つ受け取り int を返す
Func<int, int, int> add = (a, b) => a + b;

print("Hello from Action!");
Console.WriteLine(square(6));
Console.WriteLine(add(3, 4));
```

```
Hello from Action!
36
7
```

ラムダ式のパラメータ `message` や `x` に型を書いていないのは、代入先の `Action<string>` や `Func<int, int>` から、コンパイラーが型を決められるからです。

---

## 4. イベントとラムダ式

ラムダ式はイベントの購読にも使えます。名前付きメソッドを用意する必要がなくなるため、短い処理であれば読みやすくなります。

```csharp
var btn = new Button();

// ラムダ式でその場に購読処理を書く
btn.Clicked += () => Console.WriteLine("ボタンがクリックされました");

btn.Click();

class Button
{
    public event Action? Clicked;
    public void Click() => Clicked?.Invoke();
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
Func<int, int> wrong = (a, b) => a + b;   // CS1593：引数は 1 つの型なのに 2 つ渡している

// ✅ OK: 引数 2 つ、戻り値 1 つで合計 3 つの型パラメータが必要
Func<int, int, int> correct = (a, b) => a + b;
```

---

## まとめ

- ラムダ式 `(パラメータ) => 式` でメソッドをインラインに書いてデリゲートに渡せる
- 本体が式 1 つの式ラムダと、ブロックの文ラムダの 2 種類がある
- ラムダ式は `Action` / `Func` の変数に代入でき、パラメータの型は代入先から決まる
- ラムダ式でイベントを購読できるが、解除が必要な場合は名前付きメソッドを使う

---

## 理解度チェック

以下の問いに答えられるか確認しましょう。

1. `Action<int, string>` 型のデリゲートが受け取れるメソッドのシグネチャを答えてください。
2. 次のコードの出力結果は何になりますか？

   ```csharp
   Func<int, int, string> format = (a, b) => $"{a} + {b} = {a + b}";
   Console.WriteLine(format(3, 5));
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
