using System;

namespace Modsim_Game
{
    public static class JobStatTable
    {
        //   SWORDSMAN  
        private static readonly int[] SwdStr = { 2, 14, 33, 40, 47, 49, 50 };
        private static readonly int[] SwdAgi = { 30, 46 };
        private static readonly int[] SwdVit = { 6, 18, 38, 42 };
        private static readonly int[] SwdDex = { 10, 22, 36 };
        private static readonly int[] SwdLuk = { 26, 44 };

        //   MAGICIAN  
        private static readonly int[] MagAgi = { 18, 26, 40, 47 };
        private static readonly int[] MagInt = { 2, 14, 22, 33, 38, 44, 46, 50 };
        private static readonly int[] MagDex = { 6, 10, 36 };
        private static readonly int[] MagLuk = { 30, 42, 49 };

        //   ARCHER  
        private static readonly int[] ArcStr = { 6, 38, 40 };
        private static readonly int[] ArcAgi = { 26, 33, 49 };
        private static readonly int[] ArcVit = { 46 };
        private static readonly int[] ArcInt = { 10, 47 };
        private static readonly int[] ArcDex = { 2, 14, 18, 30, 36, 42, 50 };
        private static readonly int[] ArcLuk = { 22, 44 };

        //   ACOLYTE  
        private static readonly int[] AcoStr = { 26, 42, 49 };
        private static readonly int[] AcoAgi = { 22, 40 };
        private static readonly int[] AcoVit = { 6, 30, 44 };
        private static readonly int[] AcoInt = { 10, 33, 46 };
        private static readonly int[] AcoDex = { 14, 36, 46 };
        private static readonly int[] AcoLuk = { 2, 18, 38, 50 };

        //   MERCHANT  
        private static readonly int[] MerStr = { 10, 22, 40, 44, 49 };
        private static readonly int[] MerAgi = { 33 };
        private static readonly int[] MerVit = { 2, 18, 30, 47 };
        private static readonly int[] MerInt = { 26 };
        private static readonly int[] MerDex = { 6, 14, 38, 42, 50 };
        private static readonly int[] MerLuk = { 36, 46 };

        //   THIEF  
        private static readonly int[] ThiStr = { 6, 30, 38, 47 };
        private static readonly int[] ThiAgi = { 2, 33, 36, 50 };
        private static readonly int[] ThiVit = { 14, 44 };
        private static readonly int[] ThiInt = { 18 };
        private static readonly int[] ThiDex = { 10, 22, 42, 49 };
        private static readonly int[] ThiLuk = { 26, 40, 46 };

        //Get the bonus for a specific job, stat, and job level
        public static int GetBonus(string job, string stat, int jobLevel)
        {
            int[] milestones = null;
            string s = stat.ToUpper();

            switch (job)
            {
                case "Swordsman":
                    if (s == "STR") milestones = SwdStr;
                    else if (s == "AGI") milestones = SwdAgi;
                    else if (s == "VIT") milestones = SwdVit;
                    else if (s == "DEX") milestones = SwdDex;
                    else if (s == "LUK") milestones = SwdLuk;
                    break;
                case "Magician":
                    if (s == "AGI") milestones = MagAgi;
                    else if (s == "INT") milestones = MagInt;
                    else if (s == "DEX") milestones = MagDex; else if (s == "LUK") milestones = MagLuk;
                    break;
                case "Archer":
                    if (s == "STR") milestones = ArcStr;
                    else if (s == "AGI") milestones = ArcAgi;
                    else if (s == "VIT") milestones = ArcVit;
                    else if (s == "INT") milestones = ArcInt;
                    else if (s == "DEX") milestones = ArcDex; else if (s == "LUK") milestones = ArcLuk;
                    break;
                case "Acolyte":
                    if (s == "STR") milestones = AcoStr;
                    else if (s == "AGI") milestones = AcoAgi;
                    else if (s == "VIT") milestones = AcoVit;
                    else if (s == "INT") milestones = AcoInt;
                    else if (s == "DEX") milestones = AcoDex; else if (s == "LUK") milestones = AcoLuk;
                    break;
                case "Merchant":
                    if (s == "STR") milestones = MerStr;
                    else if (s == "AGI") milestones = MerAgi;
                    else if (s == "VIT") milestones = MerVit;
                    else if (s == "INT") milestones = MerInt;
                    else if (s == "DEX") milestones = MerDex; else if (s == "LUK") milestones = MerLuk;
                    break;
                case "Thief":
                    if (s == "STR") milestones = ThiStr;
                    else if (s == "AGI") milestones = ThiAgi;
                    else if (s == "VIT") milestones = ThiVit;
                    else if (s == "INT") milestones = ThiInt;
                    else if (s == "DEX") milestones = ThiDex; else if (s == "LUK") milestones = ThiLuk;
                    break;
            }
            //conditional to check how many milestones the current job level has passed and return that as the bonus
            if (milestones == null) return 0;

            int count = 0;
            foreach (int m in milestones)
            {
                if (jobLevel >= m) count++;
            }
            return count;
        }
        // For easier access,
        private static readonly int[] SwordsmanStrLevels = { 1, 6, 12, 19, 27, 34, 42 };
        private static readonly int[] SwordsmanAgiLevels = { 10, 30 };
        private static readonly int[] SwordsmanVitLevels = { 3, 15, 25, 40 };
        private static readonly int[] SwordsmanDexLevels = { 8, 22, 38 };
        private static readonly int[] SwordsmanLukLevels = { 5, 45 };


        // Arrays representing the HP columns from your image Index 0 = Level 1
        private static readonly int[] NoviceHP = { 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100, 105, 110, 115, 120, 125, 130, 135, 140, 145, 150, 155, 160, 165, 170, 175, 180, 185, 190, 195, 200, 205, 210, 215, 220, 225, 230, 235, 240, 245, 250, 255, 260, 265, 270, 275, 280, 285, 290, 295, 300, 305, 310, 315, 320, 325, 330, 335, 340, 345, 350, 355, 360, 365, 370, 375, 380, 385, 390, 395, 400, 405, 410, 415, 420, 425, 430, 435, 440, 445, 450, 455, 460, 465, 470, 475, 480, 485, 490, 495, 500, 505, 510, 515, 520, 525, 530 };

        private static readonly int[] SwordsmanHP = { 40, 46, 53, 61, 70, 79, 89, 100, 111, 123, 136, 149, 163, 178, 194, 210, 227, 245, 263, 282, 302, 322, 343, 365, 388, 411, 435, 460, 485, 511, 538, 565, 593, 622, 652, 682, 713, 745, 777, 810, 844, 878, 913, 949, 986, 1023, 1061, 1100, 1139, 1179, 1220, 1261, 1303, 1346, 1390, 1434, 1479, 1525, 1571, 1618, 1666, 1714, 1763, 1813, 1864, 1915, 1967, 2020, 2073, 2127, 2182, 2237, 2293, 2350, 2408, 2466, 2525, 2585, 2645, 2706, 2768, 2830, 2893, 2957, 3022, 3087, 3153, 3220, 3287, 3355, 3424, 3493, 3563, 3634, 3706, 3778, 3851, 3925, 3999 };

        private static readonly int[] MagicianHP = { 40, 46, 52, 58, 65, 72, 79, 86, 94, 102, 110, 119, 128, 137, 147, 157, 167, 177, 188, 199, 210, 222, 234, 246, 259, 272, 285, 298, 312, 326, 340, 355, 370, 385, 401, 417, 433, 449, 466, 483, 500, 518, 536, 554, 573, 592, 611, 630, 650, 670, 690, 711, 732, 753, 775, 797, 819, 841, 864, 887, 910, 934, 958, 982, 1007, 1032, 1057, 1082, 1108, 1134, 1160, 1187, 1214, 1241, 1269, 1297, 1325, 1353, 1382, 1411, 1440, 1470, 1500, 1530, 1561, 1592, 1623, 1654, 1686, 1718, 1750, 1783, 1816, 1849, 1883, 1917, 1951, 1985, 2020 };

        private static readonly int[] ArcherThiefHP = { 40, 46, 53, 60, 68, 76, 85, 94, 104, 114, 125, 136, 148, 160, 173, 186, 200, 214, 229, 244, 260, 276, 293, 310, 328, 346, 365, 384, 404, 424, 445, 466, 488, 510, 533, 556, 580, 604, 629, 654, 680, 706, 733, 760, 788, 816, 845, 874, 904, 934, 965, 996, 1028, 1060, 1093, 1126, 1160, 1194, 1229, 1264, 1300, 1336, 1373, 1410, 1448, 1486, 1525, 1564, 1604, 1644, 1685, 1726, 1768, 1810, 1853, 1896, 1940, 1984, 2029, 2074, 2120, 2166, 2213, 2260, 2308, 2356, 2405, 2454, 2504, 2554, 2605, 2656, 2708, 2760, 2813, 2866, 2920, 2974, 3029 };

        private static readonly int[] AcolyteMerchantHP = { 40, 46, 52, 59, 66, 73, 81, 89, 98, 107, 116, 126, 136, 147, 158, 169, 181, 193, 206, 219, 232, 246, 260, 275, 290, 305, 321, 337, 354, 371, 388, 406, 424, 443, 462, 481, 501, 521, 542, 563, 584, 606, 628, 651, 674, 697, 721, 745, 770, 795, 820, 846, 872, 899, 926, 953, 981, 1009, 1038, 1067, 1096, 1126, 1156, 1187, 1218, 1249, 1281, 1313, 1346, 1379, 1412, 1446, 1480, 1515, 1550, 1585, 1621, 1657, 1694, 1731, 1768, 1806, 1844, 1883, 1922, 1961, 2001, 2041, 2082, 2123, 2164, 2206, 2248, 2291, 2334, 2377, 2421, 2465, 2510 };

        public static int GetMaxHP(string jobClass, int level)
        {
            // Safety checks
            if (level < 1) level = 1;
            if (level > 99) level = 99;
            int index = level - 1;

            switch (jobClass)
            {
                case "Swordsman": return SwordsmanHP[index];
                case "Magician": return MagicianHP[index];
                case "Archer":
                case "Thief": return ArcherThiefHP[index];
                case "Acolyte":
                case "Merchant": return AcolyteMerchantHP[index];
                default: return NoviceHP[index];
            }
        }

        public static double GetSpJobModifier(string jobClass)
        {
            switch (jobClass)
            {
                case "Magician": return 6.0;
                case "Acolyte": return 5.0;
                case "Archer": return 2.0;
                case "Thief": return 2.0;
                case "Merchant": return 3.0;
                case "Swordsman": return 2.0;
                default: return 1.0;
            }
        }
    }
}
