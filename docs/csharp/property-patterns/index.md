---
layout: page
title: プロパティパターン
permalink: /csharp/property-patterns/
---

# プロパティパターン

**プロパティパターン**（property pattern）は、オブジェクトのプロパティの値を、パターンで調べる書き方です。`null` のチェックや、同じ変数を何度も書く手間を省いて、「オブジェクトがどんな状態か」を 1 つの条件として書けます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- プロパティパターンで、オブジェクトのプロパティの値を調べられる
- 入れ子になったオブジェクトのプロパティを、`null` のチェックを書かずに調べられる
- 型パターンとプロパティパターンを組み合わせて、型と中身を一度に調べられる
- `var` パターンで、調べた値を変数に取り出せる
- `is { } 変数名` で、`null` でない値を `?` の付かない型の変数に取り出せる
- `is` と `switch` 式の両方で、プロパティパターンを使える

## 前提知識

- [switch 式](/unity-csharp-learning/csharp/switch-expressions/) を読んでいること
- [null 許容値型](/unity-csharp-learning/csharp/nullable-value-types/) を読んでいること
- [型変換と型チェック](/unity-csharp-learning/csharp/type-casting/) を読んでいること

---

## 1. 入れ子のプロパティを調べる手間

このページでは、次のクラスを使います。[型変換と型チェック](/unity-csharp-learning/csharp/type-casting/) の `Character`・`Player`・`Enemy` に、武器を表す `Weapon` を持たせたものです。武器を持っていないときは、`Weapon` が `null` になります。

```csharp
Character[] characters =
{
    new Player("Alice", 100, new Weapon("聖剣", 90)),
    new Player("Bob", 80, null),
    new Enemy("Slime", 20, new Weapon("牙", 5), false),
    new Enemy("Dragon", 300, new Weapon("炎", 120), true),
};

foreach (Character c in characters)
{
    if (c.Weapon != null && c.Weapon.Power >= 50)
    {
        Console.WriteLine($"{c.Name} は強い武器を持っている");
    }
}

class Weapon
{
    public string Name { get; }
    public int Power { get; }

    public Weapon(string name, int power)
    {
        Name = name;
        Power = power;
    }
}

class Character
{
    public string Name { get; }
    public int Hp { get; set; }
    public Weapon? Weapon { get; set; }

    public Character(string name, int hp, Weapon? weapon)
    {
        Name = name;
        Hp = hp;
        Weapon = weapon;
    }
}

class Player : Character
{
    public Player(string name, int hp, Weapon? weapon) : base(name, hp, weapon)
    {
    }
}

class Enemy : Character
{
    public bool IsBoss { get; }

    public Enemy(string name, int hp, Weapon? weapon, bool isBoss) : base(name, hp, weapon)
    {
        IsBoss = isBoss;
    }
}
```

```
Alice は強い武器を持っている
Dragon は強い武器を持っている
```

武器の攻撃力を調べる条件 `c.Weapon.Power >= 50` の前に、`c.Weapon != null` を書く必要があります。書き忘れると、`Bob` のところで `NullReferenceException` が発生します。調べたいのは「強い武器を持っているか」という 1 つのことなのに、`c.Weapon` を 2 回書き、`null` のチェックも自分で書いています。

プロパティパターンを使うと、この条件を次のように書けます。

```
c is { Weapon.Power: >= 50 }
```

---

## 2. プロパティパターンの書き方

**書式：[プロパティパターン](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/patterns#property-pattern)**
```
{ プロパティ名1: パターン1, プロパティ名2: パターン2 }
```

| 要素 | 説明 |
|---|---|
| `{ }` | 値が `null` でなく、中に書いたすべての条件に当てはまるときに当てはまる |
| `プロパティ名: パターン` | そのプロパティの値が、パターンに当てはまるかを調べる。フィールドも調べられる |

`:` の後ろには、[パターンと is 演算子](/unity-csharp-learning/csharp/is-patterns/) で学んだどのパターンも書けます。条件を `,` で並べると、すべてに当てはまるときだけ当てはまります。

```csharp
Character[] characters =
{
    new Player("Alice", 100, new Weapon("聖剣", 90)),
    new Player("Bob", 80, null),
    new Enemy("Slime", 20, new Weapon("牙", 5), false),
    new Enemy("Dragon", 300, new Weapon("炎", 120), true),
};

foreach (Character c in characters)
{
    if (c is { Hp: >= 80 and < 200, Weapon: null })
    {
        Console.WriteLine($"{c.Name} は HP が 80 以上 200 未満で、武器を持っていない");
    }
}

// クラスは、1 節のコード例と同じ
```

```
Bob は HP が 80 以上 200 未満で、武器を持っていない
```

`{ Hp: >= 80 and < 200, Weapon: null }` は、「`Hp` が `>= 80 and < 200` に当てはまり、`Weapon` が `null` に当てはまる」というパターンです。`c.Hp` のように、調べる変数 `c` を何度も書く必要はありません。

### 文字列が空でないかを調べる

プロパティパターンは、自分で作ったクラスだけでなく、`string` のような .NET の型にも使えます。入力された文字列が `null` でも空文字列でもないことを調べます。

```csharp
string?[] inputs = { "Alice", "", null };

foreach (string? input in inputs)
{
    if (input is { Length: > 0 })
    {
        Console.WriteLine($"入力あり: {input}");
    }
    else
    {
        Console.WriteLine("入力なし");
    }
}
```

```
入力あり: Alice
入力なし
入力なし
```

`input is { Length: > 0 }` は、`input != null && input.Length > 0` と同じ条件です。`null` のときは `{ }` に当てはまらないので、`Length` を読む前に `null` を確かめる必要はありません。

---

## 3. 入れ子のプロパティを調べる

プロパティパターンの中に、プロパティパターンを書けます。1 節の条件は、次のように書き直せます。

```csharp
Character[] characters =
{
    new Player("Alice", 100, new Weapon("聖剣", 90)),
    new Player("Bob", 80, null),
    new Enemy("Slime", 20, new Weapon("牙", 5), false),
    new Enemy("Dragon", 300, new Weapon("炎", 120), true),
};

foreach (Character c in characters)
{
    if (c is { Weapon: { Power: >= 50 } })
    {
        Console.WriteLine($"{c.Name} は強い武器を持っている");
    }
}

// クラスは、1 節のコード例と同じ
```

```
Alice は強い武器を持っている
Dragon は強い武器を持っている
```

`Bob` の `Weapon` は `null` ですが、例外は発生しません。`{ Power: >= 50 }` は `null` には当てはまらないので、`Bob` は条件に当てはまらないだけです。`null` のチェックを、パターンが代わりにしてくれます。

入れ子のプロパティパターンは、`.` でつないで短く書けます（C# 10 から）。次の 2 つは同じパターンです。

```
{ Weapon: { Power: >= 50 } }
{ Weapon.Power: >= 50 }
```

`.` でつないだ書き方でも、途中の `Weapon` が `null` なら、当てはまらないだけです。

---

## 4. 型パターンと組み合わせる

型パターンの型名の後ろに、プロパティパターンを続けて書けます。型を調べて、さらに、その型のプロパティを調べられます。

```
c is Enemy { IsBoss: true }
```

`IsBoss` は `Enemy` にしかないプロパティです。型パターンだけで書くと、`Enemy` に変換した変数を用意してから調べることになります。

```
c is Enemy e && e.IsBoss
```

組み合わせたパターンでは、変換した変数を用意しなくても、`Enemy` のプロパティを調べられます。

```csharp
Character[] characters =
{
    new Player("Alice", 100, new Weapon("聖剣", 90)),
    new Player("Bob", 80, null),
    new Enemy("Slime", 20, new Weapon("牙", 5), false),
    new Enemy("Dragon", 300, new Weapon("炎", 120), true),
};

foreach (Character c in characters)
{
    if (c is Enemy { IsBoss: true })
    {
        Console.WriteLine($"{c.Name} はボス");
    }

    if (c is Player { Weapon: null })
    {
        Console.WriteLine($"{c.Name} は武器を持っていないプレイヤー");
    }
}

// クラスは、1 節のコード例と同じ
```

```
Bob は武器を持っていないプレイヤー
Dragon はボス
```

---

## 5. var パターンで値を取り出す

`var 変数名` と書いたパターンは、どんな値にも当てはまり、その値を変数に取り出します。これを **var パターン**（var pattern）といいます。プロパティパターンの中に書くと、調べるついでに、プロパティの値を取り出せます。

**書式：[var パターン](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/patterns#var-pattern)**
```
var 変数名
```

```csharp
Character[] characters =
{
    new Player("Alice", 100, new Weapon("聖剣", 90)),
    new Player("Bob", 80, null),
    new Enemy("Slime", 20, new Weapon("牙", 5), false),
    new Enemy("Dragon", 300, new Weapon("炎", 120), true),
};

foreach (Character c in characters)
{
    if (c is { Weapon: { Name: var weaponName, Power: >= 50 } })
    {
        Console.WriteLine($"{c.Name} の {weaponName} は強い");
    }
}

// クラスは、1 節のコード例と同じ
```

```
Alice の 聖剣 は強い
Dragon の 炎 は強い
```

`Name: var weaponName` は、`Weapon.Name` の値を変数 `weaponName` に取り出します。`if` の中では、`c.Weapon.Name` と書く代わりに、`weaponName` を使えます。

---

## 6. switch 式で分岐する

ここまでは `is` で 1 つのパターンを調べました。状態に応じて結果を選ぶときは、`switch` 式のアームにプロパティパターンを並べます。

```csharp
Character[] characters =
{
    new Player("Alice", 100, new Weapon("聖剣", 90)),
    new Player("Bob", 80, null),
    new Enemy("Slime", 0, new Weapon("牙", 5), false),
    new Enemy("Dragon", 300, new Weapon("炎", 120), true),
};

foreach (Character c in characters)
{
    string state = c switch
    {
        { Hp: <= 0 } => "戦闘不能",
        Enemy { IsBoss: true } => "ボス",
        { Weapon: null } => "素手",
        { Weapon.Power: >= 50 } => "強い武器を持っている",
        _ => "武器を持っている",
    };
    Console.WriteLine($"{c.Name}: {state}");
}

// クラスは、1 節のコード例と同じ
```

```
Alice: 強い武器を持っている
Bob: 素手
Slime: 戦闘不能
Dragon: ボス
```

アームは上から順に調べられます。`Slime` は `Hp` が `0` なので、最初のアームで `戦闘不能` になります。`Dragon` は強い武器も持っていますが、先にある `Enemy { IsBoss: true }` のアームが選ばれます。

---

## 7. { } と、null でない値を取り出す

中に何も書かない `{ }` は、`null` でない値なら何にでも当てはまります。

```csharp
Character? someone = new Player("Alice", 100, null);
Character? nobody = null;

Console.WriteLine(someone is { });
Console.WriteLine(nobody is { });

// クラスは、1 節のコード例と同じ
```

```
True
False
```

`x is { }` は、[パターンと is 演算子](/unity-csharp-learning/csharp/is-patterns/) で学んだ `x is not null` と同じ意味です。

### null でない値を変数に取り出す

`is { }` がよく使われるのは、`null` でない値を、変数に取り出すときです。`{ }` の後ろに変数名を書くと、当てはまった値が変数に入ります。

**書式：[プロパティパターン](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/patterns#property-pattern)（変数に取り出す）**
```
式 is { } 変数名
式 is { プロパティ名: パターン } 変数名
```

```csharp
Weapon? found = FindWeapon(1);
if (found is { } weapon)
{
    Console.WriteLine(weapon.Name);
}

int? level = GetLevel();
if (level is { } value)
{
    int next = value + 1;
    Console.WriteLine(next);
}

string? input = "alice";
if (input is { Length: > 0 } text)
{
    Console.WriteLine(text.ToUpper());
}

Weapon? FindWeapon(int id)
{
    if (id == 1)
    {
        return new Weapon("聖剣", 90);
    }
    return null;
}

int? GetLevel()
{
    return 5;
}

// クラスは、1 節のコード例と同じ
```

```
聖剣
6
ALICE
```

取り出した変数の型には、`?` が付きません。`found` は `Weapon?` 型ですが、`weapon` は `Weapon` 型です。`level` は [null 許容値型](/unity-csharp-learning/csharp/nullable-value-types/) の `int?` ですが、`value` は `int` 型なので、`value + 1` の結果を `int` の変数にそのまま代入できます（`level + 1` は `int?` になるので、`int` の変数には代入できません）。

`is not null` の後ろには、変数名を書けません。

```csharp
// ❌ NG: is not null では値を取り出せない
// Weapon? found = FindWeapon(1);
// if (found is not null weapon)  // CS1073
// {
// }
```

`null` かどうかを調べるだけなら `is not null`、`null` でない値を取り出して使うなら `is { } 変数名`、と使い分けます。

---

## よくあるミス

### 型パターンで変数を用意してから、プロパティを調べる

```csharp
// ❌ NG: 動くが、null のチェックまで自分で書いていて長い
// if (c is Enemy e && e.IsBoss && e.Weapon != null && e.Weapon.Power >= 100)

// ✅ OK: プロパティパターンでまとめる
// if (c is Enemy { IsBoss: true, Weapon.Power: >= 100 })
```

型パターンで変数を用意し、`&&` で条件をつないでいくと、`null` のチェックも自分で書くことになります。調べたいのが「どんな状態のオブジェクトか」なら、プロパティパターンで 1 つにまとめると、条件が読みやすくなります。取り出した値を `if` の中で使いたいときは、5 節の `var` パターンを組み合わせます。

---

## まとめ

- プロパティパターン `{ プロパティ名: パターン }` で、オブジェクトのプロパティの値を調べる。条件は `,` で並べられる
- 値が `null` なら当てはまらないだけで、例外は発生しない。`null` のチェックを書かなくてよい
- 入れ子のプロパティは、`{ Weapon: { Power: >= 50 } }` または `{ Weapon.Power: >= 50 }` で調べる
- `Enemy { IsBoss: true }` のように、型パターンと組み合わせると、型と中身を一度に調べられる
- `var 変数名` で、プロパティの値を変数に取り出せる
- `is` でも `switch` 式でも使える。`{ }` は `null` でない値に当てはまる
- `is { } 変数名` で、`null` でない値を取り出せる。取り出した変数の型には `?` が付かない。`is not null` では値を取り出せない

---

## 理解度チェック

1. 1 節のクラスと `characters` を使います。次のコードを実行すると何が出力されますか？

   ```csharp
   foreach (Character c in characters)
   {
       if (c is { Hp: < 100, Weapon.Power: > 0 })
       {
           Console.WriteLine(c.Name);
       }
   }
   ```

2. 次の条件を、プロパティパターンを使って書き直してください。

   ```csharp
   if (c is Player p && p.Weapon != null && p.Weapon.Power >= 50)
   ```

3. `Character? c = null;` のとき、`c is { Hp: > 0 }` を実行すると、`NullReferenceException` は発生しますか？
4. 6 節の `switch` 式で、`Enemy { IsBoss: true }` のアームを、`{ Weapon.Power: >= 50 }` のアームの後ろに移しました。`Dragon` の結果はどうなりますか？

<details markdown="1">
<summary>解答を見る</summary>

1. `Slime` だけが出力されます。`Alice` は `Hp` が `100` なので `< 100` に当てはまりません。`Bob` は `Weapon` が `null` なので、`Weapon.Power: > 0` に当てはまりません。`Dragon` は `Hp` が `300` です。
2. `if (c is Player { Weapon.Power: >= 50 })` です。`Weapon` が `null` のときは当てはまらないので、`null` のチェックは要りません。
3. 発生しません。プロパティパターンは、値が `null` なら当てはまらないだけなので、結果は `false` です。
4. `強い武器を持っている` になります。`Dragon` の武器の攻撃力は `120` なので、先にある `{ Weapon.Power: >= 50 }` のアームが選ばれます。

</details>

---

## 次のステップ

[位置パターンとリストパターン](/unity-csharp-learning/csharp/positional-list-patterns/) では、タプル・record・配列の要素を、順番に調べるパターンを学びます。
