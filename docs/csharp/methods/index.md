---
layout: page
title: メソッド
permalink: /csharp/methods/
---

# メソッド

「HP をダメージ分だけ減らす」処理を複数箇所で書くとします。

```csharp
p1.hp = p1.hp - 10;
p2.hp = p2.hp - 10;
```

同じ処理を繰り返し書くのは非効率で、修正漏れの原因にもなります。**メソッド**は処理に名前をつけてクラスに定義する仕組みです。一度書けばどこからでも呼び出して再利用できます。

## 学習目標

- `void` メソッドを定義して呼び出せる
- パラメータを使って処理に値を渡せる
- 戻り値を使って処理の結果を受け取れる
- シグネチャの概念を理解できる
- オーバーロードで同名メソッドを使い分けられる

## 前提知識

- [クラスとインスタンス](/unity-csharp-learning/csharp/classes/) を読んでいること

---

## 1. 戻り値なし・パラメータなし

最もシンプルなメソッドは「何かを実行するだけ」のものです。戻り値がない場合は `void` を使います。

**書式：メソッドの定義（基本）**
```
アクセス修飾子 戻り値の型 メソッド名()
{
    処理
}
```

| 要素 | 説明 |
|---|---|
| `アクセス修飾子` | `public` はクラスの外から呼び出せることを示す |
| `void` | 戻り値がないことを示す |
| `メソッド名` | 慣例として PascalCase（先頭大文字）で命名する |

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
p.hp = 100;
p.Greet();
```

```
こんにちは、Aliceです！
```

メソッド内からは同じインスタンスのフィールドに `.` なしで直接アクセスできます。

---

## 2. パラメータ

処理に外から値を渡したいときは**パラメータ**（引数）を定義します。

**書式：パラメータありメソッドの定義**
```
アクセス修飾子 void メソッド名(型 パラメータ名)
{
    処理
}
```

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

複数のパラメータはカンマで区切ります。

```csharp
public void Move(int x, int y)
{
    Console.WriteLine($"{name} が ({x}, {y}) に移動した");
}
```

---

## 3. 戻り値

処理の結果を呼び出し元に返したいときは、`void` の代わりに**返す型**を書き、`return` で値を返します。

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

`return` に達するとメソッドはそこで終了します。残りのコードは実行されません。`void` メソッドでは `return;`（値なし）で途中終了できます。

---

## 4. シグネチャとオーバーロード

**シグネチャ**とは、メソッドを識別するための「メソッド名＋パラメータの型の並び」のことです。

```
TakeDamage(int)         → シグネチャ: TakeDamage(int)
TakeDamage(int, bool)   → シグネチャ: TakeDamage(int, bool)
```

C# では**同じクラス内にシグネチャが異なるメソッドを複数定義できます**。これを**オーバーロード**と呼びます。たとえば「通常のダメージ計算」と「クリティカルを考慮したダメージ計算」を同じ名前で書き分けられます。

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

呼び出し時に渡した引数の型と数によって、どのメソッドが実行されるかが自動的に決まります。

> ⚠️ **注意**: 戻り値の型だけが異なるメソッドはオーバーロードになりません（コンパイルエラー）。

---

## まとめ

- **`void` メソッド** — 戻り値なし。処理を実行するだけのメソッド
- **パラメータ** — メソッドに値を渡すための仕組み。複数定義可
- **戻り値** — `return` でメソッドの実行結果を呼び出し元に返す
- **シグネチャ** — メソッド名＋パラメータの型の並び。メソッドの識別に使われる
- **オーバーロード** — シグネチャが異なる同名メソッドを同じクラスに複数定義すること

---

## 理解度チェック

1. 次のコードの出力結果を答えてください。

   ```csharp
   class Calc
   {
       public int Add(int a, int b)
       {
           return a + b;
       }

       public int Add(int a, int b, int c)
       {
           return a + b + c;
       }
   }

   Calc calc = new Calc();
   Console.WriteLine($"result={calc.Add(3, 4)}");
   Console.WriteLine($"result={calc.Add(1, 2, 3)}");
   ```

2. 次のクラスに `Heal(int amount)` メソッドを追加してください。HP が `maxHp` を超えないよう制限すること。

   ```csharp
   class Player
   {
       public string name;
       public int hp;
       public int maxHp;
   }
   ```

3. （応用）`int` 型と `double` 型のどちらを渡しても面積を返す `Area` メソッドをオーバーロードで定義してください（縦×横の長方形）。

<details markdown="1">
<summary>解答を見る</summary>

1. ```
   result=7
   result=6
   ```
   引数の数が異なるためオーバーロードが選ばれる。

2. ```csharp
   public void Heal(int amount)
   {
       hp = hp + amount;
       if (hp > maxHp) { hp = maxHp; }
   }
   ```

3. ```csharp
   class Shape
   {
       public int Area(int width, int height)
       {
           return width * height;
       }

       public double Area(double width, double height)
       {
           return width * height;
       }
   }
   ```

</details>

---

## 次のステップ

[コンストラクタ](/unity-csharp-learning/csharp/constructors/) では、インスタンス生成時に自動的に呼び出されてフィールドを初期化する仕組みを学びます。
