---
layout: page
title: 列挙型
permalink: /csharp/enums/
---

# 列挙型

**列挙型**（enum）は、関連する定数に名前を付けて、1 つの型としてまとめたものです。方向や状態のように、決まった選択肢の中から 1 つを選ぶ値を、数値ではなく名前で扱えるようになります。列挙型も値型です。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `enum` で列挙型を定義し、`switch` 文と組み合わせて使える
- 列挙型の値が整数であることと、整数との変換の方法を説明できる
- 定義されていない値が列挙型の変数に入りうることを説明できる
- 文字列と列挙型の値を変換できる

## 前提知識

- [条件分岐](/unity-csharp-learning/csharp/conditionals/) を読んでいること
- [値型と参照型](/unity-csharp-learning/csharp/value-reference-types/) を読んでいること
- [ビット演算](/unity-csharp-learning/csharp/bitwise-operations/) を読んでいること

---

## 1. 列挙型を定義する

キャラクターの向きを、`0` が北、`1` が東、`2` が南、`3` が西、という整数で表すとします。コードに `direction == 2` と書いても、`2` が南であることは、決まりを知らないと読み取れません。`4` や `-1` のような、どの向きでもない値も入れられてしまいます。

列挙型を使うと、選択肢に名前を付けられます。

**書式：[列挙型の定義](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/enum)**
```
enum 列挙型名
{
    メンバー名1,
    メンバー名2,
    // ...
}
```

列挙型の値は、`列挙型名.メンバー名` で表します。

```csharp
Direction d = Direction.South;
Console.WriteLine(d);

switch (d)
{
    case Direction.North:
        Console.WriteLine("北を向いている");
        break;
    case Direction.South:
        Console.WriteLine("南を向いている");
        break;
    default:
        Console.WriteLine("東か西を向いている");
        break;
}

enum Direction
{
    North,
    East,
    South,
    West
}
```

```
South
南を向いている
```

`Console.WriteLine` に列挙型の値を渡すと、メンバーの名前が表示されます。[条件分岐](/unity-csharp-learning/csharp/conditionals/) で学んだように、`switch` 文の `case` には列挙型の値を書けます。

---

## 2. 列挙型の値は整数

列挙型のメンバーには、それぞれ整数の値が割り当てられています。何も指定しなければ、最初のメンバーが `0` で、順に 1 ずつ増えます。`=` で値を指定することもできます。

列挙型と整数の間は、キャストで変換します。

```csharp
Console.WriteLine((int)Direction.South);
Console.WriteLine((int)Status.NotFound);

Status s = (Status)200;
Console.WriteLine(s);

enum Direction
{
    North,
    East,
    South,
    West
}

enum Status
{
    Ok = 200,
    NotFound = 404
}
```

```
2
404
Ok
```

`Direction.South` は 3 番目のメンバーなので `2` です。`Status` では、メンバーの値を `=` で指定しています。`(Status)200` のように、整数から列挙型の値にも変換できます。

### 基になる整数型

列挙型の値を表す整数の型を、**基になる型**（underlying type）といいます。既定は `int` です。列挙型名の後に `: 型` と書くと、`byte` や `long` などに変えられます。

```csharp
Console.WriteLine(sizeof(Direction));
Console.WriteLine(sizeof(Color));

enum Direction
{
    North,
    East,
    South,
    West
}

enum Color : byte
{
    Red,
    Green,
    Blue
}
```

```
4
1
```

`Direction` の基になる型は `int` なので 4 バイト、`Color` は `byte` なので 1 バイトです。[値型と参照型](/unity-csharp-learning/csharp/value-reference-types/) で学んだように、列挙型の値は変数の中に直接入ります。列挙型の値の実体は、基になる型の整数です。

---

## 3. 定義されていない値

列挙型の変数には、メンバーとして定義していない整数の値も入れられます。キャストはチェックされず、そのまま変換されるからです。定義されている値かどうかは、[Enum.IsDefined メソッド](https://learn.microsoft.com/dotnet/api/system.enum.isdefined) で調べられます。

```csharp
Direction d = (Direction)10;

Console.WriteLine(d);
Console.WriteLine(Enum.IsDefined(d));

enum Direction
{
    North,
    East,
    South,
    West
}
```

```
10
False
```

`Console.WriteLine` は、定義されていない値を、名前ではなく数値で表示します。

列挙型の既定値は、`0` を表す値です。フィールドや配列の要素を初期化しなかった列挙型の値は、`0` になります。`0` のメンバーを定義していなくても、既定値は `0` です。何も選ばれていない状態を表す `None = 0` のようなメンバーを最初に定義しておくと、既定値の意味がはっきりします。

```csharp
Direction[] directions = new Direction[1];
Console.WriteLine(directions[0]);

Weekday day = default;
Console.WriteLine(day);

enum Direction
{
    North,
    East,
    South,
    West
}

enum Weekday
{
    Monday = 1,
    Tuesday,
    Wednesday
}
```

```
North
0
```

`Weekday` は `Monday` が `1` から始まり、`Tuesday` は `2`、`Wednesday` は `3` です。`0` のメンバーがないので、既定値は数値の `0` として表示されます。

---

## 4. 文字列との変換

列挙型の値を文字列にするには、`ToString` を使います。文字列を列挙型の値に変換するには、[Enum.TryParse メソッド](https://learn.microsoft.com/dotnet/api/system.enum.tryparse) を使います。`int.TryParse` と同じように、変換できたかどうかを `bool` で返し、変換した値を `out` パラメータで返します。

```csharp
string name = Direction.West.ToString();
Console.WriteLine(name);

if (Enum.TryParse("East", out Direction d1))
{
    Console.WriteLine($"変換できた: {d1}");
}

if (!Enum.TryParse("Up", out Direction d2))
{
    Console.WriteLine("Up は Direction のメンバーではない");
}

enum Direction
{
    North,
    East,
    South,
    West
}
```

```
West
変換できた: East
Up は Direction のメンバーではない
```

`Enum.TryParse` は、大文字と小文字を区別します。区別しないで変換したいときは、`Enum.TryParse("east", true, out Direction d)` のように、2 つ目の引数に `true` を渡します。

すべてのメンバーを順に取り出すには、[Enum.GetValues メソッド](https://learn.microsoft.com/dotnet/api/system.enum.getvalues) を使います。

```csharp
foreach (Direction d in Enum.GetValues<Direction>())
{
    Console.WriteLine($"{d} = {(int)d}");
}

enum Direction
{
    North,
    East,
    South,
    West
}
```

```
North = 0
East = 1
South = 2
West = 3
```

---

## 5. [Flags] で組み合わせを表す

[ビット演算](/unity-csharp-learning/csharp/bitwise-operations/) のワンポイントアドバイスで、[Flags 属性](https://learn.microsoft.com/dotnet/api/system.flagsattribute) を付けた列挙型を紹介しました。メンバーの値を `1`、`2`、`4` のように 2 のべき乗にすると、それぞれのメンバーが別々のビットを使うので、`|` で組み合わせて、複数の状態を 1 つの値で表せます。

```csharp
PlayerState state = PlayerState.Jump | PlayerState.Run;

Console.WriteLine(state);
Console.WriteLine((int)state);
Console.WriteLine(state.HasFlag(PlayerState.Run));
Console.WriteLine(state.HasFlag(PlayerState.Crouch));

[Flags]
enum PlayerState
{
    None = 0,
    Jump = 1 << 0,
    Run = 1 << 1,
    Crouch = 1 << 2
}
```

```
Jump, Run
3
True
False
```

`[Flags]` を付けた列挙型の `ToString` は、組み合わせたメンバーの名前を `,` で区切って返します。[HasFlag メソッド](https://learn.microsoft.com/dotnet/api/system.enum.hasflag) は、指定したメンバーのビットが立っているかを調べます。どのビットも立っていない状態を表すために、`None = 0` を定義しておきます。

---

## よくあるミス

### 列挙型の変数には定義したメンバーしか入らないと思い込む

3 節で見たように、列挙型の変数には、定義していない値も入ります。`switch` 文ですべてのメンバーの `case` を書いても、それ以外の値が来る可能性はなくなりません。

```csharp
Console.WriteLine(Describe((Direction)10));

string Describe(Direction d)
{
    switch (d)
    {
        case Direction.North:
            return "北";
        case Direction.East:
            return "東";
        case Direction.South:
            return "南";
        case Direction.West:
            return "西";
        default:
            return $"不明な向き（{d}）";
    }
}

enum Direction
{
    North,
    East,
    South,
    West
}
```

```
不明な向き（10）
```

`default` を書いておくと、想定していない値が来たときにも処理を続けられます。想定していない値を受け取ったこと自体が誤りなら、`default` で例外を投げます。

---

## まとめ

- 列挙型は、関連する定数に名前を付けてまとめた値型。`enum` で定義し、`列挙型名.メンバー名` で値を表す
- 列挙型の値の実体は整数。既定の基になる型は `int` で、`: byte` のように変えられる
- 整数と列挙型の値はキャストで変換できる。定義していない値も入るので、`Enum.IsDefined` や `switch` の `default` で備える
- 列挙型の既定値は `0`。`0` を表すメンバーを定義しておくと、既定値の意味がはっきりする
- `ToString` と `Enum.TryParse` で文字列と変換でき、`Enum.GetValues` で全メンバーを取り出せる
- `[Flags]` を付け、メンバーの値を 2 のべき乗にすると、組み合わせを表せる

---

## 理解度チェック

1. 向きを `int` の `0`〜`3` で表す代わりに列挙型を使うと、どのような利点がありますか？
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   Console.WriteLine((int)Level.Medium);
   Console.WriteLine((Level)10);
   Console.WriteLine(default(Level));

   enum Level
   {
       Low = 1,
       Medium = 5,
       High
   }
   ```

3. 次の `[Flags]` 付きの列挙型で、`Permission.Read | Permission.Write` を `(int)` で整数にすると、いくつになりますか？

   ```csharp
   [Flags]
   enum Permission
   {
       None = 0,
       Read = 1,
       Write = 2,
       Execute = 4
   }
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. `Direction.South` のように名前で書けるので、コードの意味が読み取りやすくなります。また、変数の型が `Direction` になるので、向きではない値を間違って渡しにくくなります。
2. 次のように出力されます。`Medium` は `5` です。`10` は定義されていないので数値で表示され、`0` のメンバーもないので既定値も数値で表示されます。

   ```
   5
   10
   0
   ```

3. `3` になります。`Read` の `1` と `Write` の `2` のビットが両方立つからです。

</details>

---

## 次のステップ

[デリゲートの基本](/unity-csharp-learning/csharp/delegates/) では、メソッドへの参照を変数として扱うデリゲートのしくみを学びます。
