
using Common;
using Common.Grid;
using System.Collections.Generic;

namespace Game.Grid
{
    public class GameGridTerrain : GridTerrain<GameGridChangeContext>
    {
        private static readonly List<GameGridTerrain> s_All = new List<GameGridTerrain>();

        public readonly float Cost;

        private GameGridTerrain(int i_Id, string i_Name, float i_Cost) : base(i_Id, i_Name)
        {
            Cost = i_Cost;
        }

        public override float GetCost(GameGridChangeContext i_Context)
        {
            return Cost;
        }


        public static GameGridTerrain Get(int i_Id)
        {
            return s_All[i_Id];
        }

        public static GameGridTerrain Rerister(string i_Name, float i_Cost)
        {
            Log.DebugAssert(!string.IsNullOrEmpty(i_Name), "GameGridTerrain::Register given invalid name");
            var result = new GameGridTerrain(s_All.Count, i_Name, i_Cost);
            s_All.Add(result);
            return result;
        }
        
        public static GameGridTerrain Find(string i_Name)
        {
            int count = s_All.Count;
            for(int i = 0; i < count; ++i)
            {
                if(s_All[i].Name == i_Name)
                {
                    return s_All[i];
                }
            }
            return null;
        }
    }
}
