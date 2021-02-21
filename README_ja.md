# PC-1 on Unity

Unity 上で動く、パラメトロン計算機 PC-1 のシミュレーターです。
見た目はあまり実物に忠実ではないと思います。
速度はフレーム単位で正確になるよう努めました。

## パラメトロン計算機 PC-1 について

以下をご覧下さい。

- 和田英一先生の[パラメトロン計算機 PC-1](https://www.iijlab.net/~ew/pc1/)
- 和田先生の[ブログエントリー](http://parametron.blogspot.com/search/label/PC-1%E3%82%B7%E3%83%9F%E3%83%A5%E3%83%AC%E3%83%BC%E3%82%BF)
- コンピュータ博物館[PC-1 パラメトロン式計算機](http://museum.ipsj.or.jp/computer/dawn/0016.html)

## 使用法

[Windows 用バイナリ](PC1onUnity_Windows.zip)をダウンロードするか、
ビルドするか、[WebGL版](https://automaticcomputer.github.io/PC1onUnity/PC1onUnity_WebGL/index.html)を試してみてください。
Unity Editor 上であれば、Inspector でレジスターやメモリの内容を
見ることができます。

[MacOSX バイナリ](PC1onUnity_MacOSX.app.zip)も
ビルドしてみましたが、自分では試していません。
Unzip して実行してみてください。

矢印キーとPgUp, PgDn で視点の移動ができます。
より詳しい操作マニュアルはこちら: [マニュアル](https://automaticcomputer.github.io/PC1onUnity/manual.html)

通常、PC-1 のプログラムは、
一種のアセンブリ言語のようなもので書き、
イニシャルオーダー "R0" でプログラムを読み込みました。
このシミュレータでは、
和田先生の書かれた[R0(単独版)](https://www.iijlab.net/~ew/pc1/R0.html), 
[e1000桁の計算](https://www.iijlab.net/~ew/pc1/e1000.html)
を同梱しています。
使い方は以下の通りです。

- テレプリンターの左の方の "LOAD" を押し、"R0.ptw" を選ぶ。
- 本体の "INITIAL LOAD" を押す。
- 読み込みが終わったら、テレプリンターに "napier.ptr" をセットし、
"FREE RUN" を on にして、"CLEAR START" を押す。

数分間待つと、e 1000 桁が印字されます。
この他、Factorization Program, Lucas Lehmer Test が
動作することを確認しました。

もう一つ、デモとして Mandelblot 集合(1964 年には発見されていませんでしたが…)
の描画プログラムを用意しました。
R0 と同様に "mandelblot.ptw" を読み込んで実行してください。
(ステップ動作させたい場合は、
"FREE RUN" を on にしないで "CLEAR START" を押し、
その後は "RESTART" を押してゆく。)
mandelblot.ptr は同様のプログラムですが R0 で読み込むことができます。

テレプリンタで initial load 用のテープを作成することも
(かなりの手間ですが)できます。
最初に 0(blank) または 63(backspace) を(好きなだけ)打ち、
それ以外の文字を一つ打った後、
データを長語ごとに「上から 6 bit ずつ 6 文字 + 区切りの文字」
の形で打ちます。
区切りの文字は、最後以外は 63 以外の任意の文字、
最後の区切りは 63 を打ちます。
([R0.ptw](Assets/Tapes/R0.ptw.txt) が参考になるでしょう。)


## 未実装機能

"E" 命令、"Y" 命令およびブレークポイント機能は未実装です。


## 謝辞

作成にあたっては、和田先生のページの各種資料および
高橋秀俊編「パラメトロン計算機」(岩波書店)に大変お世話になりました。

このディレクトリ下にあるファイル R0.ptw, R0.ptr は
和田先生の作られた[R0(単独版)](https://www.iijlab.net/~ew/pc1/R0.html), 
[e1000桁の計算](https://www.iijlab.net/~ew/pc1/e1000.html)
を打ち込んだものです。
掲載を許可くださった和田先生に感謝いたします。

(Ver. 0.2.3) 
Initial loader の動作・テープフォーマットを、
和田先生にコメントをいただいて修正しました。


## 関連

[Ferranti Sirius on Unity](https://github.com/AutomaticComputer/SiriusOnUnity)
