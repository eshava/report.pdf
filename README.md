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
| ElementHtml | Draw a html formatted string |
| ElementHyperlink | Draw a string single/multi line and embedded a hyperlink  |
| ElementImage | Draw an image |
| ElementRectangle | Draw a rectangle (only border line) |
| ElementRectangleFill | Draw a rectangle (filled) |
| ElementLine | Draw a line |
| ElementPageNo | Draw the current and total page number |

### Element Properties
| Property | Unit | Text | Html | Image | Rectangle | Rectangle Fill | Line | Page No |
| ------ | ------ | ------ | ------ | ------ | ------ | ------ | ------ | ------ |
| Width | Point | yes | yes | yes | yes | yes | yes | yes |
| Height | Point | yes | yes | yes | yes | yes | yes | yes |
| PosX | Point | yes | yes | yes | yes | yes | yes | yes |
| PosY | Point | yes | yes | yes | yes | yes | yes | yes |
| Color | Alpha RGB, decimal | yes | yes | | yes | yes | yes | yes |
| FontFamily | string | yes | yes | | | | | yes |
| Size | Point | yes | yes | | | | | yes |
| Bold | Boolean (1,0) | yes | yes | | | | | yes |
| Italic | Boolean (1,0) | yes | yes | | | | | yes |
| Underline | Boolean (1,0) | yes | yes | | | | | yes |
| Alignment | Alignment | yes | yes | yes | | | | yes |
| VerticalAlignment | VerticalAlignment | | | yes | | | | |
| ExpandAndShift | Boolean (1,0) | yes | yes | | | | | yes |
| ShiftUpHeight | Point | yes | yes | | | | | yes |
| NoShift | Boolean (1,0) | yes | yes | | | | | yes |
| Style | DashStyle | | | | yes | yes | yes | |
| Linewidth | Point | | | | yes | yes | yes | |
| MaxHeight | Boolean (1,0) | | | | yes | yes | yes | |
| EndsDiffHeight | Boolean (1,0) | | | | yes | yes | yes | |
| SplittExtraMargin | Point | | | | | | yes | |
| SuppressOnSinglePage | Boolean (1,0) | | | | | | | yes |
| Scale | Scale | | | yes | | | | |
| Hyperlink | string | yes | | yes | | | | |

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
public enum VerticalAlignment
{
	Default = 0,
	Top = 1,
	Center = 2,
	Bottom = 3
}
```

```csharp
public enum Scale
{
	Default = 0,
	Width = 1,
	Height = 2   /* Default */,
	WidthOrHeight = 3,
	FitsWidth = 4,
	FitsHeight = 5,
	FitsWidthOrHeight = 6
}
```

### Scale Definitions
| Option | Description |
| ------ | ------ |
| Default | See Height |
| Width | Scales the image width to the maximum element width. The element height is ignored. |
| Height | Scales the image height to the maximum element height. The element width is ignored. |
| WidthOrHeight | Scales the image height or width to the maximum value without exceeding the element size. |
| FitsWidth | Reduces the image width to the maximum element width if the image width exceeds the element width. The element height is ignored. |
| FitsHeight | Reduces the image height to the maximum element height if the image height exceeds the element height. The element width is ignored. |
| FitsWidthOrHeight | Reduces the image width and/or height if they exceeds the element size. |

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

## How to use ElementText
Text blocks need a height to determine the height of the container. 
If a text block should be able to grow dynamically, the Height property must be set to 0.
If underlying elements are to be moved down, the ExpandAndShift property must also be set to 1.
If an underlying element does not move down, it does not have enough initial distance to the dynamic text block.

## How to use ElementHtml
Text blocks need a height to determine the height of the container. 
If a text block should be able to grow dynamically, the Height property must be set to 0.
If underlying elements are to be moved down, the ExpandAndShift property must also be set to 1.
If an underlying element does not move down, it does not have enough initial distance to the dynamic text block.
Element content has to be wrapped in ```<![CDATA[]]>```

Supported html tags
* b
* u
* i
* br
* p (creates a line break before and after tag content)
* ul (creates a line break before and after tag content)
* ol (creates a line break before and after tag content)
* li (creates a line break after tag content)

Supported html attributes
* font-family
* font-size
* color
* margin-left (only for li-tag; custom line indent)


## How to use ElementLine
Vertical lines that are to take up the entire position height must have the value 0 for PosY and Height. Additionally, the property MaxHeight must be set to 1.
In case a dynamic text block has to be spread over several pages, a distance to the text block can be specified for underlying horizontal lines. For this purpose the property SplittExtraMargin must be set.

## Examples
* Document in portait format 
    * document_portrait.xml 
    * use images, stantionary
* Document in landscape formart
    * document_landscape.xml
    * use images  
