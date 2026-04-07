---
layout: page
title: プロパティ
permalink: /csharp/properties/
---

# プロパティ

`GetHp()` のようなゲッターメソッドの代わりに、フィールドのように読み書きできる構文が**プロパティ**です。見た目はフィールドへのアクセスと同じでも、内部では `get` / `set` アクセサーが動きます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- プロパティを使って `private` フィールドを安全に外部へ公開できる
- `get` / `set` アクセサーの書き方を理解できる
- 自動実装プロパティ（`{ get; set; }`）を使える
- 読み取り専用プロパティの2種類のパターンを説明できる

## 前提知識

- [アクセス修飾子](/unity-csharp-learning/csharp/access-modifiers/) を読んでいること

---

## 1. 読み取り（getter）・書き込み（setter）メソッドの課題

前のページでは `private int _hp;` を守る（外部から勝手に書き換えられないようにする）ために、次のようなメソッドを書きました。このようなパターンは多くのプログラミング言語に共通する手法で、データを読み取る `GetHp()` メソッドのことを getter、データを設定する `SetHp()` メソッドのことを setter と呼びます。

```csharp
private int _hp; // バッキングフィールド
public int GetHp()
{
    return _hp;
}

public void SetHp(int value)
{
    if (value < 0) { value = 0; }
    _hp = value;
}
```

上記のように GetHp() と SetHp() で読み書きする _hp フィールドのことをバッキングフィールドと呼びます。バッキングフィールドは private で外部には非公開にして、公開メソッドを通して読み書きされます。

これらは以下のパターンとして定義できます。

```csharp
private 型 _名前; // バッキングフィールド
public 型 Get名前() { return _名前; } // getter
public void Set名前(型 value) { _名前 = value; } // setter
```

このパターンは問題なく機能しますが、呼び出しが少し冗長になります。また、同じフィールドを読み書きする関連した機能ですが文法上は分離されている個別のメソッドなので対称性を保証しません。例えば GetHp() に対して SetHP() や SetHitPoint() のようにパターンに従わない実装もできてしまいます。

また、フィールドと比較して値の設定に = 演算子を使えないので面倒になる点があります。

```csharp
Console.WriteLine(p.GetHp());        // 読み取り
p.SetHp(p.GetHp() - 30);             // 書き込み（計算が絡むと読みにくい）
```

こうした問題を解決するのが**プロパティ**です。プロパティを使うと、フィールドのように `p.Hp` と書けるようになります。

```csharp
Console.WriteLine(p.Hp);   // 読み取り
p.Hp -= 30;                // 書き込み（自然な書き方）
```

---

## 2. プロパティの基本構文（バッキングフィールドあり）

**書式：プロパティの定義**
```
アクセス修飾子 型 プロパティ名
{
    get { return バッキングフィールド; }
    set { バッキングフィールド = value; }
}
```

| 要素 | 説明 |
|---|---|
| `get` アクセサー | プロパティの値を**読み取る**ときに実行されるブロック |
| `set` アクセサー | プロパティに値を**書き込む**ときに実行されるブロック |
| `value` | `set` アクセサー内で使える特殊な変数。書き込もうとした値が入っている。型はプロパティの型と同じ（`int Hp` なら `value` も `int`） |
| バッキングフィールド | プロパティが実際に値を保存する `private` フィールド |

`value` は C# が予約している名前で、`set` ブロック内でのみ使えます。

```csharp
class Player
{
    private int _hp;

    public int Hp
    {
        get { return _hp; }
        set
        {
            if (value < 0) { value = 0; }  // バリデーション
            _hp = value;
        }
    }
}
```

```csharp
Player p = new Player();
p.Hp = 100;
Console.WriteLine(p.Hp);   // 100
p.Hp = -50;                // セッターがバリデーションを適用
Console.WriteLine(p.Hp);   // 0
```

```
100
0
```

`GetHp()` / `SetHp()` との最大の違いは、**バリデーションを1か所に集約しながら**フィールドのように自然に読み書きできる点です。

---

## 3. 自動実装プロパティ

バリデーションが不要な場合は `get` / `set` の本体を省略できます。コンパイラが自動でバッキングフィールドを生成します。

**書式：自動実装プロパティ**
```
アクセス修飾子 型 プロパティ名 { get; set; }
```

| 要素 | 説明 |
|---|---|
| `{ get; set; }` | `get` と `set` の本体を省略した短縮形。バッキングフィールドはコンパイラが生成する |

```csharp
class Player
{
    public string Name { get; set; }
    public int Level { get; set; }
}
```

```csharp
Player p = new Player();
p.Name = "Alice";
p.Level = 5;
Console.WriteLine($"{p.Name} (Lv.{p.Level})");   // Alice (Lv.5)
```

```
Alice (Lv.5)
```

バッキングフィールドはコンパイラが生成するため、自分で `private string _name;` などを書く必要はありません。

---

## 4. 読み取り専用プロパティ

クラスの外から値を変更させたくない場合は、`set` アクセサーを省略するか制限します。

### パターン①：`{ get; }` — 完全な読み取り専用

`set` を書かないと、クラスの外からも内からも代入できません。コンストラクター内での初期化のみ可能です。

> ※ コンストラクターはオブジェクトが「生まれる瞬間」だけ実行される特別なメソッドです。C# はその瞬間だけ `{ get; }` プロパティへの代入を許可しています。

```csharp
class Player
{
    public string Name { get; }

    public Player(string name)
    {
        Name = name;   // ✅ コンストラクター内のみ代入できる
    }
}
```

```csharp
Player p = new Player("Alice");
Console.WriteLine(p.Name);   // Alice
// p.Name = "Bob";            // ❌ コンパイルエラー
```

```
Alice
```

### パターン②：`{ get; private set; }` — クラス内からは変更可能

クラスの外からは読み取り専用ですが、クラスのメソッド内からは変更できます。

```csharp
class Player
{
    public int Hp { get; private set; }

    public Player(int hp)
    {
        Hp = hp;
    }

    public void TakeDamage(int damage)
    {
        Hp -= damage;                    // ✅ クラス内から変更できる
        if (Hp < 0) { Hp = 0; }
    }
}
```

```csharp
Player p = new Player(100);
p.TakeDamage(30);
Console.WriteLine(p.Hp);   // 70
// p.Hp = 999;              // ❌ コンパイルエラー（クラスの外からは書けない）
```

```
70
```

> 💡 バリデーションが必要な場合は、バッキングフィールドを使うパターン（セクション2）と組み合わせてください。

> 💡 **`init` アクセサー（C# 9 から）**: `set` の代わりに `init` と書くと、コンストラクターまたはオブジェクト初期化子（`new Player { Hp = 100 }` の形）でのみ代入できるプロパティを作れます。`{ get; init; }` のように使います。詳しくは後の章で扱います。

---

## 5. プロパティを使ったクラス設計の例

ここまでの内容をまとめた `Player` クラスの完成形です。

```csharp
class Player
{
    // 完全な読み取り専用（外からも内からも変更不可）
    public string Name { get; }

    // バリデーションあり（0〜MaxHp の範囲に収める）
    private int _hp;
    public int Hp
    {
        get { return _hp; }
        set
        {
            // ※ 条件ごとに早期リターンするスタイル（セクション2の if + 代入と同じ意味）
            if (value < 0) { _hp = 0; return; }
            if (value > MaxHp) { _hp = MaxHp; return; }
            _hp = value;
        }
    }

    // クラス内からのみ変更可能
    public int MaxHp { get; private set; }

    // バリデーション不要な自動実装プロパティ
    public int Level { get; set; }

    public Player(string name, int maxHp)
    {
        Name = name;
        MaxHp = maxHp;
        Hp = maxHp;
        Level = 1;
    }

    public void TakeDamage(int damage)
    {
        Hp -= damage;   // プロパティ経由なのでバリデーションが自動で走る
        Console.WriteLine($"{Name} が {damage} ダメージ。残りHP={Hp}/{MaxHp}");
    }

    public void LevelUp()
    {
        Level++;
        MaxHp += 20;
        Hp = MaxHp;    // HP を最大値まで回復
        Console.WriteLine($"{Name} がレベル {Level} になった！ MaxHP={MaxHp}");
    }
}
```

```csharp
Player p = new Player("Alice", 100);
p.TakeDamage(30);
p.TakeDamage(200);   // 0 未満にはならない
p.LevelUp();
```

```
Alice が 30 ダメージ。残りHP=70/100
Alice が 200 ダメージ。残りHP=0/100
Alice がレベル 2 になった！ MaxHP=120
```

---

## よくあるミス

### ミス①：バッキングフィールドをプロパティ自身で返す（無限再帰）

```csharp
// ❌ NG: プロパティが自分自身を呼び出して無限ループになる
public int Hp
{
    get { return Hp; }   // Hp（プロパティ）を呼ぶと再び get が実行される
    set { Hp = value; }
}

// ✅ OK: バッキングフィールド（_hp）を使う
private int _hp;
public int Hp
{
    get { return _hp; }
    set { _hp = value; }
}
```

### ミス②：`set` の中で `value` の代わりにプロパティ名を使う

```csharp
// ❌ NG: value ではなく Hp を参照してしまっている
set { _hp = Hp; }   // 書き込もうとした値ではなく現在値が入る

// ✅ OK: 書き込もうとした値は value
set { _hp = value; }
```

### ミス③：自動実装プロパティと不要なバッキングフィールドを両方書く

```csharp
// ❌ NG: 自動実装プロパティがあるのにフィールドが重複している
private string _name;
public string Name { get; set; }

// ✅ OK: バリデーションが不要なら自動実装プロパティだけでよい
public string Name { get; set; }
```

---

## まとめ

- **プロパティ**は `get` / `set` アクセサーを持つ「フィールドのように使えるメソッド」
- `set` アクセサー内の `value` は、書き込もうとした値が入る特殊な変数
- `get` / `set` の本体を省略した**自動実装プロパティ**はバリデーション不要な場面で便利
- **読み取り専用**にするには `{ get; }` または `{ get; private set; }` を使い分ける
- バッキングフィールドはプロパティとは別の `private` フィールドであることに注意

---

## 理解度チェック

以下の問いに答えられるか確認しましょう。

1. 次のプロパティは何が問題ですか？

   ```csharp
   public int Score
   {
       get { return Score; }
       set { Score = value; }
   }
   ```

2. `{ get; private set; }` と `{ get; }` の違いは何ですか？

3. 次のクラスの `_level` フィールドをプロパティ `Level`（クラス外から読み取り専用、クラス内から変更可能）に書き直してください。

   ```csharp
   class Hero
   {
       private int _level = 1;

       public int GetLevel() { return _level; }
       public void LevelUp() { _level++; }
   }
   ```

4. （応用）`Name`（外から変更不可）と `Score`（0 未満にならないバリデーションつき）を持つ `GameRecord` クラスをプロパティで設計してください。

<details markdown="1">
<summary>解答を見る</summary>

1. `get` の中で `Score`（プロパティ自身）を返しているため、`get` が呼ばれるたびに再び `get` が呼ばれ、無限再帰によるスタックオーバーフローになる。バッキングフィールドを用意して `return _score;` とすべき。

2. `{ get; }` はクラスの**内外どちらからも**代入できない（コンストラクター内での初期化のみ可）。`{ get; private set; }` はクラスの外からは読み取り専用だが、クラス内のメソッドからは変更できる。

3. ```csharp
   class Hero
   {
       public int Level { get; private set; } = 1;

       public void LevelUp() { Level++; }
   }
   ```

4. ```csharp
   class GameRecord
   {
       public string Name { get; }

       private int _score;
       public int Score
       {
           get { return _score; }
           set
           {
               if (value < 0) { _score = 0; return; }
               _score = value;
           }
       }

       public GameRecord(string name)
       {
           Name = name;
       }
   }
   ```

</details>

---

## 次のステップ

**継承**（準備中）では、クラスを拡張して共通の機能を再利用する方法を学びます。
