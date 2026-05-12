---
layout: page
title: ローカル関数
permalink: /csharp/local-functions/
---

# ローカル関数

**ローカル関数**は、メソッドの内側に定義できる関数です。そのメソッドの中だけで使いたい補助的な処理をまとめるのに役立ちます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- メソッドの内側にローカル関数を定義できる
- ローカル関数で再帰処理を書ける
- `static` ローカル関数でキャプチャを禁止できる
- ラムダ式とローカル関数の使い分けを説明できる

## 前提知識

- [変数キャプチャ](/unity-csharp-learning/csharp/variable-capture/) を読んでいること

---

## 1. ローカル関数の構文

**書式：ローカル関数の定義**
```
戻り値型 関数名(パラメータリスト)
{
    // 処理
}
```

| 要素 | 説明 |
|---|---|
| `戻り値型` | 返す値の型（`void` も可）|
| `関数名` | 通常のメソッドと同じ命名規則（`PascalCase` 推奨）|
| `パラメータリスト` | 引数の一覧 |

ローカル関数は、それを含むメソッドのスコープ内でのみ呼び出せます。

```csharp
using System;

public class Program
{
    public static void Main()
    {
        PrintResult(10, 3);

        void PrintResult(int a, int b)
        {
            int sum = Add(a, b);
            Console.WriteLine($"{a} + {b} = {sum}");
        }

        int Add(int x, int y) => x + y;   // 式形式のローカル関数
    }
}
```

```
10 + 3 = 13
```

> 💡 **ポイント**: ローカル関数は定義より前の行でも呼び出せます（`PrintResult` は `Add` の定義前に呼ばれていますが問題ありません）。

---

## 2. ローカル関数と再帰

ローカル関数は**再帰呼び出し**が得意です。再帰に必要なロジックを外部に公開せずにメソッドの内側に閉じ込めることができます。

```csharp
using System;

public class Program
{
    public static void Main()
    {
        Console.WriteLine(Factorial(5));

        int Factorial(int n)
        {
            if (n <= 1) return 1;
            return n * Factorial(n - 1);   // ローカル関数の再帰呼び出し
        }
    }
}
```

```
120
```

`Factorial` という名前は `Main` の内部だけに存在し、外部からは呼び出せません。補助的な再帰処理を外のクラスメンバーとして公開しなくて済みます。

---

## 3. 外側の変数のキャプチャ

ローカル関数もラムダ式と同様に、外側スコープの変数をキャプチャできます。

```csharp
using System;

public class Program
{
    public static void Main()
    {
        int baseScore = 100;

        void PrintFinalScore(int bonus)
        {
            Console.WriteLine($"最終スコア: {baseScore + bonus}");   // baseScore をキャプチャ
        }

        PrintFinalScore(50);
    }
}
```

```
最終スコア: 150
```

---

## 4. `static` ローカル関数

ローカル関数に `static` を付けると、外側の変数や `this` のキャプチャが禁止されます。ラムダ式の `static` と同じ効果で、意図しないキャプチャをコンパイル時に検出できます。

**書式：static ローカル関数**
```
static 戻り値型 関数名(パラメータリスト)
{
    // 処理
}
```

```csharp
using System;

public class Program
{
    public static void Main()
    {
        int multiplier = 3;

        // ❌ NG: static ローカル関数で外側の変数を使うとコンパイルエラー
        // static int BadMultiply(int x) => x * multiplier;

        // ✅ OK: 必要な値はパラメータとして受け取る
        static int Multiply(int x, int factor) => x * factor;

        Console.WriteLine(Multiply(7, multiplier));
    }
}
```

```
21
```

---

## 5. ラムダ式との使い分け

ローカル関数とラムダ式はどちらもメソッド内で処理を定義できますが、得意な場面が異なります。

| 比較項目 | ローカル関数 | ラムダ式 |
|---|---|---|
| 再帰呼び出し | ✅ 得意（名前で呼べる） | ❌ 難しい（変数に代入してから呼ぶ必要がある） |
| パラメータ名の明示 | ✅ 宣言に型とパラメータ名を書く | ⚠️ 型推論で省略されることが多い |
| デバッガのスタックトレース | ✅ 名前が表示される | ⚠️ 匿名として表示されることがある |
| 短い 1 行の処理 | ⚠️ やや冗長 | ✅ 簡潔に書ける |
| `Action` / `Func` に渡す | ⚠️ 名前でメソッドグループ変換が必要 | ✅ そのまま渡せる |

一般的な指針として、**再帰や複数行の補助処理にはローカル関数**、**1〜2 行の簡単な処理や即座にデリゲートに渡すにはラムダ式**を使うと読みやすくなります。

---

## よくあるミス

```csharp
// ❌ NG: ローカル関数を含むメソッドの外から呼ぼうとする
// （コンパイルエラー：スコープ外からアクセス不可）
public class Example
{
    public static void Main()
    {
        void Helper() { /* ... */ }
    }

    public static void Other()
    {
        Helper();   // エラー：Helper は Main の外からは見えない
    }
}

// ✅ OK: ローカル関数はそのメソッド内からのみ呼び出す
```

---

## まとめ

- ローカル関数はメソッドの内側に定義でき、そのメソッドの中からのみ呼び出せる
- 再帰処理を外部に公開せず内側に閉じ込めるのが得意
- 外側スコープの変数をキャプチャできる（`static` を付けると禁止）
- 再帰・複数行・デバッグしやすさが必要な場合はローカル関数、短い処理はラムダ式を使う

---

## 理解度チェック

以下の問いに答えられるか確認しましょう。

1. ローカル関数をラムダ式より選ぶ理由を 2 つ挙げてください。
2. 次のコードの出力結果は何になりますか？

   ```csharp
   using System;

   public class Program
   {
       public static void Main()
       {
           Console.WriteLine(Sum(4));

           int Sum(int n)
           {
               if (n <= 0) return 0;
               return n + Sum(n - 1);
           }
       }
   }
   ```

3. （応用）`static` ローカル関数と `static` ラムダの違いを説明してください。

<details markdown="1">
<summary>解答を見る</summary>

1. ① 再帰呼び出しが自然に書ける。② パラメータ名が明示されてコードが読みやすくなる（デバッガのスタックトレースにも名前が出る）。

2. ```
   10
   ```
   `4 + 3 + 2 + 1 + 0 = 10` が返ります。

3. どちらも外側スコープの変数や `this` をキャプチャするとコンパイルエラーになる点は同じです。`static` ローカル関数は通常のメソッドと同じ構文（名前・パラメータ型・戻り値型を明示）で書け、再帰呼び出しができます。`static` ラムダは `Action` / `Func` などのデリゲート変数に代入する形で書きます。

</details>

---

## 次のステップ

このページで「C# デリゲートとイベント」の章は終了です。デリゲート・コールバック・イベント・ラムダ式・変数キャプチャ・ローカル関数というひと続きの知識を習得しました。
