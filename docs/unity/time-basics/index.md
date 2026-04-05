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

## 前提知識

- [フィールドでデータを維持する](/unity-csharp-learning/unity/fields-basics/) を読んでいること

---

## 1. Time クラスの主なプロパティ

`Time` クラスは Unity の時間管理を担うクラスです。このページでは以下の3つのプロパティを扱います。

| プロパティ | 説明 | 既習 |
|---|---|---|
| `Time.deltaTime` | 前のフレームからの経過時間（秒） | ✅ |
| `Time.time` | ゲーム開始からの累計経過時間（秒） | このページ |
| `Time.timeScale` | 時間の流れる速さのスケール（既定値 `1.0`） | このページ |

---

## 2. Time.time — ゲーム開始からの経過時間

**`Time.time`** — ゲームが開始されてからの累計経過時間（秒）を返します。<!-- [公式ドキュメント]() -->

**書式：Time.time プロパティ**
```csharp
public static float time { get; }
```

`Time.time` はゲーム開始時を `0` として毎フレーム増え続けます。「あの時点から何秒経ったか」を計測するには、開始時刻をフィールドに記録しておき差し引きます。

```csharp
using UnityEngine;

public class TimeSample : MonoBehaviour
{
    private float startTime;

    private void Start()
    {
        startTime = Time.time;  // 開始時刻を記録
    }

    private void Update()
    {
        float elapsed = Time.time - startTime;
        Debug.Log($"経過時間: {elapsed:F1} 秒");
    }
}
```

`elapsed` は毎フレーム「現在の時刻 − 開始時刻」を計算するため、`Start` が呼ばれてからの経過秒数が得られます。

---

## 3. ゲーム時間と現実時間

`Time.time` は**ゲーム時間**です。現実の時計とは異なり、**`Time.timeScale`** の値によって速さが変わります。

**`Time.timeScale`** — 時間の流れる速さのスケールを設定・取得します。<!-- [公式ドキュメント]() -->

**書式：Time.timeScale プロパティ**
```csharp
public static float timeScale { get; set; }
```

| 値 | 効果 |
|---|---|
| `1.0`（既定値） | 現実と同じ速さ |
| `0.5` | スローモーション（半速） |
| `2.0` | 倍速 |
| `0.0` | ゲームポーズ（時間が止まる） |

`Time.timeScale` を変えると `Time.deltaTime` と `Time.time` の両方が影響を受けます。そのため `Time.deltaTime` を使ったタイマーは、**ゲームを一時停止したときに自動で止まる**という利点があります。

> 💡 **ポイント**: 現実時間（システムクロック）を扱いたい場合は、C# 標準ライブラリの `DateTime`・`DateTimeOffset` を使います。詳しくは [補足: 現実時間の取得（DateTime と DateTimeOffset）](/unity-csharp-learning/unity/time-datetime/) を参照してください。

---

## 4. 一定間隔でオンオフを繰り返す

`Time.deltaTime` をフィールドに積算し、しきい値を超えたら状態を反転させることで、一定間隔の繰り返し処理を作れます。

```csharp
using UnityEngine;

public class Blinker : MonoBehaviour
{
    [SerializeField] private float interval = 0.5f;  // 切り替え間隔（秒）
    private float timer = 0f;
    private bool isOn = true;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer -= interval;  // リセットではなく差し引いて超過分を次へ持ち越す
            isOn = !isOn;
            Debug.Log(isOn ? "ON" : "OFF");
        }
    }
}
```

`timer = 0` ではなく `timer -= interval` とすることで、フレームの長さによる超過分が次のサイクルに持ち越されます。長時間動かしてもタイミングのズレが蓄積しません。

> 💡 **ポイント**: `Debug.Log` の行を `GetComponent<Renderer>().enabled = isOn;` に置き換えると、オブジェクトの表示・非表示を一定間隔で切り替える点滅エフェクトになります。

---

## よくあるミス

```csharp
// ❌ NG: timer = 0 でリセットすると超過分が捨てられ、長時間でズレが蓄積する
if (timer >= interval)
{
    timer = 0;
    isOn = !isOn;
}

// ✅ OK: 超過分を差し引いて次のサイクルに持ち越す
if (timer >= interval)
{
    timer -= interval;
    isOn = !isOn;
}
```

---

## まとめ

- `Time.time` はゲーム開始からの累計経過時間（ゲーム時間）
- 開始時刻をフィールドに記録し `Time.time - startTime` で経過時間を計算できる
- `Time.timeScale` でゲーム時間の速さを制御できる（`0` でポーズ）
- `Time.deltaTime` をフィールドに積算してしきい値で処理するパターンで一定間隔の繰り返しを作れる
- `timer -= interval` で超過分を持ち越すとズレが生じない

---

## 理解度チェック

以下の問いに答えられるか確認しましょう。

1. `Time.time` と `Time.deltaTime` の違いを説明してください。
2. `Time.timeScale = 0` にしたとき、`Time.deltaTime` ベースのタイマーはどうなりますか？
3. 次のコードは何秒ごとに状態を切り替えますか？

   ```csharp
   [SerializeField] private float interval = 1.5f;
   private float timer = 0f;

   private void Update()
   {
       timer += Time.deltaTime;
       if (timer >= interval)
       {
           timer -= interval;
           // 状態を切り替える処理
       }
   }
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. `Time.time` はゲーム開始時を `0` とした**累計**経過時間。`Time.deltaTime` は**前のフレームから今のフレームまで**の短い経過時間。
2. `Time.deltaTime` が `0` になるため、`timer` の積算が止まりタイマーが自動的にポーズされる。
3. `interval = 1.5f` なので、**1.5秒**ごとに切り替わる。

</details>

---

## 次のステップ

[補足: 現実時間の取得（DateTime と DateTimeOffset）](/unity-csharp-learning/unity/time-datetime/) では、ゲーム時間ではなくシステムクロックから現在の日時を取得する方法を紹介します。
