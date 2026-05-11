---
layout: page
title: static メンバーと static クラス
permalink: /csharp/static-members/
---

# static メンバーと static クラス

クラスのメンバーには、各インスタンスごとに持つものと、クラス全体で共有するものがあります。`static` は「インスタンスではなく型に属する」ことを表すキーワードです。

## 学習目標

- インスタンスメンバーと `static` メンバーの違いを説明できる
- `static` フィールドと `static` メソッドの使い方を理解できる
- `static` コンストラクタが 1 回だけ実行されることを説明できる
- `static class` がインスタンス化できないことを理解できる

## 前提知識

- [再帰関数とコールスタック](/unity-csharp-learning/csharp/recursion/) を読んでいること

---

## 1. インスタンスメンバーと `static` メンバーの違い

インスタンスメンバーは、各インスタンスに独立して存在します。`static` メンバーはクラスに属し、すべてのインスタンスで共有されます。

- インスタンスメンバー: `a1` と `a2` が別々の値を持てる
- `static` メンバー: `A.Count` のようにクラス名から直接アクセスできる

インスタンスを生成しなくても使えるのが `static` メンバーの特徴です。

---

## 2. `static` フィールド

**書式：static フィールド**
```
アクセス修飾子 static 型 フィールド名;
```

| 要素 | 説明 |
|---|---|
| `static` | クラスに属するフィールドであることを示す |
| `型` | 保存する値の型 |
| `フィールド名` | 共有される変数の名前 |

`static int Count;` と書くと、すべてのインスタンスから同じ `Count` を見ます。

---

## 3. `static` メソッド

**書式：static メソッド**
```
アクセス修飾子 static 戻り値の型 メソッド名()
{
    処理
}
```

| 要素 | 説明 |
|---|---|
| `static` | クラスに属するメソッドであることを示す |
| `戻り値の型` | メソッドが返す値の型 |
| `メソッド名` | クラス名から呼び出すメソッド名 |

`static` メソッドは `クラス名.メソッド名()` で呼び出します。インスタンスに属さないので、メソッド本体に `this` は存在しません。

---

## 4. `static` コンストラクタ

**書式：static コンストラクタ**
```
static クラス名()
{
    処理
}
```

| 要素 | 説明 |
|---|---|
| `static` | 型初期化用のコンストラクタであることを示す |
| `クラス名` | 対象のクラス名 |

`static` コンストラクタは、その型が最初に使われるときに**一度だけ**自動実行されます。**パラメータなし**、**アクセス修飾子なし**がルールです。

---

## 5. `static class`

**書式：static クラス**
```
static class クラス名
{
    static メンバー
}
```

| 要素 | 説明 |
|---|---|
| `static class` | クラス全体を static にする宣言 |
| `static メンバー` | クラス内に置けるメンバー。すべて static でなければならない |

`static class` はインスタンス化できません。クラスのすべてのメンバーを `static` に限定したいときに使います。

---

## 6. 実行例

`Id` はインスタンスフィールド、`Count` は `static` フィールドです。それぞれの振る舞いの違いをまとめて確認します。

```csharp
class A
{
    public int Id;                     // インスタンスフィールド（各インスタンスが独立して持つ）
    public static int Count = 0;       // static フィールド（全インスタンスで共有される）

    static A()
    {
        Console.WriteLine("A.static_ctor");
    }

    public A(int id) { Id = id; Count++; }

    public static void M()
    {
        Console.WriteLine($"A.M: Count={Count}");
    }
}

A.M();                      // インスタンス不要
var a1 = new A(1);
var a2 = new A(2);
A.M();                      // Count=2
Console.WriteLine($"a1.Id={a1.Id}, a2.Id={a2.Id}");  // Id は独立
```

```
A.static_ctor
A.M: Count=0
A.M: Count=2
a1.Id=1, a2.Id=2
```

`Count` は `a1`・`a2` の両方の生成で更新されていますが、`Id` は `a1` と `a2` でそれぞれ独立した値を持っていることがわかります。`static` コンストラクタは最初の `A.M()` 呼び出し時に一度だけ実行されます。

### `static class` の例

```csharp
static class B
{
    public static void M() { Console.WriteLine("B.M"); }
}

B.M();
// new B(); // ❌ インスタンス化できない
```

```
B.M
```

---

## よくあるミス

### ミス①：`static` メソッドの中でインスタンスメンバーに直接アクセスする

```csharp
class A
{
    public int Value;

    // ❌ NG: static メソッドの中に this はない
    // public static void M() { Console.WriteLine(this.Value); }

    // ✅ OK: static メソッドでは static メンバーだけを直接使う
    public static int Count;
    public static void M() { Console.WriteLine($"A.M: Count={Count}"); }
}
```

### ミス②：`static` コンストラクタにアクセス修飾子を書く

```csharp
class A
{
    // ❌ NG: static コンストラクタに public/private は書けない
    // public static A() { Console.WriteLine("A.static_ctor"); }

    // ✅ OK: アクセス修飾子なしで書く
    static A() { Console.WriteLine("A.static_ctor"); }
}
```

---

## まとめ

- インスタンスメンバーは各インスタンスに属し、`static` メンバーはクラスに属する
- `static` フィールドはすべてのインスタンスで共有される
- `static` メソッドは `クラス名.メソッド名()` で呼び出す
- `static` コンストラクタは型が最初に使われるときに 1 回だけ実行される
- `static class` はインスタンス化できず、メンバーはすべて `static` でなければならない

---

## 理解度チェック

以下の問いに答えられるか確認しましょう。

1. インスタンスメンバーと `static` メンバーの違いは何ですか？
2. 次のコードの出力結果は何になりますか？

   ```csharp
   class A
   {
       public static int Count = 1;

       public A() { Count++; }

       public static void M()
       {
           Console.WriteLine($"A.M: Count={Count}");
       }
   }

   A.M();
   var a = new A();
   A.M();
   ```

3. `static class B` の中に `public static void M()` を定義し、`"B.M"` を表示するコードを書いてください。

<details markdown="1">
<summary>解答を見る</summary>

1. インスタンスメンバーは各オブジェクトごとに持つ値で、`static` メンバーはクラス全体で共有される値です。
2. 
   ```
   A.M: Count=1
   A.M: Count=2
   ```
3. ```csharp
   static class B
   {
       public static void M()
       {
           Console.WriteLine("B.M");
       }
   }

   B.M();
   ```

</details>

---

## 次のステップ

[拡張メソッド](/unity-csharp-learning/csharp/extension-methods/) では、既存の型にメソッドを追加したように見せる書き方を学びます。
