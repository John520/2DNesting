using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using _2DNesting.DataStruct;
using _2DNesting.Algorithm;
using System.Collections;
namespace _2DNesting
{
    public partial class Form1 : Form
    {
        public float ZOOM = 4F;//放大倍数
        public  float OFFSET_X = 200.0F;//偏移量
        public  float OFFSET_Y = 200.0F;

        double totalArea = 0.0;//记录排版零件的总面积
        int totalNum = 0;//记录排版零件的总个数
        int totalType = 0;//记录排版零件的总类型数目
        List<Piece> pieceList = new List<Piece>();
        ArrayList pieceDictory = new ArrayList();
        List<Polygon> layoutList = new List<Polygon>();//记录各个多边形的排版后的顶点序列
        Polygon a;//选中的多边形
        Polygon b;


        Pen pen_red = new Pen(Color.Red, 2);
        Pen pen_yellow = new Pen(Color.Yellow, 2);
        Pen pen_gray = new Pen(Color.Gray, 2);
        Pen pen_green = new Pen(Color.Green, 2);
        Pen pen_blue = new Pen(Color.Blue, 2);
        
        public Form1()
        {
            InitializeComponent();
            ZOOM = float.Parse(this.zoomtxt.Text);
            OFFSET_X = float.Parse(this.offsetx.Text);
            OFFSET_Y = float.Parse(this.offsety.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 根据txt文件的存储路径path，读取txt文件，把输入零件顶点输入到pieceList
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<Piece> ReadTxt(string path)
        {
            List<Piece> pieceList = new List<Piece>();
            //FileStream fs = new FileStream(Application.StartupPath + path, FileMode.Open, FileAccess.ReadWrite);
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            StreamReader sr = new StreamReader(fs);
            int quantity = 0;//零件数量
            int verticesNum = 0;//零件顶点数目
            LinkedList<_2DNesting.DataStruct.Point> pointList = new LinkedList<_2DNesting.DataStruct.Point>();
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if (line == "QUANTITY")
                {
                    line = sr.ReadLine();
                    quantity = Convert.ToInt32(line);
                }
                else if (line == "NUMBER OF VERTICES")
                {
                    line = sr.ReadLine();
                    verticesNum = Convert.ToInt32(line);
                }
                else if (line == "VERTICES (X,Y)")
                {
                    for (int index = 0; index < verticesNum; index++)
                    {
                        line = sr.ReadLine();
                        string[] tagsArray = line.Split(' ');
                        for (int i = 0; i < tagsArray.Length; i++)
                        {
                            tagsArray[i] = tagsArray[i].Trim();
                        }
                        pointList.AddLast(new _2DNesting.DataStruct.Point(Convert.ToDouble(tagsArray[0]), Convert.ToDouble(tagsArray[1])));
                    }
                    //加入isConvex，isRotateChange标志
                    Polygon temp = new Polygon(pointList);
                    temp.lowLeft = CommonAlogrithm.LowLeftPoint(temp);//应该确定最下最左点，与判断isRotateChange有关系
                    temp.isConvex = CommonAlogrithm.IsConvex(temp);
                    temp.isRotateChange = CommonAlogrithm.IsRotateChange(temp);
                    Piece piece = new Piece(pieceList.Count, temp, quantity);
                    pieceList.Add(piece);
                    pieceDictory.Add(new DictionaryEntry(pieceDictory.Count + "", piece));
                    //pointList.Clear();//不能写成clear
                    pointList = new LinkedList<_2DNesting.DataStruct.Point>();
                }
            }
            sr.Close();
            fs.Close();
            return pieceList;
        }
        public void DisplayPolygon(Polygon polygon, Pen pen, Graphics g)//画笔显示排版零件
        {

            LinkedListNode<_2DNesting.DataStruct.Point> current = polygon.vertex.First;
            LinkedList<_2DNesting.DataStruct.Point> linklist = polygon.vertex;
            if (linklist.Count == 1)
            {
                Pen p = new Pen(Color.Red, 3);
                g.DrawLine(p, float.Parse(current.Value.x.ToString()) * ZOOM + OFFSET_X,
                                     float.Parse(current.Value.y.ToString()) * ZOOM + OFFSET_Y,
                                     float.Parse(current.Value.x.ToString()) * ZOOM + OFFSET_X + 2,
                                     float.Parse(current.Value.y.ToString()) * ZOOM + OFFSET_Y + 2);
                return;//处理退化成点
            }

            while (current != polygon.vertex.Last)
            {
                g.DrawLine(pen, float.Parse(current.Value.x.ToString()) * ZOOM + OFFSET_X,
                                     float.Parse(current.Value.y.ToString()) * ZOOM + OFFSET_Y,
                                     float.Parse(current.Next.Value.x.ToString()) * ZOOM + OFFSET_X,
                                     float.Parse(current.Next.Value.y.ToString()) * ZOOM + OFFSET_Y);
                current = current.Next;
            }
            g.DrawLine(pen, float.Parse(current.Value.x.ToString()) * ZOOM + OFFSET_X,
                                     float.Parse(current.Value.y.ToString()) * ZOOM + OFFSET_Y,
                                     float.Parse(polygon.vertex.First.Value.x.ToString()) * ZOOM + OFFSET_X,
                                     float.Parse(polygon.vertex.First.Value.y.ToString()) * ZOOM + OFFSET_Y);
        }
        public void DisplayPolygon(Polygon polygon, Pen pen, Graphics g, int h)//画笔显示排版零件
        {
            float height = h;
            LinkedListNode<_2DNesting.DataStruct.Point> current = polygon.vertex.First;
            LinkedList<_2DNesting.DataStruct.Point> linklist = polygon.vertex;
            if (linklist.Count == 1)
            {
                Pen p = new Pen(Color.Red, 3);
                g.DrawLine(p, float.Parse(current.Value.x.ToString()) * ZOOM + OFFSET_X,
                                  height - (float.Parse(current.Value.y.ToString()) * ZOOM + OFFSET_Y),
                                     float.Parse(current.Value.x.ToString()) * ZOOM + OFFSET_X + 2,
                                  height - (float.Parse(current.Value.y.ToString()) * ZOOM + OFFSET_Y + 2));
                return;//处理退化成点
            }

            while (current != polygon.vertex.Last)
            {
                g.DrawLine(pen, float.Parse(current.Value.x.ToString()) * ZOOM + OFFSET_X,
                                    height - (float.Parse(current.Value.y.ToString()) * ZOOM + OFFSET_Y),
                                     float.Parse(current.Next.Value.x.ToString()) * ZOOM + OFFSET_X,
                                    height - (float.Parse(current.Next.Value.y.ToString()) * ZOOM + OFFSET_Y));
                current = current.Next;
            }
            g.DrawLine(pen, float.Parse(current.Value.x.ToString()) * ZOOM + OFFSET_X,
                                    height - (float.Parse(current.Value.y.ToString()) * ZOOM + OFFSET_Y),
                                     float.Parse(polygon.vertex.First.Value.x.ToString()) * ZOOM + OFFSET_X,
                                   height - (float.Parse(polygon.vertex.First.Value.y.ToString()) * ZOOM + OFFSET_Y));
        }

        public void DisplayLines(List<Line> lines, Pen pen, Graphics g)//画笔显示排版零件
        {

            for (int i = 0; i < lines.Count; i++)
            {
                g.DrawLine(pen, float.Parse(lines[i].startPoint.x.ToString()) * ZOOM + OFFSET_X,
                                         float.Parse(lines[i].startPoint.y.ToString()) * ZOOM + OFFSET_Y,
                                         float.Parse(lines[i].endPoint.x.ToString()) * ZOOM + OFFSET_X,
                                         float.Parse(lines[i].endPoint.y.ToString()) * ZOOM + OFFSET_Y);
            }

        }
        public void DisplayLines(List<Line> lines, Pen pen, Graphics g, int h)//画笔显示排版零件
        {
            float height = h;
            for (int i = 0; i < lines.Count; i++)
            {
                g.DrawLine(pen, float.Parse(lines[i].startPoint.x.ToString()) * ZOOM + OFFSET_X,
                                       height - (float.Parse(lines[i].startPoint.y.ToString()) * ZOOM + OFFSET_Y),
                                         float.Parse(lines[i].endPoint.x.ToString()) * ZOOM + OFFSET_X,
                                        height - (float.Parse(lines[i].endPoint.y.ToString()) * ZOOM + OFFSET_Y));
            }

        }

        public void DisplayPolygon(Polygon polygon, Brush brush, Graphics g)//画刷染色，表示零件
        {
            PointF[] points = new PointF[polygon.vertex.Count];
            if (points.Count() == 1)
            {
                return;//处理退化成点
            }
            LinkedListNode<_2DNesting.DataStruct.Point> current = polygon.vertex.First;
            for (int i = 0; i < polygon.vertex.Count; i++)
            {
                float x = float.Parse(current.Value.x.ToString()) * ZOOM + OFFSET_X;
                float y = float.Parse(current.Value.y.ToString()) * ZOOM + OFFSET_Y;
                points[i] = new PointF(x, y);
                current = current.Next;
            }
            FillMode newFillMode = FillMode.Winding;
            float tension = 0.000001F;
            g.FillClosedCurve(brush, points, newFillMode, tension);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            LinkedList<_2DNesting.DataStruct.Point> list1 = new LinkedList<_2DNesting.DataStruct.Point>();//点的链表，用来生成多边形

            list1.AddLast(new _2DNesting.DataStruct.Point(0, 0));
            list1.AddLast(new _2DNesting.DataStruct.Point(30, 0));
            list1.AddLast(new _2DNesting.DataStruct.Point(30, 10));
            list1.AddLast(new _2DNesting.DataStruct.Point(10, 10));
            list1.AddLast(new _2DNesting.DataStruct.Point(10, 20));
            list1.AddLast(new _2DNesting.DataStruct.Point(25, 20));
            list1.AddLast(new _2DNesting.DataStruct.Point(40, 30));
            list1.AddLast(new _2DNesting.DataStruct.Point(0, 30));

            Polygon test = new Polygon(list1);//求解退化线

            LinkedList<_2DNesting.DataStruct.Point> list3 = new LinkedList<_2DNesting.DataStruct.Point>();//点的链表，用来生成多边形
            list3.AddLast(new _2DNesting.DataStruct.Point(0, 0));
            list3.AddLast(new _2DNesting.DataStruct.Point(60, 0));
            list3.AddLast(new _2DNesting.DataStruct.Point(60, 30));
            list3.AddLast(new _2DNesting.DataStruct.Point(40, 30));
            list3.AddLast(new _2DNesting.DataStruct.Point(40, 10));
            list3.AddLast(new _2DNesting.DataStruct.Point(10, 10));
            list3.AddLast(new _2DNesting.DataStruct.Point(30, 30));
            list3.AddLast(new _2DNesting.DataStruct.Point(0, 30));

            Polygon test2 = new Polygon(list3);//求解内接nfp

            LinkedList<_2DNesting.DataStruct.Point> list4 = new LinkedList<_2DNesting.DataStruct.Point>();//点的链表，用来生成多边形
            list4.AddLast(new _2DNesting.DataStruct.Point(0, 0));
            list4.AddLast(new _2DNesting.DataStruct.Point(30, 0));
            list4.AddLast(new _2DNesting.DataStruct.Point(30, 10));
            list4.AddLast(new _2DNesting.DataStruct.Point(10, 10));

            Polygon inner = new Polygon(list4);//退化成点的多变形b


            LinkedList<_2DNesting.DataStruct.Point> list2 = new LinkedList<_2DNesting.DataStruct.Point>();//点的链表，用来生成多边形
            list2.AddLast(new _2DNesting.DataStruct.Point(0, 0));
            list2.AddLast(new _2DNesting.DataStruct.Point(10, 0));
            list2.AddLast(new _2DNesting.DataStruct.Point(10, 10));
            list2.AddLast(new _2DNesting.DataStruct.Point(0, 10));
            Polygon rect = new Polygon(list2);//小矩形

            LinkedList<_2DNesting.DataStruct.Point> list5 = new LinkedList<_2DNesting.DataStruct.Point>();//点的链表，用来生成多边形
            list5.AddLast(new _2DNesting.DataStruct.Point(0, 0));
            list5.AddLast(new _2DNesting.DataStruct.Point(0, 40));
            list5.AddLast(new _2DNesting.DataStruct.Point(40, 40));
            list5.AddLast(new _2DNesting.DataStruct.Point(40, 0));
            Polygon rect1 = new Polygon(list5);//顺时针，相当于求解内靠接NFP

            LinkedList<_2DNesting.DataStruct.Point> list6 = new LinkedList<_2DNesting.DataStruct.Point>();//点的链表，用来生成多边形
            list6.AddLast(new _2DNesting.DataStruct.Point(0, 0));
            list6.AddLast(new _2DNesting.DataStruct.Point(50, 0));
            list6.AddLast(new _2DNesting.DataStruct.Point(50, 40));
            list6.AddLast(new _2DNesting.DataStruct.Point(40, 40));
            list6.AddLast(new _2DNesting.DataStruct.Point(20, 20));
            list6.AddLast(new _2DNesting.DataStruct.Point(0, 40));
            Polygon test5 = new Polygon(list6);//逆时针，相当于求解内靠接NFP


            LinkedList<_2DNesting.DataStruct.Point> list7 = new LinkedList<_2DNesting.DataStruct.Point>();//点的链表，用来生成多边形

            list7.AddLast(new _2DNesting.DataStruct.Point(5, 0));
            list7.AddLast(new _2DNesting.DataStruct.Point(10, 10));
            list7.AddLast(new _2DNesting.DataStruct.Point(5, 5));
            list7.AddLast(new _2DNesting.DataStruct.Point(0, 10));

            Polygon test6 = new Polygon(list7);//逆时针，相当于求解内靠接NFP


            Graphics g = this.groupBox1.CreateGraphics();
            int height = this.groupBox1.Height;
            //MatrixOrder m=new MatrixOrder(1,0,0;0,-1,0;0,0,1);
            //g.TranslateTransform()
            Pen p = new Pen(Color.Red, 2);
            Pen p1 = new Pen(Color.Yellow, 2);
            Pen p2 = new Pen(Color.Gray, 2);
            Pen p3 = new Pen(Color.Blue, 2);
            DisplayPolygon(test, p1, g, height);
            //DisplayPolygon(inner, p1, g, height);
            List<Polygon> nfps = CommonAlogrithm.getNfp(test, test);
            //List<Polygon> nfps = new List<Polygon>();
            //Polygon a = test2;
            //Polygon b = test2;
            ////1、求轨迹线集合
            //List<Line> gjxLines = CommonAlogrithm.getGjxLines(CommonAlogrithm.reversePolygon(a), b);

            //DisplayLines(gjxLines, p3, g, height);
            ////切割轨迹线
            //List<_2DNesting.DataStruct.Point> specialPoint = new List<_2DNesting.DataStruct.Point>();
            //List<Line> cutGjxs = CommonAlogrithm.cutGjxLines(gjxLines, specialPoint);

            ////5、求内部nfp，可能有多个
            //List<Polygon> inLs = CommonAlogrithm.GetInLoop(cutGjxs, a, specialPoint, gjxLines);
            //for (int i = 0; i < inLs.Count; i++)
            //{
            //    nfps.Add(inLs[i]);
            //}


            for (int i = 0; i < nfps.Count; i++)
            {
                DisplayPolygon(nfps[i], p, g, height);
            }



        }

        private void button2_Click(object sender, EventArgs e)
        {
            LinkedList<_2DNesting.DataStruct.Point> list1 = new LinkedList<_2DNesting.DataStruct.Point>();//点的链表，用来生成多边形

            list1.AddLast(new _2DNesting.DataStruct.Point(0, 0));
            list1.AddLast(new _2DNesting.DataStruct.Point(30, 0));
            list1.AddLast(new _2DNesting.DataStruct.Point(30, 10));
            list1.AddLast(new _2DNesting.DataStruct.Point(10, 10));
            list1.AddLast(new _2DNesting.DataStruct.Point(10, 20));
            list1.AddLast(new _2DNesting.DataStruct.Point(30, 20));
            list1.AddLast(new _2DNesting.DataStruct.Point(40, 30));
            list1.AddLast(new _2DNesting.DataStruct.Point(0, 30));

            Polygon test = new Polygon(list1);//求解退化线

            LinkedList<_2DNesting.DataStruct.Point> list3 = new LinkedList<_2DNesting.DataStruct.Point>();//点的链表，用来生成多边形
            list3.AddLast(new _2DNesting.DataStruct.Point(0, 0));
            list3.AddLast(new _2DNesting.DataStruct.Point(60, 0));
            list3.AddLast(new _2DNesting.DataStruct.Point(60, 30));
            list3.AddLast(new _2DNesting.DataStruct.Point(40, 30));
            list3.AddLast(new _2DNesting.DataStruct.Point(40, 10));
            list3.AddLast(new _2DNesting.DataStruct.Point(10, 10));
            list3.AddLast(new _2DNesting.DataStruct.Point(30, 30));
            list3.AddLast(new _2DNesting.DataStruct.Point(0, 30));

            Polygon test2 = new Polygon(list3);//求解内接nfp

            LinkedList<_2DNesting.DataStruct.Point> list4 = new LinkedList<_2DNesting.DataStruct.Point>();//点的链表，用来生成多边形
            list4.AddLast(new _2DNesting.DataStruct.Point(0, 0));
            list4.AddLast(new _2DNesting.DataStruct.Point(30, 0));
            list4.AddLast(new _2DNesting.DataStruct.Point(30, 10));
            list4.AddLast(new _2DNesting.DataStruct.Point(10, 10));

            Polygon inner = new Polygon(list4);//退化成点的多变形b


            LinkedList<_2DNesting.DataStruct.Point> list2 = new LinkedList<_2DNesting.DataStruct.Point>();//点的链表，用来生成多边形
            list2.AddLast(new _2DNesting.DataStruct.Point(0, 0));
            list2.AddLast(new _2DNesting.DataStruct.Point(10, 0));
            list2.AddLast(new _2DNesting.DataStruct.Point(10, 10));
            list2.AddLast(new _2DNesting.DataStruct.Point(0, 10));
            Polygon rect = new Polygon(list2);//小矩形

            LinkedList<_2DNesting.DataStruct.Point> list5 = new LinkedList<_2DNesting.DataStruct.Point>();//点的链表，用来生成多边形
            list5.AddLast(new _2DNesting.DataStruct.Point(0, 0));
            list5.AddLast(new _2DNesting.DataStruct.Point(0, 40));
            list5.AddLast(new _2DNesting.DataStruct.Point(40, 40));
            list5.AddLast(new _2DNesting.DataStruct.Point(40, 0));
            Polygon rect1 = new Polygon(list5);//顺时针，相当于求解内靠接NFP

            LinkedList<_2DNesting.DataStruct.Point> list6 = new LinkedList<_2DNesting.DataStruct.Point>();//点的链表，用来生成多边形
            list6.AddLast(new _2DNesting.DataStruct.Point(0, 0));
            list6.AddLast(new _2DNesting.DataStruct.Point(50, 0));
            list6.AddLast(new _2DNesting.DataStruct.Point(50, 40));
            list6.AddLast(new _2DNesting.DataStruct.Point(40, 40));
            list6.AddLast(new _2DNesting.DataStruct.Point(20, 20));
            list6.AddLast(new _2DNesting.DataStruct.Point(0, 40));
            Polygon test5 = new Polygon(list6);//逆时针，相当于求解内靠接NFP


            LinkedList<_2DNesting.DataStruct.Point> list7 = new LinkedList<_2DNesting.DataStruct.Point>();//点的链表，用来生成多边形

            list7.AddLast(new _2DNesting.DataStruct.Point(5, 0));
            list7.AddLast(new _2DNesting.DataStruct.Point(10, 10));
            list7.AddLast(new _2DNesting.DataStruct.Point(5, 5));
            list7.AddLast(new _2DNesting.DataStruct.Point(0, 10));

            

            Graphics g = this.groupBox1.CreateGraphics();
            int height = this.groupBox1.Height;

            DisplayPolygon(test2, pen_gray, g, height);
            Polygon a = test2;
            Polygon b = test2;
            List<Polygon> nfps = new List<Polygon>();
            //1、求轨迹线集合
            List<Line> gjxLines = CommonAlogrithm.getGjxLines(CommonAlogrithm.reversePolygon(a), b);
            DisplayLines(gjxLines, pen_blue, g, height);
            ////2、求退化线

            ////3、求退化点

            //切割轨迹线
            List<_2DNesting.DataStruct.Point> specialPoint = new List<_2DNesting.DataStruct.Point>();
            List<Line> cutGjxs = CommonAlogrithm.cutGjxLines(gjxLines, specialPoint);
            //4、求外围nfp
            //Polygon outL = GetOutLoop(cutGjxs, LowLeftPoint(cutGjxs), specialPoint);
            // nfps.Add(outL);
            //5、求内部nfp，可能有多个
            List<Polygon> inLs = CommonAlogrithm.GetInLoop(cutGjxs, a, specialPoint, gjxLines);
            for (int i = 0; i < inLs.Count; i++)
            {
                nfps.Add(inLs[i]);
            }

            for (int i = 0; i < nfps.Count; i++)
            {
                DisplayPolygon(nfps[i], pen_red, g, height);
            }


        }

        private void button4_Click(object sender, EventArgs e)
        {
            LinkedList<_2DNesting.DataStruct.Point> list1 = new LinkedList<_2DNesting.DataStruct.Point>();//点的链表，用来生成多边形

            list1.AddLast(new _2DNesting.DataStruct.Point(0, 0));
            list1.AddLast(new _2DNesting.DataStruct.Point(30, 0));
            list1.AddLast(new _2DNesting.DataStruct.Point(30, 10));
            list1.AddLast(new _2DNesting.DataStruct.Point(10, 10));
            list1.AddLast(new _2DNesting.DataStruct.Point(10, 20));
            list1.AddLast(new _2DNesting.DataStruct.Point(30, 20));
            list1.AddLast(new _2DNesting.DataStruct.Point(40, 30));
            list1.AddLast(new _2DNesting.DataStruct.Point(0, 30));

            Polygon test = new Polygon(list1);//求解退化线

            LinkedList<_2DNesting.DataStruct.Point> list3 = new LinkedList<_2DNesting.DataStruct.Point>();//点的链表，用来生成多边形
            list3.AddLast(new _2DNesting.DataStruct.Point(0, 0));
            list3.AddLast(new _2DNesting.DataStruct.Point(60, 0));
            list3.AddLast(new _2DNesting.DataStruct.Point(60, 30));
            list3.AddLast(new _2DNesting.DataStruct.Point(40, 30));
            list3.AddLast(new _2DNesting.DataStruct.Point(40, 10));
            list3.AddLast(new _2DNesting.DataStruct.Point(10, 10));
            list3.AddLast(new _2DNesting.DataStruct.Point(30, 30));
            list3.AddLast(new _2DNesting.DataStruct.Point(0, 30));

            Polygon test2 = new Polygon(list3);//求解内接nfp

            LinkedList<_2DNesting.DataStruct.Point> list4 = new LinkedList<_2DNesting.DataStruct.Point>();//点的链表，用来生成多边形
            list4.AddLast(new _2DNesting.DataStruct.Point(0, 0));
            list4.AddLast(new _2DNesting.DataStruct.Point(30, 0));
            list4.AddLast(new _2DNesting.DataStruct.Point(30, 10));
            list4.AddLast(new _2DNesting.DataStruct.Point(10, 10));

            Polygon inner = new Polygon(list4);//退化成点的多变形b


            LinkedList<_2DNesting.DataStruct.Point> list2 = new LinkedList<_2DNesting.DataStruct.Point>();//点的链表，用来生成多边形
            list2.AddLast(new _2DNesting.DataStruct.Point(0, 0));
            list2.AddLast(new _2DNesting.DataStruct.Point(10, 0));
            list2.AddLast(new _2DNesting.DataStruct.Point(10, 10));
            list2.AddLast(new _2DNesting.DataStruct.Point(0, 10));
            Polygon rect = new Polygon(list2);//小矩形

            LinkedList<_2DNesting.DataStruct.Point> list5 = new LinkedList<_2DNesting.DataStruct.Point>();//点的链表，用来生成多边形
            list5.AddLast(new _2DNesting.DataStruct.Point(0, 0));
            list5.AddLast(new _2DNesting.DataStruct.Point(0, 40));
            list5.AddLast(new _2DNesting.DataStruct.Point(40, 40));
            list5.AddLast(new _2DNesting.DataStruct.Point(40, 0));
            Polygon rect1 = new Polygon(list5);//顺时针，相当于求解内靠接NFP

            LinkedList<_2DNesting.DataStruct.Point> list6 = new LinkedList<_2DNesting.DataStruct.Point>();//点的链表，用来生成多边形
            list6.AddLast(new _2DNesting.DataStruct.Point(0, 0));
            list6.AddLast(new _2DNesting.DataStruct.Point(50, 0));
            list6.AddLast(new _2DNesting.DataStruct.Point(50, 40));
            list6.AddLast(new _2DNesting.DataStruct.Point(40, 40));
            list6.AddLast(new _2DNesting.DataStruct.Point(20, 20));
            list6.AddLast(new _2DNesting.DataStruct.Point(0, 40));
            Polygon test5 = new Polygon(list6);//逆时针，相当于求解内靠接NFP


            LinkedList<_2DNesting.DataStruct.Point> list7 = new LinkedList<_2DNesting.DataStruct.Point>();//点的链表，用来生成多边形

            list7.AddLast(new _2DNesting.DataStruct.Point(5, 0));
            list7.AddLast(new _2DNesting.DataStruct.Point(10, 10));
            list7.AddLast(new _2DNesting.DataStruct.Point(5, 5));
            list7.AddLast(new _2DNesting.DataStruct.Point(0, 10));

            Polygon test6 = new Polygon(list7);//逆时针，相当于求解内靠接NFP
            Graphics g = this.groupBox1.CreateGraphics();
            int height = this.groupBox1.Height;
            
            DisplayPolygon(test, pen_gray, g, height);
            Polygon a = test;
            Polygon b = rect;
            
            List<Polygon> nfps = new List<Polygon>();
            //1、求轨迹线集合
            List<Line> gjxLines = CommonAlogrithm.getGjxLines(a, b);
            DisplayLines(gjxLines, pen_blue, g, height);
            //2、求退化线
            //List<Line> degradeLines = getDegradeLines(gjxLines);
            ////
            //for (int i = 0; i < degradeLines.Count; i++)
            //{
            //    nfps.Add(LineToPolygon(degradeLines[i]));
            //}


            //3、求退化点
            List<_2DNesting.DataStruct.Point> degradePoints = CommonAlogrithm.getDegradePoints(gjxLines);
            for (int i = 0; i < degradePoints.Count; i++)
            {
                nfps.Add(CommonAlogrithm.PointToPolygon(degradePoints[i]));
            }

            //切割轨迹线
            List<_2DNesting.DataStruct.Point> specialPoint = new List<_2DNesting.DataStruct.Point>();
            List<Line> cutGjxs = CommonAlogrithm.cutGjxLines(gjxLines, specialPoint);
            //4、求外围nfp
            Polygon outL = CommonAlogrithm.GetOutLoop(cutGjxs, CommonAlogrithm.LowLeftPoint(cutGjxs), specialPoint);
            nfps.Add(outL);
            //5、求内部nfp，可能有多个
            List<Polygon> inLs = CommonAlogrithm.GetInLoop(cutGjxs, outL, specialPoint, gjxLines);
            for (int i = 0; i < inLs.Count; i++)
            {
                nfps.Add(inLs[i]);
            }
            for (int i = 0; i < nfps.Count; i++)
            {
                DisplayPolygon(nfps[i], pen_red, g, height);
            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            LinkedList<_2DNesting.DataStruct.Point> list1 = new LinkedList<_2DNesting.DataStruct.Point>();//点的链表，用来生成多边形

            list1.AddLast(new _2DNesting.DataStruct.Point(0, 0));
            list1.AddLast(new _2DNesting.DataStruct.Point(30, 0));
            list1.AddLast(new _2DNesting.DataStruct.Point(30, 10));
            list1.AddLast(new _2DNesting.DataStruct.Point(10, 10));
            list1.AddLast(new _2DNesting.DataStruct.Point(10, 20));
            list1.AddLast(new _2DNesting.DataStruct.Point(30, 20));
            list1.AddLast(new _2DNesting.DataStruct.Point(40, 30));
            list1.AddLast(new _2DNesting.DataStruct.Point(0, 30));

            Polygon test = new Polygon(list1);//求解退化线

            Graphics g = this.groupBox1.CreateGraphics();
            int height = this.groupBox1.Height;

            DisplayPolygon(test, pen_gray, g, height);
            Polygon a = test;
            Polygon b = test;
            List<Polygon> nfps = new List<Polygon>();
            //1、求轨迹线集合
            List<Line> gjxLines = CommonAlogrithm.getGjxLines(CommonAlogrithm.reversePolygon(a), b);
            DisplayLines(gjxLines, pen_blue, g, height);
            ////2、求退化线

            ////3、求退化点

            //切割轨迹线
            List<_2DNesting.DataStruct.Point> specialPoint = new List<_2DNesting.DataStruct.Point>();
            List<Line> cutGjxs = CommonAlogrithm.cutGjxLines(gjxLines, specialPoint);
            //4、求外围nfp
            //Polygon outL = GetOutLoop(cutGjxs, LowLeftPoint(cutGjxs), specialPoint);
            // nfps.Add(outL);
            //5、求内部nfp，可能有多个
            List<Polygon> inLs = CommonAlogrithm.GetInLoop(cutGjxs, a, specialPoint, gjxLines);
            for (int i = 0; i < inLs.Count; i++)
            {
                nfps.Add(inLs[i]);
            }

            for (int i = 0; i < nfps.Count; i++)
            {
                DisplayPolygon(nfps[i], pen_red, g, height);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            LinkedList<_2DNesting.DataStruct.Point> list7 = new LinkedList<_2DNesting.DataStruct.Point>();//点的链表，用来生成多边形

            list7.AddLast(new _2DNesting.DataStruct.Point(0, 0));
            list7.AddLast(new _2DNesting.DataStruct.Point(10, -10));
            list7.AddLast(new _2DNesting.DataStruct.Point(10,0));
         //   list7.AddLast(new _2DNesting.DataStruct.Point(0, 10));
             Polygon test6 = new Polygon(list7);
             Polygon a = test6;
             MessageBox.Show(a.Area()+"");
        }
        //读取板材信息的txt文件；
        private void selectFile_Click(object sender, EventArgs e)
        {
            string path = "";
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory ="H:\\排样算法\\2DNesting-2D\\板材样例";
            openFileDialog1.Filter = "All files (*.*)|*.*|||txt files (*.txt)|*.txt";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog1.FileName;
                //string path = "\\NewBlaz.txt";
                pieceList.Clear();
                pieceDictory.Clear();
                pieceList = ReadTxt(path);//从txt文档提取多边形顶点信息
                totalType = pieceList.Count;
                for (int i = 0; i < pieceList.Count; i++)
                {
                    totalArea = totalArea + pieceList[i].parts.Area() * pieceList[i].num;
                    totalNum = totalNum + pieceList[i].num;
                }
                DateTime start = DateTime.Now;
                // Initialize();//NFP和IFR初始化
                DateTime end = DateTime.Now;
                //this.label1.Text = "NFP数总数：" + noFitPolygonList.Count + "\r\n" +
                //                            "IFR总数：" + innerFitRectangleList.Count + "\r\n" +
                //                              (end - start).ToString();
                this.label4.Text = path.Substring(path.LastIndexOf("\\") + 1);


                if (pieceList.Count != 0)
                {
                    MessageBox.Show("种类：" + pieceList.Count.ToString() + "\r\n"
                                                    + "总数：" + totalNum.ToString() + "\r\n"
                                                        + "总面积：" + totalArea.ToString() + "\r\n",
                                                        path.Substring(path.LastIndexOf("\\") + 1));
                }

                this.comboBox1.DataSource = pieceDictory;
                this.comboBox1.DisplayMember = "Key";
                this.comboBox2.DataSource = new ArrayList(pieceDictory);
                this.comboBox2.DisplayMember = "Key";
            }
        }

        private void NFPbutton_Click(object sender, EventArgs e)
        {
            String s=null;
            Graphics g = this.groupBox1.CreateGraphics();
            g.Clear(Color.White);
            int height = this.groupBox1.Height;
            Object obj1 = this.comboBox1.SelectedItem;
            Object obj2 = this.comboBox2.SelectedItem;
            if (obj1 == null || obj2 == null)
            {
                MessageBox.Show(" 未选中要求解的多边形");
                return;
            }
            Piece piece_a = (Piece)((DictionaryEntry)obj1).Value;
            Piece piece_b = (Piece)((DictionaryEntry)obj2).Value;
            a = CommonAlogrithm.CopyPolygon(piece_a.parts);
            b = CommonAlogrithm.CopyPolygon(piece_b.parts);
            if (radioButton2.Checked)
            {
                DateTime start = DateTime.Now;


               // List<Polygon> nfps = CommonAlogrithm.getNfp(a, b);
               /****************************** 为了调试 ********************************/
                List<Polygon> nfps = new List<Polygon>();
                //1、求轨迹线集合
                List<Line> gjxLines = CommonAlogrithm.getGjxLines(a, b);
                DisplayLines(gjxLines, pen_blue, g, height);
                //2、求退化线
                //List<Line> degradeLines = getDegradeLines(gjxLines);
                ////
                //for (int i = 0; i < degradeLines.Count; i++)
                //{
                //    nfps.Add(LineToPolygon(degradeLines[i]));
                //}
                //3、求退化点
                List<_2DNesting.DataStruct.Point> degradePoints = CommonAlogrithm.getDegradePoints(gjxLines);
                for (int i = 0; i < degradePoints.Count; i++)
                {
                    nfps.Add(CommonAlogrithm.PointToPolygon(degradePoints[i]));
                }

                //切割轨迹线
                List<_2DNesting.DataStruct.Point> specialPoint = new List<_2DNesting.DataStruct.Point>();
                List<Line> cutGjxs = CommonAlogrithm.cutGjxLines(gjxLines, specialPoint);
                //4、求外围nfp
                Polygon outL = CommonAlogrithm.GetOutLoop(cutGjxs, CommonAlogrithm.LowLeftPoint(cutGjxs), specialPoint);
                nfps.Add(outL);
                //5、求内部nfp，可能有多个
                List<Polygon> inLs = CommonAlogrithm.GetInLoop(cutGjxs, a, specialPoint, gjxLines);
                for (int i = 0; i < inLs.Count; i++)
                {
                    nfps.Add(inLs[i]);
                }




                /******************************  ********************************/
                DateTime end = DateTime.Now;
                TimeSpan ts = end - start;
                s = ts.TotalMilliseconds.ToString();
                DisplayPolygon(a, pen_gray, g, height);
                DisplayPolygon(b, pen_gray, g, height);
                for (int i = 0; i < nfps.Count; i++)
                {
                    DisplayPolygon(nfps[i], pen_red, g, height);
                }

            }
            else if (radioButton1.Checked)
            {
                DisplayPolygon(a, pen_gray, g, height);
                DisplayPolygon(b, pen_gray, g, height);
                DateTime start = DateTime.Now;
               // List<Polygon> nfps = CommonAlogrithm.getInnerNfp(a, b);
                /****************************** 为了调试 ********************************/
                DisplayPolygon(a, pen_gray, g, height);
                
                List<Polygon> nfps = new List<Polygon>();
                //1、求轨迹线集合
                List<Line> gjxLines = CommonAlogrithm.getGjxLines(CommonAlogrithm.reversePolygon(a), b);
                DisplayLines(gjxLines, pen_blue, g, height);
                ////2、求退化线

                ////3、求退化点

                //切割轨迹线
                List<_2DNesting.DataStruct.Point> specialPoint = new List<_2DNesting.DataStruct.Point>();
                List<Line> cutGjxs = CommonAlogrithm.cutGjxLines(gjxLines, specialPoint);
                //4、求外围nfp
                //Polygon outL = GetOutLoop(cutGjxs, LowLeftPoint(cutGjxs), specialPoint);
                // nfps.Add(outL);
                //5、求内部nfp，可能有多个
                List<Polygon> inLs = CommonAlogrithm.GetInLoop(cutGjxs, a, specialPoint, gjxLines);
                for (int i = 0; i < inLs.Count; i++)
                {
                    nfps.Add(inLs[i]);
                }
                /****************************** 为了调试 ********************************/
                DateTime end = DateTime.Now;
                TimeSpan ts = end - start;
                s = ts.TotalMilliseconds.ToString();
                for (int i = 0; i < nfps.Count; i++)
                {
                    DisplayPolygon(nfps[i], pen_red, g, height);
                }

            }
            else
            {
                MessageBox.Show("请选择外靠接或内靠接");
            }
            MessageBox.Show(s);
        }

        private void offsetx_TextChanged(object sender, EventArgs e)
        {
            OFFSET_X = float.Parse(offsetx.Text);
        }

        private void offsety_TextChanged(object sender, EventArgs e)
        {
            OFFSET_Y = float.Parse(offsety.Text);
        }

        private void zoomtxt_TextChanged(object sender, EventArgs e)
        {
            ZOOM = float.Parse(zoomtxt.Text);
        }

        private void bestbutton_Click(object sender, EventArgs e)
        {
            _2DNesting.DataStruct.Point p = new _2DNesting.DataStruct.Point();
            double value= CommonAlogrithm.GetBestContact(a, b, p);
            Graphics g = this.groupBox1.CreateGraphics();
            int height = this.groupBox1.Height;
            DisplayPolygon(b, pen_gray, g, height);
            MessageBox.Show("利用率为：" + value);
        }

        
    }
}