---
layout: page
title: アクセス修飾子
permalink: /csharp/access-modifiers/
---

# アクセス修飾子

**アクセス修飾子**（access modifier）は、フィールドやメソッドを、どこから使えるかを決めるキーワードです。クラスの外から使ってよいものだけを公開し、クラスの中の仕組みを隠すことを **カプセル化**（encapsulation）といいます。カプセル化すると、クラスが間違った使い方をされるのを防げます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `public` と `private` の違いを説明できる
- `private` のフィールドを、メソッドを通して操作するクラスを作れる
- アクセス修飾子を省略したときに、どのアクセスレベルになるかを説明できる

## 前提知識

- [コンストラクター](/unity-csharp-learning/csharp/constructors/) を読んでいること

---

## 1. すべて public にすると困ること

フィールドを `public` にすると、クラスの外から、どのような値でも入れられます。

```csharp
Player p = new Player("Alice", 100);
p.Hp = -500;
Console.WriteLine($"{p.Name}: HP={p.Hp}");

class Player
{
    public string Name;
    public int Hp;

    public Player(string name, int hp)
    {
        Name = name;
        Hp = hp;
    }
}
```

```
Alice: HP=-500
```

HP が負になるのは、ゲームとしてありえない状態です。しかし、`Hp` が `public` なので、クラスの外のどこからでも、このような値を入れられてしまいます。

---

## 2. public と private

[アクセス修飾子](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/access-modifiers) の基本は、`public` と `private` の 2 つです。

| 修飾子 | 使える範囲 |
|---|---|
| `public` | どこからでも使える |
| `private` | 同じクラスの中からだけ使える |

**書式：[アクセス修飾子を付けたメンバーの定義](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/access-modifiers)**
```
アクセス修飾子 型 フィールド名;
アクセス修飾子 戻り値の型 メソッド名(パラメータ)
```

`Hp` を `private` にして、HP を変える処理は `TakeDamage` と `Heal` のメソッドだけにします。

```csharp
Player p = new Player("Alice", 100);
p.TakeDamage(30);
p.Heal(50);
p.TakeDamage(500);
Console.WriteLine($"現在の HP={p.GetHp()}");

class Player
{
    public string Name;
    private int _hp;
    private int _maxHp;

    public Player(string name, int hp)
    {
        Name = name;
        _hp = hp;
        _maxHp = hp;
    }

    public void TakeDamage(int damage)
    {
        if (damage < 0)
        {
            return;
        }
        _hp = _hp - damage;
        if (_hp < 0)
        {
            _hp = 0;
        }
        Console.WriteLine($"{Name} が {damage} ダメージ。残り HP={_hp}");
    }

    public void Heal(int amount)
    {
        if (amount < 0)
        {
            return;
        }
        _hp = _hp + amount;
        if (_hp > _maxHp)
        {
            _hp = _maxHp;
        }
        Console.WriteLine($"{Name} が {amount} 回復。残り HP={_hp}");
    }

    public int GetHp()
    {
        return _hp;
    }
}
```

```
Alice が 30 ダメージ。残り HP=70
Alice が 50 回復。残り HP=100
Alice が 500 ダメージ。残り HP=0
現在の HP=0
```

`_hp` は `private` なので、クラスの外からは直接読み書きできません。HP を変えられるのは `TakeDamage` と `Heal` だけで、どちらも HP が 0 から最大値の範囲に収まるように調べています。HP を読みたいときのために、`_hp` の値を返す `GetHp` メソッドを `public` で用意しています。

`private` のフィールドにクラスの外から触ろうとすると、コンパイルエラーになります。

```csharp
// ❌ NG: private のフィールドは、クラスの外から使えない
// p._hp = 999;  // CS0122
```

> 💡 **名前の付け方**: `private` のフィールドは、`_hp` のように `_` で始まる camelCase（最初の単語は小文字、2 つ目以降の単語の先頭は大文字）で名付けるのが一般的です。`public` のメンバーは PascalCase で名付けます。名前を見れば、クラスの外から使えるかどうかがわかります。

---

## 3. アクセス修飾子を省略したとき

アクセス修飾子を省略すると、次のアクセスレベルになります。

| 対象 | 省略したときのアクセスレベル |
|---|---|
| クラスのメンバー（フィールド、メソッドなど） | `private` |
| クラスそのもの | `internal`（4 節の表を参照） |

```csharp
// ❌ NG: アクセス修飾子を省略したフィールドは private になる
// Box b = new Box();
// Console.WriteLine(b.Value);  // CS0122
//
// class Box
// {
//     int Value;
// }
```

省略したメンバーは `private` になりますが、読む人に意図が伝わるように、`private` も省略せずに書く習慣をつけましょう。

---

## 4. その他のアクセス修飾子

中規模以上のプログラムでは、次のアクセス修飾子も使われます。ここでは、名前と意味だけを確認しておきましょう。

| 修飾子 | 使える範囲 |
|---|---|
| `internal` | 同じアセンブリ（同じプロジェクトからビルドされたプログラム）の中から |
| `protected` | 同じクラスと、そのクラスを継承したクラスの中から |
| `protected internal` | `protected` または `internal` のどちらかの条件を満たす場所から |
| `private protected` | 同じアセンブリの中で、かつ、継承したクラスの中から |

`protected` は、継承を学んだ後の [protected 修飾子](/unity-csharp-learning/csharp/protected-modifier/) で詳しく学びます。

---

## まとめ

- `public` のメンバーはどこからでも使え、`private` のメンバーは同じクラスの中からだけ使える
- フィールドは `private` にして、値の変更はメソッドを通して行う。これをカプセル化という
- `private` のメンバーをクラスの外から使うと、コンパイルエラー（CS0122）になる
- アクセス修飾子を省略したメンバーは `private` になる

---

## 理解度チェック

1. 次のコードはコンパイルエラーになりますか？理由も答えてください。

   ```csharp
   Box b = new Box(10);
   Console.WriteLine(b._value);

   class Box
   {
       private int _value;

       public Box(int value)
       {
           _value = value;
       }
   }
   ```

2. 次のクラスの `_score` フィールドを `private` にして、スコアを足す `AddScore(int points)` メソッドと、今のスコアを返す `GetScore()` メソッドを追加してください。

   ```csharp
   class Player
   {
       public int _score;
   }
   ```

3. （応用）`private` のフィールド `_level`（最初は 1）と `_exp`（最初は 0）を持つ `Character` クラスを定義してください。`AddExp(int amount)` メソッドで `_exp` を増やし、100 以上になったら `_level` を 1 上げて `_exp` を 0 に戻します。

<details markdown="1">
<summary>解答を見る</summary>

1. コンパイルエラーになります（CS0122）。`_value` は `private` なので、`Box` クラスの外からは使えません。
2. ```csharp
   Player p = new Player();
   p.AddScore(30);
   p.AddScore(20);
   Console.WriteLine(p.GetScore());

   class Player
   {
       private int _score;

       public void AddScore(int points)
       {
           _score = _score + points;
       }

       public int GetScore()
       {
           return _score;
       }
   }
   ```

   `50` が表示されます。

3. ```csharp
   Character c = new Character();
   c.AddExp(60);
   c.AddExp(50);
   Console.WriteLine(c.GetStatus());

   class Character
   {
       private int _level = 1;
       private int _exp;

       public void AddExp(int amount)
       {
           _exp = _exp + amount;
           if (_exp >= 100)
           {
               _level = _level + 1;
               _exp = 0;
           }
       }

       public string GetStatus()
       {
           return $"Lv={_level}, EXP={_exp}";
       }
   }
   ```

   `Lv=2, EXP=0` が表示されます。

</details>

---

## 次のステップ

[プロパティ](/unity-csharp-learning/csharp/properties/) では、`private` のフィールドを、`GetHp()` のようなメソッドを使わずに、フィールドのような書き方で安全に読み書きする方法を学びます。
