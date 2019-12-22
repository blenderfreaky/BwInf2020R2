namespace Afg1Stromrallye.API
{
    public class Battery
    {
        public readonly Vector2Int Position;

        public Battery(Vector2Int position)
        {
            Position = position;
        }

        public override bool Equals(object obj) => obj is Battery voxel && Position.Equals(voxel.Position);

        public override int GetHashCode()
        {
            var hashCode = -1339578552;
            hashCode = (hashCode * -1521134295) + Position.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Battery left, Battery right) => left.Equals(right);

        public static bool operator !=(Battery left, Battery right) => !(left == right);
    }
}