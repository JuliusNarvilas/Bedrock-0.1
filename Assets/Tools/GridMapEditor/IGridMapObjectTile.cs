

namespace Tools
{
    public interface IGridMapObjectTile<TPosition, TTileSettings>
    {
        TPosition Position { get; }

        TTileSettings Settings { get; }
    }
}
