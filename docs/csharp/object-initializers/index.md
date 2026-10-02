---
layout: page
title: オブジェクト初期化子（補足）
permalink: /csharp/object-initializers/
---

# オブジェクト初期化子（補足）

[クラスとフィールド](/unity-csharp-learning/csharp/classes/) では、インスタンスを作った後に、フィールドへ 1 行ずつ値を代入しました。**オブジェクト初期化子**（object initializer）を使うと、インスタンスを作る式の中で、フィールドにまとめて値を入れられます。このページでは、オブジェクト初期化子の書き方と、コンパイラーがそれをどのようなコードに置き換えるかを学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- オブジェクト初期化子で、インスタンスを作るときにフィールドに値を入れられる
- オブジェクト初期化子が、インスタンスを作った後の代入と同じ意味になることを説明できる
- 型名を省略した `new()` とオブジェクト初期化子を組み合わせられる

## 前提知識

- [クラスとフィールド](/unity-csharp-learning/csharp/classes/) を読んでいること

---

## 1. 代入を並べる書き方の不便なところ

[クラスとフィールド](/unity-csharp-learning/csharp/classes/) では、次のようにインスタンスを作ってから、フィールドに値を代入しました。

```csharp
Player p1 = new Player();
p1.Name = "Alice";
p1.Hp = 100;

Player p2 = new Player();
p2.Name = "Bob";
p2.Hp = 80;
```

フィールドが増えるほど、`p1.` や `p2.` を繰り返す行が増えます。また、インスタンスを作る文と値を入れる文が離れているので、途中に別の処理を挟むと、どこまでがこのインスタンスの準備なのかがわかりにくくなります。

---

## 2. オブジェクト初期化子の書き方

`new クラス名()` の後に `{ }` を書き、その中に `フィールド名 = 値` を `,` で区切って並べます。

**書式：[オブジェクト初期化子](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/object-and-collection-initializers#object-initializers)**
```
new クラス名() { フィールド名 = 値, フィールド名 = 値, ... }
new クラス名 { フィールド名 = 値, フィールド名 = 値, ... }
```

| 要素 | 説明 |
|---|---|
| `{ }` | オブジェクト初期化子。作ったインスタンスのフィールドに入れる値を並べる |
| `フィールド名 = 値` | 値を入れるフィールドと、その値。`p1.` のようなインスタンスの変数は書かない |
| `,` | 項目の区切り。最後の項目の後ろにも書ける |
| `()` | オブジェクト初期化子を書くときは、省略できる |

1. のコードは、オブジェクト初期化子を使うと次のように書けます。

```csharp
Player p1 = new Player { Name = "Alice", Hp = 100 };
Player p2 = new Player { Name = "Bob", Hp = 80 };

Console.WriteLine($"{p1.Name}: HP={p1.Hp}");
Console.WriteLine($"{p2.Name}: HP={p2.Hp}");

class Player
{
    public string Name = "";
    public int Hp;
}
```

```
Alice: HP=100
Bob: HP=80
```

項目が多いときは、1 項目ずつ改行して書くと読みやすくなります。最後の項目の後ろの `,` は、あってもなくてもかまいません。付けておくと、項目を足したり並べ替えたりするときに、`,` の付け忘れが起きません。

```csharp
Player p = new Player
{
    Name = "Alice",
    Hp = 100,
};
```

---

## 3. コンパイラーが置き換えるコード

オブジェクト初期化子は、新しい機能を追加するものではなく、書き方を短くするためのものです。コンパイラーは、オブジェクト初期化子を、インスタンスを作った後に書いた順に代入するコードに置き換えます。

```csharp
Player p = new Player { Name = "Alice", Hp = 100 };
```

このコードは、次のコードと同じ意味になります。

```csharp
Player temp = new Player();
temp.Name = "Alice";
temp.Hp = 100;
Player p = temp;
```

`temp` は、説明のために付けた名前で、実際にはコンパイラーが名前のない変数を用意します。すべての代入が終わってから `p` に入るので、`p` が値を入れる途中のインスタンスを指すことはありません。

インスタンスを作るときには、先にフィールドの初期値が入ります。オブジェクト初期化子の代入は、その後に実行されます。そのため、初期値のあるフィールドをオブジェクト初期化子に書くと、初期値は上書きされます。書かなかったフィールドは、初期値のままです。

```csharp
Player p1 = new Player { Name = "Alice", Hp = 100, Level = 5 };
Player p2 = new Player { Name = "Bob" };

Console.WriteLine($"{p1.Name}: HP={p1.Hp}, Level={p1.Level}");
Console.WriteLine($"{p2.Name}: HP={p2.Hp}, Level={p2.Level}");

class Player
{
    public string Name = "";
    public int Hp;
    public int Level = 1;
}
```

```
Alice: HP=100, Level=5
Bob: HP=0, Level=1
```

`p1` の `Level` は、初期値の `1` が入った後に `5` で上書きされます。`p2` には `Hp` と `Level` を書いていないので、`Hp` は `int` の既定値の `0`、`Level` は初期値の `1` のままです。

```mermaid
flowchart LR
    A["new Player()"] --> B["初期値が入る<br>Name：空文字列、Hp：0、Level：1"]
    B --> C["Name に Alice を代入"]
    C --> D["Hp に 100 を代入"]
    D --> F["Level に 5 を代入"]
    F --> E["p1 に入る"]
```

> 💡 **ポイント**: オブジェクト初期化子に書かなかったフィールドは、エラーにも警告にもならず、初期値のままになります。「名前は必ず入れてほしい」のように、入れ忘れを防ぎたい値があるときは、[コンストラクター](/unity-csharp-learning/csharp/constructors/) で受け取ります。

---

## 4. 型名を省略した new() と組み合わせる

[クラスとフィールド](/unity-csharp-learning/csharp/classes/) で学んだ、型名を省略した `new()` にも、オブジェクト初期化子を書けます。変数の型から、どのクラスのインスタンスを作るかが決まります。

```csharp
Player p = new() { Name = "Alice", Hp = 100 };
Console.WriteLine($"{p.Name}: HP={p.Hp}");

class Player
{
    public string Name = "";
    public int Hp;
}
```

```
Alice: HP=100
```

型名を省略するときは、`()` を省略できません。`new { ... }` と書くと、別の意味になります（「よくあるミス」を参照）。

---

## 5. 配列の要素を作る

オブジェクト初期化子は式なので、配列の要素に入れる値としても書けます。[クラスとフィールド](/unity-csharp-learning/csharp/classes/) の「インスタンスを配列で扱う」の例は、次のように書けます。

```csharp
Player[] players =
{
    new Player { Name = "Alice", Hp = 100 },
    new Player { Name = "Bob", Hp = 90 },
    new Player { Name = "Carol", Hp = 80 },
};

foreach (Player p in players)
{
    Console.WriteLine($"{p.Name}: HP={p.Hp}");
}

class Player
{
    public string Name = "";
    public int Hp;
}
```

```
Alice: HP=100
Bob: HP=90
Carol: HP=80
```

配列の要素ごとに `new` を書くことは同じですが、どの要素にどの値が入るかを、1 か所で見渡せます。

---

## よくあるミス

### 型名を省略するときに () を書かない

```csharp
// ❌ NG: new { ... } は Player ではない別の型になる（CS0029）
// Player p = new { Name = "Alice", Hp = 100 };

// ✅ OK: 型名を省略するときは () を書く
Player p = new() { Name = "Alice", Hp = 100 };
```

`new` の直後に `{ }` を書くと、`Player` のインスタンスではなく、`Name` と `Hp` を持つ名前のない型（**匿名型**）のインスタンスが作られます。匿名型は `Player` ではないので、`Player` の変数に入れようとするとコンパイルエラーになります。匿名型については、[匿名型](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/anonymous-types) を参照してください。

### 同じフィールドを 2 回書く

```csharp
// ❌ NG: 1 つのオブジェクト初期化子に、同じフィールドは 1 回しか書けない（CS1912）
// Player p = new Player { Name = "Alice", Hp = 100, Hp = 80 };
```

代入を並べる書き方では、同じフィールドに 2 回代入してもエラーになりません。オブジェクト初期化子では、同じフィールドを 2 回書くとコンパイルエラーになるので、書き間違いに気付けます。

---

## ワンポイントアドバイス

### コンストラクターやプロパティと組み合わせる

オブジェクト初期化子は、この後に学ぶ機能とも組み合わせられます。

- [コンストラクター](/unity-csharp-learning/csharp/constructors/) に引数を渡したうえで、オブジェクト初期化子を書ける。コンストラクターが実行された後に、オブジェクト初期化子の代入が実行される
- [プロパティ](/unity-csharp-learning/csharp/properties/) にも、フィールドと同じように値を入れられる

それぞれのページで、組み合わせ方を紹介します。

---

## まとめ

- オブジェクト初期化子は、`new クラス名 { フィールド名 = 値, ... }` と書き、インスタンスを作る式の中でフィールドに値を入れる
- コンパイラーは、インスタンスを作った後に、書いた順に代入するコードに置き換える
- フィールドの初期値が先に入り、オブジェクト初期化子の値で上書きされる。書かなかったフィールドは初期値のまま
- 型名を省略した `new()` にも書ける。このとき `()` は省略できない
- 1 つのオブジェクト初期化子に、同じフィールドは 1 回しか書けない

---

## 理解度チェック

1. 次のコードを実行すると何が出力されますか？

   ```csharp
   Enemy e1 = new Enemy { Name = "Slime" };
   Enemy e2 = new Enemy { Name = "Goblin", Hp = 50, Attack = 8 };

   Console.WriteLine($"{e1.Name}: HP={e1.Hp}, Attack={e1.Attack}");
   Console.WriteLine($"{e2.Name}: HP={e2.Hp}, Attack={e2.Attack}");

   class Enemy
   {
       public string Name = "";
       public int Hp = 30;
       public int Attack;
   }
   ```

2. 次のコードを、オブジェクト初期化子を使って 1 つの文に書き換えてください。

   ```csharp
   Item item = new Item();
   item.Name = "回復薬";
   item.Price = 50;
   ```

3. `Player p = new { Name = "Alice" };` がコンパイルエラーになるのはなぜですか？

<details markdown="1">
<summary>解答を見る</summary>

1. 次のように出力されます。`e1` には `Hp` と `Attack` を書いていないので、`Hp` は初期値の `30`、`Attack` は `int` の既定値の `0` のままです。`e2` の `Hp` は、初期値の `30` が `50` で上書きされます。

   ```
   Slime: HP=30, Attack=0
   Goblin: HP=50, Attack=8
   ```

2. ```csharp
   Item item = new Item { Name = "回復薬", Price = 50 };
   ```

   型名を省略して、`Item item = new() { Name = "回復薬", Price = 50 };` と書くこともできます。

3. `new` の直後に `{ }` を書くと、`Player` ではなく匿名型のインスタンスが作られ、`Player` の変数に入れられないからです。型名を省略するときは、`new() { Name = "Alice" }` のように `()` を書きます。

</details>

---

## 次のステップ

[メソッド](/unity-csharp-learning/csharp/methods/) では、クラスに処理を持たせ、名前を付けて呼び出す方法を学びます。
