
namespace Common.Grid
{
    /// <summary>
    /// A simple implementation of grid terrain type data with a constant cost.
    /// </summary>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    /// <seealso cref="Common.Grid.GridTerrain{TContext}" />
    public class GridTerrainSimple<TContext> : GridTerrain<TContext>
    {
        public readonly float Cost;

        public GridTerrainSimple(int i_Id, string i_Name, float i_Cost) : base(i_Id, i_Name)
        {
            Cost = i_Cost;
        }

        public override float GetCost(TContext i_Context)
        {
            return Cost;
        }
    }
}
