---
layout: page
title: static メンバーと static クラス
permalink: /csharp/static-members/
---

# static メンバーと static クラス

クラスのメンバーには、インスタンスごとに別々に持つもの（**インスタンスメンバー**）と、クラスそのものに属し、すべてのインスタンスで共有するもの（**static メンバー**）があります。[static](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/static) は、「インスタンスではなく、クラスに属する」ことを表すキーワードです。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- インスタンスメンバーと static メンバーの違いを説明できる
- static フィールドと static メソッドを定義し、使える
- static コンストラクターが 1 回だけ実行されることを説明できる
- static クラスを定義し、インスタンスを作れないことを説明できる

## 前提知識

- [コンストラクター](/unity-csharp-learning/csharp/constructors/) を読んでいること
- [メソッド](/unity-csharp-learning/csharp/methods/) を読んでいること

---

## 1. インスタンスメンバーと static メンバー

これまでに定義したフィールドやメソッドは、すべて **インスタンスメンバー** です。インスタンスメンバーは、インスタンスごとに別々にあり、`インスタンス.メンバー名` で使います。

**static メンバー** は、クラスに 1 つだけあり、すべてのインスタンスで共有されます。`クラス名.メンバー名` で使い、インスタンスを作らなくても使えます。

| | インスタンスメンバー | static メンバー |
|---|---|---|
| 属する先 | それぞれのインスタンス | クラス |
| 数 | インスタンスの数だけある | クラスに 1 つだけ |
| 使い方 | `インスタンス.メンバー名` | `クラス名.メンバー名` |

これまで使ってきた `Console.WriteLine` や `Array.Sort` も、static メソッドです。インスタンスを作らずに、`Console.WriteLine(...)` のようにクラス名から呼び出していました。

---

## 2. static フィールド

**書式：[static フィールド](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/static-classes-and-static-class-members)**
```
アクセス修飾子 static 型 フィールド名;
```

次の `Player` クラスでは、`Name` はインスタンスごとの値、`Count` は作ったプレイヤーの数を数える、全員で共有する値です。

```csharp
Player a = new Player("Alice");
Player b = new Player("Bob");

Console.WriteLine($"{a.Name}, {b.Name}");
Console.WriteLine($"プレイヤーの数: {Player.Count}");

class Player
{
    public static int Count;

    public string Name;

    public Player(string name)
    {
        Name = name;
        Count++;
    }
}
```

```
Alice, Bob
プレイヤーの数: 2
```

`Name` は `a` と `b` でそれぞれ別の値を持ちますが、`Count` はクラスに 1 つだけです。コンストラクターで `Count++` するたびに、同じ `Count` が増えていきます。static フィールドは、`Player.Count` のようにクラス名で使います。

---

## 3. static メソッド

**書式：[static メソッド](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/static-classes-and-static-class-members)**
```
アクセス修飾子 static 戻り値の型 メソッド名(パラメータ)
{
    // 処理
}
```

static メソッドは、`クラス名.メソッド名()` で呼び出します。インスタンスのデータを使わず、引数だけで結果が決まる処理に向いています。

```csharp
Console.WriteLine(Calc.Square(4));
Console.WriteLine(Calc.Max(3, 8));

class Calc
{
    public static int Square(int x)
    {
        return x * x;
    }

    public static int Max(int a, int b)
    {
        return a > b ? a : b;
    }
}
```

```
16
8
```

static メソッドは、特定のインスタンスに属していないので、本体でインスタンスメンバーを使えません。static メソッドの本体で使えるのは、static メンバー、パラメータ、ローカル変数です（よくあるミスを参照）。

---

## 4. static コンストラクター

**書式：[static コンストラクター](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/static-constructors)**
```
static クラス名()
{
    // 初期化の処理
}
```

**static コンストラクター** は、static フィールドを初期化するためのコンストラクターです。そのクラスが初めて使われる前に、**1 回だけ** 自動的に実行されます。パラメータとアクセス修飾子は書けません。

```csharp
Console.WriteLine("プログラム開始");
Console.WriteLine(Config.Version);
Console.WriteLine(Config.Version);

class Config
{
    public static string Version;

    static Config()
    {
        Console.WriteLine("Config の static コンストラクター");
        Version = "1.0";
    }
}
```

```
プログラム開始
Config の static コンストラクター
1.0
1.0
```

`Config.Version` を 2 回使っていますが、static コンストラクターが実行されたのは、最初に使う直前の 1 回だけです。

---

## 5. static クラス

クラスに `static` を付けると、**static クラス** になります。static クラスは、インスタンスを作れず、static メンバーだけを持てます。インスタンスのデータを持たない、便利なメソッドをまとめるのに使います。.NET の `Console` や `Math` も static クラスです。

**書式：[static クラス](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/static-classes-and-static-class-members)**
```
static class クラス名
{
    // static メンバーだけを書く
}
```

```csharp
Console.WriteLine(Temperature.ToFahrenheit(100));
Console.WriteLine(Temperature.ToCelsius(32));

static class Temperature
{
    public static double ToFahrenheit(double celsius)
    {
        return celsius * 9 / 5 + 32;
    }

    public static double ToCelsius(double fahrenheit)
    {
        return (fahrenheit - 32) * 5 / 9;
    }
}
```

```
212
0
```

```csharp
// ❌ NG: static クラスのインスタンスは作れない
// Temperature t = new Temperature();  // CS0712
```

---

## よくあるミス

### static メソッドの中でインスタンスメンバーを使う

```csharp
// ❌ NG: static メソッドは、どのインスタンスの Name かわからない
// class Player
// {
//     public string Name = "";
//
//     public static void Show()
//     {
//         Console.WriteLine(Name);  // CS0120
//     }
// }
```

static メソッドは、インスタンスを作らずに `Player.Show()` と呼び出します。そのため、「どのインスタンスの `Name` か」が決まらず、インスタンスメンバーは使えません。`this` も使えません（CS0026）。インスタンスのデータが必要なら、インスタンスメソッドにするか、インスタンスを引数で受け取ります。

### static クラスにインスタンスメンバーを書く

```csharp
// ❌ NG: static クラスには static メンバーしか書けない
// static class Calc
// {
//     public int Double(int x)  // CS0708
//     {
//         return x * 2;
//     }
// }
```

### static コンストラクターにアクセス修飾子を付ける

```csharp
// ❌ NG: static コンストラクターにはアクセス修飾子を付けられない
// class Config
// {
//     public static Config()  // CS0515
//     {
//     }
// }
```

---

## まとめ

- インスタンスメンバーはインスタンスごとにあり、static メンバーはクラスに 1 つだけあって共有される
- static メンバーは `クラス名.メンバー名` で使う。インスタンスを作らなくても使える
- static メソッドの中では、インスタンスメンバーや `this` を使えない
- static コンストラクターは、クラスが初めて使われる前に 1 回だけ実行される
- static クラスはインスタンスを作れず、static メンバーだけを持つ

---

## 理解度チェック

1. インスタンスメンバーと static メンバーの違いを説明してください。
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   A.M();
   A a1 = new A();
   A a2 = new A();
   A.M();

   class A
   {
       public static int Count = 1;

       public A()
       {
           Count++;
       }

       public static void M()
       {
           Console.WriteLine($"A.M: Count={Count}");
       }
   }
   ```

3. 円の半径から面積を返す static メソッド `Area(double radius)` を持つ static クラス `Circle` を書いてください。円周率には `Math.PI` を使います。

<details markdown="1">
<summary>解答を見る</summary>

1. インスタンスメンバーはインスタンスごとに別々にあり、`インスタンス.メンバー名` で使います。static メンバーはクラスに 1 つだけあってすべてのインスタンスで共有され、`クラス名.メンバー名` で使います。
2. 次のように出力されます。`Count` はクラスに 1 つだけで、2 つのインスタンスを作るたびに増えます。

   ```
   A.M: Count=1
   A.M: Count=3
   ```

3. ```csharp
   Console.WriteLine(Circle.Area(2));

   static class Circle
   {
       public static double Area(double radius)
       {
           return Math.PI * radius * radius;
       }
   }
   ```

   `12.566370614359172` が表示されます。

</details>

---

## 次のステップ

[const と readonly（補足）](/unity-csharp-learning/csharp/const-readonly/) では、値を変えられない定数と、作った後に変えられない読み取り専用フィールドを学びます。
