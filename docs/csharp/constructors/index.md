---
layout: page
title: コンストラクター
permalink: /csharp/constructors/
---

# コンストラクター

**コンストラクター**（constructor）は、`new` でインスタンスを作るときに自動的に呼び出される、特別なメソッドです。インスタンスのフィールドに最初の値を入れる（初期化する）ために使います。パラメータのあるコンストラクターを定義すると、必要な値を渡さなければインスタンスを作れないようにできます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- コンストラクターを定義し、インスタンスを作るときにフィールドを初期化できる
- コンストラクターと通常のメソッドの違いを説明できる
- パラメータのないコンストラクターが自動的に用意される条件を説明できる

## 前提知識

- [メソッド](/unity-csharp-learning/csharp/methods/) を読んでいること

---

## 1. コンストラクターが必要な理由

インスタンスを作った後で、フィールドに 1 つずつ値を入れる方法では、入れ忘れが起きやすくなります。

```csharp
Player p = new Player();
p.Hp = 100;
p.Greet();

class Player
{
    public string Name = "";
    public int Hp;

    public void Greet()
    {
        Console.WriteLine($"こんにちは、[{Name}]です！ HP={Hp}");
    }
}
```

```
こんにちは、[]です！ HP=100
```

`Name` に値を入れ忘れたまま `Greet` を呼び出したので、名前が空のまま表示されました。コンパイラーは、この入れ忘れを見つけてくれません。

コンストラクターを使うと、インスタンスを作るときに、必要な値を必ず渡すようにできます。

---

## 2. コンストラクターを定義する

[コンストラクター](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/constructors) は、クラスと同じ名前で、戻り値の型（`void` も）を書かずに定義します。

**書式：[コンストラクターの定義](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/constructors#constructor-syntax)**
```
アクセス修飾子 クラス名(型 パラメータ名, ...)
{
    // 初期化の処理
}
```

| | 通常のメソッド | コンストラクター |
|---|---|---|
| 呼び出されるとき | `インスタンス.メソッド名()` と書いたとき | `new` でインスタンスを作るとき（自動的に） |
| 戻り値の型 | 書く（返さないときは `void`） | 書かない |
| 名前 | 自由に付ける | クラス名と同じ |

```csharp
Player p1 = new Player("Alice", 100);
Player p2 = new Player("Bob", 80);

Console.WriteLine($"{p1.Name}: HP={p1.Hp}");
Console.WriteLine($"{p2.Name}: HP={p2.Hp}");

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
Alice: HP=100
Bob: HP=80
```

`new Player("Alice", 100)` の `("Alice", 100)` は、コンストラクターに渡す引数です。コンストラクターのパラメータ `name` と `hp` に値が入り、本体でフィールドに代入されます。

このクラスでは、名前を渡さないとインスタンスを作れないので、`Name` の入れ忘れは起きません。コンストラクターで必ず値を入れるので、`Name` に `""` の初期値を書かなくても、コンパイラーは警告を出しません。

> 💡 **ポイント**: コンストラクターに引数を渡したうえで、[オブジェクト初期化子（補足）](/unity-csharp-learning/csharp/object-initializers/) を書けます。コンストラクターが実行された後に、オブジェクト初期化子の代入が実行されます。たとえば、上の `Player` で `new Player("Alice", 100) { Hp = 50 }` と書くと、コンストラクターで `100` が入った後に `50` で上書きされ、`Hp` は `50` になります。必ず入れてほしい値はコンストラクターで受け取り、入れなくてもよい値はオブジェクト初期化子で入れる、という使い分けができます。

---

## 3. パラメータのないコンストラクターが自動的に用意される条件

これまでの `new Player()` は、コンストラクターを定義していないクラスでも使えました。コンストラクターを **1 つも定義していない** クラスには、コンパイラーが、何もしないパラメータのないコンストラクター（**既定のコンストラクター**）を自動的に用意するからです。

ただし、コンストラクターを 1 つでも定義すると、既定のコンストラクターは用意されなくなります。

```csharp
// ❌ NG: パラメータのあるコンストラクターしかないので、new Player() は使えない
// Player p = new Player();  // CS7036
//
// class Player
// {
//     public string Name;
//
//     public Player(string name)
//     {
//         Name = name;
//     }
// }
```

パラメータのないコンストラクターも使いたいときは、自分で定義します。コンストラクターもメソッドと同じように、パラメータの違うものを複数定義（オーバーロード）できます。

```csharp
Player p1 = new Player();
Player p2 = new Player("Alice", 80);

Console.WriteLine($"{p1.Name}: HP={p1.Hp}");
Console.WriteLine($"{p2.Name}: HP={p2.Hp}");

class Player
{
    public string Name;
    public int Hp;

    public Player()
    {
        Name = "名無し";
        Hp = 100;
    }

    public Player(string name, int hp)
    {
        Name = name;
        Hp = hp;
    }
}
```

```
名無し: HP=100
Alice: HP=80
```

`new Player()` ではパラメータのないコンストラクターが、`new Player("Alice", 80)` では 2 つのパラメータのあるコンストラクターが呼び出されます。

---

## よくあるミス

### コンストラクターに戻り値の型を書く

```csharp
// ❌ NG: void を書くと、クラスと同じ名前のメソッドになってしまう
// class Player
// {
//     public void Player(string name)  // CS0542
//     {
//     }
// }
```

`void` などの戻り値の型を書くと、コンストラクターではなく、クラスと同じ名前のメソッドを定義したことになります。C# では、クラスと同じ名前のメンバーは定義できないので、コンパイルエラーになります。コンストラクターには、戻り値の型を書きません。

---

## ワンポイントアドバイス

### this で自分自身のメンバーを指す

コンストラクターやメソッドの本体では、[this](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/this) で、そのメソッドを呼び出したインスタンス自身を表せます。パラメータとフィールドの名前が同じときは、`this.フィールド名` と書いて、フィールドのほうを指定します。

```csharp
Item item = new Item("回復薬", 50);
Console.WriteLine($"{item.GetName()}: {item.GetPrice()} G");

class Item
{
    private string name;
    private int price;

    public Item(string name, int price)
    {
        this.name = name;
        this.price = price;
    }

    public string GetName()
    {
        return name;
    }

    public int GetPrice()
    {
        return price;
    }
}
```

```
回復薬: 50 G
```

`this.name` はフィールドの `name`、`name` だけならパラメータの `name` です。`private` は、次のページで学ぶアクセス修飾子です。このサイトでは、`private` のフィールドの名前を `_name` のように `_` で始めて、パラメータと名前が重ならないようにします。

---

## まとめ

- コンストラクターは、`new` でインスタンスを作るときに自動的に呼び出される。クラスと同じ名前で、戻り値の型を書かない
- パラメータのあるコンストラクターで、必要な値を渡さなければインスタンスを作れないようにできる
- コンストラクターを 1 つも定義していないクラスには、パラメータのない既定のコンストラクターが自動的に用意される
- コンストラクターを 1 つでも定義すると、既定のコンストラクターは用意されない
- コンストラクターも、パラメータの違うものを複数定義できる

---

## 理解度チェック

1. 次のコードの `new Item()` はコンパイルエラーになりますか？理由も答えてください。

   ```csharp
   Item i = new Item();

   class Item
   {
       public string Name;
       public int Price;

       public Item(string name, int price)
       {
           Name = name;
           Price = price;
       }
   }
   ```

2. 次のクラスのコンストラクターを完成させてください。武器の名前と攻撃力を引数で受け取り、フィールドに代入します。

   ```csharp
   class Weapon
   {
       public string Name;
       public int Damage;

       public Weapon(/* ここを埋める */)
       {
           /* ここを埋める */
       }
   }
   ```

3. （応用）`Name`（`string`）・`Hp`（`int`）・`MaxHp`（`int`）のフィールドを持つ `Player` クラスを定義してください。コンストラクターで名前と最初の HP を受け取り、`Hp` と `MaxHp` の両方を、受け取った HP で初期化します。

<details markdown="1">
<summary>解答を見る</summary>

1. コンパイルエラーになります（CS7036）。パラメータのあるコンストラクター `Item(string, int)` を定義しているので、パラメータのない既定のコンストラクターは用意されません。
2. ```csharp
   Weapon w = new Weapon("鉄の剣", 12);
   Console.WriteLine($"{w.Name}: {w.Damage}");

   class Weapon
   {
       public string Name;
       public int Damage;

       public Weapon(string name, int damage)
       {
           Name = name;
           Damage = damage;
       }
   }
   ```

   `鉄の剣: 12` が表示されます。

3. ```csharp
   Player p = new Player("Alice", 120);
   Console.WriteLine($"{p.Name}: {p.Hp}/{p.MaxHp}");

   class Player
   {
       public string Name;
       public int Hp;
       public int MaxHp;

       public Player(string name, int hp)
       {
           Name = name;
           Hp = hp;
           MaxHp = hp;
       }
   }
   ```

   `Alice: 120/120` が表示されます。

</details>

---

## 次のステップ

[アクセス修飾子](/unity-csharp-learning/csharp/access-modifiers/) では、フィールドやメソッドを使える範囲を制限し、クラスを安全に作る方法を学びます。
