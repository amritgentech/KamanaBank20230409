using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Db.Model
{
    public class ReconProcessStatus : Base
    {
        [Key]
        public int ReconStatusId { get; set; }
        public string IsReconStarted { get; set; }

        public static void UpdateIsReconStarted(string isReconStarted)
        {
            using (var context = new ReconContext())
            {
                ReconProcessStatus reconStatus = context.ReconStatuses.FirstOrDefault();
                if (reconStatus == null)
                {
                    reconStatus = new ReconProcessStatus
                    {
                        IsReconStarted = "True"
                    };
                    context.ReconStatuses.Add(reconStatus);
                }
                else
                {
                    context.ReconStatuses.Attach(reconStatus);
                    reconStatus.IsReconStarted = isReconStarted;
                }
                context.SaveChanges();
            }
        }

        public static ReconProcessStatus Find(int reconProcessStatusId)
        {
            ReconProcessStatus reconStatus = null;
            using (var context = new ReconContext())
            {
                reconStatus = context.ReconStatuses.Find(reconProcessStatusId);
            }
            return reconStatus;
        }
        public static ReconProcessStatus GetFirst()
        {
            ReconProcessStatus reconStatus = null;
            using (var context = new ReconContext())
            {
                reconStatus = context.ReconStatuses.FirstOrDefault();
            }
            return reconStatus;
        }

        public static void ResetReconActivateFlag()
        {
            using (var context = new ReconContext())
            {
                foreach (var ob in context.ReconStatuses.ToList())
                {
                    context.ReconStatuses.Remove(ob);
                }
                context.SaveChanges();
            }
        }

    }
}
