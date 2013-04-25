using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    class Smellqueue
    {
        private List<KeyValuePair<Tile, int>> theQueue;

        public Smellqueue()
        {
            theQueue = new List<KeyValuePair<Tile, int>>();
        }

        public KeyValuePair<Tile, int> pop_first()
        {
            Tile T = theQueue[0].Key;
            int c = theQueue[0].Value;

            theQueue.RemoveAt(0);

            return new KeyValuePair<Tile, int>(T, c);
        }

        public int in_list(Tile T)
        {
            for (int i = 0; i < theQueue.Count; i++)
                if (theQueue[i].Key.get_grid_c().x == T.get_grid_c().x &&
                   theQueue[i].Key.get_grid_c().y == T.get_grid_c().y)
                    return i;

            return -1;
        }

        public bool is_empty()
        {
            return theQueue.Count == 0;
        }

        public void sort_by_cost()
        {
            bool sorted = false;
            while (!sorted)
            {
                sorted = true;
                for (int i = 0; i < theQueue.Count-1; i++)
                {
                    if (theQueue[i].Value > theQueue[i + 1].Value)
                    {
                        swap(i, i + 1);
                        sorted = false;
                    }
                }
            }
        }

        public void swap(int swap_this, int with_this)
        {
            KeyValuePair<Tile, int> first_pair = theQueue[swap_this];

            theQueue[swap_this] = theQueue[with_this];
            theQueue[with_this] = first_pair;
        }

        public void add_to_end(Tile T, int C)
        {
            theQueue.Add(new KeyValuePair<Tile,int>(T, C));
            sort_by_cost();
        }

        public void update_if_smaller_cost(int ind, Tile T, int C)
        {
            if (theQueue[ind].Value < C)
            {
                theQueue.RemoveAt(ind);
                add_to_end(T, C);
                sort_by_cost();
            }
        }

        public List<Tile> return_all_tiles()
        {
            List<Tile> retList = new List<Tile>();

            for (int i = 0; i < theQueue.Count; i++)
                retList.Add(theQueue[i].Key);

            return retList;
        }
    }
}
