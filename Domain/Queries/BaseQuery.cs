using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Queries
{
    public abstract class BaseQuery
    {
        public Guid Id { get; set; }
    }
}
