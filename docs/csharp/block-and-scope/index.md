---
layout: page
title: ブロック文とスコープ（補足）
permalink: /csharp/block-and-scope/
---

# ブロック文とスコープ（補足）

`if` 文を学んだついでに、`{ }` の正体と「変数がどこから見えるか」というスコープの概念を整理します。

## 学習目標

- `{ }` がブロック文であることを理解できる
- ブロック内で宣言した変数がブロック外から見えない理由を説明できる
- `else if` が実際には `else` の中に `if` をネストした書き方であることを理解できる

## 前提知識

- [条件分岐](/unity-csharp-learning/csharp/conditionals/) を読んでいること

---

## 1. ブロック文

`{ }` で囲まれた部分を**ブロック文**と呼びます。複数の文を「ひとかたまり」として扱うための仕組みです。

**書式：ブロック文**
```
{
    文1;
    文2;
    文3;
}
```

`if` の後ろの `{ }` はブロック文です。`if` はこのブロック文を「条件が true のときに実行する文」として受け取っています。

```csharp
if (true)
{
    // ここはブロック文。3つの文が入っている
    Console.WriteLine("1行目");
    Console.WriteLine("2行目");
    Console.WriteLine("3行目");
}
```

ブロック内の文は何行でも書けます。ブロック自体が1つの「文」として扱われるため、`if` の後ろに置けます。

### ブロックがなかった場合

`if` の後ろのブロックを省略すると、直後の 1 文だけが `if` の対象になります。

```csharp
int score = 40;

if (score >= 60)
    Console.WriteLine("合格");  // if の対象はこの 1 行だけ

Console.WriteLine("処理終了");  // 常に実行される
```

可読性やミスを防ぐ観点から、**1 行であっても `{ }` を書く習慣をおすすめします**。

---

## 2. スコープ

**スコープ**とは、変数が「見える（アクセスできる）範囲」のことです。C# では**変数はそれが宣言されたブロックの中でしか使えません**。

```csharp
int x = 10;  // このブロック（メソッド全体）で有効

if (x > 5)
{
    int result = x * 2;  // この if ブロックの中でだけ有効
    Console.WriteLine(result);  // 20
}

Console.WriteLine(result);  // ❌ コンパイルエラー: 'result' は存在しない
```

`result` は `if` ブロックの中で宣言されたため、ブロックを抜けると消えます。ブロックの外からは見えません。

### なぜスコープが必要か

スコープには「変数の意図しない使い回しを防ぐ」という利点があります。

```csharp
if (isPlayer)
{
    int hp = 100;
    Console.WriteLine($"プレイヤーHP: {hp}");
}

if (isEnemy)
{
    int hp = 50;  // ✅ OK: 別のブロックなので別の変数として宣言できる
    Console.WriteLine($"敵HP: {hp}");
}
```

同じ名前の `hp` を別々のブロックで独立して使えます。

### スコープのネスト

外側のスコープで宣言した変数は、内側のブロックから参照できます。

```csharp
int score = 75;  // 外側のスコープ

if (score >= 60)
{
    // score は外側で宣言されているので参照できる
    Console.WriteLine($"合格（{score}点）");
}
```

---

## 3. else if の実体

`else if` は独立した構文ではありません。`else` の後ろに `if` 文を置いた書き方です。

次の 2 つのコードはまったく同じ意味です。

**`else if` を使った書き方（一般的）:**

```csharp
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

**`else { if ... }` と展開した書き方:**

```csharp
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

`else if` は「`else` のブロックの中に `if` 文が 1 つしかない」場合に `{ }` を省略したものです。この展開した形を頭に置いておくと、`else if` の動作（前の条件が `false` のときだけ次の条件を評価する）が自然に理解できます。

### 深いネストを避ける

展開形を見るとわかるように、`else if` を使わずに書くとネストがどんどん深くなります。条件が多い場合は `else if` を使ってフラットに書く方が読みやすくなります。

```csharp
// ❌ NG: ネストが深くて読みにくい
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
        if (score >= 60)
        {
            Console.WriteLine("可");
        }
        else
        {
            Console.WriteLine("不可");
        }
    }
}

// ✅ OK: else if でフラットに書く
if (score >= 90)
{
    Console.WriteLine("優");
}
else if (score >= 70)
{
    Console.WriteLine("良");
}
else if (score >= 60)
{
    Console.WriteLine("可");
}
else
{
    Console.WriteLine("不可");
}
```

---

## まとめ

- `{ }` はブロック文。複数の文を 1 つにまとめ、`if` などに渡せる
- 変数はそれが宣言されたブロック内でしか使えない（**スコープ**）
- 外側のスコープの変数は内側のブロックから参照できる
- `else if` は `else` の後ろに `if` 文を置いた省略形であり、独立した構文ではない

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

2. 次の `else if` を使ったコードを、`else { if ... }` の形に展開してください。

   ```csharp
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

1. `value` は `if` ブロックの中で宣言されているため、ブロックを抜けるとスコープが終わり存在しなくなる。ブロックの外の `Console.WriteLine(value)` からは参照できず、コンパイルエラーになる。

2. ```csharp
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

[条件演算子と式・文](/unity-csharp-learning/csharp/conditional-operator/) では、「式」と「文」の違いを掘り下げ、`? :` 演算子を学びます。
