
namespace Common.Grid
{
    /// <summary>
    /// Base class for representing a grid terrain data.
    /// </summary>
    /// <typeparam name="TContext">The type of the context.</typeparam>
    public abstract class GridTerrain<TContext>
    {
        public readonly int Id;
        public readonly string Name;

        public GridTerrain(int i_Id, string i_Name)
        {
            Id = i_Id;
            Name = i_Name;
        }

        public abstract float GetCost(TContext i_Context);

        public override string ToString()
        {
            return Name;
        }
    }
}
