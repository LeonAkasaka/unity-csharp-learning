---
layout: page
title: protected 修飾子
permalink: /csharp/protected-modifier/
---

# protected 修飾子

[アクセス修飾子](/unity-csharp-learning/csharp/access-modifiers/) では、どこからでも使える `public` と、同じクラスの中からだけ使える `private` を学びました。継承を使うと、この 2 つだけでは足りない場面が出てきます。「派生クラスからは使いたいが、クラスの外には公開したくない」メンバーです。このページでは、そのようなメンバーを作る **protected** と、同じアセンブリの中に公開する **internal** を学びます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `private` と `public` だけでは困る場面を説明できる
- `protected` で、派生クラスからだけ使えるメンバーを作れる
- `public`・`protected`・`private` のそれぞれで、メンバーを使える場所を比べられる
- `internal` の意味を説明できる

## 前提知識

- [型変換と型チェック](/unity-csharp-learning/csharp/type-casting/) を読んでいること
- [アクセス修飾子](/unity-csharp-learning/csharp/access-modifiers/) を読んでいること

---

## 1. private では派生クラスから使えない

[アクセス修飾子](/unity-csharp-learning/csharp/access-modifiers/) では、HP を負の値にされないように、HP を変える処理をクラスの中に閉じ込めました。`Character` でも同じように、`Hp` の `set` を `private` にし、HP を減らすのは `TakeDamage` メソッドだけにします。

プレイヤーには、休憩して HP を回復する `Rest` メソッドを追加したいとします。ところが、`Player` の中から `Hp` を変えようとすると、コンパイルエラーになります。

```csharp
// ❌ NG: Hp の set は private なので、派生クラスの Player からも使えない
// class Character
// {
//     public string Name { get; }
//     public int Hp { get; private set; }
//     ...
// }
//
// class Player : Character
// {
//     public void Rest()
//     {
//         Hp += 10;  // CS0272
//     }
// }
```

[継承](/unity-csharp-learning/csharp/inheritance/) で学んだように、基底クラスの `private` のメンバーは、派生クラスからも使えません。

それなら `set` を `public` にすればよいかというと、そうすると今度は、クラスの外から `player.Hp = -500;` のように、どんな値でも入れられてしまいます。HP を守るために `private` にしたのが、無駄になります。

欲しいのは、「`Character` とその派生クラスの中からは使えるが、クラスの外からは使えない」という、`private` と `public` の中間の範囲です。

---

## 2. protected のメンバー

メンバーに [protected](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/protected) を付けると、そのメンバーは、同じクラスの中と、派生クラスの中から使えるようになります。クラスの外からは使えません。

**書式：[protected のメンバー](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/protected)**
```
protected 型 フィールド名;
protected 戻り値の型 メソッド名(パラメータ)
public 型 プロパティ名 { get; protected set; }
```

`Hp` の `set` を `protected` にし、派生クラスで使うためのログ出力のメソッド `Log` も `protected` で用意します。

```csharp
Player player = new Player("Alice", 100);
player.TakeDamage(30);
player.Rest();
Console.WriteLine($"{player.Name}: HP={player.Hp}");

class Character
{
    public string Name { get; }
    public int Hp { get; protected set; }

    public Character(string name, int hp)
    {
        Name = name;
        Hp = hp;
    }

    public void TakeDamage(int damage)
    {
        Hp -= damage;
        if (Hp < 0)
        {
            Hp = 0;
        }
        Log($"{damage} ダメージを受けた");
    }

    protected void Log(string message)
    {
        Console.WriteLine($"[{Name}] {message}");
    }
}

class Player : Character
{
    public Player(string name, int hp) : base(name, hp)
    {
    }

    public void Rest()
    {
        Hp += 10;
        Log("休憩して HP が 10 回復した");
    }
}
```

```
[Alice] 30 ダメージを受けた
[Alice] 休憩して HP が 10 回復した
Alice: HP=80
```

`Hp` の `set` と `Log` は `protected` なので、派生クラス `Player` の `Rest` メソッドから使えます。`Hp` の `get` は `public` のままなので、クラスの外からも HP を読むことはできます。

クラスの外から HP を書き換えたり、`Log` を呼び出したりすると、コンパイルエラーになります。

```csharp
// ❌ NG: protected のメンバーは、クラスの外からは使えない
// player.Hp = -500;        // CS0272
// player.Log("不正な記録");  // CS0122
```

---

## 3. アクセス修飾子の比較

`public`・`protected`・`private` の違いを、メンバーを使う場所ごとに比べると、次のようになります。

| 修飾子 | 同じクラスの中 | 派生クラスの中 | クラスの外 |
|---|---|---|---|
| `public` | 使える | 使える | 使える |
| `protected` | 使える | 使える | 使えない |
| `private` | 使える | 使えない | 使えない |

`private` と `protected` の違いは、**派生クラスの中から使えるかどうか** だけです。どちらも、クラスの外からは使えません。

`protected` にしたメンバーは、派生クラスを作る人が使う前提のものです。基底クラスを変更するときは、派生クラスで使われていることを考える必要があります。派生クラスから使う必要のないメンバーは、`private` のままにしておきます。

---

## 4. 継承を重ねた場合

`protected` のメンバーは、派生クラスのさらに派生クラスからも使えます。2 節の `Character` に、`Enemy` と、その派生クラスのボス `Boss` を追加します。`Character` クラスは、2 節と同じものを使います。

```csharp
Boss boss = new Boss("Dragon", 500);
boss.TakeDamage(30);
boss.Regenerate();

class Enemy : Character
{
    public Enemy(string name, int hp) : base(name, hp)
    {
    }
}

class Boss : Enemy
{
    public Boss(string name, int hp) : base(name, hp)
    {
    }

    public void Regenerate()
    {
        Hp += 50;
        Log($"再生した。HP={Hp}");
    }
}
```

```
[Dragon] 30 ダメージを受けた
[Dragon] 再生した。HP=520
```

`Boss` は `Character` を直接継承していませんが、`Enemy` を通して `Character` を継承しているので、`Character` の `protected` のメンバーを使えます。

---

## 5. internal

アクセス修飾子には、クラスの継承とは関係のない [internal](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/internal) もあります。`internal` を付けたメンバーやクラスは、同じアセンブリの中なら、どこからでも使えます。**アセンブリ** は、1 つのプロジェクトをビルドしてできるプログラム（`.dll` や `.exe`）のことです。

```csharp
Tool t = new Tool();
t.Use();

internal class Tool
{
    internal void Use()
    {
        Console.WriteLine("Tool.Use");
    }
}
```

```
Tool.Use
```

1 つのプロジェクトだけで開発しているときは、`internal` の効果は `public` とほとんど変わりません。複数のプロジェクトに分けて開発するとき、プロジェクトの外には公開したくないクラスやメンバーに使います。なお、アクセス修飾子を書かずに定義したクラスは、`internal` になります。

---

## よくあるミス

### 派生クラスの中で、基底クラスの型の変数を通して protected のメンバーを使う

```csharp
// ❌ NG: Character の型の変数を通して、protected のメンバーは使えない
// class Player : Character
// {
//     public void HealOther(Character other)
//     {
//         other.Hp += 10;  // CS1540
//     }
// }
```

派生クラス `Player` の中でも、`protected` のメンバーを使えるのは、自分自身（`Hp` や `this.Hp`）か、`Player` の型の変数を通したときだけです。`Character` の型の変数 `other` の実体は、`Enemy` のような、`Player` とは別の派生クラスかもしれません。`protected` は「自分の派生クラスとしての部分」を使うための許可なので、関係のない別のクラスのインスタンスには使えないのです。

---

## ワンポイントアドバイス

### protected と internal の組み合わせ

`protected` と `internal` を組み合わせた、次の 2 つのアクセス修飾子もあります。

| 修飾子 | 使える場所 |
|---|---|
| [protected internal](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/protected-internal) | 同じアセンブリの中か、派生クラスの中（どちらか一方を満たせばよい） |
| [private protected](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/private-protected) | 同じアセンブリの中にある派生クラスの中（両方を満たす必要がある） |

複数のプロジェクトに分けて、ほかのプロジェクトからも継承されるクラスを作るときに使います。

---

## まとめ

- `private` のメンバーは派生クラスから使えず、`public` にするとクラスの外からも使えてしまう
- `protected` のメンバーは、同じクラスの中と、派生クラスの中から使える。クラスの外からは使えない
- `{ get; protected set; }` のように、プロパティの `set` だけを `protected` にすることもできる
- `protected` のメンバーは、継承を何段重ねても、派生クラスから使える
- `internal` のメンバーやクラスは、同じアセンブリの中から使える

---

## 理解度チェック

1. `private` と `protected` の違いを説明してください。
2. 次のコードはコンパイルできますか？

   ```csharp
   A a = new A();
   a.M();

   class A
   {
       protected void M() { }
   }
   ```

3. `B` が `A` を継承し、`C` が `B` を継承しています。`A` に `protected void M()` があるとき、`C` のメソッドから `M()` を呼び出せますか？
4. 「クラスの外からは読めるが、書き換えられるのは、そのクラスと派生クラスの中からだけ」という `int` 型のプロパティ `Score` は、どう書きますか？

<details markdown="1">
<summary>解答を見る</summary>

1. どちらもクラスの外からは使えませんが、`private` は同じクラスの中からだけ、`protected` は同じクラスの中と派生クラスの中から使えます。
2. できません（CS0122）。`protected` のメンバーは、クラスの外からは使えません。
3. 呼び出せます。`C` は `B` を通して `A` を継承しているので、`A` の `protected` のメンバーを使えます。
4. `public int Score { get; protected set; }` と書きます。

</details>

---

## 次のステップ

[オーバーライドとポリモーフィズム](/unity-csharp-learning/csharp/polymorphism/) では、基底クラスのメソッドの動作を、派生クラスごとに書き換える仕組みを学びます。
