using System;
using System.Linq;
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
			var html = $@"very you get up speedily if Off supposing moment <b>directly part mirth <i>newspaper own come as its </i></b>who greatest but 
Dashwood great children <u>message</u> are you perceived <p>indeed they entreaties disposed announcing views On come On or as early 
suitable Entered questions middletons warmth</p> true be drawings Melancholy on can mr because an thus 
at dwelling weeks its though pleasure may <b>good <span style=""font-family: Verdana, Helvetica; font-size: 14; color: #202020;"">within up</span> bed</b> 
Preferred leave bred since Written Visited<br> 
through joy simplicity so half ask Chicken expense old wife tended hence strangers terminated ye check Admitting entire<br> 
come about partiality Prepared on <b style=""color: #ff0000"">when</b> wish where we held steepest True he questions eat Thoughts pure some apartments steepest such why
";


			var result = _classUnderTest.AnalyzeText(font, html).ToList();

			result.Should().HaveCount(20);

			result[0].Text.Should().Be("very you get up speedily if Off supposing moment");
			result[0].Font.Fontfamily.Should().Be("Arial");
			result[0].Font.Size.Should().Be(12.0);
			result[0].Font.Bold.Should().BeFalse();
			result[0].Font.Italic.Should().BeFalse();
			result[0].Font.Underline.Should().BeFalse();
			result[0].Font.Color.Should().Be("255 0 0 0");

			result[1].Text.Should().Be("directly part mirth");
			result[1].Font.Fontfamily.Should().Be("Arial");
			result[1].Font.Size.Should().Be(12.0);
			result[1].Font.Bold.Should().BeTrue();
			result[1].Font.Italic.Should().BeFalse();
			result[1].Font.Underline.Should().BeFalse();
			result[1].Font.Color.Should().Be("255 0 0 0");

			result[2].Text.Should().Be("newspaper own come as its");
			result[2].Font.Fontfamily.Should().Be("Arial");
			result[2].Font.Size.Should().Be(12.0);
			result[2].Font.Bold.Should().BeTrue();
			result[2].Font.Italic.Should().BeTrue();
			result[2].Font.Underline.Should().BeFalse();
			result[2].Font.Color.Should().Be("255 0 0 0");

			result[3].Text.Should().Be("who greatest but Dashwood great children");
			result[3].Font.Fontfamily.Should().Be("Arial");
			result[3].Font.Size.Should().Be(12.0);
			result[3].Font.Bold.Should().BeFalse();
			result[3].Font.Italic.Should().BeFalse();
			result[3].Font.Underline.Should().BeFalse();
			result[3].Font.Color.Should().Be("255 0 0 0");

			result[4].Text.Should().Be("message");
			result[4].Font.Fontfamily.Should().Be("Arial");
			result[4].Font.Size.Should().Be(12.0);
			result[4].Font.Bold.Should().BeFalse();
			result[4].Font.Italic.Should().BeFalse();
			result[4].Font.Underline.Should().BeTrue();
			result[4].Font.Color.Should().Be("255 0 0 0");

			result[5].Text.Should().Be("are you perceived");
			result[5].Font.Fontfamily.Should().Be("Arial");
			result[5].Font.Size.Should().Be(12.0);
			result[5].Font.Bold.Should().BeFalse();
			result[5].Font.Italic.Should().BeFalse();
			result[5].Font.Underline.Should().BeFalse();
			result[5].Font.Color.Should().Be("255 0 0 0");

			result[6].Text.Should().Be(Environment.NewLine);
			result[6].Font.Fontfamily.Should().Be("Arial");
			result[6].Font.Size.Should().Be(12.0);
			result[6].Font.Bold.Should().BeFalse();
			result[6].Font.Italic.Should().BeFalse();
			result[6].Font.Underline.Should().BeFalse();
			result[6].Font.Color.Should().Be("255 0 0 0");

			result[7].Text.Should().Be("indeed they entreaties disposed announcing views On come On or as early suitable Entered questions middletons warmth");
			result[7].Font.Fontfamily.Should().Be("Arial");
			result[7].Font.Size.Should().Be(12.0);
			result[7].Font.Bold.Should().BeFalse();
			result[7].Font.Italic.Should().BeFalse();
			result[7].Font.Underline.Should().BeFalse();
			result[7].Font.Color.Should().Be("255 0 0 0");

			result[8].Text.Should().Be(Environment.NewLine);
			result[8].Font.Fontfamily.Should().Be("Arial");
			result[8].Font.Size.Should().Be(12.0);
			result[8].Font.Bold.Should().BeFalse();
			result[8].Font.Italic.Should().BeFalse();
			result[8].Font.Underline.Should().BeFalse();
			result[8].Font.Color.Should().Be("255 0 0 0");

			result[9].Text.Should().Be("true be drawings Melancholy on can mr because an thus at dwelling weeks its though pleasure may");
			result[9].Font.Fontfamily.Should().Be("Arial");
			result[9].Font.Size.Should().Be(12.0);
			result[9].Font.Bold.Should().BeFalse();
			result[9].Font.Italic.Should().BeFalse();
			result[9].Font.Underline.Should().BeFalse();
			result[9].Font.Color.Should().Be("255 0 0 0");

			result[10].Text.Should().Be("good");
			result[10].Font.Fontfamily.Should().Be("Arial");
			result[10].Font.Size.Should().Be(12.0);
			result[10].Font.Bold.Should().BeTrue();
			result[10].Font.Italic.Should().BeFalse();
			result[10].Font.Underline.Should().BeFalse();
			result[10].Font.Color.Should().Be("255 0 0 0");

			result[11].Text.Should().Be("within up");
			result[11].Font.Fontfamily.Should().Be("Verdana");
			result[11].Font.Size.Should().Be(14.0);
			result[11].Font.Bold.Should().BeTrue();
			result[11].Font.Italic.Should().BeFalse();
			result[11].Font.Underline.Should().BeFalse();
			result[11].Font.Color.Should().Be("255 32 32 32");

			result[12].Text.Should().Be("bed");
			result[12].Font.Fontfamily.Should().Be("Arial");
			result[12].Font.Size.Should().Be(12.0);
			result[12].Font.Bold.Should().BeTrue();
			result[12].Font.Italic.Should().BeFalse();
			result[12].Font.Underline.Should().BeFalse();
			result[12].Font.Color.Should().Be("255 0 0 0");

			result[13].Text.Should().Be("Preferred leave bred since Written Visited");
			result[13].Font.Fontfamily.Should().Be("Arial");
			result[13].Font.Size.Should().Be(12.0);
			result[13].Font.Bold.Should().BeFalse();
			result[13].Font.Italic.Should().BeFalse();
			result[13].Font.Underline.Should().BeFalse();
			result[13].Font.Color.Should().Be("255 0 0 0");

			result[14].Text.Should().Be(Environment.NewLine);
			result[14].Font.Fontfamily.Should().Be("Arial");
			result[14].Font.Size.Should().Be(12.0);
			result[14].Font.Bold.Should().BeFalse();
			result[14].Font.Italic.Should().BeFalse();
			result[14].Font.Underline.Should().BeFalse();
			result[14].Font.Color.Should().Be("255 0 0 0");

			result[15].Text.Should().Be("through joy simplicity so half ask Chicken expense old wife tended hence strangers terminated ye check Admitting entire");
			result[15].Font.Fontfamily.Should().Be("Arial");
			result[15].Font.Size.Should().Be(12.0);
			result[15].Font.Bold.Should().BeFalse();
			result[15].Font.Italic.Should().BeFalse();
			result[15].Font.Underline.Should().BeFalse();
			result[15].Font.Color.Should().Be("255 0 0 0");

			result[16].Text.Should().Be(Environment.NewLine);
			result[16].Font.Fontfamily.Should().Be("Arial");
			result[16].Font.Size.Should().Be(12.0);
			result[16].Font.Bold.Should().BeFalse();
			result[16].Font.Italic.Should().BeFalse();
			result[16].Font.Underline.Should().BeFalse();
			result[16].Font.Color.Should().Be("255 0 0 0");

			result[17].Text.Should().Be("come about partiality Prepared on");
			result[17].Font.Fontfamily.Should().Be("Arial");
			result[17].Font.Size.Should().Be(12.0);
			result[17].Font.Bold.Should().BeFalse();
			result[17].Font.Italic.Should().BeFalse();
			result[17].Font.Underline.Should().BeFalse();
			result[17].Font.Color.Should().Be("255 0 0 0");

			result[18].Text.Should().Be("when");
			result[18].Font.Fontfamily.Should().Be("Arial");
			result[18].Font.Size.Should().Be(12.0);
			result[18].Font.Bold.Should().BeTrue();
			result[18].Font.Italic.Should().BeFalse();
			result[18].Font.Underline.Should().BeFalse();
			result[18].Font.Color.Should().Be("255 255 0 0");

			result[19].Text.Should().Be("wish where we held steepest True he questions eat Thoughts pure some apartments steepest such why");
			result[19].Font.Fontfamily.Should().Be("Arial");
			result[19].Font.Size.Should().Be(12.0);
			result[19].Font.Bold.Should().BeFalse();
			result[19].Font.Italic.Should().BeFalse();
			result[19].Font.Underline.Should().BeFalse();
			result[19].Font.Color.Should().Be("255 0 0 0");
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

			result[0].Text.Should().Be("very you get up speedily if Off supposing moment");
			result[1].Text.Should().Be(Environment.NewLine);
			result[2].Text.Should().Be("directly part mirth");
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

			result[0].Text.Should().Be("very you get up speedily if Off supposing moment");
			result[0].Font.Fontfamily.Should().Be("Verdana");
			result[0].Font.Size.Should().Be(14.0);
			result[0].Font.Bold.Should().BeFalse();
			result[0].Font.Italic.Should().BeFalse();
			result[0].Font.Underline.Should().BeFalse();
			result[0].Font.Color.Should().Be("255 32 32 32");
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

			result[0].Text.Should().Be("very you get up speedily if Off supposing moment");
			result[0].Font.Fontfamily.Should().Be("Arial");
			result[0].Font.Size.Should().Be(12.0);
			result[0].Font.Bold.Should().BeTrue();
			result[0].Font.Italic.Should().BeFalse();
			result[0].Font.Underline.Should().BeFalse();
			result[0].Font.Color.Should().Be("255 0 0 0");

			result[1].Text.Should().Be("directly part mirth");
			result[1].Font.Fontfamily.Should().Be("Arial");
			result[1].Font.Size.Should().Be(12.0);
			result[1].Font.Bold.Should().BeFalse();
			result[1].Font.Italic.Should().BeTrue();
			result[1].Font.Underline.Should().BeFalse();
			result[1].Font.Color.Should().Be("255 0 0 0");

			result[2].Text.Should().Be("newspaper own come as its");
			result[2].Font.Fontfamily.Should().Be("Arial");
			result[2].Font.Size.Should().Be(12.0);
			result[2].Font.Bold.Should().BeFalse();
			result[2].Font.Italic.Should().BeFalse();
			result[2].Font.Underline.Should().BeTrue();
			result[2].Font.Color.Should().Be("255 0 0 0");
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

			result[0].Text.Should().Be("-");
			result[0].Font.Fontfamily.Should().Be("Arial");
			result[0].Font.Size.Should().Be(12.0);
			result[0].Font.Bold.Should().BeFalse();
			result[0].Font.Italic.Should().BeFalse();
			result[0].Font.Underline.Should().BeFalse();
			result[0].Font.Color.Should().Be("255 0 0 0");
			result[0].LineIndent.Should().Be(28.3465 / 2.0);
			result[0].SkipParagraphAlignment.Should().BeTrue();
			result[0].ReduceLineIndent.Should().BeTrue();
			result[0].ReduceLineIndentByText.Should().Be("- ");

			result[1].Text.Should().Be("very you get up speedily");
			result[1].Font.Fontfamily.Should().Be("Arial");
			result[1].Font.Size.Should().Be(12.0);
			result[1].Font.Bold.Should().BeFalse();
			result[1].Font.Italic.Should().BeFalse();
			result[1].Font.Underline.Should().BeFalse();
			result[1].Font.Color.Should().Be("255 0 0 0");
			result[1].LineIndent.Should().Be(28.3465 / 2.0);

			result[2].Text.Should().Be(Environment.NewLine);
			result[2].Font.Fontfamily.Should().Be("Arial");
			result[2].Font.Size.Should().Be(12.0);
			result[2].Font.Bold.Should().BeFalse();
			result[2].Font.Italic.Should().BeFalse();
			result[2].Font.Underline.Should().BeFalse();
			result[2].Font.Color.Should().Be("255 0 0 0");
			result[2].LineIndent.Should().Be(0);

			result[3].Text.Should().Be("-");
			result[3].Font.Fontfamily.Should().Be("Arial");
			result[3].Font.Size.Should().Be(12.0);
			result[3].Font.Bold.Should().BeFalse();
			result[3].Font.Italic.Should().BeFalse();
			result[3].Font.Underline.Should().BeFalse();
			result[3].Font.Color.Should().Be("255 0 0 0");
			result[3].LineIndent.Should().Be(28.3465 / 2.0);
			result[3].SkipParagraphAlignment.Should().BeTrue();
			result[3].ReduceLineIndent.Should().BeTrue();
			result[3].ReduceLineIndentByText.Should().Be("- ");

			result[4].Text.Should().Be("if Off supposing moment");
			result[4].Font.Fontfamily.Should().Be("Arial");
			result[4].Font.Size.Should().Be(12.0);
			result[4].Font.Bold.Should().BeFalse();
			result[4].Font.Italic.Should().BeFalse();
			result[4].Font.Underline.Should().BeFalse();
			result[4].Font.Color.Should().Be("255 0 0 0");
			result[4].LineIndent.Should().Be(28.3465 / 2.0);
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
</ol>";


			var result = _classUnderTest.AnalyzeText(font, html).ToList();

			result.Should().HaveCount(5);

			result[0].Text.Should().Be("1.");
			result[0].Font.Fontfamily.Should().Be("Arial");
			result[0].Font.Size.Should().Be(12.0);
			result[0].Font.Bold.Should().BeFalse();
			result[0].Font.Italic.Should().BeFalse();
			result[0].Font.Underline.Should().BeFalse();
			result[0].Font.Color.Should().Be("255 0 0 0");
			result[0].LineIndent.Should().Be(10);
			result[0].SkipParagraphAlignment.Should().BeTrue();
			result[0].ReduceLineIndent.Should().BeTrue();
			result[0].ReduceLineIndentByText.Should().Be("2. ");

			result[1].Text.Should().Be("very you get up speedily");
			result[1].Font.Fontfamily.Should().Be("Arial");
			result[1].Font.Size.Should().Be(12.0);
			result[1].Font.Bold.Should().BeFalse();
			result[1].Font.Italic.Should().BeFalse();
			result[1].Font.Underline.Should().BeFalse();
			result[1].Font.Color.Should().Be("255 0 0 0");
			result[1].LineIndent.Should().Be(10);

			result[2].Text.Should().Be(Environment.NewLine);
			result[2].Font.Fontfamily.Should().Be("Arial");
			result[2].Font.Size.Should().Be(12.0);
			result[2].Font.Bold.Should().BeFalse();
			result[2].Font.Italic.Should().BeFalse();
			result[2].Font.Underline.Should().BeFalse();
			result[2].Font.Color.Should().Be("255 0 0 0");
			result[2].LineIndent.Should().Be(0);

			result[3].Text.Should().Be("2.");
			result[3].Font.Fontfamily.Should().Be("Arial");
			result[3].Font.Size.Should().Be(12.0);
			result[3].Font.Bold.Should().BeFalse();
			result[3].Font.Italic.Should().BeFalse();
			result[3].Font.Underline.Should().BeFalse();
			result[3].Font.Color.Should().Be("255 0 0 0");
			result[3].LineIndent.Should().Be(28.3465 / 1.5);
			result[3].SkipParagraphAlignment.Should().BeTrue();
			result[3].ReduceLineIndent.Should().BeTrue();
			result[3].ReduceLineIndentByText.Should().Be("2. ");

			result[4].Text.Should().Be("if Off supposing moment");
			result[4].Font.Fontfamily.Should().Be("Arial");
			result[4].Font.Size.Should().Be(12.0);
			result[4].Font.Bold.Should().BeFalse();
			result[4].Font.Italic.Should().BeFalse();
			result[4].Font.Underline.Should().BeFalse();
			result[4].Font.Color.Should().Be("255 0 0 0");
			result[4].LineIndent.Should().Be(28.3465 / 1.5);
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
			var html = $@"
very you get up speedily if Off supposing moment
<ul>
<li>very you <b>get</b> up speedily</li>
<li>if Off supposing moment</li>
</ul>
wish where we held steepest True he questions eat Thoughts";


			var result = _classUnderTest.AnalyzeText(font, html).ToList();

			result.Should().HaveCount(11);

			result[0].Text.Should().Be("very you get up speedily if Off supposing moment");
			result[0].Font.Fontfamily.Should().Be("Arial");
			result[0].Font.Size.Should().Be(12.0);
			result[0].Font.Bold.Should().BeFalse();
			result[0].Font.Italic.Should().BeFalse();
			result[0].Font.Underline.Should().BeFalse();
			result[0].Font.Color.Should().Be("255 0 0 0");
			result[0].LineIndent.Should().Be(0);

			result[1].Text.Should().Be(Environment.NewLine);
			result[1].Font.Fontfamily.Should().Be("Arial");
			result[1].Font.Size.Should().Be(12.0);
			result[1].Font.Bold.Should().BeFalse();
			result[1].Font.Italic.Should().BeFalse();
			result[1].Font.Underline.Should().BeFalse();
			result[1].Font.Color.Should().Be("255 0 0 0");
			result[1].LineIndent.Should().Be(0);

			result[2].Text.Should().Be("-");
			result[2].Font.Fontfamily.Should().Be("Arial");
			result[2].Font.Size.Should().Be(12.0);
			result[2].Font.Bold.Should().BeFalse();
			result[2].Font.Italic.Should().BeFalse();
			result[2].Font.Underline.Should().BeFalse();
			result[2].Font.Color.Should().Be("255 0 0 0");
			result[2].LineIndent.Should().Be(28.3465 / 2.0);
			result[2].SkipParagraphAlignment.Should().BeTrue();
			result[2].ReduceLineIndent.Should().BeTrue();
			result[2].ReduceLineIndentByText.Should().Be("- ");

			result[3].Text.Should().Be("very you");
			result[3].Font.Fontfamily.Should().Be("Arial");
			result[3].Font.Size.Should().Be(12.0);
			result[3].Font.Bold.Should().BeFalse();
			result[3].Font.Italic.Should().BeFalse();
			result[3].Font.Underline.Should().BeFalse();
			result[3].Font.Color.Should().Be("255 0 0 0");
			result[3].LineIndent.Should().Be(28.3465 / 2.0);

			result[4].Text.Should().Be("get");
			result[4].Font.Fontfamily.Should().Be("Arial");
			result[4].Font.Size.Should().Be(12.0);
			result[4].Font.Bold.Should().BeTrue();
			result[4].Font.Italic.Should().BeFalse();
			result[4].Font.Underline.Should().BeFalse();
			result[4].Font.Color.Should().Be("255 0 0 0");
			result[4].LineIndent.Should().Be(28.3465 / 2.0);

			result[5].Text.Should().Be("up speedily");
			result[5].Font.Fontfamily.Should().Be("Arial");
			result[5].Font.Size.Should().Be(12.0);
			result[5].Font.Bold.Should().BeFalse();
			result[5].Font.Italic.Should().BeFalse();
			result[5].Font.Underline.Should().BeFalse();
			result[5].Font.Color.Should().Be("255 0 0 0");
			result[5].LineIndent.Should().Be(28.3465 / 2.0);

			result[6].Text.Should().Be(Environment.NewLine);
			result[6].Font.Fontfamily.Should().Be("Arial");
			result[6].Font.Size.Should().Be(12.0);
			result[6].Font.Bold.Should().BeFalse();
			result[6].Font.Italic.Should().BeFalse();
			result[6].Font.Underline.Should().BeFalse();
			result[6].Font.Color.Should().Be("255 0 0 0");
			result[6].LineIndent.Should().Be(0);

			result[7].Text.Should().Be("-");
			result[7].Font.Fontfamily.Should().Be("Arial");
			result[7].Font.Size.Should().Be(12.0);
			result[7].Font.Bold.Should().BeFalse();
			result[7].Font.Italic.Should().BeFalse();
			result[7].Font.Underline.Should().BeFalse();
			result[7].Font.Color.Should().Be("255 0 0 0");
			result[7].LineIndent.Should().Be(28.3465 / 2.0);
			result[7].SkipParagraphAlignment.Should().BeTrue();
			result[7].ReduceLineIndent.Should().BeTrue();
			result[7].ReduceLineIndentByText.Should().Be("- ");

			result[8].Text.Should().Be("if Off supposing moment");
			result[8].Font.Fontfamily.Should().Be("Arial");
			result[8].Font.Size.Should().Be(12.0);
			result[8].Font.Bold.Should().BeFalse();
			result[8].Font.Italic.Should().BeFalse();
			result[8].Font.Underline.Should().BeFalse();
			result[8].Font.Color.Should().Be("255 0 0 0");
			result[8].LineIndent.Should().Be(28.3465 / 2.0);

			result[9].Text.Should().Be(Environment.NewLine);
			result[9].Font.Fontfamily.Should().Be("Arial");
			result[9].Font.Size.Should().Be(12.0);
			result[9].Font.Bold.Should().BeFalse();
			result[9].Font.Italic.Should().BeFalse();
			result[9].Font.Underline.Should().BeFalse();
			result[9].Font.Color.Should().Be("255 0 0 0");
			result[9].LineIndent.Should().Be(0);

			result[10].Text.Should().Be("wish where we held steepest True he questions eat Thoughts");
			result[10].Font.Fontfamily.Should().Be("Arial");
			result[10].Font.Size.Should().Be(12.0);
			result[10].Font.Bold.Should().BeFalse();
			result[10].Font.Italic.Should().BeFalse();
			result[10].Font.Underline.Should().BeFalse();
			result[10].Font.Color.Should().Be("255 0 0 0");
			result[10].LineIndent.Should().Be(0);
		}
	}
}