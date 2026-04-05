---
layout: page
title: 多次元配列
permalink: /csharp/multidimensional-arrays/
---

# 多次元配列

**多次元配列**は行と列のような格子状（グリッド）にデータを配置する配列です。C# では `int[,]` のようにカンマで区切った型表記を使います。

## 学習目標

- 2 次元配列を宣言・初期化・アクセスできる
- `GetLength(0)` と `GetLength(1)` を使ってネストループで全要素を走査できる
- `Rank` プロパティで次元数を確認できる

## 前提知識

- [Array クラスと配列の性質（補足）](/unity-csharp-learning/csharp/array-class/) を読んでいること

---

## 1. 2 次元配列とは

2 次元配列は**行（row）と列（column）**の組み合わせでデータを管理します。座標系のグリッドやゲームのマップデータなどに使われます。

<svg viewBox="0 0 380 210" xmlns="http://www.w3.org/2000/svg" style="width:100%;max-width:500px;display:block;margin:1em 0;font-family:sans-serif;">
  <!-- Column labels -->
  <text x="118" y="22" text-anchor="middle" font-size="11" fill="#78909c">[,0]</text>
  <text x="178" y="22" text-anchor="middle" font-size="11" fill="#78909c">[,1]</text>
  <text x="238" y="22" text-anchor="middle" font-size="11" fill="#78909c">[,2]</text>
  <text x="298" y="22" text-anchor="middle" font-size="11" fill="#78909c">[,3]</text>
  <!-- Row 0 -->
  <text x="52" y="52" text-anchor="middle" font-size="11" fill="#78909c">[0,]</text>
  <rect x="88"  y="30" width="60" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="118" y="55" text-anchor="middle" font-size="16" fill="#1565c0">1</text>
  <rect x="148" y="30" width="60" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="178" y="55" text-anchor="middle" font-size="16" fill="#1565c0">2</text>
  <rect x="208" y="30" width="60" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="238" y="55" text-anchor="middle" font-size="16" fill="#1565c0">3</text>
  <rect x="268" y="30" width="60" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="298" y="55" text-anchor="middle" font-size="16" fill="#1565c0">4</text>
  <!-- Row 1 -->
  <text x="52" y="112" text-anchor="middle" font-size="11" fill="#78909c">[1,]</text>
  <rect x="88"  y="90" width="60" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="118" y="115" text-anchor="middle" font-size="16" fill="#1565c0">5</text>
  <rect x="148" y="90" width="60" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="178" y="115" text-anchor="middle" font-size="16" fill="#1565c0">6</text>
  <rect x="208" y="90" width="60" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="238" y="115" text-anchor="middle" font-size="16" fill="#1565c0">7</text>
  <rect x="268" y="90" width="60" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="298" y="115" text-anchor="middle" font-size="16" fill="#1565c0">8</text>
  <!-- Row 2 -->
  <text x="52" y="172" text-anchor="middle" font-size="11" fill="#78909c">[2,]</text>
  <rect x="88"  y="150" width="60" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="118" y="175" text-anchor="middle" font-size="16" fill="#1565c0">9</text>
  <rect x="148" y="150" width="60" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="178" y="175" text-anchor="middle" font-size="16" fill="#1565c0">10</text>
  <rect x="208" y="150" width="60" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="238" y="175" text-anchor="middle" font-size="16" fill="#1565c0">11</text>
  <rect x="268" y="150" width="60" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="298" y="175" text-anchor="middle" font-size="16" fill="#1565c0">12</text>
  <!-- access example annotation -->
  <text x="118" y="200" text-anchor="middle" font-size="10" fill="#999">matrix[0,0]</text>
  <text x="298" y="200" text-anchor="middle" font-size="10" fill="#999">matrix[2,3]</text>
</svg>

3 行 4 列の `int[,] matrix`。`matrix[行, 列]` の形でアクセスします。

---

## 2. 2 次元配列の宣言と初期化

**書式：2 次元配列の宣言**
```
型[,] 変数名 = new 型[行数, 列数];
```

| 要素 | 説明 |
|---|---|
| `型[,]` | 2 次元配列の型。カンマの数が次元数を示す |
| `new 型[行数, 列数]` | 指定したサイズの配列を生成（すべて初期値） |

```csharp
int[,] matrix = new int[3, 4];  // 3 行 4 列（すべて 0）
```

### 初期化子で宣言

```csharp
int[,] matrix = {
    { 1,  2,  3,  4 },   // 行 0
    { 5,  6,  7,  8 },   // 行 1
    { 9, 10, 11, 12 }    // 行 2
};
```

---

## 3. 要素へのアクセス

**書式：2 次元配列の要素アクセス**
```
配列[行インデックス, 列インデックス]
```

```csharp
int[,] matrix = {
    { 1,  2,  3,  4 },
    { 5,  6,  7,  8 },
    { 9, 10, 11, 12 }
};

Console.WriteLine(matrix[0, 0]);  // 1（左上）
Console.WriteLine(matrix[1, 2]);  // 7
Console.WriteLine(matrix[2, 3]);  // 12（右下）

matrix[0, 0] = 99;               // 値の書き換え
```

---

## 4. 2 次元配列の走査

`GetLength(0)` で行数、`GetLength(1)` で列数を取得してネスト for ループで全要素を走査します。

```csharp
int[,] matrix = {
    { 1,  2,  3,  4 },
    { 5,  6,  7,  8 },
    { 9, 10, 11, 12 }
};

int rows = matrix.GetLength(0);  // 3
int cols = matrix.GetLength(1);  // 4

for (int i = 0; i < rows; i++)
{
    for (int j = 0; j < cols; j++)
    {
        Console.Write($"{matrix[i, j],3}");
    }
    Console.WriteLine();
}
//   1  2  3  4
//   5  6  7  8
//   9 10 11 12
```

### Rank プロパティ

```csharp
Console.WriteLine(matrix.Rank);   // 2（2 次元配列）
Console.WriteLine(matrix.Length); // 12（全要素数 = 3 × 4）
```

---

## ワンポイントアドバイス

### 3 次元以上の配列

カンマを増やすことで 3 次元以上の配列も宣言できます。

```csharp
// 3 次元配列（奥行き × 行 × 列のようなイメージ）
int[,,] cube = new int[2, 3, 4];
Console.WriteLine(cube.Rank);   // 3
Console.WriteLine(cube.Length); // 24（2 × 3 × 4）
```

実用上は 2 次元を超える多次元配列はあまり使われません。代わりにジャグ配列（次のページ）やクラスで表現するのが一般的です。

---

## まとめ

- `型[,]` で 2 次元配列を宣言し、`new 型[行数, 列数]` または初期化子で初期化する
- `配列[行, 列]` でアクセスする
- `GetLength(0)` で行数、`GetLength(1)` で列数を取得する
- `Rank` で次元数、`Length` で全要素数を確認できる

---

## 理解度チェック

1. `int[,] grid = new int[5, 3]` の行数・列数・全要素数はそれぞれいくつですか？
2. 2 次元配列のすべての要素の合計を計算するコードを書いてください。
3. `matrix[1, 2]` と `matrix[2, 1]` は同じ要素を指しますか？

<details markdown="1">
<summary>解答を見る</summary>

1. 行数 `5`、列数 `3`、全要素数 `15`（`GetLength(0)=5`, `GetLength(1)=3`, `Length=15`）

2. ```csharp
   int[,] matrix = { { 1, 2 }, { 3, 4 }, { 5, 6 } };
   int sum = 0;
   for (int i = 0; i < matrix.GetLength(0); i++)
       for (int j = 0; j < matrix.GetLength(1); j++)
           sum += matrix[i, j];
   Console.WriteLine(sum);  // 21
   ```

3. 異なる要素です。`matrix[1, 2]` は 行1・列2、`matrix[2, 1]` は 行2・列1 を指します。

</details>

---

## 次のステップ

[ジャグ配列](/unity-csharp-learning/csharp/jagged-arrays/) では、行ごとに長さが異なる配列を学び、多次元配列との使い分けを確認します。
