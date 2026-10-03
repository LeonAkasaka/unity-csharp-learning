---
layout: page
title: 演算子のオーバーロード
permalink: /csharp/operator-overloading/
---

# 演算子のオーバーロード

`int` どうしなら `+` や `==` をそのまま使えますが、自分で作ったクラスには、最初は `+` を使えません。**演算子のオーバーロード**（operator overloading）を使うと、自分で作ったクラスで、演算子が何をするかを定義できます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- 自分で作ったクラスに `+` などの演算子を定義できる
- `implicit` と `explicit` を使い分けて、自分で作ったクラスの型変換を定義できる
- `a + b` が、演算子を定義したメソッドの呼び出しに変換されることを説明できる
- `==` と `!=` のように、ペアで定義しなければならない演算子があることを説明できる

## 前提知識

- [オーバーロード解決](/unity-csharp-learning/csharp/overload-resolution/) を読んでいること
- [プロパティ](/unity-csharp-learning/csharp/properties/) を読んでいること
- [プリミティブ型と型変換](/unity-csharp-learning/csharp/primitive-types/) で、暗黙的な型変換とキャストを学んだこと

---

## 1. 自分で作ったクラスには + を使えない

2 次元のベクトル（`X` と `Y` の組）を表す `Vector` クラスを作るとします。`int` なら `x + y` と書けますが、`Vector` のインスタンスどうしを `+` で足そうとすると、コンパイルエラーになります。

```csharp
// ❌ NG: Vector には + が定義されていない
// Vector a = new Vector(1, 2);
// Vector b = new Vector(3, 4);
// Vector c = a + b;  // CS0019
```

コンパイラーは、`Vector` どうしを `+` したときに何をすればよいかを知らないからです。

---

## 2. 演算子を定義する

演算子は、[operator](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/operator-overloading) キーワードを使って、メソッドのように定義します。

**書式：[演算子のオーバーロード](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/operator-overloading)**
```
public static 戻り値の型 operator 演算子(型 左辺, 型 右辺)
{
    // 処理
}
```

| 要素 | 説明 |
|---|---|
| `public static` | 演算子の定義には、必ず両方を付ける |
| `戻り値の型` | 演算の結果の型 |
| `operator 演算子` | 定義する演算子。`operator +` のように書く |
| `左辺`・`右辺` | 演算子の左と右に書かれた値を受け取るパラメータ。少なくとも一方は、定義しているクラスの型にする |

`static` は、インスタンスではなくクラスそのものに属するメンバーを表すキーワードです。詳しくは、[static メンバーと static クラス](/unity-csharp-learning/csharp/static-members/) で学びます。

```csharp
Vector a = new Vector(1, 2);
Vector b = new Vector(3, 4);

Vector c = a + b;
Console.WriteLine($"({c.X}, {c.Y})");

Vector d = b - a;
Console.WriteLine($"({d.X}, {d.Y})");

a += b;
Console.WriteLine($"({a.X}, {a.Y})");

class Vector
{
    public int X { get; }
    public int Y { get; }

    public Vector(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Vector operator +(Vector a, Vector b)
    {
        return new Vector(a.X + b.X, a.Y + b.Y);
    }

    public static Vector operator -(Vector a, Vector b)
    {
        return new Vector(a.X - b.X, a.Y - b.Y);
    }
}
```

```
(4, 6)
(2, 2)
(4, 6)
```

`+` を定義すると、`a += b` のような複合代入も使えるようになります。`a += b` は `a = a + b` として計算されます。

---

## 3. a + b はメソッドの呼び出しになる

`a + b` と書くと、コンパイラーは、定義した `operator +` を呼び出すコードに変換します。[中間言語と JIT コンパイル](/unity-csharp-learning/csharp/dotnet-internals/) で学んだ中間言語（IL）では、`operator +` は `op_Addition` という名前のメソッドになっています。

| 演算子 | IL でのメソッド名 |
|---|---|
| `+` | `op_Addition` |
| `-` | `op_Subtraction` |
| `==` | `op_Equality` |
| `!=` | `op_Inequality` |

演算子も、最終的にはメソッドとして扱われる、ということです。

---

## 4. ペアで定義する演算子

比較の演算子には、2 つをペアで定義しなければならないものがあります。片方だけを定義すると、コンパイルエラー（CS0216）になります。

| ペア |
|---|
| `==` と `!=` |
| `<` と `>` |
| `<=` と `>=` |

### == と != を定義する

`==` と `!=` を定義するときは、あわせて `Equals` メソッドと `GetHashCode` メソッドもオーバーライドします。オーバーライドしないと、コンパイラーが警告（CS0660・CS0661）を出します。`Equals` は「等しいかどうか」を調べる別の方法で、`==` と同じ結果を返すようにそろえておく必要があるからです。オーバーライドについては、[オーバーライドとポリモーフィズム](/unity-csharp-learning/csharp/polymorphism/) で学びます。ここでは、次の書き方をひとそろいとして覚えておきましょう。

```csharp
Vector a = new Vector(1, 2);
Vector b = new Vector(1, 2);
Vector c = new Vector(3, 4);

Console.WriteLine(a == b);
Console.WriteLine(a != c);

class Vector
{
    public int X { get; }
    public int Y { get; }

    public Vector(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static bool operator ==(Vector a, Vector b)
    {
        return a.X == b.X && a.Y == b.Y;
    }

    public static bool operator !=(Vector a, Vector b)
    {
        return !(a == b);
    }

    public override bool Equals(object? obj)
    {
        return obj is Vector other && this == other;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}
```

```
True
True
```

`a` と `b` は別々のインスタンスですが、`==` を「`X` と `Y` が等しいか」と定義したので、`True` になります。`!=` は、`==` の結果を反転して定義しています。`GetHashCode` の [HashCode.Combine メソッド](https://learn.microsoft.com/dotnet/api/system.hashcode.combine) は、複数の値から 1 つの整数（ハッシュコード）を作ります。

---

## 5. オーバーロードできる演算子

[オーバーロードできる演算子](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/operator-overloading#overloadable-operators) は決まっています。

| 種類 | 演算子 |
|---|---|
| 単項演算子 | `+`・`-`・`!`・`~`・`++`・`--` |
| 算術演算子 | `+`・`-`・`*`・`/`・`%` |
| ビット演算子 | `&`・`\|`・`^`・`<<`・`>>` |
| 比較演算子 | `==`・`!=`・`<`・`>`・`<=`・`>=`（ペアで定義する） |

`&&`・`||`・`=`・`.`・`? :` などは、オーバーロードできません。

演算子は、「`+` なら足し算のような意味」のように、元の演算子から想像できる動作にします。予想と違う動作をする演算子は、コードを読む人を混乱させます。

このほか、`operator` キーワードでは、型変換も定義できます。次の節で学びます。

---

## 6. 型変換を定義する

[プリミティブ型と型変換](/unity-csharp-learning/csharp/primitive-types/) で学んだように、`int` から `double` へは暗黙的に変換でき、`double` から `int` へはキャストで明示的に変換します。自分で作ったクラスにも、同じような型変換を定義できます。これを **ユーザー定義の型変換**（user-defined conversion）といいます。

**書式：[ユーザー定義の型変換](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/user-defined-conversion-operators)**
```
public static implicit operator 変換先の型(変換元の型 値)
{
    // 変換先の型の値を返す
}

public static explicit operator 変換先の型(変換元の型 値)
{
    // 変換先の型の値を返す
}
```

| 要素 | 説明 |
|---|---|
| `public static` | 演算子と同じく、必ず両方を付ける |
| `implicit` | 暗黙的な変換を定義する。キャストを書かなくても変換される |
| `explicit` | 明示的な変換を定義する。`(変換先の型)式` とキャストを書いたときだけ変換される |
| `operator 変換先の型` | 変換した結果の型。戻り値の型は、これとは別に書かない |
| `変換元の型 値` | 変換する値を受け取るパラメータ |

`変換先の型` と `変換元の型` は、どちらか一方を、定義しているクラスの型にします。どちらも定義しているクラス以外の型にすると、コンパイルエラー（CS0556）になります。

次の `Fraction` クラスは、分数を表します。`int` からの変換を `implicit` で、`double` への変換を `explicit` で定義しています。

```csharp
Fraction a = 3;
Console.WriteLine($"{a.Numerator}/{a.Denominator}");

Fraction half = new Fraction(1, 2);
Fraction b = half + 1;
Console.WriteLine($"{b.Numerator}/{b.Denominator}");

Fraction third = new Fraction(1, 3);
double d = (double)third;
Console.WriteLine(d);

class Fraction
{
    public int Numerator { get; }
    public int Denominator { get; }

    public Fraction(int numerator, int denominator)
    {
        Numerator = numerator;
        Denominator = denominator;
    }

    public static Fraction operator +(Fraction a, Fraction b)
    {
        return new Fraction(
            a.Numerator * b.Denominator + b.Numerator * a.Denominator,
            a.Denominator * b.Denominator);
    }

    public static implicit operator Fraction(int value)
    {
        return new Fraction(value, 1);
    }

    public static explicit operator double(Fraction f)
    {
        return (double)f.Numerator / f.Denominator;
    }
}
```

```
3/1
3/2
0.3333333333333333
```

`Fraction a = 3;` では、`int` の `3` が、定義した変換で `3/1` の `Fraction` になります。`half + 1` では、`operator +` のパラメータは 2 つとも `Fraction` なので、`1` が暗黙的に `Fraction` に変換されてから足されます。[オーバーロード解決](/unity-csharp-learning/csharp/overload-resolution/) で学んだように、暗黙的な変換は、引数をパラメータの型に合わせるときにも使われます。

`double` への変換では、`Numerator` を `double` にしてから割っています。`int` どうしの割り算では、小数点以下が切り捨てられるからです。

`implicit` と `explicit` は、組み込み型と同じ基準で選びます。

| 変換 | 選ぶキーワード | `Fraction` の例 |
|---|---|---|
| 情報が失われず、失敗しない | `implicit` | `int` の値は、分母が `1` の分数として、そのまま表せる |
| 情報が失われる可能性がある | `explicit` | `1/3` は、`double` では正確に表せず、`0.3333333333333333` のように途中で丸められる |

`explicit` で定義した変換は、キャストを書かないと使えません。

```csharp
// ❌ NG: Fraction から double への変換は explicit
// Fraction third = new Fraction(1, 3);
// double d = third;  // CS0266
```

`a + b` と同じように、型変換も、定義したメソッドの呼び出しに変換されます。IL でのメソッド名は、`implicit` が `op_Implicit`、`explicit` が `op_Explicit` です。

---

## よくあるミス

### == だけを定義して != を忘れる

```csharp
// ❌ NG: == を定義したら != も必要
// class A
// {
//     public int Value;
//
//     public static bool operator ==(A a, A b)  // CS0216
//     {
//         return a.Value == b.Value;
//     }
// }
```

### static を付け忘れる

```csharp
// ❌ NG: 演算子の定義には static が必要
// class A
// {
//     public A operator +(A a, A b)  // CS0558
//     {
//         return a;
//     }
// }
```

### 同じ変換を implicit と explicit の両方で定義する

同じ変換元と変換先の組み合わせには、`implicit` と `explicit` のどちらか一方しか定義できません。暗黙的な変換を定義すれば、キャストを書いても変換できます。

```csharp
// ❌ NG: A から int への変換が 2 つある
// class A
// {
//     public int Value;
//
//     public static implicit operator int(A a)
//     {
//         return a.Value;
//     }
//
//     public static explicit operator int(A a)  // CS0557
//     {
//         return a.Value;
//     }
// }
```

---

## まとめ

- 自分で作ったクラスには、最初は `+` や `==` などの演算子を使えない
- `public static 戻り値の型 operator 演算子(型 左辺, 型 右辺)` で演算子を定義する
- `a + b` は、定義した演算子のメソッドの呼び出しに変換される
- `==` と `!=`、`<` と `>`、`<=` と `>=` は、ペアで定義する
- `==` と `!=` を定義するときは、`Equals` と `GetHashCode` もオーバーライドする
- `public static implicit operator 変換先の型(変換元の型 値)` で暗黙的な変換を、`explicit` で明示的な変換を定義する。情報が失われず失敗しない変換だけを `implicit` にする

---

## 理解度チェック

1. 次のコードを実行すると何が出力されますか？

   ```csharp
   Money x = new Money(300);
   Money y = new Money(500);
   Money z = x + y;
   Console.WriteLine(z.Amount);

   class Money
   {
       public int Amount { get; }

       public Money(int amount)
       {
           Amount = amount;
       }

       public static Money operator +(Money a, Money b)
       {
           return new Money(a.Amount + b.Amount);
       }
   }
   ```

2. 1 の `Money` クラスに、金額を整数倍する `*` 演算子（`Money * int`）を追加してください。
3. `<` だけを定義して `>` を定義しないと、どうなりますか？
4. 次のコードを実行すると何が出力されますか？

   ```csharp
   Fraction f = 2;
   Fraction g = f + new Fraction(1, 4);
   Console.WriteLine($"{g.Numerator}/{g.Denominator}");
   Console.WriteLine((double)g);

   class Fraction
   {
       public int Numerator { get; }
       public int Denominator { get; }

       public Fraction(int numerator, int denominator)
       {
           Numerator = numerator;
           Denominator = denominator;
       }

       public static Fraction operator +(Fraction a, Fraction b)
       {
           return new Fraction(
               a.Numerator * b.Denominator + b.Numerator * a.Denominator,
               a.Denominator * b.Denominator);
       }

       public static implicit operator Fraction(int value)
       {
           return new Fraction(value, 1);
       }

       public static explicit operator double(Fraction f)
       {
           return (double)f.Numerator / f.Denominator;
       }
   }
   ```

5. 1 の `Money` クラスに、`Money` から `int`（金額）への変換を定義します。`implicit` と `explicit` のどちらにしますか？ 理由も答えてください。

<details markdown="1">
<summary>解答を見る</summary>

1. `800` が出力されます。`x + y` で `operator +` が呼び出され、`Amount` を足した新しい `Money` が返されます。

   ```
   800
   ```

2. ```csharp
   Money price = new Money(300);
   Money total = price * 3;
   Console.WriteLine(total.Amount);

   class Money
   {
       public int Amount { get; }

       public Money(int amount)
       {
           Amount = amount;
       }

       public static Money operator *(Money a, int times)
       {
           return new Money(a.Amount * times);
       }
   }
   ```

   `900` が表示されます。パラメータの型は、左辺と右辺で違っていてもかまいません。

3. コンパイルエラー（CS0216）になります。`<` と `>` は、ペアで定義する必要があります。

4. 次のように出力されます。`Fraction f = 2;` で `2` が `2/1` に変換されます。`f + new Fraction(1, 4)` は (2 × 4 + 1 × 1) / (1 × 4) で `9/4` になり、`(double)g` で `2.25` に変換されます。

   ```
   9/4
   2.25
   ```

5. `implicit` にします。`Amount` は `int` なので、`int` に変換しても情報が失われず、変換に失敗することもないからです。

   ```csharp
   Money price = new Money(300);
   int a = price;
   int b = (int)price;
   Console.WriteLine(a + b);

   class Money
   {
       public int Amount { get; }

       public Money(int amount)
       {
           Amount = amount;
       }

       public static implicit operator int(Money m)
       {
           return m.Amount;
       }
   }
   ```

   `600` が表示されます。`implicit` で定義した変換は、`(int)price` のようにキャストを書いても使えます。

</details>

---

## 次のステップ

[再帰関数とコールスタック](/unity-csharp-learning/csharp/recursion/) では、メソッドが自分自身を呼び出す再帰と、メソッドの呼び出しが積み重なるコールスタックを学びます。
