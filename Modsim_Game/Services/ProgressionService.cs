using Modsim_Game.Models;

namespace Modsim_Game.Services
{
    public static class ProgressionService
    {
        public static int CalculateStatCost(int targetValue)
        {
            int totalCost = 0;
            for (int i = 1; i < targetValue; i++)
            {
                totalCost += ((i - 1) / 10) + 2;
            }
            return totalCost;
        }

        public static int CalculateTotalAvailablePoints(int baseLevel)
        {
            int totalPoints = 48; // Starting points
            for (int i = 1; i < baseLevel; i++)
            {
                totalPoints += (i / 5) + 3;
            }
            return totalPoints;
        }

        public static int GetRequiredPointsForNextLevel(int currentStat)
        {
            return ((currentStat - 1) / 10) + 2;
        }
    }
}
