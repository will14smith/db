namespace SimpleDatabase.Schemas.Types
{
    public abstract class ColumnType
    {
        /// <summary>
        /// A signed 32-bit integer
        /// </summary>
        public class Integer : ColumnType { }

        /// <summary>
        /// An C string of up to N chars. It is zero terminated
        /// </summary>
        public class String : ColumnType
        {
            public int Length { get; }

            public String(int length)
            {
                Length = length;
            }
        }
    }
}