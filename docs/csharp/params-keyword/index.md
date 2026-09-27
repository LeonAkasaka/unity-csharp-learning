---
layout: page
title: params キーワード
permalink: /csharp/params-keyword/
---

# params キーワード

配列を受け取るメソッドを呼び出すときは、`M(new int[] { 1, 2, 3 })` のように、配列を作って渡す必要があります。パラメータに `params` を付けると、`M(1, 2, 3)` のように、値を並べて書くだけで呼び出せるようになります。このように、引数の数を自由に変えられるパラメータを **可変長引数** といいます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `params` を付けたパラメータで、いくつでも引数を受け取れる
- `M(1, 2, 3)` が、`M(new int[] { 1, 2, 3 })` に変換されることを説明できる
- 引数を 1 つも渡さないと、長さ 0 の配列が渡されることを説明できる
- `params` のパラメータの決まり（最後に 1 つだけ）を説明できる

## 前提知識

- [省略可能パラメータと名前付き引数](/unity-csharp-learning/csharp/optional-named-params/) を読んでいること
- [配列の基礎](/unity-csharp-learning/csharp/arrays/) を読んでいること

---

## 1. 配列のパラメータ

配列を受け取るメソッドを呼び出すには、呼び出す側で配列を作って渡します。いくつかの値を渡したいだけでも、`new int[] { ... }` と書く必要があります。

```csharp
A a = new A();
a.Sum(new int[] { 1, 2, 3 });

class A
{
    public void Sum(int[] values)
    {
        int total = 0;
        foreach (int value in values)
        {
            total += value;
        }
        Console.WriteLine($"{values.Length} 個の合計: {total}");
    }
}
```

```
3 個の合計: 6
```

---

## 2. params で値を並べて渡す

配列のパラメータに [params](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/method-parameters#params-modifier) を付けると、呼び出す側は、値を `,` で区切って並べるだけで渡せます。メソッドの中では、受け取った値を配列として使えます。

**書式：[params パラメータ](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/method-parameters#params-modifier)**
```
戻り値の型 メソッド名(params 要素の型[] パラメータ名)
```

| 要素 | 説明 |
|---|---|
| `params` | 引数をいくつでも受け取れることを表す |
| `要素の型[]` | 受け取った値をまとめて入れる配列の型 |

```csharp
A a = new A();
a.Sum(1, 2, 3);
a.Sum(10, 20);

class A
{
    public void Sum(params int[] values)
    {
        int total = 0;
        foreach (int value in values)
        {
            total += value;
        }
        Console.WriteLine($"{values.Length} 個の合計: {total}");
    }
}
```

```
3 個の合計: 6
2 個の合計: 30
```

---

## 3. コンパイラーによる変換

`a.Sum(1, 2, 3)` と書くと、コンパイラーはこれを `a.Sum(new int[] { 1, 2, 3 })` に変換して呼び出します。そのため、`params` のメソッドに、配列をそのまま渡すこともできます。どちらの書き方でも、メソッドは同じように配列を受け取ります。

```csharp
A a = new A();
a.Count(1, 2, 3);
a.Count(new int[] { 1, 2 });

class A
{
    public void Count(params int[] values)
    {
        Console.WriteLine($"要素の数: {values.Length}");
    }
}
```

```
要素の数: 3
要素の数: 2
```

### 引数を 1 つも渡さない

`params` のメソッドは、引数を 1 つも渡さずに呼び出すこともできます。このとき、メソッドには長さ 0 の配列が渡されます。`null` ではないので、`values.Length` をそのまま使えます。

```csharp
A a = new A();
a.Count();

class A
{
    public void Count(params int[] values)
    {
        Console.WriteLine($"要素の数: {values.Length}");
    }
}
```

```
要素の数: 0
```

---

## 4. ほかのパラメータと組み合わせる

`params` のパラメータの前に、ふつうのパラメータを置けます。前から順にふつうのパラメータに割り当てられ、残りの引数がすべて `params` の配列に入ります。

```csharp
A a = new A();
a.Log("INFO", "開始", "読み込み完了");
a.Log("WARN", "容量が少ない");

class A
{
    public void Log(string level, params string[] messages)
    {
        foreach (string message in messages)
        {
            Console.WriteLine($"[{level}] {message}");
        }
    }
}
```

```
[INFO] 開始
[INFO] 読み込み完了
[WARN] 容量が少ない
```

### params object[] で違う型の値をまとめる

`object` は、どの型の値でも入れられる型です。`params object[]` にすると、`int`・`string`・`bool` のように、違う型の値をまとめて渡せます。

```csharp
A a = new A();
a.Show(1, "text", true);

class A
{
    public void Show(params object[] values)
    {
        Console.WriteLine($"要素の数: {values.Length}");
        foreach (object value in values)
        {
            Console.WriteLine(value);
        }
    }
}
```

```
要素の数: 3
1
text
True
```

`int` や `bool` の値を `object` の配列に入れるときは、**ボクシング** という変換が行われます。ここでは、どの型の値も `object` として扱える形に変換される、と理解しておけば十分です。詳しくは [ボクシングとアンボクシング](/unity-csharp-learning/csharp/boxing/) で学びます。

---

## よくあるミス

### params のパラメータを最後以外に置く

```csharp
// ❌ NG: params のパラメータの後ろに、別のパラメータがある
// void M(params int[] values, string label)
// {
// }  // CS0231
```

`params` の配列には、残りの引数がすべて入ります。後ろに別のパラメータがあると、どこまでが配列の値なのかが決まらないので、`params` のパラメータは最後に置きます。

### params のパラメータを 2 つ以上書く

```csharp
// ❌ NG: params のパラメータは 1 つだけ
// void M(params int[] a, params int[] b)
// {
// }  // CS0231
```

`params` のパラメータは、1 つのメソッドに 1 つだけ書けます。

---

## ワンポイントアドバイス

### 配列以外にも params を付けられる（C# 13 以降）

C# 12 までは、`params` を付けられるのは配列のパラメータだけでした。C# 13 以降は、`List<int>` のような、配列以外のコレクションの型のパラメータにも `params` を付けられます。

---

## まとめ

- 配列のパラメータに `params` を付けると、値を並べて書くだけで呼び出せる
- コンパイラーは、`M(1, 2, 3)` を `M(new int[] { 1, 2, 3 })` に変換する。配列をそのまま渡すこともできる
- 引数を 1 つも渡さないと、長さ 0 の配列が渡される
- `params` のパラメータは、パラメータの並びの最後に 1 つだけ書ける
- `params object[]` にすると、違う型の値をまとめて渡せる

---

## 理解度チェック

1. `int[]` のパラメータと `params int[]` のパラメータでは、呼び出し側の書き方がどう違いますか？
2. `a.M(1, 2, 3)` と書いたとき、コンパイラーはどのような呼び出しに変換しますか？
3. 次のコードを実行すると何が出力されますか？

   ```csharp
   A a = new A();
   a.M(10, 20, 30);
   a.M();
   a.M(new int[] { 5, 6 });

   class A
   {
       public void M(params int[] values)
       {
           Console.WriteLine($"A.M: count={values.Length}");
       }
   }
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. `int[]` のパラメータでは、`M(new int[] { 1, 2, 3 })` のように、呼び出す側で配列を作って渡します。`params int[]` のパラメータでは、`M(1, 2, 3)` のように、値を並べて渡せます。
2. `a.M(new int[] { 1, 2, 3 })` に変換されます。コンパイラーが、引数の値から配列を作ります。
3. 次のように出力されます。引数を渡さない `a.M()` では、長さ 0 の配列が渡されます。

   ```
   A.M: count=3
   A.M: count=0
   A.M: count=2
   ```

</details>

---

## 次のステップ

[オーバーロード解決](/unity-csharp-learning/csharp/overload-resolution/) では、同じ名前のメソッドが複数あるとき、どのメソッドが呼び出されるかの決まりを学びます。
