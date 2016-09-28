using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2DNesting.DataStruct
{
    class DataStruct
    {
      
        
    }
    public struct Point {
       public double x;
       public double y;
       public  Point(double x, double y) {
            this.x = x;
            this.y = y;
        }
    
    }
    public struct Line
    {
      public  Point startPoint;
      public Point endPoint;
      public Line(Point s, Point e) {
            this.startPoint = s;
            this.endPoint = e;
        }
      //public Line() {
      //    this.startPoint = this.endPoint = new Point(0, 0);
      //}
    }
    public struct Angle
    {
        public  Point startPoint;
        public  Point midPoint;
        public  Point endPoint;
        public Line inLine;
        public Line outLine;
        public  Angle (Point s,Point m,Point e)
         {
             startPoint=s;
             midPoint=m;
             endPoint=e;
             inLine = new Line(s, m);
             outLine = new Line(m, e);
          }
        public Angle(Line inline,Line outLine)
        {
            startPoint = inline.startPoint;
            midPoint = inline.endPoint;
            endPoint =outLine.endPoint;
            this.inLine = inline;
            this.outLine = outLine;
        }
    }
    public struct Polygon
    { 
         public Polygon(LinkedList<Point> vertex, bool isConvex, bool isRotateChange, Point lowLeft)
        {
            this.vertex = vertex;
            this.isConvex = isConvex;
            this.isRotateChange = isRotateChange;
            this.lowLeft = lowLeft;
        }
        public Polygon(LinkedList<Point> vertex)
        {
            this.vertex = vertex;
            this.isConvex = false;
            this.isRotateChange = false;
            this.lowLeft = new Point(0.0, 0.0);
        }
        public LinkedList<Point> vertex;
        public bool isConvex;
        public bool isRotateChange;
        public Point lowLeft;
        public void Move(Point start, Point end)
        {
            double offsetX = end.x - start.x;
            double offsetY = end.y - start.y;
            LinkedListNode<Point> current = this.vertex.First;
            Point temp;
            while (current != this.vertex.Last)
            {
                temp = current.Value;
                temp.x = temp.x + offsetX;
                temp.y = temp.y + offsetY;
                current.Value = temp;
                current = current.Next;
            }
            temp = current.Value;
            temp.x = temp.x + offsetX;
            temp.y = temp.y + offsetY;
            current.Value = temp;
        }
        public void Rotate(int angle)
        {
            if (angle == 180)//逆时针转动180度
            {
                LinkedListNode<Point> current = this.vertex.First;
                Point temp;
                while (current != this.vertex.Last)
                {
                    temp = current.Value;
                    temp.x = -temp.x;
                    temp.y = -temp.y;
                    current.Value = temp;
                    current = current.Next;
                }
                temp = current.Value;
                temp.x = -temp.x;
                temp.y = -temp.y;
                current.Value = temp;
                //测试shapes的时候屏蔽 
            }
            else
                throw new Exception("使用Rotate函数，参数只能为180");
        }
    
    }
      public struct Piece
    {
        public Piece(int type, Polygon parts, int num)
        {
            this.type = type;
            this.parts = parts;
            this.num = num;
        }
        public int type;
        public Polygon parts;
        public int num;
    }
}
