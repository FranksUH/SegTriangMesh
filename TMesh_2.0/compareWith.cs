using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TMesh_2._0
{
    public partial class compareWith : Form
    {
        Mesh mesh;
        int[] clusters;
        private double tx, ty, tz, rx, ry, rz, scale;
        private Point lMouseLoc;
        private Color[] colors = { Color.Blue, Color.Red, Color.Green, Color.Gray, Color.Gold, Color.Violet, Color.Cyan, Color.Orange, Color.Lime, Color.Maroon, Color.ForestGreen, Color.DarkCyan,Color.Bisque,Color.Coral,Color.Salmon};
        public compareWith()
        {
            InitializeComponent();
        }
        public compareWith(int[] clusters, Mesh mesh)
        {
            this.mesh = mesh;
            this.clusters = clusters;
            InitializeComponent();
            Initialize();
        }

        private void Board_Paint(object sender, PaintEventArgs e)
        {
            Settings();
            DrawTriangles();
            //DrawLines();
            Board.SwapBuffers();
        }
        private void Board_Load(object sender, EventArgs e)
        {
            SetupViewPort();
            SetupLigthing();
        }
        private void Board_Resize(object sender, EventArgs e)
        {
            SetupViewPort();
            Board.Invalidate();
        }
        private void Board_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.A:
                    this.tx -= 1;
                    break;
                case Keys.S:
                    this.ty -= 1;
                    break;
                case Keys.D:
                    this.tx += 1;
                    break;
                case Keys.W:
                    this.ty += 1;
                    break;
                case Keys.T:
                    this.tz += 1;
                    break;
                case Keys.B:
                    this.tz -= 1;
                    break;
                case Keys.Q:
                    this.rz -= 10;
                    break;
                case Keys.E:
                    this.rz += 10;
                    break;
                case Keys.Z:
                    this.rx -= 10;
                    break;
                case Keys.C:
                    this.rx += 10;
                    break;
                case Keys.R:
                    this.ry -= 10;
                    break;
                case Keys.V:
                    this.ry += 10;
                    break;
                case Keys.D0:
                    if (GL.IsEnabled(EnableCap.Light0))
                        GL.Disable(EnableCap.Light0);
                    else GL.Enable(EnableCap.Light0);
                    break;
                case Keys.D1:
                    if (GL.IsEnabled(EnableCap.Light1))
                        GL.Disable(EnableCap.Light1);
                    else GL.Enable(EnableCap.Light1);
                    break;
                case Keys.D2:
                    if (GL.IsEnabled(EnableCap.Light2))
                        GL.Disable(EnableCap.Light2);
                    else GL.Enable(EnableCap.Light2);
                    break;
                case Keys.D3:
                    if (GL.IsEnabled(EnableCap.Light3))
                        GL.Disable(EnableCap.Light3);
                    else GL.Enable(EnableCap.Light3);
                    break;
                case Keys.D4:
                    if (GL.IsEnabled(EnableCap.Light4))
                        GL.Disable(EnableCap.Light4);
                    else GL.Enable(EnableCap.Light4);
                    break;
                case Keys.D5:
                    if (GL.IsEnabled(EnableCap.Light5))
                        GL.Disable(EnableCap.Light5);
                    else GL.Enable(EnableCap.Light5);
                    break;
                case Keys.D6:
                    if (GL.IsEnabled(EnableCap.Light6))
                        GL.Disable(EnableCap.Light6);
                    else GL.Enable(EnableCap.Light6);
                    break;
                case Keys.Add:
                    Zoom(2);
                    Board.Invalidate();
                    break;
                case Keys.Subtract:
                    Zoom(-3);
                    Board.Invalidate();
                    break;
                default:
                    break;
            }
            Board.Invalidate();
        }
        private void Board_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.lMouseLoc = e.Location;
            }
        }
        private void Board_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.ry += (e.Location.X - lMouseLoc.X);
                this.rx += (e.Location.Y - lMouseLoc.Y);
                Board.Invalidate();
            }
        }
        private void Zoom(double factor)
        {
            tz += factor;
            //scale += factor;
            Board.Invalidate();
        }
        private void Initialize()//Just for initialize the Form Class
        {
            this.tx = 0;
            this.ty = 0;
            this.tz = -45;
            this.rx = 0;
            this.ry = 0;
            this.rz = 0;
            this.scale = 1;
        }
        private void SetupViewPort()//Prepare the board to an apropiate image projection
        {
            int w = Board.Width;
            int h = Board.Height;
            GL.Viewport(0, 0, w, h);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Matrix4 matrix = Matrix4.Perspective(45.0f, w / h, 1.0f, 100.0f);
            GL.LoadMatrix(ref matrix);
            //GL.Ortho(-w, w, -h, h, -Math.Max(w,h), Math.Max(w,h)); // Bottom-left corner pixel has coordinate (0, 0)
            GL.MatrixMode(MatrixMode.Modelview);
            GL.Enable(EnableCap.DepthTest);

        }
        private void SetupLigthing()//Creates the lights
        {
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.Light1);
            GL.Enable(EnableCap.Light2);
            GL.Enable(EnableCap.Light3);
            GL.Enable(EnableCap.Light4);
            GL.Enable(EnableCap.Light5);
            GL.Enable(EnableCap.Light6);
            GL.Enable(EnableCap.Light7);
            //float[] diffuse = {1,1,1 }; 
            //GL.Light(LightName.Light0, LightParameter.Diffuse,diffuse);
        }
        private void Settings()//Prepare the board to just paint the triangles and lines
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Translate(tx, ty, tz);
            GL.Rotate(rx, 1, 0, 0);
            GL.Rotate(ry, 0, 1, 0);
            GL.Rotate(rz, 0, 0, 1);
            GL.Scale(scale, scale, scale);
        }
        private void DrawLines()
        {
            GL.LineWidth(2.0f);
            GL.Color3(Color.Red);
            GL.Begin(BeginMode.Lines);
            Vertex u, v;

            for (int i = 0; i < mesh.CountEdge; i++)
            {
                u = mesh.VertexesAt(mesh.EdgesAt(i).frm);
                v = mesh.VertexesAt(mesh.EdgesAt(i).dst);
                GL.Vertex3(u.X, u.Y, u.Z);
                GL.Vertex3(v.X, v.Y, v.Z);
            }
            GL.End();
        }
        private void DrawTriangles()
        {
            GL.Begin(BeginMode.Triangles);
            for (int i = 0; i < mesh.CountFace; i++)
            {
                Face f = mesh.FacesAt(i);
                GL.Color3(colors[clusters[i]]);//20
                Vector normal = Vector.normal(new Vector(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z), new Vector(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z), new Vector(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z));
                GL.Normal3(normal.elements[0], normal.elements[1], normal.elements[2]);
                GL.Vertex3(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z);
                GL.Vertex3(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z);
                GL.Vertex3(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z);
            }
            GL.End();
        }

    }
}
