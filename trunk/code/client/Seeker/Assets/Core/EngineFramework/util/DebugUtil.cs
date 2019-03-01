/*
 * 
 *  引擎调试类 
 *   通过宏 GOENGINE_DISABLE_DEBUG_LOG关闭
 * 
 */

using UnityEngine;
using UnityEngine.Internal;

namespace GOEngine
{
    public class DebugUtil
    {
        //
        // 摘要:
        //     ///
        //     Logs message to the Unity Console.
        //     ///
        //
        // 参数:
        //   message:
        //     String or object to be converted to string representation for display.
        //
        //   context:
        //     Object to which the message applies.
        public static void Log(object message)
        {
#if !GOENGINE_DISABLE_DEBUG_LOG
            Debug.Log(message);
#endif
        }

        //
        // 摘要:
        //     ///
        //     Logs message to the Unity Console.
        //     ///
        //
        // 参数:
        //   message:
        //     String or object to be converted to string representation for display.
        //
        //   context:
        //     Object to which the message applies.
        public static void Log(object message, Object context)
        {
#if !GOENGINE_DISABLE_DEBUG_LOG
            Debug.Log(message,context);
#endif
        }

        //
        // 摘要:
        //     ///
        //     A variant of DebugUtil.Log that logs an error message to the console.
        //     ///
        //
        // 参数:
        //   message:
        //     String or object to be converted to string representation for display.
        //
        //   context:
        //     Object to which the message applies.
        public static void LogError(object message)
        {
#if !GOENGINE_DISABLE_DEBUG_LOG
            Debug.LogError(message);
#endif 
        }

        //
        // 摘要:
        //     ///
        //     A variant of DebugUtil.Log that logs an error message to the console.
        //     ///
        //
        // 参数:
        //   message:
        //     String or object to be converted to string representation for display.
        //
        //   context:
        //     Object to which the message applies.
        public static void LogError(object message, Object context)
        {
#if !GOENGINE_DISABLE_DEBUG_LOG
            Debug.LogError(message, context);
#endif
        }

        //
        // 摘要:
        //     ///
        //     A variant of DebugUtil.Log that logs a warning message to the console.
        //     ///
        //
        // 参数:
        //   message:
        //     String or object to be converted to string representation for display.
        //
        //   context:
        //     Object to which the message applies.
        public static void LogWarning(object message)
        {
#if !GOENGINE_DISABLE_DEBUG_LOG
            Debug.LogWarning(message);
#endif
        }

        //
        // 摘要:
        //     ///
        //     A variant of DebugUtil.Log that logs a warning message to the console.
        //     ///
        //
        // 参数:
        //   message:
        //     String or object to be converted to string representation for display.
        //
        //   context:
        //     Object to which the message applies.
        public static void LogWarning(object message, Object context)
        {
#if !GOENGINE_DISABLE_DEBUG_LOG
            Debug.LogWarning(message,context);
#endif
        }

        //
        // 摘要:
        //     ///
        //     Logs a formatted message to the Unity Console.
        //     ///
        //
        // 参数:
        //   format:
        //     A composite format string.
        //
        //   args:
        //     Format arguments.
        //
        //   context:
        //     Object to which the message applies.
        public static void LogFormat(string format, params object[] args)
        {
#if !GOENGINE_DISABLE_DEBUG_LOG
            Debug.LogFormat(format, args);
#endif
        }

        //
        // 摘要:
        //     ///
        //     Logs a formatted message to the Unity Console.
        //     ///
        //
        // 参数:
        //   format:
        //     A composite format string.
        //
        //   args:
        //     Format arguments.
        //
        //   context:
        //     Object to which the message applies.
        public static void LogFormat(Object context, string format, params object[] args)
        {
#if !GOENGINE_DISABLE_DEBUG_LOG
            Debug.LogFormat(context,format, args);
#endif
        }

        //
        // 摘要:
        //     ///
        //     Logs a formatted error message to the Unity console.
        //     ///
        //
        // 参数:
        //   format:
        //     A composite format string.
        //
        //   args:
        //     Format arguments.
        //
        //   context:
        //     Object to which the message applies.
        public static void LogErrorFormat(string format, params object[] args)
        {
#if !GOENGINE_DISABLE_DEBUG_LOG
            Debug.LogErrorFormat(format, args);
#endif
        }

        //
        // 摘要:
        //     ///
        //     Logs a formatted error message to the Unity console.
        //     ///
        //
        // 参数:
        //   format:
        //     A composite format string.
        //
        //   args:
        //     Format arguments.
        //
        //   context:
        //     Object to which the message applies.
        public static void LogErrorFormat(Object context, string format, params object[] args)
        {
#if !GOENGINE_DISABLE_DEBUG_LOG
            Debug.LogErrorFormat(context,format, args);
#endif
        }

        //
        // 摘要:
        //     ///
        //     Draws a line between specified start and end points.
        //     ///
        //
        // 参数:
        //   start:
        //     Point in world space where the line should start.
        //
        //   end:
        //     Point in world space where the line should end.
        //
        //   color:
        //     Color of the line.
        //
        //   duration:
        //     How long the line should be visible for.
        //
        //   depthTest:
        //     Should the line be obscured by objects closer to the camera?
        public static void DrawLine(Vector3 start, Vector3 end)
        {
#if !GOENGINE_DISABLE_DEBUG_LOG
            Debug.DrawLine(start,end);
#endif
        }

        //
        // 摘要:
        //     ///
        //     Draws a line between specified start and end points.
        //     ///
        //
        // 参数:
        //   start:
        //     Point in world space where the line should start.
        //
        //   end:
        //     Point in world space where the line should end.
        //
        //   color:
        //     Color of the line.
        //
        //   duration:
        //     How long the line should be visible for.
        //
        //   depthTest:
        //     Should the line be obscured by objects closer to the camera?
        public static void DrawLine(Vector3 start, Vector3 end, Color color)
        {
#if !GOENGINE_DISABLE_DEBUG_LOG
            Debug.DrawLine(start, end, color);
#endif
        }

        //
        // 摘要:
        //     ///
        //     Draws a line between specified start and end points.
        //     ///
        //
        // 参数:
        //   start:
        //     Point in world space where the line should start.
        //
        //   end:
        //     Point in world space where the line should end.
        //
        //   color:
        //     Color of the line.
        //
        //   duration:
        //     How long the line should be visible for.
        //
        //   depthTest:
        //     Should the line be obscured by objects closer to the camera?
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
        {
#if !GOENGINE_DISABLE_DEBUG_LOG
            Debug.DrawLine(start, end, color, duration);
#endif
        }
        //
        // 摘要:
        //     ///
        //     Draws a line between specified start and end points.
        //     ///
        //
        // 参数:
        //   start:
        //     Point in world space where the line should start.
        //
        //   end:
        //     Point in world space where the line should end.
        //
        //   color:
        //     Color of the line.
        //
        //   duration:
        //     How long the line should be visible for.
        //
        //   depthTest:
        //     Should the line be obscured by objects closer to the camera?
        public static void DrawLine(Vector3 start, Vector3 end, [DefaultValue("Color.white")] Color color, [DefaultValue("0.0f")] float duration, [DefaultValue("true")] bool depthTest)
        {
#if !GOENGINE_DISABLE_DEBUG_LOG
            Debug.DrawLine(start, end, color, duration, depthTest);
#endif
        }

        //
        // 摘要:
        //     ///
        //     Pauses the editor.
        //     ///
        public static void Break()
        {
#if !GOENGINE_DISABLE_DEBUG_LOG
            Debug.Break();
#endif
        }
    }
}
