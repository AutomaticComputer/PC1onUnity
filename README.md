# PC-1 on Unity

This is a simulator for the parametron computer PC-1 on Unity. 
[Japanese](README_ja.md)

I don't think it is visually faithful. 
The timing should be correct up to a frame interval. 

## On parametron computer PC-1

See: 

- [Parametron computer PC-1](https://www.iijlab.net/~ew/pc1/) 
by Dr. Eiiti Wada. 
- [A blog entry](http://parametron.blogspot.com/search/label/PC-1%E3%82%B7%E3%83%9F%E3%83%A5%E3%83%AC%E3%83%BC%E3%82%BF) 
by Dr. Wada.
- [PC-1 Parametron computer](http://museum.ipsj.or.jp/computer/dawn/0016.html)
from Computer Museum

## How to use it

Download the [Windows binary](PC1onUnity_Windows.zip), 
build it yourself, 
or try the
[WebGL version](https://automaticcomputer.github.io/PC1onUnity/PC1onUnity_WebGL/index.html). 
On Unity Editor, 
you can also see the contents of the registers and main stores. 

To move the viewpoint, use arrow keys and PgUp, PgDn. 

For a demonstration, 
I prepared a program to draw the Mandelblot set 
(which wasn't known in 1964). 
To try it: 

- Press "LOAD" in the upper left corner of the teleprinter, 
and choose "mandelblot.ptw."
- Press "INITIAL LOAD" on the computer. 
- When the loading is done, 
switch on "FREE RUN" and press "CLEAR START". 
(Or with "FREE RUN" off, press "CLEAR START" 
and then "RESTART" to continue.)

You can also prepare a tape for "initial load" using the teleprinter 
(though it takes labor). 
See [mandelblot.ptw](Assets/Tapes/mandelblot.ptw.txt).

Usually people wrote a program in a kind of assembly language
(for an example, print "mandelblot.ptr" on the teleprinter), 
and loaded it with the initial orders "R0". 
I checked, using 
[R0](https://www.iijlab.net/~ew/pc1/R0.html), 
that "Factorization Program," "1000 digits of e", 
and "Lucas Lehmer Test" works. 


## Acknowledgments

The documents by Dr. Wada and the book 
"Parametron computer" (in Japanese, Iwanami, edited by Hidetoshi Takahashi)
were essential in developing the simulator. 


## Related

[Ferranti Sirius on Unity](https://github.com/AutomaticComputer/SiriusOnUnity)
