---
layout: page
title: protected 修飾子
permalink: /csharp/protected-modifier/
---

# protected 修飾子

`public` と `private` は「どこからでも」と「同じクラスだけ」の 2 択でした。継承を使うと、**派生クラスには見せたいが外部には隠したい**メンバーが出てきます。この用途に使うのが `protected` です。

## 学習目標

- `protected` のアクセス範囲を説明できる
- `private` / `public` / `protected` の 3 つを対比できる
- 継承チェーンを通じた `protected` の到達範囲を確認できる
- `internal` の意味を説明できる

## 前提知識

- [型変換と型チェック](/unity-csharp-learning/csharp/type-casting/) を読んでいること

---

## 1. アクセス修飾子の一覧

| 修飾子 | アクセス可能な範囲 |
|---|---|
| `public` | どこからでもアクセスできる |
| `private` | 同じクラス内のみ |
| `protected` | 同じクラス内、および派生クラス内 |
| `internal` | 同じアセンブリ（プロジェクト）内 |

`private` と `protected` の違いは「**派生クラスからアクセスできるかどうか**」です。

---

## 2. `protected` の基本

**書式：`protected` メンバーの定義**
```
protected 型 メンバー名;
```

```csharp
class A
{
    protected void M() { Console.WriteLine("A.M"); }
}

class B : A
{
    public void N()
    {
        M();    // ✅ 派生クラスから呼べる
    }
}
```

`A` の外側から `M()` を直接呼ぶことはできません。

```csharp
A a = new A();
// ❌ コンパイルエラー: protected メンバーは外部からアクセスできない
// a.M();
```

---

## 3. 継承チェーンでの到達範囲

`protected` メンバーは継承チェーン全体を通じてアクセスできます。

```csharp
class A
{
    protected void M() { Console.WriteLine("A.M"); }
}

class B : A
{
    public void CallM()
    {
        M();    // ✅ B から A の protected メンバーにアクセスできる
    }
}

class C : B
{
    public void CallM2()
    {
        M();    // ✅ C からも A の protected メンバーにアクセスできる
    }
}
```

```
A.M
A.M
```

---

## 4. `internal` について

`internal` は同じアセンブリ（同一プロジェクト）内であればどこからでもアクセスできます。

```csharp
internal class A
{
    internal void M() { }
}
```

Unity のスクリプトでは通常 1 つのアセンブリ内で開発するため、`internal` の効果は `public` に近くなります。複数アセンブリ（DLL 分割・パッケージ分割）をまたぐ場合に意味を持ちます。

---

## よくあるミス

```csharp
class A
{
    protected void M() { }
}

// ❌ NG: 継承関係にない外部クラスからは呼べない
class X
{
    void Test()
    {
        A a = new A();
        // a.M();  // コンパイルエラー
    }
}

// ✅ OK: 派生クラスの内側から呼ぶ
class B : A
{
    void Test()
    {
        M();    // 呼べる
    }
}
```

---

## まとめ

- `protected` は「同じクラス内と派生クラス内」からアクセス可能
- `private` は「同じクラス内のみ」。派生クラスからも呼べない
- `protected` は継承チェーン全体を通じて有効
- `internal` は同じアセンブリ内からアクセス可能

---

## 理解度チェック

1. `private` と `protected` の違いを一文で説明してください。
2. 次のコードはコンパイルできますか？

   ```csharp
   class A
   {
       protected void M() { }
   }

   A a = new A();
   a.M();
   ```

3. `class C : B` のとき、`B` が `A` を継承しており `A` に `protected void M()` があります。`C` から `M()` は呼べますか？

<details markdown="1">
<summary>解答を見る</summary>

1. `private` は同じクラス内のみ。`protected` は同じクラス内と派生クラス内からアクセスできます。
2. できません。`protected` メンバーはクラスの外部から直接呼べません。
3. 呼べます。`protected` は継承チェーン全体に届きます。

</details>

---

## 次のステップ

[オーバーライドとポリモーフィズム](/unity-csharp-learning/csharp/polymorphism/) では、`virtual`・`override` を使って派生クラスでメソッドの動作を変える仕組みを学びます。
