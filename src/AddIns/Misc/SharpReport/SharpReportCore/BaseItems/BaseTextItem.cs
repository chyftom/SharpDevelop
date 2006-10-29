//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by a tool.
//     Runtime Version: 1.1.4322.2032
//
//     Changes to this file may cause incorrect behavior and will be lost if 
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;

using SharpReportCore.Exporters;

/// <summary>
/// This class is the BaseClass  for all TextBased Items 
/// like <see cref="BaseDataItem"></see> etc.
/// </summary>


namespace SharpReportCore {
	
	public class BaseTextItem : SharpReportCore.BaseReportItem,IItemRenderer,IExportColumnBuilder {
		private string text;

		private string formatString = String.Empty;
		private StringFormat stringFormat;
		private StringTrimming stringTrimming;
		private ContentAlignment contentAlignment;
		
		private TextDrawer textDrawer;

		private RectangleShape shape = new RectangleShape();
		
		#region Constructor
		
		public BaseTextItem():base() {
			this.stringFormat = StringFormat.GenericTypographic;
			this.contentAlignment = ContentAlignment.MiddleLeft;
			this.stringTrimming = StringTrimming.EllipsisCharacter;
			this.textDrawer = new TextDrawer();
		}
		
		#endregion
		
		#region IExportColumnBuilder  implementation
		

		public BaseExportColumn CreateExportColumn(Graphics graphics){	
			TextStyleDecorator st = this.CreateItemStyle(graphics);
			ExportText item = new ExportText(st,false);
			
			item.Text = this.text;
			return item;
		}
		
		protected TextStyleDecorator CreateItemStyle (Graphics g) {
			TextStyleDecorator style = new TextStyleDecorator();
			SizeF measureSizeF = new SizeF ();
			measureSizeF = g.MeasureString(text,
			                               this.Font,
			                               this.Size.Width,
			                               this.stringFormat);
			RectangleF rect = base.DrawingRectangle (measureSizeF);
			
			style.BackColor = this.BackColor;
			style.Font = this.Font;
			style.ForeColor = this.ForeColor;
			style.Location = this.Location;
			style.Size = this.Size;
			style.DrawBorder = this.DrawBorder;
			
			style.StringFormat = this.stringFormat;
			style.StringTrimming = this.stringTrimming;
			style.ContentAlignment = this.contentAlignment;
			return style;
		}

		#endregion
		
		protected string FormatOutput(string valueToFormat,string format,
		                              TypeCode typeCode, string nullValue ){
			
			return StandardFormatter.FormatItem(valueToFormat,format,
			                                        typeCode,nullValue);			
		}
		
		
		public override void Render(ReportPageEventArgs rpea) {
			
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			
			base.Render(rpea);
			RectangleF rect = PrepareRectangle (rpea,this.Text);
			FillBackGround (rpea.PrintPageEventArgs.Graphics,
			                rect);
			DrawFrame (rpea.PrintPageEventArgs.Graphics,
			           Rectangle.Ceiling (rect));
			PrintTheStuff (rpea,this.Text,rect);
			base.NotiyfyAfterPrint (rpea.LocationAfterDraw);
		}
		
		public override string ToString() {
			return "BaseTextItem";
		}
		
		protected void FillBackGround (Graphics  graphics,RectangleF rectangle) {
			shape.FillShape(graphics,
			                new SolidFillPattern(this.BackColor),
			                rectangle);
		}
		
		protected void DrawFrame (Graphics graphics,Rectangle rectangle) {
			if (base.DrawBorder == true) {
				shape.DrawShape (graphics,
				                 new BaseLine (this.ForeColor,System.Drawing.Drawing2D.DashStyle.Solid,1),
				                 rectangle);
			}
		}
		
		protected RectangleF PrepareRectangle (ReportPageEventArgs rpea,string text) {
			SizeF measureSize = MeasureReportItem (rpea,text);			
			RectangleF rect = base.DrawingRectangle (measureSize);
			return rect;
		}
		
		///<summary>
		/// Measure the Size of the String rectangle
		/// </summary>
		
		private SizeF MeasureReportItem (ReportPageEventArgs rpea,string text) {
			SizeF measureSizeF = new SizeF ();
			
			measureSizeF = rpea.PrintPageEventArgs.Graphics.MeasureString(text,
			                                                          this.Font,
			                                                          this.Size.Width,
			                                                          this.stringFormat);
			return measureSizeF;
		}
		
		/// <summary>
		/// Standart Function to Draw Strings
		/// </summary>
		/// <param name="e">ReportpageEventArgs</param>
		/// <param name="toPrint">Formatted String toprint</param>
		/// <param name="rectangle">rectangle where to draw the string</param>
	
		protected void PrintTheStuff (ReportPageEventArgs rpea,
		                              string toPrint,
		                              RectangleF rectangle ) {
			
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			
			textDrawer.DrawString(rpea.PrintPageEventArgs.Graphics,
			                      toPrint,this.Font,
			                      new SolidBrush(this.ForeColor),
			                      rectangle,
			                      this.stringTrimming,this.contentAlignment);
			                      
			
			 rpea.LocationAfterDraw = new Point (this.Location.X + this.Size.Width,
			                                  this.Location.Y + this.Size.Height);
		}
	
		
		public virtual string Text {
			get {
				return text;
			}
			set {
				text = value;
				base.NotifyPropertyChanged("Text");
			}
		}
		
		///<summary>
		/// Formatstring like in MSDN
		/// </summary>
		[Browsable(true),
		 Category("Appearance"),
		 Description("String to format Number's Date's etc")]
		
		public virtual string FormatString {
			get {
				return formatString;
			}
			set {
				formatString = value;
				base.NotifyPropertyChanged("FormatString");
			}
		}
		
		
		[Category("Appearance")]
		public  StringTrimming StringTrimming {
			get {
				return stringTrimming;
			}
			set {
				stringTrimming = value;
				base.NotifyPropertyChanged("StringTrimming");
			}
		}
		
		[Category("Appearance")]
		public virtual System.Drawing.ContentAlignment ContentAlignment {
			get {
				return this.contentAlignment;
			}
			set {
				this.contentAlignment = value;
				base.NotifyPropertyChanged("ContentAlignment");
			}
		}
	
		[Browsable(false)]
		[XmlIgnoreAttribute]
		public virtual StringFormat StringFormat {
			get {
				return TextDrawer.BuildStringFormat (this.StringTrimming,
				                                          this.ContentAlignment);
			}
		}
	}
}
