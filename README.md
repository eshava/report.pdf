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



## Documentation

* Generate Pdf from Xml
* Add stationary to generated Pdf
* Prepend and append Pdfs



### Supported Elements
| Element tag | Description |
| ------ | ------ |
| ElementText | Draw a string single/multi line |
| ElementHyperlink | Draw a string single/multi line and embedded a hyperlink  |
| ElementImage | Draw an image |
| ElementRectangle | Draw a rectangle (only border line) |
| ElementRectangleFill | Draw a rectangle (filled) |
| ElementLine | Draw a line |
| ElementPageNo | Draw the current and total page number |

### Element Properties
| Property | Unit | Text | Image | Rectangle | Rectangle Fill | Line | Page No | Hyperlink |
| ------ | ------ | ------ | ------ | ------ | ------ | ------ | ------ | ------ |
| Width | Point | yes | yes | yes | yes | yes | yes | yes |
| Height | Point | yes | yes | yes | yes | yes | yes | yes |
| PosX | Point | yes | yes | yes | yes | yes | yes | yes |
| PosY | Point | yes | yes | yes | yes | yes | yes | yes |
| Color | Alpha RGB, decimal | yes | | yes | yes | yes | yes | yes |
| FontFamily | string | yes | | | | | yes | yes |
| Size | Point | yes | | | | | yes | yes |
| Bold | Boolean (1,0) | yes | | | | | yes | yes |
| Italic | Boolean (1,0) | yes | | | | | yes | yes |
| Underline | Boolean (1,0) | yes | | | | | yes | yes |
| Alignment | Alignment | yes | yes | | | | yes | yes |
| ExpandAndShift | Boolean (1,0) | yes | | | | | yes | yes |
| ShiftUpHeight | Point | yes | | | | | yes | yes |
| NoShift | Boolean (1,0) | yes | | | | | yes | yes |
| Style | DashStyle | | | yes | yes | yes | | |
| Linewidth | Point | | | yes | yes | yes | | |
| MaxHeight | Point | | | yes | yes | yes | | |
| EndsDiffHeight | Point | | | yes | yes | yes | | |
| SuppressOnSinglePage | Boolean (1,0) | | | | | | yes | |
| Scale | Scale | | yes | | | | | |

```csharp
public enum DashStyle
{
	Solid = 0,
	Dash = 1,
	Dot = 2,
	DashDot = 3,
	DashDotDot = 4,
	Custom = 5
}
```

```csharp
public enum Alignment
{
	Default = 0,
	Left = 1,
	Center = 2,
	Right = 3,
	Justify = 4
}
```

```csharp
public enum Scale
{
	Default = 0,
	Width = 1,
	Height = 2
}
```


### Position Attributes
| Attribute | Description |
| ------ | ------ |
| SequenceNo | Groups positions to an unit (important for page height calculation |
| Type | Type of the position, see enum PositonType |
| PreventLastOnPage | Specifies whether a page break is forced when the position is the last position on the page |


### Xml Frame
```xml
<?xml version="1.0" encoding="utf-8"?>
<Report>
	<Information>
		<Watermark></Watermark>
		<Stationery>{name_of_stationary}</Stationery>
		<Stationery2nd>{name_of_stationary}</Stationery2nd>
		<StationeryOnlyFirstPage>1 or 0</StationeryOnlyFirstPage>
		<Orientation>Portrait or Landscape</Orientation>
		<Reportauthor>{name_of_author}</Reportauthor>
		<Reporttitle>{report_title}</Reporttitle>
		<DocumentSize>A4</DocumentSize>
		<PageMargins Top="0" Bottom="0" Left="0" Right="0" ></PageMargins>
	</Information>
	<Header>
		<FirstPage Height="0"></FirstPage>
		<FollowingPage Height="0"></FollowingPage>
	</Header>
	<PrePositions>
		<Position></Position>
	</PrePositions>
	<Positions>
		<Position SequenceNo="0" Type="RepeatOnTop"></Position>
		<Position SequenceNo="1" Type="Default"></Position>
		<Position SequenceNo="1" Type="OnlyAsLastOnPage"></Position>
		<Position SequenceNo="2" Type="OnlyOnNewPage"></Position>
		<Position SequenceNo="2" Type="Default"></Position>
		<Position SequenceNo="2" Type="OnlyAsLastOnPage"></Position>
	</Positions>
	<PostPositions>
		<Position></Position>
	</PostPositions>
	<Footer>
		<FirstPage Height="0"></FirstPage>
		<FollowingPage Height="0"></FollowingPage>
	</Footer>
</Report>
```
```csharp
public enum PositonType
{
	Default = 0,
	Pagebreak = 2,
	RepeatOnTop = 3,
	OnlyOnNewPage = 4,
	OnlyAsLastOnPage = 5
}
```

## Examples
* Document in portait format 
    * document_portrait.xml 
    * use images, stantionary
* Document in landscape formart
    * document_landscape.xml
    * use images  
