---
layout: page
title: プライマリコンストラクター（補足）
permalink: /csharp/primary-constructors/
---

# プライマリコンストラクター（補足）

C# 12 以降では、[record](/unity-csharp-learning/csharp/records/) の位置指定の構文と同じように、クラス名の後の丸かっこにパラメータを書けます。これを **プライマリコンストラクター**（primary constructor）といいます。書き方は record と同じですが、クラスのプライマリコンストラクターはプロパティを作りません。このページでは、プライマリコンストラクターの書き方と、record との違いを学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- プライマリコンストラクターを持つクラスを定義し、パラメータをメソッドの中で使える
- クラスのプライマリコンストラクターがプロパティを作らないことを、record と比べて説明できる
- パラメータとプロパティの両方に値が保存される問題（CS9124）を避けられる
- ほかのコンストラクターや派生クラスから、プライマリコンストラクターを呼び出せる

## 前提知識

- [record](/unity-csharp-learning/csharp/records/) を読んでいること
- [コンストラクター](/unity-csharp-learning/csharp/constructors/) を読んでいること（`: this(...)` を使う）
- [継承](/unity-csharp-learning/csharp/inheritance/) を読んでいること（`: base(...)` を使う）

---

## 1. フィールドに入れるだけのコンストラクター

コンストラクターで受け取った値を、フィールドに入れるだけのクラスはよくあります。

```csharp
Player p = new Player("Alice", 100);
p.TakeDamage(30);
p.TakeDamage(20);

class Player
{
    private string _name;
    private int _hp;

    public Player(string name, int hp)
    {
        _name = name;
        _hp = hp;
    }

    public void TakeDamage(int damage)
    {
        _hp -= damage;
        Console.WriteLine($"{_name} が {damage} ダメージ。残り HP={_hp}");
    }
}
```

```
Alice が 30 ダメージ。残り HP=70
Alice が 20 ダメージ。残り HP=50
```

`name` と `hp` は、フィールドの宣言・コンストラクターのパラメータ・コンストラクターの中の代入と、3 回ずつ書かれています。値を 1 つ増やすたびに、3 か所を直す必要があります。

---

## 2. プライマリコンストラクターの書き方

[プライマリコンストラクター](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/instance-constructors#primary-constructors) は、クラス名の後に `( )` でパラメータを書きます。

**書式：[プライマリコンストラクター](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/instance-constructors#primary-constructors)**
```
class クラス名(型1 パラメータ名1, 型2 パラメータ名2, ...)
{
    // メンバー
}
```

| 要素 | 説明 |
|---|---|
| `(型 パラメータ名, ...)` | コンストラクターのパラメータ。`new クラス名(引数, ...)` で値を渡す |

1 節の `Player` を、プライマリコンストラクターで書き換えます。

```csharp
Player p = new Player("Alice", 100);
p.TakeDamage(30);
p.TakeDamage(20);

class Player(string name, int hp)
{
    public void TakeDamage(int damage)
    {
        hp -= damage;
        Console.WriteLine($"{name} が {damage} ダメージ。残り HP={hp}");
    }
}
```

```
Alice が 30 ダメージ。残り HP=70
Alice が 20 ダメージ。残り HP=50
```

プライマリコンストラクターのパラメータは、クラスの中のどのメソッドからでも使えます。`TakeDamage` を 2 回呼ぶと、1 回目で減った `hp` から、2 回目でさらに減っています。パラメータの値は、インスタンスが存在する間、保存され続けるということです。メソッドで使われるパラメータは、コンパイラーが用意する隠れたフィールドに保存されます。1 節で自分で書いたフィールドを、コンパイラーが代わりに用意してくれると考えてください。

パラメータの名前は、ふつうのコンストラクターのパラメータと同じく、`name` のように小文字で始めます（camelCase）。

プライマリコンストラクターを持つクラスには、パラメータのない既定のコンストラクターは用意されません。`new Player()` と書くと、コンパイルエラー（CS7036）になります。

---

## 3. record との違い：プロパティは作られない

record の位置指定の構文では、丸かっこに書いた `X` や `Y` から、コンパイラーがプロパティを作りました。クラスのプライマリコンストラクターは、プロパティを作りません。パラメータは、クラスの外からは使えません。

```csharp
// ❌ NG: name はパラメータで、プロパティではない
// Player p = new Player("Alice", 100);
// Console.WriteLine(p.name);  // CS1061
//
// class Player(string name, int hp)
// {
//     public void TakeDamage(int damage)
//     {
//         hp -= damage;
//         Console.WriteLine($"{name} が {damage} ダメージ。残り HP={hp}");
//     }
// }
```

値をクラスの外に公開したいときは、パラメータでプロパティを初期化します。プロパティの初期値には、プライマリコンストラクターのパラメータを書けます。

```csharp
Player p = new Player("Alice", 100);
p.TakeDamage(30);
Console.WriteLine($"{p.Name}: HP={p.Hp}");

class Player(string name, int hp)
{
    public string Name { get; } = name;
    public int Hp { get; private set; } = hp;

    public void TakeDamage(int damage)
    {
        Hp -= damage;
        Console.WriteLine($"{Name} が {damage} ダメージ。残り HP={Hp}");
    }
}
```

```
Alice が 30 ダメージ。残り HP=70
Alice: HP=70
```

このクラスでは、`TakeDamage` の中で `name` と `hp` ではなく、`Name` と `Hp` を使っています。理由は、次の「よくあるミス」で説明します。

| | `record 型名(...)` | `class 型名(...)` |
|---|---|---|
| 丸かっこの中に書いたもの | プロパティ（`X`、`Y` のように PascalCase で書く） | パラメータ（`name`、`hp` のように camelCase で書く） |
| クラスの外から使えるか | 使える（`p.X`） | 使えない。使いたいときは、自分でプロパティを書く |
| `Equals`・`ToString` などのメンバー | コンパイラーが作る | 作られない |
| 作った後に値を変えられるか | 変えられない（`init`） | パラメータは、クラスの中から変えられる |

record は「値のまとまり」を表すための型で、丸かっこの中は外に見せる値です。クラスのプライマリコンストラクターは、コンストラクターの書き方を短くするだけで、外に何を見せるかはクラスの書き手が決めます。

---

## 4. ほかのコンストラクターを用意する

プライマリコンストラクターのほかに、コンストラクターを追加できます。ただし、追加したコンストラクターは、[コンストラクター](/unity-csharp-learning/csharp/constructors/) で学んだ `: this(...)` で、プライマリコンストラクターを呼び出さなければなりません。プライマリコンストラクターのパラメータに、必ず値が入るようにするためです。

```csharp
Player p1 = new Player("Alice", 80);
Player p2 = new Player("Bob");
Console.WriteLine($"{p1.Name}: HP={p1.Hp}");
Console.WriteLine($"{p2.Name}: HP={p2.Hp}");

class Player(string name, int hp)
{
    public string Name { get; } = name;
    public int Hp { get; } = hp;

    public Player(string name) : this(name, 100)
    {
    }
}
```

```
Alice: HP=80
Bob: HP=100
```

`new Player("Bob")` では、追加したコンストラクターが `: this(name, 100)` でプライマリコンストラクターを呼び出し、`hp` に `100` が入ります。

```csharp
// ❌ NG: 追加したコンストラクターが、プライマリコンストラクターを呼び出していない
// class Player(string name, int hp)
// {
//     public Player(string name)  // CS8862
//     {
//     }
// }
```

---

## 5. 継承と組み合わせる

プライマリコンストラクターを持つクラスを継承するときは、基底クラスの名前の後に `( )` で引数を書きます。[継承](/unity-csharp-learning/csharp/inheritance/) で学んだ `: base(...)` の代わりです。

```csharp
Player player = new Player("Alice", 100, 3);
Console.WriteLine($"{player.Name}: HP={player.Hp}, 回復薬={player.Potions}");

class Character(string name, int hp)
{
    public string Name { get; } = name;
    public int Hp { get; set; } = hp;
}

class Player(string name, int hp, int potions) : Character(name, hp)
{
    public int Potions { get; set; } = potions;
}
```

```
Alice: HP=100, 回復薬=3
```

これは、[継承](/unity-csharp-learning/csharp/inheritance/) の 4 節の `Character` と `Player` を、プライマリコンストラクターで書き換えたものです。`Player(string name, int hp, int potions)` で受け取った `name` と `hp` を、`: Character(name, hp)` で基底クラスのプライマリコンストラクターに渡しています。

---

## よくあるミス

### パラメータとプロパティの両方を使う

パラメータでプロパティを初期化したうえで、メソッドの中ではパラメータを使うと、値が 2 か所に保存されます。プロパティの値を変えても、パラメータの値は変わりません。

```csharp
// ⚠ 警告 CS9124: name の値が、Name プロパティと隠れたフィールドの 2 か所に保存される
Player p = new Player("Alice");
p.Name = "Bob";
Console.WriteLine(p.Name);
p.Greet();

class Player(string name)
{
    public string Name { get; set; } = name;

    public void Greet()
    {
        Console.WriteLine($"こんにちは、{name}です");
    }
}
```

```
Bob
こんにちは、Aliceです
```

`Name` を `"Bob"` に変えたのに、`Greet` では `Alice` と表示されています。`Name` プロパティの初期化に使った後も、`Greet` が `name` を使っているので、コンパイラーは `name` の値を隠れたフィールドにも保存します。`Name` を変えても、隠れたフィールドの値は `Alice` のままです。

コンパイラーは、このような書き方に警告 CS9124 を出します。パラメータでプロパティを初期化したら、メソッドの中ではプロパティを使います（`Greet` の中を `{Name}` にする）。3 節の `TakeDamage` で `Name` と `Hp` を使っていたのは、このためです。

派生クラスで、基底クラスに渡したパラメータをメソッドの中でも使うと、同じように値が 2 か所に保存され、警告 CS9107 が出ます。

---

## ワンポイントアドバイス

### 使ってよい場面

プライマリコンストラクターは、受け取った値をクラスの中で使うだけのクラスに向いています。たとえば、処理に必要なオブジェクトを受け取り、メソッドの中で使うクラスです。

一方、受け取った値を調べてから保存したい（HP が負なら 0 にする、など）ときは、プライマリコンストラクターには本体がないので、ふつうのコンストラクターのほうが書きやすくなります。

パラメータを使わないまま残すと、警告 CS9113 が出ます。

### 構造体のプライマリコンストラクター

プライマリコンストラクターは、構造体（`struct`）にも書けます。動きはクラスと同じで、プロパティは作られません。プロパティを作りたいときは、`record struct` を使います。

---

## まとめ

- C# 12 以降では、`class クラス名(パラメータ...)` でプライマリコンストラクターを書ける
- パラメータは、クラスの中のどのメソッドからでも使え、インスタンスが存在する間、値が保存される
- record と違い、クラスのプライマリコンストラクターはプロパティを作らない。外に公開したい値は、パラメータでプロパティを初期化する
- パラメータでプロパティを初期化したら、メソッドの中ではプロパティを使う。パラメータも使うと、値が 2 か所に保存される（CS9124）
- ほかのコンストラクターは、`: this(...)` でプライマリコンストラクターを呼び出す必要がある（CS8862）
- 継承するときは、`: 基底クラス名(引数)` で基底クラスのコンストラクターに値を渡す

---

## 理解度チェック

1. `record Point(int X, int Y);` と `class Point(int x, int y) { }` の違いを説明してください。
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   Counter c = new Counter(10);
   c.Add(5);
   c.Add(3);
   Console.WriteLine(c.Start);

   class Counter(int count)
   {
       public int Start { get; } = count;

       public void Add(int n)
       {
           count += n;
           Console.WriteLine(count);
       }
   }
   ```

3. 次のクラスに、名前だけを受け取り、`level` を `1` にするコンストラクターを追加してください。

   ```csharp
   class Hero(string name, int level)
   {
       public string Name { get; } = name;
       public int Level { get; } = level;
   }
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. `record Point(int X, int Y);` は、`X` と `Y` のプロパティと、`Equals`・`ToString` などのメンバーをコンパイラーが作ります。`class Point(int x, int y) { }` の `x` と `y` はコンストラクターのパラメータで、プロパティは作られず、クラスの外からは使えません。
2. 次のように出力されます。`Start` は、作ったときの `count` の値 `10` で初期化されます。`Add` は `count`（隠れたフィールド）を変えるので、`Start` は `10` のままです。このコードは、`count` を 2 か所に保存するので、警告 CS9124 が出ます。

   ```
   15
   18
   10
   ```

3. ```csharp
   Hero h = new Hero("Alice");
   Console.WriteLine($"{h.Name}: Lv.{h.Level}");

   class Hero(string name, int level)
   {
       public string Name { get; } = name;
       public int Level { get; } = level;

       public Hero(string name) : this(name, 1)
       {
       }
   }
   ```

   `Alice: Lv.1` が表示されます。追加したコンストラクターは、`: this(...)` でプライマリコンストラクターを呼び出す必要があります。

</details>

---

## 次のステップ

[列挙型](/unity-csharp-learning/csharp/enums/) では、関連する定数に名前を付けてまとめる `enum` を学びます。
