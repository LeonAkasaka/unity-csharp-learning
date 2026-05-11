---
layout: page
title: params キーワード
permalink: /csharp/params-keyword/
---

# params キーワード

`void M(int[] values)` のように配列を受け取るメソッドを定義すると、呼び出し側は `M(new int[] { 1, 2, 3 })` と書かなければなりません。`params` を使うと、この呼び出しを `M(1, 2, 3)` と短く書けるようになります。

## 学習目標

- `int[]` パラメータと `params int[]` パラメータの呼び出し構文の違いを説明できる
- コンパイラが `M(1, 2, 3)` を `M(new int[] { 1, 2, 3 })` に変換することを説明できる
- `params` パラメータの制約（末尾に 1 つだけ）を説明できる
- `M()` で空配列が渡ることを説明できる
- `params object[]` を使うと異なる型の値をまとめて渡せることを説明できる

## 前提知識

- [省略可能パラメータと名前付き引数](/unity-csharp-learning/csharp/optional-named-params/)

---

## 1. 問題：配列パラメータの呼び出しが冗長になる

通常の配列パラメータでは、呼び出し側が配列を明示して作る必要があります。要素をいくつか渡したいだけでも、`new int[] { ... }` を毎回書くことになります。

```csharp
using System;

var a = new A();
a.M(new int[] { 1, 2, 3 });

class A
{
    public void M(int[] b)
    {
        Console.WriteLine($"A.M: count={b.Length}");
    }
}
```

```
A.M: count=3
```

---

## 2. `params` による解決

`params int[]` を使うと、呼び出し側は `M(1, 2, 3)` のように値をそのまま並べて書けます。メソッド側では、受け取った値を配列として扱えます。

**書式：params パラメータ**
```
戻り値の型 メソッド名(params 要素型[] パラメータ名)
{
    処理
}
```

| 要素 | 説明 |
|---|---|
| `params` | 引数を可変長で受け取れるようにするキーワード |
| `要素型[]` | 受け取る値をまとめて入れる配列型 |
| `パラメータ名` | メソッド内で配列として使う名前 |

```csharp
using System;

var a = new A();
a.M(1, 2, 3);

class A
{
    public void M(params int[] b)
    {
        Console.WriteLine($"A.M: count={b.Length}");
    }
}
```

```
A.M: count=3
```

---

## 3. コンパイラの変換

`M(1, 2, 3)` と書くと、コンパイラはこれを `M(new int[] { 1, 2, 3 })` に変換して呼び出します。そのため、`params` を使うメソッドには配列を直接渡すこともできます。どちらもメソッド側では同じ形の配列として受け取ります。

```csharp
using System;

var a = new A();
a.M(1, 2, 3);
a.M(new int[] { 1, 2 });

class A
{
    public void M(params int[] b)
    {
        Console.WriteLine($"A.M: count={b.Length}");
    }
}
```

```
A.M: count=3
A.M: count=2
```

---

## 4. 空の呼び出し

`params` では引数を 1 つも渡さない呼び出しもできます。`M()` は `new int[0]` を渡すのと同じで、メソッド側には長さ 0 の配列が渡ります。

```csharp
using System;

var a = new A();
a.M();

class A
{
    public void M(params int[] b)
    {
        Console.WriteLine($"A.M: count={b.Length}");
    }
}
```

```
A.M: count=0
```

---

## 5. `params object[]` で型混在

`object` はどの型の値でも参照できる型です。`params object[]` を使うと、`int`、`string`、`bool` など異なる型の値を 1 回の呼び出しでまとめて渡せます。

`int` や `bool` のような値型を `object` として配列に入れると、ボクシングが発生します。ここでは、値型が `object` として扱える形に変換されると理解しておけば十分です。

```csharp
using System;

var a = new A();
a.N(1, "text", true);

class A
{
    public void N(params object[] b)
    {
        Console.WriteLine($"A.N: count={b.Length}");

        foreach (var a in b)
        {
            Console.WriteLine(a);
        }
    }
}
```

```
A.N: count=3
1
text
True
```

---

## 6. 制約

- `params` パラメータは、パラメータリストの末尾に置かなければなりません。
- `params` パラメータは、1 つのメソッドに 1 つだけ定義できます。
- `params` を付けられるのは配列型のパラメータです。

---

## よくあるミス

### ミス①：`params` を末尾以外に置く

```csharp
// ❌ NG: params を末尾以外に置けない
// void M(params int[] a, int b) { }

// ✅ OK: params は末尾に置く
void M(int a, params int[] b) { Console.WriteLine($"M: a={a}, count={b.Length}"); }
```

### ミス②：`params` を複数定義する

```csharp
// ❌ NG: params は複数定義できない
// void M(params int[] a, params int[] b) { }

// ✅ OK: params は 1 つだけにする
void M(int[] a, params int[] b) { Console.WriteLine($"M: a={a.Length}, b={b.Length}"); }
```

---

## まとめ

- 通常の `int[]` パラメータでは、呼び出し側が `new int[] { ... }` を書く必要がある
- `params int[]` を使うと、`M(1, 2, 3)` のように値をそのまま並べて呼び出せる
- コンパイラは `M(1, 2, 3)` を `M(new int[] { 1, 2, 3 })` に変換する
- `M()` は長さ 0 の配列を渡す呼び出しになる
- `params` パラメータは末尾に 1 つだけ定義できる
- `params object[]` では異なる型をまとめて渡せるが、値型ではボクシングが発生する

---

## 理解度チェック

以下の問いに答えられるか確認しましょう。

1. `int[]` パラメータと `params int[]` パラメータでは、呼び出し側の書き方がどう違いますか？
2. `a.M(1, 2, 3)` と書いたとき、コンパイラは内部的にどのような呼び出しに変換しますか？
3. 次のコードの出力結果は何になりますか？

   ```csharp
   class A
   {
       public void M(params int[] a)
       {
           Console.WriteLine($"A.M: count={a.Length}");
       }
   }

   var a = new A();
   a.M(10, 20, 30);
   a.M();
   a.M(new int[] { 5, 6 });
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. `int[]` パラメータでは、呼び出し側が `M(new int[] { 1, 2, 3 })` のように配列を作って渡します。`params int[]` パラメータでは、`M(1, 2, 3)` のように値をそのまま並べて渡せます。
2. `M(new int[] { 1, 2, 3 })` に変換されます。コンパイラが引数リストから配列を生成します。
3.

   ```
   A.M: count=3
   A.M: count=0
   A.M: count=2
   ```

</details>

---

## 次のステップ

[オーバーロード解決](/unity-csharp-learning/csharp/overload-resolution/) では、`params` を含むメソッド呼び出しでどのオーバーロードが選ばれるかを学びます。
