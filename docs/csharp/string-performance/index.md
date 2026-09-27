---
layout: page
title: 文字列処理の割り当てを減らす
permalink: /csharp/string-performance/
---

# 文字列処理の割り当てを減らす

文字列を区切って数値を読み取ったり、値を埋め込んだ文字列を作ったりする処理は、途中で多くの文字列や配列を作りがちです。このページでは、このセクションで学んだ `ReadOnlySpan<char>`、`stackalloc`、`Span<char>` を使って、文字列の **解析** と **組み立て** で作られるものを減らす方法を学びます。最後に、このセクションで学んだ方法をまとめます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `Split` と `int.Parse` による解析で、どのようなオブジェクトが作られるかを説明できる
- `ReadOnlySpan<char>` と `IndexOf` で文字列を区切り、`int.Parse` に `ReadOnlySpan<char>` のまま渡せる
- `ReadOnlySpan<char>` の中身を、`SequenceEqual` や `is` で文字列と比べられる
- `TryFormat`、`TryWrite`、`string.Create` で、途中の文字列を作らずに文字列を組み立てられる

## 前提知識

- [文字列の不変性と StringBuilder](/unity-csharp-learning/csharp/string-immutability/) から [ArrayPool\<T\>（補足）](/unity-csharp-learning/csharp/array-pool/) までのページを読んでいること

---

## 1. 文字列の解析で作られるもの

`"10,20,30,40,50"` のように、カンマで区切られた数値の合計を求めます。[String.Split メソッド](https://learn.microsoft.com/dotnet/api/system.string.split) で区切り、[Int32.Parse メソッド](https://learn.microsoft.com/dotnet/api/system.int32.parse) で数値に変換するのが簡単です。

```csharp
int SumWithSplit(string text)
{
    int total = 0;
    foreach (string part in text.Split(','))
    {
        total += int.Parse(part);
    }
    return total;
}
```

このメソッドを 1 回呼び出すと、次のものがヒープに作られます。

- 区切った部分の文字列 `"10"`、`"20"`、`"30"`、`"40"`、`"50"`
- それらを入れる `string` の配列

数値に変換した後は、どれも使いません。[文字列の不変性と StringBuilder](/unity-csharp-learning/csharp/string-immutability/) で見た `Substring` と同じで、文字列の一部を取り出すたびに、コピーした文字列が作られています。

---

## 2. ReadOnlySpan\<char\> で解析する

区切った部分を文字列として取り出す代わりに、[Span\<T\> と ReadOnlySpan\<T\>](/unity-csharp-learning/csharp/span/) で学んだ `ReadOnlySpan<char>` で指せば、コピーは作られません。`int.Parse` には、`ReadOnlySpan<char>` を受け取るものがあるので、指したまま数値に変換できます。

区切り文字の位置は、[IndexOf メソッド](https://learn.microsoft.com/dotnet/api/system.memoryextensions.indexof) で探します。見つからなければ `-1` を返します。

```csharp
string line = "10,20,30,40,50";
Console.WriteLine(SumWithSplit(line));
Console.WriteLine(SumWithSpan(line));

long before = GC.GetAllocatedBytesForCurrentThread();
for (int i = 0; i < 1000; i++)
{
    SumWithSplit(line);
}
long after = GC.GetAllocatedBytesForCurrentThread();
Console.WriteLine($"Split: {after - before} バイト");

before = GC.GetAllocatedBytesForCurrentThread();
for (int i = 0; i < 1000; i++)
{
    SumWithSpan(line);
}
after = GC.GetAllocatedBytesForCurrentThread();
Console.WriteLine($"Span: {after - before} バイト");

int SumWithSplit(string text)
{
    int total = 0;
    foreach (string part in text.Split(','))
    {
        total += int.Parse(part);
    }
    return total;
}

int SumWithSpan(string text)
{
    ReadOnlySpan<char> rest = text;
    int total = 0;
    while (true)
    {
        int comma = rest.IndexOf(',');
        if (comma < 0)
        {
            total += int.Parse(rest);
            return total;
        }
        total += int.Parse(rest[..comma]);
        rest = rest[(comma + 1)..];
    }
}
```

64 ビットの環境で実行した結果の例です。バイト数は実行環境によって変わることがあります。

```
150
150
Split: 224000 バイト
Span: 0 バイト
```

`SumWithSpan` では、`rest` がまだ処理していない部分を指しています。カンマが見つかったら、その手前（`rest[..comma]`）を数値に変換し、`rest` をカンマの次から後ろ（`rest[(comma + 1)..]`）に狭めます。カンマが見つからなければ、残りの全体が最後の数値です。

どの `ReadOnlySpan<char>` も元の文字列 `line` の一部を指しているだけなので、ヒープにメモリを確保していません。その代わり、コードは `Split` を使うより長くなります。繰り返し呼ばれる処理など、割り当てを減らす効果が大きいところで使います。

---

## 3. 文字列と比べる

`ReadOnlySpan<char>` が指している部分の中身を、文字列と比べる方法はいくつかあります。

```csharp
string text = "id=1234;name=Alice";
ReadOnlySpan<char> name = text.AsSpan(13);
Console.WriteLine(name.SequenceEqual("Alice"));
Console.WriteLine(name is "Alice");
Console.WriteLine(name.Equals("alice", StringComparison.OrdinalIgnoreCase));
Console.WriteLine(text.AsSpan().StartsWith("id="));
```

```
True
True
True
True
```

| 書き方 | 意味 |
|---|---|
| [SequenceEqual](https://learn.microsoft.com/dotnet/api/system.memoryextensions.sequenceequal)`("Alice")` | 中身が `"Alice"` と同じか |
| `is "Alice"` | 中身が `"Alice"` と同じか。`is` の後ろに、定数の文字列を書く（[定数パターン](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/patterns#constant-pattern)） |
| [Equals](https://learn.microsoft.com/dotnet/api/system.memoryextensions.equals)`("alice", StringComparison.OrdinalIgnoreCase)` | 英字の大文字と小文字を区別せずに比べる |
| [StartsWith](https://learn.microsoft.com/dotnet/api/system.memoryextensions.startswith)`("id=")` | `"id="` で始まるか |

これらを組み合わせると、`"id=1234;name=Alice;team=Red"` のような文字列から、キーに対応する値を、文字列を作らずに探せます。

```csharp
string text = "id=1234;name=Alice;team=Red";
Console.WriteLine(FindValue(text, "name").ToString());
Console.WriteLine(FindValue(text, "team") is "Red");
Console.WriteLine(FindValue(text, "age").Length);

ReadOnlySpan<char> FindValue(ReadOnlySpan<char> text, ReadOnlySpan<char> key)
{
    while (text.Length > 0)
    {
        int end = text.IndexOf(';');
        ReadOnlySpan<char> pair = end < 0 ? text : text[..end];
        int equal = pair.IndexOf('=');
        if (equal >= 0 && pair[..equal].SequenceEqual(key))
        {
            return pair[(equal + 1)..];
        }
        text = end < 0 ? ReadOnlySpan<char>.Empty : text[(end + 1)..];
    }
    return ReadOnlySpan<char>.Empty;
}
```

```
Alice
True
0
```

`FindValue` は、`;` で区切った 1 組ずつを `pair` で指し、`=` の手前がキーと同じなら、`=` の後ろを返します。返すのは、パラメータで受け取った `text` の一部を指す `ReadOnlySpan<char>` です。[stackalloc](/unity-csharp-learning/csharp/stackalloc/) の領域とは違い、呼び出し元から渡された範囲の一部なので、戻り値として返せます。見つからなければ、長さが 0 の `ReadOnlySpan<char>.Empty` を返します。

---

## 4. 文字列を組み立てる

値を埋め込んだ文字列を作るとき、最後に文字列そのものが必要なら、その文字列の分の割り当ては避けられません。減らせるのは、途中で作られる文字列と、最後に文字列にする必要がない場合の割り当てです。

### 数値を Span\<char\> に書き込む

[Int32.TryFormat メソッド](https://learn.microsoft.com/dotnet/api/system.int32.tryformat) は、数値を文字列に変換した結果を、新しい文字列ではなく、渡された `Span<char>` に書き込みます。書き込んだ文字数は `out` パラメータで返し、領域が足りなければ `false` を返します。

```csharp
int score = 12345;
Span<char> buffer = stackalloc char[16];
if (score.TryFormat(buffer, out int written))
{
    ReadOnlySpan<char> digits = buffer[..written];
    Console.WriteLine(written);
    Console.WriteLine(digits.ToString());
}
```

```
5
12345
```

書き込む先は `stackalloc` で確保しているので、ヒープへの割り当てはありません（確認のために表示している `ToString` を除きます）。

### 文字列補間の結果を Span\<char\> に書き込む

[TryWrite メソッド](https://learn.microsoft.com/dotnet/api/system.memoryextensions.trywrite) を使うと、文字列補間の結果を、`Span<char>` に直接書き込めます。

```csharp
Span<char> buffer = stackalloc char[32];
Console.WriteLine(Format(buffer, 3, -7).ToString());

long before = GC.GetAllocatedBytesForCurrentThread();
int length = 0;
for (int i = 0; i < 1000; i++)
{
    length += Format(buffer, i, -i).Length;
}
long after = GC.GetAllocatedBytesForCurrentThread();
Console.WriteLine($"TryWrite: {after - before} バイト");

before = GC.GetAllocatedBytesForCurrentThread();
for (int i = 0; i < 1000; i++)
{
    length += $"({i}, {-i})".Length;
}
after = GC.GetAllocatedBytesForCurrentThread();
Console.WriteLine($"文字列補間: {after - before} バイト");

ReadOnlySpan<char> Format(Span<char> destination, int x, int y)
{
    destination.TryWrite($"({x}, {y})", out int written);
    return destination[..written];
}
```

64 ビットの環境で実行した結果の例です。バイト数は実行環境によって変わることがあります。

```
(3, -7)
TryWrite: 0 バイト
文字列補間: 47200 バイト
```

`TryWrite` に渡した `$"({x}, {y})"` は、文字列を作らずに、`destination` に直接書き込まれます。ふつうの文字列補間では、呼び出すたびに結果の文字列が作られています。

書き込んだ結果を、別の `ReadOnlySpan<char>` を受け取るメソッドに渡すなど、`string` にしなくてよい場面で使います。領域が足りないとき、`TryWrite` は `false` を返します。

### string.Create で直接書き込む

最後に `string` が必要なときは、[String.Create メソッド](https://learn.microsoft.com/dotnet/api/system.string.create) を使うと、途中の文字列を作らずに済みます。`string.Create` は、指定した長さの文字列を作り、その中身を `Span<char>` としてラムダ式に渡します。ラムダ式の中で書き込んだ内容が、そのまま文字列の中身になります。

**書式：[String.Create メソッド](https://learn.microsoft.com/dotnet/api/system.string.create)**
```
string.Create(文字数, 状態, (span, 状態) =>
{
    // span に文字を書き込む
});
```

| パラメータ | 説明 |
|---|---|
| `文字数` | 作る文字列の長さ |
| `状態` | ラムダ式に渡したい値。ラムダ式の 2 番目のパラメータで受け取る |
| ラムダ式 | 1 番目のパラメータの `Span<char>` に、文字列の中身を書き込む |

`###-------` のような、長さ 10 のゲージの文字列を作る 2 つの方法を比べます。

```csharp
Console.WriteLine(GaugeConcat(3, 10));
Console.WriteLine(GaugeCreate(3, 10));

long before = GC.GetAllocatedBytesForCurrentThread();
for (int i = 0; i < 1000; i++)
{
    GaugeConcat(3, 10);
}
long after = GC.GetAllocatedBytesForCurrentThread();
Console.WriteLine($"連結: {after - before} バイト");

before = GC.GetAllocatedBytesForCurrentThread();
for (int i = 0; i < 1000; i++)
{
    GaugeCreate(3, 10);
}
after = GC.GetAllocatedBytesForCurrentThread();
Console.WriteLine($"string.Create: {after - before} バイト");

string GaugeConcat(int filled, int width)
{
    return new string('#', filled) + new string('-', width - filled);
}

string GaugeCreate(int filled, int width)
{
    return string.Create(width, filled, static (span, filled) =>
    {
        span[..filled].Fill('#');
        span[filled..].Fill('-');
    });
}
```

64 ビットの環境で実行した結果の例です。バイト数は実行環境によって変わることがあります。

```
###-------
###-------
連結: 120000 バイト
string.Create: 48000 バイト
```

`GaugeConcat` は、`"###"` と `"-------"` を作ってから連結するので、1 回につき 3 つの文字列を作ります。`GaugeCreate` が作るのは、結果の文字列 1 つだけです。

ラムダ式に `static` を付けているのは、[変数キャプチャ](/unity-csharp-learning/csharp/variable-capture/) の `static` ラムダと同じ理由です。必要な値は `状態` のパラメータで渡し、変数をキャプチャしないようにすると、ラムダ式のためのオブジェクトが作られません。

---

## 5. 割り当てを減らす方法のまとめ

このセクションで学んだ方法をまとめます。

| 場面 | 方法 | 学んだページ |
|---|---|---|
| ループの中で文字列をつなぐ | `StringBuilder` | [文字列の不変性と StringBuilder](/unity-csharp-learning/csharp/string-immutability/) |
| 配列や文字列の一部を扱う | `Span<T>` / `ReadOnlySpan<T>` | [Span\<T\> と ReadOnlySpan\<T\>](/unity-csharp-learning/csharp/span/) |
| 小さな一時的な領域を使う | `stackalloc` | [stackalloc](/unity-csharp-learning/csharp/stackalloc/) |
| 配列の一部を保存する・`await` をまたぐ | `Memory<T>` | [Memory\<T\>](/unity-csharp-learning/csharp/memory/) |
| 大きな一時的な配列を使う | `ArrayPool<T>` | [ArrayPool\<T\>（補足）](/unity-csharp-learning/csharp/array-pool/) |
| 文字列を区切って解析する | `ReadOnlySpan<char>` と `IndexOf`、`int.Parse` | このページ |
| 文字列を組み立てる | `TryFormat`、`TryWrite`、`string.Create` | このページ |

どの方法も、`Split` や `+` による連結のような簡単な書き方と比べると、コードが長く、間違えやすくなります。まず簡単な書き方で書き、[ボクシングとアンボクシング](/unity-csharp-learning/csharp/boxing/) から使ってきた `GC.GetAllocatedBytesForCurrentThread` などで割り当てを測って、効果の大きいところにだけ使います。

---

## よくあるミス

### ReadOnlySpan\<char\> を == で比べる

`ReadOnlySpan<char>` と文字列を `==` で比べると、中身が同じでも `False` になります。

```csharp
string text = "id=1234;name=Alice";
ReadOnlySpan<char> name = text.AsSpan(13);
Console.WriteLine(name == "Alice");              // ❌ NG: 同じ場所を指しているかを比べる
Console.WriteLine(name.SequenceEqual("Alice"));  // ✅ OK: 中身を比べる
```

```
False
True
```

`string` の `==` は中身を比べますが、`ReadOnlySpan<T>` の `==` は、2 つが同じ場所の同じ長さの範囲を指しているかを比べます。`"Alice"` は `ReadOnlySpan<char>` に変換されてから比べられ、`name` とは別の場所を指しているので `False` になります。中身を比べるときは、`SequenceEqual` か `is` を使います。

### 途中で ToString を呼ぶ

`int.Parse(part.ToString())` のように、`ReadOnlySpan<char>` を `ToString` で文字列にしてから渡すと、`Substring` と同じように文字列が作られ、`Span` を使った意味がなくなります。`ReadOnlySpan<char>` を受け取るメソッドには、そのまま渡します。`ToString` は、最後に本当に `string` が必要なところでだけ呼びます。

---

## ワンポイントアドバイス

### ReadOnlySpan\<char\> の Split

.NET 9 以降では、`ReadOnlySpan<char>` の [Split メソッド](https://learn.microsoft.com/dotnet/api/system.memoryextensions.split) で、区切った部分の範囲を `foreach` で順に受け取れます。受け取るのは、[インデックスと範囲（補足）](/unity-csharp-learning/csharp/ranges/) で紹介した、範囲を表す `Range` 型の値で、`line[range]` のように使うと、その部分を指す `ReadOnlySpan<char>` になります。

```csharp
ReadOnlySpan<char> line = "10,20,30";
int total = 0;
foreach (Range range in line.Split(','))
{
    total += int.Parse(line[range]);
}
Console.WriteLine(total);
```

```
60
```

2 節の `SumWithSpan` のように、自分で `IndexOf` を呼んで範囲を狭めていく必要がなく、`String.Split` と同じような書き方で、文字列を作らずに区切れます。

---

## まとめ

- `Split` と `int.Parse` による解析は、区切った部分の文字列と、それを入れる配列を作る
- `ReadOnlySpan<char>` と `IndexOf` で区切り、`int.Parse` に `ReadOnlySpan<char>` のまま渡すと、文字列を作らずに解析できる
- `ReadOnlySpan<char>` の中身は、`SequenceEqual` や `is "文字列"` で比べる。`==` は同じ場所を指しているかを比べる
- `TryFormat` や `TryWrite` は、結果を `Span<char>` に書き込むので、文字列を作らない
- `string.Create` は、結果の文字列の中身に直接書き込むので、途中の文字列を作らない
- 割り当てを減らす方法は、測って効果の大きいところにだけ使う

---

## 理解度チェック

1. `ReadOnlySpan<char>` の変数 `name` が `"Alice"` という文字を指しているのに、`name == "Alice"` が `False` になるのはなぜですか？
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   ReadOnlySpan<char> text = "left:right";
   int colon = text.IndexOf(':');
   ReadOnlySpan<char> left = text[..colon];
   ReadOnlySpan<char> right = text[(colon + 1)..];
   Console.WriteLine(left.Length + right.Length);
   Console.WriteLine(right is "right");
   Console.WriteLine(left.SequenceEqual("LEFT"));
   ```

3. `"7,42,15"` のようにカンマで区切られた整数の中から、いちばん大きい値を返すメソッド `MaxValue(ReadOnlySpan<char> text)` を、`Split` を使わずに書いてください。

<details markdown="1">
<summary>解答を見る</summary>

1. `ReadOnlySpan<T>` の `==` は、中身ではなく、2 つが同じ場所の同じ長さの範囲を指しているかを比べるからです。`"Alice"` は `name` とは別の場所にある文字列なので、`False` になります。中身を比べるには `SequenceEqual` や `is "Alice"` を使います。
2. 次のように出力されます。`left` は `"left"` の 4 文字、`right` は `"right"` の 5 文字を指します。`SequenceEqual` は大文字と小文字を区別するので、`"LEFT"` とは等しくありません。

   ```
   9
   True
   False
   ```

3. ```csharp
   Console.WriteLine(MaxValue("7,42,15"));

   int MaxValue(ReadOnlySpan<char> text)
   {
       int max = int.MinValue;
       while (true)
       {
           int comma = text.IndexOf(',');
           ReadOnlySpan<char> part = comma < 0 ? text : text[..comma];
           int value = int.Parse(part);
           if (value > max)
           {
               max = value;
           }
           if (comma < 0)
           {
               return max;
           }
           text = text[(comma + 1)..];
       }
   }
   ```

   ```
   42
   ```

</details>

---

## 次のステップ

これで、C# のメモリの効率化のセクションは終わりです。[C# 言語入門](/unity-csharp-learning/csharp/) の目次に戻って、ほかのトピックも確認してみましょう。
