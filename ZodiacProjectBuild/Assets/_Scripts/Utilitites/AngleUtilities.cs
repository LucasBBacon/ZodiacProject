using UnityEngine;

namespace Utilities
{
    public static class AngleUtilities
    {
        /// <summary>
        /// Calculates the angle of a line created by two transforms from the positive or negative x-axis.
        /// </summary>
        /// <param name="receiver">Transform line is drawn to.</param>
        /// <param name="source">Transform line is drawn from.</param>
        /// <param name="direction">Direction of x-axis.</param>
        /// <returns></returns>
        public static float AngleFromFacingDirection(Transform receiver, Transform source, int direction)
        => Vector2.SignedAngle(
            Vector2.right * direction,
            source.position - receiver.position
            ) * direction;
    }
}

