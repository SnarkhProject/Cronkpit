using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    public class wound
    {
        public enum Wound_Type { Open, Impact, Burn, Frostburn };
        public Wound_Type type;
        public int severity;

        public wound(Wound_Type wType, int sDmg)
        {
            type = wType;
            severity = sDmg;
        }

        public wound(wound cpyWound)
        {
            type = cpyWound.type;
            severity = cpyWound.severity;
        }
    }

    class Limb
    {
        List<wound> injuries;
        bool is_head;
        Random rGen;

        String[] open_wounds = { "minor cut", "cut", "slash", "heavy slash", "gaping wound" };
        String[] impact_wounds = { "bruise", "large bruise", "bruised bone", "fracture", "broken bone" };
        String[] burn_wounds = { "blister", "minor burn", "burn", "severe burn", "horrific burn" };
        String[] frost_wounds = { "chillblains", "frostnip", "frostbite", "deep frostbite", "frostburn" };

        public Limb(bool head, ref Random r_gen)
        {
            injuries = new List<wound>();
            is_head = head;
            rGen = r_gen;
        }

        public bool is_disabled()
        {
            int debilitating_injuries = 0;
            for (int i = 0; i < injuries.Count; i++)
            {
                if (injuries[i].severity >= 5)
                    debilitating_injuries++;
            }

            if (is_head)
                return debilitating_injuries >= 1;
            else
                return debilitating_injuries >= 3;
        }

        public void add_injury(wound ouchie)
        {
            //Check to see if there are wounds of this type already on the part.
            //If there are 0 wounds, always add a new one.
            //If there's more than 0 but less than 3, 50/50 shot of adding new or making one worse
            //If there's 3, make one worse. Wound is chosen at random but can't be added to a 5 strength one
            for (int i = 0; i < ouchie.severity; i++)
            {
                wound.Wound_Type wound_type = ouchie.type;
                int wound_count = 0;
                for (int j = 0; j < injuries.Count; j++)
                {
                    if (injuries[j].type == wound_type)
                        wound_count++;
                }
                if (wound_count == 0)
                    injuries.Add(new wound(ouchie.type, 1));
                else if (wound_count > 0 && wound_count < 3)
                {
                    int add_or_worsen = rGen.Next(2);
                    if (add_or_worsen == 0)
                        injuries.Add(new wound(ouchie.type, 1));
                    else
                        worsen_injury(wound_count, ouchie);
                }
                else
                    worsen_injury(wound_count, ouchie);
            }
        }

        public void worsen_injury(int wound_count, wound ouchie)
        {
            //Worsen
            bool done = false;
            //First check to make sure that there are any injuries less than strength 5.
            int n_debil_wounds = 0;
            for (int i = 0; i < injuries.Count; i++)
            {
                if (injuries[i].type == ouchie.type && injuries[i].severity < 5)
                    n_debil_wounds++;
            }
            if (n_debil_wounds == 0)
            {
                injuries.Add(new wound(ouchie.type, 1));
                done = true;
            }
            
            while (!done)
            {
                int wound_to_worsen = rGen.Next(wound_count);
                int c_wound = 0;
                for (int i = 0; i < injuries.Count; i++)
                {
                    if (injuries[i].type == ouchie.type)
                        if (c_wound == wound_to_worsen && injuries[i].severity < 5)
                        {
                            injuries[i].severity++;
                            done = true;
                        }
                        else
                            c_wound++;
                }
            }
        }

        public void heal_random_wound()
        {
            if (!is_uninjured())
            {
                int heal_me = rGen.Next(injuries.Count);
                injuries[heal_me].severity--;
                if (injuries[heal_me].severity == 0)
                    injuries.RemoveAt(heal_me);
            }
        }

        public int count_debilitating_injuries()
        {
            int wound_count = 0;
            for (int i = 0; i < injuries.Count; i++)
            {
                if (injuries[i].severity >= 5)
                    wound_count++;
            }
            return wound_count;
        }

        public void consolidate_injury_report(ref List<string> wReport)
        {
            for (int i = 0; i < injuries.Count; i++)
            {
                if (injuries[i].type == wound.Wound_Type.Open)
                    wReport.Add(" - A " + open_wounds[injuries[i].severity - 1]);
                if (injuries[i].type == wound.Wound_Type.Impact)
                    wReport.Add(" - A " + impact_wounds[injuries[i].severity - 1]);
                if(injuries[i].type == wound.Wound_Type.Burn)
                    wReport.Add(" - A " + burn_wounds[injuries[i].severity - 1]);
                if (injuries[i].type == wound.Wound_Type.Frostburn)
                    wReport.Add(" - A " + frost_wounds[injuries[i].severity - 1]);
            }
        }

        public bool is_uninjured()
        {
            return injuries.Count == 0;
        }
    }
}
