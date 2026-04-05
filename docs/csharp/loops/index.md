---
layout: page
title: ループ
permalink: /csharp/loops/
---

# ループ

同じ処理を繰り返すことを**ループ**（反復）と呼びます。C# には `while`・`do-while`・`for`・`foreach` の 4 種類のループ構文があります。

## 学習目標

- `while` 文で条件が成立する間ループできる
- 無限ループの仕組みと危険性を理解し、`break` で脱出できる
- `do-while` 文と `while` 文の違いを説明できる
- `for` 文の 3 つの部分（初期化・条件・更新）を理解して書ける
- `foreach` 文で文字列の各文字を走査できる

## 前提知識

- [条件分岐](/unity-csharp-learning/csharp/conditionals/) を読んでいること

---

## 1. while 文

**書式：while 文**
```
while (条件式)
{
    繰り返す処理
}
```

| 要素 | 説明 |
|---|---|
| `条件式` | `bool` 型になる式。`true` の間ループを続ける |
| `{ }` | 毎回実行されるブロック（ループ本体） |

実行の流れは次のとおりです。

1. 条件式を評価する
2. `true` ならブロックを実行して 1 に戻る
3. `false` になったらループを抜ける

```csharp
int count = 0;

while (count < 3)
{
    Console.WriteLine(count);
    count = count + 1;
}
```

```
0
1
2
```

`count` が `0`・`1`・`2` のときは `count < 3` が `true` なのでループが続きます。`count` が `3` になると `false` になり、ループを抜けます。

> 💡 **ポイント**: `count = count + 1` は `count++` と書くこともできます（インクリメント演算子）。

---

## 2. 無限ループと break

条件式に `true` を直接書くと、条件が永遠に成立するため**無限ループ**になります。

```csharp
while (true)
{
    Console.WriteLine("永遠に繰り返す");
}
```

このコードはプログラムが止まらなくなります。**無限ループはプログラムをフリーズさせる原因**になるため、意図しない無限ループには注意が必要です。

### break — ループを強制終了する

`break` 文を使うと、ループの途中で強制的に抜け出せます。`while (true)` と組み合わせることで「ループの先頭ではなく途中で終了条件を判定する」構造が書けます。

**書式：break 文**
```
break;
```

```csharp
int count = 0;

while (true)
{
    if (count >= 3)
    {
        break;  // count が 3 以上になったらループを抜ける
    }
    Console.WriteLine(count);
    count++;
}
```

```
0
1
2
```

### 無限ループになりやすいミス

条件式の変数を更新し忘れると、条件が永遠に `true` のままになり無限ループになります。

```csharp
int count = 0;

// ❌ NG: count を更新していないので永遠に count < 3 が true
while (count < 3)
{
    Console.WriteLine(count);
    // count++ を書き忘れている
}
```

ループ変数を更新する処理を必ず書くようにしましょう。

---

## 3. do-while 文

**書式：do-while 文**
```
do
{
    繰り返す処理
} while (条件式);
```

`while` 文との違いは**条件判定のタイミング**です。

| | 条件判定 | ループ本体の最低実行回数 |
|---|---|---|
| `while` | ループの**前** | 0 回（最初から false なら実行されない） |
| `do-while` | ループの**後** | **1 回**（必ず 1 回は実行される） |

```csharp
int count = 5;

do
{
    Console.WriteLine(count);
    count++;
} while (count < 3);
```

```
5
```

`count` が最初から `5` で `count < 3` は `false` ですが、条件チェックはループ本体の**後**なので 1 回だけ実行されます。

---

## 4. for 文

カウンター変数を使ったループでは `while` より `for` がよく使われます。カウンターの初期化・条件・更新を 1 行にまとめて書けるため、ループ変数の更新を忘れにくくなります。

**書式：for 文**
```
for (初期化式; 条件式; 更新式)
{
    繰り返す処理
}
```

| 要素 | 説明 | 実行タイミング |
|---|---|---|
| `初期化式` | ループ変数の準備（例: `int i = 0`） | ループ開始時に **1 回だけ** |
| `条件式` | `true` の間ループを続ける | ループ本体の**前**に毎回 |
| `更新式` | ループ変数の更新（例: `i++`） | ループ本体の**後**に毎回 |

```csharp
for (int i = 0; i < 3; i++)
{
    Console.WriteLine(i);
}
```

```
0
1
2
```

先ほどの `while` で書いたコードとまったく同じ動作です。実行の流れを追うと次のようになります。

```
① int i = 0  （初期化）
② i < 3 → true  （条件チェック）
③ Console.WriteLine(0)  （ループ本体）
④ i++  → i = 1  （更新）
② i < 3 → true
③ Console.WriteLine(1)
④ i++ → i = 2
② i < 3 → true
③ Console.WriteLine(2)
④ i++ → i = 3
② i < 3 → false  → ループ終了
```

### for と while の対応関係

`for` は `while` の書き換えです。どちらも同じ処理を表します。

```csharp
// for 文
for (int i = 0; i < 5; i++)
{
    Console.WriteLine(i);
}

// 同じ処理を while で書いた場合
int i = 0;
while (i < 5)
{
    Console.WriteLine(i);
    i++;
}
```

カウンターを使うループには `for` を、回数が事前に決まっていないループには `while` を使うのが一般的です。

---

## 5. foreach 文

**書式：foreach 文**
```
foreach (型 変数名 in コレクション)
{
    繰り返す処理
}
```

| 要素 | 説明 |
|---|---|
| `型 変数名` | 各要素を受け取る変数。`var` も使える |
| `コレクション` | 複数の要素を持つデータ（文字列・配列・リストなど） |

`foreach` は「コレクションの要素を先頭から 1 つずつ取り出してループする」構文です。カウンター変数が不要なのでシンプルに書けます。

文字列（`string`）は文字（`char`）の並びとして扱えるため、`foreach` で 1 文字ずつ取り出せます。

```csharp
string text = "Hello";

foreach (var ch in text)
{
    Console.WriteLine(ch);
}
```

```
H
e
l
l
o
```

> 💡 **ポイント**: `foreach` の真価は配列やリストで発揮されます。配列については[配列の章]で詳しく学びます。

---

## よくあるミス

### for 文のカウンターの向きを間違える

```csharp
// ❌ NG: i++ で増えているのに i > 0 はすぐ false になり 1 回も実行されない
for (int i = 0; i > 0; i++)
{
    Console.WriteLine(i);
}

// ✅ OK: 0 から始めて 5 未満の間ループ
for (int i = 0; i < 5; i++)
{
    Console.WriteLine(i);
}
```

### foreach でループ変数に代入しようとする

```csharp
string text = "Hello";

foreach (var ch in text)
{
    ch = 'X';  // ❌ コンパイルエラー: foreach 変数は読み取り専用
}
```

`foreach` の変数は読み取り専用です。要素を変更したい場合は `for` でインデックスを使います。

---

## まとめ

- **`while`** — 条件が `true` の間ループ。条件はループ**前**に判定
- **`break`** — ループを強制終了する。無限ループ `while (true)` と組み合わせて使うことも多い
- **`do-while`** — `while` と同じだが条件はループ**後**に判定。必ず 1 回は実行される
- **`for`** — 初期化・条件・更新を 1 行に書けるカウンターループ
- **`foreach`** — コレクションの要素を 1 つずつ取り出すループ。カウンター不要

---

## 理解度チェック

1. 次のコードの出力結果を答えてください。

   ```csharp
   int i = 1;
   while (i <= 4)
   {
       Console.WriteLine(i);
       i += 2;
   }
   ```

2. 次のコードは何回ループしますか？ また、最終的に `count` の値はいくつになりますか？

   ```csharp
   int count = 10;
   do
   {
       count--;
   } while (count > 10);
   ```

3. `for` 文を使って `5` `4` `3` `2` `1` と出力するコードを書いてください。

<details markdown="1">
<summary>解答を見る</summary>

1. `1` と `3`。`i` は `1` → `3` → `5` と変化し、`5` になると `i <= 4` が `false` になってループを抜ける。

2. 1 回。最初に `count--` で `count` が `9` になり、その後 `count > 10` は `false` なのでループを抜ける。最終的な `count` の値は `9`。

3. ```csharp
   for (int i = 5; i >= 1; i--)
   {
       Console.WriteLine(i);
   }
   ```

</details>

---

## 次のステップ

[メソッド](/unity-csharp-learning/csharp/methods/) では、処理をまとめて名前をつけて再利用する方法を学びます。
