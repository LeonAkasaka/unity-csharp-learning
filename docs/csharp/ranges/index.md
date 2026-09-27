---
layout: page
title: インデックスと範囲（補足）
permalink: /csharp/ranges/
---

# インデックスと範囲（補足）

このページは、[配列の基礎](/unity-csharp-learning/csharp/arrays/) と [Array クラスと配列の性質（補足）](/unity-csharp-learning/csharp/array-class/) の補足です。末尾から数えるインデックス `^` を復習し、配列の一部を新しい配列として取り出す **範囲演算子** `..` を学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `^1` が末尾の要素を、`^0` が末尾の要素の次を指すことを説明できる
- 範囲演算子 `..` で、配列の一部を取り出せる。終了位置の要素が含まれないことを説明できる
- 開始位置や終了位置を省略した範囲や、`^` を使った範囲を書ける
- 範囲で取り出した配列が、元の配列とは別の新しい配列であることを説明できる

## 前提知識

- [配列の基礎](/unity-csharp-learning/csharp/arrays/) を読んでいること
- [Array クラスと配列の性質（補足）](/unity-csharp-learning/csharp/array-class/) を読んでいること

---

## 1. 末尾から数えるインデックス

[配列の基礎](/unity-csharp-learning/csharp/arrays/) で学んだように、インデックスの前に [`^` 演算子](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/member-access-operators#index-from-end-operator-) を付けると、末尾から数えた位置になります。`^1` は末尾の要素、`^5` は、要素が 5 つの配列では先頭の要素です。

```csharp
int[] scores = { 85, 72, 90, 68, 95 };
Console.WriteLine(scores[^1]);
Console.WriteLine(scores[^5]);
```

```
95
85
```

`^n` は、`配列.Length - n` と同じ位置です。そのため、`^0` は `配列.Length` と同じで、末尾の要素の **次** を指します。`scores[^0]` は `scores[5]` と同じなので、読み書きしようとすると `IndexOutOfRangeException` が発生します。

`^0` は、要素を指定するためには使えませんが、次の節で学ぶ範囲の終わりを表すときに使います。

---

## 2. 範囲演算子 ..

[範囲演算子 ..](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/member-access-operators#range-operator-) を使うと、配列の一部を取り出せます。

**書式：[範囲演算子 ..](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/member-access-operators#range-operator-)**
```
配列[開始位置..終了位置]
```

| 要素 | 説明 |
|---|---|
| `開始位置` | 取り出す最初の要素のインデックス。この要素は含まれる |
| `終了位置` | 取り出す範囲の終わり。**この要素は含まれない** |

```csharp
int[] scores = { 85, 72, 90, 68, 95 };
int[] middle = scores[1..4];
Console.WriteLine(string.Join(", ", middle));
Console.WriteLine(middle.Length);
```

```
72, 90, 68
3
```

`scores[1..4]` は、インデックス `1`、`2`、`3` の要素です。インデックス `4` の `95` は含まれません。取り出される要素の数は、`終了位置 - 開始位置` の `3` になります。

終了位置の要素が含まれない理由は、範囲の数値を、要素そのものではなく、要素と要素の **境界** の位置と考えるとわかりやすくなります。境界 `1` から境界 `4` までの間にあるのが、`72`、`90`、`68` の 3 つです。

![85、72、90、68、95 の配列の要素の間に、先頭から数えた境界 0〜5 と、末尾から数えた境界 ^5〜^0 が振られている。範囲 [1..4] は、境界 1 から境界 4 までの 72、90、68 を表す様子](range-boundaries.svg)

この考え方では、`^0` は最後の要素の後ろの境界です。範囲の終わりに `^0` を書くと、最後の要素までを表せます。

### 開始位置と終了位置を省略する

開始位置を省略すると先頭から、終了位置を省略すると最後までになります。`^` を使って、末尾から数えた位置も指定できます。

```csharp
int[] scores = { 85, 72, 90, 68, 95 };
Console.WriteLine(string.Join(", ", scores[2..]));
Console.WriteLine(string.Join(", ", scores[..3]));
Console.WriteLine(string.Join(", ", scores[1..^1]));
Console.WriteLine(string.Join(", ", scores[^2..]));
Console.WriteLine(string.Join(", ", scores[..]));
Console.WriteLine(scores[2..2].Length);
```

```
90, 68, 95
85, 72, 90
72, 90, 68
68, 95
85, 72, 90, 68, 95
0
```

| 範囲 | 意味 |
|---|---|
| `[2..]` | インデックス `2` から最後まで（`[2..^0]` と同じ） |
| `[..3]` | 先頭から 3 つ（`[0..3]` と同じ） |
| `[1..^1]` | 先頭と末尾の要素を除いたもの |
| `[^2..]` | 末尾の 2 つ |
| `[..]` | すべての要素 |
| `[2..2]` | 開始位置と終了位置が同じなので、要素が 0 個 |

---

## 3. 範囲は新しい配列を作る

配列に範囲演算子を使うと、取り出した要素をコピーした **新しい配列** が作られます。そのため、取り出した配列を書き換えても、元の配列は変わりません。

```csharp
int[] scores = { 85, 72, 90, 68, 95 };
int[] part = scores[1..3];
part[0] = 0;
Console.WriteLine(string.Join(", ", part));
Console.WriteLine(string.Join(", ", scores));

int[] same = scores;
int[] copy = scores[..];
same[4] = 100;
copy[0] = 0;
Console.WriteLine(string.Join(", ", scores));
```

```
0, 90
85, 72, 90, 68, 95
85, 72, 90, 68, 100
```

`part[0] = 0` で書き換えたのは新しい配列なので、`scores` は変わりません。

後半では、`scores` を代入した `same` と、`scores[..]` で取り出した `copy` を比べています。[Array クラスと配列の性質（補足）](/unity-csharp-learning/csharp/array-class/) で学んだように、代入した `same` は `scores` と同じ配列を指すので、`same[4] = 100` で `scores` も変わります。`scores[..]` はすべての要素をコピーした新しい配列を作るので、`copy[0] = 0` は `scores` に影響しません。`scores[..]` は、`Array.Copy` で配列全体をコピーするのと同じ結果になります。

---

## よくあるミス

### 終了位置の要素も含まれると思い込む

「インデックス `1` から `3` までの要素」を取り出すつもりで `[1..3]` と書くと、インデックス `3` の要素は含まれません。

```csharp
int[] scores = { 85, 72, 90, 68, 95 };

// ❌ NG: インデックス 3 の 68 が含まれない
int[] a = scores[1..3];  // 72, 90

// ✅ OK: 終了位置は、最後に含めたい要素の次のインデックスにする
int[] b = scores[1..4];  // 72, 90, 68
```

終了位置には、含めたい最後の要素の **次** のインデックスを書きます。

### 開始位置が終了位置より後ろにある・範囲が配列を超える

開始位置が終了位置より後ろにある範囲や、配列の長さを超える範囲を指定すると、実行したときに [ArgumentOutOfRangeException](https://learn.microsoft.com/dotnet/api/system.argumentoutofrangeexception) が発生します。

```csharp
int[] scores = { 85, 72, 90, 68, 95 };
int[] a = scores[3..1];  // ❌ ArgumentOutOfRangeException: 開始位置が終了位置より後ろ
int[] b = scores[2..9];  // ❌ ArgumentOutOfRangeException: 配列の長さ（5）を超える
```

1 つの要素を指定するインデックスの範囲外とは違い、`IndexOutOfRangeException` ではなく `ArgumentOutOfRangeException` になります。

---

## ワンポイントアドバイス

### 文字列にも使える

文字列の 1 文字は、配列と同じようにインデックスで読めます。`^` と範囲演算子も使えます。範囲演算子は、指定した部分の文字をコピーした新しい文字列を作ります。

```csharp
string message = "Hello, World";
Console.WriteLine(message[7..]);
Console.WriteLine(message[..5]);
Console.WriteLine(message[^1]);
```

```
World
Hello
d
```

### ^1 や 1..4 の正体

`^1` は [Index](https://learn.microsoft.com/dotnet/api/system.index)、`1..4` は [Range](https://learn.microsoft.com/dotnet/api/system.range) という型の値です。`int` の値と同じように、変数に入れて使えます。

```csharp
int[] scores = { 85, 72, 90, 68, 95 };
Index last = ^1;
Range middle = 1..4;
Console.WriteLine(scores[last]);
Console.WriteLine(string.Join(", ", scores[middle]));
Console.WriteLine(last);
Console.WriteLine(middle);
```

```
95
72, 90, 68
^1
1..4
```

同じ範囲を何度も使うときは、`Range` の変数に入れておくと、範囲を 1 か所で決められます。`Index` と `Range` は、どちらも **構造体** という種類の型です。構造体は [構造体](/unity-csharp-learning/csharp/structs/) で学びます。

---

## まとめ

- `^n` は `Length - n` の位置を表す。`^1` は末尾の要素、`^0` は末尾の要素の次
- `配列[開始位置..終了位置]` で、配列の一部を取り出せる。終了位置の要素は含まれない
- 範囲の数値は、要素と要素の間の境界の位置と考えるとわかりやすい
- 開始位置を省略すると先頭から、終了位置を省略すると最後まで。`[..]` はすべての要素
- 配列に範囲演算子を使うと、要素をコピーした新しい配列が作られる
- 範囲が正しくないと `ArgumentOutOfRangeException` が発生する

---

## 理解度チェック

1. 要素が 5 つの配列 `scores` で、`scores[^0]` を読むと例外が発生するのに、`scores[2..^0]` は例外にならないのはなぜですか？
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   int[] nums = { 10, 20, 30, 40, 50, 60 };
   int[] a = nums[2..^1];
   int[] b = nums[^3..];
   a[0] = 0;
   Console.WriteLine(string.Join(", ", a));
   Console.WriteLine(string.Join(", ", b));
   Console.WriteLine(nums[2]);
   ```

3. 配列 `values` から、先頭と末尾の要素を除いた新しい配列 `inner` を作るコードを書いてください。

<details markdown="1">
<summary>解答を見る</summary>

1. `^0` は末尾の要素の次の位置で、そこに要素はないからです。範囲の終わりは、その位置の要素を含まないので、`^0` を書くと最後の要素までを表します。
2. 次のように出力されます。`a` はインデックス `2` から末尾の要素の手前まで（`30`、`40`、`50`）、`b` は末尾の 3 つ（`40`、`50`、`60`）です。`a` は新しい配列なので、`a[0] = 0` としても `nums[2]` は `30` のままです。

   ```
   0, 40, 50
   40, 50, 60
   30
   ```

3. ```csharp
   int[] values = { 3, 1, 4, 1, 5, 9 };
   int[] inner = values[1..^1];
   Console.WriteLine(string.Join(", ", inner));
   ```

   ```
   1, 4, 1, 5
   ```

</details>

---

## 次のステップ

[ビットパッキング（補足）](/unity-csharp-learning/csharp/bit-packing/) では、`bool` の配列の値を、1 つの `byte` のビットに詰めて保存する方法を学びます。
