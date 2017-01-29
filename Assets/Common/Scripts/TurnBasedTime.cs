
namespace Common
{
    public struct TurnBasedTime
    {
        private static TurnBasedTime s_CurrentTime = new TurnBasedTime();

        public int TimeUnits;
        public int Turns;
        public int Rounds;


        static TurnBasedTime()
        {

        }

        public int GetTimePassed()
        {
            return s_CurrentTime.TimeUnits - TimeUnits;
        }

        public int GetTurnsPassed()
        {
            return s_CurrentTime.Turns - Turns;
        }

        public int GetRoundsPassed()
        {
            return s_CurrentTime.Rounds - Rounds;
        }



        public static TurnBasedTime GetNow()
        {
            return s_CurrentTime;
        }

        public static void AddTime(int i_TimeUnits)
        {
            s_CurrentTime.TimeUnits += i_TimeUnits;
        }
        public static void AddTurns(int i_Turns)
        {
            s_CurrentTime.Turns += i_Turns;
        }

        public static void AddRounds(int i_Rounds)
        {
            s_CurrentTime.Rounds += i_Rounds;
        }
    }
}
