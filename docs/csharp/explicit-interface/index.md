---
layout: page
title: インターフェイスの明示的実装
permalink: /csharp/explicit-interface/
---

# インターフェイスの明示的実装

1 つのクラスで、同じ名前のメンバーを持つ 2 つのインターフェイスを実装すると、ふつうの書き方では、両方のインターフェイスに同じ実装が使われます。**明示的実装**（explicit implementation）を使うと、どのインターフェイスのメンバーを実装するのかを指定して、それぞれに別の実装を用意できます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- インターフェイスのメンバーを、明示的実装で定義できる
- 明示的実装のメンバーは、インターフェイスの型の変数からだけ呼び出せることを説明できる
- 暗黙的実装と明示的実装の違いを説明できる

## 前提知識

- [インターフェイス](/unity-csharp-learning/csharp/interfaces/) を読んでいること

---

## 1. 同じ名前のメンバーを持つインターフェイス

`IFoo` と `IBar` が、どちらも `void M()` を宣言しているとします。[インターフェイス](/unity-csharp-learning/csharp/interfaces/) で学んだ書き方で `public void M()` を 1 つ定義すると、その `M` が、`IFoo` の `M` と `IBar` の `M` の両方の実装になります。このような、ふつうの実装の書き方を **暗黙的実装** といいます。

```csharp
A a = new A();

IFoo x = a;
x.M();

IBar y = a;
y.M();

interface IFoo
{
    void M();
}

interface IBar
{
    void M();
}

class A : IFoo, IBar
{
    public void M()
    {
        Console.WriteLine("A.M");
    }
}
```

```
A.M
A.M
```

`IFoo` として呼び出しても、`IBar` として呼び出しても、同じ `A.M` が実行されます。`IFoo` の `M` と `IBar` の `M` で、別々の動作をさせたいときは、明示的実装を使います。

---

## 2. 明示的実装の書き方

**書式：[明示的実装](https://learn.microsoft.com/dotnet/csharp/programming-guide/interfaces/explicit-interface-implementation)**
```
戻り値の型 インターフェイス名.メソッド名(パラメータ)
{
    // 処理
}
```

| 要素 | 説明 |
|---|---|
| アクセス修飾子 | 書かない |
| `インターフェイス名.メソッド名` | どのインターフェイスのメンバーを実装するのかを指定する |

```csharp
A a = new A();

IFoo x = a;
x.M();

IBar y = a;
y.M();

interface IFoo
{
    void M();
}

interface IBar
{
    void M();
}

class A : IFoo, IBar
{
    void IFoo.M()
    {
        Console.WriteLine("IFoo.M");
    }

    void IBar.M()
    {
        Console.WriteLine("IBar.M");
    }
}
```

```
IFoo.M
IBar.M
```

同じインスタンス `a` でも、`IFoo` の型の変数から呼び出すと `IFoo.M` が、`IBar` の型の変数から呼び出すと `IBar.M` が実行されます。

---

## 3. 明示的実装のメンバーの呼び出し方

明示的実装のメンバーは、クラスの型の変数からは呼び出せません。インターフェイスの型の変数に入れるか、インターフェイスの型にキャストしてから呼び出します。

```csharp
A a = new A();

((IFoo)a).M();

IFoo foo = a;
foo.M();

interface IFoo
{
    void M();
}

class A : IFoo
{
    void IFoo.M()
    {
        Console.WriteLine("IFoo.M");
    }
}
```

```
IFoo.M
IFoo.M
```

```csharp
// ❌ NG: 明示的実装のメンバーは、クラスの型の変数から呼び出せない
// A a = new A();
// a.M();  // CS1061
```

この性質を利用して、インターフェイスとして使うときにだけ必要なメンバーを、クラスの型から見えないようにする目的でも、明示的実装が使われます。

---

## 4. 暗黙的実装と明示的実装の比較

| | 暗黙的実装 | 明示的実装 |
|---|---|---|
| 書き方 | `public void M() { }` | `void IFoo.M() { }` |
| アクセス修飾子 | `public` を書く | 書かない |
| クラスの型の変数から呼び出す | できる | できない |
| インターフェイスの型の変数から呼び出す | できる | できる |
| 同じ名前のメンバーを、インターフェイスごとに別の実装にする | できない | できる |

---

## よくあるミス

### 明示的実装にアクセス修飾子を付ける

```csharp
// ❌ NG: 明示的実装にはアクセス修飾子を付けられない
// class A : IFoo
// {
//     public void IFoo.M()  // CS0106
//     {
//     }
// }
```

明示的実装のメンバーは、インターフェイスの型を通してだけ呼び出せるので、アクセス修飾子は書きません。

---

## まとめ

- 明示的実装は、`戻り値の型 インターフェイス名.メソッド名() { }` の形で書く。アクセス修飾子は付けない
- 明示的実装のメンバーは、インターフェイスの型の変数（またはキャスト）を通してだけ呼び出せる
- 複数のインターフェイスに同じ名前のメンバーがあるとき、明示的実装で、それぞれに別の実装を用意できる

---

## 理解度チェック

1. 明示的実装したメンバーを、クラスの型の変数から直接呼び出そうとすると、どうなりますか？
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   Robot r = new Robot();
   ((IWalker)r).Move();
   ((ISwimmer)r).Move();
   r.Move();

   interface IWalker
   {
       void Move();
   }

   interface ISwimmer
   {
       void Move();
   }

   class Robot : IWalker, ISwimmer
   {
       public void Move() { Console.WriteLine("移動する"); }
       void ISwimmer.Move() { Console.WriteLine("泳ぐ"); }
   }
   ```

3. `IFoo` と `IBar` が同じ名前のメソッドを持ち、それぞれ別の動作にしたいときは、暗黙的実装と明示的実装のどちらを使いますか？

<details markdown="1">
<summary>解答を見る</summary>

1. コンパイルエラー（CS1061）になります。明示的実装のメンバーは、インターフェイスの型を通してだけ呼び出せます。
2. 次のように出力されます。`ISwimmer` の `Move` は明示的実装されているので `泳ぐ` です。`IWalker` の `Move` とクラスの型から呼び出した `Move` には、暗黙的実装の `public void Move()` が使われます。

   ```
   移動する
   泳ぐ
   移動する
   ```

3. 明示的実装を使います。暗黙的実装では、1 つのメソッドが両方のインターフェイスの実装になるので、区別できません。

</details>

---

## 次のステップ

これで「C# 継承と抽象化」のセクションは終わりです。[ジェネリクスの基本](/unity-csharp-learning/csharp/generics/) からは「C# ジェネリクス」のセクションに進み、型をパラメータとして受け取る、汎用的なクラスの書き方を学びます。
