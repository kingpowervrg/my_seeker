using System;
using System.Collections.Generic;
using UnityEngine;


public class JigsawGameData
{
    public int M_game_template_id { get; set; }
    public int M_game_dimention { get; set; }
    public Dictionary<string, JigsawChipJson> M_game_chips { get; set; }


    public JigsawChipJson GetChip(int row_, int col_)
    {
        string key = string.Format("{0}{1}", row_, col_);

        if (this.M_game_chips.ContainsKey(key))
            return this.M_game_chips[key];

        return null;
    }

}

