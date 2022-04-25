using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace ImageApp
{
    public partial class Form1 : Form
    {
        SqlConnection conn = new SqlConnection("Data Source=(local)\\SQLEXPRESS;Initial Catalog=ImagesAWS;Integrated Security=True");
        SqlCommand command;
        string imgLoc = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif|All Files (*.*)|*.*";
                dlg.Title = "Select Image";

                if(dlg.ShowDialog() == DialogResult.OK)
                {
                    imgLoc = dlg.FileName.ToString();
                    pictureBox.ImageLocation = imgLoc;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] img = null;
                FileStream fs = new FileStream(imgLoc, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                img = br.ReadBytes((int)fs.Length);
                string sql = "INSERT INTO Images(ImageName,ImageData)VALUES("+ textBoxName.Text + ",@img)";
                if(conn.State != ConnectionState.Open)
                    conn.Open();
                command = new SqlCommand(sql, conn);
                command.Parameters.Add(new SqlParameter("@img", img));
                int x = command.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show(x.ToString() + " records saved");
                textBoxName.Text = "";
                pictureBox.Image = null;
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonShow_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = "SELECT ImageName,ImageData FROM Images WHERE ImageName=" + textBoxName.Text;
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                command = new SqlCommand(sql, conn);
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                if (reader.HasRows)
                {
                    textBoxName.Text = reader[0].ToString();
                    byte[] img = (byte[])(reader[1]);
                    if(img == null)
                    {
                        pictureBox.Image = null;
                    }
                    else
                    {
                        MemoryStream ms = new MemoryStream(img);
                        pictureBox.Image = Image.FromStream(ms);
                    }
                }
                else
                {
                    textBoxName.Text = "";
                    pictureBox.Image = null;
                    MessageBox.Show("Image does not exist");
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonShowAll_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            if (conn.State != ConnectionState.Open)
                conn.Open();
            SqlCommand cmd = new SqlCommand("GetPictures", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                int count = 0;
                List<PictureBox> boxList = new List<PictureBox>();
                boxList.Add(pictureBox1);
                boxList.Add(pictureBox2);
                boxList.Add(pictureBox3);
                boxList.Add(pictureBox4);
                boxList.Add(pictureBox5);
                boxList.Add(pictureBox6);
                boxList.Add(pictureBox7);
                boxList.Add(pictureBox8);
                boxList.Add(pictureBox9);
                boxList.Add(pictureBox10);

                while (rdr.Read())
                {
                    byte[] img = (byte[])(rdr["ImageData"]);
                    if (img == null)
                    {
                    }
                    else
                    {
                        MemoryStream ms = new MemoryStream(img);
                        boxList[count].Image = Image.FromStream(ms);
                        count++;
                    }
                }
            }
            conn.Close();
            textBoxAll.Text = sb.ToString();
        }
    }
}
