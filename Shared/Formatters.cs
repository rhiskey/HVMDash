using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAAuth.Shared
{
    public class Formatters
    {
        public static string FormatDateTime(DateTime dt)
        {
            return dt.ToString("dd.MM.yyyy HH:mm");       
        }
    }
}
