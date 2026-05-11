---
layout: page
title: オーバーロード解決
permalink: /csharp/overload-resolution/
---

# オーバーロード解決

同じ名前のメソッドが複数あるとき、C# コンパイラは呼び出しに最も適した 1 つを選びます。この選択処理がオーバーロード解決です。選ばれるオーバーロードを把握していないと、意図しない別のメソッドが呼ばれてバグになる場合があります。

## 学習目標

- 候補の絞り込みステップを説明できる
- 完全一致 > 暗黙変換 > params 展開の優先順位を説明できる
- あいまいな呼び出しがコンパイルエラーになる理由を説明できる
- 名前付き引数がオーバーロード解決にどう影響するかを説明できる

## 前提知識

- [params キーワード](/unity-csharp-learning/csharp/params-keyword/) を読んでいること
- [省略可能パラメータと名前付き引数](/unity-csharp-learning/csharp/optional-named-params/) を読んでいること

---

## 1. 候補の絞り込み

コンパイラは最初に、**名前が一致し、引数の数と型の組み合わせで呼び出し可能なメソッド**を候補として集めます。この段階ではまだ 1 つに決めません。

```csharp
class A
{
    public void M(int x)
    {
        Console.WriteLine($"A.M(int): x={x}");
    }

    public void M(double x)
    {
        Console.WriteLine($"A.M(double): x={x}");
    }
}

var a = new A();
a.M(1);
```

```
A.M(int): x=1
```

`a.M(1)` では、`M(int x)` は `int` に完全一致するので候補です。`M(double x)` も `int` から `double` への暗黙変換（コードに変換処理を書かなくても自動で行われる型変換）で呼び出せるため候補です。このあとの優先順位の比較で、どの候補を呼ぶか最終的に決まります。

## 2. 優先順位（高 → 低）

候補が複数あるときは、次の順序で比較します。

1. 完全一致
2. 暗黙的型変換後の一致
3. params 展開後の一致

#### ① 完全一致

引数の型がシグネチャ（メソッド名と引数の型・数の組み合わせ）とそのまま一致するときは、完全一致が最優先です。

```csharp
class A
{
    public void M(int x)
    {
        Console.WriteLine($"A.M(int): x={x}");
    }

    public void M(long x)
    {
        Console.WriteLine($"A.M(long): x={x}");
    }
}

var a = new A();
a.M(1);    // int リテラル → 完全一致で M(int) が選ばれる
a.M(1L);   // long リテラル → 完全一致で M(long) が選ばれる
```

```
A.M(int): x=1
A.M(long): x=1
```

`1` のようにコードに直接書いた値はリテラルです。`1` は `int`、`1L` は `long` として扱われるので、それぞれ対応するオーバーロードが完全一致で選ばれます。

#### ② 暗黙的型変換後の一致

完全一致がない場合は、暗黙的型変換で呼び出せる候補が選ばれます。

```csharp
class A
{
    public void M(long x)
    {
        Console.WriteLine($"A.M(long): x={x}");
    }
}

var a = new A();
int b = 3;
a.M(b);   // int → long 暗黙変換
```

```
A.M(long): x=3
```

この例では `M(int)` はありませんが、`int` から `long` へ暗黙変換できるため `M(long x)` が選ばれます。

#### ③ params 展開後の一致

`params` を使った一致は最後に検討されます。具体例は次の「params は優先順位が最も低い」で確認します。

## 3. params は優先順位が最も低い

`params` は可変個の引数を受け取れる書き方ですが、オーバーロード解決では最も優先順位が低くなります（詳細は [params キーワード](/unity-csharp-learning/csharp/params-keyword/) を参照）。

`M(int x)` と `M(params int[] values)` の両方があるとき、`M(5)` は `M(int x)` を選びます。完全一致が優先されるからです。

```csharp
class A
{
    public void M(int x)
    {
        Console.WriteLine($"A.M(int): x={x}");
    }

    public void M(params int[] values)
    {
        Console.WriteLine($"A.M(params): count={values.Length}");
    }
}

var a = new A();
a.M(5);         // 完全一致 → M(int x) が選ばれる
a.M(1, 2, 3);   // M(int x) に一致しない → params 展開 → M(params int[]) が選ばれる
```

```
A.M(int): x=5
A.M(params): count=3
```

`params` 版が存在しても、より高い順位の候補があればそちらが選ばれます。

## 4. あいまい（Ambiguous）エラー

同じランクの候補が複数残ると、コンパイラは 1 つに決められません。その場合はコンパイルエラーになります。

```csharp
class A
{
    public void M(int x, double y)
    {
        Console.WriteLine($"A.M(int,double): x={x}, y={y}");
    }

    public void M(double x, int y)
    {
        Console.WriteLine($"A.M(double,int): x={x}, y={y}");
    }
}

var a = new A();
// a.M(1, 2); // ❌ コンパイルエラー: M(int,double) と M(double,int) のどちらも同ランクの候補になる
```

`a.M(1, 2)` では `M(int, double)` も `M(double, int)` も候補です。どちらも片方の引数で `int → double` の変換が必要で、優劣がつきません。そのため、あいまいな呼び出しとしてコンパイルエラーになります。

### 5. 名前付き引数で解決する

名前付き引数を使うと、パラメータ名を手がかりにして候補を絞り込める場合があります。この節では、[省略可能パラメータと名前付き引数](/unity-csharp-learning/csharp/optional-named-params/) で学んだ内容がオーバーロード解決にどう関わるかを確認します。

**書式：名前付き引数**
```
メソッド名(引数名: 値)
メソッド名(引数名1: 値1, 引数名2: 値2)
```

| 要素 | 説明 |
|---|---|
| `引数名` | メソッド定義側のパラメータ名 |
| `:` | 名前と値を結びつける記号 |
| `値` | 実際に渡す値 |

**書式：省略可能パラメータ**
```
型 パラメータ名 = 既定値
```

| 要素 | 説明 |
|---|---|
| `型` | 受け取る値の型 |
| `パラメータ名` | メソッド定義側の名前 |
| `=` | 既定値を設定する記号 |
| `既定値` | 引数を省略したときに使われる値 |

```csharp
class A
{
    public void M(int x, int y)
    {
        Console.WriteLine($"A.M(x,y): x={x}, y={y}");
    }

    public void M(int a, int b, int c = 0)
    {
        Console.WriteLine($"A.M(a,b,c): a={a}, b={b}, c={c}");
    }
}

var d = new A();
d.M(1, 2);         // 両方が候補 → 省略パラメータが不要な M(int x, int y) が優先される
d.M(a: 1, b: 2);   // パラメータ名 a, b は M(int a, int b, int c) に一致 → M(a,b,c)
d.M(x: 1, y: 2);   // パラメータ名 x, y は M(int x, int y) に一致 → M(x,y)
```

```
A.M(x,y): x=1, y=2
A.M(a,b,c): a=1, b=2, c=0
A.M(x,y): x=1, y=2
```

`d.M(1, 2)` は `M(int a, int b, int c = 0)` も 2 引数で呼び出せますが、`M(int x, int y)` は 2 つの引数すべてがそのまま対応し、追加の省略を必要としません。コンパイラはこのような候補を、より具体的に一致している候補として扱うため、`M(int x, int y)` が選ばれます。`d.M(a: 1, b: 2)` は `a` と `b` というパラメータ名に一致する候補だけが残り、`d.M(x: 1, y: 2)` では `x` と `y` に一致する候補だけが残ります。

---

## よくあるミス

### ミス①：暗黙変換があることを忘れて意図しないオーバーロードが呼ばれる

```csharp
class A
{
    public void M(long x)
    {
        Console.WriteLine($"A.M(long): x={x}");
    }

    public void M(double x)
    {
        Console.WriteLine($"A.M(double): x={x}");
    }
}

var a = new A();
// ❌ NG: int から long と double のどちらにも暗黙変換できるので、自動で 1 つに決まると思い込む
// a.M(1); // コンパイルエラー: M(long) と M(double) のどちらも同ランクの候補

// ✅ OK: 明示的にキャストして呼ぶ
a.M((long)1);
a.M((double)1);
```

```
A.M(long): x=1
A.M(double): x=1
```

`int` から `long` と `double` はどちらも暗黙変換できます。どちらか一方が自動で必ず選ばれると決めつけず、必要なら明示的にキャストして呼び分けます。`(long)1` のように型名をかっこで囲んで値の前に書くと、変換先の型を明示できます。型変換の基本は [プリミティブ型と型変換](/unity-csharp-learning/csharp/primitive-types/) で確認してください。

### ミス②：params があると「どちらでも呼べる」と思って定義するが実際の挙動を誤解する

```csharp
class A
{
    public void M(int x)
    {
        Console.WriteLine($"A.M(int): x={x}");
    }

    public void M(params int[] values)
    {
        Console.WriteLine($"A.M(params): count={values.Length}");
    }
}

var a = new A();
// ❌ NG: params 版が呼ばれると思い込んで M(int) を定義しない
// a.M(5); を params 版で受けたつもりが、実際は M(int) が完全一致で選ばれる

// ✅ OK: 完全一致が優先されることを理解した上で、意図通りに呼び分ける
a.M(5);       // 完全一致 → M(int) が呼ばれる
a.M(5, 6);    // M(int) に一致しない → params 展開 → M(params) が呼ばれる
```

```
A.M(int): x=5
A.M(params): count=2
```

`params` は優先順位が最も低く、通常の引数リストで受け取れるオーバーロードがあるなら、そちらが先に選ばれます。

---

## まとめ

- コンパイラはまず「名前と引数の数・型が一致するメソッド」を候補に絞り込む
- 優先順位は「完全一致 > 暗黙的型変換後の一致 > params 展開後の一致」
- params は最も優先順位が低く、他に一致するオーバーロードがあればそちらが選ばれる
- 同じ優先順位の候補が複数残ると、コンパイルエラー（あいまいな呼び出し）になる
- 名前付き引数を使うと、パラメータ名でオーバーロードを一意に絞り込める場合がある

---

## 理解度チェック

**問 1**
次のクラスに `a.M(2);` と呼び出したとき、どちらのオーバーロードが選ばれますか？

```csharp
class A
{
    public void M(int x) { Console.WriteLine("int"); }
    public void M(long x) { Console.WriteLine("long"); }
}
```

**問 2**
次のコードはコンパイルエラーになりますか？なる場合、その理由を説明してください。

```csharp
class A
{
    public void M(int x, double y) { Console.WriteLine("int,double"); }
    public void M(double x, int y) { Console.WriteLine("double,int"); }
}

var a = new A();
a.M(1, 2);
```

**問 3**
次のクラスがあるとき、`a.M(p: 10);` を呼び出すとどうなりますか？

```csharp
class A
{
    public void M(int x) { Console.WriteLine($"M(int): {x}"); }
    public void M(int p, int q = 0) { Console.WriteLine($"M(p,q): p={p}, q={q}"); }
}
```

<details markdown="1">
<summary>解答を見る</summary>

1. `M(int x)` が選ばれます。`2` は `int` リテラルなので、`M(int x)` に完全一致するからです。
2. コンパイルエラーになります。`M(int, double)` と `M(double, int)` のどちらも候補になり、どちらも片方の引数で `int → double` の暗黙変換が必要で優劣がつかないためです。
3. `M(int p, int q = 0)` が呼ばれます。名前付き引数 `p:` がそのオーバーロードのパラメータ名に一致し、`q` は省略可能なので `0` が使われます。

</details>

---

## 次のステップ

- [演算子のオーバーロード](/unity-csharp-learning/csharp/operator-overloading/)
