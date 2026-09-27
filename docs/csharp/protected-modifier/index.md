---
layout: page
title: protected 修飾子
permalink: /csharp/protected-modifier/
---

# protected 修飾子

[アクセス修飾子](/unity-csharp-learning/csharp/access-modifiers/) では、どこからでも使える `public` と、同じクラスの中からだけ使える `private` を学びました。継承を使うと、「派生クラスからは使いたいが、クラスの外には公開したくない」メンバーが出てきます。このときに使うのが `protected` です。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `protected` のメンバーを使える範囲を説明できる
- `public`・`private`・`protected` の違いを比べられる
- 継承を重ねても、`protected` のメンバーを派生クラスから使えることを確かめられる
- `internal` の意味を説明できる

## 前提知識

- [継承](/unity-csharp-learning/csharp/inheritance/) を読んでいること
- [アクセス修飾子](/unity-csharp-learning/csharp/access-modifiers/) を読んでいること

---

## 1. アクセス修飾子の比較

| 修飾子 | 使える範囲 |
|---|---|
| `public` | どこからでも |
| `private` | 同じクラスの中からだけ |
| [protected](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/protected) | 同じクラスの中と、派生クラスの中から |
| [internal](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/internal) | 同じアセンブリ（同じプロジェクトからビルドされたプログラム）の中から |

`private` と `protected` の違いは、**派生クラスの中から使えるかどうか** です。どちらも、クラスの外からは使えません。

---

## 2. protected のメンバー

**書式：[protected のメンバー](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/protected)**
```
protected 型 フィールド名;
protected 戻り値の型 メソッド名(パラメータ)
```

基底クラス `Character` の HP を、派生クラスからは変更できて、クラスの外からは変更できないようにします。

```csharp
Player p = new Player("Alice", 100);
p.Rest();
Console.WriteLine($"{p.Name}: HP={p.GetHp()}");

class Character
{
    public string Name { get; }
    protected int Hp;

    public Character(string name, int hp)
    {
        Name = name;
        Hp = hp;
    }

    public int GetHp()
    {
        return Hp;
    }

    protected void Log(string message)
    {
        Console.WriteLine($"[{Name}] {message}");
    }
}

class Player : Character
{
    public Player(string name, int hp) : base(name, hp)
    {
    }

    public void Rest()
    {
        Hp += 10;
        Log("休憩して HP が 10 回復した");
    }
}
```

```
[Alice] 休憩して HP が 10 回復した
Alice: HP=110
```

`Hp` と `Log` は `protected` なので、派生クラス `Player` の `Rest` メソッドから使えます。クラスの外から `p.Hp` や `p.Log(...)` と書くと、コンパイルエラーになります。

```csharp
// ❌ NG: protected のメンバーは、クラスの外からは使えない
// p.Hp = 999;  // CS0122
```

---

## 3. 継承を重ねた場合

`protected` のメンバーは、派生クラスのさらに派生クラスからも使えます。

```csharp
C c = new C();
c.CallFromB();
c.CallFromC();

class A
{
    protected void M()
    {
        Console.WriteLine("A.M");
    }
}

class B : A
{
    public void CallFromB()
    {
        M();
    }
}

class C : B
{
    public void CallFromC()
    {
        M();
    }
}
```

```
A.M
A.M
```

`C` は `A` を直接継承していませんが、`B` を通して `A` を継承しているので、`A` の `protected` のメンバーを使えます。

---

## 4. internal

`internal` を付けたメンバーやクラスは、同じアセンブリの中なら、どこからでも使えます。アセンブリは、1 つのプロジェクトをビルドしてできるプログラム（`.dll` や `.exe`）のことです。

```csharp
Tool t = new Tool();
t.Use();

internal class Tool
{
    internal void Use()
    {
        Console.WriteLine("Tool.Use");
    }
}
```

```
Tool.Use
```

1 つのプロジェクトだけで開発しているときは、`internal` の効果は `public` とほとんど変わりません。複数のプロジェクトに分けて開発するとき、プロジェクトの外には公開したくないクラスやメンバーに使います。なお、アクセス修飾子を書かずに定義したクラスは、`internal` になります。

---

## よくあるミス

### 派生クラスの中で、基底クラスの型の別のインスタンスの protected メンバーを使う

```csharp
// ❌ NG: 基底クラス A の型の変数を通して、protected のメンバーは使えない
// class A
// {
//     protected void M() { }
// }
//
// class B : A
// {
//     public void Test(A other)
//     {
//         other.M();  // CS1540
//     }
// }
```

派生クラス `B` の中でも、`protected` のメンバーを使えるのは、自分自身（`M()` や `this.M()`）か、`B` の型の変数を通したときだけです。`A` の型の変数 `other` の実体は、`B` とは関係のない、`A` を継承した別のクラスかもしれないからです。

---

## まとめ

- `protected` のメンバーは、同じクラスの中と、派生クラスの中から使える。クラスの外からは使えない
- `private` は派生クラスからも使えず、`protected` は派生クラスから使える
- `protected` のメンバーは、継承を何段重ねても、派生クラスから使える
- `internal` のメンバーやクラスは、同じアセンブリの中から使える

---

## 理解度チェック

1. `private` と `protected` の違いを説明してください。
2. 次のコードはコンパイルできますか？

   ```csharp
   A a = new A();
   a.M();

   class A
   {
       protected void M() { }
   }
   ```

3. `B` が `A` を継承し、`C` が `B` を継承しています。`A` に `protected void M()` があるとき、`C` のメソッドから `M()` を呼び出せますか？

<details markdown="1">
<summary>解答を見る</summary>

1. どちらもクラスの外からは使えませんが、`private` は同じクラスの中からだけ、`protected` は同じクラスの中と派生クラスの中から使えます。
2. できません（CS0122）。`protected` のメンバーは、クラスの外からは使えません。
3. 呼び出せます。`C` は `B` を通して `A` を継承しているので、`A` の `protected` のメンバーを使えます。

</details>

---

## 次のステップ

[オーバーライドとポリモーフィズム](/unity-csharp-learning/csharp/polymorphism/) では、`virtual` と `override` で、派生クラスごとにメソッドの動作を変える仕組みを学びます。
