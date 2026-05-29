---
layout: page
title: キャラクターとメッセージウィンドウの連携
permalink: /conversation-scenes/scene-flow-control/
---

# キャラクターとメッセージウィンドウの連携

[キャラクター配置](/unity-csharp-learning/conversation-scenes/character-placement/) の続きです。メッセージウィンドウとキャラクター表示を別々に作っただけでは、会話シーン全体の流れはまだ管理できません。このページでは、キャラクターのフェードインが終わってからメッセージを表示する流れを `Update()` のフレーム駆動で実装します。

コルーチン、`Task`、外部ライブラリは使いません。まずは「待つ」とは何を確認し続けることなのかを、状態管理として考えます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- 複数のコンポーネントを 1 つの制御用スクリプトから呼び出せる
- `Update()` で毎フレーム状態を確認し、処理の完了を待てる
- `enum` を使って会話シーンの進行状態を管理できる
- キャラクター表示完了後にメッセージ表示を始める流れを実装できる

## 前提知識

- [メッセージウィンドウ — 文字送り](/unity-csharp-learning/conversation-scenes/typewriter-animation/) を読んでいること
- [キャラクター配置](/unity-csharp-learning/conversation-scenes/character-placement/) を読んでいること

---

## すぐに続けて呼ぶと何が起きるか

前回までに、メッセージを表示する `MessagePrinter` と、キャラクターをフェード表示する `CharacterView` を作りました。では、次のように両方を続けて呼ぶとどうなるでしょうか。

```csharp
_character.FadeIn();
_printer.ShowMessage("こんにちは。");
```

このコードは上から順に実行されます。しかし、`FadeIn()` は「フェードインを開始する」だけで、フェードインが終わるまでその場で止まるわけではありません。

前ページの `CharacterView` では、`FadeIn()` が `_targetAlpha` を `1f` に変更し、実際のアルファ値の更新は `Update()` が少しずつ行っていました。そのため、`FadeIn()` の直後に `ShowMessage()` を呼ぶと、キャラクターがまだフェード中なのにメッセージが表示され始めます。

今回作りたい流れは、次のような順番です。

1. キャラクターのフェードインを開始する
2. フェードインが終わるまで待つ
3. メッセージの文字送りを開始する
4. 文字送りが終わるまで待つ
5. 完了状態にする

ここで重要なのは、2 と 4 の「待つ」です。

---

## Update() で待つという考え方

Unity の `Update()` は毎フレーム呼ばれます。つまり、「完了したかどうか」を毎フレーム確認すれば、処理の終了を待つことができます。

たとえば、キャラクターがフェード中かどうかを `IsFading` というプロパティで確認できるなら、次のような考え方になります。

```csharp
private bool _isWaitingCharacter = false;

private void Update()
{
    if (_isWaitingCharacter && !_character.IsFading)
    {
        _isWaitingCharacter = false;
        _printer.ShowMessage("こんにちは。");
    }
}
```

この例では、`_isWaitingCharacter` が `true` の間だけキャラクターの状態を確認します。`_character.IsFading` が `false` になったら、フェードが終わったと判断してメッセージ表示に進みます。

ただし、会話シーンでは「キャラクター待ち」「メッセージ待ち」「完了」など複数の段階が出てきます。`bool` フラグを増やしていくと、どのフラグがどの状態を表しているのか分かりにくくなります。

そこで今回は、`enum` を使って「今どの段階か」を 1 つの値で表します。

---

## enum で進行状態を表す

**書式：enum 宣言**
```
private enum 型名
{
    値1,
    値2,
    値3
}
```

| 要素 | 説明 |
|---|---|
| `型名` | 状態の種類を表す名前 |
| `値1` など | その型が取りうる具体的な状態 |

`enum` は、決まった候補の中から 1 つを選んで保存したいときに使います。会話シーンの進行状態は「キャラクターを表示する」「キャラクターを待つ」「メッセージを表示する」のように候補が決まっているため、`enum` と相性が良いです。

今回は次の状態を用意します。

```csharp
private enum ConversationState
{
    ShowCharacter,
    WaitCharacter,
    ShowMessage,
    WaitMessage,
    Done
}
```

| 状態 | 役割 |
|---|---|
| `ShowCharacter` | キャラクターのフェードインを開始する |
| `WaitCharacter` | キャラクターのフェード完了を待つ |
| `ShowMessage` | メッセージの文字送りを開始する |
| `WaitMessage` | メッセージの文字送り完了を待つ |
| `Done` | 今回の会話処理が完了した状態 |

このように状態を分ける設計は、ステートマシンと呼ばれることがあります。今は難しく考えず、「今やるべき処理を 1 つの変数で管理する方法」と捉えてください。

---

## CharacterView に状態を読めるプロパティを追加する

`ScenarioSequencer` からキャラクターの完了を待つには、`CharacterView` が現在フェード中かどうかを外部に教える必要があります。

前回作った `CharacterView` に、次の 2 つのプロパティを追加します。

```csharp
public bool IsFading
{
    get => _image != null && !Mathf.Approximately(_image.color.a, _targetAlpha);
}

public bool IsVisible
{
    get => _image != null && _image.color.a > 0f;
}
```

**`Mathf.Approximately`** — 2 つの `float` 値がほぼ同じかどうかを判定するメソッドです。小数の計算では完全に同じ値にならないことがあるため、`==` ではなくこのメソッドを使います。<!-- [公式ドキュメント]() -->

**書式：Mathf.Approximately メソッド**
```csharp
bool Mathf.Approximately(float a, float b);
```

| パラメータ | 型 | 説明 |
|---|---|---|
| `a` | `float` | 比較する 1 つ目の値 |
| `b` | `float` | 比較する 2 つ目の値 |

`IsFading` は、現在のアルファ値と目標アルファ値がまだ一致していない間 `true` になります。`IsVisible` は、キャラクターが少しでも見えているかどうかを表します。

追加後の `CharacterView` は、次のようになります。

```csharp
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Image _image = default;

    [SerializeField]
    private float _fadeSpeed = 1f; // 1 秒あたりのアルファ変化量

    private float _targetAlpha = 0f; // 目標のアルファ値

    public bool IsFading
    {
        get => _image != null && !Mathf.Approximately(_image.color.a, _targetAlpha);
    }

    public bool IsVisible
    {
        get => _image != null && _image.color.a > 0f;
    }

    private void Start()
    {
        SetAlpha(0f); // 最初は完全に透明にしておく
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) // 左クリック
        {
            FadeIn();
        }
        else if (eventData.button == PointerEventData.InputButton.Right) // 右クリック
        {
            FadeOut();
        }
    }

    private void Update()
    {
        if (_image == null) { return; }

        // 現在のアルファを目標のアルファに向けて近づける
        var a = Mathf.MoveTowards(_image.color.a, _targetAlpha, _fadeSpeed * Time.deltaTime);
        SetAlpha(a);
    }

    /// <summary>
    /// キャラクターをフェードインさせます。
    /// </summary>
    public void FadeIn()
    {
        _targetAlpha = 1f;
    }

    /// <summary>
    /// キャラクターをフェードアウトさせます。
    /// </summary>
    public void FadeOut()
    {
        _targetAlpha = 0f;
    }

    private void SetAlpha(float alpha)
    {
        if (_image == null) { return; }

        var color = _image.color;
        color.a = alpha;
        _image.color = color;
    }
}
```

---

## ScenarioSequencer を作る

次に、キャラクターとメッセージをまとめて操作し、シナリオの流れを順番に進める `ScenarioSequencer` を作ります。Canvas または空の GameObject に新しい C# スクリプトとして追加してください。

`ScenarioSequencer` は、`CharacterView` と `MessagePrinter` を Inspector ビューから受け取ります。

```csharp
[SerializeField]
private CharacterView _character = default;

[SerializeField]
private MessagePrinter _printer = default;
```

表示するメッセージも、今回は 1 つだけ持たせます。

```csharp
[SerializeField]
private string _message = "こんにちは。";
```

状態ごとに処理を分けるには、`switch` 文を使います。

**書式：switch 文**
```
switch (調べる値)
{
    case 値1:
        処理;
        break;

    case 値2:
        処理;
        break;
}
```

| 要素 | 説明 |
|---|---|
| `調べる値` | どの状態かを確認したい変数 |
| `case 値` | 値が一致したときに実行する処理の入口 |
| `break` | その `case` の処理を終えて `switch` から抜ける |

完成版は次のようになります。

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class ScenarioSequencer : MonoBehaviour
{
    private enum ConversationState
    {
        ShowCharacter,
        WaitCharacter,
        ShowMessage,
        WaitMessage,
        Done
    }

    [SerializeField]
    private CharacterView _character = default;

    [SerializeField]
    private MessagePrinter _printer = default;

    [SerializeField]
    private string _message = "こんにちは。";

    private ConversationState _state = ConversationState.ShowCharacter;

    private void Start()
    {
        if (_character == null || _printer == null) { return; }

        _character.FadeIn();
        _state = ConversationState.WaitCharacter;
    }

    private void Update()
    {
        if (_character == null || _printer == null) { return; }

        switch (_state)
        {
            case ConversationState.ShowCharacter:
                _character.FadeIn();
                _state = ConversationState.WaitCharacter;
                break;

            case ConversationState.WaitCharacter:
                if (!_character.IsFading && _character.IsVisible)
                {
                    _state = ConversationState.ShowMessage;
                }
                break;

            case ConversationState.ShowMessage:
                _printer.ShowMessage(_message);
                _state = ConversationState.WaitMessage;
                break;

            case ConversationState.WaitMessage:
                UpdateMessageInput();
                break;

            case ConversationState.Done:
                break;
        }
    }

    private void UpdateMessageInput()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) { return; }

        if (_printer.IsPrinting)
        {
            _printer.Skip();
        }
        else
        {
            _state = ConversationState.Done;
        }
    }
}
```

`Start()` で `FadeIn()` を呼び、最初の状態を `WaitCharacter` にします。その後の `Update()` は、現在の `_state` に応じて処理を分けます。

`WaitCharacter` では、`!_character.IsFading && _character.IsVisible` を確認しています。これは「フェード中ではなく、かつ表示状態である」ことを意味します。この条件が成立したら、メッセージ表示を始める `ShowMessage` 状態へ進みます。

`WaitMessage` では、文字送り中にクリックされたら `Skip()` で全文表示し、文字送りが終わってからクリックされたら `Done` に進みます。

---

## Inspector ビューで参照を設定する

`ScenarioSequencer` を追加したら、Inspector ビューで次の参照を設定してください。

| フィールド | 設定するもの |
|---|---|
| `_character` | `CharacterView` コンポーネント |
| `_printer` | `MessagePrinter` コンポーネント |
| `_message` | 表示したい 1 つのメッセージ |

前のページのテストで `MessagePrinter` 側の `_message` フィールドに文字列を入れていた場合は、空に戻しておきましょう。今回のメッセージは `ScenarioSequencer` から `ShowMessage()` に渡します。

前のページで `CharacterView` をクリック操作のテスト用に作っていた場合でも、今回の制御では `ScenarioSequencer` から `FadeIn()` を呼びます。動作確認中にクリック操作が混ざって分かりにくい場合は、`CharacterView` の `IPointerClickHandler` 実装や `OnPointerClick()` を一時的に外してもかまいません。

---

## 動作確認

再生して、次の順番で動くか確認しましょう。

1. 再生開始後、キャラクターがフェードインする
2. フェード中はメッセージが表示されない
3. フェード完了後にメッセージの文字送りが始まる
4. 文字送り中にクリックすると、全文がすぐに表示される
5. 全文表示後にクリックすると、`Done` 状態に進む

`Done` 状態では、今回は何もしません。実際の会話シーンでは、ここから次のセリフへ進めたり、キャラクターをフェードアウトさせたり、会話シーン自体を閉じたりします。

---

## よくあるミス

```csharp
// ❌ NG: FadeIn() の直後に ShowMessage() を呼ぶと、フェード完了を待てない
_character.FadeIn();
_printer.ShowMessage(_message);

// ✅ OK: FadeIn() の後は状態を WaitCharacter にして、Update() で完了を待つ
_character.FadeIn();
_state = ConversationState.WaitCharacter;
```

```csharp
// ❌ NG: float の一致判定に == を使うと、期待通りにならないことがある
if (_image.color.a == _targetAlpha)
{
    // 完了
}

// ✅ OK: Mathf.Approximately を使って、ほぼ同じ値かどうかを確認する
if (Mathf.Approximately(_image.color.a, _targetAlpha))
{
    // 完了
}
```

```csharp
// ❌ NG: 文字送り中かどうかを確認せず、クリックでいきなり完了にしてしまう
if (Mouse.current.leftButton.wasPressedThisFrame)
{
    _state = ConversationState.Done;
}

// ✅ OK: 文字送り中ならスキップし、表示完了後のクリックで次へ進む
if (Mouse.current.leftButton.wasPressedThisFrame)
{
    if (_printer.IsPrinting) { _printer.Skip(); }
    else { _state = ConversationState.Done; }
}
```

---

## まとめ

- `FadeIn()` はフェード完了まで止まる処理ではなく、フェードの目標値を設定する処理
- `Update()` で毎フレーム状態を確認すると、コルーチンや `Task` を使わなくても「待つ」処理を表現できる
- `bool` フラグが増えそうな処理では、`enum` で現在の進行状態を 1 つの値として管理すると分かりやすい
- `ScenarioSequencer` のような制御役を作ると、`CharacterView` と `MessagePrinter` の役割を分けたままシナリオ全体の流れを組み立てられる

---

## 理解度チェック

以下の問いに答えられるか確認しましょう。

1. `FadeIn()` の直後に `ShowMessage()` を呼ぶと、なぜフェード完了を待てないのですか？
2. `ConversationState.WaitCharacter` は何を待つ状態ですか？
3. `bool` フラグを複数使う代わりに `enum` を使う利点は何ですか？

<details markdown="1">
<summary>解答を見る</summary>

1. `FadeIn()` は `_targetAlpha` を変更してフェードを開始するだけで、フェードが終わるまで処理を止めるわけではないためです。実際のアルファ更新は `Update()` で少しずつ行われます。
2. キャラクターのフェードが終わり、表示状態になることを待つ状態です。サンプルでは `!_character.IsFading && _character.IsVisible` を確認しています。
3. 「今どの段階か」を 1 つの変数で表せるため、複数の `bool` フラグの組み合わせを追うよりも状態を把握しやすくなります。

</details>

---

## 次のステップ

今回は `Update()` のフレーム駆動で、キャラクター表示を待ってからメッセージを表示する流れを作りました。この考え方を理解しておくと、次のような発展に進みやすくなります。

- コルーチンを使って、同じ流れを上から順に読める形に書き換える
- `Task` を使って、非同期処理として会話シーンの進行を表現する
- 複数のキャラクターや複数のメッセージウィンドウを管理する
- 会話内容を配列や外部データとして持ち、順番に再生する
