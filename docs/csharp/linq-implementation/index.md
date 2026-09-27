---
layout: page
title: Where と Select を自作する（補足）
permalink: /csharp/linq-implementation/
---

# Where と Select を自作する（補足）

LINQ の `Where` や `Select` は、特別な仕組みで作られているわけではありません。[拡張メソッド](/unity-csharp-learning/csharp/extension-methods/)、[ジェネリックメソッド](/unity-csharp-learning/csharp/generic-methods/)、[デリゲート](/unity-csharp-learning/csharp/delegates/)、[イテレーター](/unity-csharp-learning/csharp/iterators/) という、これまでに学んだ機能を組み合わせれば、自分でも同じものを作れます。このページでは、`Where` と `Select` の簡単な版を自作して、LINQ の仕組みと遅延実行の理由を確かめます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- 拡張メソッドとイテレーターを組み合わせて、`Where` と `Select` と同じ働きのメソッドを作れる
- `yield return` を使ったメソッドが遅延実行になる理由を説明できる
- `yield return` を使わないメソッドが即時実行になる理由を説明できる

## 前提知識

- [遅延実行と即時実行](/unity-csharp-learning/csharp/linq-deferred/) を読んでいること
- [拡張メソッド](/unity-csharp-learning/csharp/extension-methods/) を読んでいること
- [イテレーターと yield return](/unity-csharp-learning/csharp/iterators/) を読んでいること

---

## 1. MyWhere と MySelect を作る

[LINQ の基本](/unity-csharp-learning/csharp/linq-basics/) で見た `Where` と `Select` の書式を思い出します。

```csharp
public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate);
public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector);
```

これと同じ形のメソッドを、`MyWhere` と `MySelect` という名前で、static クラス `MyEnumerable` に作ります。中身は、`foreach` で元の並びを回し、`yield return` で結果を 1 つずつ返すだけです。

```csharp
int[] numbers = { 5, 12, 8, 3, 20, 7 };

List<int> result = numbers
    .MyWhere(n => n >= 8)
    .MySelect(n => n * 10)
    .ToList();

Console.WriteLine(string.Join(", ", result));

static class MyEnumerable
{
    public static IEnumerable<T> MyWhere<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        foreach (T item in source)
        {
            if (predicate(item))
            {
                yield return item;
            }
        }
    }

    public static IEnumerable<TResult> MySelect<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
        foreach (TSource item in source)
        {
            yield return selector(item);
        }
    }
}
```

```
120, 80, 200
```

[LINQ の基本](/unity-csharp-learning/csharp/linq-basics/) の `Where` と `Select` を使ったコードと、同じ結果になりました。それぞれの部品の役割は次のとおりです。

| 部品 | 役割 | 学んだページ |
|---|---|---|
| `static class` と `this` パラメータ | `numbers.MyWhere(...)` のように、`IEnumerable<T>` のメソッドとして呼び出せるようにする | [拡張メソッド](/unity-csharp-learning/csharp/extension-methods/) |
| `<T>`・`<TSource, TResult>` | どの型の要素にも使えるようにする。型は引数から推論される | [ジェネリックメソッド](/unity-csharp-learning/csharp/generic-methods/) |
| `Func<T, bool>`・`Func<TSource, TResult>` | 条件や変換の中身を、呼び出し側からラムダ式で受け取る | [ラムダ式](/unity-csharp-learning/csharp/lambda/) |
| `yield return` | 結果を 1 つずつ返す。取り出し役はコンパイラーが作る | [イテレーターと yield return](/unity-csharp-learning/csharp/iterators/) |

---

## 2. 遅延実行になる理由

[遅延実行と即時実行](/unity-csharp-learning/csharp/linq-deferred/) の 1 節と同じコードを、`MyWhere` で動かします。`MyEnumerable` クラスは、前のコード例の `MyWhere` と同じものを使います。

```csharp
int[] numbers = { 5, 12, 8 };

Console.WriteLine("MyWhere を呼ぶ");
IEnumerable<int> large = numbers.MyWhere(n =>
{
    Console.WriteLine($"  {n} を調べる");
    return n >= 8;
});
Console.WriteLine("foreach を始める");

foreach (int n in large)
{
    Console.WriteLine($"受け取った: {n}");
}

static class MyEnumerable
{
    public static IEnumerable<T> MyWhere<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        foreach (T item in source)
        {
            if (predicate(item))
            {
                yield return item;
            }
        }
    }
}
```

```
MyWhere を呼ぶ
foreach を始める
  5 を調べる
  12 を調べる
受け取った: 12
  8 を調べる
受け取った: 8
```

本物の `Where` と同じ順番で表示されました。`MyWhere` はイテレーターなので、[イテレーターと yield return](/unity-csharp-learning/csharp/iterators/) で学んだとおり、呼び出しても中の処理は実行されず、取り出し役を持つ `IEnumerable<T>` が返されるだけです。`foreach` が `MoveNext` を呼ぶたびに、`MyWhere` の中の `foreach` が 1 つずつ進み、条件に合う要素を見つけると `yield return` で一時停止します。LINQ の遅延実行は、イテレーターの遅延実行そのものなのです。

`MyWhere` と `MySelect` をつなげると、`MySelect` の取り出し役が `MoveNext` を呼ばれたときに、`MyWhere` の取り出し役の `MoveNext` を呼び、`MyWhere` が元の配列から要素を取り出します。要素が 1 つずつ通り抜けていくのは、このように取り出し役が鎖のようにつながっているからです。

---

## 3. 即時実行になるメソッド

`Count` のように 1 つの値を返すメソッドを、`yield return` を使わずに作ってみます。

```csharp
int[] numbers = { 5, 12, 8, 3, 20 };
Console.WriteLine(numbers.MyCount(n => n >= 8));

static class MyEnumerable
{
    public static int MyCount<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        int count = 0;
        foreach (T item in source)
        {
            if (predicate(item))
            {
                count++;
            }
        }
        return count;
    }
}
```

```
3
```

`MyCount` は `yield return` を使わないふつうのメソッドなので、呼び出すとすぐに `foreach` で全要素を調べ、数え終わってから戻ります。これが即時実行です。答えを返すには、すべての要素を調べ終わっていなければならないので、`Count` や `Sum` は遅延実行にはできません。

---

## ワンポイントアドバイス

### 本物の LINQ との違い

本物の `Where` と `Select` は、ここで作ったものより工夫されています。

- 引数を確かめるタイミング：`source` や `predicate` に `null` を渡すと、本物の `Where` は呼んだ時点ですぐに `ArgumentNullException` を投げます。`MyWhere` はイテレーターなので、中の処理は回すまで実行されず、`null` を渡しても呼んだ時点では何も起きません。本物の `Where` は、引数を確かめるふつうのメソッドと、要素を取り出すイテレーターの 2 つに分けて作られています。
- 速さの工夫：元の並びが配列や `List<T>` のときや、`Where` と `Select` が続けて呼ばれたときには、それに合わせた速い取り出し方を使います。

ただし、「拡張メソッドとイテレーターとデリゲートの組み合わせ」という仕組みは同じです。

---

## まとめ

- LINQ の `Where` や `Select` は、拡張メソッド・ジェネリックメソッド・デリゲート・イテレーターを組み合わせて作れる
- `yield return` を使うメソッドは、呼んでも中の処理が実行されないので、遅延実行になる
- つなげたメソッドの取り出し役が鎖のようにつながり、要素が 1 つずつ通り抜ける
- `yield return` を使わずに答えを返すメソッドは、呼んだ時点で全要素を調べるので、即時実行になる

---

## 理解度チェック

1. `MyWhere` を呼び出しただけで回さなかったとき、`MyWhere` の中の `foreach` は実行されますか？理由も説明してください。
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   string[] words = { "a", "bb", "ccc" };
   foreach (string w in words.MyTake(2))
   {
       Console.WriteLine(w);
   }

   static class MyEnumerable
   {
       public static IEnumerable<T> MyTake<T>(this IEnumerable<T> source, int count)
       {
           if (count <= 0)
           {
               yield break;
           }
           int taken = 0;
           foreach (T item in source)
           {
               yield return item;
               taken++;
               if (taken >= count)
               {
                   yield break;
               }
           }
       }
   }
   ```

3. 要素がすべて条件に合うときに `true` を返す `MyAll` を作るとき、`yield return` を使わずに作るのはなぜですか？

<details markdown="1">
<summary>解答を見る</summary>

1. 実行されません。`MyWhere` は `yield return` を使うイテレーターなので、呼び出したときには取り出し役を持つ `IEnumerable<T>` が返されるだけで、中の処理は `MoveNext` が呼ばれるまで実行されないからです。
2. 次のように出力されます。`MyTake(2)` は、先頭から 2 つの要素を返したところで `yield break` します。

   ```
   a
   bb
   ```

3. `MyAll` が返すのは要素の並びではなく、`bool` の 1 つの値だからです。答えを返すには要素を調べ終わっている必要があるので、呼んだ時点で要素を調べるふつうのメソッド（即時実行）として作ります。

</details>

---

## 次のステップ

[並べ替え・集計・要素の取り出し](/unity-csharp-learning/csharp/linq-aggregation/) では、要素を並べ替える `OrderBy`、集計する `Sum` や `Average`、1 つの要素を取り出す `First` などを学びます。
