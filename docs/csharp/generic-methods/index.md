---
layout: page
title: ジェネリックメソッド
permalink: /csharp/generic-methods/
---

# ジェネリックメソッド

型パラメータは、クラスだけでなく、メソッドにも付けられます。型パラメータを持つメソッドを **ジェネリックメソッド**（generic method）といいます。このページでは、ジェネリックメソッドの定義と呼び出し方、型引数を省略できる型推論、クラスの型パラメータとの違いを学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- ジェネリックメソッドを定義して、型引数を指定して呼び出せる
- 型推論で型引数を省略できる場合と、省略できない場合を説明できる
- クラスの型パラメータとメソッドの型パラメータの違いを説明し、使い分けられる

## 前提知識

- [ジェネリクスの基本](/unity-csharp-learning/csharp/generics/) を読んでいること
- [ref / out / in パラメータ](/unity-csharp-learning/csharp/ref-out-in/) を読んでいること
- [static メンバーと static クラス](/unity-csharp-learning/csharp/static-members/) を読んでいること

---

## 1. 型だけが違うメソッド

2 つの変数の値を入れ替えるメソッドを、[ref / out / in パラメータ](/unity-csharp-learning/csharp/ref-out-in/) で学んだ `ref` を使って書きます。`int` と `string` の両方で使いたいので、オーバーロードを 2 つ定義します。

```csharp
int a = 1;
int b = 2;
Util.Swap(ref a, ref b);
Console.WriteLine($"a={a}, b={b}");

string x = "left";
string y = "right";
Util.Swap(ref x, ref y);
Console.WriteLine($"x={x}, y={y}");

static class Util
{
    public static void Swap(ref int left, ref int right)
    {
        int temp = left;
        left = right;
        right = temp;
    }

    public static void Swap(ref string left, ref string right)
    {
        string temp = left;
        left = right;
        right = temp;
    }
}
```

```
a=2, b=1
x=right, y=left
```

2 つの `Swap` は、型が違うだけで処理はまったく同じです。`double` や `bool` でも使いたくなれば、同じメソッドをさらに書き足すことになります。

[ジェネリクスの基本](/unity-csharp-learning/csharp/generics/) で学んだジェネリッククラスで解決しようとすると、`Swap` を呼ぶために `new SwapHelper<int>()` のようなインスタンスを作ることになります。`Swap` はデータを持たないので、インスタンスを作る意味がありません。このようなときは、メソッドに型パラメータを付けます。

---

## 2. ジェネリックメソッドを定義する

メソッド名の直後に `<T>` を書くと、そのメソッドの中で `T` を型として使えます。

**書式：[ジェネリックメソッドの定義](https://learn.microsoft.com/dotnet/csharp/programming-guide/generics/generic-methods)**
```
アクセス修飾子 戻り値の型 メソッド名<型パラメータ>(パラメータリスト)
{
}
```

| 要素 | 説明 |
|---|---|
| `<型パラメータ>` | メソッド名の直後に書く。戻り値の型・パラメータの型・メソッドの中の変数の型として使える |

呼び出すときは、メソッド名の直後に型引数を書きます。

**書式：[ジェネリックメソッドの呼び出し](https://learn.microsoft.com/dotnet/csharp/programming-guide/generics/generic-methods)**
```
メソッド名<型引数>(引数)
```

1 節の 2 つの `Swap` を、1 つのジェネリックメソッドにまとめます。

```csharp
int a = 1;
int b = 2;
Util.Swap<int>(ref a, ref b);
Console.WriteLine($"a={a}, b={b}");

string x = "left";
string y = "right";
Util.Swap<string>(ref x, ref y);
Console.WriteLine($"x={x}, y={y}");

static class Util
{
    public static void Swap<T>(ref T left, ref T right)
    {
        T temp = left;
        left = right;
        right = temp;
    }
}
```

```
a=2, b=1
x=right, y=left
```

`Swap<int>` と呼び出すと `T` が `int` に、`Swap<string>` と呼び出すと `T` が `string` になります。メソッドの中では、`T temp` のように、ローカル変数の型にも `T` を使えます。

`Util` はジェネリッククラスではありません。ジェネリックメソッドは、ふつうのクラスにも、`static` メソッドとしてもインスタンスメソッドとしても定義できます。

---

## 3. 型推論

ジェネリックメソッドを呼び出すとき、型引数を省略できることがあります。コンパイラーが、渡した引数の型から型引数を決めるからです。これを **型推論**（type inference）といいます。

```csharp
int a = 1;
int b = 2;
Util.Swap(ref a, ref b);  // a と b が int なので、T = int
Console.WriteLine($"a={a}, b={b}");

string[] words = Util.Repeat("ha", 3);  // "ha" が string なので、T = string
Console.WriteLine(string.Join(", ", words));

int[] zeros = Util.Repeat(0, 4);  // 0 が int なので、T = int
Console.WriteLine(string.Join(", ", zeros));

static class Util
{
    public static void Swap<T>(ref T left, ref T right)
    {
        T temp = left;
        left = right;
        right = temp;
    }

    // value を count 個並べた配列を返す
    public static T[] Repeat<T>(T value, int count)
    {
        T[] items = new T[count];
        for (int i = 0; i < count; i++)
        {
            items[i] = value;
        }
        return items;
    }
}
```

```
a=2, b=1
ha, ha, ha
0, 0, 0, 0
```

`Repeat` の戻り値の型 `T[]` も、推論された `T` に合わせて `string[]` や `int[]` になります。型引数を書かなくても、コンパイラーは型を正しく扱います。型推論は、型引数を書く手間を省くだけで、型のチェックが緩くなるわけではありません。

### 型推論できないとき

型推論に使われるのは、**引数** の型だけです。型パラメータがパラメータの型に現れないメソッドでは、型引数を推論できないので、型引数を書く必要があります。

```csharp
double[] values = Util.CreateArray<double>(3);
Console.WriteLine(string.Join(", ", values));

// ❌ NG: T を決める手がかりになる引数がない（CS0411）
// double[] values2 = Util.CreateArray(3);

static class Util
{
    // 長さ length の T の配列を返す
    public static T[] CreateArray<T>(int length)
    {
        return new T[length];
    }
}
```

```
0, 0, 0
```

`CreateArray` のパラメータは `int length` だけで、`T` が出てきません。`double[] values2 = ...` のように左辺に型が書いてあっても、左辺の型は推論に使われないので、CS0411 のエラーになります。

---

## 4. クラスの型パラメータとメソッドの型パラメータ

ジェネリッククラスの中に、ジェネリックメソッドを定義することもできます。[ジェネリクスの基本](/unity-csharp-learning/csharp/generics/) の `Container<T>` と `Pair<TFirst, TSecond>` を使って、`Container<T>` の値と別の値を組にする `PairWith` メソッドを追加します。

```csharp
Container<string> name = new Container<string>("Alice");

Pair<string, int> withScore = name.PairWith(80);
Console.WriteLine($"{withScore.First}: {withScore.Second}");

Pair<string, bool> withFlag = name.PairWith(true);
Console.WriteLine($"{withFlag.First}: {withFlag.Second}");

class Container<T>
{
    private T _value;

    public Container(T value) { _value = value; }

    public void Set(T value) { _value = value; }
    public T Get() { return _value; }

    // T はクラスの型パラメータ、TOther はこのメソッドの型パラメータ
    public Pair<T, TOther> PairWith<TOther>(TOther other)
    {
        return new Pair<T, TOther>(_value, other);
    }
}

class Pair<TFirst, TSecond>
{
    public TFirst First { get; }
    public TSecond Second { get; }

    public Pair(TFirst first, TSecond second)
    {
        First = first;
        Second = second;
    }
}
```

```
Alice: 80
Alice: True
```

`PairWith` の中では、クラスの型パラメータ `T` と、メソッドの型パラメータ `TOther` の両方を使っています。2 つは、型が決まるタイミングが違います。

| 型パラメータ | 型が決まるタイミング | この例での型 |
|---|---|---|
| クラスの `T` | インスタンスを作るとき（`new Container<string>(...)`） | `name` では、いつも `string` |
| メソッドの `TOther` | メソッドを呼び出すたび | 1 回目は `int`、2 回目は `bool` |

使い分けの目安は次のとおりです。

- インスタンスが持つデータの型 → クラスの型パラメータ
- 呼び出しごとに変わる型や、データを持たない処理で使う型 → メソッドの型パラメータ

---

## よくあるミス

### メソッドの型パラメータにクラスと同じ名前を付ける

ジェネリッククラスの中のジェネリックメソッドに、クラスと同じ名前の型パラメータを付けると、コンパイラーが警告を出します。

```csharp
class Container<T>
{
    // ⚠️ NG: メソッドの T が、クラスの T を隠してしまう（警告 CS0693）
    // public void Print<T>(T value) { Console.WriteLine(value); }
}
```

メソッドの `<T>` は、クラスの `T` とは別の新しい型パラメータです。メソッドの中の `T` はメソッドの型パラメータを指すので、`Container<int>` のインスタンスでも `Print("abc")` のように `string` を渡せてしまいます。クラスの `T` を使いたいなら、メソッドに `<T>` を書かずに `public void Print(T value)` とします。別の型を扱いたいなら、`TOther` のように別の名前を付けます。

---

## まとめ

- ジェネリックメソッドは `戻り値の型 メソッド名<T>(パラメータリスト)` の形で定義する
- 呼び出すときは `メソッド名<型引数>(引数)` と書く
- 引数の型から型引数を決められるときは、型引数を省略できる（型推論）
- 型パラメータがパラメータの型に現れないときは推論できないので、型引数を書く。左辺の型は推論に使われない
- クラスの型パラメータはインスタンスを作るときに、メソッドの型パラメータは呼び出すたびに決まる

---

## 理解度チェック

1. 次の 2 つのメソッドのうち、呼び出すときに型引数を省略できるのはどちらですか？理由とともに答えてください。

   ```csharp
   public static T[] Repeat<T>(T value, int count) { ... }
   public static T[] CreateArray<T>(int length) { ... }
   ```

2. 次のコードを実行すると何が出力されますか？

   ```csharp
   string[] names = Util.Repeat("Bob", 2);
   string first = "Alice";
   Util.Swap(ref first, ref names[1]);
   Console.WriteLine($"{first} {string.Join(", ", names)}");

   static class Util
   {
       public static void Swap<T>(ref T left, ref T right)
       {
           T temp = left;
           left = right;
           right = temp;
       }

       public static T[] Repeat<T>(T value, int count)
       {
           T[] items = new T[count];
           for (int i = 0; i < count; i++)
           {
               items[i] = value;
           }
           return items;
       }
   }
   ```

3. （応用）配列の最後の要素を返すジェネリックメソッド `Last` を、`static class Util` に定義してください。`Util.Last(new[] { 1, 2, 3 })` が `3` を、`Util.Last(new[] { "a", "b" })` が `"b"` を返すようにします。

<details markdown="1">
<summary>解答を見る</summary>

1. `Repeat` です。パラメータ `value` の型が `T` なので、引数の型から `T` を推論できます。`CreateArray` はパラメータに `T` が現れないので推論できず、`CreateArray<int>(3)` のように型引数を書く必要があります。
2. `Bob Bob, Alice` が出力されます。`Repeat` で `{ "Bob", "Bob" }` を作り、`first`（`"Alice"`）と `names[1]`（`"Bob"`）を入れ替えています。配列の要素も `ref` で渡せます。
3. パラメータの型を `T[]` にすれば、引数の配列の型から `T` を推論できます。

   ```csharp
   public static T Last<T>(T[] items)
   {
       return items[items.Length - 1];
   }
   ```

</details>

---

## 次のステップ

[型制約](/unity-csharp-learning/csharp/generic-constraints/) では、型パラメータに条件を付けて、`T` に対して使える操作を増やす方法を学びます。
