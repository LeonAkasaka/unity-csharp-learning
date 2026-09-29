---
layout: page
title: 拡張メソッド
permalink: /csharp/extension-methods/
---

# 拡張メソッド

**拡張メソッド**（extension method）は、既存の型に、あとからメソッドを追加したように見せる書き方です。`string` や `int` のように、自分ではクラスの定義を書き換えられない型にも、`"Hello".Shout()` のような形で呼び出せるメソッドを用意できます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- static クラスと `this` を使って、拡張メソッドを定義できる
- 拡張メソッドを、インスタンスメソッドと同じ形で呼び出せる
- 拡張メソッドが、static メソッドの呼び出しに変換されることを説明できる
- 拡張メソッドから、対象の型の `private` メンバーを使えない理由を説明できる

## 前提知識

- [static メンバーと static クラス](/unity-csharp-learning/csharp/static-members/) を読んでいること

---

## 1. 型の定義を書き換えられない場面

文字列の最後に `!` を付けて大文字にする処理を、何度も使いたいとします。static メソッドとして書くと、次のようになります。

```csharp
Console.WriteLine(StringUtil.Shout("hello"));

static class StringUtil
{
    public static string Shout(string text)
    {
        return text.ToUpper() + "!";
    }
}
```

```
HELLO!
```

正しく動きますが、`"hello".ToUpper()` のように、文字列の後に `.` を付けて呼び出すことはできません。`string` は .NET に用意されているクラスで、自分でメソッドを追加することはできないからです。

拡張メソッドを使うと、`string` を書き換えずに、`"hello".Shout()` と書けるようになります。

---

## 2. 拡張メソッドを定義する

**書式：[拡張メソッドの定義](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/extension-methods)**
```
static class クラス名
{
    public static 戻り値の型 メソッド名(this 対象の型 パラメータ名, 追加のパラメータ)
    {
        // 処理
    }
}
```

| 要素 | 説明 |
|---|---|
| `static class` | 拡張メソッドは、static クラスの中に定義する |
| `public static` | 拡張メソッド自体も static メソッドにする |
| `this 対象の型 パラメータ名` | 1 つ目のパラメータに `this` を付けると、その型の拡張メソッドになる。呼び出したインスタンスが、このパラメータに入る |
| `追加のパラメータ` | 必要なら、2 つ目以降にふつうのパラメータを書ける |

1 節の `Shout` を、拡張メソッドに書き換えます。変わるのは、パラメータに `this` を付けることだけです。

```csharp
Console.WriteLine("hello".Shout());

string name = "alice";
Console.WriteLine(name.Shout());
Console.WriteLine(name.Repeat(3));

static class StringExtensions
{
    public static string Shout(this string text)
    {
        return text.ToUpper() + "!";
    }

    public static string Repeat(this string text, int count)
    {
        string result = "";
        for (int i = 0; i < count; i++)
        {
            result += text;
        }
        return result;
    }
}
```

```
HELLO!
ALICE!
alicealicealice
```

`name.Shout()` と書くと、`name` の値が、`this` を付けたパラメータ `text` に入ります。`name.Repeat(3)` の `3` は、2 つ目のパラメータ `count` に入ります。

`int` のような値の型にも、拡張メソッドを定義できます。

```csharp
Console.WriteLine(4.IsEven());
Console.WriteLine(7.IsEven());

static class IntExtensions
{
    public static bool IsEven(this int value)
    {
        return value % 2 == 0;
    }
}
```

```
True
False
```

---

## 3. 拡張メソッドの正体

`name.Shout()` と書くと、コンパイラーは、これを `StringExtensions.Shout(name)` という static メソッドの呼び出しに変換します。拡張メソッドは、static メソッドを、インスタンスメソッドのような形で呼び出せるようにするための **シンタックスシュガー**（書き方を短くするための構文）です。

```csharp
string name = "alice";
Console.WriteLine(name.Shout());
Console.WriteLine(StringExtensions.Shout(name));

static class StringExtensions
{
    public static string Shout(this string text)
    {
        return text.ToUpper() + "!";
    }
}
```

```
ALICE!
ALICE!
```

どちらの書き方でも、同じメソッドが呼び出されます。

拡張メソッドは、対象の型の中にメソッドを追加するわけではありません。別のクラスにある static メソッドなので、[アクセス修飾子](/unity-csharp-learning/csharp/access-modifiers/) の決まりどおり、対象の型の `public` のメンバーしか使えません。

---

## よくあるミス

### static ではないクラスに拡張メソッドを定義する

```csharp
// ❌ NG: 拡張メソッドは static クラスに定義する
// class StringExtensions
// {
//     public static string Shout(this string text)  // CS1106
//     {
//         return text.ToUpper() + "!";
//     }
// }
```

### 対象の型の private メンバーを使う

```csharp
// ❌ NG: 拡張メソッドから、対象の型の private メンバーは使えない
// class A
// {
//     private int _value = 42;
// }
//
// static class AExtensions
// {
//     public static void Show(this A a)
//     {
//         Console.WriteLine(a._value);  // CS0122
//     }
// }
```

拡張メソッドは、対象の型の外にある、ふつうの static メソッドです。対象の型の内部を特別に使えるわけではありません。

### もともとあるメソッドと同じ名前の拡張メソッドを書く

```csharp
Console.WriteLine("hello".ToUpper());

static class StringExtensions
{
    public static string ToUpper(this string text)
    {
        return "拡張メソッドの ToUpper";
    }
}
```

```
HELLO
```

`string` には、もともと引数のない `ToUpper` メソッドがあります。型にもともとあるメソッドで呼び出せるときは、そちらが優先されるので、同じシグネチャの拡張メソッドは呼び出されません。コンパイラーは警告も出さないので、気付きにくい間違いです。拡張メソッドには、対象の型にない名前を付けます。

---

## まとめ

- 拡張メソッドは、既存の型にメソッドを追加したように見せる書き方
- static クラスの中に、1 つ目のパラメータに `this` を付けた static メソッドとして定義する
- `値.メソッド名()` の形で呼び出せる。コンパイラーは、これを static メソッドの呼び出しに変換する
- 拡張メソッドから使えるのは、対象の型の `public` のメンバーだけ
- 型にもともとある同じシグネチャのメソッドが、拡張メソッドより優先される

---

## 理解度チェック

1. 拡張メソッドは、どのような場面で役に立ちますか？
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   int score = 75;
   Console.WriteLine(score.IsPassed());
   Console.WriteLine(40.IsPassed());
   Console.WriteLine(IntExtensions.IsPassed(60));

   static class IntExtensions
   {
       public static bool IsPassed(this int value)
       {
           return value >= 60;
       }
   }
   ```

3. `int` の値を 2 倍にして返す拡張メソッド `Double(this int value)` を書き、`5.Double()` の結果を表示するコードを書いてください。

<details markdown="1">
<summary>解答を見る</summary>

1. `string` や `int` のように、自分ではクラスの定義を書き換えられない型に、その型のメソッドのような形で呼び出せる処理を追加したい場面です。
2. 次のように出力されます。`IntExtensions.IsPassed(60)` は、static メソッドとして直接呼び出しています。

   ```
   True
   False
   True
   ```

3. ```csharp
   Console.WriteLine(5.Double());

   static class IntExtensions
   {
       public static int Double(this int value)
       {
           return value * 2;
       }
   }
   ```

   `10` が表示されます。

</details>

---

## 次のステップ

[名前空間](/unity-csharp-learning/csharp/namespaces/) では、自分で作る型を名前空間に入れる方法と、同じ名前の型を区別する方法を学びます。拡張メソッドを使えるかどうかが、名前空間で決まることも確かめます。
