---
layout: page
title: 構造体
permalink: /csharp/structs/
---

# 構造体

**構造体**（struct）は、クラスと同じようにフィールドやメソッドをまとめて定義できる型です。クラスとの大きな違いは、構造体が値型であることです。代入するとすべてのフィールドがコピーされ、ヒープにオブジェクトを作りません。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `struct` で構造体を定義して使える
- 構造体を代入したりメソッドに渡したりすると、値がコピーされることを説明できる
- クラスと構造体を、目的に合わせて使い分けられる
- プロパティから受け取った構造体を書き換えられない理由を説明できる

## 前提知識

- [値型と参照型](/unity-csharp-learning/csharp/value-reference-types/) を読んでいること
- [ガベージコレクション](/unity-csharp-learning/csharp/garbage-collection/) を読んでいること
- [プロパティ](/unity-csharp-learning/csharp/properties/) を読んでいること

---

## 1. 構造体を定義する

構造体は、`class` の代わりに `struct` キーワードで定義します。フィールド、コンストラクター、メソッド、プロパティなど、クラスと同じメンバーを書けます。

**書式：[構造体の定義](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/struct)**
```
struct 構造体名
{
    // フィールド・コンストラクター・メソッド・プロパティ
}
```

次の `Point` は、2 次元の座標を表す構造体です。

```csharp
Point p = new Point(1, 2);
Console.WriteLine($"({p.X}, {p.Y})");

struct Point
{
    public int X;
    public int Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}
```

```
(1, 2)
```

使い方はクラスと同じで、`new` とコンストラクターで値を作ります。ただし、構造体の `new` は、ヒープにオブジェクトを作るのではなく、変数 `p` の中に `X` と `Y` の値を直接入れます。

---

## 2. 構造体は値型

構造体は値型なので、代入するとすべてのフィールドの値がコピーされます。

```csharp
Point a = new Point(1, 2);
Point b = a;
b.X = 100;

Console.WriteLine($"a = ({a.X}, {a.Y})");
Console.WriteLine($"b = ({b.X}, {b.Y})");

struct Point
{
    public int X;
    public int Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}
```

```
a = (1, 2)
b = (100, 2)
```

`b = a` で、`a` の `X` と `Y` が `b` にコピーされます。`b.X` を書き換えても `a` は変わりません。[値型と参照型](/unity-csharp-learning/csharp/value-reference-types/) で見た `int` と同じ動きで、クラスの `Box` とは違います。

メソッドに渡したときも、構造体の値がコピーされます。呼び出し元の変数を書き換えたいときは、`ref` を使います。

```csharp
Point a = new Point(1, 2);

MoveRight(a);
Console.WriteLine($"MoveRight の後: ({a.X}, {a.Y})");

MoveRightRef(ref a);
Console.WriteLine($"MoveRightRef の後: ({a.X}, {a.Y})");

void MoveRight(Point p)
{
    p.X += 10;
}

void MoveRightRef(ref Point p)
{
    p.X += 10;
}

struct Point
{
    public int X;
    public int Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}
```

```
MoveRight の後: (1, 2)
MoveRightRef の後: (11, 2)
```

`MoveRight` が書き換えたのは、パラメータ `p` にコピーされた値です。`MoveRightRef` の `p` は `a` そのものを指しているので、`a` が書き換わります。

### 配列の要素も値そのもの

構造体の配列を作ると、要素の 1 つ 1 つが、すべてのフィールドが 0 の構造体の値になります。クラスの配列のように、要素が `null` になることはありません。

```csharp
Point[] points = new Point[3];
points[1].X = 5;

foreach (Point p in points)
{
    Console.WriteLine($"({p.X}, {p.Y})");
}

struct Point
{
    public int X;
    public int Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}
```

```
(0, 0)
(5, 0)
(0, 0)
```

`new Point[3]` で、3 つの `Point` の値が配列の中に並んで作られます。`new Point(...)` で 1 つずつ作らなくても、すぐに `points[1].X` に代入できます。`Point` がクラスなら、要素は `null` なので、`points[1].X = 5` で `NullReferenceException` が発生します。

---

## 3. クラスと構造体の違い

| | クラス | 構造体 |
|---|---|---|
| 種類 | 参照型 | 値型 |
| 代入・値渡しでコピーされるもの | 参照 | すべてのフィールドの値 |
| データが置かれる場所 | ヒープ（オブジェクト） | 変数やフィールドがある場所 |
| `null` | 入れられる | 入れられない |
| 継承 | できる | できない（[構造体の制約](/unity-csharp-learning/csharp/struct-constraints/) で学ぶ） |

構造体は、ヒープにオブジェクトを作らないので、[ガベージコレクション](/unity-csharp-learning/csharp/garbage-collection/) の対象になりません。小さなデータをたくさん扱うときに、構造体にすると、ガベージコレクターの仕事を減らせます。

一方で、構造体は代入やメソッドの呼び出しのたびに、すべてのフィールドがコピーされます。フィールドが多い大きな構造体では、このコピーのコストが大きくなります。

### どちらを選ぶか

迷ったらクラスを使います。次の条件がそろうときは、構造体を検討します。

- 座標、色、日時のように、1 つの値として扱うデータである
- フィールドが少なく、サイズが小さい
- 作った後に値を書き換えない

.NET の [DateTime](https://learn.microsoft.com/dotnet/api/system.datetime)（日時）も、この条件に当てはまる構造体です。選び方の詳しい目安は、[クラスと構造体の選択](https://learn.microsoft.com/dotnet/standard/design-guidelines/choosing-between-class-and-struct) にまとめられています。

---

## 4. プロパティから受け取った構造体は書き換えられない

構造体を返すプロパティから、構造体のフィールドを直接書き換えることはできません。

```csharp
// ❌ NG: プロパティが返した構造体のフィールドに代入している
// player.Position.X = 10;  // CS1612
```

プロパティの `get` アクセサーは、メソッドと同じように値を返します。構造体は値型なので、返されるのは `Position` の値のコピーです。コピーの `X` を書き換えても、`player` の中の値は変わりません。意味のない代入なので、コンパイラーがエラーにします。

書き換えるには、値をいったん変数に受け取って書き換え、プロパティに代入し直します。

```csharp
Player player = new Player();

Point position = player.Position;
position.X = 10;
player.Position = position;

Console.WriteLine($"({player.Position.X}, {player.Position.Y})");

class Player
{
    public Point Position { get; set; }
}

struct Point
{
    public int X;
    public int Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}
```

```
(10, 0)
```

このように、値を書き換えられる構造体は、コピーを書き換えてしまう間違いを起こしやすくなります。構造体は、作った後に値を書き換えない形で定義するのが基本です。その方法は、[構造体の制約](/unity-csharp-learning/csharp/struct-constraints/) で学びます。

---

## よくあるミス

### 構造体を == で比べる

```csharp
// ❌ NG: 自分で定義した構造体には == が定義されていない
// Point a = new Point(1, 2);
// Point b = new Point(1, 2);
// Console.WriteLine(a == b);  // CS0019
```

構造体には、`==` が自動では定義されません。すべてのフィールドが等しいかは `Equals` メソッドで比べられます。`==` で比べたいときは、[演算子のオーバーロード](/unity-csharp-learning/csharp/operator-overloading/) で `==` と `!=` を定義します。

```csharp
Point a = new Point(1, 2);
Point b = new Point(1, 2);
Console.WriteLine(a.Equals(b));

struct Point
{
    public int X;
    public int Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}
```

```
True
```

---

## まとめ

- 構造体は `struct` で定義する値型。クラスと同じメンバーを書ける
- 構造体を代入したりメソッドに渡したりすると、すべてのフィールドの値がコピーされる
- 構造体の配列の要素は、`null` ではなく、すべてのフィールドが 0 の値になる
- 構造体はヒープにオブジェクトを作らないので、ガベージコレクションの対象にならない。小さく、1 つの値として扱い、書き換えないデータに向いている
- プロパティが返す構造体はコピーなので、そのフィールドには代入できない（CS1612）

---

## 理解度チェック

1. 構造体とクラスで、代入したときにコピーされるものの違いを説明してください。
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   Size[] sizes = new Size[2];
   Size s = sizes[0];
   s.Width = 10;
   sizes[1].Width = 20;

   Console.WriteLine($"{sizes[0].Width}, {sizes[1].Width}, {s.Width}");

   struct Size
   {
       public int Width;
   }
   ```

3. 次のコードがコンパイルエラーになる理由を説明してください。

   ```csharp
   class Window
   {
       public Size Size { get; set; }
   }

   // window.Size.Width = 100;
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. 構造体では、すべてのフィールドの値がコピーされます。クラスでは、オブジェクトへの参照がコピーされ、コピー元とコピー先は同じオブジェクトを指します。
2. `0, 20, 10` が出力されます。`s` は `sizes[0]` のコピーなので、`s.Width` を書き換えても `sizes[0]` は変わりません。`sizes[1].Width` は配列の要素を直接書き換えています。
3. `window.Size` の `get` アクセサーが返すのは `Size` の値のコピーで、そのフィールドを書き換えても `window` の中の値は変わらないからです（CS1612）。

</details>

---

## 次のステップ

[構造体の制約](/unity-csharp-learning/csharp/struct-constraints/) では、構造体が継承できないことや、コンストラクターの規則、書き換えられない構造体を定義する `readonly struct` を学びます。
