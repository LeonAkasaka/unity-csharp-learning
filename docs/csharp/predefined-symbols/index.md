---
layout: page
title: 定義済みのシンボル（補足）
permalink: /csharp/predefined-symbols/
---

# 定義済みのシンボル（補足）

[プリプロセッサディレクティブ](/unity-csharp-learning/csharp/preprocessor-directives/) では、`#define` や `.csproj` の `<DefineConstants>` で、自分でシンボルを定義しました。.NET SDK は、プロジェクトをビルドするときに、いくつかのシンボルをあらかじめ定義しています。このページでは、ビルドの種類を表す `DEBUG` と、対象の .NET のバージョンを表すシンボルを紹介します。

## 学習目標

このページを読み終えると、以下のことができるようになります。

- デバッグビルドとリリースビルドを切り替えて実行できる
- `#if DEBUG` で、デバッグビルドのときだけコンパイルされるコードを書ける
- 対象の .NET のバージョンから、定義されるシンボルの名前を答えられる
- 定義済みのシンボルの一覧を、公式ドキュメントで調べられる

## 前提知識

- [プリプロセッサディレクティブ](/unity-csharp-learning/csharp/preprocessor-directives/) を読んでいること

---

## 1. ビルド構成と DEBUG

### デバッグビルドとリリースビルド

`dotnet run` や `dotnet build` は、何も指定しなければ **デバッグビルド**（Debug）でビルドします。デバッグビルドは、プログラムの誤りを調べやすいように、ソースコードに近い形でビルドされます。`-c Release` を付けると **リリースビルド**（Release）になり、速く動くように最適化してビルドされます。完成したプログラムを配るときは、リリースビルドを使います。

デバッグビルドとリリースビルドのような、ビルドの設定の組を **ビルド構成**（build configuration）といいます。ビルドされたファイルは、ビルド構成ごとに `bin/Debug` と `bin/Release` のフォルダーに分けて作られます。

```powershell
dotnet run
dotnet run -c Release
```

| 指定 | ビルド構成 |
|---|---|
| なし | デバッグビルド（Debug） |
| `-c Release` | リリースビルド（Release） |

### DEBUG シンボル

.NET SDK は、デバッグビルドのときに `DEBUG` シンボルを定義します。リリースビルドでは定義しません。`#if DEBUG` で囲んだコードは、デバッグビルドのときだけコンパイルされます。

[プリプロセッサディレクティブ](/unity-csharp-learning/csharp/preprocessor-directives/) の `SampleDirectives` プロジェクトで確かめます。`SampleDirectives.csproj` から、`<DefineConstants>` を書いた `<PropertyGroup>` を削除して、次の内容に戻します。

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

`Log.cs` を、次のように書き換えます。自分で定義した `VERBOSE` の代わりに、`DEBUG` を使っています。`#undef VERBOSE` の行も削除しています。

```csharp
static class Log
{
    public static void Write(string message)
    {
#if DEBUG
        Console.WriteLine($"[log] {message}");
#endif
    }
}
```

`Program.cs` は、[プリプロセッサディレクティブ](/unity-csharp-learning/csharp/preprocessor-directives/) の 4 節と同じです。

```csharp
int hp = 100;
int damage = 30;
hp -= damage;
Log.Write($"damage={damage}, hp={hp}");
Console.WriteLine($"HP: {hp}");
```

`dotnet run` を実行すると、デバッグビルドなので `DEBUG` が定義され、`[log]` の行が表示されます。

```
[log] damage=30, hp=70
HP: 70
```

`dotnet run -c Release` を実行すると、リリースビルドなので `DEBUG` は定義されず、`[log]` の行は表示されません。

```
HP: 70
```

`#define` も `.csproj` も書き換えずに、ビルド構成を選ぶだけで、開発中の確認用のコードを切り替えられました。

### TRACE と RELEASE

`DEBUG` のほかに、次のシンボルも定義されます。

| シンボル | 定義されるとき |
|---|---|
| `DEBUG` | デバッグビルド |
| `TRACE` | デバッグビルドとリリースビルドの両方 |

リリースビルドのときだけコンパイルしたいコードは、`#if !DEBUG` で囲みます。いまの .NET SDK は、リリースビルドのときに `RELEASE` というシンボルも定義しますが、公式ドキュメントに挙げられているのは `DEBUG` と `TRACE` です。この 2 つを使うほうが確実です。

> 💡 **ポイント**: [プリプロセッサディレクティブ](/unity-csharp-learning/csharp/preprocessor-directives/) で、`<DefineConstants>` の先頭に `$(DefineConstants);` を書いたのは、.NET SDK が定義したシンボルを残すためです。`$(DefineConstants);` を書かずに `<DefineConstants>VERBOSE</DefineConstants>` とすると、`TRACE` が定義されなくなります。`DEBUG` と、次の節のバージョンのシンボルは、`.csproj` を読み込んだ後で追加されるので、この書き方でも残ります。

---

## 2. .NET のバージョンのシンボル

### 対象のバージョンからシンボルが決まる

`.csproj` の `<TargetFramework>` は、プログラムを動かす .NET のバージョン（**ターゲットフレームワーク**、target framework）を指定します。.NET SDK は、このバージョンを表すシンボルを定義します。

シンボルの名前は、`<TargetFramework>` に書いた名前の `.` を `_` に置き換え、大文字にしたものです。`net10.0` なら `NET10_0` です。さらに、「そのバージョン以降」を表す `_OR_GREATER` の付いたシンボルが、.NET 5 からそのバージョンまでの各バージョンについて定義されます。

| `<TargetFramework>` | 定義されるシンボルの例 |
|---|---|
| `net10.0` | `NET`、`NET10_0`、`NET10_0_OR_GREATER`、`NET9_0_OR_GREATER`、`NET8_0_OR_GREATER`、……、`NET5_0_OR_GREATER` |
| `net8.0` | `NET`、`NET8_0`、`NET8_0_OR_GREATER`、`NET7_0_OR_GREATER`、……、`NET5_0_OR_GREATER` |

`NET10_0_OR_GREATER` は「.NET 10 以降」という意味です。`net10.0` でも、この後の新しいバージョンでも定義されます。

`Program.cs` を、次のように書き換えます。

```csharp
#if NET10_0_OR_GREATER
Console.WriteLine(".NET 10 以降");
#elif NET8_0_OR_GREATER
Console.WriteLine(".NET 8 または .NET 9");
#else
Console.WriteLine(".NET 8 より前");
#endif
```

`<TargetFramework>` が `net10.0` なので、`NET10_0_OR_GREATER` が定義され、次のように表示されます。

```
.NET 10 以降
```

### バージョンで切り分ける場面

1 つのプログラムを、複数のバージョンの .NET 向けにビルドすることがあります。たとえば、ほかの人に使ってもらうライブラリを、古いバージョンの .NET でも使えるようにする場合です。`.csproj` に `<TargetFrameworks>`（`s` が付く）を書くと、複数のバージョン向けに、それぞれビルドされます。

```xml
<TargetFrameworks>net8.0;net10.0</TargetFrameworks>
```

このとき、新しいバージョンにしかない機能を使う部分を、`#if NET10_0_OR_GREATER` で囲み、`#else` に古いバージョン向けのコードを書きます。`net10.0` 向けのビルドでは新しい機能を使い、`net8.0` 向けのビルドでは代わりのコードを使うようになります。

バージョンを切り分けるときは、`NET10_0` よりも `NET10_0_OR_GREATER` を使います。`NET10_0` は .NET 10 のときだけ定義されるので、.NET 11 向けにビルドすると、新しい機能を使うほうのコードが外れてしまいます。

### シンボルの一覧

定義されるシンボルの一覧は、公式ドキュメントの [ターゲット フレームワークのプリプロセッサ シンボル](https://learn.microsoft.com/dotnet/standard/frameworks#preprocessor-symbols) にあります。.NET Framework や .NET Standard を表すシンボルや、`net10.0-windows` のように OS を指定したときに定義される `WINDOWS` などのシンボルも、ここに載っています。

---

## まとめ

- `dotnet run` と `dotnet build` は既定でデバッグビルド、`-c Release` でリリースビルドになる
- デバッグビルドでは `DEBUG` が定義される。`TRACE` は両方で定義される
- リリースビルドだけのコードは `#if !DEBUG` で囲む
- `<TargetFramework>` から、`NET10_0` や `NET10_0_OR_GREATER` などのシンボルが定義される
- バージョンで切り分けるときは `_OR_GREATER` の付いたシンボルを使う
- シンボルの一覧は、公式ドキュメントの「ターゲット フレームワーク」のページで調べる

---

## 理解度チェック

1. 次のコードを、`dotnet run -c Release` で実行すると何が出力されますか？

   ```csharp
   #if DEBUG
   Console.WriteLine("A");
   #endif
   #if TRACE
   Console.WriteLine("B");
   #endif
   #if !DEBUG
   Console.WriteLine("C");
   #endif
   ```

2. `<TargetFramework>` が `net9.0` のとき、次のシンボルのうち、定義されるものをすべて選んでください。

   1. `NET9_0`
   2. `NET8_0`
   3. `NET8_0_OR_GREATER`
   4. `NET10_0_OR_GREATER`

3. 新しいバージョンの .NET にしかない機能を使う部分を、`#if NET10_0` ではなく `#if NET10_0_OR_GREATER` で囲むのはなぜですか？

<details markdown="1">
<summary>解答を見る</summary>

1. 次のように出力されます。リリースビルドでは `DEBUG` が定義されないので `A` は表示されず、`!DEBUG` が満たされて `C` が表示されます。`TRACE` はリリースビルドでも定義されるので、`B` も表示されます。

   ```
   B
   C
   ```

2. 1 と 3 です。`net9.0` では、`NET9_0` と、`NET9_0_OR_GREATER`、`NET8_0_OR_GREATER`、……、`NET5_0_OR_GREATER` が定義されます。.NET 9 は「.NET 8 以降」にも当てはまるからです。`NET8_0` は .NET 8 のときだけ、`NET10_0_OR_GREATER` は .NET 10 以降のときだけ定義されます。
3. `NET10_0` は .NET 10 向けにビルドするときだけ定義されるからです。.NET 11 など、もっと新しいバージョン向けにビルドすると、`#if NET10_0` の中は外れてしまいます。`NET10_0_OR_GREATER` は、.NET 10 以降のすべてのバージョンで定義されます。

</details>

---

## 次のステップ

これで「C# プログラムの構成」のセクションは終わりです。[継承](/unity-csharp-learning/csharp/inheritance/) からは「C# 継承と抽象化」のセクションに進み、既存のクラスのメンバーを引き継いで、新しいクラスを作る仕組みを学びます。
