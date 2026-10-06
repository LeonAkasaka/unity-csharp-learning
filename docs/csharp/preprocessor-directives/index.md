---
layout: page
title: プリプロセッサディレクティブ
permalink: /csharp/preprocessor-directives/
---

# プリプロセッサディレクティブ

開発中だけ動かしたいコードを、完成したプログラムから取り除くには、そのたびにコメントアウトする必要がありました。`#` で始まる **プリプロセッサディレクティブ**（preprocessor directive）を使うと、自分で定義した **シンボル**（symbol）によって、コードのどの部分をコンパイルするかを切り替えられます。このページでは、シンボルを定義して、コンパイルされるコードが切り替わるのを確かめます。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- `#define` でシンボルを定義し、`#if` / `#endif` で囲んだコードをコンパイルするかどうかを切り替えられる
- `#if` と `if` 文の違いを説明できる
- `#else`、`#elif` と、`!`、`&&`、`||` を使った条件を書ける
- `#define` がファイルごとに効くことを説明し、`.csproj` の `<DefineConstants>` でプロジェクト全体にシンボルを定義できる
- `#error`、`#warning`、`#region`、`#pragma warning` を使える

## 前提知識

- [ファイルの分割と global using](/unity-csharp-learning/csharp/multiple-files/) を読み、プロジェクトにファイルを追加したり、`.csproj` を編集したりできること
- [static メンバーと static クラス](/unity-csharp-learning/csharp/static-members/) を読んでいること

---

## 1. 開発中だけ動かしたいコード

[.NET SDK と dotnet CLI](/unity-csharp-learning/csharp/dotnet-sdk/) で学んだ手順で、コンソールアプリのプロジェクトを作ります。

```powershell
dotnet new console -n SampleDirectives
```

`Program.cs` を、次のように書き換えます。ダメージを受けた後の HP を表示するプログラムです。計算が正しいかを確かめるために、`[log]` で始まる行で途中の値を表示しています。

```csharp
int hp = 100;
int damage = 30;
hp -= damage;
Console.WriteLine($"[log] damage={damage}, hp={hp}");
Console.WriteLine($"HP: {hp}");
```

`SampleDirectives` フォルダーで `dotnet run` を実行します。

```
[log] damage=30, hp=70
HP: 70
```

`[log]` の行は、開発中に確かめるためのもので、完成したプログラムでは表示したくありません。しかし、この行を削除したりコメントアウトしたりすると、後でまた確かめたくなったときに書き戻す必要があります。このような行がプログラムのあちこちにあると、書き換えの手間がかかり、戻し忘れも起こります。

---

## 2. #define と #if

[#define](https://learn.microsoft.com/dotnet/csharp/language-reference/preprocessor-directives#defining-symbols) でシンボルを定義し、[#if](https://learn.microsoft.com/dotnet/csharp/language-reference/preprocessor-directives#conditional-compilation) と `#endif` でコードを囲むと、シンボルが定義されているときだけ、そのコードがコンパイルされます。このように、条件によってコンパイルするコードを切り替えることを **条件付きコンパイル**（conditional compilation）といいます。

**書式：[#define ディレクティブ](https://learn.microsoft.com/dotnet/csharp/language-reference/preprocessor-directives#defining-symbols)**
```
#define シンボル名
```

**書式：[#if ディレクティブ](https://learn.microsoft.com/dotnet/csharp/language-reference/preprocessor-directives#conditional-compilation)**
```
#if シンボル名
    シンボルが定義されているときだけコンパイルされるコード
#endif
```

| 要素 | 説明 |
|---|---|
| `#define シンボル名` | シンボルを定義する。ファイルの先頭（コードより前）に書く |
| `#if シンボル名` | シンボルが定義されていれば、`#endif` までのコードをコンパイルする |
| `#endif` | `#if` の範囲の終わり |

プリプロセッサディレクティブは、1 行に 1 つずつ書きます。行の先頭（空白の後でもよい）が `#` で始まり、末尾に `;` は付けません。シンボルには値はなく、定義されているかどうかだけを表します。

`Program.cs` を、次のように書き換えます。`VERBOSE` というシンボルを定義し、`[log]` の行を `#if VERBOSE` と `#endif` で囲んでいます。

```csharp
#define VERBOSE

int hp = 100;
int damage = 30;
hp -= damage;
#if VERBOSE
Console.WriteLine($"[log] damage={damage}, hp={hp}");
#endif
Console.WriteLine($"HP: {hp}");
```

```
[log] damage=30, hp=70
HP: 70
```

`VERBOSE` が定義されているので、`[log]` の行がコンパイルされ、表示されます。

次に、1 行目の `#define VERBOSE` を、`//` でコメントアウトします。

```csharp
// #define VERBOSE
```

もう一度 `dotnet run` を実行すると、`[log]` の行が表示されなくなります。

```
HP: 70
```

`[log]` の行を書き換えずに、1 行目だけで表示するかどうかを切り替えられました。

### コンパイルされないことを確かめる

`#if` は、`if` 文とは違い、実行中に条件を調べるわけではありません。シンボルが定義されていなければ、`#if` から `#endif` までのコードは、コンパイルの対象から外されます。

このことを、`#if` の中に、わざと `;` を書き忘れた行を入れて確かめます。`Program.cs` を、次のように書き換えます。`#define VERBOSE` は書いていません。

```csharp
int hp = 100;
int damage = 30;
hp -= damage;
#if VERBOSE
Console.WriteLine("未完成")
#endif
Console.WriteLine($"HP: {hp}");
```

```
HP: 70
```

`;` がないのに、コンパイルエラーにならずに実行できました。`VERBOSE` が定義されていないので、`#if` の中の行は、コンパイラーに渡されていないからです。

ファイルの先頭に `#define VERBOSE` を書き足すと、`#if` の中の行がコンパイルされるようになり、CS1002（`;` が必要です）のコンパイルエラーになります。確かめたら、`#define VERBOSE` の行は削除しておきます。

`if` 文で同じことをしようとすると、条件が `false` でも `if` の中のコードはコンパイルされます。`;` がなければコンパイルエラーになりますし、完成したプログラムにも、使わないコードが含まれたままになります。`#if` で外したコードは、完成したプログラムには含まれません。

> 💡 **ポイント**: プリプロセッサとは、コンパイルの前にソースコードを処理するものを指す言葉です。C# のコンパイラーには、独立したプリプロセッサはありません。コンパイラーが、ソースコードを読み込むときに、プリプロセッサディレクティブを処理します。

---

## 3. #else と #elif

[#else と #elif](https://learn.microsoft.com/dotnet/csharp/language-reference/preprocessor-directives#conditional-compilation) を使うと、`if` 文の `else` や `else if` のように、シンボルによってコンパイルするコードを選べます。

**書式：[#if・#elif・#else ディレクティブ](https://learn.microsoft.com/dotnet/csharp/language-reference/preprocessor-directives#conditional-compilation)**
```
#if 条件1
    条件1 を満たすときにコンパイルされるコード
#elif 条件2
    条件1 を満たさず、条件2 を満たすときにコンパイルされるコード
#else
    どの条件も満たさないときにコンパイルされるコード
#endif
```

条件には、シンボル名のほかに、`!`（でない）、`&&`（かつ）、`||`（または）と、かっこを使えます。

`Program.cs` を、次のように書き換えます。難易度を表すシンボル `EASY` と `HARD` によって、ダメージの値を変えています。

```csharp
#define HARD
#define VERBOSE

#if EASY
int damage = 10;
#elif HARD
int damage = 50;
#else
int damage = 30;
#endif

int hp = 100;
hp -= damage;
#if VERBOSE && !EASY
Console.WriteLine($"[log] damage={damage}, hp={hp}");
#endif
Console.WriteLine($"HP: {hp}");
```

```
[log] damage=50, hp=50
HP: 50
```

`EASY` は定義されておらず、`HARD` が定義されているので、`int damage = 50;` がコンパイルされます。`VERBOSE && !EASY` は、`VERBOSE` が定義されていて、`EASY` が定義されていないときに満たされるので、`[log]` の行も表示されます。

---

## 4. プロジェクト全体にシンボルを定義する

### #define はファイルごとに効く

ログを表示する処理を、別のファイルのクラスに分けます。`SampleDirectives` フォルダーに `Log.cs` を追加して、次のように書きます。

```csharp
static class Log
{
    public static void Write(string message)
    {
#if VERBOSE
        Console.WriteLine($"[log] {message}");
#endif
    }
}
```

`Program.cs` を、次のように書き換えます。`#define VERBOSE` で `VERBOSE` を定義してから、`Log.Write` を呼び出しています。

```csharp
#define VERBOSE

int hp = 100;
int damage = 30;
hp -= damage;
Log.Write($"damage={damage}, hp={hp}");
Console.WriteLine($"HP: {hp}");
```

```
HP: 70
```

`VERBOSE` を定義したのに、`[log]` の行が表示されません。`#define` で定義したシンボルは、そのファイルの中でだけ有効だからです。`Program.cs` で定義した `VERBOSE` は、`Log.cs` では定義されていないので、`Log.cs` の `#if VERBOSE` の中はコンパイルされません。

### .csproj の DefineConstants

プロジェクトのすべてのファイルでシンボルを定義するには、`.csproj` の [DefineConstants](https://learn.microsoft.com/dotnet/csharp/language-reference/compiler-options/language#defineconstants) 要素を使います。

まず、`Program.cs` の 1 行目の `#define VERBOSE` と、その後の空行を削除します。

```csharp
int hp = 100;
int damage = 30;
hp -= damage;
Log.Write($"damage={damage}, hp={hp}");
Console.WriteLine($"HP: {hp}");
```

次に、`SampleDirectives.csproj` の `</Project>` の前に、次の `<PropertyGroup>` を追加します。

```xml
  <PropertyGroup>
    <DefineConstants>$(DefineConstants);VERBOSE</DefineConstants>
  </PropertyGroup>
```

追加した後の `SampleDirectives.csproj` の全体は、次のようになります。

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <PropertyGroup>
    <DefineConstants>$(DefineConstants);VERBOSE</DefineConstants>
  </PropertyGroup>

</Project>
```

| 要素 | 説明 |
|---|---|
| `<DefineConstants>` | プロジェクトのすべてのファイルで定義するシンボル。複数のときは `;` で区切る |
| `$(DefineConstants)` | ここまでに定義されているシンボル。先頭に書くと、それらを残したまま `VERBOSE` を追加できる |

`dotnet run` を実行すると、`Log.cs` でも `VERBOSE` が定義され、`[log]` の行が表示されます。

```
[log] damage=30, hp=70
HP: 70
```

`$(DefineConstants)` には、.NET SDK があらかじめ定義しているシンボルが入っています。`$(DefineConstants);` を書かずに `<DefineConstants>VERBOSE</DefineConstants>` とすると、それらの一部が消えてしまいます。.NET SDK が定義しているシンボルは、[定義済みのシンボル（補足）](/unity-csharp-learning/csharp/predefined-symbols/) で紹介します。

### #undef でファイルの中だけ取り消す

[#undef](https://learn.microsoft.com/dotnet/csharp/language-reference/preprocessor-directives#defining-symbols) を使うと、定義されているシンボルを、そのファイルの中でだけ取り消せます。

**書式：[#undef ディレクティブ](https://learn.microsoft.com/dotnet/csharp/language-reference/preprocessor-directives#defining-symbols)**
```
#undef シンボル名
```

`#define` と同じように、ファイルの先頭に書きます。`Log.cs` の先頭に `#undef VERBOSE` を書き足します。

```csharp
#undef VERBOSE

static class Log
{
    public static void Write(string message)
    {
#if VERBOSE
        Console.WriteLine($"[log] {message}");
#endif
    }
}
```

```
HP: 70
```

`.csproj` では `VERBOSE` を定義したままですが、`Log.cs` の中では取り消されるので、`[log]` の行が表示されなくなります。`#undef` も `#define` と同じく、書いたファイルの中でだけ効きます。`#undef VERBOSE` を `Log.cs` ではなく `Program.cs` に書いても、`Log.cs` の `VERBOSE` は取り消されません。

---

## 5. そのほかのディレクティブ

### #error と #warning

[#error と #warning](https://learn.microsoft.com/dotnet/csharp/language-reference/preprocessor-directives#error-and-warning-information) は、コンパイルするときに、指定したメッセージのエラーや警告を出します。シンボルの組み合わせが正しくないことを知らせるために、`#if` と組み合わせて使います。

**書式：[#error・#warning ディレクティブ](https://learn.microsoft.com/dotnet/csharp/language-reference/preprocessor-directives#error-and-warning-information)**
```
#error メッセージ
#warning メッセージ
```

次のコードは、`EASY` と `HARD` が両方とも定義されているときに、コンパイルエラーにします。

```csharp
#define EASY
#define HARD

#if EASY && HARD
#error EASY と HARD は同時に定義できません
#endif

Console.WriteLine("開始");
```

`dotnet run` を実行すると、CS1029 のコンパイルエラーになり、エラーの説明に「EASY と HARD は同時に定義できません」が表示されます。`#define EASY` か `#define HARD` のどちらかを削除すると、エラーはなくなります。

`#warning` にすると、メッセージは警告（CS1030）として表示され、プログラムはビルド・実行されます。

### #region と #endregion

[#region と #endregion](https://learn.microsoft.com/dotnet/csharp/language-reference/preprocessor-directives#defining-regions) は、コードの範囲に名前を付けます。Visual Studio などのエディターでは、この範囲を折りたたんで表示できます。コンパイルの結果には影響しません。

**書式：[#region・#endregion ディレクティブ](https://learn.microsoft.com/dotnet/csharp/language-reference/preprocessor-directives#defining-regions)**
```
#region 名前
    コード
#endregion
```

長いファイルの見通しをよくするために使われることがありますが、範囲を折りたたまないと読めないほどファイルが長いなら、クラスやファイルを分けることも考えます。

### #pragma warning

[#pragma warning](https://learn.microsoft.com/dotnet/csharp/language-reference/preprocessor-directives#pragma-warning) は、指定した番号の警告を、範囲を決めて表示しないようにします。

**書式：[#pragma warning ディレクティブ](https://learn.microsoft.com/dotnet/csharp/language-reference/preprocessor-directives#pragma-warning)**
```
#pragma warning disable 警告の番号
    この範囲では、指定した警告が表示されない
#pragma warning restore 警告の番号
```

次のコードでは、値を代入しただけで使っていない変数が 2 つあり、どちらも CS0219 の警告の対象です。

```csharp
#pragma warning disable CS0219
int unused1 = 0;
#pragma warning restore CS0219
int unused2 = 0;
Console.WriteLine("開始");
```

`dotnet build` を実行すると、CS0219 の警告は `unused2` の 1 つだけが表示されます。`unused1` の警告は、`#pragma warning disable` と `restore` の間にあるので表示されません。

警告は、コードの問題を知らせるものです。理由があって問題ないとわかっている箇所にだけ、範囲を狭くして使います。

### #nullable

`#nullable` もプリプロセッサディレクティブの 1 つです。[null 許容参照型](/unity-csharp-learning/csharp/nullable-reference-types/) で学びます。

---

## よくあるミス

### #define をコードの後に書く

`#define` と `#undef` は、ファイルの中で、コードよりも前に書く必要があります。コードの後に書くと、コンパイルエラーになります。

```csharp
int hp = 100;
#define VERBOSE  // ❌ CS1032: コードの後でシンボルを定義できない
Console.WriteLine(hp);
```

`#if` は、コードの途中のどこにでも書けますが、`#define` と `#undef` はファイルの先頭にまとめます。

### #endif を書き忘れる

`#if` には、対応する `#endif` が必要です。書き忘れると、ファイルの終わりで CS1027（`#endif` ディレクティブが必要です）のコンパイルエラーになります。

```csharp
#define VERBOSE
#if VERBOSE
Console.WriteLine("1");
Console.WriteLine("2");
// ❌ CS1027: #endif がない
```

---

## まとめ

- プリプロセッサディレクティブは `#` で始まる行で、コンパイルするときに処理される
- `#define` で定義したシンボルによって、`#if` から `#endif` までのコードをコンパイルするかどうかが切り替わる
- `#if` で外したコードは、コンパイルされず、完成したプログラムにも含まれない。`if` 文とは違う
- `#else`、`#elif` と、`!`、`&&`、`||` で、条件によってコンパイルするコードを選べる
- `#define` と `#undef` は、書いたファイルの中でだけ効き、ファイルの先頭に書く
- プロジェクト全体にシンボルを定義するには、`.csproj` の `<DefineConstants>` に `$(DefineConstants);シンボル名` と書く
- `#error` / `#warning` でメッセージを出し、`#region` で範囲に名前を付け、`#pragma warning` で警告を止められる

---

## 理解度チェック

1. `#if VERBOSE` と、`bool verbose = false;` を使った `if (verbose)` の違いを説明してください。
2. 次のコードを実行すると何が出力されますか？

   ```csharp
   #define A
   #define C

   #if A && B
   Console.WriteLine("1");
   #elif A || B
   Console.WriteLine("2");
   #else
   Console.WriteLine("3");
   #endif
   #if !B && C
   Console.WriteLine("4");
   #endif
   ```

3. 4 節で、`Program.cs` に `#define VERBOSE` を書いても、`Log.cs` の `[log]` の行が表示されなかったのはなぜですか？ また、表示するにはどうしますか？

<details markdown="1">
<summary>解答を見る</summary>

1. `#if VERBOSE` は、シンボル `VERBOSE` が定義されていなければ、`#endif` までのコードをコンパイルしません。そのコードは完成したプログラムに含まれず、文法の誤りがあってもエラーになりません。`if (verbose)` は、実行中に条件を調べる文なので、`if` の中のコードもコンパイルされ、完成したプログラムに含まれます。
2. 次のように出力されます。`B` は定義されていないので `A && B` は満たされず、`A || B` が満たされて `2` が表示されます。`!B && C` も満たされるので、`4` も表示されます。

   ```
   2
   4
   ```

3. `#define` で定義したシンボルは、書いたファイルの中でだけ有効だからです。`Log.cs` では `VERBOSE` が定義されていないので、`#if VERBOSE` の中がコンパイルされません。`.csproj` に `<DefineConstants>$(DefineConstants);VERBOSE</DefineConstants>` を書くと、プロジェクトのすべてのファイルで `VERBOSE` が定義され、表示されるようになります。

</details>

---

## 次のステップ

[定義済みのシンボル（補足）](/unity-csharp-learning/csharp/predefined-symbols/) では、自分で定義しなくても使える、.NET SDK が定義しているシンボルを紹介します。
