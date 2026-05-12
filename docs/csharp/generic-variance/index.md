---
layout: page
title: 共変・反変
permalink: /csharp/generic-variance/
---

# 共変・反変

`Container<Derived>` を `Container<Base>` に代入しようとするとコンパイルエラーになります。しかし、インターフェイスの型パラメータに `out` や `in` を付けると、継承関係に応じた代入ができるようになります。この性質を**変性（variance）**と呼びます。

## 学習目標

- ジェネリック型が既定で不変（invariant）であることを説明できる
- `out T` で共変インターフェイスを定義できる
- `in T` で反変インターフェイスを定義できる
- `out` / `in` それぞれで `T` の使える位置が制限される理由を説明できる

## 前提知識

- [型制約](/unity-csharp-learning/csharp/generic-constraints/) を読んでいること
- [継承](/unity-csharp-learning/csharp/inheritance/) を読んでいること

---

## 1. 不変（invariant）

ジェネリッククラスは既定で**不変**です。`T` に継承関係があっても、そのジェネリック型同士に代入互換はありません。

```csharp
class Base { }
class Derived : Base { }
```

```csharp
class Container<T>
{
    public T Value { get; set; }
}
```

```csharp
Container<Derived> d = new Container<Derived>();

// ❌ コンパイルエラー: Container<Derived> は Container<Base> ではない
Container<Base> b = d;
```

`Container<T>` は読み書き両用のため、`Base` を書き込んだコンテナを `Derived` として読み出すのは型安全でないからです。

---

## 2. 共変（covariant）— `out T`

**読み取り専用**のインターフェイスなら、派生型のインスタンスを基底型として扱っても安全です。型パラメータに `out` を付けると共変になります。

**書式：共変インターフェイス**
```
interface インターフェイス名<out T>
{
    T メソッド名();  // T は戻り値にのみ使える
}
```

| 要素 | 説明 |
|---|---|
| `out T` | `T` を出力（戻り値）にしか使えない制約。共変であることを宣言する |

```csharp
interface IReader<out T>
{
    T Read();
}

class DataReader<T> : IReader<T>
{
    private T _data;
    public DataReader(T data) { _data = data; }
    public T Read() { return _data; }
}
```

```csharp
IReader<Derived> derivedReader = new DataReader<Derived>(new Derived());

// ✅ OK: out T があるので IReader<Derived> → IReader<Base> に代入できる
IReader<Base> baseReader = derivedReader;
Base result = baseReader.Read();
```

読み取りしか行わないため、`Derived` を `Base` として読み出しても常に安全です。

---

## 3. 反変（contravariant）— `in T`

**書き込み専用**のインターフェイスなら、基底型を受け付けるハンドラを派生型用として使っても安全です。型パラメータに `in` を付けると反変になります。

**書式：反変インターフェイス**
```
interface インターフェイス名<in T>
{
    void メソッド名(T 引数);  // T は引数にのみ使える
}
```

| 要素 | 説明 |
|---|---|
| `in T` | `T` を入力（引数）にしか使えない制約。反変であることを宣言する |

```csharp
interface IWriter<in T>
{
    void Write(T value);
}

class Logger<T> : IWriter<T>
{
    public void Write(T value) { Console.WriteLine(value); }
}
```

```csharp
IWriter<Base> baseWriter = new Logger<Base>();

// ✅ OK: in T があるので IWriter<Base> → IWriter<Derived> に代入できる
// Base を受け付けるハンドラは Derived も受け付けられる（Derived は Base だから）
IWriter<Derived> derivedWriter = baseWriter;
derivedWriter.Write(new Derived());
```

直感的に捉えると：「`Base` を処理できるなら、`Base` の一種である `Derived` も処理できる」。

---

## 4. 共変・反変・不変のまとめ

| 変性 | キーワード | 代入の方向 | `T` が使える位置 |
|---|---|---|---|
| 不変 | なし | なし | 戻り値・引数どちらでも |
| 共変 | `out T` | 派生 → 基底 | 戻り値のみ |
| 反変 | `in T` | 基底 → 派生 | 引数のみ |

---

## よくあるミス

```csharp
interface IReadWrite<out T>
{
    T Get();
    // ❌ コンパイルエラー: out T は引数には使えない
    void Set(T value);
}
```

`out T` を付けたインターフェイスで `T` を引数として使おうとするとコンパイルエラーになります。

---

## まとめ

- ジェネリック型は既定で不変。継承関係があっても代入互換はない
- `out T`（共変）: `T` を戻り値のみに使える。`IReader<Derived>` → `IReader<Base>` に代入できる
- `in T`（反変）: `T` を引数のみに使える。`IWriter<Base>` → `IWriter<Derived>` に代入できる
- `out` / `in` の制限はコンパイラによる型安全性の保証

---

## 理解度チェック

1. `IReader<out T>` に `void Write(T value)` を追加するとどうなりますか？

2. 次のコードはコンパイルエラーになりますか？ 理由とともに答えてください。

   ```csharp
   interface IWriter<in T>
   {
       T Read();  // 戻り値に T を使っている
   }
   ```

3. （応用）`IReader<Derived>` を `IReader<Base>` に代入できる理由を、型安全性の観点から説明してください。

<details markdown="1">
<summary>解答を見る</summary>

1. コンパイルエラーになります。`out T` は `T` を戻り値にしか使えないため、引数として使う `void Write(T value)` は書けません。

2. コンパイルエラーになります。`in T` は `T` を引数にしか使えないため、戻り値として使う `T Read()` は書けません。

3. `IReader<out T>` は読み取りしか行わないため、`Derived` のインスタンスを `Base` として読み出しても常に安全です（`Derived` は `Base` であることが保証されているため）。書き込みがないので型の不一致が起きる余地がありません。

</details>

---

## 次のステップ

[デリゲートの基本](/unity-csharp-learning/csharp/delegates/) では、メソッドへの参照を変数として扱うデリゲートのしくみを学びます。
