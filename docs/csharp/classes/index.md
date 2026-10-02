---
layout: page
title: クラスとフィールド
permalink: /csharp/classes/
---

# クラスとフィールド

プログラムが大きくなると、「プレイヤーの名前・HP・スコア」のように、関連する値がばらばらの変数に散らばってしまいます。**クラス**（class）を使うと、関連するデータと処理を 1 つにまとめて扱えます。このページでは、クラスを定義し、データを入れる **フィールド** を持たせる方法を学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- クラスを定義し、`new` でインスタンスを作れる
- フィールドに値を入れたり、読み取ったりできる
- インスタンスごとに、フィールドが別々の値を持つことを説明できる

## 前提知識

- [配列の基礎](/unity-csharp-learning/csharp/arrays/) を読んでいること

---

## 1. クラスが必要な理由

ゲームのプレイヤーのデータを、変数で管理するとします。

```csharp
string player1Name = "Alice";
int player1Hp = 100;

string player2Name = "Bob";
int player2Hp = 80;

Console.WriteLine($"{player1Name}: HP={player1Hp}");
Console.WriteLine($"{player2Name}: HP={player2Hp}");
```

```
Alice: HP=100
Bob: HP=80
```

プレイヤーが増えるたびに変数が増え、どの変数がどのプレイヤーのものかもわかりにくくなります。クラスを使えば、「プレイヤーは名前と HP を持つ」ということを 1 か所で定義し、そこから何人分でもプレイヤーを作れます。

---

## 2. クラスを定義する

**書式：[クラスの定義](https://learn.microsoft.com/dotnet/csharp/fundamentals/types/classes)**
```
class クラス名
{
    // フィールド、メソッドなどのメンバー
}
```

| 要素 | 説明 |
|---|---|
| `class` | クラスを定義するキーワード |
| `クラス名` | クラスの名前。単語の先頭を大文字にする PascalCase で名付けるのが慣例 |
| `{ }` | クラスの本体。フィールドやメソッドなどの **メンバー** を書く |

中身が空のクラスを定義してみます。

```csharp
class Player
{
}
```

これだけで、`Player` という新しい型ができます。`int` や `string` と同じように、`Player` 型の変数を宣言できるようになります。

---

## 3. インスタンスを作る

クラスは、データの形を決めた **設計図** です。実際にデータを入れて使うには、`new` でクラスの **インスタンス**（実体）を作ります。

**書式：[インスタンスの作成](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/new-operator)**
```
クラス名 変数名 = new クラス名();
```

```csharp
Player p = new Player();
Console.WriteLine(p.GetType());

class Player
{
}
```

```
Player
```

`new Player()` で、`Player` のインスタンスが 1 つ作られ、変数 `p` に入ります。`GetType()` で、`p` が `Player` 型であることを確かめられます。

> 💡 **ポイント**: 1 つのファイルにトップレベルの文（`Player p = new Player();` など）とクラスの定義を書くときは、クラスの定義を **後ろ** に書きます。クラスの定義を先に書くと、コンパイルエラー（CS8803）になります。このサイトのコード例も、すべてこの順序で書いています。

> 💡 **ポイント**: C# 9 以降では、`Player p = new();` のように、`new` の後の型名を省略できます。

---

## 4. フィールド

クラスにデータを持たせるには、**フィールド** を定義します。フィールドは、クラスの中に宣言する変数です。

**書式：[フィールドの定義](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/fields)**
```
アクセス修飾子 型 フィールド名;
```

| 要素 | 説明 |
|---|---|
| `アクセス修飾子` | フィールドをどこから使えるか。ここでは、クラスの外からも使えることを表す `public` を書く |
| `型` | フィールドに入れる値の型 |
| `フィールド名` | フィールドの名前。`public` のフィールドは PascalCase で名付けるのが慣例 |

アクセス修飾子は、[アクセス修飾子](/unity-csharp-learning/csharp/access-modifiers/) で詳しく学びます。

インスタンスのフィールドは、`変数名.フィールド名` で使います。

```csharp
Player p1 = new Player();
p1.Name = "Alice";
p1.Hp = 100;

Player p2 = new Player();
p2.Name = "Bob";
p2.Hp = 80;

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

### インスタンスごとに別々の値を持つ

`p1` と `p2` は、別々のインスタンスです。フィールドの値もインスタンスごとに別々にあるので、一方を書き換えても、もう一方は変わりません。

```csharp
Player p1 = new Player();
p1.Hp = 100;
Player p2 = new Player();
p2.Hp = 100;

p1.Hp = 50;
Console.WriteLine($"p1.Hp={p1.Hp}");
Console.WriteLine($"p2.Hp={p2.Hp}");

class Player
{
    public string Name = "";
    public int Hp;
}
```

```
p1.Hp=50
p2.Hp=100
```

### フィールドの初期値

フィールドに値を入れないままインスタンスを作ると、フィールドには型の既定値（`int` なら `0`）が入ります。`= 値` と書くと、インスタンスを作ったときの初期値を決められます。

```csharp
Player p = new Player();
Console.WriteLine($"Name=[{p.Name}], Hp={p.Hp}, Level={p.Level}");

p.Hp = 100;
Console.WriteLine($"Name=[{p.Name}], Hp={p.Hp}, Level={p.Level}");

class Player
{
    public string Name = "";
    public int Hp;
    public int Level = 1;
}
```

```
Name=[], Hp=0, Level=1
Name=[], Hp=100, Level=1
```

`Name` の初期値 `""` は、0 文字の文字列（空文字列）です。`string` のフィールドに初期値を書かないと、フィールドには「どの文字列も指していない」ことを表す `null` が入り、コンパイラーが警告（CS8618）を出します。このサイトのコード例では、`string` のフィールドに `""` などの初期値を書きます。この警告については、[null 許容参照型](/unity-csharp-learning/csharp/nullable-reference-types/) で学びます。

---

## 5. インスタンスを配列で扱う

クラスも型なので、クラスの配列を作れます。`new Player[3]` で作った配列の要素は、どのインスタンスも指していない `null` なので、要素ごとに `new` でインスタンスを作って入れます。

```csharp
Player[] players = new Player[3];
string[] names = { "Alice", "Bob", "Carol" };

for (int i = 0; i < players.Length; i++)
{
    players[i] = new Player();
    players[i].Name = names[i];
    players[i].Hp = 100 - i * 10;
}

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

---

## よくあるミス

### フィールドのつもりでローカル変数を作る

```csharp
Player p = new Player();

int Hp = 100;
Console.WriteLine(p.Hp);

p.Hp = 100;
Console.WriteLine(p.Hp);

class Player
{
    public int Hp;
}
```

```
0
100
```

`int Hp = 100;` は、`Hp` という名前の新しいローカル変数を作っているだけで、`p` のフィールドは変わりません。このコードをビルドすると、コンパイラーが「変数 `Hp` は代入されているが、その値は使われていない」という警告（CS0219）を出すので、それが間違いに気付く手がかりになります。インスタンスのフィールドを使うときは、必ず `p.Hp` のように、インスタンスの変数を通して書きます。

### インスタンスを変数に代入してコピーしたつもりになる

```csharp
Player a = new Player();
a.Hp = 100;

Player b = a;
b.Hp = 50;

Console.WriteLine(a.Hp);

class Player
{
    public int Hp;
}
```

```
50
```

[Array クラスと配列の性質（補足）](/unity-csharp-learning/csharp/array-class/) で学んだ配列と同じく、クラスの変数に入っているのはインスタンスへの参照です。`b = a` では新しいインスタンスは作られず、`a` と `b` は同じインスタンスを指します。詳しくは、[値型と参照型](/unity-csharp-learning/csharp/value-reference-types/) で学びます。

---

## まとめ

- クラスは、関連するデータと処理をまとめた設計図。`class クラス名 { }` で定義する
- `new クラス名()` で、クラスのインスタンスを作る
- フィールドは、クラスの中に宣言する変数。`インスタンス.フィールド名` で使う
- フィールドの値は、インスタンスごとに別々にある
- フィールドに初期値を書かないと、型の既定値が入る。`= 値` で初期値を決められる
- 1 つのファイルでは、クラスの定義をトップレベルの文より後ろに書く

---

## 理解度チェック

1. 次のコードを実行すると何が出力されますか？

   ```csharp
   Enemy e1 = new Enemy();
   Enemy e2 = new Enemy();
   e1.Name = "Slime";
   e1.Hp = 30;
   e2.Name = "Goblin";
   e2.Hp = 50;
   e1.Hp = e1.Hp - 10;

   Console.WriteLine($"{e1.Name}: HP={e1.Hp}");
   Console.WriteLine($"{e2.Name}: HP={e2.Hp}");

   class Enemy
   {
       public string Name = "";
       public int Hp;
   }
   ```

2. `Name`（`string`）と `Level`（`int`）のフィールドを持つ `Character` クラスを定義してください。インスタンスを 2 つ作り、それぞれ違う名前とレベルを入れて表示してください。
3. `public` のフィールドは、クラスの外から自由に書き換えられます。これにはどのような問題がありますか？

<details markdown="1">
<summary>解答を見る</summary>

1. 次のように出力されます。`e1` と `e2` は別々のインスタンスなので、`e1.Hp` を変えても `e2` には影響しません。

   ```
   Slime: HP=20
   Goblin: HP=50
   ```

2. ```csharp
   Character c1 = new Character();
   c1.Name = "Alice";
   c1.Level = 5;

   Character c2 = new Character();
   c2.Name = "Bob";
   c2.Level = 3;

   Console.WriteLine($"{c1.Name}: Lv={c1.Level}");
   Console.WriteLine($"{c2.Name}: Lv={c2.Level}");

   class Character
   {
       public string Name = "";
       public int Level;
   }
   ```

3. HP に負の値を入れるなど、クラスの外から、ありえない値を入れられてしまいます。これを防ぐ方法は、[アクセス修飾子](/unity-csharp-learning/csharp/access-modifiers/) と [プロパティ](/unity-csharp-learning/csharp/properties/) で学びます。

</details>

---

## 次のステップ

[オブジェクト初期化子（補足）](/unity-csharp-learning/csharp/object-initializers/) では、インスタンスを作る式の中で、フィールドにまとめて値を入れる書き方を学びます。
