---
layout: page
title: 再帰関数とコールスタック
permalink: /csharp/recursion/
---

# 再帰関数とコールスタック

メソッドが自分自身を呼び出す書き方を**再帰**といいます。再帰を理解するには、呼び出しのたびに情報が積まれる**コールスタック**も合わせて見る必要があります。

## 学習目標

- 再帰が「自分自身を呼び出すこと」だと説明できる
- 終了条件が必要な理由を理解できる
- コールスタックでスタックフレームが積み重なる流れを説明できる
- 再帰で書ける処理をループでも書けることを確認できる

## 前提知識

- [演算子のオーバーロード](/unity-csharp-learning/csharp/operator-overloading/) を読んでいること

---

## 1. 再帰とは何か

再帰は、**メソッドが自分自身を呼び出すこと**です。

**書式：再帰呼び出し**
```
戻り値の型 メソッド名(型 引数名)
{
    if (終了条件)
    {
        return;
    }

    処理
    メソッド名(次の値);
}
```

| 要素 | 説明 |
|---|---|
| `終了条件` | 再帰を止める条件 |
| `処理` | その段階で実行したい内容 |
| `次の値` | 次の呼び出しに渡す値 |

再帰では、**必ず終了条件が必要**です。終了条件がないと、自分自身を呼び出し続けて止まりません。

---

## 2. 終了条件がないと何が起こるか

終了条件がない再帰は、呼び出しを無限に続けます。するとコールスタックがあふれ、最終的に `StackOverflowException` になります。

このため、再帰を書くときは最初に「どこで止めるか」を決めます。終了条件は**先頭で確認する**のが基本です。

---

## 3. コールスタック

メソッドを呼び出すたびに、コールスタックに**スタックフレーム**が積まれます。スタックフレームには、戻り先やローカル変数など、その呼び出しに必要な情報が入ります。

- メソッドを呼び出すたびにフレームが積まれる
- `return` すると、そのフレームが解放される
- 再帰の深さが増えるほど、フレームが多く積まれる

再帰では同じメソッド名でも、各呼び出しは別々のフレームとして管理されます。

---

## 4. 実行例

```csharp
class A
{
    public void M(int n)
    {
        if (n <= 0) { Console.WriteLine("A.M: done"); return; }
        Console.WriteLine($"A.M: n={n}");
        M(n - 1);
    }
}

var a = new A();
a.M(3);
```

```
A.M: n=3
A.M: n=2
A.M: n=1
A.M: done
```

この例では、`n` を 1 ずつ減らしながら自分自身を呼び出しています。`n <= 0` になった時点で終了します。

### コールスタックのイメージ

```
M(3) 実行中
  └─ M(2) 実行中
       └─ M(1) 実行中
            └─ M(0): return → フレーム解放
         return → フレーム解放
    return → フレーム解放
```

`M(3)` が終わる前に `M(2)`、その前に `M(1)`、さらに `M(0)` が呼ばれます。`M(0)` が `return` すると、積まれた順とは逆順にフレームが解放されます。

---

## 5. ループで書き直すとどうなるか

同じ処理は、再帰ではなくループでも書けます。

```csharp
for (int n = 3; n > 0; n--)
{
    Console.WriteLine($"A.M: n={n}");
}
Console.WriteLine("A.M: done");
```

```
A.M: n=3
A.M: n=2
A.M: n=1
A.M: done
```

再帰とループは、どちらでも書ける場合があります。重要なのは「終了条件があること」と「どの順番で処理が進むか」を正しく追えることです。

---

## よくあるミス

### ミス①：終了条件を書き忘れる

```csharp
class A
{
    // ❌ NG: 止まる条件がないので無限再帰になる
    public void M(int n)
    {
        Console.WriteLine($"A.M: n={n}");
        M(n - 1);
    }

    // ✅ OK: 先頭で終了条件を確認する
    public void N(int n)
    {
        if (n <= 0) { Console.WriteLine("A.N: done"); return; }
        Console.WriteLine($"A.N: n={n}");
        N(n - 1);
    }
}
```

### ミス②：終了条件を後ろに書いて追いにくくする

```csharp
class A
{
    // ❌ NG: 先に再帰呼び出ししてから条件を確認すると流れを追いにくい
    public void M(int n)
    {
        Console.WriteLine($"A.M: n={n}");
        if (n <= 0) { return; }
        M(n - 1);
    }

    // ✅ OK: 最初に終了条件を確認する
    public void N(int n)
    {
        if (n <= 0) { Console.WriteLine("A.N: done"); return; }
        Console.WriteLine($"A.N: n={n}");
        N(n - 1);
    }
}
```

---

## まとめ

- 再帰は、メソッドが自分自身を呼び出すこと
- 再帰には必ず終了条件が必要で、ないと `StackOverflowException` になる
- メソッド呼び出しのたびに、コールスタックへスタックフレームが積まれる
- `return` するとフレームが解放される
- 同じ処理をループで書ける場合もある

---

## 理解度チェック

以下の問いに答えられるか確認しましょう。

1. 再帰で終了条件が必要なのはなぜですか？
2. 次のコードの出力結果は何になりますか？

   ```csharp
   class A
   {
       public void M(int n)
       {
           if (n <= 1) { Console.WriteLine("A.M: done"); return; }
           Console.WriteLine($"A.M: n={n}");
           M(n - 1);
       }
   }

   var a = new A();
   a.M(3);
   ```

3. `A.M(int n)` が `n` を表示しながら `0` まで減らし、最後に `"A.M: done"` を表示する再帰メソッドを書いてください。

<details markdown="1">
<summary>解答を見る</summary>

1. 止まる条件がないと自分自身を呼び出し続け、コールスタックがあふれるためです。
2. 
   ```
   A.M: n=3
   A.M: n=2
   A.M: done
   ```
3. ```csharp
   class A
   {
       public void M(int n)
       {
           if (n <= 0) { Console.WriteLine("A.M: done"); return; }
           Console.WriteLine($"A.M: n={n}");
           M(n - 1);
       }
   }
   ```

</details>

---

## 次のステップ

[static メンバーと static クラス](/unity-csharp-learning/csharp/static-members/) では、インスタンスに属するメンバーとクラスに属するメンバーの違いを学びます。
