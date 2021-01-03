# PC-1 on Unity

Unity 上で動く、パラメトロン計算機 PC-1 のシミュレーターです。

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

デモとして Mandelblot 集合(1964 年には発見されていませんでしたが…)
の描画プログラムを用意しました。
使い方は以下の通りです。

- テレプリンターの左の方の "LOAD" を押し、"mandelblot.ptw" を選ぶ。
- 本体の "INITIAL LOAD" を押す。
- 読み込みが終わったら、"FREE RUN" を on にして、"CLEAR START" を押す。
(あるいは、"FREE RUN" を on にしないで "CLEAR START" を押し、
その後は "RESTART" を押してゆく。)

テレプリンタで initial load 用のテープを作成することも
(かなりの手間ですが)できます。
長語ごとに「下から 6 bit ずつ 6 文字 + 区切りの 0(runout)」をテープに打ちます。
ただし、最後の区切りは 63(backspace)を打ちます。
([mandelblot.ptw](Assets/Tapes/mandelblot.ptw.txt) が参考になるでしょう。)

通常はアセンブリ言語のようなものでプログラムを書き、
イニシャルオーダー "R0" でプログラムを読み込みます。
和田先生のページの[R0(単独版)](https://www.iijlab.net/~ew/pc1/R0.html)
を利用して、
Factorization Program, e1000桁の計算、Lucas Lehmer Test が
動作することを確認しました。

## 関連

[Ferranti Sirius on Unity](https://github.com/AutomaticComputer/SiriusOnUnity)
