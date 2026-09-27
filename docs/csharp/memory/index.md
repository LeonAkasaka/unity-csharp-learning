---
layout: page
title: Memory<T>
permalink: /csharp/memory/
---

# Memory\<T\>

`Span<T>` は ref struct なので、クラスのフィールドにしたり、`await` をまたいで使ったりできません。**Memory\<T\>** は、`Span<T>` と同じように配列の一部を指しながら、ふつうの構造体として、どこにでも置ける型です。このページでは、`Memory<T>` の使い方と、`Span<T>` との使い分けを学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `Memory<T>` がクラスのフィールドにでき、`await` をまたいで使える理由を説明できる
- `AsMemory` で配列の一部を指す `Memory<T>` を作り、`Span` プロパティで要素を読み書きできる
- `ReadOnlyMemory<T>` で文字列の一部を指せる
- `Span<T>` と `Memory<T>` を使い分けられる

## 前提知識

- [stackalloc](/unity-csharp-learning/csharp/stackalloc/) までのページを読んでいること
- [IDisposable と using](/unity-csharp-learning/csharp/dispose-using/) で `using` 宣言を学んだこと

---

## 1. Span\<T\> を保存しておきたい

[ref struct と Span の制約](/unity-csharp-learning/csharp/ref-struct/) で学んだように、`Span<T>` は、スタックの上の領域を指すことがあるので、スタックの上にしか置けません。そのため、次のような使い方はできませんでした。

- 配列の一部を、クラスのフィールドに保存しておき、後で使う
- 配列の一部を、async メソッドの中で `await` をまたいで使う

配列だけを指すのであれば、ヒープに置かれても問題は起きないはずです。配列はヒープにあり、ガベージコレクションが、使われている間は残しておいてくれるからです。この場面のために用意されているのが `Memory<T>` です。

---

## 2. Memory\<T\>

[Memory\<T\> 構造体](https://learn.microsoft.com/dotnet/api/system.memory-1) は、配列などの連続した要素の一部を表します。配列から `Memory<T>` を作るには、[AsMemory メソッド](https://learn.microsoft.com/dotnet/api/system.memoryextensions.asmemory) を使います。使い方は `AsSpan` と同じです。

**書式：[AsMemory メソッド](https://learn.microsoft.com/dotnet/api/system.memoryextensions.asmemory)**
```
Memory<T> 変数名 = 配列.AsMemory(開始位置, 長さ);
Memory<T> 変数名 = 配列.AsMemory(開始位置);
Memory<T> 変数名 = 配列;
```

`Memory<T>` には、インデクサがありません。要素を読み書きするときは、[Span プロパティ](https://learn.microsoft.com/dotnet/api/system.memory-1.span) で `Span<T>` を取り出します。

| メンバー | 説明 |
|---|---|
| `Length` | 要素の数 |
| [Slice(開始位置, 長さ)](https://learn.microsoft.com/dotnet/api/system.memory-1.slice) | 範囲をさらに狭めた `Memory<T>` を返す |
| `Span` | 同じ範囲を指す `Span<T>` を返す |

```csharp
int[] numbers = { 10, 20, 30, 40, 50 };
Memory<int> memory = numbers.AsMemory(1, 3);
Console.WriteLine(memory.Length);
Span<int> span = memory.Span;
span[0] = 0;
Console.WriteLine(string.Join(", ", numbers));
Memory<int> tail = memory.Slice(1);
Console.WriteLine(tail.Span[0]);
```

```
3
10, 0, 30, 40, 50
30
```

`memory` は、`numbers` のインデックス `1` から 3 つの要素を表しています。`memory.Span` で取り出した `Span<int>` を通して書き換えると、元の配列が書き換わります。

### Span\<T\> との違い

`Span<T>` は、範囲の先頭の要素を直接指す参照を持っていました。`Memory<T>` は、要素ではなく配列オブジェクトそのものへの、ふつうの参照を持ち、それとは別に開始位置と長さを持っています。

![Span\<int\> は配列の要素 [1] を直接指す参照と Length を持つ。Memory\<int\> は配列オブジェクトへのふつうの参照と、開始位置 1 と Length 3 を持つ様子](span-vs-memory.svg)

配列オブジェクトへのふつうの参照は、クラスのフィールドにも配列の要素にも入れられます。そのため、`Memory<T>` は ref struct ではない、ふつうの構造体として定義されています。[ref struct と Span の制約](/unity-csharp-learning/csharp/ref-struct/) の表にある使い方は、`Memory<T>` ではどれもコンパイルエラーになりません。

その代わり、`Memory<T>` はスタックの上の領域を指せません。`stackalloc` の結果を `Memory<T>` で受け取ろうとすると、コンパイルエラー（CS8346）になります。

---

## 3. フィールドに保存する

次の `Window` は、配列の一部を `Memory<int>` のフィールドに保存しておくクラスです。[ref struct と Span の制約](/unity-csharp-learning/csharp/ref-struct/) では、`Span<int>` のフィールドを持たせるために ref struct にしましたが、`Memory<int>` ならクラスにできます。

```csharp
int[] scores = { 72, 95, 60, 88, 79 };
Window window = new Window(scores.AsMemory(1, 3));
Console.WriteLine(window.Sum());
scores[2] = 100;
Console.WriteLine(window.Sum());

class Window
{
    private readonly Memory<int> values;

    public Window(Memory<int> values)
    {
        this.values = values;
    }

    public int Sum()
    {
        int total = 0;
        foreach (int v in values.Span)
        {
            total += v;
        }
        return total;
    }
}
```

```
243
283
```

`values` は、`scores` の要素をコピーせずに指しています。そのため、`scores[2]` を書き換えると、次に `Sum` を呼んだときの結果が変わります。

`Sum` の中では、`values.Span` で `Span<int>` を取り出して、`foreach` で処理しています。保存しておくのは `Memory<T>`、要素を処理するのは `Span<T>` という役割分担です。

---

## 4. await をまたいで使う

`Memory<T>` は、async メソッドのパラメータやローカル変数にして、`await` をまたいで使えます。要素を読み書きするときは、そのたびに `Span` で `Span<T>` を取り出します。

```csharp
int[] data = { 1, 2, 3, 4, 5, 6 };
await DoubleAllAsync(data.AsMemory(2, 3));
Console.WriteLine(string.Join(", ", data));

async Task DoubleAllAsync(Memory<int> values)
{
    for (int i = 0; i < values.Length; i++)
    {
        await Task.Delay(10);
        values.Span[i] *= 2;
    }
}
```

```
1, 2, 6, 8, 10, 6
```

`values` は `data` のインデックス `2` から 3 つの要素を表しています。`await` で中断しても、`values` はステートマシンのフィールドとしてヒープに保存できます。`values.Span[i]` で取り出した `Span<int>` は、その行の中でだけ使っているので、`await` をまたぎません。

### .NET のライブラリでの例

.NET のライブラリにも、`Memory<T>` を受け取る非同期のメソッドがあります。たとえば、[Stream.ReadAsync メソッド](https://learn.microsoft.com/dotnet/api/system.io.stream.readasync) には、読み込んだデータを書き込む先として、`Memory<byte>` を受け取るものがあります。次のコードでは、メモリの上のデータを読み書きする [MemoryStream クラス](https://learn.microsoft.com/dotnet/api/system.io.memorystream) から、配列の前半と後半に分けて読み込んでいます。

```csharp
byte[] source = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
using MemoryStream stream = new MemoryStream(source);

byte[] buffer = new byte[8];
int read = await stream.ReadAsync(buffer.AsMemory(0, 4));
Console.WriteLine($"{read} バイト: {string.Join(", ", buffer)}");
read = await stream.ReadAsync(buffer.AsMemory(4));
Console.WriteLine($"{read} バイト: {string.Join(", ", buffer)}");
```

```
4 バイト: 1, 2, 3, 4, 0, 0, 0, 0
4 バイト: 1, 2, 3, 4, 5, 6, 7, 8
```

`ReadAsync` は、渡された `Memory<byte>` の長さまでデータを読み込み、読み込んだバイト数を返します。1 回目は `buffer` の前半の 4 バイトに、2 回目は後半の 4 バイトに書き込まれています。`ReadAsync` は読み込みを待つ間に中断することがあるので、`Span<byte>` ではなく `Memory<byte>` を受け取ります。

---

## 5. ReadOnlyMemory\<T\>

`Span<T>` に対する `ReadOnlySpan<T>` と同じように、読み取り専用の [ReadOnlyMemory\<T\> 構造体](https://learn.microsoft.com/dotnet/api/system.readonlymemory-1) があります。`Span` プロパティは `ReadOnlySpan<T>` を返します。文字列の `AsMemory` は、`ReadOnlyMemory<char>` を返します。

```csharp
string text = "id=1234;name=Alice";
ReadOnlyMemory<char> name = text.AsMemory(13);
await Task.Delay(10);
Console.WriteLine(name.Length);
Console.WriteLine(name.Span[0]);
Console.WriteLine(name.ToString());
```

```
5
A
Alice
```

`name` は、`await` をまたいでも使えます。`ReadOnlySpan<char>` と同じように、`ToString` で指している部分から新しい文字列を作れます。

---

## 6. Span\<T\> と Memory\<T\> の使い分け

| | `Span<T>` | `Memory<T>` |
|---|---|---|
| 種類 | ref struct | ふつうの構造体 |
| 指せるもの | 配列、文字列、`stackalloc` の領域など | 配列、文字列など（スタックの上の領域は指せない） |
| クラスのフィールド | できない | できる |
| `await` をまたぐ | できない | できる |
| 要素の読み書き | インデクサで直接 | `Span` で `Span<T>` を取り出してから |

ふつうは `Span<T>` を使います。`stackalloc` の領域も扱え、要素を直接読み書きできるからです。`Memory<T>` を使うのは、次のように `Span<T>` では書けないときです。

- 配列の一部を、クラスのフィールドなどに保存しておく
- async メソッドで、`await` をまたいで配列の一部を使う

async ではないメソッドのパラメータは、`Span<T>` か `ReadOnlySpan<T>` にします。呼び出す側が `Memory<T>` を持っているときは、`Span` で取り出して渡せます。反対に、パラメータを `Memory<T>` にすると、`stackalloc` の領域を渡せなくなります。

---

## よくあるミス

### 取り出した Span\<T\> を、await の後で使う

`Memory<T>` の `Span` で取り出したのは、ref struct の `Span<T>` です。取り出した `Span<T>` を変数に入れて、`await` をまたいで使うことはできません。

```csharp
async Task Run(Memory<int> values)
{
    Span<int> span = values.Span;
    await Task.Delay(10);
    span[0] = 1;  // ❌ CS4007: await をまたいで Span<int> を使えない
}
```

`await` の後では、`Memory<T>` から `Span` を取り出し直します。

```csharp
async Task Run(Memory<int> values)
{
    await Task.Delay(10);
    values.Span[0] = 1;  // ✅ OK: await の後で取り出す
}
```

---

## ワンポイントアドバイス

### ループの前に Span を取り出す

`Memory<T>` の `Span` プロパティは、`Memory<T>` が何を指しているかを調べてから `Span<T>` を作るので、わずかに処理がかかります。`await` を含まないループで要素を処理するときは、ループの中で毎回 `Span` を呼ぶのではなく、ループの前に 1 回だけ取り出した `Span<T>` を使います。3 節の `Sum` で、`foreach (int v in values.Span)` と書いたのは、この形です。

---

## まとめ

- `Memory<T>` は、配列などの一部を表すふつうの構造体で、クラスのフィールドにでき、`await` をまたいで使える
- `Memory<T>` は、配列オブジェクトへの参照と、開始位置と長さを持つ。スタックの上の領域は指せない
- 要素を読み書きするときは、`Span` プロパティで `Span<T>` を取り出す
- 読み取り専用の `ReadOnlyMemory<T>` がある。文字列の `AsMemory` は `ReadOnlyMemory<char>` を返す
- ふつうは `Span<T>` を使い、保存しておくときや `await` をまたぐときに `Memory<T>` を使う
- `Span` で取り出した `Span<T>` は、`await` をまたいで使えない

---

## 理解度チェック

1. `Span<T>` はクラスのフィールドにできないのに、`Memory<T>` はできるのはなぜですか？
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   int[] data = { 5, 6, 7, 8 };
   Memory<int> m = data.AsMemory(1);
   Memory<int> n = m.Slice(1, 2);
   n.Span[0] = 0;
   Console.WriteLine(string.Join(", ", data));
   Console.WriteLine($"{m.Length}, {n.Length}");
   ```

3. 次の場面では、`Span<T>` と `Memory<T>` のどちらを使いますか？
   1. 配列の一部の合計を求める、async ではないメソッドのパラメータ
   2. 受け取ったデータの一部を、後で使うためにクラスのフィールドに保存する
   3. `stackalloc` で確保した一時的な領域を受け取る変数

<details markdown="1">
<summary>解答を見る</summary>

1. `Span<T>` はスタックの上の領域を指すことがあるので、スタックの上にしか置けない ref struct になっています。`Memory<T>` は配列オブジェクトへのふつうの参照と開始位置を持ち、スタックの上の領域は指さないので、ヒープに置かれても問題が起きないからです。
2. 次のように出力されます。`m` は `data[1]` から最後まで（3 つ）を、`n` は `m` のインデックス `1` から 2 つ、つまり `data[2]` と `data[3]` を表します。`n.Span[0] = 0` で `data[2]` が `0` になります。

   ```
   5, 6, 0, 8
   3, 2
   ```

3. 1 は `Span<T>`（読むだけなので `ReadOnlySpan<T>`）、2 は `Memory<T>`、3 は `Span<T>` です。1 は async ではないので `Span<T>` で足ります。2 はクラスのフィールドなので `Span<T>` は使えません。3 の `stackalloc` の領域は `Memory<T>` では指せません。

</details>

---

## 次のステップ

[ArrayPool\<T\>（補足）](/unity-csharp-learning/csharp/array-pool/) では、大きな一時的な配列を、作り直さずに使い回す方法を学びます。
