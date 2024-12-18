using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _045_Sorita_Telan_F2
{
    public partial class frmPirates : Form
    {
        string connStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\User\\Documents\\045_Telan_Sorita_F2\\dpPirates1.accdb";
        OleDbConnection conn;
        bool is_new_record;
        

        public frmPirates()
        {
            InitializeComponent();
        }

        int ID;

        private void btnSearch_Click(object sender, EventArgs e)
        {

            string query = "SELECT ID as ID, piratename AS ALIAS, givenname AS NAME, age, " +
                           "pirategroup AS PIRATEGROUP, bounty AS BOUNTY " +
                           "FROM pirates " +
                           "WHERE (piratename LIKE @keyword OR givenname LIKE @keyword1) " +
                           "AND pirategroup = @group";


            OleDbConnection conn = new OleDbConnection(connStr);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("@keyword", "%" + txtKeyword.Text + "%");
            cmd.Parameters.AddWithValue("@keyword1", "%" + txtKeyword.Text + "%");
            cmd.Parameters.AddWithValue("@group", cboPirateGroup.Text);

            
            DataTable dt = new DataTable();
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
            adapter.Fill(dt);

            conn.Close();
            grdView.DataSource = dt;

        }

        private void btnViewDetails_Click(object sender, EventArgs e)
        {
            is_new_record = false;

            btnSave.Enabled = true;
            btnNewRecord.Enabled = false;
            txtAlias.Enabled = true;
            txtName.Enabled = true;
            txtAge.Enabled = true;
            cboPirateInfoGroup.Enabled = true;
            txtBounty.Enabled = true;
            DataTable dt = new DataTable();
            string query = "Select ID, piratename as ALIAS, givenname as NAME, age as AGE, pirategroup as PIRATEGROUP, bounty as BOUNTY from pirates where ID = @id";
            conn = new OleDbConnection(connStr);
            conn.Open();

            OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", ID);

            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
            adapter.Fill(dt);
            conn.Close();

            grdView.DataSource = dt;
            
            
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string query = "Delete from pirates where piratename = @alias";
            conn = new OleDbConnection(connStr);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("@alias", txtAlias.Text);
            int rows_affected = cmd.ExecuteNonQuery();
            conn.Close();

            if (rows_affected > 0)
            {
                MessageBox.Show("Deleted");
            }
            else
            {
                MessageBox.Show("Di na delete");
            }

            txtAlias.Text = "";
            txtName.Text = "";
            txtAge.Text = "";
            cboPirateInfoGroup.Text = "";
            txtBounty.Text = "";
            reload();
        }

        private void grdView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            ID = int.Parse(grdView.SelectedCells[0].Value.ToString());
            txtAlias.Text = grdView.SelectedCells[1].Value.ToString();
            txtName.Text = grdView.SelectedCells[2].Value.ToString();
            txtAge.Text = grdView.SelectedCells[3].Value.ToString();
            txtBounty.Text = grdView.SelectedCells[4].Value.ToString();
            cboPirateGroup.Text = grdView.SelectedCells[5].Value.ToString();
        }
        public void reload()
        {
            DataTable dt = new DataTable();
            string query = "Select ID as ID, piratename as ALIAS, givenname as NAME, age as AGE, bounty as BOUNTY, pirategroup as PIRATEGROUP, tax as TAX from pirates";

            conn = new OleDbConnection(connStr);
            conn.Open();
           
            OleDbDataAdapter adapter = new OleDbDataAdapter(query, conn);
            adapter.Fill(dt);
            conn.Close();

            grdView.DataSource = dt;

            grdView.Columns["age"].Visible = false;
            grdView.Columns["ID"].Visible = false;

        }
        private void SelectDistinct()
        {
            DataTable dt = new DataTable();
            string query = "select distinct pirategroup from pirates";
            conn = new OleDbConnection(connStr);
            conn.Open();
            OleDbDataAdapter adapter = new OleDbDataAdapter(query, conn);
            adapter.Fill(dt);

            
            cboPirateGroup.DataSource = dt;
            cboPirateGroup.DisplayMember = "pirategroup";
            cboPirateInfoGroup.DataSource = dt;
            cboPirateInfoGroup.DisplayMember = "pirategroup";
            conn.Close();
        }

        public void TextBoxEnable()
        {
            txtAge.Enabled = true;
            txtAlias.Enabled = true;
            txtBounty.Enabled = true;
            cboPirateInfoGroup.Enabled = true;
            txtBounty.Enabled = true;
            txtName.Enabled = true;
            btnNewRecord.Enabled = true;
        }
        public void TextBoxDisable()
        {
            txtAlias.Enabled = false;
            txtAge.Enabled = false;
            txtBounty.Enabled = false;
            cboPirateInfoGroup.Enabled = false;
            txtBounty.Enabled = false;
            txtName.Enabled = false;
            btnSave.Enabled = true;
        }

        private void frmPirates_Load(object sender, EventArgs e)
        {
            SelectDistinct();

            reload();

            TextBoxDisable();
            btnSave.Enabled = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (is_new_record == true)
            {

                double bounty = double.Parse(txtBounty.Text);

                double percentage = .05; // 0.05%
                double total_bounty = bounty - (bounty * percentage);

                overall_Price.Text = total_bounty.ToString();

                string query = "INSERT into [pirates] (piratename, givenname, age, bounty, pirategroup, tax) values(@alias, @name, @age, @bounty, @pirategroup, @tax)";
                conn = new OleDbConnection(connStr);
                conn.Open();
                OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@alias", txtAlias.Text);
                cmd.Parameters.AddWithValue("@name", txtName.Text);
                cmd.Parameters.AddWithValue("@age", txtAge.Text);
                cmd.Parameters.AddWithValue("@bounty", txtBounty.Text);
                cmd.Parameters.AddWithValue("@pirategroup", cboPirateInfoGroup.Text); 
                cmd.Parameters.AddWithValue("@tax", total_bounty);

                MessageBox.Show("Added");
                cmd.ExecuteNonQuery();
                conn.Close();

                reload();
            }

            else
            {

                string query = "Update [pirates] set piratename = @alias, givenname = @name, age = @age, bounty = @bounty, pirategroup = @pirategroup where ID = @id";
                conn = new OleDbConnection(connStr);
                conn.Open();
                OleDbCommand cmd = new OleDbCommand(query, conn);
                cmd.Parameters.AddWithValue("@alias", txtAlias.Text);
                cmd.Parameters.AddWithValue("@name", txtName.Text);
                cmd.Parameters.AddWithValue("@age", int.Parse(txtAge.Text));
                cmd.Parameters.AddWithValue("@bounty", double.Parse(txtBounty.Text));
                cmd.Parameters.AddWithValue("@pirategroup", cboPirateInfoGroup.Text);
                cmd.Parameters.AddWithValue("@id", grdView.SelectedCells[0].Value.ToString());
                cmd.ExecuteNonQuery();
                conn.Close();

                MessageBox.Show("Updated");
            

                reload();
            }

            TextBoxDisable();
            btnSave.Enabled = false;
            btnNewRecord.Enabled = true;
        }

        private void btnNewRecord_Click_1(object sender, EventArgs e)
        {
            is_new_record = true;
            //overall_Price.txt
            txtAge.Clear();
            txtName.Clear();
            txtAlias.Clear();
            txtBounty.Clear();
            cboPirateInfoGroup.SelectedIndex = -1;
            btnSave.Enabled = true;

            TextBoxEnable();
        }

        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            txtAge.Clear();
            cboPirateInfoGroup.SelectedIndex = -1;
            txtAge.Clear();
            txtName.Clear();
            txtBounty.Clear();

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
