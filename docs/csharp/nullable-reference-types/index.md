---
layout: page
title: null 許容参照型
permalink: /csharp/nullable-reference-types/
---

# null 許容参照型

参照型の変数には、もともと `null` を入れられます。そのため、`null` のままメンバーを使ってしまう誤りは、実行するまでわかりませんでした。**null 許容参照型**（nullable reference type）は、`string?` のように型名の後に `?` を付けて「`null` が入ることがある」と示し、`null` を扱う誤りをコンパイラーに警告させる機能です。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `string` と `string?` の違いを説明できる
- null 許容参照型の主な警告の意味を説明できる
- `string?` が `int?` と違い、別の型を作らないことを説明できる
- `null` のチェック、`??`、`?.` を使って、警告の出ないコードを書ける
- `!` 演算子が警告を消すだけで、`null` をチェックしないことを説明できる
- `null` を入れないフィールドを、初期値かコンストラクターで初期化できる

## 前提知識

- [値型と参照型](/unity-csharp-learning/csharp/value-reference-types/) を読んでいること
- [null 許容値型](/unity-csharp-learning/csharp/nullable-value-types/) を読んでいること

---

## 1. null による実行時のエラー

名前を探すメソッド `FindName` を考えます。見つからないときは `null` を返します。

```csharp
string? found = FindName(3);
Console.WriteLine(found.Length);

string? FindName(int id)
{
    if (id == 1)
    {
        return "勇者";
    }
    return null;
}
```

ビルドすると、次の警告が出ます。警告なので、ビルドは成功します。

```
warning CS8602: null 参照の可能性があるものの逆参照です。
```

実行すると、`found` は `null` なので、`found.Length` で [値型と参照型](/unity-csharp-learning/csharp/value-reference-types/) で見た `NullReferenceException` が発生します。

```
Unhandled exception. System.NullReferenceException: Object reference not set to an instance of an object.
```

CS8602 は、「`null` かもしれない変数のメンバーを使っている」という警告です。この警告を出すのが、null 許容参照型の機能です。コンパイラーは、`FindName` の戻り値の型 `string?` を見て、`found` に `null` が入ることがあると判断しています。

---

## 2. ? の有無で null が入るかを示す

null 許容参照型が有効なプロジェクトでは、参照型の変数の型に、`null` が入ることがあるかどうかを書き分けます。

**書式：[null 許容参照型](https://learn.microsoft.com/dotnet/csharp/language-reference/builtin-types/nullable-reference-types)**
```
参照型?
```

| 書き方 | 意味 |
|---|---|
| `string` | **null 非許容参照型**。`null` を入れない前提の変数 |
| `string?` | **null 許容参照型**。`null` が入ることがある変数 |

この区別をもとに、コンパイラーは `null` の扱いを調べ、問題がありそうな場所で警告を出します。主な警告は、次のとおりです。

| 警告 | 出る場面 |
|---|---|
| CS8600 | `null` や、`null` かもしれない値を、`?` のない変数に代入した |
| CS8602 | `null` かもしれない変数のメンバーを使った |
| CS8603 | 戻り値の型に `?` がないメソッドで、`null` を返した |
| CS8604 | `null` かもしれない値を、`?` のないパラメータに渡した |
| CS8618 | `?` のないフィールドを、初期化しないままにした |

たとえば、戻り値の型を `string` にしたまま `null` を返すと、CS8603 の警告が出ます。

```csharp
// ⚠️ NG: 戻り値の型に ? がないのに null を返している（警告 CS8603）
// string FindName(int id)
// {
//     if (id == 1)
//     {
//         return "勇者";
//     }
//     return null;  // CS8603
// }
```

`null` を返すことがあるなら、戻り値の型を `string?` にします。そうすれば、呼び出し元の変数も `string?` になり、1 節のように、使う側で `null` の扱いを確かめるよう警告されます。

どの警告も、エラーではなく警告です。警告が出ても、ビルドと実行はできます。ただし、警告を残したままにすると、1 節のように実行時のエラーにつながります。

---

## 3. string? の正体

[null 許容値型](/unity-csharp-learning/csharp/nullable-value-types/) の `int?` は、`Nullable<int>` という別の型でした。値型の変数には、もともと `null` を入れられないからです。

参照型の変数には、もともと `null` を入れられます。そのため、`string?` は別の型を作りません。`string?` も `string` も、実行するときは同じ `string` 型です。

```csharp
string? name = "勇者";
Console.WriteLine(name.GetType());
Console.WriteLine(typeof(int?));
```

```
System.String
System.Nullable`1[System.Int32]
```

同じ型であることは、メソッドのオーバーロードからもわかります。`int` と `int?` は別の型なので、パラメータの型だけが違うメソッドを定義できます。`string` と `string?` は同じ型なので、定義できません。

```csharp
// ❌ NG: string と string? は同じ型なので、同じシグネチャになる
// class Printer
// {
//     void Print(int value) { }
//     void Print(int? value) { }      // OK: int と Nullable<int> は別の型
//
//     void Print(string text) { }
//     void Print(string? text) { }    // CS0111
// }
```

| | null 許容値型（`int?`） | null 許容参照型（`string?`） |
|---|---|---|
| 導入された C# のバージョン | C# 2.0 | C# 8.0 |
| 実行時の型 | `Nullable<int>`（`int` とは別の型） | `string`（`string` と同じ型） |
| `?` の役割 | `null` を入れられるようにする | `null` が入ることがあると、コンパイラーに伝える |
| `?` のない型の変数に `null` を入れると | コンパイルエラー（CS0037） | 警告（CS8600） |

参照型の `?` は、コンパイラーが警告を出すための目印です。`?` を付けても付けなくても、実行するときの動作は変わりません。

---

## 4. null をチェックすると警告が消える

`null` かもしれない変数でも、`null` でないことを確かめた後なら、メンバーを使っても警告は出ません。

```csharp
string? found = FindName(3);

if (found != null)
{
    Console.WriteLine(found.Length);
}
else
{
    Console.WriteLine("見つからない");
}

string name = found ?? "名無し";
Console.WriteLine(name);

string? FindName(int id)
{
    if (id == 1)
    {
        return "勇者";
    }
    return null;
}
```

```
見つからない
名無し
```

コンパイラーは、`if (found != null)` の中では `found` が `null` でないことを、コードの流れから判断します。この仕組みを **フロー解析**（flow analysis）といいます。

[null 許容値型](/unity-csharp-learning/csharp/nullable-value-types/) で学んだ `??` 演算子も使えます。`found ?? "名無し"` は、`found` が `null` なら `"名無し"` になるので、結果は `null` になりません。`?` のない `string` の変数に、警告なしで代入できます。

---

## 5. ?. 演算子

`?.` 演算子（null 条件演算子）は、左辺が `null` でなければメンバーを使い、`null` ならメンバーを使わずに `null` を返します。

**書式：[?. 演算子](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/member-access-operators#null-conditional-operators--and-)**
```
式?.メンバー
```

| 要素 | 説明 |
|---|---|
| `式` | `null` かもしれない値 |
| `?.` | `式` が `null` なら、メンバーを使わずに、式全体の結果を `null` にする |
| `メンバー` | `式` が `null` でないときに使うメンバー |

```csharp
string? found = FindName(3);

int? length = found?.Length;
Console.WriteLine(length == null);

Console.WriteLine(found?.Length ?? 0);
Console.WriteLine(FindName(1)?.Length ?? 0);

string? FindName(int id)
{
    if (id == 1)
    {
        return "勇者";
    }
    return null;
}
```

```
True
0
2
```

`found?.Length` の結果は、`found` が `null` なら `null`、そうでなければ文字数です。`Length` は `int` ですが、結果が `null` になることがあるので、式の型は `int?` になります。`?? 0` と組み合わせると、`null` のときの値を決めて `int` として取り出せます。

---

## 6. ! 演算子

`!` 演算子（null 免除演算子）は、式の後ろに付けて、「この値は `null` ではない」とコンパイラーに伝えます。`null` かもしれないという警告が出なくなります。

**書式：[! 演算子](https://learn.microsoft.com/dotnet/csharp/language-reference/operators/null-forgiving)**
```
式!
```

`!` は警告を消すだけで、`null` かどうかは確かめません。値が `null` なら、警告が出ないまま `NullReferenceException` が発生します。

```csharp
// ❌ NG: ! で警告は消えるが、null のまま使っている
// string? found = null;
// Console.WriteLine(found!.Length);  // NullReferenceException
```

`!` を使うのは、コンパイラーには判断できないが、`null` にならないことが確かな場合だけにします。確かでないときは、4 節のように `null` をチェックするか、`??` や `?.` を使います。

---

## 7. フィールドを初期化する

`?` のない参照型のフィールドを、初期化しないままにすると、CS8618 の警告が出ます。インスタンスを作ったときに、フィールドに `null` が入ってしまうからです。

```csharp
// ⚠️ NG: Name がコンストラクターの終了時に null のまま（警告 CS8618）
// class Player
// {
//     public string Name;  // CS8618
// }
```

警告をなくすには、次のどれかにします。

- 初期値を書く（`public string Job = "戦士";`）
- コンストラクターで値を代入する
- `null` が入ることがあるなら、`?` を付ける

```csharp
Player p = new Player("勇者");
Console.WriteLine(p.Name);
Console.WriteLine(p.Job);
Console.WriteLine(p.Title ?? "称号なし");

p.Title = "竜殺し";
Console.WriteLine(p.Title ?? "称号なし");

class Player
{
    public string Name;
    public string Job = "戦士";
    public string? Title;

    public Player(string name)
    {
        Name = name;
    }
}
```

```
勇者
戦士
称号なし
竜殺し
```

[クラスとフィールド](/unity-csharp-learning/csharp/classes/) で、`string` のフィールドに `""` などの初期値を書いていたのは、この警告を出さないためです。

---

## 8. null 許容参照型の設定

null 許容参照型の警告は、プロジェクトの設定で有効になります。[.NET SDK と dotnet CLI](/unity-csharp-learning/csharp/dotnet-sdk/) で見た `.csproj` ファイルの `<Nullable>enable</Nullable>` が、この設定です。`dotnet new console` で作ったプロジェクトでは、最初から有効になっています。

ファイルの一部だけ設定を変えたいときは、`#nullable` ディレクティブを使います。

**書式：[#nullable ディレクティブ](https://learn.microsoft.com/dotnet/csharp/language-reference/preprocessor-directives#nullable-context)**
```
#nullable disable
#nullable enable
```

| 要素 | 説明 |
|---|---|
| `#nullable disable` | この行から後ろで、null 許容参照型を無効にする |
| `#nullable enable` | この行から後ろで、null 許容参照型を有効にする |

```csharp
#nullable disable
string name = null;
Console.WriteLine(name == null);
```

```
True
```

`#nullable disable` から後ろでは、null 許容参照型が無効になり、`?` のない変数に `null` を入れても警告は出ません。`#nullable enable` と書くと、また有効になります。null 許容参照型より前に書かれた古いコードを少しずつ対応させるときなどに使います。

---

## よくあるミス

### ! で警告を消して済ませる

```csharp
// ❌ NG: 警告は消えるが、null のときに実行時のエラーになる
// string? found = FindName(3);
// Console.WriteLine(found!.Length);  // NullReferenceException
```

警告は、`null` の扱いを確かめる必要がある場所を教えています。`!` で消すのではなく、`null` のときにどうするかを決めて、`if` や `??`、`?.` で書きます。

### null かもしれない値を、? のないパラメータに渡す

```csharp
// ⚠️ NG: null かもしれない値を、? のないパラメータに渡している（警告 CS8604）
// string? name = FindName(3);
// Print(name);  // CS8604
//
// void Print(string s)
// {
//     Console.WriteLine(s);
// }
```

`Print` のパラメータは `string` なので、`null` を受け取らない前提です。渡す前に `null` をチェックするか、`Print(name ?? "名無し")` のように `null` でない値にします。`Print` の中で `null` を扱えるなら、パラメータの型を `string?` にします。

---

## ワンポイントアドバイス

### 警告をエラーにする

null 許容参照型の警告は、`.csproj` に次の設定を追加すると、エラーとして扱われます。警告を見落としたままビルドできなくなります。

```xml
<WarningsAsErrors>nullable</WarningsAsErrors>
```

`<PropertyGroup>` の中の、`<Nullable>enable</Nullable>` の近くに書きます。

---

## まとめ

- null 許容参照型は、`null` を扱う誤りを、コンパイラーに警告させる機能
- `string` は `null` を入れない前提、`string?` は `null` が入ることがある変数
- 主な警告は、CS8600（代入）、CS8602（メンバーの使用）、CS8603（戻り値）、CS8604（引数）、CS8618（フィールドの初期化）。どれもエラーではなく警告
- `int?` は `Nullable<int>` という別の型だが、`string?` は `string` と同じ型。参照型の `?` は、コンパイラーのための目印
- `null` をチェックした後は、フロー解析によって警告が出なくなる。`??` や `?.` も使える
- `?.` は、左辺が `null` ならメンバーを使わずに `null` を返す
- `!` は警告を消すだけで、`null` をチェックしない
- `?` のないフィールドは、初期値かコンストラクターで初期化する
- 設定は `.csproj` の `<Nullable>` で、ファイルの一部は `#nullable` で変えられる

---

## 理解度チェック

1. 次のコードを実行すると何が出力されますか？

   ```csharp
   string? a = null;
   string? b = "hello";
   Console.WriteLine(a?.Length ?? -1);
   Console.WriteLine(b?.Length ?? -1);
   ```

2. 次のコードをビルドすると、どの行で、どの警告が出ますか？

   ```csharp
   string? a = null;
   string b = "x";
   b = a;
   if (a != null)
   {
       b = a;
   }
   Console.WriteLine(b);
   Console.WriteLine(a.Length);
   ```

3. `void Show(int value)` と `void Show(int? value)` は同じクラスに定義できますが、`void Show(string text)` と `void Show(string? text)` は定義できません。なぜですか？
4. `string? found = FindName(3);` の後で、`found!.Length` と書くと警告は出ません。この書き方の問題点は何ですか？

<details markdown="1">
<summary>解答を見る</summary>

1. 次のように出力されます。`a` は `null` なので、`a?.Length` は `null` になり、`?? -1` で `-1` になります。`b?.Length` は `"hello"` の文字数の `5` です。

   ```
   -1
   5
   ```

2. 3 行目の `b = a;` で CS8600、9 行目の `a.Length` で CS8602 が出ます。6 行目の `b = a;` は、`if (a != null)` の中なので、警告は出ません。なお、このコードを実行すると、9 行目で `NullReferenceException` が発生します。
3. `int?` は `Nullable<int>` という別の型なので、パラメータの型が違うメソッドとして定義できます。`string?` は `string` と同じ型なので、同じシグネチャのメソッドが 2 つあることになり、CS0111 のエラーになります。
4. `!` は警告を消すだけで、`null` かどうかを確かめません。`FindName(3)` が `null` を返すと、`found!.Length` で `NullReferenceException` が発生します。`if (found != null)` でチェックするか、`found?.Length ?? 0` のように書きます。

</details>

---

## 次のステップ

[タプル](/unity-csharp-learning/csharp/tuples/) では、複数の値を 1 つにまとめるタプルと、その正体である `ValueTuple` 構造体を学びます。
