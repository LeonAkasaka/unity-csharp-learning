---
layout: page
title: Array クラスと配列の性質（補足）
permalink: /csharp/array-class/
---

# Array クラスと配列の性質（補足）

配列は単純なデータの並びに見えますが、C# では `System.Array` クラスを継承する**参照型**のオブジェクトです。このページではその仕組みと、配列を操作する組み込みメソッドを学びます。

## 学習目標

- 配列が参照型であることを理解し、コピーの挙動を説明できる
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

## 2. 配列は参照型

配列は**参照型**（reference type）です。変数に格納されるのは配列本体ではなく、ヒープ上の配列への**参照（アドレス）**です。

<svg viewBox="0 0 420 130" xmlns="http://www.w3.org/2000/svg" style="width:100%;max-width:560px;display:block;margin:1em 0;font-family:sans-serif;">
  <defs>
    <marker id="ac-arr" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#555"/></marker>
  </defs>
  <!-- Stack side -->
  <text x="55" y="18" text-anchor="middle" font-size="11" fill="#888">スタック</text>
  <rect x="10" y="25" width="90" height="36" rx="3" fill="#fff9c4" stroke="#f9a825" stroke-width="1.5"/>
  <text x="55" y="41" text-anchor="middle" font-size="12" fill="#555">scores</text>
  <text x="55" y="55" text-anchor="middle" font-size="10" fill="#e65100">参照 →</text>
  <!-- Arrow to heap -->
  <line x1="100" y1="43" x2="148" y2="43" stroke="#555" stroke-width="1.5" marker-end="url(#ac-arr)"/>
  <!-- Heap side: array cells -->
  <text x="255" y="18" text-anchor="middle" font-size="11" fill="#888">ヒープ</text>
  <rect x="150" y="25" width="42" height="36" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="171" y="48" text-anchor="middle" font-size="14" fill="#1565c0">85</text>
  <rect x="192" y="25" width="42" height="36" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="213" y="48" text-anchor="middle" font-size="14" fill="#1565c0">72</text>
  <rect x="234" y="25" width="42" height="36" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="255" y="48" text-anchor="middle" font-size="14" fill="#1565c0">90</text>
  <rect x="276" y="25" width="42" height="36" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="297" y="48" text-anchor="middle" font-size="14" fill="#1565c0">68</text>
  <rect x="318" y="25" width="42" height="36" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="339" y="48" text-anchor="middle" font-size="14" fill="#1565c0">95</text>
  <!-- index labels -->
  <text x="171" y="74" text-anchor="middle" font-size="10" fill="#999">[0]</text>
  <text x="213" y="74" text-anchor="middle" font-size="10" fill="#999">[1]</text>
  <text x="255" y="74" text-anchor="middle" font-size="10" fill="#999">[2]</text>
  <text x="297" y="74" text-anchor="middle" font-size="10" fill="#999">[3]</text>
  <text x="339" y="74" text-anchor="middle" font-size="10" fill="#999">[4]</text>
</svg>

### 代入ではコピーされない

`b = a` と書くと**参照がコピー**されます。配列の中身はコピーされず、2 つの変数が同じ配列を指します。

<svg viewBox="0 0 420 145" xmlns="http://www.w3.org/2000/svg" style="width:100%;max-width:560px;display:block;margin:1em 0;font-family:sans-serif;">
  <defs>
    <marker id="ac-arr2" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#555"/></marker>
    <marker id="ac-arr3" markerWidth="8" markerHeight="6" refX="7" refY="3" orient="auto"><polygon points="0 0,8 3,0 6" fill="#c62828"/></marker>
  </defs>
  <!-- Stack -->
  <text x="55" y="18" text-anchor="middle" font-size="11" fill="#888">スタック</text>
  <rect x="10" y="25" width="90" height="36" rx="3" fill="#fff9c4" stroke="#f9a825" stroke-width="1.5"/>
  <text x="55" y="43" text-anchor="middle" font-size="12" fill="#555">a</text>
  <rect x="10" y="72" width="90" height="36" rx="3" fill="#fce4ec" stroke="#e91e63" stroke-width="1.5"/>
  <text x="55" y="90" text-anchor="middle" font-size="12" fill="#c62828">b  ← b = a</text>
  <!-- Arrows (both pointing to same array) -->
  <line x1="100" y1="43" x2="148" y2="43" stroke="#555" stroke-width="1.5" marker-end="url(#ac-arr2)"/>
  <line x1="100" y1="90" x2="148" y2="60" stroke="#c62828" stroke-width="1.5" stroke-dasharray="5,3" marker-end="url(#ac-arr3)"/>
  <!-- Heap: same array -->
  <text x="255" y="18" text-anchor="middle" font-size="11" fill="#888">ヒープ（同じ配列）</text>
  <rect x="150" y="25" width="42" height="36" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="171" y="48" text-anchor="middle" font-size="14" fill="#1565c0">85</text>
  <rect x="192" y="25" width="42" height="36" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="213" y="48" text-anchor="middle" font-size="14" fill="#1565c0">72</text>
  <rect x="234" y="25" width="42" height="36" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="255" y="48" text-anchor="middle" font-size="14" fill="#1565c0">90</text>
  <rect x="276" y="25" width="42" height="36" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="297" y="48" text-anchor="middle" font-size="14" fill="#1565c0">68</text>
  <rect x="318" y="25" width="42" height="36" rx="3" fill="#e3f2fd" stroke="#90caf9" stroke-width="1.5"/><text x="339" y="48" text-anchor="middle" font-size="14" fill="#1565c0">95</text>
  <text x="255" y="120" text-anchor="middle" font-size="11" fill="#c62828">b[0] を変更すると a[0] も変わる</text>
</svg>

```csharp
int[] a = { 1, 2, 3 };
int[] b = a;          // 参照のコピー（配列はひとつ）

b[0] = 99;
Console.WriteLine(a[0]);  // 99（a も変わっている！）
```

別の配列として独立したコピーを作るには `Array.Copy` を使います（後述）。

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

### Array.Copy — 別配列にコピー

**`Array.Copy`** — 配列の要素を別の配列にコピーします。<!-- [公式ドキュメント]() -->

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

- すべての配列は `System.Array` を継承する参照型
- 変数に格納されるのは参照（ポインタ）。`b = a` は配列本体をコピーしない
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

1. `99` です。`b = a` は参照のコピーのため、`b` と `a` は同じ配列を指しています。

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
