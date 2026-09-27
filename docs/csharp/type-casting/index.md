---
layout: page
title: 型変換と型チェック
permalink: /csharp/type-casting/
---

# 型変換と型チェック

継承の関係にあるクラスどうしでは、派生クラスのインスタンスを基底クラスの型の変数に入れたり、その逆に戻したりできます。基底クラスへの変換を **アップキャスト**、派生クラスへの変換を **ダウンキャスト** といいます。このページでは、2 つの変換の違いと、変換できるかどうかを調べる `is` と `as` を学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- アップキャストが暗黙的にできる理由を説明できる
- ダウンキャストにキャストの記述が必要な理由と、失敗したときに起きることを説明できる
- `is` で、インスタンスの型を調べられる
- `as` で変換し、失敗したときに `null` を受け取れる
- `is 型 変数名` のパターンで、型を調べると同時に変数に取り出せる

## 前提知識

- [継承](/unity-csharp-learning/csharp/inheritance/) を読んでいること
- [プリミティブ型と型変換](/unity-csharp-learning/csharp/primitive-types/) を読んでいること

---

## 1. アップキャスト

派生クラスのインスタンスを、基底クラスの型の変数に入れることを **アップキャスト** といいます。キャストを書かなくても、暗黙的に変換されます。

```csharp
B b = new B();
A a = b;
a.M();

class A
{
    public void M()
    {
        Console.WriteLine("A.M");
    }
}

class B : A
{
    public void N()
    {
        Console.WriteLine("B.N");
    }
}
```

```
A.M
```

`B` は `A` を継承しているので、`A` のメンバーをすべて持っています。`B` のインスタンスは、いつでも `A` として扱えるので、変換に失敗することはありません。そのため、暗黙的に変換できます。

ただし、`A` の型の変数からは、`A` のメンバーしか使えません。インスタンスの実体は `B` でも、`a.N()` とは書けません。

```csharp
// ❌ NG: A の型の変数からは、B で追加したメンバーは使えない
// A a = new B();
// a.N();  // CS1061
```

---

## 2. ダウンキャスト

基底クラスの型の変数を、派生クラスの型に変換することを **ダウンキャスト** といいます。ダウンキャストは、[キャスト式](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/type-testing-and-cast#cast-expression) で明示的に書く必要があります。

**書式：[キャスト式](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/type-testing-and-cast#cast-expression)**
```
(派生クラスの型)式
```

```csharp
A a = new B();
B b = (B)a;
b.N();

class A { }

class B : A
{
    public void N()
    {
        Console.WriteLine("B.N");
    }
}
```

```
B.N
```

`A` の型の変数には、`A` のインスタンスも、`B` のインスタンスも、`A` を継承した別のクラスのインスタンスも入ります。実体が `B` とは限らないので、変換に失敗することがあります。そのため、キャストを書いて「実体は `B` のはず」と明示する必要があります。

実体が変換先の型でないと、実行したときに [InvalidCastException](https://learn.microsoft.com/dotnet/api/system.invalidcastexception) が発生して、プログラムが止まります。

```csharp
// ❌ NG: a の実体は B なので、C には変換できない
// A a = new B();
// C c = (C)a;  // InvalidCastException
//
// class A { }
// class B : A { }
// class C : A { }
```

---

## 3. is — 型を調べる

[is 演算子](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/type-testing-and-cast#the-is-operator) は、式の値が、指定した型として扱えるかを調べ、`bool` で返します。

**書式：[is 演算子](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/type-testing-and-cast#the-is-operator)**
```
式 is 型
```

```csharp
A a1 = new B();
A a2 = new A();

Console.WriteLine(a1 is B);
Console.WriteLine(a2 is B);
Console.WriteLine(a1 is A);

class A { }
class B : A { }
```

```
True
False
True
```

`a1` の実体は `B` なので `a1 is B` は `True`、`a2` の実体は `A` なので `False` です。`B` は `A` を継承しているので、`a1 is A` も `True` になります。

`is` で調べてからダウンキャストすれば、`InvalidCastException` を避けられます。

---

## 4. as — 失敗したら null にする

[as 演算子](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/type-testing-and-cast#the-as-operator) は、式の値を指定した型に変換します。変換できないときは、例外を発生させずに `null` を返します。

**書式：[as 演算子](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/type-testing-and-cast#the-as-operator)**
```
式 as 型
```

```csharp
A a1 = new B();
A a2 = new A();

B? b1 = a1 as B;
B? b2 = a2 as B;

Console.WriteLine(b1 == null);
Console.WriteLine(b2 == null);

class A { }
class B : A { }
```

```
False
True
```

`a2` の実体は `A` なので、`B` には変換できず、`b2` は `null` になります。`as` の結果は `null` かもしれないので、変数の型を `B?` にしています（`?` については [値型と参照型](/unity-csharp-learning/csharp/value-reference-types/) を参照）。`as` の結果を使う前には、`null` でないことを確かめます。

---

## 5. 型パターン — 調べて取り出す

`is` の後に型と変数名を書くと、型を調べると同時に、変換した値を変数に取り出せます。これを **型パターン** といいます。`is` で調べてからキャストする処理を、1 つにまとめて書けます。

**書式：[型パターン](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/patterns)**
```
式 is 型 変数名
```

```csharp
A[] items = { new A(), new B(), new C() };

foreach (A item in items)
{
    if (item is B b)
    {
        b.N();
    }
    else
    {
        Console.WriteLine($"{item.GetType()} は B ではない");
    }
}

class A { }

class B : A
{
    public void N()
    {
        Console.WriteLine("B.N");
    }
}

class C : A { }
```

```
A は B ではない
B.N
C は B ではない
```

`item is B b` は、`item` が `B` として扱えるときに `true` になり、変換した値が変数 `b` に入ります。`if` の中では、`b` を `B` の型として使えます。

---

## よくあるミス

### as の結果を確かめずに使う

```csharp
// ❌ NG: as の結果が null かもしれないのに、そのまま使っている
// A a = new A();
// B? b = a as B;
// b.N();  // NullReferenceException（コンパイラーも警告 CS8602 を出す）
```

`as` は、変換できないときに `null` を返します。`null` のまま `b.N()` を呼び出すと、`NullReferenceException` が発生します。`if (b != null)` で確かめるか、5 節の型パターンを使います。

### 型パターンの変数を if の外で使う

```csharp
// ❌ NG: if の外では、b に値が入っているとは限らない
// A a = new A();
// if (a is B b)
// {
// }
// Console.WriteLine(b);  // CS0165
```

型パターンで宣言した変数のスコープは、`if` 文を含むブロック全体です。しかし、値が必ず入っているのは、条件が `true` になった `if` の中だけです。`if` の外で使うと、値が入っていない変数を使ったとしてコンパイルエラーになります。

---

## まとめ

- アップキャスト（派生クラス → 基底クラス）は、失敗しないので暗黙的にできる。基底クラスの型の変数からは、基底クラスのメンバーしか使えない
- ダウンキャスト（基底クラス → 派生クラス）は `(型)式` で明示的に書く。実体が違うと `InvalidCastException` が発生する
- `is` は、型として扱えるかを `bool` で返す
- `as` は変換した値を返し、変換できないときは `null` を返す
- `式 is 型 変数名` の型パターンで、型を調べると同時に、変換した値を変数に取り出せる

---

## 理解度チェック

1. アップキャストに、キャストの記述が必要ないのはなぜですか？
2. `(B)a` と `a as B` は、変換できないときの動作がどう違いますか？
3. 次のコードを実行すると何が出力されますか？

   ```csharp
   A[] items = { new B(), new A(), new B() };
   int count = 0;

   foreach (A item in items)
   {
       if (item is B)
       {
           count++;
       }
   }
   Console.WriteLine(count);
   Console.WriteLine(items[1] as B == null);

   class A { }
   class B : A { }
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. 派生クラスは基底クラスのメンバーをすべて持っているので、派生クラスのインスタンスは、いつでも基底クラスとして扱えるからです。変換に失敗することがありません。
2. `(B)a` は、変換できないと `InvalidCastException` が発生します。`a as B` は、変換できないと `null` を返します。
3. 次のように出力されます。実体が `B` の要素は 2 つです。`items[1]` の実体は `A` なので、`as B` は `null` になります。

   ```
   2
   True
   ```

</details>

---

## 次のステップ

[protected 修飾子](/unity-csharp-learning/csharp/protected-modifier/) では、派生クラスからは使えて、クラスの外からは使えないメンバーを作る方法を学びます。
