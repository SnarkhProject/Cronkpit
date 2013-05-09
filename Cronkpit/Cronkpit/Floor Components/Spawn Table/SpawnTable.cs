using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    class SpawnTable
    {
        public List<SpawnData> spData;

        public SpawnTable(int floor, bool sub_spawn_table)
        {
            spData = new List<SpawnData>();

            if(sub_spawn_table)
                build_sub_table(floor);
            else
                build_table(floor);
        }

        private void build_table(int floor)
        {
            switch (floor)
            {
                case 1:
                    spData.Add(new SpawnData("GoreHound", 29));
                    spData.Add(new SpawnData("Zombie", 99));
                    break;
                case 2:
                    spData.Add(new SpawnData("GoreHound", 34));
                    spData.Add(new SpawnData("Zombie", 99));
                    break;
                case 3:
                    spData.Add(new SpawnData("Skeleton", 4));
                    spData.Add(new SpawnData("GoreHound", 39));
                    spData.Add(new SpawnData("Zombie", 99));
                    break;
                case 4:
                    spData.Add(new SpawnData("HollowKnight", 4));
                    spData.Add(new SpawnData("Skeleton", 14));
                    spData.Add(new SpawnData("GoreHound", 44));
                    spData.Add(new SpawnData("Zombie", 99));
                    break;
                case 5:
                    spData.Add(new SpawnData("HollowKnight", 4));
                    spData.Add(new SpawnData("GoreHound", 19));
                    spData.Add(new SpawnData("Ghost", 29));
                    spData.Add(new SpawnData("Skeleton", 49));
                    spData.Add(new SpawnData("Zombie", 99));
                    break;
                case 6:
                    spData.Add(new SpawnData("HollowKnight", 9));
                    spData.Add(new SpawnData("Skeleton", 29));
                    spData.Add(new SpawnData("Ghost", 44));
                    spData.Add(new SpawnData("GoreHound", 54));
                    spData.Add(new SpawnData("Zombie", 99));
                    break;
                case 7:
                    spData.Add(new SpawnData("HollowKnight", 14));
                    spData.Add(new SpawnData("Skeleton", 39));
                    spData.Add(new SpawnData("Ghost", 49));
                    spData.Add(new SpawnData("GoreWolf", 59));
                    spData.Add(new SpawnData("Zombie", 99));
                    break;
                case 8:
                    spData.Add(new SpawnData("HollowKnight", 14));
                    spData.Add(new SpawnData("Skeleton", 29));
                    spData.Add(new SpawnData("ArmoredSkel", 39));
                    spData.Add(new SpawnData("Ghost", 49));
                    spData.Add(new SpawnData("GoreWolf", 59));
                    spData.Add(new SpawnData("Necromancer", 64));
                    spData.Add(new SpawnData("Zombie", 99));
                    break;
                case 9:
                    spData.Add(new SpawnData("HollowKnight", 14));
                    spData.Add(new SpawnData("Skeleton", 24));
                    spData.Add(new SpawnData("ArmoredSkel", 39));
                    spData.Add(new SpawnData("Ghost", 49));
                    spData.Add(new SpawnData("GoreWolf", 64));
                    spData.Add(new SpawnData("Necromancer", 69));
                    spData.Add(new SpawnData("Zombie", 99));
                    break;
                case 10:
                    spData.Add(new SpawnData("HollowKnight", 14));
                    spData.Add(new SpawnData("Skeleton", 19));
                    spData.Add(new SpawnData("ArmoredSkel", 34));
                    spData.Add(new SpawnData("Ghost", 39));
                    spData.Add(new SpawnData("VoidWraith", 44));
                    spData.Add(new SpawnData("GoreWolf", 59));
                    spData.Add(new SpawnData("Necromancer", 69));
                    spData.Add(new SpawnData("Zombie", 99));
                    break;
                case 11:
                    spData.Add(new SpawnData("HollowKnight", 14));
                    spData.Add(new SpawnData("ArmoredSkel", 34));
                    spData.Add(new SpawnData("VoidWraith", 44));
                    spData.Add(new SpawnData("GoreWolf", 64));
                    spData.Add(new SpawnData("Necromancer", 74));
                    spData.Add(new SpawnData("Zombie", 99));
                    break;
                case 12:
                    spData.Add(new SpawnData("HollowKnight", 9));
                    spData.Add(new SpawnData("ArmoredSkel", 34));
                    spData.Add(new SpawnData("GoreWolf", 44));
                    spData.Add(new SpawnData("VoidWraith", 59));
                    spData.Add(new SpawnData("Necromancer", 74));
                    spData.Add(new SpawnData("Zombie", 99));
                    break;
                default:
                    spData.Add(new SpawnData("HollowKnight", 9));
                    spData.Add(new SpawnData("ArmoredSkel", 34));
                    spData.Add(new SpawnData("GoreWolf", 44));
                    spData.Add(new SpawnData("VoidWraith", 59));
                    spData.Add(new SpawnData("Necromancer", 74));
                    spData.Add(new SpawnData("Zombie", 99));
                    break;
            }
        }

        private void build_sub_table(int floor)
        {
            int redKnight_Spawn = 0;
            redKnight_Spawn = Math.Max((floor-4), 0);

            spData.Add(new SpawnData("RedKnight", redKnight_Spawn));

            switch (floor)
            {
                case 7:
                    spData.Add(new SpawnData("ZombieFanatic", 10));
                    break;
                case 8:
                    spData.Add(new SpawnData("ZombieFanatic", 15));
                    break;
                case 9:
                    spData.Add(new SpawnData("ZombieFanatic", 20));
                    break;
                case 10:
                    spData.Add(new SpawnData("ZombieFanatic", 20));
                    break;
                case 11:
                    spData.Add(new SpawnData("ZombieFanatic", 20));
                    break;
                case 12:
                    spData.Add(new SpawnData("ZombieFanatic", 25));
                    break;
            }
        }

        public string find_monster_by_number(int number)
        {
            for (int i = 0; i < spData.Count; i++)
            {
                if (number <= spData[i].my_assoc_number)
                    return spData[i].my_assoc_monster;
            }

            return spData[spData.Count - 1].my_assoc_monster;
        }

        public bool monster_in_table(string monsterName)
        {
            for (int i = 0; i < spData.Count; i++)
                if (String.Compare(spData[i].my_assoc_monster, monsterName) == 0)
                    return true;

            return false;
        }

        public int return_spawn_chance_by_monster(string monster_name)
        {
            for (int i = 0; i < spData.Count; i++)
            {
                if (String.Compare(monster_name, spData[i].my_assoc_monster) == 0)
                    return spData[i].my_assoc_number;
            }

            return -1;
        }
    }
}
