---
layout: page
title: ブロック文とスコープ（補足）
permalink: /csharp/block-and-scope/
---

# ブロック文とスコープ（補足）

このページは、[条件分岐](/unity-csharp-learning/csharp/conditionals/) の補足です。`if` 文で使った `{ }` の正体である **ブロック文** と、変数を使える範囲である **スコープ**（scope）を学びます。また、`else if` がどのような仕組みで動いているかを確かめます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `{ }` がブロック文であることを説明できる
- ブロックの中で宣言した変数を、ブロックの外で使えない理由を説明できる
- `else if` が、`else` の後に `if` 文を書いたものであることを説明できる

## 前提知識

- [条件分岐](/unity-csharp-learning/csharp/conditionals/) を読んでいること

---

## 1. ブロック文

`{ }` で囲んだ部分を **ブロック文** といいます。複数の文を、1 つの文としてまとめるための仕組みです。

**書式：[ブロック文](https://learn.microsoft.com/dotnet/csharp/programming-guide/statements-expressions-operators/statements#group-statements-in-blocks)**
```
{
    文1;
    文2;
    // ...
}
```

`if` 文の書式は、正確には「`if (条件式)` の後に、条件式が `true` のときに実行する **文を 1 つ** 書く」というものです。ブロック文は、複数の文をまとめて 1 つの文にしたものなので、`if` の後に書けば、中の文をまとめて実行できます。

```csharp
int score = 80;

if (score >= 60)
{
    Console.WriteLine("合格");
    Console.WriteLine("おめでとうございます");
}
```

```
合格
おめでとうございます
```

### ブロックを書かない場合

`if` の後に `{ }` を書かないと、直後の 1 つの文だけが `if` の対象になります。

```csharp
int score = 40;

if (score >= 60)
    Console.WriteLine("合格");
    Console.WriteLine("おめでとうございます");
```

```
おめでとうございます
```

字下げをそろえていても、`if` の対象は `Console.WriteLine("合格");` だけです。2 つ目の `Console.WriteLine` は `if` とは関係なく実行されるので、不合格なのに `おめでとうございます` が表示されてしまいました。こうした間違いを防ぐため、対象が 1 つの文だけでも `{ }` を書く習慣をつけましょう。

---

## 2. スコープ

**スコープ** は、変数を使える範囲のことです。C# では、[変数は、それを宣言したブロックの中でしか使えません](https://learn.microsoft.com/dotnet/csharp/programming-guide/statements-expressions-operators/statements#blocks-define-variable-scope)。

```csharp
int x = 10;

if (x > 5)
{
    int result = x * 2;
    Console.WriteLine(result);
}
```

```
20
```

`result` は `if` のブロックの中で宣言したので、使えるのはこのブロックの中だけです。ブロックの外で使うと、コンパイルエラーになります。

```csharp
// ❌ NG: result はブロックの外では使えない
// if (x > 5)
// {
//     int result = x * 2;
// }
// Console.WriteLine(result);  // CS0103
```

### 外側の変数は、内側のブロックで使える

ブロックの外側で宣言した変数は、内側のブロックでも使えます。上の例の `x` も、`if` のブロックの外で宣言し、ブロックの中で使っています。

### 別々のブロックなら、同じ名前の変数を宣言できる

スコープが重ならない別々のブロックなら、同じ名前の変数をそれぞれ宣言できます。名前は同じでも、別々の変数です。

```csharp
bool isPlayerTurn = true;

if (isPlayerTurn)
{
    int hp = 100;
    Console.WriteLine($"プレイヤーの HP: {hp}");
}

if (!isPlayerTurn)
{
    int hp = 50;
    Console.WriteLine($"敵の HP: {hp}");
}
```

```
プレイヤーの HP: 100
```

スコープのおかげで、ブロックの中だけで使う変数が、ほかの場所で間違って使われることを防げます。

---

## 3. else if の正体

`else if` は、独立した構文ではありません。`else` の後に、`if` 文を 1 つ書いたものです。

次の 2 つのコードは、まったく同じ意味です。

```csharp
int score = 75;

if (score >= 90)
{
    Console.WriteLine("優");
}
else if (score >= 70)
{
    Console.WriteLine("良");
}
else
{
    Console.WriteLine("可");
}
```

```
良
```

```csharp
int score = 75;

if (score >= 90)
{
    Console.WriteLine("優");
}
else
{
    if (score >= 70)
    {
        Console.WriteLine("良");
    }
    else
    {
        Console.WriteLine("可");
    }
}
```

```
良
```

`else` の後にも、実行する文を 1 つ書きます。2 つ目のコードでは、その 1 つの文が `{ }` のブロック文で、中に `if` 文が 1 つだけ入っています。ブロックの中の文が 1 つだけなので `{ }` を省略すると、`else if` の形になります。

この形を知っていると、`else if` の条件が「前の条件が `false` のときにだけ調べられる」理由がわかります。`else if` の `if` 文は、前の `if` の `else` の中にあるからです。

条件が増えるほど、展開した形は字下げが深くなって読みにくくなります。ふだんは `else if` を使って、平らに書きます。

---

## よくあるミス

### 外側と同じ名前の変数を、内側のブロックで宣言する

```csharp
// ❌ NG: 外側の hp と同じ名前の変数を、内側のブロックで宣言している
// int hp = 100;
// if (hp > 0)
// {
//     int hp = 50;  // CS0136
// }
```

別々のブロックなら同じ名前の変数を宣言できますが、外側のスコープで使える変数と同じ名前の変数を、内側のブロックで宣言することはできません。内側のブロックでは外側の `hp` も使えるので、どちらの `hp` を指しているのかがわからなくなるからです。内側の変数には、別の名前を付けます。

---

## まとめ

- `{ }` はブロック文。複数の文をまとめて 1 つの文として扱う
- `if` や `else` の後には、実行する文を 1 つ書く。`{ }` を書かないと、直後の 1 つの文だけが対象になる
- 変数は、宣言したブロックの中でしか使えない（スコープ）。外側のブロックの変数は、内側のブロックでも使える
- 別々のブロックなら、同じ名前の変数を宣言できる。外側の変数と同じ名前の変数を、内側のブロックで宣言することはできない
- `else if` は、`else` の後に `if` 文を書いたもの

---

## 理解度チェック

1. 次のコードはコンパイルエラーになります。なぜですか？

   ```csharp
   if (true)
   {
       int value = 42;
   }
   Console.WriteLine(value);
   ```

2. 次のコードを実行すると何が出力されますか？

   ```csharp
   int x = 3;

   if (x > 5)
       Console.WriteLine("A");
   Console.WriteLine("B");
   ```

3. 次の `else if` を使ったコードを、`else { if ... }` の形に書き直してください。

   ```csharp
   int x = -2;

   if (x > 0)
   {
       Console.WriteLine("正");
   }
   else if (x < 0)
   {
       Console.WriteLine("負");
   }
   else
   {
       Console.WriteLine("ゼロ");
   }
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. `value` は `if` のブロックの中で宣言されているので、ブロックの外の `Console.WriteLine(value)` からは使えないからです（CS0103）。
2. `B` だけが出力されます。`{ }` がないので、`if` の対象は `Console.WriteLine("A");` だけです。`x > 5` は `false` なので `A` は表示されず、`Console.WriteLine("B");` は `if` と関係なく実行されます。

   ```
   B
   ```

3. ```csharp
   int x = -2;

   if (x > 0)
   {
       Console.WriteLine("正");
   }
   else
   {
       if (x < 0)
       {
           Console.WriteLine("負");
       }
       else
       {
           Console.WriteLine("ゼロ");
       }
   }
   ```

</details>

---

## 次のステップ

[条件演算子と式・文（補足）](/unity-csharp-learning/csharp/conditional-operator/) では、「式」と「文」の違いと、条件によって値を選ぶ `? :` 演算子を学びます。
