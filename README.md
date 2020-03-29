# report.pdf
A library to create pdf from xml. Based on PdfSharp


## report.pdf.core
Platform independent core to calculate and generate pdf documents


## report.pdf.netcore
.Net Core dependent implementation of the report library	

* PdfSharpCore
* SixLabors.Fonts
* SixLabors.ImageSharp


## report.pdf.netframework
.Net Framework dependent implementation of the report library
	
* PdfSharp (V 1.32.3057)
	* Version used GDI to measure strings
	* Higher versions no longer use GDI to measure strings
	* The pull request for multiline string measure support is not yet closed
* System.Drawing.Image