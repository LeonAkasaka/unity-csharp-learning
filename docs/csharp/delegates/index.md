---
layout: page
title: デリゲートの基本
permalink: /csharp/delegates/
---

# デリゲートの基本

デリゲートは、メソッドそのものではなく**「あとで呼び出すメソッドへの参照」**を変数に入れて扱うための型です。実行中の条件に応じて呼び出すメソッドを切り替えたいときに役立ちます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `delegate` キーワードで型を宣言できる
- デリゲート変数にメソッドを代入して呼び出せる
- `?.Invoke()` で null 安全に呼び出せる
- 実行時にメソッドを切り替えられることを理解できる

## 前提知識

- [メソッド](/unity-csharp-learning/csharp/methods/) を読んでいること

---

## 1. デリゲートとは

デリゲートは、**メソッドへの参照を格納できる型**です。変数に整数や文字列を入れるのと同じように、デリゲート変数には「このメソッドを呼ぶ」という情報を入れられます。

このしくみが必要になるのは、**どの処理を使うかを実行時に決めたい**場面です。たとえばゲームでは、難易度が `Easy` なら慎重に動くメソッド、`Hard` なら積極的に攻撃するメソッドを選びたくなることがあります。

つまりデリゲートは、**メソッドを値のように扱うための仕組み**です。そのため「メソッドの型」と考えると理解しやすくなります。

---

## 2. delegate 型の宣言

`delegate` キーワードを使うと、「どんな引数を受け取り、どんな戻り値を返すメソッドを入れられるか」を表す型を宣言できます。

**書式：delegate 型の宣言**
```
delegate 戻り値型 型名(パラメータリスト);
```

| 要素 | 説明 |
|---|---|
| `戻り値型` | そのデリゲートが参照できるメソッドの戻り値の型 |
| `型名` | 作成するデリゲート型の名前 |
| `パラメータリスト` | そのデリゲートが参照できるメソッドの引数一覧 |

たとえば `string` を受け取って何も返さないメソッドを扱いたいなら、次のように書けます。

```csharp
public delegate void MessageHandler(string message);
```

この宣言は、「`string` を 1 つ受け取り、戻り値は `void` のメソッドだけを代入できる型を `MessageHandler` という名前で作る」という意味です。

---

## 3. デリゲートのインスタンス化と呼び出し

デリゲート変数には、**シグネチャ**（引数の型・数・戻り値の型の組み合わせ）が一致するメソッドを代入できます。書き方は主に 2 つあります。

- `new DelegateName(method)` の形で明示的に作る
- `DelegateName d = method;` の形で代入する（これを**メソッドグループ変換**と呼び、コンパイラが自動的に `new` ありの形に変換します）

```csharp
using System;

public delegate void MessageHandler(string message);

public class Program
{
    public static void Main()
    {
        MessageHandler handlerByConstructor = new MessageHandler(ShowMessage);
        MessageHandler handlerByMethodGroup = ShowMessage;

        handlerByConstructor("new で作成した呼び出し");
        handlerByMethodGroup("メソッドグループ変換で作成した呼び出し");
    }

    private static void ShowMessage(string message)
    {
        Console.WriteLine(message);
    }
}
```

```
new で作成した呼び出し
メソッドグループ変換で作成した呼び出し
```

どちらの書き方でも、最終的には `ShowMessage` を呼び出すデリゲートが作られます。初心者のうちは `new` 付きの形で意味を理解し、そのあとメソッドグループ変換の省略形に慣れるとわかりやすいです。

---

## 4. null 安全な呼び出し

デリゲート変数は、まだ何も代入していない状態だと `null` になることがあります。この状態でそのまま呼び出すと、実行時エラーになります。

**書式：null 安全なデリゲート呼び出し**
```
デリゲート変数?.Invoke(引数);
```

| 要素 | 説明 |
|---|---|
| `デリゲート変数` | 呼び出したいデリゲート |
| `?.` | 左側が `null` なら呼び出しを行わず、そのまま処理を終える |
| `Invoke(引数)` | デリゲートが参照しているメソッドを実行する |

```csharp
using System;

public delegate void MessageHandler(string message);

public class Program
{
    public static void Main()
    {
        MessageHandler? handler = null;

        handler?.Invoke("まだ代入されていないので何も起きません");

        handler = ShowMessage;
        handler?.Invoke("こちらは安全に呼び出されます");
    }

    private static void ShowMessage(string message)
    {
        Console.WriteLine(message);
    }
}
```

```
こちらは安全に呼び出されます
```

`handler("text")` や `handler.Invoke("text")` は、`handler` が `null` だと失敗します。代入されているか不安なときは、`?.Invoke()` を使うと安全です。

---

## 5. 実行時にメソッドを切り替える

デリゲートの便利さがよくわかるのは、**条件によって代入するメソッドを切り替える**場面です。次の例では、ゲームの難易度によって敵の行動メソッドを変えています。

```csharp
using System;

public delegate void EnemyAction(string enemyName);

public class Program
{
    public static void Main()
    {
        bool isHardMode = true;
        EnemyAction action = isHardMode ? AttackAggressively : MoveCarefully;

        action("Slime");

        isHardMode = false;
        action = isHardMode ? AttackAggressively : MoveCarefully;

        action("Slime");
    }

    private static void AttackAggressively(string enemyName)
    {
        Console.WriteLine($"{enemyName} がプレイヤーに突進して攻撃した");
    }

    private static void MoveCarefully(string enemyName)
    {
        Console.WriteLine($"{enemyName} が距離を取りながら様子を見ている");
    }
}
```

```
Slime がプレイヤーに突進して攻撃した
Slime が距離を取りながら様子を見ている
```

同じ `action("Slime")` という呼び出しでも、事前にどのメソッドを代入したかで動作が変わります。これが、デリゲートが「実行時にメソッドを選べる柔軟性」を持つ理由です。

---

## よくあるミス

デリゲートには、**シグネチャが一致するメソッドだけ**を代入できます。引数の型や数、戻り値が違うメソッドは代入できません。

```csharp
using System;

public delegate void DamageHandler(int damage);

public class Program
{
    public static void Main()
    {
        DamageHandler handler = ShowDamage;
        handler(42);
    }

    // ❌ NG: 引数の型が違うので代入できない
    // private static void ShowDamage(string damage)
    // {
    //     Console.WriteLine(damage);
    // }

    // ✅ OK: デリゲート型と同じシグネチャになっている
    private static void ShowDamage(int damage)
    {
        Console.WriteLine(damage);
    }
}
```

```
42
```

`ShowDamage(int damage)` は `DamageHandler` と同じシグネチャのため代入できます。コンパイルエラーが出たら、**引数の型・引数の数・戻り値**がデリゲート型と一致しているかを確認しましょう。

---

## まとめ

- デリゲートは、メソッドへの参照を変数に入れて扱うための型である
- `delegate 戻り値型 型名(パラメータリスト);` の形で宣言できる
- デリゲート変数には `new DelegateName(method)` でも `DelegateName d = method;` でも代入できる
- `?.Invoke()` を使うと、デリゲートが `null` のときでも安全に呼び出せる
- 代入するメソッドを変えることで、実行時に処理を切り替えられる

---

## 理解度チェック

以下の問いに答えられるか確認しましょう。

1. デリゲートを「メソッドの型」と考えられるのはなぜですか？
2. 次のコードの出力結果は何になりますか？

   ```csharp
   using System;

   public delegate void TextAction(string text);

   public class Program
   {
       public static void Main()
       {
           TextAction? handler = ShowA;
           handler?.Invoke("Start");

           handler = ShowB;
           handler?.Invoke("Next");
       }

       private static void ShowA(string text)
       {
           Console.WriteLine($"A:{text}");
       }

       private static void ShowB(string text)
       {
           Console.WriteLine($"B:{text}");
       }
   }
   ```

3. （応用）ゲームの状態に応じて、`HealSmall` と `HealLarge` のどちらかを呼ぶデリゲートを作るにはどう書きますか？

<details markdown="1">
<summary>解答を見る</summary>

1. デリゲートは「どんな引数を受け取り、どんな戻り値を返すメソッドを入れられるか」を型として定義するからです。
2. 次のように出力されます。

   ```
   A:Start
   B:Next
   ```

3. たとえば次のように書けます。

   ```csharp
   using System;

   public delegate void HealAction(int amount);

   public class Program
   {
       public static void Main()
       {
           bool isBossBattle = true;
           HealAction action = isBossBattle ? HealLarge : HealSmall;

           action(10);
       }

       private static void HealSmall(int amount)
       {
           Console.WriteLine($"小回復: {amount}");
       }

       private static void HealLarge(int amount)
       {
           Console.WriteLine($"大回復: {amount * 3}");
       }
   }
   ```

</details>

---

## 次のステップ

[デリゲートの変数渡しとコールバック](/unity-csharp-learning/csharp/delegate-callback/) では、デリゲートをメソッドのパラメータとして渡すコールバックパターンを学びます。
