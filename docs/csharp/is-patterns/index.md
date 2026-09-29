---
layout: page
title: パターンと is 演算子
permalink: /csharp/is-patterns/
---

# パターンと is 演算子

**パターンマッチング**（pattern matching）は、値が決まった形（**パターン**）に当てはまるかを調べる仕組みです。[型変換と型チェック](/unity-csharp-learning/csharp/type-casting/) の型パターンも、その 1 つでした。このページでは、`is` 演算子で値そのものを調べるパターンを、`if` 文の条件と比べながら学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `is` 演算子とパターンの関係を説明できる
- 定数パターンで値や `null` を調べ、`is null` と `== null` の違いを説明できる
- 関係パターンと論理パターンを書き、`if` の条件と比べてどこが簡潔になるかを説明できる
- 型パターンと値のパターンを組み合わせられる
- `not` の優先順位と、パターンには定数しか書けないことを説明できる

## 前提知識

- [演算子のオーバーロード](/unity-csharp-learning/csharp/operator-overloading/) を読んでいること
- [型変換と型チェック](/unity-csharp-learning/csharp/type-casting/) を読んでいること
- [null 許容参照型](/unity-csharp-learning/csharp/nullable-reference-types/) を読んでいること

---

## 1. パターンと is 演算子

[型変換と型チェック](/unity-csharp-learning/csharp/type-casting/) では、`c is Player p` と書いて、`c` が `Player` として扱えるかを調べました。`is` の後ろに書いた `Player p` が、型を調べるパターン（型パターン）です。

**書式：[is 演算子](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/is)**
```
式 is パターン
```

| 要素 | 説明 |
|---|---|
| `式` | 調べる値 |
| `パターン` | 値が当てはまるかを調べる形 |

`式 is パターン` は、`式` の値がパターンに当てはまれば `true`、当てはまらなければ `false` になります。`is` の後ろには、型パターンのほかにも、いろいろなパターンを書けます。このページでは、値そのものを調べる 3 つのパターン（定数パターン・関係パターン・論理パターン）を学びます。

---

## 2. 定数パターン

**書式：[定数パターン](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/patterns#constant-pattern)**
```
式 is 定数
```

`is` の後ろに定数を書くと、値がその定数と等しいかを調べます。これを **定数パターン**（constant pattern）といいます。数値・文字・文字列・列挙型の値のほか、`null` も書けます。

```csharp
int count = 0;
string name = "Alice";

Console.WriteLine(count is 0);
Console.WriteLine(count == 0);

Console.WriteLine(name is "Bob");
Console.WriteLine(name == "Bob");
```

```
True
True
False
False
```

`count is 0` と `count == 0` は、同じ結果です。`int` や `string` の値では、定数パターンと `==` に違いはありません。

### is null と == null の違い

違いが出るのは、`null` を調べるときです。[演算子のオーバーロード](/unity-csharp-learning/csharp/operator-overloading/) で学んだように、クラスは `==` の意味を自分で決められます。次の `Money` クラスは、「金額 0 のお金は、`null` と等しい」とみなすように `==` を定義しています。

```csharp
Money? zero = new Money(0);

Console.WriteLine(zero == null);
Console.WriteLine(zero is null);

class Money
{
    public int Amount { get; }

    public Money(int amount)
    {
        Amount = amount;
    }

    // 金額 0 のお金は、null と等しいとみなす
    public static bool operator ==(Money? a, Money? b)
    {
        return (a?.Amount ?? 0) == (b?.Amount ?? 0);
    }

    public static bool operator !=(Money? a, Money? b)
    {
        return !(a == b);
    }

    public override bool Equals(object? obj)
    {
        return obj is Money m && this == m;
    }

    public override int GetHashCode()
    {
        return Amount;
    }
}
```

```
True
False
```

`zero` にはインスタンスが入っているのに、`zero == null` は `True` です。`==` は、`Money` クラスが定義した演算子を呼び出すからです。

`zero is null` は `False` です。`null` の定数パターンは、演算子のオーバーロードを使わずに、変数がどのオブジェクトも指していないかを直接調べます。「変数に `null` が入っているか」を調べたいときは、クラスの `==` の定義に左右されない `is null` を使うのが確実です。

---

## 3. 関係パターン

**書式：[関係パターン](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/patterns#relational-patterns)**
```
式 is < 定数
式 is <= 定数
式 is > 定数
式 is >= 定数
```

`<` `<=` `>` `>=` と定数を組み合わせたパターンを、**関係パターン**（relational pattern）といいます。値が定数より小さいか、大きいかを調べます。

```csharp
int temperature = -3;

Console.WriteLine(temperature is < 0);
Console.WriteLine(temperature < 0);
```

```
True
True
```

関係パターンを 1 つだけ使うなら、比較演算子で書いた条件と変わりません。関係パターンが役に立つのは、次の論理パターンで、ほかのパターンと組み合わせるときです。

---

## 4. 論理パターン

**書式：[論理パターン](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/patterns#logical-patterns)**
```
式 is パターン1 and パターン2
式 is パターン1 or パターン2
式 is not パターン
```

| 要素 | 当てはまる条件 |
|---|---|
| `and` | 両方のパターンに当てはまる |
| `or` | どちらかのパターンに当てはまる |
| `not` | パターンに当てはまらない |

`and`・`or`・`not` でパターンを組み合わせたものを、**論理パターン**（logical pattern）といいます。

### 同じ値を何度も書かなくてよい

論理演算子の `&&` と `||` で書いた条件と比べます。

```csharp
char c = 'q';
Console.WriteLine(c >= 'a' && c <= 'z');
Console.WriteLine(c is >= 'a' and <= 'z');

int day = 6;
Console.WriteLine(day == 6 || day == 7);
Console.WriteLine(day is 6 or 7);
```

```
True
True
True
True
```

結果は同じです。違いは書き方にあります。`&&` や `||` は、`c >= 'a'` のような `bool` の式どうしをつなぐので、調べる値 `c` を式ごとに書きます。`and` や `or` は、`>= 'a'` のようなパターンどうしをつなぐので、調べる値は `is` の前に 1 回だけ書きます。「`c` が `'a'` 以上 `'z'` 以下」「`day` が 6 か 7」という意図が、そのまま読み取れます。

### 調べる式が 1 回だけ評価される

調べる値がメソッドの呼び出しのときは、結果にも違いが出ます。

```csharp
Console.WriteLine(GetDay() == 6 || GetDay() == 7);
Console.WriteLine("---");
Console.WriteLine(GetDay() is 6 or 7);

int GetDay()
{
    Console.WriteLine("GetDay を呼び出した");
    return 7;
}
```

```
GetDay を呼び出した
GetDay を呼び出した
True
---
GetDay を呼び出した
True
```

`||` で書いた条件では、`GetDay()` を 2 回書いたので、2 回呼び出されます。`is` の前に書いた式は 1 回だけ評価され、その値がパターンと比べられます。`||` で書くなら、呼び出しの結果をいったん変数に入れる必要があります。

### not

`not` は、パターンに当てはまらないときに当てはまります。`null` でないことを調べる `is not null` は、よく使う書き方です。

```csharp
string? text = FindName(3);

if (text is not null)
{
    Console.WriteLine(text.Length);
}
else
{
    Console.WriteLine("見つからない");
}

string? FindName(int id)
{
    if (id == 1)
    {
        return "勇者";
    }
    return null;
}
```

```
見つからない
```

2 節の `is null` と同じく、`is not null` はクラスの `!=` の定義に左右されません。[null 許容参照型](/unity-csharp-learning/csharp/nullable-reference-types/) で学んだフロー解析は、`is not null` で確かめた後にも働くので、`if` の中では `text.Length` を警告なしで使えます。`null` でない値を、`?` の付かない型の変数に取り出す書き方は、[プロパティパターン](/unity-csharp-learning/csharp/property-patterns/) で学びます。

---

## 5. 型パターンと組み合わせる

論理パターンでは、型パターンと値のパターンも組み合わせられます。

```csharp
object o = 5;

Console.WriteLine(o is int and > 0);
Console.WriteLine(o is not string);

if (o is int n and > 0)
{
    Console.WriteLine($"正の整数 {n}");
}
```

```
True
True
正の整数 5
```

`o is int and > 0` は、「`o` が `int` で、しかも 0 より大きい」というパターンです。型パターンと `&&` で書くと、`o is int n && n > 0` のように、変換した変数を用意してから比べることになります。`o is int n and > 0` と書けば、型と値を調べながら、変換した値を `n` に取り出すこともできます。

`o is not string` は、「`o` が `string` として扱えない」ときに `true` になります。

---

## 6. 優先順位と ( )

論理パターンの優先順位は、`not`、`and`、`or` の順です。`&&` と `||` のように、`and` は `or` より先に組み合わされます。`( )` で囲むと、組み合わせる順番を変えられます。

```csharp
int x = 2;

Console.WriteLine(x is not (1 or 2));
Console.WriteLine(x is >= 0 and (< 10 or 100));
```

```
False
True
```

`x is not (1 or 2)` は、「1 か 2」に当てはまらないときに `true` です。`x` は `2` なので `False` になります。`( )` を付けずに `x is not 1 or 2` と書くと、意味が変わります（「よくあるミス」を参照）。

---

## 7. パターンには定数しか書けない

パターンに書く値は、`0` や `"Alice"` のように、プログラムを実行する前に決まる定数でなければなりません。変数は書けません。

```csharp
// ❌ NG: パターンに変数は書けない
// int passLine = 60;
// int score = 70;
// Console.WriteLine(score is >= passLine);  // CS9135
```

変数と比べたいときは、比較演算子を使って `score >= passLine` と書きます。`switch` 式でパターンと変数の条件を組み合わせる方法は、[switch 式](/unity-csharp-learning/csharp/switch-expressions/) で学びます。

---

## よくあるミス

### not の範囲を ( ) で囲まない

```csharp
// ❌ NG: (not 1) or 2 と解釈される
// int x = 2;
// Console.WriteLine(x is not 1 or 2);  // True（警告 CS9336）
```

`not` は `or` より先に組み合わされるので、`x is not 1 or 2` は「1 ではない、または 2」という意味です。`x` が `2` のときは `2` に当てはまるので、`True` になります。「1 でも 2 でもない」と書くつもりなら、`x is not (1 or 2)` と `( )` で囲みます。このパターンは、`not 1` だけで `2` も当てはまるので、コンパイラーが「パターンが冗長」という警告 CS9336 を出します。

### and の右に、パターンではない式を書く

```csharp
// ❌ NG: and の右は、b > 0 という式ではなくパターンとして読まれる
// int a = 1;
// int b = 2;
// Console.WriteLine(a is > 0 and b > 0);  // CS9135
```

`and` と `or` がつなぐのは、同じ値に対するパターンです。別の変数の条件をつなぐときは、`a is > 0 && b > 0` のように、`&&` や `||` を使います。

---

## まとめ

- `式 is パターン` は、値がパターンに当てはまれば `true` になる。パターンには、型パターンのほかに、値を調べるパターンがある
- 定数パターン `is 定数` は、値が定数と等しいかを調べる。`is null` は、クラスの `==` の定義に左右されずに `null` かを調べる
- 関係パターン `is < 定数` などは、値の大小を調べる
- 論理パターン `and` / `or` / `not` でパターンを組み合わせると、調べる値を 1 回だけ書けばよく、その式も 1 回だけ評価される
- `o is int n and > 0` のように、型パターンと値のパターンを組み合わせられる
- 優先順位は `not`、`and`、`or` の順。`x is not (1 or 2)` のように、`( )` で範囲を決める
- パターンには定数しか書けない。変数と比べるときは比較演算子を使う

---

## 理解度チェック

1. 次のコードを実行すると何が出力されますか？

   ```csharp
   int[] values = { -1, 0, 5, 10 };

   foreach (int v in values)
   {
       Console.WriteLine(v is >= 0 and < 10);
   }
   ```

2. 次の条件を、論理パターンを使って書き直してください。

   ```csharp
   if (month == 12 || month == 1 || month == 2)
   ```

3. `int x = 5;` のとき、`x is not 5 or 6` と `x is not (5 or 6)` の値は、それぞれ何ですか？
4. 変数 `m` に `null` が入っているかを調べるときに、`m == null` ではなく `m is null` と書くと、どんな違いがありますか？

<details markdown="1">
<summary>解答を見る</summary>

1. 次のように出力されます。`10` は `< 10` に当てはまらないので `False` です。

   ```
   False
   True
   True
   False
   ```

2. `if (month is 12 or 1 or 2)` です。
3. `x is not 5 or 6` は `False`、`x is not (5 or 6)` も `False` です。`x is not 5 or 6` は `(not 5) or 6` と解釈され、`5` は `not 5` にも `6` にも当てはまらないので `False` になります。この 2 つは `x` が `5` のときはたまたま同じ結果ですが、`x` が `6` のときは `True` と `False` に分かれます。
4. `m == null` は、`m` の型が `==` をオーバーロードしていると、その演算子の定義で結果が決まります。`m is null` は、演算子のオーバーロードを使わずに、`m` がどのオブジェクトも指していないかを直接調べます。

</details>

---

## 次のステップ

[switch 式](/unity-csharp-learning/csharp/switch-expressions/) では、このページのパターンを並べて、値に応じて結果を選ぶ `switch` 式を学びます。
