---
layout: page
title: マルチキャストデリゲート
permalink: /csharp/multicast-delegates/
---

# マルチキャストデリゲート

デリゲートには複数のメソッドを登録でき、1 回の呼び出しで全員に通知できます。このしくみを**マルチキャストデリゲート**と呼びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `+=` でデリゲートにメソッドを追加できる
- `-=` で登録したメソッドを解除できる
- 戻り値がある場合に最後のメソッドの値だけが返ることを説明できる
- `GetInvocationList()` で登録済みメソッドを個別に呼び出せる

## 前提知識

- [デリゲートの変数渡しとコールバック](/unity-csharp-learning/csharp/delegate-callback/) を読んでいること

---

## 1. `+=` でメソッドを追加する

**書式：[デリゲートの結合（+= 演算子）](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/addition-operator#delegate-combination)**
```
デリゲート変数 += メソッド名;
```

| 要素 | 説明 |
|---|---|
| `デリゲート変数` | 複数のメソッドを管理するデリゲート変数 |
| `+=` | デリゲートに新しいメソッドを追加する演算子 |
| `メソッド名` | 追加するメソッド（シグネチャが一致すること） |

```csharp
Notify? notify = null;
notify += SayHello;
notify += SayGoodbye;

notify?.Invoke();

void SayHello()
{
    Console.WriteLine("こんにちは！");
}

void SayGoodbye()
{
    Console.WriteLine("さようなら！");
}

delegate void Notify();
```

```
こんにちは！
さようなら！
```

`notify?.Invoke()` を 1 回呼び出すだけで、登録した順に `SayHello` → `SayGoodbye` が実行されます。

---

## 2. `-=` でメソッドを解除する

**書式：[デリゲートの削除（-= 演算子）](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/subtraction-operator#delegate-removal)**
```
デリゲート変数 -= メソッド名;
```

| 要素 | 説明 |
|---|---|
| `-=` | 登録されているメソッドをデリゲートから取り除く演算子 |

```csharp
Notify? notify = null;
notify += SayHello;
notify += SayGoodbye;

notify -= SayHello;   // SayHello だけ解除

notify?.Invoke();

void SayHello()
{
    Console.WriteLine("こんにちは！");
}

void SayGoodbye()
{
    Console.WriteLine("さようなら！");
}

delegate void Notify();
```

```
さようなら！
```

`SayHello` が解除されたため、`SayGoodbye` だけが呼び出されます。登録されていないメソッドを `-=` で解除しようとしても、エラーにはなりません。

---

## 3. 戻り値がある場合

複数のメソッドが登録されたデリゲートに戻り値がある場合、**最後に登録したメソッドの戻り値だけ**が返ります。

```csharp
Calculate calc = Double;
calc += Triple;

int result = calc(5);
Console.WriteLine(result);  // Triple(5) = 15 だけが返る

int Double(int x) => x * 2;   // 10 が返るが捨てられる
int Triple(int x) => x * 3;   // 15 が返る

delegate int Calculate(int x);
```

```
15
```

`Double` の戻り値 `10` は無視されます。複数の戻り値を個別に使いたい場合は、次のセクションで説明する `GetInvocationList()` を使います。

---

## 4. `GetInvocationList()` で個別に呼び出す

`GetInvocationList()` は、デリゲートに登録されているメソッド一覧を配列で返します。これを使うと、各メソッドの戻り値を個別に受け取れます。

**`Delegate.GetInvocationList`** — デリゲートに登録されている全メソッドを `Delegate[]` として返します。

**書式：[Delegate.GetInvocationList メソッド](https://learn.microsoft.com/dotnet/api/system.delegate.getinvocationlist)**
```csharp
Delegate[] GetInvocationList();
```

| パラメータ | 型 | 説明 |
|---|---|---|
| （なし） | — | パラメータはありません |
| **戻り値** | `Delegate[]` | 登録されているメソッドを順番に並べた配列 |

```csharp
Calculate calc = Double;
calc += Triple;

foreach (Calculate c in calc.GetInvocationList())
{
    int result = c(5);
    Console.WriteLine(result);
}

int Double(int x) => x * 2;
int Triple(int x) => x * 3;

delegate int Calculate(int x);
```

```
10
15
```

各メソッドを個別に呼び出しているため、`Double` の結果 `10` と `Triple` の結果 `15` をそれぞれ取得できます。

---

## よくあるミス

メソッドを追加するつもりで `+=` ではなく `=` と書くと、それまでに登録したメソッドがすべて置き換えられます。コンパイルエラーにも警告にもならないため、気づきにくいミスです。

```csharp
Notify? notify = null;
notify += SayHello;

// ❌ NG: = で代入すると、SayHello の登録が消える
notify = SayGoodbye;

notify?.Invoke();

void SayHello()
{
    Console.WriteLine("こんにちは！");
}

void SayGoodbye()
{
    Console.WriteLine("さようなら！");
}

delegate void Notify();
```

```
さようなら！
```

`SayHello` も呼び出したいなら、`notify += SayGoodbye;` と書きます。2 つ目以降の登録には `+=` を使いましょう。

---

## まとめ

- `+=` でデリゲートに複数のメソッドを追加でき、呼び出しは登録順に実行される
- `-=` で登録済みのメソッドを解除できる
- 戻り値があるマルチキャストデリゲートは、最後のメソッドの値だけが返る
- `GetInvocationList()` を使うと各メソッドの戻り値を個別に受け取れる

---

## 理解度チェック

以下の問いに答えられるか確認しましょう。

1. デリゲートに `+=` で 3 つのメソッドを追加して呼び出すと、どのような順で実行されますか？
2. 次のコードの出力結果は何になりますか？

   ```csharp
   Log? log = PrintA;
   log += PrintB;
   log -= PrintA;
   log?.Invoke("test");

   void PrintA(string msg) => Console.WriteLine($"A:{msg}");
   void PrintB(string msg) => Console.WriteLine($"B:{msg}");

   delegate void Log(string msg);
   ```

3. （応用）戻り値 `int` を持つデリゲートに複数のメソッドを登録し、全メソッドの戻り値の合計を求めるにはどう書きますか？

<details markdown="1">
<summary>解答を見る</summary>

1. 登録した順（`+=` で追加した順）に実行されます。
2. `PrintA` が `-=` で解除されているため、出力は次のとおりです。

   ```
   B:test
   ```

3. `GetInvocationList()` で個別に呼び出して合計します。

   ```csharp
   Calculate calc = Double;
   calc += Triple;

   int total = 0;
   foreach (Calculate c in calc.GetInvocationList())
   {
       total += c(5);
   }
   Console.WriteLine(total);

   int Double(int x) => x * 2;
   int Triple(int x) => x * 3;

   delegate int Calculate(int x);
   ```

   ```
   25
   ```

</details>

---

## 次のステップ

[イベント](/unity-csharp-learning/csharp/events/) では、`event` キーワードによってデリゲートをより安全に扱う方法と、発行者/購読者パターンを学びます。
