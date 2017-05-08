using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Domain.Concrete
{
    public class XlsData
    {
        public string[] caption;
        public ushort[] columnWidths;
        public List<List<String>> body; 
    }
}
