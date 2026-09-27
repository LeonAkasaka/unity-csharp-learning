---
layout: page
title: インデクサ
permalink: /csharp/indexers/
---

# インデクサ

**インデクサ**（indexer）は、自分で作ったクラスのインスタンスを、配列と同じように `[]` で読み書きできるようにする仕組みです。プロパティが「名前で値を読み書きする窓口」なら、インデクサは「番号やキーで値を読み書きする窓口」です。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- インデクサを定義し、自分で作ったクラスのインスタンスを `[]` で読み書きできる
- `int` と `string` の両方のインデックスでインデクサを定義できる
- 読み取り専用のインデクサを作れる

## 前提知識

- [プロパティ](/unity-csharp-learning/csharp/properties/) を読んでいること

---

## 1. インデクサが必要になる場面

配列は、`items[0]` のように `[]` で要素を読み書きできます。

```csharp
string[] items = { "剣", "盾", "回復薬" };
Console.WriteLine(items[0]);
```

```
剣
```

ゲームのアイテム欄を表す `Inventory` クラスを作り、中で配列を使うとします。配列を `private` にすると、クラスの外からは `inventory[0]` のように書けません。インデクサを定義すると、`Inventory` のインスタンスにも `[]` を使えるようになります。

---

## 2. インデクサを定義する

**書式：[インデクサの定義](https://learn.microsoft.com/dotnet/csharp/programming-guide/indexers)**
```
アクセス修飾子 型 this[インデックスの型 パラメータ名]
{
    get { /* 値を返す */ }
    set { /* value を保存する */ }
}
```

| 要素 | 説明 |
|---|---|
| `型` | `[]` で読み書きする値の型 |
| `this` | インデクサであることを表すキーワード。プロパティ名の代わりに書く |
| `インデックスの型 パラメータ名` | `[]` の中に書く値（インデックス）を受け取るパラメータ |
| `get` アクセサー | `[]` で読み取るときに実行される |
| `set` アクセサー | `[]` に書き込むときに実行される。書き込む値は `value` に入っている |

```csharp
Inventory inventory = new Inventory(4);
inventory[0] = "剣";
inventory[1] = "盾";
inventory[2] = "回復薬";

Console.WriteLine(inventory[0]);
Console.WriteLine(inventory[2]);

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

```
剣
回復薬
```

`inventory[0] = "剣";` では、`set` アクセサーが実行され、`index` に `0`、`value` に `"剣"` が入ります。`inventory[0]` を読むと、`get` アクセサーが実行されます。中の `_slots` 配列は `private` のままなので、クラスの外からは直接触れません。

### 要素の数を返すプロパティと組み合わせる

配列の `Length` のように、要素の数を返すプロパティも用意すると、`for` 文ですべての要素を処理できます。

```csharp
Inventory inventory = new Inventory(3);
inventory[0] = "剣";
inventory[1] = "盾";
inventory[2] = "回復薬";

for (int i = 0; i < inventory.Length; i++)
{
    Console.WriteLine($"スロット {i}: {inventory[i]}");
}

class Inventory
{
    private string[] _slots;

    public int Length
    {
        get { return _slots.Length; }
    }

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

```
スロット 0: 剣
スロット 1: 盾
スロット 2: 回復薬
```

---

## 3. インデックスを調べる

プロパティと同じく、`get` と `set` の中には処理を書けます。インデックスが範囲内かを調べて、範囲外のときは配列を使わないようにします。

```csharp
Inventory inventory = new Inventory(3);
inventory[0] = "剣";
inventory[5] = "魔法書";

Console.WriteLine(inventory[0]);
Console.WriteLine($"[{inventory[5]}]");

class Inventory
{
    private string[] _slots;

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
                Console.WriteLine($"{index} は範囲外のインデックスです。");
                return "";
            }
            return _slots[index];
        }
        set
        {
            if (index < 0 || index >= _slots.Length)
            {
                Console.WriteLine($"{index} は範囲外のインデックスです。");
                return;
            }
            _slots[index] = value;
        }
    }
}
```

```
5 は範囲外のインデックスです。
剣
5 は範囲外のインデックスです。
[]
```

範囲外のインデックスでも、`IndexOutOfRangeException` でプログラムが止まることはありません。書き込みは何もせず、読み取りは空文字列を返します。

範囲外のインデックスを渡すこと自体が間違いなら、メッセージを表示するより、例外を投げて呼び出し元に知らせるほうがよい場合もあります。例外を投げる方法は、[例外を投げる](/unity-csharp-learning/csharp/throwing-exceptions/) で学びます。

---

## 4. string のインデックス

インデックスの型は `int` に限りません。`string` にすると、名前（文字列のキー）で読み書きするインデクサを作れます。

```csharp
StatusEffects effects = new StatusEffects();
effects["poison"] = true;

Console.WriteLine(effects["poison"]);
Console.WriteLine(effects["paralyze"]);

class StatusEffects
{
    private bool _poisoned;
    private bool _paralyzed;

    public bool this[string name]
    {
        get
        {
            switch (name)
            {
                case "poison":
                    return _poisoned;
                case "paralyze":
                    return _paralyzed;
                default:
                    return false;
            }
        }
        set
        {
            switch (name)
            {
                case "poison":
                    _poisoned = value;
                    break;
                case "paralyze":
                    _paralyzed = value;
                    break;
            }
        }
    }
}
```

```
True
False
```

`get` の `switch` 文では、`break` の代わりに `return` で値を返して抜けています。

> 💡 **ポイント**: 文字列などのキーで値を保存・検索したいときは、.NET に用意されている [Dictionary\<TKey, TValue\>](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2) クラスが便利です。`Dictionary` も、インデクサで `dictionary["キー"]` のように読み書きします。

---

## 5. 読み取り専用のインデクサ

`set` アクセサーを書かないと、読み取り専用のインデクサになります。プロパティの `{ get; }` と同じ考え方です。

```csharp
ReadOnlyInventory inv = new ReadOnlyInventory(new string[] { "炎の剣", "氷の盾" });
Console.WriteLine(inv[0]);
Console.WriteLine(inv[1]);

class ReadOnlyInventory
{
    private string[] _slots;

    public ReadOnlyInventory(string[] items)
    {
        _slots = items;
    }

    public string this[int index]
    {
        get { return _slots[index]; }
    }
}
```

```
炎の剣
氷の盾
```

```csharp
// ❌ NG: set のないインデクサには、書き込めない
// inv[0] = "木の棒";  // CS0200
```

---

## よくあるミス

### this を書き忘れる

```csharp
// ❌ NG: this がないので、インデクサとして読み取れない
// public string [int index]  // CS1001 など
// {
//     get { return _slots[index]; }
// }
```

インデクサには、名前の代わりに `this` を書きます。`this` がないと、構文として正しく読み取れず、コンパイルエラーになります。

---

## まとめ

- インデクサを定義すると、自分で作ったクラスのインスタンスを `[]` で読み書きできる
- プロパティ名の代わりに `this[インデックスの型 パラメータ名]` と書く
- `get` と `set` の中で、インデックスが範囲内かを調べられる
- インデックスの型は `int` が一般的だが、`string` なども使える
- `set` を書かないと、読み取り専用のインデクサになる

---

## 理解度チェック

1. インデクサの書き方で、プロパティと違う点は何ですか？
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   Counter c = new Counter();
   c[0] = 10;
   c[1] = 20;
   c[2] = c[0] + c[1];
   Console.WriteLine(c[2]);

   class Counter
   {
       private int[] _counts = new int[3];

       public int this[int index]
       {
           get { return _counts[index]; }
           set { _counts[index] = value; }
       }
   }
   ```

3. 次の `ScoreBoard` クラスに、`"alice"`・`"bob"` の名前で `int` のスコアを読み書きできるインデクサを追加してください。登録されていない名前を読んだときは `0` を返します。

   ```csharp
   class ScoreBoard
   {
       private int _alice;
       private int _bob;
   }
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. プロパティ名の代わりに `this` を書き、その後の `[ ]` の中に、インデックスを受け取るパラメータを書く点です。
2. `30` が出力されます。`c[0] + c[1]` は `10 + 20` で `30` になり、`c[2]` に保存されます。

   ```
   30
   ```

3. ```csharp
   ScoreBoard board = new ScoreBoard();
   board["alice"] = 80;
   Console.WriteLine(board["alice"]);
   Console.WriteLine(board["carol"]);

   class ScoreBoard
   {
       private int _alice;
       private int _bob;

       public int this[string name]
       {
           get
           {
               switch (name)
               {
                   case "alice":
                       return _alice;
                   case "bob":
                       return _bob;
                   default:
                       return 0;
               }
           }
           set
           {
               switch (name)
               {
                   case "alice":
                       _alice = value;
                       break;
                   case "bob":
                       _bob = value;
                       break;
               }
           }
       }
   }
   ```

   `80` と `0` が表示されます。

</details>

---

## 次のステップ

これで「C# クラスとオブジェクト」のセクションは終わりです。[ref / out / in パラメータ](/unity-csharp-learning/csharp/ref-out-in/) からは「C# メソッドの応用文法」のセクションに進み、メソッドの引数の渡し方を学びます。
