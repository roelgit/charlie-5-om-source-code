using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

namespace GraphVisualizer
{
    static class Visualizer
    {
        static Pen edgePen, vertexPen;
        static Graphics graphics;
        static Bitmap bitmap;
        static Brush b;
        static Font f;

        public static void visualize(Graph g, string path)
        {
            edgePen = new Pen(Color.Green);
            vertexPen = new Pen(Color.Red);
            f = new Font("Arial", 12);
            bitmap = new Bitmap(300, 300);
            graphics = Graphics.FromImage(bitmap);

            DrawGraph(g, path);
        }

        public static void DrawGraph(Graph g, string path)
        {
            foreach (Node n in g.nodes)
            {
                drawNode(n);
            }

            foreach (Edge e in g.edges)
            {
                drawEdge(e);
            }
            write(path);
        }

        private static void drawNode(Node n)
        {
            graphics.DrawEllipse(vertexPen, toRectangle(toPoint(n.position)));
            graphics.DrawString(n.label, f, b, labelPoint(n.position));
        }

        private static void drawEdge(Edge e)
        {
            graphics.DrawLine(edgePen, toPoint(e.left.position), toPoint(e.right.position));
        }

        private static RectangleF toRectangle(PointF p)
        {
            return new RectangleF(p.X - 2, p.Y - 2, 4, 4);
        }

        private static PointF toPoint(Vector2 v)
        {
            return new PointF(v.X, v.Y);
        }

        private static PointF labelPoint(Vector2 v)
        {
            return new PointF((float)(v.X - 10.0), v.Y - 2);
        }

        // write final document to file
        private static void write(string path)
        {
            bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Bmp);
        }
    }
}
