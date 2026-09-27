---
layout: page
title: メソッドの隠ぺいと sealed
permalink: /csharp/method-hiding/
---

# メソッドの隠ぺいと sealed

`override` は、基底クラスのメソッドを書き換える仕組みでした。これとは別に、基底クラスのメソッドを書き換えずに、同じ名前の別のメソッドで **隠す** `new` 修飾子があります。また、`sealed` を使うと、それ以上の継承やオーバーライドを禁止できます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `new` 修飾子で、基底クラスのメソッドを隠せる
- 基底クラスの型の変数から呼び出したとき、`new` と `override` で結果が違うことを説明できる
- `sealed class` で、クラスの継承を禁止できる
- `sealed override` で、それ以上のオーバーライドを禁止できる

## 前提知識

- [オーバーライドとポリモーフィズム](/unity-csharp-learning/csharp/polymorphism/) を読んでいること

---

## 1. new 修飾子でメソッドを隠す

派生クラスで、基底クラスのメソッドと同じシグネチャのメソッドを定義し、[new 修飾子](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/new-modifier) を付けると、基底クラスのメソッドを **隠す** ことになります。インスタンスを作る `new` 演算子とは、同じキーワードですが別の機能です。

**書式：[new 修飾子](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/new-modifier)**
```
アクセス修飾子 new 戻り値の型 メソッド名(パラメータ)
{
    // 処理
}
```

```csharp
B b = new B();
b.M();

class A
{
    public void M()
    {
        Console.WriteLine("A.M");
    }
}

class B : A
{
    public new void M()
    {
        Console.WriteLine("B.M");
    }
}
```

```
B.M
```

`B` の型の変数から呼び出すと、`B` の `M` が実行されます。ここまでは、オーバーライドと同じように見えます。

---

## 2. new と override の違い

違いが表れるのは、**基底クラスの型の変数から呼び出したとき** です。

```csharp
A x = new B();
x.M();

A y = new C();
y.M();

C z = new C();
z.M();

class A
{
    public virtual void M()
    {
        Console.WriteLine("A.M");
    }
}

class B : A
{
    public override void M()
    {
        Console.WriteLine("B.M（override）");
    }
}

class C : A
{
    public new void M()
    {
        Console.WriteLine("C.M（new）");
    }
}
```

```
B.M（override）
A.M
C.M（new）
```

- `B` の `M` はオーバーライドなので、`A` の型の変数 `x` から呼び出しても、実体の `B` の `M` が実行されます
- `C` の `M` は `A` の `M` を隠しているだけなので、`A` の型の変数 `y` から呼び出すと、`A` の `M` が実行されます。`C` の型の変数 `z` から呼び出したときだけ、`C` の `M` が実行されます

| 変数の型 | 実体 | `override` の場合 | `new` の場合 |
|---|---|---|---|
| 基底クラス | 派生クラス | 派生クラスのメソッド | 基底クラスのメソッド |
| 派生クラス | 派生クラス | 派生クラスのメソッド | 派生クラスのメソッド |

`override` では実体の型でメソッドが決まり、`new` では変数の型でメソッドが決まります。ポリモーフィズムを使いたいなら `override` を使います。`new` は、基底クラスを変更できない事情があるときなどに限って使います。

---

## 3. sealed class

クラスに [sealed](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/sealed) を付けると、そのクラスを継承できなくなります。

**書式：[sealed class](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/sealed)**
```
sealed class クラス名
{
    // メンバー
}
```

```csharp
Settings s = new Settings();
s.Show();

sealed class Settings
{
    public void Show()
    {
        Console.WriteLine("Settings.Show");
    }
}
```

```
Settings.Show
```

```csharp
// ❌ NG: sealed のクラスは継承できない
// class MySettings : Settings { }  // CS0509
```

継承されることを想定していないクラスに `sealed` を付けると、意図しない派生クラスが作られるのを防げます。.NET の `string` も `sealed` のクラスです。

---

## 4. sealed override

`override` に `sealed` を付けると、そのメソッドを、さらに派生したクラスでオーバーライドできなくなります。

**書式：[sealed override](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/sealed)**
```
アクセス修飾子 sealed override 戻り値の型 メソッド名(パラメータ)
{
    // 処理
}
```

```csharp
A a = new C();
a.M();

class A
{
    public virtual void M()
    {
        Console.WriteLine("A.M");
    }
}

class B : A
{
    public sealed override void M()
    {
        Console.WriteLine("B.M");
    }
}

class C : B
{
}
```

```
B.M
```

`C` は `B` を継承できますが、`B` の `M` は `sealed override` なので、`C` で `M` をオーバーライドすることはできません。

```csharp
// ❌ NG: sealed override のメソッドは、それ以上オーバーライドできない
// class C : B
// {
//     public override void M() { }  // CS0239
// }
```

---

## よくあるミス

### new を付けずに同じ名前のメソッドを定義する

```csharp
B b = new B();
b.M();

class A
{
    public void M()
    {
        Console.WriteLine("A.M");
    }
}

class B : A
{
    public void M()
    {
        Console.WriteLine("B.M");
    }
}
```

```
B.M
```

`new` を付けなくても、基底クラスのメソッドは隠されます。ただし、コンパイラーは「意図して隠すなら `new` を付けること」という警告（CS0108）を出します。基底クラスに同じ名前のメソッドがあることに気付かずに定義してしまった可能性があるからです。オーバーライドしたいのか、隠したいのかを決め、`override` か `new` を明示します。

---

## まとめ

- `new` 修飾子は、基底クラスのメソッドを隠す。基底クラスの型の変数から呼び出すと、基底クラスのメソッドが実行される
- `override` は、基底クラスのメソッドを書き換える。どの型の変数から呼び出しても、実体の型のメソッドが実行される
- `sealed class` は、継承できないクラスになる
- `sealed override` のメソッドは、それ以上オーバーライドできない
- 同じシグネチャのメソッドを定義するときは、`override` か `new` を明示する

---

## 理解度チェック

1. `new` で定義したメソッドを、基底クラスの型の変数から呼び出すと、どのメソッドが実行されますか？
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   A a = new B();
   B b = new B();
   a.M();
   b.M();
   a.N();
   b.N();

   class A
   {
       public void M() { Console.WriteLine("A.M"); }
       public virtual void N() { Console.WriteLine("A.N"); }
   }

   class B : A
   {
       public new void M() { Console.WriteLine("B.M"); }
       public override void N() { Console.WriteLine("B.N"); }
   }
   ```

3. `sealed class` とは、どのようなクラスですか？

<details markdown="1">
<summary>解答を見る</summary>

1. 基底クラスのメソッドが実行されます。`new` では、変数の型によって実行されるメソッドが決まります。
2. 次のように出力されます。`M` は `new` なので変数の型で、`N` は `override` なので実体の型でメソッドが決まります。

   ```
   A.M
   B.M
   B.N
   B.N
   ```

3. 継承できないクラスです。`sealed` のクラスを基底クラスにしようとすると、コンパイルエラー（CS0509）になります。

</details>

---

## 次のステップ

[抽象クラスと抽象メソッド](/unity-csharp-learning/csharp/abstract-classes/) では、インスタンスを作れず、派生クラスにメソッドの定義を強制するクラスを学びます。
