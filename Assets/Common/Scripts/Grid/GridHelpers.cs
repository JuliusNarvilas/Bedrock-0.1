

namespace Common.Grid
{
    /// <summary>
    /// Available settings for tile blocker height and other options.
    /// </summary>
    public enum EGridTileSettings : int
    {
        None = 0,
        BlockerExtraSmall = 1,
        BlockerSmall = 2,
        BlockerMedium = 3,
        BlockerMediumLarge = 4,
        BlockerLarge = 5,
        BlockerExtraLarge = 6,

        /// <summary>
        /// Indicates the maximum number of bits that can be used for actual <see cref="EGridTileSettings"/> values.
        /// 3 bits give the range of [0;7]
        /// </summary>
        BlockerBitStride = 3,
        /// <summary>
        /// The highest value within the given <see cref="BlockerBitStride"/> number of bits.
        /// (All the available bits set to 1).
        /// </summary>
        BlockerFullySetStride = 7,

        
        CustomSettingsStart = 1 << (BlockerBitStride + 1)
    }

    public static class GridHelpers
    {
        /// <summary>
        /// A map for associating <see cref="GridTileSettings"/> enum index to the enum value.
        /// Used for conversion between real data and Unity's inspector simple enum interface options.
        /// </summary>
        public static readonly int[] BLOCKER_ENUM_VALUE_MAP = new int[] {
            (int)EGridTileSettings.None,
            (int)EGridTileSettings.BlockerExtraSmall,
            (int)EGridTileSettings.BlockerSmall,
            (int)EGridTileSettings.BlockerMedium,
            (int)EGridTileSettings.BlockerMediumLarge,
            (int)EGridTileSettings.BlockerLarge,
            (int)EGridTileSettings.BlockerExtraLarge,

            (int)EGridTileSettings.CustomSettingsStart
        };

        /// <summary>
        /// Conversion from Unity's inspector flag options to real compressed data.
        /// </summary>
        /// <param name="i_IndexFlags">Flag mask indicating <see cref="GridTileSettings"/> enum index (not actual value) selections.</param>
        /// <returns>Real compressed blocker data.</returns>
        public static int BlockerIndexFlagsToValue(int i_IndexFlags)
        {
            int result = 0;
            for (int i = 0; i < BLOCKER_ENUM_VALUE_MAP.Length; ++i)
            {
                if ((i_IndexFlags & (1 << i)) != 0)
                {
                    result |= BLOCKER_ENUM_VALUE_MAP[i];
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i_Value"></param>
        /// <returns></returns>
        public static int BlockerValueToIndexFlags(int i_Value)
        {
            bool startedAsZero = i_Value == 0;
            int result = 0;
            for (int i = BLOCKER_ENUM_VALUE_MAP.Length - 1; i >= 0; --i)
            {
                int enumBlockerValue = BLOCKER_ENUM_VALUE_MAP[i];
                if ((i_Value & enumBlockerValue) == enumBlockerValue)
                {
                    if (enumBlockerValue == 0 && startedAsZero)
                        continue;
                    result |= 1 << i;
                    i_Value ^= enumBlockerValue;
                }
            }
            return result;
        }
    }


    /**
    TODO: GridObject Serialization stuff

    public abstract class GridObject<TPosition, TTileData, TContext>
    {
        public Vector3 Position;
        public Vector3 Scale;
        public Quaternion Rotation;

        public TPosition Origin;
        public int Settings;
        public List<PairStruct<TPosition, TTileData>> DataList;

        public List<GridObject<TPosition, TTileData, TContext>> Children;

        public abstract void Damage(IGridControl<TPosition, TTileData, TContext> i_Grid, TPosition i_Position, TContext i_Context);
    }
    **/
}
