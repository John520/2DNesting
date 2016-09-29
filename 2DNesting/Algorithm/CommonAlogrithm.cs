using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using  _2DNesting.DataStruct;
namespace _2DNesting.Algorithm
{
    static class CommonAlogrithm
    {
        public  static double ACCURACY = 0.0000000001;
        /// <summary>
        /// 计算两个向量的叉乘 a×b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double cross(Line a, Line b)
        {
            double x1 = a.endPoint.x - a.startPoint.x;
            double y1 = a.endPoint.y - a.startPoint.y;
            double x2 = b.endPoint.x - b.startPoint.x;
            double y2 = b.endPoint.y - b.startPoint.y;
            return x1 * y2 - x2 * y1;
        }
       


        public static bool IsConvex(Polygon p)
        {
            int num = p.vertex.Count;//顶点个数
            LinkedListNode<Point> current = p.vertex.Last;
            for (int vertexIndex = 0; vertexIndex < num; vertexIndex++)
            {
                LinkedListNode<Point> temp = NextPoint(current, p.vertex);
                if (CrossDirection(new Line(current.Value, temp.Value),
                                            new Line(temp.Value, NextPoint(temp, p.vertex).Value)) == 2)
                    return false;
                current = NextPoint(current, p.vertex);
            }
            return true;
        }

        //求多边形的包络矩形——注意这里求的不一定是最小包络矩形！
        public static Rectangle Enclosure(Polygon p)
        {
            LinkedListNode<Point> current = p.vertex.First;
            double minX = current.Value.x;
            double minY = current.Value.y;
            double maxX = current.Value.x;
            double maxY = current.Value.y;
            current = current.Next;
            while (current != null)
            {
                if (current.Value.x < minX)
                    minX = current.Value.x;
                if (current.Value.y < minY)
                    minY = current.Value.y;
                if (current.Value.x > maxX)
                    maxX = current.Value.x;
                if (current.Value.y > maxY)
                    maxY = current.Value.y;
                current = current.Next;
            }
            return new Rectangle(new Point(minX, minY), new Point(maxX, maxY));
        }
        public static Rectangle Enclosure(Polygon a,Polygon b)
        {
            LinkedListNode<Point> current = a.vertex.First;
            double minX = current.Value.x;
            double minY = current.Value.y;
            double maxX = current.Value.x;
            double maxY = current.Value.y;
            current = current.Next;
            while (current != null)
            {
                if (current.Value.x < minX)
                    minX = current.Value.x;
                if (current.Value.y < minY)
                    minY = current.Value.y;
                if (current.Value.x > maxX)
                    maxX = current.Value.x;
                if (current.Value.y > maxY)
                    maxY = current.Value.y;
                current = current.Next;
            }
            current = b.vertex.First;
            while (current != null)
            {
                if (current.Value.x < minX)
                    minX = current.Value.x;
                if (current.Value.y < minY)
                    minY = current.Value.y;
                if (current.Value.x > maxX)
                    maxX = current.Value.x;
                if (current.Value.y > maxY)
                    maxY = current.Value.y;
                current = current.Next;
            }




            return new Rectangle(new Point(minX, minY), new Point(maxX, maxY));
        }





        public static Polygon CopyPolygon(Polygon polygon)
        {
            LinkedList<Point> list = new LinkedList<Point>();
            LinkedListNode<Point> current = polygon.vertex.First;
            while (current != null)
            {
                list.AddLast(current.Value);//这样是否是真正的复制，还是只是拷贝了引用？
                current = current.Next;
            }
            return new Polygon(list, polygon.isConvex, polygon.isRotateChange, polygon.lowLeft);
        }
        public static  List<Polygon> CopyPolygonList(List<Polygon> polygonList)
        {
            List<Polygon> copy = new List<Polygon>();
            for (int i = 0; i < polygonList.Count; i++)
            {
                copy.Add(CopyPolygon(polygonList[i]));
            }
            return copy;
        }
        public static bool IsRotateChange(Polygon polygon)
        {
            Polygon rotatePolygon = CopyPolygon(polygon);//直接赋值两个是引用，LinkedList是引用
            rotatePolygon.Rotate(180);
            Point end = polygon.lowLeft;
            Point start = LowLeftPoint(rotatePolygon);//重新找到最下最左点
            rotatePolygon.Move(start, end);

            Point lowLeft1 = end;
            Point lowLeft2 = end;

            LinkedListNode<Point> current1 = polygon.vertex.First;
            LinkedListNode<Point> current2 = rotatePolygon.vertex.First;

            while (!IsSamePoint(current1.Value, lowLeft1))
                current1 = NextPoint(current1, polygon.vertex);
            while (!IsSamePoint(current2.Value, lowLeft2))
                current2 = NextPoint(current2, rotatePolygon.vertex);

            int num = 0;
            int total = polygon.vertex.Count;
            while (num < total)
            {
                if (!IsSamePoint(current1.Value, current2.Value))
                {
                    return true;
                }
                num++;
                current1 = NextPoint(current1, polygon.vertex);
                current2 = NextPoint(current2, rotatePolygon.vertex);
            }
            return false;
        }
        /// <summary>
        /// 判断内角是否大于180°即优角（优角180-360(以逆时针为参考方向）返回true
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static bool isReflexAngle(Angle angle)
        {
            Line inLine = new Line(angle.startPoint, angle.midPoint);
            Line outLine = new Line(angle.midPoint, angle.endPoint);
            if (cross(inLine, outLine) > 0)
                return false;
            else
                return true;

        }
        public static Line reverseLine(Line l)
        {
            return new Line(l.endPoint, l.startPoint);
        }
        //平移向量l
        public static Line translateLine(Line l, Point start, Point end)
        {
            double offsetX = end.x - start.x;
            double offsetY = end.y - start.y;
            Point newEnd = new Point(l.endPoint.x + offsetX, l.endPoint.y + offsetY);
            Point newStart = new Point(l.startPoint.x + offsetX, l.startPoint.y + offsetY);
            return new Line(newStart, newEnd);
        }
        //平移向量l 使得起点与点p重合
        public static Line translateLine(Line l, Point p)
        {
            double offsetX = p.x - l.startPoint.x;
            double offsetY = p.y - l.startPoint.y;
            Point newEnd = new Point(l.endPoint.x + offsetX, l.endPoint.y + offsetY);
            Point newStart = new Point(l.startPoint.x + offsetX, l.startPoint.y + offsetY);
            return new Line(newStart, newEnd);
        }
        public static bool IsSamePoint(Point p1, Point p2)
        {
            return Math.Abs(p1.x - p2.x) < 0.0000001 && Math.Abs(p1.y - p2.y) < 0.0000001;
        }
        public static bool IsSamePolygon(Polygon p1, Polygon p2) {
            if(p1.vertex.Count!=p2.vertex.Count){
                 return false;            
            }
            LinkedListNode<Point> p1_currentNode=p1.vertex.First;
            LinkedListNode<Point> p2_currentNode=p2.vertex.First;
            //找到相同的点
            if (!IsSamePoint(p1_currentNode.Value, p1_currentNode.Value))
            {
                for (int i = 0; i < p2.vertex.Count; i++)
                { 
                 p2_currentNode=NextPoint(p2_currentNode,p2.vertex);
                 if (IsSamePoint(p1_currentNode.Value, p1_currentNode.Value)) { break; }
                }
                //p2顶点集中未找到与p1第一个顶点一致的点
                if (!IsSamePoint(p1_currentNode.Value, p1_currentNode.Value))
                {
                    return false;
                }
                
            }
            
            for (int i = 0; i < p1.vertex.Count; i++) 
            {//判断是否每个点都相同
                if (IsSamePoint(p1_currentNode.Value, p2_currentNode.Value))
                {
                    p1_currentNode = NextPoint(p1_currentNode, p1.vertex);
                    p2_currentNode=NextPoint(p2_currentNode,p2.vertex);
                }else{
                return false;
                }
                    
            }
            return true;
           
        
        
        
        }

        //找到该接触点在crossPointList中的位置，并移除该点；接触点用完，bool合成结束
        public static void RemovePointInList(Point p, List<Point> list)
        {
            for (int i = 0; i < list.Count; i++)//找到该接触点在crossPointList中的位置，并移除该点
            {
                if (IsSamePoint(list[i], p))
                {
                    list.RemoveAt(i);
                    break;
                }
            }
        }
        public static double Distance(Point p1, Point p2)//求点p1到p2的距离endPoint
        {
            return Math.Sqrt(Math.Pow((p1.x - p2.x), 2) + Math.Pow((p1.y - p2.y), 2));
        }

        public static void BubbleUp(List<double> list)//冒泡排序，从小到大排列
        {
            double temp = 0.0;
            for (int i = list.Count; i > 0; i--)
            {
                for (int j = 0; j < i - 1; j++)
                {
                    if (list[j] > list[j + 1])
                    {
                        temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;
                    }
                }
            }
        }
        public static List<Point> SortPointUp(List<Point> p, Point start)//按照与起始点的距离从小到大排序
        {
            List<Point> temp = new List<Point>();
            List<double> distance = new List<double>();
            for (int i = 0; i < p.Count; i++)
            {
                distance.Add(Distance(p[i], start));
            }
            BubbleUp(distance);//
            for (int i = 0; i < distance.Count; i++)
            {
                for (int j = 0; j < p.Count; j++)
                {
                    if (Distance(p[j], start) == distance[i])
                    {
                        temp.Add(p[j]);
                        break;
                    }
                }
            }
            return temp;
        }
        //判断点是否在线段上
        public static bool IsPointOnLine(Line line, Point point)
        {
            double accuracy = 0.00001;
            //包围盒测试
            if (point.x > Math.Max(line.startPoint.x, line.endPoint.x) + accuracy
                || point.y > Math.Max(line.startPoint.y, line.endPoint.y) + accuracy
                || point.x < Math.Min(line.startPoint.x, line.endPoint.x) - accuracy
                || point.y < Math.Min(line.startPoint.y, line.endPoint.y) - accuracy)//留下一定的余量
            {
                return false;
            }
            double s = (point.x * line.startPoint.y - point.y * line.startPoint.x
                                + line.startPoint.x * line.endPoint.y - line.startPoint.y * line.endPoint.x
                                + line.endPoint.x * point.y - line.endPoint.y * point.x) / 2;//利用什么公式？向量叉乘求平行四边形面积
            double distance = 2 * s / Distance(line.startPoint, line.endPoint);

            if (Math.Abs(distance) > 0.0000001)//用距离判断比较合理！   
            {
                return false;
            }
            else
            {
                //if 肯定false？？
                if ((point.x - line.startPoint.x) * (point.x - line.endPoint.x) > 0.000001
                    || (point.y - line.startPoint.y) * (point.y - line.endPoint.y) > 0.000001)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
            /// <summary>
        /// 面积法判断拐向，其实是把原来的一个叉积判别，改为三个叉积，更准确但是时间更长！
        /// 1左拐   2右拐  3同向共线   4反向共线
        /// </summary>
        /// <param name="a"></param>适量线a
        /// <param name="b"></param>适量线b
        /// <returns>1左拐   2右拐  3同向共线   4反向共线 </returns>    
        public  static int CrossDirection(Line a, Line b)
        {
            double accrucy = 0.000001;//2014.2.8从0.00001改为0.000001_不然凸凸NFP出错
            Line c = new Line(b.endPoint, a.startPoint);
            double result = cross(a, b) + cross(b, c) + cross(c, a);
            //double result = CrossResult(a, b);//2014.6.10
            if (Math.Abs(result) < accrucy)
            {
                if ((a.endPoint.x - a.startPoint.x) * (b.endPoint.x - b.startPoint.x) > accrucy
                    || (a.endPoint.y - a.startPoint.y) * (b.endPoint.y - b.startPoint.y) > accrucy)
                {
                    return 3;//同向共线
                }
                else
                {
                    return 4;//反向共线
                }
            }
            else if (result > accrucy)
            {
                return 1;//叉积向外,左拐
            }
            else
            {
                return 2;//叉积向内，右拐
            }
        }


        /// <summary>
        /// 判断两个向量的关系    1相交  2相离  3部分重叠  4完全重叠   5包含关系
        /// </summary>
        /// <param name="la"></param>
        /// <param name="lb"></param>
        /// <returns></returns>
        public static int LineRelation(Line a, Line b)//两条线段的关系，1相交  2相离  3部分重叠  4完全重叠   5包含关系
        {
            //包围盒测试法
            double accuracy = 0.00001;
            if (Math.Min(a.startPoint.x, a.endPoint.x) > Math.Max(b.startPoint.x, b.endPoint.x) + accuracy
                || Math.Max(a.startPoint.x, a.endPoint.x) < Math.Min(b.startPoint.x, b.endPoint.x) - accuracy
                || Math.Min(a.startPoint.y, a.endPoint.y) > Math.Max(b.startPoint.y, b.endPoint.y) + accuracy
                || Math.Max(a.startPoint.y, a.endPoint.y) < Math.Min(b.startPoint.y, b.endPoint.y) - accuracy)
            {
                return 2;//相离
            }

            double cross = (a.endPoint.x - a.startPoint.x) * (b.endPoint.y - b.startPoint.y) - (a.endPoint.y - a.startPoint.y) * (b.endPoint.x - b.startPoint.x);
            if (Math.Abs(cross) < accuracy)
            {
                if (!IsPointOnLine(a, b.startPoint) && IsPointOnLine(a, b.endPoint)
                   || IsPointOnLine(a, b.startPoint) && !IsPointOnLine(a, b.endPoint))
                {
                    return 3;//部分重叠
                }
            }
            if (IsSamePoint(a.startPoint, b.startPoint) && IsSamePoint(a.endPoint, b.endPoint)
                || IsSamePoint(a.startPoint, b.endPoint) && IsSamePoint(a.endPoint, b.startPoint))
            {
                return 4;//完全重叠
            }
            if (IsPointOnLine(a, b.startPoint) && IsPointOnLine(a, b.endPoint)
               || IsPointOnLine(b, a.startPoint) && IsPointOnLine(b, a.endPoint))
            {
                return 5;//代表一条线段包含另一条线段
            }
            if (Math.Abs(cross) < ACCURACY)//叉积为0
            {
                if (!IsPointOnLine(a, b.startPoint) && !IsPointOnLine(a, b.endPoint))
                {
                    return 2;//线段b的两个端点都不在线段a上 互相平行
                }
                if (!IsPointOnLine(a, b.startPoint) && IsPointOnLine(a, b.endPoint)
                    || IsPointOnLine(a, b.startPoint) && !IsPointOnLine(a, b.endPoint))
                {
                    return 3;//部分重叠    与上面的判断有重复可以省略
                }
            }
            else
            {
                //
                double p = ((b.startPoint.x - a.startPoint.x) * (b.endPoint.y - b.startPoint.y) - (b.endPoint.x - b.startPoint.x) * (b.startPoint.y - a.startPoint.y)) / cross;
                double q = ((a.endPoint.y - a.startPoint.y) * (b.startPoint.x - a.startPoint.x) - (a.endPoint.x - a.startPoint.x) * (b.startPoint.y - a.startPoint.y)) / cross;
                if (p >= -ACCURACY && p <= 1 + ACCURACY//为甚么是在0-1之间
                    && q >= -ACCURACY && q <= 1 + ACCURACY)
                {
                    return 1;//相交     ？？？？？？？？？？？？疑惑
                }
                else
                {
                    return 2;//相离
                }
            }
            return -1;
        }

        public static Point LineCrossPoint(Line a, Line b)
        {
            if (IsSamePoint(a.startPoint, b.startPoint))
                return a.startPoint;
            if (IsSamePoint(a.startPoint, b.endPoint))
                return a.startPoint;
            if (IsSamePoint(a.endPoint, b.startPoint))
                return a.endPoint;
            if (IsSamePoint(a.endPoint, b.endPoint))
                return a.endPoint;
            double cross = (a.endPoint.x - a.startPoint.x) * (b.endPoint.y - b.startPoint.y) - (a.endPoint.y - a.startPoint.y) * (b.endPoint.x - b.startPoint.x);
            double p = ((b.startPoint.x - a.startPoint.x) * (b.endPoint.y - b.startPoint.y) - (b.endPoint.x - b.startPoint.x) * (b.startPoint.y - a.startPoint.y)) / cross;
            double px = a.startPoint.x + (a.endPoint.x - a.startPoint.x) * p;
            double py = a.startPoint.y + (a.endPoint.y - a.startPoint.y) * p;
            return new Point(px, py);
        }
        /// <summary>
        /// 点point对于线段a的位置关系——1左拐   2右拐    3同向   4反向
        /// </summary>
        /// <param name="a"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static int Position(Line a, Point point)
        {
            double accuracy = 0.00000000001;//12.14把精度提高
            Line b = new Line(a.endPoint, point);
            Line c = new Line(point, a.startPoint);
            double result = cross(a, b) + cross(b, c) + cross(c, a);
            if (Math.Abs(result) < accuracy)
            {    
                double offx=a.endPoint.x-a.startPoint.x;
                double offy=a.endPoint.y-a.startPoint.y;
                double offpx=point.x-a.startPoint.x;
                double offpy=point.y-a.startPoint.y;
                //判断同向有点点问题万一point和a.end重合？所以改为-accuracy
                //if ((a.endPoint.x - a.startPoint.x) * (point.x - a.endPoint.x) > -accuracy|| (a.endPoint.y - a.startPoint.y) * (point.y - a.endPoint.y) >- accuracy)
                if ((offx < accuracy && offpx < accuracy && offy * offpy > 0) || (offy < accuracy && offpy < accuracy && offx * offpx > 0) || (Math.Abs(offx) > accuracy && (Math.Abs(offy) > accuracy) &&offx*offpx>0))
                {
                    return 3;//同向延长线上
                }
                
                else
                {
                    return 4;//反向延长线上
                }
            }
            else if (result < -accuracy)
            {
                return 2;//右拐
            }
            else
            {
                return 1;//左拐
            }
        }
        /// <summary>
        /// 角和边是否接触
        /// </summary>
        /// <param name="line"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static bool isContact(Line line, Angle angle)
        {

            //如果是优角，则不可能接触
            if (isReflexAngle(angle))
            { return false; }
            //平移向量到角
            Line transL = translateLine(line, line.startPoint, angle.midPoint);
            Line reverseInLine = new Line(angle.midPoint, angle.startPoint);
            Line reverseOutLine = new Line(angle.endPoint, angle.midPoint);
            //若向量line在角angle 入边反向与出边反向之间则可以接触
            Line reverseOutLine_ext = translateLine(reverseOutLine, angle.endPoint, angle.midPoint);
            if (cross(reverseInLine, transL) > 0 && cross(transL, reverseOutLine_ext) > 0 && Position(reverseInLine, transL.endPoint) == 1)
            {
                return true;
            }
            else if (Position(reverseInLine, transL.endPoint) == 3)
            {
                return true;
            }
            else if (Position(reverseOutLine_ext, transL.endPoint) == 3)
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        public static Point LowLeftPoint(List<Line> gjxLineList) //找出最低最左点
        {
            Point lowLeft = new Point();
            if (gjxLineList[0].startPoint.y - gjxLineList[0].endPoint.y < -ACCURACY ||
                    (Math.Abs(gjxLineList[0].startPoint.y - gjxLineList[0].endPoint.y) < ACCURACY && gjxLineList[0].startPoint.x < gjxLineList[0].endPoint.x))
            {
                lowLeft = gjxLineList[0].startPoint;
            }
            else
            {
                lowLeft = gjxLineList[0].endPoint;
            }
            for (int i = 1; i < gjxLineList.Count; i++)
            {
                if (gjxLineList[i].startPoint.y - lowLeft.y < -ACCURACY ||
                    (Math.Abs(gjxLineList[i].startPoint.y - lowLeft.y) < ACCURACY && gjxLineList[i].startPoint.x < lowLeft.x))
                {
                    lowLeft = gjxLineList[i].startPoint;
                }
                else if (gjxLineList[i].endPoint.y - lowLeft.y < -ACCURACY ||
                    (Math.Abs(gjxLineList[i].endPoint.y - lowLeft.y) < ACCURACY && gjxLineList[i].endPoint.x < lowLeft.x))
                {
                    lowLeft = gjxLineList[i].endPoint;
                }
            }
            return lowLeft;
        }
        public static Point LowLeftPoint(Polygon p)
        {
            LinkedListNode<Point> current = p.vertex.First;
            Point lowLeft = current.Value;
            current = current.Next;//从下一个点开始比较
            while (current != null)
            {
                if (current.Value.y - lowLeft.y < -ACCURACY ||
                    (Math.Abs(current.Value.y - lowLeft.y) < ACCURACY && current.Value.x < lowLeft.x))
                {
                    lowLeft = current.Value;
                }
                current = current.Next;
            }
            return lowLeft;
        }

        /// <summary>
        /// 沿着vertex 到linePoint 的方向平移点refp(适用于b的角移动到a 的边)
        /// </summary>
        /// <param name="vertex">角的顶点</param>
        /// <param name="refp">参考点</param>
        /// <param name="linePoint">线的端点</param>
        /// <returns>refp平移后的点</returns>
        public static Point threeToOne(Point vertex, Point refp, Point linePoint)  //移动点，参考点，线段端点；(动、参、定)
        {
            Point one = new Point();
            double xOffset = linePoint.x - vertex.x;
            double yOffset = linePoint.y - vertex.y;
            one.x = xOffset + refp.x;
            one.y = yOffset + refp.y;
            return one;
        }

        /// <summary>
        /// 多边形b的角对a各边的轨迹线，其中b是用来求取最下最左点的即参考点
        /// 且b的角要移动到a的各边
        /// </summary>
        /// <param name="angleListB"></param>
        /// <param name="LineListA"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static List<Line> GetGjxLineA(List<Angle> angleListB, List<Line> LineListA, Polygon b)
        {
            List<Line> gjxLineList = new List<Line>();//保存轨迹线线段
            Point gjxP1 = new Point();
            Point gjxP2 = new Point();
            Line gjxLine = new Line();
            Point referencePoint = LowLeftPoint(b);

            for (int i = 0; i < angleListB.Count; i++)
            {
                for (int j = 0; j < LineListA.Count; j++)
                {
                    if (isContact(LineListA[j], angleListB[i]))
                    {
                        gjxP1 = threeToOne(angleListB[i].inLine.endPoint, referencePoint, LineListA[j].startPoint);
                        gjxP2 = threeToOne(angleListB[i].inLine.endPoint, referencePoint, LineListA[j].endPoint);//是不是应该改成gjxP2 = threeToOne(angleListB[i].outLine.start , referencePoint , LineListA[j].end)
                        gjxLine = new Line(gjxP1, gjxP2);
                        gjxLineList.Add(gjxLine);
                    }
                }
            }
            return gjxLineList;
        }
        public static List<Line> GetGjxLineB(List<Angle> angleListA, List<Line> LineListB, Polygon b)
        {
            List<Line> gjxLineList = new List<Line>();//保存轨迹线线段
            // Line moveLine = new Line();//用来放移动线段，判断线段与角
            Point gjxP1 = new Point();
            Point gjxP2 = new Point();
            Line gjxLine = new Line();
            Point referencePoint = LowLeftPoint(b);

            for (int i = 0; i < angleListA.Count; i++)
            {
                for (int j = 0; j < LineListB.Count; j++)
                {
                    if (isContact(LineListB[j], angleListA[i]))
                    {
                        gjxP1 = threeToOne(LineListB[j].startPoint, referencePoint, angleListA[i].midPoint);
                        gjxP2 = threeToOne(LineListB[j].endPoint, referencePoint, angleListA[i].midPoint);
                        gjxLine = new Line(gjxP1, gjxP2);
                        gjxLineList.Add(gjxLine);
                    }
                }
            }
            return gjxLineList;
        }

        //得到多边形的角
        public static List<Angle> GetAngles(Polygon a)
        {
            List<Angle> angleListA = new List<Angle>();
            LinkedList<Point> listA = a.vertex;
            LinkedListNode<Point> currentA = listA.First;

            Point A0 = LowLeftPoint(a);//A0是多边形a的最低最左点

            while (!IsSamePoint(currentA.Value, A0))//直到找到A0在listA的位置
            {
                currentA = NextPoint(currentA, listA);
            }
            Point aStart = new Point();
            Point aEnd = new Point();
            Point bStart = new Point();
            Point bEnd = new Point();
            for (int i = 0; i < listA.Count; i++)
            {
                currentA = PrePoint(currentA, listA);
                aStart = currentA.Value;
                currentA = NextPoint(currentA, listA);
                aEnd = currentA.Value;
                bStart = currentA.Value;
                currentA = NextPoint(currentA, listA);
                bEnd = currentA.Value;
                //currentA = PrePoint(currentA, listA);
                Angle angle = new Angle(aStart, aEnd,bEnd);
                angleListA.Add(angle);
                //currentA = NextPoint(currentA, listA);//currentA向后移动
            }
            return angleListA;
        }
        public static List<Line> GetLines(Polygon a)
        {
            List<Line> lines = new List<Line>();
            Point A0 = LowLeftPoint(a);//A0是多边形a的最低最左点
            LinkedListNode<Point> currentA = a.vertex.First;
            LinkedList<Point> listA = a.vertex;
            while (!IsSamePoint(currentA.Value, A0))//直到找到A0在listA的位置
            {
                currentA = NextPoint(currentA, listA);
            }
            for (int i = 0; i < listA.Count; i++)
            {
                Point start = currentA.Value;
                Point end = NextPoint(currentA, listA).Value;
                lines.Add(new Line(start, end));
                currentA = NextPoint(currentA, listA);

            }
            //去除三点共线  mergePolygon 可以去除多边形的三点共线问题？？？（有问题）
            //double accuracy = 0.000001;
            //for (int i = 0; i < lines.Count; )
            //{
            //    if (Math.Abs(lines[i].startPoint.y - lines[i].endPoint.y) < accuracy && Math.Abs(lines[i + 1].endPoint.y - lines[i].endPoint.y) < accuracy)
            //    {
            //        lines[i] = new Line(lines[i].startPoint, lines[i + 1].endPoint);
            //        lines[i] = new Line(lines[i].startPoint, lines[i + 1].endPoint);

            //        //LineListA[i].endPoint.x = LineListA[i + 1].endPoint.x;  //不能直接讲一个点给另一个点LineListA[i].endPoint = LineListA[i+1].endPoint
            //        //LineListA[i].endPoint.y = LineListA[i + 1].endPoint.y; 
            //        lines.RemoveAt(i + 1);
            //        continue;
            //    }
            //    i++;
            //}
            return lines;
        }
        //获取先集合中，点的出边
        public static List<Line> GetPointOutLine(Point p, List<Line> gjxLineList)
        {
            List<Line> pointOutLineList = new List<Line>();//保存轨迹线线段
            for (int i = 0; i < gjxLineList.Count; i++)
            {
                if (IsSamePoint(p, gjxLineList[i].startPoint))
                {
                    pointOutLineList.Add(gjxLineList[i]);
                }
            }
            return pointOutLineList;
        }
        public static List<Line> GetPointInLine(Point p, List<Line> gjxLineList)
        {
            List<Line> pointInLineList = new List<Line>();//保存轨迹线线段
            for (int i = 0; i < gjxLineList.Count; i++)
            {
                if (IsSamePoint(p, gjxLineList[i].endPoint))
                {
                    pointInLineList.Add(gjxLineList[i]);
                }
            }
            return pointInLineList;
        }

        //循环遍历linkedlist的节点，下一个点
        public static LinkedListNode<Point> NextPoint(LinkedListNode<Point> node, LinkedList<Point> list)
        {
            if (node == list.Last)
                return list.First;
            else
                return node.Next;
        }
        //循环遍历linkedlist的节点，前一个点
        public static LinkedListNode<Point> PrePoint(LinkedListNode<Point> node, LinkedList<Point> list)
        {
            if (node == list.First)
                return list.Last;
            else
                return node.Previous;
        }
      
        //求轨迹线的交点，specialPoints 用于传值，把轨迹线中的交点（且不是轨迹线端点求出），？？？用于求解内环nfp

        //2016.9.26 修改  求出所有轨迹线交点，用于利用交点求解内环nfp
        public static List<Line> cutGjxLines(List<Line> gjxLines, List<Point> allCrossPoints)
        {
            List<Line> cutGjxLines = new List<Line>();
           
            for (int i = 0; i < gjxLines.Count; i++)
            {
                List<Point> allCrossList = new List<Point>();//用于将线段切割
                for (int j = 0; j < gjxLines.Count; j++)
                {
                    int relation = LineRelation(gjxLines[i], gjxLines[j]);
                    if (relation == 1)
                    {
                        Point cross = LineCrossPoint(gjxLines[i], gjxLines[j]);
                        if (!IsSamePoint(cross, gjxLines[i].startPoint) && !IsSamePoint(cross, gjxLines[i].endPoint))
                        {
                           
                            allCrossList.Add(cross);
                            allCrossPoints.Add(cross);//当相交并且，交点不在线段两端时，加入“特殊交点类”求内环使用到(最后统一去重)
                            

                        }

                    } if (relation == 3)//部分重叠//考虑边界？？比如首尾相接。。（解决）
                    {
                        if (IsPointOnLine(gjxLines[i], gjxLines[j].startPoint))
                        {
                            allCrossPoints.Add(gjxLines[j].startPoint);
                            if (IsSamePoint(gjxLines[i].startPoint, gjxLines[j].startPoint) || IsSamePoint(gjxLines[i].endPoint, gjxLines[j].startPoint)) { }
                            else {
                                allCrossList.Add(gjxLines[j].startPoint); 
                            }
                            
                        }
                        else
                        {
                            allCrossPoints.Add(gjxLines[j].endPoint);
                            if (IsSamePoint(gjxLines[i].startPoint, gjxLines[j].endPoint) || IsSamePoint(gjxLines[i].endPoint, gjxLines[j].endPoint)) { }
                            else
                            {
                                allCrossList.Add(gjxLines[j].endPoint);
                            }
                        }
                    } if (relation == 5)//包含
                    {//gjxLines[i]包含 gjxLines[j]
                        if (IsPointOnLine(gjxLines[i], gjxLines[j].startPoint) && IsPointOnLine(gjxLines[i], gjxLines[j].endPoint))//gjxLineList[i]包含gjxLineList[j]
                        {
                            allCrossPoints.Add(gjxLines[j].startPoint);
                            allCrossPoints.Add(gjxLines[j].endPoint);
                            if (IsSamePoint(gjxLines[i].startPoint, gjxLines[j].startPoint) || IsSamePoint(gjxLines[i].endPoint, gjxLines[j].startPoint)) { }
                            else {
                                allCrossList.Add(gjxLines[j].startPoint);
                            }
                            if (IsSamePoint(gjxLines[i].startPoint, gjxLines[j].endPoint) || IsSamePoint(gjxLines[i].endPoint, gjxLines[j].endPoint)) { }
                            else
                            {
                                allCrossList.Add(gjxLines[j].endPoint);
                            }
                        }
                    }

                }
                if (allCrossList.Count == 0)   //该线段无交点
                {
                    cutGjxLines.Add(gjxLines[i]);
                }
                if (allCrossList.Count == 1)   //该线段有1个交点
                {
                    Line L1 = new Line(gjxLines[i].startPoint, allCrossList[0]);
                    Line L2 = new Line(allCrossList[0], gjxLines[i].endPoint);
                    cutGjxLines.Add(L1);
                    cutGjxLines.Add(L2);
                }
                if (allCrossList.Count >= 2)   //去除重复的点
                {
                    for (int i2 = 0; i2 < allCrossList.Count; i2++)  //外循环是循环的次数
                    {
                        for (int j = allCrossList.Count - 1; j > i2; j--)  //内循环是 外循环一次比较的次数
                        {
                            if (IsSamePoint(allCrossList[i2], allCrossList[j]))
                            {
                                allCrossList.RemoveAt(j);
                            }
                        }
                    }

                    if (allCrossList.Count == 1)   //去重复后，该线段有1个交点
                    {
                        Line L1 = new Line(gjxLines[i].startPoint, allCrossList[0]);
                        Line L2 = new Line(allCrossList[0], gjxLines[i].endPoint);
                        cutGjxLines.Add(L1);
                        cutGjxLines.Add(L2);
                    }
                    else   //大于等于2个交点
                    {
                        List<Point> sortPointList = SortPointUp(allCrossList, gjxLines[i].startPoint);//按照与gjxLines[i].startPoint距离从近到远排序
                        cutGjxLines.Add(new Line(gjxLines[i].startPoint, sortPointList[0]));

                        for (int i3 = 1; i3 < sortPointList.Count; i3++)
                        {
                            Line L1 = new Line(sortPointList[i3 - 1], sortPointList[i3]);
                            cutGjxLines.Add(L1);
                        }
                        cutGjxLines.Add(new Line(sortPointList[sortPointList.Count - 1], gjxLines[i].endPoint));
                    }

                }
            }
            //轨迹线去重
            for (int i = 0; i < cutGjxLines.Count; i++) 
            {
                for (int j = cutGjxLines.Count - 1; j > i; j--)
                {
                    if (IsSameLineT(cutGjxLines[i], cutGjxLines[j]))
                    {
                        cutGjxLines.RemoveAt(j);
                    }
                }
            
            }
            //全部交点去重
            for (int i = 0; i < allCrossPoints.Count; i++)
            {
                for (int j = allCrossPoints.Count - 1; j > i; j--)
                {
                    if (IsSamePoint(allCrossPoints[i], allCrossPoints[j]))
                    {
                        allCrossPoints.RemoveAt(j);
                    }
                }

            }


                return cutGjxLines;
        }
        
        /// <summary>
        /// 求轨迹线所形成的外围多边形(目前未使用所有交点allCrossPoints，还未发现有出边endPoint不是交点情况)
        /// </summary>
        /// <param name="cutGjxLineList"></param>
        /// <param name="lowLeft"></param>
        /// <param name="wholeCrossList"></param>
        /// <returns></returns>
        public static Polygon GetOutLoop(List<Line> cutGjxLineList, Point lowLeft, List<Point> allCrossPoints)
        {
            LinkedList<Point> list = new LinkedList<Point>();
            Point current = new Point();
            current = lowLeft;
            Line currentLine=new Line();
            list.AddLast(lowLeft);//加入第一点，尽量不要是交点
          // RemovePointInList(list.Last<Point>(), wholeCrossList);//从交点集合中删除该点
            do
            {
                List<Line> pointOutLineList = GetPointOutLine(current, cutGjxLineList);
                //修改最右拐问题
                //第一次选边比较特殊
                if (list.Count == 1)
                {
                    if (pointOutLineList.Count == 1)
                    {
                        list.AddLast(pointOutLineList[0].endPoint);
                        current = pointOutLineList[0].endPoint;
                        currentLine = pointOutLineList[0];
                        cutGjxLineList.Remove(currentLine);//防止重复遍历
                    }
                    else if (pointOutLineList.Count >= 2)
                    {
                        Line outLine = pointOutLineList[0];
                        currentLine = new Line(new Point(0, 0), new Point(1, 0));//x轴
                        double angle0 = AngleBetween(currentLine, outLine);//初始角度
                        for (int i = 1; i < pointOutLineList.Count; i++)
                        {
                            double angle = AngleBetween(currentLine, outLine);
                            if (angle < angle0)
                            {
                                outLine = pointOutLineList[i];//最右拐
                            }
                        }
                        current = outLine.endPoint;
                        list.AddLast(current);
                        currentLine = outLine;
                        cutGjxLineList.Remove(currentLine);//防止重复遍历
                    }
                    else {
                        break;
                    }
                }
                    //非第一次选边
                else
                {
                    if (pointOutLineList.Count == 1)
                    {
                        list.AddLast(pointOutLineList[0].endPoint);
                        current = pointOutLineList[0].endPoint;
                        currentLine = pointOutLineList[0];
                        cutGjxLineList.Remove(currentLine);//防止重复遍历
                    }
                    else if (pointOutLineList.Count >= 2)
                    {
                        Line outLine = pointOutLineList[0];
                        double angle0 = AngleBetween(currentLine, outLine);//初始角度
                        for (int i = 1; i < pointOutLineList.Count; i++)
                        {
                            double angle = AngleBetween(currentLine, pointOutLineList[i]);
                            if (angle < angle0)
                            {
                                outLine = pointOutLineList[i];//最右拐
                            }
                        }
                        current = outLine.endPoint;
                        list.AddLast(current);
                        currentLine = outLine;
                        cutGjxLineList.Remove(currentLine);//防止重复遍历
                        //RemovePointInList(list.Last<Point>(), wholeCrossList);
                    }
                    else {
                        break;//没有出边跳出while(还未发现这种情景)
                    }
                    
                }
                
            } while (!IsSamePoint(list.First<Point>(), list.Last<Point>()));

            list.RemoveLast();//第一点和最后一点相同，移除最后一点
           // return MergePolygon(new Polygon(list));
            return new Polygon(list);
        }

        public static bool IsSameLineT(Line line1, Line line2)  //该函数与师兄的不同，所以加了一个T
        {
            return (IsSamePoint(line1.startPoint, line2.startPoint) && IsSamePoint(line1.endPoint, line2.endPoint));
        }


        /// <summary>
        ///求取多边形b相对于a的轨迹线
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static List<Line> getGjxLines(Polygon a, Polygon b)
        {

            List<Line> gjxLines = new List<Line>();
            List<Angle> aAngles = GetAngles(a);
            List<Angle> bAngles = GetAngles(b);
            List<Line> aLines = GetLines(a);
            List<Line> bLines = GetLines(b);
            List<Line> gjx1 = GetGjxLineA(bAngles, aLines, b);
            List<Line> gjx2 = GetGjxLineB(aAngles, bLines, b);
            gjxLines.AddRange(gjx1);
            gjxLines.AddRange(gjx2);
            //是否需要去重？
            for (int i = 0; i < gjxLines.Count; i++) {
                for (int j = gjxLines.Count-1; j > i; j--)
                {
                    if (IsSameLineT(gjxLines[i], gjxLines[j]))
                    {
                        gjxLines.RemoveAt(j);
                    }
                }
            }
                return gjxLines;
        }
        //返回退化线段
        public static List<Line> getDegradeLines(List<Line> gjxLines)
        {
            List<Line> twoHeadLines = new List<Line>();//由于得到所有的反向共边线段的交集
            List<Line> degradeLines = new List<Line>();
            for (int i = 0; i < gjxLines.Count; i++)
            {
                for (int j = i + 1; j < gjxLines.Count; j++)
                {
                    int lineRelation = LineRelation(gjxLines[i], gjxLines[j]);
                    Point cross1, cross2;
                    //部分重叠
                    if (lineRelation == 3)
                    {
                        //反向
                        if (CrossDirection(gjxLines[i], gjxLines[j]) == 4)
                        {
                            if (IsPointOnLine(gjxLines[i], gjxLines[j].startPoint))
                            {
                                cross1 = gjxLines[j].startPoint;
                            }
                            else
                            {
                                cross1 = gjxLines[j].endPoint;
                            }
                            if (IsPointOnLine(gjxLines[j], gjxLines[i].startPoint))
                            {
                                cross2 = gjxLines[i].startPoint;
                            }
                            else
                            {
                                cross2 = gjxLines[i].endPoint;
                            }
                            //得到反向共线的线段（有可能是退化线段）
                            twoHeadLines.Add(new Line(cross1, cross2));
                        }

                    }
                    else if (lineRelation == 4)//完全重叠，一定反向，因为轨迹线已经去重
                    {
                        //
                        twoHeadLines.Add(new Line(gjxLines[i].startPoint, gjxLines[i].endPoint));
                    }
                    else if (lineRelation == 5) //包含  ？？？可能包含同向线段？？？(已经解决)
                    {
                        //只要反向的
                        if (CrossDirection(gjxLines[i], gjxLines[j]) == 4) { 
                                if (IsPointOnLine(gjxLines[i], gjxLines[j].startPoint) && IsPointOnLine(gjxLines[i], gjxLines[j].endPoint))
                                {
                                    twoHeadLines.Add(new Line(gjxLines[j].startPoint, gjxLines[j].endPoint));
                                }
                                else
                                {
                                    twoHeadLines.Add(new Line(gjxLines[i].startPoint, gjxLines[i].endPoint));
                                }
                        }

                    }
                }

            }
            //去除相同的线段
            for (int i = 0; i < twoHeadLines.Count; i++)
            {
                for (int j = twoHeadLines.Count - 1; j > i; j--)
                {
                    if (IsSamePoint(twoHeadLines[i].endPoint, twoHeadLines[j].endPoint) && IsSamePoint(twoHeadLines[i].startPoint, twoHeadLines[j].startPoint) || IsSamePoint(twoHeadLines[i].endPoint, twoHeadLines[j].startPoint) && IsSamePoint(twoHeadLines[i].startPoint, twoHeadLines[j].endPoint))
                    {
                        twoHeadLines.RemoveAt(j);
                    }
                }
            }
            //求出与反向共线的交点来求出退化线
            for (int i = 0; i < twoHeadLines.Count; i++)
            {
                List<Point> crossPoints = new List<Point>();
                for (int j = 0; j < gjxLines.Count; j++)
                {
                    if (LineRelation(twoHeadLines[i], gjxLines[j]) == 1)
                    {
                        Point cross = LineCrossPoint(twoHeadLines[i], gjxLines[j]);
                        crossPoints.Add(cross);
                    }
                }
                //去重交点，求退化线 目前情况是只有两个点
                for (int m = 0; m < crossPoints.Count; m++)
                { 
                    for(int n=crossPoints.Count-1;n>m;n--){
                        if(IsSamePoint(crossPoints[m],crossPoints[n])){
                        crossPoints.RemoveAt(n);//
                        }
                        
                    }
                }
                if(crossPoints.Count>1){
                 degradeLines.Add(new Line(crossPoints[0],crossPoints[1]));//这里没有考虑三个点的，好像不会出现这种情况。
                }
               
            }
            //对退化线段去重
            for (int m = 0; m < degradeLines.Count; m++)
            {
                for (int n = degradeLines.Count - 1; n > m; n--)
                {
                    if (IsSameLineT(degradeLines[m], degradeLines[n]) || IsSameLineT(reverseLine(degradeLines[n]), degradeLines[n]))
                    degradeLines.RemoveAt(n);
                }
            }
            return degradeLines;
        }
        //返回退化点   有三条线段相交   待优化。。。？？？
        public static List<Point> getDegradePoints(List<Line> gjxLines) {
            List<Point> degradePoints = new List<Point>();
            for (int i = 0; i < gjxLines.Count; i++)
            { 
                for(int j=i+1;j<gjxLines.Count;j++)
                {
                     int lineRelation = LineRelation(gjxLines[i], gjxLines[j]);
                     //相交
                     if (lineRelation == 1) 
                     { 
                       Point cross=LineCrossPoint(gjxLines[i], gjxLines[j]);
                      for (int k = j + 1; k < gjxLines.Count; k++) 
                      { 
                      if(LineRelation(gjxLines[i], gjxLines[k])==1)
                          if (IsSamePoint(LineCrossPoint(gjxLines[i], gjxLines[k]), cross) && (CrossDirection(gjxLines[k], gjxLines[j])<3))
                          {
                              degradePoints.Add(cross);
                              break;
                          }
                      }//
                    }//3
                }//2
            }//1
            //去重
            for (int i = 0; i < degradePoints.Count; i++)
            {
                for (int j = degradePoints.Count - 1; j > i; j--)
                {
                    if (IsSamePoint(degradePoints[i], degradePoints[j])) {
                        degradePoints.RemoveAt(j);
                    }

                }

            }
                return degradePoints;
        
        
        }
        //强行把line转成Polygon
        public static Polygon LineToPolygon(Line line)
        {
            LinkedList<Point> list = new LinkedList<Point>();
            list.AddLast(line.startPoint);
            list.AddLast(line.endPoint);
            return new Polygon(list);
        }
        public static Polygon PointToPolygon(Point p)
        {
            LinkedList<Point> list = new LinkedList<Point>();
            list.AddLast(p);
            return new Polygon(list);
        }
        public static bool IsPointInList(Point p, List<Point> l) {
            if (l.Contains(p))
            {
                return true;//暂时使用这种，到时可能要改成用精度判别
            }
            else
                return false;
        
        }
        public static bool IsPointInLinedList(Point p, LinkedList<Point> l)
        {
           
            LinkedListNode<Point> pNode=l.First;
            for (int i = 0; i < l.Count; i++) {
                if (IsSamePoint(pNode.Value, p))
                    return true;
                pNode = pNode.Next;

            }
            return false;

        }
       
        


        //2016.9.26 修改，利用所有轨迹线交点来遍历求解内部多边形  
        public static List<Polygon> GetInLoop(List<Line> cutGjxLines,Polygon polygon,List<Point> specialPoints,List<Line> gjxLines){
            List<Polygon> innerPolygons = new List<Polygon>();
            Point current;
            List<Line> maybeDegrades = new List<Line>();//用于存储同线反向的线段（可能的退化线）

            // //过滤轨迹线     对于轨迹线在多边形外的




            for (int i = 0; i < specialPoints.Count; i++)
            {
                List<Line> copyGjx = CopyList(cutGjxLines);
                LinkedList<Point> vertex;//存储顶点集
                current = specialPoints[i];
                Line currentLine=new Line();
                if (pointAndPolygonRelation(current, polygon) < 0)//点在多边形外
                    continue;
                List<Line> outLines = GetPointOutLine(specialPoints[i], copyGjx);
               
                for (int j = 0; j < outLines.Count;j++ )
                {
                    List<Line> goThrough = new List<Line>();//经过的边
                    bool flag = false;//判断出边已经不满足条件
                    vertex = new LinkedList<Point>();
                    vertex.AddLast(specialPoints[i]);
                    currentLine = outLines[j];
                    current = currentLine.endPoint;
                    goThrough.Add(currentLine);
                    // copyGjx.Remove(currentLine);
                    if (pointAndPolygonRelation(currentLine.endPoint, polygon) < 0)
                    {
                        continue;//若在多边形外，则考虑其他出边
                    }
                    vertex.AddLast(currentLine.endPoint);
                    while (!IsSamePoint(vertex.Last.Value, vertex.First.Value))
                    {
                        List<Line> outLines2 = GetPointOutLine(current, copyGjx);
                        if (outLines2.Count == 0)
                        {
                            flag = true;
                            break;//退出while循环，进入下一个交点
                        }

                        else if (outLines2.Count == 1)
                        {
                            current = outLines2[0].endPoint;
                            if (pointAndPolygonRelation(current, polygon) < 0)
                            {
                                flag = true;
                                break;
                            }
                            if (IsPointInLinedList(current, vertex) && !IsSamePoint(current, vertex.First.Value))
                            {
                                //如果这个多边形经过之前的点，且不是初始点，则会陷入死循环，故排除该情况
                                flag = true;
                                break;
                            
                            }
                            vertex.AddLast(current);
                            currentLine = outLines2[0];
                            goThrough.Add(currentLine);
                            // copyGjx.Remove(currentLine);
                            //cutGjxLines.Remove(currentLine);
                        }

                        else if (outLines2.Count > 1)
                        {
                            Line outRight = outLines2[0];
                            double angle0 = AngleBetween(currentLine, outRight);
                            for (int k = 1; k < outLines2.Count; k++)
                            {
                                double angle = AngleBetween(currentLine, outLines2[k]);
                                if (angle < angle0||angle==180)//最右拐  包括反向
                                {
                                    outRight = outLines2[k];
                                }
                            }
                            current = outRight.endPoint;
                            if (pointAndPolygonRelation(current, polygon) < 0)
                            {
                                flag = true;
                                break;
                            }
                            if (IsPointInLinedList(current, vertex) && !IsSamePoint(current, vertex.First.Value))
                            {
                                //如果这个多边形经过之前的点，且不是初始点，则会陷入死循环，故排除该情况
                                flag = true;
                                break;

                            }
                            vertex.AddLast(current);
                            currentLine = outRight;
                            goThrough.Add(currentLine);
                            //copyGjx.Remove(currentLine);
                            //cutGjxLines.Remove(currentLine);
                        }
                    
                    }//while 循环
                    if (flag)
                    {
                        continue;//说明该出边不行
                    }
                    else
                    {
                        vertex.RemoveLast();
                        DeleteSameLineInList(goThrough, cutGjxLines);//轨迹线中去除内部多边形（去除已经遍历过得）

                        if (vertex.Count == 2)//有可能是退化线
                        {
                            maybeDegrades.Add(new Line(vertex.First.Value, vertex.Last.Value));
                        }
                        else {
                            innerPolygons.Add(new Polygon(vertex));
                        }
                        
                    }

                }
                


               


               /*
                do
                {
                     List<Line> outLines = GetPointOutLine(current, cutGjxLines);
                    //第一次选边 
                    if (vertex.Count == 1)
                     {
                         if (outLines.Count == 0)
                         {
                             break;//退出while循环，进入下一个交点
                         }

                         else if (outLines.Count == 1)
                         {
                             current = outLines[0].endPoint;
                             if (pointAndPolygonRelation(current, polygon) < 0)
                             {
                                 break;
                             }
                             vertex.AddLast(current);
                             currentLine = outLines[0];
                             cutGjxLines.Remove(currentLine);
                         }

                         else if (outLines.Count > 1)
                         {
                             for (int k = 0; k < outLines.Count; k++)
                             {

                             }
                             current = outRightLine.endPoint;
                             if (pointAndPolygonRelation(current, polygon) < 0)
                             {
                                 break;
                             }
                             vertex.AddLast(current);
                             currentLine = outRightLine;
                         }
                     }
                     //选取其他边
                     else 
                     {
                         if (outLines.Count > 0)
                         {
                             if (outLines.Count == 1)
                             {
                                 current = outLines[0].endPoint;
                                 currentLine = outLines[0];
                                 if (pointAndPolygonRelation(current, polygon) < 0)
                                 {
                                     break;
                                 }
                                 vertex.AddLast(current);
                             }
                             else if (outLines.Count >= 2)
                             {
                                 Line outLine = outLines[0];
                                 double angle0 = AngleBetween(outLine, currentLine);//初始角度
                                 for (int k = 1;k < outLines.Count; k++)
                                 {
                                     double angle = AngleBetween( outLines[k],currentLine);
                                     if (angle > angle0)
                                     {
                                         outLine = outLines[k];//最右拐  
                                     }
                                 }
                                 current = outLine.endPoint;
                                 vertex.AddLast(current);
                                 currentLine = outLine;
                             }
                            
                         }
                         else
                         {
                             //没有出边的情况
                             break;
                         }

                     }
                     
                    
                } while (!IsSamePoint(vertex.Last.Value, vertex.First.Value));
                if(IsSamePoint(vertex.Last.Value,vertex.First.Value)&&vertex.Count>1)
                {
                    vertex.RemoveLast();
                    innerPolygons.Add(new Polygon(vertex));

                }
               */
            
            }
            //合并同线反向的线段
            for (int i = 0; i < maybeDegrades.Count;)
            {
                bool flag = false;//是否可以合并
                for (int j = i + 1; j < maybeDegrades.Count; j++)
                {
                    Line c =new Line();
                    if (mergeLine(maybeDegrades[i],maybeDegrades[j],ref c)) 
                    { 
                    //如果两条线段可以合并
                        maybeDegrades.RemoveAt(j);
                        maybeDegrades.RemoveAt(i);
                        maybeDegrades.Insert(i, c);
                        flag = true;//可以合并，跳出
                        break;
                    }
                }
                if (!flag) {
                    //如果不能合并，继续合并其他的
                    i++;
                }
            }
            //求退化线

            for (int i = 0; i < maybeDegrades.Count; i++)
            {
                List<Point> crossPoints = new List<Point>();
                for (int j = 0; j < gjxLines.Count; j++)
                {
                    int lineRelation = LineRelation(maybeDegrades[i], gjxLines[j]);
                    if (lineRelation == 1) 
                    {
                        Point cross = LineCrossPoint(maybeDegrades[i], gjxLines[j]);
                        if (!IsPointInList(cross, crossPoints))
                        {
                            crossPoints.Add(cross);
                        }
                    }
                }
                if(crossPoints.Count==2)//退化线
                {
                    LinkedList<Point> vertex=new LinkedList<Point>();
                    vertex.AddLast(crossPoints[0]);
                    vertex.AddLast(crossPoints[1]);
                    innerPolygons.Add(new Polygon(vertex));
                }
                if (crossPoints.Count ==1)//退化点
                {
                    LinkedList<Point> vertex = new LinkedList<Point>();
                    vertex.AddLast(crossPoints[0]);
                    
                    innerPolygons.Add(new Polygon(vertex));
                }
                  
            }


            //
            List<Polygon> innerPolygonsRet = new List<Polygon>();
            for (int i = 0; i < innerPolygons.Count; i++)
            {
                innerPolygonsRet.Add(MergePolygon(innerPolygons[i]));
            }
            //去重（没必要，待删除）
            //for (int i = 0; i < innerPolygonsRet.Count; i++)
            //{
            //    for (int j = innerPolygonsRet.Count - 1; j > i; j--)
            //    {
            //        if (IsSamePolygon(innerPolygonsRet[i], innerPolygonsRet[j]))
            //        {
            //            innerPolygonsRet.RemoveAt(j);
            //        }
            //    }
            //}

            return innerPolygonsRet;
        }

       /// <summary>
       /// 合并线段（合并同线反向线段使用）,合并后的线段mergeLine
       /// </summary>
       /// <param name="a"></param>
       /// <param name="b"></param>
       /// <returns></returns>
        public static  bool   mergeLine(Line a,Line b ,ref Line mergeLine)
        {
            double accurcy=0.00001;
            //如果两条线段平行
            if(Math.Abs(cross(a,b))<accurcy){
                if(IsSamePoint(a.endPoint,b.startPoint))
                {
                    mergeLine=new Line(a.startPoint,b.endPoint);
                    return true;
                }
                else if(IsSamePoint(a.startPoint,b.startPoint)){
                     mergeLine=new Line(a.endPoint,b.endPoint);
                    return true;
                }
                else if(IsSamePoint(a.endPoint,b.endPoint)){
                     mergeLine=new Line(a.startPoint,b.startPoint);
                    return true;
                }else if(IsSamePoint(a.startPoint,b.endPoint))
                {
                     mergeLine=new Line(b.startPoint,a.endPoint);
                    return true;
                }
                else{
                return false;
                }
            }
            return false;
           
       }
        
         
        /// 由于目前所求的内接或外接NFP（主要是外接NFP）的多边形都是由切割的线段拼接而成，
        ///本方法意在整合这些线段
        public static Polygon MergePolygon(Polygon p)
        {
            LinkedList<Point> newVertex=new LinkedList<Point>();
            LinkedList<Point> vertex = p.vertex;
            LinkedListNode<Point> current = vertex.First;
            //加入第一个点
            if(vertex.Count<3){
                return p;
            }
            LinkedListNode<Point> preP, nextP;
            current = vertex.First;
            for (int i = 0; i < vertex.Count; i++)
            {
                
                preP = PrePoint(current, vertex);
                nextP = NextPoint(current, vertex);
                Line preLine = new Line(preP.Value, current.Value);
                Line nextLine = new Line(current.Value, nextP.Value);
                if (CrossDirection(preLine, nextLine) != 3) {
                    newVertex.AddLast(new LinkedListNode<Point>(current.Value));

                }
                current = nextP;
            }
            return new Polygon(newVertex);
        }
        /// <summary>
        /// 求 向量a 和b 直接的夹角
        /// 求出来是逆时针方向
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>

        public static double AngleBetween(Line a, Line b)
        {
            double x1 = a.endPoint.x - a.startPoint.x;
            double x2 = b.endPoint.x - b.startPoint.x;
            double y1 = a.endPoint.y - a.startPoint.y;
            double y2 = b.endPoint.y - b.startPoint.y;
            double sin = x1 * y2 - x2 * y1;
            
            double cos = x1*x2+y1*y2;

            return Math.Atan2(sin, cos) * (180 / Math.PI);
        }
        public static void DeleteSameLineInList(List<Line> deleteLines,List<Line> cutGjxLines)
        {
            for (int i = 0; i < deleteLines.Count; i++) {
                cutGjxLines.Remove(deleteLines[i]);
            }
        }
        public static List<Line> CopyList(List<Line> cutGjxLines) 
        {
            List<Line> newList = new List<Line>();
            for (int i = 0; i < cutGjxLines.Count; i++) {
                newList.Add(cutGjxLines[i]);
            }
            return newList;
        }



        public static List<Polygon> getNfp(Polygon a, Polygon b)
        {
            List<Polygon> nfps = new List<Polygon>();
            //1、求轨迹线集合
            List<Line> gjxLines=getGjxLines( a,  b);
      
            //2、求退化线
            //List<Line> degradeLines = getDegradeLines(gjxLines);
            ////
            //for (int i = 0; i < degradeLines.Count; i++)
            //{
            //    nfps.Add(LineToPolygon(degradeLines[i]));
            //}
               

            //3、求退化点
            List<Point> degradePoints = getDegradePoints(gjxLines);
            for (int i = 0; i < degradePoints.Count; i++)
            {
                nfps.Add(PointToPolygon(degradePoints[i]));
            }

            //切割轨迹线
            List<Point> specialPoint = new List<Point>();
            List<Line> cutGjxs = cutGjxLines(gjxLines,specialPoint);
            //4、求外围nfp
            Polygon outL = GetOutLoop(cutGjxs, LowLeftPoint(cutGjxs), specialPoint);
            nfps.Add(outL);
            //5、求内部nfp，可能有多个
            List<Polygon> inLs = GetInLoop(cutGjxs, outL, specialPoint, gjxLines);
            for (int i = 0; i < inLs.Count; i++) {
                nfps.Add(inLs[i]);
            }
            
                return nfps;
        }

    
    /***************************  以上基本完成求解nfp功能   ***************************************/
    /**************************    一下求解一些内靠接nfp   ***************************************/
        /// <summary>
        /// 把多变形的方向变一下，即把逆时针变成顺时针，或相反。
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Polygon reversePolygon(Polygon p) {
            LinkedList<Point> vertex = p.vertex;
            LinkedList<Point> vertexRet = new LinkedList<Point>();
            LinkedListNode<Point> current = vertex.Last;
            for (int i = 0; i < vertex.Count; i++) {
                vertexRet.AddLast(new LinkedListNode<Point>(current.Value));
                current = current.Previous;
            }
            return new Polygon(vertexRet);
        }

        /// <summary>
        /// 改变自List<Polygon> getNfp(Polygon a, Polygon b)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static List<Polygon> getInnerNfp(Polygon a, Polygon b)
        {
            List<Polygon> nfps = new List<Polygon>();
            //1、求轨迹线集合
            List<Line> gjxLines = getGjxLines(reversePolygon(a), b);

            ////2、求退化线
            
            ////3、求退化点
            
            //切割轨迹线
            List<Point> specialPoint = new List<Point>();
            List<Line> cutGjxs = cutGjxLines(gjxLines, specialPoint);
            //4、求外围nfp
            //Polygon outL = GetOutLoop(cutGjxs, LowLeftPoint(cutGjxs), specialPoint);
            // nfps.Add(outL);
            //5、求内部nfp，可能有多个
            List<Polygon> inLs = GetInLoop(cutGjxs, a, specialPoint, gjxLines);
            for (int i = 0; i < inLs.Count; i++)
            {
                nfps.Add(inLs[i]);
            }

            return nfps;
        }

        /// <summary>
        /// 计算点和多边形的关系，若在多边形内，返回1，在多边形上返回0，在多边形外返回-1
        /// </summary>
        /// <param name="p">点</param>
        /// <param name="polygon">多边形</param>
        /// <returns></returns>
        public static int pointAndPolygonRelation(Point p, Polygon polygon) 
        { 
            LinkedList<Point> vertex=polygon.vertex;
            int sum = 0;
            LinkedListNode<Point> current=vertex.First;
            Point currentPoint, nextPoint;
            double accurcy=0.000001;
            for (int i = 0; i < vertex.Count; i++)
            {
                if (i == 0)
                {
                    current = vertex.First;
                }
                else {
                    current = current.Next;
                }
                currentPoint = current.Value;
                if (i == vertex.Count - 1)
                {
                    nextPoint = vertex.First.Value;
                }
                else
                {
                    nextPoint = current.Next.Value;
                }
                //点在线上
                if (IsPointOnLine(new Line(currentPoint, nextPoint), p)) 
                {
                    return 0;
                }
                //水平线情况
                if (Math.Abs(currentPoint.y - nextPoint.y)<accurcy)
                { 
                //不记录sum 
                    continue;
                }
                //竖直线情况
                else if (Math.Abs(currentPoint.x - nextPoint.x) < accurcy)
                {
                    double minY = Math.Min(currentPoint.y, nextPoint.y);
                    double maxY = Math.Max(currentPoint.y, nextPoint.y);

                    if (p.x < currentPoint.x && p.y <= maxY && p.y > minY)
                    {
                        sum++;
                    }

                }
                    //斜线情况
                else 
                {
                    double minY = Math.Min(currentPoint.y, nextPoint.y);
                    double maxY = Math.Max(currentPoint.y, nextPoint.y);
                    if (p.y - minY < accurcy)
                        continue;
                    else if (p.y > minY && p.y <= maxY) 
                    {
                        double y = p.y;
                        double x = (y - currentPoint.y) / (nextPoint.y - currentPoint.y) * (nextPoint.x - currentPoint.x) + currentPoint.x;
                        if (p.x - x < accurcy)
                        {
                            sum++;
                        }
                    }
                }


               
            }
            return sum % 2 == 1 ? 1 : -1;
        
        }
        
 /*************************************************** 计算两个多边形的利用率 ****************/
        public static double Utilization(Polygon a, Polygon b)
        { 
            double area_a=a.Area();
            double area_b=b.Area();

            Rectangle rect = Enclosure(a, b);

            return (area_a + area_b) / rect.Area();
        
        }
   


        /// <summary>
        /// 获得最优的利用率
        /// 多边形b绕着多边形a
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="p">最佳接触点</param>
        /// <returns></returns>
        public static double GetBestContact(Polygon a, Polygon b, Point p)
        {
            Point bestPoint = new Point();
            double bestValue = 0;
            List<Polygon> nfps = getNfp(a, b);
            Point b_ref = LowLeftPoint(b);
            for (int i = 0; i < nfps.Count; i++)
            {
                LinkedListNode<Point> pNode=nfps[i].vertex.First;
                for (int j = 0; j < nfps[i].vertex.Count; j++)
                { 
                    Point end=pNode.Value;
                    b.Move(b_ref, end);
                    double value = Utilization(a, b);
                    if (value > bestValue)
                    {
                        bestPoint = end;
                        bestValue = value;
                    }
                    pNode=pNode.Next;
                    b_ref = end;
                
                }
            }
            p = bestPoint;
            return bestValue;
        
        }

    }

}