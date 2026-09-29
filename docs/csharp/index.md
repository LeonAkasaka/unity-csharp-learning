---
layout: page
title: C# 言語入門
permalink: /csharp/
---

# C# 言語入門

C# プログラミングをゼロから学びます。

## このセクションの内容

### .NET の仕組み

| # | トピック | 概要 |
|---|---|---|
| 1 | [C# と .NET の基本](/unity-csharp-learning/csharp/dotnet-overview/) | コンパイルと実行の仕組み、.NET の役割 |
| 2 | [中間言語と JIT コンパイル](/unity-csharp-learning/csharp/dotnet-internals/) | IL・CLR・JIT、Unity の Mono と IL2CPP |
| 3 | [.NET SDK と dotnet CLI](/unity-csharp-learning/csharp/dotnet-sdk/) | SDK のインストールから作成・ビルド・実行まで |

### C# 基本文法

| # | トピック | 概要 |
|---|---|---|
| 4 | [最初のプログラムと変数](/unity-csharp-learning/csharp/variables/) | 逐次実行・リテラル・算術演算・変数の宣言と代入 |
| 4.1 | [名前空間と using ディレクティブ（補足）](/unity-csharp-learning/csharp/using-directives/) | 完全修飾名・`using` ディレクティブ・暗黙的な using ディレクティブと `ImplicitUsings`・下の階層の名前空間は読み込まれない |
| 5 | [プリミティブ型と型変換](/unity-csharp-learning/csharp/primitive-types/) | 数値型の表現範囲・符号・char と string・型変換・異なる型の演算 |
| 5.1 | [数値リテラルと型エイリアス（補足）](/unity-csharp-learning/csharp/numeric-literals/) | 0x/0b リテラル・型サフィックス・int=System.Int32・2の補数 |
| 6 | [条件分岐](/unity-csharp-learning/csharp/conditionals/) | `if` / `else`・比較演算子と論理演算子・`switch` 文 |
| 6.1 | [ブロック文とスコープ（補足）](/unity-csharp-learning/csharp/block-and-scope/) | ブロック文・スコープ・`else if` の正体 |
| 6.2 | [条件演算子と式・文（補足）](/unity-csharp-learning/csharp/conditional-operator/) | 式と文の違い・`? :` 演算子 |
| 7 | [反復処理](/unity-csharp-learning/csharp/loops/) | while・do-while・for・foreach による繰り返し処理 |
| 7.1 | [インクリメント・デクリメント（補足）](/unity-csharp-learning/csharp/increment-decrement/) | `++` / `--` の前置と後置の違い・複合代入演算子 |
| 7.2 | [break と continue（補足）](/unity-csharp-learning/csharp/break-and-continue/) | ループの途中脱出とスキップ |
| 8 | [ビット演算](/unity-csharp-learning/csharp/bitwise-operations/) | AND・OR・XOR・シフト・ビットマスクによるフラグ管理 |

### C# 配列と集合操作

| # | トピック | 概要 |
|---|---|---|
| 9 | [配列の基礎](/unity-csharp-learning/csharp/arrays/) | 宣言・初期化・インデックスアクセス・Length・for/foreach 走査 |
| 9.1 | [配列と foreach（補足）](/unity-csharp-learning/csharp/arrays-and-foreach/) | foreach の書式詳細・var・読み取り専用・for との使い分け |
| 9.2 | [Array クラスと配列の性質（補足）](/unity-csharp-learning/csharp/array-class/) | 参照型の挙動・Sort/Reverse/IndexOf/Copy/Clear |
| 9.3 | [インデックスと範囲（補足）](/unity-csharp-learning/csharp/ranges/) | 末尾から数える `^` と `^0`・範囲演算子 `..` と終了位置を含まないこと・省略した書き方・範囲は新しい配列を作る・`Index` / `Range` |
| 9.4 | [ビットパッキング（補足）](/unity-csharp-learning/csharp/bit-packing/) | bool[8] を byte で表現するパック/アンパックの手法 |
| 10 | [多次元配列](/unity-csharp-learning/csharp/multidimensional-arrays/) | 2 次元配列（行列）の宣言・初期化・GetLength・ネストループ走査 |
| 11 | [ジャグ配列](/unity-csharp-learning/csharp/jagged-arrays/) | 可変長行の配列・多次元配列との比較と使い分け |

### C# クラスとオブジェクト

| # | トピック | 概要 |
|---|---|---|
| 12 | [クラスとフィールド](/unity-csharp-learning/csharp/classes/) | クラスの定義・インスタンスの作成・フィールドと初期値 |
| 13 | [メソッド](/unity-csharp-learning/csharp/methods/) | メソッドの定義・パラメータ・戻り値・オーバーロード |
| 14 | [コンストラクター](/unity-csharp-learning/csharp/constructors/) | `new` 時の自動初期化・既定のコンストラクター・`this` |
| 15 | [アクセス修飾子](/unity-csharp-learning/csharp/access-modifiers/) | `public` / `private` によるカプセル化 |
| 16 | [プロパティ](/unity-csharp-learning/csharp/properties/) | `get` / `set` アクセサー・自動実装・読み取り専用プロパティ |
| 17 | [インデクサ](/unity-csharp-learning/csharp/indexers/) | `this[]` で配列のようにアクセスできるクラスの定義 |

### C# メソッドの応用文法

| # | トピック | 概要 |
|---|---|---|
| 18 | [ref / out / in パラメータ](/unity-csharp-learning/csharp/ref-out-in/) | 値渡しとの違い・参照渡し・読み取り専用の参照渡し |
| 19 | [省略可能パラメータと名前付き引数](/unity-csharp-learning/csharp/optional-named-params/) | デフォルト値・名前付き引数 |
| 20 | [params キーワード](/unity-csharp-learning/csharp/params-keyword/) | `params` による可変長引数・配列渡しとの違い・コンパイラの変換 |
| 21 | [オーバーロード解決](/unity-csharp-learning/csharp/overload-resolution/) | 候補の絞り込み・完全一致 / 暗黙変換 / params の優先順位・あいまいエラー |
| 22 | [演算子のオーバーロード](/unity-csharp-learning/csharp/operator-overloading/) | 自作クラスに `+` や `==` などの演算子を定義する方法 |
| 23 | [再帰関数とコールスタック](/unity-csharp-learning/csharp/recursion/) | 再帰呼び出し・終了条件・スタックフレームの積み重なり |
| 24 | [static メンバーと static クラス](/unity-csharp-learning/csharp/static-members/) | クラスに属するメンバー・static コンストラクタ・static class |
| 24.1 | [const と readonly（補足）](/unity-csharp-learning/csharp/const-readonly/) | `const` と定数式・クラスの定数・`readonly` フィールド・`static readonly`・`readonly` なのは参照だけ |
| 25 | [拡張メソッド](/unity-csharp-learning/csharp/extension-methods/) | 既存の型にメソッドを追加したように見せる書き方 |

### C# プログラムの構成

| # | トピック | 概要 |
|---|---|---|
| 26 | [名前空間](/unity-csharp-learning/csharp/namespaces/) | グローバル名前空間・`namespace` の宣言・同じ名前の型とエイリアス・`using static`・拡張メソッドと `using`・名前空間の階層 |
| 27 | [ファイルの分割と global using](/unity-csharp-learning/csharp/multiple-files/) | プロジェクトへのファイルの追加・ファイルスコープの名前空間・`global using`・暗黙的な using ディレクティブの正体・`.csproj` の `<Using>` |

### C# 継承と抽象化

| # | トピック | 概要 |
|---|---|---|
| 28 | [継承](/unity-csharp-learning/csharp/inheritance/) | 似たクラスを別々に書く問題・基底クラスと派生クラス・メンバーの追加・`: base(...)` とコンストラクターの実行順・継承の多段化と `object`・継承を使う目安 |
| 29 | [型変換と型チェック](/unity-csharp-learning/csharp/type-casting/) | 基底クラスの型でまとめて扱う・変数の型と実体の型・アップキャストとダウンキャスト・`is`・`as`・型パターン |
| 30 | [protected 修飾子](/unity-csharp-learning/csharp/protected-modifier/) | `private` と `public` の間が必要な理由・`protected` と `{ get; protected set; }`・継承を重ねた場合・`internal` |
| 31 | [オーバーライドとポリモーフィズム](/unity-csharp-learning/csharp/polymorphism/) | 種類ごとの `is` 分岐の問題・`virtual`・`override`・実体の型でメソッドが決まる（動的ディスパッチ）・`base.メソッド名()`・`ToString` のオーバーライド |
| 32 | [メソッドの隠ぺいと sealed](/unity-csharp-learning/csharp/method-hiding/) | 基底クラスと同じ名前のメソッドを隠す・`new` 修飾子・`override` との違い・`sealed class`・`sealed override` |
| 33 | [抽象クラスと抽象メソッド](/unity-csharp-learning/csharp/abstract-classes/) | `virtual` では防げない誤り・`abstract class`・`abstract` メソッドとプロパティ・派生クラスでの強制実装・`virtual` との使い分け |
| 34 | [インターフェイス](/unity-csharp-learning/csharp/interfaces/) | 継承の関係がないクラスをまとめて扱う・`interface` の宣言と実装・インターフェイス型のパラメータ・複数の実装・抽象クラスとの違い |
| 35 | [インターフェイスの明示的実装](/unity-csharp-learning/csharp/explicit-interface/) | 同名メンバーの衝突・明示的実装の書き方・暗黙的実装との比較 |

### C# ジェネリクス

| # | トピック | 概要 |
|---|---|---|
| 36 | [ジェネリクスの基本](/unity-csharp-learning/csharp/generics/) | `object` による汎用化の問題・型パラメータと型引数・型引数ごとに別の型になる・複数の型パラメータ・ジェネリックインターフェイスと `IComparable<T>` |
| 37 | [ジェネリックメソッド](/unity-csharp-learning/csharp/generic-methods/) | メソッドに型パラメータを付ける・型推論と推論できない場合・クラスの型パラメータとの違いと使い分け |
| 38 | [型制約](/unity-csharp-learning/csharp/generic-constraints/) | `where T :` による制約・インターフェイスと基底クラスの制約・呼び出す側への制限・`class` / `struct` / `new()`・複数の制約と順序 |
| 39 | [共変・反変](/unity-csharp-learning/csharp/generic-variance/) | ジェネリック型が不変である理由・`out T`（共変）・`in T`（反変）・変性を付けられるもの・配列の共変 |

### C# コレクションとイテレーター

| # | トピック | 概要 |
|---|---|---|
| 40 | [List\<T\>](/unity-csharp-learning/csharp/list/) | 配列に追加しにくい理由・`Add` / `Insert` / `Remove` / `IndexOf`・`Count` と `Capacity`・`foreach` 中の変更 |
| 41 | [Dictionary\<TKey, TValue\>](/unity-csharp-learning/csharp/dictionary/) | キーで値を探す・インデクサでの追加と上書き・`TryGetValue`・`KeyValuePair` の `foreach` |
| 41.1 | [Queue\<T\>・Stack\<T\>・HashSet\<T\>（補足）](/unity-csharp-learning/csharp/other-collections/) | 先入れ先出し・後入れ先出し・重複のない集合と集合演算・コレクションの選び方 |
| 42 | [IEnumerable\<T\> と foreach の仕組み](/unity-csharp-learning/csharp/ienumerable/) | `GetEnumerator` / `MoveNext` / `Current`・自作クラスへの `IEnumerable<T>` の実装・取り出し役を毎回作る理由 |
| 43 | [イテレーターと yield return](/unity-csharp-learning/csharp/iterators/) | `yield return` / `yield break`・遅延実行・終わりのないシーケンス・ステートマシンへの置き換え |

### C# 値型と参照型

| # | トピック | 概要 |
|---|---|---|
| 44 | [値型と参照型](/unity-csharp-learning/csharp/value-reference-types/) | 代入・値渡しでコピーされるもの・`==` の意味・`null` と既定値・スタックとヒープ |
| 45 | [ガベージコレクション](/unity-csharp-learning/csharp/garbage-collection/) | 到達できないオブジェクトの回収・回収のタイミングは決まらない・世代・メモリ以外のリソース |
| 46 | [構造体](/unity-csharp-learning/csharp/structs/) | `struct` の定義・値のコピー・クラスとの使い分け・プロパティが返す構造体の罠（CS1612） |
| 47 | [構造体の制約](/unity-csharp-learning/csharp/struct-constraints/) | 継承できない・`System.ValueType`・インターフェイスの実装・コンストラクターと既定値・`readonly struct` |
| 48 | [ボクシングとアンボクシング](/unity-csharp-learning/csharp/boxing/) | `object` やインターフェイスへの変換でヒープにコピー・アンボクシングの型・ジェネリクスで避ける |
| 48.1 | [構造体のメモリレイアウト（補足）](/unity-csharp-learning/csharp/memory-layout/) | `Unsafe.SizeOf<T>()`・アラインメントとパディング・`StructLayout`（`Pack` / `Explicit`） |
| 49 | [null 許容値型](/unity-csharp-learning/csharp/nullable-value-types/) | `int?` と `Nullable<T>`・`HasValue` / `Value`・演算と比較・`??` / `??=` |
| 50 | [タプル](/unity-csharp-learning/csharp/tuples/) | 複数の値を返す・要素の名前・分解と `_`・正体は `ValueTuple` 構造体（名前はコンパイル時だけ）・`System.Tuple` との違い |
| 51 | [record](/unity-csharp-learning/csharp/records/) | 位置指定の構文とコンパイラーが作るメンバー・中身で比べる `==`・`Dictionary` / `HashSet` での利用・`with` 式・`record struct` |
| 52 | [列挙型](/unity-csharp-learning/csharp/enums/) | `enum` の定義・整数との変換・定義されていない値・`Enum.TryParse`・`[Flags]` |

### C# デリゲートとイベント

| # | トピック | 概要 |
|---|---|---|
| 53 | [デリゲートの基本](/unity-csharp-learning/csharp/delegates/) | `delegate` 型の宣言・インスタンス化・呼び出し・実行時のメソッド切り替え |
| 54 | [デリゲートの変数渡しとコールバック](/unity-csharp-learning/csharp/delegate-callback/) | デリゲートをパラメータとして渡す・コールバックパターン |
| 55 | [マルチキャストデリゲート](/unity-csharp-learning/csharp/multicast-delegates/) | `+=` / `-=` による複数メソッドの登録と解除・`GetInvocationList()` |
| 56 | [イベント](/unity-csharp-learning/csharp/events/) | `event` キーワード・発行者/購読者パターン・`EventHandler` 標準パターン |
| 57 | [ラムダ式](/unity-csharp-learning/csharp/lambda/) | `=>` 構文・式ラムダと文ラムダ・`Action` / `Func` 組み込みデリゲート型 |
| 58 | [変数キャプチャ](/unity-csharp-learning/csharp/variable-capture/) | ラムダ式によるスコープ外変数のキャプチャ・ループ内の罠・`static` ラムダ |
| 59 | [ローカル関数](/unity-csharp-learning/csharp/local-functions/) | メソッド内メソッド・再帰との相性・`static` ローカル関数・ラムダ式との使い分け |

### C# LINQ

| # | トピック | 概要 |
|---|---|---|
| 60 | [LINQ の基本](/unity-csharp-learning/csharp/linq-basics/) | ループで書く絞り込みと変換・`Where` / `Select`・`IEnumerable<T>` の拡張メソッド・メソッドチェーンと `ToList` |
| 61 | [遅延実行と即時実行](/unity-csharp-learning/csharp/linq-deferred/) | 回すまで実行されない・回すたびに実行し直される・キャプチャした変数の値・`ToList` / `Count` による即時実行 |
| 61.1 | [Where と Select を自作する（補足）](/unity-csharp-learning/csharp/linq-implementation/) | 拡張メソッドとイテレーターとデリゲートで `MyWhere` / `MySelect` を作る・遅延実行と即時実行の理由 |
| 62 | [並べ替え・集計・要素の取り出し](/unity-csharp-learning/csharp/linq-aggregation/) | `OrderBy` / `ThenBy`・`Count` / `Sum` / `Average` / `Max` / `MaxBy`・`Any` / `All`・`First` / `FirstOrDefault` / `Single`・`Take` / `Skip` / `Distinct` |
| 63 | [グループ化と結合](/unity-csharp-learning/csharp/linq-grouping/) | `GroupBy` と `IGrouping<TKey, TElement>`・`ToDictionary`・`SelectMany`・`Join`・匿名型 |
| 64 | [クエリ式](/unity-csharp-learning/csharp/linq-query/) | `from` / `where` / `select`・`orderby` / `group` / `join` / `let`・メソッド構文への置き換え・使い分け |

### C# 例外とリソース管理

| # | トピック | 概要 |
|---|---|---|
| 65 | [例外の基本](/unity-csharp-learning/csharp/exceptions/) | `try` / `catch` / `finally`・例外の型の継承関係と `catch` の順序・`TryParse` との使い分け |
| 66 | [例外を投げる](/unity-csharp-learning/csharp/throwing-exceptions/) | `throw`・呼び出し元への伝わり方とスタックトレース・`throw;` による再スロー・独自の例外クラスと `InnerException` |
| 67 | [IDisposable と using](/unity-csharp-learning/csharp/dispose-using/) | `Dispose` が必要な理由・`IDisposable` の実装・`using` 文と `try` / `finally`・`using` 宣言と解放の順序 |
| 67.1 | [イテレーターの後片付け（補足）](/unity-csharp-learning/csharp/iterator-dispose/) | `foreach` の `try` / `finally` と `Dispose`・`break` したときのイテレーターの `finally`・イテレーターの中の `using` |

### C# スレッド

| # | トピック | 概要 |
|---|---|---|
| 68 | [スレッドの基本](/unity-csharp-learning/csharp/threads/) | メインスレッド・`Thread` の `Start` / `Join`・実行の順序が決まらないこと・バックグラウンドスレッド |
| 69 | [共有データと lock](/unity-csharp-learning/csharp/thread-safety/) | 競合状態・`count++` が 1 回の操作ではないこと・`lock` 文・`Interlocked` |
| 70 | [スレッドの数と性能](/unity-csharp-learning/csharp/thread-performance/) | 論理プロセッサーの数・コンテキストスイッチ・計算するスレッドと待つスレッド・スレッドを増やしても速くならない理由 |
| 71 | [スレッドプール](/unity-csharp-learning/csharp/thread-pool/) | スレッドの使い回し・`ThreadPool.QueueUserWorkItem`・完了・結果・例外を扱いにくいという限界 |

### C# 非同期処理

| # | トピック | 概要 |
|---|---|---|
| 72 | [Task と Task\<T\>](/unity-csharp-learning/csharp/tasks/) | `Task.Run`・`Wait`・`Result`・`AggregateException`・Thread / スレッドプール / Task の比較 |
| 73 | [継続と ContinueWith](/unity-csharp-learning/csharp/task-continuation/) | 待たずに続きを登録する・継続のつなげ方・例外が深く包まれること・継続で組み立てる難しさ |
| 73.1 | [非同期処理のパターンの変遷（補足）](/unity-csharp-learning/csharp/async-patterns-history/) | APM（`BeginXxx` / `EndXxx`）・EAP（`XxxAsync` と `XxxCompleted`）・TAP（`Task` を返す `XxxAsync`） |
| 74 | [async と await](/unity-csharp-learning/csharp/async-await/) | `await` 演算子・async メソッドの定義と戻り値・`await` の正体（継続への置き換え）・元の例外が投げられること・`async void` |
| 75 | [スレッドを使わずに待つ](/unity-csharp-learning/csharp/async-without-threads/) | `Task.Run` で待つとスレッドを占有する・`Task.Delay`・`TaskCompletionSource<TResult>`・計算する処理と待つ処理 |
| 76 | [await の前後で実行されるスレッド](/unity-csharp-learning/csharp/await-threads/) | 完了済みの `Task` は中断しない・同期コンテキスト・`Wait` / `Result` によるデッドロック・`ConfigureAwait(false)` |
| 77 | [複数の Task を待つ](/unity-csharp-learning/csharp/task-whenall/) | 開始と `await` を分ける・`Task.WhenAll` と複数の例外・`Task.WhenAny` とタイムアウト |
| 78 | [キャンセル](/unity-csharp-learning/csharp/task-cancellation/) | 協調的なキャンセル・`CancellationTokenSource` / `CancellationToken`・`OperationCanceledException`・`CancelAfter` |
| 79 | [ValueTask](/unity-csharp-learning/csharp/value-task/) | async メソッドが作る `Task` オブジェクト・構造体の `ValueTask<TResult>` で割り当てを減らす・1 回だけ `await` する制約 |
| 80 | [IAsyncEnumerable と await foreach](/unity-csharp-learning/csharp/async-streams/) | `Task<List<T>>` との違い・`MoveNextAsync` と `ValueTask<bool>`・非同期イテレーター・`await foreach` の正体と後片付け |
| 81 | [IAsyncDisposable と await using](/unity-csharp-learning/csharp/async-dispose/) | `DisposeAsync`・`await using` 文と宣言・`await using` の正体・`using` との使い分け |

### C# メモリの効率化

| # | トピック | 概要 |
|---|---|---|
| 82 | [文字列の不変性と StringBuilder](/unity-csharp-learning/csharp/string-immutability/) | 文字列は変更できない・連結で新しい文字列が作られる・ループ内の `+=` と割り当ての増え方・`StringBuilder`・`+` / 文字列補間 / `string.Join` との使い分け |
| 83 | [ref ローカルと ref 戻り値](/unity-csharp-learning/csharp/ref-locals/) | 要素を取り出すとコピーになる・ref ローカルと指す先の付け替え・ref 戻り値・ローカル変数への参照を返せない理由・`ref readonly` |
| 84 | [Span\<T\> と ReadOnlySpan\<T\>](/unity-csharp-learning/csharp/span/) | 範囲演算子 `..` によるコピー・`AsSpan`・`Slice` と範囲演算子・`Span` を受け取るメソッド・`ReadOnlySpan<char>` と文字列・インデクサが返す ref |
| 85 | [ref struct と Span の制約](/unity-csharp-learning/csharp/ref-struct/) | `Span<T>` がスタックにしか置けない理由・`ref struct`・フィールド / ボクシング / 配列 / 型引数 / キャプチャ / `await` の制約・async メソッドでの使い方 |
| 86 | [stackalloc](/unity-csharp-learning/csharp/stackalloc/) | スタックに領域を確保して `Span<T>` で受け取る・メソッドから戻ると取り除かれる・領域を返せない理由・大きさと `new` との使い分け・ループ内の `stackalloc` |
| 87 | [Memory\<T\>](/unity-csharp-learning/csharp/memory/) | フィールドに保存する・`await` をまたぐ・`Span` プロパティ・`ReadOnlyMemory<T>`・`Stream.ReadAsync`・`Span<T>` との使い分け |
| 87.1 | [ArrayPool\<T\>（補足）](/unity-csharp-learning/csharp/array-pool/) | `Rent` / `Return`・求めた長さより長い配列・`try` / `finally` で返す・残っているデータと `clearArray`・`stackalloc` との組み合わせ |
| 88 | [文字列処理の割り当てを減らす](/unity-csharp-learning/csharp/string-performance/) | `Split` による解析で作られるもの・`ReadOnlySpan<char>` と `IndexOf` / `int.Parse`・中身の比較と `==` の違い・`TryFormat` / `TryWrite` / `string.Create`・方法のまとめ |

## 前提知識

このセクションはプログラミング未経験の方を対象としています。特別な前提知識は不要です。

## 学習目標

- C# の基本的な文法を理解できる
- 簡単なプログラムを自分で書けるようになる
- Unity スクリプトを読んで理解できる基礎力をつける
