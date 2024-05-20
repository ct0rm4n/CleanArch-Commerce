using Core.ViewModel.Generic.Abstracts;
using Core.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModel.Generic
{
    public abstract class ResultVM<T> where T : IBaseVM
    {
        public Result<List<T>> Result { get; set; }
    }
}
