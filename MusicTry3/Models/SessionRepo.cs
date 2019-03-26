using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace MusicTry3.Models
{
    public class SessionRepo
    {
        static List<Session> sessions;
        static bool running;

        private static bool SessionRemovalCondition(Session session)
        {
            return session.lastContactWithMaster.AddMinutes(5).Ticks < DateTime.UtcNow.Ticks;
        }

        public static List<Session> GetSessions()
        {
            if (sessions == null)
            {
                sessions = new List<Session>();
                running = true;
                Thread deadSessionRemover = new Thread(() => {
                    while (running)
                    {
                        IEnumerable<Session> deadSessions = sessions.FindAll(x => SessionRemovalCondition(x));
                        foreach (Session deadSession in deadSessions)
                        {
                            deadSession.Stop();
                        }
                        sessions.RemoveAll(x => SessionRemovalCondition(x));
                        // 20 minutes
                        Thread.Sleep(20 * 60 * 1000);
                    }
                });
                deadSessionRemover.Start();
            }

            return sessions;
        }
    }
}