using System.Collections.Generic;

namespace SnakeWPF
{
public class CircularSnakeList
{
    public SnakeNode Head { get; private set; }
    public SnakeNode Tail { get; private set; }
    
    public void Grow(Position newHeadPos)
    {
        SnakeNode newNode = new SnakeNode(newHeadPos);

        if (Head == null)
        {
            // Rắn chỉ có 1 đốt, tự trỏ vào chính nó
            Head = Tail = newNode;
            Head.Next = Head;
            Head.Previous = Head;
        }
        else
        {
            // Chèn node mới vào trước Head hiện tại
            newNode.Next = Head;
            newNode.Previous = Tail;
            
            Head.Previous = newNode;
            Tail.Next = newNode;
            
            // Cập nhật lại Head
            Head = newNode;
        }
    }


    public void MoveNormal(Position newHeadPos)
    {
        if (Head == null) return;
        Tail.Value = newHeadPos;
        Head = Tail;
        Tail = Tail.Previous;
    }

    public IEnumerable<Position> GetPositions()
    {
        if (Head == null) yield break;

        SnakeNode current = Head;
        do
        {
            yield return current.Value;
            current = current.Next;
        } while (current != Head);
    }
}
}