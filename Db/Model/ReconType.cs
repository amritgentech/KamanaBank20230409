using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class ReconType : Base
    {
        public int ReconTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MapReconMethod { get; set; }
        public bool IsDisplay { get; set; }
        public static ReconType Find(int ReconTypeId)
        {
            return FindReconType(rt => rt.ReconTypeId == ReconTypeId);
        }

        public static ReconType FindReconType(Func<ReconType, bool> where)
        {
            ReconType reconType = null;
            using (var context = new ReconContext())
            {
                reconType = context.ReconTypes.First(where);
            }
            return reconType;
        }
    }
}
