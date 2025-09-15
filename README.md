# Elnath
**Auriga用フロントエンド**  
このプログラムはRagnarok online エミュレーターサーバーのAuriga用フロントエンドアプリケーションです  
このアプリを使用することでAurigaの複数のCUIウィンドウを開かずに管理することができます  
.net9.0 + Avaroniaを使用しているのでxxx-server.exeをLinux用に書き換えれば動くかもしれません  
基本的に自分用に作ってますので、改造改良要望などはフォークしてから自分で行ってください

このアプリケーションは Ragnarok Online エミュレーターサーバー「Auriga」の管理を効率化するためのフロントエンドです  
複数のCUIベースの管理ウィンドウを統合し GUIで一括操作できるように設計されています  

開発には .NET 9.0 と Avaronia Framework を使用  
サーバー名をLinux向けに書き換えてビルドし直せば Linuxでの動作も可能かもしれません

本ツールは個人用途を前提に作成しています  
機能追加やカスタマイズを希望される場合は フォークしてご自身で改修をお願いします

---

## 開発環境

開発環境：Windows11Pro + Visual Studio Code  
開発言語：.net9.0 + Avaronia 11.3.3  
使用AI：Gemini CLI(Gemini2.5Pro)/Copilot/StableDiffusionXL/LamaCleaner  
画像生成モデル：ARIA-i3497  

---

## 📥 インストール方法

### Windows 実行ファイル
1. [リリースページ](/releases) から実行ファイルをダウンロードしてください  
   - 環境によっては .NET ランタイムのインストールが必要になる場合があります  
   - [.NET 公式サイト](https://dotnet.microsoft.com/ja-jp/download/dotnet/9.0) からダウンロードしてください

### ソースコードからビルド
1. リポジトリをクローンまたはダウンロードします  
2. 任意の .NET 対応 IDE（例: Visual Studio, Rider）でプロジェクトを開きます  
3. ビルドして実行ファイルを生成してください

---

## 🚀 使い方

### 基本設定

1. アプリを起動すると、以下のような画面が表示されます  

   <img src="https://tkvier.github.io/resource/Elnath/ss01.jpg" alt="アプリ画面">

2. **Auriga**サーバーのフォルダパスを設定します  
パスを設定するとサーバー起動ボタンが有効になります
   - **サーバー起動**ボタンを押すとログインサーバー、キャラクターサーバー、マップサーバーが起動します  
マップサーバーの起動処理に時間がかかってフリーズみたいになりますが、処理が終われば落ち着きます  
   - **サーバー停止**ボタンを押すと3つのサーバー全て終了します

   <img src="https://tkvier.github.io/resource/Elnath/ss02.jpg" alt="Aurigaフォルダパス入力">

3. **Ragexe.exe** へのファイルパスを設定します  
Ragexe.exeのファイル名は実行ファイルであればほかの名前でも構いません  
パスを設定するとラグナロクの**クライアント起動**ボタンと**クライアント設定**ボタンが有効になります  
   - **クライアント起動**ボタンを押すと設定した実行ファイルを実行します
   - **クライアント設定**ボタンを押すとRagnarokの設定アプリが起動します(setup.exe)  

   <img src="https://tkvier.github.io/resource/Elnath/ss03.jpg" alt="Ragexe.exeパス入力">

### サーバー使用方法

1. サーバーを起動すると各サーバーのログが表示されます  

   <img src="https://tkvier.github.io/resource/Elnath/ss04.jpg" alt="サーバー起動">

2. 各サーバーの下部には**再起動**ボタンと**終了**ボタンがあります 
   - **再起動**ボタンを押すとそのサーバーを再起動します  
   - **終了**ボタンを押すとそのサーバーを停止します

---

## 📄 ライセンス

本アプリケーションは **CC0 1.0 Universal（パブリックドメイン）** ライセンスの下で提供されます  
著作権を放棄しており、以下の条件のもとで自由にご利用いただけます

### 利用・改変・再配布について
- 本アプリケーションは、商用・非商用を問わず自由に使用できます  
- 自由に改造し、改造版を配布することができます  
- 無改造での再配布も可能です。ただし、その場合も「あなたが配布したもの」とみなされます  
- 配布物のライセンスを変更することも自由です。あなたが配布するものであるため、任意に設定してください

### 免責事項
- 本アプリケーションの利用・配布に伴う一切の責任は、利用者および配布者が負うものとします  
- あなたが第三者に渡した場合、それはあなたが提供したものであり、作者は一切関与しません  

### 法的留意事項
- ライセンスで許可される行為と、法的に許される行為は別問題です  
- 例えば、作者を偽って再配布することはライセンス上は制限されませんが、詐欺行為として法的責任を問われる可能性があります

## License

This application is provided under the **CC0 1.0 Universal (Public Domain Dedication)**.  
The copyright has been waived, and you are free to use it under the following conditions.

### Use, Modification, and Redistribution
- You may use this application freely for any purpose, including commercial and non-commercial use.  
- You may modify it and distribute your modified versions.  
- You may redistribute it without modification. However, in such cases, it will still be considered *your* distribution.  
- You may change the license for your distribution. Since it is your distribution, you are free to choose the terms.

### Disclaimer
- All responsibility for the use or distribution of this application lies with the user or distributor.  
- If you provide this application to a third party, it is considered to have been provided by you, and the original author has no involvement whatsoever.

### Legal Notice
- What is permitted under this license and what is legally permissible are separate matters.  
- For example, redistributing this application while falsely claiming to be the original author is not restricted by the license itself, but it may constitute fraud under applicable laws.