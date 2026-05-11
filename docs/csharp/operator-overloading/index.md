---
layout: page
title: 演算子のオーバーロード
permalink: /csharp/operator-overloading/
---

# 演算子のオーバーロード

`int` 同士なら `+` や `==` をそのまま使えますが、自作クラスでは最初から同じ書き方ができるとは限りません。演算子のオーバーロードを使うと、自作クラスに演算子の動作を定義できます。

## 学習目標

- 自作クラスで演算子がそのまま使えない理由を説明できる
- `public static A operator +(A a, A b)` の構文を理解できる
- `a + b` が内部では特別なメソッド呼び出しに変換されることを説明できる
- 比較演算子にペア定義の義務があることを理解できる

## 前提知識

- [オーバーロード解決](/unity-csharp-learning/csharp/overload-resolution/) を読んでいること

---

## 1. 自作クラスはそのままでは `+` で足せない

`int` には `+` が定義されているので、次のように書けます。

```csharp
int x = 1;
int y = 2;
Console.WriteLine(x + y);
```

```
3
```

一方、自作クラス `A` を作っただけでは `a + b` とは書けません。`+` の動作をコンパイラが知らないからです。

---

## 2. 演算子オーバーロードの基本構文

**書式：演算子オーバーロード**
```
public static 戻り値の型 operator 演算子(型 左辺, 型 右辺)
{
    処理
}
```

| 要素 | 説明 |
|---|---|
| `public` | 演算子を外部から使えるようにする |
| `static` | 演算子がクラスに属することを示す |
| `戻り値の型` | 演算の結果として返す型 |
| `operator` | 演算子オーバーロードであることを示すキーワード |
| `演算子` | `+`、`==` など定義したい演算子 |
| `左辺` / `右辺` | 演算子の左右に来る値 |

演算子オーバーロードは **`public` かつ `static` が必須** です。戻り値の型は任意ですが、`+` のような演算では同じ型を返すことが多いです。

---

## 3. `a + b` は内部でメソッド呼び出しに変換される

`a + b` と書くと、コンパイラはそれを演算子用の特別なメソッド呼び出しに変換します。`+` の場合は IL レベルで `op_Addition` という名前として表現されます。

```text
A.op_Addition(a, b)
```

これは「演算子も最終的にはメソッドとして扱われる」ということです。`==` なら `op_Equality`、`!=` なら `op_Inequality` に対応します。

---

## 4. 比較演算子はペアで定義する

比較演算子にはペアで定義しなければならないものがあります。

- `==` を定義したら `!=` も必要
- `<` を定義したら `>` も必要
- `<=` を定義したら `>=` も必要

片方だけ書くとコンパイルエラーになります。

---

## 5. オーバーロードできる演算子の例

代表的なものは次の通りです。

- `+`
- `-`
- `*`
- `/`
- `==`
- `!=`
- `<`
- `>`

ただし、どの演算子でも自由に定義できるわけではありません。C# が許可している演算子だけをオーバーロードできます。

---

## 6. 実行例

```csharp
class A
{
    public int Value;

    public A(int v) { Value = v; }

    public static A operator +(A a, A b)
    {
        Console.WriteLine("A.op_+");
        return new A(a.Value + b.Value);
    }

    public static bool operator ==(A a, A b)
    {
        Console.WriteLine("A.op_==");
        return a.Value == b.Value;
    }

    public static bool operator !=(A a, A b)
    {
        Console.WriteLine("A.op_!=");
        return a.Value != b.Value;
    }
}

var x = new A(1);
var y = new A(2);
var z = x + y;
Console.WriteLine(z.Value);

Console.WriteLine(x == y);
Console.WriteLine(x != y);
```

```
A.op_+
3
A.op_==
False
A.op_!=
True
```

`x + y` が `A` を返し、`x == y` と `x != y` が `bool` を返していることを確認できます。

---

## よくあるミス

### ミス①：`==` だけ定義して `!=` を忘れる

```csharp
class A
{
    public int Value;

    // ❌ NG: == だけではコンパイルエラー
    // public static bool operator ==(A a, A b) { return true; }

    // ✅ OK: != もセットで定義する
    public static bool operator ==(A a, A b) { return a.Value == b.Value; }
    public static bool operator !=(A a, A b) { return a.Value != b.Value; }
}
```

### ミス②：`static` を付け忘れる

```csharp
class A
{
    // ❌ NG: 演算子オーバーロードは static 必須
    // public A operator +(A a, A b) { return new A(); }

    // ✅ OK: public static で定義する
    public static A operator +(A a, A b) { return new A(); }

    public A() { Console.WriteLine("A.A"); }
}
```

---

## まとめ

- 自作クラスは、そのままでは `+` や `==` を使えない
- 演算子オーバーロードは `public static 戻り値の型 operator ...` の形で定義する
- `a + b` は内部で演算子用メソッド呼び出しに変換される
- `==` と `!=`、`<` と `>`、`<=` と `>=` はペアで定義する必要がある
- `+`、`-`、`*`、`/`、`==`、`!=` などが代表的な対象である

---

## 理解度チェック

以下の問いに答えられるか確認しましょう。

1. 演算子オーバーロードが `public static` でなければならないのはなぜですか？
2. 次のコードの出力結果は何になりますか？

   ```csharp
   class A
   {
       public int Value;

       public A(int v) { Value = v; }

       public static A operator +(A a, A b)
       {
           Console.WriteLine("A.op_+");
           return new A(a.Value + b.Value);
       }
   }

   var x = new A(4);
   var y = new A(5);
   var z = x + y;
   Console.WriteLine(z.Value);
   ```

3. `A` の `Value` 同士を比較して `==` と `!=` を定義してください。メソッド本体では `"A.op_=="` と `"A.op_!="` を表示するようにしてください。

<details markdown="1">
<summary>解答を見る</summary>

1. 演算子はインスタンスではなく型に属する仕組みとして扱われるためです。C# の仕様として `public static` が必須です。
2. 
   ```
   A.op_+
   9
   ```
3. ```csharp
   class A
   {
       public int Value;

       public A(int v) { Value = v; }

       public static bool operator ==(A a, A b)
       {
           Console.WriteLine("A.op_==");
           return a.Value == b.Value;
       }

       public static bool operator !=(A a, A b)
       {
           Console.WriteLine("A.op_!=");
           return a.Value != b.Value;
       }

   }

   var x = new A(1);
   var y = new A(2);
   Console.WriteLine(x == y);
   Console.WriteLine(x != y);
   ```

   ```
   A.op_==
   False
   A.op_!=
   True
   ```

</details>

---

## 次のステップ

[再帰関数とコールスタック](/unity-csharp-learning/csharp/recursion/) では、メソッドが自分自身を呼び出す処理と、そのときのメモリの積み重なり方を学びます。
