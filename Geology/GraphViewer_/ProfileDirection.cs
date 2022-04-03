using Geology.Observing;

namespace Geology.GraphViewer
{
    public enum ProfileDirection
    {
        X,
        Y
    }

    public class ProfileDirectionHelper
    {
        public static double FromPosition(ProfileDirection direction, ObservingPosition pos)
        {
            switch (direction)
            {
                case ProfileDirection.X:
                    return pos.X;
                case ProfileDirection.Y:
                    return pos.Y;
                default:
                    return pos.X;
            }
        }
    }
}
