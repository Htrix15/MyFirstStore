using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFirstStore.ViewModels
{
    public class UpdateStatus
    {
        public int IdOrder { get; set; }
        public int IdOldStatus { get; set; }
        public int IdNewStatus { get; set; }
        public string OldStatus { get; set; }
        public string NewStatus { get; set; }
        public bool? YesUpdate { get; set; }
        public int CurrentPosition { get; set; }

    }
}
