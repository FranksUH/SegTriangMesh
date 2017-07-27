using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.Platform;


namespace TMesh_2._0
{
    public partial class Form1 : Form
    {
        Mesh mesh;
        Vertex camPos;
        Vertex center;
        Vertex camUp;

        public Form1()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            Board.InitializeContexts();
            mesh = new Mesh();

            camPos = new Vertex(0, 0, 0.5);
            center = new Vertex(0, 0, 0);
            camUp = new Vertex(0, 1, 0);
        }

        private void Settings()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
            Gl.glViewport(0, 0, Board.Width, Board.Height);
            Glu.gluLookAt(camPos.X, camPos.Y, camPos.Z,
                            center.X, center.Y, center.Z,
                            camUp.X, camUp.Y, camUp.Z);
            
        }

        private void PaintGL(object sender, PaintEventArgs e)
        {
            Settings();
            DrawLines();

        }

        private void DrawLines()
        {
            Gl.glLineWidth(2);
            Gl.glColor3f(1, 1, 1);
            Gl.glBegin(Gl.GL_LINES);

            Vertex u, v;

            for (int i = 0; i < mesh.CountEdge; i++)
            {
                u = mesh.VertexesAt(mesh.EdgesAt(i).dst);
                v = mesh.VertexesAt(mesh.EdgesAt(i).mate);

                Gl.glVertex3d(u.X, u.Y, u.Z);
                Gl.glVertex3d(v.X, v.Y, v.Z);
            }

            Gl.glEnd();
        }

        private void OpenOFF(object sender, EventArgs e)
        {
            Initialize();
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "|*.off";


            if (file.ShowDialog() == DialogResult.OK)
            {
                mesh.LoadOFF(file.FileName);
                Board.Invalidate();
            }

            
            

        }

        private void Zoom(double factor)
        {
            camPos = (camPos - center) * factor + center;
        }

        private static double NormalizeAngle(double angle)
        {
            while(angle < 0) angle += 360;
            while(angle >= 360) angle -= 360;

            return angle;
        }
        
        private void XRotation(double angle)
        {
            angle = NormalizeAngle(angle);
        }

        private void YRotation(double angle)
        {
            angle = NormalizeAngle(angle);
        }

        private void ZRotation(double angle)
        {
            angle = NormalizeAngle(angle);
        }

        private void wheelEvent(object sender, ScrollEventArgs e)
        {

        }

        private void Exit(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Board_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

    }
}
