---
layout: page
title: "補足: 現実時間の取得（DateTime と DateTimeOffset）"
permalink: /unity/time-datetime/
---

# 補足: 現実時間の取得（DateTime と DateTimeOffset）

`Time.time` はゲーム時間を返しますが、現実の日時（システムクロック）が必要な場面では C# 標準ライブラリの `DateTime` と `DateTimeOffset` を使います。

> **このページは補足資料です。** セーブデータへのタイムスタンプ記録や、プレイ日時のログ出力などで役立ちます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `DateTime.Now` で現在の日時を取得できる
- `DateTimeOffset.Now` でタイムゾーン情報付きの日時を取得できる
- `Time.time` との違いを説明できる

## 前提知識

- [Time クラスと時間制御](/unity-csharp-learning/unity/time-basics/) を読んでいること

---

## 1. DateTime — 現在の日時を取得する

**`DateTime`** は C# 標準ライブラリ（`System` 名前空間）の型で、日付と時刻を表します。<!-- [公式ドキュメント]() -->

**`DateTime.Now`** — 現在のローカル日時を返します。<!-- [公式ドキュメント]() -->

**書式：DateTime.Now プロパティ**
```csharp
public static DateTime Now { get; }
```

```csharp
using System;
using UnityEngine;

public class RealTimeSample : MonoBehaviour
{
    private void Start()
    {
        Debug.Log(DateTime.Now);
        // 例: 04/06/2026 00:07:30
    }
}
```

`DateTime.Now` はタイムゾーン情報を持たないため、ローカル時刻なのか UTC なのかがコードから読み取りにくいという欠点があります。

---

## 2. DateTimeOffset — タイムゾーン情報付きの日時

**`DateTimeOffset`** は `DateTime` にUTCオフセット（時差）を加えた型です。タイムゾーンの情報が明示されるため、より安全に扱えます。<!-- [公式ドキュメント]() -->

| プロパティ | 説明 |
|---|---|
| `DateTimeOffset.Now` | 現在のローカル日時（UTCオフセット付き） |
| `DateTimeOffset.UtcNow` | 現在のUTC日時 |

```csharp
using System;
using UnityEngine;

public class RealTimeSample : MonoBehaviour
{
    private void Start()
    {
        Debug.Log(DateTimeOffset.Now);
        // 例: 04/06/2026 00:07:30 +09:00

        Debug.Log(DateTimeOffset.UtcNow);
        // 例: 04/05/2026 15:07:30 +00:00
    }
}
```

UTCオフセット（`+09:00` など）が含まれるため、異なるタイムゾーンの環境でも日時の意味が明確です。

> 💡 **ポイント**: 新しいコードでは `DateTime` より `DateTimeOffset` を使うことが推奨されています。特に複数地域でプレイされるゲームのセーブデータやログには `DateTimeOffset.UtcNow` が適しています。

---

## 3. 書式を指定して出力する

`ToString` に書式文字列を渡すと、出力形式を自由に指定できます。

```csharp
using System;
using UnityEngine;

public class RealTimeSample : MonoBehaviour
{
    private void Start()
    {
        var now = DateTimeOffset.Now;
        Debug.Log(now.ToString("yyyy/MM/dd HH:mm:ss"));
        // 例: 2026/04/06 00:07:30
    }
}
```

| 書式指定子 | 意味 | 例 |
|---|---|---|
| `yyyy` | 4桁の年 | `2026` |
| `MM` | 2桁の月 | `04` |
| `dd` | 2桁の日 | `06` |
| `HH` | 24時間制の時 | `00` |
| `mm` | 分 | `07` |
| `ss` | 秒 | `30` |

---

## 4. Time.time との違いのまとめ

| | `Time.time` | `DateTimeOffset.Now` |
|---|---|---|
| 値の種類 | ゲーム開始からの経過秒数（`float`） | 現実の日時 |
| 影響を受けるもの | `Time.timeScale` | システムクロック |
| 主な用途 | タイマー・アニメーション・ゲームロジック | ログ・セーブデータのタイムスタンプ |
| 名前空間 | `UnityEngine` | `System` |

---

## まとめ

- `DateTime.Now` で現在のローカル日時を取得できる
- `DateTimeOffset.Now` / `DateTimeOffset.UtcNow` はタイムゾーン情報付きで安全
- 新しいコードでは `DateTime` より `DateTimeOffset` を推奨
- `Time.time` はゲーム時間、`DateTimeOffset.Now` は現実時間と用途が異なる

---

## 次のステップ

[Time クラスと時間制御](/unity-csharp-learning/unity/time-basics/) に戻り、ゲーム時間を使った duration 管理パターンを確認しましょう。
