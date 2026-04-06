---
layout: page
title: クラスとオブジェクト
permalink: /csharp/classes/
---

# クラスとオブジェクト

変数が増えるにつれて「プレイヤーの名前・HP・スコア」のように**関連する値がバラバラに散らばる**問題が起きます。**クラス**を使うと、関連するデータと処理をひとつにまとめて扱えます。C# のプログラムは、このクラスを中心に設計されています。

## 学習目標

- クラスを定義してインスタンスを生成できる
- フィールドにデータを格納・参照できる
- クラスにメソッドを定義して呼び出せる
- オーバーロードでメソッドを使い分けられる
- コンストラクタでインスタンスを適切に初期化できる
- `public` と `private` の違いを理解してアクセス修飾子を使い分けられる

## 前提知識

- [配列の基礎](/unity-csharp-learning/csharp/arrays/) を読んでいること

---

## 1. クラスとインスタンス

### クラスが必要な理由

ゲームのプレイヤーを表すデータを変数で管理するとします。

```csharp
string player1Name = "Alice";
int player1Hp = 100;
int player1Score = 0;

string player2Name = "Bob";
int player2Hp = 80;
int player2Score = 0;
```

プレイヤーが増えるたびに変数が倍増し、どの変数がどのプレイヤーのものか一目でわかりません。**クラス**を使えば「プレイヤーとは何か」を一箇所に定義し、何人でも簡単に作れます。

### クラスの定義

**書式：クラスの定義**
```
class クラス名
{
    // フィールド・メソッド・コンストラクタ
}
```

| 要素 | 説明 |
|---|---|
| `class` | クラスを定義するキーワード |
| `クラス名` | 慣例として PascalCase（先頭大文字）で命名する |
| `{ }` | クラスの本体。フィールドやメソッドをここに書く |

まず空のクラスを定義してみましょう。

```csharp
class Player
{
}
```

これだけで `Player` という新しい型が作られます。

### インスタンスの生成

クラスは**設計図**に過ぎません。実際に使うには `new` でインスタンス（実体）を作ります。

**書式：インスタンスの生成**
```
クラス名 変数名 = new クラス名();
```

```csharp
Player p = new Player();
```

`new Player()` が「設計図をもとに実体を 1 つ作る」操作です。変数 `p` にそのインスタンスが格納されます。

> 💡 **ポイント**: C# 9 以降では `Player p = new();` と右辺の型名を省略できます（ターゲット型指定 new）。

### フィールド

クラスにデータを持たせるには**フィールド**を定義します。

**書式：フィールドの定義**
```
アクセス修飾子 型 フィールド名;
```

```csharp
class Player
{
    public string name;
    public int hp;
    public int score;
}
```

フィールドには `.`（ドット）でアクセスします。

```csharp
Player p1 = new Player();
p1.name = "Alice";
p1.hp = 100;
p1.score = 0;

Player p2 = new Player();
p2.name = "Bob";
p2.hp = 80;
p2.score = 0;

Console.WriteLine($"{p1.name}: HP={p1.hp}, Score={p1.score}");
Console.WriteLine($"{p2.name}: HP={p2.hp}, Score={p2.score}");
```

```
Alice: HP=100, Score=0
Bob: HP=80, Score=0
```

`p1` と `p2` はそれぞれ**独立したインスタンス**です。片方の値を変えてももう片方には影響しません。

```csharp
p1.score = 50;
Console.WriteLine($"p1.score={p1.score}");
Console.WriteLine($"p2.score={p2.score}");
```

```
p1.score=50
p2.score=0
```

> 💡 **アクセス修飾子について**: `public` はクラスの外からアクセスできることを示します。アクセス修飾子の詳細は[セクション 4](#4-アクセス修飾子)で解説します。

---

## 2. メソッド

### メソッドが必要な理由

「HP をダメージ分だけ減らす」処理を複数箇所で書くとします。

```csharp
p1.hp = p1.hp - 10;
p2.hp = p2.hp - 10;
```

同じ処理を繰り返し書くのは非効率で、修正漏れの原因にもなります。処理をクラスに**メソッド**として定義すれば、名前を呼ぶだけで何度でも再利用できます。

### 戻り値なし・パラメータなし

最もシンプルなメソッドは「何かを実行するだけ」のものです。戻り値がない場合は `void` を使います。

**書式：メソッドの定義（基本）**
```
アクセス修飾子 戻り値の型 メソッド名()
{
    処理
}
```

```csharp
class Player
{
    public string name;
    public int hp;

    public void Greet()
    {
        Console.WriteLine($"こんにちは、{name}です！");
    }
}
```

```csharp
Player p = new Player();
p.name = "Alice";
p.Greet();
```

```
こんにちは、Aliceです！
```

メソッド内からは同じインスタンスのフィールドに直接アクセスできます。

### パラメータ

処理に外から値を渡したいときは**パラメータ**を定義します。

```csharp
class Player
{
    public string name;
    public int hp;

    public void TakeDamage(int damage)
    {
        hp = hp - damage;
        Console.WriteLine($"{name} が {damage} ダメージを受けた。残りHP={hp}");
    }
}
```

```csharp
Player p = new Player();
p.name = "Alice";
p.hp = 100;
p.TakeDamage(10);
p.TakeDamage(25);
```

```
Alice が 10 ダメージを受けた。残りHP=90
Alice が 25 ダメージを受けた。残りHP=65
```

### 戻り値

処理の結果を呼び出し元に返したいときは、`void` の代わりに返す型を書き、`return` で値を返します。

**書式：メソッドの定義（戻り値あり）**
```
アクセス修飾子 戻り値の型 メソッド名(パラメータ)
{
    処理
    return 値;
}
```

```csharp
class Player
{
    public string name;
    public int hp;
    public int score;

    public bool IsAlive()
    {
        return hp > 0;
    }

    public string GetStatus()
    {
        return $"{name}: HP={hp}, Score={score}";
    }
}
```

```csharp
Player p = new Player();
p.name = "Alice";
p.hp = 100;
p.score = 50;

Console.WriteLine(p.GetStatus());
Console.WriteLine($"生存中={p.IsAlive()}");

p.hp = 0;
Console.WriteLine($"生存中={p.IsAlive()}");
```

```
Alice: HP=100, Score=50
生存中=True
生存中=False
```

`return` に達するとメソッドはそこで終了します。残りの処理は実行されません。

### シグネチャとオーバーロード

**シグネチャ**とは、メソッドを識別するための「メソッド名＋パラメータの型の並び」のことです。

```
TakeDamage(int)         → シグネチャ: TakeDamage(int)
TakeDamage(int, bool)   → シグネチャ: TakeDamage(int, bool)
```

C# では**同じクラス内にシグネチャが異なるメソッドを複数定義できます**。これを**オーバーロード**と呼びます。

```csharp
class Player
{
    public string name;
    public int hp;

    public void TakeDamage(int damage)
    {
        hp = hp - damage;
        Console.WriteLine($"{name} が {damage} ダメージ。残りHP={hp}");
    }

    public void TakeDamage(int damage, bool critical)
    {
        int actualDamage = critical ? damage * 2 : damage;
        hp = hp - actualDamage;
        Console.WriteLine($"{name} が {actualDamage} ダメージ（クリティカル={critical}）。残りHP={hp}");
    }
}
```

```csharp
Player p = new Player();
p.name = "Alice";
p.hp = 100;

p.TakeDamage(10);
p.TakeDamage(10, true);
```

```
Alice が 10 ダメージ。残りHP=90
Alice が 20 ダメージ（クリティカル=True）。残りHP=70
```

呼び出し時に渡した引数の型と数によって、どのメソッドが実行されるかが自動的に決まります。**戻り値の型だけが異なるメソッドはオーバーロードになりません**（コンパイルエラー）。

---

## 3. コンストラクタ

### 初期化の問題

現状のコードでは、インスタンスを生成した直後にフィールドへの代入を書き忘れると、意図しない初期値（`0` や `null`）のまま使ってしまうリスクがあります。

```csharp
Player p = new Player();
// name を設定し忘れた
p.TakeDamage(10);  // name が null のまま使われる可能性がある
```

**コンストラクタ**は `new` が実行されるとき**自動的に呼び出されるメソッド**です。コンストラクタにパラメータを設けることで「必要な値を必ず渡さなければインスタンスを作れない」ように強制できます。

### コンストラクタの定義

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

`new Player("Alice", 100)` と書くだけで `name` と `hp` が確実に設定されます。

### デフォルトコンストラクタ

コンストラクタを**一つも定義しない**場合、コンパイラが自動的に**パラメータなしの空のコンストラクタ**（デフォルトコンストラクタ）を生成します。

```csharp
class Enemy
{
    public string name;
    // コンストラクタを定義していない → デフォルトコンストラクタが自動生成される
}

Enemy e = new Enemy();  // OK
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

// ❌ コンパイルエラー: パラメータなしのコンストラクタは存在しない
Player p = new Player();

// ✅ OK
Player p = new Player("Alice");
```

---

## 4. アクセス修飾子

### カプセル化の意義

フィールドをすべて `public` にすると、クラスの外から自由に値を書き換えられます。

```csharp
Player p = new Player("Alice", 100);
p.hp = -9999;  // 不正な値でも制限なく代入できてしまう
```

HP が負の値になることを防ぐには、フィールドを外から直接変更させないようにする必要があります。これを**カプセル化**と言います。`private` にしたフィールドはクラスの外からはアクセスできず、クラス内のメソッドを通じてのみ操作できます。

### public と private

| 修飾子 | アクセスできる範囲 |
|---|---|
| `public` | どこからでもアクセスできる |
| `private` | そのクラスの内部からのみアクセスできる |

フィールドを `private` にし、操作用の `public` メソッドを提供することで、不正な値の混入を防げます。

```csharp
class Player
{
    public string name;
    private int _hp;  // 外から直接変更させない

    public Player(string playerName, int initialHp)
    {
        name = playerName;
        _hp = initialHp;
    }

    public void TakeDamage(int damage)
    {
        if (damage < 0) { return; }   // 不正な値を弾く
        _hp = _hp - damage;
        if (_hp < 0) { _hp = 0; }    // 0 未満にならないよう制限
        Console.WriteLine($"{name}: 残りHP={_hp}");
    }

    public int GetHp()
    {
        return _hp;
    }
}
```

```csharp
Player p = new Player("Alice", 100);
p.TakeDamage(30);
p.TakeDamage(200);
Console.WriteLine($"HP={p.GetHp()}");
```

```
Alice: 残りHP=70
Alice: 残りHP=0
HP=0
```

> 💡 **命名規則**: `private` フィールドにはアンダースコアをつけた camelCase（`_hp`, `_score`）で命名する慣例があります。

### フィールドのデフォルトアクセス修飾子

アクセス修飾子を省略した場合、クラスのメンバーは `private` として扱われます。

```csharp
class Enemy
{
    int hp;          // private int hp; と同じ
    public string name;
}
```

明示的に書く方が意図が伝わりやすいため、アクセス修飾子は省略しないようにしましょう。

### その他のアクセス修飾子

`public` と `private` 以外にも次のような修飾子があります。今の段階では名前と概要だけ把握しておけば十分です。

| 修飾子 | アクセスできる範囲 | 主な用途 |
|---|---|---|
| `public` | どこからでも | 外部に公開したいメンバー |
| `private` | 同じクラス内のみ | 実装の詳細を隠蔽する |
| `internal` | 同じアセンブリ（プロジェクト）内 | ライブラリの内部実装など |
| `protected` | 同じクラスと派生クラス | 継承を使った設計（継承については別の章で扱います） |

---

## よくあるミス

### フィールドとローカル変数を混同する

```csharp
class Player
{
    public int hp;

    public void Reset()
    {
        int hp = 100;  // ❌ NG: フィールドではなくローカル変数 hp を新たに作っている
    }

    public void ResetCorrect()
    {
        hp = 100;      // ✅ OK: フィールドの hp に代入
        // または this.hp = 100; と明示的に書くこともできる
    }
}
```

メソッド内で同名のローカル変数を宣言するとフィールドが隠れます。フィールドを明示したい場合は `this.フィールド名` と書きます。

### コンストラクタに戻り値型を書いてしまう

```csharp
class Player
{
    // ❌ NG: void を書くと通常のメソッドになりコンストラクタではなくなる
    public void Player(string name) { }

    // ✅ OK: 戻り値型なし
    public Player(string name) { }
}
```

---

## まとめ

- **クラス** — 関連するデータ（フィールド）と処理（メソッド）をひとまとめにした設計図
- **インスタンス** — `new クラス名()` で作られたクラスの実体。それぞれ独立して値を持つ
- **フィールド** — クラスが保持するデータ。インスタンスごとに独立した値を持つ
- **メソッド** — クラスに定義する処理。パラメータと戻り値を持てる
- **シグネチャ** — メソッド名＋パラメータの型の組み合わせ。オーバーロードの識別に使われる
- **オーバーロード** — 同じクラス内に異なるシグネチャの同名メソッドを複数定義すること
- **コンストラクタ** — `new` 時に自動呼び出されるメソッド。クラス名と同名で戻り値なし
- **`public`** — どこからでもアクセス可能
- **`private`** — クラス内部からのみアクセス可能。カプセル化に使う

---

## 理解度チェック

1. 次のコードの出力結果を答えてください。

   ```csharp
   class Item
   {
       public string name;
       public int price;
   }

   Item a = new Item();
   a.name = "Sword";
   a.price = 100;

   Item b = a;
   b.price = 200;

   Console.WriteLine($"a.price={a.price}");
   Console.WriteLine($"b.price={b.price}");
   ```

2. 次のクラスに `Heal(int amount)` メソッドを追加してください。HP が `maxHp` を超えないよう制限すること。

   ```csharp
   class Player
   {
       public string name;
       public int hp;
       public int maxHp;

       public Player(string playerName, int initialHp)
       {
           name = playerName;
           maxHp = initialHp;
           hp = initialHp;
       }
   }
   ```

3. （応用）`name` と `damage` を持つ `Weapon` クラスを定義してください。コンストラクタで両方を初期化し、`PrintInfo()` メソッドで `name=Sword, damage=30` の形式で出力するようにしてください。

<details markdown="1">
<summary>解答を見る</summary>

1. `a.price=200` と `b.price=200`。クラスは**参照型**のため `b = a` でコピーされるのは参照です。`a` と `b` は同じインスタンスを指しているため、どちらから変更しても両方に影響します。

2. ```csharp
   public void Heal(int amount)
   {
       hp = hp + amount;
       if (hp > maxHp) { hp = maxHp; }
   }
   ```

3. ```csharp
   class Weapon
   {
       public string name;
       public int damage;

       public Weapon(string weaponName, int weaponDamage)
       {
           name = weaponName;
           damage = weaponDamage;
       }

       public void PrintInfo()
       {
           Console.WriteLine($"name={name}, damage={damage}");
       }
   }
   ```

</details>

---

## 次のステップ

[プロパティ（補足）](/unity-csharp-learning/csharp/properties/) では、`private` フィールドに対する読み書きをより簡潔に記述できる `get`/`set` アクセサーを学びます。
