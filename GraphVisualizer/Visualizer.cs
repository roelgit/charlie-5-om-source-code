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
    internal class Visualizer
    {
        private readonly Pen _edgePen;
        private readonly Pen _vertexPen;
        private Graphics _graphics;
        private Bitmap _bitmap;
        private readonly Brush _backgroundColor;
        private readonly Font _font;
        private GraphStatistics _stats;

        // These member variables are for calculating the right coordinates
        private float _xOffset;
        private float _yOffset;
        private float _xMultiplier;
        private float _yMultiplier;

        /// <summary>
        /// The size of the node (in pixels)
        /// </summary>
        public int NodeSize { get; protected set; }


        /// <summary>
        /// Create a new Visualizer, not specifying the parameters
        /// </summary>
        public Visualizer() : this (Color.White, Color.Red, Color.Green, new Font("Arial", 12))
        {
            
        }

        /// <summary>
        /// Create a new Visualizer, specifying the parameters
        /// </summary>
        /// <param name="backgroundColor">The backgroundColor color of the graph</param>
        /// <param name="nodeColor">The node color of the graph</param>
        /// <param name="edgeColor">The edge color of the graph</param>
        /// <param name="font">The font to use for the labels</param>
        public Visualizer(Color backgroundColor, Color nodeColor, Color edgeColor, Font font)
        {
            this._backgroundColor = new SolidBrush(backgroundColor);
            _edgePen = new Pen(edgeColor);
            _vertexPen = new Pen(nodeColor);
            _font = font;
        }

        /// <summary>
        /// Start visualizing
        /// </summary>
        /// <param name="g">The graph to Visualize</param>
        /// <param name="path">The path to write an image to</param>
        /// <param name="imageWidth">The width of the image file</param>
        /// <param name="imageHeight">The height of the image file</param>
        /// <param name="nodeSize">The diameter of the nodes, in pixels</param>
        public void Visualize(Graph g, string path, int imageWidth = 300, int imageHeight = 300, int nodeSize = 5)
        {
            _stats = GraphStatistics.From(g);
            CalculateScale(_stats, imageWidth, imageHeight);

            _bitmap = new Bitmap(imageWidth, imageHeight);
            _graphics = Graphics.FromImage(_bitmap);
            _graphics.FillRectangle(_backgroundColor, 0, 0, _bitmap.Width, _bitmap.Height);

            NodeSize = nodeSize;

            DrawGraph(g, path);
        }

        /// <summary>
        /// Draw the graph to a file
        /// </summary>
        /// <param name="g">The graph to draw</param>
        /// <param name="path">The file to draw to</param>
        public void DrawGraph(Graph g, string path)
        {
            foreach (var n in g.nodes)
            {
                DrawNode(n);
            }

            foreach (var e in g.edges)
            {
                DrawEdge(e);
            }
            Write(path);
        }

        /// <summary>
        /// Draw a single node
        /// </summary>
        /// <param name="n">The node to draw</param>
        private void DrawNode(Node n)
        {
            PointF nodePosition = ToPoint(n.position);
            Console.WriteLine("Drawing node {0} at {1};{2}", n.label, nodePosition.X, nodePosition.Y);
            _graphics.DrawEllipse(_vertexPen, nodePosition.X, nodePosition.Y, NodeSize, NodeSize);
            _graphics.DrawString(n.label, _font, _backgroundColor, LabelPoint(n.position));
        }

        /// <summary>
        /// Draw a single edge
        /// </summary>
        /// <param name="e">The edge to draw</param>
        private void DrawEdge(Edge e)
        {
            _graphics.DrawLine(_edgePen, ToPoint(e.left.position), ToPoint(e.right.position));
        }

        /// <summary>
        /// Convert a point to a rectangle
        /// </summary>
        /// <param name="p">The point to convert</param>
        /// <returns>The rectangle based on the point</returns>
        private RectangleF ToRectangle(PointF p)
        {
            return new RectangleF((p.X + _xOffset)*_xMultiplier, (p.Y + _yOffset)*_yMultiplier, 4, 4);
        }

        /// <summary>
        /// Convert a vector to a point. Places the X and Y components of a Vector2 into the X and Y positions of a point
        /// </summary>
        /// <param name="v">The vector to convert</param>
        /// <returns>The point</returns>
        private PointF ToPoint(Vector2 v)
        {
            return new PointF((v.X+_xOffset)*_xMultiplier, (v.Y+_yOffset) * _yMultiplier);
        }

        /// <summary>
        /// Calculate where to draw the label
        /// </summary>
        /// <param name="v">The vector of the node</param>
        /// <returns>A point where the label is drawn</returns>
        private PointF LabelPoint(Vector2 v)
        {
            // Place the label at (-10,-2) to the node
            return ToPoint(v) - new Size(10, 2);
        }

        /// <summary>
        /// Write the final output to the file
        /// </summary>
        /// <param name="path">The path to write to</param>
        private void Write(string path)
        {
            _bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Bmp);
        }

        /// <summary>
        /// Calculate the required Bitmap size
        /// </summary>
        /// <param name="stats">The statistics to work with</param>
        /// <param name="intendedWidth">The intended height of the bitmap, in pixels</param>
        /// <param name="intendedHeight">The intended height of the bitmap, in pixels</param>
        /// <returns>A rectangle containing the bitmap size</returns>
        private void CalculateScale(GraphStatistics stats, int intendedWidth = 300, int intendedHeight = 300)
        {
            var rect = new Rectangle();

            rect.X = rect.Y = 0;
            _xMultiplier = intendedWidth/stats.Width;
            _yMultiplier = intendedHeight/stats.Height;
            _xOffset = -stats.GraphArea.X;
            _yOffset = -stats.GraphArea.Y;

            rect.Width = (int)(intendedWidth*_xMultiplier);
            rect.Height = (int) (intendedHeight*_yMultiplier);
        }
    }
}
