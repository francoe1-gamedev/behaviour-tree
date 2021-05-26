using UnityEngine;

namespace BehaviourTree
{
    internal static class BehaviourTreeSetupValue
    {
        public static float ANIMATION_SPEED { get; } = .025f;
        public static float ALPHA_DEFAULT { get; } = .3f;
        public static float INTENSITY_COLOR { get; } = .4f;
        public static Color EXIT_COLOR { get; } = new Color(INTENSITY_COLOR, INTENSITY_COLOR, INTENSITY_COLOR);
        public static Color RUN_COLOR { get; } = new Color(INTENSITY_COLOR, INTENSITY_COLOR, 0);
        public static Color FAIL_COLOR { get; } = new Color(INTENSITY_COLOR, 0, 0);
        public static Color SUCCESS_COLOR { get; } = new Color(0, INTENSITY_COLOR, 0);
        public static Color STOP_COLOR { get; } = new Color(0, INTENSITY_COLOR, INTENSITY_COLOR);
        public static Color GRAPH_BACKGROUND_COLOR { get; } = new Color(INTENSITY_COLOR / 4, INTENSITY_COLOR / 4, INTENSITY_COLOR / 4);
    }    
}