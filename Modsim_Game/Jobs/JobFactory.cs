using System.Collections.Generic;

namespace Modsim_Game.Jobs
{
    public static class JobFactory
    {
        private static readonly Dictionary<string, IJobClass> _jobs = new Dictionary<string, IJobClass>
        {
            { "Novice", new Novice() },
            { "Swordsman", new Swordsman() },
            { "Magician", new Magician() },
            { "Archer", new Archer() },
            { "Acolyte", new Acolyte() },
            { "Merchant", new Merchant() },
            { "Thief", new Thief() }
        };

        public static IJobClass GetJob(string jobName)
        {
            if (_jobs.TryGetValue(jobName, out var job))
            {
                return job;
            }
            return _jobs["Novice"];
        }

        public static IEnumerable<string> GetAllJobNames() => _jobs.Keys;
    }
}
