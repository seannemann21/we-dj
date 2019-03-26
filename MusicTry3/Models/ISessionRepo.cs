using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicTry3.Models
{
    public interface ISessionRepo
    {
        List<Session> GetSessions();
    }
}
