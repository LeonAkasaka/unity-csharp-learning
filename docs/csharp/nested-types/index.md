---
layout: page
title: 入れ子の型
permalink: /csharp/nested-types/
---

# 入れ子の型

クラスの中には、フィールドやメソッドだけでなく、別のクラスも定義できます。型の中に定義した型を **入れ子の型**（nested type）といいます。このページでは、あるクラスの中でしか使わない補助のクラスを、そのクラスの中に入れて外から隠す方法と、入れ子の型と外側の型の関係を学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- クラスの中にクラスを定義し、`private` にして外から隠せる
- 入れ子の型を、外から `外側の型名.入れ子の型名` で使える
- 入れ子の型から、外側の型の `private` メンバーを使える
- 入れ子の型が外側の型のインスタンスを持たないことを説明できる

## 前提知識

- [プロパティ](/unity-csharp-learning/csharp/properties/) を読んでいること
- [名前空間](/unity-csharp-learning/csharp/namespaces/) を読んでいること

---

## 1. 補助のクラスを外に置くと困ること

得点の記録を最大 3 件まで保存する `ScoreBoard` クラスを作ります。1 件の記録（名前と得点）は、補助のクラス `ScoreBoardEntry` で表します。

```csharp
ScoreBoard board = new ScoreBoard();
board.Add("Alice", 120);
board.Add("Bob", 90);
board.Print();

ScoreBoardEntry entry = new ScoreBoardEntry("Carol", 999);
Console.WriteLine($"{entry.Name}: {entry.Score}");

class ScoreBoard
{
    private ScoreBoardEntry[] _entries = new ScoreBoardEntry[3];
    private int _count;

    public void Add(string name, int score)
    {
        if (_count == _entries.Length)
        {
            return;
        }
        _entries[_count] = new ScoreBoardEntry(name, score);
        _count++;
    }

    public void Print()
    {
        for (int i = 0; i < _count; i++)
        {
            Console.WriteLine($"{i + 1}. {_entries[i].Name}: {_entries[i].Score}");
        }
    }
}

class ScoreBoardEntry
{
    public string Name { get; }
    public int Score { get; }

    public ScoreBoardEntry(string name, int score)
    {
        Name = name;
        Score = score;
    }
}
```

```
1. Alice: 120
2. Bob: 90
Carol: 999
```

`ScoreBoardEntry` は、`ScoreBoard` の中で記録を保存するためだけのクラスです。それでも、型の外に置いたクラスなので、`ScoreBoard` と関係のない場所でも `new ScoreBoardEntry("Carol", 999)` のように作れてしまいます。上のコードの `Carol` の記録は、`ScoreBoard` には入っていません。

また、ほかのクラスにも「記録」を表す補助のクラスがあると、名前がぶつからないように `ScoreBoardEntry` のような長い名前を付ける必要があります。

[アクセス修飾子](/unity-csharp-learning/csharp/access-modifiers/) で学んだ `private` は、フィールドやメソッドを外から隠すものでした。クラスも、別のクラスの中に入れれば、`private` にして隠せます。

---

## 2. 入れ子の型を定義する

[入れ子の型](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/nested-types) は、クラスの `{ }` の中に、フィールドやメソッドと並べて定義します。

**書式：[入れ子の型](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/nested-types)**
```
class 外側の型名
{
    アクセス修飾子 class 入れ子の型名
    {
        // 入れ子の型のメンバー
    }
}
```

| 要素 | 説明 |
|---|---|
| 外側の型名 | 入れ子の型を含む型。**外側の型**（containing type）という |
| アクセス修飾子 | フィールドやメソッドと同じように、`private` や `public` を付けられる |

1 節の `ScoreBoardEntry` を、`ScoreBoard` の中の `private` なクラス `Entry` にします。

```csharp
ScoreBoard board = new ScoreBoard();
board.Add("Alice", 120);
board.Add("Bob", 90);
board.Print();

class ScoreBoard
{
    private Entry[] _entries = new Entry[3];
    private int _count;

    public void Add(string name, int score)
    {
        if (_count == _entries.Length)
        {
            return;
        }
        _entries[_count] = new Entry(name, score);
        _count++;
    }

    public void Print()
    {
        for (int i = 0; i < _count; i++)
        {
            Console.WriteLine($"{i + 1}. {_entries[i].Name}: {_entries[i].Score}");
        }
    }

    private class Entry
    {
        public string Name { get; }
        public int Score { get; }

        public Entry(string name, int score)
        {
            Name = name;
            Score = score;
        }
    }
}
```

```
1. Alice: 120
2. Bob: 90
```

`ScoreBoard` の中では、入れ子の型を `Entry` という名前だけで使えます。`Entry` は `private` なので、`ScoreBoard` の外からは使えません。

```csharp
// ❌ NG: Entry は ScoreBoard の private なクラスなので、外からは使えない
// ScoreBoard.Entry entry = new ScoreBoard.Entry("Carol", 999);  // CS0122
```

`ScoreBoard` の外で記録を作ることはできなくなりました。`Entry` という短い名前も、ほかのクラスの中の `Entry` とはぶつかりません。

アクセス修飾子を省略したときのアクセスレベルは、型を置いた場所によって違います。

| 型を置いた場所 | アクセス修飾子を省略したとき |
|---|---|
| ほかの型の外（これまでのクラス） | `internal`（[protected 修飾子](/unity-csharp-learning/csharp/protected-modifier/) で学ぶ） |
| ほかの型の中（入れ子の型） | `private`（フィールドやメソッドと同じ） |

---

## 3. 入れ子の型を外から使う

入れ子の型を `public` にすると、外側の型の外からも使えます。外からは、`外側の型名.入れ子の型名` と書きます。

`ScoreBoard` に、記録を 1 件返す `Get` メソッドを追加します。`Get` は `public` なメソッドで、戻り値の型が `Entry` なので、`Entry` も `public` にします。

```csharp
ScoreBoard board = new ScoreBoard();
board.Add("Alice", 120);
board.Add("Bob", 90);

ScoreBoard.Entry first = board.Get(0);
Console.WriteLine($"{first.Name}: {first.Score}");

class ScoreBoard
{
    private Entry[] _entries = new Entry[3];
    private int _count;

    public void Add(string name, int score)
    {
        if (_count == _entries.Length)
        {
            return;
        }
        _entries[_count] = new Entry(name, score);
        _count++;
    }

    public Entry Get(int index)
    {
        return _entries[index];
    }

    public class Entry
    {
        public string Name { get; }
        public int Score { get; }

        public Entry(string name, int score)
        {
            Name = name;
            Score = score;
        }
    }
}
```

```
Alice: 120
```

`ScoreBoard.Entry` は、[名前空間](/unity-csharp-learning/csharp/namespaces/) で学んだ `名前空間.型名` と同じ形です。名前空間が型をまとめる入れ物であるように、外側の型も、入れ子の型の名前をまとめる入れ物になります。

`Entry` を `private` のままにすると、`public` の `Get` が、外から使えない型を返すことになるので、コンパイルエラーになります。

```csharp
// ❌ NG: public なメソッドの戻り値の型が、private な入れ子の型になっている
// class ScoreBoard
// {
//     public Entry Get(int index)  // CS0050
//     {
//         ...
//     }
//
//     private class Entry
//     {
//     }
// }
```

---

## 4. 外側の型のメンバーを使う

入れ子の型は、外側の型のメンバーの 1 つなので、外側の型の `private` なメンバーも使えます。

`ScoreBoard` の `private` な定数 `MaxScore` を、`Entry` のコンストラクターから使い、得点の上限を決めます。

```csharp
ScoreBoard board = new ScoreBoard();
board.Add("Alice", 120);
board.Add("Bob", 1500);
board.Print();

class ScoreBoard
{
    private const int MaxScore = 999;

    private Entry[] _entries = new Entry[3];
    private int _count;

    public void Add(string name, int score)
    {
        if (_count == _entries.Length)
        {
            return;
        }
        _entries[_count] = new Entry(name, score);
        _count++;
    }

    public void Print()
    {
        for (int i = 0; i < _count; i++)
        {
            Console.WriteLine($"{i + 1}. {_entries[i].Name}: {_entries[i].Score}");
        }
    }

    private class Entry
    {
        public string Name { get; }
        public int Score { get; }

        public Entry(string name, int score)
        {
            Name = name;
            if (score > MaxScore)
            {
                score = MaxScore;
            }
            Score = score;
        }
    }
}
```

```
1. Alice: 120
2. Bob: 999
```

`MaxScore` は `ScoreBoard` の `private` なメンバーですが、`Entry` の中から使えています。

反対に、外側の型から、入れ子の型の `private` なメンバーは使えません。`Entry` に `private` なフィールドがあっても、`ScoreBoard` のメソッドからは使えません（CS0122）。

### 外側の型のインスタンスは持たない

`MaxScore` は定数なので、`ScoreBoard` のインスタンスがなくても使えました。`_count` や `_entries` のようなインスタンスのフィールドは、入れ子の型の中から、名前だけでは使えません。

```csharp
// ❌ NG: Entry は、どの ScoreBoard のインスタンスの _count なのかわからない
// class ScoreBoard
// {
//     private int _count;
//
//     private class Entry
//     {
//         public int GetCount()
//         {
//             return _count;  // CS0120
//         }
//     }
// }
```

入れ子の型のインスタンスは、外側の型のインスタンスとは別々に作られ、どちらかがもう一方の中に含まれるわけではありません。入れ子の型の中で外側の型のインスタンスのメンバーを使いたいときは、コンストラクターなどで、外側の型のインスタンスへの参照を受け取ります。

`ScoreBoard` の記録を 1 件ずつ取り出す `Reader` クラスを、入れ子の型として作ります。`Reader` は、コンストラクターで `ScoreBoard` を受け取り、その `private` なフィールドを読み取ります。

```csharp
ScoreBoard board = new ScoreBoard();
board.Add("Alice", 120);
board.Add("Bob", 90);

ScoreBoard.Reader reader = board.CreateReader();
while (reader.HasNext)
{
    Console.WriteLine(reader.Next());
}

class ScoreBoard
{
    private Entry[] _entries = new Entry[3];
    private int _count;

    public void Add(string name, int score)
    {
        if (_count == _entries.Length)
        {
            return;
        }
        _entries[_count] = new Entry(name, score);
        _count++;
    }

    public Reader CreateReader()
    {
        return new Reader(this);
    }

    private class Entry
    {
        public string Name { get; }
        public int Score { get; }

        public Entry(string name, int score)
        {
            Name = name;
            Score = score;
        }
    }

    public class Reader
    {
        private ScoreBoard _board;
        private int _position;

        public Reader(ScoreBoard board)
        {
            _board = board;
        }

        public bool HasNext
        {
            get { return _position < _board._count; }
        }

        public string Next()
        {
            Entry entry = _board._entries[_position];
            _position++;
            return $"{entry.Name}: {entry.Score}";
        }
    }
}
```

```
Alice: 120
Bob: 90
```

`CreateReader` は、`this`（自分自身の `ScoreBoard`）を `Reader` のコンストラクターに渡します。`Reader` は、受け取った `_board` を通して、`ScoreBoard` の `private` な `_count` と `_entries` を読み取っています。`ScoreBoard` の外のクラスなら、`_board._count` は CS0122 になります。

このように、外側の型の中身を詳しく知っている補助のクラスを、外側の型の中に置くことで、`private` なフィールドを外に公開せずに済みます。

---

## よくあるミス

### 入れ子の型を「内部クラス」として使う

ほかのプログラミング言語には、外側のクラスのインスタンスに自動で結び付く「内部クラス」を持つものがあります（Java など）。C# の入れ子の型は、外側の型のインスタンスに結び付きません。4 節で見たように、外側の型のインスタンスのメンバーを使うには、参照を受け取る必要があります。

### 何でも入れ子にする

入れ子の型は、外側の型の中でしか使わない補助の型や、外側の型と一緒にしか使わない型に使います。ほかのクラスからも単独で使う型は、入れ子にせず、ふつうの型として定義します。外から `外側の型名.入れ子の型名` と長い名前で書くことが多いなら、入れ子にしないほうがよいという目安になります。

---

## ワンポイントアドバイス

### .NET の入れ子の型

.NET のクラスにも、入れ子の型があります。たとえば、[List\<T\>](/unity-csharp-learning/csharp/list/) には、要素を 1 つずつ取り出すための `List<T>.Enumerator` という入れ子の構造体があります。4 節の `Reader` と同じように、外側の型の中身を詳しく知っている補助の型です。[IEnumerable\<T\> と foreach の仕組み](/unity-csharp-learning/csharp/ienumerable/) で、このような取り出し役の型を自分で作ります。

---

## まとめ

- クラスの中に定義した型を入れ子の型という。フィールドやメソッドと同じように、アクセス修飾子を付けられる
- 入れ子の型のアクセス修飾子を省略すると `private` になる。外側の型の中でしか使わない補助の型は、`private` にして隠す
- `public` な入れ子の型は、外から `外側の型名.入れ子の型名` で使える
- 入れ子の型からは、外側の型の `private` なメンバーを使える。外側の型からは、入れ子の型の `private` なメンバーを使えない
- 入れ子の型は、外側の型のインスタンスを持たない。インスタンスのメンバーを使うときは、参照を受け取る

---

## 理解度チェック

1. 次のコードの `Item` を、`Shop` の外から `new Shop.Item()` で作れますか？理由も答えてください。

   ```csharp
   class Shop
   {
       class Item
       {
       }
   }
   ```

2. 次のコードを実行すると何が出力されますか？

   ```csharp
   Counter c = new Counter();
   c.Increment();
   c.Increment();

   Counter.Viewer viewer = new Counter.Viewer(c);
   Console.WriteLine(viewer.Show());
   c.Increment();
   Console.WriteLine(viewer.Show());

   class Counter
   {
       private int _count;

       public void Increment()
       {
           _count++;
       }

       public class Viewer
       {
           private Counter _counter;

           public Viewer(Counter counter)
           {
               _counter = counter;
           }

           public string Show()
           {
               return $"count = {_counter._count}";
           }
       }
   }
   ```

3. 次のコードは、`Show` の中の `_count` でコンパイルエラーになります。理由を説明してください。

   ```csharp
   class Counter
   {
       private int _count;

       public class Viewer
       {
           public string Show()
           {
               return $"count = {_count}";
           }
       }
   }
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. 作れません（CS0122）。入れ子の型のアクセス修飾子を省略すると `private` になるので、`Item` は `Shop` の中からしか使えません。
2. 次のように出力されます。`Viewer` は `private` な `_count` を、受け取った `Counter` の参照を通して読み取ります。2 回目の `Show` の前に `Increment` を呼んでいるので、同じ `Counter` の `_count` は `3` になっています。

   ```
   count = 2
   count = 3
   ```

3. 入れ子の型 `Viewer` は、`Counter` のインスタンスを持たないからです。どの `Counter` の `_count` なのかが決まらないので、CS0120 になります。2 の `Viewer` のように、`Counter` の参照を受け取って、`_counter._count` と書きます。

</details>

---

## 次のステップ

これで「C# プログラムの構成」のセクションは終わりです。[継承](/unity-csharp-learning/csharp/inheritance/) からは「C# 継承と抽象化」のセクションに進み、既存のクラスのメンバーを引き継いで、新しいクラスを作る仕組みを学びます。
