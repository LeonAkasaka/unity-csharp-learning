---
layout: page
title: 位置パターンとリストパターン
permalink: /csharp/positional-list-patterns/
---

# 位置パターンとリストパターン

[プロパティパターン](/unity-csharp-learning/csharp/property-patterns/) では、プロパティを名前で指定して調べました。このページでは、値を順番で調べるパターンを学びます。タプルや record の要素を順に調べる **位置パターン**（positional pattern）と、配列の要素を先頭から順に調べる **リストパターン**（list pattern）です。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- 位置パターンで、タプルの要素の組み合わせを調べられる
- 位置パターンで、record の要素を調べられる
- リストパターンで、配列や文字列の要素と要素の数を、インデックスの範囲外を気にせずに調べられる
- スライスパターンで、残りの要素を変数に取り出せる
- `is` と `switch` 式の両方で、位置パターンとリストパターンを使える

## 前提知識

- [プロパティパターン](/unity-csharp-learning/csharp/property-patterns/) を読んでいること
- [タプル](/unity-csharp-learning/csharp/tuples/) を読んでいること
- [record](/unity-csharp-learning/csharp/records/) を読んでいること
- [インデックスと範囲（補足）](/unity-csharp-learning/csharp/ranges/) を読んでいること

---

## 1. 値の組み合わせを調べる手間

じゃんけんの勝ち負けを判定します。自分の手 `me` と相手の手 `you` の組み合わせで、結果が決まります。`if` 文で書くと、次のようになります。

```csharp
Console.WriteLine(Judge(Hand.Rock, Hand.Scissors));
Console.WriteLine(Judge(Hand.Paper, Hand.Scissors));
Console.WriteLine(Judge(Hand.Rock, Hand.Rock));

string Judge(Hand me, Hand you)
{
    if (me == you)
    {
        return "あいこ";
    }
    if ((me == Hand.Rock && you == Hand.Scissors) ||
        (me == Hand.Scissors && you == Hand.Paper) ||
        (me == Hand.Paper && you == Hand.Rock))
    {
        return "勝ち";
    }
    return "負け";
}

enum Hand
{
    Rock,
    Scissors,
    Paper
}
```

```
勝ち
負け
あいこ
```

勝ちになる 3 つの組み合わせを、`me ==` と `you ==` を 6 回書いて表しています。調べたいのは「`(me, you)` の組がどれか」なのに、組を 1 つずつ `&&` と `||` で組み立てる必要があります。

---

## 2. 位置パターン

**書式：[位置パターン](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/patterns#positional-pattern)**
```
(パターン1, パターン2)
型名(パターン1, パターン2)
```

| 要素 | 説明 |
|---|---|
| `( )` | 値を [分解](/unity-csharp-learning/csharp/tuples/) して、1 つ目の要素をパターン1 と、2 つ目の要素をパターン2 と比べる |
| `型名` | record のように、分解できる型を調べるときに書く |

### タプルを調べる

`me` と `you` を [タプル](/unity-csharp-learning/csharp/tuples/) にまとめると、組み合わせを位置パターンで調べられます。

```
(me, you) is (Hand.Rock, Hand.Scissors)
```

これは、「1 つ目の要素が `Hand.Rock` で、2 つ目の要素が `Hand.Scissors`」というパターンです。1 節の `Judge` を、`switch` 式と位置パターンで書き直します。

```csharp
Console.WriteLine(Judge(Hand.Rock, Hand.Scissors));
Console.WriteLine(Judge(Hand.Paper, Hand.Scissors));
Console.WriteLine(Judge(Hand.Rock, Hand.Rock));

string Judge(Hand me, Hand you)
{
    return (me, you) switch
    {
        var (a, b) when a == b => "あいこ",
        (Hand.Rock, Hand.Scissors) or (Hand.Scissors, Hand.Paper) or (Hand.Paper, Hand.Rock) => "勝ち",
        _ => "負け",
    };
}

enum Hand
{
    Rock,
    Scissors,
    Paper
}
```

```
勝ち
負け
あいこ
```

勝ちになる組み合わせが、`(Hand.Rock, Hand.Scissors)` のように、組のまま並んでいます。`var (a, b)` は、2 つの要素をそれぞれ変数 `a` と `b` に取り出す書き方で、[プロパティパターン](/unity-csharp-learning/csharp/property-patterns/) で学んだ `var` パターンを、位置パターンに使ったものです。

### record を調べる

位置指定の構文で宣言した [record](/unity-csharp-learning/csharp/records/) は分解できるので、位置パターンで調べられます。

```csharp
Point[] points = { new Point(0, 0), new Point(3, 0), new Point(0, -2), new Point(1, 1) };

if (points[1] is (_, 0))
{
    Console.WriteLine($"{points[1]} は x 軸の上");
}

foreach (Point p in points)
{
    string where = p switch
    {
        (0, 0) => "原点",
        (_, 0) => "x 軸の上",
        (0, _) => "y 軸の上",
        var (x, y) when x == y => "y = x の上",
        _ => "それ以外",
    };
    Console.WriteLine($"{p}: {where}");
}

record Point(int X, int Y);
```

```
Point { X = 3, Y = 0 } は x 軸の上
Point { X = 0, Y = 0 }: 原点
Point { X = 3, Y = 0 }: x 軸の上
Point { X = 0, Y = -2 }: y 軸の上
Point { X = 1, Y = 1 }: y = x の上
```

`(_, 0)` の `_` は、[switch 式](/unity-csharp-learning/csharp/switch-expressions/) で学んだ破棄パターンです。1 つ目の要素は何でもよく、2 つ目の要素が `0` のときに当てはまります。

### プロパティパターンとの使い分け

次の 2 つは、同じことを調べています。

```
p is Point(> 0, 0)
p is Point { X: > 0, Y: 0 }
```

位置パターンは短く書けますが、どの要素を調べているかは、record の宣言の順番を知らないとわかりません。じゃんけんの `(me, you)` のように、順番そのものに意味がある組には位置パターンが向いています。要素が多いときや、一部の要素だけを調べるときは、名前が見えるプロパティパターンのほうが読みやすくなります。

---

## 3. 配列の要素を調べる手間

配列の先頭の要素が `1` かどうかを調べます。インデックスで書くと、次のようになります。

```csharp
int[][] data =
{
    new[] { 1, 2, 3 },
    new int[0],
};

foreach (int[] a in data)
{
    if (a.Length >= 1 && a[0] == 1)
    {
        Console.WriteLine("1 で始まる");
    }
    else
    {
        Console.WriteLine("1 で始まらない");
    }
}
```

```
1 で始まる
1 で始まらない
```

`a[0]` を読む前に、`a.Length >= 1` を確かめる必要があります。空の配列で `a[0]` を読むと、`IndexOutOfRangeException` が発生するからです。「先頭が `1` で、末尾が `3`」のように条件が増えると、要素の数の確認と、インデックスの計算が増えていきます。

---

## 4. リストパターン

**書式：[リストパターン](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/patterns#list-patterns)**
```
[パターン1, パターン2, パターン3]
[パターン1, ..]
[.., パターン]
```

| 要素 | 説明 |
|---|---|
| `[ ]` | 要素を先頭から順にパターンと比べる。要素の数も一致する必要がある |
| `..` | **スライスパターン**。0 個以上の要素に当てはまる。1 つのリストパターンに 1 回だけ書ける |

リストパターンは C# 11 から使えます。配列や `List<T>` を調べられます。3 節の条件は、次のように書けます。

```csharp
int[][] data =
{
    new[] { 1, 2, 3 },
    new int[0],
};

foreach (int[] a in data)
{
    if (a is [1, ..])
    {
        Console.WriteLine("1 で始まる");
    }
    else
    {
        Console.WriteLine("1 で始まらない");
    }
}
```

```
1 で始まる
1 で始まらない
```

`[1, ..]` は、「先頭が `1` で、その後ろの要素はいくつでもよい」というパターンです。要素が足りなければ当てはまらないだけなので、空の配列でも例外は発生しません。要素の数の確認は、パターンが代わりにしてくれます。

文字列も、文字の並びとして、リストパターンで調べられます。

```csharp
string word = "apple";

Console.WriteLine(word is ['a', ..]);
Console.WriteLine(word is [.., 'e']);
Console.WriteLine(word is [_, _, _]);
```

```
True
True
False
```

`word is ['a', ..]` は「`'a'` で始まる」、`word is [.., 'e']` は「`'e'` で終わる」という意味です。`"apple"` は 5 文字なので、`[_, _, _]` には当てはまりません。

`switch` 式に並べると、配列の形によって結果を選べます。

```csharp
int[][] data =
{
    new int[0],
    new[] { 5 },
    new[] { 1, 2 },
    new[] { 4, 9, 9, 3 },
};

foreach (int[] a in data)
{
    string d = a switch
    {
        [] => "空",
        [var only] => $"1 つだけ: {only}",
        [1, ..] => "1 で始まる",
        [var first, .., var last] => $"最初 {first}、最後 {last}",
        _ => "その他",
    };
    Console.WriteLine(d);
}
```

```
空
1 つだけ: 5
1 で始まる
最初 4、最後 3
```

`[]` は要素が 0 個、`[var only]` は要素がちょうど 1 個のときに当てはまります。`[var first, .., var last]` は、要素が 2 個以上のときに、先頭と末尾を取り出します。インデックスで書くなら `a[0]` と `a[a.Length - 1]` ですが、要素の数を確かめる必要はありません。

### スライスを変数に取り出す

`..` の後ろに `var 変数名` を書くと、`..` に当てはまった部分を変数に取り出せます。

```csharp
int[] nums = { 1, 2, 3, 4 };

if (nums is [var head, .. var rest])
{
    Console.WriteLine(head);
    Console.WriteLine(string.Join(", ", rest));
}

List<string> words = new List<string> { "a", "b" };
Console.WriteLine(words is [_, _]);
```

```
1
2, 3, 4
True
```

配列の場合、`rest` は [インデックスと範囲（補足）](/unity-csharp-learning/csharp/ranges/) の範囲演算子と同じように、新しい配列として作られます。`words is [_, _]` は、要素がちょうど 2 個かを調べています。

---

## よくあるミス

### リストパターンが、先頭の要素だけを調べると思い込む

```csharp
int[] nums = { 1, 2, 3 };
Console.WriteLine(nums is [1, 2]);
Console.WriteLine(nums is [1, 2, ..]);
```

```
False
True
```

`[1, 2]` は、要素がちょうど 2 個で、それが `1` と `2` のときだけ当てはまります。要素が 3 個の `nums` には当てはまりません。「先頭が `1` と `2` なら、後ろは何でもよい」ときは、`[1, 2, ..]` と書きます。

### スライスパターンを 2 回書く

```csharp
// ❌ NG: .. は 1 つのリストパターンに 1 回だけ
// int[] a = { 1 };
// Console.WriteLine(a is [.., ..]);  // CS8980
```

`..` がいくつの要素に当てはまるかは、ほかのパターンの数から決まります。`..` が 2 つあると決められないので、CS8980 のエラーになります。

---

## まとめ

- 位置パターン `(パターン1, パターン2)` で、タプルや record の要素を順番に調べる。値の組み合わせで結果を選ぶときに向いている
- `var (a, b)` で、要素を変数に取り出せる。`_` はどんな要素にも当てはまる
- 順番に意味がない値を調べるときは、名前が見えるプロパティパターンのほうが読みやすい
- リストパターン `[パターン1, パターン2]` で、配列・`List<T>`・文字列の要素と要素の数を調べる。要素が足りなければ当てはまらないだけで、例外は発生しない
- `..`（スライスパターン）は 0 個以上の要素に当てはまり、1 つのリストパターンに 1 回だけ書ける。`.. var 変数名` で、当てはまった部分を取り出せる
- どちらのパターンも、`is` と `switch` 式の両方で使える

---

## 理解度チェック

1. 次のコードを実行すると何が出力されますか？

   ```csharp
   (int, int)[] pairs = { (0, 0), (2, 0), (2, 3) };

   foreach ((int, int) pair in pairs)
   {
       string s = pair switch
       {
           (0, 0) => "両方 0",
           (_, 0) => "2 つ目が 0",
           var (a, b) => $"合計 {a + b}",
       };
       Console.WriteLine(s);
   }
   ```

2. 次のコードを実行すると何が出力されますか？

   ```csharp
   int[] a = { 3, 1, 4, 1, 5 };

   Console.WriteLine(a is [3, ..]);
   Console.WriteLine(a is [.., 5]);
   Console.WriteLine(a is [_, _, _]);
   Console.WriteLine(a is [_, 1, ..]);
   ```

3. 次の条件を、リストパターンを使って書き直してください。

   ```csharp
   if (a.Length >= 2 && a[0] == 0 && a[a.Length - 1] == 0)
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. 次のように出力されます。`(2, 0)` は `(0, 0)` に当てはまらず、`(_, 0)` に当てはまります。`(2, 3)` は最後の `var (a, b)` に当てはまります。

   ```
   両方 0
   2 つ目が 0
   合計 5
   ```

2. 次のように出力されます。`a` は要素が 5 個なので、`[_, _, _]` には当てはまりません。`[_, 1, ..]` は、2 つ目の要素が `1` なので当てはまります。

   ```
   True
   True
   False
   True
   ```

3. `if (a is [0, .., 0])` です。要素が 2 個以上あり、先頭と末尾が `0` のときに当てはまります。要素の数を確かめる `a.Length >= 2` は要りません。

</details>

---

## 次のステップ

これで「C# パターンマッチング」のセクションは終わりです。[デリゲートの基本](/unity-csharp-learning/csharp/delegates/) からは「C# デリゲートとイベント」のセクションに進み、メソッドへの参照を変数として扱うデリゲートの仕組みを学びます。
