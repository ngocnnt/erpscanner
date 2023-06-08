using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace VTTBBarcode.Helpers
{
    public class GridStyleInvisible : DataGridStyle
    {
        public GridStyleInvisible()
        {
        }

        public override Color GetHeaderBackgroundColor()
        {
            return Color.FromRgb(255, 255, 255);
        }

        public override Color GetHeaderForegroundColor()
        {
            return Color.FromRgb(255, 255, 255);
        }
        public override Color GetRowDragViewIndicatorColor()
        {
            return Color.FromRgb(255, 255, 255);
        }
        //public override Color GetRecordBackgroundColor()
        //{
        //    return Color.FromRgb(43, 43, 43);
        //}

        public override Color GetRecordForegroundColor()
        {
            return Color.FromRgb(255, 255, 255);
        }

        public override Color GetSelectionBackgroundColor()
        {
            return Color.FromRgb(255, 255, 255);
        }

        public override Color GetSelectionForegroundColor()
        {
            return Color.FromRgb(255, 255, 255);
        }

        public override Color GetCaptionSummaryRowBackgroundColor()
        {
            return Color.FromRgb(255, 255, 255);
        }
        public override Color GetCaptionSummaryRowForeGroundColor()
        {
            return Color.FromRgb(255, 255, 255);
        }


        public override Color GetBorderColor()
        {
            return Color.FromRgb(255, 255, 255);
        }

        public override Color GetLoadMoreViewBackgroundColor()
        {
            return Color.FromRgb(255, 255, 255);
        }
    }
}
