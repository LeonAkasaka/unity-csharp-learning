---
layout: page
title: ビットパッキング（補足）
permalink: /csharp/bit-packing/
---

# ビットパッキング（補足）

`bool[] flags = new bool[8]` は 8 つの真偽値を格納できますが、1 要素が内部的に 1 バイト以上占有します。これを 1 バイト（`byte`）の各ビットに詰め込むことで、メモリを節約しつつビット演算の練習にもなります。これを**ビットパッキング**と呼びます。

## 学習目標

- `bool[]` と `byte` の対応関係を理解できる
- for ループとビット演算を使って bool 配列を byte にパックできる
- byte から bool 配列にアンパックできる
- `BitArray` クラスの存在を知っている

## 前提知識

- [ビット演算](/unity-csharp-learning/csharp/bitwise-operations/) を読んでいること
- [配列の基礎](/unity-csharp-learning/csharp/arrays/) を読んでいること

---

## 1. 概念：bool[8] と byte の対応

`byte` は 8 ビットで、各ビットを独立したフラグとして扱えます。

<svg viewBox="0 0 400 100" xmlns="http://www.w3.org/2000/svg" style="width:100%;max-width:500px;display:block;margin:1em 0;font-family:monospace;">
  <text x="8" y="14" font-size="10" fill="#78909c">ビット位置</text>
  <text x="45"  y="14" text-anchor="middle" font-size="10" fill="#78909c">7</text>
  <text x="95"  y="14" text-anchor="middle" font-size="10" fill="#78909c">6</text>
  <text x="145" y="14" text-anchor="middle" font-size="10" fill="#78909c">5</text>
  <text x="195" y="14" text-anchor="middle" font-size="10" fill="#78909c">4</text>
  <text x="245" y="14" text-anchor="middle" font-size="10" fill="#78909c">3</text>
  <text x="295" y="14" text-anchor="middle" font-size="10" fill="#78909c">2</text>
  <text x="345" y="14" text-anchor="middle" font-size="10" fill="#78909c">1</text>
  <text x="385" y="14" text-anchor="middle" font-size="10" fill="#78909c">0</text>
  <rect x="20"  y="18" width="50" height="34" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="45"  y="40" text-anchor="middle" font-size="16" fill="#e65100">1</text>
  <rect x="70"  y="18" width="50" height="34" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="95"  y="40" text-anchor="middle" font-size="16" fill="#555">0</text>
  <rect x="120" y="18" width="50" height="34" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="145" y="40" text-anchor="middle" font-size="16" fill="#e65100">1</text>
  <rect x="170" y="18" width="50" height="34" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="195" y="40" text-anchor="middle" font-size="16" fill="#555">0</text>
  <rect x="220" y="18" width="50" height="34" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="245" y="40" text-anchor="middle" font-size="16" fill="#555">0</text>
  <rect x="270" y="18" width="50" height="34" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="295" y="40" text-anchor="middle" font-size="16" fill="#e65100">1</text>
  <rect x="320" y="18" width="50" height="34" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="345" y="40" text-anchor="middle" font-size="16" fill="#555">0</text>
  <rect x="360" y="18" width="40" height="34" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="1.5"/><text x="380" y="40" text-anchor="middle" font-size="16" fill="#e65100">1</text>
  <text x="8" y="72" font-size="10" fill="#78909c">bool[]</text>
  <text x="45"  y="72" text-anchor="middle" font-size="10" fill="#555">T</text>
  <text x="95"  y="72" text-anchor="middle" font-size="10" fill="#555">F</text>
  <text x="145" y="72" text-anchor="middle" font-size="10" fill="#555">T</text>
  <text x="195" y="72" text-anchor="middle" font-size="10" fill="#555">F</text>
  <text x="245" y="72" text-anchor="middle" font-size="10" fill="#555">F</text>
  <text x="295" y="72" text-anchor="middle" font-size="10" fill="#555">T</text>
  <text x="345" y="72" text-anchor="middle" font-size="10" fill="#555">F</text>
  <text x="385" y="72" text-anchor="middle" font-size="10" fill="#555">T</text>
  <text x="8"   y="90" font-size="10" fill="#78909c">byte</text>
  <text x="200" y="90" text-anchor="middle" font-size="11" fill="#555" font-family="monospace">= 0b10100101 = 165</text>
</svg>

ビット 7 が `bool[0]`（最上位ビット）、ビット 0 が `bool[7]`（最下位ビット）に対応させます。

---

## 2. Pack：bool[] → byte

bool 配列を byte に変換するには、各要素が `true` のときに対応ビットを OR で立てます。

```csharp
bool[] flags = { true, false, true, false, false, true, false, true };

byte Pack(bool[] flags)
{
    byte result = 0;
    for (int i = 0; i < 8; i++)
    {
        if (flags[i])
        {
            result |= (byte)(1 << (7 - i));  // ビット (7-i) を立てる
        }
    }
    return result;
}

byte packed = Pack(flags);
Console.WriteLine(packed);           // 165
Console.WriteLine($"0b{Convert.ToString(packed, 2).PadLeft(8, '0')}");
// 0b10100101
```

**ポイント：**
- `1 << (7 - i)` で i 番目の要素に対応するビットマスクを作る
- `|=` で該当ビットだけを 1 にする
- `(byte)` にキャストするのは `<<` の結果が `int` になるため

---

## 3. Unpack：byte → bool[]

byte から bool 配列に戻すには、各ビット位置を AND マスクで確認します。

```csharp
bool[] Unpack(byte packed)
{
    bool[] result = new bool[8];
    for (int i = 0; i < 8; i++)
    {
        result[i] = (packed & (1 << (7 - i))) != 0;
    }
    return result;
}

bool[] restored = Unpack(packed);

foreach (bool b in restored)
{
    Console.Write(b ? "T " : "F ");
}
// T F T F F T F T
```

**ポイント：**
- `packed & (1 << (7 - i))` でビット i が立っているか確認
- 結果が `0` でなければ `true`

---

## 4. ラウンドトリップ確認

Pack → Unpack を経ても元の配列が復元されることを確認します。

```csharp
bool[] original = { true, false, true, false, false, true, false, true };
byte packed      = Pack(original);
bool[] restored  = Unpack(packed);

for (int i = 0; i < 8; i++)
{
    Console.WriteLine($"[{i}] {original[i]} -> {restored[i]} {(original[i] == restored[i] ? "✓" : "✗")}");
}
// [0] True -> True ✓
// [1] False -> False ✓
// ...
```

---

## 5. BitArray クラス

`System.Collections.BitArray` を使うと、同様の操作をより簡単に行えます。

```csharp
using System.Collections;

var bits = new BitArray(new bool[] { true, false, true, false, false, true, false, true });

// byte に変換
byte[] bytes = new byte[1];
bits.CopyTo(bytes, 0);
Console.WriteLine(bytes[0]);  // 165（環境により LSB/MSB の順が異なる場合あり）
```

`BitArray` は AND・OR・XOR 演算をそのままメソッドで呼び出せる便利なクラスです。ただし、ビット順（LSB/MSB）や動作の細かい違いがあるため、本ページで紹介した手動実装の仕組みを先に理解しておくと応用が効きます。

---

## まとめ

- `bool[8]` は `byte` の 8 ビットと 1 対 1 で対応させられる
- Pack：`|= (byte)(1 << (7 - i))` で真のビットを立てる
- Unpack：`& (1 << (7 - i)) != 0` でビットが立っているか確認する
- `BitArray` を使うと同様の操作を簡潔に書ける

---

## 理解度チェック

1. `flags = { false, false, false, false, true, true, true, true }` を `Pack` するといくつになりますか？

2. `byte packed = 0b11110000` を `Unpack` すると `result[4]` はどうなりますか？

<details markdown="1">
<summary>解答を見る</summary>

1. ビット 3〜0 が立ちます。`0b00001111 = 15` です。

2. ビット 3（`1 << (7-4) = 1 << 3 = 8`）は `0b11110000 & 8 = 0` なので `false` です。

</details>

---

## 次のステップ

[多次元配列](/unity-csharp-learning/csharp/multidimensional-arrays/) では、行列形式のデータを 2 次元配列で扱う方法を学びます。
