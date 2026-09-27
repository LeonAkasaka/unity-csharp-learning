---
layout: page
title: 省略可能パラメータと名前付き引数
permalink: /csharp/optional-named-params/
---

# 省略可能パラメータと名前付き引数

パラメータに既定値を決めておくと、呼び出すときにその引数を省略できます（**省略可能パラメータ**）。また、`引数名: 値` の形で、どのパラメータに渡す値かを名前で指定することもできます（**名前付き引数**）。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- 省略可能パラメータを定義し、引数を省略して呼び出せる
- 省略可能パラメータを、パラメータの並びの最後に置く理由を説明できる
- 名前付き引数で、パラメータの名前を指定して値を渡せる

## 前提知識

- [ref / out / in パラメータ](/unity-csharp-learning/csharp/ref-out-in/) を読んでいること

---

## 1. 省略可能パラメータ

パラメータの後に `= 既定値` を書くと、呼び出すときにその引数を省略できます。省略したときは、既定値が使われます。

**書式：[省略可能パラメータ](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/named-and-optional-arguments#optional-arguments)**
```
戻り値の型 メソッド名(型 パラメータ名, 型 パラメータ名 = 既定値)
```

| 要素 | 説明 |
|---|---|
| `型 パラメータ名` | 必ず引数を渡すパラメータ |
| `型 パラメータ名 = 既定値` | 省略可能パラメータ。引数を省略すると既定値が使われる |

```csharp
A a = new A();
a.M(1);
a.M(1, 2);

class A
{
    public void M(int x, int y = 0)
    {
        Console.WriteLine($"A.M: x={x}, y={y}");
    }
}
```

```
A.M: x=1, y=0
A.M: x=1, y=2
```

`a.M(1)` では `y` を省略したので、既定値の `0` が使われます。

### 省略可能パラメータの決まり

- 省略可能パラメータは、パラメータの並びの **最後** に置く。後ろに省略できないパラメータを置くことはできない
- 既定値には、リテラルのように、コンパイルするときに決まる値（定数）を書く。変数は書けない

引数は前から順にパラメータに割り当てられるので、省略できるのは後ろの引数だけです。省略可能パラメータの後ろに省略できないパラメータがあると、その値を渡すために、前の省略可能パラメータの引数も書かなければならなくなります。

---

## 2. 名前付き引数

呼び出すときに `パラメータ名: 値` と書くと、どのパラメータに渡す値かを、名前で指定できます。

**書式：[名前付き引数](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/named-and-optional-arguments#named-arguments)**
```
メソッド名(パラメータ名: 値, パラメータ名: 値)
```

名前で指定すると、パラメータの順番と違う順番で書けます。

```csharp
A a = new A();
a.M(y: 5, x: 1);
a.M(x: 1);

class A
{
    public void M(int x, int y = 0)
    {
        Console.WriteLine($"A.M: x={x}, y={y}");
    }
}
```

```
A.M: x=1, y=5
A.M: x=1, y=0
```

### 途中の引数を省略する

省略可能パラメータが複数あるとき、名前付き引数を使うと、途中のパラメータを省略して、後ろのパラメータにだけ値を渡せます。

```csharp
A a = new A();
a.Draw("円");
a.Draw("円", size: 20);
a.Draw("四角", color: "赤");

class A
{
    public void Draw(string shape, string color = "黒", int size = 10)
    {
        Console.WriteLine($"{shape}: 色={color}, 大きさ={size}");
    }
}
```

```
円: 色=黒, 大きさ=10
円: 色=黒, 大きさ=20
四角: 色=赤, 大きさ=10
```

`a.Draw("円", size: 20)` では、`color` を省略して、`size` にだけ値を渡しています。名前を付けなければ、`size` に渡すために `color` の値も書く必要があります。

### 名前付き引数で読みやすくする

`true` や数値のような引数は、呼び出す側のコードだけでは意味がわかりにくいことがあります。名前を付けると、何を渡しているかがわかりやすくなります。

```csharp
A a = new A();
a.Save("data.txt", true);
a.Save("data.txt", overwrite: true);

class A
{
    public void Save(string path, bool overwrite)
    {
        Console.WriteLine($"{path} に保存（上書き={overwrite}）");
    }
}
```

```
data.txt に保存（上書き=True）
data.txt に保存（上書き=True）
```

2 つの呼び出しは同じ意味ですが、`overwrite: true` のほうが、`true` が何を表すかを読み取れます。

---

## よくあるミス

### 省略可能パラメータの後ろに、省略できないパラメータを置く

```csharp
// ❌ NG: 省略可能パラメータ x の後ろに、省略できない y がある
// void M(int x = 0, int y)
// {
// }  // CS1737
```

省略可能パラメータは、パラメータの並びの最後に置きます。`void M(int y, int x = 0)` のように並べ替えます。

### 既定値に変数を書く

```csharp
// ❌ NG: 既定値は定数でなければならない
// int defaultValue = 3;
// void M(int x = defaultValue)
// {
// }  // CS1736
```

---

## まとめ

- `型 パラメータ名 = 既定値` と書くと、省略可能パラメータになる。引数を省略すると既定値が使われる
- 省略可能パラメータは、パラメータの並びの最後に置く。既定値は定数にする
- `パラメータ名: 値` の名前付き引数で、どのパラメータに渡す値かを指定できる
- 名前付き引数を使うと、順番を入れ替えたり、途中の省略可能パラメータを省略したりできる

---

## 理解度チェック

1. 省略可能パラメータを、パラメータの並びの最後に置かなければならないのはなぜですか？
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   A a = new A();
   a.M(y: 3, x: 1);
   a.M(5);
   a.M(7, z: 9);

   class A
   {
       public void M(int x, int y = 10, int z = 20)
       {
           Console.WriteLine($"A.M: x={x}, y={y}, z={z}");
       }
   }
   ```

<details markdown="1">
<summary>解答を見る</summary>

1. 引数は前から順にパラメータに割り当てられるので、省略できるのは後ろの引数だけだからです。省略可能パラメータの後ろに省略できないパラメータがあると、その省略可能パラメータを省略できなくなります。
2. 次のように出力されます。`a.M(7, z: 9)` では、`y` を省略して `z` にだけ値を渡しています。

   ```
   A.M: x=1, y=3, z=20
   A.M: x=5, y=10, z=20
   A.M: x=7, y=10, z=9
   ```

</details>

---

## 次のステップ

[params キーワード](/unity-csharp-learning/csharp/params-keyword/) では、いくつでも引数を渡せるメソッドを作る方法を学びます。
