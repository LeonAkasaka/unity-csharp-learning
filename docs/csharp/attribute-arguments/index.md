---
layout: page
title: 引数を持つ属性
permalink: /csharp/attribute-arguments/
---

# 引数を持つ属性

[属性を読み取る](/unity-csharp-learning/csharp/reading-attributes/) の `[Hidden]` は、「付いているかどうか」だけを表す属性でした。このページでは、`[RangeCheck(0, 100)]` のように、属性に引数で値を渡し、その値を読み取って使う方法を学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- コンストラクターを持つ属性のクラスを作り、`[名前(引数)]` で値を渡せる
- 属性に渡した値を、読み取った属性のインスタンスから使える
- 名前付きの引数 `名前 = 値` で、属性のプロパティに値を渡せる
- 属性の引数に定数しか書けない理由を説明できる
- `[Obsolete]` の引数を使い分けられる

## 前提知識

- [属性を読み取る](/unity-csharp-learning/csharp/reading-attributes/) を読んでいること
- [省略可能パラメータと名前付き引数](/unity-csharp-learning/csharp/optional-named-params/) を読んでいること
- [const と readonly（補足）](/unity-csharp-learning/csharp/const-readonly/) を読んでいること

---

## 1. 属性に値を持たせたい

プレイヤーの HP は 0〜100、レベルは 1〜99 の範囲で使うと決めたとします。この決まりを属性で表し、範囲外の値を見つける処理を作りたいとします。

`[Hidden]` のような、付いているかどうかだけの属性では、範囲を表せません。範囲ごとに `[Range0To100]`、`[Range1To99]` のような属性を作ることもできますが、範囲の数だけ属性のクラスが必要になります。

属性に、範囲の最小値と最大値を値として持たせられれば、1 つの属性で済みます。

```
[RangeCheck(0, 100)]
public int Hp { get; set; }

[RangeCheck(1, 99)]
public int Level { get; set; }
```

---

## 2. コンストラクターで値を受け取る

**書式：[属性の引数](https://learn.microsoft.com/dotnet/csharp/advanced-topics/reflection-and-attributes#attribute-parameters)**
```
[名前(引数1, 引数2)]
宣言
```

属性に `( )` で渡した引数は、属性のクラスのコンストラクターに渡されます。値を受け取るコンストラクターと、値を取り出すプロパティを持つ属性のクラスを作ります。

```
class RangeCheckAttribute : Attribute
{
    public int Min { get; }
    public int Max { get; }

    public RangeCheckAttribute(int min, int max)
    {
        Min = min;
        Max = max;
    }
}
```

`[RangeCheck(0, 100)]` と書くと、コンストラクターの `min` に `0`、`max` に `100` が渡されます。

[属性を読み取る](/unity-csharp-learning/csharp/reading-attributes/) の `Print` と同じように、どんなオブジェクトでも、`[RangeCheck]` の付いたプロパティの値を検証する `Validate` メソッドを作ります。

```csharp
using System.Reflection;

Player player = new Player("Alice", "secret123", 150, 5);
Validate(player);

void Validate(object target)
{
    foreach (PropertyInfo property in target.GetType().GetProperties())
    {
        RangeCheckAttribute? range = property.GetCustomAttribute<RangeCheckAttribute>();
        if (range == null)
        {
            continue;
        }

        int value = (int)property.GetValue(target)!;
        if (value < range.Min || value > range.Max)
        {
            Console.WriteLine($"{property.Name} = {value} は {range.Min}〜{range.Max} の範囲外");
        }
    }
}

class HiddenAttribute : Attribute
{
}

class RangeCheckAttribute : Attribute
{
    public int Min { get; }
    public int Max { get; }

    public RangeCheckAttribute(int min, int max)
    {
        Min = min;
        Max = max;
    }
}

class Player
{
    public string Name { get; set; }

    [Hidden]
    public string Password { get; set; }

    [RangeCheck(0, 100)]
    public int Hp { get; set; }

    [RangeCheck(1, 99)]
    public int Level { get; set; }

    public Player(string name, string password, int hp, int level)
    {
        Name = name;
        Password = password;
        Hp = hp;
        Level = level;
    }
}
```

```
Hp = 150 は 0〜100 の範囲外
```

`GetCustomAttribute<RangeCheckAttribute>()` が返すのは、`RangeCheckAttribute` のインスタンスです。メタデータには、`Hp` に `RangeCheck` 属性が `0` と `100` の引数で付いていることが記録されています。読み取るときに、この記録をもとにコンストラクターが呼び出され、`Min` が `0`、`Max` が `100` のインスタンスが作られます。

`Validate` の中には、`Hp` や `Level` という名前も、`0`〜`100` という範囲も書かれていません。どのプロパティをどの範囲で検証するかは、`Player` の宣言に付けた属性だけで決まっています。

`GetValue` の戻り値は `object?` なので、`int` にキャストしています。`[RangeCheck]` は `int` のプロパティに付ける前提です。

---

## 3. 名前付きの引数でプロパティに値を渡す

範囲外のときに表示するメッセージを、プロパティごとに変えられるようにします。ただし、メッセージは省略もできるようにしたい値です。

属性では、`名前 = 値` の形で、属性のクラスの `set` できるプロパティに値を渡せます。これを **名前付きの引数** といいます。

**書式：[名前付きの引数](https://learn.microsoft.com/dotnet/csharp/advanced-topics/reflection-and-attributes#attribute-parameters)**
```
[名前(引数1, プロパティ名 = 値)]
宣言
```

| 要素 | 説明 |
|---|---|
| `引数1` | コンストラクターに渡す値。省略できない |
| `プロパティ名 = 値` | `set` できるプロパティに代入する値。省略できる。コンストラクターの引数の後ろに書く |

[省略可能パラメータと名前付き引数](/unity-csharp-learning/csharp/optional-named-params/) の名前付き引数と似ていますが、書き方は `:` ではなく `=` です。コンストラクターのパラメータではなく、プロパティに値を渡します。

`RangeCheckAttribute` に、`set` できる `Message` プロパティを追加します。省略されたときのために、初期値を `"範囲外"` にしておきます。

```csharp
using System.Reflection;

Player player = new Player("Alice", "secret123", 150, 120);
Validate(player);

void Validate(object target)
{
    foreach (PropertyInfo property in target.GetType().GetProperties())
    {
        RangeCheckAttribute? range = property.GetCustomAttribute<RangeCheckAttribute>();
        if (range == null)
        {
            continue;
        }

        int value = (int)property.GetValue(target)!;
        if (value < range.Min || value > range.Max)
        {
            Console.WriteLine($"{property.Name} = {value}: {range.Message}");
        }
    }
}

class HiddenAttribute : Attribute
{
}

class RangeCheckAttribute : Attribute
{
    public int Min { get; }
    public int Max { get; }
    public string Message { get; set; } = "範囲外";

    public RangeCheckAttribute(int min, int max)
    {
        Min = min;
        Max = max;
    }
}

class Player
{
    public string Name { get; set; }

    [Hidden]
    public string Password { get; set; }

    [RangeCheck(0, 100, Message = "HP は 0〜100 にしてください")]
    public int Hp { get; set; }

    [RangeCheck(1, 99)]
    public int Level { get; set; }

    public Player(string name, string password, int hp, int level)
    {
        Name = name;
        Password = password;
        Hp = hp;
        Level = level;
    }
}
```

```
Hp = 150: HP は 0〜100 にしてください
Level = 120: 範囲外
```

`Hp` の属性は `Message` を指定しているので、そのメッセージが表示されます。`Level` の属性は `Message` を省略しているので、初期値の `"範囲外"` のままです。

必ず渡してほしい値はコンストラクターの引数に、省略してもよい値は `set` できるプロパティにする、と使い分けます。

---

## 4. 引数には定数しか書けない

属性の引数には、`0` や `"範囲外"` のように、ビルドするときに値が決まっている定数しか書けません。[属性の基本](/unity-csharp-learning/csharp/attributes/) で学んだように、属性はメタデータに記録されます。記録されるのは、属性を付けた宣言だけでなく、属性に渡した値もです。ビルドするときに記録するので、実行するまで値が決まらない変数は書けません（「よくあるミス」を参照）。

[const](/unity-csharp-learning/csharp/const-readonly/) で宣言した定数は、ビルドするときに値が決まるので、書けます。

```
const int MaxHp = 100;

[RangeCheck(0, MaxHp)]
public int Hp { get; set; }
```

---

## 5. .NET の属性の引数

[属性の基本](/unity-csharp-learning/csharp/attributes/) で使った `[Obsolete]` も、引数を持つ属性です。[ObsoleteAttribute](https://learn.microsoft.com/dotnet/api/system.obsoleteattribute) には、引数の数が違うコンストラクターが用意されています。[属性の基本](/unity-csharp-learning/csharp/attributes/) では、引数のない `[Obsolete]` を使い、警告 CS0612 が出ることを確かめました。ここでは、引数を渡す書き方を 1 つずつ見ていきます。

### メッセージを渡す

1 つ目の引数に文字列を渡すと、その文字列が警告のメッセージになります。

```csharp
Console.WriteLine(Calc.OldAdd(1, 2));

static class Calc
{
    [Obsolete("Add を使ってください")]
    public static int OldAdd(int a, int b)
    {
        return a + b;
    }
}
```

ビルドすると、次の警告が出ます。警告の番号は、引数のないときの CS0612 ではなく、CS0618 になります。代わりに何を使えばよいかを、メッセージで伝えられます。

```
warning CS0618: 'Calc.OldAdd(int, int)' は旧形式です ('Add を使ってください')
```

### エラーにする

2 つ目の引数に `true` を渡すと、警告ではなくエラーになります。まずは警告で知らせ、しばらくしてからエラーにし、最後にメソッドを消す、という順番で使います。

```csharp
// ❌ NG: Obsolete の 2 つ目の引数が true なので、呼び出すとエラーになる
// Console.WriteLine(Calc.OldAdd(1, 2));  // CS0619
//
// static class Calc
// {
//     [Obsolete("Add を使ってください", true)]
//     public static int OldAdd(int a, int b)
//     {
//         return a + b;
//     }
// }
```

### 警告の番号を決める

`ObsoleteAttribute` には、`set` できる `DiagnosticId` プロパティもあります。3 節で学んだ名前付きの引数で値を渡すと、警告の番号を自分で決められます。

```csharp
Console.WriteLine(Calc.OldAdd(1, 2));

static class Calc
{
    [Obsolete("Add を使ってください", DiagnosticId = "CALC001")]
    public static int OldAdd(int a, int b)
    {
        return a + b;
    }
}
```

ビルドすると、CS0618 の代わりに、次の警告が出ます。

```
warning CALC001: 'Calc.OldAdd(int, int)' は旧形式です ('Add を使ってください')
```

### 書き方とビルドの結果

ここまでの `[Obsolete]` の書き方を比べると、次のようになります。

| 書き方 | ビルドの結果 |
|---|---|
| `[Obsolete]` | 警告 CS0612 |
| `[Obsolete("Add を使ってください")]` | 警告 CS0618。メッセージが警告に表示される |
| `[Obsolete("Add を使ってください", true)]` | エラー CS0619 |
| `[Obsolete("Add を使ってください", DiagnosticId = "CALC001")]` | 警告 CALC001 |

---

## よくあるミス

### 属性の引数に、定数でない値を書く

```csharp
// ❌ NG: 属性の引数には、定数しか書けない
// class Player
// {
//     static int maxHp = 100;
//
//     [RangeCheck(0, maxHp)]  // CS0182
//     public int Hp { get; set; }
// }
```

`static` のフィールドの値は、実行するまで決まらないので、属性の引数に書けません。4 節のように、`const` で宣言した定数にします。

---

## まとめ

- 属性に `( )` で渡した引数は、属性のクラスのコンストラクターに渡される
- `GetCustomAttribute<T>` が返すインスタンスから、属性に渡した値を取り出せる。メタデータの記録をもとに、読み取るときにインスタンスが作られる
- 名前付きの引数 `プロパティ名 = 値` で、属性の `set` できるプロパティに値を渡す。省略できる値に使う
- 属性の引数には、ビルドするときに値が決まる定数しか書けない。値がメタデータに記録されるから
- `[Obsolete]` は、引数なしで CS0612、メッセージ付きで CS0618、`true` を渡すと CS0619 になる

---

## 理解度チェック

1. 3 節の `Player` を `new Player("Bob", "pw", 50, 0)` で作り、`Validate` に渡すと、何が出力されますか？
2. `[RangeCheck(Message = "範囲外", 0, 100)]` と書くと、どうなりますか？
3. 属性の値を「必ず渡してほしい値」と「省略してもよい値」に分けるとき、それぞれを属性のクラスのどこで受け取りますか？
4. `[Obsolete]` と `[Obsolete("Save2 を使ってください")]` では、ビルドしたときの警告にどんな違いがありますか？

<details markdown="1">
<summary>解答を見る</summary>

1. `Level = 0: 範囲外` と出力されます。`Hp` の `50` は範囲内です。`Level` の属性は `Message` を省略しているので、初期値の `範囲外` が表示されます。
2. コンパイルエラー CS1016 になります。名前付きの引数は、コンストラクターに渡す引数の後ろに書く必要があります。
3. 必ず渡してほしい値はコンストラクターの引数で、省略してもよい値は `set` できるプロパティで受け取ります。
4. `[Obsolete]` は警告 CS0612 で、メッセージはありません。`[Obsolete("Save2 を使ってください")]` は警告 CS0618 で、`Save2 を使ってください` というメッセージが警告に表示されます。

</details>

---

## 次のステップ

[属性の対象](/unity-csharp-learning/csharp/attribute-targets/) では、属性を付けられる宣言を制限する方法と、属性を付ける対象を指定する方法を学びます。
