---
layout: page
title: タプル
permalink: /csharp/tuples/
---

# タプル

**タプル**（tuple）は、複数の値を 1 つにまとめて扱うための型です。`(int, string)` のように、まとめる値の型を丸かっこの中に並べて書きます。メソッドから複数の値を返したいときに、専用のクラスや構造体を作らずに済みます。このページでは、タプルの書き方と分解、そしてタプルの正体である **ValueTuple** 構造体を学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- タプルを使って、メソッドから複数の値を返せる
- タプルの要素に名前を付け、名前で読み書きできる
- 分解を使って、タプルの要素を別々の変数に受け取れる
- タプルの正体が `ValueTuple` 構造体であり、要素の名前はコンパイル時だけのものであることを説明できる

## 前提知識

- [null 許容値型](/unity-csharp-learning/csharp/nullable-value-types/) を読んでいること
- [構造体](/unity-csharp-learning/csharp/structs/) を読んでいること
- [ref / out / in パラメータ](/unity-csharp-learning/csharp/ref-out-in/) を読んでいること

---

## 1. メソッドから複数の値を返す

配列の最小値と最大値を、1 つのメソッドで求めたいとします。メソッドの戻り値は 1 つだけなので、[ref / out / in パラメータ](/unity-csharp-learning/csharp/ref-out-in/) で学んだ `out` パラメータを使うと、次のようになります。

```csharp
int[] scores = { 72, 85, 64, 90 };

MinMax(scores, out int min, out int max);
Console.WriteLine($"最小 {min}、最大 {max}");

void MinMax(int[] values, out int min, out int max)
{
    min = values[0];
    max = values[0];
    foreach (int v in values)
    {
        if (v < min)
        {
            min = v;
        }
        if (v > max)
        {
            max = v;
        }
    }
}
```

```
最小 64、最大 90
```

正しく動きますが、結果がパラメータから出てくるので、呼び出し側を見ただけでは、どれが入力でどれが結果かがわかりにくくなります。最小値と最大値を持つ構造体を作って返す方法もありますが、このメソッドのためだけに型を 1 つ作るのは大げさです。

タプルを使うと、2 つの値をまとめて戻り値として返せます。

```csharp
int[] scores = { 72, 85, 64, 90 };

(int Min, int Max) result = MinMax(scores);
Console.WriteLine($"最小 {result.Min}、最大 {result.Max}");

(int Min, int Max) MinMax(int[] values)
{
    int min = values[0];
    int max = values[0];
    foreach (int v in values)
    {
        if (v < min)
        {
            min = v;
        }
        if (v > max)
        {
            max = v;
        }
    }
    return (min, max);
}
```

```
最小 64、最大 90
```

戻り値の型 `(int Min, int Max)` は、「`Min` と `Max` という名前の 2 つの `int` をまとめたもの」を表します。`return (min, max);` で、2 つの値をまとめたタプルを作って返しています。

---

## 2. タプルの書き方

**書式：[タプル型](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/value-tuples)**
```
(型1, 型2, ...)              // 要素に名前を付けない
(型1 名前1, 型2 名前2, ...)  // 要素に名前を付ける
```

タプルの値は、`(値1, 値2)` のように丸かっこの中に値を並べて作ります。要素に名前を付けなかったときは、`Item1`・`Item2` という名前で読み書きします。

```csharp
(string, int) a = ("Alice", 80);
Console.WriteLine($"{a.Item1}: {a.Item2}");

(string Name, int Score) b = ("Bob", 65);
Console.WriteLine($"{b.Name}: {b.Score}");

var c = (Name: "Carol", Score: 92);
Console.WriteLine($"{c.Name}: {c.Score}");

string name = "Dave";
int score = 70;
var d = (name, score);
Console.WriteLine($"{d.name}: {d.score}");
```

```
Alice: 80
Bob: 65
Carol: 92
Dave: 70
```

- `a` は名前のないタプルなので、`Item1` と `Item2` で要素を読む
- `b` は、変数の型のほうで要素に名前を付けている
- `c` は、値のほうに `Name:` のように名前を付け、変数の型は `var` で推論させている
- `d` のように変数を並べて作ると、変数の名前がそのまま要素の名前になる

---

## 3. 分解

タプルの要素を、別々の変数に取り出すこともできます。これを **分解**（deconstruction）といいます。

**書式：[分解](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/value-tuples#tuple-assignment-and-deconstruction)**
```
(型1 変数1, 型2 変数2) = タプル;
var (変数1, 変数2) = タプル;
```

```csharp
int[] scores = { 72, 85, 64, 90 };

(int min, int max) = MinMax(scores);
Console.WriteLine($"最小 {min}、最大 {max}");

var (lo, hi) = MinMax(scores);
Console.WriteLine($"最小 {lo}、最大 {hi}");

(_, int onlyMax) = MinMax(scores);
Console.WriteLine($"最大 {onlyMax}");

// MinMax メソッドは 1 節のコード例と同じ
(int Min, int Max) MinMax(int[] values)
{
    int min = values[0];
    int max = values[0];
    foreach (int v in values)
    {
        if (v < min)
        {
            min = v;
        }
        if (v > max)
        {
            max = v;
        }
    }
    return (min, max);
}
```

```
最小 64、最大 90
最小 64、最大 90
最大 90
```

分解で受け取る変数の名前は、タプルの要素の名前と同じでなくてもかまいません。要らない要素は、変数名の代わりに `_`（**破棄**）と書くと、受け取らずに捨てられます。

### 値の入れ替え

すでにある変数に分解することもできます。これを使うと、2 つの変数の値を入れ替える処理を 1 行で書けます。

```csharp
int x = 1;
int y = 2;
(x, y) = (y, x);
Console.WriteLine($"x = {x}, y = {y}");
```

```
x = 2, y = 1
```

右辺の `(y, x)` で、今の `y` と `x` の値を持つタプルが先に作られ、それが `x` と `y` に分解されます。一時的な変数を用意しなくても、値が入れ替わります。

---

## 4. タプルの正体は ValueTuple

[null 許容値型](/unity-csharp-learning/csharp/nullable-value-types/) で、`int?` が `Nullable<int>` 構造体の短い書き方であることを学びました。タプルも同じで、`(string, int)` は [ValueTuple\<T1, T2\> 構造体](https://learn.microsoft.com/dotnet/api/system.valuetuple-2) を使った `ValueTuple<string, int>` の短い書き方です。`ValueTuple` は、要素の数ごとに、型パラメータの数が違うジェネリックな構造体として用意されています。

```csharp
(string Name, int Score) t = ("Alice", 80);

Console.WriteLine(t.GetType());
Console.WriteLine(t.Item1);

ValueTuple<string, int> v = t;
Console.WriteLine(v.Item2);

(string First, int Second) u = t;
Console.WriteLine(u.First);
```

```
System.ValueTuple`2[System.String,System.Int32]
Alice
80
Alice
```

`GetType()` で調べると、`t` の実際の型は `ValueTuple<string, int>` です（`` `2 `` は型パラメータが 2 つであることを表します）。`ValueTuple<string, int>` の要素は、`Item1` と `Item2` という名前のフィールドです。

では、`Name` や `Score` という名前はどこにあるのでしょうか。要素の名前は、コンパイラーがコンパイルするときだけ使うもので、実行されるプログラムには残りません。コンパイラーは、`t.Name` を `t.Item1` に、`t.Score` を `t.Item2` に置き換えます。そのため、名前を付けたタプルでも `t.Item1` と書けますし、要素の型が同じなら、名前の違うタプル型の変数（`u`）にも、名前のない `ValueTuple<string, int>` の変数（`v`）にも代入できます。

### 構造体なので、コピーされる

`ValueTuple` は構造体なので、タプルは値型です。[構造体](/unity-csharp-learning/csharp/structs/) で学んだように、代入すると値そのものがコピーされます。

```csharp
(string Name, int Score) a = ("Alice", 80);
(string Name, int Score) b = a;
b.Score = 100;

Console.WriteLine($"a: {a.Name} {a.Score}");
Console.WriteLine($"b: {b.Name} {b.Score}");
```

```
a: Alice 80
b: Alice 100
```

`b` は `a` のコピーなので、`b.Score` を書き換えても `a` は変わりません。`ValueTuple` の要素はふつうのフィールドなので、このように要素を書き換えることもできます。

### == で比べる

タプルどうしは、`==` と `!=` で比べられます。先頭から順に、対応する要素どうしを比べ、すべて等しければ `true` になります。要素の名前は比べる対象になりません。

```csharp
var a = (Name: "Alice", Score: 80);
var b = (Name: "Alice", Score: 80);
var c = (Title: "Alice", Point: 80);

Console.WriteLine(a == b);
Console.WriteLine(a == c);
```

```
True
True
```

`c` は要素の名前が違いますが、名前はコンパイル時だけのものなので、値が同じなら等しいと判定されます。

---

## よくあるミス

### 公開する API でタプルを使いすぎる

タプルは手軽ですが、要素の意味は名前でしか表せず、その名前もコンパイル時だけのものです。要素が 3 つ、4 つと増えたり、いろいろな場所で同じ組み合わせを使い回したりするなら、タプルではなく、名前の付いた構造体やクラスを定義します。

| 向いている場面 | 使うもの |
|---|---|
| メソッドの中や、近くのメソッドどうしで、一時的に値をまとめる | タプル |
| 多くの場所で使う、意味のある値の組み合わせ（座標、商品の情報など） | 構造体やクラス |

構造体やクラスなら、メソッドやプロパティを加えたり、[構造体の制約](/unity-csharp-learning/csharp/struct-constraints/) で学んだ `readonly struct` で書き換えを禁止したりできます。C# には、このような「値をまとめるための型」を短く書ける `record` という機能もあります。

---

## ワンポイントアドバイス

### System.Tuple と ValueTuple

.NET Framework 4.0 には、すでに [Tuple\<T1, T2\> クラス](https://learn.microsoft.com/dotnet/api/system.tuple-2) がありました。`Tuple.Create("Alice", 80)` のように作り、`Item1`・`Item2` で読みます。しかし、`Tuple` には次のような不便がありました。

- クラスなので、作るたびにヒープにオブジェクトが作られる
- 要素に名前を付けられず、`Item1`・`Item2` でしか読めない
- 要素は読み取り専用のプロパティで、書き換えられない

C# 7.0 で、構造体の `ValueTuple` と、`(int, string)` のようなタプルの構文が追加されました。現在、C# で「タプル」といえば、ふつうは `ValueTuple` を使うこの構文のことです。古いコードや、古いライブラリのメソッドの戻り値で `Tuple<T1, T2>` を見かけたときは、別の型であることに注意します。

---

## まとめ

- タプルは、複数の値を 1 つにまとめる型。`(int, string)` のように書き、`(値1, 値2)` で作る
- 要素に名前を付けると名前で読み書きでき、付けないと `Item1`・`Item2` で読み書きする
- 分解を使うと、タプルの要素を別々の変数に受け取れる。要らない要素は `_` で捨てる
- タプルの正体は `ValueTuple` 構造体。要素の名前はコンパイル時だけのもので、実体は `Item1`・`Item2` のフィールド
- タプルは値型なので、代入するとコピーされる。`==` で要素ごとに比べられる
- 多くの場所で使う値の組み合わせには、タプルではなく名前の付いた型を定義する

---

## 理解度チェック

1. 次のコードを実行すると何が出力されますか？

   ```csharp
   (int Q, int R) Divide(int a, int b)
   {
       return (a / b, a % b);
   }

   var (q, r) = Divide(17, 5);
   Console.WriteLine($"{q} 余り {r}");

   var t = Divide(9, 4);
   var copy = t;
   copy.Q = 0;
   Console.WriteLine($"{t.Q} {copy.Q}");
   ```

2. `(string Name, int Age) p = ("Alice", 20);` と宣言したとき、`p.Item1` と書いてもコンパイルできるのはなぜですか？
3. 3 つの `int` を返すメソッド `GetRgb` の戻り値をタプルで受け取り、緑の成分（2 つ目の値）だけを変数 `g` に取り出すには、どう書けばよいですか？

<details markdown="1">
<summary>解答を見る</summary>

1. 次のように出力されます。`Divide(17, 5)` は `(3, 2)` を返し、`q` と `r` に分解されます。`copy` は `t` のコピーなので、`copy.Q` を書き換えても `t.Q` は `2` のままです。

   ```
   3 余り 2
   2 0
   ```

2. タプルの正体は `ValueTuple<string, int>` 構造体で、要素の実体は `Item1` と `Item2` というフィールドだからです。`Name` や `Age` という名前はコンパイル時だけのもので、コンパイラーが `Item1`・`Item2` に置き換えます。
3. 分解を使い、要らない要素を `_` で捨てます。

   ```csharp
   (_, int g, _) = GetRgb();
   ```

</details>

---

## 次のステップ

[列挙型](/unity-csharp-learning/csharp/enums/) では、関連する定数に名前を付けてまとめる `enum` を学びます。
