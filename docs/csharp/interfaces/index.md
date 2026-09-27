---
layout: page
title: インターフェイス
permalink: /csharp/interfaces/
---

# インターフェイス

**インターフェイス**（interface）は、クラスが持つべきメソッドやプロパティの **宣言だけ** をまとめた型です。インターフェイスを実装したクラスは、宣言されたメンバーを必ず持つので、「このクラスには、こういう操作ができる」という約束（契約）を表せます。クラスは基底クラスを 1 つしか継承できませんが、インターフェイスはいくつでも実装できます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- インターフェイスを宣言し、クラスで実装できる
- インターフェイスの型の変数から、実装したクラスのメソッドを呼び出せる
- 1 つのクラスで、複数のインターフェイスを実装できる
- 抽象クラスとインターフェイスの違いを説明できる

## 前提知識

- [抽象クラスと抽象メソッド](/unity-csharp-learning/csharp/abstract-classes/) を読んでいること

---

## 1. インターフェイスを宣言する

**書式：[インターフェイスの宣言](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/interface)**
```
interface インターフェイス名
{
    戻り値の型 メソッド名(パラメータ);
    型 プロパティ名 { get; }
}
```

| 要素 | 説明 |
|---|---|
| `interface` | インターフェイスを宣言するキーワード |
| `インターフェイス名` | `I` で始めるのが慣例（例：`IAttackable`、`IDisposable`） |
| メンバーの宣言 | 本体は書かず、`;` で終える。アクセス修飾子も書かない |

```csharp
interface IAttackable
{
    string Name { get; }
    void Attack();
}
```

`IAttackable` は、「`Name` プロパティと `Attack` メソッドを持つ」ことだけを決めています。何をするかは、実装するクラスが決めます。

---

## 2. インターフェイスを実装する

クラス名の後に `: インターフェイス名` と書くと、そのクラスはインターフェイスを **実装** します。実装したクラスは、インターフェイスのすべてのメンバーを `public` で定義しなければなりません。

**書式：インターフェイスの実装**
```
class クラス名 : インターフェイス名
{
    // インターフェイスのメンバーを public で定義する
}
```

```csharp
IAttackable[] attackers = { new Warrior(), new Turret() };

foreach (IAttackable a in attackers)
{
    Console.Write($"{a.Name}: ");
    a.Attack();
}

interface IAttackable
{
    string Name { get; }
    void Attack();
}

class Warrior : IAttackable
{
    public string Name
    {
        get { return "戦士"; }
    }

    public void Attack()
    {
        Console.WriteLine("剣で斬りつけた");
    }
}

class Turret : IAttackable
{
    public string Name
    {
        get { return "砲台"; }
    }

    public void Attack()
    {
        Console.WriteLine("弾を撃った");
    }
}
```

```
戦士: 剣で斬りつけた
砲台: 弾を撃った
```

`Warrior` と `Turret` は、継承の関係がない別々のクラスです。それでも、どちらも `IAttackable` を実装しているので、`IAttackable` の型の配列にまとめ、同じ書き方で `Attack` を呼び出せます。[オーバーライドとポリモーフィズム](/unity-csharp-learning/csharp/polymorphism/) と同じく、実体の型のメソッドが実行されます。

インターフェイスのメンバーを 1 つでも定義し忘れると、コンパイルエラー（CS0535）になります。

---

## 3. 複数のインターフェイスを実装する

クラスは、`,` で区切って、複数のインターフェイスを実装できます。基底クラスも継承するときは、基底クラスを最初に書きます。

**書式：基底クラスの継承と、複数のインターフェイスの実装**
```
class クラス名 : 基底クラス名, インターフェイス名1, インターフェイス名2
{
}
```

```csharp
Player p = new Player("Alice");
p.Attack();
p.Heal();

IAttackable attacker = p;
IHealable healer = p;
Console.WriteLine($"{attacker.Name} / {healer.Name}");

interface IAttackable
{
    string Name { get; }
    void Attack();
}

interface IHealable
{
    string Name { get; }
    void Heal();
}

class Character
{
    public string Name { get; }

    public Character(string name)
    {
        Name = name;
    }
}

class Player : Character, IAttackable, IHealable
{
    public Player(string name) : base(name)
    {
    }

    public void Attack()
    {
        Console.WriteLine($"{Name} の攻撃");
    }

    public void Heal()
    {
        Console.WriteLine($"{Name} の回復");
    }
}
```

```
Alice の攻撃
Alice の回復
Alice / Alice
```

- `Player` は、`Character` を継承し、`IAttackable` と `IHealable` を実装しています
- インターフェイスの `Name` プロパティは、基底クラス `Character` から引き継いだ `Name` で実装されています
- `Player` のインスタンスは、`IAttackable` の型の変数にも、`IHealable` の型の変数にも入れられます

---

## 4. 抽象クラスとインターフェイスの違い

| | 抽象クラス | インターフェイス |
|---|---|---|
| 継承・実装できる数 | 1 つだけ | いくつでも |
| フィールド | 持てる | 持てない（CS0525） |
| コンストラクター | 持てる | 持てない |
| メソッドの本体 | 書ける | ふつうは書かない（ワンポイントアドバイスを参照） |
| インスタンスの作成 | できない | できない |

抽象クラスは、「共通のデータや処理を持つ、同じ種類のクラスの基底」に向いています。インターフェイスは、「種類の違うクラスに、共通の操作を持たせる」のに向いています。上の例の戦士と砲台のように、継承の関係がないクラスにも同じ操作を持たせられるのが、インターフェイスの強みです。

.NET にも、多くのインターフェイスが用意されています。たとえば、[IDisposable と using](/unity-csharp-learning/csharp/dispose-using/) で学ぶ `IDisposable` は、「後片付けの `Dispose` メソッドを持つ」ことを表すインターフェイスです。

---

## よくあるミス

### インターフェイスのメンバーを実装し忘れる

```csharp
// ❌ NG: IAttackable の Attack を実装していない
// class Warrior : IAttackable  // CS0535
// {
//     public string Name
//     {
//         get { return "戦士"; }
//     }
// }
```

### 実装したメンバーに public を付け忘れる

```csharp
// ❌ NG: インターフェイスを実装するメンバーは public にする
// class Warrior : IAttackable
// {
//     public string Name
//     {
//         get { return "戦士"; }
//     }
//
//     void Attack()  // CS0737
//     {
//     }
// }
```

アクセス修飾子を省略すると `private` になるので、インターフェイスのメンバーの実装として使えません。

---

## ワンポイントアドバイス

### インターフェイスの既定の実装（C# 8 以降）

C# 8 以降では、インターフェイスのメソッドに本体を書いて、**既定の実装** を持たせることもできます。インターフェイスにメソッドを追加したいが、すでにあるクラスをすべて書き換えるのは難しい、という場面のための機能です。ふだんは、インターフェイスには宣言だけを書き、本体は実装するクラスに書きます。

---

## まとめ

- インターフェイスは、クラスが持つべきメンバーの宣言だけをまとめた型。名前は `I` で始めるのが慣例
- `class クラス名 : インターフェイス名` で実装し、すべてのメンバーを `public` で定義する
- インターフェイスの型の変数から呼び出すと、実体のクラスのメソッドが実行される
- クラスは、基底クラスは 1 つだけ継承でき、インターフェイスはいくつでも実装できる
- 継承の関係がないクラスにも、インターフェイスで共通の操作を持たせられる

---

## 理解度チェック

1. インターフェイスが「契約」と呼ばれるのはなぜですか？
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   IShape[] shapes = { new Square(3), new Square(5) };
   int total = 0;
   foreach (IShape s in shapes)
   {
       total += s.Area();
   }
   Console.WriteLine(total);

   interface IShape
   {
       int Area();
   }

   class Square : IShape
   {
       private int _side;

       public Square(int side)
       {
           _side = side;
       }

       public int Area()
       {
           return _side * _side;
       }
   }
   ```

3. 抽象クラスとインターフェイスの違いを、2 つ挙げてください。

<details markdown="1">
<summary>解答を見る</summary>

1. インターフェイスを実装したクラスは、宣言されたメンバーを必ず持たなければならず、その約束が守られているかをコンパイラーが確かめるからです。
2. `34` が出力されます。`3 × 3 = 9` と `5 × 5 = 25` の合計です。

   ```
   34
   ```

3. （例）抽象クラスは 1 つしか継承できないが、インターフェイスはいくつでも実装できる。抽象クラスはフィールドを持てるが、インターフェイスは持てない。

</details>

---

## 次のステップ

[インターフェイスの明示的実装](/unity-csharp-learning/csharp/explicit-interface/) では、複数のインターフェイスに同じ名前のメンバーがあるときに、それぞれ別の実装を用意する方法を学びます。
