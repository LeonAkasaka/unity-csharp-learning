---
layout: page
title: コンストラクタ
permalink: /csharp/constructors/
---

# コンストラクタ

フィールドを定義しただけでは、インスタンスを生成した直後に必要な値を設定し忘れるリスクがあります。

```csharp
Player p = new Player();
// name の設定を忘れた
p.TakeDamage(10);  // name が null のまま使われる可能性がある
```

**コンストラクタ**は `new` が実行されたときに**自動的に呼び出されるメソッド**です。パラメータを設けることで「必要な値を渡さなければインスタンスを作れない」ように強制でき、初期化忘れを防ぎます。

## 学習目標

- コンストラクタを定義してインスタンスを初期化できる
- コンストラクタと通常のメソッドの違いを説明できる
- デフォルトコンストラクタが提供される条件を理解できる

## 前提知識

- [メソッド](/unity-csharp-learning/csharp/methods/) を読んでいること

---

## 1. コンストラクタの定義

コンストラクタはクラス名と同じ名前を持ち、戻り値型（`void` を含む）を書きません。

**書式：コンストラクタの定義**
```
アクセス修飾子 クラス名(パラメータ)
{
    初期化処理
}
```

| 比較 | 通常のメソッド | コンストラクタ |
|---|---|---|
| 呼び出しタイミング | 任意のタイミング | `new` のときに自動呼び出し |
| 戻り値型 | 指定する（`void` 含む） | 書かない |
| 名前 | 任意 | クラス名と同じ |

```csharp
class Player
{
    public string name;
    public int hp;
    public int score;

    public Player(string playerName, int initialHp)
    {
        name = playerName;
        hp = initialHp;
        score = 0;
    }
}
```

```csharp
Player p1 = new Player("Alice", 100);
Player p2 = new Player("Bob", 80);

Console.WriteLine($"{p1.name}: HP={p1.hp}");
Console.WriteLine($"{p2.name}: HP={p2.hp}");
```

```
Alice: HP=100
Bob: HP=80
```

`new Player("Alice", 100)` と書くだけで `name` と `hp` が確実に設定されます。コンストラクタのパラメータが必須になるため、設定忘れがコンパイルエラーとして検出されます。

---

## 2. デフォルトコンストラクタ

コンストラクタを**一つも定義しない**場合、コンパイラが自動的に**パラメータなしの空のコンストラクタ**（デフォルトコンストラクタ）を生成します。

```csharp
class Enemy
{
    public string name;
    public int hp;
    // コンストラクタを定義していない → デフォルトコンストラクタが自動生成される
}

Enemy e = new Enemy();  // OK: デフォルトコンストラクタが使われる
e.name = "Slime";
e.hp = 30;
```

ただし、**パラメータありのコンストラクタを 1 つでも定義すると**、デフォルトコンストラクタは自動生成されなくなります。

```csharp
class Player
{
    public string name;

    public Player(string playerName)
    {
        name = playerName;
    }
}

Player p = new Player();        // ❌ コンパイルエラー: 引数が必要
Player p = new Player("Alice"); // ✅ OK
```

パラメータなしでも作れるようにしたい場合は、明示的にパラメータなしコンストラクタを追加します。

```csharp
class Player
{
    public string name;
    public int hp;

    public Player()
    {
        name = "名無し";
        hp = 100;
    }

    public Player(string playerName, int initialHp)
    {
        name = playerName;
        hp = initialHp;
    }
}
```

```csharp
Player p1 = new Player();
Player p2 = new Player("Alice", 80);

Console.WriteLine($"{p1.name}: HP={p1.hp}");
Console.WriteLine($"{p2.name}: HP={p2.hp}");
```

```
名無し: HP=100
Alice: HP=80
```

---

## よくあるミス

### コンストラクタに戻り値型を書いてしまう

```csharp
class Player
{
    // ❌ NG: void を書くと通常のメソッドになりコンストラクタではなくなる
    //        new Player() で自動呼び出しされない
    public void Player(string name) { }

    // ✅ OK: 戻り値型なし
    public Player(string name) { }
}
```

---

## まとめ

- **コンストラクタ** — `new` 時に自動呼び出される。クラス名と同名で戻り値型なし
- パラメータを設けることで初期化忘れをコンパイルエラーとして検出できる
- コンストラクタを定義しなければデフォルトコンストラクタが自動生成される
- パラメータありのコンストラクタを定義するとデフォルトコンストラクタは消える

---

## 理解度チェック

1. 次のコードはコンパイルエラーになりますか？ 理由とともに答えてください。

   ```csharp
   class Item
   {
       public string name;
       public int price;

       public Item(string itemName, int itemPrice)
       {
           name = itemName;
           price = itemPrice;
       }
   }

   Item i = new Item();
   ```

2. 次のクラスのコンストラクタを完成させてください。`name` と `damage` を引数で受け取り、フィールドに代入してください。

   ```csharp
   class Weapon
   {
       public string name;
       public int damage;

       public Weapon(/* ここを埋める */)
       {
           /* ここを埋める */
       }
   }
   ```

3. （応用）`name`（string）・`hp`（int）・`maxHp`（int）を持つ `Player` クラスを定義してください。コンストラクタで `name` と初期 HP を受け取り、`maxHp` と `hp` の両方を初期 HP で初期化してください。

<details markdown="1">
<summary>解答を見る</summary>

1. コンパイルエラーになる。`Item(string, int)` というパラメータありのコンストラクタを定義した時点でデフォルトコンストラクタが自動生成されなくなるため、`new Item()` は引数不足でエラーになる。

2. ```csharp
   public Weapon(string weaponName, int weaponDamage)
   {
       name = weaponName;
       damage = weaponDamage;
   }
   ```

3. ```csharp
   class Player
   {
       public string name;
       public int hp;
       public int maxHp;

       public Player(string playerName, int initialHp)
       {
           name = playerName;
           hp = initialHp;
           maxHp = initialHp;
       }
   }
   ```

</details>

---

## 次のステップ

[アクセス修飾子](/unity-csharp-learning/csharp/access-modifiers/) では、フィールドやメソッドへのアクセス範囲を制御してクラスを安全に設計する方法を学びます。
