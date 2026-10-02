---
layout: page
title: init と required（補足）
permalink: /csharp/init-required/
---

# init と required（補足）

[オブジェクト初期化子（補足）](/unity-csharp-learning/csharp/object-initializers/) を使うと、プロパティの名前を書きながら値を入れられるので、何にどの値を入れたのかが読みやすくなります。ただし、そのままでは、作った後にも値を変えられ、値を入れ忘れても気付けません。このページでは、作るときにだけ値を入れられる **init アクセサー** と、値を入れることを必須にする **required 修飾子** を学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `init` アクセサーで、インスタンスを作るときにだけ値を入れられるプロパティを定義できる
- `required` 修飾子で、オブジェクト初期化子で値を入れることを必須にできる
- コンストラクターで受け取る方法と、`required` で必須にする方法の違いを説明できる

## 前提知識

- [プロパティ](/unity-csharp-learning/csharp/properties/) を読んでいること
- [オブジェクト初期化子（補足）](/unity-csharp-learning/csharp/object-initializers/) を読んでいること

---

## 1. オブジェクト初期化子の不便なところ

オブジェクト初期化子では、フィールドと同じように、`set` を持つプロパティにも値を入れられます。

```csharp
Item potion = new Item { Name = "回復薬", Price = 50 };
Item sword = new Item { Price = 1200 };

Console.WriteLine($"[{potion.Name}] {potion.Price} G");
Console.WriteLine($"[{sword.Name}] {sword.Price} G");

class Item
{
    public string Name { get; set; } = "";
    public int Price { get; set; }
}
```

```
[回復薬] 50 G
[] 1200 G
```

この書き方には、次の 2 つの問題があります。

- `sword` は `Name` を入れ忘れていますが、コンパイラーは気付きません。名前が空のまま使われています
- `set` を持つプロパティなので、作った後に `potion.Price = 0;` のように、どこからでも値を変えられます

[コンストラクター](/unity-csharp-learning/csharp/constructors/) で受け取るようにすれば、入れ忘れを防げます。プロパティを `{ get; }` にすれば、作った後に値を変えられなくなります（[プロパティ](/unity-csharp-learning/csharp/properties/)）。ただし、`new Item("回復薬", 50)` のように引数を並べる書き方では、どの値が何を表すのかが、呼び出す側のコードからは読み取りにくくなります。プロパティが増えるほど、この問題は大きくなります。

オブジェクト初期化子の読みやすさを保ったまま、2 つの問題を解決するのが、`init` と `required` です。

---

## 2. init アクセサー

C# 9 以降では、`set` の代わりに [init](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/init) と書くと、インスタンスを作るときにだけ値を入れられるプロパティになります。

**書式：[init アクセサー](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/init)**
```
アクセス修飾子 型 プロパティ名 { get; init; }
```

`init` のプロパティに値を入れられるのは、次の場所だけです。

- オブジェクト初期化子
- そのクラスのコンストラクター
- プロパティの初期値（`= 初期値`）

```csharp
Item potion = new Item { Name = "回復薬", Price = 50 };
Console.WriteLine($"{potion.Name}: {potion.Price} G");

class Item
{
    public string Name { get; init; } = "";
    public int Price { get; init; }
}
```

```
回復薬: 50 G
```

作った後に値を入れようとすると、クラスの外でも、クラスの中のメソッドでも、コンパイルエラーになります。

```csharp
// ❌ NG: init のプロパティには、作った後に値を入れられない
// Item potion = new Item { Name = "回復薬", Price = 50 };
// potion.Price = 0;  // CS8852
```

[プロパティ](/unity-csharp-learning/csharp/properties/) で学んだ読み取り専用のプロパティと比べると、次のようになります。

| 書き方 | オブジェクト初期化子 | コンストラクター | 作った後（クラスの外） | 作った後（クラスの中のメソッド） |
|---|---|---|---|---|
| `{ get; set; }` | 入れられる | 入れられる | 入れられる | 入れられる |
| `{ get; private set; }` | 入れられない | 入れられる | 入れられない | 入れられる |
| `{ get; init; }` | 入れられる | 入れられる | 入れられない | 入れられない |
| `{ get; }` | 入れられない | 入れられる | 入れられない | 入れられない |

`{ get; }` のプロパティには、オブジェクト初期化子でも値を入れられません。

```csharp
// ❌ NG: { get; } のプロパティには、オブジェクト初期化子でも値を入れられない
// Enemy e = new Enemy { Name = "Slime" };  // CS0200
//
// class Enemy
// {
//     public string Name { get; } = "";
// }
```

`init` で、作った後に値を変えられる問題は解決しました。ただ、`Name` の入れ忘れには、まだ気付けません。

---

## 3. required 修飾子

C# 11 以降では、プロパティに [required](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/required) 修飾子を付けると、オブジェクト初期化子で値を入れることが必須になります。値を入れずにインスタンスを作ると、コンパイルエラーになります。

**書式：[required 修飾子](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/required)**
```
アクセス修飾子 required 型 プロパティ名 { get; init; }
```

`Name` と `Price` を必須にし、説明の `Description` は入れなくてもよいプロパティにします。

```csharp
Item potion = new Item { Name = "回復薬", Price = 50 };
Item sword = new Item { Name = "剣", Price = 1200, Description = "よく切れる" };

Console.WriteLine($"{potion.Name}: {potion.Price} G [{potion.Description}]");
Console.WriteLine($"{sword.Name}: {sword.Price} G [{sword.Description}]");

class Item
{
    public required string Name { get; init; }
    public required int Price { get; init; }
    public string Description { get; init; } = "";
}
```

```
回復薬: 50 G []
剣: 1200 G [よく切れる]
```

`Name` を入れ忘れると、コンパイルエラーになります。

```csharp
// ❌ NG: required の Name に値を入れていない
// Item sword = new Item { Price = 1200 };  // CS9035
```

`required` と `init` を組み合わせると、次のようなプロパティになります。

- 作るときに、必ず値を入れなければならない（`required`）
- 作った後は、値を変えられない（`init`）

`required` は `set` を持つプロパティにも付けられます。そのときは、作るときに値を入れることが必須で、作った後も値を変えられるプロパティになります。

`required` を付けるプロパティには、次の制約があります。

- `set` か `init` が必要（`{ get; }` のプロパティには付けられない。CS9034）
- クラスと同じかそれより広く公開されている必要がある（`private` のプロパティには付けられない。CS9032）

オブジェクト初期化子で値を入れるのは、クラスの外のコードです。値を入れられないプロパティを必須にしても、誰も値を入れられないからです。

---

## 4. コンストラクターと required の使い分け

インスタンスを作るときに値を必ず受け取る方法は、コンストラクターと `required` の 2 つになりました。

| | コンストラクターで受け取る | `required` で必須にする |
|---|---|---|
| 作る側の書き方 | `new Item("回復薬", 50)` | `new Item { Name = "回復薬", Price = 50 }` |
| どの値が何を表すか | 引数の順序で決まる。呼び出す側のコードからは読み取りにくい | プロパティの名前が書かれているので、読み取りやすい |
| 値を調べる処理 | コンストラクターの本体に書ける | `init` アクセサーの本体に書ける（1 つずつ） |
| 複数の値を組み合わせて調べる | できる | できない（どの順序で値が入るかは、作る側が決める） |

値の組み合わせに条件があるとき（最小値が最大値以下でなければならない、など）は、コンストラクターで受け取ります。値が多く、それぞれが独立しているときは、`required` が向いています。

---

## よくあるミス

### コンストラクターで値を入れても、required は満たされない

`required` のプロパティに、コンストラクターの中で値を入れても、作る側でオブジェクト初期化子を書かなければコンパイルエラーになります。コンパイラーは、コンストラクターの中で何に値を入れているかを調べないからです。

```csharp
// ❌ NG: コンストラクターで Name と Price に値を入れていても、CS9035 になる
// Item potion = new Item("回復薬", 50);  // CS9035
//
// class Item
// {
//     public required string Name { get; init; }
//     public required int Price { get; init; }
//
//     public Item(string name, int price)
//     {
//         Name = name;
//         Price = price;
//     }
// }
```

`required` のプロパティに値を入れるコンストラクターを用意したいときは、そのコンストラクターに [SetsRequiredMembers 属性](https://learn.microsoft.com/dotnet/api/system.diagnostics.codeanalysis.setsrequiredmembersattribute) を付けます。`[ ]` で囲んで宣言に付けるこの書き方は **属性** といい、[属性の基本](/unity-csharp-learning/csharp/attributes/) で学びます。`[SetsRequiredMembers]` は、「このコンストラクターは `required` のプロパティすべてに値を入れる」とコンパイラーに伝えます。使うには、`using System.Diagnostics.CodeAnalysis;` が必要です。

```csharp
using System.Diagnostics.CodeAnalysis;

Item potion = new Item("回復薬", 50);
Item sword = new Item { Name = "剣", Price = 1200 };

Console.WriteLine($"{potion.Name}: {potion.Price} G");
Console.WriteLine($"{sword.Name}: {sword.Price} G");

class Item
{
    public required string Name { get; init; }
    public required int Price { get; init; }

    public Item()
    {
    }

    [SetsRequiredMembers]
    public Item(string name, int price)
    {
        Name = name;
        Price = price;
    }
}
```

```
回復薬: 50 G
剣: 1200 G
```

コンパイラーは、`[SetsRequiredMembers]` を付けたコンストラクターが、本当にすべての値を入れているかは調べません。値を入れ忘れないように注意します。

---

## ワンポイントアドバイス

### required と null 許容参照型の警告

[null 許容参照型](/unity-csharp-learning/csharp/nullable-reference-types/) で学ぶように、`string` のプロパティに初期値を書かず、コンストラクターでも値を入れないと、警告 CS8618 が出ます。`null` のまま使われるおそれがあるからです。このページの最初の例で `Name` に `= ""` を書いていたのは、この警告を出さないためです。

`required` を付けたプロパティは、作る側で必ず値が入るので、この警告は出ません。`= ""` のような仮の初期値を書かずに済みます。

---

## まとめ

- `init` アクセサーのプロパティには、オブジェクト初期化子・コンストラクター・初期値でだけ値を入れられ、作った後は変えられない
- `required` 修飾子を付けたプロパティは、オブジェクト初期化子で値を入れないとコンパイルエラーになる（CS9035）
- `required` と `init` を組み合わせると、作るときに必須で、作った後は変えられないプロパティになる
- コンストラクターで値を入れても `required` は満たされない。そのコンストラクターには `[SetsRequiredMembers]` を付ける
- 値の組み合わせを調べたいときはコンストラクター、独立した値が多いときは `required` を使う

---

## 理解度チェック

1. `{ get; init; }` と `{ get; }` のプロパティの違いを説明してください。
2. 次のコードのうち、コンパイルエラーになる行はどれですか？

   ```csharp
   Book a = new Book { Title = "C# 入門", Pages = 300 };
   Book b = new Book { Title = "LINQ 入門" };
   Book c = new Book { Pages = 200 };
   a.Pages = 320;

   class Book
   {
       public required string Title { get; init; }
       public int Pages { get; init; }
   }
   ```

3. 次のコードを実行すると何が出力されますか？

   ```csharp
   Book a = new Book { Title = "C# 入門" };
   Book b = new Book { Title = "LINQ 入門", Pages = 150 };
   Console.WriteLine($"{a.Title}: {a.Pages}");
   Console.WriteLine($"{b.Title}: {b.Pages}");

   class Book
   {
       public required string Title { get; init; }
       public int Pages { get; init; } = 100;
   }
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. `{ get; init; }` には、コンストラクターと初期値のほか、オブジェクト初期化子でも値を入れられます。`{ get; }` には、コンストラクターと初期値でしか値を入れられず、オブジェクト初期化子で入れるとコンパイルエラー（CS0200）になります。どちらも、作った後は値を変えられません。
2. `Book c = new Book { Pages = 200 };` は、`required` の `Title` に値を入れていないので CS9035 になります。`a.Pages = 320;` は、`init` のプロパティに作った後で値を入れているので CS8852 になります。`b` は、`Pages` が `required` ではないので、入れなくてもエラーになりません。
3. 次のように出力されます。`a` は `Pages` に値を入れていないので、初期値の `100` のままです。

   ```
   C# 入門: 100
   LINQ 入門: 150
   ```

</details>

---

## 次のステップ

[インデクサ](/unity-csharp-learning/csharp/indexers/) では、自分で作ったクラスに、配列のように `[]` で要素を読み書きする機能を持たせる方法を学びます。
