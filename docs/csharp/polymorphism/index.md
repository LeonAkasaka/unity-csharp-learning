---
layout: page
title: オーバーライドとポリモーフィズム
permalink: /csharp/polymorphism/
---

# オーバーライドとポリモーフィズム

**オーバーライド**は、基底クラスで定義されたメソッドの動作を派生クラスで上書きする仕組みです。**ポリモーフィズム**は、基底クラス型の変数を通じてメソッドを呼んだとき、実体の型のメソッドが実行される性質です。

## 学習目標

- `virtual` メソッドが派生クラスでオーバーライド可能であることを説明できる
- `override` でメソッドを上書きできる
- 基底クラス型の変数から派生クラスの実装が呼ばれる仕組み（動的ディスパッチ）を説明できる
- `sealed` で継承・オーバーライドを禁止できる

## 前提知識

- [protected 修飾子](/unity-csharp-learning/csharp/protected-modifier/) を読んでいること

---

## 1. `virtual` メソッド

**書式：`virtual` メソッドの定義**
```
アクセス修飾子 virtual 戻り値の型 メソッド名(引数リスト)
{
}
```

| 要素 | 説明 |
|---|---|
| `virtual` | 派生クラスでオーバーライドを許可することを示すキーワード |

`virtual` を付けたメソッドは、派生クラスで `override` できます。`virtual` を付けないメソッドはオーバーライドできません。

```csharp
class A
{
    public virtual void M() { Console.WriteLine("A.M"); }
}
```

**書式：`override` メソッドの定義**
```
アクセス修飾子 override 戻り値の型 メソッド名(引数リスト)
{
}
```

| 要素 | 説明 |
|---|---|
| `override` | 基底クラスの `virtual` メソッドを上書きすることを示すキーワード |

`override` するメソッドのシグネチャ（名前・戻り値型・引数の型）は基底クラスのものと一致させます。

```csharp
class A
{
    public virtual void M() { Console.WriteLine("A.M"); }
}

class B : A
{
    public override void M() { Console.WriteLine("B.M"); }
}
```

---

## 3. ポリモーフィズム（動的ディスパッチ）

基底クラス型の変数に派生クラスのインスタンスを代入してメソッドを呼ぶと、**実体の型のメソッドが実行**されます。これを**動的ディスパッチ**と呼びます。

```csharp
class A
{
    public virtual void M() { Console.WriteLine("A.M"); }
}

class B : A
{
    public override void M() { Console.WriteLine("B.M"); }
}

A x = new B();
x.M();
```

```
B.M
```

変数の型が `A` であっても、実体が `B` であれば `B.M()` が呼ばれます。

---

## よくあるミス

```csharp
class A
{
    public void M() { }     // ❌ virtual がない
}

class B : A
{
    public override void M() { }    // ❌ コンパイルエラー: A.M() はオーバーライドできない
}

// ✅ OK
class A
{
    public virtual void M() { Console.WriteLine("A.M"); }
}

class B : A
{
    public override void M() { Console.WriteLine("B.M"); }
}
```

---

## まとめ

- `virtual` はオーバーライドを許可するキーワード
- `override` は基底クラスの `virtual` メソッドを上書きするキーワード
- 動的ディスパッチ：基底クラス型の変数から呼んでも実体の型のメソッドが実行される

---

## 理解度チェック

1. `virtual` のないメソッドを派生クラスで `override` しようとするとどうなりますか？
2. 次のコードで `x.M()` を呼んだとき、`A.M()` と `B.M()` のどちらが実行されますか？

   ```csharp
   class A { public virtual void M() { Console.WriteLine("A.M"); } }
   class B : A { public override void M() { Console.WriteLine("B.M"); } }

   A x = new B();
   x.M();
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. コンパイルエラーになります。`virtual` のないメソッドはオーバーライドできません。
2. `B.M()` が実行されます。変数の型は `A` ですが実体が `B` なので、動的ディスパッチによって `B.M()` が呼ばれます。

</details>

---

## 次のステップ

[メソッドの隠ぺいと sealed](/unity-csharp-learning/csharp/method-hiding/) では、`new` 修飾子による隠ぺいと `sealed` による拡張の制限を学びます。
