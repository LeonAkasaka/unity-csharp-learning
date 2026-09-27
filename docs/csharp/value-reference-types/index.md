---
layout: page
title: 値型と参照型
permalink: /csharp/value-reference-types/
---

# 値型と参照型

C# の型は、**値型**（value type）と **参照型**（reference type）の 2 種類に分かれます。値型の変数には値そのものが入り、参照型の変数にはオブジェクトの場所を指す **参照** が入ります。この違いによって、代入やメソッドの呼び出しでコピーされるものが変わります。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- 値型と参照型で、代入したときにコピーされるものの違いを説明できる
- 参照型の変数をメソッドに渡したとき、呼び出し元に影響する操作としない操作を区別できる
- `null` と既定値が、値型と参照型でどう違うかを説明できる

## 前提知識

- [クラスとフィールド](/unity-csharp-learning/csharp/classes/) を読んでいること
- [Array クラスと配列の性質（補足）](/unity-csharp-learning/csharp/array-class/) を読んでいること
- [ref / out / in パラメータ](/unity-csharp-learning/csharp/ref-out-in/) を読んでいること

---

## 1. 値型と参照型

これまでに使ってきた型は、次のように分かれます。

| 種類 | 主な型 |
|---|---|
| [値型](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/value-types) | `int`・`double`・`bool`・`char` などの数値や文字の型、構造体、列挙型 |
| [参照型](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/reference-types) | クラス、配列、`string`、`object`、インターフェイス、デリゲート |

構造体と列挙型は、このセクションで学びます。自分で定義したクラスは参照型です。[クラスとフィールド](/unity-csharp-learning/csharp/classes/) や [Array クラスと配列の性質（補足）](/unity-csharp-learning/csharp/array-class/) で、クラスや配列を代入すると同じオブジェクトを指すようになることを学びました。これは、クラスや配列が参照型だからです。このページでは、値型と比べながら、その違いを整理します。

---

## 2. 代入でコピーされるもの

次のコードでは、`int` の変数と、自分で定義した `Box` クラスの変数を、それぞれ代入してから書き換えます。

```csharp
int a = 10;
int b = a;
b = 20;
Console.WriteLine($"a = {a}, b = {b}");

Box p = new Box();
p.Value = 10;
Box q = p;
q.Value = 20;
Console.WriteLine($"p.Value = {p.Value}, q.Value = {q.Value}");

class Box
{
    public int Value;
}
```

```
a = 10, b = 20
p.Value = 20, q.Value = 20
```

`int` は値型なので、`b = a` では値の `10` がコピーされます。`b` を書き換えても `a` は変わりません。

`Box` は参照型なので、`q = p` でコピーされるのは、オブジェクトへの参照です。`p` と `q` は同じ 1 つのオブジェクトを指しているので、`q.Value` を書き換えると、`p.Value` も変わったように見えます。

![値型の a と b はそれぞれ別の値を持ち、参照型の p と q は同じ 1 つの Box オブジェクトを指している様子](value-vs-reference.svg)

---

## 3. メソッドに渡したときの違い

[ref / out / in パラメータ](/unity-csharp-learning/csharp/ref-out-in/) で学んだように、`ref` などを付けないパラメータには、引数の値のコピーが渡されます（値渡し）。参照型の変数を渡したときにコピーされるのは、参照です。

```csharp
int n = 1;
Box box = new Box();
box.Value = 1;

Change(n, box);
Console.WriteLine($"n = {n}, box.Value = {box.Value}");

Replace(box);
Console.WriteLine($"box.Value = {box.Value}");

void Change(int x, Box b)
{
    x = 100;
    b.Value = 100;
}

void Replace(Box b)
{
    b = new Box();
    b.Value = 999;
}

class Box
{
    public int Value;
}
```

```
n = 1, box.Value = 100
box.Value = 100
```

`Change` の `x` は `n` の値のコピーなので、書き換えても `n` は変わりません。`b` は `box` の参照のコピーで、`box` と同じオブジェクトを指しています。そのため、`b.Value` の書き換えは、呼び出し元の `box` からも見えます。

`Replace` では、パラメータ `b` に新しいオブジェクトを代入しています。変わるのは `b` が指す先だけで、呼び出し元の `box` は元のオブジェクトを指したままです。呼び出し元の変数そのものを書き換えたいときは、参照型であっても `ref` を使います。

| 操作 | 値型のパラメータ | 参照型のパラメータ |
|---|---|---|
| パラメータに別の値やオブジェクトを代入する | 呼び出し元に影響しない | 呼び出し元に影響しない |
| パラメータが指すオブジェクトの中身を書き換える | （該当しない） | 呼び出し元からも見える |

---

## 4. == で比べるもの

[等値演算子 ==](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/equality-operators) は、値型では値が等しいかを比べます。クラスでは、同じオブジェクトを指しているかを比べます。中身が同じでも、別々に作ったオブジェクトは等しくなりません。

```csharp
Box a = new Box();
a.Value = 1;
Box b = new Box();
b.Value = 1;
Box c = a;

Console.WriteLine(a == b);
Console.WriteLine(a == c);

class Box
{
    public int Value;
}
```

```
False
True
```

`string` は参照型ですが、`==` で中身の文字列を比べます。[演算子のオーバーロード](/unity-csharp-learning/csharp/operator-overloading/) で学んだように、`string` クラスが `==` を、文字列の中身を比べるように定義しているからです。同じオブジェクトかどうかは、[Object.ReferenceEquals メソッド](https://learn.microsoft.com/dotnet/api/system.object.referenceequals) で調べられます。

```csharp
string s1 = "aaa";
string s2 = new string('a', 3);

Console.WriteLine(s1 == s2);
Console.WriteLine(object.ReferenceEquals(s1, s2));
```

```
True
False
```

`new string('a', 3)` は、`'a'` を 3 つ並べた文字列を新しく作ります。`s1` と `s2` は別々のオブジェクトですが、中身が同じなので `==` は `True` になります。

---

## 5. null と既定値

参照型の変数には、どのオブジェクトも指していないことを表す **null** を入れられます。値型の変数には `null` を入れられません。

```csharp
// ❌ NG: 値型の変数に null は入れられない
// int n = null;  // CS0037
```

`null` が入った変数からメンバーを使うと、`NullReferenceException` が発生します。

> 💡 **ポイント**: このサイトのコード例では、参照型の変数に `null` を入れるとき、`Box?` のように型名の後に `?` を付けます。`?` のない参照型の変数に `null` を入れると、コンパイラーが警告を出す設定になっているからです。`?` の意味は [null 許容値型](/unity-csharp-learning/csharp/nullable-value-types/) で説明します。

フィールドや配列の要素は、値を代入しなくても **既定値** で初期化されます。既定値は、[default 演算子](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/default) で調べられます。

```csharp
Console.WriteLine(default(int));
Console.WriteLine(default(bool));
Console.WriteLine(default(Box) == null);

Box?[] boxes = new Box?[2];
Console.WriteLine(boxes[0] == null);

class Box
{
}
```

```
0
False
True
True
```

値型の既定値は、`0` や `false` のように、すべてのビットが 0 の値です。参照型の既定値は `null` です。`Box` の配列を作っただけでは、要素は `null` で、`Box` のオブジェクトはまだ 1 つもありません。

---

## 6. スタックとヒープ

プログラムが使うメモリには、**スタック** と **ヒープ** という 2 つの領域があります。

- **スタック**：メソッドのローカル変数やパラメータが置かれる領域。[再帰関数とコールスタック](/unity-csharp-learning/csharp/recursion/) で学んだように、メソッドを呼び出すと積まれ、メソッドから戻ると取り除かれる
- **ヒープ**：`new` で作ったオブジェクトが置かれる領域。メソッドから戻っても、オブジェクトは残る

次のコードで、変数とオブジェクトがどこに置かれるかを見てみましょう。

```csharp
int n = 10;
Box box = new Box();
box.Value = 20;
Console.WriteLine($"n = {n}, box.Value = {box.Value}");

class Box
{
    public int Value;
}
```

```
n = 10, box.Value = 20
```

![スタックにローカル変数 n と box があり、n には 10 が入っている。box にはヒープ上の Box オブジェクトへの参照が入っていて、Box オブジェクトの中に Value の 20 がある様子](stack-and-heap.svg)

ローカル変数 `n` と `box` はスタックに置かれます。`n` には値の `10` がそのまま入っています。`box` に入っているのは参照で、`Box` のオブジェクトそのものはヒープにあります。

「値型はスタックに置かれる」という説明を見かけることがありますが、正確ではありません。`Value` は `int` 型（値型）のフィールドですが、`Box` オブジェクトの一部なので、ヒープに置かれています。値型の値は、**その変数やフィールドがある場所に直接入る**、と覚えておきましょう。ローカル変数ならスタックに、クラスのフィールドならヒープのオブジェクトの中に入ります。

ヒープに作られたオブジェクトは、使い終わっても自分で消す必要はありません。どこからも使われなくなったオブジェクトは、.NET が自動的に片付けます。この仕組みは、次のページで学びます。

---

## よくあるミス

### オブジェクトをコピーしたつもりで、同じオブジェクトを書き換える

```csharp
Box original = new Box();
original.Value = 1;

Box copy = original;
copy.Value = 2;

Console.WriteLine(original.Value);

class Box
{
    public int Value;
}
```

```
2
```

`copy = original` でコピーされるのは参照だけで、オブジェクトは 1 つのままです。別のオブジェクトとして扱いたいときは、`new` で新しいオブジェクトを作り、フィールドの値を移します。

---

## まとめ

- 値型の変数には値そのものが、参照型の変数にはオブジェクトへの参照が入る
- 代入や値渡しでコピーされるのは、値型では値、参照型では参照。参照をコピーした変数どうしは、同じオブジェクトを指す
- クラスの `==` は同じオブジェクトかどうかを比べる。`string` は中身を比べるように定義されている
- 参照型の変数には `null` を入れられるが、値型の変数には入れられない。既定値は、値型ではすべてのビットが 0 の値、参照型では `null`
- 値型の値は、変数やフィールドがある場所に直接入る。`new` で作ったオブジェクトはヒープに置かれる

---

## 理解度チェック

1. 値型と参照型の変数で、代入したときにコピーされるものの違いを説明してください。
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   int x = 5;
   Box a = new Box();
   a.Value = 5;

   Update(x, a);
   Console.WriteLine($"{x}, {a.Value}");

   void Update(int n, Box b)
   {
       n++;
       b.Value++;
       b = new Box();
       b.Value = 100;
   }

   class Box
   {
       public int Value;
   }
   ```

3. 「値型の値は必ずスタックに置かれる」という説明が正確でない理由を説明してください。

<details markdown="1">
<summary>解答を見る</summary>

1. 値型では値そのものがコピーされ、コピー先を書き換えてもコピー元は変わりません。参照型ではオブジェクトへの参照がコピーされ、コピー元とコピー先は同じオブジェクトを指します。
2. `5, 6` が出力されます。`n` は `x` のコピーなので `x` は変わりません。`b.Value++` は `a` と同じオブジェクトを書き換えるので `a.Value` は `6` になります。その後の `b = new Box()` は `b` が指す先を変えるだけで、`a` には影響しません。
3. クラスのフィールドにある値型の値は、オブジェクトの一部としてヒープに置かれるからです。値型の値は、変数やフィールドがある場所に直接入ります。

</details>

---

## 次のステップ

[ガベージコレクション](/unity-csharp-learning/csharp/garbage-collection/) では、使われなくなったオブジェクトを .NET が自動的に片付ける仕組みを学びます。
