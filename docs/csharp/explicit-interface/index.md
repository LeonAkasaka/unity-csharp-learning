---
layout: page
title: インターフェイスの明示的実装
permalink: /csharp/explicit-interface/
---

# インターフェイスの明示的実装

1 つのクラスで複数のインターフェイスを実装すると、別々のインターフェイスが、たまたま同じ名前のメンバーを持っていることがあります。ふつうの書き方では、1 つのメンバーが両方のインターフェイスの実装になり、区別できません。**明示的実装**（explicit implementation）を使うと、どのインターフェイスのメンバーを実装するのかを指定して、それぞれに別の実装を用意できます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- 同じ名前のメンバーを持つインターフェイスを、ふつうの書き方で実装すると困る場面を説明できる
- インターフェイスのメンバーを、明示的実装で定義できる
- 明示的実装のメンバーは、インターフェイスの型を通してだけ呼び出せることを説明できる
- 暗黙的実装と明示的実装の違いを説明できる

## 前提知識

- [インターフェイス](/unity-csharp-learning/csharp/interfaces/) を読んでいること

---

## 1. 同じ名前のメンバーを持つインターフェイス

ゲームに、宝箱に化けたモンスター「ミミック」を登場させます。ミミックは、宝箱として調べることも、モンスターとして戦うこともできるので、2 つのインターフェイスを実装します。

- `ITreasure`：宝箱として扱うためのインターフェイス。`Describe` は、プレイヤーが調べたときの説明を返す
- `IMonster`：モンスターとして扱うためのインターフェイス。`Describe` は、戦闘中に表示する説明を返す

2 つのインターフェイスは別々に作られたもので、どちらも `string Describe()` を宣言しています。[インターフェイス](/unity-csharp-learning/csharp/interfaces/) で学んだ書き方で、`public string Describe()` を 1 つ定義すると、その `Describe` が、`ITreasure` の `Describe` と `IMonster` の `Describe` の両方の実装になります。このような、ふつうの実装の書き方を **暗黙的実装**（implicit implementation）といいます。

```csharp
Mimic mimic = new Mimic();

ITreasure treasure = mimic;
Console.WriteLine($"調べる: {treasure.Describe()}");

IMonster monster = mimic;
Console.WriteLine($"戦闘: {monster.Describe()}");

interface ITreasure
{
    string Describe();
}

interface IMonster
{
    string Describe();
}

class Mimic : ITreasure, IMonster
{
    public string Describe()
    {
        return "ミミック（宝箱に化けたモンスター）";
    }
}
```

```
調べる: ミミック（宝箱に化けたモンスター）
戦闘: ミミック（宝箱に化けたモンスター）
```

宝箱として調べたときにも、正体がわかる説明が表示されてしまいました。`ITreasure` の `Describe` と `IMonster` の `Describe` は、名前が同じでも意味が違います。宝箱として調べたときは「ふつうの宝箱」に、戦闘中は「ミミック」に見せたいのですが、暗黙的実装では 1 つのメソッドしか書けないので、区別できません。

---

## 2. 明示的実装の書き方

メンバーの名前の前に `インターフェイス名.` を付けて定義すると、そのインターフェイスのメンバーだけの実装になります。これを **明示的実装** といいます。

**書式：[明示的実装](https://learn.microsoft.com/dotnet/csharp/programming-guide/interfaces/explicit-interface-implementation)**
```
戻り値の型 インターフェイス名.メソッド名(パラメータ)
{
    // 処理
}
```

| 要素 | 説明 |
|---|---|
| アクセス修飾子 | 書かない |
| `インターフェイス名.メソッド名` | どのインターフェイスのメンバーを実装するのかを指定する |

1 節の `Mimic` を、明示的実装で書き直します。

```csharp
Mimic mimic = new Mimic();

ITreasure treasure = mimic;
Console.WriteLine($"調べる: {treasure.Describe()}");

IMonster monster = mimic;
Console.WriteLine($"戦闘: {monster.Describe()}");

interface ITreasure
{
    string Describe();
}

interface IMonster
{
    string Describe();
}

class Mimic : ITreasure, IMonster
{
    string ITreasure.Describe()
    {
        return "古びた宝箱だ。中に何か入っていそうだ";
    }

    string IMonster.Describe()
    {
        return "ミミックが正体を現した！";
    }
}
```

```
調べる: 古びた宝箱だ。中に何か入っていそうだ
戦闘: ミミックが正体を現した！
```

同じインスタンス `mimic` でも、`ITreasure` の型の変数から呼び出すと `ITreasure.Describe` が、`IMonster` の型の変数から呼び出すと `IMonster.Describe` が実行されます。どちらの `Describe` が呼ばれるかは、どのインターフェイスとして扱っているかで決まります。

---

## 3. 明示的実装のメンバーの呼び出し方

明示的実装のメンバーは、クラスの型の変数からは呼び出せません。`mimic.Describe()` と書いても、`ITreasure` と `IMonster` のどちらの `Describe` なのかが決まらないからです。明示的実装のメンバーは、クラスのメンバーとしては公開されず、インターフェイスのメンバーとしてだけ存在します。

呼び出すには、インターフェイスの型の変数に入れるか、インターフェイスの型にキャストします。2 節の `ITreasure`・`IMonster`・`Mimic` を使います。

```csharp
Mimic mimic = new Mimic();

Console.WriteLine(((ITreasure)mimic).Describe());
Console.WriteLine(((IMonster)mimic).Describe());

// ❌ NG: 明示的実装のメンバーは、クラスの型の変数から呼び出せない（CS1061）
// Console.WriteLine(mimic.Describe());
```

```
古びた宝箱だ。中に何か入っていそうだ
ミミックが正体を現した！
```

`(ITreasure)mimic` はアップキャストなので、失敗することはありません。外側の `( )` は、キャストした結果に対して `.Describe()` を呼び出すために必要です。

### インターフェイスとして使うときだけ必要なメンバーを隠す

名前が衝突していなくても、明示的実装が使われることがあります。クラスの型の変数からは呼び出せないという性質を利用して、「インターフェイスとして使うときにだけ必要で、ふだんクラスを使う人には見せなくてよい」メンバーを、クラスのメンバーの一覧から隠すためです。

たとえば、後で学ぶ [IEnumerable\<T\> と foreach の仕組み](/unity-csharp-learning/csharp/ienumerable/) では、古い仕組みとの互換性のためだけに必要なメンバーを、明示的実装で定義します。

---

## 4. 暗黙的実装と明示的実装の比較

| | 暗黙的実装 | 明示的実装 |
|---|---|---|
| 書き方 | `public string Describe() { }` | `string ITreasure.Describe() { }` |
| アクセス修飾子 | `public` を書く | 書かない |
| クラスの型の変数から呼び出す | できる | できない |
| インターフェイスの型の変数から呼び出す | できる | できる |
| 同じ名前のメンバーを、インターフェイスごとに別の実装にする | できない | できる |

ふだんは暗黙的実装を使い、名前が衝突して区別が必要なときや、クラスの型からメンバーを隠したいときに、明示的実装を使います。1 つのクラスの中で、あるインターフェイスは暗黙的実装、別のインターフェイスは明示的実装、と組み合わせることもできます（理解度チェックの 2 を参照）。

---

## よくあるミス

### 明示的実装にアクセス修飾子を付ける

```csharp
// ❌ NG: 明示的実装にはアクセス修飾子を付けられない
// class Mimic : ITreasure
// {
//     public string ITreasure.Describe()  // CS0106
//     {
//         return "古びた宝箱だ";
//     }
// }
```

明示的実装のメンバーは、インターフェイスの型を通してだけ呼び出せるので、アクセス修飾子は書きません。

---

## まとめ

- 同じ名前のメンバーを持つ複数のインターフェイスを暗黙的実装すると、1 つのメンバーが両方の実装になり、区別できない
- 明示的実装は、`戻り値の型 インターフェイス名.メソッド名() { }` の形で書く。アクセス修飾子は付けない
- 明示的実装のメンバーは、インターフェイスの型の変数（またはキャスト）を通してだけ呼び出せる
- 明示的実装は、名前が衝突したときの区別のほか、クラスの型からメンバーを隠すためにも使われる

---

## 理解度チェック

1. 明示的実装したメンバーを、クラスの型の変数から直接呼び出そうとすると、どうなりますか？
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   Robot r = new Robot();
   ((IWalker)r).Move();
   ((ISwimmer)r).Move();
   r.Move();

   interface IWalker
   {
       void Move();
   }

   interface ISwimmer
   {
       void Move();
   }

   class Robot : IWalker, ISwimmer
   {
       public void Move() { Console.WriteLine("移動する"); }
       void ISwimmer.Move() { Console.WriteLine("泳ぐ"); }
   }
   ```

3. `IFoo` と `IBar` が同じ名前のメソッドを持ち、それぞれ別の動作にしたいときは、暗黙的実装と明示的実装のどちらを使いますか？

<details markdown="1">
<summary>解答を見る</summary>

1. コンパイルエラー（CS1061）になります。明示的実装のメンバーは、インターフェイスの型を通してだけ呼び出せます。
2. 次のように出力されます。`ISwimmer` の `Move` は明示的実装されているので `泳ぐ` です。`IWalker` の `Move` とクラスの型から呼び出した `Move` には、暗黙的実装の `public void Move()` が使われます。

   ```
   移動する
   泳ぐ
   移動する
   ```

3. 明示的実装を使います。暗黙的実装では、1 つのメソッドが両方のインターフェイスの実装になるので、区別できません。

</details>

---

## 次のステップ

これで「C# 継承と抽象化」のセクションは終わりです。[ジェネリクスの基本](/unity-csharp-learning/csharp/generics/) からは「C# ジェネリクス」のセクションに進み、型をパラメータとして受け取る、汎用的なクラスの書き方を学びます。
