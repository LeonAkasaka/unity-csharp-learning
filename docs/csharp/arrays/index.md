---
layout: page
title: 配列の基礎
permalink: /csharp/arrays/
---

# 配列の基礎

**配列**（array）は、同じ型の値を連続して格納するデータ構造です。複数の値を 1 つの変数でまとめて管理できます。

## 学習目標

- 配列を宣言・初期化できる
- インデックスで要素を読み書きできる
- `Length` を使って要素数を取得できる
- `for` と `foreach` で配列を走査できる
- 範囲外アクセス（`IndexOutOfRangeException`）を避けられる

## 前提知識

- [反復処理](/unity-csharp-learning/csharp/loops/) を読んでいること

---

## 1. 配列とは

配列はメモリ上に**連続して並んだ格納場所**の集まりです。各格納場所には `0` から始まる**インデックス**（番号）でアクセスします。

<svg viewBox="0 0 390 95" xmlns="http://www.w3.org/2000/svg" style="width:100%;max-width:520px;display:block;margin:1em 0;font-family:sans-serif;">
  <defs>
    <marker id="a9-arr" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#888"/></marker>
  </defs>
  <text x="4" y="53" font-size="13" fill="#555" font-style="italic">scores</text>
  <line x1="52" y1="48" x2="68" y2="48" stroke="#888" stroke-width="1.5" marker-end="url(#a9-arr)"/>
  <text x="97"  y="22" text-anchor="middle" font-size="11" fill="#78909c">[0]</text>
  <text x="147" y="22" text-anchor="middle" font-size="11" fill="#78909c">[1]</text>
  <text x="197" y="22" text-anchor="middle" font-size="11" fill="#78909c">[2]</text>
  <text x="247" y="22" text-anchor="middle" font-size="11" fill="#78909c">[3]</text>
  <text x="297" y="22" text-anchor="middle" font-size="11" fill="#78909c">[4]</text>
  <rect x="72"  y="28" width="50" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="97"  y="53" text-anchor="middle" font-size="18" fill="#1565c0">85</text>
  <rect x="122" y="28" width="50" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="147" y="53" text-anchor="middle" font-size="18" fill="#1565c0">72</text>
  <rect x="172" y="28" width="50" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="197" y="53" text-anchor="middle" font-size="18" fill="#1565c0">90</text>
  <rect x="222" y="28" width="50" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="247" y="53" text-anchor="middle" font-size="18" fill="#1565c0">68</text>
  <rect x="272" y="28" width="50" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="297" y="53" text-anchor="middle" font-size="18" fill="#1565c0">95</text>
  <text x="97"  y="84" text-anchor="middle" font-size="10" fill="#999">scores[0]</text>
  <text x="297" y="84" text-anchor="middle" font-size="10" fill="#999">scores[4]</text>
</svg>

5 つの整数を格納する `int[] scores` を例に説明します。先頭は `scores[0]`、末尾は `scores[4]` でアクセスします。

---

## 2. 配列の宣言と初期化

### 要素数を指定して宣言

**書式：配列の宣言（要素数指定）**
```
型[] 変数名 = new 型[要素数];
```

| 要素 | 説明 |
|---|---|
| `型[]` | 配列の型。`[]` が「配列であること」を示す |
| `new 型[要素数]` | 指定した要素数の配列を新しく生成する |

```csharp
int[] scores = new int[5];  // 5 要素の int 配列（すべて 0 で初期化）
```

数値型は `0`、`bool` は `false`、参照型は `null` で自動初期化されます。

### 初期化子で宣言

**書式：配列の宣言（初期化子）**
```
型[] 変数名 = { 値1, 値2, ... };
```

```csharp
int[] scores = { 85, 72, 90, 68, 95 };

// new を明示する書き方も同じ意味
int[] scores2 = new int[] { 85, 72, 90, 68, 95 };

// var を使う場合は new が必要
var scores3 = new int[] { 85, 72, 90, 68, 95 };  // var は int[] と推論される
var scores4 = new[] { 85, 72, 90, 68, 95 };       // 要素の型から自動推論（型名を省略）

// ❌ NG: var と { } の組み合わせ — 左も右も型を決められず推論不可
var scores5 = { 85, 72, 90, 68, 95 };             // コンパイルエラー
```

| 左辺 | 右辺 | 結果 |
|---|---|---|
| `int[]` | `{ ... }` | ✅ 左辺が型を決める |
| `var` | `new int[] { ... }` | ✅ 右辺が型を決める |
| `var` | `new[] { ... }` | ✅ 要素の型から右辺が決める |
| `var` | `{ ... }` | ❌ 左も右も型を持たず推論不可 |

宣言と同時に値を指定する場合は `new 型[]` を省略できます。`var` を使う場合は `new` が必要です（`var scores = { ... }` とは書けません）。

---

## 3. 要素へのアクセス

### インデックスアクセス

**書式：要素アクセス**
```
配列[インデックス]
```

| 要素 | 説明 |
|---|---|
| `インデックス` | `0` 以上 `Length - 1` 以下の整数 |

```csharp
int[] scores = { 85, 72, 90, 68, 95 };

Console.WriteLine(scores[0]);  // 85（先頭）
Console.WriteLine(scores[4]);  // 95（末尾）

scores[2] = 100;               // 値の書き換え
Console.WriteLine(scores[2]);  // 100
```

### Length プロパティ

**`Length`** — 配列の要素数を返します。<!-- [公式ドキュメント]() -->

**書式：Length プロパティ**
```csharp
int Length { get; }
```

```csharp
int[] scores = { 85, 72, 90, 68, 95 };
Console.WriteLine(scores.Length);  // 5
```

### 末尾からのインデックス（C# 8 以降）

**書式：末尾インデックス**
```
配列[^n]
```

`^1` は末尾の要素、`^2` は末尾から 2 番目を指します。

<svg viewBox="0 0 330 105" xmlns="http://www.w3.org/2000/svg" style="width:100%;max-width:440px;display:block;margin:1em 0;font-family:sans-serif;">
  <text x="65"  y="22" text-anchor="middle" font-size="11" fill="#78909c">[0]</text>
  <text x="115" y="22" text-anchor="middle" font-size="11" fill="#78909c">[1]</text>
  <text x="165" y="22" text-anchor="middle" font-size="11" fill="#78909c">[2]</text>
  <text x="215" y="22" text-anchor="middle" font-size="11" fill="#78909c">[3]</text>
  <text x="265" y="22" text-anchor="middle" font-size="11" fill="#78909c">[4]</text>
  <rect x="40"  y="28" width="50" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="65"  y="53" text-anchor="middle" font-size="18" fill="#555">85</text>
  <rect x="90"  y="28" width="50" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="115" y="53" text-anchor="middle" font-size="18" fill="#555">72</text>
  <rect x="140" y="28" width="50" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="165" y="53" text-anchor="middle" font-size="18" fill="#555">90</text>
  <rect x="190" y="28" width="50" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="215" y="53" text-anchor="middle" font-size="18" fill="#555">68</text>
  <rect x="240" y="28" width="50" height="40" rx="3" fill="#f5f5f5" stroke="#bbb" stroke-width="1.5"/><text x="265" y="53" text-anchor="middle" font-size="18" fill="#555">95</text>
  <text x="65"  y="87" text-anchor="middle" font-size="11" fill="#e65100">[^5]</text>
  <text x="115" y="87" text-anchor="middle" font-size="11" fill="#e65100">[^4]</text>
  <text x="165" y="87" text-anchor="middle" font-size="11" fill="#e65100">[^3]</text>
  <text x="215" y="87" text-anchor="middle" font-size="11" fill="#e65100">[^2]</text>
  <text x="265" y="87" text-anchor="middle" font-size="11" fill="#e65100">[^1]</text>
</svg>

```csharp
int[] scores = { 85, 72, 90, 68, 95 };

Console.WriteLine(scores[^1]);  // 95（末尾）
Console.WriteLine(scores[^2]);  // 68（末尾から 2 番目）
```

> 💡 `scores[^1]` は `scores[scores.Length - 1]` と同じ意味です。

---

## 4. 配列の走査

### for ループ

```csharp
int[] scores = { 85, 72, 90, 68, 95 };

for (int i = 0; i < scores.Length; i++)
{
    Console.WriteLine($"scores[{i}] = {scores[i]}");
}
// scores[0] = 85
// scores[1] = 72
// scores[2] = 90
// scores[3] = 68
// scores[4] = 95
```

インデックスが必要な場合（要素の書き換えや位置の利用）は `for` を使います。

### foreach ループ

```csharp
int[] scores = { 85, 72, 90, 68, 95 };

foreach (int score in scores)
{
    Console.WriteLine(score);
}
// 85
// 72
// 90
// 68
// 95
```

インデックスが不要で要素を順に読むだけなら `foreach` が簡潔です。`var` 型推論・読み取り専用の制約・`break`/`continue` との組み合わせ・`for` との使い分けは[配列と foreach（補足）](/unity-csharp-learning/csharp/arrays-and-foreach/)で詳しく解説します。

---

## よくあるミス

```csharp
int[] scores = { 85, 72, 90, 68, 95 };

// ❌ NG: インデックスが Length と等しい（範囲外）
Console.WriteLine(scores[5]);  // System.IndexOutOfRangeException

// ✅ OK: 最後の要素は Length - 1 または ^1
Console.WriteLine(scores[scores.Length - 1]);  // 95
Console.WriteLine(scores[^1]);                  // 95
```

配列のインデックスは `0` 〜 `Length - 1` の範囲です。`Length` そのものはインデックスとして使えません。

---

## ワンポイントアドバイス

### C# 12 のコレクション式

C# 12 から `[...]` を使ったコレクション式で配列を初期化できます。

```csharp
// C# 12 以降
int[] scores = [85, 72, 90, 68, 95];
```

`{ }` 構文と同じ意味ですが、`List<T>` などでも同じ記法が使える統一構文です。

---

## まとめ

- 配列は同じ型の値を連続して格納するデータ構造
- `型[] 変数名 = new 型[要素数]` または `{ ... }` で宣言・初期化する
- インデックスは `0` 始まり。末尾は `[^1]`（C# 8 以降）
- `Length` で要素数を取得する
- 要素の読み取りだけなら `foreach`、書き換えや位置が必要なら `for`
- 範囲外アクセスは `IndexOutOfRangeException` を引き起こす

---

## 理解度チェック

1. 要素数 3 の `string` 配列を `"red"`, `"green"`, `"blue"` で初期化するコードを書いてください。
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   int[] nums = new int[3];
   Console.WriteLine(nums[1]);
   ```

3. `for` ループを使って配列のすべての要素の合計を計算するコードを書いてください。

<details markdown="1">
<summary>解答を見る</summary>

1. ```csharp
   string[] colors = { "red", "green", "blue" };
   ```

2. `0` が出力されます。数値型配列の要素は `0` で自動初期化されるためです。

3. ```csharp
   int[] nums = { 1, 2, 3, 4, 5 };
   int sum = 0;
   for (int i = 0; i < nums.Length; i++)
   {
       sum += nums[i];
   }
   Console.WriteLine(sum);  // 15
   ```

</details>

---

## 次のステップ

[配列と foreach（補足）](/unity-csharp-learning/csharp/arrays-and-foreach/) では、foreach の書式詳細・`var` 推論・`break`/`continue` との組み合わせ・`for` との使い分けを学びます。
