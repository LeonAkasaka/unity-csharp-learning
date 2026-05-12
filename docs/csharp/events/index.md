---
layout: page
title: イベント
permalink: /csharp/events/
---

# イベント

`event` キーワードを使うと、マルチキャストデリゲートのバッキングフィールドを隠ぺいし、`+=` / `-=` だけを安全に外部へ公開できます。ゲームのスコア変化やボタンクリックなど「何かが起きた」ことを通知する設計に広く使われます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- 生のデリゲートを公開することの問題点を説明できる
- `add` / `remove` アクセサーを使ってイベントを手動実装できる
- `event` キーワードによる自動実装イベントを書ける
- 発行者（Publisher）と購読者（Subscriber）の役割を説明できる
- `EventHandler` / `EventHandler<TEventArgs>` の標準パターンを使える

## 前提知識

- [マルチキャストデリゲート](/unity-csharp-learning/csharp/multicast-delegates/) を読んでいること

---

## 1. 生のデリゲートを公開することの問題

マルチキャストデリゲートをそのまま `public` で公開すると、クラスの外から 2 つの危険な操作ができます。

```csharp
using System;

public delegate void Notify();

public class Button
{
    public Notify? Clicked;   // デリゲートをそのまま公開

    public void Click() => Clicked?.Invoke();
}

public class Program
{
    public static void Main()
    {
        var btn = new Button();
        btn.Clicked += OnClick;

        // ❌ 問題①: = で全登録を上書きできる
        btn.Clicked = null;

        // ❌ 問題②: クラスの外から直接呼び出せる
        btn.Clicked?.Invoke();

        btn.Click();   // 何も呼ばれない
    }

    private static void OnClick() => Console.WriteLine("クリック！");
}
```

これはプロパティで「`private` フィールドを直接 `public` にしてはいけない」のと同じ問題です。バッキングフィールドを隠し、外部からは `+=` / `-=` だけを許可する必要があります。

---

## 2. `add` / `remove` アクセサーで手動実装する

プロパティの `get` / `set` と同じ発想で、デリゲートの登録（`+=`）と解除（`-=`）をアクセサーで制御できます。

**書式：add / remove アクセサーつきイベント**
```
アクセス修飾子 event デリゲート型 イベント名
{
    add    { /* += されたときの処理 */ }
    remove { /* -= されたときの処理 */ }
}
```

| 要素 | 説明 |
|---|---|
| `add` | `+=` によってハンドラが登録されるときに実行されるブロック |
| `remove` | `-=` によってハンドラが解除されるときに実行されるブロック |
| `value` | 登録・解除しようとしているデリゲート（`set` の `value` と同じ発想）|

アクセサーを定義した場合、バッキングフィールド（デリゲートを保持する変数）は自分で用意します。

```csharp
using System;

public delegate void Notify();

public class Button
{
    // バッキングフィールドを private で隠す
    private Notify? _clickedHandlers;

    public event Notify Clicked
    {
        add    { _clickedHandlers += value; }
        remove { _clickedHandlers -= value; }
    }

    // 内部からのみ発火できる
    public void Click() => _clickedHandlers?.Invoke();
}

public class Program
{
    public static void Main()
    {
        var btn = new Button();
        btn.Clicked += OnClick;   // add が呼ばれる

        btn.Click();

        btn.Clicked -= OnClick;   // remove が呼ばれる
    }

    private static void OnClick() => Console.WriteLine("クリック！");
}
```

```
クリック！
```

`_clickedHandlers` は `private` なので、クラスの外からは `=` で上書きも直接呼び出しもできません。外部に見えるのは `+=` / `-=` だけです。

---

## 3. 自動実装イベント（省略形）

毎回バッキングフィールドと `add` / `remove` を書くのは冗長です。プロパティに `{ get; set; }` があるように、イベントにも自動実装の省略形があります。`event` キーワードだけを付けると、コンパイラがバッキングフィールドと `add` / `remove` の標準実装を自動生成します。

**書式：自動実装イベント**
```
アクセス修飾子 event デリゲート型 イベント名;
```

| 要素 | 説明 |
|---|---|
| `event` | バッキングフィールドと add/remove をコンパイラが自動生成するキーワード |

```csharp
using System;

public delegate void Notify();

public class Button
{
    // バッキングフィールドと add/remove はコンパイラが生成する
    public event Notify? Clicked;

    public void Click() => Clicked?.Invoke();
}

public class Program
{
    public static void Main()
    {
        var btn = new Button();
        btn.Clicked += OnClick;
        btn.Click();
    }

    private static void OnClick() => Console.WriteLine("クリック！");
}
```

```
クリック！
```

セクション 2 で書いたコードと同じ保護が、1 行で得られます。ほとんどの場面ではこの自動実装で十分です。`add` / `remove` を明示的に書くのは、登録時にログを出したい・弱参照で管理したい・スレッドセーフな実装が必要といった特殊な要件がある場合に限られます。

---

## 4. 発行者と購読者

イベントを使う設計では、役割を 2 つに分けます。

| 役割 | 説明 |
|---|---|
| **発行者（Publisher）** | イベントを宣言し、適切なタイミングで発火（`Invoke`）するクラス |
| **購読者（Subscriber）** | イベントに `+=` でメソッドを登録し、通知を受け取るクラス |

```csharp
using System;

public delegate void ScoreChangedHandler(int newScore);

// 発行者
public class ScoreManager
{
    private int _score;

    public event ScoreChangedHandler? ScoreChanged;

    public void AddScore(int value)
    {
        _score += value;
        ScoreChanged?.Invoke(_score);
    }
}

// 購読者
public class HUD
{
    public void Subscribe(ScoreManager manager)
    {
        manager.ScoreChanged += UpdateDisplay;
    }

    private void UpdateDisplay(int newScore)
    {
        Console.WriteLine($"スコア表示を更新: {newScore}");
    }
}

public class Program
{
    public static void Main()
    {
        var manager = new ScoreManager();
        var hud = new HUD();
        hud.Subscribe(manager);

        manager.AddScore(100);
        manager.AddScore(50);
    }
}
```

```
スコア表示を更新: 100
スコア表示を更新: 150
```

---

## 5. `EventHandler` 標準パターン

.NET には `EventHandler` と `EventHandler<TEventArgs>` という組み込みのデリゲート型があります。自前でデリゲート型を宣言せずにイベントを定義できます。

**`EventHandler`** — 引数なしのイベント用デリゲート型です。<!-- [公式ドキュメント]() -->

**書式：EventHandler デリゲート**
```csharp
public delegate void EventHandler(object? sender, EventArgs e);
```

| パラメータ | 型 | 説明 |
|---|---|---|
| `sender` | `object?` | イベントを発行したオブジェクト（発行者自身を渡す慣習） |
| `e` | `EventArgs` | イベントのデータ。追加情報がなければ `EventArgs.Empty` を渡す |

**`EventHandler<TEventArgs>`** — イベント固有のデータを渡せる汎用版です。<!-- [公式ドキュメント]() -->

**書式：EventHandler\<TEventArgs\> デリゲート**
```csharp
public delegate void EventHandler<TEventArgs>(object? sender, TEventArgs e);
```

| パラメータ | 型 | 説明 |
|---|---|---|
| `sender` | `object?` | イベントを発行したオブジェクト |
| `e` | `TEventArgs` | イベントに付随するデータ |

イベントデータを渡すには、`EventArgs` を継承したクラスを作ります。

```csharp
using System;

// イベントデータクラス（EventArgs を継承）
public class DamageEventArgs : EventArgs
{
    public int Amount { get; }
    public DamageEventArgs(int amount) { Amount = amount; }
}

// 発行者
public class Enemy
{
    public event EventHandler<DamageEventArgs>? Damaged;

    public void TakeDamage(int amount)
    {
        Console.WriteLine($"敵が {amount} ダメージを受けた");
        Damaged?.Invoke(this, new DamageEventArgs(amount));
    }
}

// 購読者
public class BattleLog
{
    public void Subscribe(Enemy enemy)
    {
        enemy.Damaged += OnDamaged;
    }

    private void OnDamaged(object? sender, DamageEventArgs e)
    {
        Console.WriteLine($"ログ: ダメージ量 {e.Amount} を記録");
    }
}

public class Program
{
    public static void Main()
    {
        var enemy = new Enemy();
        var log = new BattleLog();
        log.Subscribe(enemy);

        enemy.TakeDamage(30);
    }
}
```

```
敵が 30 ダメージを受けた
ログ: ダメージ量 30 を記録
```

---

## よくあるミス

```csharp
// ❌ NG: クラス外から event を = で上書きしようとするとコンパイルエラー
btn.Clicked = OnClick;

// ✅ OK: += で購読する
btn.Clicked += OnClick;
```

---

## まとめ

- 生のデリゲートを `public` にすると外部から `=` 上書きや直接呼び出しができてしまう
- `add` / `remove` アクセサーはプロパティの `get` / `set` と同じ発想でデリゲートの登録・解除を制御する
- `event` キーワード（自動実装）を使うと、バッキングフィールドと標準の `add` / `remove` をコンパイラが生成する
- `event` を付けるとクラス外からの `=` 上書きと `Invoke()` 直接呼び出しが禁止される
- 発行者がイベントを宣言して発火し、購読者が `+=` で受け取る（発行者/購読者パターン）
- `EventHandler<TEventArgs>` を使うと自前のデリゲート型を定義せずにイベントを実装できる

---

## 理解度チェック

以下の問いに答えられるか確認しましょう。

1. デリゲートをそのまま `public` にした場合と、`event` を付けた場合で、外部からできる操作の違いを答えてください。
2. `event` の `add` / `remove` アクセサーは、プロパティの何に相当しますか？
3. 次のコードでコンパイルエラーになるのはどの行ですか？理由も答えてください。

   ```csharp
   public class Counter
   {
       public event Action? Incremented;
       public void Increment() => Incremented?.Invoke();
   }

   var c = new Counter();
   c.Incremented += () => Console.WriteLine("増えた");   // A
   c.Incremented = null;                                  // B
   c.Incremented?.Invoke();                               // C
   ```

4. （応用）`add` / `remove` を明示的に書く必要があるのはどのような場面ですか？

<details markdown="1">
<summary>解答を見る</summary>

1. デリゲートを `public` にすると `=` による上書きと `Invoke()` の直接呼び出しが外部から可能。`event` を付けると `+=` / `-=` のみ許可され、`=` と `Invoke()` はクラス外から禁止される。

2. プロパティの `get` / `set` に相当します（`add` が `set`、`remove` は対応するものがプロパティにはないが、登録解除の口として機能する点でアクセサーの考え方は同じ）。

3. B 行（`c.Incremented = null;`）と C 行（`c.Incremented?.Invoke();`）がコンパイルエラーになります。クラス外から `=` 代入と `Invoke()` は許可されないためです。

4. 登録時にログを出力したい・弱参照でハンドラを管理したい・スレッドセーフな実装（`lock` など）が必要な場合など、標準の `+=` / `-=` 動作を変えたい特殊な要件があるとき。

</details>

---

## 次のステップ

[ラムダ式](/unity-csharp-learning/csharp/lambda/) では、メソッドを短く書ける `=>` 構文と、組み込みデリゲート型 `Action` / `Func` を学びます。


# イベント

`event` キーワードを使うと、デリゲートに制限を加えてクラスの外から安全に扱える**イベント**を定義できます。ゲームのスコア変化やボタンクリックなど「何かが起きた」ことを通知する設計に広く使われます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `event` キーワードでイベントを宣言できる
- 発行者（Publisher）と購読者（Subscriber）の役割を説明できる
- `EventHandler` / `EventHandler<TEventArgs>` の標準パターンを使える
- `event` とデリゲートの違いを説明できる

## 前提知識

- [マルチキャストデリゲート](/unity-csharp-learning/csharp/multicast-delegates/) を読んでいること

---

## 1. デリゲートだけだと何が問題か

マルチキャストデリゲートだけで通知を実装すると、**クラスの外から `=` で上書きしたり、直接呼び出したりできる**という問題があります。

```csharp
using System;

public delegate void Notify();

public class Button
{
    public Notify? Clicked;   // デリゲートをそのまま公開

    public void Click() => Clicked?.Invoke();
}

public class Program
{
    public static void Main()
    {
        var btn = new Button();
        btn.Clicked += OnClick;

        // ❌ クラス外から = で全登録を上書きできてしまう
        btn.Clicked = null;

        btn.Click();   // 何も呼ばれない
    }

    private static void OnClick() => Console.WriteLine("クリック！");
}
```

`event` を使うとこの上書きと直接呼び出しをクラス外から禁止できます。

---

## 2. `event` キーワード

**書式：イベントの宣言**
```
アクセス修飾子 event デリゲート型 イベント名;
```

| 要素 | 説明 |
|---|---|
| `event` | デリゲートをイベントとして公開するキーワード |
| `デリゲート型` | イベントのシグネチャを表すデリゲート型 |
| `イベント名` | イベントの名前（慣習として動詞または動詞句） |

`event` を付けると、クラスの**外からは `+=` / `-=` だけ**が許可され、`=` による上書きと直接呼び出し（`Invoke()`）は禁止されます。

```csharp
using System;

public delegate void Notify();

public class Button
{
    public event Notify? Clicked;   // event を付ける

    public void Click() => Clicked?.Invoke();   // 内部からは Invoke() できる
}

public class Program
{
    public static void Main()
    {
        var btn = new Button();
        btn.Clicked += OnClick;

        btn.Click();
    }

    private static void OnClick() => Console.WriteLine("クリック！");
}
```

```
クリック！
```

---

## 3. 発行者と購読者

イベントを使う設計では、役割を 2 つに分けます。

| 役割 | 説明 |
|---|---|
| **発行者（Publisher）** | イベントを宣言し、適切なタイミングで発火（`Invoke`）するクラス |
| **購読者（Subscriber）** | イベントに `+=` でメソッドを登録し、通知を受け取るクラス |

```csharp
using System;

public delegate void ScoreChangedHandler(int newScore);

// 発行者
public class ScoreManager
{
    private int _score;

    public event ScoreChangedHandler? ScoreChanged;

    public void AddScore(int value)
    {
        _score += value;
        ScoreChanged?.Invoke(_score);
    }
}

// 購読者
public class HUD
{
    public void Subscribe(ScoreManager manager)
    {
        manager.ScoreChanged += UpdateDisplay;
    }

    private void UpdateDisplay(int newScore)
    {
        Console.WriteLine($"スコア表示を更新: {newScore}");
    }
}

public class Program
{
    public static void Main()
    {
        var manager = new ScoreManager();
        var hud = new HUD();
        hud.Subscribe(manager);

        manager.AddScore(100);
        manager.AddScore(50);
    }
}
```

```
スコア表示を更新: 100
スコア表示を更新: 150
```

---

## 4. `EventHandler` 標準パターン

.NET には `EventHandler` と `EventHandler<TEventArgs>` という組み込みのデリゲート型があります。自前でデリゲート型を宣言せずにイベントを定義できます。

**`EventHandler`** — 引数なしのイベント用デリゲート型です。<!-- [公式ドキュメント]() -->

**書式：EventHandler デリゲート**
```csharp
public delegate void EventHandler(object? sender, EventArgs e);
```

| パラメータ | 型 | 説明 |
|---|---|---|
| `sender` | `object?` | イベントを発行したオブジェクト（発行者自身を渡す慣習） |
| `e` | `EventArgs` | イベントのデータ。追加情報がなければ `EventArgs.Empty` を渡す |

**`EventHandler<TEventArgs>`** — イベント固有のデータを渡せる汎用版です。<!-- [公式ドキュメント]() -->

**書式：EventHandler\<TEventArgs\> デリゲート**
```csharp
public delegate void EventHandler<TEventArgs>(object? sender, TEventArgs e);
```

| パラメータ | 型 | 説明 |
|---|---|---|
| `sender` | `object?` | イベントを発行したオブジェクト |
| `e` | `TEventArgs` | イベントに付随するデータ |

イベントデータを渡すには、`EventArgs` を継承したクラスを作ります。

```csharp
using System;

// イベントデータクラス（EventArgs を継承）
public class DamageEventArgs : EventArgs
{
    public int Amount { get; }
    public DamageEventArgs(int amount) { Amount = amount; }
}

// 発行者
public class Enemy
{
    public event EventHandler<DamageEventArgs>? Damaged;

    public void TakeDamage(int amount)
    {
        Console.WriteLine($"敵が {amount} ダメージを受けた");
        Damaged?.Invoke(this, new DamageEventArgs(amount));
    }
}

// 購読者
public class BattleLog
{
    public void Subscribe(Enemy enemy)
    {
        enemy.Damaged += OnDamaged;
    }

    private void OnDamaged(object? sender, DamageEventArgs e)
    {
        Console.WriteLine($"ログ: ダメージ量 {e.Amount} を記録");
    }
}

public class Program
{
    public static void Main()
    {
        var enemy = new Enemy();
        var log = new BattleLog();
        log.Subscribe(enemy);

        enemy.TakeDamage(30);
    }
}
```

```
敵が 30 ダメージを受けた
ログ: ダメージ量 30 を記録
```

---

## 5. `add` / `remove` アクセサー

通常の `event` 宣言（フィールドライクイベント）では、`+=` / `-=` の動作はコンパイラが自動生成します。これを**自分で制御したい**場合は、`add` / `remove` アクセサーを明示的に定義できます。プロパティの `get` / `set` に相当するしくみです。

**書式：add / remove アクセサーつきイベント**
```
アクセス修飾子 event デリゲート型 イベント名
{
    add    { /* += されたときの処理 */ }
    remove { /* -= されたときの処理 */ }
}
```

| 要素 | 説明 |
|---|---|
| `add` | `+=` によってハンドラが登録されるときに実行されるブロック |
| `remove` | `-=` によってハンドラが解除されるときに実行されるブロック |
| `value` | 登録・解除しようとしているデリゲート（暗黙的に使える変数） |

アクセサーを定義した場合、バッキングフィールド（デリゲートを保持する変数）は自分で用意します。

```csharp
using System;

public class Button
{
    // バッキングフィールドを自分で管理する
    private Action? _clickedHandlers;

    public event Action Clicked
    {
        add
        {
            Console.WriteLine("ハンドラを登録しました");
            _clickedHandlers += value;
        }
        remove
        {
            Console.WriteLine("ハンドラを解除しました");
            _clickedHandlers -= value;
        }
    }

    public void Click() => _clickedHandlers?.Invoke();
}

public class Program
{
    public static void Main()
    {
        var btn = new Button();
        btn.Clicked += OnClick;    // add が呼ばれる
        btn.Click();
        btn.Clicked -= OnClick;    // remove が呼ばれる
    }

    private static void OnClick() => Console.WriteLine("クリック！");
}
```

```
ハンドラを登録しました
クリック！
ハンドラを解除しました
```

> 💡 **ポイント**: ほとんどの場合、フィールドライクイベント（`public event Action Clicked;`）で十分です。`add` / `remove` を明示的に書くのは、登録時にログを出したい・弱参照で管理したい・スレッドセーフな実装が必要といった特殊な要件がある場合に限られます。

---

## よくあるミス

```csharp
// ❌ NG: クラス外から event を = で上書きしようとするとコンパイルエラー
btn.Clicked = OnClick;

// ✅ OK: += で購読する
btn.Clicked += OnClick;
```

---

## まとめ

- `event` キーワードを付けると、クラス外からの `=` 上書きと直接呼び出しが禁止される
- 発行者がイベントを宣言して発火し、購読者が `+=` で受け取る（発行者/購読者パターン）
- `EventHandler<TEventArgs>` を使うと自前のデリゲート型を定義せずにイベントを実装できる

---

## 理解度チェック

以下の問いに答えられるか確認しましょう。

1. `event` を付けないデリゲートと付けたイベントの違いを 2 つ挙げてください。
2. 次のコードでコンパイルエラーになるのはどの行ですか？理由も答えてください。

   ```csharp
   public class Counter
   {
       public event Action? Incremented;
       public void Increment() => Incremented?.Invoke();
   }

   var c = new Counter();
   c.Incremented += () => Console.WriteLine("増えた");   // A
   c.Incremented = null;                                  // B
   c.Incremented?.Invoke();                               // C
   ```

3. （応用）`string` 型のメッセージをイベントデータとして渡す `MessageEventArgs` クラスと、それを使う `event EventHandler<MessageEventArgs>? MessageSent` を持つクラスを定義してください。

<details markdown="1">
<summary>解答を見る</summary>

1. ① `event` を付けると `=` による上書きがクラス外から禁止される。② `event` を付けると `Invoke()` の直接呼び出しがクラス外から禁止される。

2. B 行（`c.Incremented = null;`）と C 行（`c.Incremented?.Invoke();`）がコンパイルエラーになります。クラス外から `=` 代入と `Invoke()` は許可されないためです。

3. ```csharp
   public class MessageEventArgs : EventArgs
   {
       public string Message { get; }
       public MessageEventArgs(string message) { Message = message; }
   }

   public class Messenger
   {
       public event EventHandler<MessageEventArgs>? MessageSent;

       public void Send(string message)
       {
           MessageSent?.Invoke(this, new MessageEventArgs(message));
       }
   }
   ```

</details>

---

## 次のステップ

[ラムダ式](/unity-csharp-learning/csharp/lambda/) では、メソッドを短く書ける `=>` 構文と、組み込みデリゲート型 `Action` / `Func` を学びます。
