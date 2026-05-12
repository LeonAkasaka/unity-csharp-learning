---
layout: page
title: デリゲートの変数渡しとコールバック
permalink: /csharp/delegate-callback/
---

# デリゲートの変数渡しとコールバック

デリゲートをメソッドのパラメータとして渡すことで、処理完了後に呼び出すメソッドを呼び出し元が自由に指定できるようになります。このパターンを**コールバック**と呼びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- コールバックとはどのようなパターンかを説明できる
- デリゲート型をメソッドのパラメータとして受け取るコードを書ける
- `?.Invoke()` を使ってコールバックを安全に呼び出せる

## 前提知識

- [デリゲートの基本](/unity-csharp-learning/csharp/delegates/) を読んでいること

---

## 1. コールバックとは

コールバックとは、**処理が終わったときに呼び出してほしいメソッドを、あらかじめ渡しておくパターン**です。

身近なたとえでは、「荷物が届いたら連絡して」という依頼と同じです。荷物を届ける側（処理を実行する側）は「届けたあとに何をするか」を知らなくてよく、受け取り側（呼び出し元）が「届いたら電話する」「届いたらメールする」などを自由に決められます。

プログラムでは、このように「ある処理が完了した後に実行するメソッド」をあらかじめデリゲートとして渡しておく手法がコールバックです。

---

## 2. デリゲートをパラメータとして渡す

デリゲート型をメソッドのパラメータとして受け取ると、呼び出し元が「処理が終わったあとに何をするか」を指定できます。

**書式：デリゲートをパラメータとして受け取るメソッド**
```
戻り値型 メソッド名(デリゲート型 パラメーター名)
```

| 要素 | 説明 |
|---|---|
| `デリゲート型` | あらかじめ宣言した `delegate` 型。呼び出したいメソッドのシグネチャを表す |
| `パラメーター名` | 受け取ったデリゲートをメソッド内で参照するときの名前 |

書式では `?` を省略しています。コード例の `Callback?` の `?` は「このパラメータには `null` を渡してもよい」という意味です。

```csharp
using System;

public delegate void Callback();

public class Program
{
    public static void Main()
    {
        RunTask(OnTaskDone);
    }

    private static void RunTask(Callback? onComplete)
    {
        Console.WriteLine("タスクを実行します。");
        Console.WriteLine("タスクが終了しました。");
        onComplete?.Invoke();
    }

    private static void OnTaskDone()
    {
        Console.WriteLine("完了後のコールバックを実行しました。");
    }
}
```

```
タスクを実行します。
タスクが終了しました。
完了後のコールバックを実行しました。
```

この例では `Main` から `RunTask(OnTaskDone)` を呼び出し、`RunTask` の中で処理完了後に `onComplete?.Invoke()` を実行しています。

---

## 3. コールバックの活用例

同じメソッドに異なるコールバックを渡すと、同じ処理でも終了後の動作を変えられます。

```csharp
using System;

public delegate void OnDamageCalculated(int damage);

public class Program
{
    public static void Main()
    {
        CalculateDamage(20, 5, LogDamage);
        CalculateDamage(20, 5, ShakeScreen);
    }

    private static void CalculateDamage(int baseAtk, int defense, OnDamageCalculated? callback)
    {
        int damage = Math.Max(0, baseAtk - defense); // 0 を下回らないようにダメージを計算
        callback?.Invoke(damage);
    }

    private static void LogDamage(int damage)
    {
        Console.WriteLine($"ダメージ {damage} をログに記録しました。");
    }

    private static void ShakeScreen(int damage)
    {
        Console.WriteLine($"ダメージ {damage} — 画面をシェイク（プレースホルダー）");
    }
}
```

```
ダメージ 15 をログに記録しました。
ダメージ 15 — 画面をシェイク（プレースホルダー）
```

`callback?.Invoke(damage)` を使うことで、`callback` が `null` のときも安全に動作します。

---

## 4. コールバックを使った設計の利点

**疎結合（そけつごう）**とは、処理を実行するクラスが呼び出し元の詳細を知らずに済む設計のことです。

コールバックを使うと：

- 処理を実行する側は「完了したらデリゲートを呼ぶ」だけでよく、その後何が起きるかを知らなくてよい
- 呼び出し元は好きな処理を自由に渡せる
- 結果として、両者が独立して変更できるようになる

---

## よくあるミス

渡されたデリゲートをそのまま `callback(damage)` や `callback.Invoke(damage)` と呼び出してしまうと、`null` のときに実行時エラーになります。

```csharp
// ❌ NG: null の可能性があるデリゲートをそのまま呼び出している
callback(damage);

// ✅ OK: ?.Invoke() で null チェックをしてから呼び出す
callback?.Invoke(damage);
```

---

## まとめ

- コールバックとは、処理完了後に呼び出すメソッドをあらかじめデリゲートとして渡しておくパターンである
- デリゲート型をメソッドのパラメータとして宣言することで、呼び出し元が任意のメソッドを渡せる
- `?.Invoke()` を使うと、渡されたデリゲートが `null` でも安全に呼び出せる
- 同じメソッドに異なるコールバックを渡すことで、処理完了後の動作を柔軟に変えられる
- この設計により呼び出し元と処理実装側が疎結合になる

---

## 理解度チェック

### 問1（概念）
コールバックとはどのようなパターンですか？「荷物の配達」のたとえを使って説明してください。

### 問2（実行結果）
次のコードの出力結果は何になりますか？

```csharp
using System;

public delegate void ResultCallback(string result);

public class Program
{
    public static void Main()
    {
        Process("処理A", PrintResult);
        Process("処理B", null);
    }

    private static void Process(string name, ResultCallback? callback)
    {
        Console.WriteLine($"{name} を実行中...");
        callback?.Invoke($"{name} が完了");
    }

    private static void PrintResult(string result)
    {
        Console.WriteLine(result);
    }
}
```

### 問3（応用）
`int` を受け取り `void` を返すデリゲート型 `ScoreHandler` を宣言し、スコアを受け取るメソッド `ApplyScore(int score, ScoreHandler? onScore)` を定義するにはどう書きますか？ `onScore` は受け取ったスコアをそのまま渡して呼び出してください。

<details markdown="1"><summary>解答を見る</summary>

問1の解答：コールバックは、「荷物が届いたら連絡して」のように、処理を実行する側（荷物を届ける側）があらかじめ受け取っておいたメソッド（連絡先）を処理完了後に呼び出すパターンです。

問2の解答：
```
処理A を実行中...
処理A が完了
処理B を実行中...
```
（`Process("処理B", null)` では `callback` が `null` のため `?.Invoke()` は何も実行されない）

問3の解答：
```csharp
public delegate void ScoreHandler(int score);

public static void ApplyScore(int score, ScoreHandler? onScore)
{
    onScore?.Invoke(score);
}
```

</details>

---

## 次のステップ

[マルチキャストデリゲート](/unity-csharp-learning/csharp/multicast-delegates/) では、一つのデリゲートに複数のメソッドを登録して一括呼び出しする方法を学びます。
