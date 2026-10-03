---
layout: page
title: 並べ替え・集計・要素の取り出し
permalink: /csharp/linq-aggregation/
---

# 並べ替え・集計・要素の取り出し

LINQ には、`Where` と `Select` のほかにも、よく使う処理のメソッドが多く用意されています。このページでは、要素を並べ替える `OrderBy`、個数や合計を求める `Count` や `Sum`、条件に合う要素があるかを調べる `Any` と `All`、1 つの要素を取り出す `First` や `Single`、一部の要素を取り出す `Take` や `Skip` を学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `OrderBy`・`OrderByDescending`・`ThenBy` で、要素を並べ替えられる
- `Count`・`Sum`・`Average`・`Max`・`Min` で、要素を集計できる
- `Any`・`All` で、条件に合う要素があるかを調べられる
- `First`・`FirstOrDefault`・`Single` の違いを説明し、使い分けられる
- `Take`・`Skip`・`Distinct` で、要素の一部を取り出せる

## 前提知識

- [Where と Select を自作する（補足）](/unity-csharp-learning/csharp/linq-implementation/) を読んでいること
- [タプル](/unity-csharp-learning/csharp/tuples/) を読んでいること

---

このページのコード例では、次のタプルの配列 `players` を使います。名前・点数・チームの 3 つの要素を持つタプルです。コード例は、どれもこの `players` の宣言の後に続けて書きます。

```csharp
(string Name, int Score, string Team)[] players =
{
    ("Alice", 80, "赤"),
    ("Bob", 65, "青"),
    ("Carol", 92, "赤"),
    ("Dave", 65, "赤"),
    ("Eve", 78, "青"),
};
```

---

## 1. 並べ替える

要素を小さい順（昇順）に並べ替えるには [OrderBy メソッド](https://learn.microsoft.com/dotnet/api/system.linq.enumerable.orderby) を、大きい順（降順）に並べ替えるには [OrderByDescending メソッド](https://learn.microsoft.com/dotnet/api/system.linq.enumerable.orderbydescending) を使います。何を基準に並べるかは、要素から比べる値（**キー**）を取り出すラムダ式で指定します。

**書式：[Enumerable.OrderBy メソッド](https://learn.microsoft.com/dotnet/api/system.linq.enumerable.orderby)**
```csharp
public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector);
```

| パラメータ | 説明 |
|---|---|
| `keySelector` | 要素を受け取り、並べ替えの基準にする値を返すメソッド |

キーが同じ要素をさらに別の基準で並べるには、`OrderBy` や `OrderByDescending` の後に [ThenBy メソッド](https://learn.microsoft.com/dotnet/api/system.linq.enumerable.thenby)（降順なら `ThenByDescending`）を続けます。

```csharp
foreach (var p in players.OrderBy(p => p.Score))
{
    Console.WriteLine($"{p.Name} {p.Score}");
}
Console.WriteLine("---");
foreach (var p in players.OrderByDescending(p => p.Score).ThenBy(p => p.Name))
{
    Console.WriteLine($"{p.Name} {p.Score}");
}
```

```
Bob 65
Dave 65
Eve 78
Alice 80
Carol 92
---
Carol 92
Alice 80
Eve 78
Bob 65
Dave 65
```

`OrderBy` は、キーが同じ要素（`Bob` と `Dave` の 65 点）の順番を、元の並びのまま保ちます。2 つ目の例では、点数の降順に並べた後、同じ点数の中を名前の昇順に並べています。

`OrderBy` は、取り出したキーどうしを、キーの型の `IComparable<T>` で比べます。`int` や `string` のキーはそのまま並べ替えられます。自分で作ったクラスをキーにするときは、[比較の仕組み](/unity-csharp-learning/csharp/comparison/) で学んだように、そのクラスに `IComparable<T>` を実装するか、`IComparer<TKey>` を受け取るオーバーロードで比べ方を渡します。

`OrderBy` も遅延実行です。元の配列は並べ替えられず、並べ替えた順に要素を取り出す `IEnumerable<T>` が返されます。

---

## 2. 集計する

要素の数、合計、平均、最大、最小を求めるメソッドがあります。どれも、集計する値を取り出すラムダ式を渡せます。どれも即時実行で、1 つの値を返します。

| メソッド | 返す値 |
|---|---|
| [Count](https://learn.microsoft.com/dotnet/api/system.linq.enumerable.count) | 要素の数。条件を渡すと、条件に合う要素の数 |
| [Sum](https://learn.microsoft.com/dotnet/api/system.linq.enumerable.sum) | 合計 |
| [Average](https://learn.microsoft.com/dotnet/api/system.linq.enumerable.average) | 平均。`int` の平均でも `double` で返す |
| [Max](https://learn.microsoft.com/dotnet/api/system.linq.enumerable.max) / [Min](https://learn.microsoft.com/dotnet/api/system.linq.enumerable.min) | 最大 / 最小の値 |
| [MaxBy](https://learn.microsoft.com/dotnet/api/system.linq.enumerable.maxby) / [MinBy](https://learn.microsoft.com/dotnet/api/system.linq.enumerable.minby) | 値が最大 / 最小になる要素そのもの |

```csharp
Console.WriteLine($"人数: {players.Length}");
Console.WriteLine($"赤チームの人数: {players.Count(p => p.Team == "赤")}");
Console.WriteLine($"合計: {players.Sum(p => p.Score)}");
Console.WriteLine($"平均: {players.Average(p => p.Score)}");
Console.WriteLine($"最高点: {players.Max(p => p.Score)}");
Console.WriteLine($"最低点: {players.Min(p => p.Score)}");
Console.WriteLine($"最高点の人: {players.MaxBy(p => p.Score).Name}");
```

```
人数: 5
赤チームの人数: 3
合計: 380
平均: 76
最高点: 92
最低点: 65
最高点の人: Carol
```

`Max` が返すのは最高点の `92` という値ですが、`MaxBy` が返すのは、点数が最大の要素（`Carol` のタプル）です。「最高点は何点か」なら `Max`、「最高点は誰か」なら `MaxBy` を使います。

### Any と All

[Any メソッド](https://learn.microsoft.com/dotnet/api/system.linq.enumerable.any) は、条件に合う要素が 1 つでもあれば `true` を返します。[All メソッド](https://learn.microsoft.com/dotnet/api/system.linq.enumerable.all) は、すべての要素が条件に合えば `true` を返します。

```csharp
Console.WriteLine($"90 点以上がいるか: {players.Any(p => p.Score >= 90)}");
Console.WriteLine($"全員 60 点以上か: {players.All(p => p.Score >= 60)}");
Console.WriteLine($"全員 70 点以上か: {players.All(p => p.Score >= 70)}");
```

```
90 点以上がいるか: True
全員 60 点以上か: True
全員 70 点以上か: False
```

「条件に合う要素があるか」を調べるときは、`Count(...) > 0` より `Any(...)` を使います。`Count` はすべての要素を数えますが、`Any` は条件に合う要素を 1 つ見つけた時点で調べるのをやめるからです。

---

## 3. 1 つの要素を取り出す

条件に合う要素を 1 つだけ取り出すメソッドには、次のものがあります。どれも即時実行です。

| メソッド | 返す要素 | 条件に合う要素がないとき | 条件に合う要素が 2 つ以上あるとき |
|---|---|---|---|
| [First](https://learn.microsoft.com/dotnet/api/system.linq.enumerable.first) | 最初の 1 つ | `InvalidOperationException` | 最初の 1 つ |
| [FirstOrDefault](https://learn.microsoft.com/dotnet/api/system.linq.enumerable.firstordefault) | 最初の 1 つ | 型の既定値 | 最初の 1 つ |
| [Last](https://learn.microsoft.com/dotnet/api/system.linq.enumerable.last) | 最後の 1 つ | `InvalidOperationException` | 最後の 1 つ |
| [Single](https://learn.microsoft.com/dotnet/api/system.linq.enumerable.single) | ただ 1 つの要素 | `InvalidOperationException` | `InvalidOperationException` |

```csharp
var first = players.First(p => p.Team == "青");
Console.WriteLine($"最初の青チーム: {first.Name}");

var last = players.Last(p => p.Team == "赤");
Console.WriteLine($"最後の赤チーム: {last.Name}");

var none = players.FirstOrDefault(p => p.Score > 100);
Console.WriteLine($"100 点より上: {none.Name ?? "(いない)"} {none.Score}");

var carol = players.Single(p => p.Name == "Carol");
Console.WriteLine($"Carol: {carol.Score}");
```

```
最初の青チーム: Bob
最後の赤チーム: Dave
100 点より上: (いない) 0
Carol: 92
```

`FirstOrDefault` は、条件に合う要素がないとき、要素の型の既定値を返します。要素の型はタプルなので、既定値は、すべての要素が既定値のタプル（`Name` が `null`、`Score` が `0`）です。`none.Name` が `null` なので、[null 許容値型](/unity-csharp-learning/csharp/nullable-value-types/) で学んだ `??` 演算子で `(いない)` を表示しています。

`Single` は、「条件に合う要素がちょうど 1 つ」であることを確かめながら取り出します。名前のように重複しないはずの値で探すときに使うと、重複していたときに気づけます。

---

## 4. 一部を取り出す

先頭からいくつかの要素を取り出すには [Take メソッド](https://learn.microsoft.com/dotnet/api/system.linq.enumerable.take) を、先頭からいくつかの要素を飛ばすには [Skip メソッド](https://learn.microsoft.com/dotnet/api/system.linq.enumerable.skip) を使います。重複した要素を取り除くには [Distinct メソッド](https://learn.microsoft.com/dotnet/api/system.linq.enumerable.distinct) を使います。どれも遅延実行です。次のコードは、`players` を使わない別のプログラムです。

```csharp
int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
Console.WriteLine(string.Join(", ", numbers.Take(3)));
Console.WriteLine(string.Join(", ", numbers.Skip(7)));
Console.WriteLine(string.Join(", ", numbers.Skip(3).Take(3)));

int[] dice = { 3, 5, 3, 1, 5, 6 };
Console.WriteLine(string.Join(", ", dice.Distinct()));
```

```
1, 2, 3
8, 9, 10
4, 5, 6
3, 5, 1, 6
```

`Skip(3).Take(3)` は、先頭の 3 つを飛ばしてから 3 つを取り出します。一覧を何件かずつに分けて表示するときの、2 ページ目の取り出し方です。`Distinct` は、最初に出てきた順番のまま、2 回目以降に出てきた同じ値を取り除きます。

### 組み合わせる

これまでのメソッドは、メソッドチェーンで自由に組み合わせられます。次のコードは、点数の上位 3 人の名前を取り出します。

```csharp
var top3 = players
    .OrderByDescending(p => p.Score)
    .Take(3)
    .Select(p => p.Name);
Console.WriteLine(string.Join(", ", top3));
```

```
Carol, Alice, Eve
```

---

## よくあるミス

### 要素がないときに First や Max を使う

`First` は、条件に合う要素が 1 つもないと `InvalidOperationException` を投げます。`Max`・`Min`・`Average` も、要素が 1 つもない並びに使うと `InvalidOperationException` を投げます（`Sum` は `0` を返します）。

```csharp
// ❌ NG: 条件に合う要素がないので InvalidOperationException が発生する
var x = players.First(p => p.Score > 100);
```

条件に合う要素がないかもしれないときは、`FirstOrDefault` を使い、既定値が返ってきたときの処理を書きます。集計の前に `Any` で要素があるかを確かめる方法もあります。

---

## まとめ

- `OrderBy` / `OrderByDescending` でキーの昇順 / 降順に並べ替え、`ThenBy` で同じキーの中を並べ替える
- `Count`・`Sum`・`Average`・`Max`・`Min` は集計した値を、`MaxBy`・`MinBy` はその値になる要素を返す
- `Any` は条件に合う要素が 1 つでもあるか、`All` はすべてが条件に合うかを調べる
- `First` は最初の要素、`FirstOrDefault` はないときに既定値、`Single` はちょうど 1 つであることを確かめて取り出す
- `Take` で先頭から取り出し、`Skip` で先頭を飛ばし、`Distinct` で重複を取り除く
- 要素がないときの `First`・`Max`・`Min`・`Average` は `InvalidOperationException` を投げる

---

## 理解度チェック

1. 次のコードを実行すると何が出力されますか？

   ```csharp
   int[] scores = { 70, 85, 60, 85, 95 };
   Console.WriteLine(scores.Distinct().Count());
   Console.WriteLine(scores.OrderByDescending(s => s).Skip(1).First());
   Console.WriteLine(scores.Any(s => s < 60));
   ```

2. `players` から「最低点の人の名前」を取り出すには、どう書けばよいですか？
3. ID で利用者を探すとき、`First` ではなく `Single` を使うと、どのような利点がありますか？

<details markdown="1">
<summary>解答を見る</summary>

1. 次のように出力されます。重複を除くと `70`・`85`・`60`・`95` の 4 つです。降順に並べると `95, 85, 85, 70, 60` なので、1 つ飛ばした最初の要素は `85` です。`60` 未満の点数はないので `False` です。

   ```
   4
   85
   False
   ```

2. `MinBy` で最低点の要素を取り出し、`Name` を読みます。

   ```csharp
   string name = players.MinBy(p => p.Score).Name;
   ```

   最低点の人が 2 人（`Bob` と `Dave`）いるので、`MinBy` は最初に見つかった `Bob` を返します。

3. ID は重複しないはずなので、条件に合う要素がちょうど 1 つであることを `Single` が確かめてくれます。データの誤りで同じ ID が 2 つあったときに、`First` なら気づかずに最初の 1 つを使ってしまいますが、`Single` なら `InvalidOperationException` で知らせてくれます。

</details>

---

## 次のステップ

[グループ化と結合](/unity-csharp-learning/csharp/linq-grouping/) では、要素をキーごとにまとめる `GroupBy` や、2 つの並びをキーで結び付ける `Join` などを学びます。
