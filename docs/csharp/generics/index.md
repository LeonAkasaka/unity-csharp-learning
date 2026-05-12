---
layout: page
title: ジェネリクスの基本
permalink: /csharp/generics/
---

# ジェネリクスの基本

同じロジックを複数の型に対して再利用したいとき、`object` 型で汎用化する方法は型安全性を失います。**ジェネリクス**は型をパラメータとして受け取ることで、コンパイル時の型チェックを維持したまま再利用できるしくみです。

## 学習目標

- `object` を使った汎用化の問題点を説明できる
- 型パラメータ `<T>` を使ったジェネリッククラスを定義できる
- 具体型を指定してインスタンス化できる
- 複数の型パラメータを持つクラスを書けるようになる

## 前提知識

- [インターフェイスの明示的実装](/unity-csharp-learning/csharp/explicit-interface/) を読んでいること

---

## 1. `object` 型による汎用化の問題

`int` と `string` の両方を格納できるコンテナを作りたいとします。`object` を使えば 1 つのクラスで対応できますが、取り出すときにキャストが必要になり、誤った型のキャストはランタイムエラーになります。

```csharp
class Container
{
    private object _value;

    public void Set(object value) { _value = value; }
    public object Get() { return _value; }
}
```

```csharp
Container c = new Container();
c.Set(42);

// キャストが必要で、間違えると実行時例外になる
int n = (int)c.Get();
string s = (string)c.Get();  // ❌ 実行時に InvalidCastException
```

コンパイラは型の誤りを検出できません。

---

## 2. ジェネリッククラス

**書式：ジェネリッククラスの定義**
```
class クラス名<型パラメータ>
{
    // 型パラメータをフィールドやメソッドの型として使う
}
```

| 要素 | 説明 |
|---|---|
| `<型パラメータ>` | 任意の名前を付けられる。慣例として `T`・`TKey`・`TValue` などが使われる |

```csharp
class Container<T>
{
    private T _value;

    public void Set(T value) { _value = value; }
    public T Get() { return _value; }
}
```

型パラメータ `T` はクラス定義の中で通常の型と同じように使えます。

---

## 3. インスタンス化

**書式：ジェネリッククラスのインスタンス化**
```
クラス名<具体的な型> 変数名 = new クラス名<具体的な型>();
```

```csharp
Container<int> intContainer = new Container<int>();
intContainer.Set(42);
int n = intContainer.Get();  // キャスト不要。int として返る

Container<string> strContainer = new Container<string>();
strContainer.Set("hello");
string s = strContainer.Get();  // string として返る
```

```
// 誤った型を渡すとコンパイルエラーになる
// intContainer.Set("hello");  ❌ コンパイルエラー
```

`object` 版と違い、誤った型の代入は**コンパイル時**に検出されます。

---

## 4. 複数の型パラメータ

型パラメータは複数持てます。2 つの値をペアで保持するクラスの例です。

**書式：複数型パラメータ**
```
class クラス名<型パラメータ1, 型パラメータ2>
```

```csharp
class Pair<T1, T2>
{
    public T1 First { get; }
    public T2 Second { get; }

    public Pair(T1 first, T2 second)
    {
        First = first;
        Second = second;
    }
}
```

```csharp
Pair<int, string> pair = new Pair<int, string>(1, "alpha");
Console.WriteLine(pair.First);   // 1
Console.WriteLine(pair.Second);  // alpha
```

```
1
alpha
```

---

## まとめ

- `object` による汎用化はキャストが必要で型安全でない
- ジェネリッククラスは `class クラス名<T>` の形で定義する
- インスタンス化するとき `new クラス名<具体的な型>()` と書く
- 型の誤りはコンパイル時に検出される
- 型パラメータは複数持てる（`<T1, T2>`）

---

## 理解度チェック

1. `object` を使った汎用化の問題点は何ですか？
2. 次のクラスを `double` 型で使うには、インスタンス化をどう書きますか？

   ```csharp
   class Container<T>
   {
       public T Value { get; set; }
   }
   ```

3. （応用）`Pair<T1, T2>` のインスタンスで `First` と `Second` を入れ替えて返すメソッドを追加してください。

<details markdown="1">
<summary>解答を見る</summary>

1. 取り出すときにキャストが必要で、型が合わなかった場合は実行時例外になります。コンパイラが型の誤りを検出できません。

2. `Container<double> c = new Container<double>();`

3. ```csharp
   public Pair<T2, T1> Swap()
   {
       return new Pair<T2, T1>(Second, First);
   }
   ```

</details>

---

## 次のステップ

[ジェネリックメソッド](/unity-csharp-learning/csharp/generic-methods/) では、クラスではなくメソッド単体に型パラメータを付ける方法と、型推論のしくみを学びます。
