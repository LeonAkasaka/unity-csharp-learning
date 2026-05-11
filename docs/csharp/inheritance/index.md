---
layout: page
title: 継承
permalink: /csharp/inheritance/
---

# 継承

**継承**は、既存のクラス（基底クラス）のメンバーを別のクラス（派生クラス）に引き継ぐ仕組みです。派生クラスは基底クラスのメンバーをそのまま使えるほか、新しいメンバーを追加できます。

## 学習目標

- `class B : A` の構文を理解できる
- 派生クラスが基底クラスのメンバーを継承することを確認できる
- `base` キーワードでコンストラクタ連鎖と基底メンバーへのアクセスができる
- C# の単一継承と `object` の位置づけを説明できる

## 前提知識

- [クラスとフィールド](/unity-csharp-learning/csharp/classes/) を読んでいること
- [コンストラクタ](/unity-csharp-learning/csharp/constructors/) を読んでいること

---

## 1. 継承の構文

**書式：継承の定義**
```
class 派生クラス名 : 基底クラス名
{
}
```

| 要素 | 説明 |
|---|---|
| `派生クラス名` | 新しく定義するクラスの名前 |
| `:` | 継承を表す記号 |
| `基底クラス名` | メンバーを引き継ぐ元のクラス名 |

```csharp
class A
{
    public void M() { Console.WriteLine("A.M"); }
}

class B : A
{
}
```

`B` は `A` を継承しています。`B` のインスタンスから `A.M()` を呼び出せます。

```csharp
B b = new B();
b.M();
```

```
A.M
```

---

## 2. 派生クラスへのメンバー追加

派生クラスには、基底クラスにないメンバーを追加できます。

```csharp
class A
{
    public void M() { Console.WriteLine("A.M"); }
}

class B : A
{
    public void N() { Console.WriteLine("B.N"); }
}
```

`B` のインスタンスは `M()` と `N()` の両方を持ちます。`A` のインスタンスは `M()` のみです。

---

## 3. `base` キーワード

`base` は、派生クラスから基底クラスのメンバーに明示的にアクセスするキーワードです。

**書式：基底クラスのメンバーへのアクセス**
```
base.メンバー名
```

**書式：コンストラクタ連鎖**
```
class 派生クラス名 : 基底クラス名
{
    public 派生クラス名(引数リスト) : base(引数リスト)
    {
    }
}
```

| 要素 | 説明 |
|---|---|
| `base.メンバー名` | 基底クラスのメンバーを呼び出す |
| `: base(...)` | 派生クラスのコンストラクタから基底クラスのコンストラクタを呼ぶ |

基底クラスにパラメーターありのコンストラクタしかない場合、派生クラスでは `: base(...)` が必須です。

```csharp
class A
{
    public int Value { get; }

    public A(int value)
    {
        Value = value;
    }
}

class B : A
{
    public B(int value) : base(value)
    {
    }
}
```

`B` のコンストラクタが呼ばれると、`: base(value)` によって先に `A` のコンストラクタが実行されます。

---

## 4. 継承チェーンと `object`

C# では、1 つのクラスが継承できる基底クラスは **1 つだけ**です（単一継承）。継承は何段でもつなげられます。

```csharp
class A { }
class B : A { }
class C : B { }
```

`class` キーワードで定義したすべてのクラスは、明示しなくても暗黙的に `object` を継承します。`object` は C# におけるすべての型の共通基底クラスです。

---

## よくあるミス

```csharp
class A
{
    public A(int value) { }
}

// ❌ NG: 基底クラスにパラメーターなしコンストラクタがないため base(...) を省略するとコンパイルエラー
class B : A
{
    public B(int value) { }
}

// ✅ OK: base(...) で基底コンストラクタを明示的に呼ぶ
class B : A
{
    public B(int value) : base(value) { }
}
```

---

## まとめ

- `class B : A` で `B` は `A` を継承する
- 派生クラスは基底クラスのメンバーをすべて引き継ぐ
- `base.メンバー名` で基底クラスのメンバーへ明示的にアクセスできる
- `: base(...)` でコンストラクタ連鎖を記述する
- C# は単一継承。すべてのクラスは `object` を暗黙的な基底クラスとして持つ

---

## 理解度チェック

1. `class B : A` における `:` は何を意味しますか？
2. 基底クラス `A` に `public A(int x)` しかないとき、派生クラス `B` のコンストラクタはどのように書きますか？
3. 次のコードはコンパイルできますか？理由とともに答えてください。

   ```csharp
   class A { }
   class B : A { }
   class C : A, B { }
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. `B` が `A` を継承することを表す記号です。
2. `public B(int x) : base(x) { }` と書きます。
3. できません。C# は単一継承のため、1 つのクラスが継承できる基底クラスは 1 つだけです。

</details>

---

## 次のステップ

[型変換と型チェック](/unity-csharp-learning/csharp/type-casting/) では、継承関係にある型の間での変換と確認方法を学びます。
