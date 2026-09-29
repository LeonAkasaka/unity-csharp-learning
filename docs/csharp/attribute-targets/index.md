---
layout: page
title: 属性の対象
permalink: /csharp/attribute-targets/
---

# 属性の対象

[引数を持つ属性](/unity-csharp-learning/csharp/attribute-arguments/) で作った `[RangeCheck]` は、プロパティに付けて使う属性です。このページでは、属性を付けられる宣言を制限する方法と、同じ属性を複数付けられるようにする方法、属性を付ける対象を指定する方法を学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `[AttributeUsage]` と `AttributeTargets` で、属性を付けられる宣言を制限できる
- `AllowMultiple` で、同じ属性を 1 つの宣言に複数付けられるようにできる
- `GetProperty` で、名前を指定してプロパティの情報を取得できる
- `GetCustomAttributes<T>` で、複数付いた属性をまとめて読み取れる
- `[field: ]` で、自動実装プロパティの値をしまうフィールドに属性を付けられる

## 前提知識

- [引数を持つ属性](/unity-csharp-learning/csharp/attribute-arguments/) を読んでいること

---

## 1. 意味のない宣言にも付けられてしまう

`[RangeCheck]` は、`int` のプロパティの値の範囲を表す属性です。ところが、今の `RangeCheckAttribute` は、クラスにもメソッドにも付けられます。

```
[RangeCheck(0, 100)]
class Player
{
    // ...
}
```

このコードは、エラーにならずにコンパイルできます。しかし、[引数を持つ属性](/unity-csharp-learning/csharp/attribute-arguments/) の `Validate` はプロパティの属性しか読み取らないので、クラスに付けた `[RangeCheck]` は、何の効果もありません。付けた人は範囲を検証しているつもりでも、実際には何も起きません。

属性を作る側が、付けられる宣言を決めておけば、このような誤りをコンパイルするときに見つけられます。

---

## 2. AttributeUsage で付けられる宣言を制限する

[AttributeUsage 属性](https://learn.microsoft.com/dotnet/api/system.attributeusageattribute) は、属性のクラスに付けて、その属性を付けられる宣言を決める属性です。

**書式：[AttributeUsage 属性](https://learn.microsoft.com/dotnet/csharp/language-reference/attributes/general#attributeusage-attribute)**
```
[AttributeUsage(AttributeTargets.対象)]
class 名前Attribute : Attribute
{
}
```

`対象` には、[AttributeTargets 列挙型](https://learn.microsoft.com/dotnet/api/system.attributetargets) のメンバーを書きます。

| 指定 | 付けられる宣言 |
|---|---|
| `AttributeTargets.Class` | クラス |
| `AttributeTargets.Struct` | 構造体 |
| `AttributeTargets.Method` | メソッド |
| `AttributeTargets.Property` | プロパティ |
| `AttributeTargets.Field` | フィールド |
| `AttributeTargets.Parameter` | パラメータ |
| `AttributeTargets.All` | すべて |

`AttributeTargets` は `[Flags]` の付いた列挙型なので、`AttributeTargets.Property | AttributeTargets.Field` のように `|` でつないで、複数の対象を許可できます。`[AttributeUsage]` を付けていない属性は、すべての宣言に付けられます。

`RangeCheckAttribute` を、プロパティだけに付けられるようにします。

```
[AttributeUsage(AttributeTargets.Property)]
class RangeCheckAttribute : Attribute
{
    // 引数を持つ属性の 3 節と同じ
}
```

これで、クラスに付けると、コンパイルエラーになります。

```csharp
// ❌ NG: [RangeCheck] はプロパティにしか付けられない
// [RangeCheck(0, 100)]  // CS0592
// class Player
// {
// }
```

CS0592 は、「属性 'RangeCheck' はこの宣言型では無効です。'プロパティ、インデクサー' 宣言でのみ有効です」というエラーです。`[AttributeUsage]` の `AttributeTargets.Property` は、コンパイラーが読み取っています。

---

## 3. 同じ属性を複数付けられるようにする

プロパティに、説明のメモを付ける `[Note]` 属性を作ります。1 つのプロパティに、メモを複数付けたいとします。

`[AttributeUsage]` で何も指定しなければ、同じ属性は 1 つの宣言に 1 つしか付けられません。2 つ付けると、CS0579 のエラーになります。

```csharp
// ❌ NG: 同じ属性を 2 つ付けている
// class NoteAttribute : Attribute
// {
//     public NoteAttribute(string text)
//     {
//     }
// }
//
// class Player
// {
//     [Note("体力")]
//     [Note("0 になると戦闘不能")]  // CS0579
//     public int Hp { get; set; }
// }
```

複数付けられるようにするには、`[AttributeUsage]` の `AllowMultiple` に `true` を渡します。`AllowMultiple` は、`AttributeUsageAttribute` の `set` できるプロパティなので、[引数を持つ属性](/unity-csharp-learning/csharp/attribute-arguments/) で学んだ名前付きの引数で渡します。

```
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
class NoteAttribute : Attribute
{
    public string Text { get; }

    public NoteAttribute(string text)
    {
        Text = text;
    }
}

class Player
{
    [Note("体力")]
    [Note("0 になると戦闘不能")]
    public int Hp { get; set; }
}
```

`AttributeTargets.Property` は順番で渡す引数（コンストラクターの引数）、`AllowMultiple = true` は名前付きの引数（プロパティへの代入）です。これで、`Hp` に `[Note]` を 2 つ付けても、エラーになりません。

---

## 4. 複数付いた属性を読み取る

複数付いた属性を読み取る前に、`Hp` プロパティの情報を取得します。[属性を読み取る](/unity-csharp-learning/csharp/reading-attributes/) では `GetProperties` ですべてのプロパティを取得しましたが、ここでは `Hp` の 1 つだけがほしいので、[GetProperty メソッド](https://learn.microsoft.com/dotnet/api/system.type.getproperty) を使います。

**書式：[GetProperty メソッド](https://learn.microsoft.com/dotnet/api/system.type.getproperty)**
```csharp
public PropertyInfo? GetProperty(string name);
```

| パラメータ | 説明 |
|---|---|
| `name` | 取得するプロパティの名前 |

`GetProperty` は、指定した名前の `public` なプロパティの情報を返します。見つからないときは `null` を返します。

`GetCustomAttribute<T>` は、属性を 1 つだけ返すメソッドです。同じ属性が複数付いているときは、[GetCustomAttributes\<T\> メソッド](https://learn.microsoft.com/dotnet/api/system.reflection.customattributeextensions.getcustomattributes)（最後に `s` の付いたほう）で、すべてをまとめて読み取ります。

**書式：[GetCustomAttributes\<T\> メソッド](https://learn.microsoft.com/dotnet/api/system.reflection.customattributeextensions.getcustomattributes)**
```csharp
public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo element) where T : Attribute;
```

付いている属性のインスタンスを、`IEnumerable<T>` で返します。1 つも付いていなければ、要素のない `IEnumerable<T>` を返すので、`foreach` でそのまま回せます。

```csharp
using System.Reflection;

PropertyInfo hp = typeof(Player).GetProperty("Hp")!;
foreach (NoteAttribute note in hp.GetCustomAttributes<NoteAttribute>())
{
    Console.WriteLine(note.Text);
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
class NoteAttribute : Attribute
{
    public string Text { get; }

    public NoteAttribute(string text)
    {
        Text = text;
    }
}

class Player
{
    [Note("体力")]
    [Note("0 になると戦闘不能")]
    public int Hp { get; set; }
}
```

```
体力
0 になると戦闘不能
```

`Hp` があることはわかっているので、`GetProperty` の戻り値に `!` を付けて、`null` でないことをコンパイラーに伝えています。`Hp` に付けた 2 つの `[Note]` が、両方とも読み取られています。なお、`GetCustomAttributes<T>` が返す属性の順番は、決まっていません。付けた順番に頼る処理は書かないようにします。

---

## 5. 属性を付ける対象を指定する

属性をどの宣言に付けるかは、ふつうは、属性を書いた位置で決まります。プロパティの直前に書けばプロパティに、メソッドの直前に書けばメソッドに付きます。

位置だけでは決まらない場合は、`対象:` を書いて、属性を付ける対象を指定します。

**書式：[属性の対象](https://learn.microsoft.com/dotnet/csharp/advanced-topics/reflection-and-attributes#attribute-targets)**
```
[対象: 属性名]
宣言
```

よく使う対象は `field:` です。ほかに、メソッドの戻り値を表す `return:` などもありますが、このページでは `field:` だけを扱います。

[プロパティ](/unity-csharp-learning/csharp/properties/) で学んだ自動実装プロパティ `{ get; set; }` では、値をしまうフィールドを、コンパイラーが見えないところに作ります。そのフィールドは C# のコードには書かれていないので、属性を付ける位置がありません。`[field: 属性名]` と書くと、プロパティではなく、そのフィールドに属性を付けられます。

フィールドに付けた属性を、リフレクションで確かめます。[Type.GetFields メソッド](https://learn.microsoft.com/dotnet/api/system.type.getfields) に `BindingFlags.NonPublic | BindingFlags.Instance` を渡すと、`private` なインスタンスのフィールドを取得できます。

```csharp
using System.Reflection;

foreach (FieldInfo field in typeof(Player).GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
{
    NoteAttribute? note = field.GetCustomAttribute<NoteAttribute>();
    Console.WriteLine($"{field.Name}: {note?.Text}");
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
class NoteAttribute : Attribute
{
    public string Text { get; }

    public NoteAttribute(string text)
    {
        Text = text;
    }
}

class Player
{
    [field: Note("体力")]
    public int Hp { get; set; }
}
```

```
<Hp>k__BackingField: 体力
```

`Hp` プロパティのために、コンパイラーが `<Hp>k__BackingField` という名前のフィールドを作っていて、`[field: Note("体力")]` の属性は、このフィールドに付いています。`<` や `>` を含む名前は、C# のコードでは書けないので、コードから直接このフィールドを使うことはできません。

`NoteAttribute` の `[AttributeUsage]` に `AttributeTargets.Field` を加えている点にも注目してください。`[field: Note(...)]` はフィールドに付けるので、フィールドへの付け方を許可しておく必要があります（「よくあるミス」を参照）。

---

## よくあるミス

### field: で付ける属性に、フィールドを許可していない

```csharp
// ❌ NG: NoteAttribute はプロパティにしか付けられないのに、field: でフィールドに付けている
// [AttributeUsage(AttributeTargets.Property)]
// class NoteAttribute : Attribute
// {
//     public NoteAttribute(string text)
//     {
//     }
// }
//
// class Player
// {
//     [field: Note("体力")]  // CS0592
//     public int Hp { get; set; }
// }
```

`[field: ]` で付ける先は、プロパティではなくフィールドです。属性の `[AttributeUsage]` が `AttributeTargets.Field` を許可していないと、CS0592 のエラーになります。

---

## まとめ

- `[AttributeUsage(AttributeTargets.対象)]` を属性のクラスに付けると、その属性を付けられる宣言を制限できる。`|` で複数の対象を許可できる。付けられない宣言に付けると CS0592
- `[AttributeUsage]` を付けていない属性は、すべての宣言に付けられる
- 同じ属性は、ふつうは 1 つの宣言に 1 つだけ（2 つ付けると CS0579）。`AllowMultiple = true` で複数付けられるようにする
- `GetProperty` で、名前を指定してプロパティの情報を取得できる。見つからなければ `null`
- 複数付いた属性は、`GetCustomAttributes<T>` でまとめて読み取る。順番は決まっていない
- `[field: 属性名]` で、自動実装プロパティの値をしまうフィールドに属性を付けられる。属性の側で `AttributeTargets.Field` を許可しておく

---

## 理解度チェック

1. 次の属性を付けられるのは、どの宣言ですか？

   ```
   [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
   class LogAttribute : Attribute
   {
   }
   ```

2. `[AttributeUsage]` を付けていない属性を、クラスとプロパティとメソッドに付けると、どうなりますか？
3. 4 節の `NoteAttribute` の `AllowMultiple = true` を消すと、4 節のコードはどうなりますか？
4. 自動実装プロパティ `public int Hp { get; set; }` の値をしまうフィールドに、`[Note("体力")]` を付けるには、どう書きますか？

<details markdown="1">
<summary>解答を見る</summary>

1. クラスとメソッドです。`AttributeTargets.Class | AttributeTargets.Method` で、2 つの対象を許可しています。
2. どれにも付けられます。`[AttributeUsage]` を付けていない属性は、すべての宣言に付けられます。
3. `Hp` に `[Note]` を 2 つ付けている行で、コンパイルエラー CS0579 になります。
4. プロパティの直前に `[field: Note("体力")]` と書きます。`NoteAttribute` の `[AttributeUsage]` で、`AttributeTargets.Field` を許可しておく必要があります。

</details>

---

## 次のステップ

これで「C# 属性」のセクションは終わりです。[デリゲートの基本](/unity-csharp-learning/csharp/delegates/) からは「C# デリゲートとイベント」のセクションに進み、メソッドへの参照を変数として扱うデリゲートの仕組みを学びます。
