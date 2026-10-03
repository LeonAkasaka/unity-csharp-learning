---
layout: page
title: 型制約
permalink: /csharp/generic-constraints/
---

# 型制約

型パラメータ `T` は、どんな型にもなりえます。そのため、制約のない `T` に対してできる操作は、どの型でも使える `object` のメンバーに限られます。**型制約**（type constraint）で `T` になれる型に条件を付けると、その条件で保証されるメンバーを `T` に対して使えるようになります。このページでは、`where` による型制約の書き方と、インターフェイス・基底クラス・`class`・`struct`・`new()` の各制約を学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- 制約のない `T` でできる操作の限界を説明できる
- `where T : インターフェイス名` や `where T : クラス名` で、`T` のメンバーを使えるようにできる
- 型制約が、呼び出す側が指定できる型も制限することを説明できる
- `class`・`struct`・`new()` の各制約の意味を説明できる
- 複数の制約を、正しい順序で組み合わせて書ける

## 前提知識

- [ジェネリックメソッド](/unity-csharp-learning/csharp/generic-methods/) を読んでいること
- [型変換と型チェック](/unity-csharp-learning/csharp/type-casting/) を読んでいること
- [プロパティ](/unity-csharp-learning/csharp/properties/) を読んでいること
- [オブジェクト初期化子（補足）](/unity-csharp-learning/csharp/object-initializers/) を読んでいること

---

## 1. 制約のない T でできること

2 つの値のうち大きい方を返すジェネリックメソッド `Max` を書こうとすると、コンパイルエラーになります。

```csharp
static class Util
{
    public static T Max<T>(T a, T b)
    {
        // ❌ NG: T に > 演算子があるとは限らない（CS0019）
        // return a > b ? a : b;
    }
}
```

`T` には、`int` のように大小を比べられる型だけでなく、大小の決まらないクラスなども指定できます。コンパイラーは、どの型が指定されても正しく動くことを確かめられる操作しか許しません。制約のない `T` に使えるのは、すべての型が持つ `object` のメンバー（`ToString`・`Equals` など）だけです。[ジェネリクスの基本](/unity-csharp-learning/csharp/generics/) で学んだ `CompareTo` も、`T` が `IComparable<T>` を実装しているとは限らないので呼び出せません。

---

## 2. インターフェイス制約

型パラメータの後に `where` を書くと、`T` になれる型に条件を付けられます。

**書式：[型制約](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/where-generic-type-constraint)**
```
アクセス修飾子 戻り値の型 メソッド名<T>(パラメータリスト) where T : 制約
class クラス名<T> where T : 制約
```

| 要素 | 説明 |
|---|---|
| `where T :` | 型パラメータ `T` に制約を付けることを表す |
| `制約` | インターフェイス名・クラス名・`class`・`struct`・`new()` など |

`where T : IComparable<T>` と書くと、`T` は `IComparable<T>` を実装した型に限られます。そのため、`T` の値に対して `CompareTo` を呼び出せるようになります。

```csharp
Console.WriteLine(Util.Max(3, 7));
Console.WriteLine(Util.Max("abc", "xyz"));
Console.WriteLine(Util.Max(2.5, 1.5));

static class Util
{
    public static T Max<T>(T a, T b) where T : IComparable<T>
    {
        return a.CompareTo(b) >= 0 ? a : b;
    }
}
```

```
7
xyz
2.5
```

`int`・`string`・`double` は、どれも `IComparable<T>` を実装しているので、`Max` に渡せます。自分で作ったクラスに `IComparable<T>` を実装する方法は、[比較の仕組み](/unity-csharp-learning/csharp/comparison/) で学びます。戻り値の型も `T` なので、`Max(3, 7)` の結果は `int` のまま使えます。

### 制約は呼び出す側も制限する

型制約は、メソッドの中で使える操作を増やす代わりに、呼び出す側が指定できる型を制限します。制約を満たさない型を指定すると、コンパイルエラーになります。

```csharp
// ❌ NG: object は IComparable<object> を実装していない（CS0311）
// Util.Max(new object(), new object());
```

メソッドを書く側は「`T` は必ず `CompareTo` を持つ」と頼ることができ、呼び出す側は「`CompareTo` を持たない型は渡せない」と約束します。この約束をコンパイラーが両側で確かめるので、実行時に「`CompareTo` がなかった」というエラーは起きません。

> 💡 **ポイント**: パラメータの型を `IComparable` にしたジェネリックでないメソッドでも、`CompareTo` は呼び出せます。しかし、その場合は戻り値の型も `IComparable` や `object` になり、`int` として使うにはキャストが必要です。[ジェネリクスの基本](/unity-csharp-learning/csharp/generics/) の `object` 版の `Container` と同じ問題です。型制約を使えば、戻り値を `T` のまま返せます。

---

## 3. 基底クラス制約

制約には、クラス名も書けます。`where T : Character` と書くと、`T` は `Character` か、その派生クラスに限られます。`T` の値に対して、`Character` のメンバーを使えるようになります。

```csharp
Player alice = new Player { Name = "Alice", Hp = 70 };
Player bob = new Player { Name = "Bob", Hp = 90 };

Player stronger = Battle.Stronger(alice, bob);
stronger.UsePotion();

static class Battle
{
    // HP が大きい方を返す
    public static T Stronger<T>(T a, T b) where T : Character
    {
        return a.Hp >= b.Hp ? a : b;
    }
}

class Character
{
    public string Name { get; set; } = "";
    public int Hp { get; set; }
}

class Player : Character
{
    public void UsePotion()
    {
        Hp += 20;
        Console.WriteLine($"{Name} は回復薬を使った。HP={Hp}");
    }
}
```

```
Bob は回復薬を使った。HP=110
```

`Stronger` の中では、`T` が `Character` の派生クラスであることが保証されているので、`a.Hp` を読めます。

パラメータの型を `Character` にした、ジェネリックでないメソッドでも `Hp` は比べられます。しかし、その場合は戻り値の型も `Character` になるので、`Player` だけが持つ `UsePotion` を呼ぶにはキャストが必要です。ジェネリックメソッドなら、`Player` を渡すと `T` が `Player` に推論され、戻り値も `Player` のまま受け取れます。

---

## 4. class 制約と struct 制約

`where T : class` は、`T` を参照型に限ります。`where T : struct` は、`T` を値型に限ります。`int`・`double`・`bool` などは値型で、`string` や自分で定義したクラスは参照型です。値型と参照型の違いは、[値型と参照型](/unity-csharp-learning/csharp/value-reference-types/) で詳しく学びます。

```csharp
Console.WriteLine(Kind.OfValue(42));
Console.WriteLine(Kind.OfReference("abc"));

// ❌ NG: struct 制約の T に string は指定できない（CS0453）
// Kind.OfValue("abc");

// ❌ NG: class 制約の T に int は指定できない（CS0452）
// Kind.OfReference(42);

static class Kind
{
    public static string OfValue<T>(T value) where T : struct
    {
        return $"{value} は値型";
    }

    public static string OfReference<T>(T value) where T : class
    {
        return $"{value} は参照型";
    }
}
```

```
42 は値型
abc は参照型
```

`class` 制約は、参照型でなければできない操作を `T` に対して行うときに使います。たとえば、[型変換と型チェック](/unity-csharp-learning/csharp/type-casting/) で学んだ `as` 演算子は、変換できないときに `null` を返すので、変換先は参照型でなければなりません。`value as T` と書けるのは、`T` に `class` 制約があるときだけです（制約がないと CS0413）。

`struct` 制約は、`T` が値型であること、つまり `T` の値が `null` にならないことを保証したいときに使います。たとえば .NET の `Nullable<T>` は `struct` 制約を使っています。これは [null 許容値型](/unity-csharp-learning/csharp/nullable-value-types/) で学びます。

---

## 5. new() 制約

制約のない `T` では、`new T()` と書いてインスタンスを作れません（CS0304）。`T` に、引数なしで呼び出せるコンストラクターがあるとは限らないからです。`where T : new()` を付けると、`T` は `public` で引数のないコンストラクターを持つ型に限られ、`new T()` と書けるようになります。

**書式：[new() 制約](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/new-constraint)**
```
where T : new()
```

次の `CreateAll` は、長さ `count` の配列を作り、すべての要素に `new T()` で作ったインスタンスを入れて返します。

```csharp
Counter[] counters = Factory.CreateAll<Counter>(3);
counters[0].Count = 5;

foreach (Counter c in counters)
{
    Console.WriteLine(c.Count);
}

static class Factory
{
    public static T[] CreateAll<T>(int count) where T : new()
    {
        T[] items = new T[count];
        for (int i = 0; i < count; i++)
        {
            items[i] = new T();
        }
        return items;
    }
}

class Counter
{
    public int Count { get; set; }
}
```

```
5
0
0
```

要素ごとに別のインスタンスが作られているので、`counters[0].Count` を変えても、ほかの要素は `0` のままです。`CreateAll` は、`T` を推論する手がかりになる引数がないので、`CreateAll<Counter>` と型引数を書いて呼び出します。

コンストラクターを 1 つも書いていないクラスには、コンパイラーが引数のない `public` コンストラクターを用意するので、`new()` 制約を満たします。引数のあるコンストラクターだけを定義したクラスは満たさないので、型引数に指定するとコンパイルエラー（CS0310）になります。

---

## 6. 複数の制約

1 つの型パラメータに、`,` で区切って複数の制約を付けられます。

**書式：[複数の制約](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/where-generic-type-constraint)**
```
where T : 制約1, 制約2
```

制約を書く順序には決まりがあります。

1. `class`・`struct`・基底クラスは、先頭に 1 つだけ書く
2. インターフェイスは、その後に書く
3. `new()` は、最後に書く

次の `Spawn` は、`Character` の派生クラスのインスタンスを `new T()` で作り、名前と HP を設定して返します。`Name` と `Hp` を設定するために基底クラス制約を、`new T()` を書くために `new()` 制約を付けています。

```csharp
Player hero = Spawner.Spawn<Player>("Alice", 100);
Enemy slime = Spawner.Spawn<Enemy>("Slime", 20);

hero.UsePotion();
Console.WriteLine($"{slime.Name} HP={slime.Hp}");

static class Spawner
{
    public static T Spawn<T>(string name, int hp) where T : Character, new()
    {
        T character = new T();
        character.Name = name;
        character.Hp = hp;
        return character;
    }
}

class Character
{
    public string Name { get; set; } = "";
    public int Hp { get; set; }
}

class Player : Character
{
    public void UsePotion()
    {
        Hp += 20;
        Console.WriteLine($"{Name} は回復薬を使った。HP={Hp}");
    }
}

class Enemy : Character
{
}
```

```
Alice は回復薬を使った。HP=120
Slime HP=20
```

型パラメータが複数あるときは、型パラメータごとに `where` を書きます。

```csharp
class Ranking<TKey, TItem>
    where TKey : IComparable<TKey>
    where TItem : Character, new()
{
}
```

---

## 7. 制約の一覧

| 制約 | `T` になれる型 | `T` に対してできること |
|---|---|---|
| なし | すべての型 | `object` のメンバー（`ToString`・`Equals` など）を使う |
| `where T : インターフェイス名` | そのインターフェイスを実装した型 | インターフェイスのメンバーを使う |
| `where T : クラス名` | そのクラスと、その派生クラス | クラスのメンバーを使う |
| `where T : class` | 参照型 | `as` 演算子の変換先にするなど、参照型の操作をする |
| `where T : struct` | 値型（null 許容値型を除く） | `T` の値が `null` にならないことを前提にする |
| `where T : new()` | `public` で引数のないコンストラクターを持つ型 | `new T()` でインスタンスを作る |

---

## よくあるミス

### 制約の順序を間違える

`class`・`struct` を先頭以外に書いたり、`new()` を最後以外に書いたりすると、コンパイルエラーになります。

```csharp
// ❌ NG: class 制約が先頭にない（CS0449）
// public static void M1<T>() where T : IComparable<T>, class { }

// ❌ NG: new() 制約が最後にない（CS0401）
// public static void M2<T>() where T : new(), IComparable<T> { }

// ✅ OK: class → インターフェイス → new() の順
public static void M3<T>() where T : class, IComparable<T>, new() { }
```

---

## ワンポイントアドバイス

### そのほかの制約

このページで紹介した制約のほかにも、`null` を許さない型に限る `notnull`、ほかの型パラメータを制約にする `where T : U` などがあります。一覧は [型パラメーターの制約](https://learn.microsoft.com/dotnet/csharp/programming-guide/generics/constraints-on-type-parameters) にまとめられています。

---

## まとめ

- 制約のない `T` は、`object` のメンバーしか使えない
- `where T : 制約` で、`T` になれる型に条件を付けると、条件で保証されるメンバーを使えるようになる
- 型制約は、呼び出す側が指定できる型も制限する。満たさない型を指定するとコンパイルエラーになる
- インターフェイス制約・基底クラス制約を使うと、戻り値を `T` のまま返せるので、キャストが要らない
- `class` は参照型に、`struct` は値型に限る。`new()` は `new T()` を書けるようにする
- 複数の制約は `,` で区切る。`class`・`struct`・基底クラスが先頭、`new()` が最後

---

## 理解度チェック

1. 次のメソッドはコンパイルエラーになります。理由を説明し、コンパイルが通るように直してください。

   ```csharp
   public static T Min<T>(T a, T b)
   {
       return a.CompareTo(b) <= 0 ? a : b;
   }
   ```

2. 次のコードを実行すると何が出力されますか？

   ```csharp
   Console.WriteLine(Util.Max(Util.Max(3, 9), 5));
   Console.WriteLine(Util.Max("pear", "apple"));

   static class Util
   {
       public static T Max<T>(T a, T b) where T : IComparable<T>
       {
           return a.CompareTo(b) >= 0 ? a : b;
       }
   }
   ```

3. 次の `Item` クラスを、5 節の `Factory.CreateAll<T>` の型引数に指定できますか？理由とともに答えてください。

   ```csharp
   class Item
   {
       public string Name { get; }

       public Item(string name) { Name = name; }
   }
   ```

4. （応用）「参照型であり、かつ引数のないコンストラクターを持つ」という制約はどう書きますか？

<details markdown="1">
<summary>解答を見る</summary>

1. 制約のない `T` は `IComparable<T>` を実装しているとは限らないので、`CompareTo` を呼び出せません。`where T : IComparable<T>` を付けます。

   ```csharp
   public static T Min<T>(T a, T b) where T : IComparable<T>
   {
       return a.CompareTo(b) <= 0 ? a : b;
   }
   ```

2. `9` と `pear` が出力されます。内側の `Max(3, 9)` が `9` を返し、`Max(9, 5)` が `9` を返します。文字列は辞書の順で比べられ、`"pear"` は `"apple"` より後なので大きいと判定されます。
3. 指定できません（コンパイルエラー CS0310）。`Item` は引数のあるコンストラクターだけを定義しているので、引数のないコンストラクターを持たず、`new()` 制約を満たさないからです。
4. `where T : class, new()` と書きます。`class` を先頭に、`new()` を最後に書きます。

</details>

---

## 次のステップ

[共変・反変](/unity-csharp-learning/csharp/generic-variance/) では、`IReader<Player>` を `IReader<Character>` の変数に代入できるようにする、型パラメータの `out`・`in` を学びます。
