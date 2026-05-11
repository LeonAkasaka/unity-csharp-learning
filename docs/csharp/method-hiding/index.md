---
layout: page
title: メソッドの隠ぺいと sealed
permalink: /csharp/method-hiding/
---

# メソッドの隠ぺいと sealed

`override` はポリモーフィズムを利用した「上書き」です。これとは別に、基底クラスのメソッドを「隠す」`new` 修飾子と、それ以上の派生・オーバーライドを禁止する `sealed` があります。

## 学習目標

- `new` 修飾子によるメソッドの隠ぺいと `override` の違いを説明できる
- 基底クラス型変数を通じた呼び出し結果が `new` と `override` で異なることを確認できる
- `sealed class` でクラスの継承を禁止できる
- `sealed override` でメソッドのオーバーライドを禁止できる

## 前提知識

- [オーバーライドとポリモーフィズム](/unity-csharp-learning/csharp/polymorphism/) を読んでいること

---

## 1. メソッドの隠ぺい（`new` 修飾子）

派生クラスで基底クラスと同名のメソッドを定義すると、コンパイラは警告を出します。意図的に隠すことを示すキーワードが `new` 修飾子です（インスタンス生成の `new` とは別物です）。

**書式：メソッドの隠ぺい**
```
アクセス修飾子 new 戻り値の型 メソッド名(引数リスト)
{
}
```

| 要素 | 説明 |
|---|---|
| `new` | 基底クラスの同名メソッドを隠ぺいすることを明示するキーワード |

```csharp
class A
{
    public void M() { Console.WriteLine("A.M"); }
}

class B : A
{
    public new void M() { Console.WriteLine("B.M"); }
}
```

`B` のインスタンスを `B` 型で参照すると `B.M()` が呼ばれます。

```csharp
B b = new B();
b.M();
```

```
B.M
```

---

## 2. `new` と `override` の違い

**最も重要な差異は「基底クラス型の変数を通じて呼んだとき」の動作**です。

```csharp
class A
{
    public virtual void M() { Console.WriteLine("A.M"); }
}

class B : A
{
    public override void M() { Console.WriteLine("B.override.M"); }
}

class C : A
{
    public new void M() { Console.WriteLine("C.new.M"); }
}
```

```csharp
A x = new B();
x.M();      // override: 実体の型のメソッドが呼ばれる

A y = new C();
y.M();      // new: 変数の型（A）のメソッドが呼ばれる
```

```
B.override.M
A.M
```

| 変数の型 | 実体 | `override` の場合 | `new` の場合 |
|---|---|---|---|
| `A` | `B` | `B.M()` | `A.M()` |
| `B` | `B` | `B.M()` | `B.M()` |

`override` は「どの型の変数を経由しても実体のメソッドが呼ばれる」のに対し、`new` は「変数の型によって呼ばれるメソッドが変わる」という違いがあります。

---

## 3. `sealed class`

**書式：`sealed class`**
```
sealed class クラス名
{
}
```

| 要素 | 説明 |
|---|---|
| `sealed` | そのクラスをこれ以上継承できないことを示すキーワード |

```csharp
sealed class A { }

// ❌ コンパイルエラー: sealed クラスは継承できない
class B : A { }
```

`sealed` を付けることで、意図しない派生を防ぎ設計の制約をコードで表現できます。

---

## 4. `sealed override`

**書式：`sealed override`**
```
アクセス修飾子 sealed override 戻り値の型 メソッド名(引数リスト)
{
}
```

| 要素 | 説明 |
|---|---|
| `sealed override` | そのメソッドをこれ以上オーバーライドできないことを示す |

```csharp
class A
{
    public virtual void M() { Console.WriteLine("A.M"); }
}

class B : A
{
    public sealed override void M() { Console.WriteLine("B.M"); }
}

class C : B
{
    // ❌ コンパイルエラー: B.M() は sealed なのでオーバーライドできない
    // public override void M() { }
}
```

`sealed override` は継承チェーンの途中でオーバーライドをそれ以上許可しないことを表します。

---

## よくあるミス

```csharp
class A
{
    public void M() { }
}

// ⚠ 警告: A.M() を隠している。意図的なら new を付けること
class B : A
{
    public void M() { }
}

// ✅ OK: new を付けて隠ぺいを明示する
class B : A
{
    public new void M() { }
}
```

---

## まとめ

- `new` 修飾子は基底クラスのメソッドを「隠す」。動的ディスパッチは働かない
- `override` は `virtual`/`abstract` メソッドを「上書き」する。基底クラス型変数を経由しても実体のメソッドが呼ばれる
- `sealed class` でそのクラスの継承を禁止する
- `sealed override` で継承チェーンの途中でそれ以上のオーバーライドを禁止する

---

## 理解度チェック

1. `new` を使ってメソッドを定義したとき、基底クラス型の変数を通じて呼ぶとどのメソッドが実行されますか？
2. `override` で定義したとき、基底クラス型の変数を通じて呼ぶとどのメソッドが実行されますか？
3. `sealed class` とはどんなクラスですか？

<details markdown="1">
<summary>解答を見る</summary>

1. 変数の型（基底クラス）のメソッドが実行されます。
2. 実体のクラス（派生クラス）のメソッドが実行されます（動的ディスパッチ）。
3. それ以上継承できないクラスです。`sealed` を付けたクラスを基底クラスにしようとするとコンパイルエラーになります。

</details>

---

## 次のステップ

[抽象クラスと抽象メソッド](/unity-csharp-learning/csharp/abstract-classes/) では、インスタンス化を禁止し実装を強制する仕組みを学びます。
