using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Infrastructure
{
    public interface ILoggerManager
    {
        void LogInformation(
          string message,
          params object[] parameters);
        void LogError(
            Exception exception,
            string message,
            params object[] parameters);
    }
}
