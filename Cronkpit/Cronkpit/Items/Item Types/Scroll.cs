using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    class Scroll: Item
    {
        int scroll_tier;

        public Scroll(int IDno, int goldVal, string myName, int stier)
            : base(IDno, goldVal, myName)
        {
            scroll_tier = stier;
        }

        public Scroll(int IDno, int goldVal, string myName, Scroll s)
            : base(IDno, goldVal, myName)
        {
            scroll_tier = s.get_tier();
        }

        public int get_tier()
        {
            return scroll_tier;
        }

        public override string get_my_texture_name()
        {
            switch (scroll_tier)
            {
                case 1:
                    return "tier1scroll_icon";
                case 2:
                    return "tier2scroll_icon";
                case 3:
                    return "tier3scroll_icon";
                default:
                    if (scroll_tier > 3)
                        return "tier3scroll_icon";
                    else
                        return "tier1scroll_icon";
            }
        }
    }
}
