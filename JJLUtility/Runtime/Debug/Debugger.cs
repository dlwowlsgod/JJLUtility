using UnityEngine;

namespace JJLUtility
{
    /// <summary>
    /// A class that provides a performance improvement when using UnityEngine.Debug at runtime.
    /// Additional runtime debugging can be achieved through preprocessor extension processing.
    /// </summary>
    public static class Debugger
    {
        /// <summary>
        /// Logs a message to the Unity console with an optional context and tag.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="context">The optional Unity Object context associated with the log.</param>
        /// <param name="tag">The optional tag used to categorize the log message.</param>
        public static void Log(object message, Object context = null, string tag = "UnityXOPS")
        {
#if UNITY_EDITOR
            Debug.Log($"[{tag}] {message}", context);
#else

#endif
        }

        /// <summary>
        /// Logs a warning message to the Unity console with an optional context and tag.
        /// </summary>
        /// <param name="message">The warning message to be logged.</param>
        /// <param name="context">The optional Unity Object context associated with the log.</param>
        /// <param name="tag">The optional tag used to categorize the warning message.</param>
        public static void LogWarning(object message, Object context = null, string tag = "UnityXOPS")
        {
#if UNITY_EDITOR
            Debug.LogWarning($"[{tag}] {message}", context);
#else
            
#endif
        }

        /// <summary>
        /// Logs an error message to the Unity console with an optional context and tag.
        /// </summary>
        /// <param name="message">The error message to be logged.</param>
        /// <param name="context">The optional Unity Object context associated with the log.</param>
        /// <param name="tag">The optional tag used to categorize the error message.</param>
        public static void LogError(object message, Object context = null, string tag = "UnityXOPS")
        {
#if UNITY_EDITOR
            Debug.LogError($"[{tag}] {message}", context);
#else
            
#endif
        }
    }
}