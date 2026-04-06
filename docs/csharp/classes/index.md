---
layout: page
title: クラスとインスタンス
permalink: /csharp/classes/
---

# クラスとインスタンス

変数が増えるにつれて「プレイヤーの名前・HP・スコア」のように**関連する値がバラバラに散らばる**問題が起きます。**クラス**を使うと、関連するデータと処理をひとつにまとめて扱えます。C# のプログラムは、このクラスを中心に設計されています。

## 学習目標

- クラスを定義してインスタンスを生成できる
- フィールドにデータを格納・参照できる
- 複数のインスタンスがそれぞれ独立した値を持つことを理解できる

## 前提知識

- [配列の基礎](/unity-csharp-learning/csharp/arrays/) を読んでいること

---

## 1. クラスが必要な理由

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

---

## 2. クラスの定義

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

---

## 3. インスタンスの生成

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

---

## 4. フィールド

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

> 💡 **アクセス修飾子について**: `public` はクラスの外からアクセスできることを示します。アクセス修飾子の詳細は[アクセス修飾子](/unity-csharp-learning/csharp/access-modifiers/)で解説します。

---

## よくあるミス

### フィールドとローカル変数を混同する

インスタンスのフィールドを変更したつもりが、同名のローカル変数を作っただけになることがあります。

```csharp
class Player
{
    public string name;
    public int hp;
}

Player p = new Player();
p.name = "Alice";

int hp = 100;              // ❌ NG: ローカル変数 hp を作っているだけ。p.hp は変わらない
Console.WriteLine(p.hp);  // 0（フィールドの初期値のまま）

p.hp = 100;                // ✅ OK: インスタンスのフィールドに代入
Console.WriteLine(p.hp);  // 100
```

```
0
100
```

`p.hp` にアクセスするには必ずインスタンス変数（`p`）を経由します。`int hp = 100;` はそのブロック内だけで有効なローカル変数であり、フィールドとは別物です。

---

## まとめ

- **クラス** — 関連するデータ（フィールド）と処理（メソッド）をひとまとめにした設計図
- **インスタンス** — `new クラス名()` で作られたクラスの実体。それぞれ独立して値を持つ
- **フィールド** — クラスが保持するデータ。インスタンスごとに独立した値を持つ

---

## 理解度チェック

1. 次のコードの出力結果を答えてください。

   ```csharp
   class Enemy
   {
       public string name;
       public int hp;
   }

   Enemy e1 = new Enemy();
   Enemy e2 = new Enemy();
   e1.name = "Slime";
   e1.hp = 30;
   e2.name = "Goblin";
   e2.hp = 50;
   e1.hp = e1.hp - 10;

   Console.WriteLine($"{e1.name}: HP={e1.hp}");
   Console.WriteLine($"{e2.name}: HP={e2.hp}");
   ```

2. `name`（string）と `level`（int）フィールドを持つ `Character` クラスを定義してください。インスタンスを 2 つ作り、それぞれ異なる名前とレベルを設定して出力してください。

3. （応用）`public` フィールドを外から直接変更できることの問題点を説明し、その対策として何を使うべきかを答えてください。（ヒント：後のページで学びます）

<details markdown="1">
<summary>解答を見る</summary>

1. ```
   Slime: HP=20
   Goblin: HP=50
   ```
   `e1` と `e2` は独立したインスタンスなので `e1.hp` の変化は `e2` に影響しない。

2. ```csharp
   class Character
   {
       public string name;
       public int level;
   }

   Character c1 = new Character();
   c1.name = "Alice";
   c1.level = 5;

   Character c2 = new Character();
   c2.name = "Bob";
   c2.level = 3;

   Console.WriteLine($"{c1.name}: Lv={c1.level}");
   Console.WriteLine($"{c2.name}: Lv={c2.level}");
   ```

3. 外部から意図しない値（負の HP など）を自由に書き込めてしまう。対策として `private` フィールドとアクセス修飾子を使う。

</details>

---

## 次のステップ

[メソッド](/unity-csharp-learning/csharp/methods/) では、処理に名前をつけてクラスに定義する仕組みを学びます。

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

2. `name`（string）と `level`（int）を持つ `Hero` クラスを定義し、インスタンスを作って値をセット・出力するコードを書いてください。

3. （応用）`Player` クラスのインスタンスを 3 つ作り、それぞれ異なる `name` と `hp` を設定して `for` 文で全員の情報を出力するにはどう書きますか？

<details markdown="1">
<summary>解答を見る</summary>

1. `a.price=200` と `b.price=200`。クラスは**参照型**のため `b = a` でコピーされるのは参照です。`a` と `b` は同じインスタンスを指しているため、どちらから変更しても両方に影響します。

2. ```csharp
   class Hero
   {
       public string name;
       public int level;
   }

   Hero h = new Hero();
   h.name = "Arthur";
   h.level = 5;
   Console.WriteLine($"{h.name}: Lv={h.level}");
   ```

3. ```csharp
   Player[] players = new Player[3];

   players[0] = new Player();
   players[0].name = "Alice";
   players[0].hp = 100;

   players[1] = new Player();
   players[1].name = "Bob";
   players[1].hp = 80;

   players[2] = new Player();
   players[2].name = "Carol";
   players[2].hp = 90;

   for (int i = 0; i < players.Length; i++)
   {
       Console.WriteLine($"{players[i].name}: HP={players[i].hp}");
   }
   ```

</details>

---

## 次のステップ

[メソッド](/unity-csharp-learning/csharp/methods/) では、クラスに処理をまとめて再利用する方法を学びます。
