---
layout: page
title: ref フィールドと scoped
permalink: /csharp/ref-fields-scoped/
---

# ref フィールドと scoped

[ref struct と Span の制約](/unity-csharp-learning/csharp/ref-struct/) で、`Span<T>` の内部には、先頭の要素を指す **ref フィールド**（ref field）があると紹介しました。ref フィールドを使うと、自分で作る ref struct にも、変数を指すフィールドを持たせられます。このページでは、ref フィールドの書き方と、ref struct の値をメソッドの外へ持ち出さないことをコンパイラーに約束する **scoped** を学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- ref struct に ref フィールドを宣言し、`= ref` で変数を指させられる
- ref フィールドを持つ値を、メソッドから返せる場合と返せない場合を説明できる
- `scoped` を付けたパラメータに、`stackalloc` の領域を指す `Span<T>` を渡せる理由を説明できる
- `scoped` を付けたローカル変数に、後から `stackalloc` の領域を代入できる

## 前提知識

- [ref ローカルと ref 戻り値](/unity-csharp-learning/csharp/ref-locals/) から [stackalloc](/unity-csharp-learning/csharp/stackalloc/) までのページを読んでいること

---

## 1. ref フィールド

[ref ローカルと ref 戻り値](/unity-csharp-learning/csharp/ref-locals/) で学んだ ref ローカルは、ほかの変数を指すローカル変数でした。**ref フィールド** は、ほかの変数を指すフィールドです。ref フィールドは、ref struct の中にだけ宣言できます。

**書式：[ref フィールド](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/ref-struct#ref-fields)**
```
ref struct 構造体名
{
    ref 型 フィールド名;
}
```

ref フィールドに、指す先の変数を設定するときは、ref ローカルと同じように `= ref` で代入します。`= ref` を付けずに読み書きすると、指している変数を読み書きします。

次の `ScoreAdder` は、コンストラクターで受け取った変数を指し、`Add` でその変数に点数を加えます。

```csharp
int score = 0;
ScoreAdder adder = new ScoreAdder(ref score);
adder.Add(10);
adder.Add(5);
Console.WriteLine(score);

ref struct ScoreAdder
{
    private ref int target;

    public ScoreAdder(ref int target)
    {
        this.target = ref target;
    }

    public void Add(int points)
    {
        target += points;
    }
}
```

```
15
```

`adder.Add` を呼び出すと、`adder` の中の `target` が指している変数 `score` が書き換わります。`adder` に `score` の値がコピーされているわけではありません。

ref フィールドを、ふつうの `struct` やクラスに宣言すると、コンパイルエラーになります。

```csharp
struct Plain
{
    private ref int target;  // ❌ CS9059: ref フィールドは ref struct にだけ宣言できる
}
```

ref フィールドが指す変数は、スタックの上のローカル変数かもしれません。ref フィールドを持つ値がヒープに置かれると、メソッドから戻った後で、存在しない変数を指し続けることになります。`Span<T>` と同じ理由で、ref フィールドを持つ値も、スタックの上にしか置けない ref struct でなければなりません。

> 💡 **ポイント**: `Span<T>` も、先頭の要素を指す ref フィールドと、要素の数を表す `int` のフィールドを持つ ref struct です。`Span<T>` には、1 つの変数を指す、長さ 1 の `Span<T>` を作るコンストラクター `new Span<T>(ref 変数)` もあります。

---

## 2. ref フィールドを持つ値を返す

ref フィールドを持つ値は、何を指しているかによって、メソッドから返せるかどうかが決まります。

次の `Target` は、配列の要素を指す `ScoreAdder` を作って返します。配列はヒープにあり、メソッドから戻っても残るので、返せます。

```csharp
int[] scores = { 10, 20, 30 };
ScoreAdder adder = Target(scores, 1);
adder.Add(5);
Console.WriteLine(string.Join(", ", scores));

ScoreAdder Target(int[] array, int index)
{
    return new ScoreAdder(ref array[index]);
}

ref struct ScoreAdder
{
    private ref int target;

    public ScoreAdder(ref int target)
    {
        this.target = ref target;
    }

    public void Add(int points)
    {
        target += points;
    }
}
```

```
10, 25, 30
```

一方、メソッドのローカル変数を指す `ScoreAdder` は返せません。上のコードの `Target` を、次の `Create` に置き換えるとコンパイルエラーになります。

```csharp
ScoreAdder Create()
{
    int local = 0;
    return new ScoreAdder(ref local);  // ❌ CS8347、CS8168: ローカル変数を指す値は返せない
}
```

`local` は、`Create` から戻ると存在しなくなります。[ref ローカルと ref 戻り値](/unity-csharp-learning/csharp/ref-locals/) でローカル変数への参照を返せなかったのと、[stackalloc](/unity-csharp-learning/csharp/stackalloc/) で `stackalloc` の領域を指す `Span<T>` を返せなかったのと、同じ理由です。

コンパイラーは、ref struct の値の 1 つ 1 つについて、その値をどこまで持ち出せるか（**安全なコンテキスト**、safe context）を調べています。持ち出せる範囲は、値が何を指しているかで決まります。

| 値が指しているもの | 持ち出せる範囲 |
|---|---|
| ヒープ上の配列の要素、`ref` パラメータで受け取った変数 | メソッドの外へ返せる |
| メソッドのローカル変数、`stackalloc` の領域 | そのメソッドの中だけ |

`Create` では、`new ScoreAdder(ref local)` が `local` を指しているかもしれないので、作った値は `Create` の中でしか使えません。

---

## 3. scoped パラメータ

`Span<T>` を受け取るメソッドに、`stackalloc` の領域を渡せないことがあります。次の `Writer` は、文字を書き込む先の `Span<char>` をフィールドに持つ ref struct です。`WriteRepeated` は、同じ文字を並べた領域を `stackalloc` で作り、`Write` に渡しています。このコードはコンパイルエラーになります。

```csharp
Writer writer = new Writer(new char[32]);
writer.Write("score");
writer.WriteRepeated('.', 5);
writer.Write("90");
Console.WriteLine(writer.ToString());

ref struct Writer
{
    private Span<char> buffer;
    private int position;

    public Writer(Span<char> buffer)
    {
        this.buffer = buffer;
        position = 0;
    }

    public void Write(ReadOnlySpan<char> text)
    {
        text.CopyTo(buffer[position..]);
        position += text.Length;
    }

    public void WriteRepeated(char c, int count)
    {
        Span<char> chars = stackalloc char[count];
        chars.Fill(c);
        Write(chars);  // ❌ CS8350、CS8352: stackalloc の領域を渡せない
    }

    public override string ToString()
    {
        return buffer[..position].ToString();
    }
}
```

`Write` は、`text` の文字を `buffer` にコピーしているだけです。しかし、コンパイラーは、呼び出し元をコンパイルするときに `Write` の中身を見ず、宣言だけで判断します。宣言だけを見ると、`Write` は `this.buffer = text;` のように、受け取った `text` を `Writer` のフィールドに保存するかもしれません。

`writer` は、ヒープ上の配列を指しているので、`WriteRepeated` から戻った後も使われます。もし `chars` がフィールドに保存されると、`writer` は、`WriteRepeated` から戻ると取り除かれる `stackalloc` の領域を指し続けることになります。そこでコンパイラーは、`Write` に `chars` を渡すことを禁止します。

パラメータに **scoped** を付けると、そのパラメータで受け取った値を、メソッドの外へ持ち出さないことをコンパイラーに約束できます。

**書式：[scoped](https://learn.microsoft.com/dotnet/csharp/language-reference/statements/declarations#scoped-ref)（パラメータ）**
```
戻り値の型 メソッド名(scoped 型 パラメータ名)
```

`scoped` を付けられるのは、`Span<T>` などの ref struct の型のパラメータと、`ref` などを付けたパラメータです。

`Write` のパラメータに `scoped` を付けると、エラーがなくなります。

```csharp
Writer writer = new Writer(new char[32]);
writer.Write("score");
writer.WriteRepeated('.', 5);
writer.Write("90");
Console.WriteLine(writer.ToString());

ref struct Writer
{
    private Span<char> buffer;
    private int position;

    public Writer(Span<char> buffer)
    {
        this.buffer = buffer;
        position = 0;
    }

    public void Write(scoped ReadOnlySpan<char> text)
    {
        text.CopyTo(buffer[position..]);
        position += text.Length;
    }

    public void WriteRepeated(char c, int count)
    {
        Span<char> chars = stackalloc char[count];
        chars.Fill(c);
        Write(chars);
    }

    public override string ToString()
    {
        return buffer[..position].ToString();
    }
}
```

```
score.....90
```

`scoped` を付けたパラメータは、そのメソッドの中でしか使えない値として扱われます。メソッドの中で、約束を破ってフィールドに保存しようとすると、今度はそのメソッドの中がコンパイルエラーになります。

```csharp
ref struct Recorder
{
    private ReadOnlySpan<char> last;

    public void Record(scoped ReadOnlySpan<char> text)
    {
        last = text;  // ❌ CS8352: scoped のパラメータはフィールドに保存できない
    }
}
```

---

## 4. scoped ローカル変数

[stackalloc](/unity-csharp-learning/csharp/stackalloc/) では、大きさによって `stackalloc` と `new` を使い分けるときに、条件演算子 `? :` を使いました。同じことを `if` 文で書くと、コンパイルエラーになります。

```csharp
Console.WriteLine(Describe(10));
Console.WriteLine(Describe(1000));

string Describe(int count)
{
    Span<int> buffer;
    if (count <= 256)
    {
        buffer = stackalloc int[count];  // ❌ CS8353: stackalloc の領域を代入できない
    }
    else
    {
        buffer = new int[count];
    }
    for (int i = 0; i < buffer.Length; i++)
    {
        buffer[i] = i;
    }
    return $"{buffer.Length} 個, 最後は {buffer[^1]}";
}
```

ref struct のローカル変数が持ち出せる範囲は、宣言したときに決まります。初期値を書かずに宣言した変数や、配列で初期化した変数は、メソッドの外へ返せる変数として扱われます。そのような変数に、メソッドの中でしか使えない `stackalloc` の領域を代入すると、`return buffer;` のように外へ持ち出せてしまうので、エラーになります。

ローカル変数の宣言に `scoped` を付けると、その変数を、メソッドの中でしか使えない変数として宣言できます。

**書式：[scoped](https://learn.microsoft.com/dotnet/csharp/language-reference/statements/declarations#scoped-ref)（ローカル変数）**
```
scoped 型 変数名;
scoped 型 変数名 = 初期値;
```

```csharp
Console.WriteLine(Describe(10));
Console.WriteLine(Describe(1000));

string Describe(int count)
{
    scoped Span<int> buffer;
    if (count <= 256)
    {
        buffer = stackalloc int[count];
    }
    else
    {
        buffer = new int[count];
    }
    for (int i = 0; i < buffer.Length; i++)
    {
        buffer[i] = i;
    }
    return $"{buffer.Length} 個, 最後は {buffer[^1]}";
}
```

```
10 個, 最後は 9
1000 個, 最後は 999
```

`scoped` を付けた変数は、配列を指しているときでも、メソッドから返せません。

```csharp
Span<int> Bad()
{
    scoped Span<int> span = new int[3];
    return span;  // ❌ CS8352: scoped の変数は返せない
}
```

---

## よくあるミス

### ref フィールドに ref を付けずに代入する

コンストラクターで、ref フィールドに `= ref` ではなく `=` で代入すると、指す先の変数が設定されません。

```csharp
int score = 0;
ScoreAdder adder = new ScoreAdder(ref score);
adder.Add(10);
Console.WriteLine(score);

ref struct ScoreAdder
{
    private ref int target;

    public ScoreAdder(ref int target)
    {
        this.target = target;  // ⚠ CS9201: ref フィールドが何も指していない
    }

    public void Add(int points)
    {
        target += points;
    }
}
```

実行すると、次の例外が発生してプログラムが終了します（スタックトレースは省略しています）。

```
Unhandled exception. System.NullReferenceException: Object reference not set to an instance of an object.
```

`this.target = target;` は、「`this.target` が指している変数に、`target` の値を代入する」という意味になります。ref フィールドは、何も指していない状態で始まるので、指す先のない変数に書き込もうとして `NullReferenceException` が発生します。コンパイル時には、CS9201 と CS9265 の警告が出ます。ref フィールドに指す先を設定するときは、`this.target = ref target;` のように `ref` を付けます。

---

## ワンポイントアドバイス

### C# のバージョンによる違い

ref フィールドと `scoped` は、C# 11 で追加されました。ref フィールドを使うには、実行する .NET も ref フィールドに対応している（.NET 7 以降）必要があります。

---

## まとめ

- ref フィールドは、ほかの変数を指すフィールド。ref struct の中にだけ宣言できる
- ref フィールドに指す先を設定するときは `= ref` で代入する。`=` だけでは、指している変数への代入になる
- ref struct の値は、何を指しているかによって、メソッドの外へ持ち出せるかどうかが決まる
- コンパイラーはメソッドの宣言だけを見て、受け取った値がフィールドに保存されるかもしれないと判断する
- パラメータに `scoped` を付けると、受け取った値を外へ持ち出さないと約束でき、`stackalloc` の領域を指す `Span<T>` を渡せる
- ローカル変数に `scoped` を付けると、後から `stackalloc` の領域を代入できる。その変数はメソッドから返せない

---

## 理解度チェック

1. ref フィールドを、ふつうの `struct` に宣言できないのはなぜですか？
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   int a = 1;
   int b = 2;
   Pointer p = new Pointer(ref a);
   Pointer q = p;
   q.Set(10);
   p = new Pointer(ref b);
   p.Set(20);
   Console.WriteLine($"{a}, {b}");

   ref struct Pointer
   {
       private ref int target;

       public Pointer(ref int target)
       {
           this.target = ref target;
       }

       public void Set(int value)
       {
           target = value;
       }
   }
   ```

3. 3 節の `Writer` で、`Write` のパラメータに `scoped` を付けないと、`WriteRepeated` の中の `Write(chars)` がコンパイルエラーになります。`Write` の中身は `text` を保存していないのに、エラーになるのはなぜですか？

<details markdown="1">
<summary>解答を見る</summary>

1. ref フィールドは、スタックの上のローカル変数を指していることがあるからです。ふつうの構造体は、クラスのフィールドや配列の要素になってヒープに置かれることがあり、メソッドから戻った後で、存在しない変数を指し続けるおそれがあります。
2. 次のように出力されます。`q` は `p` のコピーですが、コピーされるのは ref フィールドが指す先なので、`q` も `a` を指しています。`q.Set(10)` で `a` が `10` になります。その後、`p` を `b` を指す新しい `Pointer` に置き換えたので、`p.Set(20)` で `b` が `20` になります。

   ```
   10, 20
   ```

3. コンパイラーは、呼び出し元をコンパイルするとき、`Write` の中身ではなく宣言だけを見て判断するからです。宣言だけでは、`Write` が `text` を `Writer` のフィールドに保存するかもしれません。保存されると、`WriteRepeated` から戻った後も使われる `writer` が、取り除かれた `stackalloc` の領域を指すことになるので、`chars` を渡すことが禁止されます（CS8350）。

</details>

---

## 次のステップ

[Memory\<T\>](/unity-csharp-learning/csharp/memory/) では、`Span<T>` と同じように配列の一部を指しながら、クラスのフィールドにしたり、`await` をまたいだりできる型を学びます。
