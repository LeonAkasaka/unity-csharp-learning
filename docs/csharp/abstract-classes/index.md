---
layout: page
title: 抽象クラスと抽象メソッド
permalink: /csharp/abstract-classes/
---

# 抽象クラスと抽象メソッド

**抽象クラス**は、インスタンス化を禁止し、派生クラスの共通の基底として使うためのクラスです。**抽象メソッド**は、本体を持たず派生クラスで必ずオーバーライドさせるメソッドです。

## 学習目標

- `abstract class` が直接インスタンス化できない理由を説明できる
- `abstract` メソッドが本体を持たない理由を説明できる
- 派生クラスでの `override` が必須であることを確認できる
- `virtual` との違いを説明できる

## 前提知識

- [メソッドの隠ぺいと sealed](/unity-csharp-learning/csharp/method-hiding/) を読んでいること

---

## 1. `abstract class`

**書式：抽象クラスの定義**
```
abstract class クラス名
{
}
```

| 要素 | 説明 |
|---|---|
| `abstract` | そのクラスはインスタンス化できず、継承して使うことを示すキーワード |

`abstract class` は `new` でインスタンスを作れません。

```csharp
abstract class A { }

// ❌ コンパイルエラー: 抽象クラスはインスタンス化できない
// A a = new A();
```

非抽象の派生クラスを作ることで使えます。

```csharp
abstract class A { }

class B : A { }

B b = new B();  // ✅
```

---

## 2. `abstract` メソッド

**書式：抽象メソッドの宣言**
```
アクセス修飾子 abstract 戻り値の型 メソッド名(引数リスト);
```

| 要素 | 説明 |
|---|---|
| `abstract` | メソッドに本体がなく、派生クラスで必ず実装することを示すキーワード |
| `;` | 本体の `{ }` の代わりにセミコロンで終える |

`abstract` メソッドは本体を書きません。派生クラスが必ず `override` して本体を定義する義務を持ちます。

```csharp
abstract class A
{
    public abstract void M();
}

class B : A
{
    public override void M() { Console.WriteLine("B.M"); }    // 必須
}
```

`abstract` メソッドを実装せずに非抽象クラスを定義しようとするとコンパイルエラーになります。

```csharp
abstract class A
{
    public abstract void M();
}

// ❌ コンパイルエラー: A.M() が実装されていない
class B : A
{
}
```

---

## 3. `virtual` との比較

| 項目 | `virtual` | `abstract` |
|---|---|---|
| 本体 | 必須 | 不可 |
| 派生クラスでの `override` | 任意 | 必須 |
| 使用できる場所 | 通常クラス・抽象クラス | 抽象クラスのみ |

`virtual` は「上書きしてもよい」、`abstract` は「必ず上書きしなければならない」を表します。

```csharp
abstract class A
{
    public virtual void V() { Console.WriteLine("A.V"); }   // override は任意
    public abstract void M();                               // override は必須
}

class B : A
{
    public override void M() { Console.WriteLine("B.M"); }  // 必須なので書く
    // V() は省略してもコンパイルエラーにならない
}
```

---

## よくあるミス

```csharp
// ❌ NG: abstract メソッドに本体を書くとコンパイルエラー
abstract class A
{
    public abstract void M() { }
}

// ✅ OK: abstract メソッドはセミコロンで終える
abstract class A
{
    public abstract void M();
}
```

---

## まとめ

- `abstract class` は `new` でインスタンス化できない
- `abstract` メソッドは本体を持たず、派生クラスでの `override` が必須
- `abstract` メソッドは `abstract class` 内にしか書けない
- `virtual`（任意）と `abstract`（必須）の違いを区別して使い分ける

---

## 理解度チェック

1. `abstract class` を直接 `new` するとどうなりますか？
2. `abstract` メソッドに本体（`{ }`）を書くとどうなりますか？
3. `virtual` と `abstract` の違いを `override` の観点から説明してください。

<details markdown="1">
<summary>解答を見る</summary>

1. コンパイルエラーになります。
2. コンパイルエラーになります。`abstract` メソッドはセミコロンで終える必要があります。
3. `virtual` メソッドのオーバーライドは任意です。`abstract` メソッドのオーバーライドは必須で、実装しない非抽象派生クラスはコンパイルエラーになります。

</details>

---

## 次のステップ

[インターフェイス](/unity-csharp-learning/csharp/interfaces/) では、クラスに「できること」を宣言する別の仕組みを学びます。
