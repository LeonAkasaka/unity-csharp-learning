---
layout: page
title: ref ローカルと ref 戻り値
permalink: /csharp/ref-locals/
---

# ref ローカルと ref 戻り値

[ref / out / in パラメータ](/unity-csharp-learning/csharp/ref-out-in/) では、`ref` を付けたパラメータで、呼び出し元の変数そのものを受け取りました。`ref` は、パラメータのほかに、ローカル変数やメソッドの戻り値にも付けられます。このページでは、ほかの変数を指す **ref ローカル**（ref local）と、変数を指す参照を返す **ref 戻り値**（ref return）を学びます。この後のページで学ぶ `Span<T>` を理解するための準備です。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- 構造体の配列の要素を変数に取り出すとコピーになることを説明できる
- ref ローカルで配列の要素を指し、コピーせずに読み書きできる
- ref 戻り値を返すメソッドを定義し、呼び出し側で ref ローカルとして受け取れる
- メソッドのローカル変数への参照を返せない理由を説明できる
- `ref readonly` で、書き換えられない参照を作れる

## 前提知識

- [ref / out / in パラメータ](/unity-csharp-learning/csharp/ref-out-in/) を読んでいること
- [再帰関数とコールスタック](/unity-csharp-learning/csharp/recursion/) でスタックフレームを学んだこと
- [構造体](/unity-csharp-learning/csharp/structs/) を読んでいること

---

## 1. 要素を変数に取り出すとコピーになる

[構造体](/unity-csharp-learning/csharp/structs/) で学んだように、構造体の配列では、要素の 1 つ 1 つが値そのものです。要素を変数に代入すると、値がコピーされます。

```csharp
Point[] points = new Point[3];

Point copy = points[0];
copy.X = 10;
Console.WriteLine($"copy: ({copy.X}, {copy.Y})");
Console.WriteLine($"points[0]: ({points[0].X}, {points[0].Y})");

struct Point
{
    public int X;
    public int Y;
}
```

```
copy: (10, 0)
points[0]: (0, 0)
```

`copy` は `points[0]` のコピーなので、`copy.X` を書き換えても配列の要素は変わりません。配列の要素を書き換えるには、`points[0].X = 10` のように、毎回配列とインデックスを書く必要があります。

要素に名前を付けて、何度も読み書きしたい場面があります。そのとき必要なのは、値のコピーではなく、配列の要素そのものを指す変数です。

---

## 2. ref ローカル

変数の宣言で型の前に `ref` を付けると、ほかの変数を指す **ref ローカル** になります。初期化するときは、右辺にも `ref` を付けて、指す先の変数を書きます。

**書式：[ref ローカル](https://learn.microsoft.com/dotnet/csharp/language-reference/statements/declarations#reference-variables)**
```
ref 型 変数名 = ref 指す先の変数;
```

| 要素 | 説明 |
|---|---|
| `ref`（左辺） | この変数が、ほかの変数を指す ref ローカルであることを表す |
| `ref`（右辺） | 値ではなく、変数そのものを指すことを表す |
| `指す先の変数` | ローカル変数、配列の要素、フィールドなど |

1 節のコードに、ref ローカルを使う例を追加します。

```csharp
Point[] points = new Point[3];

Point copy = points[0];
copy.X = 10;
Console.WriteLine($"copy: ({copy.X}, {copy.Y})");
Console.WriteLine($"points[0]: ({points[0].X}, {points[0].Y})");

ref Point p = ref points[0];
p.X = 10;
Console.WriteLine($"p: ({p.X}, {p.Y})");
Console.WriteLine($"points[0]: ({points[0].X}, {points[0].Y})");

struct Point
{
    public int X;
    public int Y;
}
```

```
copy: (10, 0)
points[0]: (0, 0)
p: (10, 0)
points[0]: (10, 0)
```

`p` は `points[0]` そのものを指しているので、`p.X = 10` で配列の要素が書き換わります。`ref` パラメータと同じように、`p` は `points[0]` の別名として働きます。

![配列 points の要素 [0] を ref Point p が直接指している。Point copy は配列とは別の場所にある、取り出したときのコピーである様子](copy-vs-ref.svg)

ref ローカルは、宣言するときに必ず指す先を決めます。`ref int r;` のように初期化せずに宣言すると、コンパイルエラー（CS8174）になります。

### 指す先を付け替える

ref ローカルに、右辺に `ref` を付けて代入すると、指す先を別の変数に付け替えられます。`ref` を付けずに代入すると、指している変数に値を書き込みます。

```csharp
int[] numbers = { 1, 2, 3 };
ref int r = ref numbers[0];
r = 100;
r = ref numbers[2];
r = 300;
Console.WriteLine(string.Join(", ", numbers));
```

```
100, 2, 300
```

| 書き方 | 意味 |
|---|---|
| `r = 100;` | `r` が指している変数に `100` を書き込む |
| `r = ref numbers[2];` | `r` が指す先を `numbers[2]` に付け替える |

---

## 3. ref 戻り値

メソッドの戻り値の型に `ref` を付けると、値ではなく、変数を指す参照を返せます。これを **ref 戻り値** といいます。`return` にも `ref` を付けて、返す変数を書きます。

**書式：[ref 戻り値](https://learn.microsoft.com/dotnet/csharp/language-reference/statements/jump-statements#ref-returns)**
```
ref 型 メソッド名(パラメータ)
{
    return ref 返す変数;
}

ref 型 変数名 = ref メソッド名(引数);
```

次の `FindLowest` は、配列の中でいちばん小さい要素を探し、その要素への参照を返します。

```csharp
int[] scores = { 70, 85, 60, 90 };
ref int lowest = ref FindLowest(scores);
lowest = 100;
Console.WriteLine(string.Join(", ", scores));

int copy = FindLowest(scores);
copy = 0;
Console.WriteLine(string.Join(", ", scores));

ref int FindLowest(int[] values)
{
    int index = 0;
    for (int i = 1; i < values.Length; i++)
    {
        if (values[i] < values[index])
        {
            index = i;
        }
    }
    return ref values[index];
}
```

```
70, 85, 100, 90
70, 85, 100, 90
```

`ref FindLowest(scores)` を ref ローカルの `lowest` で受け取ると、`lowest` は配列の `60` の要素を指します。`lowest = 100` で、配列の要素が `100` に書き換わりました。

ref 戻り値のメソッドを呼び出すときに `ref` を付けないと、指している変数の値がコピーされます。2 回目の呼び出しでは、`copy` はいちばん小さい要素（`70`）のコピーなので、`copy = 0` としても配列は変わりません。

| 呼び出し方 | 受け取るもの |
|---|---|
| `ref int x = ref FindLowest(scores);` | 配列の要素を指す参照 |
| `int x = FindLowest(scores);` | 配列の要素の値のコピー |

---

## 4. 返せる参照と返せない参照

ref 戻り値で返せるのは、メソッドが終わった後も残っている変数への参照だけです。メソッドのローカル変数への参照を返そうとすると、コンパイルエラーになります。

```csharp
ref int Bad()
{
    int local = 10;
    return ref local;  // ❌ CS8168: ローカル変数への参照は返せない
}
```

[再帰関数とコールスタック](/unity-csharp-learning/csharp/recursion/) で学んだように、ローカル変数は、メソッドを呼び出したときに積まれるスタックフレームの中にあります。メソッドから戻ると、スタックフレームは取り除かれ、その場所は次に呼び出すメソッドが使います。もし `local` への参照を返せたとすると、呼び出し元は、もう存在しない変数を指す参照を持つことになります。

値渡しのパラメータも、メソッドのスタックフレームの中にあるので、参照を返せません（CS8166）。返せるのは、次のような変数への参照です。

| 返せる変数 | 理由 |
|---|---|
| パラメータで受け取った配列の要素 | 配列はヒープにあり、メソッドが終わっても残る |
| `ref` パラメータ | 呼び出し元の変数を指しているので、呼び出し元では残っている |
| クラスのフィールド | オブジェクトはヒープにあり、メソッドが終わっても残る |

コンパイラーは、参照が指す変数がいつまで存在するかを調べ、存在しなくなる変数を指す参照を外に出さないようにしています。この後のページで学ぶ `Span<T>` にも、同じ考え方による制約があります。

---

## 5. ref readonly

`ref` の代わりに `ref readonly` を付けると、読み取り専用の参照になります。指している変数を読めますが、書き換えることはできません。

**書式：[ref readonly](https://learn.microsoft.com/dotnet/csharp/language-reference/statements/declarations#reference-variables)**
```
ref readonly 型 変数名 = ref 指す先の変数;

ref readonly 型 メソッド名(パラメータ)
```

```csharp
Point[] points = { new Point { X = 1, Y = 2 }, new Point { X = 3, Y = 4 } };
ref readonly Point p = ref points[1];
Console.WriteLine($"({p.X}, {p.Y})");
points[1].X = 30;
Console.WriteLine($"({p.X}, {p.Y})");

struct Point
{
    public int X;
    public int Y;
}
```

```
(3, 4)
(30, 4)
```

`p` はコピーではなく `points[1]` そのものを指しているので、配列の要素を書き換えると、`p` から読む値も変わります。一方、`p` を通して書き換えることはできません。

```csharp
ref readonly Point p = ref points[1];
p.X = 5;  // ❌ CS0131: 読み取り専用の参照を通して書き換えることはできない
```

`ref readonly` の使いどころは、[ref / out / in パラメータ](/unity-csharp-learning/csharp/ref-out-in/) の `in` パラメータと同じです。フィールドの多い大きな構造体を、コピーせずに読み取りたいときに使います。ref 戻り値を `ref readonly` にすると、呼び出し元に要素を読ませつつ、書き換えは禁止できます。

---

## よくあるミス

### 右辺の ref を付け忘れる

ref ローカルの初期化では、左辺と右辺の両方に `ref` を書きます。右辺の `ref` を忘れると、値を初期化に使おうとしていることになり、コンパイルエラーになります。

```csharp
int[] values = { 1, 2, 3 };

// ❌ NG: 右辺に ref がない
// ref int r = values[0];  // CS8172

// ✅ OK: 両方に ref を書く
ref int r = ref values[0];
```

### List\<T\> の要素を ref で受け取る

配列の要素とは違い、[List\<T\>](/unity-csharp-learning/csharp/list/) の要素は `ref` で受け取れません。

```csharp
List<int> list = new List<int> { 1, 2, 3 };
ref int r = ref list[0];  // ❌ CS0206: 参照を返さないインデクサは ref で使えない
```

配列の `values[0]` は、配列の中の変数そのものを表します。一方、`List<T>` の `list[0]` は、[インデクサ](/unity-csharp-learning/csharp/indexers/) の `get` アクセサーを呼び出して、要素の値のコピーを受け取る式です。[構造体](/unity-csharp-learning/csharp/structs/) の 4 節で、プロパティから受け取った構造体を書き換えられなかったのと同じ理由です。

---

## ワンポイントアドバイス

### フィールドへの参照を返すときの注意

クラスのフィールドへの参照を ref 戻り値で返すと、`private` のフィールドでも、呼び出し元から自由に書き換えられるようになります。[アクセス修飾子](/unity-csharp-learning/csharp/access-modifiers/) で守っていたフィールドを外に開くことになるので、書き換えさせる必要がなければ `ref readonly` で返します。

---

## まとめ

- 構造体の配列の要素を変数に代入すると、値のコピーになる
- `ref 型 変数名 = ref 変数;` で、ほかの変数を指す ref ローカルを宣言できる。ref ローカルを通して、指している変数を読み書きできる
- ref ローカルは、右辺に `ref` を付けて代入すると、指す先を付け替えられる
- 戻り値の型に `ref` を付けると、変数への参照を返せる。呼び出し側で `ref` を付けずに受け取ると、値のコピーになる
- メソッドのローカル変数や値渡しのパラメータは、メソッドから戻ると存在しなくなるので、参照を返せない
- `ref readonly` は、コピーせずに読み取るための、書き換えられない参照

---

## 理解度チェック

1. メソッドのローカル変数への参照を ref 戻り値で返せないのはなぜですか？
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   int[] values = { 1, 2, 3 };
   ref int a = ref values[0];
   int b = values[1];
   a = 10;
   b = 20;
   a = ref values[2];
   a += 5;
   Console.WriteLine(string.Join(", ", values));
   ```

3. `int` の配列を受け取り、最後の要素への参照を返すメソッド `Last` を書いてください。また、`Last` を使って、配列 `{ 1, 2, 3 }` の最後の要素を `99` に書き換えてください。

<details markdown="1">
<summary>解答を見る</summary>

1. ローカル変数は、メソッドのスタックフレームの中にあり、メソッドから戻ると存在しなくなるからです。参照を返せたとすると、呼び出し元が、もう存在しない変数を指す参照を持つことになります。
2. 次のように出力されます。`a` は最初 `values[0]` を指しているので、`a = 10` で `values[0]` が `10` になります。`b` は `values[1]` のコピーなので、`b = 20` は配列に影響しません。`a = ref values[2]` で指す先が `values[2]` に変わり、`a += 5` で `values[2]` が `8` になります。

   ```
   10, 2, 8
   ```

3. ```csharp
   int[] values = { 1, 2, 3 };
   ref int last = ref Last(values);
   last = 99;
   Console.WriteLine(string.Join(", ", values));

   ref int Last(int[] array)
   {
       return ref array[array.Length - 1];
   }
   ```

   ```
   1, 2, 99
   ```

</details>

---

## 次のステップ

[Span\<T\> と ReadOnlySpan\<T\>](/unity-csharp-learning/csharp/span/) では、配列や文字列の一部を、コピーせずに指して扱う方法を学びます。
