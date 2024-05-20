using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModel.Generic.Abstracts
{
    public abstract class ISaveVM
    {
        public int Id { get; set; }
        public DateTime InsertedDate { get; set; }
    }
}