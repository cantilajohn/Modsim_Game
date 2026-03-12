using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modsim_Game
{
    public class Novice
    {
        public string Name { get; set; }
        public List<string> Weapon { get; set; }
        public int Damage { get; set; }
        public Novice(string name, List<string> weapon, int damage)
        {
            Name = name = "Novice";
            Weapon = weapon = new List<string>
                {
                    "Hand",
                    "Dagger",
                    "One-Handed-Sword",
                    "One-Handed-Axe",
                    "One-Handed-Mace",
                    "Two-Handed-Mace",
                    "Rod&Staff",
                    "Two-Handed-Staff"
                };

        }
    }

    
    public class JobClass
    {
        public string Novice;

        public string Name { get; set; }
        public List<string> Weapon { get; set; }

        public JobClass(string name, List<string> weapon)
        {
            Name = name;
            Weapon = weapon;
        }

        // Optionally, you can add a default constructor for "Novice" with default weapons:
        public static JobClass CreateNovice()
        {
            return new JobClass(
                "Novice",
                new List<string>
                {
                    "Hand",
                    "Dagger",
                    "One-Handed-Sword",
                    "One-Handed-Axe",
                    "One-Handed-Mace",
                    "Two-Handed-Mace",
                    "Rod&Staff",
                    "Two-Handed-Staff"
                }
            );
        }
    }
}
