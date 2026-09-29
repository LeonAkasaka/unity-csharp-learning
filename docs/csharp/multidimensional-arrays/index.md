---
layout: page
title: 多次元配列
permalink: /csharp/multidimensional-arrays/
---

# 多次元配列

**多次元配列** は、行と列のように、2 つ以上の番号で要素を指定する配列です。表や、マス目のあるマップのようなデータを扱えます。C# では、`int[,]` のように `[]` の中に `,` を書いて表します。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- 2 次元配列を作り、初期化し、要素を読み書きできる
- `GetLength(0)` と `GetLength(1)` を使い、入れ子の `for` 文ですべての要素を処理できる
- `Rank` と `Length` で、2 次元配列の次元の数と要素の数を調べられる

## 前提知識

- [配列の基礎](/unity-csharp-learning/csharp/arrays/) を読んでいること
- [Array クラスと配列の性質（補足）](/unity-csharp-learning/csharp/array-class/) を読んでいること

---

## 1. 2 次元配列とは

2 次元配列は、**行**（row）と **列**（column）の 2 つのインデックスで要素を指定します。

![3 行 4 列の 2 次元配列 matrix。1 行目に 1〜4、2 行目に 5〜8、3 行目に 9〜12 が並ぶ。左上は matrix[0, 0]、右下は matrix[2, 3]](matrix.svg)

3 行 4 列の `int[,] matrix` では、`matrix[行, 列]` の形で要素を指定します。左上は `matrix[0, 0]`、右下は `matrix[2, 3]` です。

---

## 2. 2 次元配列を作る

**書式：[2 次元配列の作成](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/arrays#multidimensional-arrays)**
```
型[,] 変数名 = new 型[行数, 列数];
```

| 要素 | 説明 |
|---|---|
| `型[,]` | 2 次元配列の型。`,` の数 + 1 が次元の数 |
| `new 型[行数, 列数]` | 指定した大きさの配列を作る。要素には既定値が入る |

```csharp
int[,] matrix = new int[3, 4];
Console.WriteLine(matrix[2, 3]);
```

```
0
```

### 配列初期化子で作る

1 行分の値を `{ }` で囲み、それを行の数だけ並べます。

```csharp
int[,] matrix =
{
    { 1, 2, 3, 4 },
    { 5, 6, 7, 8 },
    { 9, 10, 11, 12 }
};

Console.WriteLine(matrix[1, 0]);
```

```
5
```

1 つ目の `{ }` が行 0、2 つ目が行 1、3 つ目が行 2 です。どの行も、同じ数の値を書く必要があります。

---

## 3. 要素を読み書きする

**書式：[2 次元配列の要素へのアクセス](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/arrays#multidimensional-arrays)**
```
配列[行のインデックス, 列のインデックス]
```

```csharp
int[,] matrix =
{
    { 1, 2, 3, 4 },
    { 5, 6, 7, 8 },
    { 9, 10, 11, 12 }
};

Console.WriteLine(matrix[0, 0]);
Console.WriteLine(matrix[1, 2]);
Console.WriteLine(matrix[2, 3]);

matrix[0, 0] = 99;
Console.WriteLine(matrix[0, 0]);
```

```
1
7
12
99
```

`matrix[1, 2]` は、行 1 の列 2 の要素なので `7` です。

---

## 4. すべての要素を処理する

### 入れ子の for 文

[Array クラスと配列の性質（補足）](/unity-csharp-learning/csharp/array-class/) で学んだ `GetLength` を使うと、`GetLength(0)` で行の数、`GetLength(1)` で列の数がわかります。外側の `for` 文で行を、内側の `for` 文で列を順に変えると、すべての要素を処理できます。

```csharp
int[,] matrix =
{
    { 1, 2, 3, 4 },
    { 5, 6, 7, 8 },
    { 9, 10, 11, 12 }
};

int rows = matrix.GetLength(0);
int cols = matrix.GetLength(1);
Console.WriteLine($"{rows} 行 {cols} 列");

for (int i = 0; i < rows; i++)
{
    for (int j = 0; j < cols; j++)
    {
        Console.Write($"{matrix[i, j],3}");
    }
    Console.WriteLine();
}
```

```
3 行 4 列
  1  2  3  4
  5  6  7  8
  9 10 11 12
```

文字列補間の `{matrix[i, j],3}` は、値を 3 文字分の幅で右にそろえて表示します。幅の指定は、[文字列リテラルと書式（補足）](/unity-csharp-learning/csharp/string-literals/) で説明しています。行ごとに `Console.Write` で横に並べ、行の最後に `Console.WriteLine()` で改行しています。

### foreach 文

`foreach` 文を使うと、2 次元配列のすべての要素を、行 0 の左から順に 1 つずつ取り出せます。

```csharp
int[,] matrix =
{
    { 1, 2, 3 },
    { 4, 5, 6 }
};

foreach (int value in matrix)
{
    Console.Write(value + " ");
}
Console.WriteLine();
```

```
1 2 3 4 5 6 
```

何行目の何列目かがわからなくなるので、行や列を区別したいときは、入れ子の `for` 文を使います。

### Rank と Length

```csharp
int[,] matrix = new int[3, 4];
Console.WriteLine(matrix.Rank);
Console.WriteLine(matrix.Length);
```

```
2
12
```

`Rank` は次元の数、`Length` はすべての要素の数（3 × 4 = 12）です。2 次元配列の `Length` は、行の数ではないことに注意しましょう。

---

## よくあるミス

### Length を行の数だと思って使う

```csharp
// ❌ NG: Length（すべての要素の数 12）を行の数として使っている
// int[,] matrix = new int[3, 4];
// for (int i = 0; i < matrix.Length; i++)
// {
//     Console.WriteLine(matrix[i, 0]);  // i が 3 のとき IndexOutOfRangeException
// }
```

2 次元配列の `Length` は、すべての要素の数の `12` です。行の数のつもりで `i < matrix.Length` と書くと、存在しない行 3 までインデックスが進み、`IndexOutOfRangeException` が発生します。行の数は `GetLength(0)`、列の数は `GetLength(1)` で調べます。

---

## ワンポイントアドバイス

### 3 次元以上の配列

`,` を増やすと、3 次元以上の配列も作れます。

```csharp
int[,,] cube = new int[2, 3, 4];
Console.WriteLine(cube.Rank);
Console.WriteLine(cube.Length);
```

```
3
24
```

実際には、3 次元以上の配列はあまり使われません。次のページで学ぶジャグ配列や、後で学ぶクラスを組み合わせて表すことが多いです。

---

## まとめ

- `型[,]` は 2 次元配列の型。`new 型[行数, 列数]` か配列初期化子で作る
- 要素は `配列[行, 列]` で指定する
- `GetLength(0)` で行の数、`GetLength(1)` で列の数がわかる。入れ子の `for` 文ですべての要素を処理できる
- `foreach` 文では、すべての要素が行の順に取り出される
- `Rank` は次元の数、`Length` はすべての要素の数

---

## 理解度チェック

1. `int[,] grid = new int[5, 3];` の行の数、列の数、すべての要素の数は、それぞれいくつですか？
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   int[,] matrix = { { 1, 2 }, { 3, 4 }, { 5, 6 } };
   int sum = 0;
   for (int i = 0; i < matrix.GetLength(0); i++)
   {
       for (int j = 0; j < matrix.GetLength(1); j++)
       {
           sum += matrix[i, j];
       }
   }
   Console.WriteLine(sum);
   Console.WriteLine(matrix[2, 1]);
   ```

3. `matrix[1, 2]` と `matrix[2, 1]` は、同じ要素を指しますか？

<details markdown="1">
<summary>解答を見る</summary>

1. 行の数は `5`（`GetLength(0)`）、列の数は `3`（`GetLength(1)`）、すべての要素の数は `15`（`Length`）です。
2. 次のように出力されます。すべての要素の合計は `21` です。`matrix[2, 1]` は行 2 の列 1 なので `6` です。

   ```
   21
   6
   ```

3. 違う要素です。`matrix[1, 2]` は行 1 の列 2、`matrix[2, 1]` は行 2 の列 1 を指します。

</details>

---

## 次のステップ

[ジャグ配列](/unity-csharp-learning/csharp/jagged-arrays/) では、行ごとに長さの違う「配列の配列」を学び、多次元配列との使い分けを確かめます。
