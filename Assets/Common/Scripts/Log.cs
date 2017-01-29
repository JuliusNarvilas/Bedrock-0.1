#if DEBUG_LOGS_OFF
    #undef DEBUG_LOGS
    #if !DEBUG_ASSERTS
        #define DEBUG_ASSERTS_OFF
    #endif
#elif UNITY_EDITOR || DEBUG || DEBUG_LOGS
    #define DEBUG_LOGS
    #if DEBUG_ASSERTS_OFF
        #undef DEBUG_ASSERTS
    #else
        #define DEBUG_ASSERTS
    #endif
#else
    #define DEBUG_LOGS_OFF
    #if !DEBUG_ASSERTS
        #define DEBUG_ASSERTS_OFF
    #endif
#endif

#if PRODUCTION_LOGS_OFF
    #undef PRODUCTION_LOGS
    #if !PRODUCTION_ASSERTS
        #define PRODUCTION_ASSERTS_OFF
    #endif
#else
    #define PRODUCTION_LOGS
    #define PRODUCTION_ASSERTS
#endif

using System.Diagnostics;

namespace Common
{
    public class Log
    {
        public delegate void LoggerOutputLogTargetFunc(string i_Message, params object[] i_Args);
        public delegate void LoggerOutputAssertTargetFunc(bool i_Assert, string i_Message, params object[] i_Args);

        private LoggerOutputLogTargetFunc m_Log;
        private LoggerOutputLogTargetFunc m_Warning;
        private LoggerOutputLogTargetFunc m_Error;
        private LoggerOutputAssertTargetFunc m_Assert;

        //Default assert function to bypass Unity overloaded function group assignment problems
        private static void DefaultAssertFunc(bool i_Assert, string i_Message, params object[] i_Args)
        {
            UnityEngine.Debug.AssertFormat(i_Assert, i_Message, i_Args);
        }

        public Log(LoggerOutputLogTargetFunc i_Log, LoggerOutputLogTargetFunc i_Warning, LoggerOutputLogTargetFunc i_Error, LoggerOutputAssertTargetFunc i_Assert)
        {
            m_Log = i_Log;
            m_Warning = i_Warning;
            m_Error = i_Error;
            m_Assert = i_Assert;
        }

        [Conditional("DEBUG_LOGS")]
        public static void DebugLog(string i_Message, params object[] i_Args)
        {
            Debug.m_Log(i_Message, i_Args);
        }
        [Conditional("DEBUG_LOGS")]
        public static void DebugLogIf(bool i_Condition, string i_Message, params object[] i_Args)
        {
            if (i_Condition)
            {
                DebugLog(i_Message, i_Args);
            }
        }

        [Conditional("DEBUG_LOGS")]
        public static void DebugLogWarning(string i_Message, params object[] i_Args)
        {
            Debug.m_Warning(i_Message, i_Args);
        }
        [Conditional("DEBUG_LOGS")]
        public static void DebugLogWarningIf(bool i_Condition, string i_Message, params object[] i_Args)
        {
            if(i_Condition)
            {
                DebugLogWarning(i_Message, i_Args);
            }
        }

        [Conditional("DEBUG_LOGS")]
        public static void DebugLogError(string i_Message, params object[] i_Args)
        {
            Debug.m_Error(i_Message, i_Args);
        }
        [Conditional("DEBUG_LOGS")]
        public static void DebugLogErrorIf(bool i_Condition, string i_Message, params object[] i_Args)
        {
            if (i_Condition)
            {
                DebugLogError(i_Message, i_Args);
            }
        }

        [Conditional("DEBUG_ASSERTS")]
        public static void DebugAssert(bool i_Assertion, string i_Message, params object[] i_Args)
        {
            Debug.m_Assert(i_Assertion, i_Message, i_Args);
        }




        [Conditional("PRODUCTION_LOGS")]
        public static void ProductionLog(string i_Message, params object[] i_Args)
        {
            Production.m_Log(i_Message, i_Args);
        }
        [Conditional("PRODUCTION_LOGS")]
        public static void ProductionLogIf(bool i_Condition, string i_Message, params object[] i_Args)
        {
            if (i_Condition)
            {
                ProductionLog(i_Message, i_Args);
            }
        }

        [Conditional("PRODUCTION_LOGS")]
        public static void ProductionLogWarning(string i_Message, params object[] i_Args)
        {
            Production.m_Warning(i_Message, i_Args);
        }
        [Conditional("PRODUCTION_LOGS")]
        public static void ProductionLogWarningIf(bool i_Condition, string i_Message, params object[] i_Args)
        {
            if (i_Condition)
            {
                ProductionLogWarning(i_Message, i_Args);
            }
        }

        [Conditional("PRODUCTION_LOGS")]
        public static void ProductionLogError(string i_Message, params object[] i_Args)
        {
            Production.m_Error(i_Message, i_Args);
        }
        [Conditional("PRODUCTION_LOGS")]
        public static void ProductionLogErrorIf(bool i_Condition, string i_Message, params object[] i_Args)
        {
            if (i_Condition)
            {
                ProductionLogError(i_Message, i_Args);
            }
        }

        [Conditional("PRODUCTION_ASSERTS")]
        public static void ProductionAssert(bool i_Assertion, string i_Message, params object[] i_Args)
        {
            Debug.m_Assert(i_Assertion, i_Message, i_Args);
        }

        public static readonly Log Default = new Log(
            UnityEngine.Debug.LogFormat, UnityEngine.Debug.LogWarningFormat, UnityEngine.Debug.LogErrorFormat, DefaultAssertFunc
        );

        public static Log Debug = Default;
        public static Log Production = Default;
    }
}
