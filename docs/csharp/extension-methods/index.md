---
layout: page
title: 拡張メソッド
permalink: /csharp/extension-methods/
---

# 拡張メソッド

既存のクラスに新しいメソッドを追加したい場面があります。しかし、対象が外部ライブラリの型や `sealed` クラスなら、クラス定義そのものを編集できないことがあります。そういうときに使うのが**拡張メソッド**です。

## 学習目標

- 拡張メソッドが必要になる場面を説明できる
- `static class` と `this` を使った定義方法を理解できる
- 通常のインスタンスメソッドと同じ形で呼び出せることを説明できる
- 拡張メソッドが `private` / `protected` メンバーにアクセスできない理由を理解できる

## 前提知識

- [static メンバーと static クラス](/unity-csharp-learning/csharp/static-members/) を読んでいること

---

## 1. 既存の型を直接編集できない場面

たとえば既存のクラスに便利なメソッドを足したくても、そのクラスの定義を変更できないことがあります。

- 外部ライブラリのクラスである
- `sealed` クラスで継承できない
- 既存コードを変更したくない

このようなとき、別の `static class` にメソッドを書くことで、元の型にメソッドが増えたように扱えます。

---

## 2. 拡張メソッドの定義

**書式：拡張メソッド**
```
static class 拡張クラス名
{
    public static 戻り値の型 メソッド名(this 対象の型 引数名, 追加引数)
    {
        処理
    }
}
```

| 要素 | 説明 |
|---|---|
| `static class` | 拡張メソッドを置くクラス。必ず static |
| `public static` | 拡張メソッド本体。必ず static |
| `this` | 第一引数が拡張対象であることを示す |
| `対象の型` | 拡張したい型 |
| `引数名` | 拡張先のインスタンスを受け取る名前 |
| `追加引数` | 必要なら第2引数以降に追加する引数 |

`this` を付けた**第一引数**が、「どの型を拡張するか」を決めます。

---

## 3. 呼び出し方

呼び出し側では、通常のインスタンスメソッドと同じように `変数.M()` と書けます。

コンパイラはこれを、内部では `AExtensions.M(a)` のような `static` メソッド呼び出しに変換します。つまり、拡張メソッドは**糖衣構文**です。

---

## 4. 実行例

```csharp
class A
{
    public int Value;
}

static class AExtensions
{
    public static void M(this A a)
    {
        Console.WriteLine($"AExtensions.M: {a.Value}");
    }

    public static void N(this A a, int x)
    {
        Console.WriteLine($"AExtensions.N: {a.Value + x}");
    }
}

var a = new A { Value = 42 };
a.M();
a.N(10);
```

```
AExtensions.M: 42
AExtensions.N: 52
```

`a.M()` と書いていますが、実体は `AExtensions.M(a)` です。`a.N(10)` の実体は `AExtensions.N(a, 10)` です。

---

## 5. 注意点

拡張メソッドは、対象クラスの内部に入り込む仕組みではありません。そのため、使えるのは対象型の **`public` メンバー** が中心です。

- `private` メンバーにはアクセスできない
- `protected` メンバーにもアクセスできない
- 既存クラスの実装そのものを書き換えるわけではない

つまり、拡張メソッドは「見た目をインスタンスメソッドに近づける書き方」であり、アクセス制御を無視する仕組みではありません。

---

## よくあるミス

### ミス①：拡張メソッドを non-static クラスに定義する

```csharp
class A
{
    public int Value;
}

// ❌ NG: 拡張メソッドを置くクラスは static 必須
// class AExtensions
// {
//     public static void M(this A a) { Console.WriteLine(a.Value); }
// }

// ✅ OK: static class に定義する
static class AExtensions
{
    public static void M(this A a) { Console.WriteLine($"AExtensions.M: {a.Value}"); }
}
```

### ミス②：対象クラスの `private` メンバーにアクセスしようとする

```csharp
class A
{
    private int _value = 42;
    public int Value { get { return _value; } }
}

static class AExtensions
{
    // ❌ NG: private メンバーにはアクセスできない
    // public static void M(this A a) { Console.WriteLine(a._value); }

    // ✅ OK: public メンバーを使う
    public static void M(this A a) { Console.WriteLine($"AExtensions.M: {a.Value}"); }
}
```

---

## まとめ

- 拡張メソッドは、既存の型にメソッドを追加したように見せる書き方
- `static class` の中に `public static 戻り値 M(this 型 引数名, ...)` と定義する
- 呼び出し側では `変数.M()` と書けるが、内部では `static` メソッド呼び出しに変換される
- 拡張メソッドは糖衣構文であり、元のクラスのアクセス制御は変えられない
- `private` / `protected` メンバーにはアクセスできない

---

## 理解度チェック

以下の問いに答えられるか確認しましょう。

1. 拡張メソッドが必要になるのはどのような場面ですか？
2. 次のコードの出力結果は何になりますか？

   ```csharp
   class A
   {
       public int Value;
   }

   static class AExtensions
   {
       public static void M(this A a)
       {
           Console.WriteLine($"AExtensions.M: {a.Value}");
       }
   }

   var a = new A { Value = 7 };
   a.M();
   ```

3. `A` の `Value` に `x` を足した結果を表示する拡張メソッド `N(this A a, int x)` を書いてください。

<details markdown="1">
<summary>解答を見る</summary>

1. 外部ライブラリの型や `sealed` クラスなど、元のクラスを直接編集できないのに便利なメソッドを追加したい場面です。
2. 
   ```
   AExtensions.M: 7
   ```
3. ```csharp
   class A
   {
       public int Value;
   }

   static class AExtensions
   {
       public static void N(this A a, int x)
       {
           Console.WriteLine($"AExtensions.N: {a.Value + x}");
       }
   }

   var a = new A { Value = 10 };
   a.N(5);
   ```

   ```
   AExtensions.N: 15
   ```

</details>

---

## 次のステップ

[継承](/unity-csharp-learning/csharp/inheritance/) では、既存クラスのメンバーを引き継いで新しいクラスを作る仕組みを学びます。
