namespace SnakeWPF
{
    public class SnakeNode
    {
        public Position Value { get; set; }
        public SnakeNode Next { get; set; }
        public SnakeNode Previous { get; set; }

        public SnakeNode(Position value)
        {
            Value = value;
        }
    }
}