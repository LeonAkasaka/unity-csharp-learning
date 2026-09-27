---
layout: page
title: ref / out / in パラメータ
permalink: /csharp/ref-out-in/
---

# ref / out / in パラメータ

メソッドのパラメータには、ふつう引数の値のコピーが渡されます（**値渡し**）。パラメータに `ref`・`out`・`in` を付けると、呼び出し元の変数そのものを渡せます（**参照渡し**）。メソッドの中から呼び出し元の変数を書き換えたり、複数の結果を返したりするときに使います。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- 値渡しでは、呼び出し元の変数が変わらないことを説明できる
- `ref` で、呼び出し元の変数をメソッドの中から書き換えられる
- `out` で、メソッドから結果を受け取れる
- `in` が、読み取り専用の参照渡しであることを説明できる

## 前提知識

- [メソッド](/unity-csharp-learning/csharp/methods/) を読んでいること

---

## 1. 値渡し

ふつうのパラメータは **値渡し** です。メソッドには、引数の値のコピーが渡されます。

```csharp
A a = new A();
int v = 0;
a.M(v);
Console.WriteLine($"v={v}");

class A
{
    public void M(int x)
    {
        x = 1;
        Console.WriteLine($"A.M: x={x}");
    }
}
```

```
A.M: x=1
v=0
```

メソッドの中で `x` を `1` にしても、呼び出し元の `v` は `0` のままです。`x` には `v` の値のコピーが入っていて、`x` と `v` は別々の変数だからです。

---

## 2. ref — 呼び出し元の変数を書き換える

パラメータに [ref](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/method-parameters#ref-parameter-modifier) を付けると、呼び出し元の変数そのものを受け取ります。メソッドの中でパラメータを書き換えると、呼び出し元の変数も変わります。

**書式：[ref パラメータ](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/method-parameters#ref-parameter-modifier)**
```
戻り値の型 メソッド名(ref 型 パラメータ名)

メソッド名(ref 変数名);
```

| 要素 | 説明 |
|---|---|
| `ref`（定義側） | このパラメータが参照渡しであることを表す |
| `ref`（呼び出し側） | 変数を参照渡しで渡すことを表す。定義側と呼び出し側の両方に書く |
| `変数名` | 渡す変数。値を入れておく必要がある |

```csharp
A a = new A();
int v = 0;
a.M(ref v);
Console.WriteLine($"v={v}");

class A
{
    public void M(ref int x)
    {
        x = 1;
        Console.WriteLine($"A.M: x={x}");
    }
}
```

```
A.M: x=1
v=1
```

`x` は `v` そのものを指しているので、`x = 1` で `v` も `1` になります。

`ref` で渡す変数には、呼び出す前に値を入れておく必要があります。メソッドの中で、今の値を読み取るかもしれないからです。

### 2 つの変数の値を入れ替える

`ref` を使うと、呼び出し元の 2 つの変数の値を入れ替えるメソッドを作れます。

```csharp
A a = new A();
int x = 1;
int y = 2;
a.Swap(ref x, ref y);
Console.WriteLine($"x={x}, y={y}");

class A
{
    public void Swap(ref int a, ref int b)
    {
        int temp = a;
        a = b;
        b = temp;
    }
}
```

```
x=2, y=1
```

値渡しでは、メソッドの中で入れ替えてもコピーが入れ替わるだけなので、このメソッドは作れません。

---

## 3. out — メソッドから結果を受け取る

[out](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/method-parameters#out-parameter-modifier) も参照渡しですが、目的は **メソッドから呼び出し元に値を返すこと** です。戻り値のほかに、結果を返したいときに使います。

**書式：[out パラメータ](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/method-parameters#out-parameter-modifier)**
```
戻り値の型 メソッド名(out 型 パラメータ名)

メソッド名(out 変数名);
メソッド名(out 型 変数名);
```

`out` には、`ref` と違う決まりがあります。

- 渡す変数に、前もって値を入れておく必要はない
- メソッドは、終わるまでに必ず `out` のパラメータに値を入れなければならない
- 呼び出し側で `out int w` のように書くと、変数の宣言と受け取りを同時にできる

```csharp
A a = new A();

a.Divide(17, 5, out int remainder);
Console.WriteLine($"余り={remainder}");

int quotient = a.Divide(17, 5, out int r);
Console.WriteLine($"商={quotient}, 余り={r}");

class A
{
    public int Divide(int x, int y, out int remainder)
    {
        remainder = x % y;
        return x / y;
    }
}
```

```
余り=2
商=3, 余り=2
```

`Divide` は、戻り値で商を、`out` のパラメータで余りを返します。

### int.TryParse

.NET にも、`out` を使うメソッドがあります。[int.TryParse メソッド](https://learn.microsoft.com/dotnet/api/system.int32.tryparse) は、文字列を `int` に変換できたかどうかを戻り値の `bool` で返し、変換した値を `out` のパラメータで返します。

```csharp
if (int.TryParse("42", out int n))
{
    Console.WriteLine($"変換できた: {n}");
}

if (!int.TryParse("abc", out int m))
{
    Console.WriteLine($"変換できなかった: m={m}");
}
```

```
変換できた: 42
変換できなかった: m=0
```

変換できなかったときも、`out` のパラメータには必ず値（`int` の既定値 `0`）が入ります。

---

## 4. in — 読み取り専用の参照渡し

[in](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/method-parameters#in-parameter-modifier) も参照渡しですが、**読み取り専用** です。メソッドの中でパラメータを読めますが、書き換えることはできません。

**書式：[in パラメータ](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/method-parameters#in-parameter-modifier)**
```
戻り値の型 メソッド名(in 型 パラメータ名)

メソッド名(変数名);
メソッド名(in 変数名);
```

`in` は、`ref` や `out` と違い、呼び出し側では書いても省略してもかまいません。

```csharp
A a = new A();
int v = 5;
a.Show(v);
a.Show(in v);

class A
{
    public void Show(in int x)
    {
        Console.WriteLine($"A.Show: x={x}");
    }
}
```

```
A.Show: x=5
A.Show: x=5
```

値渡しでは値がコピーされますが、`in` ではコピーせずに元の変数を参照します。`int` のような小さな型では効果はほとんどありませんが、フィールドの多い大きな構造体（[構造体](/unity-csharp-learning/csharp/structs/) で学びます）を渡すときに、コピーの手間を省けます。

---

## 5. 3 つの違いのまとめ

| | `ref` | `out` | `in` |
|---|---|---|---|
| 目的 | 呼び出し元の変数を読み書きする | 結果を返す | 読み取るだけ（コピーを避ける） |
| 渡す前に値が必要 | 必要 | 不要 | 必要 |
| メソッドの中で書き換え | できる | 必ず代入する | できない |
| 呼び出し側のキーワード | 必要 | 必要 | 省略できる |

---

## よくあるミス

### 呼び出し側の ref を付け忘れる

```csharp
// ❌ NG: 呼び出し側にも ref が必要
// int v = 0;
// M(v);  // CS1620
//
// void M(ref int x)
// {
//     x = 1;
// }
```

`ref` のパラメータには、呼び出し側でも `ref` を付けて渡します。呼び出し側で `ref` を書かせることで、変数が書き換えられるかもしれないことが、呼び出し元のコードを読むだけでわかるようになっています。

### out のパラメータに値を入れずにメソッドを終える

```csharp
// ❌ NG: out のパラメータ x に値を入れていない
// void M(out int x)
// {
//     Console.WriteLine("M");
// }  // CS0177
```

`out` のパラメータには、メソッドが終わるまでに必ず値を入れます。途中で `return` する道筋がある場合も、それぞれの道筋で値を入れる必要があります。

### in のパラメータを書き換える

```csharp
// ❌ NG: in のパラメータは読み取り専用
// void M(in int x)
// {
//     x = 2;  // CS8331
// }
```

---

## まとめ

- ふつうのパラメータは値渡しで、メソッドにはコピーが渡される。呼び出し元の変数は変わらない
- `ref` は参照渡しで、メソッドの中から呼び出し元の変数を書き換えられる。渡す前に値が必要
- `out` は結果を返すための参照渡しで、メソッドは必ず値を入れる。`out int n` のように宣言と同時に受け取れる
- `in` は読み取り専用の参照渡しで、呼び出し側のキーワードは省略できる
- `ref` と `out` は、定義側と呼び出し側の両方に書く

---

## 理解度チェック

1. 値渡しのパラメータをメソッドの中で書き換えても、呼び出し元の変数が変わらないのはなぜですか？
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   A a = new A();
   int v = 3;
   a.M(ref v);
   a.M(ref v);
   Console.WriteLine(v);

   class A
   {
       public void M(ref int x)
       {
           x = x * 2;
       }
   }
   ```

3. 配列の最小値と最大値を、`out` のパラメータで 2 つとも返す `MinMax(int[] values, out int min, out int max)` メソッドを書いてください。

<details markdown="1">
<summary>解答を見る</summary>

1. 値渡しでは、引数の値のコピーがパラメータに入るからです。パラメータと呼び出し元の変数は、別々の変数です。
2. `12` が出力されます。`v` は `ref` で渡されているので、1 回目で `6`、2 回目で `12` になります。

   ```
   12
   ```

3. ```csharp
   A a = new A();
   a.MinMax(new int[] { 5, 2, 9, 4 }, out int min, out int max);
   Console.WriteLine($"min={min}, max={max}");

   class A
   {
       public void MinMax(int[] values, out int min, out int max)
       {
           min = values[0];
           max = values[0];
           foreach (int value in values)
           {
               if (value < min)
               {
                   min = value;
               }
               if (value > max)
               {
                   max = value;
               }
           }
       }
   }
   ```

   `min=2, max=9` が表示されます。

</details>

---

## 次のステップ

[省略可能パラメータと名前付き引数](/unity-csharp-learning/csharp/optional-named-params/) では、引数を省略したり、パラメータの名前を指定して渡したりする書き方を学びます。
