---
layout: page
title: オーバーロード解決
permalink: /csharp/overload-resolution/
---

# オーバーロード解決

同じ名前のメソッドが複数あるとき（オーバーロード）、コンパイラーは、呼び出しの引数に最も合うメソッドを 1 つ選びます。これを **オーバーロード解決**（overload resolution）といいます。どのメソッドが選ばれるかを知らないと、思っていたのとは別のメソッドが呼び出されることがあります。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- 呼び出せるメソッドの候補が、どのように絞り込まれるかを説明できる
- 型が完全に一致するメソッドや、よりよい変換で呼び出せるメソッドが選ばれることを説明できる
- `params` や省略可能パラメータのあるメソッドが、どのように扱われるかを説明できる
- 1 つに決められない呼び出しが、コンパイルエラーになることを説明できる

## 前提知識

- [メソッド](/unity-csharp-learning/csharp/methods/) を読んでいること
- [params キーワード](/unity-csharp-learning/csharp/params-keyword/) を読んでいること
- [プリミティブ型と型変換](/unity-csharp-learning/csharp/primitive-types/) を読んでいること

---

## 1. 呼び出せるメソッドの候補

コンパイラーは、まず、名前が同じで、引数の数と型から **呼び出せる** メソッドを、すべて候補として集めます。引数の型がパラメータの型と違っていても、[暗黙的な型変換](/unity-csharp-learning/csharp/primitive-types/) で変換できれば、呼び出せる候補になります。

```csharp
A a = new A();
a.M(1);
a.M(1.5);

class A
{
    public void M(int x)
    {
        Console.WriteLine($"A.M(int): x={x}");
    }

    public void M(double x)
    {
        Console.WriteLine($"A.M(double): x={x}");
    }
}
```

```
A.M(int): x=1
A.M(double): x=1.5
```

`a.M(1)` の引数 `1` は `int` です。`M(int)` はそのまま呼び出せ、`M(double)` も `int` から `double` への暗黙的な変換で呼び出せるので、どちらも候補になります。候補が複数あるときは、次の節の規則で 1 つを選びます。

`a.M(1.5)` の引数 `1.5` は `double` です。`double` から `int` へは暗黙的に変換できないので、`M(int)` は候補になりません。候補は `M(double)` だけです。

---

## 2. 最もよい候補を選ぶ

候補が複数あるときは、引数ごとに、どの候補の変換がよりよいかを比べます。

### 型が完全に一致する候補

引数の型とパラメータの型がまったく同じなら、変換は必要ありません。これが最もよい変換です。1 節の `a.M(1)` で `M(int)` が選ばれたのは、`M(int)` が `int` の引数と完全に一致するからです。

```csharp
A a = new A();
a.M(1);
a.M(1L);

class A
{
    public void M(int x)
    {
        Console.WriteLine($"A.M(int): x={x}");
    }

    public void M(long x)
    {
        Console.WriteLine($"A.M(long): x={x}");
    }
}
```

```
A.M(int): x=1
A.M(long): x=1
```

`1` は `int`、`1L` は `long` のリテラルなので、それぞれ完全に一致するメソッドが選ばれます。

### よりよい変換で呼び出せる候補

完全に一致する候補がないときは、変換先の型を比べます。2 つの変換先の型のうち、一方からもう一方へ暗黙的に変換できるなら、変換元になれるほうが **よりよい変換** です。

```csharp
A a = new A();
a.M(1);

class A
{
    public void M(long x)
    {
        Console.WriteLine($"A.M(long): x={x}");
    }

    public void M(double x)
    {
        Console.WriteLine($"A.M(double): x={x}");
    }
}
```

```
A.M(long): x=1
```

`int` の引数は、`long` にも `double` にも暗黙的に変換できます。`long` から `double` へは暗黙的に変換できますが、`double` から `long` へはできません。そのため、`long` への変換のほうがよりよい変換とされ、`M(long)` が選ばれます。範囲の狭いほう、つまり引数の型に近いほうが選ばれる、と考えるとわかりやすいでしょう。

---

## 3. params と省略可能パラメータ

### params を展開しない候補が優先される

[params キーワード](/unity-csharp-learning/csharp/params-keyword/) のメソッドは、引数を並べて呼び出すと、コンパイラーが配列を作って渡します（展開した形）。展開しなくても呼び出せる候補と、展開すれば呼び出せる候補が同じくらいよいときは、展開しない候補が選ばれます。

```csharp
A a = new A();
a.M(5);
a.M(1, 2, 3);

class A
{
    public void M(int x)
    {
        Console.WriteLine($"A.M(int): x={x}");
    }

    public void M(params int[] values)
    {
        Console.WriteLine($"A.M(params): count={values.Length}");
    }
}
```

```
A.M(int): x=5
A.M(params): count=3
```

`a.M(5)` は、`M(int)` でも、`params` を展開した `M(params int[])` でも呼び出せますが、展開しない `M(int)` が選ばれます。`a.M(1, 2, 3)` は、引数が 3 つなので `M(int)` では呼び出せず、`M(params int[])` が選ばれます。

### 既定値を使わない候補が優先される

同じように、省略可能パラメータの既定値を使わずに呼び出せる候補と、既定値を使えば呼び出せる候補が同じくらいよいときは、既定値を使わない候補が選ばれます。名前付き引数を使うと、パラメータの名前で候補を絞り込めます。

```csharp
A a = new A();
a.M(1, 2);
a.M(a: 1, b: 2);
a.M(x: 1, y: 2);

class A
{
    public void M(int x, int y)
    {
        Console.WriteLine($"A.M(x, y): x={x}, y={y}");
    }

    public void M(int a, int b, int c = 0)
    {
        Console.WriteLine($"A.M(a, b, c): a={a}, b={b}, c={c}");
    }
}
```

```
A.M(x, y): x=1, y=2
A.M(a, b, c): a=1, b=2, c=0
A.M(x, y): x=1, y=2
```

- `a.M(1, 2)` は、どちらでも呼び出せますが、既定値を使わない `M(int x, int y)` が選ばれます
- `a.M(a: 1, b: 2)` は、パラメータ `a` と `b` を持つ `M(int a, int b, int c = 0)` だけが候補になります
- `a.M(x: 1, y: 2)` は、パラメータ `x` と `y` を持つ `M(int x, int y)` だけが候補になります

---

## 4. 1 つに決められない呼び出し

最もよい候補を 1 つに決められないときは、コンパイルエラーになります。

```csharp
// ❌ NG: どちらの候補も、1 つの引数では変換がよく、もう 1 つでは悪い
// A a = new A();
// a.M(1, 2);  // CS0121
//
// class A
// {
//     public void M(int x, double y) { }
//     public void M(double x, int y) { }
// }
```

`a.M(1, 2)` の 1 つ目の引数では `M(int, double)` のほうが、2 つ目の引数では `M(double, int)` のほうがよい変換です。どちらかがすべての引数で同じかよりよい、ということがないので、1 つに決められません。このような呼び出しを、**あいまいな呼び出し** といいます。

あいまいな呼び出しは、引数をキャストして型をはっきりさせると解決できます。

```csharp
A a = new A();
a.M(1, 2.0);
a.M((double)1, 2);

class A
{
    public void M(int x, double y)
    {
        Console.WriteLine("A.M(int, double)");
    }

    public void M(double x, int y)
    {
        Console.WriteLine("A.M(double, int)");
    }
}
```

```
A.M(int, double)
A.M(double, int)
```

---

## よくあるミス

### リテラルと変数で、選ばれるメソッドが変わる

```csharp
A a = new A();
a.M(1);

int v = 1;
a.M(v);

class A
{
    public void M(uint x)
    {
        Console.WriteLine("A.M(uint)");
    }

    public void M(long x)
    {
        Console.WriteLine("A.M(long)");
    }
}
```

```
A.M(uint)
A.M(long)
```

同じ値 `1` を渡しているのに、呼び出されるメソッドが違います。

- `a.M(1)` の `1` は、値が決まっている定数です。`uint` の範囲に収まる `int` の定数は、`uint` に暗黙的に変換できるので、`M(uint)` と `M(long)` の両方が候補になります。`uint` から `long` へは暗黙的に変換できるので、`uint` への変換のほうがよりよい変換とされ、`M(uint)` が選ばれます
- `a.M(v)` の `v` は `int` の変数です。変数の値はコンパイルするときには決まらないので、`int` から `uint` へは暗黙的に変換できません（負の値かもしれないため）。候補は `M(long)` だけです

符号あり・なしや、大きさの違う整数型でオーバーロードを定義すると、このように呼び出す側の書き方によって結果が変わります。できるだけ避け、必要なら、呼び出す側でキャストして型をはっきりさせます。

---

## まとめ

- コンパイラーは、まず、呼び出せるメソッドを候補として集める。暗黙的な型変換で呼び出せるメソッドも候補になる
- 候補が複数あるときは、引数ごとに変換のよさを比べる。型が完全に一致するのが最もよく、次に、変換先の型が引数の型に近いほうがよい
- `params` を展開しない候補や、既定値を使わない候補が優先される
- 名前付き引数を使うと、パラメータの名前で候補を絞り込める
- 最もよい候補を 1 つに決められないと、あいまいな呼び出しとしてコンパイルエラー（CS0121）になる

---

## 理解度チェック

1. 次のクラスで `a.M(2);` と呼び出すと、どちらのメソッドが選ばれますか？

   ```csharp
   class A
   {
       public void M(int x) { Console.WriteLine("int"); }
       public void M(long x) { Console.WriteLine("long"); }
   }
   ```

2. 次のコードを実行すると何が出力されますか？

   ```csharp
   A a = new A();
   a.M(1);
   a.M(1.0f);

   class A
   {
       public void M(long x) { Console.WriteLine("long"); }
       public void M(double x) { Console.WriteLine("double"); }
   }
   ```

3. 次のクラスで `a.M(p: 10);` と呼び出すと、どうなりますか？

   ```csharp
   class A
   {
       public void M(int x) { Console.WriteLine($"M(int): {x}"); }
       public void M(int p, int q = 0) { Console.WriteLine($"M(p, q): p={p}, q={q}"); }
   }
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. `M(int x)` が選ばれます。`2` は `int` のリテラルなので、`M(int x)` と完全に一致します。
2. 次のように出力されます。`1`（`int`）は、`double` より引数の型に近い `long` への変換がよいので `M(long)` です。`1.0f`（`float`）は、`long` へは暗黙的に変換できないので、候補は `M(double)` だけです。

   ```
   long
   double
   ```

3. `M(int p, int q = 0)` が呼び出され、`M(p, q): p=10, q=0` が表示されます。パラメータ `p` を持つのはこのメソッドだけで、`q` は省略できるので既定値の `0` が使われます。

</details>

---

## 次のステップ

[演算子のオーバーロード](/unity-csharp-learning/csharp/operator-overloading/) では、自分で作ったクラスで `+` や `==` などの演算子を使えるようにする方法を学びます。
