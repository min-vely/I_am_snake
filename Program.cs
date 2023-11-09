using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class Point
{
    public int x { get; set; }
    public int y { get; set; }
    public char sym { get; set; }

    public Point(int _x, int _y, char _sym)
    {
        x = _x;
        y = _y;
        sym = _sym;
    }

    public void Draw()
    {
        Console.SetCursorPosition(x, y);
        Console.Write(sym);
    }

    public void Clear()
    {
        sym = ' ';
        Draw();
    }

    public bool IsHit(Point p)
    {
        return p.x == x && p.y == y;
    }

    public void Move(Direction direction)
    {
        // 방향에 따라 x 또는 y 좌표를 조정하여 이동
        if (direction == Direction.LEFT)
            x--;
        else if (direction == Direction.RIGHT)
            x++;
        else if (direction == Direction.UP)
            y--;
        else if (direction == Direction.DOWN)
            y++;
    }

    public bool IsHitWall(int maxX, int maxY)
    {
        return x <= 0 || x >= maxX || y <= 0 || y >= maxY;
    }
}

public class Snake
{
    public List<Point> body;
    private Direction direction;
    private int maxX;
    private int maxY;
    private int foodCount = 0;
    private FoodCreator foodCreator;
    private Point food;

    public Snake(Point tail, int length, Direction _direction, int _maxX, int _maxY)
    {
        body = new List<Point>();
        direction = _direction;
        maxX = _maxX;
        maxY = _maxY;
        foodCreator = new FoodCreator(80, 20, '$'); // FoodCreator 초기화
        food = foodCreator.CreateFood(); // 초기 먹이 생성

        // 초기 뱀의 길이를 4로 설정
        for (int i = 3; i >= 0; i--) // 몸 길이 4개의 Point를 생성
        {
            Point p = new Point(tail.x + i, tail.y, '*');
            body.Add(p);
        }
    }

    public void Move()
    {
        Point tail = body.Last();
        Point head = GetNextPoint();
        body.Insert(0, head);

        if (Eat(food))
        {
            // 먹이를 먹었을 때만 뱀의 몸 길이를 증가시키고 새로운 먹이를 생성
            IncreaseFoodCount();
            food = foodCreator.CreateFood();
            food.Draw();
        }
        else
        {
            // 먹이를 먹지 않았을 때는 꼬리를 제거
            body.RemoveAt(body.Count - 1);
        }
    }

    public Point GetNextPoint()
    {
        Point head = body.First();
        Point nextPoint = new Point(head.x, head.y, head.sym);
        nextPoint.Move(direction);
        return nextPoint;
    }

    public void HandleKey(ConsoleKey key)
    {
        if (key == ConsoleKey.LeftArrow && direction != Direction.RIGHT)
            direction = Direction.LEFT;
        else if (key == ConsoleKey.RightArrow && direction != Direction.LEFT)
            direction = Direction.RIGHT;
        else if (key == ConsoleKey.UpArrow && direction != Direction.DOWN)
            direction = Direction.UP;
        else if (key == ConsoleKey.DownArrow && direction != Direction.UP)
            direction = Direction.DOWN;
    }

    public void Draw()
    {
        foreach (Point p in body)
        {
            p.Draw();
        }

        // 먹이를 그릴 때 먹이의 sym이 공백이 아닌 경우에만 그리도록 수정
        if (food.sym != ' ')
        {
            food.Draw();
        }
    }

    public bool IsGameOver()
    {
        Point head = body.First();

        // 머리가 벽에 부딪히는 경우
        if (head.IsHitWall(maxX, maxY))
            return true;

        // 머리가 몸에 부딪히는 경우
        for (int i = 1; i < body.Count; i++)
        {
            if (head.IsHit(body[i]))
                return true;
        }

        return false;
    }

    public bool Eat(Point food)
    {
        Point head = GetNextPoint();
        if (head.IsHit(food))
        {
            food.sym = ' ';
            return true;
        }
        return false;
    }

    public void IncreaseFoodCount()
    {
        foodCount++;
    }

    public int GetFoodCount()
    {
        return foodCount;
    }
}

public class FoodCreator
{
    private int mapWidth;
    private int mapHeight;
    private char sym;

    private Random random = new Random();

    public FoodCreator(int mapWidth, int mapHeight, char sym)
    {
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
        this.sym = sym;
    }

    public Point CreateFood()
    {
        int x = random.Next(1, 81); // X 좌표 범위를 1부터 80까지로
        int y = random.Next(1, 21); // Y 좌표 범위를 1부터 20까지로
        return new Point(x, y, sym);
    }
}

public enum Direction
{
    LEFT,
    RIGHT,
    UP,
    DOWN
}

class Program
{
    static void Main()
    {
        Console.SetWindowSize(80, 25);
        Console.SetBufferSize(80, 25);

        Point tail = new Point(4, 5, '*');
        int maxX = Console.WindowWidth;
        int maxY = Console.WindowHeight;
        Snake snake = new Snake(tail, 4, Direction.RIGHT, maxX, maxY);

        //FoodCreator foodCreator = new FoodCreator(80, 20, '$');
        //Point food = foodCreator.CreateFood();
        //food.Draw();

        while (true)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                snake.HandleKey(key.Key);
            }

            snake.Move();

            //if (snake.Eat(food))
            //{
            //    snake.IncreaseFoodCount();
            //    food = foodCreator.CreateFood();
            //    food.Draw();
            //}


            if (snake.IsGameOver())
            {
                Console.Clear();
                Console.SetCursorPosition(35, 12);
                Console.Write("Game Over");
                break;
            }

            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.Write("먹은 음식의 수 : {0}, 현재 뱀의 길이 : {1}", snake.GetFoodCount(), snake.body.Count());

            snake.Draw();
            //food.Draw();

            Thread.Sleep(300); // 이동 속도 조절을 위해 100보다 작은 값으로 수정 가능


        }
    }
}
