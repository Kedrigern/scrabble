

all: prog user

prog:  prog.doc.cs.tex 
	vlna -l prog.doc.cs.tex
	pdflatex prog.doc.cs.tex 
	pdflatex prog.doc.cs.tex

user: user.doc.cs.tex
	vlna prog.doc.cs.tex
	pdflatex user.doc.cs.tex

gen:
	cd ../scripts/ && ./makeDoc.sh

clean:
	rm -f *~ *.toc *.idx *.aux *.out *.log
