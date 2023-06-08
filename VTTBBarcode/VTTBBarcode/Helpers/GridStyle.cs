using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace VTTBBarcode.Helpers
{
    public class GridStyle : DataGridStyle
    {
        public GridStyle()
        {
        }

        public override Color GetHeaderBackgroundColor()
        {
            return Color.FromHex("#149C62");
        }

        public override Color GetHeaderForegroundColor()
        {
            return Color.FromRgb(255, 255, 255);
        }
        public override Color GetRowDragViewIndicatorColor()
        {
            return Color.SkyBlue;
        }
        //public override Color GetRecordBackgroundColor()
        //{
        //    return Color.FromRgb(43, 43, 43);
        //}

        public override Color GetRecordForegroundColor()
        {
            return Color.Black;
        }

        public override Color GetSelectionBackgroundColor()
        {
            return Color.OrangeRed;
        }

        public override Color GetSelectionForegroundColor()
        {
            return Color.FromRgb(255, 255, 255);
        }

        public override Color GetCaptionSummaryRowBackgroundColor()
        {
            return Color.SkyBlue;
        }
        public override Color GetCaptionSummaryRowForeGroundColor()
        {
            return Color.White;
        }


        public override Color GetBorderColor()
        {
            return Color.FromRgb(81, 83, 82);
        }

        public override Color GetLoadMoreViewBackgroundColor()
        {
            return Color.FromRgb(242, 242, 242);
        }
    }
}
