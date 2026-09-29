---
layout: page
title: 最初のプログラムと変数
permalink: /csharp/variables/
---

# 最初のプログラムと変数

コンソールアプリで、C# のプログラムがどのように実行されるかを確かめます。コードに直接書く値である **リテラル**（literal）と、値に名前を付けて保存しておく **変数**（variable）を学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- プログラムが上から下へ順番に実行されることを説明できる
- `Console.WriteLine` で値を出力できる
- リテラルに型があることを説明できる
- 算術演算子で計算し、`int` どうしの割り算の結果を説明できる
- 変数の宣言・初期化子・代入の違いを説明できる

## 前提知識

- [.NET SDK と dotnet CLI](/unity-csharp-learning/csharp/dotnet-sdk/) を読み、C# のプログラムを実行できること

---

## 1. プログラムは上から下へ実行される

プログラムは、原則として **上の行から下の行へ、1 行ずつ順番に** 実行されます。これを **逐次実行** といいます。

```csharp
Console.WriteLine("1行目");
Console.WriteLine("2行目");
Console.WriteLine("3行目");
```

```
1行目
2行目
3行目
```

必ず「1行目」「2行目」「3行目」の順に表示されます。行の順番を入れ替えれば、表示される順番も入れ替わります。書いた順に実行されるという原則が、プログラムの基本です。

---

## 2. Console.WriteLine で値を出力する

[Console.WriteLine メソッド](https://learn.microsoft.com/dotnet/api/system.console.writeline) は、渡した値をコンソールに表示して、最後に改行します。

**書式：[Console.WriteLine メソッド](https://learn.microsoft.com/dotnet/api/system.console.writeline)**
```csharp
public static void WriteLine(object? value);
```

| パラメータ | 説明 |
|---|---|
| `value` | 表示する値。文字列、数値、真偽値など、どの型の値でも渡せる |

```csharp
Console.WriteLine("こんにちは");
Console.WriteLine(42);
Console.WriteLine(3.14);
Console.WriteLine(true);
```

```
こんにちは
42
3.14
True
```

`true` は `True` と表示されます。

> 💡 **ポイント**: `Console` の正式な名前は `System.Console` です。`System` は、型をまとめる **名前空間** の名前です。ここで名前空間を書かずに済んでいるのは、`dotnet new console` で作ったプロジェクトの設定（[.NET SDK と dotnet CLI](/unity-csharp-learning/csharp/dotnet-sdk/) の `ImplicitUsings`）で、`System` があらかじめ読み込まれているからです。読み込まれていない名前空間の型を使うときは、`using` ディレクティブが必要です。詳しくは [名前空間と using ディレクティブ（補足）](/unity-csharp-learning/csharp/using-directives/) で学びます。

改行しない [Console.Write メソッド](https://learn.microsoft.com/dotnet/api/system.console.write) もあります。`Console.Write` で表示した後の値は、同じ行に続けて表示されます。

```csharp
Console.Write("A");
Console.Write("B");
Console.WriteLine("C");
Console.WriteLine("D");
```

```
ABC
D
```

---

## 3. リテラルと型

コードに直接書く値を **リテラル** といいます。リテラルには、それぞれ **型** が決まっています。型は、その値がどのような種類のデータかを表します。

| 書き方の例 | [型](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/built-in-types) | 説明 |
|---|---|---|
| `"こんにちは"` | `string`（文字列） | `"` で囲んだ文字の並び |
| `42` | `int`（整数） | 小数点のない数値 |
| `3.14` | `double`（小数） | 小数点を含む数値 |
| `true` / `false` | `bool`（真偽値） | 正しい / 正しくない |

コンパイラーは、型を見て値の扱い方を決めます。たとえば `"42"` と `42` は見た目が似ていますが、`"42"` は文字列、`42` は整数で、別の種類のデータとして扱われます。

---

## 4. 算術演算

2 つの値を演算子でつないで計算する式を、**二項演算式** といいます。

**書式：二項演算式**
```
左辺 演算子 右辺
```

| 要素 | 説明 |
|---|---|
| `左辺` | 演算の対象となる左側の値 |
| `演算子` | 演算の種類を表す記号 |
| `右辺` | 演算の対象となる右側の値 |

式は、全体で 1 つの値になります。たとえば `3 + 2` という式は、全体で `5` という値になります。

C# の基本的な [算術演算子](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/arithmetic-operators) は次のとおりです。

| 演算子 | 意味 | 例 | 結果 |
|---|---|---|---|
| `+` | 加算 | `3 + 2` | `5` |
| `-` | 減算 | `3 - 2` | `1` |
| `*` | 乗算 | `3 * 2` | `6` |
| `/` | 除算 | `7 / 2` | `3` |
| `%` | 剰余（割り算の余り） | `7 % 2` | `1` |

式は、そのまま `Console.WriteLine` に渡せます。

```csharp
Console.WriteLine(3 + 2);
Console.WriteLine(3 - 2);
Console.WriteLine(3 * 2);
Console.WriteLine(7 / 2);
Console.WriteLine(7 % 2);
```

```
5
1
6
3
1
```

### 整数どうしの割り算

`int` どうしの `/` は、**小数点以下を切り捨てた整数** になります。`7 / 2` は `3.5` ではなく `3` です。小数の結果がほしいときは、どちらかを小数のリテラルにします。

```csharp
Console.WriteLine(7 / 2);
Console.WriteLine(7.0 / 2);
```

```
3
3.5
```

`7.0` は `double` のリテラルです。`int` と `double` を混ぜて計算すると、`double` として計算されます。型を混ぜた計算の規則は、[プリミティブ型と型変換](/unity-csharp-learning/csharp/primitive-types/) で学びます。

### 演算の優先順位

1 つの式に複数の演算子があるときは、数学と同じように、`*`・`/`・`%` が `+`・`-` より先に計算されます。先に計算したい部分は `( )` で囲みます。

```csharp
Console.WriteLine(2 + 3 * 4);
Console.WriteLine((2 + 3) * 4);
```

```
14
20
```

---

## 5. 変数

**変数** は、値に名前を付けて保存しておく仕組みです。同じ値を何度も使うときや、値を後から変えたいときに使います。

変数を使うときの操作には、**宣言**・**初期化子**・**代入** の 3 つがあります。

### 宣言 — 変数を用意する

**書式：[変数の宣言](https://learn.microsoft.com/dotnet/csharp/language-reference/statements/declarations)**
```
型 変数名;
```

| 要素 | 説明 |
|---|---|
| `型` | 変数に入れられる値の種類（`int`・`double`・`string` など） |
| `変数名` | 変数を区別するための名前 |

```csharp
int score;
```

`score` という名前の、`int` の値を入れられる変数を用意します。この時点では、変数にはまだ値が入っていません。

### 初期化子 — 宣言と同時に値を入れる

**書式：[変数の宣言（初期化子あり）](https://learn.microsoft.com/dotnet/csharp/language-reference/statements/declarations)**
```
型 変数名 = 式;
```

| 要素 | 説明 |
|---|---|
| `型 変数名` | 変数の宣言 |
| `= 式` | 初期化子。変数に最初に入れる値 |

```csharp
int score = 100;
```

宣言の `=` から後ろを **初期化子** といいます。変数を用意すると同時に、最初の値を入れます。

### 代入 — 後から値を入れる

**書式：[代入](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/assignment-operator)**
```
変数名 = 式;
```

| 要素 | 説明 |
|---|---|
| `変数名` | 値を入れる変数（宣言済みのもの） |
| `=` | 代入演算子。右辺の値を左辺の変数に入れる |
| `式` | 代入する値。リテラル、変数、式など |

```csharp
int score = 100;
Console.WriteLine(score);

score = 200;
Console.WriteLine(score);
```

```
100
200
```

`変数名 = 式;` で変数に新しい値を入れることを **代入** といいます。`=` は「右辺の値を左辺の変数に入れる」という意味で、数学の「等しい」とは違います。

### 3 つの操作の関係

```csharp
int score;
score = 0;
int level = 1;
level = level + 1;

Console.WriteLine(score);
Console.WriteLine(level);
```

```
0
2
```

1. `int score;` は宣言です。`score` にはまだ値が入っていません
2. `score = 0;` は代入です。`score` に `0` を入れます
3. `int level = 1;` は、初期化子のある宣言です。`level` を用意して `1` を入れます
4. `level = level + 1;` は代入です。`level` の今の値 `1` に 1 を足し、その結果 `2` を `level` に入れます

---

## よくあるミス

### 値を入れていない変数を使う

```csharp
// ❌ NG: 値を入れていない変数は読み取れない
// int score;
// Console.WriteLine(score);  // CS0165
```

C# では、宣言しただけで値を入れていない変数を読み取ろうとすると、コンパイルエラーになります。変数を使う前に、初期化子か代入で値を入れます。

### 整数どうしの割り算の結果を小数の変数に入れる

```csharp
double result = 7 / 2;
Console.WriteLine(result);
```

```
3
```

`result` は `double` ですが、`3.5` にはなりません。右辺の `7 / 2` が先に `int` どうしで計算されて `3` になり、それが `double` の `3` として代入されるからです。`7.0 / 2` のように、計算の時点で小数にします。

---

## ワンポイントアドバイス

### var による型推論

初期化子のある宣言では、型の代わりに [var](https://learn.microsoft.com/dotnet/csharp/language-reference/statements/declarations#implicitly-typed-local-variables) と書けます。コンパイラーが、初期化子の値から型を決めます。

```csharp
var message = "Hello";
var score = 100;

Console.WriteLine(message.GetType());
Console.WriteLine(score.GetType());
```

```
System.String
System.Int32
```

`message` は `string`、`score` は `int` になります（`System.String` と `System.Int32` は、それぞれ `string` と `int` の .NET での名前です）。型が決まるのはコンパイルのときで、後から別の型の値を入れることはできません。初期化子のない宣言では、`var` は使えません。

---

## まとめ

- プログラムは上から下へ 1 行ずつ実行される（逐次実行）
- `Console.WriteLine` は値を表示して改行する。`Console.Write` は改行しない
- リテラルには型がある（`string`・`int`・`double`・`bool` など）
- 算術演算子は `+`・`-`・`*`・`/`・`%`。`int` どうしの `/` は小数点以下を切り捨てる。`*`・`/`・`%` は `+`・`-` より先に計算される
- 変数は、値に名前を付けて保存する仕組み
  - 宣言：`型 変数名;` で変数を用意する
  - 初期化子：`型 変数名 = 式;` で、宣言と同時に値を入れる
  - 代入：`変数名 = 式;` で、後から値を入れる

---

## 理解度チェック

1. 次のコードを実行すると何が出力されますか？

   ```csharp
   Console.WriteLine(10 / 3);
   Console.WriteLine(10 % 3);
   Console.WriteLine(10 - 4 / 2);
   ```

2. 次のコードは、コンパイルエラーになります。何が問題ですか？

   ```csharp
   int count;
   count = count + 1;
   Console.WriteLine(count);
   ```

3. 次のコードの `x` の型は何になりますか？

   ```csharp
   var x = 2.5;
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. 次のように出力されます。`10 / 3` は `int` どうしの割り算なので、小数点以下が切り捨てられて `3` です。`10 % 3` は余りの `1` です。`10 - 4 / 2` は、`4 / 2` が先に計算されるので `8` です。

   ```
   3
   1
   8
   ```

2. `count` に値を入れないまま、`count + 1` で読み取っているからです（CS0165）。`int count = 0;` のように、初期化子で値を入れてから使います。
3. `double` です。`2.5` は小数点を含むリテラルなので `double` 型で、`x` の型も `double` になります。

</details>

---

## 次のステップ

[名前空間と using ディレクティブ（補足）](/unity-csharp-learning/csharp/using-directives/) では、`Console` の正式な名前と、名前空間にある型を `using` ディレクティブで使えるようにする方法を学びます。
