---
layout: page
title: 条件演算子と式・文（補足）
permalink: /csharp/conditional-operator/
---

# 条件演算子と式・文（補足）

`if` 文の次に学ぶ `? :` 演算子（条件演算子）は、`if` と似ていますが根本的に異なります。その違いを理解するには「**式（expression）**」と「**文（statement）**」の区別が欠かせません。

## 学習目標

- 式と文の違いを説明できる
- `if` が文であり値を持たないことを理解できる
- 条件演算子 `? :` を使って条件に応じた値を得られる
- `if` 文と条件演算子の使い分けができる

## 前提知識

- [条件分岐](/unity-csharp-learning/csharp/conditionals/) を読んでいること

---

## 1. 式と文

### 式（expression）— 値になるもの

**式**とは、評価すると**値が得られる**コードのことです。

```csharp
3 + 2        // 値: 5
score * 0.8  // 値: score の 80%
"Hello"      // 値: "Hello" という文字列
true         // 値: true
```

式は値を持つので、変数に代入したりメソッドに渡したりできます。

```csharp
int result = 3 + 2;           // 式 3+2 を変数に代入
Console.WriteLine(score * 2); // 式 score*2 をメソッドに渡す
```

### 文（statement）— 処理を行うもの

**文**とは、**処理を実行する**コードのことです。文は値を持ちません。

```csharp
int score = 100;     // 変数宣言文
score = 200;         // 代入文
Console.WriteLine(); // 式文（メソッド呼び出しを文として使う）
```

### if は文

`if` は「処理を実行する」構造であり、**文**です。値を持ちません。

```csharp
// ❌ NG: if は値を持たないので代入できない
var label = if (score >= 60) { "合格" } else { "不合格" };
```

このようなコードは C# では書けません。`if` で条件によって変数に値を入れたい場合は、次のように書きます。

```csharp
int score = 75;
string label;

if (score >= 60)
{
    label = "合格";
}
else
{
    label = "不合格";
}

Console.WriteLine(label);  // 合格
```

これは正しく動きますが、「条件によって値を選ぶ」という目的に対してコードが長くなります。

---

## 2. 条件演算子

**書式：条件演算子**
```
条件式 ? 真のときの値 : 偽のときの値
```

| 要素 | 説明 |
|---|---|
| `条件式` | `bool` 型になる式 |
| `真のときの値` | 条件式が `true` のとき、この式の値が全体の値になる |
| `偽のときの値` | 条件式が `false` のとき、この式の値が全体の値になる |

条件演算子全体が**1 つの式**になり、値を持ちます。

```csharp
int score = 75;
string label = score >= 60 ? "合格" : "不合格";

Console.WriteLine(label);  // 合格
```

`score >= 60` が `true` なので `"合格"` が選ばれ、`label` に代入されます。

`if` 文との比較で見ると、書いている内容はまったく同じです。

```csharp
// if 文バージョン（文）
string label;
if (score >= 60)
    label = "合格";
else
    label = "不合格";

// 条件演算子バージョン（式）
string label = score >= 60 ? "合格" : "不合格";
```

### メソッドの引数に直接渡す

条件演算子は式なので、値が期待される場所ならどこにでも書けます。

```csharp
int score = 40;
Console.WriteLine(score >= 60 ? "合格" : "不合格");  // 不合格
```

---

## 3. if 文と条件演算子の使い分け

| | `if` 文 | 条件演算子 `? :` |
|---|---|---|
| 種別 | 文（値なし） | 式（値あり） |
| 目的 | 処理の切り替え | 値の選択 |
| 複数行の処理 | ✅ 得意 | ❌ 向かない |
| 値として使う | ❌ できない | ✅ できる |

**条件演算子が向いている場面** — 「条件によって値を選ぶ」だけの場合

```csharp
// ✅ シンプルで読みやすい
string message = isLoggedIn ? "ようこそ" : "ログインしてください";
int abs = x >= 0 ? x : -x;  // 絶対値
```

**`if` 文が向いている場面** — 分岐内で複数の処理が必要な場合

```csharp
// ✅ 処理が複数あるので if 文が適切
if (score >= 60)
{
    Console.WriteLine("合格");
    SendCertificate();
    UpdateDatabase();
}
else
{
    Console.WriteLine("不合格");
    SendRetryNotice();
}
```

---

## よくあるミス

### 真・偽の値の型が一致していない

```csharp
// ❌ NG: int と string は型が違うのでコンパイルエラー
var result = score >= 60 ? 1 : "不合格";

// ✅ OK: 両方 string に揃える
string result = score >= 60 ? "合格" : "不合格";
```

条件演算子の真・偽の値は型が一致している必要があります。

---

## まとめ

- **式**は評価すると値が得られるコード。変数への代入やメソッドの引数に使える
- **文**は処理を実行するコード。値を持たない。`if` 文は「文」
- **条件演算子 `? :`** は「条件によって値を選ぶ」式。`if` 文の代わりに使える
- 分岐内で複数の処理が必要なら `if` 文、値を選ぶだけなら条件演算子が読みやすい

---

## 理解度チェック

1. 次の式の値は何になりますか？

   ```csharp
   int x = 10;
   string result = x % 2 == 0 ? "偶数" : "奇数";
   ```

2. 次のコードを条件演算子 1 行で書き直してください。

   ```csharp
   int score = 85;
   string grade;
   if (score >= 70)
       grade = "合格";
   else
       grade = "不合格";
   ```

3. 次のコードはコンパイルエラーになります。なぜですか？

   ```csharp
   int value = 5;
   var result = value > 0 ? "正" : 0;
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. `"偶数"`。`10 % 2 == 0` は `true` なので左側の `"偶数"` が選ばれる。

2. ```csharp
   string grade = score >= 70 ? "合格" : "不合格";
   ```

3. 真のときの値 `"正"` は `string` 型、偽のときの値 `0` は `int` 型で、型が一致していないためコンパイルエラーになる。

</details>

---

## 次のステップ

[ループ](/unity-csharp-learning/csharp/loops/) では、`for` と `while` を使った繰り返し処理を学びます。
