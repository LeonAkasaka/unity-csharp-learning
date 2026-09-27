---
layout: page
title: インクリメント・デクリメント（補足）
permalink: /csharp/increment-decrement/
---

# インクリメント・デクリメント（補足）

このページは、[反復処理](/unity-csharp-learning/csharp/loops/) の補足です。変数を 1 増やす `++`（**インクリメント**）と、1 減らす `--`（**デクリメント**）を学びます。特に、`++` を変数の前に書くか後に書くかで、式の値が変わる点に注意します。あわせて、`+=` などの **複合代入演算子** も学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `++` と `--` が何をする演算子かを説明できる
- 後置（`count++`）と前置（`++count`）で、式の値がどう違うかを説明できる
- `+=` などの複合代入演算子を使える

## 前提知識

- [反復処理](/unity-csharp-learning/csharp/loops/) を読んでいること
- [条件演算子と式・文（補足）](/unity-csharp-learning/csharp/conditional-operator/) を読んでいること

---

## 1. インクリメントとデクリメント

[インクリメント演算子](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/arithmetic-operators#increment-operator-) `++` は変数の値を 1 増やし、[デクリメント演算子](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/arithmetic-operators#decrement-operator---) `--` は変数の値を 1 減らします。

**書式：[インクリメント演算子とデクリメント演算子](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/arithmetic-operators#increment-operator-)**
```
変数++
変数--
```

`count++;` は、`count = count + 1;` と同じ結果になります。

```csharp
int count = 0;

count++;
Console.WriteLine(count);

count--;
Console.WriteLine(count);
```

```
1
0
```

`for` 文の更新式でよく使います。

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

---

## 2. 前置と後置

`++` と `--` は、変数の **前** にも書けます。変数の後に書くものを **後置**、前に書くものを **前置** といいます。

**書式：[前置インクリメント演算子と前置デクリメント演算子](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/arithmetic-operators#prefix-increment-operator)**
```
++変数
--変数
```

```csharp
int count = 0;
++count;
Console.WriteLine(count);
```

```
1
```

変数が 1 増えるという点では、後置と同じです。違いが表れるのは、`++count` や `count++` を **式として使ったとき** です。

---

## 3. 前置と後置の違い

[条件演算子と式・文（補足）](/unity-csharp-learning/csharp/conditional-operator/) で学んだように、式は値を持ちます。`count++` と `++count` はどちらも式で、どちらも変数を 1 増やしますが、**式の値** が違います。

| | 式の値 | 変数への影響 |
|---|---|---|
| `count++`（後置） | 1 増やす **前** の値 | 1 増える |
| `++count`（前置） | 1 増やした **後** の値 | 1 増える |

### 変数に代入して確かめる

```csharp
int a = 10;
int resultA = a++;
Console.WriteLine($"resultA={resultA}, a={a}");

int b = 10;
int resultB = ++b;
Console.WriteLine($"resultB={resultB}, b={b}");
```

```
resultA=10, a=11
resultB=11, b=11
```

`a` と `b` は、どちらも 11 に増えています。しかし、後置の `a++` の値は増やす前の `10`、前置の `++b` の値は増やした後の `11` なので、代入された値が違います。

### Console.WriteLine に直接渡して確かめる

```csharp
int x = 5;
Console.WriteLine(x++);
Console.WriteLine(x);

int y = 5;
Console.WriteLine(++y);
Console.WriteLine(y);
```

```
5
6
6
6
```

`x++` は、`x` を 6 に増やしますが、式の値は増やす前の `5` なので、`5` が表示されます。`++y` は、`y` を 6 に増やし、式の値も増やした後の `6` です。

### 評価される順序

後置の `x++` は、次の順に処理されます。

1. 今の `x` の値（`5`）を、式の値として取っておく
2. `x` に 1 を足す（`x` は `6` になる）
3. 取っておいた `5` が、式の値になる

前置の `++y` は、次の順に処理されます。

1. `y` に 1 を足す（`y` は `6` になる）
2. 足した後の `y` の値（`6`）が、式の値になる

### 単独の文として使う場合は同じ

`count++;` や `++count;` のように、式の値を使わずに単独の文として書く場合は、どちらでも結果は同じです。`for` 文の更新式 `i++` もこの使い方なので、`++i` と書いても動作は変わりません。

---

## 4. 複合代入演算子

`++` と `--` は 1 だけ増減します。任意の値で計算して代入したいときは、[複合代入演算子](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/assignment-operator#compound-assignment) を使います。

**書式：[複合代入演算子](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/assignment-operator#compound-assignment)**
```
変数 演算子= 式
```

`x += 3` は、`x = x + 3` と同じ意味です。

| 演算子 | 意味 | 例 | 結果（`x` が `10` のとき） |
|---|---|---|---|
| `+=` | 足して代入 | `x += 3` | `13` |
| `-=` | 引いて代入 | `x -= 3` | `7` |
| `*=` | 掛けて代入 | `x *= 3` | `30` |
| `/=` | 割って代入 | `x /= 3` | `3` |
| `%=` | 余りを代入 | `x %= 3` | `1` |

```csharp
int hp = 100;

hp -= 30;
Console.WriteLine(hp);

hp += 10;
Console.WriteLine(hp);
```

```
70
80
```

`+=` は、文字列の連結にも使えます。

```csharp
string message = "Hello";
message += ", World";
Console.WriteLine(message);
```

```
Hello, World
```

---

## よくあるミス

### 前置と後置を取り違える

```csharp
int count = 0;

int result = count++;
Console.WriteLine(result);

int result2 = ++count;
Console.WriteLine(result2);
```

```
0
2
```

増やした後の値 `1` を `result` に入れるつもりで `count++` と書くと、増やす前の `0` が入ります。増やした後の値を使いたいときは、前置の `++count` を使います。2 つ目の `++count` では、`count` はすでに `1` なので、`2` になります。

---

## ワンポイントアドバイス

### 1 つの式の中で ++ を何度も使わない

`x++ + ++x` のように、1 つの式の中で同じ変数に `++` や `--` を何度も使うと、式の値を追うのが難しくなります。C# では計算の順序は決まっているので結果は一定ですが、読む人が間違えやすくなります。`++` と `--` は、単独の文として使うのが安全です。

---

## まとめ

- `++` は変数を 1 増やし、`--` は変数を 1 減らす
- 後置（`count++`）の式の値は、増やす前の値
- 前置（`++count`）の式の値は、増やした後の値
- 単独の文として使う場合は、前置でも後置でも結果は同じ
- `+=`・`-=`・`*=`・`/=`・`%=` は、計算した結果を同じ変数に代入する

---

## 理解度チェック

1. 次のコードを実行すると何が出力されますか？

   ```csharp
   int a = 3;
   int b = a++;
   Console.WriteLine(a);
   Console.WriteLine(b);
   ```

2. 次のコードを実行すると何が出力されますか？

   ```csharp
   int x = 3;
   int y = ++x;
   Console.WriteLine(x);
   Console.WriteLine(y);
   ```

3. `hp` が `100` のとき、`hp -= 25;` を 3 回実行した後の `hp` の値はいくつですか？

<details markdown="1">
<summary>解答を見る</summary>

1. 次のように出力されます。後置なので、`b` には増やす前の `3` が代入され、`a` は `4` になります。

   ```
   4
   3
   ```

2. 次のように出力されます。前置なので、`x` が先に `4` になり、その値が `y` に代入されます。

   ```
   4
   4
   ```

3. `25` です。`100 - 25 - 25 - 25` で `25` になります。

</details>

---

## 次のステップ

[break と continue（補足）](/unity-csharp-learning/csharp/break-and-continue/) では、繰り返しを途中で終えたり、1 回分を飛ばしたりする方法を学びます。
