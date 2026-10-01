---
layout: page
title: マルチキャストデリゲート
permalink: /csharp/multicast-delegates/
---

# マルチキャストデリゲート

デリゲートには複数のメソッドを登録でき、1 回の呼び出しで、登録したすべてのメソッドを呼び出せます。このしくみを**マルチキャストデリゲート**（multicast delegate）と呼びます。1 つの出来事を、複数の相手に知らせたいときに使います。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- 1 つの出来事を複数の相手に知らせたい場面で、マルチキャストデリゲートが役立つことを説明できる
- `+=` でデリゲートにメソッドを追加できる
- `-=` で登録したメソッドを解除できる
- 戻り値がある場合に最後のメソッドの値だけが返ることを説明できる
- `GetInvocationList()` で登録済みメソッドを個別に呼び出せる
- `+=` が新しいデリゲートを作ることと、途中のメソッドで例外が起きたときの動きを説明できる

## 前提知識

- [デリゲートの変数渡しとコールバック](/unity-csharp-learning/csharp/delegate-callback/) を読んでいること

---

## 1. 複数の相手に知らせたい

[デリゲートの変数渡しとコールバック](/unity-csharp-learning/csharp/delegate-callback/) では、処理の完了を 1 つのメソッドに知らせました。実際には、1 つの出来事を、複数の相手に知らせたいことがよくあります。たとえば、ダウンロードが完了したら、画面にメッセージを出し、さらにログにも記録したい、といった場面です。

これまでのデリゲートで実現するには、知らせたい相手をすべて呼び出すメソッドを作って、それを渡すことになります。

```csharp
Action onComplete = NotifyAll;
onComplete();

void NotifyAll()
{
    ShowMessage();
    WriteLog();
}

void ShowMessage()
{
    Console.WriteLine("完了しました");
}

void WriteLog()
{
    Console.WriteLine("[ログ] ダウンロード完了");
}
```

```
完了しました
[ログ] ダウンロード完了
```

この方法では、知らせる相手を増やしたり減らしたりするたびに、`NotifyAll` を書き換えなければなりません。また、実行中に「ここからはログを記録しない」のように相手を変えることもできません。

デリゲートには、メソッドを 1 つだけでなく、複数入れられます。知らせる相手を、デリゲートに登録したり解除したりして管理できるのが、マルチキャストデリゲートです。

---

## 2. `+=` でメソッドを追加する

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

## 3. `-=` でメソッドを解除する

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

同じメソッドを 2 回登録すると、2 回呼び出されます。そのメソッドを `-=` で解除すると、最後に登録した 1 つだけが取り除かれます。

```csharp
Notify? notify = null;
notify += SayHello;
notify += SayGoodbye;
notify += SayHello;   // SayHello をもう一度登録する

notify -= SayHello;   // 最後に登録した SayHello だけが取り除かれる

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

---

## 4. `+=` は新しいデリゲートを作る

デリゲートは、一度作ると中身（登録されているメソッドの並び）を変えられません。`+=` や `-=` は、既存のデリゲートを書き換えるのではなく、**メソッドを追加した（取り除いた）新しいデリゲートを作って、変数に代入し直します**。

次の例では、`a` を `b` にコピーしてから、`a` に `+=` しています。

```csharp
Action a = SayHello;
Action b = a;          // b は、SayHello だけが入ったデリゲートを指す

a += SayGoodbye;       // 新しいデリゲートが作られ、a に代入される

Console.WriteLine("a:");
a();
Console.WriteLine("b:");
b();

void SayHello()
{
    Console.WriteLine("こんにちは！");
}

void SayGoodbye()
{
    Console.WriteLine("さようなら！");
}
```

```
a:
こんにちは！
さようなら！
b:
こんにちは！
```

`a += SayGoodbye` は `a = a + SayGoodbye` と同じ意味で、`b` が指している元のデリゲートは変わりません。そのため、`b` を呼び出しても `SayHello` だけが実行されます。

この性質のおかげで、呼び出しの直前にデリゲートを変数にコピーしておけば、そのあとでほかのコードが `+=` や `-=` をしても、コピーしたデリゲートの中身は変わりません。`notify?.Invoke()` も、`notify` の値を一度だけ読み取ってから呼び出すので、`null` かどうかを調べてから呼び出すまでの間に、ほかのコードが `notify` を `null` にしても失敗しません。

---

---

## 5. 戻り値がある場合

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

## 6. `GetInvocationList()` で個別に呼び出す

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

> 💡 **ポイント**: マルチキャストデリゲートを呼び出したとき、途中のメソッドで例外（実行時のエラー）が起きると、そのあとに登録されたメソッドは呼び出されません。登録した順に 1 つずつ呼び出しているからです。1 つのメソッドが失敗しても残りのメソッドを呼び出したいときは、`GetInvocationList()` で 1 つずつ取り出し、それぞれの呼び出しで例外を処理します。例外の処理は、[例外の基本](/unity-csharp-learning/csharp/exceptions/) で学びます。

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

- マルチキャストデリゲートを使うと、1 つの出来事を知らせる相手を、登録と解除で管理できる
- `+=` でデリゲートに複数のメソッドを追加でき、呼び出しは登録順に実行される
- `-=` で登録済みのメソッドを解除できる。同じメソッドが複数あるときは、最後に登録したものが取り除かれる
- デリゲートは変更できず、`+=` / `-=` は新しいデリゲートを作って代入し直す
- 戻り値があるマルチキャストデリゲートは、最後のメソッドの値だけが返る
- `GetInvocationList()` を使うと各メソッドの戻り値を個別に受け取れる
- 途中のメソッドで例外が起きると、そのあとのメソッドは呼び出されない

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
