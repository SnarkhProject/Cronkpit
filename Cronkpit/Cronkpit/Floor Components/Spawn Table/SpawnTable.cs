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
                    spData.Add(new SpawnData("Skeleton", 24));
                    spData.Add(new SpawnData("GoldMimic", 34));
                    spData.Add(new SpawnData("GoreHound", 49));
                    spData.Add(new SpawnData("Zombie", 99));
                    break;
                case 6:
                    spData.Add(new SpawnData("HollowKnight", 9));
                    spData.Add(new SpawnData("Skeleton", 29));
                    spData.Add(new SpawnData("GoldMimic", 49));
                    spData.Add(new SpawnData("GoreHound", 54));
                    spData.Add(new SpawnData("Zombie", 99));
                    break;
                case 7:
                    spData.Add(new SpawnData("HollowKnight", 14));
                    spData.Add(new SpawnData("Skeleton", 39));
                    spData.Add(new SpawnData("GoldMimic", 59));
                    spData.Add(new SpawnData("Zombie", 99));
                    break;
                    //for 8 and over we do GRENDELS
                case 8:
                    spData.Add(new SpawnData("HollowKnight", 14));
                    spData.Add(new SpawnData("Skeleton", 39));
                    spData.Add(new SpawnData("Grendel", 44));
                    spData.Add(new SpawnData("GoldMimic", 64));
                    spData.Add(new SpawnData("Zombie", 99));
                    break;
                case 9:
                    spData.Add(new SpawnData("HollowKnight", 14));
                    spData.Add(new SpawnData("Skeleton", 39));
                    spData.Add(new SpawnData("Grendel", 49));
                    spData.Add(new SpawnData("GoldMimic", 64));
                    spData.Add(new SpawnData("Necromancer", 69));
                    spData.Add(new SpawnData("Zombie", 99));
                    break;
                case 10:
                    spData.Add(new SpawnData("HollowKnight", 14));
                    spData.Add(new SpawnData("Skeleton", 34));
                    spData.Add(new SpawnData("Grendel", 49));
                    spData.Add(new SpawnData("GoldMimic", 59));
                    spData.Add(new SpawnData("Necromancer", 69));
                    spData.Add(new SpawnData("Zombie", 99));
                    break;
                case 11:
                    spData.Add(new SpawnData("HollowKnight", 14));
                    spData.Add(new SpawnData("Skeleton", 34));
                    spData.Add(new SpawnData("Grendel", 54));
                    spData.Add(new SpawnData("GoldMimic", 64));
                    spData.Add(new SpawnData("Necromancer", 74));
                    spData.Add(new SpawnData("Zombie", 99));
                    break;
                case 12:
                    spData.Add(new SpawnData("HollowKnight", 9));
                    spData.Add(new SpawnData("Skeleton", 24));
                    spData.Add(new SpawnData("Grendel", 49));
                    spData.Add(new SpawnData("GoldMimic", 59));
                    spData.Add(new SpawnData("Necromancer", 74));
                    spData.Add(new SpawnData("Zombie", 99));
                    break;
                default:
                    spData.Add(new SpawnData("HollowKnight", 9));
                    spData.Add(new SpawnData("Skeleton", 24));
                    spData.Add(new SpawnData("Grendel", 54));
                    spData.Add(new SpawnData("GoldMimic", 64));
                    spData.Add(new SpawnData("Necromancer", 79));
                    spData.Add(new SpawnData("Zombie", 99));
                    break;
            }
        }

        private void build_sub_table(int floor)
        {
            int redKnight_Spawn = 0;
            redKnight_Spawn = Math.Max((floor-5)*2, 0) + 1;

            spData.Add(new SpawnData("RedKnight", redKnight_Spawn));
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
