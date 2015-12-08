using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriemLib
{
    public class ExamenBlock
    {
        public Guid BlockId { get; set; }
        public string BlockName { get; set; }

        public Guid SelectedUnitId { get; set; }
        public string SelectedUnitName { get; set; }

        public List<KeyValuePair<Guid, string>> UnitList { get; set; }
    }

    public class ExamenBlockUnit
    {
        public Guid UnitId { get; set; }
        public int ExamId { get; set; }
        public double? EgeMin { get; set; }
        public string ExamUnitName { get; set; }
    }

    public delegate void UnitListUpdateHandler(ExamenBlockUnit unit);
}
