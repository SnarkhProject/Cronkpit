using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cronkpit
{
    class MossyPatch
    {
        public gridCoordinate grid_position;
        public int width;
        public int height;
        public List<List<bool>> mossConfig;

        Random rGen;

        public MossyPatch(int s_w_val, int s_h_val, gridCoordinate s_grid_pos, ref Random s_r_gen)
        {
            grid_position = s_grid_pos;
            width = s_w_val;
            height = s_h_val;

            rGen = s_r_gen;

            mossConfig = new List<List<bool>>();
            for (int x = 0; x < width; x++)
            {
                List<bool> nextColumn = generate_new_column();

                if (x == 0)
                    mossConfig.Add(nextColumn);
                else
                {
                    if(!test_for_valid_pattern(nextColumn, x-1))
                        while (!test_for_valid_pattern(nextColumn, x - 1))
                            nextColumn = generate_new_column();
                    
                    mossConfig.Add(nextColumn);
                }
            }
            
        }

        bool test_for_valid_pattern(List<bool> row_to_Test, int index)
        {
            bool is_valid = false;

            for (int i = 0; i < height; i++)
            {
                if (mossConfig[index][i] == true && row_to_Test[i] == true)
                    is_valid = true;
            }

            return is_valid;
        }

        List<bool> generate_new_column()
        {
            List<bool> nCol = new List<bool>();
            int moss_height = rGen.Next(Math.Min(height, 2), height + 1);
            int moss_indent = rGen.Next((height - moss_height + 1));

            for (int i = 0; i < moss_indent; i++)
                nCol.Add(false);

            for (int i = 0; i < moss_height; i++)
                nCol.Add(true);

            for (int i = moss_indent + moss_height; i < height; i++)
                nCol.Add(false);

            return nCol;
        }
    }
}
