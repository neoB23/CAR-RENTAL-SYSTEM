using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;


namespace CAR_RENTAL_SYSTEM
{
    public partial class Form6 : Form
    {
        MySqlConnection con = new MySqlConnection("server=localhost;user id=root;database=final system database;sslMode=none");
        MySqlCommand cmd;
        MySqlDataAdapter adapt;

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );
        public Form6()
        {
            InitializeComponent();
            DisplayData();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 30, 30));
            this.SetStyle(ControlStyles.ResizeRedraw, true); // this is to avoid visual artifacts

        }
        private const int cGrip = 16;
        private const int cCaption = 32;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84)
            {
                Point pos = new Point(m.LParam.ToInt32());
                pos = this.PointToClient(pos);
                if (pos.Y < cCaption)
                {
                    m.Result = (IntPtr)2;
                    return;
                }

                if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                {
                    m.Result = (IntPtr)17;
                    return;
                }
            }
            base.WndProc(ref m);
        }


        private void Form6_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form fr4 = new Form4();
            this.Hide();
            fr4.Show();
        }

        private void guna2GradientButton1_Click(object sender, EventArgs e)
        {


            // Check if all required fields are filled
            if (txtName.Text != "" && txtContactNumber.Text != "" && txtAge.Text != "" && cmbSex.SelectedItem != null)
            {
                // Create a command to update the user information
                string sql = "UPDATE rent SET `contact number`=@contactnum, Age=@age, Sex=@sex WHERE Name=@name";
                cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@contactnum", txtContactNumber.Text);
                cmd.Parameters.AddWithValue("@age", txtAge.Text);
                cmd.Parameters.AddWithValue("@sex", cmbSex.SelectedItem.ToString());
                cmd.Parameters.AddWithValue("@name", txtName.Text);

                // Open the connection and execute the command
                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();

                // Check if any row was affected
                if (rowsAffected > 0)
                {
                    MessageBox.Show("User information updated successfully!", "UPDATE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DisplayData();
                    ClearData();
                }
                else
                {
                    MessageBox.Show("No user found with this name!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Fill out all the information needed", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Check if the user name is entered
            if (txtName.Text != "")
            {
                // Create a command to delete the user
                string sql = "DELETE FROM rent WHERE Name=@name";
                cmd = new MySqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@name", txtName.Text);

                // Open the connection and execute the command
                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();

                // Check if any row was affected
                if (rowsAffected > 0)
                {
                    MessageBox.Show("User deleted successfully!", "DELETE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DisplayData();
                    ClearData();
                }
                else
                {
                    MessageBox.Show("No user found with this name!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Enter the name of the user you want to delete", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            // Checks if Username Exists
            MySqlCommand cmd1 = new MySqlCommand("SELECT * FROM rent WHERE name = @name", con);
            cmd1.Parameters.AddWithValue("@name", txtName.Text);
            con.Open();
            bool userExists = false;
            using (var dr1 = cmd1.ExecuteReader())
                if (userExists = dr1.HasRows)
                    MessageBox.Show("Username not available!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            con.Close();
            if (!(userExists))
            {
                // Adds a User in the Database
                if (txtName.Text != "" && txtContactNumber.Text != "" && txtAge.Text != "" && cmbSex.SelectedItem != null)
                {
                    cmd = new MySqlCommand("insert into rent(Name, `contact number`, Age, Sex) values(@name, @contactnum, @age, @sex)", con);
                    con.Open();
                    cmd.Parameters.AddWithValue("@name", txtName.Text);
                    cmd.Parameters.AddWithValue("@contactnum", txtContactNumber.Text);
                    cmd.Parameters.AddWithValue("@age", txtAge.Text);
                    cmd.Parameters.AddWithValue("@sex", cmbSex.SelectedItem.ToString());
                    cmd.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("You have successfully rented a car", "INSERT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DisplayData();
                    ClearData();
                    
                }
                else
                {
                    MessageBox.Show("Fill out all the information needed", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            txtName.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            txtContactNumber.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtAge.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            cmbSex.SelectedItem = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
           
        }
        private void DisplayData()
        {
            string sql = "SELECT Name, `contact number`, Age, Sex FROM rent";
            cmd = new MySqlCommand(sql, con);
            adapt = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapt.Fill(dt);
            dataGridView1.DataSource = dt;
        }
        // Clears the Data  
        private void ClearData()
        {
            txtName.Text = "";
            txtAge.Text = "";
            txtContactNumber.Text = "";
            cmbSex.SelectedItem = null;
            

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void cmbSex_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
    
}
