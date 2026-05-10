---
layout: page
title: 線形補間アニメーション（Lerp）
permalink: /unity/lerp-animation/
---

# 線形補間アニメーション（Lerp）

「オブジェクトをゆっくり目標地点まで移動させたい」というときに使う**線形補間（Lerp）**を学びます。まず手計算で仕組みを理解し、次に Unity の便利なメソッドで書き直します。最後に「イージング」を使ってアニメーションに緩急をつける方法も学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- 線形補間の計算式を理解できる
- `Time.deltaTime` を使って毎フレーム進捗度を更新できる
- `Vector3.Lerp` でオブジェクトを滑らかに移動させられる
- イージング関数の概念を理解し、Linear / Ease In / Ease Out / Ease In Out を実装できる

## 前提知識

- [Update メソッドと連続実行](/unity-csharp-learning/unity/update-basics/) を読んでいること
- [Time クラスと時間制御](/unity-csharp-learning/unity/time-basics/) を読んでいること
- [チュートリアル: 信号機](/unity-csharp-learning/unity/traffic-light/) を読んでいること

---

## 1. 手計算で理解する線形補間

「X 座標を 0 から 10 に、2 秒かけて動かす」場面を考えます。1 秒後（全体の半分）なら値は 5、1.5 秒後（全体の 75%）なら値は 7.5 であるべきです。これを一般化すると次の式になります。

```
現在値 = 開始値 + (目標値 − 開始値) × t
```

**`t`（進捗度）** は 0 〜 1 の値で、0 が「まだ始まっていない」、1 が「完了した」を意味します。

```
t = 経過時間 ÷ アニメーション全体の時間
```

たとえば「全体 2 秒のアニメーションで 1.5 秒が経過した」場合、`t = 1.5 ÷ 2 = 0.75` です。

```
現在値 = 0 + (10 − 0) × 0.75 = 7.5
```

### 毎フレーム更新する

Unity では `Time.deltaTime` で「直前のフレームから今のフレームまでの経過時間（秒）」を取得できます。これを `Update` 内で積み重ねることで経過時間を計測し、毎フレーム `t` を更新できます。

**`Mathf.Clamp01`** — 値を 0 〜 1 の範囲に制限します。`Mathf.Clamp(value, 0f, 1f)` と同じ意味ですが、よく使うので専用メソッドが用意されています。<!-- [公式ドキュメント]() -->

**書式：Mathf.Clamp01 メソッド**
```csharp
public static float Clamp01(float value);
```

| パラメータ | 型 | 説明 |
|---|---|---|
| `value` | `float` | 制限したい値 |

戻り値: `float` — `value` が 0 未満なら `0`、1 より大きければ `1`、それ以外はそのまま返す

`_elapsed / _duration` は時間が経つにつれて 1 を超えます。`Clamp01` で制限しないと、オブジェクトが目標地点を通り越して動き続けてしまいます。

```csharp
using UnityEngine;

public class LerpSample : MonoBehaviour
{
    private float _elapsed  = 0f;   // 経過時間（秒）
    private float _duration = 2f;   // アニメーション全体の時間（秒）
    private float _startX   = 0f;   // 開始 X 座標
    private float _endX     = 10f;  // 目標 X 座標

    private void Update()
    {
        _elapsed += Time.deltaTime;

        float t = _elapsed / _duration;   // 進捗度（0 〜 ∞）
        t = Mathf.Clamp01(t);             // 0 〜 1 に制限

        float x = _startX + (_endX - _startX) * t;

        var pos = transform.position;
        pos.x = x;
        transform.position = pos;
    }
}
```

このコードを実行すると、オブジェクトは 2 秒かけて X = 0 から X = 10 まで等速で移動して止まります。

---

## 2. Unity の Lerp メソッド

前のセクションで書いた式 `開始値 + (目標値 − 開始値) × t` は、Unity が `Lerp` メソッドとして提供しています。

**`Vector3.Lerp`** — 2 つの `Vector3` を `t` の割合で線形補間した値を返します。<!-- [公式ドキュメント]() -->

**書式：Vector3.Lerp メソッド**
```csharp
public static Vector3 Lerp(Vector3 a, Vector3 b, float t);
```

| パラメータ | 型 | 説明 |
|---|---|---|
| `a` | `Vector3` | 開始値（`t = 0` のとき返る値） |
| `b` | `Vector3` | 終了値（`t = 1` のとき返る値） |
| `t` | `float` | 補間の進捗度（0 〜 1）。範囲外は自動的に丸められる |

`Vector3.Lerp(a, b, t)` は内部で `a + (b - a) * t` を計算します。手計算の式とまったく同じです。

先ほどのコードを `Vector3.Lerp` を使って書き直すと、次のようにすっきりします。

```csharp
using UnityEngine;

public class LerpSample : MonoBehaviour
{
    private float   _elapsed  = 0f;
    private float   _duration = 2f;
    private Vector3 _startPos;
    private Vector3 _endPos;

    private void Start()
    {
        _startPos = transform.position;
        _endPos   = transform.position + new Vector3(10f, 0f, 0f);
    }

    private void Update()
    {
        _elapsed += Time.deltaTime;

        float t = Mathf.Clamp01(_elapsed / _duration);
        transform.position = Vector3.Lerp(_startPos, _endPos, t);
    }
}
```

> 💡 **ポイント**: `Vector3.Lerp` の `t` は自動的に 0 〜 1 に丸められます。ただし明示的に `Clamp01` を書いておくと、コードの意図が読み手に伝わりやすくなります。

---

### Mathf.Lerp — float の補間

座標だけでなく、音量・透明度・速度などの `float` 値を補間したいときは **`Mathf.Lerp`** を使います。

**`Mathf.Lerp`** — 2 つの `float` を `t` の割合で線形補間した値を返します。<!-- [公式ドキュメント]() -->

**書式：Mathf.Lerp メソッド**
```csharp
public static float Lerp(float a, float b, float t);
```

| パラメータ | 型 | 説明 |
|---|---|---|
| `a` | `float` | 開始値 |
| `b` | `float` | 終了値 |
| `t` | `float` | 補間の進捗度（0 〜 1） |

```csharp
// 音量を 0 から 1 に 3 秒かけてフェードイン
float volume = Mathf.Lerp(0f, 1f, t);
```

`Vector3.Lerp` との違いは扱う型だけです。仕組みはまったく同じです。

---

## 3. イージング関数

線形補間では `t` が一定の速さで 0 → 1 へ進みます。これが「等速運動」のように見える原因です。

**イージング関数**とは、`t` の進み方を加工することで、アニメーションの見た目に**緩急**をつける関数です。加工した値を `t'` とすれば、`Lerp` の `t` に `t'` を渡すだけで実現できます。

### Linear（線形）

加工なし。等速で動きます。

```
t' = t
```

```csharp
float tEased = t;
transform.position = Vector3.Lerp(_startPos, _endPos, tEased);
```

---

### Ease In（ゆっくり始まり → 加速）

最初はゆっくりで、後半に向かって速くなります。動き始めの唐突感を和らげるのに使います。

```
t' = t × t
```

```csharp
float tEased = t * t;
transform.position = Vector3.Lerp(_startPos, _endPos, tEased);
```

---

### Ease Out（勢いよく始まり → 減速）

最初は速く、後半はゆっくり止まります。ボールが転がってゆっくり止まるような印象になります。

```
t' = 1 − (1 − t) × (1 − t)
```

```csharp
float tEased = 1f - (1f - t) * (1f - t);
transform.position = Vector3.Lerp(_startPos, _endPos, tEased);
```

---

### Ease In Out（ゆっくり始まり → 速くなり → ゆっくり終わる）

最初と最後はゆっくり、中間は速くなります。UI パネルの開閉やカメラの移動に向いています。

```
t' = t < 0.5 のとき → 2 × t × t
     t ≥ 0.5 のとき → 1 − 2 × (1 − t) × (1 − t)
```

```csharp
float tEased = t < 0.5f
    ? 2f * t * t
    : 1f - 2f * (1f - t) * (1f - t);
transform.position = Vector3.Lerp(_startPos, _endPos, tEased);
```

> 💡 **ポイント**: `t = 0.5` のとき、前半の式も後半の式も同じ値（`0.5`）を返します。そのため、中間点でなめらかにつながります。

---

### イージング込みの完全なサンプル

どのイージングを使うかを **`int` の数値**で切り替えるサンプルです。`0 = Linear`、`1 = Ease In`、`2 = Ease Out`、`3 = Ease In Out` とします。

```csharp
using UnityEngine;

public class EasingDemo : MonoBehaviour
{
    [SerializeField] private int   _easingType = 0;  // 0=Linear, 1=Ease In, 2=Ease Out, 3=Ease In Out
    [SerializeField] private float _duration   = 2f;

    private Vector3 _startPos;
    private Vector3 _endPos;
    private float   _elapsed = 0f;

    private void Start()
    {
        _startPos = transform.position;
        _endPos   = transform.position + new Vector3(5f, 0f, 0f);
    }

    private void Update()
    {
        _elapsed += Time.deltaTime;
        float t      = Mathf.Clamp01(_elapsed / _duration);
        float tEased = ApplyEasing(t);
        transform.position = Vector3.Lerp(_startPos, _endPos, tEased);
    }

    private float ApplyEasing(float t)
    {
        if (_easingType == 1)
        {
            return t * t;
        }

        if (_easingType == 2)
        {
            return 1f - (1f - t) * (1f - t);
        }

        if (_easingType == 3)
        {
            if (t < 0.5f)
            {
                return 2f * t * t;
            }

            return 1f - 2f * (1f - t) * (1f - t);
        }

        return t;
    }
}
```

Inspector の **Easing Type** を `0`、`1`、`2`、`3` に切り替えて、動きの違いを確認してみましょう。

---

## よくあるミス

```csharp
// ❌ NG: _elapsed を t に直接渡す（1 を超えると目標地点を通り越す）
transform.position = Vector3.Lerp(_startPos, _endPos, _elapsed);

// ✅ OK: Clamp01 で 0〜1 に制限してから渡す
float t = Mathf.Clamp01(_elapsed / _duration);
transform.position = Vector3.Lerp(_startPos, _endPos, t);
```

```csharp
// ❌ NG: 毎フレーム「今の位置」から補間すると、一定速度ではなく最後に近づくほどゆっくりになる
transform.position = Vector3.Lerp(transform.position, _endPos, 0.1f);

// ✅ OK: Start で記録した開始座標を _startPos フィールドに保持しておく
transform.position = Vector3.Lerp(_startPos, _endPos, t);
```

---

## まとめ

- 線形補間の式は `現在値 = 開始値 + (目標値 − 開始値) × t`
- `t = Mathf.Clamp01(経過時間 / アニメーション時間)` で 0 〜 1 の進捗度を作る
- `Vector3.Lerp(a, b, t)` は手計算の式と同じ計算を行う
- イージングは `t` を加工して動きに緩急をつける関数

---

## 理解度チェック

1. `t = 0.3` のとき、`Vector3.Lerp(Vector3.zero, new Vector3(10, 0, 0), t)` はどの座標を返しますか？
2. `Mathf.Clamp01` が必要な理由を説明してください。
3. Ease Out の式 `1 − (1 − t) × (1 − t)` において `t = 0` と `t = 1` を代入すると、それぞれいくつになりますか？

<details markdown="1">
<summary>解答を見る</summary>

1. `(3, 0, 0)` — `0 + (10 − 0) × 0.3 = 3`
2. `_elapsed / _duration` は時間が経つと 1 を超える。`Clamp01` で制限しないと、オブジェクトが目標地点を通り越してしまう。
3. `t = 0` のとき `1 − (1 − 0) × (1 − 0) = 0`、`t = 1` のとき `1 − (1 − 1) × (1 − 1) = 1`

</details>

---

## 視覚化デモ

イージング関数の違いを動かして確認できます。グラフの横軸は経過時間、縦軸は進捗度 `t'` です。

<div id="lerp-demo" style="border:1px solid #ddd;border-radius:8px;padding:20px;margin:24px 0;background:#fafafa;font-family:sans-serif;">

<h3 style="margin:0 0 16px;font-size:16px;">インタラクティブデモ</h3>

<div style="display:flex;flex-wrap:wrap;gap:12px;margin-bottom:16px;align-items:center;">
  <label style="display:flex;align-items:center;gap:6px;font-size:14px;">開始値
    <input id="ld-start" type="number" value="0" style="width:64px;padding:3px 6px;border:1px solid #ccc;border-radius:4px;">
  </label>
  <label style="display:flex;align-items:center;gap:6px;font-size:14px;">終了値
    <input id="ld-end" type="number" value="100" style="width:64px;padding:3px 6px;border:1px solid #ccc;border-radius:4px;">
  </label>
  <label style="display:flex;align-items:center;gap:6px;font-size:14px;">時間（秒）
    <input id="ld-dur" type="number" value="3" min="0.5" max="10" step="0.5" style="width:64px;padding:3px 6px;border:1px solid #ccc;border-radius:4px;">
  </label>
  <label style="display:flex;align-items:center;gap:6px;font-size:14px;">イージング
    <select id="ld-ease" style="padding:3px 6px;border:1px solid #ccc;border-radius:4px;">
      <option value="linear">Linear</option>
      <option value="easein">Ease In</option>
      <option value="easeout">Ease Out</option>
      <option value="easeinout">Ease In Out</option>
    </select>
  </label>
</div>

<svg id="ld-svg" viewBox="0 0 500 225" style="width:100%;max-width:540px;display:block;background:#fff;border:1px solid #e0e0e0;border-radius:4px;">
  <line x1="55" y1="15"  x2="480" y2="15"  stroke="#f0f0f0" stroke-width="1"/>
  <line x1="55" y1="57"  x2="480" y2="57"  stroke="#f0f0f0" stroke-width="1"/>
  <line x1="55" y1="99"  x2="480" y2="99"  stroke="#f0f0f0" stroke-width="1"/>
  <line x1="55" y1="141" x2="480" y2="141" stroke="#f0f0f0" stroke-width="1"/>
  <line x1="55" y1="183" x2="480" y2="183" stroke="#f0f0f0" stroke-width="1"/>
  <line x1="55" y1="15" x2="55" y2="183" stroke="#aaa" stroke-width="1.5"/>
  <line x1="55" y1="183" x2="480" y2="183" stroke="#aaa" stroke-width="1.5"/>
  <text x="48" y="18"  text-anchor="end" font-size="10" fill="#888">1</text>
  <text x="48" y="60"  text-anchor="end" font-size="10" fill="#888">.75</text>
  <text x="48" y="102" text-anchor="end" font-size="10" fill="#888">.5</text>
  <text x="48" y="144" text-anchor="end" font-size="10" fill="#888">.25</text>
  <text x="48" y="186" text-anchor="end" font-size="10" fill="#888">0</text>
  <text transform="rotate(-90)" x="-99" y="16" font-size="10" fill="#888">t'（進捗度）</text>
  <text x="55"  y="200" text-anchor="middle" font-size="10" fill="#888">0s</text>
  <text id="ld-xmax" x="480" y="200" text-anchor="middle" font-size="10" fill="#888">3s</text>
  <text x="268" y="218" text-anchor="middle" font-size="11" fill="#888">経過時間</text>
  <path id="ld-curve" d="" fill="none" stroke="#2196f3" stroke-width="2.5" stroke-linejoin="round" stroke-linecap="round"/>
  <line id="ld-vline" x1="55" y1="15" x2="55" y2="183" stroke="#e53935" stroke-width="1.5" stroke-dasharray="4,3" opacity="0"/>
  <circle id="ld-dot" cx="55" cy="183" r="5" fill="#e53935" stroke="#fff" stroke-width="2" opacity="0"/>
</svg>

<div style="margin-top:14px;padding:10px 14px;background:#f0f0f0;border-radius:6px;">
  <div style="font-size:12px;color:#666;margin-bottom:6px;">現在値</div>
  <div style="display:flex;align-items:center;gap:10px;flex-wrap:wrap;">
    <span id="ld-sval" style="font-size:12px;color:#999;min-width:36px;text-align:right;">0</span>
    <div id="ld-track" style="position:relative;flex:1;min-width:120px;max-width:320px;height:18px;background:#d0d0d0;border-radius:9px;overflow:hidden;">
      <div id="ld-ball" style="position:absolute;top:1px;bottom:1px;width:16px;height:16px;background:#2196f3;border-radius:50%;left:0px;box-shadow:0 1px 3px rgba(0,0,0,0.25);"></div>
    </div>
    <span id="ld-eval" style="font-size:12px;color:#999;min-width:36px;">100</span>
    <span id="ld-curval" style="font-size:15px;font-weight:bold;color:#333;min-width:64px;">0.00</span>
  </div>
</div>

<div id="ld-btns" style="display:flex;gap:8px;flex-wrap:wrap;margin-top:14px;"></div>

<div id="ld-timeinfo" style="font-size:12px;color:#999;margin-top:8px;min-height:18px;"></div>

</div>

<script>
(function () {
  var GX1 = 55, GX2 = 480, GY1 = 15, GY2 = 183;
  var GW = GX2 - GX1, GH = GY2 - GY1;
  var FRAME = 1 / 60;

  var _state = 'stopped';
  var _elapsed = 0;
  var _rafId = null;
  var _lastTs = null;

  function $$(id) { return document.getElementById(id); }
  var eStart  = $$('ld-start');
  var eEnd    = $$('ld-end');
  var eDur    = $$('ld-dur');
  var eEase   = $$('ld-ease');
  var eCurve  = $$('ld-curve');
  var eVline  = $$('ld-vline');
  var eDot    = $$('ld-dot');
  var eBall   = $$('ld-ball');
  var eSval   = $$('ld-sval');
  var eEval   = $$('ld-eval');
  var eCurval = $$('ld-curval');
  var eBtns   = $$('ld-btns');
  var eTime   = $$('ld-timeinfo');
  var eXmax   = $$('ld-xmax');

  function params() {
    return {
      start:    parseFloat(eStart.value)                   || 0,
      end:      parseFloat(eEnd.value)                     || 100,
      duration: Math.max(0.5, parseFloat(eDur.value)       || 3),
      easing:   eEase.value
    };
  }

  function applyEase(t, type) {
    if (t <= 0) return 0;
    if (t >= 1) return 1;
    if (type === 'easein')    return t * t;
    if (type === 'easeout')   return 1 - (1 - t) * (1 - t);
    if (type === 'easeinout') return t < 0.5 ? 2 * t * t : 1 - 2 * (1 - t) * (1 - t);
    return t;
  }

  function drawCurve() {
    var p = params();
    eXmax.textContent = p.duration.toFixed(1) + 's';
    eSval.textContent = p.start;
    eEval.textContent = p.end;
    var N = 120, d = '';
    for (var i = 0; i <= N; i++) {
      var t  = i / N;
      var te = applyEase(t, p.easing);
      var x  = (GX1 + t  * GW).toFixed(1);
      var y  = (GY2 - te * GH).toFixed(1);
      d += (i === 0 ? 'M' : 'L') + x + ',' + y + ' ';
    }
    eCurve.setAttribute('d', d);
  }

  function updateVisuals() {
    var p  = params();
    var t  = Math.min(_elapsed / p.duration, 1);
    var te = applyEase(t, p.easing);
    var cv = p.start + (p.end - p.start) * te;

    if (_state !== 'stopped' || _elapsed > 0) {
      var ix = (GX1 + t  * GW).toFixed(1);
      var iy = (GY2 - te * GH).toFixed(1);
      eVline.setAttribute('x1', ix); eVline.setAttribute('x2', ix);
      eVline.setAttribute('opacity', '0.85');
      eDot.setAttribute('cx', ix); eDot.setAttribute('cy', iy);
      eDot.setAttribute('opacity', '1');
    } else {
      eVline.setAttribute('opacity', '0');
      eDot.setAttribute('opacity', '0');
    }

    var track = $$('ld-track');
    var tw = track.offsetWidth || 200;
    eBall.style.left = Math.max(0, Math.min(tw - 16, te * (tw - 16))).toFixed(0) + 'px';

    eCurval.textContent = cv.toFixed(2);
    eTime.textContent   = '経過時間: ' + _elapsed.toFixed(2) + ' s / ' + p.duration.toFixed(1) + ' s   (t = ' + t.toFixed(3) + ')';
  }

  function mkBtn(label, primary, fn) {
    var b = document.createElement('button');
    b.textContent = label;
    b.style.cssText = 'padding:6px 16px;border-radius:4px;border:1px solid #bbb;cursor:pointer;font-size:13px;background:#fff;color:#333;';
    if (primary) { b.style.background = '#2196f3'; b.style.color = '#fff'; b.style.borderColor = '#1976d2'; }
    b.addEventListener('click', fn);
    return b;
  }

  function renderBtns() {
    eBtns.innerHTML = '';
    if (_state === 'stopped') {
      eBtns.appendChild(mkBtn('▶ 再生', true, doPlay));
    } else if (_state === 'playing') {
      eBtns.appendChild(mkBtn('⏸ 一時停止', false, doPause));
      eBtns.appendChild(mkBtn('⏹ 停止',     false, doStop));
    } else {
      eBtns.appendChild(mkBtn('▶ 再生', true, doPlay));
      eBtns.appendChild(mkBtn('⏹ 停止',   false, doStop));
      eBtns.appendChild(mkBtn('◀ 戻る',   false, doBack));
      eBtns.appendChild(mkBtn('進む ▶',   false, doFwd));
    }
  }

  function doPlay() {
    if (_state === 'stopped') _elapsed = 0;
    _state  = 'playing';
    _lastTs = null;
    renderBtns();
    _rafId = requestAnimationFrame(tick);
  }

  function doPause() {
    _state = 'paused';
    if (_rafId) { cancelAnimationFrame(_rafId); _rafId = null; }
    renderBtns();
  }

  function doStop() {
    _state   = 'stopped';
    _elapsed = 0;
    if (_rafId) { cancelAnimationFrame(_rafId); _rafId = null; }
    renderBtns();
    updateVisuals();
  }

  function doBack() {
    if (_state !== 'paused') return;
    _elapsed = Math.max(0, _elapsed - FRAME);
    updateVisuals();
  }

  function doFwd() {
    if (_state !== 'paused') return;
    _elapsed = Math.min(params().duration, _elapsed + FRAME);
    updateVisuals();
  }

  function tick(ts) {
    if (!_lastTs) _lastTs = ts;
    var delta = (ts - _lastTs) / 1000;
    _lastTs = ts;
    _elapsed += delta;
    var p = params();
    if (_elapsed >= p.duration) {
      _elapsed = p.duration;
      updateVisuals();
      _state = 'stopped';
      if (_rafId) { cancelAnimationFrame(_rafId); _rafId = null; }
      renderBtns();
      return;
    }
    updateVisuals();
    if (_state === 'playing') _rafId = requestAnimationFrame(tick);
  }

  [eStart, eEnd, eDur, eEase].forEach(function (inp) {
    inp.addEventListener('input',  function () { drawCurve(); if (_state !== 'playing') updateVisuals(); });
    inp.addEventListener('change', function () { drawCurve(); if (_state !== 'playing') updateVisuals(); });
  });

  drawCurve();
  updateVisuals();
  renderBtns();
}());
</script>

---

## 次のステップ

[Rigidbody で力を加える](/unity-csharp-learning/unity/rigidbody-force/) では、オブジェクトに物理的な力を加えて動かす方法を学びます。
