# PC-1 on Unity

This is a simulator for the parametron computer PC-1 on Unity. 
[Japanese](README_ja.md)
You can try its 
[WebGL version](https://automaticcomputer.github.io/PC1onUnity/webgl.html) on a web browser. 
I don't think it is visually faithful. 
The timing should be correct up to a frame interval. 

  - [On parametron computer PC-1](#on-parametron-computer-pc-1)
  - [How to use it](#how-to-use-it)
  - [Unimplemented features](#unimplemented-features)
  - [Acknowledgments](#acknowledgments)
  - [Related](#related)

New! (2023.3.26) 
Added functionality to read a text file as a paper tape, 
and to rename and delete paper tapes. 
On WebGL version, added functionality to upload and download tapes. 
Some bug fixes. 
The size of the main unit is now a little closer to the real one. 

(2021.4.18)
Now you can move and resize using the touch screen. 

(2021.4.18)
Sound(only on PC and Mac), 
according to [計算機による音楽演奏](http://parametron.blogspot.com/search/label/%E8%A8%88%E7%AE%97%E6%A9%9F%E3%81%AB%E3%82%88%E3%82%8B%E9%9F%B3%E6%A5%BD%E6%BC%94%E5%A5%8F) by Dr. Wada. 
See "soundtest.ptr" for an example. 
You can switch on/off the sound with "SPEAKER" button. 
When something goes wrong, switching off/on may fix it. 


(2021.3.24) 
[1000 digits of pi](https://automaticcomputer.github.io/PC1onUnity/pi.html).

## On parametron computer PC-1

See: 

- [Parametron computer PC-1](https://www.iijlab.net/~ew/pc1/) 
by Dr. Eiiti Wada. 
- [A blog entry](http://parametron.blogspot.com/search/label/PC-1%E3%82%B7%E3%83%9F%E3%83%A5%E3%83%AC%E3%83%BC%E3%82%BF) 
by Dr. Wada.
- [PC-1 Parametron computer](http://museum.ipsj.or.jp/computer/dawn/0016.html)
from Computer Museum.

## How to use it

Download the [Windows binary](PC1onUnity_Windows.zip), 
build it yourself, 
or try the
[WebGL version](https://automaticcomputer.github.io/PC1onUnity/webgl.html). 
On Unity Editor, 
you can also see the contents of the registers and main stores. 

I also built a [MacOSX binary](PC1onUnity_MacOSX.app.zip), 
but I haven't tried it yet. 
Hopefully, it works after unzipping. 

To move the viewpoint, use arrow keys and PgUp, PgDn. 
You can also use the touch panel. 

Usually people wrote a program in a kind of assembly language 
and loaded it with the initial orders "R0". 

In this simulator, 
[R0](https://www.iijlab.net/~ew/pc1/R0.html) 
and 
[1000 digits of e](https://www.iijlab.net/~ew/pc1/e1000.html) are included. 
To try them: 
- Press "LOAD" in the upper left corner of the teleprinter, 
and choose "R0.ptw."
- Press "INITIAL LOAD" on the computer. 
- When the loading is done, load "napier.ptr" on the teleprinter, 
switch on "FREE RUN" and press "CLEAR START". 

After a while, the first 1000 digits of e is printed. 

"Factorization Program" and "Lucas Lehmer Test" also worked. 

For another demonstration, 
I prepared a program to draw the Mandelblot set 
(which wasn't known in 1964), 
inspired by 
[Mandelblot set on IBM 1401](http://www.righto.com/2015/03/12-minute-mandelbrot-fractals-on-50.html)
by Ken Shirriff. 
Load "mandelblot.ptw" in the same way as "R0". 
(To execute in steps, 
switch off "FREE RUN", press "CLEAR START" 
and then "RESTART" to continue.)

mandelblot.ptr works similarly but is read in with R0. 

I also tried 
[1000 digits of pi](https://automaticcomputer.github.io/PC1onUnity/pi.html).

You can also prepare a tape for "initial load" using the teleprinter 
(though it takes labor). 
See [R0.ptw](Assets/Tapes/R0.ptw.txt).

## Unimplemented features

The "E" order, "Y" order and the break-point feature 
are not yet implemented. 

## Acknowledgments

The documents by Dr. Wada and the book 
"Parametron computer" (in Japanese, Iwanami, edited by Hidetosi Takahasi)
were essential in developing this simulator. 

The files R0.ptw, R0.ptr were typed from 
[R0](https://www.iijlab.net/~ew/pc1/R0.html), 
[1000 digits of e](https://www.iijlab.net/~ew/pc1/e1000.html). 
I thank Dr. Wada for permission. 

(Ver. 0.2.3) 
The behavior of Initial loader and the tape format 
were corrected in accordance with Dr. Wada's comments. 

(Ver. 0.2.5)
The output routine "pi_1000_3.ptw" 
is a modified version of that of "e1000". 


## Related

[Ferranti Sirius on Unity](https://github.com/AutomaticComputer/SiriusOnUnity)
