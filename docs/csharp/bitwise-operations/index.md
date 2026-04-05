---
layout: page
title: ビット演算
permalink: /csharp/bitwise-operations/
---

# ビット演算

コンピューターはすべてのデータを 0 と 1 の**ビット**で扱います。**ビット演算**はそのビットを直接操作する演算です。フラグ管理・パフォーマンス最適化・低レベルなデータ操作などで使われます。

## 学習目標

- AND・OR・XOR・NOT の 4 種類のビット演算子を使える
- 左シフト・右シフトで値を 2 の倍数で乗除算できる
- ビットマスクでフラグを設定・確認・解除できる

## 前提知識

- [数値リテラルと型エイリアス（補足）](/unity-csharp-learning/csharp/numeric-literals/) を読んでいること（2進数・16進数リテラルの書き方を理解していること）

---

## 1. ビット演算子

4 つのビット演算子はビットを 1 本ずつ独立して処理します。

### & — AND（論理積）

両方のビットが `1` のとき、結果が `1` になります。

<svg viewBox="0 0 330 190" xmlns="http://www.w3.org/2000/svg" style="width:100%;max-width:440px;display:block;margin:1em 0;font-family:sans-serif;">
  <text x="100" y="18" text-anchor="middle" font-size="10" fill="#999">bit3</text>
  <text x="150" y="18" text-anchor="middle" font-size="10" fill="#999">bit2</text>
  <text x="200" y="18" text-anchor="middle" font-size="10" fill="#999">bit1</text>
  <text x="250" y="18" text-anchor="middle" font-size="10" fill="#999">bit0</text>
  <text x="45" y="48" text-anchor="middle" font-size="13" fill="#555">A =</text>
  <text x="45" y="98" text-anchor="middle" font-size="13" fill="#555">B =</text>
  <text x="34" y="125" text-anchor="middle" font-size="20" fill="#333">&amp;</text>
  <text x="42" y="158" text-anchor="middle" font-size="12" fill="#555">A &amp; B =</text>
  <rect x="75"  y="25" width="50" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="100" y="50" text-anchor="middle" font-size="22" fill="#e65100">1</text>
  <rect x="125" y="25" width="50" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="150" y="50" text-anchor="middle" font-size="22" fill="#e65100">1</text>
  <rect x="175" y="25" width="50" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="200" y="50" text-anchor="middle" font-size="22" fill="#333">0</text>
  <rect x="225" y="25" width="50" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="250" y="50" text-anchor="middle" font-size="22" fill="#333">0</text>
  <text x="285" y="50" font-size="12" fill="#888">= 12</text>
  <rect x="75"  y="75" width="50" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="100" y="100" text-anchor="middle" font-size="22" fill="#e65100">1</text>
  <rect x="125" y="75" width="50" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="150" y="100" text-anchor="middle" font-size="22" fill="#333">0</text>
  <rect x="175" y="75" width="50" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="200" y="100" text-anchor="middle" font-size="22" fill="#e65100">1</text>
  <rect x="225" y="75" width="50" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="250" y="100" text-anchor="middle" font-size="22" fill="#333">0</text>
  <text x="285" y="100" font-size="12" fill="#888">= 10</text>
  <line x1="75" y1="120" x2="275" y2="120" stroke="#888" stroke-width="1.5"/>
  <rect x="75"  y="130" width="50" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="100" y="155" text-anchor="middle" font-size="22" fill="#e65100">1</text>
  <rect x="125" y="130" width="50" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="150" y="155" text-anchor="middle" font-size="22" fill="#333">0</text>
  <rect x="175" y="130" width="50" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="200" y="155" text-anchor="middle" font-size="22" fill="#333">0</text>
  <rect x="225" y="130" width="50" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="250" y="155" text-anchor="middle" font-size="22" fill="#333">0</text>
  <text x="285" y="155" font-size="12" fill="#888">= 8</text>
</svg>

```csharp
int a = 0b_1100;  // 12
int b = 0b_1010;  // 10
Console.WriteLine(a & b);           // 8
Console.WriteLine($"{a & b:B4}");   // 1000
```

### | — OR（論理和）

どちらか一方でも `1` なら結果が `1` になります。

<svg viewBox="0 0 330 190" xmlns="http://www.w3.org/2000/svg" style="width:100%;max-width:440px;display:block;margin:1em 0;font-family:sans-serif;">
  <text x="100" y="18" text-anchor="middle" font-size="10" fill="#999">bit3</text>
  <text x="150" y="18" text-anchor="middle" font-size="10" fill="#999">bit2</text>
  <text x="200" y="18" text-anchor="middle" font-size="10" fill="#999">bit1</text>
  <text x="250" y="18" text-anchor="middle" font-size="10" fill="#999">bit0</text>
  <text x="45" y="48" text-anchor="middle" font-size="13" fill="#555">A =</text>
  <text x="45" y="98" text-anchor="middle" font-size="13" fill="#555">B =</text>
  <text x="34" y="125" text-anchor="middle" font-size="20" fill="#333">|</text>
  <text x="42" y="158" text-anchor="middle" font-size="12" fill="#555">A | B =</text>
  <rect x="75"  y="25" width="50" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="100" y="50" text-anchor="middle" font-size="22" fill="#e65100">1</text>
  <rect x="125" y="25" width="50" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="150" y="50" text-anchor="middle" font-size="22" fill="#e65100">1</text>
  <rect x="175" y="25" width="50" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="200" y="50" text-anchor="middle" font-size="22" fill="#333">0</text>
  <rect x="225" y="25" width="50" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="250" y="50" text-anchor="middle" font-size="22" fill="#333">0</text>
  <text x="285" y="50" font-size="12" fill="#888">= 12</text>
  <rect x="75"  y="75" width="50" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="100" y="100" text-anchor="middle" font-size="22" fill="#e65100">1</text>
  <rect x="125" y="75" width="50" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="150" y="100" text-anchor="middle" font-size="22" fill="#333">0</text>
  <rect x="175" y="75" width="50" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="200" y="100" text-anchor="middle" font-size="22" fill="#e65100">1</text>
  <rect x="225" y="75" width="50" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="250" y="100" text-anchor="middle" font-size="22" fill="#333">0</text>
  <text x="285" y="100" font-size="12" fill="#888">= 10</text>
  <line x1="75" y1="120" x2="275" y2="120" stroke="#888" stroke-width="1.5"/>
  <rect x="75"  y="130" width="50" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="100" y="155" text-anchor="middle" font-size="22" fill="#e65100">1</text>
  <rect x="125" y="130" width="50" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="150" y="155" text-anchor="middle" font-size="22" fill="#e65100">1</text>
  <rect x="175" y="130" width="50" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="200" y="155" text-anchor="middle" font-size="22" fill="#e65100">1</text>
  <rect x="225" y="130" width="50" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="250" y="155" text-anchor="middle" font-size="22" fill="#333">0</text>
  <text x="285" y="155" font-size="12" fill="#888">= 14</text>
</svg>

```csharp
int a = 0b_1100;
int b = 0b_1010;
Console.WriteLine(a | b);  // 14
```

### ^ — XOR（排他的論理和）

2 つのビットが**異なる**とき、結果が `1` になります。同じなら `0` です。

<svg viewBox="0 0 330 190" xmlns="http://www.w3.org/2000/svg" style="width:100%;max-width:440px;display:block;margin:1em 0;font-family:sans-serif;">
  <text x="100" y="18" text-anchor="middle" font-size="10" fill="#999">bit3</text>
  <text x="150" y="18" text-anchor="middle" font-size="10" fill="#999">bit2</text>
  <text x="200" y="18" text-anchor="middle" font-size="10" fill="#999">bit1</text>
  <text x="250" y="18" text-anchor="middle" font-size="10" fill="#999">bit0</text>
  <text x="45" y="48" text-anchor="middle" font-size="13" fill="#555">A =</text>
  <text x="45" y="98" text-anchor="middle" font-size="13" fill="#555">B =</text>
  <text x="34" y="125" text-anchor="middle" font-size="20" fill="#333">^</text>
  <text x="42" y="158" text-anchor="middle" font-size="12" fill="#555">A ^ B =</text>
  <rect x="75"  y="25" width="50" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="100" y="50" text-anchor="middle" font-size="22" fill="#e65100">1</text>
  <rect x="125" y="25" width="50" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="150" y="50" text-anchor="middle" font-size="22" fill="#e65100">1</text>
  <rect x="175" y="25" width="50" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="200" y="50" text-anchor="middle" font-size="22" fill="#333">0</text>
  <rect x="225" y="25" width="50" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="250" y="50" text-anchor="middle" font-size="22" fill="#333">0</text>
  <text x="285" y="50" font-size="12" fill="#888">= 12</text>
  <rect x="75"  y="75" width="50" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="100" y="100" text-anchor="middle" font-size="22" fill="#e65100">1</text>
  <rect x="125" y="75" width="50" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="150" y="100" text-anchor="middle" font-size="22" fill="#333">0</text>
  <rect x="175" y="75" width="50" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="200" y="100" text-anchor="middle" font-size="22" fill="#e65100">1</text>
  <rect x="225" y="75" width="50" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="250" y="100" text-anchor="middle" font-size="22" fill="#333">0</text>
  <text x="285" y="100" font-size="12" fill="#888">= 10</text>
  <line x1="75" y1="120" x2="275" y2="120" stroke="#888" stroke-width="1.5"/>
  <rect x="75"  y="130" width="50" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="100" y="155" text-anchor="middle" font-size="22" fill="#333">0</text>
  <rect x="125" y="130" width="50" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="150" y="155" text-anchor="middle" font-size="22" fill="#e65100">1</text>
  <rect x="175" y="130" width="50" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="200" y="155" text-anchor="middle" font-size="22" fill="#e65100">1</text>
  <rect x="225" y="130" width="50" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="250" y="155" text-anchor="middle" font-size="22" fill="#333">0</text>
  <text x="285" y="155" font-size="12" fill="#888">= 6</text>
</svg>

```csharp
int a = 0b_1100;
int b = 0b_1010;
Console.WriteLine(a ^ b);  // 6
```

### ~ — NOT（ビット反転）

すべてのビットを反転します（0→1、1→0）。

<svg viewBox="0 0 460 165" xmlns="http://www.w3.org/2000/svg" style="width:100%;max-width:600px;display:block;margin:1em 0;font-family:sans-serif;">
  <defs>
    <marker id="bn-b" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#1565c0"/></marker>
    <marker id="bn-f" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#bbb"/></marker>
  </defs>
  <text x="82" y="16" font-size="11" fill="#888">~（NOT）— 8 ビット表記（実際の int は 32 ビット）</text>
  <text x="2" y="42" font-size="11" fill="#555">入力</text><text x="2" y="55" font-size="11" fill="#555">(15)</text>
  <text x="2" y="120" font-size="11" fill="#555">~ 後</text><text x="2" y="133" font-size="11" fill="#555">(-16)</text>
  <text x="100" y="26" text-anchor="middle" font-size="10" fill="#999">bit7</text>
  <text x="136" y="26" text-anchor="middle" font-size="10" fill="#999">bit6</text>
  <text x="172" y="26" text-anchor="middle" font-size="10" fill="#999">bit5</text>
  <text x="208" y="26" text-anchor="middle" font-size="10" fill="#999">bit4</text>
  <text x="244" y="26" text-anchor="middle" font-size="10" fill="#999">bit3</text>
  <text x="280" y="26" text-anchor="middle" font-size="10" fill="#999">bit2</text>
  <text x="316" y="26" text-anchor="middle" font-size="10" fill="#999">bit1</text>
  <text x="352" y="26" text-anchor="middle" font-size="10" fill="#999">bit0</text>
  <rect x="82"  y="30" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="100" y="55" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="118" y="30" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="136" y="55" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="154" y="30" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="172" y="55" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="190" y="30" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="208" y="55" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="226" y="30" width="36" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="244" y="55" text-anchor="middle" font-size="20" fill="#e65100">1</text>
  <rect x="262" y="30" width="36" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="280" y="55" text-anchor="middle" font-size="20" fill="#e65100">1</text>
  <rect x="298" y="30" width="36" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="316" y="55" text-anchor="middle" font-size="20" fill="#e65100">1</text>
  <rect x="334" y="30" width="36" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="352" y="55" text-anchor="middle" font-size="20" fill="#e65100">1</text>
  <line x1="100" y1="70" x2="100" y2="93" stroke="#1565c0" stroke-width="1.5" marker-end="url(#bn-b)"/>
  <line x1="136" y1="70" x2="136" y2="93" stroke="#1565c0" stroke-width="1.5" marker-end="url(#bn-b)"/>
  <line x1="172" y1="70" x2="172" y2="93" stroke="#1565c0" stroke-width="1.5" marker-end="url(#bn-b)"/>
  <line x1="208" y1="70" x2="208" y2="93" stroke="#1565c0" stroke-width="1.5" marker-end="url(#bn-b)"/>
  <line x1="244" y1="70" x2="244" y2="93" stroke="#ccc" stroke-width="1.5" stroke-dasharray="4,3" marker-end="url(#bn-f)"/>
  <line x1="280" y1="70" x2="280" y2="93" stroke="#ccc" stroke-width="1.5" stroke-dasharray="4,3" marker-end="url(#bn-f)"/>
  <line x1="316" y1="70" x2="316" y2="93" stroke="#ccc" stroke-width="1.5" stroke-dasharray="4,3" marker-end="url(#bn-f)"/>
  <line x1="352" y1="70" x2="352" y2="93" stroke="#ccc" stroke-width="1.5" stroke-dasharray="4,3" marker-end="url(#bn-f)"/>
  <rect x="82"  y="100" width="36" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="100" y="125" text-anchor="middle" font-size="20" fill="#1565c0">1</text>
  <rect x="118" y="100" width="36" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="136" y="125" text-anchor="middle" font-size="20" fill="#1565c0">1</text>
  <rect x="154" y="100" width="36" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="172" y="125" text-anchor="middle" font-size="20" fill="#1565c0">1</text>
  <rect x="190" y="100" width="36" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="208" y="125" text-anchor="middle" font-size="20" fill="#1565c0">1</text>
  <rect x="226" y="100" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="244" y="125" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="262" y="100" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="280" y="125" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="298" y="100" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="316" y="125" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="334" y="100" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="352" y="125" text-anchor="middle" font-size="20" fill="#333">0</text>
</svg>

```csharp
int a = 0b_0000_1111;  // 15
Console.WriteLine(~a); // -16
```

`int` は 32 ビットなので、`0b_0000_1111` を反転すると上位ビットもすべて `1` になり、負の値になります（2 の補数表現）。

---

## 2. シフト演算子

ビット列を左右にずらす演算子です。

**書式：シフト演算子**
```
値 << ビット数    // 左シフト
値 >> ビット数    // 右シフト（算術）
値 >>> ビット数   // 右シフト（論理）※ C# 11 以降
```

| 演算子 | 意味 | 効果 |
|---|---|---|
| `<<` | 左シフト | ビットを左にずらす。右端に `0` が入る |
| `>>` | 算術右シフト | 符号ビットを保持しながら右にずらす |
| `>>>` | 論理右シフト | 符号に関わらず左端に `0` を入れて右にずらす（C# 11+） |

### 左シフト — 2 の乗算

左シフトでは、各ビットが左の位置へ移動し、右端に `0` が補充されます。

<svg viewBox="0 0 370 205" xmlns="http://www.w3.org/2000/svg" style="width:100%;max-width:480px;display:block;margin:1em 0;font-family:sans-serif;">
  <defs>
    <marker id="arr" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto">
      <polygon points="0 0, 8 3, 0 6" fill="#555"/>
    </marker>
    <marker id="arr-fade" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto">
      <polygon points="0 0, 8 3, 0 6" fill="#bbb"/>
    </marker>
    <marker id="arr-blue" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto">
      <polygon points="0 0, 8 3, 0 6" fill="#1565c0"/>
    </marker>
  </defs>

  <!-- 方向ラベル -->
  <text x="90" y="16" font-size="12" fill="#888">← ← ← 左シフト (&lt;&lt;1) ← ← ←</text>

  <!-- 行ラベル -->
  <text x="2" y="50" font-size="12" fill="#555">元の値</text>
  <text x="2" y="64" font-size="12" fill="#555">( = 1 )</text>
  <text x="2" y="150" font-size="12" fill="#555">&lt;&lt;1 後</text>
  <text x="2" y="164" font-size="12" fill="#555">( = 2 )</text>

  <!-- ビット番号ラベル（上行） -->
  <text x="115" y="26" text-anchor="middle" font-size="11" fill="#999">bit3</text>
  <text x="165" y="26" text-anchor="middle" font-size="11" fill="#999">bit2</text>
  <text x="215" y="26" text-anchor="middle" font-size="11" fill="#999">bit1</text>
  <text x="265" y="26" text-anchor="middle" font-size="11" fill="#999">bit0</text>

  <!-- 上段セル -->
  <rect x="90"  y="30" width="50" height="40" rx="4" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/>
  <text x="115" y="56" text-anchor="middle" font-size="22" fill="#333">0</text>

  <rect x="140" y="30" width="50" height="40" rx="4" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/>
  <text x="165" y="56" text-anchor="middle" font-size="22" fill="#333">0</text>

  <rect x="190" y="30" width="50" height="40" rx="4" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/>
  <text x="215" y="56" text-anchor="middle" font-size="22" fill="#333">0</text>

  <rect x="240" y="30" width="50" height="40" rx="4" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/>
  <text x="265" y="56" text-anchor="middle" font-size="22" fill="#e65100">1</text>

  <!-- 矢印：各ビットが1つ左の位置へ移動 -->
  <!-- bit3 → 消える（点線） -->
  <line x1="115" y1="70" x2="58" y2="118" stroke="#ccc" stroke-width="1.5" stroke-dasharray="5,4" marker-end="url(#arr-fade)"/>
  <text x="38" y="115" text-anchor="middle" font-size="11" fill="#bbb">消える</text>

  <!-- bit2 → bit3 -->
  <line x1="165" y1="70" x2="115" y2="118" stroke="#555" stroke-width="1.5" marker-end="url(#arr)"/>

  <!-- bit1 → bit2 -->
  <line x1="215" y1="70" x2="165" y2="118" stroke="#555" stroke-width="1.5" marker-end="url(#arr)"/>

  <!-- bit0（1）→ bit1 -->
  <line x1="265" y1="70" x2="215" y2="118" stroke="#e65100" stroke-width="1.5" marker-end="url(#arr)"/>

  <!-- 下段セル -->
  <rect x="90"  y="120" width="50" height="40" rx="4" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/>
  <text x="115" y="146" text-anchor="middle" font-size="22" fill="#333">0</text>

  <rect x="140" y="120" width="50" height="40" rx="4" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/>
  <text x="165" y="146" text-anchor="middle" font-size="22" fill="#333">0</text>

  <rect x="190" y="120" width="50" height="40" rx="4" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/>
  <text x="215" y="146" text-anchor="middle" font-size="22" fill="#e65100">1</text>

  <!-- bit0：0 補充 -->
  <rect x="240" y="120" width="50" height="40" rx="4" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/>
  <text x="265" y="146" text-anchor="middle" font-size="22" fill="#1565c0">0</text>

  <!-- 0 補充 矢印（下から） -->
  <line x1="265" y1="195" x2="265" y2="163" stroke="#1565c0" stroke-width="1.5" stroke-dasharray="4,3" marker-end="url(#arr-blue)"/>
  <text x="265" y="205" text-anchor="middle" font-size="11" fill="#1565c0">0 補充</text>
</svg>

1 ビット左にシフトするごとに値が 2 倍になります（`x << n` = `x × 2ⁿ`）。

```csharp
int x = 1;
Console.WriteLine(x << 1);  // 2  （1 × 2¹）
Console.WriteLine(x << 2);  // 4  （1 × 2²）
Console.WriteLine(x << 3);  // 8  （1 × 2³）
```

### 右シフト — 算術シフトと論理シフト

右シフトでは、各ビットが右の位置へ移動し、右端のビットは捨てられます。左端に何が入るかで 2 種類に分かれます。

**算術右シフト（`>>`）** — 符号ビット（最上位ビット）と同じ値で左端を埋めます。`int` などの**符号付き型**では `>>` が算術シフトになります。

正の数（符号ビット = 0）は左端に `0` が入り、値は半分になります。（以下は 8 ビット表記。実際の `int` は 32 ビット）

<svg viewBox="0 0 460 210" xmlns="http://www.w3.org/2000/svg" style="width:100%;max-width:600px;display:block;margin:1em 0;font-family:sans-serif;">
  <defs>
    <marker id="rp-a" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#555"/></marker>
    <marker id="rp-f" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#bbb"/></marker>
    <marker id="rp-b" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#1565c0"/></marker>
    <marker id="rp-o" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#e65100"/></marker>
  </defs>
  <text x="82" y="16" font-size="11" fill="#888">&gt;&gt;1（算術右シフト）— 正の数（16）</text>
  <text x="2" y="50" font-size="11" fill="#555">元の値</text><text x="2" y="63" font-size="11" fill="#555">( 16 )</text>
  <text x="2" y="167" font-size="11" fill="#555">&gt;&gt;1 後</text><text x="2" y="180" font-size="11" fill="#555">( 8 )</text>
  <text x="100" y="26" text-anchor="middle" font-size="10" fill="#999">bit7</text>
  <text x="136" y="26" text-anchor="middle" font-size="10" fill="#999">bit6</text>
  <text x="172" y="26" text-anchor="middle" font-size="10" fill="#999">bit5</text>
  <text x="208" y="26" text-anchor="middle" font-size="10" fill="#999">bit4</text>
  <text x="244" y="26" text-anchor="middle" font-size="10" fill="#999">bit3</text>
  <text x="280" y="26" text-anchor="middle" font-size="10" fill="#999">bit2</text>
  <text x="316" y="26" text-anchor="middle" font-size="10" fill="#999">bit1</text>
  <text x="352" y="26" text-anchor="middle" font-size="10" fill="#999">bit0</text>
  <rect x="82"  y="30" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="100" y="55" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="118" y="30" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="136" y="55" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="154" y="30" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="172" y="55" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="190" y="30" width="36" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="208" y="55" text-anchor="middle" font-size="20" fill="#e65100">1</text>
  <rect x="226" y="30" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="244" y="55" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="262" y="30" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="280" y="55" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="298" y="30" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="316" y="55" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="334" y="30" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="352" y="55" text-anchor="middle" font-size="20" fill="#333">0</text>
  <line x1="100" y1="70" x2="136" y2="118" stroke="#555" stroke-width="1.5" marker-end="url(#rp-a)"/>
  <line x1="136" y1="70" x2="172" y2="118" stroke="#555" stroke-width="1.5" marker-end="url(#rp-a)"/>
  <line x1="172" y1="70" x2="208" y2="118" stroke="#555" stroke-width="1.5" marker-end="url(#rp-a)"/>
  <line x1="208" y1="70" x2="244" y2="118" stroke="#e65100" stroke-width="1.5" marker-end="url(#rp-o)"/>
  <line x1="244" y1="70" x2="280" y2="118" stroke="#555" stroke-width="1.5" marker-end="url(#rp-a)"/>
  <line x1="280" y1="70" x2="316" y2="118" stroke="#555" stroke-width="1.5" marker-end="url(#rp-a)"/>
  <line x1="316" y1="70" x2="352" y2="118" stroke="#555" stroke-width="1.5" marker-end="url(#rp-a)"/>
  <line x1="352" y1="70" x2="415" y2="118" stroke="#ccc" stroke-width="1.5" stroke-dasharray="5,4" marker-end="url(#rp-f)"/>
  <text x="420" y="115" font-size="11" fill="#bbb">消える</text>
  <line x1="35" y1="142" x2="80" y2="142" stroke="#1565c0" stroke-width="1.5" stroke-dasharray="4,3" marker-end="url(#rp-b)"/>
  <text x="17" y="136" text-anchor="middle" font-size="10" fill="#1565c0">0 補充</text>
  <text x="17" y="149" text-anchor="middle" font-size="10" fill="#1565c0">(符号)</text>
  <rect x="82"  y="125" width="36" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="100" y="150" text-anchor="middle" font-size="20" fill="#1565c0">0</text>
  <rect x="118" y="125" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="136" y="150" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="154" y="125" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="172" y="150" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="190" y="125" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="208" y="150" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="226" y="125" width="36" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="244" y="150" text-anchor="middle" font-size="20" fill="#e65100">1</text>
  <rect x="262" y="125" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="280" y="150" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="298" y="125" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="316" y="150" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="334" y="125" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="352" y="150" text-anchor="middle" font-size="20" fill="#333">0</text>
</svg>

負の数（符号ビット = 1）は左端に `1` が補充され、符号が保たれます。

<svg viewBox="0 0 460 210" xmlns="http://www.w3.org/2000/svg" style="width:100%;max-width:600px;display:block;margin:1em 0;font-family:sans-serif;">
  <defs>
    <marker id="rn-a" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#555"/></marker>
    <marker id="rn-f" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#bbb"/></marker>
    <marker id="rn-r" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#c62828"/></marker>
    <marker id="rn-o" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#e65100"/></marker>
  </defs>
  <text x="82" y="16" font-size="11" fill="#888">&gt;&gt;1（算術右シフト）— 負の数（-16）</text>
  <text x="2" y="50" font-size="11" fill="#555">元の値</text><text x="2" y="63" font-size="11" fill="#555">( -16 )</text>
  <text x="2" y="167" font-size="11" fill="#555">&gt;&gt;1 後</text><text x="2" y="180" font-size="11" fill="#555">( -8 )</text>
  <text x="100" y="26" text-anchor="middle" font-size="10" fill="#999">bit7</text>
  <text x="136" y="26" text-anchor="middle" font-size="10" fill="#999">bit6</text>
  <text x="172" y="26" text-anchor="middle" font-size="10" fill="#999">bit5</text>
  <text x="208" y="26" text-anchor="middle" font-size="10" fill="#999">bit4</text>
  <text x="244" y="26" text-anchor="middle" font-size="10" fill="#999">bit3</text>
  <text x="280" y="26" text-anchor="middle" font-size="10" fill="#999">bit2</text>
  <text x="316" y="26" text-anchor="middle" font-size="10" fill="#999">bit1</text>
  <text x="352" y="26" text-anchor="middle" font-size="10" fill="#999">bit0</text>
  <rect x="82"  y="30" width="36" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="100" y="55" text-anchor="middle" font-size="20" fill="#e65100">1</text>
  <rect x="118" y="30" width="36" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="136" y="55" text-anchor="middle" font-size="20" fill="#e65100">1</text>
  <rect x="154" y="30" width="36" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="172" y="55" text-anchor="middle" font-size="20" fill="#e65100">1</text>
  <rect x="190" y="30" width="36" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="208" y="55" text-anchor="middle" font-size="20" fill="#e65100">1</text>
  <rect x="226" y="30" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="244" y="55" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="262" y="30" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="280" y="55" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="298" y="30" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="316" y="55" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="334" y="30" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="352" y="55" text-anchor="middle" font-size="20" fill="#333">0</text>
  <line x1="100" y1="70" x2="136" y2="118" stroke="#e65100" stroke-width="1.5" marker-end="url(#rn-o)"/>
  <line x1="136" y1="70" x2="172" y2="118" stroke="#e65100" stroke-width="1.5" marker-end="url(#rn-o)"/>
  <line x1="172" y1="70" x2="208" y2="118" stroke="#e65100" stroke-width="1.5" marker-end="url(#rn-o)"/>
  <line x1="208" y1="70" x2="244" y2="118" stroke="#e65100" stroke-width="1.5" marker-end="url(#rn-o)"/>
  <line x1="244" y1="70" x2="280" y2="118" stroke="#555" stroke-width="1.5" marker-end="url(#rn-a)"/>
  <line x1="280" y1="70" x2="316" y2="118" stroke="#555" stroke-width="1.5" marker-end="url(#rn-a)"/>
  <line x1="316" y1="70" x2="352" y2="118" stroke="#555" stroke-width="1.5" marker-end="url(#rn-a)"/>
  <line x1="352" y1="70" x2="415" y2="118" stroke="#ccc" stroke-width="1.5" stroke-dasharray="5,4" marker-end="url(#rn-f)"/>
  <text x="420" y="115" font-size="11" fill="#bbb">消える</text>
  <line x1="35" y1="142" x2="80" y2="142" stroke="#c62828" stroke-width="1.5" stroke-dasharray="4,3" marker-end="url(#rn-r)"/>
  <text x="17" y="136" text-anchor="middle" font-size="10" fill="#c62828">1 補充</text>
  <text x="17" y="149" text-anchor="middle" font-size="10" fill="#c62828">(符号)</text>
  <rect x="82"  y="125" width="36" height="40" rx="3" fill="#fce4ec" stroke="#e91e63" stroke-width="1.5"/><text x="100" y="150" text-anchor="middle" font-size="20" fill="#c62828">1</text>
  <rect x="118" y="125" width="36" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="136" y="150" text-anchor="middle" font-size="20" fill="#e65100">1</text>
  <rect x="154" y="125" width="36" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="172" y="150" text-anchor="middle" font-size="20" fill="#e65100">1</text>
  <rect x="190" y="125" width="36" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="208" y="150" text-anchor="middle" font-size="20" fill="#e65100">1</text>
  <rect x="226" y="125" width="36" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="244" y="150" text-anchor="middle" font-size="20" fill="#e65100">1</text>
  <rect x="262" y="125" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="280" y="150" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="298" y="125" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="316" y="150" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="334" y="125" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="352" y="150" text-anchor="middle" font-size="20" fill="#333">0</text>
</svg>

```csharp
int positive = 16;
Console.WriteLine(positive >> 1);  // 8（正の数は左端に 0 が入る）

int negative = -16;
Console.WriteLine(negative >> 1);  // -8（負の数は左端に 1 が入り、符号が保たれる）
```

**論理右シフト（`>>>`）** — 符号に関わらず左端に常に `0` を入れます（C# 11 以降）。

<svg viewBox="0 0 460 210" xmlns="http://www.w3.org/2000/svg" style="width:100%;max-width:600px;display:block;margin:1em 0;font-family:sans-serif;">
  <defs>
    <marker id="rl-a" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#555"/></marker>
    <marker id="rl-f" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#bbb"/></marker>
    <marker id="rl-b" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#1565c0"/></marker>
    <marker id="rl-o" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#e65100"/></marker>
  </defs>
  <text x="82" y="16" font-size="11" fill="#888">&gt;&gt;&gt;1（論理右シフト）— 負の数（-16）</text>
  <text x="2" y="50" font-size="11" fill="#555">元の値</text><text x="2" y="63" font-size="11" fill="#555">( -16 )</text>
  <text x="2" y="167" font-size="11" fill="#555">&gt;&gt;&gt;1 後</text><text x="2" y="180" font-size="11" fill="#555">（正の大値）</text>
  <text x="100" y="26" text-anchor="middle" font-size="10" fill="#999">bit7</text>
  <text x="136" y="26" text-anchor="middle" font-size="10" fill="#999">bit6</text>
  <text x="172" y="26" text-anchor="middle" font-size="10" fill="#999">bit5</text>
  <text x="208" y="26" text-anchor="middle" font-size="10" fill="#999">bit4</text>
  <text x="244" y="26" text-anchor="middle" font-size="10" fill="#999">bit3</text>
  <text x="280" y="26" text-anchor="middle" font-size="10" fill="#999">bit2</text>
  <text x="316" y="26" text-anchor="middle" font-size="10" fill="#999">bit1</text>
  <text x="352" y="26" text-anchor="middle" font-size="10" fill="#999">bit0</text>
  <rect x="82"  y="30" width="36" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="100" y="55" text-anchor="middle" font-size="20" fill="#e65100">1</text>
  <rect x="118" y="30" width="36" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="136" y="55" text-anchor="middle" font-size="20" fill="#e65100">1</text>
  <rect x="154" y="30" width="36" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="172" y="55" text-anchor="middle" font-size="20" fill="#e65100">1</text>
  <rect x="190" y="30" width="36" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="208" y="55" text-anchor="middle" font-size="20" fill="#e65100">1</text>
  <rect x="226" y="30" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="244" y="55" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="262" y="30" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="280" y="55" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="298" y="30" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="316" y="55" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="334" y="30" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="352" y="55" text-anchor="middle" font-size="20" fill="#333">0</text>
  <line x1="100" y1="70" x2="136" y2="118" stroke="#e65100" stroke-width="1.5" marker-end="url(#rl-o)"/>
  <line x1="136" y1="70" x2="172" y2="118" stroke="#e65100" stroke-width="1.5" marker-end="url(#rl-o)"/>
  <line x1="172" y1="70" x2="208" y2="118" stroke="#e65100" stroke-width="1.5" marker-end="url(#rl-o)"/>
  <line x1="208" y1="70" x2="244" y2="118" stroke="#e65100" stroke-width="1.5" marker-end="url(#rl-o)"/>
  <line x1="244" y1="70" x2="280" y2="118" stroke="#555" stroke-width="1.5" marker-end="url(#rl-a)"/>
  <line x1="280" y1="70" x2="316" y2="118" stroke="#555" stroke-width="1.5" marker-end="url(#rl-a)"/>
  <line x1="316" y1="70" x2="352" y2="118" stroke="#555" stroke-width="1.5" marker-end="url(#rl-a)"/>
  <line x1="352" y1="70" x2="415" y2="118" stroke="#ccc" stroke-width="1.5" stroke-dasharray="5,4" marker-end="url(#rl-f)"/>
  <text x="420" y="115" font-size="11" fill="#bbb">消える</text>
  <line x1="35" y1="142" x2="80" y2="142" stroke="#1565c0" stroke-width="1.5" stroke-dasharray="4,3" marker-end="url(#rl-b)"/>
  <text x="17" y="136" text-anchor="middle" font-size="10" fill="#1565c0">0 補充</text>
  <text x="17" y="149" text-anchor="middle" font-size="10" fill="#1565c0">(強制)</text>
  <rect x="82"  y="125" width="36" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="100" y="150" text-anchor="middle" font-size="20" fill="#1565c0">0</text>
  <rect x="118" y="125" width="36" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="136" y="150" text-anchor="middle" font-size="20" fill="#e65100">1</text>
  <rect x="154" y="125" width="36" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="172" y="150" text-anchor="middle" font-size="20" fill="#e65100">1</text>
  <rect x="190" y="125" width="36" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="208" y="150" text-anchor="middle" font-size="20" fill="#e65100">1</text>
  <rect x="226" y="125" width="36" height="40" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="244" y="150" text-anchor="middle" font-size="20" fill="#e65100">1</text>
  <rect x="262" y="125" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="280" y="150" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="298" y="125" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="316" y="150" text-anchor="middle" font-size="20" fill="#333">0</text>
  <rect x="334" y="125" width="36" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="352" y="150" text-anchor="middle" font-size="20" fill="#333">0</text>
</svg>

```csharp
int negative = -16;
Console.WriteLine(negative >>> 1);  // 2147483640（符号ビットが 0 になる）
```

符号なし型（`uint` など）に `>>` を使う場合は常に論理シフトになります。

> 💡 **ポイント**: 正の数の 2 の除算には `>>` を普通に使えます。負の数のビット列を符号を無視して扱いたい場合は `>>>` を使います。

---

## 3. ビットマスク — フラグの管理

複数の ON/OFF 状態を 1 つの整数値で管理するテクニックを**ビットマスク**と呼びます。各ビットが 1 つのフラグに対応します。

```csharp
// 各フラグを 1 ビットずつ割り当てる
const int FlagJump   = 0b_0001;  // ビット0: ジャンプ中
const int FlagRun    = 0b_0010;  // ビット1: 走り中
const int FlagCrouch = 0b_0100;  // ビット2: しゃがみ中
```

### フラグを立てる（OR）

```csharp
int state = 0;
state = state | FlagJump;  // ジャンプ中にする
state |= FlagRun;          // 走り中も追加（|= は複合代入）
Console.WriteLine($"{state:B4}");  // 0011
```

### フラグを確認する（AND）

特定のビットが `1` かどうかは、AND を使って 0 以外になるかで判定します。

```csharp
if ((state & FlagJump) != 0)
{
    Console.WriteLine("ジャンプ中");
}
```

### フラグを下ろす（AND + NOT）

```csharp
state &= ~FlagJump;  // ジャンプフラグだけを 0 にする
Console.WriteLine($"{state:B4}");  // 0010
```

`~FlagJump` は `FlagJump` のビットだけが `0` で残りが `1` のマスクになります。AND を取ることで、そのビットだけを `0` にできます。

### フラグを切り替える（XOR）

```csharp
state ^= FlagRun;  // 走り中なら OFF に、OFF なら ON にする
```

---

## ワンポイントアドバイス

**`[Flags]` 属性付き enum** — ビットマスクをより安全・わかりやすく書けます。

```csharp
[Flags]
enum PlayerState
{
    None   = 0,
    Jump   = 1 << 0,  // 1
    Run    = 1 << 1,  // 2
    Crouch = 1 << 2,  // 4
}

var state = PlayerState.Jump | PlayerState.Run;
Console.WriteLine(state);                          // Jump, Run
Console.WriteLine(state.HasFlag(PlayerState.Jump)); // True
```

`[Flags]` を付けると `ToString()` が `"Jump, Run"` のように読みやすく表示されます。`HasFlag` でフラグの確認もできます。

> 💡 **Unity での活用例**: `LayerMask` はビットマスクで複数のレイヤーを同時に指定する仕組みです。`Physics.Raycast` の `layerMask` パラメータなどで使われています。

---

## よくあるミス

### 論理演算子（`&&`・`||`）と混同する

```csharp
bool a = true;
bool b = false;

// ❌ NG: & はビット演算子。bool に使えるが短絡評価されない
if (a & b) { }

// ✅ OK: && は論理 AND。左辺が false なら右辺を評価しない（短絡評価）
if (a && b) { }
```

条件分岐には `&&`・`||` を使います。`&`・`|` はビット操作専用と考えると混乱しません。

### フラグの確認で `== 1` と書いてしまう

```csharp
// ❌ NG: 他のフラグも立っていると 1 にならない
if ((state & FlagJump) == 1) { }

// ✅ OK: 0 以外かどうかで判定する
if ((state & FlagJump) != 0) { }
```

---

## まとめ

- `&`（AND）— 両方 `1` のビットだけ `1`。フラグの確認・消去に使う
- `|`（OR）— どちらか `1` なら `1`。フラグの追加に使う
- `^`（XOR）— 異なるビットだけ `1`。フラグの切り替えに使う
- `~`（NOT）— すべてのビットを反転
- `<<`（左シフト）— 2 の乗算。`x << n` は `x × 2ⁿ`
- `>>`（算術右シフト）— 符号ビットを保持しながら右にずらす。正の数では 2 の除算
- `>>>`（論理右シフト、C# 11+）— 符号に関わらず左端に `0` を入れて右にずらす
- ビットマスクで複数の ON/OFF フラグを 1 つの整数で管理できる

---

## 理解度チェック

1. 次の式の結果を 2 進数と 10 進数で答えてください。

   ```csharp
   Console.WriteLine(0b_1111 & 0b_1010);
   Console.WriteLine(0b_1111 | 0b_1010);
   ```

2. `int flags = 0b_0101` のとき、ビット 1（`0b_0010`）のフラグを追加した結果はいくつ（10 進数）になりますか？

3. `1 << 4` の値はいくつですか？

<details markdown="1">
<summary>解答を見る</summary>

1. `0b_1111 & 0b_1010` = `0b_1010` = `10`。`0b_1111 | 0b_1010` = `0b_1111` = `15`。

2. `0b_0101 | 0b_0010` = `0b_0111` = `7`。

3. `16`。`1 × 2⁴ = 16`。

</details>

---

## 次のステップ

[ループ](/unity-csharp-learning/csharp/loops/) と組み合わせると、ビット列の全ビットを走査するような処理も書けるようになります。次のセクション「[C# 配列と集合操作](/unity-csharp-learning/csharp/)」では、複数の値をまとめて扱うデータ構造を学びます。

