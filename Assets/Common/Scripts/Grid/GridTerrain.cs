
namespace Common.Grid
{
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
