---
layout: page
title: "補足: IEnumerator と yield return"
permalink: /unity/ienumerator-yield/
---

# 補足: IEnumerator と yield return

Unity のコルーチンは、C# の `IEnumerator` と `yield return` の仕組みを使っています。このページでは、配列を順番に取り出す処理から始めて、`IEnumerable` を自作し、最後に `yield return` で同じ処理を短く書く流れを確認します。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `foreach` の裏側で `IEnumerator` が使われていることを説明できる
- `IEnumerable` / `IEnumerator` を実装したクラスを `foreach` で使えることを理解できる
- `yield return` で列挙処理を簡潔に書けることを理解できる
- Unity のコルーチンが `IEnumerator` を Unity 側で進める仕組みだと説明できる

## 前提知識

- [配列の基礎](/unity-csharp-learning/csharp/arrays/) を読んでいること
- [配列と foreach（補足）](/unity-csharp-learning/csharp/arrays-and-foreach/) を読んでいること
- [インターフェイス](/unity-csharp-learning/csharp/interfaces/) の考え方を知っていること

---

## 1. foreach と IEnumerator

`foreach` は、配列などの要素を先頭から順に取り出す構文です。

```csharp
using UnityEngine;

public class ForeachSample : MonoBehaviour
{
    private void Start()
    {
        int[] numbers = { 10, 20, 30 };

        foreach (int number in numbers)
        {
            Debug.Log(number);
        }
    }
}
```

この `foreach` の裏側では、`GetEnumerator()` で列挙用のオブジェクトを取り出し、`MoveNext()` で次の要素へ進んでいます。手動で書くと次のようになります。

```csharp
using System.Collections;
using UnityEngine;

public class EnumeratorSample : MonoBehaviour
{
    private void Start()
    {
        int[] numbers = { 10, 20, 30 };
        IEnumerator enumerator = numbers.GetEnumerator();

        while (enumerator.MoveNext())
        {
            int number = (int)enumerator.Current;
            Debug.Log(number);
        }
    }
}
```

| 名前 | 役割 |
|---|---|
| `IEnumerable` | 順番に取り出せるものを表す |
| `IEnumerator` | 実際に「次へ進む」ための操作を持つ |
| `MoveNext()` | 次の要素へ進む。次があれば `true`、なければ `false` |
| `Current` | 現在位置の要素 |

`foreach` は、`GetEnumerator()`、`MoveNext()`、`Current` を使う処理を読みやすく書くための構文だと考えるとよいでしょう。

---

## 2. IEnumerable を自分で実装する

配列は最初から `foreach` で使えますが、自分で作ったクラスも `IEnumerable` を実装すれば `foreach` で処理できるようになります。

例として、指定した個数だけフィボナッチ数を返すクラスを作ってみます。フィボナッチ数は、前の2つの数を足して次の数を作る数列です。

```text
0, 1, 1, 2, 3, 5, 8, 13, ...
```

まず `IEnumerable` を実装するクラスを作ります。`GetEnumerator()` は、実際に次の値へ進む `IEnumerator` を返します。

```csharp
using System.Collections;

public class FibonacciSequence : IEnumerable
{
    private int _count;

    public FibonacciSequence(int count)
    {
        _count = count;
    }

    public IEnumerator GetEnumerator()
    {
        return new FibonacciEnumerator(_count);
    }
}
```

次に、`IEnumerator` 側を実装します。

```csharp
using System.Collections;

public class FibonacciEnumerator : IEnumerator
{
    private int _count;
    private int _index = -1;
    private int _previous = 1;
    private int _current = 0;

    public FibonacciEnumerator(int count)
    {
        _count = count;
    }

    public object Current
    {
        get { return _current; }
    }

    public bool MoveNext()
    {
        _index++;
        if (_index >= _count)
        {
            return false;
        }

        if (_index == 0)
        {
            _current = 0;
            _previous = 1;
            return true;
        }

        int next = _previous + _current;
        _previous = _current;
        _current = next;
        return true;
    }

    public void Reset()
    {
        _index = -1;
        _previous = 1;
        _current = 0;
    }
}
```

これで `FibonacciSequence` は `foreach` で使えます。

```csharp
using UnityEngine;

public class FibonacciSample : MonoBehaviour
{
    private void Start()
    {
        var sequence = new FibonacciSequence(8);

        foreach (int number in sequence)
        {
            Debug.Log(number);
        }
    }
}
```

出力は次のようになります。

```text
0
1
1
2
3
5
8
13
```

このように、`IEnumerable` と `IEnumerator` を実装すると、自作のロジックでも「順番に値を取り出せるもの」として扱えます。ただし、`Current`、`MoveNext()`、`Reset()`、内部状態のフィールドをすべて自分で書く必要があり、少し大げさです。

---

## 3. yield return で短く書く

フィボナッチ数のように「次の値を順番に返す」ロジックは、`yield return` を使うとかなり短く書けます。

```csharp
using System.Collections;
using UnityEngine;

public class YieldFibonacciSample : MonoBehaviour
{
    private void Start()
    {
        foreach (int number in Fibonacci(8))
        {
            Debug.Log(number);
        }
    }

    private IEnumerable Fibonacci(int count)
    {
        int previous = 1;
        int current = 0;

        for (int i = 0; i < count; i++)
        {
            yield return current;

            int next = previous + current;
            previous = current;
            current = next;
        }
    }
}
```

`yield return current;` は、「今の値として `current` を返し、次に進められるまでここで止まる」という意味です。次に `MoveNext()` が呼ばれると、止まった場所の続きから再開します。

`yield return` を使うと、`IEnumerator` 用のクラスを別に作らなくても、値を返す流れをそのまま書けます。

---

## 4. 条件付きの列挙にも使える

素数のように、「条件を満たした値だけを順番に返す」処理にも `yield return` は向いています。

```csharp
using System.Collections;
using UnityEngine;

public class YieldPrimeSample : MonoBehaviour
{
    private void Start()
    {
        foreach (int number in PrimeNumbers(20))
        {
            Debug.Log(number);
        }
    }

    private IEnumerable PrimeNumbers(int max)
    {
        for (int n = 2; n <= max; n++)
        {
            if (IsPrime(n))
            {
                yield return n;
            }
        }
    }

    private bool IsPrime(int n)
    {
        for (int i = 2; i * i <= n; i++)
        {
            if (n % i == 0)
            {
                return false;
            }
        }

        return true;
    }
}
```

この例では、`PrimeNumbers(20)` が `2, 3, 5, 7, 11, 13, 17, 19` を順番に返します。

---

## 5. Unity のコルーチンへの橋渡し

`yield return` を使ったメソッドも、内部的には `MoveNext()` で少しずつ進められる `IEnumerator` として扱われます。

Unity のコルーチンは、この `IEnumerator` の仕組みを「フレームをまたぐ処理」に応用したものです。Unity はコルーチンの `IEnumerator` を持っておき、条件を満たしたタイミングで `MoveNext()` を呼んで続きを進めます。

C# の数列では `yield return 10;` のように「値」を返していました。Unity のコルーチンでは、返した値を Unity が「いつ続きを実行するか」の合図として解釈します。

```csharp
private IEnumerator CountFrames()
{
    Debug.Log("1フレーム目");
    yield return null;

    Debug.Log("2フレーム目");
    yield return null;

    Debug.Log("3フレーム目");
}
```

一番基本的な `yield return null` は、「次のフレームまで待つ」という合図です。この考え方を使って、Unity では時間をまたぐ処理をコルーチンとして書けます。

---

## まとめ

- `foreach` の裏側では `GetEnumerator()`、`MoveNext()`、`Current` が使われている
- `IEnumerable` / `IEnumerator` を実装すると、自作クラスも `foreach` で走査できる
- `yield return` を使うと、列挙処理を短く手続き的に書ける
- Unity のコルーチンは `IEnumerator` を Unity が進める仕組みである

---

## 理解度チェック

1. `MoveNext()` はどのような役割を持つメソッドですか？
2. `IEnumerable` を実装したクラスが `foreach` で使えるのはなぜですか？
3. `yield return` を使うと、手書きの `IEnumerator` 実装と比べて何が簡単になりますか？

<details markdown="1">
<summary>解答を見る</summary>

1. 列挙処理を次の位置へ進める。次の要素があれば `true`、もうなければ `false` を返す。
2. `foreach` が `GetEnumerator()` を呼び出して、返された `IEnumerator` を `MoveNext()` で進められるため。
3. `Current`、`MoveNext()`、`Reset()`、内部状態を持つ列挙用クラスを自分で書かなくても、値を返す流れをそのまま書ける。

</details>

---

## 次のステップ

[コルーチンの基本](/unity-csharp-learning/unity/coroutines/) では、`IEnumerator` と `yield return` の仕組みを Unity のフレーム待機や時間待ちに応用する方法を学びます。
