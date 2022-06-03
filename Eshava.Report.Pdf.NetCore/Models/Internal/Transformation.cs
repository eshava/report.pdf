namespace Eshava.Report.Pdf.Models.Internal
{
	internal class Transformation
	{
		/*
		 * • Translations are specified as [ 1 0 0 1 tx ty ], 
		 *   where tx and ty are the distances to translate 
		 *   the origin of the coordinate system in the horizontal 
		 *   and vertical dimensions, respectively.
		 *   
		 * • Scaling is obtained by [ sx 0 0 sy 0 0 ]. 
		 *   This scales the coordinates so that 1 unit in the horizontal 
		 *   and vertical dimensions of the new coordinate system 
		 *   is the same size as sx and sy units, respectively, 
		 *   in the previous coordinate system.
		 *   
		 * • Rotations are produced by [ cos θ sin θ −sin θ cos θ 0 0 ], 
		 *   which has the effect of rotating the coordinate system 
		 *   axes by an angle θ counterclockwise.
		 *   
		 * • Skew is specified by [ 1 tan α tan β 1 0 0 ], 
		 *   which skews the x axis by an angle α 
		 *   and the y axis by an angle β.
		 */

		public double Value1 { get; set; }
		public double Value2 { get; set; }
		public double Value3 { get; set; }
		public double Value4 { get; set; }
		public double Value5 { get; set; }
		public double Value6 { get; set; }

		public Transformation Multiply(Transformation transformation)
		{
			return new Transformation
			{
				Value1 = Value1 * transformation.Value1 + Value2 * transformation.Value3,
				Value2 = Value1 * transformation.Value2 + Value2 * transformation.Value4,
				Value3 = Value3 * transformation.Value1 + Value4 * transformation.Value3,
				Value4 = Value3 * transformation.Value2 + Value4 * transformation.Value4,
				Value5 = Value5 * transformation.Value1 + Value6 * transformation.Value3 + transformation.Value5,
				Value6 = Value5 * transformation.Value2 + Value6 * transformation.Value4 + transformation.Value6
			};
		}
	}
}