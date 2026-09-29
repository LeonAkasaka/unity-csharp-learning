---
layout: page
title: 属性を読み取る
permalink: /csharp/reading-attributes/
---

# 属性を読み取る

[属性の基本](/unity-csharp-learning/csharp/attributes/) では、`[Hidden]` を作って付けても、それだけではプログラムの動作が変わらないことを確かめました。このページでは、実行中に属性を読み取って、動作を変える処理を作ります。実行中に、メタデータから型やメンバーの情報を調べる仕組みを、**リフレクション**（reflection）といいます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `typeof` で、型名から型の情報を表す `Type` を取得できる
- `GetType` で、値の実体の型の `Type` を取得できる
- `GetProperties` で、型のプロパティの一覧を取得できる
- `GetValue` で、オブジェクトのプロパティの値を取得できる
- `GetCustomAttribute<T>` で、プロパティに属性が付いているかを調べ、プログラムの動作を変えられる

## 前提知識

- [属性の基本](/unity-csharp-learning/csharp/attributes/) を読んでいること
- [型変換と型チェック](/unity-csharp-learning/csharp/type-casting/) を読んでいること
- [名前空間](/unity-csharp-learning/csharp/namespaces/) を読んでいること
- [ジェネリックメソッド](/unity-csharp-learning/csharp/generic-methods/) を読んでいること

---

## 1. 型の情報を表す Type

属性は、メタデータの中で、型やメンバーの情報と一緒に記録されています。属性を読み取るには、まず、型の情報を取得します。

型の情報は、[Type クラス](https://learn.microsoft.com/dotnet/api/system.type) のインスタンスで表されます。`Type` には、型の名前を表す [Name プロパティ](https://learn.microsoft.com/dotnet/api/system.type.name) や、名前空間を含めた完全修飾名を表す [FullName プロパティ](https://learn.microsoft.com/dotnet/api/system.type.fullname) があります。

`Type` を取得する方法は 2 つあります。1 つずつ見ていきます。

### typeof 演算子

**書式：[typeof 演算子](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/type-testing-and-cast#the-typeof-operator)**
```
typeof(型名)
```

`typeof` は、`( )` に書いた型の `Type` を返します。

```csharp
Type intType = typeof(int);
Console.WriteLine(intType.Name);
Console.WriteLine(intType.FullName);

Type stringType = typeof(string);
Console.WriteLine(stringType.Name);
```

```
Int32
System.Int32
String
```

`int` は `System.Int32` の別名なので（[数値リテラルと型エイリアス（補足）](/unity-csharp-learning/csharp/numeric-literals/)）、`Name` は `Int32`、`FullName` は `System.Int32` です。

`typeof` には、型名を書きます。どの型の情報がほしいかを、プログラムを書くときに決めている場合に使います。

### GetType メソッド

**書式：[GetType メソッド](https://learn.microsoft.com/dotnet/api/system.object.gettype)**
```
式.GetType()
```

`GetType` は、すべての型の基底クラスである `object` のメソッドです。値の **実体の型** の `Type` を返します。

```csharp
object o = new Player("Alice", "secret123", 5);
Type playerType = o.GetType();
Console.WriteLine(playerType.Name);

class HiddenAttribute : Attribute
{
}

class Player
{
    public string Name { get; set; }

    [Hidden]
    public string Password { get; set; }

    public int Level { get; set; }

    public Player(string name, string password, int level)
    {
        Name = name;
        Password = password;
        Level = level;
    }
}
```

```
Player
```

変数 `o` の型は `object` ですが、`GetType` は `object` ではなく、`o` に入っているインスタンスの型の `Player` を返します。[型変換と型チェック](/unity-csharp-learning/csharp/type-casting/) で学んだ「変数の型」と「実体の型」のうち、実体の型です。

`GetType` は、値を受け取るまで型がわからない場合に使います。このページでこの後に作る `Print` のように、`object` で受け取った値の型を調べるときです。

### typeof と GetType の違い

`typeof` と `GetType` が返す `Type` は、同じ型なら同じものです。`==` で比べられます。

```csharp
object o = new Player("Alice", "secret123", 5);
Console.WriteLine(o.GetType() == typeof(Player));
Console.WriteLine(o.GetType() == typeof(object));

// HiddenAttribute クラスと Player クラスは、上のコード例と同じ
```

```
True
False
```

| | `typeof(型名)` | `式.GetType()` |
|---|---|---|
| 書くもの | 型名 | 値（変数など） |
| 返す型の情報 | 書いた型 | 値の実体の型 |
| 型が決まるとき | プログラムを書くとき | 実行するとき |

---

## 2. プロパティの一覧を取得する

`Type` の [GetProperties メソッド](https://learn.microsoft.com/dotnet/api/system.type.getproperties) は、その型の `public` なプロパティの情報を、[PropertyInfo](https://learn.microsoft.com/dotnet/api/system.reflection.propertyinfo) の配列で返します。`PropertyInfo` の `Name` はプロパティの名前、`PropertyType` はプロパティの型の `Type` です。

**書式：[GetProperties メソッド](https://learn.microsoft.com/dotnet/api/system.type.getproperties)**
```csharp
public PropertyInfo[] GetProperties();
```

`PropertyInfo` は `System.Reflection` 名前空間にあるので、ファイルの先頭に `using System.Reflection;` を書きます。

```csharp
using System.Reflection;

foreach (PropertyInfo property in typeof(Player).GetProperties())
{
    Console.WriteLine($"{property.Name}: {property.PropertyType.Name}");
}

// HiddenAttribute クラスと Player クラスは、1 節のコード例と同じ
```

```
Name: String
Password: String
Level: Int32
```

プログラムの中にプロパティの名前を書かなくても、どんなプロパティがあるかを、実行中にメタデータから調べられます。

なお、`GetProperties` が返すプロパティの順番は、決まっていません。宣言の順番に返ることが多いですが、順番に頼る処理は書かないようにします。

---

## 3. プロパティの値を取得する

`PropertyInfo` の [GetValue メソッド](https://learn.microsoft.com/dotnet/api/system.reflection.propertyinfo.getvalue) は、指定したオブジェクトの、そのプロパティの値を返します。

**書式：[GetValue メソッド](https://learn.microsoft.com/dotnet/api/system.reflection.propertyinfo.getvalue)**
```csharp
public object? GetValue(object? obj);
```

| パラメータ | 説明 |
|---|---|
| `obj` | 値を取り出すオブジェクト |

`PropertyInfo` はプロパティの情報で、どのオブジェクトの値かは決まっていません。そのため、`GetValue` には、値を取り出すオブジェクトを渡します。戻り値の型は `object?` です。

どんなオブジェクトでも、プロパティの名前と値を一覧表示する `Print` メソッドを作ります。1 節の `GetType` で、渡されたオブジェクトの実体の型を調べ、2 節の `GetProperties` でプロパティの一覧を取得します。

```csharp
using System.Reflection;

Player player = new Player("Alice", "secret123", 5);
Print(player);

void Print(object target)
{
    foreach (PropertyInfo property in target.GetType().GetProperties())
    {
        Console.WriteLine($"{property.Name}: {property.GetValue(target)}");
    }
}

// HiddenAttribute クラスと Player クラスは、1 節のコード例と同じ
```

```
Name: Alice
Password: secret123
Level: 5
```

`Print` の中には、`Name` や `Level` といったプロパティの名前が書かれていません。`Player` にプロパティを追加すれば、`Print` を書き直さなくても、表示されるようになります。

ただし、このままでは、`[Hidden]` の付いた `Password` も表示されてしまいます。

---

## 4. 属性を読み取る

プロパティに付いた属性は、`PropertyInfo` の [GetCustomAttribute\<T\> メソッド](https://learn.microsoft.com/dotnet/api/system.reflection.customattributeextensions.getcustomattribute) で読み取れます。

**書式：[GetCustomAttribute\<T\> メソッド](https://learn.microsoft.com/dotnet/api/system.reflection.customattributeextensions.getcustomattribute)**
```csharp
public static T? GetCustomAttribute<T>(this MemberInfo element) where T : Attribute;
```

`T` に属性のクラスを指定すると、その属性が付いていれば属性のインスタンスを返し、付いていなければ `null` を返します。`System.Reflection` 名前空間にある拡張メソッドなので、`property.GetCustomAttribute<HiddenAttribute>()` の形で呼び出せます。

3 節の `Print` を、`[Hidden]` の付いたプロパティを表示しないように変えます。

```csharp
using System.Reflection;

Player player = new Player("Alice", "secret123", 5);
Print(player);

void Print(object target)
{
    foreach (PropertyInfo property in target.GetType().GetProperties())
    {
        if (property.GetCustomAttribute<HiddenAttribute>() != null)
        {
            continue;
        }
        Console.WriteLine($"{property.Name}: {property.GetValue(target)}");
    }
}

// HiddenAttribute クラスと Player クラスは、1 節のコード例と同じ
```

```
Name: Alice
Level: 5
```

`Password` には `[Hidden]` が付いているので、`GetCustomAttribute<HiddenAttribute>()` がインスタンスを返し、`continue` で次のプロパティに進みます。`Name` と `Level` には付いていないので、`null` が返り、表示されます。

これで、`[Hidden]` に効果が出ました。表示しないプロパティを増やしたいときは、そのプロパティに `[Hidden]` を付けるだけです。`Print` を書き直す必要はありません。[属性の基本](/unity-csharp-learning/csharp/attributes/) の 1 節で、「表示しない」という決まりが 2 か所に分かれる問題を見ましたが、今は、決まりは `Player` の宣言に付けた属性の 1 か所だけにあります。

```mermaid
flowchart LR
    P["Player の宣言<br/>Password に [Hidden]"] --> M["メタデータ"]
    M --> R["Print<br/>GetCustomAttribute で読み取る"]
    R --> O["Hidden の付いたプロパティを<br/>表示しない"]
```

[属性の基本](/unity-csharp-learning/csharp/attributes/) で見た `[Flags]` と列挙型の `ToString` の関係も、この `[Hidden]` と `Print` の関係と同じです。.NET やライブラリが「属性を付けると動作が変わる」機能を提供しているときは、その内部で、このように属性を読み取っています。

---

## よくあるミス

### using System.Reflection を書き忘れる

```csharp
// ❌ NG: ファイルの先頭に using System.Reflection; がない
// void Print(object target)
// {
//     foreach (var property in target.GetType().GetProperties())
//     {
//         if (property.GetCustomAttribute<HiddenAttribute>() != null)  // CS1061
//         {
//             continue;
//         }
//     }
// }
```

`var` で書いたので、`PropertyInfo` という型名は出てきませんが、`property` の型は `PropertyInfo` です。[名前空間](/unity-csharp-learning/csharp/namespaces/) で学んだように、拡張メソッドは、その名前空間を読み込んだときだけ呼び出せます。`GetCustomAttribute<T>` は `System.Reflection` 名前空間の `CustomAttributeExtensions` クラスにある拡張メソッドで、`System.Reflection` は暗黙的な using ディレクティブに含まれていません。ファイルの先頭に `using System.Reflection;` を書きます。

---

## まとめ

- 実行中に、メタデータから型やメンバーの情報を調べる仕組みを、リフレクションという
- 型の情報は `Type` で表される
- `typeof(型名)` は、書いた型の `Type` を返す。型をプログラムを書くときに決めている場合に使う
- `式.GetType()` は、値の実体の型の `Type` を返す。値を受け取るまで型がわからない場合に使う
- `GetProperties` で、プロパティの情報（`PropertyInfo`）の一覧を取得できる。順番は決まっていない
- `GetValue` で、指定したオブジェクトのプロパティの値を取得できる
- `GetCustomAttribute<T>` は、属性が付いていればそのインスタンスを、付いていなければ `null` を返す。`using System.Reflection;` が必要
- 属性を読み取る処理を書くと、属性を付けるだけで動作を変えられるようになる

---

## 理解度チェック

1. `object value = "abc";` のとき、`value.GetType() == typeof(string)` と `typeof(object) == value.GetType()` の値は、それぞれ何ですか？
2. 4 節の `Player` の `Level` にも `[Hidden]` を付けました。`Print(player)` を実行すると、何が出力されますか？
3. 4 節の `Print` を、`[Hidden]` の付いたプロパティ**だけ**を表示するように変えるには、どこをどう変えますか？
4. 3 節の `Print` で、`typeof` ではなく `GetType` を使っているのはなぜですか？

<details markdown="1">
<summary>解答を見る</summary>

1. `value.GetType() == typeof(string)` は `True`、`typeof(object) == value.GetType()` は `False` です。`GetType` は、変数の型の `object` ではなく、実体の型の `string` を返します。
2. `Name: Alice` だけが出力されます。
3. `!= null` を `== null` に変えます。`[Hidden]` が付いていないプロパティで `continue` し、付いているプロパティだけを表示するようになります。
4. `Print` は `object` で値を受け取るので、どんな型のオブジェクトが渡されるかは、実行するまでわからないからです。`GetType` は、渡された値の実体の型を返します。

</details>

---

## 次のステップ

[引数を持つ属性](/unity-csharp-learning/csharp/attribute-arguments/) では、`[RangeCheck(0, 100)]` のように、属性に値を持たせる方法を学びます。
