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
- ラムダ式をメソッドの引数として、その場に書いて渡せる
- ラムダ式で購読したイベントを解除する方法を説明できる

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

> 💡 **ポイント**: [メソッド](/unity-csharp-learning/csharp/methods/) で学んだ式形式のメソッド（`int Double(int x) => x * 2;`）も `=>` を使いますが、ラムダ式とは別の文法です。式形式は、名前のあるメソッドやプロパティの本体を短く書く書き方です。ラムダ式は、名前のないメソッドを式としてその場に書き、デリゲートの変数や引数に渡すものです。

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

## 4. ラムダ式をメソッドの引数として渡す

ラムダ式が最も役立つのは、メソッドの引数としてデリゲートを渡す場面です。[デリゲートの変数渡しとコールバック](/unity-csharp-learning/csharp/delegate-callback/) では、点数を数える条件を `IsPassed` や `IsPerfect` という名前付きのメソッドにして、`Count` に渡しました。ラムダ式を使うと、条件を呼び出しの場所に直接書けます。

```csharp
int[] scores = { 45, 100, 72, 60, 100, 38 };

Console.WriteLine($"合格: {Count(scores, score => score >= 60)} 人");
Console.WriteLine($"満点: {Count(scores, score => score == 100)} 人");
Console.WriteLine($"追試: {Count(scores, score => score < 40)} 人");

int Count(int[] values, Func<int, bool> condition)
{
    int count = 0;
    foreach (int value in values)
    {
        if (condition(value))
        {
            count++;
        }
    }
    return count;
}
```

```
合格: 4 人
満点: 2 人
追試: 1 人
```

1 回しか使わない条件のために、名前を考えてメソッドを宣言する必要がなくなります。また、何を数えているのかが、呼び出しの場所を見るだけでわかります。`score` の型を書いていないのは、`Count` のパラメータが `Func<int, bool>` なので、`score` が `int` だとコンパイラーが決められるからです。

.NET の標準ライブラリにも、デリゲートを受け取るメソッドがたくさんあります。たとえば、[List\<T\>](/unity-csharp-learning/csharp/list/) の `Sort` メソッドには、2 つの要素の比べ方をデリゲートで渡せます。比べ方は、1 つ目を前にするなら負の数、後ろにするなら正の数、同じなら 0 を返すメソッドで表します。

```csharp
var names = new List<string> { "Slime", "Dragon", "Bat", "Skeleton" };

// 文字数が少ない順に並べる
names.Sort((a, b) => a.Length - b.Length);
Console.WriteLine(string.Join(", ", names));

// 文字数が多い順に並べる
names.Sort((a, b) => b.Length - a.Length);
Console.WriteLine(string.Join(", ", names));
```

```
Bat, Slime, Dragon, Skeleton
Skeleton, Dragon, Slime, Bat
```

並べ替えの手順は `Sort` が担当し、比べ方だけをラムダ式で渡しています。このラムダ式は、比べ方を表す `Comparison<string>` というデリゲート型に変換されます。比べ方を渡す仕組みは、[比較の仕組み](/unity-csharp-learning/csharp/comparison/) で詳しく学びます。このように、処理の一部をラムダ式で渡す書き方は、[LINQ の基本](/unity-csharp-learning/csharp/linq-basics/) で学ぶ LINQ でも中心になります。

---

## 5. イベントとラムダ式

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

ただし、ラムダ式で購読したイベントは、同じラムダ式を書いて `-=` しても解除できません。同じ内容を書いても、ラムダ式を書くたびに別のメソッドとして扱われるからです。[イベント](/unity-csharp-learning/csharp/events/) で学んだように、使い終わった購読者は解除する必要があります。解除が必要なときは、ラムダ式を変数に入れておき、その変数で `+=` と `-=` をします。

```csharp
var btn = new Button();

// ❌ 同じ内容のラムダ式を書いても、解除できない
btn.Clicked += () => Console.WriteLine("ラムダ式 A");
btn.Clicked -= () => Console.WriteLine("ラムダ式 A");

// ✅ 変数に入れておけば、同じデリゲートで解除できる
Action handler = () => Console.WriteLine("ラムダ式 B");
btn.Clicked += handler;
btn.Clicked -= handler;

btn.Click();

class Button
{
    public event Action? Clicked;
    public void Click() => Clicked?.Invoke();
}
```

```
ラムダ式 A
```

解除できなかった `ラムダ式 A` だけが表示され、解除した `ラムダ式 B` は表示されません。名前付きのメソッドで購読しても、同じように解除できます。

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
- ラムダ式は、メソッドの引数（`Count` の条件や `Sort` の比べ方）としてその場に書いて渡すと、名前付きのメソッドを宣言せずに済む
- ラムダ式でイベントを購読できる。解除が必要なときは、ラムダ式を変数に入れておくか、名前付きのメソッドを使う

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

3. 呼び出して確かめるコードも含めると、次のように書けます。

   ```csharp
   Func<List<int>, int> sumOfSquares = list =>
   {
       int total = 0;
       foreach (int n in list)
       {
           total += n * n;
       }
       return total;
   };

   Console.WriteLine(sumOfSquares(new List<int> { 1, 2, 3 }));
   ```

   ```
   14
   ```

</details>

---

## 次のステップ

[変数キャプチャ](/unity-csharp-learning/csharp/variable-capture/) では、ラムダ式が外側スコープの変数を取り込む「クロージャ」のしくみを学びます。
