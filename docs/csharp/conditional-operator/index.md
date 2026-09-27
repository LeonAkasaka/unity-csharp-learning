---
layout: page
title: 条件演算子と式・文（補足）
permalink: /csharp/conditional-operator/
---

# 条件演算子と式・文（補足）

このページは、[条件分岐](/unity-csharp-learning/csharp/conditionals/) の補足です。条件によって値を選ぶ `? :` 演算子（**条件演算子**）を学びます。条件演算子は `if` 文と似ていますが、`if` 文とは違って値を持ちます。その違いを理解するために、まず **式**（expression）と **文**（statement）の違いを整理します。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- 式と文の違いを説明できる
- `if` 文が値を持たないことを説明できる
- 条件演算子 `? :` で、条件に応じた値を選べる
- `if` 文と条件演算子を使い分けられる

## 前提知識

- [条件分岐](/unity-csharp-learning/csharp/conditionals/) を読んでいること

---

## 1. 式と文

### 式 — 値になるもの

**式** は、計算すると値になるコードです。

| 式 | 値 |
|---|---|
| `3 + 2` | `5` |
| `score * 2` | `score` の 2 倍 |
| `"Hello"` | `"Hello"` という文字列 |
| `score >= 60` | `true` か `false` |

式は値になるので、変数に代入したり、メソッドに渡したりできます。

```csharp
int score = 40;
int doubled = score * 2;
Console.WriteLine(doubled);
Console.WriteLine(score >= 60);
```

```
80
False
```

### 文 — 処理を行うもの

**文** は、実行すると何かの処理を行うコードです。C# のプログラムは、文を並べて書きます。文そのものは値を持ちません。

| 文 | 例 |
|---|---|
| 宣言文 | `int score = 100;` |
| 式文 | `score = 200;`、`Console.WriteLine(score);` |
| `if` 文 | `if (score >= 60) { ... }` |

式の後に `;` を付けると、**式文** になります。代入（`score = 200`）やメソッドの呼び出し（`Console.WriteLine(score)`）は式で、`;` を付けて文として使っています。

### if は文

`if` は、条件によって実行する処理を切り替える **文** です。値を持たないので、変数に代入することはできません。

```csharp
// ❌ NG: if 文は値を持たないので、代入できない
// var label = if (score >= 60) { "合格" } else { "不合格" };  // CS1525 など
```

`if` 文で、条件によって変数に入れる値を変えたいときは、それぞれのブロックの中で代入します。

```csharp
int score = 75;
string label;

if (score >= 60)
{
    label = "合格";
}
else
{
    label = "不合格";
}

Console.WriteLine(label);
```

```
合格
```

正しく動きますが、「条件によって値を選ぶ」だけのために、何行も書く必要があります。

---

## 2. 条件演算子

[条件演算子](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/conditional-operator) は、条件によって 2 つの値のどちらかを選ぶ演算子です。

**書式：[条件演算子](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/conditional-operator)**
```
条件式 ? 真のときの値 : 偽のときの値
```

| 要素 | 説明 |
|---|---|
| `条件式` | `bool` 型になる式 |
| `真のときの値` | 条件式が `true` のとき、全体の値になる式 |
| `偽のときの値` | 条件式が `false` のとき、全体の値になる式 |

条件演算子は、全体で 1 つの **式** なので、値を持ちます。

```csharp
int score = 75;
string label = score >= 60 ? "合格" : "不合格";
Console.WriteLine(label);
```

```
合格
```

`score >= 60` が `true` なので、`"合格"` が全体の値になり、`label` に代入されます。前の節の `if` 文と同じことを、1 行で書けました。

### メソッドの引数に直接書く

条件演算子は式なので、値を書ける場所なら、どこにでも書けます。

```csharp
int score = 40;
Console.WriteLine(score >= 60 ? "合格" : "不合格");
```

```
不合格
```

### 真と偽の値の型

真のときの値と偽のときの値は、どちらか一方の型に、もう一方を暗黙的に変換できる必要があります。全体の値の型は、その型になります。

```csharp
int score = 40;
var result = score >= 60 ? 1 : 0.5;
Console.WriteLine(result);
Console.WriteLine(result.GetType());
```

```
0.5
System.Double
```

`int` の `1` は `double` に暗黙的に変換できるので、全体の型は `double` になります。`int` と `string` のように、どちらにも変換できない組み合わせは、コンパイルエラーになります（よくあるミスを参照）。

---

## 3. if 文と条件演算子の使い分け

| | `if` 文 | 条件演算子 `? :` |
|---|---|---|
| 種類 | 文（値を持たない） | 式（値を持つ） |
| 目的 | 実行する処理を切り替える | 値を選ぶ |
| 複数の処理 | 書ける | 書けない |
| 値として使う | できない | できる |

**条件演算子が向いている場面**：条件によって値を選ぶだけのとき。

```csharp
bool isLoggedIn = false;
int x = -5;

string message = isLoggedIn ? "ようこそ" : "ログインしてください";
int abs = x >= 0 ? x : -x;

Console.WriteLine(message);
Console.WriteLine(abs);
```

```
ログインしてください
5
```

`abs` は、`x` が 0 以上なら `x` を、負なら符号を反転した `-x` を選ぶので、`x` の絶対値になります。

**`if` 文が向いている場面**：条件によって、いくつかの処理を実行したいとき。

```csharp
int score = 80;

if (score >= 60)
{
    Console.WriteLine("合格");
    Console.WriteLine("修了証を発行します");
}
else
{
    Console.WriteLine("不合格");
    Console.WriteLine("再試験を案内します");
}
```

```
合格
修了証を発行します
```

条件演算子を入れ子にすれば 3 つ以上の値から選ぶこともできますが、読みにくくなります。条件が多いときは、`if` 文を使いましょう。

---

## よくあるミス

### 真と偽の値の型が合わない

```csharp
// ❌ NG: int と string は、どちらからどちらへも暗黙的に変換できない
// int score = 75;
// var result = score >= 60 ? 1 : "不合格";  // CS0173
```

`1`（`int`）と `"不合格"`（`string`）は、どちらの型にもそろえられないので、全体の型が決まりません。両方を `string` にするなど、型をそろえます。

---

## まとめ

- 式は、計算すると値になるコード。変数に代入したり、メソッドの引数にしたりできる
- 文は、処理を行うコード。式の後に `;` を付けると式文になる。`if` 文は値を持たない
- 条件演算子 `条件式 ? 真のときの値 : 偽のときの値` は、条件によって値を選ぶ式
- 真と偽の値は、一方の型にもう一方を暗黙的に変換できる必要がある
- 値を選ぶだけなら条件演算子、複数の処理を切り替えるなら `if` 文を使う

---

## 理解度チェック

1. 次のコードを実行すると何が出力されますか？

   ```csharp
   int x = 10;
   Console.WriteLine(x % 2 == 0 ? "偶数" : "奇数");
   Console.WriteLine(x > 100 ? 1 : 2.5);
   ```

2. 次のコードを、条件演算子を使って 1 つの文に書き直してください。

   ```csharp
   int score = 85;
   string grade;
   if (score >= 70)
   {
       grade = "合格";
   }
   else
   {
       grade = "不合格";
   }
   ```

3. 次のコードは、なぜコンパイルエラーになりますか？

   ```csharp
   int value = 5;
   var result = value > 0 ? "正" : 0;
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. 次のように出力されます。`10 % 2 == 0` は `true` なので `偶数` です。`x > 100` は `false` なので `2.5` が選ばれます。全体の型は `double` です。

   ```
   偶数
   2.5
   ```

2. ```csharp
   int score = 85;
   string grade = score >= 70 ? "合格" : "不合格";
   ```

3. 真のときの値 `"正"` は `string`、偽のときの値 `0` は `int` で、どちらの型にもそろえられないからです（CS0173）。

</details>

---

## 次のステップ

[反復処理](/unity-csharp-learning/csharp/loops/) では、同じ処理を繰り返す `while` 文や `for` 文を学びます。
