using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Tracker.TrackedTasksEditor
{
    class TimeRangePositionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            double a = (double)values[0];
            DateTime b = (DateTime)values[1];
            DateTime dayStart = DateTime.Today;
            double res = (b - dayStart).TotalMinutes * a;
            return res;


        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
