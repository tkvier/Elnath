# Elnath
**Auriga用フロントエンド**  

このアプリケーションはRagnarok Online エミュレーターサーバー「[Auriga](https://github.com/auriga/auriga)」の運用を効率化するためのフロントエンドツールです  
CUIベースの複数の管理ウィンドウを統合し GUIで一括操作できるように設計されています  

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
1. [リリースページ](https://github.com/tkvier/Elnath/releases) から実行ファイルをダウンロードしてください  
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

本アプリケーションは **GNU General Public License v3.0（GPL-3.0）** の下で提供されます。  
このライセンスは、ユーザーに自由な使用・改変・再配布の権利を認めつつ、ソフトウェアの自由を守ることを目的としています。

### 利用・改変・再配布について
- 本アプリケーションは、商用・非商用を問わず自由に使用できます  
- ソースコードを改変し、改変版を配布することができます  
- 改変の有無にかかわらず、再配布する際には**GPL-3.0ライセンスの下で配布**する必要があります  
- 再配布時には、**ソースコードを同時に提供するか、入手方法を明示する必要**があります

### 免責事項
- 本アプリケーションは「現状のまま」提供され、**いかなる保証もありません**  
- 利用者または配布者が本アプリケーションを使用したことによる損害について、作者は一切の責任を負いません

### 法的留意事項
- GPLライセンスは、著作権法に基づいて適用されます  
- ライセンスに違反した場合、著作権侵害として法的責任を問われる可能性があります  
- 作者を偽って配布することは、GPLの精神に反するだけでなく、詐欺行為として法的責任を問われる可能性があります

---

## License

This application is licensed under the **GNU General Public License v3.0 (GPL-3.0)**.  
This license grants users the freedom to use, modify, and redistribute the software, while ensuring that those freedoms are preserved.

### Use, Modification, and Redistribution
- You may use this application freely for any purpose, including commercial and non-commercial use  
- You may modify the source code and distribute your modified versions  
- Whether modified or not, any redistribution must be under the **same GPL-3.0 license**  
- You must also **provide the source code** or indicate how it can be obtained when redistributing

### Disclaimer
- This application is provided **"as is"**, without any warranty  
- The author shall not be held liable for any damages arising from the use or distribution of this application

### Legal Notice
- The GPL license operates under copyright law  
- Violating the license terms may result in legal consequences for copyright infringement  
- Misrepresenting the authorship of this application may constitute fraud under applicable laws
