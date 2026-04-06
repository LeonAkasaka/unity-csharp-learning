# Unity & C# 学習教材

Unity と C# の基礎を学ぶための日本語教材サイトです。

🌐 **サイト**: https://LeonAkasaka.github.io/unity-csharp-learning/

## 対象読者

- プログラミングが初めての方
- C# や Unity でゲーム開発を始めたい方

## コンテンツ構成

```
docs/
  index.md      # トップページ
  csharp/       # C# 基礎
  unity/        # Unity 基礎（Unity UI サブセクションを含む）
  hands-on/     # ハンズオン
  grid-games/   # グリッドゲーム
```

## 動作環境

- Unity 6 以降
- Visual Studio 2026 または Visual Studio Code

## ローカルでのプレビュー

Docker を使って Jekyll をインストールせずにプレビューできます。

**前提条件：** Docker Desktop が起動していること

```bat
preview.bat
```

ブラウザで http://localhost:4000/unity-csharp-learning/ を開いてください。

- ファイルを保存すると自動で再ビルドされます（`--watch`）
- 停止は `Ctrl+C`、コンテナは自動削除されます（`--rm`）

## ライセンス

[LICENSE](./LICENSE) を参照してください。
