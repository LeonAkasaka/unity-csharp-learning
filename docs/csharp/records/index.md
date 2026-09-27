---
layout: page
title: record
permalink: /csharp/records/
---

# record

座標、金額、商品の情報のように、「いくつかの値のまとまり」を表し、中身が同じなら同じものとして扱いたい型があります。**record**（レコード）を使うと、このような型を 1 行で宣言でき、中身で比べる `==` や `Equals`、中身を表示する `ToString` などを、コンパイラーが自動で作ってくれます。このページでは、record の宣言、中身による比較、一部だけ変えたコピーを作る `with` 式、構造体版の `record struct` を学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- 位置指定の構文で record を宣言し、コンパイラーが作るメンバーを説明できる
- record の `==` が、参照ではなく中身を比べることを説明できる
- `with` 式で、一部のプロパティだけを変えた新しいインスタンスを作れる
- `record` と `record struct` の違いを説明し、タプル・構造体・クラスと使い分けられる

## 前提知識

- [タプル](/unity-csharp-learning/csharp/tuples/) を読んでいること
- [値型と参照型](/unity-csharp-learning/csharp/value-reference-types/) を読んでいること
- [演算子のオーバーロード](/unity-csharp-learning/csharp/operator-overloading/) を読んでいること
- [プロパティ](/unity-csharp-learning/csharp/properties/) を読んでいること

---

## 1. 値を表すクラスの手間

座標を表す `Point` クラスを、ふつうに定義してみます。

```csharp
Point a = new Point(1, 2);
Point b = new Point(1, 2);

Console.WriteLine(a == b);
Console.WriteLine(a);

class Point
{
    public int X { get; }
    public int Y { get; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}
```

```
False
Point
```

`a` と `b` の中身は同じですが、[値型と参照型](/unity-csharp-learning/csharp/value-reference-types/) で学んだように、クラスの `==` は同じオブジェクトかどうかを比べるので、`False` になります。`Console.WriteLine(a)` で表示されるのも、型の名前 `Point` だけです。

座標のように「中身が同じなら同じもの」として扱いたい型では、これでは困ります。[演算子のオーバーロード](/unity-csharp-learning/csharp/operator-overloading/) では、`==`・`!=`・`Equals`・`GetHashCode` を自分で書いて、中身で比べるようにしました。さらに `ToString` もオーバーライドすれば中身を表示できますが、プロパティが 1 つ増えるたびに、これらのメソッドをすべて直す必要があります。

---

## 2. record を宣言する

C# 9 で追加された [record](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/record) を使うと、同じ `Point` を次のように宣言できます。

**書式：[record](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/record#positional-syntax-for-property-and-field-definition)（位置指定の構文）**
```
record 型名(型1 プロパティ名1, 型2 プロパティ名2, ...);
```

型名の後の丸かっこにプロパティを並べるこの書き方を、**位置指定の構文**（positional syntax）といいます。

```csharp
Point a = new Point(1, 2);
Point b = new Point(1, 2);

Console.WriteLine(a == b);
Console.WriteLine(a.Equals(b));
Console.WriteLine(object.ReferenceEquals(a, b));
Console.WriteLine(a);
Console.WriteLine($"X = {a.X}, Y = {a.Y}");

record Point(int X, int Y);
```

```
True
True
False
Point { X = 1, Y = 2 }
X = 1, Y = 2
```

`a` と `b` は別々のオブジェクトです（`ReferenceEquals` は `False`）が、`==` と `Equals` は中身を比べて `True` を返します。`Console.WriteLine(a)` では、プロパティの名前と値が表示されます。

この 1 行の宣言から、コンパイラーは次のメンバーを作ります。

| コンパイラーが作るもの | 内容 |
|---|---|
| プロパティ `X`・`Y` | `{ get; init; }` のプロパティ。作った後は書き換えられない |
| コンストラクター | `new Point(1, 2)` のように、プロパティの値を順に受け取る |
| `Equals`・`GetHashCode`・`==`・`!=` | すべてのプロパティの値で比べる |
| `ToString` | `Point { X = 1, Y = 2 }` の形で中身を返す |
| `Deconstruct` | `var (x, y) = a;` のように分解できるようにする |

1 節で手で書くと何十行にもなるメソッドを、コンパイラーが代わりに作ってくれます。プロパティを増やしたときも、丸かっこの中に 1 つ加えるだけで、これらのメソッドはすべて新しいプロパティを含むように作り直されます。

`record` は、クラスの一種です。`record Point` は参照型で、`new` で作るとヒープにオブジェクトが作られます。違うのは、コンパイラーが値を比べるメンバーを作ってくれる点です。

### プロパティは書き換えられない

位置指定の構文で作られるプロパティは、[プロパティ](/unity-csharp-learning/csharp/properties/) で学んだ `init` アクセサーを持つので、作った後に代入するとコンパイルエラーになります。

```csharp
// ❌ NG: record のプロパティは init 専用なので代入できない（CS8852）
// Point a = new Point(1, 2);
// a.X = 10;
```

中身で比べる型のプロパティが途中で変わると、`Dictionary` のキーに使ったときなどに困ります（3 節）。そのため、record は、作った後に書き換えない使い方が基本です。

### メンバーを追加する

record にも、クラスと同じようにプロパティやメソッドを追加できます。そのときは、`;` の代わりに `{ }` を書いて、その中にメンバーを書きます。

```csharp
Item sword = new Item("剣", 1200);
Console.WriteLine(sword);
Console.WriteLine(sword.PriceWithTax);

record Item(string Name, int Price)
{
    public int PriceWithTax
    {
        get { return Price * 110 / 100; }
    }
}
```

```
Item { Name = 剣, Price = 1200, PriceWithTax = 1320 }
1320
```

追加した public のプロパティも、`ToString` の表示に含まれます。

---

## 3. 中身で比べる

record は `Equals` と `GetHashCode` を中身で作るので、[Dictionary\<TKey, TValue\>](/unity-csharp-learning/csharp/dictionary/) のキーや、[HashSet\<T\>](/unity-csharp-learning/csharp/other-collections/) の要素にそのまま使えます。Dictionary のワンポイントで、「自分で作った型をキーにするときは、`Equals` と `GetHashCode` の両方を正しくオーバーライドする必要がある」と書きました。record なら、その必要がありません。

```csharp
HashSet<Point> visited = new HashSet<Point>();
visited.Add(new Point(1, 2));
visited.Add(new Point(3, 4));
visited.Add(new Point(1, 2));
Console.WriteLine($"Count = {visited.Count}");
Console.WriteLine(visited.Contains(new Point(3, 4)));

Dictionary<Point, string> names = new Dictionary<Point, string>();
names[new Point(0, 0)] = "原点";
Console.WriteLine(names[new Point(0, 0)]);

record Point(int X, int Y);
```

```
Count = 2
True
原点
```

3 回目の `Add` は、中身が同じ `(1, 2)` がすでにあるので追加されません。`Contains` や `Dictionary` のインデクサも、`new` で新しく作った `Point` で探せます。ふつうのクラスでは、どれも別のオブジェクトとして扱われるので、こうはなりません。

---

## 4. with 式で一部だけ変える

record のプロパティは書き換えられないので、「レベルだけ 1 つ上げたプレイヤー」が欲しいときは、新しいインスタンスを作ります。[with 式](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/with-expression) を使うと、元のインスタンスをコピーし、指定したプロパティだけを変えた新しいインスタンスを作れます。

**書式：[with 式](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/with-expression)**
```
元のインスタンス with { プロパティ名 = 値, ... }
```

```csharp
Player alice = new Player("Alice", 5, 100);
Player leveled = alice with { Level = 6 };

Console.WriteLine(alice);
Console.WriteLine(leveled);
Console.WriteLine(object.ReferenceEquals(alice, leveled));

record Player(string Name, int Level, int Hp);
```

```
Player { Name = Alice, Level = 5, Hp = 100 }
Player { Name = Alice, Level = 6, Hp = 100 }
False
```

`with` 式は `alice` を書き換えず、`Level` だけが違う新しい `Player` を作ります。変えなかった `Name` と `Hp` は、`alice` からコピーされます。元のインスタンスを書き換えないので、`alice` をほかの場所で使っていても影響がありません。

### 分解

位置指定の構文で宣言した record は、[タプル](/unity-csharp-learning/csharp/tuples/) と同じように分解できます。

```csharp
Point p = new Point(3, 4);
var (x, y) = p;
Console.WriteLine($"x = {x}, y = {y}");

record Point(int X, int Y);
```

```
x = 3, y = 4
```

---

## 5. record struct

`record` はクラス（参照型）ですが、C# 10 以降では、構造体（値型）の record を `record struct` で宣言できます。書き換えを禁止したいときは `readonly record struct` にします。

```csharp
PointS a = new PointS(1, 2);
PointS b = a;
b.X = 10;
Console.WriteLine(a);
Console.WriteLine(b);
Console.WriteLine(a == new PointS(1, 2));

ReadOnlyPoint r = new ReadOnlyPoint(1, 2);
ReadOnlyPoint moved = r with { X = 5 };
Console.WriteLine(moved);

record struct PointS(int X, int Y);
readonly record struct ReadOnlyPoint(int X, int Y);
```

```
PointS { X = 1, Y = 2 }
PointS { X = 10, Y = 2 }
True
ReadOnlyPoint { X = 5, Y = 2 }
```

- `record struct` は構造体なので、`b = a` で値がコピーされ、`b.X` を書き換えても `a` は変わらない。位置指定の構文のプロパティは、`record` と違って書き換えられる
- `readonly record struct` のプロパティは、`record` と同じく書き換えられない。一部を変えるときは `with` 式を使う
- どちらも、`==`・`Equals`・`ToString` はコンパイラーが作る

[構造体の制約](/unity-csharp-learning/csharp/struct-constraints/) で「書き換える必要がなければ `readonly struct` にする」と学んだのと同じく、構造体の record も、書き換える必要がなければ `readonly record struct` にします。

---

## よくあるミス

### 参照型のプロパティは参照で比べられる

record が作る `Equals` は、プロパティごとにその型の `Equals` で比べます。プロパティが配列や `List<T>` のような参照型だと、その比較は中身ではなく、同じオブジェクトかどうかになります。

```csharp
Team a = new Team("赤", new List<string> { "Alice" });
Team b = new Team("赤", new List<string> { "Alice" });
Console.WriteLine(a == b);

record Team(string Name, List<string> Members);
```

```
False
```

`Members` の中身はどちらも `"Alice"` だけですが、別々に作った `List<string>` なので、`a == b` は `False` になります。また、record のプロパティは書き換えられなくても、`a.Members.Add("Bob")` のように `List<string>` の中身は変えられます。[const と readonly（補足）](/unity-csharp-learning/csharp/const-readonly/) で学んだ「`readonly` なのは参照だけ」と同じです。record のプロパティには、`int`・`string`・ほかの record のように、中身で比べられる型を使うのが基本です。

---

## ワンポイントアドバイス

### タプル・構造体・クラス・record の使い分け

| 使いたい場面 | 選ぶもの |
|---|---|
| メソッドの中や、近くのメソッドどうしで、一時的に値をまとめる | タプル |
| 多くの場所で使う値のまとまりで、中身で比べたい | `record`（小さくて頻繁に作るなら `readonly record struct`） |
| 状態を持ち、途中で変化していくもの（プレイヤーの状態、ゲームの進行など） | クラス |
| 小さな値で、書き換えも必要 | 構造体（`record struct`） |

record も、クラスと同じように継承できます（`record Student(string Name, int Grade) : Person(Name);` のように書きます）。ただし、record を継承できるのは record だけで、ふつうのクラスとは混ぜられません。

---

## まとめ

- `record 型名(プロパティ...);` の 1 行で、プロパティ・コンストラクター・`Equals`・`GetHashCode`・`==`・`ToString`・`Deconstruct` がコンパイラーによって作られる
- `record` はクラス（参照型）だが、`==` と `Equals` はすべてのプロパティの値で比べる
- 中身で比べるので、`Dictionary` のキーや `HashSet` の要素にそのまま使える
- 位置指定の構文のプロパティは `init` 専用で、作った後は書き換えられない。一部を変えた新しいインスタンスは `with` 式で作る
- `record struct` は構造体の record。書き換えを禁止するには `readonly record struct` にする
- 参照型のプロパティは、中身ではなく参照で比べられる

---

## 理解度チェック

1. 次のコードを実行すると何が出力されますか？

   ```csharp
   Card a = new Card("ハート", 1);
   Card b = a with { Rank = 13 };
   Card c = new Card("ハート", 1);

   Console.WriteLine(a == c);
   Console.WriteLine(b);
   Console.WriteLine(a == b);

   record Card(string Suit, int Rank);
   ```

2. `record Point(int X, int Y);` を、ふつうのクラスで同じように動かすには、何を自分で書く必要がありますか？主なものを挙げてください。
3. `record Point` と `record struct Point` で、`Point b = a;` とした後の動きはどう違いますか？

<details markdown="1">
<summary>解答を見る</summary>

1. 次のように出力されます。`a` と `c` は中身が同じなので `True` です。`b` は `a` の `Rank` だけを `13` に変えた新しいインスタンスなので、`a` とは中身が違い `False` です。

   ```
   True
   Card { Suit = ハート, Rank = 13 }
   False
   ```

2. `X` と `Y` のプロパティとコンストラクターに加えて、中身で比べる `Equals` と `GetHashCode` のオーバーライド、`==` と `!=` の演算子、中身を返す `ToString` のオーバーライド、分解のための `Deconstruct` メソッドが必要です。
3. `record Point` は参照型なので、`b = a` で参照がコピーされ、`a` と `b` は同じオブジェクトを指します。`record struct Point` は値型なので、値そのものがコピーされ、`a` と `b` は別々の値になります。

</details>

---

## 次のステップ

[列挙型](/unity-csharp-learning/csharp/enums/) では、関連する定数に名前を付けてまとめる `enum` を学びます。
