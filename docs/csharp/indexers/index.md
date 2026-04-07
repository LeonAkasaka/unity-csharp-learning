---
layout: page
title: インデクサ
permalink: /csharp/indexers/
---

# インデクサ

配列と同じように `[]` でアクセスできる、自作クラスを作るための仕組みが**インデクサ**です。プロパティが「フィールドを外部に公開する構文」なら、インデクサは「内部の配列やデータを外部に公開する構文」と言えます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- インデクサの構文を理解し、自作クラスに `[]` でアクセスできるようにできる
- `int` キーと `string` キーの両方でインデクサを定義できる
- 読み取り専用インデクサを作れる
- よくある実装ミス（範囲外アクセス・`this` の書き忘れ）を避けられる

## 前提知識

- [プロパティ](/unity-csharp-learning/csharp/properties/) を読んでいること

---

## 1. インデクサが必要になる場面

配列は `items[0]` のように `[]` で要素にアクセスできます。

```csharp
string[] items = { "剣", "盾", "回復薬" };
Console.WriteLine(items[0]);   // 剣
```

自分で作ったクラスも同じように `inventory[0]` と書けたら自然ですが、普通のクラスはそのままでは `[]` をサポートしていません。

```csharp
Inventory inventory = new Inventory(4);
// inventory[0] = "剣";   // ❌ このままではコンパイルエラー
```

これを可能にするのが**インデクサ**です。プロパティが「フィールド名でアクセスする窓口」なら、インデクサは「インデックス（番号やキー）でアクセスする窓口」です。

---

## 2. インデクサの基本構文

**書式：インデクサの定義**
```
アクセス修飾子 型 this[インデックスの型 パラメーター名]
{
    get { ... }
    set { ... }
}
```

| 要素 | 説明 |
|---|---|
| `this` | インデクサであることを示すキーワード。プロパティとの最大の違い |
| `インデックスの型` | `[]` の中に書く値の型。`int` が最も一般的で、`string` も使える |
| `パラメーター名` | インデックスの値を受け取るローカル変数名（`index`、`key` など） |
| `get` アクセサー | `[]` で読み取るときに実行されるブロック |
| `set` アクセサー | `[]` で書き込むときに実行されるブロック。書き込む値は `value` で参照する |

プロパティと同じく、`value` は `set` アクセサー内でのみ使える特殊な変数です。

---

## 3. int インデックスのインデクサ

### 最もシンプルな例

ゲームのアイテム欄を管理する `Inventory` クラスで試してみましょう。

```csharp
class Inventory
{
    private string[] _slots;

    public Inventory(int size)
    {
        _slots = new string[size];
    }

    public string this[int index]
    {
        get { return _slots[index]; }
        set { _slots[index] = value; }
    }
}
```

```csharp
Inventory inventory = new Inventory(4);
inventory[0] = "剣";
inventory[1] = "盾";
inventory[2] = "回復薬";

Console.WriteLine(inventory[0]);   // 剣
Console.WriteLine(inventory[2]);   // 回復薬
```

```
剣
回復薬
```

`[]` でアクセスするたびに、インデクサの `get` / `set` が呼ばれます。内部の `_slots` 配列は `private` なので、外部からは直接触れません。

### Length プロパティと組み合わせる

コレクションらしく扱えるよう、`Length` プロパティも追加するとより便利です。
前の例に `Length` プロパティを1つ加えた版です。

```csharp
class Inventory
{
    private string[] _slots;

    public int Length => _slots.Length;   // ★ 追加

    public Inventory(int size)
    {
        _slots = new string[size];
    }

    public string this[int index]
    {
        get { return _slots[index]; }
        set { _slots[index] = value; }
    }
}
```

```csharp
Inventory inventory = new Inventory(3);
inventory[0] = "剣";
inventory[1] = "盾";
inventory[2] = "回復薬";

for (int i = 0; i < inventory.Length; i++)
{
    Console.WriteLine($"スロット{i}: {inventory[i]}");
}
```

```
スロット0: 剣
スロット1: 盾
スロット2: 回復薬
```

---

## 4. バリデーションを入れる

プロパティと同様に、インデクサの `get` / `set` の中に処理を書けます。範囲外アクセスを防ぐバリデーションを入れてみましょう。
前の例の `get` / `set` に範囲チェックを追加したものです。

```csharp
class Inventory
{
    private string[] _slots;

    public int Length => _slots.Length;

    public Inventory(int size)
    {
        _slots = new string[size];
    }

    public string this[int index]
    {
        get
        {
            if (index < 0 || index >= _slots.Length)
            {
                Console.WriteLine("範囲外のインデックスです。");
                return string.Empty;
            }
            return _slots[index];
        }
        set
        {
            if (index < 0 || index >= _slots.Length)
            {
                Console.WriteLine("範囲外のインデックスです。");
                return;
            }
            _slots[index] = value;
        }
    }
}
```

```csharp
Inventory inventory = new Inventory(3);
inventory[0] = "剣";
inventory[5] = "魔法書";   // 範囲外 → メッセージが出るだけでクラッシュしない

Console.WriteLine(inventory[0]);   // 剣
Console.WriteLine(inventory[5]);   // 範囲外 → 空文字が返る
```

```
範囲外のインデックスです。
剣
範囲外のインデックスです。

```

---

## 5. string インデックスのインデクサ

インデックスの型は `int` に限りません。`string` を使えば、名前（文字列キー）でアクセスするインデクサも作れます。

```csharp
class StatusEffects
{
    private bool _poisoned;
    private bool _paralyzed;
    private bool _cursed;

    public bool this[string effectName]
    {
        get
        {
            // switch 式：「値 switch { 条件 => 結果, ... }」と書き、値を返します（C# 8 以降）
            return effectName switch
            {
                "poison"   => _poisoned,
                "paralyze" => _paralyzed,
                "curse"    => _cursed,
                _          => false
            };
        }
        set
        {
            switch (effectName)
            {
                case "poison":   _poisoned  = value; break;
                case "paralyze": _paralyzed = value; break;
                case "curse":    _cursed    = value; break;
            }
        }
    }
}
```

> ※ `switch` 式（`effectName switch { ... }`）は C# 8 から使えます。

```csharp
StatusEffects effects = new StatusEffects();
effects["poison"] = true;
effects["paralyze"] = false;

Console.WriteLine(effects["poison"]);    // True
Console.WriteLine(effects["paralyze"]); // False
Console.WriteLine(effects["curse"]);    // False
```

```
True
False
False
```

> 💡 文字列キーでデータを管理する場面では、`Dictionary<string, T>` が便利です。ただし、`Dictionary` の詳細はコレクション（`List<T>`・`Dictionary` など）の章（準備中）でさらに詳しく扱います。

---

## 6. 読み取り専用インデクサ

`set` アクセサーを省略すると、外部からの書き込みを禁止できます。プロパティの読み取り専用と同じ仕組みです。

```csharp
class ReadOnlyInventory
{
    private string[] _slots;

    public int Length => _slots.Length;

    public ReadOnlyInventory(string[] items)
    {
        _slots = items;
    }

    public string this[int index]
    {
        get { return _slots[index]; }
        // set がないので外部から書き込めない
    }
}
```

```csharp
ReadOnlyInventory inv = new ReadOnlyInventory(new[] { "炎の剣", "氷の盾" });
Console.WriteLine(inv[0]);   // 炎の剣
// inv[0] = "木の棒";          // ❌ コンパイルエラー（set がないため書き込み不可）
```

```
炎の剣
```

---

## よくあるミス

### ミス①：`this` を書き忘れてコンパイルエラー

```csharp
// ❌ NG: this がないとインデクサにならず、コンパイルエラー
public string [int index]
{
    get { return _slots[index]; }
    set { _slots[index] = value; }
}

// ✅ OK: this を必ず付ける
public string this[int index]
{
    get { return _slots[index]; }
    set { _slots[index] = value; }
}
```

### ミス②：範囲チェックを忘れて IndexOutOfRangeException

```csharp
// ❌ NG: 範囲チェックなし → 範囲外アクセスで例外が発生しクラッシュ
public string this[int index]
{
    get { return _slots[index]; }
    set { _slots[index] = value; }
}

// ✅ OK: 事前に範囲を確認する
public string this[int index]
{
    get
    {
        if (index < 0 || index >= _slots.Length) { return string.Empty; }
        return _slots[index];
    }
    set
    {
        if (index < 0 || index >= _slots.Length) { return; }
        _slots[index] = value;
    }
}
```

---

## まとめ

- **インデクサ**を使うと、自作クラスに `[]` でアクセスできるようになる
- 構文はプロパティに似ているが、プロパティ名の代わりに `this[型 パラメーター名]` と書く
- インデックスの型は `int` が一般的だが、`string` など他の型も使える
- `set` を省略すると**読み取り専用インデクサ**になる
- `get` / `set` の中にバリデーションを書いて範囲外アクセスを防ぐことが推奨される

---

## 理解度チェック

以下の問いに答えられるか確認しましょう。

1. インデクサの構文でプロパティと異なる点は何ですか？

2. 次のコードの出力結果は何になりますか？

   ```csharp
   class Counter
   {
       private int[] _counts = new int[3];

       public int this[int index]
       {
           get { return _counts[index]; }
           set { _counts[index] = value; }
       }
   }

   Counter c = new Counter();
   c[0] = 10;
   c[1] = 20;
   c[2] = c[0] + c[1];
   Console.WriteLine(c[2]);
   ```

3. 次の `ScoreBoard` クラスに、`string` キーで `int` スコアを読み書きできるインデクサを追加してください。

   ```csharp
   class ScoreBoard
   {
       private int _alice;
       private int _bob;
       private int _carol;

       // ここにインデクサを追加してください
   }
   ```

4. （応用）サイズが `3` の `Inventory` クラスで、`inventory[0]` から `inventory[2]` を読み取り専用にするにはどう書きますか？コンストラクターで初期値を受け取るようにしてください。

<details markdown="1">
<summary>解答を見る</summary>

1. プロパティ名の代わりに `this[インデックスの型 パラメーター名]` と書く点。`this` キーワードを使うのがインデクサの特徴。

2. `30`

3. ```csharp
   class ScoreBoard
   {
       private int _alice;
       private int _bob;
       private int _carol;

       public int this[string name]
       {
           get
           {
               return name switch
               {
                   "alice" => _alice,
                   "bob"   => _bob,
                   "carol" => _carol,
                   _       => 0
               };
           }
           set
           {
               switch (name)
               {
                   case "alice": _alice = value; break;
                   case "bob":   _bob   = value; break;
                   case "carol": _carol = value; break;
               }
           }
       }
   }
   ```

4. ```csharp
   class Inventory
   {
       private string[] _slots;

       public int Length => _slots.Length;

       public Inventory(string item0, string item1, string item2)
       {
           _slots = new[] { item0, item1, item2 };
       }

       public string this[int index]
       {
           get
           {
               if (index < 0 || index >= _slots.Length) { return string.Empty; }
               return _slots[index];
           }
           // set を書かないので読み取り専用
       }
   }
   ```

</details>

---

## 次のステップ

**静的メンバー（static）**（準備中）では、インスタンスを作らなくても使えるメソッドやフィールドの書き方を学びます。
