using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

namespace GraphVisualizer
{
    /// <summary>
    /// A class to Visualize the graph
    /// </summary>
    static class Visualizer
    {
        static Pen edgePen, vertexPen;
        static Graphics graphics;
        static Bitmap bitmap;
        static Brush b;
        static Font f;

        /// <summary>
        /// Start visualizing
        /// </summary>
        /// <param name="g">The graph to Visualize</param>
        /// <param name="path">The path to write an image to</param>
        public static void Visualize(Graph g, string path)
        {
            edgePen = new Pen(Color.Green);
            b = new SolidBrush(Color.White);
            vertexPen = new Pen(Color.Red);
            f = new Font("Arial", 12);
            bitmap = new Bitmap(300, 300);
            graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);

            DrawGraph(g, path);
        }

        /// <summary>
        /// Draw the graph to a file
        /// </summary>
        /// <param name="g">The graph to draw</param>
        /// <param name="path">The file to draw to</param>
        public static void DrawGraph(Graph g, string path)
        {
            foreach (Node n in g.nodes)
            {
                DrawNode(n);
            }

            foreach (Edge e in g.edges)
            {
                DrawEdge(e);
            }
            Write(path);
        }

        /// <summary>
        /// Draw a single node
        /// </summary>
        /// <param name="n">The node to draw</param>
        private static void DrawNode(Node n)
        {
            graphics.DrawEllipse(vertexPen, ToRectangle(ToPoint(n.position)));
            graphics.DrawString(n.label, f, b, LabelPoint(n.position));
        }

        /// <summary>
        /// Draw a single edge
        /// </summary>
        /// <param name="e">The edge to draw</param>
        private static void DrawEdge(Edge e)
        {
            graphics.DrawLine(edgePen, ToPoint(e.left.position), ToPoint(e.right.position));
        }

        /// <summary>
        /// Convert a point to a rectangle
        /// </summary>
        /// <param name="p">The point to convert</param>
        /// <returns>The rectangle based on the point</returns>
        private static RectangleF ToRectangle(PointF p)
        {
            return new RectangleF(p.X - 2, p.Y - 2, 4, 4);
        }

        /// <summary>
        /// Convert a vector to a point. Places the X and Y components of a Vector2 into the X and Y positions of a point
        /// </summary>
        /// <param name="v">The vector to convert</param>
        /// <returns>The point</returns>
        private static PointF ToPoint(Vector2 v)
        {
            return new PointF(v.X, v.Y);
        }

        /// <summary>
        /// Calculate where to draw the label
        /// </summary>
        /// <param name="v">The vector of the node</param>
        /// <returns>A point where the label is drawn</returns>
        private static PointF LabelPoint(Vector2 v)
        {
            return new PointF((float)(v.X - 10.0), v.Y - 2);
        }

        /// <summary>
        /// Write the final output to the file
        /// </summary>
        /// <param name="path">The path to write to</param>
        private static void Write(string path)
        {
            bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Bmp);
        }
    }
}
