---
layout: page
title: プリミティブ型と型変換
permalink: /csharp/primitive-types/
---

# プリミティブ型と型変換

C# には、値の種類ごとに型が用意されています。どの型を選ぶかで、扱える値の範囲、精度、使うメモリの量が変わります。このページでは、数値・文字・文字列の型と、型どうしの変換、異なる型を混ぜた演算の規則を学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- 主な整数型・浮動小数点型の種類と表現範囲を説明できる
- 符号あり / 符号なしの整数型の違いを説明できる
- `char` と `string` の違いを説明できる
- 暗黙的な型変換とキャスト（明示的な型変換）の違いを説明できる
- 異なる型を混ぜた演算の結果が、どの型になるかを説明できる

## 前提知識

- [最初のプログラムと変数](/unity-csharp-learning/csharp/variables/) を読んでいること

---

## 1. 整数型の種類と表現範囲

C# の [整数型](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/integral-numeric-types) は、「何ビットで値を記憶するか」と「負の数を扱うか（符号）」の組み合わせで決まります。

| 型名 | ビット数 | 符号 | 最小値 | 最大値 |
|---|---|---|---|---|
| `byte` | 8 | なし | 0 | 255 |
| `sbyte` | 8 | あり | -128 | 127 |
| `short` | 16 | あり | -32,768 | 32,767 |
| `ushort` | 16 | なし | 0 | 65,535 |
| `int` | 32 | あり | -2,147,483,648 | 2,147,483,647 |
| `uint` | 32 | なし | 0 | 4,294,967,295 |
| `long` | 64 | あり | -9,223,372,036,854,775,808 | 9,223,372,036,854,775,807 |
| `ulong` | 64 | なし | 0 | 18,446,744,073,709,551,615 |

> 💡 **型の選び方**: 迷ったら `int` を使います。`int` は最もよく使われる整数型です。`int` の範囲を超える大きな数（累計の数など）には `long` を、色の成分（0〜255）のような小さな数には `byte` を使います。

型の最大値と最小値は、[int.MaxValue フィールド](https://learn.microsoft.com/dotnet/api/system.int32.maxvalue) と [int.MinValue フィールド](https://learn.microsoft.com/dotnet/api/system.int32.minvalue) で確かめられます。`byte.MaxValue` や `long.MaxValue` のように、ほかの整数型にも同じものがあります。

**書式：[int.MaxValue フィールド](https://learn.microsoft.com/dotnet/api/system.int32.maxvalue)**
```csharp
public const int MaxValue = 2147483647;
```

```csharp
Console.WriteLine(int.MaxValue);
Console.WriteLine(int.MinValue);
Console.WriteLine(byte.MaxValue);
```

```
2147483647
-2147483648
255
```

### 符号あり と 符号なし

整数型には、**符号あり**（signed）と **符号なし**（unsigned）の 2 種類があります。

- **符号あり**（`int`・`long` など）：負の数も表せる。同じビット数の符号なしの型と比べると、正の最大値はおよそ半分になる
- **符号なし**（`uint`・`ulong` など）：0 以上の値だけを表せる。その分、同じビット数でより大きな正の数を表せる

8 ビットの型で比べると、次のようになります。どちらも 2⁸ = 256 通りの値を表しますが、範囲の割り当て方が違います。

| 型 | 範囲 | 256 通りの割り当て方 |
|---|---|---|
| `sbyte` | -128 〜 127 | 負の数・0・正の数に振り分ける |
| `byte` | 0 〜 255 | すべて 0 以上に使う |

### オーバーフロー

型の範囲を超える計算をすると、**オーバーフロー** が起きます。C# の既定の動作では、例外は発生せず、値が反対側の端に折り返します。

```csharp
int maxInt = int.MaxValue;
Console.WriteLine(maxInt);
Console.WriteLine(maxInt + 1);
```

```
2147483647
-2147483648
```

最大値に 1 を足すと、最小値になります。オーバーフローはエラーにならずに起きるので、扱う値の範囲に合った型を選ぶことが大切です。なぜ折り返すのかは、[数値リテラルと型エイリアス（補足）](/unity-csharp-learning/csharp/numeric-literals/) で説明します。

---

## 2. 浮動小数点型の種類と精度

小数を扱う [浮動小数点型](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types) は 3 種類あります。

| 型名 | ビット数 | 有効桁数 | 主な用途 |
|---|---|---|---|
| `float` | 32 | 約 6〜9 桁 | 3D グラフィックスの座標・角度など（精度より速度とメモリを優先） |
| `double` | 64 | 約 15〜17 桁 | 一般的な小数の計算（C# の既定） |
| `decimal` | 128 | 28〜29 桁 | 金額の計算など（10 進数の小数を誤差なく扱いたい場合） |

同じ `1 / 3` を計算すると、型によって表せる桁数が違うことがわかります。

```csharp
Console.WriteLine(1.0f / 3);
Console.WriteLine(1.0 / 3);
Console.WriteLine(1.0m / 3);
```

```
0.33333334
0.3333333333333333
0.3333333333333333333333333333
```

### リテラルのサフィックス

小数のリテラルは、既定で `double` 型です。`float` や `decimal` のリテラルを書くには、**サフィックス**（接尾辞）を付けます。

| 書き方 | 型 |
|---|---|
| `3.14` | `double` |
| `3.14f` | `float` |
| `3.14m` | `decimal` |

```csharp
double d = 3.14;
float f = 3.14f;
decimal m = 3.14m;
Console.WriteLine($"{d}, {f}, {m}");
```

```
3.14, 3.14, 3.14
```

`$"..."` は、文字列の中に値を埋め込む書き方です。6 節で説明します。

### 浮動小数点数の誤差

`float` と `double` は、値を 2 進数で **近似** して記憶します。そのため、計算の結果に小さな誤差が出ることがあります。

```csharp
Console.WriteLine(0.1 + 0.2);
Console.WriteLine(0.1 + 0.2 == 0.3);
```

```
0.30000000000000004
False
```

`0.1` や `0.2` は、2 進数ではぴったり表せません。金額のように、10 進数の小数を誤差なく扱いたいときは `decimal` を使います。

---

## 3. char — 1 文字を扱う型

[char](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/char) は **1 文字** を表す型です。リテラルは **シングルクォート**（`'`）で囲みます。

```csharp
char c = 'A';
Console.WriteLine(c);
```

```
A
```

`char` の値は、内部では文字に割り当てられた番号（UTF-16 という方式での 0〜65535 の整数）として記憶されています。そのため、整数に変換したり、計算に使ったりできます。

```csharp
char c = 'A';
Console.WriteLine((int)c);
Console.WriteLine((char)(c + 1));
```

```
65
B
```

`'A'` の番号は `65` です。`c + 1` は `int` の `66` になるので、`(char)` で文字に戻すと `'B'` になります。`(int)` や `(char)` は型を変換するキャストで、5 節で説明します。

---

## 4. string — 文字列を扱う型

[string](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/reference-types#the-string-type) は **文字の並び** を表す型です。リテラルは **ダブルクォート**（`"`）で囲みます。文字数は [Length プロパティ](https://learn.microsoft.com/dotnet/api/system.string.length) で調べられます。

```csharp
string name = "Alice";
Console.WriteLine(name);
Console.WriteLine(name.Length);
```

```
Alice
5
```

### char と string の違い

| | `char` | `string` |
|---|---|---|
| 文字数 | 必ず 1 文字 | 0 文字以上 |
| リテラルの囲み | `'`（シングルクォート） | `"`（ダブルクォート） |
| 例 | `'A'` | `"Alice"`、`"A"`、`""` |

`"A"` のように 1 文字でも、ダブルクォートで囲めば `string` です。

### 文字列の連結

`+` 演算子で文字列をつなげられます。

```csharp
string firstName = "Alice";
string lastName = "Smith";
string fullName = firstName + " " + lastName;
Console.WriteLine(fullName);
```

```
Alice Smith
```

### エスケープシーケンス

改行やダブルクォートのように、そのままでは文字列に書けない文字は、`\` から始まる **エスケープシーケンス** で表します。`char` のリテラルでも使えます。

| シーケンス | 意味 |
|---|---|
| `\n` | 改行 |
| `\t` | タブ |
| `\\` | `\` そのもの |
| `\"` | `"` そのもの |
| `\'` | `'` そのもの |

```csharp
Console.WriteLine("1行目\n2行目");
Console.WriteLine("A\tB");
Console.WriteLine("\"Hello\"");
Console.WriteLine("C:\\Users");
```

```
1行目
2行目
A	B
"Hello"
C:\Users
```

---

## 5. 型変換

ある型の値を別の型の値にすることを **型変換** といいます。

### 暗黙的な型変換

情報が失われない変換は、自動的に行われます。これを **暗黙的な型変換**（implicit conversion）といいます。

```csharp
int i = 42;
long l = i;
double d = i;
Console.WriteLine($"{l}, {d}");
```

```
42, 42
```

主な [暗黙的な数値変換](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/numeric-conversions#implicit-numeric-conversions) は、次の矢印の向きに行われます。

```
byte → short → int → long → float → double
```

表せる範囲が広い型へは、暗黙的に変換できます。ただし `long` から `float` への変換のように、値は範囲に収まっても、桁数が足りずに下の桁が丸められることがあります。

### 明示的な型変換（キャスト）

情報が失われる可能性のある変換は、自動では行われません。変換先の型を書いて、明示的に変換します。これを **キャスト**（cast）といいます。

**書式：[キャスト式](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/type-testing-and-cast#cast-expression)**
```
(変換先の型)式
```

| 要素 | 説明 |
|---|---|
| `(変換先の型)` | 変換したい型名を `( )` で囲む |
| `式` | 変換する値 |

```csharp
double d = 3.7;
int i = (int)d;
Console.WriteLine(i);
```

```
3
```

小数から整数へのキャストでは、小数点以下が **切り捨て** られます。四捨五入ではないので、`3.7` は `3` になります。

範囲を超える値をキャストすると、上位のビットが捨てられ、元とはまったく違う値になります。

```csharp
int big = 300;
byte b = (byte)big;
Console.WriteLine(b);
```

```
44
```

`byte` の範囲は 0〜255 なので、`300` は収まりません。`300` を 256 で割った余りの `44` になります。

### 文字列から数値への変換

文字列を数値に変換するには、[int.Parse メソッド](https://learn.microsoft.com/dotnet/api/system.int32.parse) などを使います。

**書式：[int.Parse メソッド](https://learn.microsoft.com/dotnet/api/system.int32.parse)**
```csharp
public static int Parse(string s);
```

| パラメータ | 説明 |
|---|---|
| `s` | 整数を表す文字列 |

```csharp
string s = "42";
int i = int.Parse(s);
Console.WriteLine(i + 1);
```

```
43
```

数値として読めない文字列を渡すと、実行したときに [FormatException](https://learn.microsoft.com/dotnet/api/system.formatexception) という例外が発生します。例外については、[例外の基本](/unity-csharp-learning/csharp/exceptions/) で学びます。

```csharp
// ❌ NG: 数値として読めない文字列を変換している
// int.Parse("abc");  // FormatException
```

### 数値から文字列への変換

数値を文字列に変換するには、[ToString メソッド](https://learn.microsoft.com/dotnet/api/system.object.tostring) を使います。

```csharp
int score = 100;
string s = score.ToString();
Console.WriteLine(s + "点");
```

```
100点
```

---

## 6. 異なる型の演算

### 数値型の昇格

整数と浮動小数点数のように、異なる型の数値を混ぜて演算すると、範囲の広い型に変換されてから計算されます。これを **数値の昇格**（numeric promotion）といいます。

```csharp
int i = 5;
double d = 1.5;
var result = i + d;
Console.WriteLine(result);
Console.WriteLine(result.GetType());
```

```
6.5
System.Double
```

`i` が `double` に変換されてから計算されるので、`result` は `double` になります。

| 演算の組み合わせ | 結果の型 |
|---|---|
| `int` と `int` | `int` |
| `int` と `long` | `long` |
| `int` と `float` | `float` |
| `int` と `double` | `double` |
| `float` と `double` | `double` |

> 💡 **ポイント**: 範囲の広い型に合わせると覚えましょう。ただし、`byte` や `short` どうしの演算は、`int` に変換してから計算されるので、結果は `int` になります。

### string と数値の + 演算

`string` と数値を `+` でつなぐと、数値が文字列に変換されてから連結されます。

```csharp
int score = 100;
string message = "スコア: " + score;
Console.WriteLine(message);
```

```
スコア: 100
```

`+` は左から順に計算されるので、数値の足し算と組み合わせると、意図しない結果になることがあります。

```csharp
Console.WriteLine("1 + 2 = " + 1 + 2);
Console.WriteLine("1 + 2 = " + (1 + 2));
```

```
1 + 2 = 12
1 + 2 = 3
```

1 行目は、`"1 + 2 = " + 1` が先に計算されて `"1 + 2 = 1"` という文字列になり、さらに `+ 2` で `"1 + 2 = 12"` になります。数値の計算は、`( )` で囲んで先に済ませます。

### 文字列補間

文字列の前に `$` を付けると、文字列の中に `{式}` で値を埋め込めます。これを [文字列補間](https://learn.microsoft.com/dotnet/csharp/language-reference/tokens/interpolated) といいます。`+` でつなぐより読みやすく、計算の順序の問題も起きません。

**書式：[文字列補間](https://learn.microsoft.com/dotnet/csharp/language-reference/tokens/interpolated)**
```
$"文字列{式}文字列"
```

```csharp
string name = "Alice";
int score = 100;
Console.WriteLine($"{name} のスコアは {score} 点です。");
Console.WriteLine($"1 + 2 = {1 + 2}");
```

```
Alice のスコアは 100 点です。
1 + 2 = 3
```

`{ }` の中の式が計算され、その結果が文字列として埋め込まれます。以降のページのコード例でも、値を表示するときによく使います。

---

## よくあるミス

### float のリテラルに f を付け忘れる

```csharp
// ❌ NG: 3.14 は double のリテラルなので、float の変数に入れられない
// float speed = 1.5;  // CS0664
```

`1.5` は `double` のリテラルです。`double` から `float` へは暗黙的に変換できないので、`1.5f` と書きます。

### char と string のリテラルを混同する

```csharp
// ❌ NG: "A" は string のリテラルなので、char の変数に入れられない
// char c = "A";  // CS0029
```

1 文字でも、`"` で囲むと `string` です。`char` には `'A'` と書きます。

### Math.Round が四捨五入だと思い込む

キャストは小数点以下を切り捨てます。丸めたいときは [Math.Round メソッド](https://learn.microsoft.com/dotnet/api/system.math.round) を使いますが、`Math.Round` の既定の動作は四捨五入ではありません。ちょうど `.5` のときは、結果が偶数になる方に丸めます。

```csharp
Console.WriteLine(Math.Round(2.5));
Console.WriteLine(Math.Round(3.5));
Console.WriteLine(Math.Round(2.5, MidpointRounding.AwayFromZero));
```

```
2
4
3
```

学校で習う四捨五入にしたいときは、2 つ目の引数に [MidpointRounding.AwayFromZero](https://learn.microsoft.com/dotnet/api/system.midpointrounding) を指定します。

---

## ワンポイントアドバイス

### checked でオーバーフローを検出する

計算を [checked](https://learn.microsoft.com/dotnet/csharp/language-reference/statements/checked-and-unchecked) で囲むと、オーバーフローしたときに値を折り返さず、[OverflowException](https://learn.microsoft.com/dotnet/api/system.overflowexception) という例外を発生させます。オーバーフローしたら困る計算で、間違いに気付けるようにするために使います。

```csharp
int maxInt = int.MaxValue;
int result = checked(maxInt + 1);
```

このコードは、実行すると `OverflowException` が発生して止まります。

---

## まとめ

- 整数型は、ビット数と符号の有無で決まる。迷ったら `int` を使う
- 範囲を超えるとオーバーフローが起き、値が折り返す
- 浮動小数点型は `float`（32 ビット）・`double`（64 ビット、既定）・`decimal`（128 ビット、10 進数の小数を正確に扱う）の 3 種類。`float` と `double` の計算には誤差が出ることがある
- 小数のリテラルは `3.14` が `double`、`3.14f` が `float`、`3.14m` が `decimal`
- `char` は 1 文字（`'A'`）、`string` は 0 文字以上の文字列（`"Alice"`）
- 情報が失われない変換は暗黙的に行われる。情報が失われる可能性のある変換は、キャスト `(型)式` で明示的に行う。小数から整数へのキャストは切り捨て
- 異なる数値型の演算では、範囲の広い型に昇格してから計算される
- `string` と数値の `+` は文字列の連結になる。値を埋め込むには文字列補間 `$"..."` が便利

---

## 理解度チェック

1. `int` と `uint` の違いを説明してください。また、同じ 32 ビットなのに最大値が異なるのはなぜですか？
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   double d = 9.9;
   int i = (int)d;
   Console.WriteLine(i);
   Console.WriteLine("答えは " + 3 + 4);
   Console.WriteLine($"答えは {3 + 4}");
   ```

3. 次のコードはコンパイルエラーになります。どこを直せばよいですか？

   ```csharp
   float speed = 5.0;
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. `int` は負の数も扱えます（符号あり）が、`uint` は 0 以上の値しか扱えません（符号なし）。どちらも 2³² 通りの値を表せますが、`int` はその半分を負の数に使うので、正の最大値が `uint` のおよそ半分になります。
2. 次のように出力されます。キャストは小数点以下を切り捨てるので `9` です。`"答えは " + 3 + 4` は左から順に連結されて `答えは 34` になります。文字列補間では `{3 + 4}` が計算されて `7` になります。

   ```
   9
   答えは 34
   答えは 7
   ```

3. `5.0` は `double` のリテラルなので、`float` の変数には入れられません。`f` を付けて `5.0f` と書きます。

</details>

---

## 次のステップ

[数値リテラルと型エイリアス（補足）](/unity-csharp-learning/csharp/numeric-literals/) では、16 進数や 2 進数のリテラル、`int` と `System.Int32` の関係、負の数のビット表現を学びます。
