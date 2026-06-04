---
layout: page
title: コルーチンの基本
permalink: /unity/coroutines/
---

# コルーチンの基本

`Update` メソッドは毎フレーム処理を書くための基本的な仕組みですが、「少し動かす → 1秒待つ → 別の動きをする」のように**順番のある処理**を書くと、状態管理が複雑になりがちです。このページでは、Unity のコルーチンを使って、時間をまたぐ処理を手続き的に書く方法を学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `Update` だけで時間差のある処理を書くと状態管理が必要になる理由を説明できる
- `yield return null` が次のフレームまで待つ合図であることを理解できる
- `StartCoroutine` でコルーチンを開始できる
- `WaitForSeconds` などの標準的な待機方法を説明できる

## 前提知識

- [Update メソッドと連続実行](/unity-csharp-learning/unity/update-basics/) を読んでいること
- [Time クラスと時間制御](/unity-csharp-learning/unity/time-basics/) を読んでいること
- [線形補間アニメーション（Lerp）](/unity-csharp-learning/unity/lerp-animation/) を読んでいること

---

## 1. なぜコルーチンが必要なのか

たとえば、オブジェクトに次のような演出をさせたいとします。

1. 2秒かけて右へ移動する
2. 1秒待つ
3. 2秒かけて元の位置へ戻る
4. 完了メッセージを表示する

`Update` で毎フレーム処理を書く場合、今どの段階なのかをフィールドに保存しておく必要があります。

```csharp
using UnityEngine;

public class UpdateSequenceSample : MonoBehaviour
{
    private int _state = 0;       // 0=右へ移動, 1=待つ, 2=戻る, 3=完了
    private float _timer = 0f;
    private Vector3 _startPos;
    private Vector3 _endPos;

    private void Start()
    {
        _startPos = transform.position;
        _endPos = _startPos + new Vector3(3f, 0f, 0f);
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_state == 0)
        {
            float t = Mathf.Clamp01(_timer / 2f);
            transform.position = Vector3.Lerp(_startPos, _endPos, t);

            if (t >= 1f)
            {
                _state = 1;
                _timer = 0f;
            }
        }
        else if (_state == 1)
        {
            if (_timer >= 1f)
            {
                _state = 2;
                _timer = 0f;
            }
        }
        else if (_state == 2)
        {
            float t = Mathf.Clamp01(_timer / 2f);
            transform.position = Vector3.Lerp(_endPos, _startPos, t);

            if (t >= 1f)
            {
                _state = 3;
                Debug.Log("完了");
            }
        }
    }
}
```

このコードは動きます。しかし、処理が増えるほど `_state` の種類が増え、`if` の分岐も増えます。「右へ動かす」「待つ」「戻る」という流れを読みたいだけなのに、タイマーのリセットや状態番号の切り替えに意識を取られます。

コルーチンを使うと、同じ流れを次のように上から下へ書けます。

```csharp
private IEnumerator MoveSequence()
{
    yield return MoveTo(_startPos, _endPos, 2f);
    yield return new WaitForSeconds(1f);
    yield return MoveTo(_endPos, _startPos, 2f);
    Debug.Log("完了");
}
```

> 💡 **ポイント**: コルーチンは「時間をまたぐ処理の続きを Unity に覚えておいてもらう」仕組みです。`Update` で自分で管理していた状態を、コルーチンの実行位置として表現できます。

---

## 2. Unity が IEnumerator を進める

コルーチンは `IEnumerator` を返すメソッドとして書きます。

```csharp
using System.Collections;
using UnityEngine;

public class CountFramesSample : MonoBehaviour
{
    private IEnumerator CountFrames()
    {
        Debug.Log("1フレーム目");
        yield return null;

        Debug.Log("2フレーム目");
        yield return null;

        Debug.Log("3フレーム目");
    }
}
```

`yield return null` は「次のフレームまで待つ」という合図です。Unity はこの `IEnumerator` を持っておき、次のフレームになったら続きを進めます。

`IEnumerator` や `yield return` の仕組みそのものを詳しく知りたい場合は、[補足: IEnumerator と yield return](/unity-csharp-learning/unity/ienumerator-yield/) を参照してください。このページでは、Unity がそれをフレーム待機や時間待ちに使う部分へ集中します。

`yield return` で止まるのは、アプリ全体でも、クラス全体でもありません。**止まるのは、そのコルーチンの続きだけ**です。他の `Update` や他のコンポーネントの処理は通常どおり動き続けます。

---

## 3. StartCoroutine なしで簡易コルーチンを作る

Unity の `StartCoroutine` を使う前に、自分で `IEnumerator` を毎フレーム進める簡単な仕組みを作ってみましょう。

```csharp
using System.Collections;
using UnityEngine;

public class SimpleCoroutineRunner : MonoBehaviour
{
    private IEnumerator _routine;

    private void Start()
    {
        _routine = SampleRoutine();
    }

    private void Update()
    {
        if (_routine == null) return;

        bool hasNext = _routine.MoveNext();
        if (!hasNext)
        {
            _routine = null;
        }
    }

    private IEnumerator SampleRoutine()
    {
        Debug.Log("開始");
        yield return null;

        Debug.Log("1フレーム待った");
        yield return null;

        Debug.Log("さらに1フレーム待った");
    }
}
```

この例では、`Update` が毎フレーム `_routine.MoveNext()` を呼んでいます。`yield return null` に到達するとそこで止まり、次の `Update` でまた続きから進みます。

ただし、この簡易版は `yield return null` のように「次の `MoveNext` まで待つ」動きだけを見せるためのものです。`yield return new WaitForSeconds(1f)` のような待機時間の処理や、`yield return` で別のコルーチンを返す処理には対応していません。

> 💡 **ポイント**: Unity の `StartCoroutine` は、このような `IEnumerator` を Unity 側で管理してくれる仕組みです。実際の開発では自分で `_routine.MoveNext()` を呼ぶのではなく、`StartCoroutine` を使います。

---

## 4. StartCoroutine の基本

Unity でコルーチンを実行するには、`StartCoroutine` を使います。

**`MonoBehaviour.StartCoroutine`** — `IEnumerator` を返すメソッドをコルーチンとして開始します。<!-- [公式ドキュメント]() -->

**書式：StartCoroutine メソッド**
```csharp
public Coroutine StartCoroutine(IEnumerator routine);
```

| パラメータ | 型 | 説明 |
|---|---|---|
| `routine` | `IEnumerator` | 実行したいコルーチン |

```csharp
using System.Collections;
using UnityEngine;

public class CoroutineStartSample : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(MoveAndWait());
    }

    private IEnumerator MoveAndWait()
    {
        Debug.Log("開始");
        yield return null;

        Debug.Log("次のフレーム");
        yield return new WaitForSeconds(1f);

        Debug.Log("1秒後");
    }
}
```

`StartCoroutine(MoveAndWait())` と書くことで、Unity が `MoveAndWait` の続きをフレームごとに管理します。

---

## 5. 標準の Wait 系クラス

コルーチンでは、`yield return` に何を返すかによって待ち方が変わります。

| 書き方 | 待ち方 |
|---|---|
| `yield return null;` | 次のフレームまで待つ |
| `yield return new WaitForSeconds(1f);` | ゲーム時間で1秒待つ |
| `yield return new WaitForSecondsRealtime(1f);` | 現実時間で1秒待つ |
| `yield return new WaitForEndOfFrame();` | そのフレームの描画処理の終わりまで待つ |
| `yield return new WaitUntil(() => 条件);` | 条件が `true` になるまで待つ |
| `yield return new WaitWhile(() => 条件);` | 条件が `true` の間待つ |
| `yield return new WaitForFixedUpdate();` | 次の FixedUpdate まで待つ（物理演算フレームに合わせる） |

`WaitForSeconds` はゲーム時間を使うため、`Time.timeScale` の影響を受けます。たとえば `Time.timeScale = 0` でゲームをポーズしている間は、`WaitForSeconds` の待ち時間も進みません。ポーズ中も現実時間で待ちたい場合は `WaitForSecondsRealtime` を使います。

---

## 6. Lerp とコルーチンを組み合わせる

最後に、最初に出てきた「右へ移動 → 待つ → 戻る」を `StartCoroutine` で書いてみます。

```csharp
using System.Collections;
using UnityEngine;

public class CoroutineMoveSample : MonoBehaviour
{
    private Vector3 _startPos;
    private Vector3 _endPos;

    private void Start()
    {
        _startPos = transform.position;
        _endPos = _startPos + new Vector3(3f, 0f, 0f);

        StartCoroutine(MoveSequence());
    }

    private IEnumerator MoveSequence()
    {
        yield return MoveTo(_startPos, _endPos, 2f);
        yield return new WaitForSeconds(1f);
        yield return MoveTo(_endPos, _startPos, 2f);

        Debug.Log("完了");
    }

    private IEnumerator MoveTo(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.position = Vector3.Lerp(from, to, t);

            yield return null;
        }

        transform.position = to;
    }
}
```

`MoveSequence` では、「移動する」「待つ」「戻る」という流れをそのまま上から下へ読めます。`MoveTo` の中では `while` で毎フレーム少しずつ移動し、最後に `yield return null` で次のフレームまで待っています。

ここで `yield return MoveTo(...)` と書くと、`MoveTo` のコルーチンが終わるまで `MoveSequence` の続きは待機します。`yield return` に `IEnumerator` を渡すと、Unity はその `IEnumerator` を内部でコルーチンとして実行し、完了してから呼び出し元の続きを進めます。

---

## コルーチンの停止

開始したコルーチンを途中で止めたい場合は `StopCoroutine` を使います。`StartCoroutine` の戻り値を `Coroutine` 型で受け取っておくと、後から停止できます。

```csharp
private Coroutine _routine;

private void Start()
{
    _routine = StartCoroutine(MoveAndWait());
}

private void Stop()
{
    StopCoroutine(_routine);
}
```

入門段階ではまず「開始する」「待つ」「順番に進める」ことを優先して理解しましょう。停止やスキップの細かい制御は、実践的なチュートリアルで扱います。

---

## よくあるミス

`StartCoroutine` を呼ばずにメソッドを直接呼ぶと、コルーチンとして動きません。

```csharp
// ❌ NG: メソッドを呼んでいるだけで、コルーチンとして開始していない
MoveAndWait();

// ✅ OK: Unity にコルーチンとして管理してもらう
StartCoroutine(MoveAndWait());
```

`MoveAndWait()` を呼ぶだけでは、`IEnumerator` オブジェクトが作られるだけです。Unity はその `IEnumerator` を管理しないため、コルーチンとしては進みません。

`while` ループの中に `yield return null` を書き忘れると、ループが1フレームで走り切ります。

```csharp
// ❌ NG: yield return null がないため、while が1フレームで走り切る
while (elapsed < duration)
{
    elapsed += Time.deltaTime;
    transform.position = Vector3.Lerp(from, to, elapsed / duration);
}

// ✅ OK: 毎フレーム少しずつ進める
while (elapsed < duration)
{
    elapsed += Time.deltaTime;
    transform.position = Vector3.Lerp(from, to, elapsed / duration);
    yield return null;
}
```

`yield return null` を忘れると、`while` は同じフレームの中で最後まで実行されます。アニメーションが一瞬で終わったり、長いループで Unity が固まったりする原因になります。

---

## まとめ

- `Update` で順番のある処理を書くと、状態番号やタイマーの管理が増えやすい
- `yield return null` は次のフレームまで待つ合図である
- `StartCoroutine` は `IEnumerator` を Unity 側で管理し、フレームや時間をまたいで続きを実行する
- `WaitForSeconds` などの Wait 系クラスで待ち方を指定できる

---

## 理解度チェック

以下の問いに答えられるか確認しましょう。

1. `Update` だけで「移動 → 待機 → 移動」のような処理を書くと、どのようなフィールド管理が必要になりますか？
2. `yield return null` は Unity のコルーチンでどのような意味を持ちますか？
3. `StartCoroutine(MyRoutine());` と `MyRoutine();` の違いを説明してください。
4. `WaitForSeconds` と `WaitForSecondsRealtime` の違いを説明してください。

<details markdown="1">
<summary>解答を見る</summary>

1. 今どの段階かを表す `_state`、経過時間を表す `_timer`、開始位置や終了位置などをフィールドとして管理する必要がある。
2. そのコルーチンの続きの実行を次のフレームまで待つ。
3. `StartCoroutine(MyRoutine())` は Unity にコルーチンとして管理してもらう。`MyRoutine()` だけでは `IEnumerator` が作られるだけで、自動的には進まない。
4. `WaitForSeconds` は `Time.timeScale` の影響を受けるゲーム時間で待つ。`WaitForSecondsRealtime` は `Time.timeScale` の影響を受けない現実時間で待つ。

</details>

---

## 次のステップ

今後追加する「イベント・コールバックと Unity」では、処理の完了や入力を別の処理へ通知する方法を学びます。
