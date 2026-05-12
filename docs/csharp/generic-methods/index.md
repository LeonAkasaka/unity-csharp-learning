---
layout: page
title: ジェネリックメソッド
permalink: /csharp/generic-methods/
---

# ジェネリックメソッド

型パラメータはクラス全体ではなく、**メソッド単体**に付けることもできます。特定のメソッドだけを汎用化したいときや、型推論を活用して呼び出し側を簡潔に書きたいときに使います。

## 学習目標

- メソッドに型パラメータを付ける書き方を理解できる
- 型推論により `<T>` の省略ができる場面と条件を説明できる
- ジェネリッククラスのメソッドとの使い分けを判断できる

## 前提知識

- [ジェネリクスの基本](/unity-csharp-learning/csharp/generics/) を読んでいること

---

## 1. ジェネリックメソッドの定義

**書式：ジェネリックメソッドの定義**
```
戻り値の型 メソッド名<型パラメータ>(引数リスト)
{
}
```

| 要素 | 説明 |
|---|---|
| `<型パラメータ>` | メソッド名の直後に書く。戻り値の型や引数の型として使える |

```csharp
static T Identity<T>(T value)
{
    return value;
}
```

```csharp
int n = Identity<int>(42);
string s = Identity<string>("hello");
```

```
42
hello
```

---

## 2. 型推論

引数の型からコンパイラが `T` を推論できる場合、`<T>` を省略できます。

```csharp
int n = Identity(42);       // T = int と推論される
string s = Identity("hi");  // T = string と推論される
```

型推論が効くのは **型パラメータが引数の型に現れる場合**です。戻り値だけに `T` が出てくる場合は推論できないため、明示的に指定が必要になります。

```csharp
// 戻り値にのみ T が使われる例: 推論不可
T Default<T>()
{
    return default;
}

// 呼び出し側は型を明示する必要がある
int zero = Default<int>();
```

---

## 3. 複数のパラメータと戻り値への応用

型パラメータが引数に複数現れる場合も書き方は同じです。

```csharp
static Pair<T1, T2> MakePair<T1, T2>(T1 first, T2 second)
{
    return new Pair<T1, T2>(first, second);
}
```

```csharp
Pair<int, string> p = MakePair(1, "alpha");  // T1=int, T2=string と推論される
```

> 💡 **ポイント**: 型推論は左辺の型ではなく、**渡した引数の型**を見て行われます。

---

## 4. ジェネリッククラスのメソッドとの使い分け

ジェネリッククラスのメソッドが使うのはクラスに付いた `T` です。一方、ジェネリックメソッドはそのメソッド専用の `T` を持ちます。

```csharp
class Container<T>
{
    public T Value { get; set; }

    // クラスの T を使う（Container<int> なら T=int）
    public bool Equals(T other)
    {
        return Value?.Equals(other) ?? false;
    }

    // メソッド専用の型パラメータ U — Container<T> を Container<U> に変換する
    public Container<U> Cast<U>(U newValue)
    {
        return new Container<U> { Value = newValue };
    }
}
```

```csharp
Container<int> intBox = new Container<int> { Value = 42 };

// Cast<U> はメソッド専用の U を持つ
Container<string> strBox = intBox.Cast("hello");
```

使い分けの基準：
- **クラス全体を通じて型を固定したい** → ジェネリッククラス
- **特定のメソッドだけ別の型を扱いたい** → ジェネリックメソッド

---

## よくあるミス

```csharp
// ❌ NG: 型パラメータが引数に現れないのに型推論を期待している
T Zero<T>() => default;
int z = Zero();  // コンパイルエラー: T を推論できない

// ✅ OK: 型を明示する
int z = Zero<int>();
```

---

## まとめ

- ジェネリックメソッドは `戻り値 メソッド名<T>(T arg)` の形で定義する
- 型パラメータが引数に現れる場合、呼び出し側で `<T>` を省略できる（型推論）
- 戻り値にしか `T` が出ない場合は型を明示する必要がある
- クラス全体ではなくメソッド単体を汎用化したいときに使う

---

## 理解度チェック

1. 次のメソッドの呼び出し `Wrap(100)` で型推論は効きますか？ 理由とともに答えてください。

   ```csharp
   static Container<T> Wrap<T>(T value)
   {
       return new Container<T> { Value = value };
   }
   ```

2. 次のメソッドを型推論なしで呼び出す場合の書き方は？

   ```csharp
   static T Clone<T>(T source) { return source; }
   string s = Clone("hello");
   ```

3. （応用）2 つの値の大きい方を返すジェネリックメソッド `Max<T>` を `IComparable<T>` を使って書いてください（型制約は次のページで正式に学びます）。

<details markdown="1">
<summary>解答を見る</summary>

1. 型推論は効きます。引数 `value` の型 `100`（`int`）から `T = int` と推論できます。

2. `string s = Clone<string>("hello");`

3. ```csharp
   static T Max<T>(T a, T b) where T : IComparable<T>
   {
       return a.CompareTo(b) >= 0 ? a : b;
   }
   ```

</details>

---

## 次のステップ

[型制約](/unity-csharp-learning/csharp/generic-constraints/) では、型パラメータに条件を付けることで、`T` に対して使える操作を増やす方法を学びます。
