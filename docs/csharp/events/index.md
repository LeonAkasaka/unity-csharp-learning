---
layout: page
title: イベント
permalink: /csharp/events/
---

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
var btn = new Button();
btn.Clicked += OnClick;

// ❌ クリックされていないのに、クラス外から直接呼び出せてしまう
btn.Clicked?.Invoke();

// ❌ クラス外から = で全登録を上書きできてしまう
btn.Clicked = null;

btn.Click();   // 登録が消えたので、何も呼ばれない

void OnClick() => Console.WriteLine("クリック！");

delegate void Notify();

class Button
{
    public Notify? Clicked;   // デリゲートをそのまま公開

    public void Click() => Clicked?.Invoke();
}
```

```
クリック！
```

表示された `クリック！` は、`btn.Click()` ではなく、クラスの外から `btn.Clicked?.Invoke()` で直接呼び出した結果です。そのあとの `btn.Click()` では、`= null` で登録が消えているため、何も表示されません。

「クリックされたこと」を知らせるのは `Button` の役目のはずです。ところが、デリゲートをそのまま公開すると、外のコードがクリックを偽装したり、ほかの購読者の登録を消したりできてしまいます。`event` を使うと、この直接呼び出しと上書きをクラス外から禁止できます。

---

## 2. `event` キーワード

**書式：[event キーワード](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/event)**
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
var btn = new Button();
btn.Clicked += OnClick;

btn.Click();

void OnClick() => Console.WriteLine("クリック！");

delegate void Notify();

class Button
{
    public event Notify? Clicked;   // event を付ける

    public void Click() => Clicked?.Invoke();   // 内部からは Invoke() できる
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
var manager = new ScoreManager();
var hud = new HUD();
hud.Subscribe(manager);

manager.AddScore(100);
manager.AddScore(50);

delegate void ScoreChangedHandler(int newScore);

// 発行者
class ScoreManager
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
class HUD
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
```

```
スコア表示を更新: 100
スコア表示を更新: 150
```

---

## 4. `EventHandler` 標準パターン

.NET には `EventHandler` と `EventHandler<TEventArgs>` という組み込みのデリゲート型があります。自前でデリゲート型を宣言せずにイベントを定義できます。

**`EventHandler`** — イベントのデータを持たないイベント用のデリゲート型です（`sender` と `e` の 2 つのパラメータは持ちます）。

**書式：[EventHandler デリゲート](https://learn.microsoft.com/dotnet/api/system.eventhandler)**
```csharp
public delegate void EventHandler(object? sender, EventArgs e);
```

| パラメータ | 型 | 説明 |
|---|---|---|
| `sender` | `object?` | イベントを発行したオブジェクト（発行者自身を渡す慣習） |
| `e` | `EventArgs` | イベントのデータ。追加情報がなければ `EventArgs.Empty` を渡す |

**`EventHandler<TEventArgs>`** — イベント固有のデータを渡せる汎用版です。

**書式：[EventHandler\<TEventArgs\> デリゲート](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)**
```csharp
public delegate void EventHandler<TEventArgs>(object? sender, TEventArgs e);
```

| パラメータ | 型 | 説明 |
|---|---|---|
| `sender` | `object?` | イベントを発行したオブジェクト |
| `e` | `TEventArgs` | イベントに付随するデータ |

イベントデータを渡すには、`EventArgs` を継承したクラスを作ります。

```csharp
var enemy = new Enemy();
var log = new BattleLog();
log.Subscribe(enemy);

enemy.TakeDamage(30);

// イベントデータクラス（EventArgs を継承）
class DamageEventArgs : EventArgs
{
    public int Amount { get; }
    public DamageEventArgs(int amount) { Amount = amount; }
}

// 発行者
class Enemy
{
    public event EventHandler<DamageEventArgs>? Damaged;

    public void TakeDamage(int amount)
    {
        Console.WriteLine($"敵が {amount} ダメージを受けた");
        Damaged?.Invoke(this, new DamageEventArgs(amount));
    }
}

// 購読者
class BattleLog
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
```

```
敵が 30 ダメージを受けた
ログ: ダメージ量 30 を記録
```

---

## 5. `add` / `remove` アクセサー

通常の `event` 宣言（フィールドライクイベント）では、`+=` / `-=` の動作はコンパイラが自動生成します。これを**自分で制御したい**場合は、`add` / `remove` アクセサーを明示的に定義できます。プロパティの `get` / `set` に相当するしくみです。

**書式：[add](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/add) / [remove](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/remove) アクセサーつきイベント**
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
var btn = new Button();
btn.Clicked += OnClick;    // add が呼ばれる
btn.Click();
btn.Clicked -= OnClick;    // remove が呼ばれる

void OnClick() => Console.WriteLine("クリック！");

class Button
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
// ❌ NG: クラス外から event を = で上書きしようとするとコンパイルエラー（CS0070）
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
   var c = new Counter();
   c.Incremented += ShowMessage;   // A
   c.Incremented = null;           // B
   c.Incremented?.Invoke();        // C

   void ShowMessage() => Console.WriteLine("増えた");

   delegate void Notify();

   class Counter
   {
       public event Notify? Incremented;
       public void Increment() => Incremented?.Invoke();
   }
   ```

3. （応用）`string` 型のメッセージをイベントデータとして渡す `MessageEventArgs` クラスと、それを使う `event EventHandler<MessageEventArgs>? MessageSent` を持つクラスを定義してください。

<details markdown="1">
<summary>解答を見る</summary>

1. ① `event` を付けると `=` による上書きがクラス外から禁止される。② `event` を付けると `Invoke()` の直接呼び出しがクラス外から禁止される。

2. B 行（`c.Incremented = null;`）と C 行（`c.Incremented?.Invoke();`）がコンパイルエラー（CS0070）になります。クラス外から `=` 代入と `Invoke()` は許可されないためです。A 行の `+=` は、クラス外からも使えます。

3. 使う側のコードも含めると、次のように書けます。

   ```csharp
   var messenger = new Messenger();
   messenger.MessageSent += OnMessageSent;
   messenger.Send("こんにちは");

   void OnMessageSent(object? sender, MessageEventArgs e)
   {
       Console.WriteLine($"受信: {e.Message}");
   }

   class MessageEventArgs : EventArgs
   {
       public string Message { get; }
       public MessageEventArgs(string message) { Message = message; }
   }

   class Messenger
   {
       public event EventHandler<MessageEventArgs>? MessageSent;

       public void Send(string message)
       {
           MessageSent?.Invoke(this, new MessageEventArgs(message));
       }
   }
   ```

   ```
   受信: こんにちは
   ```

</details>

---

## 次のステップ

[ラムダ式](/unity-csharp-learning/csharp/lambda/) では、メソッドをその場に短く書ける `=>` 構文を学びます。
