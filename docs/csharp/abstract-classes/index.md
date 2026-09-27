---
layout: page
title: 抽象クラスと抽象メソッド
permalink: /csharp/abstract-classes/
---

# 抽象クラスと抽象メソッド

**抽象クラス**（abstract class）は、インスタンスを作れず、継承されることを前提にしたクラスです。**抽象メソッド**（abstract method）は、本体を持たず、派生クラスで必ずオーバーライドしなければならないメソッドです。「形の面積を求める」のように、共通の操作として必ず用意したいが、中身は派生クラスごとに違う、という場面で使います。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `abstract class` を定義し、インスタンスを作れない理由を説明できる
- `abstract` メソッドを定義し、派生クラスでオーバーライドできる
- `virtual` と `abstract` の違いを説明できる

## 前提知識

- [メソッドの隠ぺいと sealed](/unity-csharp-learning/csharp/method-hiding/) を読んでいること

---

## 1. 抽象クラスが必要な場面

円や長方形などの図形を、基底クラス `Shape` から派生させるとします。どの図形も面積を求められるようにしたいので、`Shape` に `Area` メソッドを用意したいところです。しかし、「図形」というだけでは、面積の求め方は決まりません。

`virtual` のメソッドにすると、基底クラスにも何かの本体を書かなければなりません。また、派生クラスがオーバーライドを忘れても、コンパイラーは何も指摘しません。抽象メソッドを使うと、本体を書かずに、派生クラスにオーバーライドを強制できます。

---

## 2. 抽象クラスと抽象メソッドを定義する

**書式：[抽象クラスと抽象メソッド](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/abstract)**
```
abstract class クラス名
{
    アクセス修飾子 abstract 戻り値の型 メソッド名(パラメータ);
}
```

| 要素 | 説明 |
|---|---|
| `abstract class` | 抽象クラス。インスタンスを作れず、継承して使う |
| `abstract` のメソッド | 抽象メソッド。本体の `{ }` を書かず、`;` で終える。抽象クラスの中にだけ書ける |

抽象メソッドは、派生クラスで `override` を付けて、本体を定義します。

```csharp
Shape[] shapes = { new Circle(2), new Rectangle(3, 4) };

foreach (Shape s in shapes)
{
    s.Describe();
}

abstract class Shape
{
    public abstract string Name { get; }
    public abstract double Area();

    public void Describe()
    {
        Console.WriteLine($"{Name} の面積: {Area():0.00}");
    }
}

class Circle : Shape
{
    private double _radius;

    public Circle(double radius)
    {
        _radius = radius;
    }

    public override string Name
    {
        get { return "円"; }
    }

    public override double Area()
    {
        return Math.PI * _radius * _radius;
    }
}

class Rectangle : Shape
{
    private double _width;
    private double _height;

    public Rectangle(double width, double height)
    {
        _width = width;
        _height = height;
    }

    public override string Name
    {
        get { return "長方形"; }
    }

    public override double Area()
    {
        return _width * _height;
    }
}
```

```
円 の面積: 12.57
長方形 の面積: 12.00
```

- `Shape` の `Area` は抽象メソッドで、本体がありません。`Circle` と `Rectangle` が、それぞれの面積の求め方でオーバーライドしています
- プロパティも、`abstract` にして派生クラスでオーバーライドできます。ここでは `Name` を抽象プロパティにしています
- 抽象クラスには、ふつうのメソッドも書けます。`Describe` は、派生クラスがオーバーライドした `Name` と `Area` を使って、どの図形でも同じ形式で表示します
- `{Area():0.00}` の `0.00` は、小数点以下 2 桁で表示する書式です

### 抽象クラスのインスタンスは作れない

抽象クラスは、中身の決まっていないメソッドを持っているので、インスタンスを作れません。

```csharp
// ❌ NG: 抽象クラスのインスタンスは作れない
// Shape s = new Shape();  // CS0144
```

ただし、抽象クラスの型の変数や配列は作れます。上の例の `Shape[] shapes` のように、派生クラスのインスタンスを入れて使います。

---

## 3. virtual と abstract の違い

| | `virtual` | `abstract` |
|---|---|---|
| 本体 | 書く | 書かない |
| 派生クラスでのオーバーライド | してもしなくてもよい | 必ずする |
| 書ける場所 | ふつうのクラスと抽象クラス | 抽象クラスだけ |

`virtual` は「書き換えてもよい」、`abstract` は「必ず書き換えなければならない」を表します。

```csharp
Dog d = new Dog();
d.Speak();
d.Sleep();

abstract class Animal
{
    public abstract void Speak();

    public virtual void Sleep()
    {
        Console.WriteLine("すやすや");
    }
}

class Dog : Animal
{
    public override void Speak()
    {
        Console.WriteLine("ワン");
    }
}
```

```
ワン
すやすや
```

`Dog` は、抽象メソッドの `Speak` を必ずオーバーライドします。`virtual` の `Sleep` はオーバーライドしていないので、`Animal` の `Sleep` が使われます。

---

## よくあるミス

### 抽象メソッドをオーバーライドし忘れる

```csharp
// ❌ NG: Dog は Animal の抽象メソッド Speak をオーバーライドしていない
// abstract class Animal
// {
//     public abstract void Speak();
// }
//
// class Dog : Animal  // CS0534
// {
// }
```

抽象ではないクラスは、基底クラスのすべての抽象メソッドをオーバーライドする必要があります。オーバーライドし忘れると、コンパイラーが教えてくれます。これが、抽象メソッドを使う利点です。

### 抽象メソッドに本体を書く

```csharp
// ❌ NG: 抽象メソッドには本体を書けない
// abstract class Animal
// {
//     public abstract void Speak() { }  // CS0500
// }
```

### 抽象ではないクラスに抽象メソッドを書く

```csharp
// ❌ NG: 抽象メソッドは、抽象クラスの中にだけ書ける
// class Animal
// {
//     public abstract void Speak();  // CS0513
// }
```

---

## まとめ

- `abstract class` は、インスタンスを作れず、継承して使うクラス
- 抽象メソッドは本体を持たず、抽象ではない派生クラスで必ずオーバーライドする
- 抽象クラスには、ふつうのメソッドやフィールドも書ける
- 抽象クラスの型の変数や配列には、派生クラスのインスタンスを入れられる
- `virtual` はオーバーライドしてもしなくてもよく、`abstract` は必ずオーバーライドする

---

## 理解度チェック

1. 抽象クラスのインスタンスを `new` で作ろうとすると、どうなりますか？
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   Greeter[] greeters = { new Japanese(), new English() };
   foreach (Greeter g in greeters)
   {
       g.Greet();
   }

   abstract class Greeter
   {
       protected abstract string Hello();

       public void Greet()
       {
           Console.WriteLine($"{Hello()}!");
       }
   }

   class Japanese : Greeter
   {
       protected override string Hello() { return "こんにちは"; }
   }

   class English : Greeter
   {
       protected override string Hello() { return "Hello"; }
   }
   ```

3. `virtual` のメソッドと `abstract` のメソッドの違いを、オーバーライドの観点から説明してください。

<details markdown="1">
<summary>解答を見る</summary>

1. コンパイルエラー（CS0144）になります。
2. 次のように出力されます。`Greet` は `Greeter` のメソッドですが、中で呼び出す `Hello` は、実体の型でオーバーライドされたメソッドが実行されます。

   ```
   こんにちは!
   Hello!
   ```

3. `virtual` のメソッドは、派生クラスでオーバーライドしてもしなくてもかまいません。`abstract` のメソッドは、抽象ではない派生クラスで必ずオーバーライドしなければならず、しないとコンパイルエラー（CS0534）になります。

</details>

---

## 次のステップ

[インターフェイス](/unity-csharp-learning/csharp/interfaces/) では、クラスが持つべきメンバーだけを宣言する、もう 1 つの仕組みを学びます。
