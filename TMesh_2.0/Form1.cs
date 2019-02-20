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
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace TMesh_2._0
{
    public partial class Form1 : Form
    {
        Mesh mesh;
        private enum Status { selected, segmented, none, Visible };
        private bool showBB;
        private Status act_state;
        private double[] r = {1,0,0,1,1,0,0.5,0.1,0.8,0.9 };//10
        private double[] g = {0,1,0,1,0,1,0.5,0.2,0.6,0.4 };
        private double[] b = {0,0,1,0,1,1,0.5,0.3,0.4,0.1 };
        private Color[] colors = { Color.Blue, Color.Red,Color.Green,Color.Gray,Color.Gold,Color.Violet,Color.Cyan, Color.Orange, Color.Lime,Color.Maroon,Color.ForestGreen,Color.DarkCyan};
        private double tx, ty, tz, rx, ry, rz, scale;
        private Point lMouseLoc;
        private List<Tuple<double[], double[]>> bbx;
        private List<Face> visibles;
        private Matrix4 matr;

        public Form1()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()//Just for initialize the Form Class
        {
            mesh = new Mesh();
            this.distanceSelector.Text = mesh.Distance.ToString();
            act_state = Status.none;
            this.tx = 0;
            this.ty = 0;
            this.tz = -10;
            this.rx = 0;
            this.ry = 0;
            this.rz = 0;
            this.scale = 1;
            this.showBB = false;
        }
        private void SetupViewPort()//Prepare the board to an apropiate image projection
        {
            int w = Board.Width;
            int h = Board.Height;
            GL.Viewport(0, 0, w, h);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Matrix4 matrix = Matrix4.Perspective(45.0f, w / h, 1.0f, 100.0f);
            this.matr = matrix;
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
            Face f;
            if (this.chkFarest.Checked && this.act_state == Status.segmented)
            {
                for (int i = 0; i < mesh.partitions; i++)
                {
                    f = mesh.FacesAt(mesh.nextIndex[i]);
                    GL.Color3(Color.White);
                    Vector normal = Vector.normal(new Vector(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z), new Vector(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z), new Vector(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z));
                    GL.Normal3(normal.elements[0],normal.elements[1],normal.elements[2]);
                    GL.Vertex3(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z);
                    GL.Vertex3(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z);
                    GL.Vertex3(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z);
                }
            }
            if (this.act_state == Status.Visible)
            {
                    f = mesh.FacesAt(1);
                    Vector normal = Vector.normal(new Vector(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z), new Vector(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z), new Vector(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z));
                    GL.Normal3(normal.elements[0], normal.elements[1], normal.elements[2]);
                    GL.Color3(Color.Red);
                    GL.Vertex3(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z);
                    GL.Color3(Color.Green);
                    GL.Vertex3(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z);
                    GL.Color3(Color.Blue);
                    GL.Vertex3(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z);

                foreach (var item in visibles)
                {
                    f = item;
                    GL.Color3(Color.Gold);
                    normal = Vector.normal(new Vector(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z), new Vector(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z), new Vector(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z));
                    GL.Normal3(normal.elements[0], normal.elements[1], normal.elements[2]);
                    GL.Vertex3(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z);
                    GL.Vertex3(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z);
                    GL.Vertex3(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z);
                }
            }
            else
            {
                for (int i = 0; i < mesh.CountFace; i++)
                {
                    f = mesh.FacesAt(i);
                    if (this.act_state == Status.selected)//cuando crezca hacer un switch
                    {
                        if (i == 2)
                        {
                            Vector normal = Vector.normal(new Vector(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z), new Vector(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z), new Vector(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z));
                            GL.Normal3(normal.elements[0], normal.elements[1], normal.elements[2]);
                            GL.Color3(Color.Red);
                            GL.Vertex3(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z);
                            GL.Color3(Color.Green);
                            GL.Vertex3(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z);
                            GL.Color3(Color.Blue);
                            GL.Vertex3(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z);
                        }
                        else
                        {
                            double dist = mesh.distancesMatrix[2][i];
                            if (dist > 1)
                                GL.Color3(Color.Red);
                            else GL.Color3(dist, 1 - dist, 0);
                            Vector normal = Vector.normal(new Vector(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z), new Vector(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z), new Vector(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z));
                            GL.Normal3(normal.elements[0], normal.elements[1], normal.elements[2]);
                            GL.Vertex3(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z);
                            GL.Vertex3(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z);
                            GL.Vertex3(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z);
                        }
                    }
                    else
                    {
                        if (this.act_state == Status.segmented)
                        {
                            //GL.Color3(r[mesh.cluster[i]], g[mesh.cluster[i]], b[mesh.cluster[i]]);
                            GL.Color3(colors[mesh.cluster[i]]);//20
                            Vector normal = Vector.normal(new Vector(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z), new Vector(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z), new Vector(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z));
                            GL.Normal3(normal.elements[0], normal.elements[1], normal.elements[2]);
                            GL.Vertex3(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z);
                            GL.Vertex3(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z);
                            GL.Vertex3(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z);
                        }
                        else
                        {
                            GL.Color3(Color.LightSeaGreen);
                            Vector normal = Vector.normal(new Vector(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z), new Vector(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z), new Vector(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z));
                            GL.Normal3(normal.elements[0], normal.elements[1], normal.elements[2]);
                            GL.Vertex3(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z);
                            GL.Vertex3(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z);
                            GL.Vertex3(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z);
                        }
                    }
                }
            }
            GL.End();
        }
        private void DrawCoords()
        {
            GL.LineWidth(3.0f);
            GL.Color3(Color.Red);
            GL.Begin(BeginMode.Lines);

            GL.Vertex3(-100, 0, 0);
            GL.Vertex3(100, 0, 0);

            GL.Vertex3(0, -100, 0);
            GL.Vertex3(0, 100, 0);

            GL.Vertex3(0, 0,-10);
            GL.Vertex3(0, 0, 10);

            GL.End();

            DrawNormal();
        }
        private void DrawNormal()
        {
            GL.LineWidth(3.0f);
            GL.Color3(Color.Red);
            GL.Begin(BeginMode.Lines);
            for (int i = 0; i < mesh.CountFace; i++)
            {
                Vector baricenter = mesh.Baricenter(i);
                Vector normal = Vector.normal(new Vector(mesh.VertexesAt(mesh.FacesAt(i).i).X, mesh.VertexesAt(mesh.FacesAt(i).i).Y, mesh.VertexesAt(mesh.FacesAt(i).i).Z), new Vector(mesh.VertexesAt(mesh.FacesAt(i).j).X, mesh.VertexesAt(mesh.FacesAt(i).j).Y, mesh.VertexesAt(mesh.FacesAt(i).j).Z), new Vector(mesh.VertexesAt(mesh.FacesAt(i).k).X, mesh.VertexesAt(mesh.FacesAt(i).k).Y, mesh.VertexesAt(mesh.FacesAt(i).k).Z));
                GL.Vertex3(baricenter.elements[0],baricenter.elements[1],baricenter.elements[2]);
                GL.Vertex3(baricenter.elements[0]+normal.elements[0], baricenter.elements[1] + normal.elements[1], baricenter.elements[2]+normal.elements[2]);
            }
            GL.End();
        }

        private void OpenOFF(object sender, EventArgs e)
        {
            Initialize();
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "|*.off";

            if (file.ShowDialog() == DialogResult.OK)
            {
                mesh.LoadOFF(file.FileName);
                lblInfo.Text = "Vertexes: " + mesh.CountVertex.ToString() + "\n" + "Faces: " + mesh.CountFace.ToString();
                Board.Invalidate();
            }
        }
        private int[] OpenSEG(string fileName,int n)
        {
            int[] clusters = new int[n];
            StreamReader reader = new StreamReader(fileName);
            for (int i = 0; i < n; i++)
                clusters[i] = int.Parse(reader.ReadLine());
            return clusters;
        }
        private void Zoom(double factor)
        {
            tz += factor;
            //scale += factor;
            Board.Invalidate();
        }
        private static double NormalizeAngle(double angle)
        {
            while(angle < 0) angle += 360;
            while(angle >= 360) angle -= 360;
            return angle;
        }             
        private void Exit(object sender, EventArgs e)
        {
            this.Close();
        }       

        private void chBoxShowAxes_CheckedChanged(object sender, EventArgs e)
        {
            Board.Invalidate();
        }
        private void btn_segment_Click(object sender, EventArgs e)
        {
            SegmentationSettings s = new SegmentationSettings();
            CombinationMenu c = new CombinationMenu();

            Menu:
            s.ShowDialog();
            if (s.DialogResult == DialogResult.OK)
            {
                if (this.distanceSelector.SelectedIndex == 4)
                {
                    c.ShowDialog();
                    if (c.DialogResult == DialogResult.Cancel || (c.angular==0 && c.geodesic==0))
                        goto Menu;                    
                }
                panel1.Visible = false;
                panel2.Visible = false;
                panel3.Visible = false;
                lblFar.Visible = false;
                lblMid.Visible = false;
                lblNear.Visible = false;
                mesh.Segment(s.numGroups, s.numIter, c.angular, c.geodesic, s.K);
                //Thread t2 = new Thread(new ThreadStart(() => mesh.Segment(s.numGroups, s.numIter, c.angular, c.geodesic, s.K)));
                //t2.Start();
                //updateBar();
                //t2.Join();
                this.act_state = Status.segmented;                
                //mandar a segmentar
                Board.Invalidate();
                this.lblK.Visible = true;
                this.lblK.Text = "K: " + mesh.K.ToString();
            }
        }
        //private void segment(int groups,int iter,double angular,double geodesic,int K)
        //{
        //    mesh.Segment(groups, iter, angular, geodesic, K);
        //}
        private void updateBar()
        {
            int readed = 0;
            this.segPBar.Visible = true;
            this.lblSegStatus.Visible = true;
            while (readed <= 99)
            {
                try
                {
                    readed = int.Parse(FilesChat.ReadLine("build"));
                    if (readed != -1)
                    {
                        this.segPBar.Value = readed;
                        this.lblSegStatus.Text = readed.ToString();
                    }
                }
                catch
                { }
            }
            this.segPBar.Visible = false;
            this.lblSegStatus.Visible = false;
        }

        private void Board_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                mesh.build_dual_graph(Math.Max((int)((double)mesh.CountFace),3),true);
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
                Zoom(1*moved);
                Board.Invalidate();
            }
            else
            {
                Zoom(-1*moved);
                Board.Invalidate();
            }
        }
        private void ZoomOut(object sender, EventArgs e)
        {
            Zoom(-3);
            //Zoom(-0.2);
            Board.Invalidate();
        }
        private void ZoomIn(object sender, EventArgs e)
        {
            Zoom(2);
            //Zoom(1);
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
        private void Board_Paint(object sender, PaintEventArgs e)//!!!!!!PAINTER
        {
            Settings();
            if (chBoxShowAxes.Checked)
            {
                DrawCoords();
            }
            DrawTriangles();
            if(chkLines.Checked)
                DrawLines();            
            if (showBB)
            {
                GL.Begin(BeginMode.Lines);
                GL.Color3(Color.Azure);
                for (int i = 0; i < bbx.Count; i++)
                {
                    GL.Vertex3(bbx[i].Item1[0], bbx[i].Item1[1], bbx[i].Item1[2]);
                    GL.Vertex3(bbx[i].Item2[0], bbx[i].Item2[1], bbx[i].Item2[2]);
                }
                GL.End();
            }
            if (aabbTest.Checked)
            {
                GL.Begin(BeginMode.Lines);
                GL.Color3(Color.Green);
                double epsilon = 0.1;
                for (int i = 0; i < mesh.aabb.tests.Count; i++)
                {
                    GL.Vertex3(mesh.aabb.tests[i][0]-epsilon, mesh.aabb.tests[i][1], mesh.aabb.tests[i][2]);
                    GL.Vertex3(mesh.aabb.tests[i][0]+epsilon, mesh.aabb.tests[i][1], mesh.aabb.tests[i][2]);

                    GL.Vertex3(mesh.aabb.tests[i][0], mesh.aabb.tests[i][1]-epsilon, mesh.aabb.tests[i][2]);
                    GL.Vertex3(mesh.aabb.tests[i][0], mesh.aabb.tests[i][1]+epsilon, mesh.aabb.tests[i][2]);

                    GL.Vertex3(mesh.aabb.tests[i][0], mesh.aabb.tests[i][1], mesh.aabb.tests[i][2]-epsilon);
                    GL.Vertex3(mesh.aabb.tests[i][0], mesh.aabb.tests[i][1], mesh.aabb.tests[i][2]+epsilon);
                }
                GL.End();
            }
            // DrawNormal();
            Board.SwapBuffers();
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
            if (e.Button == MouseButtons.Right)
            {
                mesh.build_dual_graph(mesh.CountFace,true);
                this.act_state = Status.selected;
                panel1.BackColor = Color.FromArgb(255, 0, 0);
                panel2.BackColor = Color.FromArgb((int)((double)255 * 0.5), (int)((double)255 * 0.5), 0);
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
        private void Board_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.ry += (e.Location.X - lMouseLoc.X);
                this.rx += (e.Location.Y - lMouseLoc.Y);
                Board.Invalidate();
                Cursor = Cursors.Default;
            }
        }

        private void visibilityBtn_Click(object sender, EventArgs e)
        {
            this.act_state = Status.Visible;
            this.visibles = mesh.TestVisibility(1,30);
            this.Board.Invalidate();
        }
        private void btnBbox_Click(object sender, EventArgs e)
        {
            if (this.showBB)
                this.showBB = false;
            else this.showBB = true;
            this.bbx = mesh.Getbbx();
            this.Board.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int width = Board.Width;
                int height = Board.Height;
                byte[] rgbData = new byte[width * height * 3];

                Bitmap bmp = new Bitmap(width, height);
                BitmapData data = bmp.LockBits(Board.ClientRectangle, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                GL.ReadPixels(0, 0, width, height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
                bmp.UnlockBits(data);
                saveFileDialog1.Title = "Choose a folder to save the screen";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (saveFileDialog1.FileName != "")
                        bmp.Save(saveFileDialog1.FileName, ImageFormat.Bmp);
                }
            }
            catch
            {
                throw new Exception("There was an error saving file");
            }
        }

        private void btnCmp_Click(object sender, EventArgs e)
        {
            compare();
            //Thread thread = new Thread(compare);
            //thread.Start();
            //thread.Join();
            //if (!thread.IsAlive)
            //    thread.Interrupt();           

            //new Thread(() =>
            //{
                //OpenFileDialog file = new OpenFileDialog();
                //file.Filter = "|*.seg";
                //this.lblInfo.Text = "ThrED";
                //if (file.ShowDialog() == DialogResult.OK)
                //{
                //    int[] other = OpenSEG(file.FileName, mesh.CountFace);
                //    this.lblSim.Text = mesh.compareWith(other).ToString();
                //    this.lblSim.Visible = true;
                //    compareWith c = new compareWith(other, mesh);
                //    c.ShowDialog();
                //}
                //this.lblSim.Visible = false;
            //}).Start();

        }
        private void compare()
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "|*.seg";

            if (file.ShowDialog() == DialogResult.OK)
            {
                int[] other = OpenSEG(file.FileName, mesh.CountFace);
                compareWith c = new compareWith(other, mesh);
                MessageBox.Show("Resultado usando Indice de Rand: " + mesh.compareWith(other).ToString());
                Thread aux = new Thread(() => c.ShowDialog());
                aux.Start();                
                //this.lblSim.Text = mesh.compareWith(other).ToString();
                //this.lblSim.Visible = true;
                aux.Join();
            }
            this.lblSim.Visible = false;
        }

        private void Board_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.lMouseLoc != null && e.Button == MouseButtons.Left)
            {
                Cursor = Cursors.NoMove2D;
                this.ry += (e.Location.X - lMouseLoc.X);
                this.rx += (e.Location.Y - lMouseLoc.Y);
                Board.Invalidate();
            }
            this.lMouseLoc = e.Location;
        }

        private void aabbTest_CheckedChanged(object sender, EventArgs e)
        {
            Board.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (mesh.cluster != null)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Segmentation file|*.seg|Binary file|*.bin";
                saveFileDialog1.Title = "Choose a folder to save the segmentation";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (saveFileDialog1.FileName != "")
                    {
                        if (saveFileDialog1.FileName.Contains(".seg"))
                        {
                            StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
                            for (int i = 0; i < mesh.cluster.Length; i++)
                                sw.WriteLine(mesh.cluster[i]);
                            sw.Close();
                        }
                        else
                            mesh.Save(saveFileDialog1.FileName);
                    }
                }
                
            }
        }

        private void cargarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Segmentation file|*.seg|Binary file|*.bin";

            if (file.ShowDialog() == DialogResult.OK)
            {
                if (file.FileName.Contains(".bin"))
                {
                    IFormatter format = new BinaryFormatter();
                    Stream s = new FileStream(file.FileName, FileMode.Open);
                    this.mesh = (Mesh)format.Deserialize(s);
                    s.Close();
                    if (mesh.cluster != null)
                        this.act_state = Status.segmented;
                    Board.Invalidate();
                }
                else
                {
                    StreamReader sr = new StreamReader(file.FileName);
                    mesh.cluster = new int[mesh.CountFace];
                    for (int i = 0; i < mesh.cluster.Length; i++)
                    {
                        try
                        {
                            mesh.cluster[i] = int.Parse(sr.ReadLine());
                        }
                        catch
                        {
                            throw new Exception("The file is corrupted or not represent the loaded OFF file");
                        }
                    }
                    sr.Close();
                    this.act_state = Status.segmented;
                    Board.Invalidate();
                }
            }
        }

        private void chkLines_CheckedChanged(object sender, EventArgs e)
        {
            Board.Invalidate();
        }

        private void distanceSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (distanceSelector.SelectedIndex)
            {
                case 0:
                    distanceSelector.Text = "Angular";
                    this.mesh.Distance = Mesh.DistanceType.Angular;
                    break;
                case 1:
                    distanceSelector.Text = "Geodesic";
                    this.mesh.Distance = Mesh.DistanceType.Geodesic;
                    break;
                case 2:
                    distanceSelector.Text = "Volumetric";
                    this.mesh.Distance = Mesh.DistanceType.Volumetric;
                    break;
                case 3:
                    distanceSelector.Text = "Producto Angular-Geodesica";
                    this.mesh.Distance = Mesh.DistanceType.Combined1;
                    break;
                case 4:
                    distanceSelector.Text = "Combinacion Angular-Geodesica-Volumetrica";
                    this.mesh.Distance = Mesh.DistanceType.Combined2;
                    break;
                default:
                    break;
            }
        }
        private void chkFarest_CheckedChanged(object sender, EventArgs e)
        {
            Board.Invalidate();
        }
        private void btnCeldas_Click(object sender, EventArgs e)
        {
            SegmentationSettings s = new SegmentationSettings();
            s.ShowDialog();
            if (s.DialogResult == DialogResult.OK)
            {
                mesh.Vorinoy(s.numGroups);
                this.act_state = Status.segmented;
                Board.Invalidate();
            }
        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TestMenu tm = new TestMenu();
            TestResults tr = new TestResults();
            tm.ShowDialog();
            if(tm.DialogResult == DialogResult.OK)
            {
                List<Vertex> verts = new List<Vertex>();
                verts.Add(new Vertex(tm.values[0],tm.values[1],tm.values[2]));
                verts.Add(new Vertex(tm.values[3], tm.values[4], tm.values[5]));
                verts.Add(new Vertex(tm.values[6], tm.values[7], tm.values[8]));
                verts.Add(new Vertex(tm.values[9], tm.values[10], tm.values[11]));
                Face t1 = new Face(0,1,2);
                Face t2 = new Face(0,3,1);
                double result = this.mesh.testGeodesic(t1,t2,verts);
                MessageBox.Show(result.ToString());
            }
        }
        private void chbJustDistances_CheckedChanged(object sender, EventArgs e)
        {
            this.mesh.GetAffinity = !chbJustDistances.Checked;
        }
    }
}
