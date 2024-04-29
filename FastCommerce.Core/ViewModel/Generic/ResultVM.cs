using FastCommerce.Core.ViewModel.Generic.Abstracts;
using FastCommerce.Core.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCommerce.Core.ViewModel.Generic
{
    public abstract class ResultVM<T> where T : IBaseVM
    {
        public Result<List<T>> Result { get; set; }
    }
}
