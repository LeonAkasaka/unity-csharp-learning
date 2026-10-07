---
layout: page
title: Time クラスと時間制御
permalink: /unity/time-basics/
---

# Time クラスと時間制御

一定時間ごとに状態を切り替えたり、N秒後に何かを起こしたりするには、`Time` クラスを使って経過時間を管理します。このページでは経過時間（duration）の概念と、それをフィールドで管理する方法を学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `Time.time` でゲーム開始からの経過時間を取得できる
- フィールドと `Time.deltaTime` を組み合わせて duration を管理できる
- 一定間隔で状態をオンオフする処理を実装できる
- `Time.time` がゲーム時間であり現実時間とは異なることを説明できる
- `Time.fixedDeltaTime` が物理演算の間隔であり、`Time.timeScale` の影響を受けることを説明できる

## 前提知識

- [フィールドでデータを維持する](/unity-csharp-learning/unity/fields-basics/) を読んでいること
- 5 節は、[補足: Unity のメッセージと実行順序](/unity-csharp-learning/unity/messages/) の `FixedUpdate` の説明を前提にします

---

## 1. Time クラスの主なプロパティ

`Time` クラスは Unity の時間管理を担うクラスです。このページでは以下のプロパティを扱います。`Time.deltaTime` は、[Update メソッドと連続実行](/unity-csharp-learning/unity/update-basics/) で学んだものを、このページでも使います。

| プロパティ | 説明 | 解説しているページ |
|---|---|---|
| `Time.deltaTime` | 前のフレームからの経過時間（秒） | [Update メソッドと連続実行](/unity-csharp-learning/unity/update-basics/) |
| `Time.time` | ゲーム開始からの累計経過時間（秒） | このページ |
| `Time.timeScale` | 時間の流れる速さのスケール（既定値 `1.0`） | このページ |
| `Time.fixedDeltaTime` | 物理演算と `FixedUpdate` を実行する間隔（秒、既定値 `0.02`） | このページ |

---

## 2. Time.time — ゲーム開始からの経過時間

**`Time.time`** — ゲームが開始されてからの累計経過時間（秒）を返します。

**書式：[Time.time プロパティ](https://docs.unity3d.com/ScriptReference/Time-time.html)**
```csharp
public static float time { get; }
```

`Time.time` はゲーム開始時を `0` として毎フレーム増え続けます。「あの時点から何秒経ったか」を計測するには、開始時刻をフィールドに記録しておき差し引きます。

```csharp
using UnityEngine;

public class TimeSample : MonoBehaviour
{
    private float _startTime;

    private void Start()
    {
        _startTime = Time.time;  // 開始時刻を記録
    }

    private void Update()
    {
        float elapsed = Time.time - _startTime;
        Debug.Log($"経過時間: {elapsed:F1} 秒");
    }
}
```

`elapsed` は毎フレーム「現在の時刻 − 開始時刻」を計算するため、`Start` が呼ばれてからの経過秒数が得られます。

---

## 3. 一定間隔でオンオフを繰り返す

`Time.deltaTime` をフィールドに積算し、しきい値を超えたら状態を反転させることで、一定間隔の繰り返し処理を作れます。

```csharp
using UnityEngine;

public class Blinker : MonoBehaviour
{
    [SerializeField] private float _interval = 0.5f;  // 切り替え間隔（秒）
    private float _timer = 0f;
    private bool _isOn = true;

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _interval)
        {
            _timer -= _interval;  // リセットではなく差し引いて超過分を次へ持ち越す
            _isOn = !_isOn;
            Debug.Log(_isOn ? "ON" : "OFF");
        }
    }
}
```

`_timer = 0` ではなく `_timer -= _interval` とすることで、フレームの長さによる超過分が次のサイクルに持ち越されます。長時間動かしてもタイミングのズレが蓄積しません。

> 💡 **ポイント**: `Debug.Log` の行を `GetComponent<Renderer>().enabled = _isOn;` に置き換えると、オブジェクトの表示・非表示を一定間隔で切り替える点滅エフェクトになります。

---

## 4. ゲーム時間と現実時間

`Time.time` は**ゲーム時間**です。現実の時計とは異なり、**`Time.timeScale`** の値によって速さが変わります。

**`Time.timeScale`** — 時間の流れる速さのスケールを設定・取得します。

**書式：[Time.timeScale プロパティ](https://docs.unity3d.com/ScriptReference/Time-timeScale.html)**
```csharp
public static float timeScale { get; set; }
```

| 値 | 効果 |
|---|---|
| `1.0`（既定値） | 現実と同じ速さ |
| `0.5` | スローモーション（半速） |
| `2.0` | 倍速 |
| `0.0` | ゲームポーズ（時間が止まる） |

前のセクションで作った `Blinker` に `[SerializeField]` で `_timeScale` を持たせると、Inspector から値を変えるだけで点滅速度が変化することを確認できます。

```csharp
using UnityEngine;

public class Blinker : MonoBehaviour
{
    [SerializeField] private float _interval = 0.5f;
    [SerializeField] private float _timeScale = 1.0f;  // Inspector から変更できる
    private float _timer = 0f;
    private bool _isOn = true;

    private void Start()
    {
        Time.timeScale = _timeScale;
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _interval)
        {
            _timer -= _interval;
            _isOn = !_isOn;
            Debug.Log(_isOn ? "ON" : "OFF");
        }
    }
}
```

`_timeScale` を変えると `Time.deltaTime` の値が変わるため、`_timer` の積算速度が変化します。

| Inspector の `_timeScale` | `Time.deltaTime` の変化 | 現実時間での切り替わり |
|---|---|---|
| `0.5` | 半分になる | 1.0 秒ごと（遅い） |
| `1.0` | そのまま | 0.5 秒ごと（通常） |
| `2.0` | 2倍になる | 0.25 秒ごと（速い） |
| `0.0` | `0` になる | 切り替わらない（停止） |

> 💡 **ポイント**: 現実時間（システムクロック）を扱いたい場合は、C# 標準ライブラリの `DateTime`・`DateTimeOffset` を使います。詳しくは [補足: 現実時間の取得（DateTime と DateTimeOffset）](/unity-csharp-learning/unity/time-datetime/) を参照してください。

---

## 5. 物理演算の時間（Time.fixedDeltaTime）

`Time.deltaTime` は、1 フレームの時間でした。物理演算は、フレームとは別の一定の間隔で時間を進めます。[補足: Unity のメッセージと実行順序](/unity-csharp-learning/unity/messages/) で、`FixedUpdate` が 1 フレームに 0 回から複数回呼ばれる理由として紹介した間隔です。この間隔を表すのが **`Time.fixedDeltaTime`** です。

**`Time.fixedDeltaTime`** — 物理演算と `FixedUpdate` を実行する間隔（秒）を設定・取得します。

**書式：[Time.fixedDeltaTime プロパティ](https://docs.unity3d.com/ScriptReference/Time-fixedDeltaTime.html)**
```csharp
public static float fixedDeltaTime { get; set; }
```

既定値は `0.02`（1 秒間に 50 回）です。Editor では、**Edit → Project Settings → Time** の **Fixed Timestep** で設定します。ふつうは既定値のまま使います。

### timeScale と物理演算

`Time.fixedDeltaTime` の間隔は、`Time.deltaTime` と同じく**ゲーム時間**で数えます。そのため、`Time.timeScale` を変えると、物理演算の進む速さも変わります。

| `Time.timeScale` | 現実時間での物理演算の間隔 | Rigidbody の動き |
|---|---|---|
| `1.0`（既定値） | 0.02 秒ごと | 通常の速さ |
| `0.5` | 0.04 秒ごと | 半分の速さ（スローモーション） |
| `0.0` | 物理演算が進まない（`FixedUpdate` も呼ばれない） | 止まる |

4 節の `Blinker` で、物理演算が止まることを確かめます。

1. `Blinker` をアタッチしたゲームオブジェクトがあるシーンで、**GameObject → 3D Object → Cube** を選択して立方体を追加する
2. Cube の Inspector ビューで、Transform の Position を `(0, 3, 0)` にする
3. Cube の Inspector ビューの **Add Component** から **Rigidbody** を追加する
4. `Blinker` の Inspector ビューで **Time Scale** を `0` にして、Play ボタンを押す

Cube は空中で止まったまま落下せず、Console にも `ON` / `OFF` が表示されません。**Time Scale** を `0.5` にして Play し直すと、`1` のときの半分の速さで落下します。

`Time.timeScale` が `0` でも `Update` は毎フレーム呼ばれます。止まるのは、ゲーム時間で数える処理（`Time.deltaTime` を積算するタイマーや、物理演算と `FixedUpdate`）です。

> 💡 **ポイント**: `Time.timeScale` を `0.5` にすると、物理演算は現実時間の 0.04 秒ごとにしか進まないので、フレームレートが高い環境では動きがカクついて見えることがあります。スローモーションでも物理演算を滑らかに保ちたいときは、`Time.fixedDeltaTime` にも同じ倍率を掛けて、物理演算の間隔を短くします。

---

## よくあるミス

```csharp
// ❌ NG: _timer = 0 でリセットすると超過分が捨てられ、長時間でズレが蓄積する
if (_timer >= _interval)
{
    _timer = 0;
    _isOn = !_isOn;
}

// ✅ OK: 超過分を差し引いて次のサイクルに持ち越す
if (_timer >= _interval)
{
    _timer -= _interval;
    _isOn = !_isOn;
}
```

---

## まとめ

- `Time.time` はゲーム開始からの累計経過時間（ゲーム時間）
- 開始時刻をフィールドに記録し `Time.time - _startTime` で経過時間を計算できる
- `Time.deltaTime` をフィールドに積算してしきい値で処理するパターンで一定間隔の繰り返しを作れる
- `_timer -= _interval` で超過分を持ち越すとズレが生じない
- `Time.timeScale` でゲーム時間の速さを制御できる（`0` でポーズ、`Time.deltaTime` が `0` になりタイマーが止まる）
- `Time.fixedDeltaTime` は物理演算と `FixedUpdate` の間隔（既定 0.02 秒）。ゲーム時間で数えるので、`Time.timeScale` を `0` にすると物理演算も止まる

---

## 理解度チェック

以下の問いに答えられるか確認しましょう。

1. `Time.time` と `Time.deltaTime` の違いを説明してください。
2. `Time.timeScale = 0` にしたとき、`Time.deltaTime` ベースのタイマーはどうなりますか？
3. `Time.timeScale = 0` にしたとき、Rigidbody を付けたオブジェクトの落下はどうなりますか？
4. 次のコードは何秒ごとに状態を切り替えますか？

   ```csharp
   [SerializeField] private float _interval = 1.5f;
   private float _timer = 0f;

   private void Update()
   {
       _timer += Time.deltaTime;
       if (_timer >= _interval)
       {
           _timer -= _interval;
           // 状態を切り替える処理
       }
   }
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. `Time.time` はゲーム開始時を `0` とした**累計**経過時間。`Time.deltaTime` は**前のフレームから今のフレームまで**の短い経過時間。
2. `Time.deltaTime` が `0` になるため、`_timer` の積算が止まりタイマーが自動的にポーズされる。
3. 止まる。`Time.fixedDeltaTime` の間隔はゲーム時間で数えるので、ゲーム時間が進まないと物理演算も `FixedUpdate` も実行されない。
4. `_interval = 1.5f` なので、**1.5秒**ごとに切り替わる。

</details>

---

## 次のステップ

[チュートリアル: 信号機](/unity-csharp-learning/unity/traffic-light/) では、Update と Time を組み合わせたステートマシンパターンを総合演習として実装します。
