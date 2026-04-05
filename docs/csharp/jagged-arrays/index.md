---
layout: page
title: ジャグ配列
permalink: /csharp/jagged-arrays/
---

# ジャグ配列

**ジャグ配列**（jagged array）は「配列の配列」です。各行の長さが異なる可変長のデータを表現できます。`int[][]` のように `[]` を二重に書くことで宣言します。

## 学習目標

- ジャグ配列を宣言・初期化・アクセスできる
- 多次元配列との違いを説明できる
- 用途に応じてどちらを選ぶか判断できる

## 前提知識

- [多次元配列](/unity-csharp-learning/csharp/multidimensional-arrays/) を読んでいること

---

## 1. ジャグ配列とは

多次元配列がすべての行で列数が同じ**矩形グリッド**なのに対し、ジャグ配列は**各行が独立した配列**であり、行ごとに長さを変えられます。

<svg viewBox="0 0 380 185" xmlns="http://www.w3.org/2000/svg" style="width:100%;max-width:500px;display:block;margin:1em 0;font-family:sans-serif;">
  <defs>
    <marker id="jg-arr" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#555"/></marker>
  </defs>
  <!-- Outer array (index array) -->
  <text x="45" y="16" text-anchor="middle" font-size="11" fill="#888">外側の配列</text>
  <rect x="10" y="22" width="70" height="36" rx="3" fill="#fff9c4" stroke="#f9a825" stroke-width="1.5"/>
  <text x="45" y="38" text-anchor="middle" font-size="12" fill="#555">jagged[0]</text>
  <text x="45" y="52" text-anchor="middle" font-size="10" fill="#e65100">参照</text>
  <rect x="10" y="68" width="70" height="36" rx="3" fill="#fff9c4" stroke="#f9a825" stroke-width="1.5"/>
  <text x="45" y="84" text-anchor="middle" font-size="12" fill="#555">jagged[1]</text>
  <text x="45" y="98" text-anchor="middle" font-size="10" fill="#e65100">参照</text>
  <rect x="10" y="114" width="70" height="36" rx="3" fill="#fff9c4" stroke="#f9a825" stroke-width="1.5"/>
  <text x="45" y="130" text-anchor="middle" font-size="12" fill="#555">jagged[2]</text>
  <text x="45" y="144" text-anchor="middle" font-size="10" fill="#e65100">参照</text>
  <!-- Arrows -->
  <line x1="80" y1="40" x2="118" y2="40" stroke="#555" stroke-width="1.5" marker-end="url(#jg-arr)"/>
  <line x1="80" y1="86" x2="118" y2="86" stroke="#555" stroke-width="1.5" marker-end="url(#jg-arr)"/>
  <line x1="80" y1="132" x2="118" y2="132" stroke="#555" stroke-width="1.5" marker-end="url(#jg-arr)"/>
  <!-- Row 0: 3 elements -->
  <text x="148" y="16" text-anchor="middle" font-size="10" fill="#999">3 要素</text>
  <rect x="120" y="22" width="56" height="36" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="148" y="45" text-anchor="middle" font-size="15" fill="#1565c0">10</text>
  <rect x="176" y="22" width="56" height="36" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="204" y="45" text-anchor="middle" font-size="15" fill="#1565c0">20</text>
  <rect x="232" y="22" width="56" height="36" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="260" y="45" text-anchor="middle" font-size="15" fill="#1565c0">30</text>
  <!-- Row 1: 5 elements -->
  <text x="176" y="72" text-anchor="middle" font-size="10" fill="#999">5 要素</text>
  <rect x="120" y="78" width="44" height="36" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="142" y="101" text-anchor="middle" font-size="15" fill="#1565c0">1</text>
  <rect x="164" y="78" width="44" height="36" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="186" y="101" text-anchor="middle" font-size="15" fill="#1565c0">2</text>
  <rect x="208" y="78" width="44" height="36" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="230" y="101" text-anchor="middle" font-size="15" fill="#1565c0">3</text>
  <rect x="252" y="78" width="44" height="36" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="274" y="101" text-anchor="middle" font-size="15" fill="#1565c0">4</text>
  <rect x="296" y="78" width="44" height="36" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="318" y="101" text-anchor="middle" font-size="15" fill="#1565c0">5</text>
  <!-- Row 2: 2 elements -->
  <text x="148" y="118" text-anchor="middle" font-size="10" fill="#999">2 要素</text>
  <rect x="120" y="124" width="56" height="36" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="148" y="147" text-anchor="middle" font-size="15" fill="#1565c0">7</text>
  <rect x="176" y="124" width="56" height="36" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="204" y="147" text-anchor="middle" font-size="15" fill="#1565c0">8</text>
</svg>

外側の配列（黄色）が各行への参照を持ち、各行（青）は独立した 1 次元配列です。

---

## 2. ジャグ配列の宣言と初期化

**書式：ジャグ配列の宣言**
```
型[][] 変数名 = new 型[行数][];
```

| 要素 | 説明 |
|---|---|
| `型[][]` | ジャグ配列の型。`[][]` が「配列の配列」を示す |
| `new 型[行数][]` | 外側の配列（行への参照を格納）を生成する |

```csharp
int[][] jagged = new int[3][];   // 3 行の外側配列を確保（各行はまだ null）

// 各行を個別に初期化
jagged[0] = new int[] { 10, 20, 30 };      // 行 0: 3 要素
jagged[1] = new int[] { 1, 2, 3, 4, 5 };  // 行 1: 5 要素
jagged[2] = new int[] { 7, 8 };            // 行 2: 2 要素
```

初期化子をまとめて書くこともできます。

```csharp
int[][] jagged = {
    new int[] { 10, 20, 30 },
    new int[] { 1, 2, 3, 4, 5 },
    new int[] { 7, 8 }
};
```

---

## 3. 要素へのアクセス

**書式：ジャグ配列の要素アクセス**
```
配列[行インデックス][列インデックス]
```

```csharp
Console.WriteLine(jagged[0][1]);  // 20（行 0 の 2 番目）
Console.WriteLine(jagged[1][4]);  // 5（行 1 の末尾）

jagged[2][0] = 99;                // 値の書き換え
```

各行は独立した 1 次元配列なので `jagged[i].Length` でその行の要素数を取得できます。

```csharp
for (int i = 0; i < jagged.Length; i++)
{
    Console.Write($"行{i}({jagged[i].Length}要素): ");
    for (int j = 0; j < jagged[i].Length; j++)
    {
        Console.Write(jagged[i][j] + " ");
    }
    Console.WriteLine();
}
// 行0(3要素): 10 20 30
// 行1(5要素): 1 2 3 4 5
// 行2(2要素): 7 8
```

---

## 4. 多次元配列との比較

| 比較項目 | 多次元配列 `int[,]` | ジャグ配列 `int[][]` |
|---|---|---|
| 形状 | すべての行が同じ列数（矩形） | 行ごとに列数が異なる |
| 宣言 | `new int[3, 4]` | `new int[3][]` + 各行の初期化 |
| アクセス | `a[i, j]` | `a[i][j]` |
| `Length` | 全要素数（行 × 列） | 行数のみ |
| メモリ | 一続きの連続領域 | 行ごとに別々の領域 |
| 向いている用途 | 行列演算・グリッドマップ | 三角形データ・可変長行のデータ |

### 使い分けの目安

- **すべての行が同じ長さ** → 多次元配列 `int[,]`
- **行によって長さが違う**（例: 隣接リスト・三角行列） → ジャグ配列 `int[][]`

---

## よくあるミス

```csharp
int[][] jagged = new int[3][];

// ❌ NG: 各行を初期化する前にアクセスすると NullReferenceException
Console.WriteLine(jagged[0][0]);  // NullReferenceException!

// ✅ OK: 各行を先に初期化してからアクセス
jagged[0] = new int[4];
Console.WriteLine(jagged[0][0]);  // 0
```

外側の配列を生成しただけでは各行は `null` です。必ず行ごとに初期化が必要です。

---

## まとめ

- ジャグ配列（`型[][]`）は「配列の配列」で、行ごとに長さが異なるデータを表現できる
- 外側の配列を `new 型[行数][]` で生成し、各行を別途 `new 型[]{ ... }` で初期化する
- アクセスは `a[行][列]` の 2 段階
- 形が一定なら多次元配列、行ごとに長さが違うならジャグ配列を選ぶ

---

## 理解度チェック

1. ジャグ配列 `int[][] data = new int[4][]` を生成した直後、`data[0]` の値は何ですか？
2. `jagged[1].Length` は何を返しますか？
3. 3 行のジャグ配列で、行 0 に `{1}`, 行 1 に `{1,2}`, 行 2 に `{1,2,3}` を格納し、すべての要素を出力するコードを書いてください。

<details markdown="1">
<summary>解答を見る</summary>

1. `null` です。外側の配列が生成されただけで、各行はまだ初期化されていません。

2. `jagged[1]` の要素数（列数）を返します。行によって異なります。

3. ```csharp
   int[][] jagged = {
       new int[] { 1 },
       new int[] { 1, 2 },
       new int[] { 1, 2, 3 }
   };
   for (int i = 0; i < jagged.Length; i++)
       for (int j = 0; j < jagged[i].Length; j++)
           Console.WriteLine($"[{i}][{j}] = {jagged[i][j]}");
   ```

</details>

---

## 次のステップ

配列セクションはここで完結です。次は **C# クラスとオブジェクト** セクションでクラス・メソッド・プロパティを学びます。
