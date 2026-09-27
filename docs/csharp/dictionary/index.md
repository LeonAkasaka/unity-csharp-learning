---
layout: page
title: Dictionary<TKey, TValue>
permalink: /csharp/dictionary/
---

# Dictionary\<TKey, TValue\>

**Dictionary\<TKey, TValue\>** は、**キー**（key）と **値**（value）を組にして保存し、キーを指定して値を取り出すコレクションです。「名前から点数を調べる」「商品コードから在庫数を調べる」のように、何かを手がかりに値を探したいときに使います。このページでは、`Dictionary<TKey, TValue>` への追加・取り出し・削除と、`foreach` による列挙を学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- キーで値を探すときに、配列や `List<T>` より `Dictionary<TKey, TValue>` が適している理由を説明できる
- `Add` とインデクサで値を追加・更新し、インデクサで値を取り出せる
- `TryGetValue` で、キーがないときにも例外を発生させずに値を取り出せる
- `foreach` と `KeyValuePair<TKey, TValue>` で、すべてのキーと値を取り出せる

## 前提知識

- [List\<T\>](/unity-csharp-learning/csharp/list/) を読んでいること
- [ref / out / in パラメータ](/unity-csharp-learning/csharp/ref-out-in/) を読んでいること

---

## 1. キーで値を探す

名前と点数を、2 つの配列に同じ順番で入れているとします。`Bob` の点数を調べるには、まず名前の配列から `Bob` のインデックスを探し、そのインデックスで点数の配列を読みます。

```csharp
string[] names = { "Alice", "Bob", "Carol" };
int[] scores = { 80, 65, 92 };

int index = Array.IndexOf(names, "Bob");
Console.WriteLine($"Bob: {scores[index]}");
```

```
Bob: 65
```

この方法には、次のような問題があります。

- 2 つの配列の順番がずれないように、自分で気をつけなければならない
- `Array.IndexOf` は、先頭から 1 つずつ比べて探す。要素が多いほど、探すのに時間がかかる

[Dictionary\<TKey, TValue\> クラス](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2) を使うと、名前（キー）と点数（値）を組にして保存し、名前から直接点数を取り出せます。要素がどれだけ多くても、キーから値を探す時間はほとんど変わりません。

---

## 2. 追加する・取り出す

`Dictionary<TKey, TValue>` は、キーの型 `TKey` と値の型 `TValue` の 2 つの型パラメータを持ちます。名前から点数を調べるなら、`Dictionary<string, int>` です。

キーと値の組を追加するには、[Add メソッド](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2.add) を使います。

**書式：[Dictionary\<TKey, TValue\>.Add メソッド](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2.add)**
```csharp
public void Add(TKey key, TValue value);
```

| パラメータ | 説明 |
|---|---|
| `key` | キー。同じ `Dictionary<TKey, TValue>` の中で重複してはいけない |
| `value` | キーに結び付ける値 |

値は、[インデクサ](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2.item) で `scores["Bob"]` のようにキーを指定して読み書きします。[インデクサ](/unity-csharp-learning/csharp/indexers/) で学んだように、インデクサのパラメータは `int` でなくてもかまいません。`Dictionary<string, int>` のインデクサは、`string` のキーを受け取ります。

```csharp
Dictionary<string, int> scores = new Dictionary<string, int>();
scores.Add("Alice", 80);
scores.Add("Bob", 65);
scores["Carol"] = 92;

Console.WriteLine($"Bob: {scores["Bob"]}");
Console.WriteLine($"Count = {scores.Count}");

scores["Bob"] = 70;
Console.WriteLine($"Bob: {scores["Bob"]}");
```

```
Bob: 65
Count = 3
Bob: 70
```

インデクサで書き込むとき、キーがなければ新しい組として追加され、キーがあれば値が上書きされます。`scores["Carol"] = 92` は追加、`scores["Bob"] = 70` は上書きです。`Add` は、すでにあるキーを指定すると例外を投げます（よくあるミスを参照）。

### 作るときに組を入れる

作るときに最初の組を決めておきたいときは、コレクション初期化子の中で `[キー] = 値` と書きます。

```csharp
Dictionary<string, int> scores = new Dictionary<string, int>
{
    ["Alice"] = 80,
    ["Bob"] = 65,
};
```

---

## 3. キーがないときの取り出し方

インデクサでないキーを読もうとすると、[KeyNotFoundException](https://learn.microsoft.com/dotnet/api/system.collections.generic.keynotfoundexception) が発生します。キーがあるかどうかわからないときは、[TryGetValue メソッド](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2.trygetvalue) を使います。

**書式：[Dictionary\<TKey, TValue\>.TryGetValue メソッド](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2.trygetvalue)**
```csharp
public bool TryGetValue(TKey key, out TValue value);
```

| パラメータ | 説明 |
|---|---|
| `key` | 探すキー |
| `value` | キーが見つかれば、その値が入る。見つからなければ `TValue` の既定値が入る |

`TryGetValue` は、キーが見つかれば `true` を、見つからなければ `false` を返します。値は、[ref / out / in パラメータ](/unity-csharp-learning/csharp/ref-out-in/) で学んだ `out` パラメータで受け取ります。`int.TryParse` と同じ形です。

キーがあるかどうかだけを調べるには [ContainsKey メソッド](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2.containskey) を、組を削除するには [Remove メソッド](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2.remove) を使います。`Remove` は、削除できたら `true`、キーが見つからなければ `false` を返します。

```csharp
Dictionary<string, int> scores = new Dictionary<string, int>
{
    ["Alice"] = 80,
    ["Bob"] = 65,
};

if (scores.TryGetValue("Carol", out int score))
{
    Console.WriteLine($"Carol: {score}");
}
else
{
    Console.WriteLine("Carol は登録されていない");
}

Console.WriteLine($"ContainsKey(\"Alice\") = {scores.ContainsKey("Alice")}");
Console.WriteLine($"Remove(\"Alice\") = {scores.Remove("Alice")}");
Console.WriteLine($"Remove(\"Alice\") = {scores.Remove("Alice")}");
Console.WriteLine($"Count = {scores.Count}");
```

```
Carol は登録されていない
ContainsKey("Alice") = True
Remove("Alice") = True
Remove("Alice") = False
Count = 1
```

`ContainsKey` で調べてからインデクサで読むと、キーを 2 回探すことになります。値を使うなら、1 回で済む `TryGetValue` を使います。

### 数を数える

`TryGetValue` とインデクサを組み合わせると、「それぞれの単語が何回出てきたか」のような数え上げを書けます。

```csharp
string[] words = { "apple", "banana", "apple", "cherry", "banana", "apple" };
Dictionary<string, int> counts = new Dictionary<string, int>();

foreach (string word in words)
{
    if (counts.TryGetValue(word, out int count))
    {
        counts[word] = count + 1;
    }
    else
    {
        counts[word] = 1;
    }
}

Console.WriteLine($"apple: {counts["apple"]}");
Console.WriteLine($"banana: {counts["banana"]}");
Console.WriteLine($"cherry: {counts["cherry"]}");
```

```
apple: 3
banana: 2
cherry: 1
```

初めて出てきた単語は `1` で追加し、2 回目からは今の値に 1 を足して上書きしています。

---

## 4. foreach ですべての組を取り出す

`Dictionary<TKey, TValue>` を `foreach` 文で回すと、キーと値の組が [KeyValuePair\<TKey, TValue\> 構造体](https://learn.microsoft.com/dotnet/api/system.collections.generic.keyvaluepair-2) として 1 つずつ取り出されます。`Key` プロパティでキーを、`Value` プロパティで値を読みます。

キーだけ、値だけが必要なときは、[Keys プロパティ](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2.keys) と [Values プロパティ](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2.values) を使います。

```csharp
Dictionary<string, int> scores = new Dictionary<string, int>
{
    ["Alice"] = 80,
    ["Bob"] = 65,
    ["Carol"] = 92,
};

foreach (KeyValuePair<string, int> pair in scores)
{
    Console.WriteLine($"{pair.Key}: {pair.Value}");
}

Console.WriteLine(string.Join(", ", scores.Keys));
Console.WriteLine(string.Join(", ", scores.Values));
```

```
Alice: 80
Bob: 65
Carol: 92
Alice, Bob, Carol
80, 65, 92
```

この例では追加した順に取り出されていますが、`Dictionary<TKey, TValue>` は、取り出す順番を保証していません。組を削除したあとに追加すると、順番が変わることがあります。決まった順番で取り出したいときは、キーを `List<T>` に入れて並べ替えるなどの工夫が必要です。

---

## よくあるミス

### 同じキーを Add する

`Add` は、すでにあるキーを指定すると [ArgumentException](https://learn.microsoft.com/dotnet/api/system.argumentexception) を投げます。

```csharp
// ❌ NG: 同じキーを 2 回 Add すると ArgumentException が発生する
Dictionary<string, int> scores = new Dictionary<string, int>();
scores.Add("Alice", 80);
scores.Add("Alice", 90);
```

キーがあれば上書きしてよいなら、インデクサで `scores["Alice"] = 90;` と書きます。

### ないキーをインデクサで読む

インデクサでないキーを読むと、`KeyNotFoundException` が発生します。

```csharp
// ❌ NG: ないキーを読むと KeyNotFoundException が発生する
Dictionary<string, int> scores = new Dictionary<string, int>();
Console.WriteLine(scores["Dave"]);
```

キーがあるとは限らないときは、`TryGetValue` を使います。

---

## ワンポイントアドバイス

### キーから値を速く探せる理由

`Dictionary<TKey, TValue>` は、キーから **ハッシュ値**（hash code）と呼ばれる整数を計算し、その整数をもとに、値を置く場所を決めています。探すときも、キーからハッシュ値を計算して、その場所だけを調べます。先頭から 1 つずつ比べる必要がないので、要素がどれだけ多くても、探す時間はほとんど変わりません。

ハッシュ値は、`object` クラスの `GetHashCode` メソッドで計算されます。`string` や `int` をキーにするときは何もする必要はありませんが、自分で作ったクラスや構造体をキーにするときは、`Equals` と `GetHashCode` の両方を正しくオーバーライドする必要があります。

---

## まとめ

- `Dictionary<TKey, TValue>` は、キーと値を組にして保存し、キーから値を取り出すコレクション
- `Add` は新しい組を追加する。同じキーを `Add` すると `ArgumentException` が発生する
- インデクサで書き込むと、キーがなければ追加、あれば上書きになる
- インデクサでないキーを読むと `KeyNotFoundException` が発生する。キーがあるとは限らないときは `TryGetValue` を使う
- `foreach` では、組が `KeyValuePair<TKey, TValue>` として取り出される。取り出す順番は保証されない
- キーからハッシュ値を計算して場所を決めるので、要素が多くても速く探せる

---

## 理解度チェック

1. 名前から電話番号を調べるデータを作るとき、`List<T>` より `Dictionary<TKey, TValue>` が適しているのはなぜですか？
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   Dictionary<string, int> stock = new Dictionary<string, int> { ["りんご"] = 3 };
   stock["りんご"] += 2;
   stock["みかん"] = 4;
   stock.Remove("りんご");
   Console.WriteLine(stock.Count);
   Console.WriteLine(stock.ContainsKey("りんご"));
   ```

3. `scores.Add("Bob", 70)` と `scores["Bob"] = 70` は、`Bob` がすでに登録されているとき、それぞれどうなりますか？

<details markdown="1">
<summary>解答を見る</summary>

1. `Dictionary<TKey, TValue>` なら、名前をキーにして電話番号を直接取り出せるからです。`List<T>` では、名前が一致する要素を先頭から順に探す必要があり、要素が多いほど時間がかかります。
2. 次のように出力されます。`りんご` は `5` に更新されたあと削除され、残るのは `みかん` だけです。

   ```
   1
   False
   ```

3. `Add` は、キーがすでにあるので `ArgumentException` を投げます。インデクサでの書き込みは、`Bob` の値を `70` に上書きします。

</details>

---

## 次のステップ

[Queue\<T\>・Stack\<T\>・HashSet\<T\>（補足）](/unity-csharp-learning/csharp/other-collections/) では、用途に合わせて使い分ける、そのほかのコレクションを紹介します。
