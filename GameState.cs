namespace SnakeWPF
{
    using System;
    using System.Collections.Generic;

    public class GameState
    {
        public int Rows { get; }
        public int Cols { get; }
        public GridValue[,] Grid { get; }
        public Direction Dir { get; private set; }
        public int Score { get; private set; }
        public bool GameOver { get; private set; }

        private readonly LinkedList<Direction> _dirChanges = new();
        
        // 1. Đã sửa tên biến
        private CircularSnakeList _snakePositions = new CircularSnakeList();
        private readonly Random _random = new Random();

        public GameState(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Grid = new GridValue[Rows, Cols];
            Dir = Direction.Right;

            AddSnake();
            AddFood();
        }

        private void AddSnake()
        {
            int r = Rows / 2;
            for (int c = 1; c <= 3; c++)
            {
                Grid[r, c] = GridValue.Snake;
                _snakePositions.Grow(new Position(r, c));
            }
        }

        private IEnumerable<Position> EmptyPositions()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Grid[r, c] == GridValue.Empty)
                        yield return new Position(r, c);
                }
            }
        }

        private void AddFood()
        {
            List<Position> empty = new List<Position>(EmptyPositions());

            if (empty.Count == 0)
                return;

            Position pos = empty[_random.Next(empty.Count)];
            Grid[pos.Row, pos.Col] = GridValue.Food;
        }

        public Position HeadPosition() => _snakePositions.Head.Value;

        // 2. Đã sửa .Last thành .Tail
        public Position TailPosition() => _snakePositions.Tail.Value;

        // 3. Đã gọi hàm GetPositions()
        public IEnumerable<Position> SnakePositions() => _snakePositions.GetPositions();

        // (Đã xóa 2 hàm AddHead và RemoveTail thừa)

        private Direction GetLastDirection()
        {
            if (_dirChanges.Count == 0)
                return Dir;

            return _dirChanges.Last.Value;
        }

        private bool CanChangeDir(Direction newDir)
        {
            if (_dirChanges.Count == 2)
                return false;

            Direction lastDir = GetLastDirection();
            return newDir != lastDir && newDir != lastDir.Opposite();
        }

        public void ChangeDirection(Direction dir)
        {
            if (CanChangeDir(dir))
            {
                _dirChanges.AddLast(dir);
            }
        }

        private bool OutsideGrid(Position pos) => pos.Row < 0 || pos.Row >= Rows || pos.Col < 0 || pos.Col >= Cols;

        private GridValue WillHit(Position newHeadPos)
        {
            if (OutsideGrid(newHeadPos))
                return GridValue.Outside;

            if (newHeadPos == TailPosition())
                return GridValue.Empty;

            return Grid[newHeadPos.Row, newHeadPos.Col];
        }

        public void Move()
        {
            if (_dirChanges.Count > 0)
            {
                Dir = _dirChanges.First.Value;
                _dirChanges.RemoveFirst();
            }

            Position newHeadPos = HeadPosition().Translate(Dir);
            GridValue hit = WillHit(newHeadPos);

            if (hit == GridValue.Outside || hit == GridValue.Snake)
            {
                GameOver = true;
            }
            else if (hit == GridValue.Empty)
            {
                Position tailPos = _snakePositions.Tail.Value; 
                
                // 4. Đã sửa .Column thành .Col
                Grid[tailPos.Row, tailPos.Col] = GridValue.Empty;

                _snakePositions.MoveNormal(newHeadPos);

                Grid[newHeadPos.Row, newHeadPos.Col] = GridValue.Snake;
            }
            else if (hit == GridValue.Food)
            {
                _snakePositions.Grow(newHeadPos);
                
                Grid[newHeadPos.Row, newHeadPos.Col] = GridValue.Snake;
                
                Score++;
                AddFood();
            }
        }
    }
}