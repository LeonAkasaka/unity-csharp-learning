---
layout: page
title: null 許容値型
permalink: /csharp/nullable-value-types/
---

# null 許容値型

値型の変数には `null` を入れられません。しかし、「まだ値が決まっていない」「値が見つからなかった」のように、値がないことを表したい場面があります。`int?` のように型名の後に `?` を付けた **null 許容値型**（nullable value type）を使うと、値型の値に加えて `null` も入れられます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `int?` などの null 許容値型の変数を使い、値があるかどうかを調べられる
- `int?` の正体が `Nullable<int>` 構造体であることを説明できる
- null 許容値型どうしの演算や比較の結果を説明できる
- `??` 演算子などで、`null` のときの値を決めて取り出せる

## 前提知識

- [値型と参照型](/unity-csharp-learning/csharp/value-reference-types/) を読んでいること
- [構造体](/unity-csharp-learning/csharp/structs/) を読んでいること
- [型制約](/unity-csharp-learning/csharp/generic-constraints/) を読んでいること
- [型変換と型チェック](/unity-csharp-learning/csharp/type-casting/) を読んでいること

---

## 1. 値型で「値がない」を表す

テストの点数を `int` で表すとします。まだ受けていないテストの点数を `0` にすると、「0 点だった」のか「まだ受けていない」のか区別できません。`-1` のような特別な値を決める方法もありますが、その決まりを知らないと、`-1` 点として計算してしまうおそれがあります。

[null 許容値型](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/nullable-value-types) を使うと、値がないことを `null` で表せます。

**書式：null 許容値型**
```
値型?
```

```csharp
int? score = null;
Console.WriteLine(score.HasValue);

score = 80;
Console.WriteLine(score.HasValue);
Console.WriteLine(score.Value);
```

```
False
True
80
```

[HasValue プロパティ](https://learn.microsoft.com/dotnet/api/system.nullable-1.hasvalue) は、値が入っていれば `true`、`null` なら `false` を返します。[Value プロパティ](https://learn.microsoft.com/dotnet/api/system.nullable-1.value) は、入っている値を返します。

`null` のときに `Value` を読むと、[InvalidOperationException](https://learn.microsoft.com/dotnet/api/system.invalidoperationexception) が発生します。`Value` を読む前に、`HasValue` や、4 節の方法で `null` でないことを確かめます。

次のコードは、`null` かもしれない値の `Value` を読んでいるので、ビルドするとコンパイラーが警告（CS8629）を出します。警告が出てもビルドは成功し、実行すると例外が発生します。

```csharp
int? score = null;

try
{
    Console.WriteLine(score.Value);
}
catch (InvalidOperationException e)
{
    Console.WriteLine(e.GetType().Name);
}
```

```
InvalidOperationException
```

---

## 2. int? の正体は Nullable\<int\>

`int?` は、[Nullable\<T\> 構造体](https://learn.microsoft.com/dotnet/api/system.nullable-1) を使った `Nullable<int>` の短い書き方です。`Nullable<T>` は、値そのものと、値があるかどうかを表す `bool` を持つ、ジェネリックな構造体です。

```csharp
Nullable<int> a = 5;
int? b = 5;
Console.WriteLine(a == b);
```

```
True
```

`Nullable<T>` は構造体なので、`int?` も値型です。`null` を入れても参照型になるわけではなく、「`HasValue` が `false` の値」が入ります。

`Nullable<T>` の型パラメータには、[型制約](/unity-csharp-learning/csharp/generic-constraints/) で学んだ `where T : struct` の制約が付いています。そのため、`?` で null 許容値型にできるのは値型だけです。`int??` のように、null 許容値型をさらに null 許容値型にすることもできません。

---

## 3. 演算と比較

null 許容値型どうしでも、元の型と同じ演算子を使えます。どちらかが `null` のとき、算術演算の結果は `null` になります。

```csharp
int? a = 10;
int? b = null;

Console.WriteLine(a + 1);
Console.WriteLine($"[{a + b}]");
```

```
11
[]
```

`Console.WriteLine` や文字列補間に `null` を渡すと、空の文字列として表示されます。ここでは、結果が `null` であることがわかるように `[` と `]` で囲んでいます。

`<` や `>` などの比較では、どちらかが `null` なら、結果は必ず `false` になります。`==` と `!=` では、`null` どうしは等しいとみなされます。

```csharp
int? b = null;

Console.WriteLine(b > 5);
Console.WriteLine(b <= 5);
Console.WriteLine(b == null);
```

```
False
False
True
```

`b > 5` と `b <= 5` がどちらも `false` になる点に注意しましょう。`!(b > 5)` が `true` だからといって、`b <= 5` とは限りません。

---

## 4. 値を取り出す

null 許容値型の値を、元の値型の変数に代入するときは、`null` だった場合の扱いを決める必要があります。そのまま代入することはできません。

```csharp
// ❌ NG: int? を int に暗黙的に変換することはできない
// int? score = 80;
// int n = score;  // CS0266
```

### ?? 演算子

[?? 演算子](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/null-coalescing-operator)（null 合体演算子）は、左辺が `null` でなければ左辺の値を、`null` なら右辺の値を返します。`??=` は、左辺の変数が `null` のときだけ、右辺の値を代入します。

**書式：[?? 演算子と ??= 演算子](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/null-coalescing-operator)**
```
式1 ?? 式2
変数 ??= 式
```

```csharp
int? input = null;

int value = input ?? 0;
Console.WriteLine(value);

input ??= 50;
Console.WriteLine(input);

input ??= 100;
Console.WriteLine(input);
```

```
0
50
50
```

`input` は `null` なので、`input ?? 0` は `0` になります。最初の `input ??= 50` で `50` が代入されます。2 回目の `input ??= 100` では、`input` はもう `null` ではないので、何も代入されません。

[GetValueOrDefault メソッド](https://learn.microsoft.com/dotnet/api/system.nullable-1.getvalueordefault) でも、`null` のときに既定値（`int` なら `0`）を取り出せます。

### パターンマッチング

[型変換と型チェック](/unity-csharp-learning/csharp/type-casting/) で学んだ `is` によるパターンマッチングを使うと、`null` でないかを調べるのと同時に、値を変数に取り出せます。

```csharp
int? score = 80;

if (score is int s)
{
    Console.WriteLine($"点数は {s} 点");
}
else
{
    Console.WriteLine("未受験");
}
```

```
点数は 80 点
```

`score` が `null` なら、`else` のほうが実行されます。

---

## 5. null 許容値型を返すメソッド

「見つからない」「決められない」ことがあるメソッドの戻り値にも、null 許容値型を使えます。次の `FindIndex` は、配列から値を探し、見つかった位置を返します。見つからなかったときは `null` を返します。

```csharp
int[] numbers = { 3, 8, 5 };

int? index1 = FindIndex(numbers, 8);
int? index2 = FindIndex(numbers, 7);

Console.WriteLine(index1 is int i1 ? $"8 は {i1} 番目" : "8 は見つからない");
Console.WriteLine(index2 is int i2 ? $"7 は {i2} 番目" : "7 は見つからない");

int? FindIndex(int[] array, int target)
{
    for (int i = 0; i < array.Length; i++)
    {
        if (array[i] == target)
        {
            return i;
        }
    }
    return null;
}
```

```
8 は 1 番目
7 は見つからない
```

戻り値の型が `int?` なので、呼び出し元は、見つからなかった場合を扱う必要があることに気付けます。見つからないときに `-1` を返す方法では、`-1` を確かめ忘れても、コンパイラーは何も指摘しません。

---

## ワンポイントアドバイス

### 参照型の ? は別の機能

[値型と参照型](/unity-csharp-learning/csharp/value-reference-types/) で、`Box?` のように参照型にも `?` を付けました。これは **null 許容参照型**（nullable reference type）という別の機能です。

参照型の変数にはもともと `null` を入れられるので、参照型の `?` は、`Nullable<T>` のような別の型を作るわけではありません。「この変数には `null` が入ることがある」という目印をコンパイラーに伝えるだけです。コンパイラーはこの目印を使って、`?` のない変数に `null` を入れたり、`?` のある変数を `null` かどうか確かめずに使ったりしたときに、警告を出します。この警告は、プロジェクトの設定（`Nullable`）で有効になります。このサイトのコード例を実行する環境では、有効になっています。詳しくは、次の [null 許容参照型](/unity-csharp-learning/csharp/nullable-reference-types/) で学びます。

---

## まとめ

- `値型?` と書くと、値型の値に加えて `null` を入れられる null 許容値型になる
- `int?` は `Nullable<int>` の短い書き方。`Nullable<T>` は値型の構造体
- `HasValue` で値があるかを調べ、`Value` で値を取り出す。`null` のときに `Value` を読むと例外が発生する
- 算術演算は、どちらかが `null` なら結果も `null`。`<` や `>` の比較は、どちらかが `null` なら `false`
- `??` や `??=`、`is` によるパターンマッチングで、`null` のときの扱いを決めて値を取り出す

---

## 理解度チェック

1. `int` ではなく `int?` を使いたくなるのは、どのような場面ですか？
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   int? a = null;
   int? b = 3;

   Console.WriteLine($"[{a * b}]");
   Console.WriteLine(a < b);
   Console.WriteLine(a ?? b);
   Console.WriteLine((a ?? 0) + (b ?? 0));
   ```

3. `string?` と `int?` の `?` の違いを説明してください。

<details markdown="1">
<summary>解答を見る</summary>

1. テストをまだ受けていない、値が見つからなかった、のように「値がない」ことを、`0` などの値と区別して表したい場面です。
2. 次のように出力されます。`a` が `null` なので、`a * b` は `null`、`a < b` は `false` になります。`a ?? b` は、`a` が `null` なので `b` の値 `3` になります。

   ```
   []
   False
   3
   3
   ```

3. `int?` の `?` は、`Nullable<int>` という別の型を表します。`string?` の `?` は、`null` が入ることがあるという目印をコンパイラーに伝えるだけで、型は `string` のままです。

</details>

---

## 次のステップ

[null 許容参照型](/unity-csharp-learning/csharp/nullable-reference-types/) では、参照型の `?` の意味と、`null` を扱う誤りをコンパイラーに警告させる方法を学びます。
