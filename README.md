# Unity & C# 学習教材

Unity と C# の基礎を学ぶための日本語教材サイトです。

🌐 **サイト**: https://LeonAkasaka.github.io/unity-csharp-learning/

## 対象読者

- プログラミングが初めての方
- C# や Unity でゲーム開発を始めたい方

## コンテンツ構成

```
docs/
  index.md          # トップページ
  csharp/           # C# 基礎
    index.md
    variables/      # 1ページ = 1フォルダー（関連アセットも同フォルダー内）
  unity/            # Unity 基礎
    index.md
samples/
  csharp/           # C# サンプルコード
  unity/            # Unity サンプルスクリプト
```

## 動作環境

- Unity 6 以降
- Visual Studio 2022 または Visual Studio Code

## ローカルでのプレビュー

GitHub Pages の組み込みビルドを使用しています。ローカルでプレビューする場合は Jekyll をインストールしてください。

```bash
gem install jekyll bundler
cd docs
jekyll serve
# http://localhost:4000/unity-csharp-learning/ でプレビュー
```

## ライセンス

[LICENSE](./LICENSE) を参照してください。
