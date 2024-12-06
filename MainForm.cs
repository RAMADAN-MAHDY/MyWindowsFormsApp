using System;
using System.Data;
using Microsoft.Data.SqlClient; // تأكد من استخدام Microsoft.Data.SqlClient
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using System.IO;

public partial class MainForm : Form
{
    private SqlConnection sql_con;
    private SqlDataAdapter dataAdapter;
    private DataTable dataTable;
    private IConfiguration configuration;

    public MainForm()
    {
        InitializeComponent();
        sql_con = null!;
        dataAdapter = null!;
        dataTable = null!;
        configuration = null!;
        LoadConfiguration();
        SetConnection();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();
        // 
        // MainForm
        // 
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Name = "MainForm";
        this.ResumeLayout(false);
    }

    private void LoadConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        configuration = builder.Build();
    }

    private void SetConnection()
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection");
        sql_con = new SqlConnection(connectionString);
    }

    private void ExecuteQuery(string query)
    {
        if (sql_con.State == ConnectionState.Closed)
            sql_con.Open();
        SqlCommand sql_cmd = new SqlCommand(query, sql_con);
        sql_cmd.ExecuteNonQuery();
        sql_con.Close();
    }

    private DataTable LoadData(string query)
    {
        if (sql_con.State == ConnectionState.Closed)
            sql_con.Open();
        dataAdapter = new SqlDataAdapter(query, sql_con);
        dataTable = new DataTable();
        dataAdapter.Fill(dataTable);
        sql_con.Close();
        return dataTable;
    }
}
