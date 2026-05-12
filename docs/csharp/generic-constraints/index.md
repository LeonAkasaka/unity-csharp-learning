---
layout: page
title: 型制約
permalink: /csharp/generic-constraints/
---

# 型制約

型パラメータ `T` には制約を付けることで、`T` に対して呼び出せる操作を増やせます。制約がない `T` は `object` のメンバー（`ToString` など）しか使えませんが、制約を付けることで特定のインターフェイスのメソッドや `new()` などが使えるようになります。

## 学習目標

- 制約なしの `T` で使える操作の限界を説明できる
- `where T : インターフェイス名` で操作を追加できる
- `class`・`struct`・`new()` の各制約の意味を理解できる
- 複数の制約を組み合わせて書ける

## 前提知識

- [ジェネリックメソッド](/unity-csharp-learning/csharp/generic-methods/) を読んでいること

---

## 1. 制約なしの問題

制約がない `T` に対して使える操作は `object` のメンバーだけです。

```csharp
static T Max<T>(T a, T b)
{
    // ❌ コンパイルエラー: T に > 演算子がない
    return a > b ? a : b;

    // ❌ コンパイルエラー: T に CompareTo がない
    return a.CompareTo(b) >= 0 ? a : b;
}
```

比較や特定の操作を行いたい場合は**型制約**で `T` が満たすべき条件を指定します。

---

## 2. インターフェイス制約

**書式：型制約**
```
型パラメータ名 メソッド名<T>(引数) where T : 制約
```

| 要素 | 説明 |
|---|---|
| `where T :` | 型制約の開始キーワード |
| `制約` | インターフェイス名・クラス名・`class`・`struct`・`new()` など |

`where T : IComparable<T>` を付けると、`T` に `CompareTo` メソッドが使えます。

```csharp
static T Max<T>(T a, T b) where T : IComparable<T>
{
    return a.CompareTo(b) >= 0 ? a : b;
}
```

```csharp
int larger = Max(3, 7);        // 7
string later = Max("abc", "xyz");  // xyz
```

```
7
xyz
```

`int` も `string` も `IComparable<T>` を実装しているため、どちらも渡せます。

---

## 3. `class` 制約 / `struct` 制約

`where T : class` は参照型のみを許可します。`null` を扱うことが保証されます。
`where T : struct` は値型のみを許可します。`null` になり得ないことが保証されます。

```csharp
// 参照型のみ受け付ける（null チェックが意味を持つ）
static T RequireNonNull<T>(T value) where T : class
{
    if (value == null) throw new ArgumentNullException(nameof(value));
    return value;
}
```

```csharp
// 値型のみ受け付ける（ボクシングなしで使える）
static T Double<T>(T value) where T : struct
{
    // ...
}
```

---

## 4. `new()` 制約

`where T : new()` は、`T` がパラメータなしのコンストラクタを持つことを要求します。これにより、ジェネリッククラス内で `new T()` と書けるようになります。

```csharp
static T CreateInstance<T>() where T : new()
{
    return new T();
}
```

```csharp
class Config
{
    public int Timeout { get; set; } = 30;
}

Config cfg = CreateInstance<Config>();
Console.WriteLine(cfg.Timeout);  // 30
```

```
30
```

---

## 5. 基底クラス制約

インターフェイスだけでなく、特定のクラスを継承していることを制約にできます。

```csharp
class Processor<T> where T : Stream
{
    public void Process(T stream)
    {
        // T は Stream のメンバー（Read, Write など）を持つことが保証される
        byte[] buffer = new byte[1024];
        stream.Read(buffer, 0, buffer.Length);
    }
}
```

---

## 6. 複数の制約

複数の制約を同時に指定できます。インターフェイス制約と `new()` 制約を組み合わせる例です。

**書式：複数制約**
```
where T : 制約1, 制約2
```

```csharp
static T CloneAndInitialize<T>() where T : ICloneable, new()
{
    T instance = new T();
    return (T)instance.Clone();
}
```

> 💡 **ポイント**: `class` / `struct` 制約は必ず最初に書く必要があります。

複数の型パラメータに個別に制約を付けることもできます。

```csharp
static void Convert<TIn, TOut>(TIn input, out TOut output)
    where TIn : IConvertible
    where TOut : struct
{
    output = (TOut)System.Convert.ChangeType(input, typeof(TOut));
}
```

---

## まとめ

- 制約なしの `T` は `object` のメンバーしか使えない
- `where T : IComparable<T>` のようにインターフェイス制約でメソッドを呼び出せる
- `where T : class` — 参照型のみ許可
- `where T : struct` — 値型のみ許可
- `where T : new()` — `new T()` の呼び出しが可能になる
- 制約は複数組み合わせられる

---

## 理解度チェック

1. 次のメソッドにインターフェイス制約を追加して、コンパイルが通るようにしてください。

   ```csharp
   static void Print<T>(T value)
   {
       Console.WriteLine(value.Length);  // Length がない
   }
   ```

2. `where T : new()` 制約がない場合、`new T()` と書くとどうなりますか？

3. （応用）「参照型であり、かつパラメータなしコンストラクタを持つ」制約の書き方は？

<details markdown="1">
<summary>解答を見る</summary>

1. `Length` を持つ標準インターフェイスはないため、直接はできません。`IEnumerable` などで `Count()` を使うか、独自インターフェイス `interface IHasLength { int Length { get; } }` を定義して `where T : IHasLength` とします。

2. コンパイルエラーになります。`new()` 制約がない `T` に対して `new T()` は書けません。

3. `where T : class, new()`（`class` 制約を先に書く）

</details>

---

## 次のステップ

[共変・反変](/unity-csharp-learning/csharp/generic-variance/) では、ジェネリック型の代入互換性（`IEnumerable<Derived>` を `IEnumerable<Base>` に代入できるか）を制御する `out`・`in` キーワードを学びます。
