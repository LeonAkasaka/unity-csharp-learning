---
layout: page
title: Array クラスと配列の性質（補足）
permalink: /csharp/array-class/
---

# Array クラスと配列の性質（補足）

配列は単純なデータの並びに見えますが、C# では `System.Array` クラスを継承するオブジェクトです。このページでは配列変数のコピーの挙動と、配列を操作する組み込みメソッドを学びます。

## 学習目標

- 変数の代入（`b = a`）では同じ配列を指すことになると説明できる
- `Array.Copy` で独立したコピーを作れる
- `Length`・`Rank`・`GetLength` で配列の情報を取得できる
- `Array.Sort`・`Array.Reverse`・`Array.IndexOf`・`Array.Copy`・`Array.Clear` を使える

## 前提知識

- [配列の基礎](/unity-csharp-learning/csharp/arrays/) を読んでいること

---

## 1. System.Array — 配列の正体

C# のすべての配列は `System.Array` クラスを継承しています。`int[]` を宣言するとき、実は `System.Array` のサブクラスのインスタンスが生成されています。

```csharp
int[] scores = { 85, 72, 90, 68, 95 };
Console.WriteLine(scores.GetType());  // System.Int32[]
Console.WriteLine(scores is Array);   // True
```

---

## 2. 変数には配列への参照が入る

`int[] scores` のような変数に格納されるのは配列本体ではなく、**配列がどこにあるかを示す参照**です。

<svg viewBox="0 0 360 75" xmlns="http://www.w3.org/2000/svg" style="width:100%;max-width:460px;display:block;margin:1em 0;font-family:sans-serif;">
  <defs>
    <marker id="ac-a1-arr" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#555"/></marker>
  </defs>
  <rect x="8" y="18" width="75" height="32" rx="4" fill="#fff9c4" stroke="#f9a825" stroke-width="1.5"/>
  <text x="46" y="38" text-anchor="middle" font-size="13" fill="#555" font-family="monospace">scores</text>
  <line x1="83" y1="34" x2="112" y2="34" stroke="#555" stroke-width="1.5" marker-end="url(#ac-a1-arr)"/>
  <rect x="114" y="14" width="44" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="136" y="39" text-anchor="middle" font-size="16" fill="#1565c0">85</text>
  <rect x="158" y="14" width="44" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="180" y="39" text-anchor="middle" font-size="16" fill="#1565c0">72</text>
  <rect x="202" y="14" width="44" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="224" y="39" text-anchor="middle" font-size="16" fill="#1565c0">90</text>
  <rect x="246" y="14" width="44" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="268" y="39" text-anchor="middle" font-size="16" fill="#1565c0">68</text>
  <rect x="290" y="14" width="44" height="40" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="312" y="39" text-anchor="middle" font-size="16" fill="#1565c0">95</text>
  <text x="136" y="64" text-anchor="middle" font-size="10" fill="#999">[0]</text>
  <text x="180" y="64" text-anchor="middle" font-size="10" fill="#999">[1]</text>
  <text x="224" y="64" text-anchor="middle" font-size="10" fill="#999">[2]</text>
  <text x="268" y="64" text-anchor="middle" font-size="10" fill="#999">[3]</text>
  <text x="312" y="64" text-anchor="middle" font-size="10" fill="#999">[4]</text>
</svg>

`new` を書くたびに新しい配列が生成されます。変数にはその配列への参照（どこにあるかという情報）が入ります。

```csharp
int[] a = { 1, 2, 3 };           // new int[] { 1, 2, 3 } と同じ
int[] b = new int[] { 1, 2, 3 }; // new → 別の配列が作られる
// a と b は独立した別の配列
```

### 代入は参照のコピー（同じ配列を指す）

`b = a` と書くと**参照がコピー**されます。`new` が書かれていないので新しい配列は作られず、`a` と `b` は**同じ配列を指す**ことになります。

<svg viewBox="0 0 280 88" xmlns="http://www.w3.org/2000/svg" style="width:100%;max-width:360px;display:block;margin:1em 0;font-family:sans-serif;">
  <defs>
    <marker id="ac-a2-arr" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#555"/></marker>
  </defs>
  <rect x="8" y="10" width="58" height="28" rx="3" fill="#fff9c4" stroke="#f9a825" stroke-width="1.5"/>
  <text x="37" y="28" text-anchor="middle" font-size="14" fill="#555" font-family="monospace">a</text>
  <rect x="8" y="50" width="58" height="28" rx="3" fill="#fff9c4" stroke="#f9a825" stroke-width="1.5"/>
  <text x="37" y="68" text-anchor="middle" font-size="14" fill="#555" font-family="monospace">b</text>
  <rect x="115" y="29" width="40" height="32" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="135" y="50" text-anchor="middle" font-size="15" fill="#1565c0">1</text>
  <rect x="155" y="29" width="40" height="32" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="175" y="50" text-anchor="middle" font-size="15" fill="#1565c0">2</text>
  <rect x="195" y="29" width="40" height="32" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="215" y="50" text-anchor="middle" font-size="15" fill="#1565c0">3</text>
  <line x1="66" y1="24" x2="113" y2="43" stroke="#555" stroke-width="1.5" marker-end="url(#ac-a2-arr)"/>
  <line x1="66" y1="64" x2="113" y2="46" stroke="#555" stroke-width="1.5" marker-end="url(#ac-a2-arr)"/>
  <text x="165" y="82" text-anchor="middle" font-size="11" fill="#c62828">同じ配列を指している</text>
</svg>

```csharp
int[] a = { 1, 2, 3 };
int[] b = a;          // 参照のコピー（new がない → 同じ配列）

b[0] = 99;
Console.WriteLine(a[0]);  // 99（a も変わっている！）
```

`Array.Copy` を使うと新しい配列に要素をコピーして独立させることができます。

---

## 3. 配列のプロパティ

### Length / Rank / GetLength

**`Length`** — 全要素数（すべての次元を含む）。<!-- [公式ドキュメント]() -->

**`Rank`** — 配列の次元数。1 次元配列は `1`、2 次元は `2`。<!-- [公式ドキュメント]() -->

**`GetLength(n)`** — 指定した次元の要素数を返します。<!-- [公式ドキュメント]() -->

**書式：GetLength メソッド**
```csharp
int GetLength(int dimension);
```

| パラメータ | 型 | 説明 |
|---|---|---|
| `dimension` | `int` | 取得する次元のインデックス（0 始まり） |

```csharp
int[] arr = { 10, 20, 30, 40 };
Console.WriteLine(arr.Length);       // 4
Console.WriteLine(arr.Rank);         // 1
Console.WriteLine(arr.GetLength(0)); // 4（0 次元目の要素数）
```

---

## 4. Array クラスの静的メソッド

### Array.Sort — 昇順に並び替え

**`Array.Sort`** — 配列を昇順にソートします（元の配列を直接変更します）。<!-- [公式ドキュメント]() -->

**書式：Array.Sort メソッド**
```csharp
static void Array.Sort(Array array);
```

```csharp
int[] nums = { 40, 10, 30, 20 };
Array.Sort(nums);

foreach (int n in nums)
    Console.Write(n + " ");  // 10 20 30 40
```

### Array.Reverse — 逆順に並び替え

**`Array.Reverse`** — 配列の要素順を逆にします（元の配列を直接変更します）。<!-- [公式ドキュメント]() -->

**書式：Array.Reverse メソッド**
```csharp
static void Array.Reverse(Array array);
```

```csharp
int[] nums = { 10, 20, 30, 40 };
Array.Reverse(nums);

foreach (int n in nums)
    Console.Write(n + " ");  // 40 30 20 10
```

### Array.IndexOf — 要素を検索

**`Array.IndexOf`** — 指定した値が最初に見つかったインデックスを返します。見つからない場合は `-1`。<!-- [公式ドキュメント]() -->

**書式：Array.IndexOf メソッド**
```csharp
static int Array.IndexOf(Array array, object? value);
```

| パラメータ | 型 | 説明 |
|---|---|---|
| `array` | `Array` | 検索する配列 |
| `value` | `object?` | 検索する値 |

```csharp
string[] fruits = { "apple", "banana", "cherry" };
Console.WriteLine(Array.IndexOf(fruits, "banana"));  // 1
Console.WriteLine(Array.IndexOf(fruits, "grape"));   // -1（見つからない）
```

### Array.Copy — 独立したコピーを作る

**`Array.Copy`** — 配列の要素を別の配列にコピーします。`b = a` の代入とは異なり、`Array.Copy` はコピー先として **`new` で新たに作った配列**を使うため、2 つの配列が独立した状態になります。<!-- [公式ドキュメント]() -->

<svg viewBox="0 0 300 198" xmlns="http://www.w3.org/2000/svg" style="width:100%;max-width:380px;display:block;margin:1em 0;font-family:sans-serif;">
  <defs>
    <marker id="ac-cmp-arr" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#555"/></marker>
  </defs>
  <!-- Top: b = a (same array) -->
  <text x="8" y="13" font-size="11" fill="#888">変数の代入（b = a）</text>
  <rect x="8" y="20" width="55" height="26" rx="3" fill="#fff9c4" stroke="#f9a825" stroke-width="1.5"/>
  <text x="36" y="37" text-anchor="middle" font-size="13" fill="#555" font-family="monospace">a</text>
  <rect x="8" y="58" width="55" height="26" rx="3" fill="#fff9c4" stroke="#f9a825" stroke-width="1.5"/>
  <text x="36" y="75" text-anchor="middle" font-size="13" fill="#555" font-family="monospace">b</text>
  <rect x="118" y="34" width="40" height="32" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="138" y="55" text-anchor="middle" font-size="15" fill="#1565c0">1</text>
  <rect x="158" y="34" width="40" height="32" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="178" y="55" text-anchor="middle" font-size="15" fill="#1565c0">2</text>
  <rect x="198" y="34" width="40" height="32" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="218" y="55" text-anchor="middle" font-size="15" fill="#1565c0">3</text>
  <line x1="63" y1="33" x2="116" y2="50" stroke="#555" stroke-width="1.5" marker-end="url(#ac-cmp-arr)"/>
  <line x1="63" y1="71" x2="116" y2="50" stroke="#555" stroke-width="1.5" marker-end="url(#ac-cmp-arr)"/>
  <text x="170" y="82" text-anchor="middle" font-size="10" fill="#c62828">同じ配列 / b[0]=99 で a[0] も変わる</text>
  <!-- Divider -->
  <line x1="0" y1="93" x2="300" y2="93" stroke="#e0e0e0" stroke-width="1"/>
  <!-- Bottom: Array.Copy (separate arrays) -->
  <text x="8" y="107" font-size="11" fill="#888">Array.Copy（独立したコピー）</text>
  <rect x="8" y="114" width="82" height="26" rx="3" fill="#fff9c4" stroke="#f9a825" stroke-width="1.5"/>
  <text x="49" y="131" text-anchor="middle" font-size="11" fill="#555" font-family="monospace">original</text>
  <rect x="8" y="154" width="82" height="26" rx="3" fill="#fff9c4" stroke="#f9a825" stroke-width="1.5"/>
  <text x="49" y="171" text-anchor="middle" font-size="11" fill="#555" font-family="monospace">copy</text>
  <rect x="118" y="114" width="40" height="32" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="138" y="135" text-anchor="middle" font-size="15" fill="#1565c0">1</text>
  <rect x="158" y="114" width="40" height="32" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="178" y="135" text-anchor="middle" font-size="15" fill="#1565c0">2</text>
  <rect x="198" y="114" width="40" height="32" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="218" y="135" text-anchor="middle" font-size="15" fill="#1565c0">3</text>
  <!-- copy array: [0] highlighted orange (copy[0] = 99) -->
  <rect x="118" y="154" width="40" height="32" rx="3" fill="#fff3e0" stroke="#f9a825" stroke-width="2"/><text x="138" y="175" text-anchor="middle" font-size="13" fill="#e65100">99</text>
  <rect x="158" y="154" width="40" height="32" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="178" y="175" text-anchor="middle" font-size="15" fill="#1565c0">2</text>
  <rect x="198" y="154" width="40" height="32" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="218" y="175" text-anchor="middle" font-size="15" fill="#1565c0">3</text>
  <line x1="90" y1="127" x2="116" y2="127" stroke="#555" stroke-width="1.5" marker-end="url(#ac-cmp-arr)"/>
  <line x1="90" y1="167" x2="116" y2="167" stroke="#555" stroke-width="1.5" marker-end="url(#ac-cmp-arr)"/>
  <text x="150" y="195" text-anchor="middle" font-size="10" fill="#555">↑ copy[0]=99 にしても original は変わらない</text>
</svg>

**書式：Array.Copy メソッド**
```csharp
static void Array.Copy(Array sourceArray, Array destinationArray, int length);
```

| パラメータ | 型 | 説明 |
|---|---|---|
| `sourceArray` | `Array` | コピー元の配列 |
| `destinationArray` | `Array` | コピー先の配列 |
| `length` | `int` | コピーする要素数 |

```csharp
int[] original = { 1, 2, 3 };
int[] copy = new int[3];
Array.Copy(original, copy, original.Length);

copy[0] = 99;
Console.WriteLine(original[0]);  // 1（元の配列は変わらない）
Console.WriteLine(copy[0]);      // 99
```

### Array.Clear — 要素を初期値にリセット

**`Array.Clear`** — 指定した範囲の要素を型の初期値（数値は `0`、参照型は `null`）にリセットします。<!-- [公式ドキュメント]() -->

**書式：Array.Clear メソッド**
```csharp
static void Array.Clear(Array array, int index, int length);
```

| パラメータ | 型 | 説明 |
|---|---|---|
| `array` | `Array` | 対象の配列 |
| `index` | `int` | リセット開始インデックス |
| `length` | `int` | リセットする要素数 |

```csharp
int[] nums = { 10, 20, 30, 40, 50 };
Array.Clear(nums, 1, 3);  // インデックス 1 〜 3 を 0 に

foreach (int n in nums)
    Console.Write(n + " ");  // 10 0 0 0 50
```

---

## よくあるミス

```csharp
int[] a = { 1, 2, 3 };

// ❌ NG: 代入は参照のコピー（中身はコピーされない）
int[] b = a;
b[0] = 99;  // a[0] も 99 になる

// ✅ OK: Array.Copy で独立したコピーを作る
int[] c = new int[a.Length];
Array.Copy(a, c, a.Length);
c[0] = 99;  // a[0] は変わらない
```

---

## まとめ

- すべての配列は `System.Array` を継承するオブジェクト
- 変数には配列への参照が入る。`b = a` では新しい配列は作られず同じ配列を指す
- `new` を書くたびに新しい配列が生成される
- 独立したコピーが必要なときは `Array.Copy` を使う
- `Length`・`Rank`・`GetLength(n)` で配列の構造を確認できる
- `Array.Sort`・`Array.Reverse` で並び替え、`Array.IndexOf` で検索できる

---

## 理解度チェック

1. `int[] a = {1,2,3}; int[] b = a; b[1] = 99;` を実行後、`a[1]` の値は何ですか？
2. `Array.Sort` と `Array.Reverse` を組み合わせて配列を降順に並べるコードを書いてください。
3. 次のコードで `nums` の最終的な内容は何ですか？

   ```csharp
   int[] nums = { 5, 10, 15, 20, 25 };
   Array.Clear(nums, 2, 2);
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. `99` です。`b = a` は参照のコピーのため（`new` が書かれていない）、`b` と `a` は同じ配列を指しています。

2. ```csharp
   int[] nums = { 3, 1, 4, 1, 5 };
   Array.Sort(nums);    // 昇順: 1 1 3 4 5
   Array.Reverse(nums); // 逆順: 5 4 3 1 1
   ```

3. `{ 5, 10, 0, 0, 25 }` です。インデックス 2 と 3 が `0` にリセットされます。

</details>

---

## 次のステップ

[多次元配列](/unity-csharp-learning/csharp/multidimensional-arrays/) では、行と列を持つ 2 次元配列を学びます。
