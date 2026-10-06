---
layout: page
title: ref struct と Span の制約
permalink: /csharp/ref-struct/
---

# ref struct と Span の制約

`Span<T>` は構造体ですが、ふつうの構造体とは違い、クラスのフィールドにしたり、`object` に代入したりできません。`Span<T>` が、スタックの上にしか置けない **ref struct** として定義されているからです。このページでは、`Span<T>` に課された制約と、その理由を学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `Span<T>` がスタックの上にしか置けない理由を説明できる
- `ref struct` を定義し、`Span<T>` をフィールドに持つ型を作れる
- ref struct をフィールド、ボクシング、配列、型引数、ラムダ式、`await` をまたぐ変数に使えない理由を説明できる
- async メソッドの中で、制約に触れずに `Span<T>` を使える

## 前提知識

- [Span\<T\> と ReadOnlySpan\<T\>](/unity-csharp-learning/csharp/span/) を読んでいること
- [ボクシングとアンボクシング](/unity-csharp-learning/csharp/boxing/) と [変数キャプチャ](/unity-csharp-learning/csharp/variable-capture/) を読んでいること
- [async と await](/unity-csharp-learning/csharp/async-await/) と [イテレーターと yield return](/unity-csharp-learning/csharp/iterators/) を読んでいること

---

## 1. Span\<T\> はクラスのフィールドにできない

配列の一部を指す `Span<T>` を、クラスのフィールドに持たせて、後で使おうとするとコンパイルエラーになります。

```csharp
class Holder
{
    private Span<int> values;  // ❌ CS8345: Span<int> 型のフィールドは作れない
}
```

[Span\<T\> と ReadOnlySpan\<T\>](/unity-csharp-learning/csharp/span/) の範囲では、`Span<T>` が指していたのは配列の要素でした。配列はヒープにあるので、`Span<T>` がどこに置かれていても問題はなさそうです。しかし、`Span<T>` が指せるのは、配列の要素だけではありません。次のページで学ぶ `stackalloc` を使うと、スタックの上に確保した領域を `Span<T>` で指せます。

スタックの上の領域は、それを確保したメソッドから戻ると存在しなくなります。もし `Span<T>` をクラスのフィールドに入れられたとすると、ヒープ上のオブジェクトが、もう存在しない領域を指す `Span<T>` を持ち続けることになります。[ref ローカルと ref 戻り値](/unity-csharp-learning/csharp/ref-locals/) で、ローカル変数への参照を返せなかったのと同じ問題です。

そこで、`Span<T>` そのものも、スタックの上にしか置けない型として定義されています。スタックの上の `Span<T>` は、指している領域よりも先に消えるので、存在しない領域を指すことはありません。

---

## 2. ref struct

`struct` の前に `ref` を付けて定義した構造体を **ref struct** といいます。ref struct の値は、スタックの上にしか置けません。`Span<T>` と `ReadOnlySpan<T>` は ref struct です。

**書式：[ref struct](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/ref-struct)**
```
ref struct 構造体名
{
    // メンバー
}
```

`Span<T>` 型のフィールドを持てるのは、ref struct だけです。ref struct 自身もスタックの上にしか置けないので、中の `Span<T>` もスタックの上にとどまるからです。

次の `Window` は、配列の一部を `Span<int>` のフィールドに持つ ref struct です。

```csharp
int[] numbers = { 3, 1, 4, 1, 5 };
Window window = new Window(numbers, 1, 3);
Console.WriteLine(window.Sum());

ref struct Window
{
    private Span<int> values;

    public Window(int[] array, int start, int length)
    {
        values = array.AsSpan(start, length);
    }

    public int Sum()
    {
        int total = 0;
        foreach (int v in values)
        {
            total += v;
        }
        return total;
    }
}
```

```
6
```

`Window` を `ref` の付かない、ふつうの `struct` にすると、クラスと同じように CS8345 のコンパイルエラーになります。ふつうの構造体は、クラスのフィールドや配列の要素になってヒープに置かれることがあるからです。

---

## 3. ref struct の制約

ref struct の値がヒープに置かれる可能性のある使い方は、すべてコンパイルエラーになります。

| 使い方 | エラー | ヒープに置かれる理由 |
|---|---|---|
| クラスや、ふつうの構造体のフィールドにする | CS8345 | クラスのオブジェクトはヒープにある |
| `object` やインターフェイスの型に代入する | CS0029 | ボクシングで、値がヒープにコピーされる |
| 配列の要素にする | CS0611 | 配列はヒープにある |
| `List<T>` などの型引数にする | CS9244 | 型パラメータの値は、フィールドや配列に入れられることがある |
| ラムダ式やローカル関数でキャプチャする | CS8175 | キャプチャした変数は、ヒープ上のオブジェクトに移される |
| `await` や `yield return` をまたいで使う | CS4007 | 中断している間、ローカル変数はヒープ上のオブジェクトに保存される |

最後の 2 つは、見た目ではヒープに置かれるとわかりにくいものです。

[変数キャプチャ](/unity-csharp-learning/csharp/variable-capture/) で学んだように、ラムダ式でキャプチャした変数は、コンパイラーが作るクラスのフィールドに移されます。そのため、`Span<T>` の変数はキャプチャできません。

```csharp
Span<int> span = new int[3];
Action a = () => span[0] = 1;  // ❌ CS8175: ラムダ式の中で Span<int> の変数は使えない
```

async メソッドとイテレーターでは、コンパイラーがメソッドをステートマシンに置き換えます。`await` や `yield return` で中断するとき、その後で使うローカル変数は、ステートマシンのフィールドとしてヒープに保存されます。そのため、`await` の後で、その前に作った `Span<T>` を使うことはできません。

```csharp
async Task Run()
{
    Span<int> span = new int[3];
    span[0] = 1;
    await Task.Delay(10);
    Console.WriteLine(span[0]);  // ❌ CS4007: await をまたいで Span<int> を使えない
}
```

---

## 4. async メソッドで Span を使う

`await` をまたがなければ、async メソッドの中でも `Span<T>` を使えます。中断するときに保存する必要がないからです。`await` の後で同じ要素を使いたいときは、`Span<T>` ではなく、指す先の配列を変数に持っておきます。

```csharp
await Run();

async Task Run()
{
    int[] data = new int[3];
    Span<int> span = data;
    span[0] = 1;
    await Task.Delay(10);
    Console.WriteLine(data[0]);
}
```

```
1
```

`span` を使うのは `await` の前だけなので、エラーにはなりません。`await` の後では、配列 `data` から読んでいます。配列はふつうの参照型なので、`await` をまたいで使えます。

`Span<T>` を使う処理を、async ではないメソッドに分ける方法もあります。そのメソッドの中では、`await` を気にせずに `Span<T>` を使えます。

```csharp
await Run();

async Task Run()
{
    int[] data = { 1, 2, 3 };
    await Task.Delay(10);
    Console.WriteLine(Sum(data));
}

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
6
```

配列の一部を指したまま `await` をまたぎたいときは、[Memory\<T\>](/unity-csharp-learning/csharp/memory/) を使います。

---

## よくあるミス

### Span\<T\> に LINQ を使う

`Span<T>` は、[IEnumerable\<T\>](/unity-csharp-learning/csharp/ienumerable/) を実装していません。ref struct は、インターフェイスの型に変換するとボクシングされてしまうので、インターフェイスの型として扱えないからです。そのため、`IEnumerable<T>` の拡張メソッドである [LINQ](/unity-csharp-learning/csharp/linq-basics/) のメソッドは、`Span<T>` には使えません。

```csharp
Span<int> span = new int[] { 1, 2, 3 };
var big = span.Where(x => x > 1);  // ❌ コンパイルエラー
```

このときのエラーは CS0411 のように、原因がわかりにくいものになることがあります。`Span<T>` の要素は `foreach` や `for` で処理します。`foreach` が使えるのは、`Span<T>` が `GetEnumerator` メソッドを持っているからで、インターフェイスを通していません。

---

## ワンポイントアドバイス

### C# のバージョンによる違い

ref struct に関する規則は、C# のバージョンが上がるにつれて緩められてきました。

| C# のバージョン | 変更点 |
|---|---|
| C# 11 | ref struct のフィールドに、`ref` を付けた **ref フィールド** を宣言できるようになった |
| C# 13 | async メソッドとイテレーターの中で、`await` や `yield return` をまたがなければ ref struct の変数を使えるようになった。型パラメータに `allows ref struct` を付けると、ref struct を型引数に渡せるようになった |

C# 12 以前では、4 節の最初のコードのように、async メソッドの中で `Span<T>` の変数を宣言するだけでエラー（CS9202 など）になります。その場合は、`Span<T>` を使う処理を async ではないメソッドに分けます。

`Span<T>` の内部には、先頭の要素を指す ref フィールドと、要素の数を表すフィールドがあります。ref フィールドは、[ref ローカルと ref 戻り値](/unity-csharp-learning/csharp/ref-locals/) の ref ローカルと同じように変数を指すフィールドで、ref struct の中にしか宣言できません。ref フィールドの書き方は、[ref フィールドと scoped](/unity-csharp-learning/csharp/ref-fields-scoped/) で学びます。

---

## まとめ

- `Span<T>` はスタックの上の領域を指すことがあるので、`Span<T>` 自身もスタックの上にしか置けない
- `ref struct` で定義した構造体は、スタックの上にしか置けない。`Span<T>` と `ReadOnlySpan<T>` は ref struct
- `Span<T>` 型のフィールドを持てるのは ref struct だけ
- ref struct は、クラスのフィールド、ボクシング、配列の要素、型引数、ラムダ式のキャプチャ、`await` や `yield return` をまたぐ変数に使えない
- async メソッドでは、`await` をまたがない範囲で `Span<T>` を使うか、`Span<T>` を使う処理を async ではないメソッドに分ける
- `Span<T>` は `IEnumerable<T>` を実装していないので、LINQ は使えない

---

## 理解度チェック

1. `Span<T>` 型のフィールドを、クラスに持たせられないのはなぜですか？
2. 次の 4 つのうち、コンパイルエラーになるものをすべて選んでください。`span` は `Span<int>` 型の変数とします。

   1. `ref struct` の中に `Span<int>` 型のフィールドを宣言する
   2. `object o = span;`
   3. `Span<int>[] spans = new Span<int>[2];`
   4. async ではないメソッドのパラメータを `Span<int>` 型にする

3. 次の async メソッドはコンパイルエラーになります。配列 `data` を使って、エラーにならないように書き換えてください。

   ```csharp
   async Task PrintFirstAsync(int[] data)
   {
       Span<int> span = data;
       await Task.Delay(10);
       Console.WriteLine(span[0]);
   }
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. `Span<T>` はスタックの上の領域を指すことがあり、その領域はメソッドから戻ると存在しなくなるからです。クラスのオブジェクトはヒープにあって長く残るので、フィールドに入れられると、存在しない領域を指す `Span<T>` が残ってしまいます。
2. 2 と 3 です。2 はボクシングでヒープにコピーされ、3 は配列の要素としてヒープに置かれるからです。1 の ref struct と、4 のパラメータは、スタックの上にとどまるのでエラーになりません。
3. `await` の後では、`Span<int>` ではなく配列 `data` を使います。

   ```csharp
   await PrintFirstAsync(new int[] { 7, 8, 9 });

   async Task PrintFirstAsync(int[] data)
   {
       await Task.Delay(10);
       Console.WriteLine(data[0]);
   }
   ```

   ```
   7
   ```

</details>

---

## 次のステップ

[stackalloc](/unity-csharp-learning/csharp/stackalloc/) では、ヒープではなくスタックの上に配列のような領域を確保し、`Span<T>` で扱う方法を学びます。
