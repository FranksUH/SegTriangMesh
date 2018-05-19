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
        private enum Status { selected, segmented, none };
        private Status act_state;
        private double[] r = {1,0,0,1,1,0,0.5 };
        private double[] g = {0,1,0,1,0,1,0.5 };
        private double[] b = {0,0,1,0,1,1,0.5 };

        public Form1()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            Board.InitializeContexts();
            mesh = new Mesh();

            camPos = new Vertex(-4, 1, 2);
            center = new Vertex(0, 0, 0);
            camUp = new Vertex(0, 1, 0);
            act_state = Status.none;
        }

        private void Settings()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glLoadIdentity();
            Glu.gluPerspective(45, Board.Width / Board.Height, 1, 1000);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            //Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glEnable(Gl.GL_DEPTH_TEST);//para que no sean transparentes las caras y sin el _TEST son transparentes
            Gl.glViewport(0, 0, Board.Width, Board.Height);

            Glu.gluLookAt(camPos.X, camPos.Y, camPos.Z,
                            center.X, center.Y, center.Z,
                            camUp.X, camUp.Y, camUp.Z);
            //InitLight();
            Gl.glShadeModel(Gl.GL_SMOOTH);
            Gl.glLightModeli(Gl.GL_LIGHT_MODEL_TWO_SIDE, 1);
            Gl.glEnable(Gl.GL_LIGHT0);
            //Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
        }

        private void PaintGL(object sender, PaintEventArgs e)
        {
            Settings();
            if (chBoxShowAxes.Checked)
            {
                DrawCoords();
            }
            //InitLight();
            DrawTriangles();
            DrawLines();
        }

        private void InitLight()
        {
            Gl.glPushMatrix();
            Gl.glLoadIdentity();
            // light from a light source
            #region Creating lights
            //float[] diffuseLight = { (float)0.4, (float)0.4, (float)0.4, (float)1.0 };
            //// light from no particulat light source
            //float[] ambientLight = { (float)0.1, (float)0.1, (float)0.1, (float)1.0 };
            //// light positions for 4 lights
            //float[] aux1 = { (float)1.0, (float)1.0, (float)0.0, (float)0.0 };
            //float[] aux2 = { (float)-1.0, (float)-1.0, (float)0.0, (float)0.0 };
            //float[] aux3 = { (float)-0.1, (float)-0.1, (float)1.0, (float)0.0 };
            //float[] aux4 = { (float)0.1, (float)0.1, (float)-1.0, (float)0.0 };

            //Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, ambientLight);

            //Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, diffuseLight);
            //Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, aux1);
            //Gl.glEnable(Gl.GL_LIGHT0);

            //Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_DIFFUSE, diffuseLight);
            //Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION, aux2);
            //Gl.glEnable(Gl.GL_LIGHT1);

            //Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_DIFFUSE, diffuseLight);
            //Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_POSITION, aux3);
            //Gl.glEnable(Gl.GL_LIGHT2);

            //Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_DIFFUSE, diffuseLight);
            //Gl.glLightfv(Gl.GL_LIGHT3, Gl.GL_POSITION, aux4);
            //Gl.glEnable(Gl.GL_LIGHT3);
            #endregion
            float[] lightPos = { 1, 1, 1, 0 };
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, lightPos);
            Gl.glPopMatrix();
            Gl.glEnable(Gl.GL_LIGHTING);
        }

        private void DrawLines()
        {
            Gl.glLineWidth(2);
            Gl.glColor3d(0.3,0.3,0.3);
            Gl.glBegin(Gl.GL_LINES);

            Vertex u, v;

            for (int i = 0; i < mesh.CountEdge; i++)
            {
                u = mesh.VertexesAt(mesh.EdgesAt(i).frm);
                v = mesh.VertexesAt(mesh.EdgesAt(i).dst);
                Gl.glVertex3d(u.X, u.Y, u.Z);
                Gl.glVertex3d(v.X, v.Y, v.Z);
            }

            Gl.glEnd();           
        }

        private void DrawTriangles()
        {
            Gl.glBegin(Gl.GL_TRIANGLES);
            Face f;
            for (int i = 0; i < mesh.CountFace; i++)
            {
                f = mesh.FacesAt(i);
                if (this.act_state == Status.selected)//cuando crezca hacer un switch
                {
                    if(i==2)
                    {
                        Gl.glColor3d(1, 0, 0);
                        Gl.glVertex3d(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z);
                        Gl.glColor3d(0, 1, 0);
                        Gl.glVertex3d(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z);
                        Gl.glColor3d(0, 0, 1);
                        Gl.glVertex3d(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z);
                    }
                    else
                    {
                        double dist = mesh.dual_graph_cost[2][ i];
                        if(dist>1)
                            Gl.glColor3d(1, 0, 0);
                        else Gl.glColor3d(dist, 1-dist, 0);
                        Gl.glVertex3d(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z);
                        Gl.glVertex3d(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z);
                        Gl.glVertex3d(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z);
                    }
                }
                else
                {
                    if (this.act_state == Status.segmented)
                    {
                        Gl.glColor3d(r[mesh.cluster[i]], g[mesh.cluster[i]], b[mesh.cluster[i]]);
                        Gl.glVertex3d(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z);
                        Gl.glVertex3d(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z);
                        Gl.glVertex3d(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z);
                    }
                    else
                    {
                        Gl.glColor3d(0, 200, 230);
                        Gl.glVertex3d(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z);
                        Gl.glVertex3d(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z);
                        Gl.glVertex3d(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z);
                    }
                }                
            }
            Gl.glEnd();
            Gl.glDisable(Gl.GL_LIGHTING);
        }

        private void DrawCoords()
        {
            Gl.glLineWidth(1);
            Gl.glColor3f(1, 0, 0);
            Gl.glBegin(Gl.GL_LINES);

            Gl.glVertex3d(-100, 0, 0);
            Gl.glVertex3d(100, 0, 0);

            Gl.glVertex3d(0, -100, 0);
            Gl.glVertex3d(0, 100, 0);

            Gl.glVertex3d(0, 0,-10);
            Gl.glVertex3d(0, 0, 10);

            //for (int x  = -100; x < 100; x++)
            //{
            //    Gl.glVertex3d(x, 0, -100);
            //    Gl.glVertex3d(x, 0, 100);               
            //}

            //for (int z = -100; z < 100; z++)
            //{                
            //    Gl.glVertex3d(-100, 0, z);
            //    Gl.glVertex3d(100, 0, z);
            //}

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
            //camPos = (camPos - center) * factor + center;
            camPos *= factor;
            Board.Invalidate();
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

        private void Exit(object sender, EventArgs e)
        {
            this.Close();
        }

        #region Botones de movimiento manual
        private void UpdateCamPosX(object sender, EventArgs e)
        {
            camPos.X = (double)camPosX.Value;
            Board.Invalidate();
        }

        private void UpdateCamPosY(object sender, EventArgs e)
        {
            camPos.Y = (double)camPosY.Value;
            Board.Invalidate();
        }

        private void UpdateCamPosZ(object sender, EventArgs e)
        {
            camPos.Z = (double)camPosZ.Value;
            Board.Invalidate();
        }

        private void UpdateCenterX(object sender, EventArgs e)
        {
            center.X = (double)centerX.Value;
            Board.Invalidate();
        }

        private void UpdateCenterY(object sender, EventArgs e)
        {
            center.Z = (double)centerZ.Value;
            Board.Invalidate();
        }

        private void UpdateCenterZ(object sender, EventArgs e)
        {
            center.Z = (double)centerZ.Value;
            Board.Invalidate();
        }

        private void UpdateCamUpX(object sender, EventArgs e)
        {
            camUp.X = (double)camUpX.Value;
            Board.Invalidate();
        }

        private void UpdateCamUpY(object sender, EventArgs e)
        {
            camUp.Y = (double)camUpY.Value;
            Board.Invalidate();
        }

        private void UpdateCamUpZ(object sender, EventArgs e)
        {
            camUp.Z = (double)camUpZ.Value;
            Board.Invalidate();
        }
        #endregion

        private void Board_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D)
            {
                if (camPos.X == -4 && camPos.Z < 4)
                    camPos.Z += 0.5;
                else
                {
                    if (camPos.X < 4 && camPos.Z == 4)
                        camPos.X += 0.5;
                    else
                    {
                        if (camPos.X == 4 && camPos.Z > -4)
                            camPos.Z -= 0.5;
                        else
                        {
                            if (camPos.X == 4  && camPos.Z == -4)
                                camPos.Z += 0.5;
                            else
                                camPos.Z += 0.5;
                        }
                    }
                }
                Board.Invalidate();
            }
            if (e.KeyCode == Keys.A)
            {
                if (camPos.X == -4 && camPos.Z < 4)
                    camPos.Z -= 0.5;
                else
                {
                    if (camPos.X < 4 && camPos.Z == 4)
                        camPos.X -= 0.5;
                    else
                    {
                        if (camPos.X == 4 && camPos.Z > -4)
                            camPos.Z += 0.5;
                        else
                        {
                            if (camPos.X == 4 && camPos.Z == -4)
                                camPos.Z -= 0.5;
                            else
                                camPos.Z -= 0.5;
                        }
                    }
                }
                Board.Invalidate();
            }
            if (e.KeyCode == Keys.W)
            {
                camPos.Y+= 0.5;
                Board.Invalidate();
            }
            if (e.KeyCode == Keys.S)
            {
                camPos.Y -= 0.5;
                Board.Invalidate();
            }
            camPosX.Value = (decimal)camPos.X;
            camPosY.Value = (decimal)camPos.Y;
            camPosZ.Value = (decimal)camPos.Z;
        }

        private void chBoxShowAxes_CheckedChanged(object sender, EventArgs e)
        {
            Board.Invalidate();
        }

        private void btn_segment_Click(object sender, EventArgs e)
        {
            SegmentationSettings s = new SegmentationSettings();
            s.ShowDialog();
            if (s.DialogResult == DialogResult.OK)
            {
                panel1.Visible = false;
                panel2.Visible = false;
                panel3.Visible = false;
                lblFar.Visible = false;
                lblMid.Visible = false;
                lblNear.Visible = false;
                this.mesh.Segment(s.numGroups,s.numIter);
                this.act_state = Status.segmented;                
                //mandar a segmentar
                Board.Invalidate();
            }
        }

        private void Board_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                mesh.build_dual_graph();
                this.act_state = Status.selected;
                panel1.BackColor = Color.FromArgb(255, 0, 0);
                panel2.BackColor = Color.FromArgb((int)((double)255*0.5), (int)((double)255 * 0.5), 0);
                panel3.BackColor = Color.FromArgb(0, 255, 0);
                lblFar.Text = "1";
                lblMid.Text = "0.5";
                lblNear.Text = "0";
                panel1.Visible = true;
                panel2.Visible = true;
                panel3.Visible = true;
                lblFar.Visible = true;
                lblMid.Visible = true;
                lblNear.Visible = true;
                Board.Invalidate();
            }            
        }

        private void Board_Scroll(object sender, ScrollEventArgs e)
        {
            int moved = e.OldValue - e.NewValue;
            if (moved > 0)
            {
                Zoom(1.005*moved);
                Board.Invalidate();
            }
            else
            {
                Zoom(0.95*moved);
                Board.Invalidate();
            }
        }
        private void ZoomOut(object sender, EventArgs e)
        {
            Zoom(1.1);
            Board.Invalidate();
        }

        private void ZoomIn(object sender, EventArgs e)
        {
            Zoom(0.90);
            Board.Invalidate();
        }

        private void scrollZoom_Scroll(object sender, ScrollEventArgs e)
        {
            int moved = e.OldValue-e.NewValue;
            if (moved > 0)
            {
                Zoom(1.005);
                Board.Invalidate();
            }
            else
            {
                Zoom(0.95);
                Board.Invalidate();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
