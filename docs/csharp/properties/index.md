---
layout: page
title: プロパティ
permalink: /csharp/properties/
---

# プロパティ

**プロパティ**（property）は、フィールドと同じように `p.Hp` と書いて読み書きでき、実際には `get` / `set` という処理が実行される仕組みです。`private` のフィールドを、`GetHp()` のようなメソッドを使わずに、安全に公開できます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `get` / `set` アクセサーのあるプロパティを定義できる
- `set` アクセサーで値を調べてから、フィールドに保存できる
- 自動実装プロパティ（`{ get; set; }`）を使える
- 読み取り専用のプロパティの 2 つの書き方の違いを説明できる

## 前提知識

- [アクセス修飾子](/unity-csharp-learning/csharp/access-modifiers/) を読んでいること

---

## 1. Get / Set メソッドの不便なところ

[アクセス修飾子](/unity-csharp-learning/csharp/access-modifiers/) では、`private` のフィールド `_hp` を守るために、値を読み取る `GetHp` メソッドを用意しました。値を書き込むための `SetHp` メソッドも用意すると、次のようになります。

```csharp
Player p = new Player();
p.SetHp(100);
p.SetHp(p.GetHp() - 30);
Console.WriteLine(p.GetHp());

class Player
{
    private int _hp;

    public int GetHp()
    {
        return _hp;
    }

    public void SetHp(int value)
    {
        if (value < 0)
        {
            value = 0;
        }
        _hp = value;
    }
}
```

```
70
```

値を読み取るメソッドを **ゲッター**（getter）、値を書き込むメソッドを **セッター**（setter）といいます。このとき、実際に値を保存している `private` のフィールド `_hp` を、**バッキングフィールド** といいます。

この書き方は正しく動きますが、次のような不便があります。

- `p.SetHp(p.GetHp() - 30)` のように、計算を含むと読みにくい
- ゲッターとセッターは別々のメソッドなので、`GetHp` に対して `SetHitPoint` のように、ばらばらな名前を付けることもできてしまう

プロパティを使うと、同じことを `p.Hp -= 30;` と、フィールドのように書けます。

---

## 2. プロパティを定義する

**書式：[プロパティの定義](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/properties#properties-with-backing-fields)**
```
アクセス修飾子 型 プロパティ名
{
    get { return バッキングフィールド; }
    set { バッキングフィールド = value; }
}
```

| 要素 | 説明 |
|---|---|
| `get` アクセサー | プロパティの値を **読み取る** ときに実行される。`return` で値を返す |
| `set` アクセサー | プロパティに値を **書き込む** ときに実行される |
| [value](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/value) | `set` アクセサーの中で使える特別な変数。書き込もうとしている値が入っている。型はプロパティの型と同じ |

1 節の `GetHp` と `SetHp` を、プロパティ `Hp` に書き換えます。プロパティの名前は、`public` のメンバーなので PascalCase にします。

```csharp
Player p = new Player();
p.Hp = 100;
Console.WriteLine(p.Hp);

p.Hp -= 30;
Console.WriteLine(p.Hp);

p.Hp = -50;
Console.WriteLine(p.Hp);

class Player
{
    private int _hp;

    public int Hp
    {
        get { return _hp; }
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            _hp = value;
        }
    }
}
```

```
100
70
0
```

- `p.Hp = 100;` では、`set` アクセサーが実行され、`value` に `100` が入ります
- `Console.WriteLine(p.Hp)` では、`get` アクセサーが実行され、`_hp` の値が返されます
- `p.Hp -= 30;` では、`get` で今の値を読み取り、30 を引いた値を `set` で書き込みます
- `p.Hp = -50;` では、`set` アクセサーの中で負の値が `0` に直されてから保存されます

使う側はフィールドと同じように書けて、値を調べる処理は `set` アクセサーの 1 か所にまとめられます。

---

## 3. 自動実装プロパティ

値を調べる必要がなく、読み書きするだけなら、`get` と `set` の本体を省略できます。これを [自動実装プロパティ](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties) といいます。バッキングフィールドは、コンパイラーが自動的に用意します。

**書式：[自動実装プロパティ](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/properties#automatically-implemented-properties)**
```
アクセス修飾子 型 プロパティ名 { get; set; }
アクセス修飾子 型 プロパティ名 { get; set; } = 初期値;
```

```csharp
Player p = new Player();
Console.WriteLine($"[{p.Name}] (Lv.{p.Level})");

p.Name = "Alice";
p.Level = 5;
Console.WriteLine($"{p.Name} (Lv.{p.Level})");

class Player
{
    public string Name { get; set; } = "";
    public int Level { get; set; } = 1;
}
```

```
[] (Lv.1)
Alice (Lv.5)
```

フィールドと同じように、`= 初期値` で初期値を書けます。自分で `private string _name;` のようなバッキングフィールドを書く必要はありません。

---

## 4. 読み取り専用のプロパティ

クラスの外から値を変えられないようにするには、`set` アクセサーを書かないか、`set` を `private` にします。

### { get; } — コンストラクターでだけ値を入れられる

`set` を書かない自動実装プロパティは、コンストラクターの中（と初期値）でだけ値を入れられます。インスタンスを作った後は、クラスの外からも中からも変えられません。

```csharp
Player p = new Player("Alice");
Console.WriteLine(p.Name);

class Player
{
    public string Name { get; }

    public Player(string name)
    {
        Name = name;
    }
}
```

```
Alice
```

```csharp
// ❌ NG: set のないプロパティには、代入できない
// p.Name = "Bob";  // CS0200
```

### { get; private set; } — クラスの中からだけ値を変えられる

`set` の前に `private` を付けると、クラスの外からは読み取り専用で、クラスの中のメソッドからは値を変えられるプロパティになります。

```csharp
Player p = new Player(100);
p.TakeDamage(30);
Console.WriteLine(p.Hp);

class Player
{
    public int Hp { get; private set; }

    public Player(int hp)
    {
        Hp = hp;
    }

    public void TakeDamage(int damage)
    {
        Hp -= damage;
        if (Hp < 0)
        {
            Hp = 0;
        }
    }
}
```

```
70
```

```csharp
// ❌ NG: set が private なので、クラスの外からは代入できない
// p.Hp = 999;  // CS0272
```

| 書き方 | クラスの外から | クラスの中のメソッドから | コンストラクターから |
|---|---|---|---|
| `{ get; set; }` | 読み書きできる | 読み書きできる | 読み書きできる |
| `{ get; private set; }` | 読み取りだけ | 読み書きできる | 読み書きできる |
| `{ get; }` | 読み取りだけ | 読み取りだけ | 読み書きできる |

---

## 5. プロパティを使ったクラスの例

ここまでの書き方を組み合わせた `Player` クラスです。

```csharp
Player p = new Player("Alice", 100);
p.TakeDamage(30);
p.TakeDamage(200);
p.LevelUp();

class Player
{
    // 読み取り専用（コンストラクターでだけ値を入れる）
    public string Name { get; }

    // クラスの中からだけ変えられる
    public int MaxHp { get; private set; }

    // 読み書きできる
    public int Level { get; set; } = 1;

    // 値を 0 から MaxHp の範囲に収める
    private int _hp;
    public int Hp
    {
        get { return _hp; }
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            if (value > MaxHp)
            {
                value = MaxHp;
            }
            _hp = value;
        }
    }

    public Player(string name, int maxHp)
    {
        Name = name;
        MaxHp = maxHp;
        Hp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        Hp -= damage;
        Console.WriteLine($"{Name} が {damage} ダメージ。残り HP={Hp}/{MaxHp}");
    }

    public void LevelUp()
    {
        Level++;
        MaxHp += 20;
        Hp = MaxHp;
        Console.WriteLine($"{Name} がレベル {Level} になった！ HP={Hp}/{MaxHp}");
    }
}
```

```
Alice が 30 ダメージ。残り HP=70/100
Alice が 200 ダメージ。残り HP=0/100
Alice がレベル 2 になった！ HP=120/120
```

`TakeDamage` の中でも `Hp` プロパティを通して値を変えているので、HP が負になることはありません。クラスの中でも、値を調べる処理を通したいときは、フィールドではなくプロパティを使います。

---

## よくあるミス

### get や set の中で、プロパティ自身を使う

```csharp
// ❌ NG: get の中で Score（プロパティ自身）を返している
// class Player
// {
//     public int Score
//     {
//         get { return Score; }
//         set { Score = value; }
//     }
// }
```

`get` の中で `Score` を読むと、また `Score` の `get` が実行され、それが終わりなく続きます。コンパイルエラーにはなりませんが、実行するとスタックオーバーフロー（[再帰関数とコールスタック](/unity-csharp-learning/csharp/recursion/) で学ぶ、呼び出しの積み重ねがあふれるエラー）でプログラムが異常終了します。値は、バッキングフィールド（`_score`）に読み書きします。

### set の中で value の代わりにプロパティを使う

```csharp
Player p = new Player();
p.Hp = 100;
Console.WriteLine(p.Hp);

class Player
{
    private int _hp;

    public int Hp
    {
        get { return _hp; }
        set { _hp = Hp; }
    }
}
```

```
0
```

`set` の中の `Hp` は、`get` で読み取った **今の値** です。書き込もうとしている値は `value` に入っているので、`_hp = value;` と書きます。

---

## ワンポイントアドバイス

### init アクセサー（C# 9 以降）

`set` の代わりに [init](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/init) と書くと、コンストラクターのほか、インスタンスを作る式の中でだけ値を入れられるプロパティになります。

```csharp
Player p = new Player { Name = "Alice", Level = 3 };
Console.WriteLine($"{p.Name} (Lv.{p.Level})");

class Player
{
    public string Name { get; init; } = "";
    public int Level { get; init; } = 1;
}
```

```
Alice (Lv.3)
```

`new Player { Name = "Alice", Level = 3 }` の `{ }` の部分を、[オブジェクト初期化子](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/object-and-collection-initializers) といいます。インスタンスを作った後に `p.Name = "Bob";` と書くと、コンパイルエラーになります。

---

## まとめ

- プロパティは、フィールドのように読み書きでき、実際には `get` / `set` アクセサーが実行される
- `set` アクセサーの中の `value` には、書き込もうとしている値が入っている
- `set` アクセサーで値を調べると、範囲外の値が保存されるのを防げる
- 値を調べる必要がなければ、自動実装プロパティ `{ get; set; }` を使う
- `{ get; }` はコンストラクターでだけ、`{ get; private set; }` はクラスの中からだけ、値を入れられる
- `get` や `set` の中では、プロパティ自身ではなくバッキングフィールドを読み書きする

---

## 理解度チェック

1. 次のプロパティには、どのような問題がありますか？

   ```csharp
   public int Score
   {
       get { return Score; }
       set { Score = value; }
   }
   ```

2. `{ get; private set; }` と `{ get; }` の違いを説明してください。
3. 次のクラスの `_level` フィールドと `GetLevel` メソッドを、プロパティ `Level`（クラスの外からは読み取り専用、クラスの中からは変更できる）に書き換えてください。

   ```csharp
   class Hero
   {
       private int _level = 1;

       public int GetLevel()
       {
           return _level;
       }

       public void LevelUp()
       {
           _level++;
       }
   }
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. `get` の中で `Score`（プロパティ自身）を読むので、`get` が終わりなく呼び出され、スタックオーバーフローでプログラムが異常終了します。バッキングフィールド `_score` を用意して、`get` では `return _score;`、`set` では `_score = value;` と書きます。
2. `{ get; }` は、コンストラクターの中（と初期値）でだけ値を入れられ、その後はクラスの中からも変えられません。`{ get; private set; }` は、クラスの外からは読み取り専用ですが、クラスの中のメソッドからは変えられます。
3. ```csharp
   Hero h = new Hero();
   h.LevelUp();
   Console.WriteLine(h.Level);

   class Hero
   {
       public int Level { get; private set; } = 1;

       public void LevelUp()
       {
           Level++;
       }
   }
   ```

   `2` が表示されます。

</details>

---

## 次のステップ

[インデクサ](/unity-csharp-learning/csharp/indexers/) では、自分で作ったクラスに、配列のように `[]` で要素を読み書きする機能を持たせる方法を学びます。
