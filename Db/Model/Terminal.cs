using Db.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db.Model
{
    public class Terminal : Base
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TerminalId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public TerminalType TerminalType { set; get; }
        public string TerminalBrand { set; get; }
        public string TerminalIP { set; get; }
        public string TerminalUserName { set; get; }
        public string TerminalPassword { set; get; }
        public int BranchId { get; set; }
        public string Cbs_terminal_ac { set; get; }
        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }

        public static List<Terminal> All()
        {
            List<Terminal> terminals = null;
            using (var context = new ReconContext())
            {
                terminals = context.Terminals.ToList();
            }
            return terminals;
        }

        public static Terminal FindTerminal(Func<Terminal, bool> where)
        {
            Terminal _Terminal = null;
            using (var context = new ReconContext())
            {
                _Terminal = context.Terminals.First(where);
            }
            return _Terminal;
        }
    }
}
