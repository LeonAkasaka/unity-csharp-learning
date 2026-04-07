---
layout: page
title: アクセス修飾子
permalink: /csharp/access-modifiers/
---

# アクセス修飾子

フィールドをすべて `public` にしておくと、クラスの外から自由に書き換えられてしまいます。

```csharp
Player p = new Player("Alice", 100);
p.hp = 999999;  // ゲームバランスを無視した書き換えが可能
```

**アクセス修飾子**はフィールドやメソッドへの「アクセス可能な範囲」を制限する仕組みです。外から触ってよい部分だけを公開し、内部の詳細を隠すことを**カプセル化**と呼びます。カプセル化によってクラスの誤用を防ぎ、後から内部実装を変更しやすくなります。

## 学習目標

- `public` と `private` の違いを説明できる
- `private` フィールドをメソッド経由で操作できる
- デフォルトのアクセスレベルを理解できる

## 前提知識

- [コンストラクタ](/unity-csharp-learning/csharp/constructors/) を読んでいること

---

## 1. `public` と `private`

| 修飾子 | アクセス可能な範囲 |
|---|---|
| `public` | どこからでもアクセスできる |
| `private` | 同じクラス内からのみアクセスできる |

**書式：フィールドの定義（修飾子あり）**
```
アクセス修飾子 型 フィールド名;
```

```csharp
class Player
{
    public string name;    // クラスの外から読み書きできる
    private int _hp;       // クラスの外から直接触れない
    private int _maxHp;

    public Player(string playerName, int initialHp)
    {
        name = playerName;
        _hp = initialHp;
        _maxHp = initialHp;
    }

    public void TakeDamage(int damage)
    {
        if (damage < 0) { return; }  // 不正な値を弾く
        _hp = _hp - damage;
        if (_hp < 0) { _hp = 0; }
        Console.WriteLine($"{name} が {damage} ダメージ。残りHP={_hp}");
    }

    public void Heal(int amount)
    {
        if (amount < 0) { return; }
        _hp = _hp + amount;
        if (_hp > _maxHp) { _hp = _maxHp; }
        Console.WriteLine($"{name} が {amount} 回復。残りHP={_hp}");
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
p.Heal(20);
Console.WriteLine($"現在HP={p.GetHp()}");

// p._hp = 999;  // ❌ コンパイルエラー: private フィールドには外からアクセスできない
```

```
Alice が 30 ダメージ。残りHP=70
Alice が 20 回復。残りHP=90
現在HP=90
```

`_hp` を `private` にすることで `TakeDamage` と `Heal` だけが値を変更できるようになりました。不正な値のチェックをメソッド内に集約でき、バグを防ぎやすくなります。

> 📌 C# では `private` フィールドの命名に `_camelCase`（アンダースコア＋小文字始まり）を使うことが多いです。

---

## 2. デフォルトのアクセスレベル

アクセス修飾子を省略した場合のデフォルトは次のとおりです。

| 要素 | 省略時のデフォルト |
|---|---|
| クラスのメンバー（フィールド・メソッド） | `private` |
| クラス自体（トップレベル） | `internal` |

意図しない公開を防ぐため、フィールドには明示的に `private` と書く習慣をつけましょう。

---

## 3. その他のアクセス修飾子

Unity や中規模以上のプロジェクトでは以下の修飾子も登場します。現段階では存在だけ確認してください。

| 修飾子 | アクセス可能な範囲 |
|---|---|
| `internal` | 同じアセンブリ（プロジェクト）内からのみ |
| `protected` | 同じクラスおよびそのクラスを継承したクラスから |
| `protected internal` | `protected` と `internal` の和（どちらかを満たせばOK） |
| `private protected` | 同じアセンブリ内かつ派生クラスのみ |

`internal` は複数プロジェクトを組み合わせる場面で、`protected` は継承（派生クラス）の学習後に詳しく扱います。

---

## まとめ

- **`public`** — どこからでもアクセス可能。外部に公開したいメソッドや情報に使う
- **`private`** — クラス内部からのみアクセス可能。フィールドは基本的に `private` にする
- **カプセル化** — 内部状態を隠してメソッドを通じてのみ操作させる設計の考え方
- アクセス修飾子を省略したメンバーは `private` 扱いになる

---

## 理解度チェック

1. 次のコードはコンパイルエラーになりますか？ 理由とともに答えてください。

   ```csharp
   class Box
   {
       private int _value;

       public Box(int v) { _value = v; }
   }

   Box b = new Box(10);
   Console.WriteLine(b._value);
   ```

2. 下のクラスの `_score` フィールドを `private` に変更し、スコアを加算する `AddScore(int points)` メソッドと現在のスコアを返す `GetScore()` メソッドを追加してください。

   ```csharp
   class Player
   {
       public string name;
       public int _score;  // ← private に変える
   }
   ```

3. （応用）`private` フィールド `_level`（初期値 1）と `_exp` を持つ `Character` クラスを定義してください。`AddExp(int amount)` メソッドで `_exp` を加算し、100 以上になったら `_level` を 1 上げて `_exp` を 0 にリセットするよう実装してください。

<details markdown="1">
<summary>解答を見る</summary>

1. コンパイルエラーになる。`_value` は `private` であり、クラスの外（`Box` クラスの外）からアクセスできない。

2. ```csharp
   class Player
   {
       public string name;
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

3. ```csharp
   class Character
   {
       public string name;
       private int _level;
       private int _exp;

       public Character(string charName)
       {
           name = charName;
           _level = 1;
           _exp = 0;
       }

       public void AddExp(int amount)
       {
           _exp = _exp + amount;
           if (_exp >= 100)
           {
               _level = _level + 1;
               _exp = 0;
               Console.WriteLine($"{name} がレベル {_level} になった！");
           }
       }

       public string GetStatus()
       {
           return $"{name}: Lv={_level}, EXP={_exp}";
       }
   }
   ```

</details>

---

## 次のステップ

**プロパティ**（`get` / `set`）を使うと、`private` フィールドへの読み書きをより簡潔に安全に記述できます。
次のページで学びましょう → [プロパティ](/unity-csharp-learning/csharp/properties/)
