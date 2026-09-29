---
layout: page
title: 数値リテラルと型エイリアス（補足）
permalink: /csharp/numeric-literals/
---

# 数値リテラルと型エイリアス（補足）

このページは、[プリミティブ型と型変換](/unity-csharp-learning/csharp/primitive-types/) の補足です。`int` などの型名と .NET の型の関係、16 進数・2 進数のリテラル、リテラルの型を決めるサフィックス、負の数のビット表現を学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `int` などの C# のキーワードが、.NET の型の別名であることを説明できる
- 10 進数・16 進数・2 進数のリテラルを書ける
- 型サフィックス（`L`・`f`・`m` など）の意味と、サフィックスがないときのリテラルの型を説明できる
- 2 の補数による負の数の表し方と、オーバーフローで値が折り返す理由を説明できる

## 前提知識

- [プリミティブ型と型変換](/unity-csharp-learning/csharp/primitive-types/) を読んでいること

---

## 1. C# のキーワードは .NET の型の別名

`int`・`double`・`string` などの C# のキーワードは、.NET に定義されている型に、C# が付けた **別名**（エイリアス）です。どちらで書いても、まったく同じ型を表します。

```csharp
int x = 42;
System.Int32 y = 42;

Console.WriteLine(x.GetType());
Console.WriteLine(y.GetType());
```

```
System.Int32
System.Int32
```

`GetType` は、値の型を返すメソッドです。`x` も `y` も `System.Int32` 型であることがわかります。

主な [組み込み型](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/built-in-types) の対応は、次のとおりです。

| C# のキーワード | .NET の型 | ビット数 |
|---|---|---|
| `sbyte` | `System.SByte` | 8 |
| `byte` | `System.Byte` | 8 |
| `short` | `System.Int16` | 16 |
| `ushort` | `System.UInt16` | 16 |
| `int` | `System.Int32` | 32 |
| `uint` | `System.UInt32` | 32 |
| `long` | `System.Int64` | 64 |
| `ulong` | `System.UInt64` | 64 |
| `float` | `System.Single` | 32 |
| `double` | `System.Double` | 64 |
| `decimal` | `System.Decimal` | 128 |
| `bool` | `System.Boolean` | — |
| `char` | `System.Char` | 16 |
| `string` | `System.String` | — |

> 💡 **どちらを使うか**: C# のコードでは、`int` や `string` などのキーワードを使うのが一般的です。`System.Int32` のような .NET の型名は、`GetType` の結果やエラーメッセージ、公式ドキュメントでよく目にします。

---

## 2. 数値リテラルの表記法

### 10 進数・16 進数・2 進数

[整数リテラル](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/integral-numeric-types#integer-literals) は、10 進数のほかに、**16 進数**（`0x` で始める）と **2 進数**（`0b` で始める）でも書けます。

| プレフィックス | 基数 | 例 | 値 |
|---|---|---|---|
| なし | 10 進数 | `255` | 255 |
| `0x` | 16 進数 | `0xFF` | 255 |
| `0b` | 2 進数 | `0b1111_1111` | 255 |

```csharp
int dec = 255;
int hex = 0xFF;
int bin = 0b1111_1111;

Console.WriteLine(dec);
Console.WriteLine(hex);
Console.WriteLine(bin);
```

```
255
255
255
```

16 進数の `F` は 15 を表すので、`0xFF` は 15 × 16 + 15 = 255 です。16 進数は、色の値（`0xFF0000` は赤）やビットの並びを短く書きたいときによく使います。2 進数は、ビットの並びをそのまま書きたいときに便利です。

### 桁区切りの _

長い数値のリテラルは、`_`（アンダースコア）で桁を区切ると読みやすくなります。値には影響しません。

```csharp
int million = 1_000_000;
int color = 0xFF_80_00;
int flags = 0b_1010_0011;

Console.WriteLine(million);
Console.WriteLine(color);
Console.WriteLine(flags);
```

```
1000000
16744448
163
```

`0b_1010_0011` のように、`0x` や `0b` の直後にも `_` を書けます。

---

## 3. リテラルの型サフィックス

数値のリテラルに **サフィックス**（接尾辞）を付けると、リテラルの型を指定できます。

| サフィックス | 型 | 例 |
|---|---|---|
| `U` または `u` | `uint`（範囲を超えるときは `ulong`） | `42U` |
| `L` または `l` | `long`（範囲を超えるときは `ulong`） | `42L` |
| `UL` または `ul` | `ulong` | `42UL` |
| `f` または `F` | `float` | `3.14f` |
| `d` または `D` | `double` | `3.14d` |
| `m` または `M` | `decimal` | `3.14m` |

```csharp
var a = 42;
var b = 42L;
var c = 42U;
var d = 3.14;
var e = 3.14f;
var f = 3.14m;

Console.WriteLine($"{a.GetType()} {b.GetType()} {c.GetType()}");
Console.WriteLine($"{d.GetType()} {e.GetType()} {f.GetType()}");
```

```
System.Int32 System.Int64 System.UInt32
System.Double System.Single System.Decimal
```

> 💡 小文字の `l` は、フォントによっては数字の `1` と見分けにくいので、`L` と大文字で書くのが一般的です。

### サフィックスがないときの型

サフィックスのない小数のリテラルは `double` です。サフィックスのない整数のリテラルは、`int`・`uint`・`long`・`ulong` の順に、値が収まる最初の型になります。

```csharp
var small = 100;
var big = 3_000_000_000;
var huge = 10_000_000_000;

Console.WriteLine(small.GetType());
Console.WriteLine(big.GetType());
Console.WriteLine(huge.GetType());
```

```
System.Int32
System.UInt32
System.Int64
```

`3_000_000_000` は `int` の最大値（約 21 億）を超えますが、`uint` の最大値（約 42 億）には収まるので `uint` です。

---

## 4. 2 の補数 — 負の数のビット表現

コンピューターは、内部では 0 と 1 しか扱えません。符号ありの整数型が負の数をどう表しているかを、8 ビットの `byte` と `sbyte` で見てみましょう。

### byte（符号なし）

8 桁の 2 進数に、`0000 0000`（0）から `1111 1111`（255）までを順に割り当てます。

| 10 進数 | 2 進数 |
|---|---|
| `0` | `0000 0000` |
| `1` | `0000 0001` |
| `127` | `0111 1111` |
| `128` | `1000 0000` |
| `255` | `1111 1111` |

### sbyte（符号あり）

符号ありの型では、いちばん左のビット（**最上位ビット**、MSB）が `0` なら 0 以上、`1` なら負の数を表します。そのため、正の最大値は `0111 1111` の 127 になります。

| 10 進数 | 2 進数 | 備考 |
|---|---|---|
| `127` | `0111 1111` | 最上位ビットが 0 → 正の最大値 |
| `1` | `0000 0001` | |
| `0` | `0000 0000` | |
| `-1` | `1111 1111` | 最上位ビットが 1 → 負 |
| `-127` | `1000 0001` | |
| `-128` | `1000 0000` | 負の最小値 |

### 2 の補数による負の数の求め方

負の数のビットの並びは、**2 の補数**（two's complement）という方式で決まります。「元の数のビットをすべて反転して、1 を足す」という手順で求められます。

`-5` のビットの並びを求めると、次のようになります。

```
① 5 のビットの並び      0000 0101
② すべてのビットを反転  1111 1010
③ 1 を足す              1111 1011  ← -5 のビットの並び
```

`1111 1011` に同じ手順を行うと、`0000 0101`（5）に戻ります。

> 💡 **2 の補数を使う理由**: この方式なら、正の数と負の数を区別せずに、同じ足し算の回路で正しく計算できます。現在のほとんどのコンピューターが、この方式を使っています。

### オーバーフローで折り返す理由

`sbyte` の最大値 `127`（`0111 1111`）に 1 を足すと、次のようになります。

```
  0111 1111   (127)
+ 0000 0001   (  1)
-----------
  1000 0000   → 2 の補数では -128
```

繰り上がりで最上位ビットが `1` になるので、符号ありの型では `-128` と解釈されます。[プリミティブ型と型変換](/unity-csharp-learning/csharp/primitive-types/) で見た、`int.MaxValue + 1` が `int.MinValue` になる現象も、32 ビットで同じことが起きた結果です。

同じビットの並びを、`byte` と `sbyte` の両方で見てみます。`unchecked` は、範囲を超える定数のキャストを、エラーにせずにビットの並びのまま変換するための指定です。

```csharp
byte b = 0b1000_0000;
sbyte s = unchecked((sbyte)0b1000_0000);

Console.WriteLine(b);
Console.WriteLine(s);
```

```
128
-128
```

同じ `1000 0000` でも、`byte` では `128`、`sbyte` では `-128` です。

---

## まとめ

- `int` は `System.Int32` の別名。C# のキーワードと .NET の型名は、どちらで書いても同じ型
- 整数のリテラルは、10 進数（`255`）・16 進数（`0xFF`）・2 進数（`0b1111_1111`）で書ける。`_` で桁を区切れる
- サフィックス `U`・`L`・`UL`・`f`・`d`・`m` で、リテラルの型を指定できる
- サフィックスのない整数のリテラルは、`int`・`uint`・`long`・`ulong` の順に、値が収まる最初の型になる。小数のリテラルは `double`
- 符号ありの整数型は、最上位ビットで符号を表す。負の数は 2 の補数（ビットを反転して 1 を足す）で表す
- オーバーフローで最大値の次が最小値になるのは、2 の補数で計算した自然な結果

---

## 理解度チェック

1. `int.MaxValue` を、.NET の型名を使って書き直してください。
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   Console.WriteLine(0x10);
   Console.WriteLine(0b101);
   Console.WriteLine(1_0_0);
   Console.WriteLine((5L).GetType());
   ```

3. `-1` を、8 ビットの 2 の補数で表してください。手順も示してください。

<details markdown="1">
<summary>解答を見る</summary>

1. `System.Int32.MaxValue` です。
2. 次のように出力されます。`0x10` は 16 進数の 10 で `16`、`0b101` は 2 進数の 101 で `5` です。`_` は値に影響しないので、`1_0_0` は `100` です。`5L` は `long`（`System.Int64`）です。

   ```
   16
   5
   100
   System.Int64
   ```

3. `1111 1111` です。

   ```
   ① 1 のビットの並び      0000 0001
   ② すべてのビットを反転  1111 1110
   ③ 1 を足す              1111 1111
   ```

</details>

---

## 次のステップ

[文字列リテラルと書式（補足）](/unity-csharp-learning/csharp/string-literals/) では、`\` や `"` をそのまま書ける文字列リテラルと、文字列補間で数値の表示のしかたや幅を決める方法を学びます。
