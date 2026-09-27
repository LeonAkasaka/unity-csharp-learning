---
layout: page
title: ジャグ配列
permalink: /csharp/jagged-arrays/
---

# ジャグ配列

**ジャグ配列**（jagged array）は、要素が配列になっている「配列の配列」です。行ごとに長さの違うデータを表せます。`int[][]` のように `[]` を 2 つ続けて書きます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- ジャグ配列を作り、初期化し、要素を読み書きできる
- ジャグ配列と多次元配列の違いを説明できる
- データの形に合わせて、ジャグ配列と多次元配列を選べる

## 前提知識

- [多次元配列](/unity-csharp-learning/csharp/multidimensional-arrays/) を読んでいること

---

## 1. ジャグ配列とは

多次元配列は、どの行も同じ列の数を持つ長方形の形です。ジャグ配列は、外側の配列の各要素が **それぞれ別の 1 次元の配列** を指しているので、行ごとに長さを変えられます。

![外側の配列の 3 つの要素が、それぞれ別の配列を指している。[0] は 10、20、30 の 3 要素、[1] は 1〜5 の 5 要素、[2] は 7、8 の 2 要素](jagged.svg)

[Array クラスと配列の性質（補足）](/unity-csharp-learning/csharp/array-class/) で学んだように、配列の変数には配列の場所を示す参照が入ります。ジャグ配列の外側の配列の要素にも、それぞれの行の配列への参照が入っています。

---

## 2. ジャグ配列を作る

**書式：[ジャグ配列の作成](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/arrays#jagged-arrays)**
```
型[][] 変数名 = new 型[行数][];
```

| 要素 | 説明 |
|---|---|
| `型[][]` | ジャグ配列の型。`型[]` の配列であることを表す |
| `new 型[行数][]` | 外側の配列を作る。各行の配列はまだ作られない |

`new int[3][]` で作られるのは、外側の配列だけです。各行の配列は、1 つずつ作って代入します。

```csharp
int[][] jagged = new int[3][];

jagged[0] = new int[] { 10, 20, 30 };
jagged[1] = new int[] { 1, 2, 3, 4, 5 };
jagged[2] = new int[] { 7, 8 };

Console.WriteLine(jagged[1][4]);
```

```
5
```

配列初期化子を使うと、まとめて書けます。

```csharp
int[][] jagged =
{
    new int[] { 10, 20, 30 },
    new int[] { 1, 2, 3, 4, 5 },
    new int[] { 7, 8 }
};

Console.WriteLine(jagged[0][2]);
```

```
30
```

---

## 3. 要素を読み書きする

**書式：[ジャグ配列の要素へのアクセス](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/arrays#jagged-arrays)**
```
配列[行のインデックス][列のインデックス]
```

`jagged[i]` で行 `i` の配列を取り出し、その配列の `[j]` で要素を指定します。

```csharp
int[][] jagged =
{
    new int[] { 10, 20, 30 },
    new int[] { 1, 2, 3, 4, 5 },
    new int[] { 7, 8 }
};

Console.WriteLine(jagged[0][1]);
Console.WriteLine(jagged[1][4]);

jagged[2][0] = 99;
Console.WriteLine(jagged[2][0]);
```

```
20
5
99
```

### すべての要素を処理する

各行は 1 次元の配列なので、`jagged[i].Length` でその行の要素の数がわかります。外側の配列の `jagged.Length` は、行の数です。

```csharp
int[][] jagged =
{
    new int[] { 10, 20, 30 },
    new int[] { 1, 2, 3, 4, 5 },
    new int[] { 7, 8 }
};

for (int i = 0; i < jagged.Length; i++)
{
    Console.Write($"行 {i}（{jagged[i].Length} 要素）: ");
    for (int j = 0; j < jagged[i].Length; j++)
    {
        Console.Write(jagged[i][j] + " ");
    }
    Console.WriteLine();
}
```

```
行 0（3 要素）: 10 20 30 
行 1（5 要素）: 1 2 3 4 5 
行 2（2 要素）: 7 8 
```

内側の `for` 文の条件に `jagged[i].Length` を使っているので、行ごとに違う長さに合わせて繰り返せます。

---

## 4. 多次元配列との比較

| | 多次元配列 `int[,]` | ジャグ配列 `int[][]` |
|---|---|---|
| 形 | どの行も同じ列の数（長方形） | 行ごとに列の数を変えられる |
| 作り方 | `new int[3, 4]` | `new int[3][]` の後、各行を作る |
| 要素の指定 | `a[i, j]` | `a[i][j]` |
| `Length` | すべての要素の数 | 行の数 |
| 行の要素の数 | `GetLength(1)`（どの行も同じ） | `a[i].Length`（行ごとに違う） |
| メモリ | 1 つの続いた領域 | 行ごとに別々の配列 |

### 使い分けの目安

- **どの行も同じ長さ**（表、マス目のマップなど）→ 多次元配列 `int[,]`
- **行によって長さが違う**（学年ごとに人数の違う名簿、三角形の表など）→ ジャグ配列 `int[][]`

---

## よくあるミス

### 行の配列を作る前に要素を使う

```csharp
// ❌ NG: 外側の配列を作っただけで、行の配列はまだない
// int[][] jagged = new int[3][];
// Console.WriteLine(jagged[0][0]);  // NullReferenceException
```

`new int[3][]` で作った直後の外側の配列の要素は、どの配列も指していない `null` です。`jagged[0][0]` を使おうとすると、[NullReferenceException](https://learn.microsoft.com/dotnet/api/system.nullreferenceexception) が発生します。各行の配列を作ってから使います。

```csharp
int[][] jagged = new int[3][];
jagged[0] = new int[4];
Console.WriteLine(jagged[0][0]);
```

```
0
```

---

## まとめ

- ジャグ配列 `型[][]` は「配列の配列」で、行ごとに長さの違うデータを表せる
- `new 型[行数][]` で外側の配列を作り、各行の配列を別に作って代入する
- 要素は `配列[行][列]` で指定する
- `jagged.Length` は行の数、`jagged[i].Length` は行 `i` の要素の数
- どの行も同じ長さなら多次元配列、行によって長さが違うならジャグ配列を選ぶ

---

## 理解度チェック

1. `int[][] data = new int[4][];` を実行した直後の `data[0]` には、何が入っていますか？
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   int[][] jagged =
   {
       new int[] { 1 },
       new int[] { 1, 2 },
       new int[] { 1, 2, 3 }
   };

   Console.WriteLine(jagged.Length);
   Console.WriteLine(jagged[2].Length);
   Console.WriteLine(jagged[1][1]);
   ```

3. 2 のジャグ配列のすべての要素の合計を表示するコードを書いてください。

<details markdown="1">
<summary>解答を見る</summary>

1. `null` です。外側の配列を作っただけで、各行の配列はまだ作られていません。
2. 次のように出力されます。`jagged.Length` は行の数の `3`、`jagged[2].Length` は行 2 の要素の数の `3`、`jagged[1][1]` は行 1 の 2 番目の要素の `2` です。

   ```
   3
   3
   2
   ```

3. ```csharp
   int[][] jagged =
   {
       new int[] { 1 },
       new int[] { 1, 2 },
       new int[] { 1, 2, 3 }
   };

   int sum = 0;
   for (int i = 0; i < jagged.Length; i++)
   {
       for (int j = 0; j < jagged[i].Length; j++)
       {
           sum += jagged[i][j];
       }
   }
   Console.WriteLine(sum);
   ```

   `10` が表示されます。

</details>

---

## 次のステップ

これで「C# 配列と集合操作」のセクションは終わりです。[クラスとフィールド](/unity-csharp-learning/csharp/classes/) からは「C# クラスとオブジェクト」のセクションに進み、データと処理をまとめる **クラス** を学びます。
