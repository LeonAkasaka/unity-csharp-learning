---
layout: page
title: Span<T> と ReadOnlySpan<T>
permalink: /csharp/span/
---

# Span\<T\> と ReadOnlySpan\<T\>

配列や文字列の一部を取り出すと、ふつうはその部分をコピーした新しい配列や文字列が作られます。**Span\<T\>** は、配列などの連続した要素の一部を、コピーせずに指して扱うための型です。このページでは、`Span<T>` で配列の一部を読み書きする方法と、読み取り専用の **ReadOnlySpan\<T\>** で文字列の一部を扱う方法を学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- 範囲演算子 `..` で配列の一部を取り出すと、新しい配列が作られることを説明できる
- `AsSpan` で配列の一部を指す `Span<T>` を作り、元の配列を読み書きできる
- `Slice` や範囲演算子で、`Span<T>` をさらに切り分けられる
- `ReadOnlySpan<T>` をパラメータにして、配列やその一部を受け取るメソッドを書ける
- `ReadOnlySpan<char>` で、文字列の一部をコピーせずに扱える

## 前提知識

- [ref ローカルと ref 戻り値](/unity-csharp-learning/csharp/ref-locals/) を読んでいること
- [文字列の不変性と StringBuilder](/unity-csharp-learning/csharp/string-immutability/) を読んでいること
- [インデックスと範囲（補足）](/unity-csharp-learning/csharp/ranges/) で範囲演算子 `..` を学んだこと

---

## 1. 配列の一部を取り出すとコピーが作られる

[インデックスと範囲（補足）](/unity-csharp-learning/csharp/ranges/) で学んだように、配列の一部は範囲演算子 `..` で取り出せます。

```csharp
int[] numbers = { 10, 20, 30, 40, 50 };
int[] part = numbers[1..4];
Console.WriteLine(string.Join(", ", part));
part[0] = 0;
Console.WriteLine(string.Join(", ", numbers));
```

```
20, 30, 40
10, 20, 30, 40, 50
```

`numbers[1..4]` は、3 つの要素をコピーした新しい配列を作ります。そのため、`part[0]` を書き換えても、元の `numbers` は変わりません。

配列の一部を読むだけのときでも、取り出すたびに新しい配列がヒープに作られます。[文字列の不変性と StringBuilder](/unity-csharp-learning/csharp/string-immutability/) で見た `Substring` も同じで、取り出すたびに新しい文字列が作られます。コピーを避けるには、配列と開始位置と長さの 3 つをメソッドに渡す方法もありますが、扱う値が増えて間違えやすくなります。

---

## 2. Span\<T\>

[Span\<T\> 構造体](https://learn.microsoft.com/dotnet/api/system.span-1) は、連続して並んだ `T` 型の要素の一部を指す型です。配列から `Span<T>` を作るには、[AsSpan メソッド](https://learn.microsoft.com/dotnet/api/system.memoryextensions.asspan) を使います。

**書式：[AsSpan メソッド](https://learn.microsoft.com/dotnet/api/system.memoryextensions.asspan)**
```
Span<T> 変数名 = 配列.AsSpan(開始位置, 長さ);
Span<T> 変数名 = 配列.AsSpan(開始位置);
Span<T> 変数名 = 配列;
```

| 書き方 | 指す範囲 |
|---|---|
| `配列.AsSpan(開始位置, 長さ)` | `開始位置` から `長さ` 個の要素 |
| `配列.AsSpan(開始位置)` | `開始位置` から最後までの要素 |
| `Span<T> 変数名 = 配列;` | 配列の全体。配列から `Span<T>` へは暗黙に変換できる |

`Span<T>` の要素は、配列と同じようにインデックスで読み書きでき、要素の数は `Length` で調べられます。

```csharp
int[] numbers = { 10, 20, 30, 40, 50 };
Span<int> span = numbers.AsSpan(1, 3);
Console.WriteLine(span.Length);
Console.WriteLine(span[0]);
span[0] = 0;
Console.WriteLine(string.Join(", ", numbers));
```

```
3
20
10, 0, 30, 40, 50
```

`span` は、`numbers` のインデックス `1` から 3 つの要素を指しています。`span[0]` は `numbers[1]` のことです。`span[0] = 0` で、元の配列の `numbers[1]` が書き換わりました。

`Span<T>` は、要素をコピーしません。持っているのは、範囲の先頭の要素への参照と、要素の数（`Length`）だけです。[ref ローカルと ref 戻り値](/unity-csharp-learning/csharp/ref-locals/) の ref ローカルが 1 つの変数を指すのに対して、`Span<T>` は並んだ複数の変数をまとめて指します。

![配列 numbers のインデックス 1 から 3 までを span が表している。span は先頭の要素 numbers[1] への参照と、Length = 3 だけを持つ様子](span-over-array.svg)

### ヒープに確保されるメモリを比べる

配列の一部を、範囲演算子で取り出す場合と、`Span<T>` で指す場合を 1000 回ずつ繰り返し、ヒープに確保されたメモリの量を比べます。

```csharp
int[] numbers = { 10, 20, 30, 40, 50 };
long before = GC.GetAllocatedBytesForCurrentThread();
int total = 0;
for (int i = 0; i < 1000; i++)
{
    int[] part = numbers[1..4];
    total += part[0];
}
long after = GC.GetAllocatedBytesForCurrentThread();
Console.WriteLine($"配列の範囲: {after - before} バイト");

before = GC.GetAllocatedBytesForCurrentThread();
for (int i = 0; i < 1000; i++)
{
    Span<int> part = numbers.AsSpan(1, 3);
    total += part[0];
}
after = GC.GetAllocatedBytesForCurrentThread();
Console.WriteLine($"Span: {after - before} バイト");
```

64 ビットの環境で実行した結果の例です。バイト数は実行環境によって変わることがあります。

```
配列の範囲: 40000 バイト
Span: 0 バイト
```

範囲演算子では、要素が 3 つの配列が 1000 個作られています。`Span<int>` は構造体なので、変数に値そのものが入り、ヒープにメモリを確保しません。

---

## 3. Span を切り分ける

`Span<T>` の一部を、さらに `Span<T>` として取り出せます。[Slice メソッド](https://learn.microsoft.com/dotnet/api/system.span-1.slice) か、範囲演算子を使います。どちらも、要素はコピーせずに、指す範囲だけを狭めた新しい `Span<T>` を返します。

**書式：[Span\<T\>.Slice メソッド](https://learn.microsoft.com/dotnet/api/system.span-1.slice)**
```
span.Slice(開始位置, 長さ)
span.Slice(開始位置)
span[開始位置..終了位置]
```

範囲演算子の書き方は、配列に使うときと同じです。開始位置や終了位置の省略、`^` を使った位置の指定は、[インデックスと範囲（補足）](/unity-csharp-learning/csharp/ranges/) を参照してください。

`Span<T>` の要素は、`foreach` で順に取り出せます。

```csharp
int[] numbers = { 10, 20, 30, 40, 50 };
Span<int> all = numbers;
Span<int> middle = all.Slice(1, 3);
Span<int> tail = all[3..];
foreach (int n in middle)
{
    Console.Write($"{n} ");
}
Console.WriteLine();
foreach (int n in tail)
{
    Console.Write($"{n} ");
}
Console.WriteLine();
```

```
20 30 40 
40 50 
```

範囲演算子は、配列に使うと新しい配列を作りますが、`Span<T>` に使うとコピーを作りません。同じ書き方でも、使う対象によって結果が違うことに注意します。

---

## 4. Span を受け取るメソッド

メソッドのパラメータを `Span<T>` や `ReadOnlySpan<T>` にすると、配列の全体も、配列の一部も、同じメソッドで処理できます。

```csharp
int[] numbers = { 10, 20, 30, 40, 50 };
Console.WriteLine(Sum(numbers));
Console.WriteLine(Sum(numbers.AsSpan(1, 3)));
Console.WriteLine(Sum(numbers.AsSpan()[3..]));

int Sum(ReadOnlySpan<int> values)
{
    int total = 0;
    foreach (int v in values)
    {
        total += v;
    }
    return total;
}
```

```
150
90
90
```

配列はそのまま `ReadOnlySpan<int>` に変換されて渡されます。配列の一部を合計したいときは、その部分を指す `Span<int>` を渡します。配列と開始位置と長さを別々に渡す必要はありません。

`Sum` のパラメータには、次の節で学ぶ `ReadOnlySpan<int>` を使っています。`Sum` は要素を読むだけなので、書き換えられない型にしておくと、呼び出す側は配列が書き換えられないことがわかります。

---

## 5. ReadOnlySpan\<T\>

[ReadOnlySpan\<T\> 構造体](https://learn.microsoft.com/dotnet/api/system.readonlyspan-1) は、読み取り専用の `Span<T>` です。要素を読めますが、書き換えることはできません。

```csharp
int[] numbers = { 10, 20, 30 };
ReadOnlySpan<int> span = numbers;
span[0] = 1;  // ❌ CS8331: 読み取り専用なので代入できない
```

`Span<T>` は `ReadOnlySpan<T>` に暗黙に変換できます。反対に、`ReadOnlySpan<T>` を `Span<T>` に変換することはできません。

### 文字列の一部を扱う

`string` の中身は書き換えられないので、文字列から作れるのは `ReadOnlySpan<char>` です。文字列の `AsSpan` で、文字列の一部を指す `ReadOnlySpan<char>` を作れます。

```csharp
string text = "id=1234;name=Alice";
ReadOnlySpan<char> name = text.AsSpan(13);
Console.WriteLine(name.Length);
Console.WriteLine(name[0]);
Console.WriteLine(name.ToString());
Console.WriteLine(name.SequenceEqual("Alice"));
```

```
5
A
Alice
True
```

`name` は、`text` の 13 文字目から最後までの `"Alice"` の部分を指しています。`ToString` を呼ぶと、指している部分から新しい `string` が作られます。文字列と中身を比べるには、[SequenceEqual メソッド](https://learn.microsoft.com/dotnet/api/system.memoryextensions.sequenceequal) を使います。

`Substring` と `AsSpan` で、文字列の一部を 1000 回ずつ取り出して比べます。

```csharp
string text = "id=1234;name=Alice";
long before = GC.GetAllocatedBytesForCurrentThread();
for (int i = 0; i < 1000; i++)
{
    string name = text.Substring(13);
}
long after = GC.GetAllocatedBytesForCurrentThread();
Console.WriteLine($"Substring: {after - before} バイト");

before = GC.GetAllocatedBytesForCurrentThread();
for (int i = 0; i < 1000; i++)
{
    ReadOnlySpan<char> name = text.AsSpan(13);
}
after = GC.GetAllocatedBytesForCurrentThread();
Console.WriteLine($"AsSpan: {after - before} バイト");
```

64 ビットの環境で実行した結果の例です。バイト数は実行環境によって変わることがあります。

```
Substring: 32000 バイト
AsSpan: 0 バイト
```

`Substring` は呼び出すたびに `"Alice"` の文字列を作りますが、`AsSpan` は元の文字列の一部を指すだけです。`ReadOnlySpan<char>` を使って文字列を解析する方法は、[文字列処理の割り当てを減らす](/unity-csharp-learning/csharp/string-performance/) で学びます。

---

## 6. インデクサは ref を返す

`Span<T>` のインデクサは、要素の値ではなく、要素への参照を ref 戻り値で返します。そのため、[ref ローカルと ref 戻り値](/unity-csharp-learning/csharp/ref-locals/) で学んだ ref ローカルで受け取れます。`span[0] = 0` で元の配列を書き換えられたのも、インデクサが要素への参照を返しているからです。

要素を別の配列にコピーしたいときは、[ToArray メソッド](https://learn.microsoft.com/dotnet/api/system.span-1.toarray) を使います。

```csharp
int[] numbers = { 10, 20, 30 };
Span<int> span = numbers;
ref int first = ref span[0];
first = 1;
int[] copy = span.ToArray();
copy[1] = 0;
Console.WriteLine(string.Join(", ", numbers));
Console.WriteLine(string.Join(", ", copy));
```

```
1, 20, 30
1, 0, 30
```

`first` は `numbers[0]` を指しているので、`first = 1` で配列が書き換わります。`ToArray` が返すのは新しい配列なので、`copy` を書き換えても `numbers` は変わりません。

---

## よくあるミス

### 配列の範囲を超えた Span を作る

`AsSpan` や `Slice` で、元の範囲を超える位置や長さを指定すると、実行したときに例外が発生します。

```csharp
int[] numbers = { 10, 20, 30 };
Span<int> span = numbers.AsSpan(1, 5);  // ❌ ArgumentOutOfRangeException
```

`Span<T>` は、作るときに範囲が元の配列の中に収まっているかを確かめます。そのため、`Span<T>` を通して、配列の外のメモリを読み書きしてしまうことはありません。

---

## ワンポイントアドバイス

### Span\<T\> のメソッド

`Span<T>` には、範囲の要素をまとめて処理するメソッドがあります。たとえば、[Fill メソッド](https://learn.microsoft.com/dotnet/api/system.span-1.fill) はすべての要素に同じ値を入れ、[CopyTo メソッド](https://learn.microsoft.com/dotnet/api/system.span-1.copyto) は要素を別の `Span<T>` にコピーします。

```csharp
Span<int> span = new int[] { 1, 2, 3, 4 };
span.Slice(1, 2).Fill(0);
foreach (int n in span)
{
    Console.Write($"{n} ");
}
Console.WriteLine();
```

```
1 0 0 4 
```

配列の一部を `Slice` で指してから `Fill` を呼ぶと、その部分だけを `0` にできます。

---

## まとめ

- 範囲演算子 `..` で配列の一部を取り出すと、新しい配列が作られる
- `Span<T>` は、連続した要素の一部を指す構造体で、先頭の要素への参照と長さだけを持つ。要素はコピーしない
- `Span<T>` を通して要素を書き換えると、元の配列が書き換わる
- `Slice` や範囲演算子で、`Span<T>` の範囲をさらに狭められる
- パラメータを `ReadOnlySpan<T>` にすると、配列の全体も一部も同じメソッドで受け取れる
- `ReadOnlySpan<T>` は読み取り専用の `Span<T>`。文字列の `AsSpan` は `ReadOnlySpan<char>` を返す
- `Span<T>` のインデクサは要素への参照を返す

---

## 理解度チェック

1. `numbers[1..4]` と `numbers.AsSpan(1, 3)` は、どちらも同じ 3 つの要素を表します。ヒープへの割り当てに違いがあるのはなぜですか？
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   int[] data = { 1, 2, 3, 4, 5, 6 };
   Span<int> span = data.AsSpan(2);
   span[0] = 0;
   Span<int> inner = span[1..3];
   inner[1] = 9;
   Console.WriteLine(string.Join(", ", data));
   Console.WriteLine(inner.Length);
   ```

3. `ReadOnlySpan<int>` を受け取り、いちばん大きい要素を返すメソッド `Max` を書いてください。要素は 1 つ以上あるものとします。

<details markdown="1">
<summary>解答を見る</summary>

1. 配列に範囲演算子を使うと、要素をコピーした新しい配列がヒープに作られます。`Span<int>` は、元の配列の要素への参照と長さだけを持つ構造体なので、ヒープにメモリを確保しないからです。
2. 次のように出力されます。`span` は `data[2]` から最後までを指すので、`span[0] = 0` で `data[2]` が `0` になります。`inner` は `span` のインデックス `1` と `2`、つまり `data[3]` と `data[4]` を指すので、`inner[1] = 9` で `data[4]` が `9` になります。

   ```
   1, 2, 0, 4, 9, 6
   2
   ```

3. ```csharp
   int[] numbers = { 3, 8, 5 };
   Console.WriteLine(Max(numbers));

   int Max(ReadOnlySpan<int> values)
   {
       int max = values[0];
       foreach (int v in values)
       {
           if (v > max)
           {
               max = v;
           }
       }
       return max;
   }
   ```

   ```
   8
   ```

</details>

---

## 次のステップ

[ref struct と Span の制約](/unity-csharp-learning/csharp/ref-struct/) では、`Span<T>` をクラスのフィールドにできない理由など、`Span<T>` に課された制約を学びます。
