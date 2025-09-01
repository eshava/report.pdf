using System;
using System.Linq;
using System.Text;
using Eshava.Report.Pdf.Core;
using Eshava.Report.Pdf.Core.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eshava.Test.Report.Pdf.Core
{
	[TestClass, TestCategory("Eshava.Report.Pdf.Core")]
	public class HtmlInterpreterTests
	{
		private HtmlInterpreter _classUnderTest;

		[TestInitialize]
		public void Setup()
		{
			_classUnderTest = new HtmlInterpreter();
		}

		[TestMethod]
		public void AnalyzeTextTest()
		{
			var font = new Font
			{
				Color = "#000000",
				Size = 12.0,
				Bold = false,
				Underline = false,
				Italic = false,
				Fontfamily = "Arial"
			};
			var html = $@"very you get up speedily if Off supposing moment <b>directly part mirth <i>newspaper own come as its </i> </b> who greatest but 
Dashwood great children <u>message</u> are you perceived <p>indeed they entreaties disposed announcing views On come On or as early 
suitable Entered questions middletons warmth</p> true be drawings Melancholy on can mr because an thus 
at dwelling weeks its though pleasure may <b>good <span style=""font-family: Verdana, Helvetica; font-size: 14; color: #202020;"">within up</span> bed</b> 
Preferred leave bred since Written Visited<br> 
through joy simplicity so half ask Chicken expense old wife tended hence strangers terminated ye check Admitting entire<br> 
come about partiality Prepared on <b style=""color: #ff0000"">when</b> wish where we held steepest True he questions eat Thoughts pure some apartments steepest such why
";


			var result = _classUnderTest.AnalyzeText(font, html).ToList();

			result.Should().HaveCount(20);

			var index = 0;
			result[index].Text.Should().Be("very you get up speedily if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");

			index++;
			result[index].Text.Should().Be(" directly part mirth");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeTrue();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");

			index++;
			result[index].Text.Should().Be(" newspaper own come as its");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeTrue();
			result[index].Font.Italic.Should().BeTrue();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");

			index++;
			result[index].Text.Should().Be(" who greatest but Dashwood great children");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");

			index++;
			result[index].Text.Should().Be(" message");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeTrue();
			result[index].Font.Color.Should().Be("255 0 0 0");

			index++;
			result[index].Text.Should().Be(" are you perceived ");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");

			index++;
			result[index].Text.Should().Be("indeed they entreaties disposed announcing views On come On or as early suitable Entered questions middletons warmth");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");

			index++;
			result[index].Text.Should().Be(" true be drawings Melancholy on can mr because an thus at dwelling weeks its though pleasure may");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");

			index++;
			result[index].Text.Should().Be(" good");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeTrue();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");

			index++;
			result[index].Text.Should().Be(" within up");
			result[index].Font.Fontfamily.Should().Be("Verdana");
			result[index].Font.Size.Should().Be(14.0);
			result[index].Font.Bold.Should().BeTrue();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 32 32 32");

			index++;
			result[index].Text.Should().Be(" bed");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeTrue();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");

			index++;
			result[index].Text.Should().Be(" Preferred leave bred since Written Visited");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");

			index++;
			result[index].Text.Should().Be("through joy simplicity so half ask Chicken expense old wife tended hence strangers terminated ye check Admitting entire");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");

			index++;
			result[index].Text.Should().Be("come about partiality Prepared on");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");

			index++;
			result[index].Text.Should().Be(" when");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeTrue();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 255 0 0");

			index++;
			result[index].Text.Should().Be(" wish where we held steepest True he questions eat Thoughts pure some apartments steepest such why");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
		}

		[TestMethod]
		public void AnalyzeTextStartsAndEndsWithPTagTest()
		{
			var font = new Font
			{
				Color = "#000000",
				Size = 12.0,
				Bold = false,
				Underline = false,
				Italic = false,
				Fontfamily = "Arial"
			};
			var html = $@"<p>very you get up speedily if Off supposing moment</p>
<p>directly part mirth</p>";


			var result = _classUnderTest.AnalyzeText(font, html).ToList();

			result.Should().HaveCount(3);

			var index = 0;
			result[index].Text.Should().Be("very you get up speedily if Off supposing moment");
			
			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			
			index++;
			result[index].Text.Should().Be("directly part mirth");
		}

		[TestMethod]
		public void AnalyzeTextAttributesTest()
		{
			var font = new Font
			{
				Color = "#000000",
				Size = 12.0,
				Bold = false,
				Underline = false,
				Italic = false,
				Fontfamily = "Arial"
			};
			var html = $@"<span style=""font-family: Verdana, Helvetica; font-size: 14; color: #202020;"">very you get up speedily if Off supposing moment</span>";


			var result = _classUnderTest.AnalyzeText(font, html).ToList();

			result.Should().HaveCount(1);

			var index = 0;
			result[index].Text.Should().Be("very you get up speedily if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Verdana");
			result[index].Font.Size.Should().Be(14.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 32 32 32");
		}

		[TestMethod]
		public void AnalyzeTextFontTagsTest()
		{
			var font = new Font
			{
				Color = "#000000",
				Size = 12.0,
				Bold = false,
				Underline = false,
				Italic = false,
				Fontfamily = "Arial"
			};
			var html = $@"<b>very you get up speedily </b>
<b>if Off supposing moment</b>
<i>directly part mirth</i>
<u>newspaper own come as its</u>";


			var result = _classUnderTest.AnalyzeText(font, html).ToList();

			result.Should().HaveCount(3);

			var index = 0;
			result[index].Text.Should().Be("very you get up speedily if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeTrue();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");

			index++;
			result[index].Text.Should().Be("directly part mirth");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeTrue();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");

			index++;
			result[index].Text.Should().Be("newspaper own come as its");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeTrue();
			result[index].Font.Color.Should().Be("255 0 0 0");
		}

		[TestMethod]
		public void AnalyzeTextListTagsTest()
		{
			var font = new Font
			{
				Color = "#000000",
				Size = 12.0,
				Bold = false,
				Underline = false,
				Italic = false,
				Fontfamily = "Arial"
			};
			var html = $@"<ul>
<li>very you get up speedily</li>
<li>if Off supposing moment</li>
</ul>";


			var result = _classUnderTest.AnalyzeText(font, html).ToList();

			result.Should().HaveCount(5);

			var index = 0;
			result[index].Text.Should().Be("-");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);
			result[index].SkipParagraphAlignment.Should().BeTrue();
			result[index].ReduceLineIndent.Should().BeTrue();
			result[index].ReduceLineIndentByText.Should().Be("- ");

			index++;
			result[index].Text.Should().Be(" very you get up speedily");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("-");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);
			result[index].SkipParagraphAlignment.Should().BeTrue();
			result[index].ReduceLineIndent.Should().BeTrue();
			result[index].ReduceLineIndentByText.Should().Be("- ");

			index++;
			result[index].Text.Should().Be(" if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);
		}

		[TestMethod]
		public void AnalyzeTextNumericListTagsTest()
		{
			var font = new Font
			{
				Color = "#000000",
				Size = 12.0,
				Bold = false,
				Underline = false,
				Italic = false,
				Fontfamily = "Arial"
			};
			var html = $@"<ol>
<li style=""margin-left: 10;"">very you get up speedily</li>
<li>if Off supposing moment</li>
<li>very you get up speedily</li>
</ol>";


			var result = _classUnderTest.AnalyzeText(font, html).ToList();

			result.Should().HaveCount(8);

			var index = 0;
			result[index].Text.Should().Be("1.");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(10);
			result[index].SkipParagraphAlignment.Should().BeTrue();
			result[index].ReduceLineIndent.Should().BeTrue();
			result[index].ReduceLineIndentByText.Should().Be("1. ");

			index++;
			result[index].Text.Should().Be(" very you get up speedily");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(10);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("2.");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 1.5);
			result[index].SkipParagraphAlignment.Should().BeTrue();
			result[index].ReduceLineIndent.Should().BeTrue();
			result[index].ReduceLineIndentByText.Should().Be("2. ");

			index++;
			result[index].Text.Should().Be(" if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 1.5);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("3.");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 1.5);
			result[index].SkipParagraphAlignment.Should().BeTrue();
			result[index].ReduceLineIndent.Should().BeTrue();
			result[index].ReduceLineIndentByText.Should().Be("3. ");

			index++;
			result[index].Text.Should().Be(" very you get up speedily");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 1.5);
		}

		[TestMethod]
		public void AnalyzeTextEmbeddedListTagsTest()
		{
			var font = new Font
			{
				Color = "#000000",
				Size = 12.0,
				Bold = false,
				Underline = false,
				Italic = false,
				Fontfamily = "Arial"
			};
			var html = $@"very you get up speedily if Off supposing moment<br>
<ul>
<li>very you <b>get</b> up speedily</li>
<li>if Off supposing moment</li>
</ul><br>
wish where we held steepest True he questions eat Thoughts";


			var result = _classUnderTest.AnalyzeText(font, html).ToList();

			result.Should().HaveCount(13);
			
			var index = 0;
			result[index].Text.Should().Be("very you get up speedily if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("-");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);
			result[index].SkipParagraphAlignment.Should().BeTrue();
			result[index].ReduceLineIndent.Should().BeTrue();
			result[index].ReduceLineIndentByText.Should().Be("- ");

			index++;
			result[index].Text.Should().Be(" very you");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);

			index++;
			result[index].Text.Should().Be(" get");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeTrue();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);

			index++;
			result[index].Text.Should().Be(" up speedily");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("-");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);
			result[index].SkipParagraphAlignment.Should().BeTrue();
			result[index].ReduceLineIndent.Should().BeTrue();
			result[index].ReduceLineIndentByText.Should().Be("- ");

			index++;
			result[index].Text.Should().Be(" if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("wish where we held steepest True he questions eat Thoughts");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);
		}

		[TestMethod]
		public void AnalyzeTextTwoEmbeddedListTagsTest()
		{
			var font = new Font
			{
				Color = "#000000",
				Size = 12.0,
				Bold = false,
				Underline = false,
				Italic = false,
				Fontfamily = "Arial"
			};
			var html = $"""
				<span>very you get up speedily if Off supposing moment</span>
				<ul>
					<li>very you get up speedily</li>
					<li>if Off supposing moment</li>
				</ul>
				<ol>
					<li>very you get up speedily</li>
					<li>if Off supposing moment</li>
				</ol>
				""";


			var result = _classUnderTest.AnalyzeText(font, html).ToList();

			result.Should().HaveCount(13);

			var index = 0;
			result[index].Text.Should().Be("very you get up speedily if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("-");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);
			result[index].SkipParagraphAlignment.Should().BeTrue();
			result[index].ReduceLineIndent.Should().BeTrue();
			result[index].ReduceLineIndentByText.Should().Be("- ");

			index++;
			result[index].Text.Should().Be(" very you get up speedily");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("-");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);
			result[index].SkipParagraphAlignment.Should().BeTrue();
			result[index].ReduceLineIndent.Should().BeTrue();
			result[index].ReduceLineIndentByText.Should().Be("- ");

			index++;
			result[index].Text.Should().Be(" if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("1.");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 1.5);
			result[index].SkipParagraphAlignment.Should().BeTrue();
			result[index].ReduceLineIndent.Should().BeTrue();
			result[index].ReduceLineIndentByText.Should().Be("1. ");

			index++;
			result[index].Text.Should().Be(" very you get up speedily");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 1.5);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("2.");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 1.5);
			result[index].SkipParagraphAlignment.Should().BeTrue();
			result[index].ReduceLineIndent.Should().BeTrue();
			result[index].ReduceLineIndentByText.Should().Be("2. ");

			index++;
			result[index].Text.Should().Be(" if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 1.5);
		}

		[TestMethod]
		public void AnalyzeTextTwoEmbeddedListTagsWithPTagTest()
		{
			var font = new Font
			{
				Color = "#000000",
				Size = 12.0,
				Bold = false,
				Underline = false,
				Italic = false,
				Fontfamily = "Arial"
			};
			var html = $"""
				<p>very you get up speedily if Off supposing moment</p>
				<p>very you get up speedily if Off supposing moment</p>
				<ul>
					<li>very you get up speedily</li>
					<li>if Off supposing moment</li>
				</ul>
				<ol>
					<li>very you get up speedily</li>
					<li>if Off supposing moment</li>
				</ol>
				<p>very you get up speedily if Off supposing moment</p>
				<p>very you get up speedily if Off supposing moment</p>
				""";


			var result = _classUnderTest.AnalyzeText(font, html).ToList();

			result.Should().HaveCount(19);

			var index = 0;
			result[index].Text.Should().Be("very you get up speedily if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("very you get up speedily if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("-");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);
			result[index].SkipParagraphAlignment.Should().BeTrue();
			result[index].ReduceLineIndent.Should().BeTrue();
			result[index].ReduceLineIndentByText.Should().Be("- ");

			index++;
			result[index].Text.Should().Be(" very you get up speedily");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("-");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);
			result[index].SkipParagraphAlignment.Should().BeTrue();
			result[index].ReduceLineIndent.Should().BeTrue();
			result[index].ReduceLineIndentByText.Should().Be("- ");

			index++;
			result[index].Text.Should().Be(" if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("1.");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 1.5);
			result[index].SkipParagraphAlignment.Should().BeTrue();
			result[index].ReduceLineIndent.Should().BeTrue();
			result[index].ReduceLineIndentByText.Should().Be("1. ");

			index++;
			result[index].Text.Should().Be(" very you get up speedily");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 1.5);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("2.");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 1.5);
			result[index].SkipParagraphAlignment.Should().BeTrue();
			result[index].ReduceLineIndent.Should().BeTrue();
			result[index].ReduceLineIndentByText.Should().Be("2. ");

			index++;
			result[index].Text.Should().Be(" if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 1.5);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("very you get up speedily if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("very you get up speedily if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);
		}


		[TestMethod]
		public void AnalyzeTextTwoEmbeddedListTagsWithPTag2Test()
		{
			var font = new Font
			{
				Color = "#000000",
				Size = 12.0,
				Bold = false,
				Underline = false,
				Italic = false,
				Fontfamily = "Arial"
			};
			var html = $"""
				<h1>very you get up speedily if Off supposing moment</h1>
				<br>
				<p>very you get up speedily if Off supposing moment</p>
				<ul><li>very you get up speedily</li><li>if Off supposing moment</li></ul>
				<br>
				<p>very you get up speedily if Off supposing moment</p>
				<ol><li>very you get up speedily</li><li>if Off supposing moment</li></ol>
				<br>
				<p>very you get up speedily if Off supposing moment</p>
				""";


			var result = _classUnderTest.AnalyzeText(font, html).ToList();

			result.Should().HaveCount(22);

			var index = 0;
			result[index].Text.Should().Be("very you get up speedily if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("very you get up speedily if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("-");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);
			result[index].SkipParagraphAlignment.Should().BeTrue();
			result[index].ReduceLineIndent.Should().BeTrue();
			result[index].ReduceLineIndentByText.Should().Be("- ");

			index++;
			result[index].Text.Should().Be(" very you get up speedily");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("-");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);
			result[index].SkipParagraphAlignment.Should().BeTrue();
			result[index].ReduceLineIndent.Should().BeTrue();
			result[index].ReduceLineIndentByText.Should().Be("- ");

			index++;
			result[index].Text.Should().Be(" if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("very you get up speedily if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("1.");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 1.5);
			result[index].SkipParagraphAlignment.Should().BeTrue();
			result[index].ReduceLineIndent.Should().BeTrue();
			result[index].ReduceLineIndentByText.Should().Be("1. ");

			index++;
			result[index].Text.Should().Be(" very you get up speedily");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 1.5);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("2.");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 1.5);
			result[index].SkipParagraphAlignment.Should().BeTrue();
			result[index].ReduceLineIndent.Should().BeTrue();
			result[index].ReduceLineIndentByText.Should().Be("2. ");

			index++;
			result[index].Text.Should().Be(" if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 1.5);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);
						
			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("very you get up speedily if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);
		}

		[TestMethod]
		public void AnalyzeTextTwoEmbeddedListTagsWithPTag3Test()
		{
			var font = new Font
			{
				Color = "#000000",
				Size = 12.0,
				Bold = false,
				Underline = false,
				Italic = false,
				Fontfamily = "Arial"
			};
			var html = $"""
				<p>very you get up speedily if Off supposing moment</p>
				<ul><li>very you get up speedily</li><li>if Off supposing moment</li></ul>
				""";


			var result = _classUnderTest.AnalyzeText(font, html).ToList();

			result.Should().HaveCount(7);

			var index = 0;
			result[index].Text.Should().Be("very you get up speedily if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);
					
			index++;
			result[index].Text.Should().Be("-");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);
			result[index].SkipParagraphAlignment.Should().BeTrue();
			result[index].ReduceLineIndent.Should().BeTrue();
			result[index].ReduceLineIndentByText.Should().Be("- ");

			index++;
			result[index].Text.Should().Be(" very you get up speedily");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("-");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);
			result[index].SkipParagraphAlignment.Should().BeTrue();
			result[index].ReduceLineIndent.Should().BeTrue();
			result[index].ReduceLineIndentByText.Should().Be("- ");

			index++;
			result[index].Text.Should().Be(" if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(28.3465 / 2.0);
		}
		[TestMethod]
		public void AnalyzeTextOnlyPTagsTest()
		{
			var font = new Font
			{
				Color = "#000000",
				Size = 12.0,
				Bold = false,
				Underline = false,
				Italic = false,
				Fontfamily = "Arial"
			};
			var html = $"""
				<p>very you get up speedily if Off supposing moment</p>
				<p>very you get up speedily if Off supposing moment</p>
				<p>very you get up speedily if Off supposing moment</p>
				""";


			var result = _classUnderTest.AnalyzeText(font, html).ToList();

			result.Should().HaveCount(5);

			var index = 0;
			result[index].Text.Should().Be("very you get up speedily if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("very you get up speedily if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be(Environment.NewLine);
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);

			index++;
			result[index].Text.Should().Be("very you get up speedily if Off supposing moment");
			result[index].Font.Fontfamily.Should().Be("Arial");
			result[index].Font.Size.Should().Be(12.0);
			result[index].Font.Bold.Should().BeFalse();
			result[index].Font.Italic.Should().BeFalse();
			result[index].Font.Underline.Should().BeFalse();
			result[index].Font.Color.Should().Be("255 0 0 0");
			result[index].LineIndent.Should().Be(0);
		}

		[TestMethod]
		public void ConvertToHtmlListingWithinTextBlockTest()
		{
			// Arrange
			var plainText = @"very you get up speedily if Off supposing moment
- very you get up speedily
- if Off supposing moment
wish where we held steepest True he questions eat Thoughts";

			// Act
			var htmlText = _classUnderTest.ConvertToHtml(plainText);

			// Arrange
			var expectedResult = new StringBuilder();
			expectedResult.Append("<span>very you get up speedily if Off supposing moment</span>");
			expectedResult.Append("<ul>");
			expectedResult.Append("<li>very you get up speedily</li>");
			expectedResult.Append("<li>if Off supposing moment</li>");
			expectedResult.Append("</ul>");
			expectedResult.Append("<span>wish where we held steepest True he questions eat Thoughts</span>");

			htmlText.Should().Be(expectedResult.ToString());
		}

		[TestMethod]
		public void ConvertToHtmlStartWithListingTest()
		{
			// Arrange
			var plainText = @"- very you get up speedily
- if Off supposing moment
wish where we held steepest True he questions eat Thoughts";

			// Act
			var htmlText = _classUnderTest.ConvertToHtml(plainText);

			// Arrange
			var expectedResult = new StringBuilder();
			expectedResult.Append("<ul>");
			expectedResult.Append("<li>very you get up speedily</li>");
			expectedResult.Append("<li>if Off supposing moment</li>");
			expectedResult.Append("</ul>");
			expectedResult.Append("<span>wish where we held steepest True he questions eat Thoughts</span>");

			htmlText.Should().Be(expectedResult.ToString());
		}

		[TestMethod]
		public void ConvertToHtmlOnlyWithListingTest()
		{
			// Arrange
			var plainText = @"- very you get up speedily
- if Off supposing moment";

			// Act
			var htmlText = _classUnderTest.ConvertToHtml(plainText);

			// Arrange
			var expectedResult = new StringBuilder();
			expectedResult.Append("<ul>");
			expectedResult.Append("<li>very you get up speedily</li>");
			expectedResult.Append("<li>if Off supposing moment</li>");
			expectedResult.Append("</ul>");

			htmlText.Should().Be(expectedResult.ToString());
		}

		[TestMethod]
		public void ConvertToHtmlWithMultipleListingsTest()
		{
			// Arrange
			var plainText = @"very you get up speedily if Off supposing moment
- very you get up speedily
- if Off supposing moment

wish where we held steepest True he questions eat Thoughts

- very you get up speedily

very you get up speedily if Off supposing moment
- very you get up speedily

- if Off supposing moment
wish where we held steepest True he questions eat Thoughts";

			// Act
			var htmlText = _classUnderTest.ConvertToHtml(plainText);

			// Arrange
			var expectedResult = new StringBuilder();
			expectedResult.Append("<span>very you get up speedily if Off supposing moment</span>");
			expectedResult.Append("<ul>");
			expectedResult.Append("<li>very you get up speedily</li>");
			expectedResult.Append("<li>if Off supposing moment</li>");
			expectedResult.Append("</ul>");
			expectedResult.Append("<br>");
			expectedResult.Append("<span>wish where we held steepest True he questions eat Thoughts</span>");
			expectedResult.Append("<br>");
			expectedResult.Append("<ul>");
			expectedResult.Append("<li>very you get up speedily</li>");
			expectedResult.Append("</ul>");
			expectedResult.Append("<br>");
			expectedResult.Append("<span>very you get up speedily if Off supposing moment</span>");
			expectedResult.Append("<ul>");
			expectedResult.Append("<li>very you get up speedily</li>");
			expectedResult.Append("</ul>");
			expectedResult.Append("<ul>");
			expectedResult.Append("<li>if Off supposing moment</li>");
			expectedResult.Append("</ul>");
			expectedResult.Append("<span>wish where we held steepest True he questions eat Thoughts</span>");

			htmlText.Should().Be(expectedResult.ToString());
		}

		[TestMethod]
		public void ConvertToHtmlOnlyWithSingleListingItemTest()
		{
			// Arrange
			var plainText = @"- very you get up speedily";

			// Act
			var htmlText = _classUnderTest.ConvertToHtml(plainText);

			// Arrange
			var expectedResult = new StringBuilder();
			expectedResult.Append("<ul>");
			expectedResult.Append("<li>very you get up speedily</li>");
			expectedResult.Append("</ul>");

			htmlText.Should().Be(expectedResult.ToString());
		}
	}
}