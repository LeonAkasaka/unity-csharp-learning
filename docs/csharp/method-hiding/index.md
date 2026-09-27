---
layout: page
title: メソッドの隠ぺいと sealed
permalink: /csharp/method-hiding/
---

# メソッドの隠ぺいと sealed

派生クラスに、基底クラスと同じ名前のメソッドを定義すると、`override` を付けなければ、基底クラスのメソッドを書き換えずに **隠す** ことになります。このページでは、メソッドが隠されるとどうなるかと、隠すことを明示する `new` 修飾子、オーバーライドとの違いを学びます。また、それ以上の継承やオーバーライドを禁止する `sealed` も学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- 基底クラスと同じ名前のメソッドを定義すると、メソッドが隠されることを説明できる
- `new` 修飾子で、メソッドを隠すことを明示できる
- 基底クラスの型の変数から呼び出したとき、`new` と `override` で結果が違うことを説明できる
- `sealed class` と `sealed override` で、継承やオーバーライドを禁止できる

## 前提知識

- [オーバーライドとポリモーフィズム](/unity-csharp-learning/csharp/polymorphism/) を読んでいること

---

## 1. 基底クラスと同じ名前のメソッド

基底クラスと派生クラスを、別々の人が作ることがあります。たとえば、`Character` クラスはライブラリとして提供されていて、自分では変更できないとします。自分は `Character` を継承して `Player` を作り、回復薬を使う `Heal` メソッドを追加しました。

その後、ライブラリが新しくなり、`Character` にも `Heal` メソッドが追加されました。`Character` の `Heal` は、`virtual` ではありません。自分の `Player` の `Heal` と、名前もパラメータも同じです。

```csharp
Player player = new Player("Alice");
player.Heal();

Character c = player;
c.Heal();

// ライブラリの Character（後から Heal が追加された）
class Character
{
    public string Name { get; }

    public Character(string name)
    {
        Name = name;
    }

    public void Heal()
    {
        Console.WriteLine($"{Name} は休んで HP を 10 回復した");
    }
}

// 自分で作った Player
class Player : Character
{
    public Player(string name) : base(name) { }

    public void Heal()
    {
        Console.WriteLine($"{Name} は回復薬で HP を 50 回復した");
    }
}
```

```
Alice は回復薬で HP を 50 回復した
Alice は休んで HP を 10 回復した
```

`Character` の `Heal` は `virtual` ではないので、`Player` の `Heal` はオーバーライドにはなりません。`Player` の `Heal` は、`Character` の `Heal` とは別のメソッドとして、`Character` の `Heal` を **隠し**（hide）ます。

隠したメソッドは、変数の型によって、どちらが実行されるかが決まります。

- `Player` の型の変数 `player` から呼び出すと、`Player` の `Heal` が実行される
- `Character` の型の変数 `c` から呼び出すと、実体は `Player` でも、`Character` の `Heal` が実行される

同じインスタンスなのに、どの型の変数から呼び出すかで動作が変わるのは、まぎらわしい状態です。そのため、コンパイラーは、このコードに警告（CS0108）を出します。「基底クラスの `Heal` を隠している。意図して隠すなら `new` を付けること」という内容です。

---

## 2. new 修飾子で隠すことを明示する

基底クラスのメソッドを意図して隠すときは、派生クラスのメソッドに [new 修飾子](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/new-modifier) を付けます。インスタンスを作る `new` 演算子とは、同じキーワードですが別の機能です。

**書式：[new 修飾子](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/new-modifier)**
```
アクセス修飾子 new 戻り値の型 メソッド名(パラメータ)
{
    // 処理
}
```

1 節の `Player` の `Heal` に `new` を付けます。

```csharp
class Player : Character
{
    public Player(string name) : base(name) { }

    public new void Heal()
    {
        Console.WriteLine($"{Name} は回復薬で HP を 50 回復した");
    }
}
```

`new` を付けても、動作は 1 節と変わりません。変わるのは、コンパイラーの警告が消えることです。`new` は、「基底クラスに同じメソッドがあることを知ったうえで、隠している」ことを、コンパイラーとコードを読む人に伝えるためのものです。

ただし、隠すことは、1 節のようなまぎらわしさを残します。自分のメソッドの名前を変えられるなら（たとえば `UsePotion` にする）、名前を変えるほうがわかりやすくなります。`new` は、名前を変えられない事情があるときなどに限って使います。

---

## 3. new と override の違い

`new` と `override` の違いが表れるのは、**基底クラスの型の変数から呼び出したとき** です。`Character` に、`virtual` の `Attack` と、`virtual` でない `Greet` を用意します。`Warrior` は、`Attack` を `override` で書き換え、`Greet` を `new` で隠します。

```csharp
Warrior w = new Warrior("Alice");
Character c = w;

w.Attack();
c.Attack();

w.Greet();
c.Greet();

class Character
{
    public string Name { get; }

    public Character(string name)
    {
        Name = name;
    }

    public virtual void Attack()
    {
        Console.WriteLine($"{Name} の攻撃");
    }

    public void Greet()
    {
        Console.WriteLine($"{Name}: よろしく");
    }
}

class Warrior : Character
{
    public Warrior(string name) : base(name) { }

    public override void Attack()
    {
        Console.WriteLine($"{Name} は剣で斬りつけた（override）");
    }

    public new void Greet()
    {
        Console.WriteLine($"{Name}: 剣なら任せて（new）");
    }
}
```

```
Alice は剣で斬りつけた（override）
Alice は剣で斬りつけた（override）
Alice: 剣なら任せて（new）
Alice: よろしく
```

- `Attack` はオーバーライドなので、`Character` の型の変数 `c` から呼び出しても、実体の `Warrior` の `Attack` が実行されます
- `Greet` は隠しているだけなので、`Character` の型の変数 `c` から呼び出すと、`Character` の `Greet` が実行されます

| 変数の型 | 実体 | `override` の場合 | `new` の場合 |
|---|---|---|---|
| 基底クラス | 派生クラス | 派生クラスのメソッド | 基底クラスのメソッド |
| 派生クラス | 派生クラス | 派生クラスのメソッド | 派生クラスのメソッド |

`override` では実体の型でメソッドが決まり、`new` では変数の型でメソッドが決まります。[オーバーライドとポリモーフィズム](/unity-csharp-learning/csharp/polymorphism/) で学んだように、基底クラスの型の配列にまとめて同じ書き方で扱えるのは、`override` のときだけです。種類ごとに動作を変えたいなら、`override` を使います。

---

## 4. sealed class

継承は便利ですが、どのクラスでも継承してよいわけではありません。派生クラスは基底クラスの `protected` のメンバーを使い、`virtual` のメソッドを書き換えられます。継承されることを想定して作られていないクラスを継承されると、基底クラスの中の処理が、作った人の意図しない形で変えられてしまうことがあります。また、基底クラスを変更したときに、どこかの派生クラスが壊れる心配も生まれます。

クラスに [sealed](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/sealed) を付けると、そのクラスを継承できなくなります。

**書式：[sealed class](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/sealed)**
```
sealed class クラス名
{
    // メンバー
}
```

```csharp
GameSettings s = new GameSettings();
s.Show();

sealed class GameSettings
{
    public int Volume { get; set; } = 80;

    public void Show()
    {
        Console.WriteLine($"音量: {Volume}");
    }
}
```

```
音量: 80
```

`GameSettings` のインスタンスは、ふつうのクラスと同じように作って使えます。継承しようとすると、コンパイルエラーになります。

```csharp
// ❌ NG: sealed のクラスは継承できない（CS0509）
// class MySettings : GameSettings { }
```

継承されることを想定していないクラスに `sealed` を付けておくと、意図しない派生クラスが作られるのを防げます。.NET の `string` も `sealed` のクラスです。

---

## 5. sealed override

クラス全体ではなく、特定のメソッドのオーバーライドだけを止めたいこともあります。`override` に `sealed` を付けると、そのメソッドを、さらに派生したクラスでオーバーライドできなくなります。

騎士 `Knight` の攻撃は、「ふつうの攻撃の後に盾で押し返す」ものと決めて、`Knight` の派生クラスには変えさせないようにします。

**書式：[sealed override](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/sealed)**
```
アクセス修飾子 sealed override 戻り値の型 メソッド名(パラメータ)
{
    // 処理
}
```

```csharp
Character c = new HolyKnight("Alice");
c.Attack();

class Character
{
    public string Name { get; }

    public Character(string name)
    {
        Name = name;
    }

    public virtual void Attack()
    {
        Console.WriteLine($"{Name} の攻撃");
    }
}

class Knight : Character
{
    public Knight(string name) : base(name) { }

    public sealed override void Attack()
    {
        base.Attack();
        Console.WriteLine("さらに盾で押し返した");
    }
}

class HolyKnight : Knight
{
    public HolyKnight(string name) : base(name) { }
}
```

```
Alice の攻撃
さらに盾で押し返した
```

聖騎士 `HolyKnight` は `Knight` を継承できて、`Knight` の `Attack` をそのまま引き継いでいます。しかし、`Knight` の `Attack` は `sealed override` なので、`HolyKnight` で `Attack` をオーバーライドすることはできません。

```csharp
// ❌ NG: sealed override のメソッドは、それ以上オーバーライドできない
// class HolyKnight : Knight
// {
//     public HolyKnight(string name) : base(name) { }
//
//     public override void Attack() { }  // CS0239
// }
```

---

## まとめ

- 派生クラスに基底クラスと同じメソッドを `override` なしで定義すると、基底クラスのメソッドを隠す。`new` を付けないと、コンパイラーが警告（CS0108）を出す
- `new` 修飾子は、基底クラスのメソッドを意図して隠すことを明示する
- 隠したメソッドは変数の型で、オーバーライドしたメソッドは実体の型で、実行されるメソッドが決まる
- 同じシグネチャのメソッドを定義するときは、`override` か `new` を明示する。種類ごとに動作を変えたいなら `override` を使う
- `sealed class` は、継承できないクラスになる
- `sealed override` のメソッドは、それ以上オーバーライドできない

---

## 理解度チェック

1. `new` で定義したメソッドを、基底クラスの型の変数から呼び出すと、どのメソッドが実行されますか？
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   A a = new B();
   B b = new B();
   a.M();
   b.M();
   a.N();
   b.N();

   class A
   {
       public void M() { Console.WriteLine("A.M"); }
       public virtual void N() { Console.WriteLine("A.N"); }
   }

   class B : A
   {
       public new void M() { Console.WriteLine("B.M"); }
       public override void N() { Console.WriteLine("B.N"); }
   }
   ```

3. 派生クラスで、基底クラスと同じ名前・同じパラメータのメソッドを、`override` も `new` も付けずに定義すると、どうなりますか？
4. `sealed class` とは、どのようなクラスですか？

<details markdown="1">
<summary>解答を見る</summary>

1. 基底クラスのメソッドが実行されます。`new` では、変数の型によって実行されるメソッドが決まります。
2. 次のように出力されます。`M` は `new` なので変数の型で、`N` は `override` なので実体の型でメソッドが決まります。

   ```
   A.M
   B.M
   B.N
   B.N
   ```

3. 基底クラスのメソッドを隠すことになります（`new` を付けたときと同じ動作）。コンパイラーは、基底クラスのメソッドが `virtual` でなければ警告 CS0108 を、`virtual` なら警告 CS0114 を出します。
4. 継承できないクラスです。`sealed` のクラスを基底クラスにしようとすると、コンパイルエラー（CS0509）になります。

</details>

---

## 次のステップ

[抽象クラスと抽象メソッド](/unity-csharp-learning/csharp/abstract-classes/) では、インスタンスを作れず、派生クラスにメソッドの定義を強制するクラスを学びます。
